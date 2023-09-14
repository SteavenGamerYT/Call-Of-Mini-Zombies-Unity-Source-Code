using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TUICamera : MonoBehaviour
{
	public void Initialize(bool landscape, int layer, int depth)
	{
		float width;
		float height;
		bool hd;
		GetScreenInfo(out width, out height, out hd);
		if (landscape)
		{
			float num = width;
			width = height;
			height = num;
		}
		base.transform.localPosition = Vector3.zero;
		base.transform.localRotation = Quaternion.identity;
		base.transform.localScale = Vector3.one;
		GetComponent<Camera>().transform.localPosition = new Vector3(1f / (hd ? 4f : 2f), -1f / (hd ? 4f : 2f), 0f);
		GetComponent<Camera>().clearFlags = CameraClearFlags.Nothing;
		GetComponent<Camera>().backgroundColor = Color.white;
		GetComponent<Camera>().nearClipPlane = -128f;
		GetComponent<Camera>().farClipPlane = 128f;
		GetComponent<Camera>().orthographic = true;
		GetComponent<Camera>().depth = depth;
		GetComponent<Camera>().cullingMask = 1 << layer;
		GetComponent<Camera>().aspect = width / height;
		GetComponent<Camera>().orthographicSize = height / (hd ? 4f : 2f);
	}

	private void GetScreenInfo(out float width, out float height, out bool hd)
	{
		width = 0f;
		height = 0f;
		hd = false;
		if (Application.isPlaying)
		{
			if (Mathf.Max(Screen.width, Screen.height) > 1000)
			{
				width = 768f;
				height = 1024f;
				hd = true;
			}
			else if (Mathf.Max(Screen.width, Screen.height) > 900)
			{
				width = 640f;
				height = 960f;
				hd = true;
			}
			else
			{
				width = 320f;
				height = 480f;
				hd = false;
			}
		}
		else
		{
			width = 320f;
			height = 480f;
			hd = false;
		}
	}
}
