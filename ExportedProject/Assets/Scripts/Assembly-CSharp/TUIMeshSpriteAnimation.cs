using UnityEngine;

public class TUIMeshSpriteAnimation : TUIMeshSprite
{
	public string[] frame_array;

	public int frameRate = 10;

	protected int currentFrame;

	protected float lastFrameChangeTime;

	public bool isLoop;

	public bool is_animate;

	public TUIMeshSpriteAnimationEnd on_animation_end;

	public override void UpdateMesh()
	{
		if (is_animate && frame_array != null)
		{
			if (Time.time - lastFrameChangeTime > 1f / (float)frameRate)
			{
				currentFrame++;
				if (currentFrame == frame_array.Length)
				{
					if (!isLoop)
					{
						if (on_animation_end != null)
						{
							on_animation_end(this);
						}
						Static = true;
						currentFrame = frame_array.Length - 1;
					}
					else
					{
						currentFrame = 0;
					}
				}
				lastFrameChangeTime = Time.time;
			}
			frameName = frame_array[currentFrame];
		}
		base.UpdateMesh();
	}

	public void Play()
	{
		base.gameObject.active = true;
		Static = false;
		currentFrame = 0;
		is_animate = true;
	}
}
