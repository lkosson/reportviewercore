using System;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class RealTimeValue
	{
		private string inputValueName = "Default";

		private double value;

		private DateTime timestamp = DateTime.Now;

		public string InputValueName
		{
			get
			{
				return inputValueName;
			}
			set
			{
				inputValueName = value;
			}
		}

		public double Value
		{
			get
			{
				return value;
			}
			set
			{
				this.value = value;
			}
		}

		public DateTime Timestamp
		{
			get
			{
				return timestamp;
			}
			set
			{
				timestamp = value;
			}
		}

		public RealTimeValue()
		{
		}

		public RealTimeValue(string inputValueName, double value, DateTime timestamp)
		{
			this.inputValueName = inputValueName;
			this.value = value;
			this.timestamp = timestamp;
		}
	}
}
