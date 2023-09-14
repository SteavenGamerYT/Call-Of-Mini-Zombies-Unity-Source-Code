using System.Collections.Generic;
using UnityEngine;

namespace Zombie3D
{
	public class TopWatchingInputController : InputController
	{
		public override void ProcessInput(float deltaTime, InputInfo inputInfo)
		{
			Vector3 getHitFlySpeed = player.GetHitFlySpeed;
			List<Weapon> weapons = GameApp.GetInstance().GetGameState().GetWeapons();
			Transform respawnTransform = player.GetRespawnTransform();
			if (Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.Android)
			{
				if (Input.GetButton("Fire1"))
				{
					player.Fire(deltaTime);
				}
				else
				{
					player.StopFire();
				}
				moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
			}
			else
			{
				touchX = 0f;
				touchY = 0f;
				lastShootTouch = shootThumbCenterToScreen;
				cameraRotation.x = 0f;
				cameraRotation.y = 0f;
				bool flag = false;
				for (int i = 0; i < Input.touchCount && i != 2; i++)
				{
					Touch touch = Input.GetTouch(i);
					phaseStr = touch.phase.ToString() + touch.fingerId + " p:" + touch.position.x + "," + touch.position.y;
					Vector2 vector = touch.position - thumbCenterToScreen;
					bool flag2 = vector.sqrMagnitude < thumbRadius * thumbRadius;
					bool flag3 = touch.fingerId == thumbTouchFingerId;
					if (touch.phase == TouchPhase.Stationary)
					{
						if (flag2 || flag3)
						{
							if (flag2)
							{
								touchX = vector.x / thumbRadius;
								touchY = vector.y / thumbRadius;
							}
							else
							{
								touchX = vector.x / thumbRadius;
								touchY = vector.y / thumbRadius;
								if (Mathf.Abs(touchX) > Mathf.Abs(touchY))
								{
									touchY /= Mathf.Abs(touchX);
									touchX = ((touchX > 0f) ? 1 : (-1));
								}
								else if (touchY != 0f)
								{
									touchX /= Mathf.Abs(touchY);
									touchY = ((touchY > 0f) ? 1 : (-1));
								}
								else
								{
									touchX = 0f;
									touchY = 0f;
								}
							}
							thumbTouchFingerId = touch.fingerId;
						}
						else if ((touch.position - shootThumbCenterToScreen).sqrMagnitude < thumbRadius * thumbRadius)
						{
							player.Fire(deltaTime);
							shootingTouchFingerId = touch.fingerId;
							flag = true;
							lastShootTouch = touch.position;
						}
					}
					else if (touch.phase == TouchPhase.Moved)
					{
						if (flag2 || flag3)
						{
							if (flag2)
							{
								touchX = vector.x / thumbRadius;
								touchY = vector.y / thumbRadius;
							}
							else
							{
								touchX = vector.x / thumbRadius;
								touchY = vector.y / thumbRadius;
								if (Mathf.Abs(touchX) > Mathf.Abs(touchY))
								{
									touchY /= Mathf.Abs(touchX);
									touchX = ((touchX > 0f) ? 1 : (-1));
								}
								else if (touchY != 0f)
								{
									touchX /= Mathf.Abs(touchY);
									touchY = ((touchY > 0f) ? 1 : (-1));
								}
								else
								{
									touchX = 0f;
									touchY = 0f;
								}
							}
							thumbTouchFingerId = touch.fingerId;
						}
						else
						{
							cameraRotation.x = touch.deltaPosition.x * 0.2f;
							cameraRotation.y = touch.deltaPosition.y * 0.2f;
							bool flag4 = (touch.position - shootThumbCenterToScreen).sqrMagnitude < thumbRadius * thumbRadius;
							if (shootingTouchFingerId == touch.fingerId || flag4)
							{
								player.Fire(deltaTime);
								flag = true;
								if (flag4)
								{
									lastShootTouch = touch.position;
								}
							}
						}
					}
					else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
					{
						if (touch.fingerId == thumbTouchFingerId)
						{
							thumbTouchFingerId = -1;
						}
						if (touch.fingerId == shootingTouchFingerId)
						{
							shootingTouchFingerId = -1;
						}
					}
					lastTouch[i] = touch;
				}
				if (!flag)
				{
					player.StopFire();
				}
				touchX = Mathf.Clamp(touchX, -1f, 1f);
				touchY = Mathf.Clamp(touchY, -1f, 1f);
				moveDirection = new Vector3(touchX, 0f, touchY);
			}
			moveDirection = respawnTransform.TransformDirection(moveDirection);
			moveDirection += Physics.gravity * deltaTime;
			getHitFlySpeed.x = Mathf.Lerp(getHitFlySpeed.x, 0f, 5f * Time.deltaTime);
			getHitFlySpeed.y = Mathf.Lerp(getHitFlySpeed.y, 0f, (0f - Physics.gravity.y) * Time.deltaTime);
			getHitFlySpeed.z = Mathf.Lerp(getHitFlySpeed.z, 0f, 5f * Time.deltaTime);
			for (int j = 1; j <= 3; j++)
			{
				if (Input.GetButton("Weapon" + j) && weapons[j - 1] != null)
				{
					player.ChangeWeapon(weapons[j - 1]);
					player.SendNetUserChangeWeaponMsg(j - 1);
				}
			}
			if (Input.GetAxis("Vertical") != 0f || Input.GetAxis("Horizontal") != 0f || touchX != 0f || touchY != 0f)
			{
				player.Run();
			}
			else
			{
				player.StopRun();
			}
		}
	}
}
