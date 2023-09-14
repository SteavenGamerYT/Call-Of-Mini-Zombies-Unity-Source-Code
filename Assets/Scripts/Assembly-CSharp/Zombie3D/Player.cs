using System.Collections.Generic;
using Sfs2X;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Variables;
using Sfs2X.Requests;
using UnityEngine;

namespace Zombie3D
{
	public class Player
	{
		public const float net_status_rate = 0.2f;

		public PlayerState IDLE_STATE;

		public PlayerState RUN_STATE;

		public PlayerState SHOOT_STATE;

		public PlayerState RUNSHOOT_STATE;

		public PlayerState GOTHIT_STATE;

		public PlayerState DEAD_STATE;

		protected GameObject playerObject;

		protected BaseCameraScript gameCamera;

		protected Transform playerTransform;

		protected CharacterController charController;

		protected Animation animation;

		protected Collider collider;

		protected GameObject powerObj;

		protected Transform respawnTrans;

		protected PlayerConfig playerConfig;

		protected AudioPlayer audioPlayer;

		protected AvatarType avatarType;

		protected PlayerState playerState;

		protected InputController inputController;

		protected Vector3 getHitFlySpeed;

		protected BombSpot bombSpot;

		protected Vector3 lastHitPosition;

		public Vector3 m_direction = Vector3.zero;

		protected Weapon cur_weapon;

		public List<Weapon> weaponList;

		protected float maxHp;

		protected float hp;

		protected float guiHp;

		protected float walkSpeed;

		protected float powerBuff;

		protected float powerBuffStartTime;

		protected float lastUpdateNearestWayPointTime;

		protected int currentWeaponIndex;

		protected GameObject[] points;

		public uint birth_point_index;

		public int m_life_packet_count = 1;

		public int m_life_packet_count_temp = 1;

		public NetworkObj net_com;

		public float last_user_status_time;

		protected PlayerShell playerShell;

		public int net_player_id = -1;

		protected SphereCollider rebirth_triger;

		public float net_ping_sum;

		public int m_multi_id;

		protected float gothitEndTime;

		protected string weaponNameEnd;

		protected bool isRunning;

		protected bool No_Damage = true;

		protected float god_time;

		protected bool is_god;

		public bool is_real_dead;

		public float lastFireDamagedTime;

		public int m_death_count;

		protected Vector3 last_pos = Vector3.zero;

		protected Vector3 last_rot = Vector3.zero;

		protected Vector3 last_dir = Vector3.zero;

		protected float vs_rebirth_time;

		protected int vs_kill_count;

		protected int vs_cash_loot;

		protected SmartFox smartFox;

		protected NetworkTransform net_trans;

		public User sfs_user;

		public int vs_combo_val;

		public int vs_combo_val_temp;

		private bool first_blood;

		protected Transform aimedTransform;

		public WayPointScript NearestWayPoint { get; set; }

		public int Vs_Kill_Count
		{
			get
			{
				return vs_kill_count;
			}
		}

		public Vector3 HitPoint { get; set; }

		public float WalkSpeed
		{
			get
			{
				if (avatarType != AvatarType.Worker)
				{
					float num = GameApp.GetInstance().GetGameConfig().playerConf.walkSpeed - cur_weapon.GetSpeedDrag();
					if (avatarType == AvatarType.Cowboy)
					{
						return num * 1.3f;
					}
					if (avatarType == AvatarType.EnegyArmor || avatarType == AvatarType.Ninja)
					{
						num = GameApp.GetInstance().GetGameConfig().playerConf.walkSpeed;
						return num * 1.3f;
					}
					return num;
				}
				return GameApp.GetInstance().GetGameConfig().playerConf.walkSpeed;
			}
		}

		public InputController InputController
		{
			get
			{
				return inputController;
			}
		}

		public bool IsRunning
		{
			get
			{
				return isRunning;
			}
		}

		public string WeaponNameEnd
		{
			get
			{
				return weaponNameEnd;
			}
			set
			{
				weaponNameEnd = value;
			}
		}

		public Vector3 GetHitFlySpeed
		{
			get
			{
				return getHitFlySpeed;
			}
		}

		public Vector3 LastHitPosition
		{
			get
			{
				return lastHitPosition;
			}
			set
			{
				lastHitPosition = value;
			}
		}

		public BombSpot BombSpot
		{
			get
			{
				return bombSpot;
			}
			set
			{
				bombSpot = value;
			}
		}

		public GameObject PlayerObject
		{
			get
			{
				return playerObject;
			}
		}

		public float PowerBuff
		{
			get
			{
				return powerBuff;
			}
		}

		public float HP
		{
			get
			{
				return hp;
			}
		}

		public NetworkTransform networkTransform
		{
			get
			{
				return net_trans;
			}
			set
			{
				net_trans = value;
			}
		}

		public void RandomSwordAnimation()
		{
			if ("_Saw2" == weaponNameEnd)
			{
				weaponNameEnd = "_Saw";
			}
			else
			{
				weaponNameEnd = "_Saw2";
			}
		}

		public void RandomSawAnimation()
		{
			if (Math.RandomRate(50f))
			{
				weaponNameEnd = "_Saw";
			}
			else
			{
				weaponNameEnd = "_Saw2";
			}
		}

		public void ResetSawAnimation()
		{
			if (cur_weapon.GetWeaponType() == WeaponType.Saw || cur_weapon.GetWeaponType() == WeaponType.Sword)
			{
				weaponNameEnd = "_Saw";
			}
		}

		public virtual void CreateScreenBlood(float damage)
		{
			if (gameCamera != null)
			{
				gameCamera.CreateScreenBlood(1f);
			}
		}

