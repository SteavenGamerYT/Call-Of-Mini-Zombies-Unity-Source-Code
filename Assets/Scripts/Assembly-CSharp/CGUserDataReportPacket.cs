public class CGUserDataReportPacket : BinaryPacket
{
	public static Packet MakePacket(uint user_id, uint kill_count, uint death_count, uint cash_loot, long damage_val)
	{
		uint iBodylen = 24u;
		Packet packet = BinaryPacket.MakePacket(65550u, iBodylen, 1u);
		packet.PushUInt32(user_id);
		packet.PushUInt32(kill_count);
		packet.PushUInt32(death_count);
		packet.PushUInt32(cash_loot);
		packet.PushUInt64((ulong)damage_val);
		return packet;
	}
}
