using System.Collections.Generic;
using UnityEngine;

internal class UIScrollView : UIControl, UIContainer
{
	public enum ScrollOrientation
	{
		Horizontal = 1,
		Vertical = 2,
		Horizontal_Vertical = 3
	}

	public enum ListOrientation
	{
		Horizontal = 0,
		Vertical = 1
	}

	protected const float m_reboundSpeed = 1f;

	protected const float m_overscrollAllowance = 0.5f;

	protected const float m_scrollDecelCoef = 0.4f;

	protected const float m_lowPassKernelWidthInSeconds = 0.03f;

	protected const float m_scrollDeltaUpdateInterval = 0.0166f;

	protected const float m_lowPassFilterFactor = 83f / 150f;

	private UIControl m_thumbH;

	private UIControl m_thumbV;

	private UIMoveOuter m_Move;

	private List<UIControl> m_Controls = new List<UIControl>();

	private IScrollBar m_ScrollBar;

	private Rect m_bounds;

	private ScrollOrientation m_scrollOri = ScrollOrientation.Horizontal;

	private ListOrientation m_listOri;

	private Vector2 m_contentExtent;

	private Vector2 m_posOrigin = default(Vector2);

	private float m_itemSpacingH;

	private float m_itemSpacingV;

	private float m_scrollPosH;

	private float m_scrollPosV;

	private float m_scrollDeltaH;

	private float m_scrollDeltaV;

	private bool m_isScrolling;

	private bool m_noTouch = true;

	private float m_scrollInertiaH;

	private float m_scrollInertiaV;

	protected float m_scrollMaxH;

	protected float m_scrollMaxV;

	private float m_lastTime;

	private float m_timeDelta;

	protected bool Scroll_Enable = true;

	public UIControl ThumbH
	{
		set
		{
			m_thumbH = value;
			m_thumbH.SetParent(this);
		}
	}

	public UIControl ThumbV
	{
		set
		{
			m_thumbV = value;
			m_thumbV.SetParent(this);
		}
	}

	public IScrollBar ScrollBar
	{
		get
		{
			return m_ScrollBar;
		}
		set
		{
			m_ScrollBar = value;
		}
	}

	public Rect Bounds
	{
		get
		{
			return m_bounds;
		}
		set
		{
			m_bounds = value;
		}
	}

	public ScrollOrientation ScrollOri
	{
		get
		{
			return m_scrollOri;
		}
		set
		{
			m_scrollOri = value;
		}
	}

	public ListOrientation ListOri
	{
		get
		{
			return m_listOri;
		}
		set
		{
			m_listOri = value;
		}
	}

	public float ItemSpacingH
	{
		get
		{
			return m_itemSpacingH;
		}
		set
		{
			m_itemSpacingH = value;
		}
	}

	public float ItemSpacingV
	{
		get
		{
			return m_itemSpacingV;
		}
		set
		{
			m_itemSpacingV = value;
		}
	}

	public float ScrollPosH
	{
		get
		{
			return m_scrollPosH;
		}
		set
		{
			m_scrollPosH = value;
			ScrollListTo_InternalH(m_scrollPosH);
			UpdateControlPos();
		}
	}

	public float ScrollPosV
	{
		get
		{
			return m_scrollPosV;
		}
		set
		{
			m_scrollPosV = value;
			ScrollListTo_InternalV(m_scrollPosV);
			UpdateControlPos();
		}
	}

	public override bool Visible
	{
		get
		{
			return base.Visible;
		}
		set
		{
			base.Visible = value;
			for (int num = m_Controls.Count; num > 0; num--)
			{
				m_Controls[num - 1].Visible = value;
			}
		}
	}

	public override bool Enable
	{
		get
		{
			return base.Enable;
		}
		set
		{
			base.Enable = value;
			for (int num = m_Controls.Count; num > 0; num--)
			{
				m_Controls[num - 1].Enable = value;
			}
		}
	}

	public override Rect Rect
	{
		get
		{
			return base.Rect;
		}
		set
		{
			Rect rect = Rect;
			int count = m_Controls.Count;
			for (int i = 0; i < count; i++)
			{
				UIControl uIControl = m_Controls[i];
				Rect rect2 = uIControl.Rect;
				float num = (rect2.x - rect.x) / rect.width;
				float num2 = (rect2.y - rect.y) / rect.height;
				rect2.x = value.x + m_Rect.width * num;
				rect2.y = value.y + m_Rect.height * num2;
				float num3 = value.width / rect.width;
				float num4 = value.height / rect.height;
				rect2.width *= num3;
				rect2.height *= num4;
				uIControl.Rect = rect2;
			}
			base.Rect = value;
		}
	}

