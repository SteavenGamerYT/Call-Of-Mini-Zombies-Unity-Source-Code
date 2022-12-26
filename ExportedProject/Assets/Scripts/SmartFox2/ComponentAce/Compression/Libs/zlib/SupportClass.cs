namespace ComponentAce.Compression.Libs.zlib
{
	public class SupportClass
	{
		public static long Identity(long literal)
		{
			return literal;
		}

		public static int URShift(int number, int bits)
		{
			if (number >= 0)
			{
				return number >> bits;
			}
			return (number >> bits) + (2 << ~bits);
		}

		public static long URShift(long number, int bits)
		{
			if (number >= 0)
			{
				return number >> bits;
			}
			return (number >> bits) + (2L << ~bits);
		}
	}
}
