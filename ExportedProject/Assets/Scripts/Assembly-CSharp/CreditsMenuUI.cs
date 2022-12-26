using UnityEngine;
using Zombie3D;

public class CreditsMenuUI : UIPanel, UIHandler
{
	protected Rect[] buttonRect;

	protected Material creditsMenuMaterial;

	protected Material backgroundMenuMaterial;

	protected UIImage background;

	protected UIImage dialog;

	protected UIImage titleImage;

	protected UIClickButton returnButton;

	protected UIClickButton okButton;

	protected UIText designerText;

	protected UIText artistText;

	protected UIText programmerText;

	protected UIText qaText;

	protected UIImage creditsBackground;

	protected UIClickButton creditsReturnButton;

	private CreditsMenuUIPosition uiPos;

	private CreditsMenuTexturePosition texPos;

	protected float screenRatioX;

	protected float screenRatioY;

	protected GameState gameState;

	protected GameObject optionsUI;

	protected bool enableBackground = true;

	protected ArenaMenuUI ui;

	public CreditsMenuUI()
	{
		uiPos = new CreditsMenuUIPosition();
		texPos = new CreditsMenuTexturePosition();
		gameState = GameApp.GetInstance().GetGameState();
		if (enableBackground)
		{
			backgroundMenuMaterial = UIResourceMgr.GetInstance().GetMaterial("ArenaMenu");
			Material material = UIResourceMgr.GetInstance().GetMaterial("Credits");
			background = new UIImage();
			background.SetTexture(material, CreditsMenuTexturePosition.Background, AutoRect.AutoSize(CreditsMenuTexturePosition.Background));
			background.Rect = AutoRect.AutoPos(uiPos.Background);
			titleImage = new UIImage();
			titleImage.SetTexture(backgroundMenuMaterial, texPos.TitleImage);
			titleImage.Rect = uiPos.TitleImage;
			returnButton = new UIClickButton();
			returnButton.SetTexture(UIButtonBase.State.Normal, backgroundMenuMaterial, ArenaMenuTexturePosition.ReturnButtonNormal, AutoRect.AutoSize(ArenaMenuTexturePosition.ReturnButtonNormal));
			returnButton.SetTexture(UIButtonBase.State.Pressed, backgroundMenuMaterial, ArenaMenuTexturePosition.ReturnButtonPressed, AutoRect.AutoSize(ArenaMenuTexturePosition.ReturnButtonPressed));
			returnButton.Rect = AutoRect.AutoPos(uiPos.ReturnButton);
		}
		okButton = new UIClickButton();
		okButton.SetTexture(UIButtonBase.State.Normal, creditsMenuMaterial, texPos.RightButtonNormal);
		okButton.SetTexture(UIButtonBase.State.Pressed, creditsMenuMaterial, texPos.RightButtonPressed);
		okButton.Rect = uiPos.RightButton;
		designerText = new UIText();
		designerText.Set("font1", "DESIGNER", ColorName.fontColor_orange);
		designerText.Rect = uiPos.DesignerText;
		artistText = new UIText();
		artistText.Set("font1", "ARTIST", ColorName.fontColor_orange);
		artistText.Rect = uiPos.ArtistText;
		programmerText = new UIText();
		programmerText.Set("font1", "PROGRAMMER", ColorName.fontColor_orange);
		programmerText.Rect = uiPos.ProgrammerText;
		qaText = new UIText();
		qaText.Set("font1", "QA", ColorName.fontColor_orange);
		qaText.Rect = uiPos.QAText;
		if (enableBackground)
		{
			Add(background);
			Add(returnButton);
		}
		else
		{
			Add(okButton);
		}
		Hide();
		GameObject gameObject = GameObject.Find("ArenaMenuUI");
		if (gameObject != null)
		{
			ui = gameObject.GetComponent<ArenaMenuUI>();
		}
		SetUIHandler(this);
	}

	private void Awake()
	{
	}

	public void RemoveBackground()
	{
		enableBackground = false;
	}

	public void SetOptionsUI(GameObject obj)
	{
		optionsUI = obj;
	}

	public void HandleEvent(UIControl control, int command, float wparam, float lparam)
	{
		if (control == returnButton || control == okButton)
		{
			MapUI.GetInstance().GetAudioPlayer().PlayAudio("Button");
			Hide();
			MapUI.GetInstance().GetOptionsMenuUI().Show();
		}
	}
}
