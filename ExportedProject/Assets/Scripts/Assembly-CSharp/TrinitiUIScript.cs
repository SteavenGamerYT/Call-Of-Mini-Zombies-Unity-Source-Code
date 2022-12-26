using System.Collections;
using UnityEngine;
using Zombie3D;

public class TrinitiUIScript : MonoBehaviour
{
	private IEnumerator Start()
	{
		OpenClickPlugin.Initialize("567D21BF-DA59-41F2-B7CC-9951F6187640");
		TapjoyPlugin.RequestConnect("a5e2b768-5c51-424c-aed2-62de204a8272", "gmupeHGvRpp8CXmfNsd9");
		GameCenterPlugin.Login();
		if (Application.isEditor || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.WindowsPlayer)
		{
			Application.runInBackground = true;
		}
		else
		{
			Application.runInBackground = false;
		}
		switch (Input.deviceOrientation)
		{
		case DeviceOrientation.Portrait:
		case DeviceOrientation.PortraitUpsideDown:
		case DeviceOrientation.FaceUp:
		case DeviceOrientation.FaceDown:
			Screen.orientation = ScreenOrientation.LandscapeLeft;
			yield return new WaitForSeconds(0.5f);
			Screen.orientation = ScreenOrientation.AutoRotation;
			break;
		}
		yield return 1;
		GameApp.GetInstance().GetGameState().PaperMenuStatus = PaperUIEnterStatus.StartMenu;
		Application.LoadLevel("CoopAdTUI");
	}
}
