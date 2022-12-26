using UnityEngine;

public class BodyGeneratorScript : MonoBehaviour
{
	public Timer timer = new Timer();

	public ResourceConfigScript rConfig;

	public EnemyConfigScript eConfig;

	private void Start()
	{
		rConfig = GameObject.Find("GameResourceConfig").GetComponent<ResourceConfigScript>();
		eConfig = GameObject.Find("EnemyResourceConfig").GetComponent<EnemyConfigScript>();
		timer.SetTimer(4f, false);
	}

	public void PlayDead()
	{
		PlayBloodEffect();
		PlayBodyExlodeEffect();
		GameObject original = eConfig.deadhead[0];
		GameObject gameObject = Object.Instantiate(original, base.transform.position + new Vector3(0f, 2f, 0f), base.transform.rotation);
		gameObject.GetComponent<Rigidbody>().AddForce(Random.Range(-5, 5), Random.Range(-5, 0), Random.Range(-5, 5), ForceMode.Impulse);
	}

	public void PlayBodyExlodeEffect()
	{
		GameObject original = eConfig.deadbody[0];
		Object.Instantiate(original, base.transform.position + new Vector3(0f, 0.2f, 0f), Quaternion.identity);
	}

	public void PlayBloodEffect()
	{
		GameObject deadBlood = rConfig.deadBlood;
		int num = Random.Range(0, 100);
		float y = 10000.119f;
		GameObject original;
		if (num > 50)
		{
			original = rConfig.deadFoorblood;
		}
		else
		{
			original = rConfig.deadFoorblood2;
			y = 10000.109f;
		}
		Object.Instantiate(deadBlood, base.transform.position + new Vector3(0f, 0.5f, 0f), Quaternion.Euler(0f, 0f, 0f));
		Object.Instantiate(original, new Vector3(base.transform.position.x, y, base.transform.position.z), Quaternion.Euler(270f, 0f, 0f));
	}

	private void Update()
	{
		if (timer.Ready())
		{
			PlayDead();
			timer.Do();
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.magenta;
		Gizmos.DrawSphere(base.transform.position, 1f);
	}
}
