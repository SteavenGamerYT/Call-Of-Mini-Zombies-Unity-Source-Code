using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Sfs2X.Bitswarm;
using Sfs2X.FSM;
using Sfs2X.Logging;

namespace Sfs2X.Core.Sockets
{
	public class TCPSocketLayer : ISocketLayer
	{
		public enum States
		{
			Disconnected = 0,
			Connecting = 1,
			Connected = 2
		}

		public enum Transitions
		{
			StartConnect = 0,
			ConnectionSuccess = 1,
			ConnectionFailure = 2,
			Disconnect = 3
		}

		private static readonly int READ_BUFFER_SIZE = 4096;

		private Logger log;

		private FiniteStateMachine fsm;

		private bool isDisconnecting = false;

		private int socketPollSleep;

		private Thread thrConnect;

		private int socketNumber;

		private IPAddress ipAddress;

		private TcpClient connection;

		private NetworkStream networkStream;

		private Thread thrSocketReader;

		private byte[] byteBuffer = new byte[READ_BUFFER_SIZE];

		private OnDataDelegate onData = null;

		private OnErrorDelegate onError = null;

		private ConnectionDelegate onConnect;

		private ConnectionDelegate onDisconnect;

		public States State
		{
			get
			{
				return (States)fsm.GetCurrentState();
			}
		}

		public bool IsConnected
		{
			get
			{
				return State == States.Connected;
			}
		}

		public OnDataDelegate OnData
		{
			get
			{
				return onData;
			}
			set
			{
				onData = value;
			}
		}

		public OnErrorDelegate OnError
		{
			get
			{
				return onError;
			}
			set
			{
				onError = value;
			}
		}

		public ConnectionDelegate OnConnect
		{
			get
			{
				return onConnect;
			}
			set
			{
				onConnect = value;
			}
		}

		public ConnectionDelegate OnDisconnect
		{
			get
			{
				return onDisconnect;
			}
			set
			{
				onDisconnect = value;
			}
		}

		public TCPSocketLayer(BitSwarmClient bs)
		{
			if (bs != null)
			{
				log = bs.Log;
			}
			InitStates();
		}

		private void InitStates()
		{
			fsm = new FiniteStateMachine();
			fsm.AddAllStates(typeof(States));
			fsm.AddStateTransition(States.Disconnected, States.Connecting, Transitions.StartConnect);
			fsm.AddStateTransition(States.Connecting, States.Connected, Transitions.ConnectionSuccess);
			fsm.AddStateTransition(States.Connecting, States.Disconnected, Transitions.ConnectionFailure);
			fsm.AddStateTransition(States.Connected, States.Disconnected, Transitions.Disconnect);
			fsm.SetCurrentState(States.Disconnected);
		}

		private void LogWarn(string msg)
		{
			if (log != null)
			{
				log.Warn("TCPSocketLayer: " + msg);
			}
		}

		private void LogError(string msg)
		{
			if (log != null)
			{
				log.Error("TCPSocketLayer: " + msg);
			}
		}

		private void ConnectThread()
		{
			try
			{
				connection = new TcpClient();
				connection.Client.Connect(ipAddress, socketNumber);
				networkStream = connection.GetStream();
				fsm.ApplyTransition(Transitions.ConnectionSuccess);
				CallOnConnect();
				thrSocketReader = new Thread(Read);
				thrSocketReader.Start();
			}
			catch (SocketException ex)
			{
				string err = "Connection error: " + ex.Message + " " + ex.StackTrace;
				HandleError(err, ex.SocketErrorCode);
			}
			catch (Exception ex2)
			{
				string err2 = "General exception on connection: " + ex2.Message + " " + ex2.StackTrace;
				HandleError(err2);
			}
		}

		private void HandleError(string err, SocketError se)
		{
			fsm.ApplyTransition(Transitions.ConnectionFailure);
			if (!isDisconnecting)
			{
				LogError(err);
				CallOnError(err, se);
			}
			HandleDisconnection();
		}

		private void HandleError(string err)
		{
			HandleError(err, SocketError.NotSocket);
		}

		private void HandleDisconnection()
		{
			if (State == States.Disconnected)
			{
				return;
			}
			try
			{
				if (thrSocketReader.IsAlive)
				{
					thrSocketReader.Abort();
				}
			}
			catch
			{
				LogWarn("Exception trying to abort socket reader thread");
			}
			fsm.ApplyTransition(Transitions.Disconnect);
			CallOnDisconnect();
		}

		private void WriteSocket(byte[] buf)
		{
			if (State != States.Connected)
			{
				LogError("Trying to write to disconnected socket");
				return;
			}
			try
			{
				networkStream.Write(buf, 0, buf.Length);
			}
			catch (SocketException ex)
			{
				string err = "Error writing to socket: " + ex.Message;
				HandleError(err, ex.SocketErrorCode);
			}
			catch (Exception ex2)
			{
				string err2 = "General error writing to socket: " + ex2.Message + " " + ex2.StackTrace;
				HandleError(err2);
			}
		}

		private void Read()
		{
			//Discarded unreachable code: IL_0093
			int num = 0;
			while (true)
			{
				try
				{
					if (State != States.Connected)
					{
						break;
					}
					if (socketPollSleep > 0)
					{
						Thread.Sleep(socketPollSleep);
					}
					num = networkStream.Read(byteBuffer, 0, READ_BUFFER_SIZE);
					if (num < 1)
					{
						HandleError("Connection closed by the remote side");
						break;
					}
					HandleBinaryData(byteBuffer, num);
				}
				catch (SocketException ex)
				{
					HandleError("Error reading data from socket: " + ex.Message, ex.SocketErrorCode);
				}
				catch (ThreadAbortException)
				{
					break;
				}
				catch (Exception ex3)
				{
					HandleError("General error reading data from socket: " + ex3.Message + " " + ex3.StackTrace);
				}
			}
		}

		private void HandleBinaryData(byte[] buf, int size)
		{
			byte[] array = new byte[size];
			Buffer.BlockCopy(buf, 0, array, 0, size);
			CallOnData(array);
		}

		public void Connect(IPAddress adr, int port)
		{
			if (State != 0)
			{
				LogWarn("Calling connect when the socket is not disconnected");
				return;
			}
			socketNumber = port;
			ipAddress = adr;
			fsm.ApplyTransition(Transitions.StartConnect);
			thrConnect = new Thread(ConnectThread);
			thrConnect.Start();
		}

		public void Disconnect()
		{
			if (State != States.Connected)
			{
				LogWarn("Calling disconnect when the socket is not connected");
				return;
			}
			isDisconnecting = true;
			try
			{
				connection.Client.Shutdown(SocketShutdown.Both);
			}
			catch (Exception)
			{
				LogWarn("Trying to disconnect a non-connected tcp socket");
			}
			HandleDisconnection();
			isDisconnecting = false;
		}

		private void CallOnData(byte[] data)
		{
			if (onData != null)
			{
				onData(data);
			}
		}

		private void CallOnError(string msg, SocketError se)
		{
			if (onError != null)
			{
				onError(msg, se);
			}
		}

		private void CallOnConnect()
		{
			if (onConnect != null)
			{
				onConnect();
			}
		}

		private void CallOnDisconnect()
		{
			if (onDisconnect != null)
			{
				onDisconnect();
			}
		}

		public void Write(byte[] data)
		{
			WriteSocket(data);
		}
	}
}