		public virtual void Move(Vector3 motion)
		{
			if (charController != null)
			{
				charController.Move(motion);
				if ((playerState == RUN_STATE || playerState == RUNSHOOT_STATE) && GameApp.GetInstance().GetGameState().SoundOn)
				{
					audioPlayer.PlayAudio("Walk");
				}
			}
		}

		public float GetGuiHp()
		{
			return guiHp;
		}

		public float GetHp()
		{
			return hp;
		}

		public float GetMaxHp()
		{
			return maxHp;
		}

		public Transform GetTransform()
		{
			return playerTransform;
		}

		public Collider GetCollider()
		{
			return collider;
		}

		public PlayerState GetPlayerState()
		{
			return playerState;
		}

		public Transform GetRespawnTransform()
		{
			return respawnTrans;
		}

		public void PlusVsKillCount()
		{
			vs_kill_count++;
			Debug.Log("Kill Plyaer!Now count:" + vs_kill_count);
			GameVSScene gameVSScene = GameApp.GetInstance().GetGameScene() as GameVSScene;
			vs_combo_val_temp++;
			if (vs_combo_val_temp > vs_combo_val)
			{
				vs_combo_val = vs_combo_val_temp;
				Debug.Log("Commbo kill count:" + vs_combo_val);
			}
			if (!first_blood && vs_combo_val_temp == 1)
			{
				gameVSScene.game_tui.SetComboCountLabel(vs_combo_val_temp);
				first_blood = true;
			}
			else if (vs_combo_val_temp > 1)
			{
				gameVSScene.game_tui.SetComboCountLabel(vs_combo_val_temp);
			}
			gameVSScene.game_tui.SetKillCountLabel(vs_kill_count);
			GameApp.GetInstance().GetGameState().AddCashForReport(2000);
			UpdateVSStatistic();
		}

		public void UpdateVSStatistic()
		{
			List<UserVariable> list = new List<UserVariable>();
			SFSObject sFSObject = new SFSObject();
			sFSObject.PutInt("killCount", vs_kill_count);
			sFSObject.PutInt("deathCount", m_death_count);
			sFSObject.PutInt("cashLoot", GameApp.GetInstance().GetGameState().loot_cash);
			sFSObject.PutInt("vsCombo", vs_combo_val);
			list.Add(new SFSUserVariable("userStatistic", sFSObject));
			smartFox.Send(new SetUserVariablesRequest(list));
		}

		public virtual void Init()
		{
			IDLE_STATE = new PlayerIdleState();
			RUN_STATE = new PlayerRunState();
			SHOOT_STATE = new PlayerShootState();
			RUNSHOOT_STATE = new PlayerRunAndShootState();
			GOTHIT_STATE = new PlayerGotHitState();
			DEAD_STATE = new PlayerDeadState();
			birth_point_index = 0u;
			points = GameObject.FindGameObjectsWithTag("WayPoint");
			GameObject[] array = GameObject.FindGameObjectsWithTag("Respawn");
			GameObject gameObject;
			if (GameApp.GetInstance().GetGameState().endless_multiplayer)
			{
				net_com = GameApp.GetInstance().GetGameState().net_com;
				if (GameApp.GetInstance().GetGameState().net_com.m_netUserInfo.is_master)
				{
					birth_point_index = 0u;
				}
				else
				{
					birth_point_index = (uint)GameApp.GetInstance().GetGameState().net_com.m_netUserInfo.room_index;
				}
				gameObject = array[birth_point_index];
				GameMultiplayerScene gameMultiplayerScene = GameApp.GetInstance().GetGameScene() as GameMultiplayerScene;
				gameMultiplayerScene.game_tui.AddMultiplayerMark(this, (int)birth_point_index);
			}
			else
			{
				gameObject = array[birth_point_index = (uint)Random.Range(0, array.Length)];
			}
			respawnTrans = gameObject.transform;
			avatarType = GameApp.GetInstance().GetGameState().Avatar;
			List<int> avatar_use_count;
			List<int> list = (avatar_use_count = GameApp.GetInstance().GetGameState().user_statistics.avatar_use_count);
			int index;
			int index2 = (index = (int)avatarType);
			index = avatar_use_count[index];
			list[index2] = index + 1;
			playerObject = AvatarFactory.GetInstance().CreateAvatar(avatarType);
			playerObject.transform.position = gameObject.transform.position;
			playerObject.transform.rotation = gameObject.transform.rotation;
			playerObject.name = "Player";
			playerTransform = playerObject.transform;
			aimedTransform = playerTransform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head");
			playerConfig = GameApp.GetInstance().GetGameConfig().playerConf;
			int armorLevel = GameApp.GetInstance().GetGameState().ArmorLevel;
			if (GameApp.GetInstance().GetGameState().VS_mode)
			{
				GameVSScene gameVSScene = GameApp.GetInstance().GetGameScene() as GameVSScene;
				smartFox = SmartFoxConnection.Connection;
				net_trans = NetworkTransform.FromTransform(playerTransform);
				sfs_user = smartFox.MySelf;
			}
			hp = playerConfig.hp * (1f + (float)armorLevel * 0.5f);
			maxHp = hp;
			if (avatarType == AvatarType.Swat)
			{
				hp *= 2f;
				maxHp = hp;
			}
			else if (avatarType == AvatarType.EnegyArmor || avatarType == AvatarType.Eskimo)
			{
				hp *= 3f;
				maxHp = hp;
			}
			gameCamera = GameApp.GetInstance().GetGameScene().GetCamera();
			charController = playerObject.GetComponent<CharacterController>();
			animation = playerObject.GetComponent<Animation>();
			collider = playerObject.GetComponent<Collider>();
			audioPlayer = new AudioPlayer();
			Transform folderTrans = playerTransform.Find("Audio");
			audioPlayer.AddAudio(folderTrans, "GetItem", true);
			audioPlayer.AddAudio(folderTrans, "Dead", true);
			audioPlayer.AddAudio(folderTrans, "Switch", true);
			audioPlayer.AddAudio(folderTrans, "Walk", true);
			audioPlayer.AddAudio(folderTrans, "GetHp", true);
			audioPlayer.AddAudio(folderTrans, "GetBullet", true);
			GameApp.GetInstance().GetGameState().InitWeapons();
			weaponList = GameApp.GetInstance().GetGameState().GetBattleWeapons();
			GameApp.GetInstance().GetGameState().StatisticWeapons();
			playerState = IDLE_STATE;
			foreach (Weapon weapon in weaponList)
			{
				weapon.Init();
			}
			int num = 0;
			foreach (Weapon weapon2 in weaponList)
			{
				if (weapon2.IsSelectedForBattle || GameApp.GetInstance().GetGameState().VS_mode)
				{
					ChangeWeapon(weapon2);
					break;
				}
				num++;
			}
			walkSpeed = GameApp.GetInstance().GetGameConfig().playerConf.walkSpeed - cur_weapon.GetSpeedDrag();
			ChangeToNormalState();
			if (gameCamera.GetCameraType() == CameraType.TPSCamera)
			{
				inputController = new TPSInputController();
				inputController.Init();
				((TPSInputController)inputController).weaponList = weaponList;
			}
			else if (gameCamera.GetCameraType() == CameraType.TopWatchingCamera)
			{
				inputController = new TopWatchingInputController();
				inputController.Init();
			}
			playerShell = playerObject.AddComponent<PlayerShell>();
			playerShell.m_player = this;
			last_user_status_time = Time.time;
			m_life_packet_count = (m_life_packet_count_temp = GameApp.GetInstance().GetGameState().m_rescue_packet_count);
			if (m_life_packet_count == 0)
			{
				m_life_packet_count = (m_life_packet_count_temp = 1);
			}
			UpdateNearestWayPoint();
			SendNetMsg();
			GameApp.GetInstance().GetGameState().SaveSceneStatistics();
			vs_rebirth_time = 0f;
		}

