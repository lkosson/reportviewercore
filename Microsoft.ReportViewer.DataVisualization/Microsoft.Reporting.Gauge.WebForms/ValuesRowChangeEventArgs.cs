using System;
using System.Data;
using System.Diagnostics;

namespace Microsoft.Reporting.Gauge.WebForms
{
	[DebuggerStepThrough]
	internal class ValuesRowChangeEventArgs : EventArgs
	{
		private ValuesRow eventRow;

		private DataRowAction eventAction;

		public ValuesRow Row => eventRow;

		public DataRowAction Action => eventAction;

		public ValuesRowChangeEventArgs(ValuesRow row, DataRowAction action)
		{
			eventRow = row;
			eventAction = action;
		}
	}
}
