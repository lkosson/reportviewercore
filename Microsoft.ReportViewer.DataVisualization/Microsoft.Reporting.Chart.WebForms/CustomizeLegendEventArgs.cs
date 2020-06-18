using System;

namespace Microsoft.Reporting.Chart.WebForms
{
	internal class CustomizeLegendEventArgs : EventArgs
	{
		private LegendItemsCollection legendItems;

		private string legendName = "";

		public string LegendName => legendName;

		public LegendItemsCollection LegendItems => legendItems;

		private CustomizeLegendEventArgs()
		{
		}

		public CustomizeLegendEventArgs(LegendItemsCollection legendItems)
		{
			this.legendItems = legendItems;
		}

		public CustomizeLegendEventArgs(LegendItemsCollection legendItems, string legendName)
		{
			this.legendItems = legendItems;
			this.legendName = legendName;
		}
	}
}
