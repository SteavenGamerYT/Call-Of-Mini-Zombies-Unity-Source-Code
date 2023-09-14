using UnityEngine;

namespace Zombie3D
{
	public class Hunter : Enemy
	{
		public static EnemyState JUMP_STATE = new JumpState();

		public static EnemyState LOOKAROUND_STATE = new LookAroundState();

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

		public Vector3 speed;

		protected Collider leftHandCollider;

		protected bool jumpended;

		public bool JumpEnded
		{
			get
			{
				return jumpended;
			}
		}

		public override void Init(GameObject gObject)
		{
			m_tip_height = 2.5f;
			base.Init(gObject);
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
			detectionRange = 120f;
			MonsterConfig monsterConfig = gConfig.GetMonsterConfig("Hunter");
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
				hp *= 1.5f;
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
			animation["Run"].speed = 1.5f;
			animation["JumpStart01"].wrapMode = WrapMode.ClampForever;
			animation["JumpIdle01"].wrapMode = WrapMode.Loop;
			animation["JumpEnd01"].wrapMode = WrapMode.ClampForever;
		}

		public override void OnAttack()
		{
			base.OnAttack();
			Animate("Attack01", WrapMode.ClampForever);
			attacked = false;
			lastAttackTime = Time.time;
		}

		public override void OnDead()
		{
			gameScene.IncreaseKills();
			PlayBloodEffect();
			deadTime = Time.time;
			animation["Death01"].wrapMode = WrapMode.ClampForever;
			animation.Play("Death01");
			enemyObject.layer = 18;
			enemyObject.SendMessage("OnLoot", m_isPrey);
			GameApp.GetInstance().GetGameState().Achievement.KillEnemy();
			CheckPreyEnemyDeath();
			RemoveEnemyMark();
		}

		public override void CheckHit()
		{
			if (CouldMakeNextAttack() || (!attacked && IsAnimationPlayedPercentage("Attack01", 0.6f)))
			{
				if (!GameApp.GetInstance().GetGameState().endless_multiplayer)
				{
					Collider collider = player.GetCollider();
					if (collider != null && handCollider.bounds.Intersects(collider.bounds))
					{
						player.OnHit(attackDamage);
						if (CouldMakeNextAttack())
						{
							lastAttackTime = Time.time;
						}
						else if (!attacked && IsAnimationPlayedPercentage("Attack01", 0.6f))
						{
							attacked = true;
						}
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
							if (CouldMakeNextAttack())
							{
								lastAttackTime = Time.time;
							}
							else if (!attacked && IsAnimationPlayedPercentage("Attack01", 0.6f))
							{
								attacked = true;
							}
							break;
						}
					}
				}
			}
			base.CheckHit();
		}

		public bool Jump(float deltaTime)
		{
			if ((Time.time - lastRushingTime > 0.5f && enemyTransform.position.y <= 10000.3f) || Time.time - lastRushingTime > 4f)
			{
				CheckHit();
			}
			else
			{
				speed += Physics.gravity * deltaTime;
				controller.Move(speed * deltaTime);
			}
			if ((Time.time - lastRushingTime > 0.5f && enemyTransform.position.y <= 10001.699f) || Time.time - lastRushingTime > 2f || controller.isGrounded)
			{
				if (!jumpended)
				{
					animation.CrossFade("JumpEnd01");
					jumpended = true;
				}
				if (IsAnimationPlayedPercentage("JumpEnd01", 1f))
				{
					return true;
				}
			}
			return false;
		}

		public override void DoLogic(float deltaTime)
		{
			base.DoLogic(deltaTime);
			if (state == Enemy.DEAD_STATE)
			{
				speed = Physics.gravity * 10f;
				controller.Move(speed * deltaTime);
			}
		}

		public bool ReadyForJump()
		{
			if (Time.time - lastRushingTime > 5.5f && (enemyTransform.position - target.position).sqrMagnitude > 64f && (enemyTransform.position - target.position).sqrMagnitude < 225f)
			{
				return true;
			}
			return false;
		}

		public void StartJump()
		{
			lastRushingTime = Time.time;
			Vector3 vector = new Vector3(enemyTransform.position.x, 10000.1f, enemyTransform.position.z);
			float num = 0f;
			Vector3 vector2 = new Vector3(player.GetTransform().position.x, 10000.1f, player.GetTransform().position.z) - vector;
			float magnitude = vector2.magnitude;
			float num2 = 10f;
			float num3 = magnitude / num2;
			float num4 = (num - 0.5f * Physics.gravity.y * num3 * num3) / num3;
			speed = Vector3.up * num4 + vector2.normalized * num2;
			animation.CrossFade("JumpStart01");
			audio.PlayAudio("Special");
			jumpended = false;
		}

		public bool LookAroundTimOut()
		{
			if (Time.time - lookAroundStartTime > 2f)
			{
				return true;
			}
			return false;
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
			if (weaponType == WeaponType.Sword)
			{
				Vector3 vector = player.GetTransform().position - enemyTransform.position;
				vector.Normalize();
				Object.Instantiate(rConfig.swordAttack, enemyTransform.position + Vector3.up * 0.7f + vector * 0.5f, Quaternion.identity);
			}
			Object.Instantiate(rConfig.hitBlood, enemyTransform.position + Vector3.up * 0.5f, Quaternion.identity);
			hp -= dp.damage;
			if (weaponType == WeaponType.AssaultRifle || weaponType == WeaponType.Saw)
			{
				if (Time.time - gotHitTime > 0.3f)
				{
					gotHitTime = Time.time;
					state.OnHit(this);
				}
			}
			else
			{
				gotHitTime = Time.time;
				state.OnHit(this);
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

		public override void DoMove(float deltaTime)
		{
			Vector3 vector = enemyTransform.TransformDirection(Vector3.forward);
			vector += Physics.gravity * 0.2f;
			controller.Move(vector * runSpeed * deltaTime);
			audio.PlayAudio("Walk");
		}

		public override EnemyState EnterSpecialState(float deltaTime)
		{
			EnemyState result = null;
			target = player.GetTransform();
			if (Time.time - lastRushingTime > 5f && Time.time - lookAroundStartTime > 10f)
			{
				int num = Random.Range(0, 100);
				if (num < 10)
				{
					result = new LookAroundState();
					lookAroundStartTime = Time.time;
					spawnCenter = enemyTransform.position;
				}
				else if (ReadyForJump())
				{
					StartJump();
					result = new JumpState();
				}
			}
			return result;
		}
	}
}
