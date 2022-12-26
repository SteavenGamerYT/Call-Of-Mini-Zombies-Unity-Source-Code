using System.IO;

namespace ComponentAce.Compression.Libs.zlib
{
	public class ZStreamException : IOException
	{
		public ZStreamException(string s)
			: base(s)
		{
		}
	}
}
