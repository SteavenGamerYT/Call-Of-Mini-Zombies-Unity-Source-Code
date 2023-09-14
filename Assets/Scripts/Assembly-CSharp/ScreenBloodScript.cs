using UnityEngine;

public class ScreenBloodScript : MonoBehaviour
{
	public float scrollSpeed = 1f;

	protected float alpha;

	protected float startTime;

	protected float deltaTime;

	public string alphaPropertyName = "_Alpha";

	private void Start()
	{
		alpha = GetComponent<Renderer>().material.GetFloat(alphaPropertyName);
		startTime = Time.time;
	}

	public void NewBlood(float damage)
	{
		GetComponent<Renderer>().enabled = true;
		alpha = damage;
		alpha = Mathf.Clamp(alpha, 0f, 1f);
	}

	private void Update()
	{
		deltaTime += Time.deltaTime;
		if (!(deltaTime < 0.03f))
		{
			alpha -= 0.5f * deltaTime;
			if (alpha <= 0f)
			{
				GetComponent<Renderer>().enabled = false;
			}
			GetComponent<Renderer>().material.SetFloat(alphaPropertyName, alpha);
			deltaTime = 0f;
		}
	}
}
