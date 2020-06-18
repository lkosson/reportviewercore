using System;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class DataBindEventArgs : EventArgs
	{
		public new static DataBindEventArgs Empty = new DataBindEventArgs();

		private DataBindingRuleBase dataBinding;

		public DataBindingRuleBase DataBinding => dataBinding;

		public DataBindEventArgs()
		{
		}

		public DataBindEventArgs(DataBindingRuleBase dataBanding)
		{
			dataBinding = dataBanding;
		}
	}
}
