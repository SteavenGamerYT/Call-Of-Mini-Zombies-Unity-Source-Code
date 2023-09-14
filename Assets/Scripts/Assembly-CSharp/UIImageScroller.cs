using System.Collections.Generic;
using UnityEngine;

public class UIImageScroller : UIPanel, UIHandler
{
	public enum Command
	{
		ScrollSelect = 0,
		PressSelect = 1,
		PressEnd = 2,
		DragMove = 3
	}

	protected UIMove m_UIMove = new UIMove();

	protected UIImage m_CenterFrameImage = new UIImage();

	protected List<UIImage> m_overlayImageList = new List<UIImage>();

	protected List<TexturePosInfo> m_overlayInfoList = new List<TexturePosInfo>();

	protected List<UIImage> m_MaskList = new List<UIImage>();

	protected int m_CountPerRow = 1;

	protected int m_IconWidth;

	protected int m_IconHeight;

	protected float m_ScrollerPos;

	protected bool m_Scalable;

	protected ScrollerDir m_Dir;

	protected Vector2 m_Velocity;

	protected Vector2 m_LastMove;

	protected int m_SelectionIndex;

	protected float m_CenterPos;

	protected bool m_fingerOn;

	protected new Rect m_ClipRect;

	protected Vector2 m_Spacing;

	protected int m_LongPressSelectionIndex = -1;

	protected float m_lastBeginPressTime = -1f;

	protected bool m_EnableLongPress;

	protected Vector2 m_BeginPos;

	protected bool m_bMoveNotDrag;

	public UIImageScroller(Rect eventRect, Rect clipRect, int countPerRow, Vector2 iconSize, ScrollerDir dir, bool scalable)
	{
		m_UIMove.Rect = eventRect;
		m_CountPerRow = countPerRow;
		m_IconWidth = (int)iconSize.x;
		m_IconHeight = (int)iconSize.y;
		m_Spacing = iconSize;
		m_Dir = dir;
		m_Scalable = scalable;
		SetScrollerClip(clipRect);
		if (m_Dir == ScrollerDir.Vertical)
		{
			m_CenterPos = m_ClipRect.y + 0.5f * m_ClipRect.height - 0.5f * (float)m_IconHeight;
		}
		else if (m_Dir == ScrollerDir.Horizontal)
		{
			m_CenterPos = m_ClipRect.x + 0.5f * m_ClipRect.width - 0.5f * (float)m_IconWidth;
		}
		m_CenterFrameImage = new UIImage();
		m_CenterFrameImage.SetParent(this);
		SetUIHandler(this);
	}

	public void SetCenterFrameTexture(Material material, Rect rect)
	{
		m_CenterFrameImage.SetTexture(material, rect, AutoRect.AutoSize(rect));
		if (m_Dir == ScrollerDir.Vertical)
		{
			float x = m_ClipRect.x + 0.5f * m_ClipRect.width - 0.5f * rect.width;
			float y = m_ClipRect.y + 0.5f * m_ClipRect.height - 0.5f * rect.height;
			m_CenterFrameImage.Rect = new Rect(x, y, rect.width, rect.height);
		}
	}

	public void EnableLongPress()
	{
		m_EnableLongPress = true;
	}

	public void SetImageSpacing(Vector2 spacing)
	{
		m_Spacing = spacing;
	}

	protected void SetScrollerClip(Rect rect)
	{
		m_ClipRect = rect;
	}

	public void Reset()
	{
		SetSelection(0);
	}

	public void SetSelection(int index)
	{
		m_SelectionIndex = index;
		if (m_Dir == ScrollerDir.Horizontal)
		{
			m_ScrollerPos = m_CenterPos - (float)m_SelectionIndex * m_Spacing.x;
		}
		else if (m_Dir == ScrollerDir.Vertical)
		{
			m_ScrollerPos = m_CenterPos + (float)m_SelectionIndex * m_Spacing.y;
		}
	}

	public int GetSelection()
	{
		return m_SelectionIndex;
	}

	public void Clear()
	{
		m_Controls.Clear();
		m_overlayImageList.Clear();
		m_overlayInfoList.Clear();
	}

	public Rect GetCenterRect()
	{
		return new Rect((int)(m_ClipRect.x + 0.5f * (m_ClipRect.width - (float)m_IconWidth)), m_CenterPos, m_IconWidth, m_IconHeight);
	}

