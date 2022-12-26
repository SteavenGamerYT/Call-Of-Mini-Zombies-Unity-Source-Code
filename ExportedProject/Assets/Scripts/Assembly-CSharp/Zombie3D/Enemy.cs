using System.Collections.Generic;
using UnityEngine;

namespace Zombie3D
{
	public abstract class Enemy
	{
		public const float net_status_rate = 0.2f;

		public const int multimode_val = 3;

		protected const float ReCheckFindPath_time = 5f;

		public static EnemyState GRAVEBORN_STATE = new GraveBornState();

		public static EnemyState PREYGONE_STATE = new PreyGoneState();

		public static EnemyState IDLE_STATE = new IdleState();

		public static EnemyState CATCHING_STATE = new CatchingState();

		public static EnemyState GOTHIT_STATE = new GotHitState();

		public static EnemyState PATROL_STATE = new PatrolState();

		public static EnemyState ATTACK_STATE = new AttackState();

		public static EnemyState DEAD_STATE = new DeadState();

		protected GameObject enemyObject;

		protected Transform enemyTransform;

		protected Animation animation;

		protected Rigidbody rigidbody;

		protected Transform aimedTransform;

		protected Transform target;

		protected Vector3 spawnCenter;

		protected Vector3 patrolTarget;

		protected GameObject m_enemy_mark;

		protected Collider collider;

		protected ResourceConfigScript rConfig;

		protected EnemyConfigScript _EnemyResourceConfig;

		protected GameConfig gConfig;

		protected EnemyType enemyType;

		protected Vector3 lastTarget;

		protected GameScene gameScene;

		protected Player player;

		protected CharacterController controller;

		protected Vector3 dir;

		protected AudioPlayer audio;

		protected IPathFinding pathFinding;

		public int m_enemy_id;

		protected float hp;

		protected float runSpeed;

		protected bool beWokeUp;

		protected float deadTime;

		public float last_enemy_status_time;

		protected Quaternion rot_to;

		protected Vector3 pos_to;

		protected bool m_is_lerp_position;

		protected float runSlowTime;

		protected bool is_RunSlow;

		protected string name;

		protected bool visible;

		protected ObjectPool hitBloodObjectPool;

		protected bool moveWithCharacterController;

		protected float lastUpdateTime;

		protected float lastPathFindingTime;

		protected float lastSearchPathTime;

		protected float lastReCheckPathTime;

		protected EnemyState state;

		protected float attackRange;

		protected float detectionRange;

		protected float minRange;

		protected float attackFrequency;

		protected float attackDamage;

		protected float idlePeriod = 1.5f;

		protected float aiRadius = 100f;

		protected int score;

		protected int lootCash;

		protected float gotHitTime;

		protected float idleStartTime;

		protected float lastAttackTime = -100f;

		protected float lookAroundStartTime;

		protected int nextPoint = -1;

		protected string runAnimationName = "Run";

		protected GameObject targetObj;

		protected float onhitRate = 100f;

		protected bool criticalAttacked;

		protected bool attacked;

		protected Quaternion deadRotation;

		protected Vector3 deadPosition;

		protected Vector3[] path;

		protected float m_tip_height;

		public Ray ray;

		public RaycastHit rayhit;

		public Ray ray_tem;

		public RaycastHit rayhit_tem;

		public float lastStateTime;

		public bool m_isBoss;

		public bool m_isPrey;

		protected int m_multiplayer_index;

		protected bool is_multi_dead;

		public float lastFireDamagedTime;

		protected GameObject PreyTip;

		protected Vector3 last_pos = Vector3.zero;

		protected Vector3 last_rot = Vector3.zero;

		protected Vector3 last_dir = Vector3.zero;

		public bool IsElite { get; set; }

		public AudioPlayer Audio
		{
			get
			{
				return audio;
			}
		}

		public string RunAnimationName
		{
			get
			{
				return runAnimationName;
			}
		}

