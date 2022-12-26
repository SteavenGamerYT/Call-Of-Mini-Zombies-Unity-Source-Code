using UnityEngine;
using Zombie3D;

public class NickNameUIPanel : UIPanel, UIHandler
{
	protected UIImage back_img;

	protected UIClickButton return_Button;

	protected UITextButton go_button;

	protected UIText nick_name_text;

	protected NickNameUIPosition uiPos;

	protected Timer fadeTimer = new Timer();

	public void Start()
	{
		uiPos = new NickNameUIPosition();
		GameApp.GetInstance().InitForMenu();
		Material material = UIResourceMgr.GetInstance().GetMaterial("ArenaMenu");
		Material material2 = UIResourceMgr.GetInstance().GetMaterial("ShopUI");
		Material material3 = UIResourceMgr.GetInstance().GetMaterial("Buttons");
		back_img = new UIImage();
		back_img.SetTexture(material, ArenaMenuTexturePosition.Background, AutoRect.AutoSize(ArenaMenuTexturePosition.Background));
		back_img.Rect = AutoRect.AutoPos(uiPos.Background);
		return_Button = new UIClickButton();
		return_Button.SetTexture(UIButtonBase.State.Normal, material, ArenaMenuTexturePosition.ReturnButtonNormal, AutoRect.AutoSize(ArenaMenuTexturePosition.ReturnButtonNormal));
		return_Button.SetTexture(UIButtonBase.State.Pressed, material, ArenaMenuTexturePosition.ReturnButtonPressed, AutoRect.AutoSize(ArenaMenuTexturePosition.ReturnButtonPressed));
		return_Button.Rect = AutoRect.AutoPos(uiPos.ReturnButton);
		go_button = new UITextButton();
		go_button.SetTexture(UIButtonBase.State.Normal, material3, ButtonsTexturePosition.ButtonNormal, AutoRect.AutoSize(ButtonsTexturePosition.ButtonNormal, 0.8f));
		go_button.SetTexture(UIButtonBase.State.Pressed, material3, ButtonsTexturePosition.ButtonPressed, AutoRect.AutoSize(ButtonsTexturePosition.ButtonPressed, 0.8f));
		go_button.Rect = AutoRect.AutoPos(uiPos.GoButton);
		go_button.SetText("font1", "GO", ColorName.fontColor_orange);
		go_button.Visible = false;
		go_button.Enable = false;
		nick_name_text = new UIText();
		nick_name_text.Set("font1", string.Empty, ColorName.fontColor_darkorange);
		nick_name_text.AlignStyle = UIText.enAlignStyle.center;
		nick_name_text.Rect = AutoRect.AutoPos(uiPos.NickName);
		Add(back_img);
		Add(return_Button);
		Add(go_button);
		Add(nick_name_text);
		SetUIHandler(this);
	}

	public override void Update()
	{
		if (fadeTimer.Ready())
		{
			if (fadeTimer.Name == "Back")
			{
				UIResourceMgr.GetInstance().UnloadAllUIMaterials();
				GameApp.GetInstance().GetGameState().FromShopMenu = true;
				SceneName.LoadLevel("MainMapTUI");
			}
			else if (fadeTimer.Name == "Go")
			{
				UIResourceMgr.GetInstance().UnloadAllUIMaterials();
				GameApp.GetInstance().GetGameState().FromShopMenu = true;
				SceneName.LoadLevel("MultiplayRoomTUI");
			}
			fadeTimer.Do();
		}
	}

	public void HandleEvent(UIControl control, int command, float wparam, float lparam)
	{
		if (control == return_Button)
		{
			FadeAnimationScript.GetInstance().FadeInBlack(0.3f);
			fadeTimer.Name = "Back";
			fadeTimer.SetTimer(0.3f, false);
		}
		else if (control == go_button)
		{
			FadeAnimationScript.GetInstance().FadeInBlack(0.3f);
			fadeTimer.Name = "Go";
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
