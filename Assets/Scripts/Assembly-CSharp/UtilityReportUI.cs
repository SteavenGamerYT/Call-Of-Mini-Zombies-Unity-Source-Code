using UnityEngine;

public class UtilityReportUI : MonoBehaviour
{
	public int m_pageCount;

	public TUIScroll m_scroll;

	private int m_pageIndex = -1;

	public void Start()
	{
	}

	public void Update()
	{
	}

	private void UpdatePage(int pageIndex)
	{
		m_pageIndex = pageIndex;
	}
}
