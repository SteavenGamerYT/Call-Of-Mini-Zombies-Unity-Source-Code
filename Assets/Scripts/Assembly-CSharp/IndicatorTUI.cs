using UnityEngine;

public class IndicatorTUI : MonoBehaviour
{
	protected GameObject m_content;

	protected bool is_show;

	public void Start()
	{
		m_content = base.gameObject.transform.Find("BK/Content").gameObject;
		m_content.GetComponent<TUIMeshText>().Static = false;
	}

	public void Update()
	{
	}

	public void SetContent(string str)
	{
		if (m_content != null)
		{
			m_content.GetComponent<TUIMeshText>().text = str;
			m_content.GetComponent<TUIMeshText>().fontName = "CAI-14";
		}
	}

	public void Show()
	{
		if (!is_show)
		{
			bool flag = false;
			int num = 0;
			if (AutoRect.GetPlatform() == Platform.IPad)
			{
				flag = true;
				num = 0;
			}
			else
			{
				flag = false;
				num = 1;
			}
			Utils.ShowIndicatorSystem(num, flag, 1f, 1f, 1f, 1f);
			base.gameObject.transform.localPosition = new Vector3(0f, 0f, base.gameObject.transform.localPosition.z);
			is_show = true;
		}
	}

	public void Hide()
	{
		if (is_show)
		{
			Utils.HideIndicatorSystem();
			base.gameObject.transform.localPosition = new Vector3(0f, 2000f, base.gameObject.transform.localPosition.z);
			is_show = false;
		}
	}

	public void OnDestroy()
	{
		Hide();
	}
}
