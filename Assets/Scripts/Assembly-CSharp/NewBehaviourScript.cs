using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
	public GameObject slider_obj;

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

	private void OnTUIMoveBegin(TUIInput input)
	{
		Debug.Log("OnTUIMoveBegin" + input.position);
		slider_obj.transform.position = new Vector3(input.position.x, input.position.y, slider_obj.transform.localPosition.z);
		slider_obj.GetComponent<TUIButtonSliderCallBack>().SimCommandDown(input);
	}

	private void OnTUIMoveEnd(TUIInput input)
	{
		Debug.Log("OnTUIMoveEnd" + input.position);
	}
}
