using UnityEngine;
using Zombie3D;

public class HellFireEnemyScript : MonoBehaviour
{
	protected GameScene gameScene;

	protected Player player;

	protected Weapon weapon;

	public void Start()
	{
		gameScene = GameApp.GetInstance().GetGameScene();
		if (gameScene != null)
		{
			player = gameScene.GetPlayer();
		}
	}

	private void OnParticleCollision(GameObject other)
	{
		if (other.layer == 8 && player != null && Time.time - player.lastFireDamagedTime > 0.5f)
		{
			player.OnHit(20f);
			player.lastFireDamagedTime = Time.time;
		}
	}
}
