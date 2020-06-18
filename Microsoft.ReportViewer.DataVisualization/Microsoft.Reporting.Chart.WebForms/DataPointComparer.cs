using System;
using System.Collections;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeDataPointComparer_DataPointComparer")]
	internal class DataPointComparer : IComparer
	{
		private Series series;

		private PointsSortOrder sortingOrder;

		private int sortingValueIndex = 1;

		private DataPointComparer()
		{
		}

		public DataPointComparer(Series series, PointsSortOrder order, string sortBy)
		{
			sortBy = sortBy.ToUpper(CultureInfo.InvariantCulture);
			if (string.Compare(sortBy, "X", StringComparison.Ordinal) == 0)
			{
				sortingValueIndex = -1;
			}
			else if (string.Compare(sortBy, "Y", StringComparison.Ordinal) == 0)
			{
				sortingValueIndex = 0;
			}
			else if (string.Compare(sortBy, "AXISLABEL", StringComparison.Ordinal) == 0)
			{
				sortingValueIndex = -2;
			}
			else
			{
				if (sortBy.Length != 2 || !sortBy.StartsWith("Y", StringComparison.Ordinal) || !char.IsDigit(sortBy[1]))
				{
					throw new ArgumentException(SR.ExceptionDataPointConverterInvalidSorting, "sortBy");
				}
				sortingValueIndex = int.Parse(sortBy.Substring(1), CultureInfo.InvariantCulture) - 1;
			}
			if (sortingValueIndex > 0 && sortingValueIndex >= series.YValuesPerPoint)
			{
				throw new ArgumentException(SR.ExceptionDataPointConverterUnavailableSorting(sortBy, series.YValuesPerPoint.ToString(CultureInfo.InvariantCulture)), "sortBy");
			}
			sortingOrder = order;
			this.series = series;
		}

		public int Compare(object point1, object point2)
		{
			int num = -1;
			if (!(point1 is DataPoint) || !(point2 is DataPoint))
			{
				throw new ArgumentException(SR.ExceptionDataPointConverterWrongTypes);
			}
			num = ((sortingValueIndex == -1) ? ((DataPoint)point1).XValue.CompareTo(((DataPoint)point2).XValue) : ((sortingValueIndex != -2) ? ((DataPoint)point1).YValues[sortingValueIndex].CompareTo(((DataPoint)point2).YValues[sortingValueIndex]) : string.Compare(((DataPoint)point1).AxisLabel, ((DataPoint)point2).AxisLabel, StringComparison.CurrentCultureIgnoreCase)));
			if (sortingOrder == PointsSortOrder.Descending)
			{
				num = -num;
			}
			return num;
		}
	}
}
