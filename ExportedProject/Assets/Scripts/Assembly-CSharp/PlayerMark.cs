using UnityEngine;
using Zombie3D;

public class PlayerMark : MonoBehaviour
{
	public GameObject SceneTUI;

	public Player m_player;

	public int Mark_Player_ID = -1;

	protected float ori_x;

	protected float ori_y;

	protected float tar_x;

	protected float tar_y;

	protected string ori_frame = string.Empty;

	protected Vector3 tem_pos;

	protected Vector3 tem_rot;

	protected Quaternion q;

	protected Vector2 tem;

	private void Start()
	{
		ori_frame = GetComponent<TUIMeshSprite>().frameName;
		base.transform.localPosition = new Vector3(-5000f, 0f, base.transform.localPosition.z);
	}

	private void Update()
	{
		if (m_player == null || !SceneTUI.GetComponent<GameSceneTUI>().is_inited)
		{
			return;
		}
		if (m_player.m_multi_id == GameApp.GetInstance().GetGameScene().GetPlayer()
			.m_multi_id)
		{
			base.transform.localPosition = new Vector3(0f, 0f, base.transform.localPosition.z);
		}
		else
		{
			tem_pos = GameApp.GetInstance().GetGameScene().GetPlayer()
				.GetTransform()
				.InverseTransformPoint(m_player.GetTransform().position);
			tem = new Vector2(tem_pos.x, tem_pos.z);
			tem_rot = GameApp.GetInstance().GetGameScene().GetPlayer()
				.GetTransform()
				.InverseTransformDirection(m_player.GetTransform().forward);
			q = Quaternion.FromToRotation(Vector3.forward, tem_rot);
			if (tem.sqrMagnitude > 1444f)
			{
				tem_pos = tem_pos.normalized * 38f;
			}
			base.transform.localPosition = new Vector3(tem_pos.x, tem_pos.z, base.transform.localPosition.z);
			base.transform.localEulerAngles = new Vector3(0f, 0f, 0f - q.eulerAngles.y);
		}
		if (m_player.GetPlayerState() == m_player.DEAD_STATE)
		{
			base.gameObject.GetComponent<TUIMeshSprite>().frameName = "PlayerMarkDead";
			base.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		}
		else
		{
			base.gameObject.GetComponent<TUIMeshSprite>().frameName = ori_frame;
		}
	}

	public void SetPlayer(Player mPlayer)
	{
		if (mPlayer != null)
		{
			m_player = mPlayer;
			Mark_Player_ID = m_player.m_multi_id;
		}
	}

	public void RemoveMark()
	{
		m_player = null;
		base.transform.localPosition = new Vector3(-5000f, 0f, base.transform.localPosition.z);
	}
}
