using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Timers;
using Sfs2X.Bitswarm.BBox;
using Sfs2X.Controllers;
using Sfs2X.Core;
using Sfs2X.Core.Sockets;
using Sfs2X.Exceptions;
using Sfs2X.Logging;
using Sfs2X.Util;

namespace Sfs2X.Bitswarm
{
	public class BitSwarmClient : IDispatchable
	{
		private ISocketLayer socket = null;

		private IoHandler ioHandler;

		private Dictionary<int, IController> controllers = new Dictionary<int, IController>();

		private int compressionThreshold = 2000000;

		private int maxMessageSize = 10000;

		private SmartFox sfs;

		private string lastIpAddress;

		private int lastTcpPort;

		private int reconnectionSeconds = 0;

		private bool attemptingReconnection = false;

		private Logger log;

		private SystemController sysController;

		private ExtensionController extController;

		private IUDPManager udpManager;

		private bool controllersInited = false;

		private EventDispatcher dispatcher;

		private BBClient bbClient;

		private bool useBlueBox = false;

		private bool bbConnected = false;

		private string connectionMode;

		private bool manualDisconnection = false;

		private Timer retryTimer = null;

		public string ConnectionMode
		{
			get
			{
				return connectionMode;
			}
		}

		public bool UseBlueBox
		{
			get
			{
				return useBlueBox;
			}
		}

		public bool Debug
		{
			get
			{
				if (sfs == null)
				{
					return true;
				}
				return sfs.Debug;
			}
		}

		public SmartFox Sfs
		{
			get
			{
				return sfs;
			}
		}

		public bool Connected
		{
			get
			{
				if (useBlueBox)
				{
					return bbConnected;
				}
				if (socket == null)
				{
					return false;
				}
				return socket.IsConnected;
			}
		}

		public IoHandler IoHandler
		{
			get
			{
				return ioHandler;
			}
			set
			{
				if (ioHandler != null)
				{
					throw new SFSError("IOHandler is already set!");
				}
				ioHandler = value;
			}
		}

		public int CompressionThreshold
		{
			get
			{
				return compressionThreshold;
			}
			set
			{
				if (value > 100)
				{
					compressionThreshold = value;
					return;
				}
				throw new ArgumentException("Compression threshold cannot be < 100 bytes.");
			}
		}

		public int MaxMessageSize
		{
			get
			{
				return maxMessageSize;
			}
			set
			{
				maxMessageSize = value;
			}
		}

		public ISocketLayer Socket
		{
			get
			{
				return socket;
			}
		}

		public BBClient HttpClient
		{
			get
			{
				return bbClient;
			}
		}

		public bool IsReconnecting
		{
			get
			{
				return attemptingReconnection;
			}
			set
			{
				attemptingReconnection = value;
			}
		}

		public int ReconnectionSeconds
		{
			get
			{
				if (reconnectionSeconds < 0)
				{
					return 0;
				}
				return reconnectionSeconds;
			}
			set
			{
				reconnectionSeconds = value;
			}
		}

		public EventDispatcher Dispatcher
		{
			get
			{
				return dispatcher;
			}
		}

		public Logger Log
		{
			get
			{
				if (sfs == null)
				{
					return new Logger(null);
				}
				return sfs.Log;
			}
		}

		public IUDPManager UdpManager
		{
			get
			{
				return udpManager;
			}
			set
			{
				udpManager = value;
			}
		}

		public BitSwarmClient(SmartFox sfs)
		{
			this.sfs = sfs;
			log = sfs.Log;
		}

		public void ForceBlueBox(bool val)
		{
			if (!bbConnected)
			{
				useBlueBox = val;
				return;
			}
			throw new Exception("You can't change the BlueBox mode while the connection is running");
		}