		public virtual void OnVSRebirth()
		{
			OnRebirth();
			GameObject[] array = GameObject.FindGameObjectsWithTag("Respawn");
			int num = Random.Range(0, array.Length);
			GameObject gameObject = array[num];
			respawnTrans = gameObject.transform;
			playerObject.transform.position = gameObject.transform.position;
			playerObject.transform.rotation = gameObject.transform.rotation;
			vs_rebirth_time = 0f;
			ISFSObject iSFSObject = new SFSObject();
			ISFSObject iSFSObject2 = new SFSObject();
			net_trans = NetworkTransform.FromTransform(playerTransform);
			net_trans.TimeStamp = TimeManager.Instance.NetworkTime;
			net_trans.ToSFSObject(iSFSObject2);
			iSFSObject.PutSFSObject("rebirth", iSFSObject2);
			smartFox.Send(new ObjectMessageRequest(iSFSObject, smartFox.LastJoinedRoom));
			gameCamera.player = this;
		}

		public void SendNetMsg()
		{
			if (GameApp.GetInstance().GetGameState().endless_multiplayer)
			{
				m_multi_id = GameApp.GetInstance().GetGameState().net_com.m_netUserInfo.user_id;
				Debug.Log("Player id:" + m_multi_id);
				uint[] array = new uint[3];
				for (int i = 0; i < 3; i++)
				{
					array[i] = 9999u;
				}
				int num = 0;
				foreach (Weapon weapon in weaponList)
				{
					array[num] = (uint)weapon.weapon_index;
					num++;
				}
				Packet packet = CGUserBirthPacket.MakePacket((long)(Time.time * 1000f), birth_point_index, array[0], array[1], array[2]);
				net_com.Send(packet);
				AddRebirthCom();
			}
			else
			{
				if (!GameApp.GetInstance().GetGameState().VS_mode)
				{
					return;
				}
				int[] array2 = new int[3];
				for (int j = 0; j < 3; j++)
				{
					array2[j] = 9999;
				}
				int num2 = 0;
				foreach (Weapon weapon2 in weaponList)
				{
					array2[num2] = weapon2.weapon_index;
					num2++;
				}
				float[] array3 = new float[3];
				for (int k = 0; k < 3; k++)
				{
					array3[k] = 0f;
				}
				num2 = 0;
				foreach (Weapon weapon3 in weaponList)
				{
					array3[num2] = weapon3.AttackFrequency;
					num2++;
				}
				List<UserVariable> list = new List<UserVariable>();
				SFSObject sFSObject = new SFSObject();
				sFSObject.PutUtfString("NickName", GameApp.GetInstance().GetGameState().nick_name);
				sFSObject.PutInt("avatarType", (int)GetAvatarType());
				sFSObject.PutInt("weapon1", array2[0]);
				sFSObject.PutInt("weapon2", array2[1]);
				sFSObject.PutInt("weapon3", array2[2]);
				sFSObject.PutFloat("weaponPara1", array3[0]);
				sFSObject.PutFloat("weaponPara2", array3[1]);
				sFSObject.PutFloat("weaponPara3", array3[2]);
				sFSObject.PutFloat("maxHp", maxHp);
				sFSObject.PutInt("birthPoint", (int)birth_point_index);
				list.Add(new SFSUserVariable("avatarData", sFSObject));
				list.Add(new SFSUserVariable("CurWeapon", 0));
				list.Add(new SFSUserVariable("PlayerState", (int)playerState.GetStateType()));
				smartFox.Send(new SetUserVariablesRequest(list));
			}
		}

		public virtual void AddRebirthCom()
		{
			rebirth_triger = playerObject.AddComponent<SphereCollider>();
			rebirth_triger.radius = 1f;
			rebirth_triger.isTrigger = true;
			rebirth_triger.enabled = false;
			playerObject.AddComponent<PlayerRebirth>();
		}

