using System.Collections.Generic;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using UnityEngine;

namespace Zombie3D
{
	public class Multiplayer : Player
	{
		protected const float lerp_limit = 0.5f;

		public bool multiplayer_inited;

		protected Quaternion rot_to;

		protected Vector3 pos_to;

		protected float last_pos_check_time;

		public float net_ping;

		public bool m_is_lerp_position;

		protected float m_cur_lerp_time;

		protected Vector3 tem_pos_to;

		protected float total_offset_val;

		protected float cur_offset_val;

		public string nick_name = string.Empty;

		protected GameObject RebirthTimerEff;

		protected GameObject Multi_NickName;

		protected float rebirth_time = 10f;

		public bool is_rebirth_msg;

		public Multiplayer()
		{
			multiplayer_inited = false;
			m_is_lerp_position = false;
		}

		public override void Move(Vector3 motion)
		{
			if (charController != null)
			{
				charController.Move(motion);
				if (playerState == RUN_STATE || playerState == RUNSHOOT_STATE)
				{
					audioPlayer.PlayAudio("Walk");
				}
			}
		}

		public void InitAvatar(AvatarType type, uint birth_index)
		{
			avatarType = type;
			birth_point_index = birth_index;
		}

		public void InitWeaponList(int weapon1, int weapon2, int weapon3)
		{
			weaponList = new List<Weapon>();
			if (weapon1 != 9999)
			{
				weaponList.Add(GameApp.GetInstance().GetGameState().InitMultiWeapon(weapon1));
			}
			if (weapon2 != 9999)
			{
				weaponList.Add(GameApp.GetInstance().GetGameState().InitMultiWeapon(weapon2));
			}
			if (weapon3 != 9999)
			{
				weaponList.Add(GameApp.GetInstance().GetGameState().InitMultiWeapon(weapon3));
			}
		}

		public void InitWeaponList(int weapon1, float AttackFrequency1, int weapon2, float AttackFrequency2, int weapon3, float AttackFrequency3)
		{
			weaponList = new List<Weapon>();
			Weapon weapon4 = null;
			if (weapon1 != 9999)
			{
				weapon4 = GameApp.GetInstance().GetGameState().InitMultiWeapon(weapon1);
				weapon4.VsAttackFrenquency = AttackFrequency1;
				weaponList.Add(weapon4);
			}
			if (weapon2 != 9999)
			{
				weapon4 = GameApp.GetInstance().GetGameState().InitMultiWeapon(weapon2);
				weapon4.VsAttackFrenquency = AttackFrequency2;
				weaponList.Add(weapon4);
			}
			if (weapon3 != 9999)
			{
				weapon4 = GameApp.GetInstance().GetGameState().InitMultiWeapon(weapon3);
				weapon4.VsAttackFrenquency = AttackFrequency3;
				weaponList.Add(weapon4);
			}
		}

		public void NetInit()
		{
			IDLE_STATE = new MultiplayerIdleState();
			RUN_STATE = new MultiplayerRunState();
			SHOOT_STATE = new MultiplayerShootState();
			RUNSHOOT_STATE = new MultiplayerRunAndShootState();
			GOTHIT_STATE = new MultiplayerGotHitState();
			DEAD_STATE = new MultiplayerDeadState();
			points = GameObject.FindGameObjectsWithTag("WayPoint");
			playerConfig = GameApp.GetInstance().GetGameConfig().playerConf;
			int num = 0;
			hp = playerConfig.hp * (1f + (float)num * 0.5f);
			maxHp = hp;
			playerState = IDLE_STATE;
		}

