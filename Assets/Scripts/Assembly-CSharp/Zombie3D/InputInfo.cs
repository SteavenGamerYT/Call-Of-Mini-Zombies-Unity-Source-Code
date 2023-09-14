using UnityEngine;

namespace Zombie3D
{
	public class InputInfo
	{
		public bool fire;

		public bool stopFire;

		public Vector3 moveDirection = Vector3.zero;

		public bool IsMoving()
		{
			if (moveDirection.x != 0f || moveDirection.z != 0f)
			{
				return true;
			}
			return false;
		}
	}
}
