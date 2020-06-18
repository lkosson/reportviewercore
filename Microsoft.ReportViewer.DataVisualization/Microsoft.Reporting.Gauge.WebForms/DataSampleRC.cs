using System;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class DataSampleRC : HistoryEntry
	{
		internal bool Invalid = true;

		public override DateTime Timestamp
		{
			get
			{
				return base.Timestamp;
			}
			set
			{
				base.Timestamp = value;
				Invalid = false;
			}
		}

		public override double Value
		{
			get
			{
				return base.Value;
			}
			set
			{
				base.Value = value;
				Invalid = false;
			}
		}

		internal void Assign(DataSampleRC data)
		{
			Timestamp = data.Timestamp;
			Value = data.Value;
		}
	}
}
