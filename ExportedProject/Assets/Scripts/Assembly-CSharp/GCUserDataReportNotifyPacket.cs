public class GCUserDataReportNotifyPacket : BinaryPacket
{
	public uint m_iUserId;

	public uint mKill_count;

	public uint mDeath_count;

	public uint mCash_loot;

	public long mDamage_val;

	public override bool ParserPacket(Packet packet)
	{
		if (!base.ParserPacket(packet))
		{
			return false;
		}
		if (!packet.PopUInt32(ref m_iUserId))
		{
			return false;
		}
		if (!packet.PopUInt32(ref mKill_count))
		{
			return false;
		}
		if (!packet.PopUInt32(ref mDeath_count))
		{
			return false;
		}
		if (!packet.PopUInt32(ref mCash_loot))
		{
			return false;
		}
		ulong val = 0uL;
		if (!packet.PopUInt64(ref val))
		{
			return false;
		}
		mDamage_val = (long)val;
		return true;
	}
}