	public void SetMoveParam(Rect rcMove, float moveMinX, float moveMinY)
	{
		m_Move = new UIMoveOuter();
		m_Move.Rect = rcMove;
		m_Move.MinX = moveMinX;
		m_Move.MinY = moveMinY;
		m_Move.SetParent(this);
	}

	public void Add(UIControl control)
	{
		control.SetParent(this);
		if (m_Controls == null)
		{
			m_Controls = new List<UIControl>();
		}
		m_Controls.Add(control);
		if (m_listOri == ListOrientation.Horizontal)
		{
			m_contentExtent.x += control.Rect.width + m_itemSpacingH;
			float b = control.Rect.height + m_itemSpacingV;
			m_contentExtent.y = Mathf.Max(m_contentExtent.y, b);
		}
		else if (m_listOri == ListOrientation.Vertical)
		{
			m_contentExtent.y += control.Rect.height + m_itemSpacingV;
			float b2 = control.Rect.width + m_itemSpacingH;
			m_contentExtent.x = Mathf.Max(m_contentExtent.x, b2);
		}
		if ((m_scrollOri & ScrollOrientation.Horizontal) > (ScrollOrientation)0)
		{
			m_scrollMaxH = m_bounds.width / (m_contentExtent.x - m_bounds.width) * 0.5f;
		}
		if ((m_scrollOri & ScrollOrientation.Vertical) > (ScrollOrientation)0)
		{
			m_scrollMaxV = m_bounds.height / (m_contentExtent.y - m_bounds.height) * 0.5f;
		}
		UpdateControlPos();
		if (m_Clip)
		{
			control.SetClip(m_ClipRect);
		}
	}

	public void Remove(UIControl control)
	{
		m_Controls.Remove(control);
	}

	public void ScrollEnable(bool status)
	{
		Scroll_Enable = status;
	}

	public override void SetClip(Rect clip_rect)
	{
		base.SetClip(clip_rect);
		for (int i = 0; i < m_Controls.Count; i++)
		{
			UIControl uIControl = m_Controls[i];
			uIControl.SetClip(clip_rect);
		}
	}

	public override void Draw()
	{
		for (int i = 0; i < m_Controls.Count; i++)
		{
			UIControl uIControl = m_Controls[i];
			uIControl.Update();
			if (uIControl.Visible && Visible)
			{
				uIControl.Draw();
			}
		}
		if (m_thumbH != null)
		{
			m_thumbH.Update();
			if (m_thumbH.Visible)
			{
				m_thumbH.Draw();
			}
		}
		if (m_thumbV != null)
		{
			m_thumbV.Update();
			if (m_thumbV.Visible)
			{
				m_thumbV.Draw();
			}
		}
	}

	public override bool HandleInput(UITouchInner touch)
	{
		if (m_Move.HandleInput(touch))
		{
			for (int num = m_Controls.Count - 1; num >= 0; num--)
			{
				UIControl uIControl = m_Controls[num];
				if (uIControl.GetType().Equals(typeof(UIClickButton)))
				{
					if (uIControl.Enable && Enable)
					{
						((UIClickButton)uIControl).Reset();
					}
				}
				else if (uIControl.GetType().Equals(typeof(UISelectButton)))
				{
					if (uIControl.Enable && Enable)
					{
						((UISelectButton)uIControl).Reset();
					}
				}
				else if (uIControl.GetType().Equals(typeof(UIGroupControl)))
				{
					UIGroupControl uIGroupControl = (UIGroupControl)uIControl;
					for (int num2 = uIGroupControl.Controls.Count - 1; num2 >= 0; num2--)
					{
						UIControl uIControl2 = (UIControl)uIGroupControl.Controls[num2];
						if (uIControl2.GetType().Equals(typeof(UIClickButton)))
						{
							if (uIControl2.Enable && Enable)
							{
								((UIClickButton)uIControl2).Reset();
							}
						}
						else if (uIControl2.GetType().Equals(typeof(UISelectButton)) && uIControl2.Enable && Enable)
						{
							((UISelectButton)uIControl2).Reset();
						}
					}
				}
			}
			return true;
		}
		for (int num3 = m_Controls.Count - 1; num3 >= 0; num3--)
		{
			UIControl uIControl3 = m_Controls[num3];
			if (uIControl3.Enable && Enable && uIControl3.HandleInput(touch))
			{
				return true;
			}
		}
		return false;
	}

