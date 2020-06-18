using System;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.Utilities
{
	internal sealed class OfficeImageHasher
	{
		private int m_a = 1732584193;

		private int m_b = -271733879;

		private int m_c = -1732584194;

		private int m_d = 271733878;

		private int[] m_dd;

		private int m_numwords;

		internal byte[] Hash
		{
			get
			{
				byte[] array = new byte[16];
				Array.Copy(BitConverter.GetBytes(m_a), 0, array, 0, 4);
				Array.Copy(BitConverter.GetBytes(m_b), 0, array, 4, 4);
				Array.Copy(BitConverter.GetBytes(m_c), 0, array, 8, 4);
				Array.Copy(BitConverter.GetBytes(m_d), 0, array, 12, 4);
				return array;
			}
		}

		internal OfficeImageHasher(byte[] inputBuffer)
		{
			Mdinit(inputBuffer);
			Calc();
		}

		internal OfficeImageHasher(Stream inputStream)
		{
			Mdinit(inputStream);
			Calc();
		}

		internal void Mdinit(byte[] inputBuffer)
		{
			long num = inputBuffer.Length * 8;
			int num2 = inputBuffer.Length % 64;
			int num3 = (num2 >= 56) ? (64 - num2 + 64) : (64 - num2);
			int num4 = inputBuffer.Length + num3;
			byte[] array = new byte[num4];
			Array.Copy(inputBuffer, array, inputBuffer.Length);
			array[inputBuffer.Length] = 128;
			for (int i = 0; i < 8; i++)
			{
				array[num4 - 8 + i] = (byte)(num & 0xFF);
				num >>= 8;
			}
			m_numwords = num4 / 4;
			m_dd = new int[m_numwords];
			for (int i = 0; i < num4; i += 4)
			{
				m_dd[i / 4] = (array[i] & 0xFF) + ((array[i + 1] & 0xFF) << 8) + ((array[i + 2] & 0xFF) << 16) + ((array[i + 3] & 0xFF) << 24);
			}
		}

		internal void Mdinit(Stream inputStream)
		{
			long num = inputStream.Length * 8;
			int num2 = (int)(inputStream.Length % 64);
			int num3 = (num2 >= 56) ? (64 - num2 + 64) : (64 - num2);
			int num4 = (int)inputStream.Length + num3;
			byte[] array = new byte[num4];
			inputStream.Read(array, 0, (int)inputStream.Length);
			array[(int)inputStream.Length] = 128;
			for (int i = 0; i < 8; i++)
			{
				array[num4 - 8 + i] = (byte)(num & 0xFF);
				num >>= 8;
			}
			m_numwords = num4 / 4;
			m_dd = new int[m_numwords];
			for (int i = 0; i < num4; i += 4)
			{
				m_dd[i / 4] = (array[i] & 0xFF) + ((array[i + 1] & 0xFF) << 8) + ((array[i + 2] & 0xFF) << 16) + ((array[i + 3] & 0xFF) << 24);
			}
		}

		internal void Calc()
		{
			for (int i = 0; i < m_numwords / 16; i++)
			{
				int a = m_a;
				int b = m_b;
				int c = m_c;
				int d = m_d;
				Round1(i);
				Round2(i);
				Round3(i);
				m_a += a;
				m_b += b;
				m_c += c;
				m_d += d;
			}
		}

		internal static int F(int x, int y, int z)
		{
			return (x & y) | (~x & z);
		}

		internal static int G(int x, int y, int z)
		{
			return (x & y) | (x & z) | (y & z);
		}

		internal static int H(int x, int y, int z)
		{
			return x ^ y ^ z;
		}

		internal void Round1(int blk)
		{
			int num = 16 * blk;
			m_a = Rotintlft(m_a + F(m_b, m_c, m_d) + m_dd[num], 3);
			m_d = Rotintlft(m_d + F(m_a, m_b, m_c) + m_dd[1 + num], 7);
			m_c = Rotintlft(m_c + F(m_d, m_a, m_b) + m_dd[2 + num], 11);
			m_b = Rotintlft(m_b + F(m_c, m_d, m_a) + m_dd[3 + num], 19);
			m_a = Rotintlft(m_a + F(m_b, m_c, m_d) + m_dd[4 + num], 3);
			m_d = Rotintlft(m_d + F(m_a, m_b, m_c) + m_dd[5 + num], 7);
			m_c = Rotintlft(m_c + F(m_d, m_a, m_b) + m_dd[6 + num], 11);
			m_b = Rotintlft(m_b + F(m_c, m_d, m_a) + m_dd[7 + num], 19);
			m_a = Rotintlft(m_a + F(m_b, m_c, m_d) + m_dd[8 + num], 3);
			m_d = Rotintlft(m_d + F(m_a, m_b, m_c) + m_dd[9 + num], 7);
			m_c = Rotintlft(m_c + F(m_d, m_a, m_b) + m_dd[10 + num], 11);
			m_b = Rotintlft(m_b + F(m_c, m_d, m_a) + m_dd[11 + num], 19);
			m_a = Rotintlft(m_a + F(m_b, m_c, m_d) + m_dd[12 + num], 3);
			m_d = Rotintlft(m_d + F(m_a, m_b, m_c) + m_dd[13 + num], 7);
			m_c = Rotintlft(m_c + F(m_d, m_a, m_b) + m_dd[14 + num], 11);
			m_b = Rotintlft(m_b + F(m_c, m_d, m_a) + m_dd[15 + num], 19);
		}

		internal void Round2(int blk)
		{
			int num = 16 * blk;
			m_a = Rotintlft(m_a + G(m_b, m_c, m_d) + m_dd[num] + 1518500249, 3);
			m_d = Rotintlft(m_d + G(m_a, m_b, m_c) + m_dd[4 + num] + 1518500249, 5);
			m_c = Rotintlft(m_c + G(m_d, m_a, m_b) + m_dd[8 + num] + 1518500249, 9);
			m_b = Rotintlft(m_b + G(m_c, m_d, m_a) + m_dd[12 + num] + 1518500249, 13);
			m_a = Rotintlft(m_a + G(m_b, m_c, m_d) + m_dd[1 + num] + 1518500249, 3);
			m_d = Rotintlft(m_d + G(m_a, m_b, m_c) + m_dd[5 + num] + 1518500249, 5);
			m_c = Rotintlft(m_c + G(m_d, m_a, m_b) + m_dd[9 + num] + 1518500249, 9);
			m_b = Rotintlft(m_b + G(m_c, m_d, m_a) + m_dd[13 + num] + 1518500249, 13);
			m_a = Rotintlft(m_a + G(m_b, m_c, m_d) + m_dd[2 + num] + 1518500249, 3);
			m_d = Rotintlft(m_d + G(m_a, m_b, m_c) + m_dd[6 + num] + 1518500249, 5);
			m_c = Rotintlft(m_c + G(m_d, m_a, m_b) + m_dd[10 + num] + 1518500249, 9);
			m_b = Rotintlft(m_b + G(m_c, m_d, m_a) + m_dd[14 + num] + 1518500249, 13);
			m_a = Rotintlft(m_a + G(m_b, m_c, m_d) + m_dd[3 + num] + 1518500249, 3);
			m_d = Rotintlft(m_d + G(m_a, m_b, m_c) + m_dd[7 + num] + 1518500249, 5);
			m_c = Rotintlft(m_c + G(m_d, m_a, m_b) + m_dd[11 + num] + 1518500249, 9);
			m_b = Rotintlft(m_b + G(m_c, m_d, m_a) + m_dd[15 + num] + 1518500249, 13);
		}

		internal void Round3(int blk)
		{
			int num = 16 * blk;
			m_a = Rotintlft(m_a + H(m_b, m_c, m_d) + m_dd[num] + 1859775393, 3);
			m_d = Rotintlft(m_d + H(m_a, m_b, m_c) + m_dd[8 + num] + 1859775393, 9);
			m_c = Rotintlft(m_c + H(m_d, m_a, m_b) + m_dd[4 + num] + 1859775393, 11);
			m_b = Rotintlft(m_b + H(m_c, m_d, m_a) + m_dd[12 + num] + 1859775393, 15);
			m_a = Rotintlft(m_a + H(m_b, m_c, m_d) + m_dd[2 + num] + 1859775393, 3);
			m_d = Rotintlft(m_d + H(m_a, m_b, m_c) + m_dd[10 + num] + 1859775393, 9);
			m_c = Rotintlft(m_c + H(m_d, m_a, m_b) + m_dd[6 + num] + 1859775393, 11);
			m_b = Rotintlft(m_b + H(m_c, m_d, m_a) + m_dd[14 + num] + 1859775393, 15);
			m_a = Rotintlft(m_a + H(m_b, m_c, m_d) + m_dd[1 + num] + 1859775393, 3);
			m_d = Rotintlft(m_d + H(m_a, m_b, m_c) + m_dd[9 + num] + 1859775393, 9);
			m_c = Rotintlft(m_c + H(m_d, m_a, m_b) + m_dd[5 + num] + 1859775393, 11);
			m_b = Rotintlft(m_b + H(m_c, m_d, m_a) + m_dd[13 + num] + 1859775393, 15);
			m_a = Rotintlft(m_a + H(m_b, m_c, m_d) + m_dd[3 + num] + 1859775393, 3);
			m_d = Rotintlft(m_d + H(m_a, m_b, m_c) + m_dd[11 + num] + 1859775393, 9);
			m_c = Rotintlft(m_c + H(m_d, m_a, m_b) + m_dd[7 + num] + 1859775393, 11);
			m_b = Rotintlft(m_b + H(m_c, m_d, m_a) + m_dd[15 + num] + 1859775393, 15);
		}

		internal int[] Getregs()
		{
			return new int[4]
			{
				m_a,
				m_b,
				m_c,
				m_d
			};
		}

		internal static int Rotintlft(int val, int numbits)
		{
			return (val << numbits) | (int)((uint)val >> 32 - numbits);
		}

		public override string ToString()
		{
			return Tohex(m_a) + Tohex(m_b) + Tohex(m_c) + Tohex(m_d);
		}

		internal static string Tohex(int i)
		{
			string text = "";
			for (int j = 0; j < 4; j++)
			{
				text = text + Convert.ToString((i >> 4) & 0xF, 16) + Convert.ToString(i & 0xF, 16);
				i >>= 8;
			}
			return text;
		}
	}
}