		public bool MoveWithCharacterController
		{
			get
			{
				return moveWithCharacterController;
			}
			set
			{
				moveWithCharacterController = value;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		public float HP
		{
			get
			{
				return hp;
			}
		}

		public float DetectionRange
		{
			get
			{
				return detectionRange;
			}
		}

		public float AttackRange
		{
			get
			{
				return attackRange;
			}
		}

		public EnemyType EnemyType
		{
			get
			{
				return enemyType;
			}
			set
			{
				enemyType = value;
			}
		}

		public Vector3 LastTarget
		{
			get
			{
				return lastTarget;
			}
		}

		public Player TargetPlayer
		{
			get
			{
				return player;
			}
			set
			{
				player = value;
				target = player.GetTransform();
				lastTarget = target.position;
			}
		}

		public float SqrDistanceFromPlayer
		{
			get
			{
				return (player.GetTransform().position - enemyTransform.position).sqrMagnitude;
			}
		}

		public Transform GetTransform()
		{
			return enemyTransform;
		}

		public bool CouldMakeNextAttack()
		{
			if (Time.time - lastAttackTime >= attackFrequency)
			{
				return true;
			}
			return false;
		}

		public virtual bool CouldEnterAttackState()
		{
			if (SqrDistanceFromPlayer < AttackRange * AttackRange)
			{
				return true;
			}
			return false;
		}

		public void SetPlayer(Player mPlayer)
		{
			player = mPlayer;
			if (mPlayer != null)
			{
				target = player.GetTransform();
				lastTarget = target.position;
				Debug.Log("get enemy:" + name + "change target plyer:" + player.m_multi_id);
			}
			else
			{
				Debug.Log("enemy:" + m_enemy_id + "change target player(null).");
				target = null;
			}
		}

		public bool IsAnimationPlayedPercentage(string aniName, float percentage)
		{
			if (animation[aniName].time >= animation[aniName].clip.length * percentage)
			{
				return true;
			}
			return false;
		}

		public virtual bool AttackAnimationEnds()
		{
			if (Time.time - lastAttackTime > enemyObject.GetComponent<Animation>()["Attack01"].length)
			{
				return true;
			}
			return false;
		}

		public virtual bool GotHitAnimationEnds()
		{
			if (Time.time - gotHitTime >= animation["Damage"].clip.length)
			{
				return true;
			}
			return false;
		}

		public virtual void Animate(string animationName, WrapMode wrapMode)
		{
			animation[animationName].wrapMode = wrapMode;
			if (!animation.IsPlaying("Damage"))
			{
				if (wrapMode == WrapMode.Loop || (!animation.IsPlaying(animationName) && animationName != "Damage"))
				{
					animation.CrossFade(animationName);
					return;
				}
				animation.Stop();
				animation.Play(animationName);
			}
		}

		public void SetInGrave(bool inGrave)
		{
			if (inGrave)
			{
				SetState(GRAVEBORN_STATE);
				enemyTransform.Translate(Vector3.down * 2f);
				enemyObject.layer = 13;
				Object.DestroyImmediate(enemyObject.GetComponent<Rigidbody>());
				enemyObject.AddComponent<Rigidbody>();
				enemyTransform.GetComponent<Rigidbody>().freezeRotation = true;
				enemyTransform.GetComponent<Rigidbody>().useGravity = false;
			}
			else
			{
				enemyObject.layer = 9;
				enemyTransform.GetComponent<Rigidbody>().useGravity = true;
			}
		}

		public void SetInPreyGone(bool state)
		{
			if (state)
			{
				SetState(PREYGONE_STATE);
				enemyObject.layer = 13;
				Object.DestroyImmediate(enemyObject.GetComponent<Rigidbody>());
				enemyObject.AddComponent<Rigidbody>();
				enemyTransform.GetComponent<Rigidbody>().freezeRotation = true;
				enemyTransform.GetComponent<Rigidbody>().useGravity = false;
				enemyTransform.GetComponent<Rigidbody>().isKinematic = true;
			}
			else
			{
				gameScene.GetEnemies().Remove(enemyObject.name);
				enemyObject.SetActiveRecursively(false);
				Object.Destroy(PreyTip);
			}
		}

		public bool MoveFromGrave(float deltaTime)
		{
			enemyTransform.Translate(Vector3.up * deltaTime * 2f);
			if (enemyTransform.position.y >= 10000.1f)
			{
				return true;
			}
			return false;
		}

		public bool MoveToMucilage(float deltaTime)
		{
			enemyTransform.Translate(Vector3.down * deltaTime * 2f);
			float num = 2f;
			if (enemyType == EnemyType.E_TANK)
			{
				num = 4f;
			}
			if (enemyTransform.position.y <= 10000.1f - num)
			{
				return true;
			}
			return false;
		}

		public void SetTargetWithMultiplayer()
		{
			if (GameApp.GetInstance().GetGameState().endless_multiplayer && GameApp.GetInstance().GetGameState().net_com.m_netUserInfo.is_master)
			{
				target = GetRandomMultiplayer();
			}
		}

		public Transform GetRandomMultiplayer()
		{
			int num = Random.Range(0, GameApp.GetInstance().GetGameScene().m_multi_player_arr.Count);
			int num2 = 0;
			foreach (Player item in GameApp.GetInstance().GetGameScene().m_multi_player_arr)
			{
				if (num2 == num)
				{
					player = item;
					return player.GetTransform();
				}
				num2++;
			}
			return null;
		}

		public virtual void Init(GameObject gObject)
		{
			gameScene = GameApp.GetInstance().GetGameScene();
			player = gameScene.GetPlayer();
			target = player.GetTransform();
			enemyObject = gObject;
			enemyTransform = enemyObject.transform;
			animation = enemyObject.GetComponent<Animation>();
			aimedTransform = enemyTransform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head");
			rigidbody = enemyObject.GetComponent<Rigidbody>();
			collider = enemyObject.transform.GetComponent<Collider>();
			rConfig = GameApp.GetInstance().GetResourceConfig();
			_EnemyResourceConfig = GameApp.GetInstance().GetEnemyResourceConfig();
			gConfig = GameApp.GetInstance().GetGameConfig();
			controller = enemyObject.GetComponent<Collider>() as CharacterController;
			detectionRange = 150f;
			attackRange = 1.5f;
			minRange = 1.5f;
			lootCash = 0;
			criticalAttacked = false;
			spawnCenter = enemyTransform.position;
			SetTargetWithMultiplayer();
			audio = new AudioPlayer();
			Transform folderTrans = enemyTransform.Find("Audio");
			audio.AddAudio(folderTrans, "Attack", true);
			audio.AddAudio(folderTrans, "Walk", true);
			audio.AddAudio(folderTrans, "Dead", true);
			audio.AddAudio(folderTrans, "Special", true);
			audio.AddAudio(folderTrans, "Shout", true);
			hitBloodObjectPool = GameApp.GetInstance().GetGameScene().HitBloodObjectPool;
			pathFinding = new GraphPathFinding();
			pathFinding.InitPath(gameScene.scene_points);
			animation.wrapMode = WrapMode.Loop;
			animation.Play("Idle01");
			state = IDLE_STATE;
			lastUpdateTime = Time.time;
			lastPathFindingTime = Time.time;
			idleStartTime = -2f;
			path = GameApp.GetInstance().GetGameScene().GetPath();
			if (GameApp.GetInstance().GetGameState().endless_level)
			{
				LootManagerScript component = enemyObject.GetComponent<LootManagerScript>();
				component.dropRate *= 0.5f;
			}
			rot_to = Quaternion.Euler(enemyTransform.rotation.x, enemyTransform.rotation.y, enemyTransform.rotation.z);
			CreatePreyTip();
			InitEnemyMark();
		}

		public void InitEnemyMark()
		{
			if (GameApp.GetInstance().GetGameState().endless_multiplayer)
			{
				m_enemy_mark = Object.Instantiate(Resources.Load("Prefabs/TUI/EnemyMark")) as GameObject;
				GameMultiplayerScene gameMultiplayerScene = GameApp.GetInstance().GetGameScene() as GameMultiplayerScene;
				m_enemy_mark.transform.parent = gameMultiplayerScene.game_tui.map_show.transform;
				m_enemy_mark.transform.localPosition = new Vector3(0f, 0f, -1f);
				m_enemy_mark.GetComponent<EnemyMark>().m_enemy = this;
				m_enemy_mark.GetComponent<EnemyMark>().SceneTUI = gameMultiplayerScene.game_tui.gameObject;
			}
		}

		public void RemoveEnemyMark()
		{
			if (m_enemy_mark != null)
			{
				Object.Destroy(m_enemy_mark);
			}
		}

		public void SetState(EnemyState newState)
		{
			state = newState;
		}

		public EnemyState GetState()
		{
			return state;
		}

		public virtual void CheckHit()
		{
			if (GameApp.GetInstance().GetGameState().endless_multiplayer && GameApp.GetInstance().GetGameState().net_com.m_netUserInfo.is_master && player.GetPlayerState() == player.DEAD_STATE)
			{
				SetTargetWithMultiplayer();
			}
		}

		public Transform GetAimedTransform()
		{
			return aimedTransform;
		}

		public Vector3 GetPosition()
		{
			return enemyTransform.position;
		}

		public Collider GetCollider()
		{
			return collider;
		}

		public virtual void OnHit(DamageProperty dp, WeaponType weaponType, bool criticalAttack, Player mPlayer)
		{
			if (state == GRAVEBORN_STATE)
			{
				return;
			}
			beWokeUp = true;
			if (mPlayer != null)
			{
				if (mPlayer.GetAvatarType() == AvatarType.Marine)
				{
					dp.damage *= 1.2f;
				}
				else if (mPlayer.GetAvatarType() == AvatarType.EnegyArmor)
				{
					dp.damage *= 2f;
				}
				else if (mPlayer.GetAvatarType() == AvatarType.Pirate)
				{
					if (weaponType == WeaponType.Sword || weaponType == WeaponType.Saw)
					{
						dp.damage *= 1.2f;
					}
				}
				else if (mPlayer.GetAvatarType() == AvatarType.Pastor)
				{
					OnPastorAffect();
				}
				if (weaponType == WeaponType.Sword)
				{
					Vector3 vector = mPlayer.GetTransform().position - enemyTransform.position;
					vector.Normalize();
					Object.Instantiate(rConfig.swordAttack, enemyTransform.position + Vector3.up * 1.2f + vector * 0.5f, Quaternion.identity);
				}
			}
			Object.Instantiate(rConfig.hitBlood, enemyTransform.position + Vector3.up * 1f, Quaternion.identity);
			gotHitTime = Time.time;
			hp -= dp.damage;
			criticalAttacked = criticalAttack;
			state.OnHit(this);
			if (GameApp.GetInstance().GetGameState().endless_multiplayer && dp.damage > 0f)
			{
				int criticalAttack2 = (criticalAttack ? 1 : 0);
				Packet packet = CGEnemyGotHitPacket.MakePacket(name, (long)(dp.damage * 1000f), (uint)weaponType, (uint)criticalAttack2);
				GameApp.GetInstance().GetGameState().net_com.Send(packet);
				GameMultiplayerScene gameMultiplayerScene = GameApp.GetInstance().GetGameScene() as GameMultiplayerScene;
				gameMultiplayerScene.player_injured_val += dp.damage;
				if (hp <= 0f && mPlayer != null && !is_multi_dead)
				{
					int bElite = (IsElite ? 1 : 0);
					Packet packet2 = CGEnemyDeadPacket.MakePacket((uint)mPlayer.m_multi_id, name, (uint)enemyType, (uint)bElite, (uint)weaponType);
					GameApp.GetInstance().GetGameState().net_com.Send(packet2);
					is_multi_dead = true;
				}
				else if (hp <= 0f && mPlayer == null && !is_multi_dead)
				{
					Packet packet3 = CGEnemyRemovePacket.MakePacket(name);
					GameApp.GetInstance().GetGameState().net_com.Send(packet3);
					is_multi_dead = true;
				}
			}
		}

		public virtual void OnMultiHit(float dp, WeaponType weaponType, int criticalAttack)
		{
			if (state != GRAVEBORN_STATE)
			{
				beWokeUp = true;
				Object.Instantiate(rConfig.hitBlood, enemyTransform.position + Vector3.up * 1f, Quaternion.identity);
				gotHitTime = Time.time;
				hp -= dp;
				criticalAttacked = criticalAttack == 1;
				state.OnHit(this);
			}
		}

		public void OnPastorAffect()
		{
			if (!is_RunSlow)
			{
				runSpeed *= 0.5f;
				animation[runAnimationName].speed *= 0.5f;
			}
			is_RunSlow = true;
			runSlowTime = Time.time;
		}

		public virtual void OnAttack()
		{
			audio.PlayAudio("Attack");
		}

		public virtual void PlayDeadEffects()
		{
			if ((bool)enemyObject && enemyObject.active)
			{
				PlayBloodEffect();
				PlayBodyExlodeEffect();
				GameObject original = _EnemyResourceConfig.deadhead[(int)EnemyType];
				GameObject gameObject = Object.Instantiate(original, enemyTransform.position + new Vector3(0f, 2f, 0f), enemyTransform.rotation);
				gameObject.GetComponent<Rigidbody>().AddForce(Random.Range(-5, 5), Random.Range(-5, 0), Random.Range(-5, 5), ForceMode.Impulse);
				gameScene.GetEnemies().Remove(enemyObject.name);
				enemyObject.SetActiveRecursively(false);
			}
		}

		public virtual void PlayBloodEffect()
		{
			if ((bool)enemyObject && enemyObject.active)
			{
				GameObject deadBlood = rConfig.deadBlood;
				int num = Random.Range(0, 100);
				float y = 10000.119f;
				GameObject original;
				if (num > 50)
				{
					original = rConfig.deadFoorblood;
				}
				else
				{
					original = rConfig.deadFoorblood2;
					y = 10000.109f;
				}
				Object.Instantiate(deadBlood, enemyTransform.position + new Vector3(0f, 0.5f, 0f), Quaternion.Euler(0f, 0f, 0f));
				GameObject gameObject = Object.Instantiate(original, new Vector3(enemyTransform.position.x, y, enemyTransform.position.z), Quaternion.Euler(270f, 0f, 0f));
				gameObject.transform.rotation = deadRotation * gameObject.transform.rotation;
				gameObject.transform.position = deadPosition;
			}
		}

		public void PlayBloodExplodeEffect(Vector3 pos)
		{
			GameObject deadBlood = rConfig.deadBlood;
			Object.Instantiate(deadBlood, pos, Quaternion.Euler(0f, 0f, 0f));
		}

		public virtual void PlayBodyExlodeEffect()
		{
			if ((bool)enemyObject && enemyObject.active)
			{
				Quaternion rotation = Quaternion.Euler(enemyTransform.rotation.eulerAngles.x, Random.Range(0, 360), enemyTransform.rotation.eulerAngles.z);
				ObjectPool deadBodyPool = GameApp.GetInstance().GetGameScene().GetDeadBodyPool(enemyType);
				GameObject gameObject = deadBodyPool.CreateObject(enemyTransform.position + new Vector3(0f, 0.2f, 0f), rotation);
				gameObject.transform.rotation = deadRotation * gameObject.transform.rotation;
				AudioSource component = gameObject.GetComponent<AudioSource>();
				if (component != null)
				{
					component.mute = !GameApp.GetInstance().GetGameState().SoundOn;
				}
			}
		}

		public virtual void FindPath()
		{
			if ((GameApp.GetInstance().GetGameState().endless_multiplayer && !GameApp.GetInstance().GetGameState().net_com.m_netUserInfo.is_master) || target == null)
			{
				return;
			}
			Vector3 position = target.position;
			if (!(Time.time - lastPathFindingTime > 0.25f))
			{
				return;
			}
			lastPathFindingTime = Time.time;
			position.y = enemyTransform.position.y;
			if (lastTarget == Vector3.zero)
			{
				lastTarget = target.position;
			}
			ray = new Ray(enemyTransform.position + new Vector3(0f, 0.5f, 0f), target.position + new Vector3(0f, 0.5f, 0f) - (enemyTransform.position + new Vector3(0f, 0.5f, 0f)));
			if (Physics.Raycast(ray, out rayhit, 100f, 67217664))
			{
				if (rayhit.collider.gameObject.tag == "Player" && Mathf.Abs(enemyTransform.position.y - player.GetTransform().position.y) < 0.5f)
				{
					PlayerShell component = rayhit.collider.gameObject.GetComponent<PlayerShell>();
					if (component != null)
					{
						target = rayhit.collider.gameObject.transform;
						lastTarget = target.position;
						if (component.m_player.m_multi_id != player.m_multi_id)
						{
							player = component.m_player;
							if (GameApp.GetInstance().GetGameState().endless_multiplayer)
							{
								Packet packet = CGEnemyChangeTargetPacket.MakePacket(name, (uint)player.m_multi_id);
								GameApp.GetInstance().GetGameState().net_com.Send(packet);
							}
						}
						pathFinding.ClearPath();
					}
				}
				else if (Time.time - lastReCheckPathTime > 5f)
				{
					if (GameApp.GetInstance().GetGameState().endless_multiplayer)
					{
						if (!MultiplayerDistanceChack())
						{
							ray = new Ray(enemyTransform.position + new Vector3(0f, 0.5f, 0f), lastTarget - (enemyTransform.position + new Vector3(0f, 0.5f, 0f)));
							if (Physics.Raycast(ray, out rayhit, 100f, 67584))
							{
								pathFinding.ClearPath();
								Transform nextWayPoint = pathFinding.GetNextWayPoint(enemyTransform.position, player.GetTransform().position);
								if (nextWayPoint != null)
								{
									lastTarget = nextWayPoint.position;
								}
							}
						}
					}
					else
					{
						ray = new Ray(enemyTransform.position + new Vector3(0f, 0.5f, 0f), lastTarget - (enemyTransform.position + new Vector3(0f, 0.5f, 0f)));
						if (Physics.Raycast(ray, out rayhit, 100f, 67584))
						{
							pathFinding.ClearPath();
							Transform nextWayPoint2 = pathFinding.GetNextWayPoint(enemyTransform.position, player.GetTransform().position);
							if (nextWayPoint2 != null)
							{
								lastTarget = nextWayPoint2.position;
							}
						}
					}
					lastReCheckPathTime = Time.time;
				}
			}
			if ((enemyTransform.position - lastTarget).sqrMagnitude < 1f)
			{
				pathFinding.PopNode();
				Transform nextWayPoint3 = pathFinding.GetNextWayPoint(enemyTransform.position, player.GetTransform().position);
				if (nextWayPoint3 != null)
				{
					lastTarget = nextWayPoint3.position;
				}
			}
			enemyTransform.LookAt(new Vector3(lastTarget.x, enemyTransform.position.y, lastTarget.z));
			dir = (lastTarget - enemyTransform.position).normalized;
		}

		public bool MultiplayerDistanceChack()
		{
			int multi_id = this.player.m_multi_id;
			if (GameApp.GetInstance().GetGameState().endless_multiplayer && GameApp.GetInstance().GetGameState().net_com.m_netUserInfo.is_master)
			{
				float num = Vector3.SqrMagnitude(this.player.GetTransform().position - enemyTransform.position);
				Player player = null;
				foreach (Player item in GameApp.GetInstance().GetGameScene().m_multi_player_arr)
				{
					if (Vector3.SqrMagnitude(item.GetTransform().position - enemyTransform.position) < num && item.m_multi_id != this.player.m_multi_id)
					{
						ray_tem = new Ray(enemyTransform.position + new Vector3(0f, 0.5f, 0f), item.GetTransform().position + new Vector3(0f, 0.5f, 0f) - enemyTransform.position + new Vector3(0f, 0.5f, 0f));
						if (Physics.Raycast(ray_tem, out rayhit_tem, 100f, 67217664) && rayhit_tem.collider.gameObject.tag == "Player" && Mathf.Abs(enemyTransform.position.y - rayhit_tem.collider.gameObject.transform.position.y) < 0.5f)
						{
							num = Vector3.SqrMagnitude(item.GetTransform().position - enemyTransform.position);
							player = item;
						}
					}
				}
				if (player != null)
				{
					target = player.GetTransform();
					lastTarget = target.position;
					this.player = player;
					pathFinding.ClearPath();
					Packet packet = CGEnemyChangeTargetPacket.MakePacket(name, (uint)this.player.m_multi_id);
					GameApp.GetInstance().GetGameState().net_com.Send(packet);
					return true;
				}
			}
			return false;
		}

		public virtual void Patrol(float deltaTime)
		{
		}

		public void ResetRunSpeedTimer()
		{
			if (is_RunSlow && Time.time - runSlowTime > 3f)
			{
				runSpeed /= 0.5f;
				animation[runAnimationName].speed /= 0.5f;
				is_RunSlow = false;
			}
		}

		public void RemoveDeadBodyTimer()
		{
			if (Time.time - deadTime > 3f)
			{
				gameScene.GetEnemies().Remove(enemyObject.name);
				enemyObject.SetActiveRecursively(false);
			}
		}

		public void RemoveEnemyNow()
		{
			GameObject mucilage_M = GameApp.GetInstance().GetGameResourceConfig().mucilage_M;
			GameObject gameObject = Object.Instantiate(mucilage_M, new Vector3(enemyTransform.position.x, enemyTransform.position.y + 0.1f, enemyTransform.position.z), Quaternion.identity);
			SetInPreyGone(true);
		}

		public virtual void OnDead()
		{
			deadTime = Time.time;
			gameScene.IncreaseKills();
			gameScene.ModifyEnemyNum(-1);
			GameApp.GetInstance().GetGameState().Achievement.KillEnemy();
			GameApp.GetInstance().GetGameState().AddCashForRecord((int)((float)lootCash * gameScene.GetDifficultyCashDropFactor));
			if (GameApp.GetInstance().GetGameState().endless_level)
			{
				GameApp.GetInstance().GetGameScene().endless_get_cash += (int)((float)lootCash * gameScene.GetDifficultyCashDropFactor);
			}
			if (GameApp.GetInstance().GetGameState().endless_multiplayer && is_multi_dead)
			{
				enemyObject.SendMessage("OnLoot", m_isPrey);
			}
			else if (!GameApp.GetInstance().GetGameState().endless_multiplayer)
			{
				enemyObject.SendMessage("OnLoot", m_isPrey);
			}
			if (IsElite && enemyType != EnemyType.E_BOOMER)
			{
				criticalAttacked = false;
			}
			if (enemyType == EnemyType.E_DOG || enemyType == EnemyType.E_HELL_FIRER)
			{
				criticalAttacked = false;
			}
			deadRotation = Quaternion.identity;
			deadPosition = enemyTransform.position;
			deadPosition.y = 10000.119f;
			if (enemyTransform.position.y > 10000.6f)
			{
				Ray ray = new Ray(enemyTransform.position + Vector3.up * 0.5f, -Vector3.up);
				RaycastHit hitInfo;
				if (Physics.Raycast(ray, out hitInfo, 50f, 32768))
				{
					deadRotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
					deadPosition = hitInfo.point + Vector3.up * 0.01f;
				}
			}
			if (criticalAttacked)
			{
				PlayDeadEffects();
			}
			else
			{
				if ((bool)animation)
				{
					animation["Death01"].wrapMode = WrapMode.ClampForever;
					animation["Death01"].speed = 1f;
					animation.CrossFade("Death01");
				}
				if ((bool)enemyObject && enemyObject.active)
				{
					enemyTransform.rotation = deadRotation * enemyTransform.rotation;
					enemyObject.layer = 18;
				}
				PlayBloodEffect();
			}
			CheckPreyEnemyDeath();
			RemoveEnemyMark();
		}

		public void CheckPreyEnemyDeath()
		{
			if (!m_isPrey)
			{
				return;
			}
			if (GameApp.GetInstance().GetGameState().Hunting_val == 0)
			{
				List<Weapon> weapons = GameApp.GetInstance().GetGameState().GetWeapons();
				WeaponConfig weaponConfig = gConfig.GetWeaponConfig("HellFire");
				if (weaponConfig != null)
				{
					foreach (Weapon item in weapons)
					{
						if (item.Name == weaponConfig.name)
						{
							if (item.Exist == WeaponExistState.Locked)
							{
								item.Exist = WeaponExistState.Unlocked;
							}
							GameApp.GetInstance().GetGameScene().BonusWeapon = item;
							GameUIScript.GetGameUIScript().GetPanel(2).Show();
							break;
						}
					}
				}
			}
			GameApp.GetInstance().GetGameState().Hunting_val = 1;
			GameApp.GetInstance().PlayerPrefsSave();
			GameUIScript.GetGameUIScript().DeleteHuntingTimeText();
			Object.Destroy(PreyTip);
			PreyTip = null;
		}

		public void CreatePreyTip()
		{
			if (m_isPrey)
			{
				GameObject preyTip = GameApp.GetInstance().GetResourceConfig().PreyTip;
				if (PreyTip == null)
				{
					PreyTip = Object.Instantiate(preyTip, enemyTransform.TransformPoint(Vector3.up * m_tip_height), Quaternion.identity);
					PreyTip.transform.parent = enemyTransform;
				}
			}
		}

		public virtual bool OnSpecialState(float deltaTime)
		{
			return false;
		}

		public virtual EnemyState EnterSpecialState(float deltaTime)
		{
			return null;
		}

		public virtual void DoMove(float deltaTime)
		{
			enemyTransform.Translate(dir * runSpeed * deltaTime, Space.World);
		}

		public float GetSqrDistanceFromPlayer()
		{
			return (enemyTransform.position - player.GetTransform().position).sqrMagnitude;
		}

		public virtual void DoLogic(float deltaTime)
		{
			if (!(target == null))
			{
				CheckMultiRot();
				state.NextState(this, deltaTime, player);
				RemoveExceptionPositionEnemy();
				ResetRunSpeedTimer();
				if (Time.time - last_enemy_status_time >= 0.2f)
				{
					SendNetEnemyStatusMsg();
					last_enemy_status_time = Time.time;
				}
			}
		}

		public void SendNetEnemyStatusMsg()
		{
			if (GameApp.GetInstance().GetGameState().endless_multiplayer && GameApp.GetInstance().GetGameState().net_com.m_netUserInfo.is_master && (last_pos != enemyTransform.position || last_rot != enemyTransform.rotation.eulerAngles || last_dir != dir))
			{
				Packet packet = CGEnemyStatusPacket.MakePacket(name, enemyTransform.position, enemyTransform.rotation.eulerAngles, dir);
				GameApp.GetInstance().GetGameState().net_com.Send(packet);
				last_pos = enemyTransform.position;
				last_rot = enemyTransform.rotation.eulerAngles;
				last_dir = dir;
			}
		}

		public void SetNetEnemyStatus(Vector3 direction, Vector3 rotation, Vector3 position)
		{
			dir = direction;
			if (Vector3.Distance(enemyTransform.position, position) >= runSpeed * 0.2f * 2f)
			{
				pos_to = position;
				m_is_lerp_position = true;
			}
			else
			{
				m_is_lerp_position = false;
			}
			rot_to = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
			CheckMultiStatus();
		}

		public void CheckMultiRot()
		{
			if (GameApp.GetInstance().GetGameState().endless_multiplayer && !GameApp.GetInstance().GetGameState().net_com.m_netUserInfo.is_master)
			{
				enemyTransform.rotation = Quaternion.Lerp(enemyTransform.rotation, rot_to, Time.deltaTime * 10f);
			}
		}

		public void CheckMultiStatus()
		{
			if (GameApp.GetInstance().GetGameState().endless_multiplayer && !GameApp.GetInstance().GetGameState().net_com.m_netUserInfo.is_master)
			{
				enemyTransform.rotation = Quaternion.Lerp(enemyTransform.rotation, rot_to, Time.deltaTime * 10f);
				if (m_is_lerp_position)
				{
					Vector3 translation = pos_to - enemyTransform.position;
					enemyTransform.Translate(translation, Space.World);
					m_is_lerp_position = false;
				}
			}
		}

		protected void RemoveExceptionPositionEnemy()
		{
			if (enemyTransform.position.y < 9980.1f)
			{
				DamageProperty damageProperty = new DamageProperty();
				damageProperty.damage = HP;
				OnHit(damageProperty, WeaponType.NoGun, false, null);
			}
		}
	}
}
