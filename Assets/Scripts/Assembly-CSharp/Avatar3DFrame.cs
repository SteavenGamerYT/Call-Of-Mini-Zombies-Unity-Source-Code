using UnityEngine;
using Zombie3D;

public class Avatar3DFrame : UI3DFrame
{
	protected Vector3 scale;

	protected float lastMotionTime;

	public Avatar3DFrame(Rect rect, Vector3 pos, Vector3 scale)
		: base(rect, pos)
	{
		this.scale = scale;
		ChangeAvatar(GameApp.GetInstance().GetGameState().Avatar);
	}

	public void ChangeAvatar(AvatarType aType)
	{
		ClearModels();
		GameObject gameObject = AvatarFactory.GetInstance().CreateAvatar(aType);
		gameObject.transform.rotation = Quaternion.Euler(0f, 200f, 0f);
		Weapon weapon = GameApp.GetInstance().GetGameState().GetBattleWeapons()[0];
		string name = weapon.Name;
		string weaponNameEnd = Weapon.GetWeaponNameEnd(weapon.GetWeaponType());
		GameObject gameObject2 = WeaponFactory.GetInstance().CreateWeaponModel(name, gameObject.transform.position, gameObject.transform.rotation);
		Transform parent = gameObject.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand/Weapon_Dummy");
		gameObject2.transform.parent = parent;
		gameObject.transform.localScale = scale;
		gameObject.GetComponent<Animation>()["Idle01" + weaponNameEnd].wrapMode = WrapMode.Loop;
		gameObject.GetComponent<Animation>().Play("Idle01" + weaponNameEnd);
		SetModel(gameObject);
		lastMotionTime = Time.time;
	}

	public void UpdateAnimation()
	{
		GameObject model = GetModel();
		Weapon weapon = GameApp.GetInstance().GetGameState().GetBattleWeapons()[0];
		string weaponNameEnd = Weapon.GetWeaponNameEnd(weapon.GetWeaponType());
		if (!(model != null))
		{
			return;
		}
		if (weapon.GetWeaponType() == WeaponType.RocketLauncher || weapon.GetWeaponType() == WeaponType.Sniper)
		{
			if (Time.time - lastMotionTime > 7f)
			{
				string empty = string.Empty;
				empty = (model.GetComponent<Animation>().IsPlaying("Run01" + weaponNameEnd) ? ("Idle01" + weaponNameEnd) : ("Run01" + weaponNameEnd));
				model.GetComponent<Animation>()[empty].wrapMode = WrapMode.Loop;
				model.GetComponent<Animation>().CrossFade(empty);
				lastMotionTime = Time.time;
			}
		}
		else if (weapon.GetWeaponType() == WeaponType.Saw || weapon.GetWeaponType() == WeaponType.Sword)
		{
			if (Time.time - lastMotionTime > 7f)
			{
				model.GetComponent<Animation>()["Shoot01_Saw2"].wrapMode = WrapMode.ClampForever;
				model.GetComponent<Animation>().CrossFade("Shoot01_Saw");
				model.GetComponent<Animation>().CrossFadeQueued("Shoot01_Saw2");
				lastMotionTime = Time.time;
			}
			if (model.GetComponent<Animation>().IsPlaying("Shoot01_Saw2") && Time.time - lastMotionTime > model.GetComponent<Animation>()["Shoot01_Saw2"].clip.length * 2f)
			{
				model.GetComponent<Animation>().CrossFade("Idle01" + weaponNameEnd);
			}
		}
		else
		{
			if (Time.time - lastMotionTime > 7f)
			{
				model.GetComponent<Animation>()["Standby03"].wrapMode = WrapMode.ClampForever;
				model.GetComponent<Animation>().CrossFade("Standby03");
				lastMotionTime = Time.time;
			}
			if (model.GetComponent<Animation>()["Standby03"].time > model.GetComponent<Animation>()["Standby03"].clip.length)
			{
				model.GetComponent<Animation>().CrossFade("Idle01" + weaponNameEnd);
			}
		}
	}
}
