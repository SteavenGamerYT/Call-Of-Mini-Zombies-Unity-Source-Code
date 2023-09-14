using UnityEngine;
using Zombie3D;

public class RoomOwnerPanel : MonoBehaviour
{
	public GameObject scoll;

	protected int height = -1;

	public GameObject[] Client_Arr = new GameObject[4];

	protected NetworkObj net_com;

	public int client_count;

	public void Awake()
	{
		net_com = GameApp.GetInstance().GetGameState().net_com;
	}

	public void Start()
	{
	}

	public void Update()
	{
	}

	public void SetClient(int index, NetUserInfo client)
	{
		GameObject gameObject = Client_Arr[index];
		gameObject.GetComponent<RoomCellData>().Room_cell_data = client;
		GameObject gameObject2 = gameObject.transform.Find("Kick_Button").gameObject;
		if (height == -1)
		{
			height = (int)gameObject2.GetComponent<TUIButtonClick>().size.y + 2;
		}
		if (client != null)
		{
			gameObject2.GetComponent<TUIButtonClick>().enabled = base.enabled;
			if (!GameApp.GetInstance().GetGameState().net_com.m_netUserInfo.is_master)
			{
				gameObject2.GetComponent<TUIButtonClick>().enabled = false;
			}
			else if (GameApp.GetInstance().GetGameState().net_com.m_netUserInfo.is_master && client.room_index == 0)
			{
				gameObject2.GetComponent<TUIButtonClick>().enabled = false;
			}
			GameObject gameObject3 = gameObject.transform.Find("Label_Name").gameObject;
			gameObject3.GetComponent<TUIMeshText>().text = client.nick_name;
			GameObject gameObject4 = gameObject.transform.Find("Label_Job").gameObject;
			gameObject4.GetComponent<TUIMeshText>().text = GetAvataName(client.avatarType);
			GameObject gameObject5 = gameObject.transform.Find("Label_Level").gameObject;
			gameObject5.GetComponent<TUIMeshText>().text = client.levelDays.ToString();
			GameObject gameObject6 = gameObject.transform.Find("Arrow").gameObject;
			gameObject6.GetComponent<TUIMeshSprite>().frameName = "PlayerMark" + client.room_index;
		}
	}

	public void RefrashClientCellShowData()
	{
		for (int i = 0; i < 4; i++)
		{
			if (i == net_com.m_netUserInfo.room_index)
			{
				SetClient(i, net_com.m_netUserInfo);
			}
			else
			{
				SetClient(i, net_com.netUserInfo_array[i]);
			}
		}
	}

	public void OnBeKicked()
	{
	}

	public void RefrashClientCellShow()
	{
		int num = 0;
		GameObject[] client_Arr = Client_Arr;
		GameObject[] array = client_Arr;
		foreach (GameObject gameObject in array)
		{
			if (gameObject.GetComponent<RoomCellData>().Room_cell_data != null && gameObject.GetComponent<RoomCellData>().Room_cell_data.room_index == 0)
			{
				gameObject.transform.localPosition = new Vector3(0f, 44f, gameObject.transform.localPosition.z);
				num++;
				break;
			}
		}
		GameObject[] client_Arr2 = Client_Arr;
		GameObject[] array2 = client_Arr2;
		foreach (GameObject gameObject2 in array2)
		{
			if (gameObject2.GetComponent<RoomCellData>().Room_cell_data != null)
			{
				if (gameObject2.GetComponent<RoomCellData>().Room_cell_data.room_index != 0)
				{
					gameObject2.transform.localPosition = new Vector3(0f, 44 - num * height, gameObject2.transform.localPosition.z);
					num++;
				}
			}
			else
			{
				gameObject2.transform.localPosition = new Vector3(0f, 1000f, gameObject2.transform.localPosition.z);
			}
		}
		client_count = num;
	}

	public string GetAvataName(int avatar)
	{
		string result = string.Empty;
		switch (avatar)
		{
		case 0:
			result = "Joe Blo";
			break;
		case 1:
			result = "Worker";
			break;
		case 2:
			result = "Nerd";
			break;
		case 3:
			result = "Doctor";
			break;
		case 4:
			result = "Cowboy";
			break;
		case 5:
			result = "Swat";
			break;
		case 6:
			result = "Marine";
			break;
		case 7:
			result = "B.E.A.F";
			break;
		case 8:
			result = "Drake";
			break;
		case 9:
			result = "Kunoichi";
			break;
		case 10:
			result = "Priest";
			break;
		case 11:
			result = "Eskimo";
			break;
		}
		return result;
	}

	public void OnScrollBegin()
	{
	}

	public void OnScrollMove()
	{
	}

	public void OnScrollEnd()
	{
	}
}
