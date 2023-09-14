using UnityEngine;

public class UIColoredButton : UIClickButton
{
	protected Color animatedColor = Color.red;

	protected float alpha;

	public void SetAnimatedColor(Color color)
	{
		animatedColor = color;
	}

	public override void Draw()
	{
		alpha = Mathf.PingPong(Time.time * 4f, 1f);
		SetColor(State.Normal, new Color(animatedColor.r, animatedColor.g, animatedColor.b, alpha));
		SetColor(State.Pressed, new Color(animatedColor.r, animatedColor.g, animatedColor.b, 1f));
		base.Draw();
	}
}
