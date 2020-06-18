namespace Microsoft.ReportingServices.Rendering.Utilities
{
	internal class LittleEndian
	{
		internal static void PutInt(byte[] data, int offset, int val)
		{
			PutNumber(data, offset, val, 4);
		}

		internal static void PutInt(byte[] data, int val)
		{
			PutInt(data, 0, val);
		}

		internal static void PutUShort(byte[] data, int offset, ushort val)
		{
			PutNumber(data, offset, val, 2);
		}

		internal static void PutUShort(byte[] data, ushort val)
		{
			PutUShort(data, 0, val);
		}

		internal static void PutShort(byte[] data, int offset, short val)
		{
			PutNumber(data, offset, val, 2);
		}

		internal static void PutShort(byte[] data, short val)
		{
			PutShort(data, 0, val);
		}

		internal static int getInt(byte[] data)
		{
			return getInt(data, 0);
		}

		internal static int getInt(byte[] data, int offset)
		{
			return (int)getNumber(data, offset, 4);
		}

		internal static ushort getUShort(byte[] data, int offset)
		{
			return (ushort)getNumber(data, offset, 2);
		}

		internal static short getShort(byte[] data, int offset)
		{
			return (short)getNumber(data, offset, 2);
		}

		private static long getNumber(byte[] data, int offset, int size)
		{
			long num = 0L;
			for (int num2 = offset + size - 1; num2 >= offset; num2--)
			{
				num <<= 8;
				num |= (uint)(0xFF & data[num2]);
			}
			return num;
		}

		private static void PutNumber(byte[] data, int offset, long val, int size)
		{
			int num = size + offset;
			long num2 = val;
			for (int i = offset; i < num; i++)
			{
				data[i] = (byte)(num2 & 0xFF);
				num2 >>= 8;
			}
		}
	}
}