		public AvatarType GetAvatarType()
		{
			return avatarType;
		}

		public void SetAvatarType(AvatarType type)
		{
			avatarType = type;
		}

		public void UpdateNearestWayPoint()
		{
			float num = 99999f;
			GameObject[] array = points;
			GameObject[] array2 = array;
			foreach (GameObject gameObject in array2)
			{
				WayPointScript component = gameObject.GetComponent<WayPointScript>();
				float magnitude = (component.transform.position - playerTransform.position).magnitude;
				if (magnitude < num)
				{
					Ray ray = new Ray(playerTransform.position + new Vector3(0f, 0.5f, 0f), component.transform.position - playerTransform.position);
					RaycastHit hitInfo;
					if (!Physics.Raycast(ray, out hitInfo, magnitude, 34816))
					{
						NearestWayPoint = component;
						num = magnitude;
					}
				}
			}
		}

		public void Run()
		{
			isRunning = true;
		}

		public void StopRun()
		{
			isRunning = false;
		}

		public virtual void SetState(PlayerState state)
		{
			playerState = state;
			if (GameApp.GetInstance().GetGameState().endless_multiplayer && net_com != null)
			{
				Packet packet = CGUserActionPacket.MakePacket((uint)net_com.m_netUserInfo.user_id, (uint)playerState.GetStateType());
				net_com.Send(packet);
			}
			else if (GameApp.GetInstance().GetGameState().VS_mode)
			{
				List<UserVariable> list = new List<UserVariable>();
				list.Add(new SFSUserVariable("PlayerState", (int)playerState.GetStateType()));
				smartFox.Send(new SetUserVariablesRequest(list));
			}
		}

		public void SetStateWithType(PlayerStateType type)
		{
			switch (type)
			{
			case PlayerStateType.Dead:
				SetState(DEAD_STATE);
				break;
			case PlayerStateType.GotHit:
				SetState(GOTHIT_STATE);
				break;
			case PlayerStateType.Idle:
				SetState(IDLE_STATE);
				break;
			case PlayerStateType.RunShoot:
				SetState(RUNSHOOT_STATE);
				break;
			case PlayerStateType.Run:
				SetState(RUN_STATE);
				break;
			case PlayerStateType.Shoot:
				SetState(SHOOT_STATE);
				break;
			}
		}

		public bool IsPlayingAnimation(string name)
		{
			return animation.IsPlaying(name);
		}

		public bool AnimationEnds(string name)
		{
			if (animation[name].time >= animation[name].clip.length * 1f || animation[name].wrapMode == WrapMode.Loop)
			{
				return true;
			}
			return false;
		}

		public bool IsAnimationPlayedPercentage(string aniName, float percentage)
		{
			if (animation[aniName].time >= animation[aniName].clip.length * percentage)
			{
				return true;
			}
			return false;
		}

		public void PlayAnimate(string animationName, WrapMode wrapMode)
		{
			if (!(animation[animationName] != null))
			{
				return;
			}
			animation[animationName].wrapMode = wrapMode;
			if (!IsPlayingAnimation("Damage01") || animationName.StartsWith("Death0"))
			{
				if (wrapMode == WrapMode.Loop || (!animation.IsPlaying(animationName) && animationName != "Damage01"))
				{
					animation.Play(animationName);
					return;
				}
				animation.Stop();
				animation.Play(animationName);
			}
		}

		public void Animate(string animationName, WrapMode wrapMode)
		{
			if (!(animation[animationName] != null))
			{
				return;
			}
			animation[animationName].wrapMode = wrapMode;
			if (!IsPlayingAnimation("Damage01") || animationName.StartsWith("Death0"))
			{
				if (wrapMode == WrapMode.Loop || (!animation.IsPlaying(animationName) && animationName != "Damage01"))
				{
					animation.CrossFade(animationName);
					return;
				}
				animation.Stop();
				animation.Play(animationName);
			}
		}

		public void CheckBombSpot()
		{
			bombSpot = null;
			List<BombSpot> bombSpots = GameApp.GetInstance().GetGameScene().GetBombSpots();
			foreach (BombSpot item in bombSpots)
			{
				item.DoLogic();
				if (item.CheckInSpot())
				{
					bombSpot = item;
				}
				else if (item.isInstalling())
				{
					bombSpot = item;
				}
			}
		}

		public void ZoomIn(float deltaTime)
		{
			if (cur_weapon.GetWeaponType() == WeaponType.AssaultRifle || cur_weapon.GetWeaponType() == WeaponType.MachineGun)
			{
				gameCamera.ZoomIn(deltaTime);
			}
		}

		public void AutoAim(float deltaTime)
		{
			cur_weapon.AutoAim(deltaTime);
		}

		public void Fire(float deltaTime)
		{
			if (GameApp.GetInstance().GetGameScene().GamePlayingState == PlayingState.GamePlaying)
			{
				cur_weapon.Fire(deltaTime);
			}
		}

		public void ZoomOut(float deltaTime)
		{
			gameCamera.ZoomOut(deltaTime);
		}

		public void StopFire()
		{
			cur_weapon.StopFire();
		}

