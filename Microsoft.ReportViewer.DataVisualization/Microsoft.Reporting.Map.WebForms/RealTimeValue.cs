using System;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class RealTimeValue
	{
		private string inputValueName = "Default";

		private double value;

		private DateTime timeStamp = DateTime.Now;

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

		public DateTime TimeStamp
		{
			get
			{
				return timeStamp;
			}
			set
			{
				timeStamp = value;
			}
		}

		public RealTimeValue()
		{
		}

		public RealTimeValue(string inputValueName, double value, DateTime timeStamp)
		{
			this.inputValueName = inputValueName;
			this.value = value;
			this.timeStamp = timeStamp;
		}
	}
}
