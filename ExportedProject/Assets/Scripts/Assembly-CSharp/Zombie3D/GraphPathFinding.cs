using System.Collections.Generic;
using UnityEngine;

namespace Zombie3D
{
	public class GraphPathFinding : IPathFinding
	{
		protected WayPointScript currentWayPoint;

		protected Stack<WayPointScript> openStack = new Stack<WayPointScript>();

		protected Stack<WayPointScript> closeStack = new Stack<WayPointScript>();

		protected Stack<Transform> path;

		protected GameObject[] points;

		public void InitPath(GameObject[] scene_points)
		{
			points = scene_points;
		}

		public Stack<Transform> FindPath(Vector3 enemyPos, Vector3 playerPos)
		{
			float num = 99999f;
			WayPointScript wayPointScript = null;
			WayPointScript wayPointScript2 = null;
			GameObject[] array = points;
			GameObject[] array2 = array;
			foreach (GameObject gameObject in array2)
			{
				WayPointScript component = gameObject.GetComponent<WayPointScript>();
				component.parent = null;
				float magnitude = (component.transform.position - enemyPos).magnitude;
				if (magnitude < num)
				{
					Ray ray = new Ray(enemyPos + new Vector3(0f, 0.5f, 0f), component.transform.position - enemyPos);
					RaycastHit hitInfo;
					if (!Physics.Raycast(ray, out hitInfo, magnitude, 100352))
					{
						wayPointScript = component;
						num = magnitude;
					}
				}
				wayPointScript2 = GameApp.GetInstance().GetGameScene().GetPlayer()
					.NearestWayPoint;
			}
			if (wayPointScript != null && wayPointScript2 != null)
			{
				path = SearchPath(wayPointScript, wayPointScript2);
			}
			if (wayPointScript2 == null)
			{
				Debug.Log("to null");
			}
			return path;
		}

		public Transform GetNextWayPoint(Vector3 enemyPos, Vector3 playerPos)
		{
			if (path != null && path.Count > 0)
			{
				return path.Peek();
			}
			path = FindPath(enemyPos, playerPos);
			if (path != null && path.Count > 0)
			{
				return path.Peek();
			}
			return null;
		}

		public void PopNode()
		{
			if (path != null && path.Count > 0)
			{
				path.Pop();
			}
		}

		public bool HavePath()
		{
			if (path != null && path.Count > 0)
			{
				return true;
			}
			return false;
		}

		public void ClearPath()
		{
			if (path != null)
			{
				path.Clear();
			}
		}

		public Stack<Transform> SearchPath(WayPointScript from, WayPointScript to)
		{
			Stack<Transform> stack = new Stack<Transform>();
			if (from == to)
			{
				stack.Push(to.transform);
				return stack;
			}
			openStack.Push(from);
			while (openStack.Count > 0)
			{
				if (openStack.Count > 100)
				{
					Debug.Log("Memeroy Explode! To many nodes in open stack..");
					Debug.Break();
					break;
				}
				WayPointScript wayPointScript = openStack.Pop();
				closeStack.Push(wayPointScript);
				WayPointScript[] nodes = wayPointScript.nodes;
				WayPointScript[] array = nodes;
				WayPointScript[] array2 = array;
				foreach (WayPointScript wayPointScript2 in array2)
				{
					if (wayPointScript2 == to)
					{
						wayPointScript2.parent = wayPointScript;
						break;
					}
					if (!openStack.Contains(wayPointScript2) && !closeStack.Contains(wayPointScript2))
					{
						wayPointScript2.parent = wayPointScript;
						openStack.Push(wayPointScript2);
					}
				}
			}
			openStack.Clear();
			closeStack.Clear();
			WayPointScript wayPointScript3 = to;
			stack.Push(to.transform);
			while (wayPointScript3.parent != null)
			{
				wayPointScript3 = wayPointScript3.parent;
				if (stack.Count > 30)
				{
					Debug.Log("Memeroy Explode! Parent Forever..");
					Debug.Break();
					break;
				}
				stack.Push(wayPointScript3.transform);
			}
			return stack;
		}
	}
}