		public virtual void DoLogic(float deltaTime)
		{
			playerState.DoStateLogic(this, deltaTime);
			if (GameApp.GetInstance().GetGameState().VS_mode && playerState == DEAD_STATE)
			{
				vs_rebirth_time += deltaTime;
				if (vs_rebirth_time >= 5f)
				{
					OnVSRebirth();
					if (GameApp.GetInstance().GetGameState().vs_toturial_triger_dead == 1)
					{
						GameVSScene gameVSScene = GameApp.GetInstance().GetGameScene() as GameVSScene;
						gameVSScene.game_tui.HideTutorialMsgDead();
					}
				}
			}
			if (guiHp != hp)
			{
				float num = Mathf.Abs(guiHp - hp);
				guiHp = Mathf.MoveTowards(guiHp, hp, num * 5f * deltaTime);
			}
			if (powerBuff != 1f && Time.time - powerBuffStartTime > 30f)
			{
				ChangeToNormalState();
			}
			if (is_god)
			{
				god_time += deltaTime;
				if (god_time >= 5f)
				{
					ChangeToNormalState();
				}
			}
			foreach (Weapon weapon in weaponList)
			{
				if (weapon.IsSelectedForBattle)
				{
					weapon.DoLogic();
				}
			}
			if ((avatarType == AvatarType.Doctor || avatarType == AvatarType.Pastor) && playerState != DEAD_STATE && !GameApp.GetInstance().GetGameState().VS_mode)
			{
				hp += maxHp * 1f / 100f * deltaTime;
				if (hp > maxHp)
				{
					hp = maxHp;
				}
			}
			if (Time.time - lastUpdateNearestWayPointTime > 1f && !GameApp.GetInstance().GetGameState().VS_mode)
			{
				UpdateNearestWayPoint();
				lastUpdateNearestWayPointTime = Time.time;
			}
			if (Time.time - last_user_status_time >= 0.2f)
			{
				SendNetUserStatusMsg();
				last_user_status_time = Time.time;
			}
			if (GameApp.GetInstance().GetGameState().endless_multiplayer)
			{
				GameMultiplayerScene gameMultiplayerScene = GameApp.GetInstance().GetGameScene() as GameMultiplayerScene;
				if (No_Damage)
				{
					gameMultiplayerScene.multi_god_time += deltaTime;
				}
				if (playerState != DEAD_STATE)
				{
					gameMultiplayerScene.multi_survive_time += deltaTime;
				}
			}
		}

		public void SendNetUserStatusMsg()
		{
			if (GameApp.GetInstance().GetGameState().endless_multiplayer)
			{
				if (last_pos != playerTransform.position || last_rot != playerTransform.rotation.eulerAngles || last_dir != m_direction)
				{
					Packet packet = CGUserStatusPacket.MakePacket((uint)net_com.m_netUserInfo.user_id, playerTransform.position, playerTransform.rotation.eulerAngles, m_direction, (ulong)(net_com.m_netUserInfo.net_ping * 1000f));
					net_com.Send(packet);
					last_pos = playerTransform.position;
					last_rot = playerTransform.rotation.eulerAngles;
					last_dir = m_direction;
				}
			}
			else if (GameApp.GetInstance().GetGameState().VS_mode)
			{
				ISFSObject iSFSObject = new SFSObject();
				net_trans = NetworkTransform.FromTransform(playerTransform);
				net_trans.TimeStamp = TimeManager.Instance.NetworkTime;
				net_trans.ToSFSObject(iSFSObject);
				ISFSObject iSFSObject2 = new SFSObject();
				iSFSObject2.PutSFSObject("data", iSFSObject);
				smartFox.Send(new ExtensionRequest("Room.Common.UdpBroadcast", iSFSObject2, smartFox.LastJoinedRoom, true));
			}
		}

		public virtual void OnHit(float damage)
		{
			if (is_god)
			{
				return;
			}
			hp -= damage;
			hp = (int)hp;
			hp = Mathf.Clamp(hp, 0f, maxHp);
			playerState.OnHit(this, damage);
			if (GameApp.GetInstance().GetGameState().endless_multiplayer)
			{
				Packet packet = CGUserInjuryPacket.MakePacket((uint)GameApp.GetInstance().GetGameState().net_com.m_netUserInfo.user_id, (long)(damage * 1000f), (long)(maxHp * 1000f), (long)(hp * 1000f));
				GameApp.GetInstance().GetGameState().net_com.Send(packet);
				if (No_Damage)
				{
					No_Damage = false;
				}
			}
		}

		public bool CouldGetAnotherHit()
		{
			if (Time.time - gothitEndTime > 0.5f)
			{
				gothitEndTime = Time.time;
				return true;
			}
			return false;
		}

		public Transform GetAimedTransform()
		{
			return aimedTransform;
		}

		public virtual void OnRebirth()
		{
			Debug.Log("Player OnRebirth...");
			playerObject.GetComponent<Collider>().enabled = true;
			if (!GameApp.GetInstance().GetGameState().VS_mode)
			{
				rebirth_triger.enabled = false;
			}
			playerObject.layer = 8;
			Transform transform = gameCamera.gameObject.transform.Find("Screen_Blood_Dead");
			if (transform != null)
			{
				transform.gameObject.active = false;
			}
			GetHealed((int)maxHp);
			GameScene gameScene = GameApp.GetInstance().GetGameScene();
			gameScene.GamePlayingState = PlayingState.GamePlaying;
			gameCamera.GetComponent<TPSSimpleCameraScript>().minAngelV = -25f;
			gameCamera.GetComponent<TPSSimpleCameraScript>().maxAngelV = 0f;
			SetState(IDLE_STATE);
			ChangeToGodBuffState();
			if (GameApp.GetInstance().GetGameState().endless_multiplayer)
			{
				GameMultiplayerScene gameMultiplayerScene = gameScene as GameMultiplayerScene;
				gameMultiplayerScene.m_multi_player_arr.Add(this);
				gameMultiplayerScene.game_tui.HideRebirthMsg();
				gameMultiplayerScene.ResetEnemyTarget();
				is_real_dead = false;
			}
		}

		public virtual void PlayerRealDead()
		{
			Packet packet = CGGameOverPacket.MakePacket((uint)m_multi_id);
			net_com.Send(packet);
			is_real_dead = true;
			SceneCheckOver();
		}

		public virtual void OnVsInjured(User sender, float damage, int weapon_type)
		{
		}

