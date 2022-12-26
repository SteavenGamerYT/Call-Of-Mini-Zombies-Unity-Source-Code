using UnityEngine;
using Zombie3D;

[AddComponentMenu("TPS/TopWatchingCamera")]
public class TopWatchingCameraScript : BaseCameraScript
{
	protected bool cameraset;

	protected Vector3 absoluteDistanceFromPlayer;

	public override CameraType GetCameraType()
	{
		return CameraType.TopWatchingCamera;
	}

	private void Awake()
	{
		cameraTransform = Camera.main.transform;
	}

	private void Start()
	{
	}

	public override void Init()
	{
		base.Init();
		moveTo = player.GetTransform().TransformPoint(cameraDistanceFromPlayer);
		absoluteDistanceFromPlayer = moveTo - player.GetTransform().position;
		base.transform.LookAt(player.GetTransform());
		started = true;
	}

	public override void CreateScreenBlood(float damage)
	{
		if (bs != null)
		{
			bs.NewBlood(damage);
		}
		else
		{
			Debug.Log("bs null");
		}
	}

	private void Update()
	{
	}

	private void LateUpdate()
	{
		if (started)
		{
			deltaTime = Time.deltaTime;
			float num = player.InputController.CameraRotation.x;
			float num2 = player.InputController.CameraRotation.y;
			if (Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.Android)
			{
				num = Input.GetAxis("Mouse X");
				num2 = Input.GetAxis("Mouse Y");
			}
			angelH += num * deltaTime * cameraSwingSpeed;
			angelV += num2 * deltaTime * cameraSwingSpeed;
			angelV = Mathf.Clamp(angelV, minAngelV, maxAngelV);
			if (gameScene.GamePlayingState == PlayingState.GamePlaying)
			{
				player.GetTransform().rotation = Quaternion.Euler(0f, angelH, 0f);
				moveTo = player.GetTransform().position + absoluteDistanceFromPlayer;
				cameraTransform.position = moveTo;
			}
			lastUpdateTime = Time.time;
		}
	}
}
