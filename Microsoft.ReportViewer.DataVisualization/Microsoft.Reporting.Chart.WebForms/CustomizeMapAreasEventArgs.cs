using System;

namespace Microsoft.Reporting.Chart.WebForms
{
	internal class CustomizeMapAreasEventArgs : EventArgs
	{
		private MapAreasCollection areaItems;

		public MapAreasCollection MapAreaItems => areaItems;

		private CustomizeMapAreasEventArgs()
		{
		}

		public CustomizeMapAreasEventArgs(MapAreasCollection areaItems)
		{
			this.areaItems = areaItems;
		}
	}
}
