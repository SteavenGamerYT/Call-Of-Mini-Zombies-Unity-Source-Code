using UnityEngine;

public class WayPointScript : MonoBehaviour
{
	public WayPointScript[] nodes;

	public WayPointScript parent;

	public float[] weights;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnDrawGizmos()
	{
		if (base.transform.position.y < 10005f)
		{
			Gizmos.color = Color.white;
		}
		else
		{
			Gizmos.color = Color.magenta;
		}
		Gizmos.DrawSphere(base.transform.position, 1f);
		WayPointScript[] array = nodes;
		WayPointScript[] array2 = array;
		foreach (WayPointScript wayPointScript in array2)
		{
			Gizmos.DrawLine(base.transform.position, wayPointScript.transform.position);
		}
	}
}
