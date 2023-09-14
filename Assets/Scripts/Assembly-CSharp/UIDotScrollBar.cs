using UnityEngine;

public class UIDotScrollBar : UIControlVisible, IScrollBar
{
	public enum ScrollOrientation
	{
		Horizontal = 1,
		Vertical = 2
	}

	private int m_PageCount;

	private float m_ScrollPercent = -1f;

	private Vector2 m_SliderSize;

	private ScrollOrientation m_ScrollOri = ScrollOrientation.Horizontal;

	private Material m_MatScrollBar;

	private Rect m_ScrollBarTextureRect;

	private Material m_MatSlider;

	private Rect m_SliderTextureRect;

	public ScrollOrientation ScrollOri
	{
		get
		{
			return m_ScrollOri;
		}
		set
		{
			m_ScrollOri = value;
		}
	}

	public float DotPageWidth { get; set; }

	public new Rect Rect
	{
		get
		{
			return base.Rect;
		}
		set
		{
			base.Rect = value;
		}
	}

	~UIDotScrollBar()
	{
	}

	public override void Draw()
	{
		if (m_Sprite != null)
		{
			for (int i = 0; i < m_Sprite.Length; i++)
			{
				m_Parent.DrawSprite(m_Sprite[i]);
			}
		}
	}

	public void SetPageCount(int page_count)
	{
		m_PageCount = page_count;
		CreateSprite(m_PageCount + 1);
	}

	public void SetScrollBarTexture(Material matDotScrollBarBg, Rect rcDotScrollBarBgTexture, Material matSlider, Rect rcSliderTexture)
	{
		m_MatScrollBar = matDotScrollBarBg;
		m_ScrollBarTextureRect = rcDotScrollBarBgTexture;
		m_MatSlider = matSlider;
		m_SliderTextureRect = rcSliderTexture;
	}

	public void SetScrollPercent(float scroll_percent)
	{
		if (m_PageCount < 1)
		{
			return;
		}
		if (m_Sprite[m_PageCount].Material != null)
		{
			int num = Mathf.RoundToInt(scroll_percent * (float)(m_PageCount - 1));
			int num2 = Mathf.RoundToInt(m_ScrollPercent * (float)(m_PageCount - 1));
			if (num == num2)
			{
				return;
			}
		}
		m_ScrollPercent = Mathf.Clamp01(scroll_percent);
		if (ScrollOri == ScrollOrientation.Horizontal)
		{
			for (int i = 0; i < m_PageCount; i++)
			{
				Vector2 position = new Vector2(Rect.x + (float)i * DotPageWidth + DotPageWidth / 2f, Rect.y + Rect.height / 2f);
				m_Sprite[i] = new UISprite();
				m_Sprite[i].Position = position;
				m_Sprite[i].Size = new Vector2(m_ScrollBarTextureRect.width, m_ScrollBarTextureRect.height);
				m_Sprite[i].Material = m_MatScrollBar;
				m_Sprite[i].TextureRect = m_ScrollBarTextureRect;
			}
			int num3 = Mathf.RoundToInt(m_ScrollPercent * (float)(m_PageCount - 1));
			Vector2 position2 = new Vector2(Rect.x + (float)num3 * DotPageWidth + DotPageWidth / 2f, Rect.y + Rect.height / 2f);
			m_Sprite[m_PageCount] = new UISprite();
			m_Sprite[m_PageCount].Position = position2;
			m_Sprite[m_PageCount].Size = new Vector2(m_SliderTextureRect.width, m_SliderTextureRect.height);
			m_Sprite[m_PageCount].Material = m_MatSlider;
			m_Sprite[m_PageCount].TextureRect = m_SliderTextureRect;
		}
		else if (ScrollOri == ScrollOrientation.Vertical)
		{
		}
	}
}
