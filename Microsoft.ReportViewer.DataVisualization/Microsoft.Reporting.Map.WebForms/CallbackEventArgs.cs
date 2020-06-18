using System;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class CallbackEventArgs : EventArgs
	{
		private string commandName;

		private string commandArgument;

		private MapControl mapControl;

		private string returnCommandName = string.Empty;

		private string returnCommandArgument = string.Empty;

		public string CommandName => commandName;

		public string CommandArgument => commandArgument;

		public MapControl MapControl => mapControl;

		public string ReturnCommandName
		{
			get
			{
				return returnCommandName;
			}
			set
			{
				returnCommandName = value;
			}
		}

		public string ReturnCommandArgument
		{
			get
			{
				return returnCommandArgument;
			}
			set
			{
				returnCommandArgument = value;
			}
		}

		public CallbackEventArgs(string commandName, string commandArgument, MapControl mapControl)
		{
			this.commandName = commandName;
			this.commandArgument = commandArgument;
			this.mapControl = mapControl;
		}
	}
}
