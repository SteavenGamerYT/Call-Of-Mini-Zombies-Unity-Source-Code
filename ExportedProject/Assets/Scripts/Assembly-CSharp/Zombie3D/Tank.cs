using System;
using System.Collections;
using UnityEngine;

namespace Zombie3D
{
	public class Tank : Enemy
	{
		public static EnemyState RUSHINGSTART_STATE = new RushingStartState();

		public static EnemyState RUSHING_STATE = new RushingState();

		public static EnemyState RUSHINGATTACK_STATE = new RushingAttackState();

		protected Collider handCollider;

		protected Vector3 targetPosition;

		protected Vector3[] p = new Vector3[4];

		protected bool startAttacking;

		protected float rushingInterval;

		protected float rushingSpeed;

		protected float rushingDamage;

		protected float rushingAttackDamage;

		protected float lastRushingTime;

		protected string rndRushingAnimationName;

		protected Vector3 rushingTarget;

		protected bool rushingCollided;

		protected bool rushingAttacked;

		protected Collider leftHandCollider;

		public override void Init(GameObject gObject)
		{
			m_tip_height = 4.5f;
			base.Init(gObject);
			if (Application.loadedLevelName == "Zombie3D_Arena")
			{
				pathFinding = new FastPathFinding();
			}
			handCollider = enemyTransform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand").GetComponent<Collider>();
			leftHandCollider = enemyTransform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 L Clavicle/Bip01 L UpperArm/Bip01 L Forearm/Bip01 L Hand").GetComponent<Collider>();
			collider = enemyTransform.GetComponent<Collider>();
			controller = enemyTransform.GetComponent<Collider>() as CharacterController;
			lastTarget = Vector3.zero;
			attackRange = 3f;
			onhitRate = 10f;
			lastRushingTime = -99f;
			startAttacking = false;
			rushingCollided = false;
			rushingAttacked = false;
			MonsterConfig monsterConfig = gConfig.GetMonsterConfig("Tank");
			hp = monsterConfig.hp * gameScene.GetDifficultyHpFactor;
			attackDamage = monsterConfig.damage * gameScene.GetDifficultyDamageFactor;
			attackFrequency = monsterConfig.attackRate;
			runSpeed = monsterConfig.walkSpeed;
			rushingInterval = monsterConfig.rushInterval;
			rushingSpeed = monsterConfig.rushSpeed;
			rushingDamage = monsterConfig.rushDamage * gameScene.GetDifficultyDamageFactor;
			rushingAttackDamage = monsterConfig.rushAttackDamage * gameScene.GetDifficultyDamageFactor;
			lootCash = monsterConfig.lootCash;
			if (GameApp.GetInstance().GetGameState().endless_multiplayer)
			{
				hp *= 3f;
				attackDamage *= 3f;
			}
			if (base.IsElite)
			{
				hp *= 1.2f;
				runSpeed += 1f;
				attackDamage *= 1.2f;
				rushingDamage *= 1.2f;
				rushingAttackDamage *= 1.2f;
			}
			if (m_isBoss)
			{
				hp *= 2f;
			}
			if (m_isPrey)
			{
				hp *= GameApp.GetInstance().GetGameConfig().hunting_hp;
				attackDamage *= GameApp.GetInstance().GetGameConfig().hunting_damage;
				runSpeed *= GameApp.GetInstance().GetGameConfig().hunting_run_speed;
			}
		}

		public override void OnHit(DamageProperty dp, WeaponType weaponType, bool criticalAttack, Player mPlayer)
		{
			if (state == Enemy.GRAVEBORN_STATE)
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
			}
			UnityEngine.Object.Instantiate(rConfig.hitBlood, enemyTransform.position + Vector3.up * 1.5f, Quaternion.identity);
			hp -= dp.damage;
			switch (weaponType)
			{
			case WeaponType.RocketLauncher:
			case WeaponType.LaserGun:
			case WeaponType.Sniper:
			case WeaponType.M32:
				gotHitTime = Time.time;
				state.OnHit(this);
				break;
			case WeaponType.AssaultRifle:
			case WeaponType.Saw:
				if (Math.RandomRate(10f) && Time.time - gotHitTime > 2f)
				{
					gotHitTime = Time.time;
					state.OnHit(this);
				}
				break;
			case WeaponType.ShotGun:
			case WeaponType.MachineGun:
				if (Math.RandomRate(30f))
				{
					gotHitTime = Time.time;
					PlayBloodExplodeEffect(enemyTransform.TransformPoint(new Vector3(0f, 1f, 1f)));
					state.OnHit(this);
				}
				break;
			}
			if (GameApp.GetInstance().GetGameState().endless_multiplayer && dp.damage > 0f)
			{
				int criticalAttack2 = (criticalAttack ? 1 : 0);
				Packet packet = CGEnemyGotHitPacket.MakePacket(name, (long)(dp.damage * 1000f), (uint)weaponType, (uint)criticalAttack2);
				GameApp.GetInstance().GetGameState().net_com.Send(packet);
				GameMultiplayerScene gameMultiplayerScene = GameApp.GetInstance().GetGameScene() as GameMultiplayerScene;
				gameMultiplayerScene.player_injured_val += dp.damage;
				if (hp <= 0f && mPlayer != null && !is_multi_dead)
				{
					int bElite = (base.IsElite ? 1 : 0);
					Packet packet2 = CGEnemyDeadPacket.MakePacket((uint)mPlayer.m_multi_id, name, (uint)enemyType, (uint)bElite, (uint)weaponType);
					GameApp.GetInstance().GetGameState().net_com.Send(packet2);
					is_multi_dead = true;
				}
				else if (hp <= 0f && mPlayer == null && !is_multi_dead)
				{
					int bElite2 = (base.IsElite ? 1 : 0);
					Packet packet3 = CGEnemyDeadPacket.MakePacket(0u, name, (uint)enemyType, (uint)bElite2, (uint)weaponType);
					GameApp.GetInstance().GetGameState().net_com.Send(packet3);
					is_multi_dead = true;
				}
			}
		}

