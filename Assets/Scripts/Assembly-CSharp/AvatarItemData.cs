using UnityEngine;
using Zombie3D;

public class AvatarItemData : MonoBehaviour
{
	public AvatarType avatar_type;

	public void SetAvatarItem()
	{
		switch (GameApp.GetInstance().GetGameState().GetAvatarData(avatar_type))
		{
		case AvatarState.ToBuy:
			base.transform.Find("but_tip").gameObject.active = true;
			break;
		case AvatarState.Avaliable:
			base.transform.Find("but_tip").gameObject.active = false;
			break;
		}
	}
}
