using UnityEngine;

public class VsMsgDestroy : MonoBehaviour
{
	private void OnDestroy()
	{
		if (base.transform.parent.GetComponent<GameMessagePanel>() != null)
		{
			base.transform.parent.GetComponent<GameMessagePanel>().RemoveMsgObj(base.gameObject);
		}
	}
}
