using System.Collections.Generic;
using UnityEngine;

public class UIAnimatedImage : UIImage
{
	protected List<TexturePosInfo> animationTexturesList = new List<TexturePosInfo>();

	protected int currentFrame;

	protected int frameRate = 10;

	protected float lastFrameChangeTime;

	public void AddAnimation(Material material, Rect texture_rect, Vector2 size)
	{
		animationTexturesList.Add(new TexturePosInfo(material, texture_rect, size));
	}

	public void SetAnimationFrameRate(int frameRate)
	{
		this.frameRate = frameRate;
	}

	public override void Draw()
	{
		Enable = false;
		TexturePosInfo texturePosInfo = animationTexturesList[currentFrame];
		SetTexture(texturePosInfo.m_Material, texturePosInfo.m_TexRect, texturePosInfo.m_Size);
		base.Draw();
		if (Time.time - lastFrameChangeTime > 1f / (float)frameRate)
		{
			currentFrame++;
			if (currentFrame == animationTexturesList.Count)
			{
				currentFrame = 0;
			}
			lastFrameChangeTime = Time.time;
		}
	}
}
