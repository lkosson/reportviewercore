using System;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class ValueChangedEventArgs : EventArgs
	{
		private double value;

		private DateTime date;

		private bool playbackMode;

		private string senderName;

		public double Value => value;

		public DateTime Date => date;

		public ValueChangedEventArgs(double value, DateTime date, string senderName, bool playbackMode)
		{
			this.value = value;
			this.date = date;
			this.playbackMode = playbackMode;
			this.senderName = senderName;
		}
	}
}
