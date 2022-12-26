using UnityEngine;

public class TexturePosInfo
{
	public Material m_Material;

	public Rect m_TexRect;

	public Vector2 m_Size;

	public TexturePosInfo(Material material, Rect texRect)
	{
		m_Material = material;
		m_TexRect = texRect;
	}

	public TexturePosInfo(Material material, Rect texRect, Vector2 size)
	{
		m_Material = material;
		m_TexRect = texRect;
		m_Size = size;
	}

	public TexturePosInfo()
	{
	}
}