		public void SetPlayerObj(GameObject obj, PlayerShell shell)
		{
			playerObject = obj;
			playerTransform = playerObject.transform;
			aimedTransform = playerTransform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head");
			charController = playerObject.GetComponent<CharacterController>();
			animation = playerObject.GetComponent<Animation>();
			audioPlayer = new AudioPlayer();
			Transform folderTrans = playerTransform.Find("Audio");
			audioPlayer.AddAudio(folderTrans, "GetItem", true);
			audioPlayer.AddAudio(folderTrans, "Dead", true);
			audioPlayer.AddAudio(folderTrans, "Switch", true);
			audioPlayer.AddAudio(folderTrans, "Walk", true);
			audioPlayer.AddAudio(folderTrans, "GetHp", true);
			audioPlayer.AddAudio(folderTrans, "GetBullet", true);
			walkSpeed = GameApp.GetInstance().GetGameConfig().playerConf.walkSpeed;
			playerShell = shell;
			ChangeToNormalState();
			UpdateNearestWayPoint();
			multiplayer_inited = true;
			collider = playerObject.GetComponent<Collider>();
		}

		public override void Init()
		{
			if (multiplayer_inited)
			{
				return;
			}
			IDLE_STATE = new MultiplayerIdleState();
			RUN_STATE = new MultiplayerRunState();
			SHOOT_STATE = new MultiplayerShootState();
			RUNSHOOT_STATE = new MultiplayerRunAndShootState();
			GOTHIT_STATE = new MultiplayerGotHitState();
			DEAD_STATE = new MultiplayerDeadState();
			net_com = GameApp.GetInstance().GetGameState().net_com;
			points = GameObject.FindGameObjectsWithTag("WayPoint");
			GameObject[] array = GameObject.FindGameObjectsWithTag("Respawn");
			GameObject gameObject = array[birth_point_index];
			respawnTrans = gameObject.transform;
			playerObject = AvatarFactory.GetInstance().CreateAvatar(avatarType);
			playerObject.transform.position = gameObject.transform.position;
			playerObject.transform.rotation = gameObject.transform.rotation;
			playerObject.name = "Multiplayer" + m_multi_id;
			playerTransform = playerObject.transform;
			aimedTransform = playerTransform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head");
			playerConfig = GameApp.GetInstance().GetGameConfig().playerConf;
			int armorLevel = GameApp.GetInstance().GetGameState().ArmorLevel;
			if (GameApp.GetInstance().GetGameState().endless_multiplayer)
			{
				GameUIScript.GetGameUIScript().AddMultiHpBar(m_multi_id, avatarType, nick_name, (int)birth_point_index);
			}
			hp = playerConfig.hp * (1f + (float)armorLevel * 0.5f);
			maxHp = hp;
			charController = playerObject.GetComponent<CharacterController>();
			animation = playerObject.GetComponent<Animation>();
			audioPlayer = new AudioPlayer();
			Transform folderTrans = playerTransform.Find("Audio");
			audioPlayer.AddAudio(folderTrans, "GetItem", true);
			audioPlayer.AddAudio(folderTrans, "Dead", true);
			audioPlayer.AddAudio(folderTrans, "Switch", true);
			audioPlayer.AddAudio(folderTrans, "Walk", true);
			audioPlayer.AddAudio(folderTrans, "GetHp", true);
			audioPlayer.AddAudio(folderTrans, "GetBullet", true);
			playerState = IDLE_STATE;
			foreach (Weapon weapon in weaponList)
			{
				weapon.WeaponPlayer = this;
				weapon.Init();
				weapon.VSReset();
			}
			foreach (Weapon weapon2 in weaponList)
			{
				if (weapon2.IsSelectedForBattle)
				{
					ChangeWeapon(weapon2);
					break;
				}
			}
			aimedTransform = playerTransform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head");
			walkSpeed = GameApp.GetInstance().GetGameConfig().playerConf.walkSpeed - cur_weapon.GetSpeedDrag();
			ChangeToNormalState();
			playerShell = playerObject.AddComponent<PlayerShell>();
			playerShell.m_player = this;
			UpdateNearestWayPoint();
			rot_to = playerTransform.rotation;
			pos_to = playerTransform.position;
			last_pos_check_time = Time.time;
			m_cur_lerp_time = Time.time;
			multiplayer_inited = true;
			AddNickNameMesh(nick_name);
			collider = playerObject.GetComponent<Collider>();
			if (GameApp.GetInstance().GetGameState().endless_multiplayer)
			{
				AddRebirthCom();
				GameMultiplayerScene gameMultiplayerScene = GameApp.GetInstance().GetGameScene() as GameMultiplayerScene;
				gameMultiplayerScene.game_tui.AddMultiplayerMark(this, (int)birth_point_index);
			}
			else if (GameApp.GetInstance().GetGameState().VS_mode)
			{
				smartFox = SmartFoxConnection.Connection;
				net_trans = NetworkTransform.FromTransform(playerTransform);
				playerObject.GetComponent<NetworkTransformInterpolation>().enabled = true;
				playerObject.GetComponent<NetworkTransformInterpolation>().StartReceiving();
			}
		}

