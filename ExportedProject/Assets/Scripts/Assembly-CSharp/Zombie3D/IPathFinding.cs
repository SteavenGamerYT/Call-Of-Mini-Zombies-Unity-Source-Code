using System.Collections.Generic;
using UnityEngine;

namespace Zombie3D
{
	public interface IPathFinding
	{
		Stack<Transform> FindPath(Vector3 enemyPos, Vector3 playerPos);

		Transform GetNextWayPoint(Vector3 enemyPos, Vector3 playerPos);

		void ClearPath();

		bool HavePath();

		void PopNode();

		void InitPath(GameObject[] scene_points);
	}
}
