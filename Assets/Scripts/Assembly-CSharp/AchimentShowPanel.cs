using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zombie3D;

public class AchimentShowPanel : MonoBehaviour
{
	public List<GameObject> achievement_buttons = new List<GameObject>();

	public void Awake()
	{
		int num = 0;
		string empty = string.Empty;
		IEnumerator enumerator = GameApp.GetInstance().GetGameConfig().Multi_AchievementConfTable.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				MultiAchievementCfg multiAchievementCfg = (MultiAchievementCfg)enumerator.Current;
				if (multiAchievementCfg.level == 1)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/TUI/Achivment_Button")) as GameObject;
					gameObject.name = "Achivment_Button" + multiAchievementCfg.m_class;
					gameObject.transform.parent = base.gameObject.transform;
					gameObject.GetComponent<AchievementData>().achievement_data = multiAchievementCfg;
					achievement_buttons.Add(gameObject);
					int num2 = multiAchievementCfg.icon.IndexOf('_');
					empty = multiAchievementCfg.icon.Substring(0, num2 + 1) + "0";
					gameObject.GetComponent<TUIButtonClick>().frameNormal.GetComponent<TUIMeshSprite>().frameName = empty;
					gameObject.GetComponent<TUIButtonClick>().framePressed.GetComponent<TUIMeshSprite>().frameName = empty;
					gameObject.GetComponent<TUIButtonClick>().frameDisabled.GetComponent<TUIMeshSprite>().frameName = empty;
					gameObject.transform.localPosition = new Vector3(-175 + num % 6 * 70, 84 - num / 6 * 80, gameObject.transform.localPosition.z);
					num++;
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = enumerator as IDisposable) != null)
			{
				disposable.Dispose();
			}
		}
		GameObject gameObject2 = null;
		num = 0;
		IEnumerator enumerator2 = GameApp.GetInstance().GetGameConfig().Multi_AchievementConfTable.GetEnumerator();
		try
		{
			while (enumerator2.MoveNext())
			{
				MultiAchievementCfg multiAchievementCfg2 = (MultiAchievementCfg)enumerator2.Current;
				gameObject2 = FindAchivmentItem(multiAchievementCfg2.m_class);
				if (gameObject2 != null && multiAchievementCfg2.finish == 1)
				{
					gameObject2.GetComponent<AchievementData>().achievement_data = multiAchievementCfg2;
					gameObject2.GetComponent<TUIButtonClick>().frameNormal.GetComponent<TUIMeshSprite>().frameName = multiAchievementCfg2.icon;
					gameObject2.GetComponent<TUIButtonClick>().framePressed.GetComponent<TUIMeshSprite>().frameName = multiAchievementCfg2.icon;
					gameObject2.GetComponent<TUIButtonClick>().frameDisabled.GetComponent<TUIMeshSprite>().frameName = multiAchievementCfg2.icon;
				}
				else if (gameObject2 != null && multiAchievementCfg2.finish == 0 && gameObject2.GetComponent<AchievementData>().achievement_data.finish == 1 && multiAchievementCfg2.level - gameObject2.GetComponent<AchievementData>().achievement_data.level == 1)
				{
					gameObject2.GetComponent<AchievementData>().achievement_data = multiAchievementCfg2;
				}
				gameObject2 = null;
			}
		}
		finally
		{
			IDisposable disposable2;
			if ((disposable2 = enumerator2 as IDisposable) != null)
			{
				disposable2.Dispose();
			}
		}
	}

	public void Start()
	{
	}

	public void Update()
	{
	}

	public GameObject FindAchivmentItem(string class_name)
	{
		GameObject result = null;
		foreach (GameObject achievement_button in achievement_buttons)
		{
			if (achievement_button.GetComponent<AchievementData>().achievement_data.m_class == class_name)
			{
				return achievement_button;
			}
		}
		return result;
	}
}
