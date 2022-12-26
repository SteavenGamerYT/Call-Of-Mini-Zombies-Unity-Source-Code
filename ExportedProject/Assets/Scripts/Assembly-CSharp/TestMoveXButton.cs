using UnityEngine;

public class TestMoveXButton : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnSliderBegin()
	{
	}

	private void OnSliderMove()
	{
	}

	private void OnSliderEnd()
	{
		Debug.Log("OnSliderEnd" + base.transform.position);
	}
}
