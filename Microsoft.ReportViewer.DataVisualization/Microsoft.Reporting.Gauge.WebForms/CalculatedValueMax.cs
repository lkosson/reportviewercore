using System;
using System.ComponentModel;

namespace Microsoft.Reporting.Gauge.WebForms
{
	[TypeConverter(typeof(CalculatedValueMaxConverter))]
	internal class CalculatedValueMax : CalculatedValue
	{
		internal override void CalculateValue(double value, DateTime timestamp)
		{
			GaugeDuration aggregateDuration = base.aggregateDuration;
			if (aggregateDuration.IsEmpty || ((IValueConsumer)this).GetProvider() == null)
			{
				base.CalculateValue(value, timestamp);
				return;
			}
			noMoreData = false;
			HistoryCollection data = ((IValueConsumer)this).GetProvider().GetData(aggregateDuration, timestamp);
			HistoryEntry[] array = data.Select(aggregateDuration, timestamp);
			double num = value;
			bool flag = false;
			HistoryEntry[] array2 = array;
			foreach (HistoryEntry historyEntry in array2)
			{
				if (!double.IsNaN(historyEntry.Value))
				{
					num = Math.Max(historyEntry.Value, num);
					flag = true;
				}
			}
			if (!flag)
			{
				num = ((data.Count <= 0) ? double.NaN : data.Top.Value);
				noMoreData = true;
			}
			base.CalculateValue(num, timestamp);
		}
	}
}
