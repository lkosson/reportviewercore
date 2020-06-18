using System;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class ClickEventArgs : EventArgs
	{
		private int x;

		private int y;

		private MapControl mapControl;

		private string returnCommandName = string.Empty;

		private string returnCommandArgument = string.Empty;

		public int X => x;

		public int Y => y;

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

		public ClickEventArgs(int x, int y, MapControl mapControl)
		{
			this.x = x;
			this.y = y;
			this.mapControl = mapControl;
		}
	}
}