		public void Init()
		{
			if (dispatcher == null)
			{
				dispatcher = new EventDispatcher(this);
			}
			if (!controllersInited)
			{
				InitControllers();
				controllersInited = true;
			}
			if (socket == null)
			{
				socket = new TCPSocketLayer(this);
				ISocketLayer socketLayer = socket;
				socketLayer.OnConnect = (ConnectionDelegate)Delegate.Combine(socketLayer.OnConnect, new ConnectionDelegate(OnSocketConnect));
				ISocketLayer socketLayer2 = socket;
				socketLayer2.OnDisconnect = (ConnectionDelegate)Delegate.Combine(socketLayer2.OnDisconnect, new ConnectionDelegate(OnSocketClose));
				ISocketLayer socketLayer3 = socket;
				socketLayer3.OnData = (OnDataDelegate)Delegate.Combine(socketLayer3.OnData, new OnDataDelegate(OnSocketData));
				ISocketLayer socketLayer4 = socket;
				socketLayer4.OnError = (OnErrorDelegate)Delegate.Combine(socketLayer4.OnError, new OnErrorDelegate(OnSocketError));
				bbClient = new BBClient(this);
				bbClient.AddEventListener(BBEvent.CONNECT, OnBBConnect);
				bbClient.AddEventListener(BBEvent.DATA, OnBBData);
				bbClient.AddEventListener(BBEvent.DISCONNECT, OnBBDisconnect);
				bbClient.AddEventListener(BBEvent.IO_ERROR, OnBBError);
				bbClient.AddEventListener(BBEvent.SECURITY_ERROR, OnBBError);
			}
		}

		public IController GetController(int id)
		{
			return controllers[id];
		}

		private void AddController(int id, IController controller)
		{
			if (controller == null)
			{
				throw new ArgumentException("Controller is null, it can't be added.");
			}
			if (controllers.ContainsKey(id))
			{
				throw new ArgumentException("A controller with id: " + id + " already exists! Controller can't be added: " + controller);
			}
			controllers[id] = controller;
		}

		public void Connect(string ip, int port)
		{
			lastIpAddress = ip;
			lastTcpPort = port;
			if (useBlueBox)
			{
				bbClient.Connect(ip, port);
				connectionMode = ConnectionModes.HTTP;
			}
			else
			{
				socket.Connect(IPAddress.Parse(lastIpAddress), lastTcpPort);
				connectionMode = ConnectionModes.SOCKET;
			}
		}

		public void Send(IMessage message)
		{
			ioHandler.Codec.OnPacketWrite(message);
		}

		public void Disconnect(string reason)
		{
			if (reason == ClientDisconnectionReason.MANUAL)
			{
				manualDisconnection = true;
			}
			if (useBlueBox)
			{
				bbClient.Disconnect();
				return;
			}
			socket.Disconnect();
			if (udpManager != null)
			{
				udpManager.Disconnect();
			}
		}

		private void InitControllers()
		{
			sysController = new SystemController(this);
			extController = new ExtensionController(this);
			AddController(0, sysController);
			AddController(1, extController);
		}

		private void OnSocketConnect()
		{
			BitSwarmEvent bitSwarmEvent = new BitSwarmEvent(BitSwarmEvent.CONNECT);
			Hashtable hashtable = new Hashtable();
			hashtable["success"] = true;
			hashtable["isReconnection"] = attemptingReconnection;
			bitSwarmEvent.Params = hashtable;
			DispatchEvent(bitSwarmEvent);
		}

		private void OnSocketClose()
		{
			bool flag = sfs == null || (!attemptingReconnection && sfs.GetReconnectionSeconds() == 0);
			bool flag2 = manualDisconnection;
			if (attemptingReconnection || flag || flag2)
			{
				if (udpManager != null)
				{
					udpManager.Reset();
				}
				if (flag2)
				{
					Hashtable hashtable = new Hashtable();
					hashtable["reason"] = ClientDisconnectionReason.MANUAL;
					DispatchEvent(new BitSwarmEvent(BitSwarmEvent.DISCONNECT, hashtable));
				}
			}
			else
			{
				log.Debug("Attempting reconnection in " + ReconnectionSeconds + " sec");
				attemptingReconnection = true;
				DispatchEvent(new BitSwarmEvent(BitSwarmEvent.RECONNECTION_TRY));
				RetryConnection(ReconnectionSeconds * 1000);
			}
		}