	public override void Add(UIControl control)
	{
		base.Add(control);
		UIImage uIImage = new UIImage();
		m_overlayImageList.Add(uIImage);
		uIImage.Visible = false;
		uIImage.SetParent(this);
		UIImage uIImage2 = new UIImage();
		m_MaskList.Add(uIImage2);
		uIImage2.SetParent(this);
	}

	public void EnableScroll()
	{
		Add(m_UIMove);
		Reset();
	}

	public void SetOverlay(int iconID, int overlayID)
	{
		if (overlayID != -1)
		{
			TexturePosInfo texturePosInfo = m_overlayInfoList[overlayID];
			m_overlayImageList[iconID].SetTexture(texturePosInfo.m_Material, texturePosInfo.m_TexRect);
			m_overlayImageList[iconID].Visible = true;
		}
		else
		{
			m_overlayImageList[iconID].Visible = false;
		}
	}

	public void AddOverlay(Material material, Rect texRect)
	{
		m_overlayInfoList.Add(new TexturePosInfo(material, texRect));
	}

	public void SetMaskImage(Material material, Rect texRect)
	{
		for (int i = 0; i < m_MaskList.Count; i++)
		{
			UIImage uIImage = m_MaskList[i];
			uIImage.SetTexture(material, texRect, AutoRect.AutoSize(texRect));
		}
	}

