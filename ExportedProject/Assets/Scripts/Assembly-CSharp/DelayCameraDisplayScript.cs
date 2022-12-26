using UnityEngine;

public class DelayCameraDisplayScript : MonoBehaviour
{
	public float delayTime = 0.5f;

	protected float startTime;

	private void Start()
	{
		GetComponent<Camera>().enabled = false;
		startTime = Time.time;
	}

	private void Update()
	{
		if (Time.time - startTime > delayTime)
		{
			GetComponent<Camera>().enabled = true;
		}
	}
}
