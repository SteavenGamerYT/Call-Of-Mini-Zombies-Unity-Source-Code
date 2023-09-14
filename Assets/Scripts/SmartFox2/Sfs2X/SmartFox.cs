using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Timers;
using Sfs2X.Bitswarm;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Managers;
using Sfs2X.Exceptions;
using Sfs2X.Logging;
using Sfs2X.Requests;
using Sfs2X.Util;

namespace Sfs2X
{
	public class SmartFox : IDispatchable
	{
		private const int DEFAULT_HTTP_PORT = 8080;

		private const int MAX_BB_CONNECT_ATTEMPTS = 3;

		private int majVersion = 1;

		private int minVersion = 0;

		private int subVersion = 1;

		private BitSwarmClient bitSwarm;

		private LagMonitor lagMonitor;

		private bool useBlueBox = true;

		private bool isJoining = false;

		private User mySelf;

		private string sessionToken;

		private Room lastJoinedRoom;

		private Logger log;

		private bool inited = false;

		private bool debug = false;

		private bool threadSafeMode = true;

		private bool isConnecting = false;

		private IUserManager userManager;

		private IRoomManager roomManager;

		private IBuddyManager buddyManager;

		private ConfigData config;

		private string currentZone;

		private bool autoConnectOnConfig = false;

		private string lastIpAddress;

		private EventDispatcher dispatcher;

		private object eventsLocker = new object();

		private Queue<BaseEvent> eventsQueue = new Queue<BaseEvent>();

		private int bbConnectionAttempt = 0;

		private Timer disconnectTimer = null;

		public Logger Log
		{
			get
			{
				return log;
			}
		}

		public bool IsConnecting
		{
			get
			{
				return isConnecting;
			}
		}

		public LagMonitor LagMonitor
		{
			get
			{
				return lagMonitor;
			}
		}

		public bool IsConnected
		{
			get
			{
				bool result = false;
				if (bitSwarm != null)
				{
					result = bitSwarm.Connected;
				}
				return result;
			}
		}

		public string Version
		{
			get
			{
				return "" + majVersion + "." + minVersion + "." + subVersion;
			}
		}

		public ConfigData Config
		{
			get
			{
				return config;
			}
		}

		public bool Debug
		{
			get
			{
				return debug;
			}
		}

		public User MySelf
		{
			get
			{
				return mySelf;
			}
			set
			{
				mySelf = value;
			}
		}

		public Logger Logger
		{
			get
			{
				return log;
			}
		}

		public Room LastJoinedRoom
		{
			get
			{
				return lastJoinedRoom;
			}
			set
			{
				lastJoinedRoom = value;
			}
		}

		public List<Room> JoinedRooms
		{
			get
			{
				return roomManager.GetJoinedRooms();
			}
		}

		public IRoomManager RoomManager
		{
			get
			{
				return roomManager;
			}
		}

		public IUserManager UserManager
		{
			get
			{
				return userManager;
			}
		}

		public IBuddyManager BuddyManager
		{
			get
			{
				return buddyManager;
			}
		}

		public bool IsJoining
		{
			set
			{
				isJoining = value;
			}
		}

		public string SessionToken
		{
			get
			{
				return sessionToken;
			}
		}

		public EventDispatcher Dispatcher
		{
			get
			{
				return dispatcher;
			}
		}

		public SmartFox(bool debug)
		{
			log = new Logger(this);
			log.EnableEventDispatching = true;
			if (debug)
			{
				log.LoggingLevel = LogLevel.DEBUG;
			}
			this.debug = debug;
			Initialize();
		}

		private void Initialize()
		{
			if (!inited)
			{
				if (dispatcher == null)
				{
					dispatcher = new EventDispatcher(this);
				}
				bitSwarm = new BitSwarmClient(this);
				bitSwarm.IoHandler = new SFSIOHandler(bitSwarm);
				bitSwarm.Init();
				bitSwarm.Dispatcher.AddEventListener(BitSwarmEvent.CONNECT, OnSocketConnect);
				bitSwarm.Dispatcher.AddEventListener(BitSwarmEvent.DISCONNECT, OnSocketClose);
				bitSwarm.Dispatcher.AddEventListener(BitSwarmEvent.RECONNECTION_TRY, OnSocketReconnectionTry);
				bitSwarm.Dispatcher.AddEventListener(BitSwarmEvent.IO_ERROR, OnSocketIOError);
				bitSwarm.Dispatcher.AddEventListener(BitSwarmEvent.SECURITY_ERROR, OnSocketSecurityError);
				bitSwarm.Dispatcher.AddEventListener(BitSwarmEvent.DATA_ERROR, OnSocketDataError);
				inited = true;
				Reset();
			}
		}