	public void HandleEvent(UIControl control, int command, float wparam, float lparam)
	{
		if (control != m_UIMove)
		{
			return;
		}
		switch (command)
		{
		case 1:
			if (m_LongPressSelectionIndex == -1)
			{
				m_lastBeginPressTime = -1f;
				m_LongPressSelectionIndex = -1;
				if (m_Dir == ScrollerDir.Horizontal)
				{
					m_ScrollerPos += wparam;
				}
				else if (m_Dir == ScrollerDir.Vertical)
				{
					m_ScrollerPos += lparam;
				}
				m_LastMove = new Vector2(wparam, lparam);
				m_fingerOn = true;
			}
			break;
		case 3:
			if (m_LongPressSelectionIndex != -1)
			{
				m_Parent.SendEvent(this, 3, wparam, lparam);
			}
			else
			{
				if (!m_EnableLongPress)
				{
					break;
				}
				Vector2 vector = m_BeginPos - new Vector2(wparam, lparam);
				if (vector.sqrMagnitude > AutoRect.AutoValue(10f) * AutoRect.AutoValue(10f))
				{
					if (Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
					{
						m_LongPressSelectionIndex = m_SelectionIndex;
						m_Parent.SendEvent(this, 1, m_SelectionIndex, 0f);
					}
					else
					{
						m_bMoveNotDrag = true;
					}
				}
			}
			break;
		case 2:
		{
			float x2 = Mathf.Clamp(m_LastMove.x, -30f, 30f);
			float y = Mathf.Clamp(m_LastMove.y, -30f, 30f);
			m_Velocity = new Vector2(x2, y);
			m_fingerOn = false;
			m_LastMove = Vector2.zero;
			if (m_EnableLongPress)
			{
				m_lastBeginPressTime = -1f;
				m_LongPressSelectionIndex = -1;
				m_Parent.SendEvent(this, 2, wparam, lparam);
			}
			break;
		}
		case 0:
			if (m_EnableLongPress)
			{
				m_BeginPos = new Vector2(wparam, lparam);
				float x = (int)(m_ClipRect.x + 0.5f * (m_ClipRect.width - (float)m_IconWidth));
				float centerPos = m_CenterPos;
				if (new Rect(x, centerPos, m_IconWidth, m_IconHeight).Contains(new Vector2(wparam, lparam)) && m_lastBeginPressTime == -1f)
				{
					m_lastBeginPressTime = Time.time;
				}
			}
			break;
		}
	}

	public override void Draw()
	{
		UpdateImage();
		if (m_fingerOn)
		{
			m_CenterFrameImage.Draw();
		}
		for (int i = 0; i < m_SelectionIndex; i++)
		{
			UIImage uIImage = m_Controls[i] as UIImage;
			uIImage.Draw();
			if (m_overlayImageList[i].Visible)
			{
				m_overlayImageList[i].Draw();
			}
			m_MaskList[i].Draw();
		}
		for (int num = m_Controls.Count - 2; num > m_SelectionIndex; num--)
		{
			UIImage uIImage2 = m_Controls[num] as UIImage;
			uIImage2.Draw();
			if (m_overlayImageList[num].Visible)
			{
				m_overlayImageList[num].Draw();
			}
			m_MaskList[num].Draw();
		}
		if (!m_fingerOn)
		{
			m_CenterFrameImage.Draw();
		}
		UIImage uIImage3 = m_Controls[m_SelectionIndex] as UIImage;
		uIImage3.Draw();
		if (m_overlayImageList[m_SelectionIndex].Visible)
		{
			m_overlayImageList[m_SelectionIndex].Draw();
		}
	}

	public void UpdateImage()
	{
		if (m_EnableLongPress && m_LongPressSelectionIndex == -1 && m_lastBeginPressTime != -1f && Time.time - m_lastBeginPressTime > 1f)
		{
			m_LongPressSelectionIndex = m_SelectionIndex;
			m_Parent.SendEvent(this, 1, m_SelectionIndex, 0f);
		}
		int num = 5;
		if (Mathf.Abs(m_Velocity.x) >= (float)num)
		{
			m_Velocity.x -= Time.deltaTime * 60f * Mathf.Sign(m_Velocity.x);
		}
		else if (!m_fingerOn)
		{
			m_Velocity.x = Mathf.Sign(m_Velocity.x) * (float)num;
		}
		if (Mathf.Abs(m_Velocity.y) >= (float)num)
		{
			m_Velocity.y -= Time.deltaTime * 60f * Mathf.Sign(m_Velocity.y);
		}
		else if (!m_fingerOn && m_Velocity.y != 0f)
		{
			m_Velocity.y = Mathf.Sign(m_Velocity.y) * (float)num;
		}
		if (m_Dir == ScrollerDir.Horizontal)
		{
			m_ScrollerPos += (int)(m_Velocity.x * Time.deltaTime * 100f);
			m_ScrollerPos = Mathf.Clamp(m_ScrollerPos, m_CenterPos - (float)(m_Controls.Count - 2) * m_Spacing.x, m_CenterPos);
			for (int i = 0; i < m_Controls.Count - 1; i++)
			{
				float num2 = m_ScrollerPos + (float)i * m_Spacing.x;
				float num3 = m_ClipRect.width * 0.5f + m_ClipRect.x;
				float num4 = num2 + 0.5f * (float)m_IconWidth;
				if (Mathf.Abs(num4 - num3) < m_Spacing.x / 2f)
				{
					int num5 = i;
					m_Velocity.x = 0f;
					if (num5 != m_SelectionIndex)
					{
						m_Parent.SendEvent(this, 0, num5, 0f);
						m_SelectionIndex = num5;
					}
				}
			}
			if (!m_fingerOn && Mathf.Abs(m_Velocity.x) <= (float)num)
			{
				float num6 = m_CenterPos - (float)m_SelectionIndex * m_Spacing.x;
				if (Mathf.Abs(m_ScrollerPos - num6) > 20f)
				{
					m_ScrollerPos += Mathf.Sign(num6 - m_ScrollerPos) * 100f * Time.deltaTime * 10f;
				}
				else
				{
					m_ScrollerPos = num6;
				}
			}
		}
		else if (m_Dir == ScrollerDir.Vertical)
		{
			m_ScrollerPos += m_Velocity.y * Time.deltaTime * 100f;
			m_ScrollerPos = Mathf.Clamp(m_ScrollerPos, m_CenterPos, m_CenterPos + (float)(m_Controls.Count - 2) * m_Spacing.y);
			for (int j = 0; j < m_Controls.Count - 1; j++)
			{
				float num7 = m_ScrollerPos - (float)j * m_Spacing.y;
				float num8 = m_ClipRect.height * 0.5f + m_ClipRect.y;
				float num9 = num7 + 0.5f * (float)m_IconHeight;
				if (Mathf.Abs(num9 - num8) < m_Spacing.y / 2f)
				{
					int num10 = j;
					m_Velocity.y = 0f;
					if (num10 != m_SelectionIndex)
					{
						m_Parent.SendEvent(this, 0, num10, 0f);
						m_SelectionIndex = num10;
					}
				}
			}
			if (!m_fingerOn && Mathf.Abs(m_Velocity.y) <= (float)num)
			{
				float num11 = m_CenterPos + (float)m_SelectionIndex * m_Spacing.y;
				if (Mathf.Abs(m_ScrollerPos - num11) > 20f)
				{
					m_ScrollerPos += Mathf.Sign(num11 - m_ScrollerPos) * 100f * Time.deltaTime * 10f;
				}
				else
				{
					m_ScrollerPos = num11;
				}
			}
		}
		for (int k = 0; k < m_Controls.Count - 1; k++)
		{
			float num12 = 0f;
			float num13 = 0f;
			if (m_Dir == ScrollerDir.Horizontal)
			{
				num12 = m_Spacing.x * (float)(k / m_CountPerRow) + m_ScrollerPos;
				num13 = m_IconHeight * (k % m_CountPerRow) + (int)(m_ClipRect.y + 0.5f * (m_ClipRect.height - (float)m_IconHeight));
				UIImage uIImage = m_Controls[k] as UIImage;
				uIImage.Rect = new Rect(num12, num13, m_IconWidth, m_IconHeight);
				m_overlayImageList[k].Rect = new Rect(num12, num13, m_IconWidth, m_IconHeight);
				m_MaskList[k].Rect = uIImage.Rect;
				if (m_SelectionIndex == k)
				{
					uIImage.SetTextureSize(new Vector2((float)m_IconWidth * 1f, (float)m_IconHeight * 1f));
					if (m_overlayInfoList.Count > 0)
					{
						m_overlayImageList[k].SetTextureSize(new Vector2(m_overlayInfoList[0].m_TexRect.width * 1f, m_overlayInfoList[0].m_TexRect.height * 1f));
					}
				}
				else
				{
					uIImage.SetTextureSize(new Vector2((float)m_IconWidth * 0.7f, (float)m_IconHeight * 0.7f));
					if (m_overlayInfoList.Count > 0)
					{
						m_overlayImageList[k].SetTextureSize(new Vector2(m_overlayInfoList[0].m_TexRect.width * 0.7f, m_overlayInfoList[0].m_TexRect.height * 0.7f));
					}
				}
				uIImage.SetClip(m_ClipRect);
				m_overlayImageList[k].SetClip(m_ClipRect);
			}
			else
			{
				if (m_Dir != ScrollerDir.Vertical)
				{
					continue;
				}
				int num14 = k;
				num12 = m_IconWidth * (num14 % m_CountPerRow) + (int)(m_ClipRect.x + 0.5f * (m_ClipRect.width - (float)m_IconWidth));
				num13 = m_ScrollerPos - m_Spacing.y * (float)(num14 / m_CountPerRow);
				num13 = Mathf.Clamp(num13, m_CenterPos - m_Spacing.y * 1.5f, m_CenterPos + m_Spacing.y * 1.5f);
				UIImage uIImage2 = m_Controls[k] as UIImage;
				uIImage2.Rect = new Rect(num12, num13, m_IconWidth, m_IconHeight);
				m_overlayImageList[num14].Rect = new Rect(num12, num13, m_IconWidth, m_IconHeight);
				m_MaskList[num14].Rect = new Rect(num12, num13, m_IconWidth, m_IconHeight);
				if (m_Scalable)
				{
					float num15 = uIImage2.Rect.y + uIImage2.Rect.height * 0.5f;
					float num16 = m_ClipRect.y + m_ClipRect.height * 0.5f;
					float num17 = Mathf.Abs(num16 - num15);
					float num18 = 1f - num17 / m_ClipRect.height;
					uIImage2.SetTextureSize(new Vector2((float)m_IconWidth * num18, (float)m_IconHeight * num18));
					m_MaskList[num14].SetTextureSize(new Vector2((float)m_IconWidth * num18, (float)m_IconHeight * num18));
					if (m_overlayInfoList.Count > 0)
					{
						m_overlayImageList[num14].SetTextureSize(AutoRect.AutoSize(new Vector2(m_overlayInfoList[0].m_TexRect.width * num18, m_overlayInfoList[0].m_TexRect.height * num18)));
					}
				}
				uIImage2.SetClip(m_ClipRect);
				m_overlayImageList[num14].SetClip(m_ClipRect);
				m_MaskList[num14].SetClip(m_ClipRect);
			}
		}
	}
}
