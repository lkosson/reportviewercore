namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	internal class BitField
	{
		private int m_mask;

		private int m_shiftCount;

		internal BitField(int mask)
		{
			m_mask = mask;
			int num = 0;
			int num2 = mask;
			if (num2 != 0)
			{
				while ((num2 & 1) == 0)
				{
					num++;
					num2 >>= 1;
				}
			}
			m_shiftCount = num;
		}

		internal virtual int GetValue(int holder)
		{
			return GetRawValue(holder) >> m_shiftCount;
		}

		internal virtual short GetShortValue(short holder)
		{
			return (short)GetValue(holder);
		}

		internal virtual int GetRawValue(int holder)
		{
			return holder & m_mask;
		}

		internal virtual short GetShortRawValue(short holder)
		{
			return (short)GetRawValue(holder);
		}

		internal virtual bool IsSet(int holder)
		{
			return (holder & m_mask) != 0;
		}

		internal virtual bool IsAllSet(int holder)
		{
			return (holder & m_mask) == m_mask;
		}

		internal virtual int SetValue(int holder, int value_Renamed)
		{
			return (holder & ~m_mask) | ((value_Renamed << m_shiftCount) & m_mask);
		}

		internal virtual short SetShortValue(short holder, short value_Renamed)
		{
			return (short)SetValue(holder, value_Renamed);
		}

		internal virtual int Clear(int holder)
		{
			return holder & ~m_mask;
		}

		internal virtual short ClearShort(short holder)
		{
			return (short)Clear(holder);
		}

		internal virtual byte ClearByte(byte holder)
		{
			return (byte)Clear(holder);
		}

		internal virtual int Mark(int holder)
		{
			return holder | m_mask;
		}

		internal virtual short SetShort(short holder)
		{
			return (short)Mark(holder);
		}

		internal virtual byte SetByte(byte holder)
		{
			return (byte)Mark(holder);
		}

		internal virtual int SetBoolean(int holder, bool flag)
		{
			if (!flag)
			{
				return Clear(holder);
			}
			return Mark(holder);
		}

		internal virtual short SetShortBoolean(short holder, bool flag)
		{
			if (!flag)
			{
				return ClearShort(holder);
			}
			return SetShort(holder);
		}

		internal virtual byte SetByteBoolean(byte holder, bool flag)
		{
			if (!flag)
			{
				return ClearByte(holder);
			}
			return SetByte(holder);
		}
	}
}
