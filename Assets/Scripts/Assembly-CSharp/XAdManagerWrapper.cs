using UnityEngine;

public class XAdManagerWrapper
{
	public static void SetVideoAdUrl(string url)
	{
		Debug.Log("SetVideoAdUrl");
	}

	public static void SetImageAdUrl(string url)
	{
		Debug.Log("SetImageAdUrl");
	}

	public static void SetImagePosition(int x, int y)
	{
		Debug.Log("SetImagePosition");
	}

	public static void ShowVideoAdLocal()
	{
		Debug.Log("ShowVideoAdLocal");
	}

	public static void ShowImageAd()
	{
		Debug.Log("ShowImageAd");
	}

	public static void HideImageAd()
	{
		Debug.Log("HideImageAd");
	}
}
