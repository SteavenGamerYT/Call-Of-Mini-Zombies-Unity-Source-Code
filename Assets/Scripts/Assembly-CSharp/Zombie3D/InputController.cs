using UnityEngine;

namespace Zombie3D
{
	public abstract class InputController
	{
		protected Touch[] lastTouch = new Touch[2];

		protected Vector2 cameraRotation = new Vector2(0f, 0f);

		protected Vector2 deflection;

		protected Vector2 thumbCenter;

		protected Vector2 thumbCenterToScreen;

		protected Vector2 shootThumbCenter;

		protected Vector2 shootThumbCenterToScreen;

		protected Vector2 lastShootTouch = default(Vector2);

		protected float touchX;

		protected float touchY;

		protected float thumbRadius;

		protected int thumbTouchFingerId = -1;

		protected int shootingTouchFingerId = -1;

		protected int moveTouchFingerId = -1;

		protected int moveTouchFingerId2 = -1;

		protected string phaseStr = ".";

		protected Vector3 moveDirection = Vector3.zero;

		protected GameScene gameScene;

		protected Player player;

		public bool EnableMoveInput { get; set; }

		public bool EnableTurningAround { get; set; }

		public bool EnableShootingInput { get; set; }

		public string PhaseStr
		{
			get
			{
				return phaseStr;
			}
		}

		public Vector2 LastTouchPos
		{
			get
			{
				return new Vector2(thumbCenterToScreen.x + touchX * thumbRadius, thumbCenterToScreen.y + touchY * thumbRadius);
			}
		}

		public Vector2 LastShootTouch
		{
			get
			{
				return new Vector2(lastShootTouch.x, lastShootTouch.y);
			}
		}

		public Vector2 ThumbCenter
		{
			get
			{
				return thumbCenter;
			}
		}

		public Vector2 ShootThumbCenter
		{
			get
			{
				return shootThumbCenter;
			}
		}

		public float ThumbRadius
		{
			get
			{
				return thumbRadius;
			}
		}

		public Vector2 CameraRotation
		{
			get
			{
				return cameraRotation;
			}
			set
			{
				cameraRotation = value;
			}
		}

		public Vector2 Deflection
		{
			get
			{
				return deflection;
			}
		}

		public int GetMoveTouchFingerID()
		{
			return thumbTouchFingerId;
		}

		public int GetShootingTouchFingerID()
		{
			return shootingTouchFingerId;
		}

		public void Init()
		{
			thumbCenter.x = AutoRect.AutoX(110f);
			thumbCenter.y = AutoRect.AutoY(530f);
			thumbRadius = AutoRect.AutoValue(85f);
			shootThumbCenter.x = AutoRect.AutoX(852f);
			shootThumbCenter.y = AutoRect.AutoY(530f);
			if (AutoRect.GetPlatform() == Platform.IPad)
			{
				thumbCenter.x = AutoRect.AutoX(66f);
				shootThumbCenter.x = AutoRect.AutoX(896f);
				thumbCenter.y = AutoRect.AutoY(500f);
				shootThumbCenter.y = AutoRect.AutoY(500f);
			}
			thumbCenterToScreen = new Vector2(thumbCenter.x, (float)Screen.height - thumbCenter.y);
			shootThumbCenterToScreen = new Vector2(shootThumbCenter.x, (float)Screen.height - shootThumbCenter.y);
			lastShootTouch = shootThumbCenterToScreen;
			for (int i = 0; i < 2; i++)
			{
				lastTouch[i] = default(Touch);
			}
			gameScene = GameApp.GetInstance().GetGameScene();
			player = gameScene.GetPlayer();
			EnableMoveInput = true;
			EnableShootingInput = true;
			EnableTurningAround = true;
		}

		public abstract void ProcessInput(float deltaTime, InputInfo inputInfo);
	}
}
