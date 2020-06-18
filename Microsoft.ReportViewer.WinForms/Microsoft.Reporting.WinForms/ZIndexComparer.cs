using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System.Collections.Generic;

namespace Microsoft.Reporting.WinForms
{
	internal sealed class ZIndexComparer : IComparer<RPLItemMeasurement>, IComparer<RenderingItemBorderTablix>
	{
		public int Compare(RPLItemMeasurement x, RPLItemMeasurement y)
		{
			return Compare(x.ZIndex, y.ZIndex);
		}

		public int Compare(RenderingItemBorderTablix x, RenderingItemBorderTablix y)
		{
			int num;
			if (x.CompareRowFirst || y.CompareRowFirst)
			{
				num = Compare(x.RowZIndex, y.RowZIndex);
				if (num != 0)
				{
					return num;
				}
				num = Compare(x.ColumnZIndex, y.ColumnZIndex);
				if (num != 0)
				{
					return num;
				}
			}
			else
			{
				num = Compare(x.ColumnZIndex, y.ColumnZIndex);
				if (num != 0)
				{
					return num;
				}
				num = Compare(x.RowZIndex, y.RowZIndex);
				if (num != 0)
				{
					return num;
				}
			}
			num = Compare(x.RowIndex, y.RowIndex);
			if (num != 0)
			{
				return num;
			}
			num = Compare(x.ColumnIndex, y.ColumnIndex);
			if (num != 0)
			{
				return num;
			}
			return 0;
		}

		public int Compare(int x, int y)
		{
			if (x == y)
			{
				return 0;
			}
			if (x < y)
			{
				return -1;
			}
			return 1;
		}
	}
}