		private void Reset()
		{
			bbConnectionAttempt = 0;
			userManager = new SFSGlobalUserManager(this);
			roomManager = new SFSRoomManager(this);
			buddyManager = new SFSBuddyManager(this);
			if (lagMonitor != null)
			{
				lagMonitor.Destroy();
			}
			isJoining = false;
			currentZone = null;
			lastJoinedRoom = null;
			sessionToken = null;
			mySelf = null;
		}

		public BitSwarmClient GetSocketEngine()
		{
			return bitSwarm;
		}

		public List<Room> GetRoomListFromGroup(string groupId)
		{
			return roomManager.GetRoomListFromGroup(groupId);
		}

		public void Connect(string host, int port)
		{
			//Discarded unreachable code: IL_0141
			if (IsConnected)
			{
				log.Warn("Already connected");
				return;
			}
			if (isConnecting)
			{
				log.Warn("A connection attempt is already in progress");
				return;
			}
			if (config != null)
			{
				if (host == null)
				{
					host = config.Host;
				}
				if (port == -1)
				{
					port = config.Port;
				}
			}
			if (host == null || host.Length == 0)
			{
				throw new ArgumentException("Invalid connection host/address");
			}
			if (port < 0 || port > 65535)
			{
				throw new ArgumentException("Invalid connection port");
			}
			try
			{
				IPAddress.Parse(host);
			}
			catch (FormatException)
			{
				try
				{
					host = Dns.GetHostEntry(host).AddressList[0].ToString();
				}
				catch (Exception ex)
				{
					string text = "Failed to lookup hostname " + host + ". Connection failed. Reason " + ex.Message;
					log.Error(text);
					Hashtable hashtable = new Hashtable();
					hashtable["success"] = false;
					hashtable["errorMessage"] = text;
					DispatchEvent(new SFSEvent(SFSEvent.CONNECTION, hashtable));
					return;
				}
			}
			lastIpAddress = host;
			isConnecting = true;
			bitSwarm.Connect(host, port);
		}

		public void Disconnect()
		{
			if (IsConnected)
			{
				if (bitSwarm.ReconnectionSeconds > 0)
				{
					Send(new ManualDisconnectionRequest());
				}
				bitSwarm.Disconnect(ClientDisconnectionReason.MANUAL);
			}
			else
			{
				log.Info("You are not connected");
			}
		}

		public void InitUDP(string udpHost, int udpPort)
		{
			//Discarded unreachable code: IL_0110
			if (!IsConnected)
			{
				Logger.Warn("Cannot initialize UDP protocol until the client is connected to SFS2X.");
				return;
			}
			if (config != null)
			{
				if (udpHost == null)
				{
					udpHost = config.UdpHost;
				}
				if (udpPort == -1)
				{
					udpPort = config.UdpPort;
				}
			}
			if (udpHost == null || udpHost.Length == 0)
			{
				throw new ArgumentException("Invalid UDP host/address");
			}
			if (udpPort < 0 || udpPort > 65535)
			{
				throw new ArgumentException("Invalid UDP port range");
			}
			try
			{
				IPAddress.Parse(udpHost);
			}
			catch (FormatException)
			{
				try
				{
					udpHost = Dns.GetHostEntry(udpHost).AddressList[0].ToString();
				}
				catch (Exception ex)
				{
					string text = "Failed to lookup hostname " + udpHost + ". UDP init failed. Reason " + ex.Message;
					log.Error(text);
					Hashtable hashtable = new Hashtable();
					hashtable["success"] = false;
					DispatchEvent(new SFSEvent(SFSEvent.UDP_INIT, hashtable));
					return;
				}
			}
			if (bitSwarm.UdpManager == null || !bitSwarm.UdpManager.Inited)
			{
				IUDPManager udpManager = new UDPManager(this);
				bitSwarm.UdpManager = udpManager;
			}
			try
			{
				bitSwarm.UdpManager.Initialize(udpHost, udpPort);
			}
			catch (Exception ex3)
			{
				log.Error("Exception initializing UDP: " + ex3.Message);
			}
		}