		public void AddNickNameMesh(string name)
		{
			nick_name = name;
			Multi_NickName = Object.Instantiate(Resources.Load("Prefabs/TUI/MultiNickName")) as GameObject;
			Multi_NickName.transform.parent = playerObject.transform;
			Multi_NickName.transform.localPosition = new Vector3(0f, 0f, 0f);
			Multi_NickName.transform.position = new Vector3(Multi_NickName.transform.position.x, Multi_NickName.transform.position.y + 2f, Multi_NickName.transform.position.z);
			Multi_NickName.GetComponent<LabelSceneScript>().text = nick_name;
			Multi_NickName.GetComponent<LabelSceneScript>().color = ColorName.GetPlayerMarkColor((int)birth_point_index);
		}

		public override void RefrashMasterKiller(bool is_master)
		{
			if (is_master)
			{
				Multi_NickName.GetComponent<LabelSceneScript>().color = Color.red;
			}
			else
			{
				Multi_NickName.GetComponent<LabelSceneScript>().color = ColorName.GetPlayerMarkColor((int)birth_point_index);
			}
		}

		public override void OnVSRebirth()
		{
			OnRebirth();
			playerObject.GetComponent<NetworkTransformInterpolation>().StartReceiving();
		}

		public override bool AddVSWeapon(int type)
		{
			Weapon weapon = GameApp.GetInstance().GetGameState().InitMultiWeapon(type);
			weapon.WeaponPlayer = this;
			weapon.Init();
			weapon.VSReset();
			weapon.IsSelectedForBattle = true;
			weaponList.Add(weapon);
			return true;
		}

