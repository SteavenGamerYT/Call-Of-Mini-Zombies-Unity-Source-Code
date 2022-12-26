using UnityEngine;

public class UIColoredImage : UIImage
{
	protected Color animatedColor = Color.white;

	protected float alpha;

	public void SetAnimatedColor(Color color)
	{
		animatedColor = color;
	}

	public override void Draw()
	{
		alpha = Mathf.PingPong(Time.time * 3f, 1f);
		SetColor(new Color(animatedColor.r, animatedColor.g, animatedColor.b, alpha));
		base.Draw();
	}
}
