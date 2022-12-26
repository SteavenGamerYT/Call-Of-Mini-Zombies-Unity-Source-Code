using UnityEngine;

public class MsgBoxDelegate : MonoBehaviour
{
	public GameObject Content;

	public MsgBoxType m_type;

	public void SetMsgContent(string content, MsgBoxType type)
	{
		if ((bool)Content)
		{
			Content.GetComponent<TUIMeshText>().text = content;
			m_type = type;
		}
	}

	public void SetContentOffset(Vector2 pos)
	{
		if ((bool)Content)
		{
			Content.transform.localPosition = new Vector3(pos.x, pos.y, -1f);
		}
	}

	public void Show(string content, MsgBoxType type)
	{
		if ((bool)Content)
		{
			Content.GetComponent<TUIMeshText>().horizontalAlignment = TUIMeshText.HorizontalAlignment.Left;
			Content.GetComponent<TUIMeshText>().verticalAlignment = TUIMeshText.VerticalAlignment.Top;
			if (type == MsgBoxType.MultiToturial || type == MsgBoxType.NotEnoughUser || type == MsgBoxType.None || type == MsgBoxType.VSPasswordToturial)
			{
				Content.GetComponent<TUIMeshText>().fontName = "CAI-14";
			}
			else
			{
				Content.GetComponent<TUIMeshText>().fontName = "CAI-18";
			}
		}
		int num = 1;
		foreach (char c in content)
		{
			if (c == '\n')
			{
				num++;
			}
		}
		switch (num)
		{
		case 5:
			SetContentOffset(new Vector2(-124f, 50f));
			break;
		case 4:
			SetContentOffset(new Vector2(-124f, 43f));
			break;
		case 3:
			SetContentOffset(new Vector2(-118f, 35f));
			break;
		case 2:
			SetContentOffset(new Vector2(-118f, 21f));
			break;
		case 1:
			SetContentOffset(new Vector2(0f, 10f));
			if ((bool)Content)
			{
				Content.GetComponent<TUIMeshText>().horizontalAlignment = TUIMeshText.HorizontalAlignment.Center;
				Content.GetComponent<TUIMeshText>().verticalAlignment = TUIMeshText.VerticalAlignment.Center;
			}
			break;
		}
		string content2 = content.ToUpper();
		SetMsgContent(content2, type);
		base.gameObject.transform.localPosition = new Vector3(0f, 0f, base.gameObject.transform.localPosition.z);
	}

	public void Hide()
	{
		base.gameObject.transform.localPosition = new Vector3(0f, 1000f, base.gameObject.transform.localPosition.z);
	}
}
