using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.BIFF8.Records
{
	internal static class StringUtil
	{
		internal static void WriteBIFF8String(BinaryWriter aOut, string aString, bool compressed)
		{
			WriteBIFF8String(aOut, aString, writeLength: true, compressed);
		}

		internal static void WriteBIFF8String(BinaryWriter output, string str, bool writeLength, bool compressed)
		{
			string text = str;
			if (text.Length > 256)
			{
				text = text.Substring(0, 256);
			}
			if (writeLength)
			{
				output.Write((ushort)text.Length);
			}
			output.Write(!compressed);
			if (compressed)
			{
				DecodeTo1Byte(output, text);
			}
			else
			{
				DecodeTo2ByteLE(output, text);
			}
		}

		internal static int CalcBIFF8StringSize(string str, bool tryToCompress, out bool compressed)
		{
			string text = str;
			if (text.Length > 256)
			{
				text = text.Substring(0, 256);
			}
			if (!tryToCompress)
			{
				compressed = false;
				return text.Length * 2;
			}
			if (CanCompress(text))
			{
				compressed = true;
				return text.Length;
			}
			compressed = false;
			return text.Length * 2;
		}

		internal static int CalcBIFF8StringSize(string aString, out bool compressed)
		{
			return CalcBIFF8StringSize(aString, tryToCompress: true, out compressed);
		}

		internal static byte[] DecodeTo1Byte(string aStr)
		{
			byte[] array = new byte[aStr.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = (byte)(aStr[i] & 0xFF);
			}
			return array;
		}

		internal static void DecodeTo1Byte(BinaryWriter aOut, string aStr)
		{
			for (int i = 0; i < aStr.Length; i++)
			{
				aOut.Write((byte)(aStr[i] & 0xFF));
			}
		}

		internal static void DecodeTo1Byte(Stream aOut, string aStr, int aOffset, int aLength)
		{
			for (int i = 0; i < aLength; i++)
			{
				aOut.WriteByte((byte)(aStr[i + aOffset] & 0xFF));
			}
		}

		internal static byte[] DecodeTo2ByteLE(string aStr)
		{
			byte[] array = new byte[aStr.Length * 2];
			for (int i = 0; i < aStr.Length; i++)
			{
				char c = aStr[i];
				array[i * 2] = (byte)(c & 0xFF);
				array[i * 2 + 1] = (byte)(((int)c >> 8) & 0xFF);
			}
			return array;
		}

		internal static void DecodeTo2ByteLE(BinaryWriter aOut, string aStr)
		{
			foreach (char c in aStr)
			{
				aOut.Write((byte)(c & 0xFF));
				aOut.Write((byte)(((int)c >> 8) & 0xFF));
			}
		}

		internal static void DecodeTo2ByteLE(Stream aOut, string aStr, int aOffset, int aLength)
		{
			for (int i = 0; i < aLength; i++)
			{
				char c = aStr[i + aOffset];
				aOut.WriteByte((byte)(c & 0xFF));
				aOut.WriteByte((byte)(((int)c >> 8) & 0xFF));
			}
		}

		internal static bool CanCompress(string aStr)
		{
			for (int i = 0; i < aStr.Length; i++)
			{
				if ((aStr[i] & 0xFF00) > 0)
				{
					return false;
				}
			}
			return true;
		}

		internal static bool CanCompress(char[] aChars, int aOffset, int aLength)
		{
			for (int i = 0; i < aLength; i++)
			{
				if ((aChars[i + aOffset] & 0xFF00) > 0)
				{
					return false;
				}
			}
			return true;
		}

		internal static bool CanCompress(string aStr, int aOffset, int aLength)
		{
			for (int i = 0; i < aLength; i++)
			{
				if ((aStr[i + aOffset] & 0xFF00) > 0)
				{
					return false;
				}
			}
			return true;
		}
	}
}
