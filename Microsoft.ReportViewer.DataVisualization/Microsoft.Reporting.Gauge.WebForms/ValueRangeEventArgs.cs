using System;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class ValueRangeEventArgs : ValueChangedEventArgs
	{
		private NamedElement pointer;

		public ValueRangeEventArgs(double value, DateTime date, string senderName, bool playbackMode, NamedElement pointer)
			: base(value, date, senderName, playbackMode)
		{
			this.pointer = pointer;
		}
	}
}
