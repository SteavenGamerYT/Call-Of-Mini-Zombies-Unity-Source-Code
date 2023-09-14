using UnityEngine;

public class CGEnemyBirthPacket : BinaryPacket
{
	public static Packet MakePacket(uint wave, uint enemy_id, uint enemy_type, uint isElite, uint isGrave, uint isBoss, Vector3 position, uint target_id)
	{
		uint iBodylen = 40u;
		Packet packet = BinaryPacket.MakePacket(65542u, iBodylen, 1u);
		packet.PushUInt32(wave);
		packet.PushUInt32(enemy_id);
		packet.PushUInt32(enemy_type);
		packet.PushUInt32(isElite);
		packet.PushUInt32(isGrave);
		packet.PushUInt32(isBoss);
		int num = 0;
		num = (int)(position.x * 100f);
		packet.PushUInt32((uint)num);
		num = (int)(position.y * 100f);
		packet.PushUInt32((uint)num);
		num = (int)(position.z * 100f);
		packet.PushUInt32((uint)num);
		packet.PushUInt32(target_id);
		return packet;
	}
}