		public void SceneCheckOver()
		{
			GameMultiplayerScene gameMultiplayerScene = GameApp.GetInstance().GetGameScene() as GameMultiplayerScene;
			gameMultiplayerScene.CheckMultiGameOver();
		}

		public virtual void OnDead()
		{
			audioPlayer.PlayAudio("Dead");
			cur_weapon.StopFire();
			int num = Random.Range(1, 4);
			Animate("Death0" + num, WrapMode.ClampForever);
			Transform transform = gameCamera.gameObject.transform.Find("Screen_Blood_Dead");
			if (transform != null)
			{
				transform.gameObject.active = true;
			}
			m_death_count++;
			playerObject.GetComponent<Collider>().enabled = false;
			GameScene gameScene = GameApp.GetInstance().GetGameScene();
			if (GameApp.GetInstance().GetGameState().endless_multiplayer)
			{
				gameScene.GamePlayingState = PlayingState.GameLose;
				rebirth_triger.enabled = true;
				playerObject.layer = 27;
				GameMultiplayerScene gameMultiplayerScene = gameScene as GameMultiplayerScene;
				GameApp.GetInstance().GetGameScene().OnMultiPlayerDead(this);
				playerObject.GetComponent<PlayerRebirth>().CancelRebirth();
				Packet packet = CGOnUserDeadPacket.MakePacket();
				net_com.Send(packet);
				if (GameApp.GetInstance().GetGameState().m_rebirth_packet_count > 0)
				{
					is_real_dead = false;
					gameMultiplayerScene.game_tui.ShowRebirthMsg();
				}
				else
				{
					PlayerRealDead();
				}
			}
			else if (GameApp.GetInstance().GetGameState().VS_mode)
			{
				GameVSScene gameVSScene = GameApp.GetInstance().GetGameScene() as GameVSScene;
				gameVSScene.game_tui.SetDeathCountLabel(m_death_count);
				if (GameApp.GetInstance().GetGameState().vs_toturial_triger_dead == 1)
				{
					gameVSScene.game_tui.ShowTutorialMsgDead();
				}
				gameScene.GamePlayingState = PlayingState.GameLose;
				UpdateVSStatistic();
				vs_combo_val_temp = 0;
			}
			else
			{
				gameScene.GamePlayingState = PlayingState.GameLose;
				GameUIScript component = GameObject.Find("SceneGUI").GetComponent<GameUIScript>();
				component.GetPanel(1).Show();
				GameApp.GetInstance().GetGameState().Achievement.LoseGame();
				gameCamera.GetComponent<AudioSource>().Stop();
				gameCamera.loseAudio.Play();
				gameCamera.loseAudio.mute = !GameApp.GetInstance().GetGameState().SoundOn;
			}
		}

		public void GetHealed(int point)
		{
			hp += point;
			hp = Mathf.Clamp(hp, 0f, maxHp);
		}

		public void GetFullyHealed()
		{
			hp = maxHp;
		}

		public void SendNetUserChangeWeaponMsg(int index)
		{
			if (GameApp.GetInstance().GetGameState().endless_multiplayer)
			{
				Packet packet = CGUserChangeWeaponPacket.MakePacket((uint)net_com.m_netUserInfo.user_id, (uint)index);
				net_com.Send(packet);
				Debug.Log("Player change weapon : " + index);
			}
			else if (GameApp.GetInstance().GetGameState().VS_mode)
			{
				List<UserVariable> list = new List<UserVariable>();
				list.Add(new SFSUserVariable("CurWeapon", index));
				smartFox.Send(new SetUserVariablesRequest(list));
			}
		}

		public virtual void ChangeWeapon(Weapon weapon)
		{
			if (weapon.IsSelectedForBattle || GameApp.GetInstance().GetGameState().VS_mode)
			{
				if (cur_weapon != null)
				{
					cur_weapon.GunOff();
					Animate("Idle01" + weaponNameEnd, WrapMode.Loop);
				}
				cur_weapon = weapon;
				cur_weapon.changeReticle();
				cur_weapon.GunOn();
				audioPlayer.PlayAudio("Switch");
				if (cur_weapon.GetWeaponType() == WeaponType.RocketLauncher)
				{
					weaponNameEnd = "_RPG";
				}
				else if (cur_weapon.GetWeaponType() == WeaponType.ShotGun || cur_weapon.GetWeaponType() == WeaponType.M32)
				{
					weaponNameEnd = "_Shotgun";
				}
				else if (cur_weapon.GetWeaponType() == WeaponType.Sniper)
				{
					weaponNameEnd = "_RPG";
				}
				else if (cur_weapon.GetWeaponType() == WeaponType.LaserGun)
				{
					weaponNameEnd = string.Empty;
				}
				else if (cur_weapon.GetWeaponType() == WeaponType.MachineGun)
				{
					weaponNameEnd = string.Empty;
				}
				else if (cur_weapon.GetWeaponType() == WeaponType.Saw)
				{
					weaponNameEnd = "_Saw";
				}
				else
				{
					weaponNameEnd = string.Empty;
				}
				gameCamera.isAngelVFixed = false;
				GameUIScript component = GameObject.Find("SceneGUI").GetComponent<GameUIScript>();
				component.SetWeaponLogo(cur_weapon.GetWeaponType());
			}
		}

		public void NextWeapon()
		{
			if (GameApp.GetInstance().GetGameScene().GamePlayingState != 0)
			{
				return;
			}
			currentWeaponIndex++;
			if (currentWeaponIndex >= weaponList.Count)
			{
				currentWeaponIndex = 0;
			}
			while (true)
			{
				bool flag = true;
				if (weaponList[currentWeaponIndex] == null)
				{
					flag = false;
				}
				else if (!weaponList[currentWeaponIndex].IsSelectedForBattle)
				{
					flag = false;
				}
				if (flag)
				{
					break;
				}
				currentWeaponIndex++;
				if (currentWeaponIndex >= weaponList.Count)
				{
					currentWeaponIndex = 0;
				}
			}
			ChangeWeapon(weaponList[currentWeaponIndex]);
			SendNetUserChangeWeaponMsg(currentWeaponIndex);
		}