		public override void OnAttack()
		{
			base.OnAttack();
			Animate("Attack01", WrapMode.ClampForever);
			startAttacking = true;
			lastAttackTime = Time.time;
		}

		public override void CheckHit()
		{
			if (startAttacking && animation["Attack01"].time > animation["Attack01"].clip.length * 0.8f)
			{
				if (!GameApp.GetInstance().GetGameState().endless_multiplayer)
				{
					Collider collider = player.GetCollider();
					if (collider != null && handCollider.bounds.Intersects(collider.bounds))
					{
						player.OnHit(attackDamage);
					}
				}
				else
				{
					foreach (Player item in GameApp.GetInstance().GetGameScene().m_multi_player_arr)
					{
						Collider collider2 = item.GetCollider();
						if (collider2 != null && handCollider.bounds.Intersects(collider2.bounds))
						{
							item.OnHit(attackDamage);
							break;
						}
					}
				}
				startAttacking = false;
			}
			base.CheckHit();
		}

		public override void OnDead()
		{
			gameScene.IncreaseKills();
			audio.PlayAudio("Dead");
			deadTime = Time.time;
			PlayBloodEffect();
			animation["Death01"].wrapMode = WrapMode.ClampForever;
			animation.CrossFade("Death01");
			enemyObject.layer = 18;
			enemyObject.SendMessage("OnLoot", m_isPrey);
			GameApp.GetInstance().GetGameState().Achievement.KillEnemy();
			GameApp.GetInstance().GetGameState().Achievement.CheckAchievemnet_BraveHeart();
			CheckPreyEnemyDeath();
		}

