using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zombie3D;

public class AchimentReportPanelBase : MonoBehaviour
{
	public List<GameObject> achievement_buttons = new List<GameObject>();

	public void Awake()
	{
	}

	public void Start()
	{
	}

	public void Update()
	{
	}

	public void InitAchivments()
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
					gameObject.transform.localPosition = new Vector3(-181 + num % 6 * 70, 84 - num / 6 * 80, -1f);
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
		IEnumerator enumerator2 = GameApp.GetInstance().GetGameConfig().Multi_AchievementConfTable.GetEnumerator();
		try
		{
			while (enumerator2.MoveNext())
			{
				MultiAchievementCfg achi = (MultiAchievementCfg)enumerator2.Current;
				AddAchivmentItem(achi);
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

	public void AddAchivmentItem(MultiAchievementCfg achi)
	{
		GameObject gameObject = null;
		gameObject = FindAchivmentItem(achi.m_class);
		if (gameObject != null && achi.finish == 1)
		{
			gameObject.GetComponent<AchievementData>().achievement_data = achi;
			gameObject.GetComponent<TUIButtonClick>().frameNormal.GetComponent<TUIMeshSprite>().frameName = achi.icon;
			gameObject.GetComponent<TUIButtonClick>().framePressed.GetComponent<TUIMeshSprite>().frameName = achi.icon;
			gameObject.GetComponent<TUIButtonClick>().frameDisabled.GetComponent<TUIMeshSprite>().frameName = achi.icon;
		}
		else if (gameObject != null && achi.finish == 0 && gameObject.GetComponent<AchievementData>().achievement_data.finish == 1 && achi.level - gameObject.GetComponent<AchievementData>().achievement_data.level == 1)
		{
			gameObject.GetComponent<AchievementData>().achievement_data = achi;
		}
	}

	public void AddAchivmentItemAni(MultiAchievementCfg achi)
	{
		GameObject gameObject = null;
		gameObject = FindAchivmentItem(achi.m_class);
		if (gameObject != null && achi.finish == 1)
		{
			gameObject.GetComponent<AchievementData>().achievement_data = achi;
			GameObject gameObject2 = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/TUI/Achivment_Effact")) as GameObject;
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.transform.localPosition = new Vector3(0f, 0f, -2f - (float)achi.level / 100f);
			gameObject2.GetComponent<AchivAnimateEff>().EffGo(achi.icon);
			gameObject2.GetComponent<AudioSource>().mute = !GameApp.GetInstance().GetGameState().SoundOn;
		}
		else if (gameObject != null && achi.finish == 0 && gameObject.GetComponent<AchievementData>().achievement_data.finish == 1 && achi.level - gameObject.GetComponent<AchievementData>().achievement_data.level == 1)
		{
			gameObject.GetComponent<AchievementData>().achievement_data = achi;
		}
	}

	public void CheckAchivmentItems()
	{
		IEnumerator enumerator = GameApp.GetInstance().GetGameConfig().Multi_AchievementConfTable.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				MultiAchievementCfg multiAchievementCfg = (MultiAchievementCfg)enumerator.Current;
				GameObject gameObject = null;
				gameObject = FindAchivmentItem(multiAchievementCfg.m_class);
				if (gameObject != null && multiAchievementCfg.finish == 0 && gameObject.GetComponent<AchievementData>().achievement_data.finish == 1 && multiAchievementCfg.level - gameObject.GetComponent<AchievementData>().achievement_data.level == 1)
				{
					gameObject.GetComponent<AchievementData>().achievement_data = multiAchievementCfg;
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
	}
}
