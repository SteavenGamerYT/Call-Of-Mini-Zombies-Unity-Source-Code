using System;
using System.Collections;
using UnityEngine;

namespace Zombie3D
{
	public class BombSpot
	{
		public enum BombSpotState
		{
			UnInstalled = 0,
			Installing = 1,
			Installed = 2
		}

		public GameScene gameScene;

		public GameObject bombSpotObj;

		public float spotRadius = 5f;

		public float installTimeTakes = 5f;

		protected float lastInstallTime;

		protected BombSpotState bss;

		protected Transform spotTransform;

		public BombSpotState GetSpotState()
		{
			return bss;
		}

		public void Init()
		{
			spotTransform = bombSpotObj.transform;
			gameScene = GameApp.GetInstance().GetGameScene();
		}

		public void DoLogic()
		{
			if (bss == BombSpotState.Installing && Time.time - lastInstallTime > installTimeTakes)
			{
				bss = BombSpotState.Installed;
				bombSpotObj.transform.Find("glow").GetComponent<Renderer>().enabled = true;
				BombQuest bombQuest = gameScene.GetQuest() as BombQuest;
				bombQuest.CheckAllBombComplete();
			}
			if (bss != BombSpotState.Installing)
			{
				return;
			}
			Hashtable enemies = gameScene.GetEnemies();
			IEnumerator enumerator = enemies.Values.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Enemy enemy = (Enemy)enumerator.Current;
					if (enemy.GetState() != Enemy.DEAD_STATE && (enemy.GetPosition() - spotTransform.position).sqrMagnitude < spotRadius * spotRadius)
					{
						bss = BombSpotState.UnInstalled;
						break;
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = enumerator as IDisposable) != null)
				{
					disposable.Dispose();
				}
			}
		}

		public bool CheckInSpot()
		{
			if (bss != 0)
			{
				return false;
			}
			Player player = gameScene.GetPlayer();
			float sqrMagnitude = (player.GetTransform().position - spotTransform.position).sqrMagnitude;
			if (sqrMagnitude < spotRadius * spotRadius)
			{
				return true;
			}
			return false;
		}

		public void Install()
		{
			lastInstallTime = Time.time;
			bss = BombSpotState.Installing;
		}

		public bool isInstalling()
		{
			return bss == BombSpotState.Installing;
		}

		public float GetInstallingProgress()
		{
			return (Time.time - lastInstallTime) / installTimeTakes;
		}
	}
}
