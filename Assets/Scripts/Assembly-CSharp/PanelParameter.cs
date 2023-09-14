using UnityEngine;

public class PanelParameter : MonoBehaviour
{
	public Vector3 panel_show_pos = new Vector3(0f, 0f, 0f);

	public Vector3 panel_hide_pos = new Vector3(0f, 0f, 0f);

	public void ShowPanel()
	{
		base.gameObject.transform.localPosition = panel_show_pos;
	}

	public void HidePanel()
	{
		base.gameObject.transform.localPosition = panel_hide_pos;
	}
}
