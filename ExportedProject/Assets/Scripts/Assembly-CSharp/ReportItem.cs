using UnityEngine;

public class ReportItem : MonoBehaviour
{
	public TUIMeshText Label_Content_Name;

	public TUIMeshText Label_Content_KillCount;

	public TUIMeshText Label_Content_Death;

	public TUIMeshText Label_Content_Money;

	public TUIMeshText Label_Content_Damage;

	public GameObject Win_Tip;

	public void SetContent(string name, int kill_count, int death_count, int cash_loot, float damage_val)
	{
		Label_Content_Name.text = name;
		Label_Content_KillCount.text = kill_count.ToString();
		Label_Content_Death.text = death_count.ToString();
		Label_Content_Money.text = "$" + cash_loot;
		Label_Content_Damage.text = ((int)damage_val).ToString();
	}
}
