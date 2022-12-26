using System.Collections.Generic;
using UnityEngine;

public class UIDragGrid : UIPanel, UIHandler
{
	public enum Command
	{
		DragBegin = 0,
		DragMove = 1,
		DragEnd = 2,
		DragExchange = 3,
		DragOutSide = 4
	}

	public class UIDragIcon
	{
		public UIMove m_UIMove;

		public UIImage m_Image;

		public UIImage m_Background;
	}

	protected List<UIDragIcon> m_dragIcons = new List<UIDragIcon>();

	protected int m_GridCount;

	protected Rect m_IconRect;

	public UIDragGrid(int gridCount)
	{
		m_GridCount = gridCount;
		SetUIHandler(this);
	}

	public void AddGrid(Rect gridRect, Material backMaterial, Rect backTexPos)
	{
		UIDragIcon uIDragIcon = new UIDragIcon();
		uIDragIcon.m_Background = new UIImage();
		uIDragIcon.m_Background.Rect = gridRect;
		uIDragIcon.m_Background.SetTexture(backMaterial, backTexPos, AutoRect.AutoSize(backTexPos));
		uIDragIcon.m_Background.SetParent(this);
		uIDragIcon.m_Background.Enable = false;
		uIDragIcon.m_Image = new UIImage();
		uIDragIcon.m_Image.Rect = gridRect;
		uIDragIcon.m_Image.SetParent(this);
		uIDragIcon.m_Image.Enable = false;
		uIDragIcon.m_UIMove = new UIMove();
		uIDragIcon.m_UIMove.Rect = gridRect;
		uIDragIcon.m_UIMove.Enable = true;
		Add(uIDragIcon.m_UIMove);
		m_IconRect = gridRect;
		m_dragIcons.Add(uIDragIcon);
	}

	public void SetGridTexturePosition(int fromID, int toID)
	{
		m_dragIcons[fromID].m_Image.Rect = m_dragIcons[toID].m_UIMove.Rect;
	}

	public void HideGridTexture(int gridID)
	{
		m_dragIcons[gridID].m_Image.Visible = false;
	}

	public void SetGridTexture(int gridID, Material material, Rect textRect)
	{
		m_dragIcons[gridID].m_Image.Visible = true;
		m_dragIcons[gridID].m_Image.SetTexture(material, textRect, AutoRect.AutoSize(textRect));
	}

	public override void Draw()
	{
		base.Draw();
		if (!Visible)
		{
			return;
		}
		foreach (UIDragIcon dragIcon in m_dragIcons)
		{
			if (dragIcon.m_Background.Visible)
			{
				dragIcon.m_Background.Draw();
			}
		}
		foreach (UIDragIcon dragIcon2 in m_dragIcons)
		{
			if (dragIcon2.m_Image.Visible)
			{
				dragIcon2.m_Image.Draw();
			}
		}
	}

	public void HandleEvent(UIControl control, int command, float wparam, float lparam)
	{
		for (int i = 0; i < m_dragIcons.Count; i++)
		{
			UIMove uIMove = m_dragIcons[i].m_UIMove;
			UIImage image = m_dragIcons[i].m_Image;
			if (control != uIMove)
			{
				continue;
			}
			switch (command)
			{
			case 3:
				image.Rect = new Rect(wparam - 0.5f * m_IconRect.width, lparam - 0.5f * m_IconRect.height, m_IconRect.width, m_IconRect.height);
				break;
			case 2:
			{
				image.Rect = uIMove.Rect;
				int num = -1;
				for (int j = 0; j < m_dragIcons.Count; j++)
				{
					if (m_dragIcons[j].m_UIMove.Rect.Contains(new Vector2(wparam, lparam)))
					{
						num = j;
					}
				}
				if (num == -1)
				{
					image.Rect = new Rect(-1000f, -1000f, 200f, 200f);
					m_Parent.SendEvent(this, 4, i, 0f);
				}
				else
				{
					image.Rect = m_dragIcons[num].m_UIMove.Rect;
					m_Parent.SendEvent(this, 3, i, num);
				}
				break;
			}
			}
		}
	}
}
