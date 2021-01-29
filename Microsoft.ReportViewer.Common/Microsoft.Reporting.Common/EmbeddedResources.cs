using System.IO;
using System.Reflection;

namespace Microsoft.Reporting.Common
{
	internal static class EmbeddedResources
	{
		public static byte[] Get(ResourceList list, string name, out string mimeType)
		{
			Stream stream = GetStream(list, name, out mimeType);
			if (stream == null)
			{
				return null;
			}
			using (stream)
			{
				int num = (int)stream.Length;
				byte[] array = new byte[num];
				stream.Read(array, 0, num);
				return array;
			}
		}

		public static Stream GetStream(ResourceList list, string name, out string mimeType)
		{
			if (list.TryGetResourceItem(name, out ResourceItem item))
			{
				mimeType = item.MimeType;
				return Assembly.GetExecutingAssembly().GetManifestResourceStream(item.EffectiveName);
			}
			mimeType = null;
			return null;
		}
	}
}
