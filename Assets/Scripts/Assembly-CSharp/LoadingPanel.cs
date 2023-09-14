using UnityEngine;
using Zombie3D;

public class LoadingPanel : UIPanel
{
	protected UIText m_LoadingText;

	protected UIText m_desc;

	protected int m_step;

	protected int m_frame;

	public LoadingPanel()
	{
		m_LoadingText = new UIText();
		m_LoadingText.Set("font1", "LOADING...", ColorName.fontColor_darkorange);
		m_LoadingText.AlignStyle = UIText.enAlignStyle.left;
		m_LoadingText.Rect = AutoRect.AutoPos(new Rect(360f, 70f, 960f, 100f));
		m_desc = new UIText();
		m_desc.Set("font2", "\n\nYOU WOKE UP TO FIND THAT YOU WERE \n\nTHE ONLY HUMAN LEFT...\n\nTHE ENTIRE TOWN HAD BEEN... ZOMBIFIED...", Color.white);
		m_desc.AlignStyle = UIText.enAlignStyle.center;
		m_desc.Rect = AutoRect.AutoPos(new Rect(0f, 120f, 960f, 640f));
		Add(m_desc);
		if (!GameApp.GetInstance().GetGameState().FirstTimeGame || !(Application.loadedLevelName != "ArenaMenuUI"))
		{
			Add(m_LoadingText);
			int max = AvatarInfo.TIPS_INO.Length;
			int num = Random.Range(0, max);
			m_desc.Rect = AutoRect.AutoPos(new Rect(0f, 0f, 960f, 640f));
			m_desc.SetText(AvatarInfo.TIPS_INO[num]);
		}
		m_step = 0;
		m_frame = 0;
	}

	public override void Show()
	{
		base.Show();
		m_LoadingText.Visible = true;
		m_desc.Visible = true;
	}

	public override void Hide()
	{
		base.Hide();
		m_LoadingText.Visible = false;
		m_desc.Visible = false;
	}

	public override void Update()
	{
		if (m_step % 4 == 0)
		{
			m_LoadingText.Set("font1", "LOADING...", ColorName.fontColor_darkorange);
		}
		else if (m_step % 4 == 1)
		{
			m_LoadingText.Set("font1", "LOADING..", ColorName.fontColor_darkorange);
		}
		else if (m_step % 4 == 2)
		{
			m_LoadingText.Set("font1", "LOADING.", ColorName.fontColor_darkorange);
		}
		else
		{
			m_LoadingText.Set("font1", "LOADING..", ColorName.fontColor_darkorange);
		}
		m_frame++;
		if (m_frame % 5 == 0)
		{
			m_step++;
		}
	}
}
