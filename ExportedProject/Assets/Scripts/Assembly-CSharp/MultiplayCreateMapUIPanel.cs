using UnityEngine;
using Zombie3D;

public class MultiplayCreateMapUIPanel : UIPanel, UIHandler
{
	protected UIImage back_img;

	protected UITextImage EndlessPanel;

	protected UIClickButton return_Button;

	protected UITextButton go_Button;

	protected UIImageScroller map_Scroller;

	protected EndlessUIPos uiPos;

	protected int cur_stage_index;

	protected Timer fadeTimer = new Timer();

	protected NetworkObj net_com;

	public CreateMapTUI CreateMap_TUI;

	public void Start()
	{
		net_com = GameApp.GetInstance().GetGameState().net_com;
		if (net_com != null)
		{
			net_com.connect_delegate = OnConnected;
			net_com.packet_delegate = OnPacket;
			net_com.close_delegate = OnClosed;
		}
		uiPos = new EndlessUIPos();
		GameApp.GetInstance().InitForMenu();
		Material material = UIResourceMgr.GetInstance().GetMaterial("ArenaMenu");
		Material material2 = UIResourceMgr.GetInstance().GetMaterial("ShopUI");
		back_img = new UIImage();
		back_img.SetTexture(material, ArenaMenuTexturePosition.Background, AutoRect.AutoSize(ArenaMenuTexturePosition.Background));
		back_img.Rect = AutoRect.AutoPos(uiPos.Background);
		EndlessPanel = new UITextImage();
		EndlessPanel.SetTexture(material2, ShopTexturePosition.DayLargePanel, AutoRect.AutoSize(ShopTexturePosition.DayLargePanel));
		EndlessPanel.Rect = AutoRect.AutoPos(uiPos.EndlessPanel);
		EndlessPanel.SetText("font2", "SELECT MAP", ColorName.fontColor_darkred);
		return_Button = new UIClickButton();
		return_Button.SetTexture(UIButtonBase.State.Normal, material, ArenaMenuTexturePosition.ReturnButtonNormal, AutoRect.AutoSize(ArenaMenuTexturePosition.ReturnButtonNormal));
		return_Button.SetTexture(UIButtonBase.State.Pressed, material, ArenaMenuTexturePosition.ReturnButtonPressed, AutoRect.AutoSize(ArenaMenuTexturePosition.ReturnButtonPressed));
		return_Button.Rect = AutoRect.AutoPos(uiPos.ReturnButton);
		Material material3 = UIResourceMgr.GetInstance().GetMaterial("Buttons");
		go_Button = new UITextButton();
		go_Button.SetTexture(UIButtonBase.State.Normal, material3, ButtonsTexturePosition.ButtonNormal, AutoRect.AutoSize(ButtonsTexturePosition.ButtonNormal));
		go_Button.SetTexture(UIButtonBase.State.Pressed, material3, ButtonsTexturePosition.ButtonPressed, AutoRect.AutoSize(ButtonsTexturePosition.ButtonPressed));
		go_Button.SetTexture(UIButtonBase.State.Disabled, material3, ButtonsTexturePosition.ButtonNormal, AutoRect.AutoSize(ButtonsTexturePosition.ButtonNormal));
		go_Button.Rect = AutoRect.AutoPos(uiPos.GoButton);
		go_Button.SetText("font1", "SELECT MAP", ColorName.fontColor_orange);
		Add(back_img);
		Add(EndlessPanel);
		Add(return_Button);
		Add(go_Button);
		SetUIHandler(this);
		Material material4 = UIResourceMgr.GetInstance().GetMaterial("EndlessUI");
		Material material5 = UIResourceMgr.GetInstance().GetMaterial("EndlessUI1");
		Material material6 = UIResourceMgr.GetInstance().GetMaterial("EndlessUI2");
		map_Scroller = new UIImageScroller(AutoRect.AutoPos(new Rect(0f, 100f, 960f, 540f)), AutoRect.AutoPos(new Rect(151f, 95f, 700f, 500f)), 1, AutoRect.AutoSize(EndlessTexturePosition.GetEndMissionRect(1)), ScrollerDir.Vertical, true);
		map_Scroller.SetImageSpacing(AutoRect.AutoSize(WeaponsLogoTexturePosition.WeaponLogoSpacing));
		map_Scroller.SetCenterFrameTexture(material5, EndlessTexturePosition.EndMissionBacK);
		for (int i = 0; i < 7; i++)
		{
			UIImage uIImage = new UIImage();
			if (i == 6)
			{
				uIImage.SetTexture(material6, EndlessTexturePosition.GetEndMissionRect(1), AutoRect.AutoSize(EndlessTexturePosition.GetEndMissionRect(1)));
			}
			else if (i + 1 > 4)
			{
				uIImage.SetTexture(material5, EndlessTexturePosition.GetEndMissionRect(i + 1), AutoRect.AutoSize(EndlessTexturePosition.GetEndMissionRect(i + 1)));
			}
			else
			{
				uIImage.SetTexture(material4, EndlessTexturePosition.GetEndMissionRect(i + 1), AutoRect.AutoSize(EndlessTexturePosition.GetEndMissionRect(i + 1)));
			}
			uIImage.Rect = EndlessTexturePosition.GetEndMissionRect(i + 1);
			map_Scroller.Add(uIImage);
		}
		Add(map_Scroller);
		map_Scroller.EnableScroll();
		map_Scroller.SetMaskImage(material5, EndlessTexturePosition.EndMissionMask);
		map_Scroller.Show();
		Show();
		if (CreateMap_TUI != null)
		{
			CreateMap_TUI.CreateMapUIPanel = this;
			if (CreateMap_TUI.CheckToturialInit())
			{
				Enable = false;
				go_Button.Enable = false;
				return_Button.Enable = false;
			}
		}
	}

