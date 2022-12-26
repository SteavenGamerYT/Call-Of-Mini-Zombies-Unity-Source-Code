using UnityEngine;

public class InitiatePerformanceScript : MonoBehaviour
{
	public EnemyConfigScript rConfig;

	protected Timer timer = new Timer();

	private void Start()
	{
		rConfig = GameObject.Find("EnemyResourceConfig").GetComponent<EnemyConfigScript>();
		timer.SetTimer(1f, false);
	}

	private void Update()
	{
		if (timer.Ready())
		{
			for (int i = 0; i < 10; i++)
			{
				GameObject original = rConfig.deadbody[0];
				GameObject obj = Object.Instantiate(original, base.transform.position + new Vector3(0f, 0.2f, 0f), Quaternion.identity);
				Object.Destroy(obj);
			}
			timer.Do();
		}
	}
}
