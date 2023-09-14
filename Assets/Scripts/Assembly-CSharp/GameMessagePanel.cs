using System.Collections.Generic;
using UnityEngine;

public class GameMessagePanel : MonoBehaviour
{
	public int height = 20;

	protected List<GameObject> message_cells = new List<GameObject>();

	private void Awake()
	{
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void AddSFSRoom(string msg)
	{
		GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/TUI/VsGameMessage")) as GameObject;
		gameObject.name = "VsMessage";
		gameObject.transform.parent = base.gameObject.transform;
		gameObject.GetComponent<TUIMeshText>().text = msg;
		message_cells.Add(gameObject);
		int num = 0;
		for (int num2 = message_cells.Count - 1; num2 >= 0; num2--)
		{
			message_cells[num2].transform.localPosition = new Vector3(gameObject.transform.localPosition.x, 100 - num * height, gameObject.transform.localPosition.z);
			num++;
		}
	}

	public void RemoveMsgObj(GameObject msg_obj)
	{
		if (message_cells.Contains(msg_obj))
		{
			message_cells.Remove(msg_obj);
		}
	}
}
