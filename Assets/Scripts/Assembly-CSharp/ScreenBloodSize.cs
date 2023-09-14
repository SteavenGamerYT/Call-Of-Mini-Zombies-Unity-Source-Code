using UnityEngine;

public class ScreenBloodSize : MonoBehaviour
{
	private void Start()
	{
		float num = (float)Screen.width / (float)Screen.height;
		base.transform.localScale = new Vector3(2.8f * num, 1.8666667f * num, 0.5f);
	}
}