	public void DrawSprite(UISprite sprite)
	{
		m_Parent.DrawSprite(sprite);
	}

	public void SendEvent(UIControl control, int command, float wparam, float lparam)
	{
		if (control == m_Move && Scroll_Enable)
		{
			switch (command)
			{
			case 1:
				ScrollDragged(new Vector2(wparam, lparam));
				break;
			case 2:
				PointerReleased();
				break;
			}
		}
		else
		{
			m_Parent.SendEvent(control, command, wparam, lparam);
		}
	}

	public override void Update()
	{
		base.Update();
		m_timeDelta = Time.realtimeSinceStartup - m_lastTime;
		m_lastTime = Time.realtimeSinceStartup;
		if (m_isScrolling && m_noTouch)
		{
			if ((m_scrollOri & ScrollOrientation.Horizontal) > (ScrollOrientation)0)
			{
				m_scrollDeltaH -= m_scrollDeltaH * 0.4f * (m_timeDelta / 0.166f);
				if (m_scrollPosH < 0f)
				{
					m_scrollPosH -= m_scrollPosH * 1f * (m_timeDelta / 0.166f);
					m_scrollDeltaH *= Mathf.Clamp01(1f + m_scrollPosH / m_scrollMaxH);
				}
				else if (m_scrollPosH > 1f)
				{
					m_scrollPosH -= (m_scrollPosH - 1f) * 1f * (m_timeDelta / 0.166f);
					m_scrollDeltaH *= Mathf.Clamp01(1f - (m_scrollPosH - 1f) / m_scrollMaxH);
				}
				if (Mathf.Abs(m_scrollDeltaH) < 0.0001f)
				{
					m_scrollDeltaH = 0f;
					if (m_scrollPosH > -0.0001f && m_scrollPosH < 0.0001f)
					{
						m_scrollPosH = Mathf.Clamp01(m_scrollPosH);
					}
				}
				ScrollListTo_InternalH(m_scrollPosH + m_scrollDeltaH);
			}
			if ((m_scrollOri & ScrollOrientation.Vertical) > (ScrollOrientation)0)
			{
				m_scrollDeltaV -= m_scrollDeltaV * 0.4f * (m_timeDelta / 0.166f);
				if (m_scrollPosV < 0f)
				{
					m_scrollPosV -= m_scrollPosV * 1f * (m_timeDelta / 0.166f);
					m_scrollDeltaV *= Mathf.Clamp01(1f + m_scrollPosV / m_scrollMaxV);
				}
				else if (m_scrollPosV > 1f)
				{
					m_scrollPosV -= (m_scrollPosV - 1f) * 1f * (m_timeDelta / 0.166f);
					m_scrollDeltaV *= Mathf.Clamp01(1f - (m_scrollPosV - 1f) / m_scrollMaxV);
				}
				if (Mathf.Abs(m_scrollDeltaV) < 0.0001f)
				{
					m_scrollDeltaV = 0f;
					if (m_scrollPosV > -0.0001f && m_scrollPosV < 0.0001f)
					{
						m_scrollPosV = Mathf.Clamp01(m_scrollPosV);
					}
				}
				ScrollListTo_InternalV(m_scrollPosV + m_scrollDeltaV);
			}
			UpdateControlPos();
			if (m_scrollPosH >= 0f && m_scrollPosH <= 1.001f && m_scrollDeltaH == 0f && m_scrollPosV >= 0f && m_scrollPosV <= 1.001f && m_scrollDeltaV == 0f)
			{
				m_isScrolling = false;
			}
		}
		else
		{
			if ((m_scrollOri & ScrollOrientation.Horizontal) > (ScrollOrientation)0)
			{
				m_scrollInertiaH = Mathf.Lerp(m_scrollInertiaH, m_scrollDeltaH, 83f / 150f);
			}
			if ((m_scrollOri & ScrollOrientation.Vertical) > (ScrollOrientation)0)
			{
				m_scrollInertiaV = Mathf.Lerp(m_scrollInertiaV, m_scrollDeltaV, 83f / 150f);
			}
		}
		if (ScrollBar != null)
		{
			if ((m_scrollOri & ScrollOrientation.Horizontal) > (ScrollOrientation)0)
			{
				ScrollBar.SetScrollPercent(Mathf.Clamp01(m_scrollPosH));
			}
			if ((m_scrollOri & ScrollOrientation.Vertical) > (ScrollOrientation)0)
			{
				ScrollBar.SetScrollPercent(Mathf.Clamp01(m_scrollPosV));
			}
		}
	}

