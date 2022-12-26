using System.Collections.Generic;
using UnityEngine;

namespace Zombie3D
{
	public class Boomer : Enemy
	{
		public static EnemyState EXPLODE_STATE = new BoomerExplodeState();

		protected Collider handCollider;

		protected Vector3 targetPosition;

		protected Vector3[] p = new Vector3[4];

		protected GameObject explodeObject;

		public override void Init(GameObject gObject)
		{
			base.Init(gObject);
			handCollider = enemyTransform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand").gameObject.GetComponent<Collider>();
			lastTarget = Vector3.zero;
			MonsterConfig monsterConfig = gConfig.GetMonsterConfig("Boomer");
			hp = monsterConfig.hp * gameScene.GetDifficultyHpFactor;
			attackDamage = monsterConfig.damage * gameScene.GetDifficultyDamageFactor;
			attackFrequency = monsterConfig.attackRate;
			runSpeed = monsterConfig.walkSpeed;
			lootCash = monsterConfig.lootCash;
			explodeObject = rConfig.boomerExplosion;
			if (GameApp.GetInstance().GetGameState().endless_multiplayer)
			{
				hp *= 3f;
				attackDamage *= 3f;
			}
			if (base.IsElite)
			{
				hp *= 2f;
				attackDamage *= 1.5f;
			}
			if (m_isBoss)
			{
				hp *= 2f;
			}
			enemyObject.GetComponent<AudioSource>().mute = !GameApp.GetInstance().GetGameState().SoundOn;
		}

		public void Explode()
		{
			if (!GameApp.GetInstance().GetGameState().endless_multiplayer)
			{
				float sqrMagnitude = (enemyTransform.position - player.GetTransform().position).sqrMagnitude;
				if (sqrMagnitude < 25f)
				{
					player.OnHit(attackDamage);
				}
				return;
			}
			List<Player> list = new List<Player>();
			foreach (Player item in GameApp.GetInstance().GetGameScene().m_multi_player_arr)
			{
				float sqrMagnitude2 = (enemyTransform.position - item.GetTransform().position).sqrMagnitude;
				if (sqrMagnitude2 < 25f)
				{
					list.Add(item);
				}
			}
			foreach (Player item2 in list)
			{
				item2.OnHit(attackDamage);
			}
		}

		public override void OnDead()
		{
			GameObject gameObject = Object.Instantiate(explodeObject, enemyTransform.position, Quaternion.identity);
			gameObject.GetComponent<AudioSource>().mute = !GameApp.GetInstance().GetGameState().SoundOn;
			criticalAttacked = true;
			base.OnDead();
		}

		public override void OnAttack()
		{
			base.OnAttack();
			DamageProperty damageProperty = new DamageProperty();
			damageProperty.damage = hp;
			Explode();
			OnHit(damageProperty, WeaponType.NoGun, true, null);
		}

		public override EnemyState EnterSpecialState(float deltaTime)
		{
			EnemyState result = null;
			if (base.SqrDistanceFromPlayer < base.AttackRange * base.AttackRange)
			{
				Animate("Attack01", WrapMode.Loop);
				result = EXPLODE_STATE;
			}
			return result;
		}

		public override void Animate(string animationName, WrapMode wrapMode)
		{
			base.Animate(animationName, wrapMode);
		}

		public override void PlayDeadEffects()
		{
			base.PlayDeadEffects();
		}
	}
}
