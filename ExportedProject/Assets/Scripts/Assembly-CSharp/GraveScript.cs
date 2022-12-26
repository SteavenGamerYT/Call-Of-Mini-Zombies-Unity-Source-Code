using UnityEngine;

public class GraveScript : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawSphere(base.transform.position, 1f);
	}
}