		public int GetReconnectionSeconds()
		{
			return bitSwarm.ReconnectionSeconds;
		}

		public void SetReconnectionSeconds(int seconds)
		{
			bitSwarm.ReconnectionSeconds = seconds;
		}

		public void Send(IRequest request)
		{
			if (!IsConnected)
			{
				log.Warn("You are not connected. Request cannot be sent: " + request);
				return;
			}
			try
			{
				if (request is JoinRoomRequest)
				{
					if (isJoining)
					{
						return;
					}
					isJoining = true;
				}
				request.Validate(this);
				request.Execute(this);
				bitSwarm.Send(request.Message);
			}
			catch (SFSValidationError sFSValidationError)
			{
				string text = sFSValidationError.Message;
				foreach (string error in sFSValidationError.Errors)
				{
					text = text + "\t" + error + "\n";
				}
				log.Warn(text);
			}
			catch (SFSCodecError sFSCodecError)
			{
				log.Warn(sFSCodecError.Message);
			}
		}

		public void AddLogListener(LogLevel logLevel, EventListenerDelegate eventListener)
		{
			AddEventListener(LoggerEvent.LogEventType(logLevel), eventListener);
			log.EnableEventDispatching = true;
		}

		private void OnSocketConnect(BaseEvent e)
		{
			BitSwarmEvent bitSwarmEvent = e as BitSwarmEvent;
			if ((bool)bitSwarmEvent.Params["success"])
			{
				SendHandshakeRequest((bool)bitSwarmEvent.Params["isReconnection"]);
				return;
			}
			log.Warn("Connection attempt failed");
			HandleConnectionProblem(bitSwarmEvent);
		}

		private void OnSocketClose(BaseEvent e)
		{
			BitSwarmEvent bitSwarmEvent = e as BitSwarmEvent;
			Reset();
			Hashtable hashtable = new Hashtable();
			hashtable["reason"] = bitSwarmEvent.Params["reason"];
			DispatchEvent(new SFSEvent(SFSEvent.CONNECTION_LOST, hashtable));
		}

		private void OnSocketReconnectionTry(BaseEvent e)
		{
			DispatchEvent(new SFSEvent(SFSEvent.CONNECTION_RETRY));
		}

		private void OnSocketDataError(BaseEvent e)
		{
			Hashtable hashtable = new Hashtable();
			hashtable["errorMessage"] = e.Params["message"];
			DispatchEvent(new SFSEvent(SFSEvent.SOCKET_ERROR, hashtable));
		}

		private void OnSocketIOError(BaseEvent e)
		{
			BitSwarmEvent e2 = e as BitSwarmEvent;
			if (isConnecting)
			{
				HandleConnectionProblem(e2);
			}
		}

		private void OnSocketSecurityError(BaseEvent e)
		{
			BitSwarmEvent e2 = e as BitSwarmEvent;
			if (isConnecting)
			{
				HandleConnectionProblem(e2);
			}
		}

