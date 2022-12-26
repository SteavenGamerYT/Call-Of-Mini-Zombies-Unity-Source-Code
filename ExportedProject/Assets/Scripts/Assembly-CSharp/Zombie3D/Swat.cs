using UnityEngine;

namespace Zombie3D
{
	public class Swat : Enemy
	{
		protected GameObject hitParticles;

		protected LineRenderer lineR;

		public override void Init(GameObject gObject)
		{
			m_tip_height = 2f;
			base.Init(gObject);
			hitParticles = rConfig.hitparticles;
			lineR = enemyObject.GetComponent<Renderer>() as LineRenderer;
			attackRange = 14f;
			MonsterConfig monsterConfig = gConfig.GetMonsterConfig("Swat");
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
				hp *= 1.5f;
				runSpeed += 2f;
				attackDamage *= 1.5f;
				animation[runAnimationName].speed = 1.2f;
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
			TimerManager.GetInstance().SetTimer(1, 12f, true);
		}

		public override bool CouldEnterAttackState()
		{
			if (base.CouldEnterAttackState())
			{
				if (Mathf.Abs(enemyTransform.position.y - player.GetTransform().position.y) < 2f)
				{
					return true;
				}
				return false;
			}
			return false;
		}

		protected void RandomRunAnimation()
		{
			runAnimationName = "Run";
		}

		public override void DoLogic(float deltaTime)
		{
			base.DoLogic(deltaTime);
			if (TimerManager.GetInstance().Ready(1))
			{
				audio.PlayAudio("Shout");
				TimerManager.GetInstance().Do(1);
			}
		}

		public override void CheckHit()
		{
			if (!attacked && IsAnimationPlayedPercentage("Attack01", 0.6f))
			{
				Vector3 vector = new Vector3(enemyTransform.position.x, enemyTransform.position.y, enemyTransform.position.z);
				float num = -1f;
				if (target == null)
				{
					return;
				}
				Vector3 vector2 = new Vector3(target.position.x, enemyTransform.position.y, target.position.z) - vector;
				float magnitude = vector2.magnitude;
				float num2 = 5f;
				float num3 = magnitude / num2;
				float num4 = (num - 0.5f * Physics.gravity.y * 0.5f * num3 * num3) / num3;
				Vector3 vector3 = Vector3.up * num4 + vector2.normalized * num2;
				GameObject gameObject = Object.Instantiate(rConfig.copBomb, vector + Vector3.up * (0f - num), Quaternion.LookRotation(-vector3));
				CopBombScript component = gameObject.GetComponent<CopBombScript>();
				component.damage = attackDamage;
				gameObject.GetComponent<Rigidbody>().AddForce(vector3, ForceMode.Impulse);
				attacked = true;
			}
			base.CheckHit();
		}

		public override void OnAttack()
		{
			base.OnAttack();
			Animate("Attack01", WrapMode.ClampForever);
			enemyTransform.LookAt(target);
			attacked = false;
			lastAttackTime = Time.time;
		}
	}
}