	private void PointerReleased()
	{
		m_noTouch = true;
		if (m_scrollInertiaH != 0f)
		{
			m_scrollDeltaH = m_scrollInertiaH;
		}
		m_scrollInertiaH = 0f;
		if (m_scrollInertiaV != 0f)
		{
			m_scrollDeltaV = m_scrollInertiaV;
		}
		m_scrollInertiaV = 0f;
	}

	private void ScrollDragged(Vector2 deltaPos)
	{
		if ((m_scrollOri & ScrollOrientation.Horizontal) > (ScrollOrientation)0)
		{
			m_scrollDeltaH = (0f - deltaPos.x) / (m_contentExtent.x - m_bounds.width);
			float num = m_scrollPosH + m_scrollDeltaH;
			if (num > 1f)
			{
				m_scrollDeltaH *= Mathf.Clamp01(1f - (num - 1f) / m_scrollMaxH);
			}
			else if (num < 0f)
			{
				m_scrollDeltaH *= Mathf.Clamp01(1f + num / m_scrollMaxH);
			}
			ScrollListTo_InternalH(m_scrollPosH + m_scrollDeltaH);
		}
		if ((m_scrollOri & ScrollOrientation.Vertical) > (ScrollOrientation)0)
		{
			m_scrollDeltaV = deltaPos.y / (m_contentExtent.y - m_bounds.height);
			float num2 = m_scrollPosV + m_scrollDeltaV;
			if (num2 > 1f)
			{
				m_scrollDeltaV *= Mathf.Clamp01(1f - (num2 - 1f) / m_scrollMaxV);
			}
			else if (num2 < 0f)
			{
				m_scrollDeltaV *= Mathf.Clamp01(1f + num2 / m_scrollMaxV);
			}
			ScrollListTo_InternalV(m_scrollPosV + m_scrollDeltaV);
		}
		UpdateControlPos();
		m_noTouch = false;
		m_isScrolling = true;
	}

	private void ScrollListTo_InternalH(float pos)
	{
		if (!float.IsNaN(pos))
		{
			float num = m_contentExtent.x - m_bounds.width;
			m_posOrigin.x = Mathf.Clamp(num, 0f, num) * (0f - pos);
			m_scrollPosH = pos;
		}
	}

	private void ScrollListTo_InternalV(float pos)
	{
		if (!float.IsNaN(pos))
		{
			float num = m_contentExtent.y - m_bounds.height;
			m_posOrigin.y = Mathf.Clamp(num, 0f, num) * (0f - pos);
			m_scrollPosV = pos;
		}
	}

	public void ScrollListToH(float pos)
	{
		m_scrollInertiaH = 0f;
		m_scrollDeltaH = 0f;
		ScrollListTo_InternalH(pos);
	}

	public void ScrollListToV(float pos)
	{
		m_scrollInertiaV = 0f;
		m_scrollDeltaV = 0f;
		ScrollListTo_InternalV(pos);
	}

	private void UpdateControlPos()
	{
		int count = m_Controls.Count;
		for (int i = 0; i < count; i++)
		{
			UIControl uIControl = m_Controls[i];
			Rect rect = uIControl.Rect;
			if (m_listOri == ListOrientation.Horizontal)
			{
				rect.x = m_bounds.x + m_posOrigin.x + (float)i * (rect.width + m_itemSpacingH);
				rect.y = m_bounds.yMax - rect.height - m_posOrigin.y;
			}
			else if (m_listOri == ListOrientation.Vertical)
			{
				rect.x = m_bounds.x + m_posOrigin.x;
				rect.y = m_bounds.yMax - m_posOrigin.y - (float)i * (rect.height + m_itemSpacingV) - rect.height;
			}
			uIControl.Rect = rect;
		}
		if (m_thumbH != null && (m_scrollOri & ScrollOrientation.Horizontal) > (ScrollOrientation)0)
		{
			Rect rect2 = m_thumbH.Rect;
			float num = (m_bounds.width - rect2.width - m_itemSpacingH) * Mathf.Clamp01(m_scrollPosH);
			rect2.x = m_bounds.x + num;
			m_thumbH.Rect = rect2;
		}
		if (m_thumbV != null && (m_scrollOri & ScrollOrientation.Vertical) > (ScrollOrientation)0)
		{
			Rect rect3 = m_thumbV.Rect;
			float num2 = (m_bounds.height - rect3.height - m_itemSpacingV) * Mathf.Clamp01(m_scrollPosV);
			rect3.y = m_bounds.yMax - num2 - rect3.height;
			m_thumbV.Rect = rect3;
		}
	}
}
