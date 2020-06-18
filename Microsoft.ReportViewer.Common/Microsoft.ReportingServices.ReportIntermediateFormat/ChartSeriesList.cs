using Microsoft.ReportingServices.ReportProcessing;
using System;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class ChartSeriesList : RowList
	{
		internal new ChartSeries this[int index] => (ChartSeries)base[index];

		public ChartSeriesList()
		{
		}

		internal ChartSeriesList(int capacity)
			: base(capacity)
		{
		}

		internal ChartSeries GetByName(string seriesName)
		{
			for (int i = 0; i < Count; i++)
			{
				ChartSeries chartSeries = this[i];
				if (Microsoft.ReportingServices.ReportProcessing.ReportProcessing.CompareWithInvariantCulture(seriesName, chartSeries.Name, ignoreCase: false) == 0)
				{
					return chartSeries;
				}
			}
			return null;
		}
	}
}
