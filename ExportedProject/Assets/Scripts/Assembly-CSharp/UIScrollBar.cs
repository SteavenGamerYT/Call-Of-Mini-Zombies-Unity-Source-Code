using UnityEngine;

public class UIScrollBar : UIControlVisible, IScrollBar
{
	public enum ScrollOrientation
	{
		Horizontal = 1,
		Vertical = 2
	}

	private float m_ScrollPercent;

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

	public UIScrollBar()
	{
		CreateSprite(2);
	}

	~UIScrollBar()
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

	public void SetScrollBarTexture(Material matScrollBar, Rect rcScrollBarTexture, Material matSlider, Rect rcSliderTexture)
	{
		m_MatScrollBar = matScrollBar;
		m_ScrollBarTextureRect = rcScrollBarTexture;
		m_MatSlider = matSlider;
		m_SliderTextureRect = rcSliderTexture;
	}

	public void SetSliderSize(Vector2 slider_size)
	{
		m_SliderSize = slider_size;
	}

	public void SetScrollPercent(float scroll_percent)
	{
		m_ScrollPercent = scroll_percent;
		if (ScrollOri == ScrollOrientation.Horizontal)
		{
			m_Sprite[0] = new UISprite();
			m_Sprite[0].Position = new Vector2(m_Rect.x + m_Rect.width / 2f, m_Rect.y + m_Rect.height / 2f);
			m_Sprite[0].Size = new Vector2(m_Rect.width, m_Rect.height);
			m_Sprite[0].Material = m_MatScrollBar;
			m_Sprite[0].TextureRect = m_ScrollBarTextureRect;
			m_Sprite[1] = new UISprite();
			m_Sprite[1].Position = new Vector2(m_Rect.x + m_SliderSize.x / 2f + (m_Rect.width - m_SliderSize.x) * m_ScrollPercent, m_Rect.y + m_SliderSize.y / 2f);
			m_Sprite[1].Size = m_SliderSize;
			m_Sprite[1].Material = m_MatSlider;
			m_Sprite[1].TextureRect = m_SliderTextureRect;
		}
		else if (ScrollOri == ScrollOrientation.Vertical)
		{
			m_Sprite[0] = new UISprite();
			m_Sprite[0].Position = new Vector2(m_Rect.x + m_Rect.width / 2f, m_Rect.y + m_Rect.height / 2f);
			m_Sprite[0].Size = new Vector2(m_Rect.width, m_Rect.height);
			m_Sprite[0].Material = m_MatScrollBar;
			m_Sprite[0].TextureRect = m_ScrollBarTextureRect;
			m_Sprite[1] = new UISprite();
			m_Sprite[1].Position = new Vector2(m_Rect.x + m_SliderSize.x / 2f, m_Rect.yMax - m_SliderSize.y / 2f - (m_Rect.height - m_SliderSize.y) * m_ScrollPercent);
			m_Sprite[1].Size = m_SliderSize;
			m_Sprite[1].Material = m_MatSlider;
			m_Sprite[1].TextureRect = m_SliderTextureRect;
		}
	}
}