		public override void DoLogic(float deltaTime)
		{
			if (!multiplayer_inited || weaponList == null)
			{
				return;
			}
			if (guiHp != hp)
			{
				float num = Mathf.Abs(guiHp - hp);
				guiHp = Mathf.MoveTowards(guiHp, hp, num * 5f * deltaTime);
			}
			CheckMultiStatus();
			playerState.DoStateLogic(this, deltaTime);
			if (!GameApp.GetInstance().GetGameState().VS_mode)
			{
				playerTransform.rotation = Quaternion.Lerp(playerTransform.rotation, rot_to, Time.deltaTime * 10f);
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
			if (Time.time - lastUpdateNearestWayPointTime > 1f)
			{
				UpdateNearestWayPoint();
				lastUpdateNearestWayPointTime = Time.time;
			}
			if (is_rebirth_msg)
			{
				rebirth_time -= Time.deltaTime;
				if (rebirth_time <= 0f)
				{
					is_rebirth_msg = false;
					PlayerRealDead();
					rebirth_time = 0f;
				}
			}
		}

		public override void PlayerRealDead()
		{
			is_real_dead = true;
			SceneCheckOver();
		}

		public void CheckMultiStatus()
		{
			if (!GameApp.GetInstance().GetGameState().VS_mode && m_is_lerp_position)
			{
				Vector3 motion = Vector3.MoveTowards(Vector3.zero, pos_to - playerTransform.position, Time.deltaTime * base.WalkSpeed * 50f);
				charController.Move(motion);
				m_is_lerp_position = false;
			}
		}

		public void OnMultiSniperFire()
		{
			if (cur_weapon.GetWeaponType() == WeaponType.Sniper)
			{
				cur_weapon.Fire(0f);
			}
		}

		public new void SetStateWithType(PlayerStateType type)
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

		public override void SetState(PlayerState state)
		{
			playerState = state;
		}

		public void SetNetUserStatus(Vector3 direction, Vector3 rotation, Vector3 position, float ping)
		{
			m_direction = direction;
			if (!(playerTransform == null))
			{
				if (Vector3.Distance(playerTransform.position, position) >= base.WalkSpeed * 0.2f * 2f)
				{
					pos_to = position;
					m_is_lerp_position = true;
					total_offset_val = Vector3.Distance(playerTransform.position, pos_to);
					cur_offset_val = 0f;
				}
				else
				{
					m_is_lerp_position = false;
				}
				net_ping_sum = ping + net_com.m_netUserInfo.net_ping;
				rot_to = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
			}
		}

		public override void OnHit(float damage)
		{
			if (!is_god)
			{
				if (GameApp.GetInstance().GetGameState().multi_toturial_triger == 1)
				{
					hp -= damage;
					hp = (int)hp;
					hp = Mathf.Clamp(hp, 0f, maxHp);
				}
				playerState.OnHit(this, damage);
			}
		}

		public void OnMultiInjury(float damage, float max_hp, float cur_hp)
		{
			if (!(hp <= 0f))
			{
				maxHp = max_hp;
				hp = (int)cur_hp;
				hp = Mathf.Clamp(hp, 0f, maxHp);
				playerState.MultiOnHit(this, damage);
			}
		}

		public override void CreateScreenBlood(float damage)
		{
		}

		public override void OnDead()
		{
			audioPlayer.PlayAudio("Dead");
			cur_weapon.StopFire();
			int num = Random.Range(1, 4);
			Animate("Death0" + num, WrapMode.ClampForever);
			if (!GameApp.GetInstance().GetGameState().VS_mode)
			{
				playerObject.GetComponent<Collider>().enabled = false;
				rebirth_triger.enabled = true;
				playerObject.layer = 27;
				playerObject.GetComponent<PlayerRebirth>().CancelRebirth();
				GameApp.GetInstance().GetGameScene().OnMultiPlayerDead(this);
				GameUIScript.GetGameUIScript().SetMultiHelpAniStatus(m_multi_id, true);
				if (GameApp.GetInstance().GetGameState().multi_toturial_triger_game_rescue == 1)
				{
					GameMultiplayerScene gameMultiplayerScene = GameApp.GetInstance().GetGameScene() as GameMultiplayerScene;
					gameMultiplayerScene.game_tui.ShowTutorialMsgRescue(1);
				}
				is_rebirth_msg = true;
				rebirth_time = 10f;
			}
			else
			{
				playerObject.GetComponent<NetworkTransformInterpolation>().StopReceiving();
			}
		}

		public void ChangeWeaponWithindex(int index)
		{
			Debug.Log("On MultiPlayer change weapon : " + index);
			ChangeWeapon(weaponList[index]);
		}

		public override void ChangeWeapon(Weapon w)
		{
			if (w.IsSelectedForBattle || GameApp.GetInstance().GetGameState().VS_mode)
			{
				if (cur_weapon != null)
				{
					cur_weapon.GunOff();
					Animate("Idle01" + weaponNameEnd, WrapMode.Loop);
				}
				cur_weapon = w;
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
			}
		}

		public override bool OnPickUp(ItemType itemID)
		{
			switch (itemID)
			{
			case ItemType.Hp:
				GetHealed((int)maxHp);
				audioPlayer.PlayAudio("GetHp");
				break;
			case ItemType.Gold:
			{
				int num2 = (int)(300f * GameApp.GetInstance().GetGameScene().GetDifficultyCashDropFactor);
				GameApp.GetInstance().GetGameState().AddCashForRecord(num2);
				if (GameApp.GetInstance().GetGameState().endless_level)
				{
					GameApp.GetInstance().GetGameScene().endless_get_cash += num2;
				}
				audioPlayer.PlayAudio("GetItem");
				break;
			}
			case ItemType.Gold_Big:
			{
				int num = (int)(300f * GameApp.GetInstance().GetGameScene().GetDifficultyCashDropFactor * 5f);
				GameApp.GetInstance().GetGameState().AddCashForRecord(num);
				if (GameApp.GetInstance().GetGameState().endless_level)
				{
					GameApp.GetInstance().GetGameScene().endless_get_cash += num;
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
				goto default;
			case ItemType.ShotGun:
				foreach (Weapon weapon2 in weaponList)
				{
					if (weapon2.GetWeaponType() == WeaponType.ShotGun)
					{
						weapon2.AddBullets(weapon2.WConf.bullet / 4);
						break;
					}
				}
				goto default;
			case ItemType.RocketLauncer:
				foreach (Weapon weapon3 in weaponList)
				{
					if (weapon3.GetWeaponType() == WeaponType.RocketLauncher)
					{
						weapon3.AddBullets(weapon3.WConf.bullet / 4);
						break;
					}
				}
				goto default;
			case ItemType.LaserGun:
				foreach (Weapon weapon4 in weaponList)
				{
					if (weapon4.GetWeaponType() == WeaponType.LaserGun)
					{
						weapon4.AddBullets(weapon4.WConf.bullet / 4);
						break;
					}
				}
				goto default;
			case ItemType.Saw:
				foreach (Weapon weapon5 in weaponList)
				{
					if (weapon5.GetWeaponType() == WeaponType.Saw)
					{
						weapon5.AddBullets(weapon5.WConf.bullet / 4);
						break;
					}
				}
				goto default;
			case ItemType.Sniper:
				foreach (Weapon weapon6 in weaponList)
				{
					if (weapon6.GetWeaponType() == WeaponType.Sniper)
					{
						weapon6.AddBullets(weapon6.WConf.bullet / 4);
						break;
					}
				}
				goto default;
			case ItemType.MachineGun:
				foreach (Weapon weapon7 in weaponList)
				{
					if (weapon7.GetWeaponType() == WeaponType.MachineGun)
					{
						weapon7.AddBullets(weapon7.WConf.bullet / 4);
						break;
					}
				}
				goto default;
			case ItemType.M32:
				foreach (Weapon weapon8 in weaponList)
				{
					if (weapon8.GetWeaponType() == WeaponType.M32)
					{
						weapon8.AddBullets(weapon8.WConf.bullet / 4);
						break;
					}
				}
				goto default;
			case ItemType.Fire:
				foreach (Weapon weapon9 in weaponList)
				{
					if (weapon9.GetWeaponType() == WeaponType.FireGun)
					{
						weapon9.AddBullets(weapon9.WConf.bullet / 4);
						break;
					}
				}
				goto default;
			default:
				audioPlayer.PlayAudio("GetBullet");
				break;
			}
			return true;
		}

		public override void OnRebirth()
		{
			Debug.Log("Multipalyer OnRebirth...");
			playerObject.GetComponent<Collider>().enabled = true;
			if (!GameApp.GetInstance().GetGameState().VS_mode)
			{
				rebirth_triger.enabled = false;
				GameUIScript.GetGameUIScript().SetMultiHelpAniStatus(m_multi_id, false);
			}
			playerObject.layer = 8;
			GetHealed((int)maxHp);
			ChangeToGodBuffState();
			if (GameApp.GetInstance().GetGameState().endless_multiplayer)
			{
				GameMultiplayerScene gameMultiplayerScene = GameApp.GetInstance().GetGameScene() as GameMultiplayerScene;
				gameMultiplayerScene.m_multi_player_arr.Add(this);
				gameMultiplayerScene.ResetEnemyTarget();
				is_real_dead = false;
				is_rebirth_msg = false;
			}
			if (RebirthTimerEff != null)
			{
				Object.Destroy(RebirthTimerEff);
				RebirthTimerEff = null;
			}
		}

		public override void OnRebirthStart()
		{
			Debug.Log("Multiplayer OnRebirthStart id:" + m_multi_id);
			RebirthTimerEff = Object.Instantiate(Resources.Load("Prefabs/TUI/RebirthTimerEff")) as GameObject;
			RebirthTimerEff.transform.parent = playerObject.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand/Weapon_Dummy");
			RebirthTimerEff.transform.localPosition = new Vector3(0f, 0f, 0f);
			RebirthTimerEff.transform.position = new Vector3(RebirthTimerEff.transform.position.x, RebirthTimerEff.transform.position.y + 2f, RebirthTimerEff.transform.position.z);
			RebirthTimerEff.GetComponent<ClipMeshEffScript>().StartClip();
		}

		public override void OnRebirthStay(float time)
		{
			if (RebirthTimerEff != null)
			{
				RebirthTimerEff.GetComponent<ClipMeshEffScript>().UpdateMesh(time);
			}
		}

		public override void OnRebirthExit()
		{
			if (RebirthTimerEff != null)
			{
				Object.Destroy(RebirthTimerEff);
				RebirthTimerEff = null;
			}
		}

		public override void OnRebirthFinish()
		{
			if (RebirthTimerEff != null)
			{
				Object.Destroy(RebirthTimerEff);
				RebirthTimerEff = null;
			}
			if (GameApp.GetInstance().GetGameState().multi_toturial_triger == 1)
			{
				OnRebirth();
				playerState = IDLE_STATE;
				GameMultiplayerScene gameMultiplayerScene = GameApp.GetInstance().GetGameScene() as GameMultiplayerScene;
				gameMultiplayerScene.Multi_Tutorial_Triger.GetComponent<MultiTutorialTriger>().isEnd = true;
			}
		}

		public override void UpdateNetworkTrans()
		{
			playerObject.GetComponent<NetworkTransformInterpolation>().ReceivedTransform(NetworkTransform.Clone(net_trans));
		}

		public override void OnVsInjured(User sender, float damage, int weapon_type)
		{
			if (weapon_type == 10)
			{
				GameVSScene gameVSScene = GameApp.GetInstance().GetGameScene() as GameVSScene;
				Vector3 vector = gameVSScene.SFS_Player_Arr[sender].GetTransform().position - GetTransform().position;
				vector.Normalize();
				Object.Instantiate(GameApp.GetInstance().GetResourceConfig().swordAttack, GetTransform().position + Vector3.up * 1.2f + vector * 0.5f, Quaternion.identity);
			}
			Object.Instantiate(GameApp.GetInstance().GetResourceConfig().hitBlood, GetTransform().position + Vector3.up * 1f, Quaternion.identity);
			ISFSObject iSFSObject = new SFSObject();
			iSFSObject.PutFloat("damageVal", damage);
			iSFSObject.PutInt("weaponType", weapon_type);
			ISFSObject iSFSObject2 = new SFSObject();
			iSFSObject2.PutSFSObject("damage", iSFSObject);
			List<User> list = new List<User>();
			list.Add(sfs_user);
			smartFox.Send(new ObjectMessageRequest(iSFSObject2, smartFox.LastJoinedRoom, list));
		}

		public void MultiplayerSniperFire(Vector3 target)
		{
			MultiSniper multiSniper = GetWeapon() as MultiSniper;
			if (multiSniper != null)
			{
				multiSniper.AddMultiTarget(target);
				OnMultiSniperFire();
			}
		}
	}
}
