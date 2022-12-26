using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zombie3D;

[AddComponentMenu("TPS/ProjectileScript")]
public class ProjectileScript : MonoBehaviour
{
	protected GameObject resObject;

	protected GameObject explodeObject;

	protected GameObject smallExplodeObject;

	protected GameObject laserHitObject;

	public Transform targetTransform;

	protected Transform proTransform;

	protected WeaponType gunType;

	protected ResourceConfigScript rConf;

	public Vector3 dir;

	public float hitForce;

	public float explodeRadius;

	public float flySpeed;

	public Vector3 speed;

	public float life = 2f;

	public float damage;

	protected float createdTime;

	protected float lastTriggerTime;

	protected float gravity = 16f;

	protected float downSpeed;

	protected float deltaTime;

	public Vector3 targetPos;

	protected Vector3 lastPos;

	public float initAngel = 40f;

	protected float lastCheckPosTime;

	protected float heightDelta = 0.02f;

	public bool m_isMultiSniper;

	public Player player;

	public WeaponType GunType
	{
		set
		{
			gunType = value;
		}
	}

	public void Start()
	{
		rConf = GameApp.GetInstance().GetResourceConfig();
		resObject = rConf.projectile;
		smallExplodeObject = rConf.salivaExplosion;
		explodeObject = rConf.rocketExlposion;
		proTransform = base.transform;
		laserHitObject = rConf.laserHit;
		createdTime = Time.time;
		lastCheckPosTime = Time.time;
		lastPos = proTransform.position;
	}

	public void Update()
	{
		deltaTime += Time.deltaTime;
		if (deltaTime < 0.03f)
		{
			return;
		}
		if (gunType == WeaponType.Sniper)
		{
			if (!m_isMultiSniper && targetTransform != null)
			{
				if (targetTransform.gameObject.active)
				{
					dir = (targetTransform.position - proTransform.position).normalized;
					targetPos = targetTransform.position;
				}
				else
				{
					targetTransform = null;
				}
			}
			proTransform.LookAt(targetPos);
			initAngel -= deltaTime * 80f;
			if (initAngel <= 0f)
			{
				initAngel = 0f;
			}
			proTransform.rotation = Quaternion.AngleAxis(initAngel, -1f * proTransform.right) * proTransform.rotation;
			proTransform.RotateAround(proTransform.forward, Time.time * 10f);
			dir = proTransform.forward;
			if (Time.time - lastCheckPosTime > 0.3f)
			{
				lastCheckPosTime = Time.time;
				if ((proTransform.position - lastPos).sqrMagnitude < 2f)
				{
					UnityEngine.Object.DestroyObject(base.gameObject);
					return;
				}
				lastPos = proTransform.position;
			}
		}
		else if (gunType == WeaponType.M32)
		{
			proTransform.LookAt(targetPos);
			initAngel -= deltaTime * 80f;
			if (initAngel <= -40f)
			{
				initAngel = -40f;
			}
			proTransform.rotation = Quaternion.AngleAxis(initAngel, -1f * proTransform.right) * proTransform.rotation;
			dir = proTransform.forward;
		}
		if (gunType == WeaponType.NurseSaliva)
		{
			speed += Physics.gravity.y * Vector3.up * deltaTime;
			proTransform.Translate(speed * deltaTime, Space.World);
			proTransform.LookAt(proTransform.position + speed * 10f);
		}
		else
		{
			proTransform.Translate(flySpeed * dir * deltaTime, Space.World);
			if (gunType == WeaponType.RocketLauncher)
			{
				proTransform.Rotate(Vector3.forward, deltaTime * 1000f, Space.Self);
			}
		}
		if (Time.time - createdTime > life)
		{
			UnityEngine.Object.DestroyObject(base.gameObject);
		}
		deltaTime = 0f;
	}

