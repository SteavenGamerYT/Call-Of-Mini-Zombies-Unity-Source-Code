using UnityEngine;
using Zombie3D;

public class EndlessModeUIPanel : UIPanel, UIHandler
{
	protected UIImage back_img;

	protected UIClickButton return_Button;

	protected UITextButton Single_Button;

	protected UITextButton Multi_Button;

	protected EndlessModeUIPosition uiPos;

	protected Timer fadeTimer = new Timer();

	public void Start()
	{
		uiPos = new EndlessModeUIPosition();
		GameApp.GetInstance().InitForMenu();
		Material material = UIResourceMgr.GetInstance().GetMaterial("ArenaMenu");
		Material material2 = UIResourceMgr.GetInstance().GetMaterial("ShopUI");
		back_img = new UIImage();
		back_img.SetTexture(material, ArenaMenuTexturePosition.Background, AutoRect.AutoSize(ArenaMenuTexturePosition.Background));
		back_img.Rect = AutoRect.AutoPos(uiPos.Background);
		return_Button = new UIClickButton();
		return_Button.SetTexture(UIButtonBase.State.Normal, material, ArenaMenuTexturePosition.ReturnButtonNormal, AutoRect.AutoSize(ArenaMenuTexturePosition.ReturnButtonNormal));
		return_Button.SetTexture(UIButtonBase.State.Pressed, material, ArenaMenuTexturePosition.ReturnButtonPressed, AutoRect.AutoSize(ArenaMenuTexturePosition.ReturnButtonPressed));
		return_Button.Rect = AutoRect.AutoPos(uiPos.ReturnButton);
		Material material3 = UIResourceMgr.GetInstance().GetMaterial("Buttons");
		Single_Button = new UITextButton();
		Single_Button.SetTexture(UIButtonBase.State.Normal, material3, ButtonsTexturePosition.ButtonNormal, AutoRect.AutoSize(ButtonsTexturePosition.ButtonNormal));
		Single_Button.SetTexture(UIButtonBase.State.Pressed, material3, ButtonsTexturePosition.ButtonPressed, AutoRect.AutoSize(ButtonsTexturePosition.ButtonPressed));
		Single_Button.Rect = AutoRect.AutoPos(uiPos.Single_Button);
		Single_Button.SetText("font1", "Single", ColorName.fontColor_orange);
		Multi_Button = new UITextButton();
		Multi_Button.SetTexture(UIButtonBase.State.Normal, material3, ButtonsTexturePosition.ButtonNormal, AutoRect.AutoSize(ButtonsTexturePosition.ButtonNormal));
		Multi_Button.SetTexture(UIButtonBase.State.Pressed, material3, ButtonsTexturePosition.ButtonPressed, AutoRect.AutoSize(ButtonsTexturePosition.ButtonPressed));
		Multi_Button.Rect = AutoRect.AutoPos(uiPos.Multi_Button);
		Multi_Button.SetText("font1", "Multiplay", ColorName.fontColor_orange);
		Add(back_img);
		Add(return_Button);
		Add(Single_Button);
		Add(Multi_Button);
		SetUIHandler(this);
	}

	public override void Update()
	{
		if (!fadeTimer.Ready())
		{
			return;
		}
		if (fadeTimer.Name == "Back")
		{
			UIResourceMgr.GetInstance().UnloadAllUIMaterials();
			GameApp.GetInstance().GetGameState().FromShopMenu = true;
			SceneName.LoadLevel("MainMapTUI");
		}
		else if (fadeTimer.Name == "Single")
		{
			UIResourceMgr.GetInstance().UnloadAllUIMaterials();
			GameApp.GetInstance().GetGameState().FromShopMenu = true;
			SceneName.LoadLevel("EndlessMenuUI");
		}
		else if (fadeTimer.Name == "Multi")
		{
			UIResourceMgr.GetInstance().UnloadAllUIMaterials();
			GameApp.GetInstance().GetGameState().FromShopMenu = true;
			if (GameApp.GetInstance().GetGameState().multiplay_named == 0)
			{
				SceneName.LoadLevel("NickNameTUI");
			}
			else
			{
				SceneName.LoadLevel("MultiplayRoomTUI");
			}
		}
		fadeTimer.Do();
	}

	public void HandleEvent(UIControl control, int command, float wparam, float lparam)
	{
		if (control == return_Button)
		{
			FadeAnimationScript.GetInstance().FadeInBlack(0.3f);
			fadeTimer.Name = "Back";
			fadeTimer.SetTimer(0.3f, false);
		}
		else if (control == Single_Button)
		{
			FadeAnimationScript.GetInstance().FadeInBlack(0.3f);
			fadeTimer.Name = "Single";
			fadeTimer.SetTimer(0.3f, false);
		}
		else if (control == Multi_Button)
		{
			FadeAnimationScript.GetInstance().FadeInBlack(0.3f);
			fadeTimer.Name = "Multi";
			fadeTimer.SetTimer(0.3f, false);
		}
	}

	public override void Hide()
	{
		base.Hide();
	}

	public override void Show()
	{
		base.Show();
	}
}
