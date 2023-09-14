using UnityEngine;
using Zombie3D;

public class GameScript : MonoBehaviour
{
	private void Awake()
	{
		CheckGameResourceConfig();
		CheckEnemyResourceConfig();
		CheckGlobalResourceConfig();
	}

	private void Start()
	{
		GameApp.GetInstance().Init();
		GameApp.GetInstance().CreateScene();
	}

	private void Update()
	{
		GameApp.GetInstance().Loop(Time.deltaTime);
	}

	public static void CheckGameResourceConfig()
	{
		if (GameObject.Find("GameResourceConfig") == null)
		{
			GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/GameResourceConfig")) as GameObject;
			gameObject.name = "GameResourceConfig";
		}
	}

	public static void CheckEnemyResourceConfig()
	{
		if (GameObject.Find("EnemyResourceConfig") == null)
		{
			GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/EnemyResourceConfig")) as GameObject;
			gameObject.name = "EnemyResourceConfig";
		}
	}

	public static void CheckGlobalResourceConfig()
	{
		if (GameObject.Find("GlobalResourceConfig") == null)
		{
			GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/Global")) as GameObject;
			gameObject.name = "GlobalResourceConfig";
			Object.DontDestroyOnLoad(gameObject);
		}
	}

	public static void CheckMenuResourceConfig()
	{
		if (GameObject.Find("MenuResourceConfig") == null)
		{
			GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/MenuResourceConfig")) as GameObject;
			gameObject.name = "MenuResourceConfig";
		}
	}

	public static void CheckNetWorkCom()
	{
		if (GameObject.Find("NetworkObjHall") == null)
		{
			GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/NetworkObj")) as GameObject;
			gameObject.name = "NetworkObjHall";
			Object.DontDestroyOnLoad(gameObject);
			GameApp.GetInstance().GetGameState().net_com_hall = gameObject.GetComponent<NetworkObj>();
			Debug.Log("CheckNetWorkCom, Create NetworkObjHall...");
		}
		if (GameObject.Find("NetworkObj") == null)
		{
			GameObject gameObject2 = Object.Instantiate(Resources.Load("Prefabs/NetworkObj")) as GameObject;
			gameObject2.name = "NetworkObj";
			Object.DontDestroyOnLoad(gameObject2);
			GameApp.GetInstance().GetGameState().net_com = gameObject2.GetComponent<NetworkObj>();
			Debug.Log("CheckNetWorkCom, Create NetworkObj...");
		}
	}
}
