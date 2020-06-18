using System;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class CallbackEventArgs : EventArgs
	{
		private string commandName;

		private string commandArgument;

		public string CommandName => commandName;

		public string CommandArgument => commandArgument;

		public CallbackEventArgs(string commandName, string commandArgument)
		{
			this.commandName = commandName;
			this.commandArgument = commandArgument;
		}
	}
}
