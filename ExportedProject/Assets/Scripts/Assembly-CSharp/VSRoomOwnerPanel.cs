using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using UnityEngine;
using Zombie3D;

public class VSRoomOwnerPanel : MonoBehaviour
{
	public GameObject scoll;

	protected int height = -1;

	public GameObject[] Client_Arr;

	public int client_count;

	private void Awake()
	{
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnDestroy()
	{
	}

	private void OnRefrash()
	{
	}

	public void ClearClientsData()
	{
		GameObject[] client_Arr = Client_Arr;
		GameObject[] array = client_Arr;
		foreach (GameObject gameObject in array)
		{
			if (gameObject != null)
			{
				gameObject.GetComponent<VSRoomCellData>().sfs_user = null;
			}
		}
	}

	public void AddClient(User user)
	{
	}

	public void RemoveClient(User user)
	{
		GameObject[] client_Arr = Client_Arr;
		GameObject[] array = client_Arr;
		foreach (GameObject gameObject in array)
		{
			if (gameObject != null && gameObject.GetComponent<VSRoomCellData>().sfs_user != null && gameObject.GetComponent<VSRoomCellData>().sfs_user.Id == user.Id)
			{
				gameObject.GetComponent<VSRoomCellData>().sfs_user = null;
			}
		}
	}

	public bool SetClient(int index, User client)
	{
		Debug.Log("SetClient:" + index + " user:" + client.Name);
		if (index >= 8)
		{
			Debug.LogError("index out of rang!");
			return false;
		}
		GameObject gameObject = Client_Arr[index];
		string[] array = client.Name.Split('|');
		if (gameObject == null)
		{
			Debug.Log("error!:" + index);
			return false;
		}
		gameObject.GetComponent<VSRoomCellData>().sfs_user = client;
		GameObject gameObject2 = gameObject.transform.Find("Kick_Button").gameObject;
		GameObject gameObject3 = gameObject.transform.Find("Avatar_Icon").gameObject;
		if (gameObject3 != null)
		{
			string frameName = string.Empty;
			if (client.Id == SmartFoxConnection.Connection.MySelf.Id)
			{
				frameName = GameApp.GetInstance().GetGameState().Avatar.ToString();
			}
			else if (client.ContainsVariable("RoomState"))
			{
				SFSObject sFSObject = client.GetVariable("RoomState").GetSFSObjectValue() as SFSObject;
				if (sFSObject.GetBool("InRoom"))
				{
					frameName = ((AvatarType)sFSObject.GetInt("avatarType")).ToString();
				}
				else
				{
					gameObject.GetComponent<VSRoomCellData>().sfs_user = null;
					Debug.LogError("user:" + client.Name + ", InRoom state:false, index:" + index);
				}
			}
			gameObject3.GetComponent<TUIMeshSprite>().frameName = frameName;
		}
		if (height == -1)
		{
			height = (int)gameObject2.GetComponent<TUIButtonClick>().size.y + 2;
		}
		if (gameObject.GetComponent<VSRoomCellData>().sfs_user != null)
		{
			gameObject2.GetComponent<TUIButtonClick>().enabled = true;
			if (!SmartFoxConnection.is_server)
			{
				gameObject2.GetComponent<TUIButtonClick>().enabled = false;
			}
			else if (SmartFoxConnection.is_server && index == 0)
			{
				gameObject2.GetComponent<TUIButtonClick>().enabled = false;
			}
			GameObject gameObject4 = gameObject.transform.Find("Label_Name").gameObject;
			gameObject4.GetComponent<TUIMeshText>().text = array[0];
			return true;
		}
		return false;
	}

	public void RefrashClientCellShowData()
	{
		ClearClientsData();
		int num = 0;
		if (FindRoomMaster() != null)
		{
			SetClient(num, FindRoomMaster());
			num++;
		}
		foreach (User user in SmartFoxConnection.Connection.LastJoinedRoom.UserList)
		{
			if (((FindRoomMaster() != null && user.Id != FindRoomMaster().Id) || FindRoomMaster() == null) && SetClient(num, user))
			{
				num++;
			}
		}
	}

	public void RefrashClientCellShow()
	{
		int num = 0;
		Vector3 zero = Vector3.zero;
		GameObject[] client_Arr = Client_Arr;
		GameObject[] array = client_Arr;
		foreach (GameObject gameObject in array)
		{
			if (gameObject != null)
			{
				if (gameObject.GetComponent<VSRoomCellData>().sfs_user != null)
				{
					zero.x = ((num % 2 == 0) ? (-90) : 118);
					zero.y = 70 - num / 2 * 40;
					zero.z = gameObject.transform.localPosition.z;
					gameObject.transform.localPosition = zero;
					num++;
				}
				else
				{
					gameObject.transform.localPosition = new Vector3(0f, 1000f, gameObject.transform.localPosition.z);
				}
			}
		}
		client_count = num;
	}

	public User FindRoomMaster()
	{
		if (SmartFoxConnection.Connection.LastJoinedRoom.ContainsVariable("OwnerId"))
		{
			int intValue = SmartFoxConnection.Connection.LastJoinedRoom.GetVariable("OwnerId").GetIntValue();
			foreach (User user in SmartFoxConnection.Connection.LastJoinedRoom.UserList)
			{
				if (intValue == user.Id)
				{
					return user;
				}
			}
		}
		return null;
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
