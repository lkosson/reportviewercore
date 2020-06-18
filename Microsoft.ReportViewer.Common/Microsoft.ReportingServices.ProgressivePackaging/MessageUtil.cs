using System.IO;
using System.Text;

namespace Microsoft.ReportingServices.ProgressivePackaging
{
	internal static class MessageUtil
	{
		internal static readonly Encoding StringEncoding;

		static MessageUtil()
		{
			StringEncoding = Encoding.UTF8;
		}

		internal static string[] ReadStringArray(BinaryReader reader)
		{
			string[] array = new string[reader.ReadInt32()];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = reader.ReadString();
			}
			return array;
		}

		internal static void WriteStringArray(BinaryWriter writer, string[] value)
		{
			writer.Write(value.Length);
			for (int i = 0; i < value.Length; i++)
			{
				writer.Write(value[i]);
			}
		}

		internal static void WriteByteArray(BinaryWriter writer, byte[] value, int offset, int length)
		{
			writer.Write(length);
			writer.Write(value, offset, length);
		}
	}
}
