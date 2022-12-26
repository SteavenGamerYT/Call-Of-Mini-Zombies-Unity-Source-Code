using System.Collections.Generic;
using UnityEngine;

namespace Zombie3D
{
	public class FastPathFinding : IPathFinding
	{
		public Stack<Transform> FindPath(Vector3 enemyPos, Vector3 playerPos)
		{
			return null;
		}

		public Transform GetNextWayPoint(Vector3 enemyPos, Vector3 playerPos)
		{
			return null;
		}

		public void ClearPath()
		{
		}

		public bool HavePath()
		{
			return false;
		}

		public void PopNode()
		{
		}

		public void InitPath(GameObject[] scene_points)
		{
		}
	}
}