	public override void Update()
	{
		if (fadeTimer.Ready())
		{
			if (fadeTimer.Name == "Back")
			{
				UIResourceMgr.GetInstance().UnloadAllUIMaterials();
				GameApp.GetInstance().GetGameState().FromShopMenu = true;
				SceneName.LoadLevel("MultiplayRoomTUI");
			}
			else if (fadeTimer.Name == "Create")
			{
				SceneName.LoadLevel("RoomOwnerTUI");
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
		else if (control == go_Button)
		{
			if (GameApp.GetInstance().GetGameState().multi_toturial_triger == 1)
			{
				float num = 0f;
				net_com.m_netUserInfo.SetNetUserInfo(true, GameApp.GetInstance().GetGameState().nick_name, 0, 0, num, cur_stage_index, 0, (int)GameApp.GetInstance().GetGameState().Avatar, GameApp.GetInstance().GetGameState().LevelNum);
				FadeAnimationScript.GetInstance().FadeInBlack(0.3f);
				fadeTimer.Name = "Create";
				fadeTimer.SetTimer(0.3f, false);
				Debug.Log("Create map now! ping : " + num);
			}
			else
			{
				Packet packet = CGCreateRoomPacket.MakePacket((uint)cur_stage_index, (long)(Time.time * 1000f), GameApp.GetInstance().GetGameState().nick_name, (uint)GameApp.GetInstance().GetGameState().Avatar, (uint)GameApp.GetInstance().GetGameState().LevelNum);
				net_com.Send(packet);
				Debug.Log("Ready Create map");
			}
		}
		else
		{
			if (control != map_Scroller || command != 0)
			{
				return;
			}
			cur_stage_index = (int)wparam;
			if (GameApp.GetInstance().GetGameState().multi_toturial_triger == 1)
			{
				if (cur_stage_index == 1)
				{
					CreateMap_TUI.OnToturialMapRight();
					go_Button.Enable = true;
				}
				else
				{
					CreateMap_TUI.OnToturialMapError();
					go_Button.Enable = false;
				}
			}
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

	public void CreateRoom(Packet packet)
	{
		GCCreateRoomPacket gCCreateRoomPacket = new GCCreateRoomPacket();
		if (gCCreateRoomPacket.ParserPacket(packet))
		{
			if (gCCreateRoomPacket.m_iResult == 0)
			{
				float num = (Time.time - (float)gCCreateRoomPacket.m_lLocalTime / 1000f) / 2f;
				net_com.m_netUserInfo.SetNetUserInfo(true, GameApp.GetInstance().GetGameState().nick_name, (int)gCCreateRoomPacket.m_iRoomId, (int)gCCreateRoomPacket.m_iUserId, num, cur_stage_index, 0, (int)GameApp.GetInstance().GetGameState().Avatar, GameApp.GetInstance().GetGameState().LevelNum);
				FadeAnimationScript.GetInstance().FadeInBlack(0.3f);
				fadeTimer.Name = "Create";
				fadeTimer.SetTimer(0.3f, false);
				Debug.Log("Create map now! ping : " + num);
			}
			else
			{
				Debug.Log("result error : " + gCCreateRoomPacket.m_iResult);
				CreateMap_TUI.OnCreateRoomError((int)gCCreateRoomPacket.m_iResult);
				go_Button.Enable = false;
				return_Button.Enable = false;
			}
		}
	}

	public void OnConnected()
	{
		Debug.Log("CreateMapUI OnConnected");
	}

	public void OnPacket(Packet packet)
	{
		uint val = 0u;
		if (packet.WatchUInt32(ref val, 4) && val == 4099)
		{
			CreateRoom(packet);
		}
	}

	public void OnClosed()
	{
		Debug.Log("CreateMapUI OnClosed");
	}
}
