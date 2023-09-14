using System.Collections.Generic;
using Sfs2X.Entities;
using UnityEngine;
using Zombie3D;

public class MultiRoomPanel : MonoBehaviour
{
	public GameObject scoll;

	protected int height = -1;

	protected int cur_index;

	protected NetworkObj net_com;

	protected List<GameObject> Room_Cell_Arr = new List<GameObject>();

	protected List<TUIButtonClick> Room_Cell_Button_Arr = new List<TUIButtonClick>();

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

	public void OnScrollBegin()
	{
	}

	public void OnScrollMove()
	{
	}

	public void OnScrollEnd()
	{
		foreach (TUIButtonClick item in Room_Cell_Button_Arr)
		{
			item.Reset();
		}
	}

	public void ResetRoomCells()
	{
		foreach (GameObject item in Room_Cell_Arr)
		{
			Object.Destroy(item);
		}
		Room_Cell_Arr.Clear();
		Room_Cell_Button_Arr.Clear();
		cur_index = 0;
	}

	public void AddRoomInfo(GCRoomListPacket.RoomInfo room_info)
	{
		GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/TUI/RoomCell")) as GameObject;
		gameObject.name = "RoomCell" + cur_index;
		gameObject.transform.parent = base.gameObject.transform;
		gameObject.GetComponent<RoomInfoData>().Room_info_data = room_info;
		Room_Cell_Arr.Add(gameObject);
		GameObject gameObject2 = gameObject.transform.Find("R_Button").gameObject;
		gameObject2.name = "R_Button_" + cur_index;
		Room_Cell_Button_Arr.Add(gameObject2.GetComponent<TUIButtonClick>());
		GameObject gameObject3 = gameObject.transform.Find("Label_ID").gameObject;
		gameObject3.GetComponent<TUIMeshText>().text = room_info.m_iRoomId.ToString();
		GameObject gameObject4 = gameObject.transform.Find("Label_Name").gameObject;
		gameObject4.GetComponent<TUIMeshText>().text = room_info.m_strCreaterNickname;
		GameObject gameObject5 = gameObject.transform.Find("Label_Count").gameObject;
		gameObject5.GetComponent<TUIMeshText>().text = room_info.m_iOnlineNum + "/" + room_info.m_iMaxUserNum;
		if (room_info.m_room_status != 0)
		{
			gameObject3.GetComponent<TUIMeshText>().color = Color.gray;
			gameObject4.GetComponent<TUIMeshText>().color = Color.gray;
			gameObject5.GetComponent<TUIMeshText>().color = Color.gray;
		}
		else
		{
			gameObject3.GetComponent<TUIMeshText>().color = ColorName.fontColor_orange;
			gameObject4.GetComponent<TUIMeshText>().color = ColorName.fontColor_orange;
			gameObject5.GetComponent<TUIMeshText>().color = ColorName.fontColor_orange;
		}
		if (height == -1)
		{
			height = (int)gameObject2.GetComponent<TUIButtonClick>().size.y + 2;
		}
		gameObject.transform.localPosition = new Vector3(0f, 44 - cur_index * height, gameObject.transform.localPosition.z);
		cur_index++;
		if ((bool)scoll)
		{
			if (cur_index > 3)
			{
				scoll.GetComponent<TUIScroll>().rangeYMax = (cur_index - 3) * height;
				scoll.GetComponent<TUIScroll>().borderYMax = (cur_index - 3) * height;
			}
			else
			{
				scoll.GetComponent<TUIScroll>().rangeYMax = 0f;
				scoll.GetComponent<TUIScroll>().borderYMax = 0f;
			}
		}
	}

