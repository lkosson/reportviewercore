using System;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class PointerPositionChangeEventArgs : ValueChangedEventArgs
	{
		private bool accept = true;

		public bool Accept => accept;

		public PointerPositionChangeEventArgs(double value, DateTime date, string senderName, bool playbackMode)
			: base(value, date, senderName, playbackMode)
		{
		}
	}
}