		private void RetryConnection(int timeout)
		{
			if (retryTimer == null)
			{
				retryTimer = new Timer(timeout);
			}
			retryTimer.AutoReset = false;
			retryTimer.Elapsed += OnRetryConnectionEvent;
			retryTimer.Enabled = true;
		}

		private void OnRetryConnectionEvent(object source, ElapsedEventArgs e)
		{
			retryTimer.Enabled = false;
			socket.Connect(IPAddress.Parse(lastIpAddress), lastTcpPort);
		}

		private void DispatchEvent(BitSwarmEvent evt)
		{
			dispatcher.DispatchEvent(evt);
		}

		private void OnSocketData(byte[] data)
		{
			try
			{
				ByteArray buffer = new ByteArray(data);
				ioHandler.OnDataRead(buffer);
			}
			catch (Exception ex)
			{
				log.Error("## SocketDataError: " + ex.Message);
				BitSwarmEvent bitSwarmEvent = new BitSwarmEvent(BitSwarmEvent.DATA_ERROR);
				Hashtable hashtable = new Hashtable();
				hashtable["message"] = ex.ToString();
				bitSwarmEvent.Params = hashtable;
				DispatchEvent(bitSwarmEvent);
			}
		}

		private void OnSocketError(string message, SocketError se)
		{
			BitSwarmEvent bitSwarmEvent = null;
			Hashtable hashtable = new Hashtable();
			hashtable["message"] = message + ((se == SocketError.NotSocket) ? "" : (" [" + se.ToString() + "]"));
			if (se != SocketError.AccessDenied)
			{
				if (!attemptingReconnection && !sfs.IsConnecting && !manualDisconnection)
				{
					Hashtable hashtable2 = new Hashtable();
					hashtable2["reason"] = ClientDisconnectionReason.UNKNOWN;
					DispatchEvent(new BitSwarmEvent(BitSwarmEvent.DISCONNECT, hashtable2));
				}
				bitSwarmEvent = new BitSwarmEvent(BitSwarmEvent.IO_ERROR);
				bitSwarmEvent.Params = hashtable;
			}
			else
			{
				bitSwarmEvent = new BitSwarmEvent(BitSwarmEvent.SECURITY_ERROR);
				bitSwarmEvent.Params = hashtable;
			}
			manualDisconnection = false;
			DispatchEvent(bitSwarmEvent);
		}

		public long NextUdpPacketId()
		{
			return udpManager.NextUdpPacketId;
		}

		public void AddEventListener(string eventType, EventListenerDelegate listener)
		{
			dispatcher.AddEventListener(eventType, listener);
		}

		private void OnBBConnect(BaseEvent e)
		{
			bbConnected = true;
			BitSwarmEvent bitSwarmEvent = new BitSwarmEvent(BitSwarmEvent.CONNECT);
			bitSwarmEvent.Params = new Hashtable();
			bitSwarmEvent.Params["success"] = true;
			bitSwarmEvent.Params["isReconnection"] = false;
			DispatchEvent(bitSwarmEvent);
		}

		private void OnBBData(BaseEvent e)
		{
			BBEvent bBEvent = e as BBEvent;
			ByteArray buffer = (ByteArray)bBEvent.Params["data"];
			ioHandler.OnDataRead(buffer);
		}

		private void OnBBDisconnect(BaseEvent e)
		{
			bbConnected = false;
			Hashtable hashtable = new Hashtable();
			hashtable["reason"] = ClientDisconnectionReason.UNKNOWN;
			DispatchEvent(new BitSwarmEvent(BitSwarmEvent.DISCONNECT, hashtable));
		}

		private void OnBBError(BaseEvent e)
		{
			BBEvent bBEvent = e as BBEvent;
			log.Error("## BlueBox Error: " + (string)bBEvent.Params["message"]);
			BitSwarmEvent bitSwarmEvent = new BitSwarmEvent(BitSwarmEvent.IO_ERROR);
			bitSwarmEvent.Params = new Hashtable();
			bitSwarmEvent.Params["message"] = bBEvent.Params["message"];
			DispatchEvent(bitSwarmEvent);
		}
	}
}
