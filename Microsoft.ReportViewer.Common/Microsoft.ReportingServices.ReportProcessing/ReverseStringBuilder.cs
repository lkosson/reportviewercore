using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal class ReverseStringBuilder
	{
		private char[] m_buffer;

		private int m_pos;

		public ReverseStringBuilder()
			: this(16)
		{
		}

		public ReverseStringBuilder(int capacity)
		{
			m_buffer = new char[capacity];
			m_pos = capacity - 1;
		}

		public void Append(string str)
		{
			int length = str.Length;
			EnsureCapacity(length);
			for (int i = 0; i < length; i++)
			{
				m_buffer[m_pos--] = str[i];
			}
		}

		public void Append(char c)
		{
			EnsureCapacity(1);
			m_buffer[m_pos--] = c;
		}

		private void EnsureCapacity(int lengthNeeded)
		{
			int num = m_buffer.Length;
			if (m_pos < 0 || num - m_pos < lengthNeeded)
			{
				int num2 = num - m_pos - 1;
				int num3 = Math.Max(lengthNeeded, num) * 2;
				int num4 = num3 - num2 - 1;
				char[] array = new char[num3];
				Array.Copy(m_buffer, m_pos + 1, array, num4 + 1, num2);
				m_buffer = array;
				m_pos = num4;
			}
		}

		public override string ToString()
		{
			return new string(m_buffer, m_pos + 1, m_buffer.Length - m_pos - 1);
		}
	}
}
