using System.Collections;
using UnityEngine;
using Zombie3D;

public class PlayerShell : MonoBehaviour
{
	public Player m_player;

	private void Awake()
	{
	}

	private void Start()
	{
	}

	public Player InitNetPlayer()
	{
		m_player = new Multiplayer();
		((Multiplayer)m_player).NetInit();
		((Multiplayer)m_player).SetPlayerObj(base.gameObject, this);
		return m_player;
	}

	[RPC]
	public void RPCOnDead(NetworkMessageInfo info)
	{
		Debug.Log("get RPCOnDead");
		if (m_player != null && !m_player.PlayerObject.GetComponent<NetworkView>().isMine)
		{
			m_player.StopFire();
			m_player.OnDead();
			m_player.SetState(m_player.DEAD_STATE);
		}
	}

	[RPC]
	public void RPCOnRebirth(NetworkMessageInfo info)
	{
		if (m_player != null)
		{
			m_player.OnVSRebirth();
		}
	}

	[RPC]
	public void RPCAddVSWeapon(int type, NetworkMessageInfo info)
	{
		if (m_player != null)
		{
			m_player.AddVSWeapon(type);
		}
	}

	[RPC]
	public void RPCChangeWeapon(int type, NetworkMessageInfo info)
	{
		if (m_player != null)
		{
			((Multiplayer)m_player).ChangeWeaponWithindex(type);
		}
	}

	[RPC]
	public void RPCInitVSWeapon(int type1, float AttackFrequency1, int type2, float AttackFrequency2, int type3, float AttackFrequency3, NetworkMessageInfo info)
	{
		if (m_player != null)
		{
			((Multiplayer)m_player).InitWeaponList(type1, AttackFrequency1, type2, AttackFrequency2, type3, AttackFrequency3);
		}
	}

	[RPC]
	public void RPCSniperFire(Vector3 target, NetworkMessageInfo info)
	{
		if (m_player != null)
		{
			MultiSniper multiSniper = m_player.GetWeapon() as MultiSniper;
			if (multiSniper != null)
			{
				multiSniper.AddMultiTarget(target);
				Multiplayer multiplayer = m_player as Multiplayer;
				multiplayer.OnMultiSniperFire();
			}
		}
	}

	[RPC]
	public void RPCKillPlayer(NetworkMessageInfo info)
	{
		if (GetComponent<NetworkView>().isMine)
		{
			if (m_player != null)
			{
				m_player.PlusVsKillCount();
			}
		}
		else
		{
			GetComponent<NetworkView>().RPC("RPCKillPlayer", GetComponent<NetworkView>().owner);
		}
	}

	public IEnumerator SentRPCOnDead(NetworkView view)
	{
		yield return 1;
		view.RPC("RPCOnDead", RPCMode.Others);
		Debug.Log("sent RPCOnDead");
	}

	public IEnumerator SentRPCKillPlayer(NetworkView view)
	{
		yield return 1;
		view.RPC("RPCKillPlayer", view.owner);
		Debug.Log("Sent RPCKillPlayer");
	}

	public IEnumerator SentRPCNotifyPlayerDeath(NetworkView view, int killer_id, int deader_id)
	{
		yield return 1;
		view.RPC("NotifyPlayerDeath", RPCMode.All, killer_id, deader_id);
		Debug.Log("sent NotifyPlayerDeath");
	}

	public void OnDeadCameraChange(Player mPlayer)
	{
		StartCoroutine(CameraChangeTarget(mPlayer));
	}

	public IEnumerator CameraChangeTarget(Player mPlayer)
	{
		yield return new WaitForSeconds(2f);
		if (mPlayer != null)
		{
			GameApp.GetInstance().GetGameScene().GamePlayingState = PlayingState.GameVSLoser;
			GameApp.GetInstance().GetGameScene().GetCamera()
				.player = mPlayer;
		}
	}
}
