using UnityEngine;
using Zombie3D;

public class WoodBoxScript : MonoBehaviour
{
	public float hp = 10f;

	protected ResourceConfigScript rConf;

	protected Transform boxTransform;

	protected float startTime;

	protected bool m_enable;

	private void Start()
	{
		boxTransform = base.gameObject.transform;
		GetComponent<Rigidbody>().useGravity = false;
		startTime = Time.time;
		if (GameApp.GetInstance().GetGameState().VS_mode)
		{
			base.gameObject.active = false;
		}
	}

	private void Update()
	{
		if (!GameApp.GetInstance().GetGameState().VS_mode)
		{
			if (Time.time - startTime > 20f)
			{
				GetComponent<Rigidbody>().useGravity = true;
			}
			if (base.transform.position.y < 10030.1f)
			{
				GetComponent<Renderer>().enabled = true;
			}
			else
			{
				GetComponent<Renderer>().enabled = false;
			}
		}
	}

	public bool OnHit(float damage)
	{
		if (!base.gameObject.active)
		{
			return false;
		}
		bool result = false;
		rConf = GameApp.GetInstance().GetResourceConfig();
		hp -= damage;
		if (hp <= 0f)
		{
			Player player = GameApp.GetInstance().GetGameScene().GetPlayer();
			Weapon weapon = player.GetWeapon();
			if (weapon.GetWeaponType() == WeaponType.Sword)
			{
				Object.Instantiate(rConf.swordAttack, base.transform.position + base.transform.up * 1f, Quaternion.identity);
			}
			Object.Destroy(base.gameObject);
			GameObject gameObject = Object.Instantiate(rConf.woodExplode, base.transform.position, Quaternion.identity);
			AudioSource component = gameObject.GetComponent<AudioSource>();
			if (component != null)
			{
				component.mute = !GameApp.GetInstance().GetGameState().SoundOn;
			}
			SendMessage("OnLoot", false);
			result = true;
		}
		return result;
	}

	public void CheckEnable()
	{
		Debug.Log(Time.time - startTime);
		if (!m_enable && Time.time - startTime > 20f)
		{
			base.gameObject.SetActiveRecursively(true);
			m_enable = true;
		}
	}
}
