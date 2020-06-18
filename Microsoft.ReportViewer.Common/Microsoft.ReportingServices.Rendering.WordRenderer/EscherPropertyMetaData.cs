namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	internal class EscherPropertyMetaData
	{
		internal static byte TYPE_UNKNOWN = 0;

		internal static byte TYPE_BOOLEAN = 1;

		internal static byte TYPE_RGB = 2;

		internal static byte TYPE_SHAPEPATH = 3;

		internal static byte TYPE_SIMPLE = 4;

		internal static byte TYPE_ARRAY = 5;

		private string description;

		private byte type;

		internal virtual string Description => description;

		internal virtual byte Type => type;

		internal EscherPropertyMetaData(string description)
		{
			this.description = description;
		}

		internal EscherPropertyMetaData(string description, byte type)
		{
			this.description = description;
			this.type = type;
		}
	}
}