		public void HandleHandShake(BaseEvent evt)
		{
			ISFSObject iSFSObject = evt.Params["message"] as ISFSObject;
			if (iSFSObject.IsNull(BaseRequest.KEY_ERROR_CODE))
			{
				sessionToken = iSFSObject.GetUtfString(HandshakeRequest.KEY_SESSION_TOKEN);
				bitSwarm.CompressionThreshold = iSFSObject.GetInt(HandshakeRequest.KEY_COMPRESSION_THRESHOLD);
				bitSwarm.MaxMessageSize = iSFSObject.GetInt(HandshakeRequest.KEY_MAX_MESSAGE_SIZE);
				if (debug)
				{
					log.Debug(string.Format("Handshake response: tk => {0}, ct => {1}", sessionToken, bitSwarm.CompressionThreshold));
				}
				if (bitSwarm.IsReconnecting)
				{
					bitSwarm.IsReconnecting = false;
					DispatchEvent(new SFSEvent(SFSEvent.CONNECTION_RESUME));
					return;
				}
				isConnecting = false;
				Hashtable hashtable = new Hashtable();
				hashtable["success"] = true;
				DispatchEvent(new SFSEvent(SFSEvent.CONNECTION, hashtable));
			}
			else
			{
				short @short = iSFSObject.GetShort(BaseRequest.KEY_ERROR_CODE);
				string errorMessage = SFSErrorCodes.GetErrorMessage(@short, log, iSFSObject.GetUtfStringArray(BaseRequest.KEY_ERROR_PARAMS));
				Hashtable hashtable2 = new Hashtable();
				hashtable2["success"] = false;
				hashtable2["errorMessage"] = errorMessage;
				hashtable2["errorCode"] = @short;
				DispatchEvent(new SFSEvent(SFSEvent.CONNECTION, hashtable2));
			}
		}

		public void HandleLogin(BaseEvent evt)
		{
			currentZone = evt.Params["zone"] as string;
		}

		public void HandleClientDisconnection(string reason)
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("reason", reason);
			DispatchEvent(new SFSEvent(SFSEvent.CONNECTION_LOST, hashtable));
			bitSwarm.ReconnectionSeconds = 0;
			bitSwarm.Disconnect(reason);
			Reset();
		}

		public void HandleLogout()
		{
			if (lagMonitor != null && lagMonitor.IsRunning)
			{
				lagMonitor.Stop();
			}
			userManager = new SFSGlobalUserManager(this);
			roomManager = new SFSRoomManager(this);
			isJoining = false;
			lastJoinedRoom = null;
			currentZone = null;
			mySelf = null;
		}

		private void HandleConnectionProblem(BaseEvent e)
		{
			if (bitSwarm.ConnectionMode == ConnectionModes.SOCKET && useBlueBox && bbConnectionAttempt < 3)
			{
				bbConnectionAttempt++;
				bitSwarm.ForceBlueBox(true);
				int port = ((config == null) ? 8080 : config.HttpPort);
				bitSwarm.Connect(lastIpAddress, port);
				DispatchEvent(new SFSEvent(SFSEvent.CONNECTION_ATTEMPT_HTTP, new Hashtable()));
			}
			else
			{
				bitSwarm.ForceBlueBox(false);
				bbConnectionAttempt = 0;
				BitSwarmEvent bitSwarmEvent = e as BitSwarmEvent;
				Hashtable hashtable = new Hashtable();
				hashtable["success"] = false;
				hashtable["errorMessage"] = bitSwarmEvent.Params["message"];
				DispatchEvent(new SFSEvent(SFSEvent.CONNECTION, hashtable));
				isConnecting = false;
			}
		}

		private void SendHandshakeRequest(bool isReconnection)
		{
			IRequest request = new HandshakeRequest(Version, (!isReconnection) ? null : sessionToken);
			Send(request);
		}

		internal void DispatchEvent(BaseEvent evt)
		{
			if (!threadSafeMode)
			{
				Dispatcher.DispatchEvent(evt);
			}
			else
			{
				EnqueueEvent(evt);
			}
		}

		private void EnqueueEvent(BaseEvent evt)
		{
			lock (eventsLocker)
			{
				eventsQueue.Enqueue(evt);
			}
		}

		public void ProcessEvents()
		{
			if (threadSafeMode)
			{
				BaseEvent[] array;
				lock (eventsLocker)
				{
					array = eventsQueue.ToArray();
					eventsQueue.Clear();
				}
				BaseEvent[] array2 = array;
				foreach (BaseEvent evt in array2)
				{
					Dispatcher.DispatchEvent(evt);
				}
			}
		}

		public void AddEventListener(string eventType, EventListenerDelegate listener)
		{
			dispatcher.AddEventListener(eventType, listener);
		}

		public void RemoveAllEventListeners()
		{
			dispatcher.RemoveAll();
		}
	}
}
