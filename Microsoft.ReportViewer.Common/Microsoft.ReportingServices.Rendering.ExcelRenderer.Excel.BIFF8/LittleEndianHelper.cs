using System;
using System.Globalization;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8
{
	internal sealed class LittleEndianHelper
	{
		internal const int BYTE_SIZE = 1;

		internal const int SHORT_SIZE = 2;

		internal const int CHAR_SIZE = 2;

		internal const int INT_SIZE = 4;

		internal const int LONG_SIZE = 8;

		internal const int FLOAT_SIZE = 4;

		internal const int DOUBLE_SIZE = 8;

		internal static readonly short UBYTE_MAX = 511;

		internal static readonly int USHORT_MAX = 65535;

		internal static readonly long UINT_MAX = 4294967295L;

		internal static short ReadShort(byte[] aBuf, int aOff)
		{
			return (short)(((aBuf[aOff + 1] & 0xFF) << 8) | (aBuf[aOff] & 0xFF));
		}

		internal static char readChar(byte[] aBuf, int aOff)
		{
			return (char)ReadShort(aBuf, aOff);
		}

		internal static int ReadInt(byte[] aBuf, int aOff)
		{
			int num = 0;
			for (int i = 0; i < 32; i += 8)
			{
				num |= (int)((long)(aBuf[aOff + i / 8] & 0xFF) << i);
			}
			return num;
		}

		internal static long ReadLong(byte[] aBuf, int aOff)
		{
			long num = 0L;
			for (int i = 0; i < 64; i += 8)
			{
				num |= (long)(aBuf[aOff + i / 8] & 0xFF) << i;
			}
			return num;
		}

		internal static float ReadFloat(byte[] aBuf, int aOff)
		{
			return Convert.ToSingle(ReadInt(aBuf, aOff));
		}

		internal static double ReadDouble(byte[] aBuf, int aOff)
		{
			return Convert.ToDouble(ReadLong(aBuf, aOff));
		}

		internal static short ReadByteU(byte[] aBuf, int aOff)
		{
			return (short)(aBuf[aOff] & 0xFF);
		}

		internal static int ReadShortU(byte[] aBuf, int aOff)
		{
			return ReadShort(aBuf, aOff) & 0xFFFF;
		}

		internal static long ReadIntU(byte[] aBuf, int aOff)
		{
			return (long)ReadInt(aBuf, aOff) & -1L;
		}

		internal static double ReadFixed32(byte[] aBuff, int aOff)
		{
			int num = ReadInt(aBuff, aOff);
			bool flag = false;
			if (num < 0)
			{
				num *= -1;
				flag = true;
			}
			double num2 = URShift(num, 16);
			num &= 0xFFFF;
			num2 += (double)num / 65536.0;
			if (flag)
			{
				num2 *= -1.0;
			}
			return num2;
		}

		internal static double ReadFixed32U(byte[] aBuff, int aOff)
		{
			long num = ReadIntU(aBuff, aOff);
			double num2 = URShift(num, 16);
			num &= 0xFFFF;
			return num2 + (double)num / 65536.0;
		}

		internal static void WriteShort(short aVal, byte[] aBuf, int aOff)
		{
			aBuf[aOff + 1] = (byte)(aVal >> 8);
			aBuf[aOff] = (byte)(aVal & 0xFF);
		}

		internal static void WriteShort(ushort aVal, byte[] aBuf, int aOff)
		{
			aBuf[aOff + 1] = (byte)(aVal >> 8);
			aBuf[aOff] = (byte)(aVal & 0xFF);
		}

		internal static void WriteInt(int aVal, byte[] aBuf, int aOff)
		{
			for (int i = 0; i < 32; i += 8)
			{
				aBuf[aOff + i / 8] = (byte)((aVal >> i) & 0xFF);
			}
		}

		internal static void WriteLong(long aVal, byte[] aBuf, int aOff)
		{
			for (int i = 0; i < 64; i += 8)
			{
				aBuf[aOff + i / 8] = (byte)((aVal >> i) & 0xFF);
			}
		}

		internal static void WriteFloat(float aVal, byte[] aBuf, int aOff)
		{
			WriteInt(Convert.ToInt32(aVal), aBuf, aOff);
		}

		internal static void WriteDouble(double aVal, byte[] aBuf, int aOff)
		{
			WriteLong(Convert.ToInt64(aVal), aBuf, aOff);
		}

		internal static void writeByteU(short aVal, byte[] aBuf, int aOff)
		{
			if (aVal > UBYTE_MAX)
			{
				throw new IOException(ExcelRenderRes.MaxValueExceeded(UBYTE_MAX.ToString(CultureInfo.InvariantCulture)));
			}
			aBuf[aOff] = (byte)(aVal & 0xFF);
		}

		internal static void WriteShortU(int aVal, byte[] aBuf, int aOff)
		{
			if (aVal > USHORT_MAX)
			{
				throw new IOException(ExcelRenderRes.MaxValueExceeded(USHORT_MAX.ToString(CultureInfo.InvariantCulture)));
			}
			WriteShort((short)(aVal & 0xFFFF), aBuf, aOff);
		}

		internal static void WriteIntU(long aVal, byte[] aBuf, int aOff)
		{
			if (aVal > UINT_MAX)
			{
				throw new IOException(ExcelRenderRes.MaxValueExceeded(UINT_MAX.ToString(CultureInfo.InvariantCulture)));
			}
			WriteInt((int)(aVal & -1), aBuf, aOff);
		}

		internal static void WriteFixed32(double aVal, byte[] aBuff, int aOff)
		{
			bool flag = false;
			if (aVal < 0.0)
			{
				aVal *= -1.0;
				flag = true;
			}
			double num = Math.Floor(aVal);
			double num2 = aVal - num;
			num2 *= 65536.0;
			int num3 = (int)num << 16;
			num3 += (int)num2;
			if (flag)
			{
				num3 *= -1;
			}
			WriteInt(num3, aBuff, aOff);
		}

		internal static void WriteFixed32U(double aVal, byte[] aBuff, int aOff)
		{
			double num = Math.Floor(aVal);
			double num2 = aVal - num;
			num2 *= 65536.0;
			WriteIntU(((long)num << 16) + (long)num2, aBuff, aOff);
		}

		internal static void WriteShortU(Stream aOut, int aVal)
		{
			for (int i = 0; i < 2; i++)
			{
				aOut.WriteByte((byte)((aVal >> 8 * i) & 0xFF));
			}
		}

		internal static void WriteIntU(Stream aOut, long aVal)
		{
			for (int i = 0; i < 4; i++)
			{
				aOut.WriteByte((byte)((aVal >> 8 * i) & 0xFF));
			}
		}

		internal static int URShift(int number, int bits)
		{
			if (number >= 0)
			{
				return number >> bits;
			}
			return (number >> bits) + (2 << ~bits);
		}

		internal static int URShift(int number, long bits)
		{
			return URShift(number, (int)bits);
		}

		internal static long URShift(long number, int bits)
		{
			if (number >= 0)
			{
				return number >> bits;
			}
			return (number >> bits) + (2L << ~bits);
		}

		internal static long URShift(long number, long bits)
		{
			return URShift(number, (int)bits);
		}
	}
}
