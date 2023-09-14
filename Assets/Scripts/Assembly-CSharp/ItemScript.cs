using UnityEngine;
using Zombie3D;

public class ItemScript : MonoBehaviour
{
	public ItemType itemType;

	private bool moveUp;

	public Vector3 rotationSpeed = new Vector3(0f, 45f, 0f);

	public bool enableUpandDown = true;

	protected float deltaTime;

	public float moveSpeed = 0.2f;

	public float HighPos = 1.2f;

	public float LowPos = 1f;

	protected float floorY = 10000.1f;

	private void Start()
	{
		Ray ray = new Ray(base.transform.position + Vector3.up * 1f, Vector3.down);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 100f, 32768))
		{
			floorY = hitInfo.point.y;
		}
	}

	private void Update()
	{
		deltaTime += Time.deltaTime;
		if (deltaTime < 0.03f)
		{
			return;
		}
		base.transform.Rotate(rotationSpeed * deltaTime);
		if (enableUpandDown)
		{
			if (!moveUp)
			{
				float num = Mathf.MoveTowards(base.transform.position.y, floorY + LowPos, moveSpeed * deltaTime);
				base.transform.position = new Vector3(base.transform.position.x, num, base.transform.position.z);
				if (num <= floorY + LowPos)
				{
					moveUp = true;
				}
			}
			else
			{
				float num2 = Mathf.MoveTowards(base.transform.position.y, floorY + HighPos, moveSpeed * deltaTime);
				base.transform.position = new Vector3(base.transform.position.x, num2, base.transform.position.z);
				if (num2 >= floorY + HighPos)
				{
					moveUp = false;
				}
			}
		}
		deltaTime = 0f;
	}

	private void OnTriggerEnter(Collider c)
	{
		if (c.GetComponent<Collider>().gameObject.layer != 8)
		{
			return;
		}
		PlayerShell component = c.gameObject.GetComponent<PlayerShell>();
		if (!(component != null))
		{
			return;
		}
		if (GameApp.GetInstance().GetGameState().VS_mode)
		{
			if (component.m_player.net_player_id == GameApp.GetInstance().GetGameScene().GetPlayer()
				.net_player_id && component.m_player.OnPickUp(itemType))
			{
				Object.Destroy(base.gameObject);
			}
		}
		else
		{
			component.m_player.OnPickUp(itemType);
			Object.Destroy(base.gameObject);
		}
	}
}
