using UnityEngine;

namespace Zombie3D
{
	public class AvatarFactory
	{
		protected static AvatarFactory instance;

		public static AvatarFactory GetInstance()
		{
			if (instance == null)
			{
				instance = new AvatarFactory();
			}
			return instance;
		}

		public GameObject CreateAvatar(AvatarType aType)
		{
			GameObject gameObject = null;
			GameObject gameObject2 = null;
			switch (aType)
			{
			case AvatarType.Human:
				gameObject = Object.Instantiate(Resources.Load("Prefabs/Avata/Human")) as GameObject;
				break;
			case AvatarType.Worker:
				gameObject = Object.Instantiate(Resources.Load("Prefabs/Avata/Plumber")) as GameObject;
				break;
			case AvatarType.Marine:
				gameObject = Object.Instantiate(Resources.Load("Prefabs/Avata/Marine")) as GameObject;
				break;
			case AvatarType.EnegyArmor:
				gameObject = Object.Instantiate(Resources.Load("Prefabs/Avata/EnegyArmor")) as GameObject;
				break;
			case AvatarType.Nerd:
				gameObject = Object.Instantiate(Resources.Load("Prefabs/Avata/Nerd")) as GameObject;
				break;
			case AvatarType.Doctor:
				gameObject = Object.Instantiate(Resources.Load("Prefabs/Avata/Doctor")) as GameObject;
				break;
			case AvatarType.Cowboy:
				gameObject = Object.Instantiate(Resources.Load("Prefabs/Avata/Cowboy")) as GameObject;
				break;
			case AvatarType.Swat:
				gameObject = Object.Instantiate(Resources.Load("Prefabs/Avata/Swat")) as GameObject;
				break;
			case AvatarType.Pirate:
				gameObject = Object.Instantiate(Resources.Load("Prefabs/Avata/Corsair")) as GameObject;
				break;
			case AvatarType.Ninja:
				gameObject = Object.Instantiate(Resources.Load("Prefabs/Avata/Ninja")) as GameObject;
				break;
			case AvatarType.Pastor:
				gameObject = Object.Instantiate(Resources.Load("Prefabs/Avata/Pastor")) as GameObject;
				break;
			case AvatarType.Eskimo:
				gameObject = Object.Instantiate(Resources.Load("Prefabs/Avata/Eskimo")) as GameObject;
				break;
			}
			AvataConfigScript component = gameObject.GetComponent<AvataConfigScript>();
			return Object.Instantiate(component.Avata_Instance, Vector3.zero, Quaternion.identity);
		}
	}
}
