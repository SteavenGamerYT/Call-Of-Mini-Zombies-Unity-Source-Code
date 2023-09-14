using System.Text;

public class CGCreateRoomPacket : BinaryPacket
{
	public static Packet MakePacket(uint iMapId, long lLocalTime, string strNickname, uint avatarType, uint level)
	{
		byte[] bytes = Encoding.ASCII.GetBytes(strNickname);
		uint iBodylen = (uint)(8 + bytes.Length + 8 + 4 + 4);
		Packet packet = BinaryPacket.MakePacket(3u, iBodylen, 1u);
		packet.PushUInt32(iMapId);
		packet.PushUInt64((ulong)lLocalTime);
		packet.PushUInt32((uint)bytes.Length);
		packet.PushByteArray(bytes, bytes.Length);
		packet.PushUInt32(avatarType);
		packet.PushUInt32(level);
		return packet;
	}
}
