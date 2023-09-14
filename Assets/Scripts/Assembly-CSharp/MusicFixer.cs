using UnityEngine;

public class MusicFixer : MonoBehaviour
{
	private bool gotIt;

	private bool supposedToBeGone = true;

	private void Start()
	{
		Application.targetFrameRate = 120;
	}

	private void Update()
	{
		if (!Application.isMobilePlatform)
		{
			if (Application.loadedLevelName.StartsWith("Zombie3D"))
			{
				if (!gotIt)
				{
					supposedToBeGone = true;
					gotIt = true;
				}
				GameObject.Find("UIMesh").GetComponent<MeshRenderer>().enabled = !supposedToBeGone;
				Cursor.visible = !supposedToBeGone;
				Screen.lockCursor = supposedToBeGone;
			}
			else
			{
				supposedToBeGone = false;
				gotIt = false;
				Cursor.visible = true;
				Screen.lockCursor = false;
			}
			if (Input.GetKeyDown(KeyCode.F1))
			{
				supposedToBeGone = !supposedToBeGone;
			}
		}
		if (Application.loadedLevelName.StartsWith("Zombie3D") && GetComponent<AudioSource>().isPlaying)
		{
			GetComponent<AudioSource>().Stop();
		}
		if (!Application.loadedLevelName.StartsWith("Zombie3D") && !GetComponent<AudioSource>().isPlaying)
		{
			GetComponent<AudioSource>().Play();
		}
	}
}