		public bool CheckWeaponAvailably()
		{
			bool result = false;
			foreach (Weapon weapon in weaponList)
			{
				if (weapon.IsAvailably())
				{
					return true;
				}
			}
			return result;
		}

		public virtual bool OnPickUp(ItemType itemID)
		{
			if (GameApp.GetInstance().GetGameState().endless_multiplayer)
			{
				GameMultiplayerScene gameMultiplayerScene = GameApp.GetInstance().GetGameScene() as GameMultiplayerScene;
				gameMultiplayerScene.loot_item++;
			}
			switch (itemID)
			{
			case ItemType.Hp:
				GetHealed((int)maxHp);
				audioPlayer.PlayAudio("GetHp");
				break;
			case ItemType.Gold:
			{
				int num = (int)(300f * GameApp.GetInstance().GetGameScene().GetDifficultyCashDropFactor);
				GameApp.GetInstance().GetGameState().AddCashForRecord(num);
				if (GameApp.GetInstance().GetGameState().endless_level)
				{
					GameApp.GetInstance().GetGameScene().endless_get_cash += num;
				}
				audioPlayer.PlayAudio("GetItem");
				break;
			}
			case ItemType.Gold_Big:
			{
				int num2 = (int)(300f * GameApp.GetInstance().GetGameScene().GetDifficultyCashDropFactor * 5f);
				GameApp.GetInstance().GetGameState().AddCashForRecord(num2);
				if (GameApp.GetInstance().GetGameState().endless_level)
				{
					GameApp.GetInstance().GetGameScene().endless_get_cash += num2;
				}
				audioPlayer.PlayAudio("GetItem");
				break;
			}
			case ItemType.Power:
				ChangeToPowerBuffState();
				audioPlayer.PlayAudio("GetItem");
				break;
			case ItemType.AssaultGun:
				foreach (Weapon weapon in weaponList)
				{
					if (weapon.GetWeaponType() == WeaponType.AssaultRifle)
					{
						weapon.AddBullets(weapon.WConf.bullet / 4);
						break;
					}
				}
				goto IL_0540;
			case ItemType.ShotGun:
				foreach (Weapon weapon2 in weaponList)
				{
					if (weapon2.GetWeaponType() == WeaponType.ShotGun)
					{
						weapon2.AddBullets(weapon2.WConf.bullet / 4);
						break;
					}
				}
				goto IL_0540;
			case ItemType.RocketLauncer:
				foreach (Weapon weapon3 in weaponList)
				{
					if (weapon3.GetWeaponType() == WeaponType.RocketLauncher)
					{
						weapon3.AddBullets(weapon3.WConf.bullet / 4);
						break;
					}
				}
				goto IL_0540;
			case ItemType.LaserGun:
				foreach (Weapon weapon4 in weaponList)
				{
					if (weapon4.GetWeaponType() == WeaponType.LaserGun)
					{
						weapon4.AddBullets(weapon4.WConf.bullet / 4);
						break;
					}
				}
				goto IL_0540;
			case ItemType.Saw:
				foreach (Weapon weapon5 in weaponList)
				{
					if (weapon5.GetWeaponType() == WeaponType.Saw)
					{
						weapon5.AddBullets(weapon5.WConf.bullet / 4);
						break;
					}
				}
				goto IL_0540;
			case ItemType.Sniper:
				foreach (Weapon weapon6 in weaponList)
				{
					if (weapon6.GetWeaponType() == WeaponType.Sniper)
					{
						weapon6.AddBullets(weapon6.WConf.bullet / 4);
						break;
					}
				}
				goto IL_0540;
			case ItemType.MachineGun:
				foreach (Weapon weapon7 in weaponList)
				{
					if (weapon7.GetWeaponType() == WeaponType.MachineGun)
					{
						weapon7.AddBullets(weapon7.WConf.bullet / 4);
						break;
					}
				}
				goto IL_0540;
			case ItemType.M32:
				foreach (Weapon weapon8 in weaponList)
				{
					if (weapon8.GetWeaponType() == WeaponType.M32)
					{
						weapon8.AddBullets(weapon8.WConf.bullet / 4);
						break;
					}
				}
				goto IL_0540;
			case ItemType.Fire:
				foreach (Weapon weapon9 in weaponList)
				{
					if (weapon9.GetWeaponType() == WeaponType.FireGun)
					{
						weapon9.AddBullets(weapon9.WConf.bullet / 4);
						break;
					}
				}
				goto IL_0540;
			default:
				{
					if (AddVSWeapon((int)(itemID - 16)))
					{
						return true;
					}
					return false;
				}
				IL_0540:
				audioPlayer.PlayAudio("GetBullet");
				break;
			}
			return true;
		}

		public virtual bool AddVSWeapon(int type)
		{
			if (weaponList.Count >= 3)
			{
				return false;
			}
			Weapon weapon = GameApp.GetInstance().GetGameState().GetWeapons()[type];
			weapon.Init();
			weapon.IsSelectedForBattle = true;
			weaponList.Add(weapon);
			playerObject.GetComponent<NetworkView>().RPC("RPCAddVSWeapon", RPCMode.Others, type);
			NextWeapon();
			return true;
		}

		public Weapon GetWeapon()
		{
			return cur_weapon;
		}

