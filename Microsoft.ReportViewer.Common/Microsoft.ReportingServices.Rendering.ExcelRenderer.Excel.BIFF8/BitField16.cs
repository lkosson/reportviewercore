namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8
{
	internal static class BitField16
	{
		internal static short GetValue(short aValue, short aMask)
		{
			short num = (short)(aValue & aMask);
			while ((aMask & 1) != 1)
			{
				num = (short)(num >> 1);
				aMask = (short)(aMask >> 1);
			}
			return num;
		}

		internal static short PutValue(short aDst, short aMask, short aValue)
		{
			short num = aMask;
			while ((num & 1) != 1)
			{
				aValue = (short)(aValue << 1);
				num = (short)(num >> 1);
			}
			aDst = (short)(aDst & (short)(~aMask));
			aDst = (short)(aDst | aValue);
			return aDst;
		}

		internal static ushort GetValue(ushort aValue, ushort aMask)
		{
			aMask = (ushort)(aMask & 0xFFFF);
			ushort num = (ushort)(aValue & aMask);
			while ((aMask & 1) != 1)
			{
				num = (ushort)(num >> 1);
				aMask = (ushort)(aMask >> 1);
			}
			return num;
		}

		internal static ushort PutValue(ushort aDst, ushort aMask, ushort aValue)
		{
			aMask = (ushort)(aMask & 0xFFFF);
			ushort num = aMask;
			while ((num & 1) != 1)
			{
				aValue = (ushort)(aValue << 1);
				num = (ushort)(num >> 1);
			}
			aDst = (ushort)(aDst & (ushort)(~aMask));
			aDst = (ushort)(aDst | aValue);
			return aDst;
		}
	}
}
