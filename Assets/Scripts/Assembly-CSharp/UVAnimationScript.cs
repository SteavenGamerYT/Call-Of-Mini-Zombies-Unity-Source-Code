using UnityEngine;

public class UVAnimationScript : MonoBehaviour
{
	public bool u = true;

	public bool v = true;

	public float scrollSpeed = 1f;

	protected float alpha;

	protected float startTime;

	public string alphaPropertyName = "_Alpha";

	public string texturePropertyName = "_MainTex";

	public Material Eff_Material;

	private void Start()
	{
		startTime = Time.time;
	}

	private void Update()
	{
		float num = Time.time * scrollSpeed % 1f;
		if (Eff_Material != null)
		{
			if (u && v)
			{
				Eff_Material.SetTextureOffset(texturePropertyName, new Vector2(num, num));
			}
			else if (u)
			{
				Eff_Material.SetTextureOffset(texturePropertyName, new Vector2(num, 0f));
			}
			else if (v)
			{
				Eff_Material.SetTextureOffset(texturePropertyName, new Vector2(0f, num));
			}
		}
		else if (u && v)
		{
			GetComponent<Renderer>().material.SetTextureOffset(texturePropertyName, new Vector2(num, num));
		}
		else if (u)
		{
			GetComponent<Renderer>().material.SetTextureOffset(texturePropertyName, new Vector2(num, 0f));
		}
		else if (v)
		{
			GetComponent<Renderer>().material.SetTextureOffset(texturePropertyName, new Vector2(0f, num));
		}
	}
}
