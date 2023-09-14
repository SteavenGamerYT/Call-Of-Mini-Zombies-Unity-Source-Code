using UnityEngine;

namespace Zombie3D
{
	public class ZombieTutorial : Enemy
	{
		protected Collider handCollider;

		protected Vector3 targetPosition;

		protected Vector3[] p = new Vector3[4];

		public static int AI_Player_ID = 250;

		protected void RandomRunAnimation()
		{
			int num = Random.Range(0, 10);
			if (num < 7)
			{
				runAnimationName = "Run";
			}
			else
			{
				switch (num)
				{
				case 7:
					runAnimationName = "Forward01";
					break;
				case 8:
					runAnimationName = "Forward02";
					break;
				case 9:
					runAnimationName = "Forward03";
					break;
				}
			}
			if (base.IsElite)
			{
				runAnimationName = "Run";
			}
		}

		public override void Init(GameObject gObject)
		{
			m_tip_height = 2f;
			base.Init(gObject);
			handCollider = enemyTransform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand").gameObject.GetComponent<Collider>();
			lastTarget = Vector3.zero;
			MonsterConfig monsterConfig = gConfig.GetMonsterConfig("Zombie");
			hp = 200f;
			attackDamage = 500f;
			attackFrequency = monsterConfig.attackRate;
			runSpeed = monsterConfig.walkSpeed;
			lootCash = 0;
			RandomRunAnimation();
			TimerManager.GetInstance().SetTimer(0, 8f, true);
			target = null;
			player = null;
		}

		public override void CheckHit()
		{
			if (!attacked && IsAnimationPlayedPercentage("Attack01", 0.4f))
			{
				if (!GameApp.GetInstance().GetGameState().endless_multiplayer)
				{
					Collider collider = player.GetCollider();
					if (collider != null && handCollider.bounds.Intersects(collider.bounds))
					{
						player.OnHit(attackDamage);
						attacked = true;
					}
				}
				else
				{
					foreach (Player item in GameApp.GetInstance().GetGameScene().m_multi_player_arr)
					{
						Collider collider2 = item.GetCollider();
						if (collider2 != null && handCollider.bounds.Intersects(collider2.bounds) && item.m_multi_id == AI_Player_ID)
						{
							item.OnHit(attackDamage);
							attacked = true;
							break;
						}
					}
				}
			}
			if (GameApp.GetInstance().GetGameState().endless_multiplayer && GameApp.GetInstance().GetGameState().net_com.m_netUserInfo.is_master && player.GetPlayerState() == player.DEAD_STATE)
			{
				Debug.Log("Target is dead, leave now...");
				RemoveEnemyNow();
				GameMultiplayerScene gameMultiplayerScene = GameApp.GetInstance().GetGameScene() as GameMultiplayerScene;
				gameMultiplayerScene.game_tui.ShowTutorialMsg();
			}
		}

		public override void OnAttack()
		{
			base.OnAttack();
			Animate("Attack01", WrapMode.ClampForever);
			attacked = false;
			lastAttackTime = Time.time;
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
				dp.damage = 0f;
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
		}

		public override void FindPath()
		{
			if (GameApp.GetInstance().GetGameState().endless_multiplayer && !GameApp.GetInstance().GetGameState().net_com.m_netUserInfo.is_master)
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
						pathFinding.ClearPath();
					}
				}
			}
			else if (Time.time - lastReCheckPathTime > 5f)
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
				lastReCheckPathTime = Time.time;
			}
			if ((enemyTransform.position - lastTarget).sqrMagnitude < 1f)
			{
				pathFinding.PopNode();
				Transform nextWayPoint2 = pathFinding.GetNextWayPoint(enemyTransform.position, player.GetTransform().position);
				if (nextWayPoint2 != null)
				{
					lastTarget = nextWayPoint2.position;
				}
			}
			enemyTransform.LookAt(new Vector3(lastTarget.x, enemyTransform.position.y, lastTarget.z));
			dir = (lastTarget - enemyTransform.position).normalized;
		}
	}
}
