using System;
using System.IO;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class BitWriter
	{
		private Stream baseStream;

		private long startOffset;

		private byte currentByte;

		private int currentBitIndex;

		public Stream BaseStream => baseStream;

		public long BytesWritten => baseStream.Position - startOffset;

		public BitWriter()
		{
			baseStream = new MemoryStream();
		}

		public BitWriter(Stream stream)
		{
			baseStream = stream;
			startOffset = stream.Position;
		}

		public void Flush()
		{
			if (currentBitIndex > 0)
			{
				baseStream.WriteByte(currentByte);
				currentBitIndex = 0;
				currentByte = 0;
			}
		}

		public void Close()
		{
			if (baseStream != null)
			{
				baseStream.Close();
			}
		}

		public void WriteBit(bool bitValue)
		{
			if (bitValue)
			{
				currentByte |= (byte)(1 << 7 - currentBitIndex);
			}
			currentBitIndex++;
			if (currentBitIndex > 7)
			{
				baseStream.WriteByte(currentByte);
				currentBitIndex = 0;
				currentByte = 0;
			}
		}

		public void WriteByte(byte byteValue)
		{
			if (currentBitIndex == 0)
			{
				baseStream.WriteByte(byteValue);
				return;
			}
			byte[] array = SplitAndShiftByte(byteValue, currentBitIndex);
			currentByte |= array[0];
			baseStream.WriteByte(currentByte);
			currentByte = array[1];
		}

		public void WriteBytes(byte[] byteValues)
		{
			foreach (byte byteValue in byteValues)
			{
				WriteByte(byteValue);
			}
		}

		public void WriteBits(byte[] byteValues, int bitNumber)
		{
			int num = bitNumber;
			while (byteValues.Length * 8 < num)
			{
				if (num - byteValues.Length * 8 > 8)
				{
					WriteByte(0);
					num -= 8;
				}
				else
				{
					WriteBit(bitValue: false);
					num--;
				}
			}
			int num2 = byteValues.Length - 1 - (int)Math.Ceiling((double)(num / 8));
			int num3;
			for (num3 = num; num3 > 8; num3 -= 8)
			{
			}
			if (num3 == 8)
			{
				num2++;
				WriteByte(byteValues[num2]);
			}
			else
			{
				int num4 = 128;
				for (int i = num3; i <= 7; i++)
				{
					num4 >>= 1;
				}
				for (int j = 8 - num3; j <= 7; j++)
				{
					WriteBit((byteValues[num2] & num4) == num4);
					num4 >>= 1;
				}
			}
			for (int k = num2 + 1; k < byteValues.Length; k++)
			{
				WriteByte(byteValues[k]);
			}
		}

		public void WriteSI32(int intValue)
		{
			Flush();
			byte[] bytes = BitConverter.GetBytes(intValue);
			if (!BitConverter.IsLittleEndian)
			{
				byte b = bytes[0];
				bytes[0] = bytes[3];
				bytes[3] = b;
				b = bytes[1];
				bytes[1] = bytes[2];
				bytes[2] = b;
			}
			baseStream.Write(bytes, 0, bytes.Length);
		}

		public void WriteSI16(short shortValue)
		{
			Flush();
			byte[] bytes = BitConverter.GetBytes(shortValue);
			if (!BitConverter.IsLittleEndian)
			{
				byte b = bytes[0];
				bytes[0] = bytes[1];
				bytes[1] = b;
			}
			baseStream.Write(bytes, 0, bytes.Length);
		}

		public void WriteBitValue(int intValue, int bitNumber, bool signed)
		{
			byte[] bytes = BitConverter.GetBytes(intValue);
			if (BitConverter.IsLittleEndian)
			{
				byte b = bytes[0];
				bytes[0] = bytes[3];
				bytes[3] = b;
				b = bytes[1];
				bytes[1] = bytes[2];
				bytes[2] = b;
			}
			if (signed && intValue < 0)
			{
				int num = bitNumber;
				int num2 = 3;
				while (num > 8)
				{
					num2--;
					num -= 8;
				}
				byte b2 = (byte)(128 >> 8 - num);
				bytes[num2] = (byte)(bytes[num2] | b2);
			}
			WriteBits(bytes, bitNumber);
		}

		private byte[] SplitAndShiftByte(byte byteValue, int splitIndex)
		{
			byte[] array = new byte[2];
			ushort num = byteValue;
			num = (ushort)(num << 8 - splitIndex);
			array[0] = (byte)((num & 0xFF00) >> 8);
			array[1] = (byte)(num & 0xFF);
			return array;
		}
	}
}
