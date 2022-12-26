using UnityEngine;

public class KeepOnlyIfDebugBuild : MonoBehaviour
{
	private void Start()
	{
		if (!Debug.isDebugBuild || Application.platform == RuntimePlatform.WindowsEditor)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