		public void SetTransparent(bool bTrue)
		{
			if (bTrue)
			{
				playerObject.transform.Find("Avatar_Suit").GetComponent<Renderer>().material.shader = Shader.Find("iPhone/AlphaBlend_Color");
				playerObject.transform.Find("Avatar_Suit").GetComponent<Renderer>().material.SetColor("_TintColor", new Color(1f, 1f, 1f, 0.1f));
			}
			else
			{
				playerObject.transform.Find("Avatar_Suit").GetComponent<Renderer>().material.shader = Shader.Find("iPhone/SolidTexture");
			}
		}

		public void ChangeToGodBuffState()
		{
			is_god = true;
			god_time = 0f;
			if (avatarType == AvatarType.EnegyArmor)
			{
				return;
			}
			Color white = Color.white;
			playerObject.transform.Find("Avatar_Suit").GetComponent<Renderer>().material.shader = Shader.Find("iPhone/AlphaBlend_Color");
			playerObject.transform.Find("Avatar_Suit").GetComponent<Renderer>().material.SetColor("_TintColor", white);
			playerObject.transform.Find("Avatar_Suit").GetComponent<AlphaAnimationScript>().enableAlphaAnimation = true;
			playerObject.transform.Find("Avatar_Suit").GetComponent<AlphaAnimationScript>().animationSpeed = 3f;
			Transform transform = playerObject.transform.Find("Avatar_Cap");
			if (avatarType == AvatarType.Pastor)
			{
				if (transform != null)
				{
					transform.GetComponent<Renderer>().material.SetColor("_TintColor", white);
					transform.GetComponent<AlphaAnimationScript>().enableAlphaAnimation = true;
					transform.GetComponent<AlphaAnimationScript>().animationSpeed = 3f;
				}
			}
			else if (transform != null)
			{
				transform.GetComponent<Renderer>().material.shader = Shader.Find("iPhone/AlphaBlend_Color");
				transform.GetComponent<Renderer>().material.SetColor("_TintColor", white);
				transform.GetComponent<AlphaAnimationScript>().enableAlphaAnimation = true;
				transform.GetComponent<AlphaAnimationScript>().animationSpeed = 3f;
			}
		}

		public void ChangeToPowerBuffState()
		{
			powerBuff = 2f;
			powerBuffStartTime = Time.time;
			GameObject powerLogo = GameApp.GetInstance().GetResourceConfig().powerLogo;
			if (powerObj == null)
			{
				powerObj = Object.Instantiate(powerLogo, playerTransform.TransformPoint(Vector3.up * 2f), Quaternion.identity);
				powerObj.transform.parent = playerTransform;
			}
		}

		public void ChangeToNormalState()
		{
			is_god = false;
			god_time = 0f;
			powerBuff = 1f;
			if (avatarType != AvatarType.EnegyArmor)
			{
				Color value = new Color(0.8f, 0.8f, 0.8f);
				playerObject.transform.Find("Avatar_Suit").GetComponent<Renderer>().material.shader = Shader.Find("iPhone/SolidTexture");
				playerObject.transform.Find("Avatar_Suit").GetComponent<Renderer>().material.SetColor("_TintColor", value);
				playerObject.transform.Find("Avatar_Suit").GetComponent<AlphaAnimationScript>().enableBrightAnimation = false;
				playerObject.transform.Find("Avatar_Suit").GetComponent<AlphaAnimationScript>().enableAlphaAnimation = false;
				Transform transform = playerObject.transform.Find("Avatar_Cap");
				if (avatarType == AvatarType.Pastor)
				{
					if (transform != null)
					{
						transform.GetComponent<Renderer>().material.SetColor("_TintColor", value);
						transform.GetComponent<AlphaAnimationScript>().enableBrightAnimation = false;
						transform.GetComponent<AlphaAnimationScript>().enableAlphaAnimation = false;
					}
				}
				else if (transform != null)
				{
					transform.GetComponent<Renderer>().material.shader = Shader.Find("iPhone/SolidTexture");
					transform.GetComponent<Renderer>().material.SetColor("_TintColor", value);
					transform.GetComponent<AlphaAnimationScript>().enableBrightAnimation = false;
					transform.GetComponent<AlphaAnimationScript>().enableAlphaAnimation = false;
				}
			}
			Object.Destroy(powerObj);
			powerObj = null;
		}

		public virtual void OnRebirthStart()
		{
		}

		public virtual void OnRebirthStay(float time)
		{
		}

		public virtual void OnRebirthExit()
		{
		}

		public virtual void OnRebirthFinish()
		{
		}

		public virtual void UpdateNetworkTrans()
		{
		}

		public virtual void OnInjuredWithUser(User sender, float damage, int weapon_type)
		{
			if (HP > 0f)
			{
				Debug.Log("OnInjuredWithUser damage:" + damage + " come from id:" + sender.Id);
				OnHit(damage);
				if (HP <= 0f)
				{
					ISFSObject iSFSObject = new SFSObject();
					iSFSObject.PutBool("killed", true);
					List<User> list = new List<User>();
					list.Add(sender);
					smartFox.Send(new ObjectMessageRequest(iSFSObject, smartFox.LastJoinedRoom, list));
					ISFSObject iSFSObject2 = new SFSObject();
					iSFSObject2.PutBool("deaded", true);
					smartFox.Send(new ObjectMessageRequest(iSFSObject2, smartFox.LastJoinedRoom));
					GameVSScene gameVSScene = GameApp.GetInstance().GetGameScene() as GameVSScene;
					Multiplayer multiplayer = gameVSScene.SFS_Player_Arr[sender] as Multiplayer;
					smartFox.Send(new PublicMessageRequest(multiplayer.nick_name + " FRAGGED " + GameApp.GetInstance().GetGameState().nick_name));
					playerShell.OnDeadCameraChange(gameVSScene.SFS_Player_Arr[sender]);
				}
			}
		}

		public virtual void RefrashMasterKiller(bool is_master)
		{
		}
	}
}
