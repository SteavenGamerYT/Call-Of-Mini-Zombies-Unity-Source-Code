using System.Collections.Generic;
using UnityEngine;

namespace Zombie3D
{
	public class BombQuest : Quest
	{
		protected bool bombCompleted;

		protected Vector3 exitPosition;

		protected Renderer exitGlowRenderer;

		protected float radius = 2f;

		protected int bombTotal;

		protected int bombLeft;

		public override void Init()
		{
			base.Init();
			Transform transform = GameObject.Find("Exit").transform;
			exitPosition = transform.position;
			exitGlowRenderer = transform.Find("glow").GetComponent<Renderer>();
			exitGlowRenderer.enabled = false;
			questType = QuestType.Bomb;
			bombTotal = GameApp.GetInstance().GetGameScene().GetBombSpots()
				.Count;
			bombLeft = bombTotal;
		}

		public override void DoLogic()
		{
			base.DoLogic();
			Player player = gameScene.GetPlayer();
			if (bombCompleted && (player.GetTransform().position - exitPosition).sqrMagnitude < radius * radius)
			{
				questCompleted = true;
			}
		}

		public void CheckAllBombComplete()
		{
			List<BombSpot> bombSpots = gameScene.GetBombSpots();
			bombLeft = bombTotal;
			foreach (BombSpot item in bombSpots)
			{
				if (item.GetSpotState() == BombSpot.BombSpotState.Installed)
				{
					bombLeft--;
				}
			}
			if (bombLeft == 0)
			{
				bombCompleted = true;
				if (!exitGlowRenderer.enabled)
				{
					exitGlowRenderer.enabled = true;
				}
			}
		}

		public override string GetQuestInfo()
		{
			string result = "Mission: " + questType.ToString() + " " + bombLeft + "/" + bombTotal;
			if (bombCompleted && !questCompleted)
			{
				result = "Mission: Bomb Complete, Get to The Exit!";
			}
			if (questCompleted)
			{
				result = "Mission Complete";
			}
			return result;
		}
	}
}
