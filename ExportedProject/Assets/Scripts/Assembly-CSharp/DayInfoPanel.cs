using UnityEngine;

public class DayInfoPanel : UIPanel
{
	public Rect Day = new Rect(300f, 286f, 210f, 108f);

	protected UIImage dayImg;

	protected UIImage[] numberImg = new UIImage[3];

	protected Material material;

	protected float aniStartTime;

	public DayInfoPanel()
	{
		dayImg = new UIImage();
		this.material = UIResourceMgr.GetInstance().GetMaterial("GameUI");
		Material material = UIResourceMgr.GetInstance().GetMaterial("Buttons");
		dayImg.SetTexture(material, ButtonsTexturePosition.Day, AutoRect.AutoSize(ButtonsTexturePosition.Day));
		dayImg.Rect = AutoRect.AutoPos(Day);
		dayImg.Enable = false;
		Day = AutoRect.AutoPos(Day);
		Add(dayImg);
		for (int i = 0; i < 3; i++)
		{
			numberImg[i] = new UIImage();
			numberImg[i].Rect = GetNumberPos(i);
			numberImg[i].Enable = false;
			Add(numberImg[i]);
		}
	}

	protected Rect GetNumberPos(int index)
	{
		return AutoRect.AutoPos(new Rect(480 + index * 59, 300f, 182f, 76f));
	}

	public void SetDay(int day)
	{
		if (day < 10)
		{
			numberImg[0].SetTexture(material, GameUITexturePosition.GetNumberRect(day));
			numberImg[1].SetTexture(material, GameUITexturePosition.GetNumberRect(-1));
			numberImg[2].SetTexture(material, GameUITexturePosition.GetNumberRect(-1));
		}
		else if (day < 100)
		{
			int n = day / 10;
			int n2 = day % 10;
			numberImg[0].SetTexture(material, GameUITexturePosition.GetNumberRect(n));
			numberImg[1].SetTexture(material, GameUITexturePosition.GetNumberRect(n2));
			numberImg[2].SetTexture(material, GameUITexturePosition.GetNumberRect(-1));
		}
		else
		{
			int num = day / 100;
			day -= num * 100;
			int n3 = day / 10;
			int n4 = day % 10;
			numberImg[0].SetTexture(material, GameUITexturePosition.GetNumberRect(num));
			numberImg[1].SetTexture(material, GameUITexturePosition.GetNumberRect(n3));
			numberImg[2].SetTexture(material, GameUITexturePosition.GetNumberRect(n4));
		}
	}

	public override void Show()
	{
		if (Application.loadedLevelName != "Zombie3D_Tutorial")
		{
			base.Show();
		}
		aniStartTime = Time.time;
	}

	public void UpdateAnimation()
	{
		float num = Time.time - aniStartTime;
		if (!(num < 15f))
		{
			return;
		}
		float num2 = ((0.5f - num > 0f) ? (0.5f - num) : 0f);
		dayImg.Rect = new Rect(Day.x + AutoRect.AutoX(2000f * num2), Day.y, Day.width, Day.height);
		if (num2 == 0f)
		{
			float num3 = num - 0.5f;
			if (num3 >= 0f && num3 <= 0.1f)
			{
				dayImg.SetTextureSize(new Vector2(ButtonsTexturePosition.Day.width * (1f + num3 * 5f), ButtonsTexturePosition.Day.height * (1f + num3 * 5f)));
			}
			else if (num3 > 0.1f && num3 <= 0.2f)
			{
				dayImg.SetTextureSize(new Vector2(ButtonsTexturePosition.Day.width * (2f - num3 * 5f), ButtonsTexturePosition.Day.height * (2f - num3 * 5f)));
			}
		}
		for (int i = 0; i < 3; i++)
		{
			float num4 = ((0.5f + (float)(i + 1) * 0.5f - num > 0f) ? (0.5f + (float)(i + 1) * 0.5f - num) : 0f);
			Rect numberPos = GetNumberPos(i);
			numberImg[i].Rect = new Rect(numberPos.x + AutoRect.AutoX(2000f * num4), numberPos.y, numberPos.width, numberPos.height);
			if (num4 == 0f)
			{
				float num5 = num - (0.5f + (float)(i + 1) * 0.5f);
				Rect numberRect = GameUITexturePosition.GetNumberRect(0);
				if (num5 >= 0f && num5 <= 0.1f)
				{
					numberImg[i].SetTextureSize(new Vector2(numberRect.width * (1f + num5 * 5f), numberRect.height * (1f + num5 * 5f)));
				}
				else if (num5 > 0.1f && num5 <= 0.2f)
				{
					numberImg[i].SetTextureSize(new Vector2(numberRect.width * (2f - num5 * 5f), numberRect.height * (2f - num5 * 5f)));
				}
			}
		}
		if (num > 4f)
		{
			dayImg.Rect = new Rect(Day.x - AutoRect.AutoX(2000f * (num - 4f)), Day.y, Day.width, Day.height);
			for (int j = 0; j < 3; j++)
			{
				Rect numberPos2 = GetNumberPos(j);
				numberImg[j].Rect = new Rect(numberPos2.x - AutoRect.AutoX(2000f * (num - 4f)), numberPos2.y, numberPos2.width, numberPos2.height);
			}
		}
	}
}
