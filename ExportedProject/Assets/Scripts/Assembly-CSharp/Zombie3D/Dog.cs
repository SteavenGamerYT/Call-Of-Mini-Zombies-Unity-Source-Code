using UnityEngine;

namespace Zombie3D
{
	public class Dog : Enemy
	{
		protected Collider headCollider;

		protected Vector3 targetPosition;

		protected Vector3[] p = new Vector3[4];

		protected void RandomRunAnimation()
		{
			runAnimationName = "Run01";
		}

		public override void Init(GameObject gObject)
		{
			if (base.IsElite)
			{
				m_tip_height = 2.8f;
			}
			else
			{
				m_tip_height = 1.8f;
			}
			base.Init(gObject);
			headCollider = enemyTransform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head/Bip01 HeadNub").gameObject.GetComponent<Collider>();
			lastTarget = Vector3.zero;
			MonsterConfig monsterConfig = gConfig.GetMonsterConfig("Dog");
			hp = monsterConfig.hp * gameScene.GetDifficultyHpFactor;
			attackDamage = monsterConfig.damage * gameScene.GetDifficultyDamageFactor;
			attackFrequency = monsterConfig.attackRate;
			runSpeed = monsterConfig.walkSpeed;
			lootCash = monsterConfig.lootCash;
			RandomRunAnimation();
			if (GameApp.GetInstance().GetGameState().endless_multiplayer)
			{
				hp *= 3f;
				attackDamage *= 3f;
			}
			if (base.IsElite)
			{
				hp *= 3f;
				attackDamage *= 2f;
				attackRange = 2f;
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
			TimerManager.GetInstance().SetTimer(6, 8f, true);
		}

		public override void CheckHit()
		{
			if (!attacked && IsAnimationPlayedPercentage("Attack01", 0.4f))
			{
				if (!GameApp.GetInstance().GetGameState().endless_multiplayer)
				{
					Collider collider = player.GetCollider();
					if (collider != null && headCollider.bounds.Intersects(collider.bounds))
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
						if (collider2 != null && headCollider.bounds.Intersects(collider2.bounds))
						{
							item.OnHit(attackDamage);
							attacked = true;
							break;
						}
					}
				}
			}
			base.CheckHit();
		}

		public override void DoLogic(float deltaTime)
		{
			base.DoLogic(deltaTime);
			if (TimerManager.GetInstance().Ready(6))
			{
				audio.PlayAudio("Shout");
				TimerManager.GetInstance().Do(6);
			}
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
			deadTime = Time.time;
			gameScene.IncreaseKills();
			gameScene.ModifyEnemyNum(-1);
			GameApp.GetInstance().GetGameState().Achievement.KillEnemy();
			GameApp.GetInstance().GetGameState().AddCashForRecord((int)((float)lootCash * gameScene.GetDifficultyCashDropFactor));
			if (GameApp.GetInstance().GetGameState().endless_level)
			{
				GameApp.GetInstance().GetGameScene().endless_get_cash += (int)((float)lootCash * gameScene.GetDifficultyCashDropFactor);
			}
			enemyObject.SendMessage("OnLoot", m_isPrey);
			criticalAttacked = false;
			audio.PlayAudio("Dead");
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
					animation.Stop();
					animation.Play("Death01");
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

		public override void Animate(string animationName, WrapMode wrapMode)
		{
			base.Animate(animationName, wrapMode);
		}
	}
}
