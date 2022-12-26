using UnityEngine;

public class AlphaAnimationScript : MonoBehaviour
{
	public float maxAlpha = 1f;

	public float minAlpha;

	public float animationSpeed = 5.5f;

	public float maxBright = 1f;

	public float minBright;

	public bool enableAlphaAnimation;

	public bool enableBrightAnimation;

	public string colorPropertyName = "_TintColor";

	protected float alpha;

	protected float startTime;

	protected bool increasing = true;

	public Color startColor = Color.yellow;

	protected float lastUpdateTime;

	protected float deltaTime;

	private void Start()
	{
		startTime = Time.time;
	}

	private void Update()
	{
		deltaTime += Time.deltaTime;
		if (deltaTime < 0.02f)
		{
			return;
		}
		Color value = Color.white;
		if (enableAlphaAnimation || enableBrightAnimation)
		{
			value = GetComponent<Renderer>().material.GetColor(colorPropertyName);
		}
		if (enableAlphaAnimation)
		{
			if (increasing)
			{
				value.a += animationSpeed * deltaTime;
				value.a = Mathf.Clamp(value.a, minAlpha, maxAlpha);
				if (value.a == maxAlpha)
				{
					increasing = false;
				}
			}
			else
			{
				value.a -= animationSpeed * deltaTime;
				value.a = Mathf.Clamp(value.a, minAlpha, maxAlpha);
				if (value.a == minAlpha)
				{
					increasing = true;
				}
			}
		}
		if (enableBrightAnimation)
		{
			if (increasing)
			{
				value.r += animationSpeed * deltaTime;
				value.g += animationSpeed * deltaTime;
				value.b += animationSpeed * deltaTime;
				if (value.r >= maxBright || value.g >= maxBright || value.b >= maxBright)
				{
					increasing = false;
				}
			}
			else
			{
				value.r -= animationSpeed * deltaTime;
				value.g -= animationSpeed * deltaTime;
				value.b -= animationSpeed * deltaTime;
				if (value.r <= minBright || value.g <= minBright || value.b <= minBright)
				{
					increasing = true;
				}
			}
		}
		GetComponent<Renderer>().material.SetColor(colorPropertyName, value);
		deltaTime = 0f;
	}
}
