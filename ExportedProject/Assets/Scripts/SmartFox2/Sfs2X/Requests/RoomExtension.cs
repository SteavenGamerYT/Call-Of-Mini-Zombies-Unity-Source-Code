namespace Sfs2X.Requests
{
	public class RoomExtension
	{
		private string id;

		private string className;

		private string propertiesFile;

		public string Id
		{
			get
			{
				return id;
			}
		}

		public string ClassName
		{
			get
			{
				return className;
			}
		}

		public string PropertiesFile
		{
			get
			{
				return propertiesFile;
			}
		}

		public RoomExtension(string id, string className)
		{
			this.id = id;
			this.className = className;
			propertiesFile = "";
		}
	}
}