	private void OnTriggerStay(Collider other)
	{
		if (gunType != WeaponType.LaserGun)
		{
			return;
		}
		GameScene gameScene = GameApp.GetInstance().GetGameScene();
		Weapon weapon = this.player.GetWeapon();
		if (weapon.GetWeaponType() != WeaponType.LaserGun)
		{
			return;
		}
		LaserGun laserGun = weapon as LaserGun;
		if (laserGun == null || !laserGun.CouldMakeNextShoot())
		{
			return;
		}
		if (other.gameObject.layer == 9)
		{
			Enemy enemyByID = gameScene.GetEnemyByID(other.gameObject.name);
			if (enemyByID != null && enemyByID.GetState() != Enemy.DEAD_STATE)
			{
				UnityEngine.Object.Instantiate(laserHitObject, enemyByID.GetPosition(), Quaternion.identity);
				DamageProperty damageProperty = new DamageProperty();
				damageProperty.hitForce = (dir + new Vector3(0f, 0.18f, 0f)) * hitForce;
				damageProperty.damage = damage;
				enemyByID.OnHit(damageProperty, gunType, false, this.player);
			}
		}
		else if (other.gameObject.layer == 8 && GameApp.GetInstance().GetGameState().VS_mode && !(this.player.PlayerObject == other.gameObject))
		{
			Player player = other.gameObject.GetComponent<PlayerShell>().m_player;
			if (player != null && player.GetPlayerState() != player.DEAD_STATE && damage > 0f)
			{
				Debug.Log("hit player:" + damage * this.player.PowerBuff);
				player.OnVsInjured(this.player.sfs_user, damage * this.player.PowerBuff, (int)gunType);
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		GameScene gameScene = GameApp.GetInstance().GetGameScene();
		if (gunType == WeaponType.RocketLauncher || gunType == WeaponType.M32 || gunType == WeaponType.Sniper)
		{
			if (this.player.PlayerObject == other.gameObject)
			{
				return;
			}
			UnityEngine.Object.Instantiate(rConf.rpgFloor, new Vector3(proTransform.position.x, 10000.199f, proTransform.position.z), Quaternion.Euler(270f, 0f, 0f));
			GameObject gameObject = UnityEngine.Object.Instantiate(explodeObject, proTransform.position, Quaternion.identity);
			AudioSource component = gameObject.GetComponent<AudioSource>();
			if (component != null)
			{
				component.mute = !GameApp.GetInstance().GetGameState().SoundOn;
			}
			UnityEngine.Object.DestroyObject(base.gameObject);
			this.player.LastHitPosition = proTransform.position;
			int num = 0;
			IEnumerator enumerator = gameScene.GetEnemies().Values.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Enemy enemy = (Enemy)enumerator.Current;
					if (enemy.GetState() == Enemy.DEAD_STATE)
					{
						continue;
					}
					float sqrMagnitude = (enemy.GetPosition() - proTransform.position).sqrMagnitude;
					float num2 = explodeRadius * explodeRadius;
					if (sqrMagnitude < num2)
					{
						DamageProperty damageProperty = new DamageProperty();
						damageProperty.hitForce = (dir + new Vector3(0f, 0.18f, 0f)) * hitForce;
						if (sqrMagnitude * 4f < num2)
						{
							damageProperty.damage = damage * this.player.PowerBuff;
							enemy.OnHit(damageProperty, gunType, true, this.player);
						}
						else
						{
							damageProperty.damage = damage / 2f * this.player.PowerBuff;
							enemy.OnHit(damageProperty, gunType, false, this.player);
						}
					}
					if (enemy.HP <= 0f)
					{
						num++;
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
			if (num >= 4)
			{
				GameApp.GetInstance().GetGameState().Achievement.CheckAchievemnet_WeaponMaster();
			}
			if (GameApp.GetInstance().GetGameState().VS_mode)
			{
				GameVSScene gameVSScene = gameScene as GameVSScene;
				foreach (Player value in gameVSScene.SFS_Player_Arr.Values)
				{
					if (value == null || value == this.player || value.GetPlayerState() == value.DEAD_STATE)
					{
						continue;
					}
					float sqrMagnitude2 = (value.GetTransform().position - proTransform.position).sqrMagnitude;
					float num3 = explodeRadius * explodeRadius;
					if (!(sqrMagnitude2 < num3))
					{
						continue;
					}
					DamageProperty damageProperty2 = new DamageProperty();
					damageProperty2.hitForce = (dir + new Vector3(0f, 0.18f, 0f)) * hitForce;
					if (sqrMagnitude2 * 4f < num3)
					{
						if (damage > 0f)
						{
							Debug.Log("hit player:" + damage * this.player.PowerBuff);
							value.OnVsInjured(this.player.sfs_user, damage * this.player.PowerBuff, (int)gunType);
						}
					}
					else if (damage > 0f)
					{
						Debug.Log("hit player:" + damage / 2f * this.player.PowerBuff);
						value.OnVsInjured(this.player.sfs_user, damage * this.player.PowerBuff / 2f, (int)gunType);
					}
				}
			}
			GameObject[] woodBoxes = gameScene.GetWoodBoxes();
			GameObject[] array = woodBoxes;
			GameObject[] array2 = array;
			foreach (GameObject gameObject2 in array2)
			{
				if (gameObject2 != null)
				{
					float sqrMagnitude3 = (gameObject2.transform.position - proTransform.position).sqrMagnitude;
					float num4 = explodeRadius * explodeRadius;
					if (sqrMagnitude3 < num4)
					{
						WoodBoxScript component2 = gameObject2.GetComponent<WoodBoxScript>();
						component2.OnHit(damage * this.player.PowerBuff);
					}
				}
			}
		}
		else if (gunType == WeaponType.LaserGun)
		{
			if (other.gameObject.layer == 9)
			{
				Enemy enemyByID = gameScene.GetEnemyByID(other.gameObject.name);
				if (enemyByID != null && enemyByID.GetState() != Enemy.DEAD_STATE)
				{
					DamageProperty damageProperty3 = new DamageProperty();
					damageProperty3.hitForce = (dir + new Vector3(0f, 0.18f, 0f)) * hitForce;
					damageProperty3.damage = damage * this.player.PowerBuff;
					enemyByID.OnHit(damageProperty3, gunType, false, this.player);
				}
			}
			else if (other.gameObject.layer == 19)
			{
				WoodBoxScript component3 = other.gameObject.GetComponent<WoodBoxScript>();
				component3.OnHit(damage * this.player.PowerBuff);
			}
			else
			{
				if (other.gameObject.layer != 8 || !GameApp.GetInstance().GetGameState().VS_mode || this.player.PlayerObject == other.gameObject)
				{
					return;
				}
				Player player = other.gameObject.GetComponent<PlayerShell>().m_player;
				if (player != null && player.GetPlayerState() != player.DEAD_STATE)
				{
					Debug.Log("hit player:" + damage * this.player.PowerBuff);
					if (damage > 0f)
					{
						player.OnVsInjured(this.player.sfs_user, damage * this.player.PowerBuff, (int)gunType);
					}
				}
			}
		}
		else if (gunType == WeaponType.Sword)
		{
			UnityEngine.Object.Instantiate(rConf.swordAttack, proTransform.position, Quaternion.identity);
			if (other.gameObject.layer == 9)
			{
				Enemy enemyByID2 = gameScene.GetEnemyByID(other.gameObject.name);
				if (enemyByID2 != null && enemyByID2.GetState() != Enemy.DEAD_STATE)
				{
					DamageProperty damageProperty4 = new DamageProperty();
					damageProperty4.hitForce = (dir + new Vector3(0f, 0.18f, 0f)) * hitForce;
					damageProperty4.damage = damage * this.player.PowerBuff;
					enemyByID2.OnHit(damageProperty4, gunType, false, this.player);
				}
			}
			else if (other.gameObject.layer == 19)
			{
				WoodBoxScript component4 = other.gameObject.GetComponent<WoodBoxScript>();
				component4.OnHit(damage * this.player.PowerBuff);
			}
			else if (other.gameObject.layer == 15 || other.gameObject.layer == 11)
			{
				UnityEngine.Object.DestroyObject(base.gameObject);
			}
		}
		else
		{
			if (gunType != WeaponType.NurseSaliva)
			{
				return;
			}
			Player player2 = gameScene.GetPlayer();
			if (other.gameObject.layer == 9)
			{
				return;
			}
			Ray ray = new Ray(proTransform.position + Vector3.up * 1f, Vector3.down);
			float num5 = 10000.1f;
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 100f, 32768))
			{
				num5 = hitInfo.point.y;
			}
			UnityEngine.Object.Instantiate(smallExplodeObject, new Vector3(proTransform.position.x, num5 + 0.1f, proTransform.position.z), Quaternion.identity);
			GameObject gameObject3 = UnityEngine.Object.Instantiate(rConf.nurseSaliva, new Vector3(proTransform.position.x, num5 + 0.1f, proTransform.position.z), Quaternion.Euler(0f, 0f, 0f));
			gameObject3.transform.Rotate(270f, 0f, 0f);
			AudioSource component5 = gameObject3.GetComponent<AudioSource>();
			if (component5 != null)
			{
				component5.mute = !GameApp.GetInstance().GetGameState().SoundOn;
			}
			UnityEngine.Object.DestroyObject(base.gameObject);
			if (GameApp.GetInstance().GetGameState().endless_multiplayer)
			{
				List<Player> list = new List<Player>();
				foreach (Player item in GameApp.GetInstance().GetGameScene().m_multi_player_arr)
				{
					if ((item.GetTransform().position - proTransform.position).sqrMagnitude < explodeRadius * explodeRadius)
					{
						list.Add(item);
					}
				}
				{
					foreach (Player item2 in list)
					{
						item2.OnHit(damage * player2.PowerBuff);
					}
					return;
				}
			}
			if ((player2.GetTransform().position - proTransform.position).sqrMagnitude < explodeRadius * explodeRadius)
			{
				player2.OnHit(damage * player2.PowerBuff);
			}
		}
	}
}