	public void AddRoomElement(HostData element)
	{
		string[] array = element.comment.Split(' ');
		if (array[1] != GameApp.GetInstance().GetGameState().cur_net_map)
		{
			return;
		}
		GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/TUI/VSRoomCell")) as GameObject;
		gameObject.name = "VSRoomCell" + cur_index;
		gameObject.transform.parent = base.gameObject.transform;
		gameObject.GetComponent<VSRoomData>().Room_data = element;
		Room_Cell_Arr.Add(gameObject);
		GameObject gameObject2 = gameObject.transform.Find("R_Button").gameObject;
		gameObject2.name = "R_Button_" + cur_index;
		Room_Cell_Button_Arr.Add(gameObject2.GetComponent<TUIButtonClick>());
		int length = element.gameName.IndexOf(' ');
		string text = element.gameName.Substring(0, length);
		GameObject gameObject3 = gameObject.transform.Find("Label_Name").gameObject;
		gameObject3.GetComponent<TUIMeshText>().text = text;
		GameObject gameObject4 = gameObject.transform.Find("Label_Count").gameObject;
		gameObject4.GetComponent<TUIMeshText>().text = element.connectedPlayers + "/" + element.playerLimit;
		if (array[0] == "open")
		{
			gameObject3.GetComponent<TUIMeshText>().color = ColorName.fontColor_orange;
			gameObject4.GetComponent<TUIMeshText>().color = ColorName.fontColor_orange;
		}
		else
		{
			gameObject3.GetComponent<TUIMeshText>().color = Color.gray;
			gameObject4.GetComponent<TUIMeshText>().color = Color.gray;
		}
		if (height == -1)
		{
			height = (int)gameObject2.GetComponent<TUIButtonClick>().size.y + 2;
		}
		gameObject.transform.localPosition = new Vector3(0f, 44 - cur_index * height, gameObject.transform.localPosition.z);
		cur_index++;
		if ((bool)scoll)
		{
			if (cur_index > 3)
			{
				scoll.GetComponent<TUIScroll>().rangeYMax = (cur_index - 3) * height;
				scoll.GetComponent<TUIScroll>().borderYMax = (cur_index - 3) * height;
			}
			else
			{
				scoll.GetComponent<TUIScroll>().rangeYMax = 0f;
				scoll.GetComponent<TUIScroll>().borderYMax = 0f;
			}
		}
	}

	public void AddSFSRoom(Room room)
	{
		string[] array = room.Name.Split('|');
		GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/TUI/VSRoomCell")) as GameObject;
		gameObject.name = "VSRoomCell" + cur_index;
		gameObject.transform.parent = base.gameObject.transform;
		gameObject.GetComponent<SFSRoomData>().Room_data = room;
		Room_Cell_Arr.Add(gameObject);
		GameObject gameObject2 = gameObject.transform.Find("R_Button").gameObject;
		gameObject2.name = "R_Button_" + cur_index;
		Room_Cell_Button_Arr.Add(gameObject2.GetComponent<TUIButtonClick>());
		string text = array[0] + "'s Room";
		GameObject gameObject3 = gameObject.transform.Find("Label_Name").gameObject;
		gameObject3.GetComponent<TUIMeshText>().text = text;
		GameObject gameObject4 = gameObject.transform.Find("Label_Count").gameObject;
		gameObject4.GetComponent<TUIMeshText>().text = room.UserCount + "/" + room.Capacity;
		GameObject gameObject5 = gameObject.transform.Find("Lock_tip").gameObject;
		gameObject5.active = room.IsPasswordProtected;
		GameObject gameObject6 = gameObject.transform.Find("Label_Battle").gameObject;
		if (array[2] == "open")
		{
			gameObject6.active = false;
		}
		else
		{
			gameObject6.active = true;
		}
		if (height == -1)
		{
			height = (int)gameObject2.GetComponent<TUIButtonClick>().size.y + 2;
		}
		gameObject.transform.localPosition = new Vector3(0f, 44 - cur_index * height, gameObject.transform.localPosition.z);
		cur_index++;
		if ((bool)scoll)
		{
			if (cur_index > 3)
			{
				scoll.GetComponent<TUIScroll>().rangeYMax = (cur_index - 3) * height;
				scoll.GetComponent<TUIScroll>().borderYMax = (cur_index - 3) * height;
			}
			else
			{
				scoll.GetComponent<TUIScroll>().rangeYMax = 0f;
				scoll.GetComponent<TUIScroll>().borderYMax = 0f;
			}
		}
	}
}
