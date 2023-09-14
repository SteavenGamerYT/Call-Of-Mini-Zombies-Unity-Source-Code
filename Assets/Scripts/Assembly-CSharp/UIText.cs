using System.Collections;
using UnityEngine;

public class UIText : UIControlVisible
{
	public enum enAlignStyle
	{
		left = 0,
		center = 1,
		right = 2
	}

	private string m_Text;

	private Font m_Font;

	private float m_LineSpacing = 1f;

	private float m_CharacterSpacing = 1f;

	private Color m_Color = Color.black;

	private bool m_bIsAutoLine = true;

	private enAlignStyle m_AlignStyle;

	public Font Font
	{
		get
		{
			return m_Font;
		}
		set
		{
			m_Font = value;
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
			m_Rect = value;
			UpdateText();
		}
	}

	public float CharacterSpacing
	{
		get
		{
			return m_CharacterSpacing;
		}
		set
		{
			m_CharacterSpacing = value;
		}
	}

	public float LineSpacing
	{
		get
		{
			return m_LineSpacing;
		}
		set
		{
			m_LineSpacing = value;
		}
	}

	public enAlignStyle AlignStyle
	{
		get
		{
			return m_AlignStyle;
		}
		set
		{
			m_AlignStyle = value;
		}
	}

	public bool AutoLine
	{
		get
		{
			return m_bIsAutoLine;
		}
		set
		{
			m_bIsAutoLine = value;
		}
	}

	~UIText()
	{
	}

	public void Set(string font, string text, Color color)
	{
		m_Font = mgrFont.Instance().getFont(font);
		m_Color = color;
		m_Text = text;
		UpdateText();
	}

	public void SetColor(Color clr)
	{
		m_Color = clr;
		UpdateText();
	}

	public void SetFont(string name)
	{
		m_Font = mgrFont.Instance().getFont(name);
		UpdateText();
	}

	public void SetText(string text)
	{
		m_Text = text;
		UpdateText();
	}

	public string GetText()
	{
		return m_Text;
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

	private void UpdateText()
	{
		m_Sprite = null;
		if (m_Font == null || m_Text == null || m_Text.Length <= 0)
		{
			return;
		}
		ArrayList arrayList = new ArrayList();
		ArrayList arrayList2 = new ArrayList();
		string[] array = m_Text.Split('\n');
		if (m_bIsAutoLine)
		{
			for (int i = 0; i < array.Length; i++)
			{
				ArrayList arrayList3 = new ArrayList();
				string[] array2 = array[i].Split(' ');
				string text = string.Empty;
				float num = 0f;
				for (int j = 0; j < array2.Length; j++)
				{
					float textWidth = m_Font.GetTextWidth(array2[j], CharacterSpacing);
					if (num + textWidth <= Rect.width * 0.95f)
					{
						text += array2[j];
						num += textWidth;
					}
					else
					{
						text.Trim();
						if (string.Empty != text)
						{
							arrayList3.Add(text);
						}
						text = array2[j];
						num = textWidth;
					}
					text += " ";
					num += CharacterSpacing;
					num += m_Font.GetTextWidth(" ");
				}
				text.Trim();
				if (string.Empty != text)
				{
					arrayList3.Add(text);
				}
				for (int k = 0; k < arrayList3.Count; k++)
				{
					arrayList2.Add(arrayList3[k]);
				}
			}
		}
		else
		{
			for (int l = 0; l < array.Length; l++)
			{
				arrayList2.Add(array[l]);
			}
		}
		float num2 = (float)m_Font.CellHeight + LineSpacing;
		int num3 = m_Font.TextureWidth / m_Font.CellWidth;
		for (int m = 0; m < arrayList2.Count; m++)
		{
			float num4 = 0f;
			for (int n = 0; n < ((string)arrayList2[m]).Length; n++)
			{
				char c = ((string)arrayList2[m])[n];
				float charWidth = m_Font.getCharWidth(c);
				int num5 = c - 32;
				int num6 = num5 % num3;
				int num7 = num5 / num3;
				float x = num6 * m_Font.CellWidth;
				float y = num7 * m_Font.CellHeight;
				UISprite uISprite = new UISprite();
				uISprite.Position = new Vector2(m_Rect.x + num4 + (float)(m_Font.CellWidth / 2) * ResolutionConstant.R, m_Rect.y + m_Rect.height - (float)(m + 1) * num2 * ResolutionConstant.R + ResolutionConstant.R * (float)m_Font.CellHeight / 2f);
				uISprite.Size = new Vector2((float)m_Font.CellWidth * ResolutionConstant.R, (float)m_Font.CellHeight * ResolutionConstant.R);
				uISprite.Material = m_Font.getTexture();
				uISprite.TextureRect = new Rect(x, y, m_Font.CellWidth, m_Font.CellHeight);
				uISprite.Color = m_Color;
				if (m_Clip)
				{
					uISprite.SetClip(m_ClipRect);
				}
				arrayList.Add(uISprite);
				num4 += charWidth + CharacterSpacing;
			}
		}
		if (AlignStyle == enAlignStyle.center)
		{
			int num8 = 0;
			for (int num9 = 0; num9 < arrayList2.Count; num9++)
			{
				string text2 = (string)arrayList2[num9];
				float textWidth2 = m_Font.GetTextWidth(text2, CharacterSpacing);
				float num10 = (Rect.width - textWidth2) / 2f;
				float num11 = (Rect.height - (float)m_Font.CellHeight * ResolutionConstant.R - Rect.height * 0.1f) / 2f;
				for (int num12 = 0; num12 < text2.Length; num12++)
				{
					((UISprite)arrayList[num12 + num8]).Position = new Vector2(((UISprite)arrayList[num12 + num8]).Position.x + num10, ((UISprite)arrayList[num12 + num8]).Position.y - num11);
				}
				num8 += text2.Length;
			}
		}
		else if (AlignStyle == enAlignStyle.right)
		{
			int num13 = 0;
			for (int num14 = 0; num14 < arrayList2.Count; num14++)
			{
				string text3 = (string)arrayList2[num14];
				float textWidth3 = m_Font.GetTextWidth(text3, CharacterSpacing);
				float num15 = Rect.width - textWidth3;
				for (int num16 = 0; num16 < text3.Length; num16++)
				{
					((UISprite)arrayList[num16 + num13]).Position = new Vector2(((UISprite)arrayList[num16 + num13]).Position.x + num15, ((UISprite)arrayList[num16 + num13]).Position.y);
				}
				num13 += text3.Length;
			}
		}
		m_Sprite = new UISprite[arrayList.Count];
		for (int num17 = 0; num17 < arrayList.Count; num17++)
		{
			m_Sprite[num17] = (UISprite)arrayList[num17];
		}
	}
}