		public bool Rush(float deltaTime)
		{
			Collider collider = player.GetCollider();
			enemyTransform.LookAt(rushingTarget);
			Vector3 vector = enemyTransform.TransformDirection(Vector3.forward) * rushingSpeed + Physics.gravity * 0.5f;
			controller.Move(vector * deltaTime);
			if (!rushingCollided && collider != null)
			{
				Vector3 vector2 = enemyTransform.InverseTransformPoint(player.GetTransform().position);
				if (vector2.sqrMagnitude < 25f && vector2.z > 1f)
				{
					player.OnHit(rushingDamage);
					lastAttackTime = Time.time;
					rushingCollided = true;
				}
			}
			IEnumerator enumerator = gameScene.GetEnemies().Values.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Enemy enemy = (Enemy)enumerator.Current;
					if (enemy.GetState() != Enemy.DEAD_STATE && enemy.EnemyType != EnemyType.E_TANK && base.collider.bounds.Intersects(enemy.GetCollider().bounds))
					{
						DamageProperty damageProperty = new DamageProperty();
						damageProperty.damage = rushingDamage;
						enemy.OnHit(damageProperty, WeaponType.NoGun, true, null);
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = enumerator as IDisposable) != null)
				{
					disposable.Dispose();
				}
			}
			GameObject[] woodBoxes = gameScene.GetWoodBoxes();
			GameObject[] array = woodBoxes;
			GameObject[] array2 = array;
			foreach (GameObject gameObject in array2)
			{
				if (gameObject != null && base.collider.bounds.Intersects(gameObject.GetComponent<Collider>().bounds))
				{
					WoodBoxScript component = gameObject.GetComponent<WoodBoxScript>();
					component.OnHit(attackDamage);
				}
			}
			if ((enemyTransform.position - rushingTarget).sqrMagnitude < 1f || (enemyTransform.position - player.GetTransform().position).sqrMagnitude < 4f || Time.time - lastRushingTime > 3f)
			{
				animation["Rush03"].wrapMode = WrapMode.ClampForever;
				animation.CrossFade("Rush03");
				return true;
			}
			return false;
		}

		public bool RushAttack(float deltaTime)
		{
			Collider collider = player.GetCollider();
			if (!rushingAttacked && animation["Rush03"].time >= animation["Rush03"].clip.length * 0.3f)
			{
				if (collider != null && (base.collider.bounds.Intersects(collider.bounds) || leftHandCollider.bounds.Intersects(collider.bounds)))
				{
					player.OnHit(rushingAttackDamage);
					lastAttackTime = Time.time;
				}
				rushingAttacked = true;
			}
			if (rushingAttacked && IsAnimationPlayedPercentage("Rush03", 1f))
			{
				rushingAttacked = false;
				return true;
			}
			return false;
		}

		public override EnemyState EnterSpecialState(float deltaTime)
		{
			if (Time.time - lastRushingTime > rushingInterval && enemyTransform.position.y < 10000.3f)
			{
				rushingTarget = new Vector3(target.position.x, enemyTransform.position.y, target.position.z);
				Vector3 normalized = (rushingTarget - enemyTransform.position).normalized;
				rushingTarget += normalized * 5f;
				lastRushingTime = Time.time;
				float magnitude = (rushingTarget - enemyTransform.position).magnitude;
				Ray ray = new Ray(enemyTransform.position + new Vector3(0f, 0.5f, 0f), normalized);
				RaycastHit hitInfo;
				if (!Physics.Raycast(ray, out hitInfo, magnitude, 4261888))
				{
					enemyTransform.LookAt(rushingTarget);
					rushingCollided = false;
					animation["Rush01"].wrapMode = WrapMode.ClampForever;
					animation["Rush01"].speed = 2f;
					animation.CrossFade("Rush01");
					return RUSHINGSTART_STATE;
				}
			}
			return null;
		}

		public override void DoMove(float deltaTime)
		{
			Vector3 vector = enemyTransform.TransformDirection(Vector3.forward);
			vector += Physics.gravity * 0.5f;
			controller.Move(vector * runSpeed * deltaTime);
			audio.PlayAudio("Walk");
		}

		public override void FindPath()
		{
			if (Application.loadedLevelName == "Zombie3D_Arena")
			{
				if ((GameApp.GetInstance().GetGameState().endless_multiplayer && !GameApp.GetInstance().GetGameState().net_com.m_netUserInfo.is_master) || target == null)
				{
					return;
				}
				Vector3 position = target.position;
				if (Time.time - lastPathFindingTime > 0.5f)
				{
					lastPathFindingTime = Time.time;
					position.y = enemyTransform.position.y;
					if (lastTarget == Vector3.zero)
					{
						lastTarget = target.position;
					}
					base.ray = new Ray(enemyTransform.position + new Vector3(0f, 0.5f, 0f), position - (enemyTransform.position + new Vector3(0f, 0f, 0f)));
					if (Physics.Raycast(base.ray, out rayhit, 100f, 4262144))
					{
						if (rayhit.collider.gameObject.name == "Player")
						{
							lastTarget = position;
							nextPoint = -1;
						}
						else
						{
							float num = 999999f;
							if (nextPoint == -1)
							{
								for (int i = 0; i < path.Length; i++)
								{
									float magnitude = (path[i] - enemyTransform.position).magnitude;
									if (magnitude < num)
									{
										Ray ray = new Ray(enemyTransform.position + new Vector3(0f, 0.8f, 0f), path[i] - enemyTransform.position);
										RaycastHit hitInfo;
										if (!Physics.Raycast(ray, out hitInfo, magnitude, 4261888))
										{
											lastTarget = path[i];
											num = magnitude;
											nextPoint = i;
										}
									}
								}
							}
							else if ((enemyTransform.position - lastTarget).sqrMagnitude < 4f)
							{
								int num2 = nextPoint - 1;
								int num3 = nextPoint + 1;
								if (num3 == path.Length)
								{
									num3 = 0;
								}
								if (num2 == -1)
								{
									num2 = path.Length - 1;
								}
								if ((path[num3] - player.GetTransform().position).magnitude + (path[num3] - enemyTransform.position).magnitude < (path[num2] - player.GetTransform().position).magnitude + (path[num2] - enemyTransform.position).magnitude)
								{
									nextPoint = num3;
								}
								else
								{
									nextPoint = num2;
								}
								lastTarget = path[nextPoint];
							}
						}
					}
				}
				lastTarget.y = enemyTransform.position.y;
				enemyTransform.LookAt(lastTarget);
			}
			else
			{
				base.FindPath();
			}
		}

		public override void Animate(string animationName, WrapMode wrapMode)
		{
			base.Animate(animationName, wrapMode);
		}
	}
}
