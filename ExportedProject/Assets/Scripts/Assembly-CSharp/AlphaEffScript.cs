using UnityEngine;

public class AlphaEffScript : MonoBehaviour
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

	protected Shader shaderEff;

	protected Material EffMater;

	private void Start()
	{
		shaderEff = Shader.Find("iPhone/LightMap_Effect");
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
			Material[] sharedMaterials = GetComponent<Renderer>().sharedMaterials;
			Material[] array = sharedMaterials;
			foreach (Material material in array)
			{
				if (material.shader == shaderEff)
				{
					value = material.GetColor(colorPropertyName);
					break;
				}
			}
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
		Material[] sharedMaterials2 = GetComponent<Renderer>().sharedMaterials;
		Material[] array2 = sharedMaterials2;
		foreach (Material material2 in array2)
		{
			if (material2.shader == shaderEff)
			{
				material2.SetColor(colorPropertyName, value);
			}
		}
		deltaTime = 0f;
	}
}
