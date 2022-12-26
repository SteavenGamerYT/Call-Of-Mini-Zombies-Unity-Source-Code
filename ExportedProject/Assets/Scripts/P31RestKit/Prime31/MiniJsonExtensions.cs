using System.Collections;

namespace Prime31
{
	public static class MiniJsonExtensions
	{
		public static Hashtable hashtableFromJson(this string json)
		{
			return MiniJSON.jsonDecode(json) as Hashtable;
		}
	}
}
