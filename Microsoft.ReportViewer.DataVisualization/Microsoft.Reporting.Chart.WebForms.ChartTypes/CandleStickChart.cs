using System.Drawing;

namespace Microsoft.Reporting.Chart.WebForms.ChartTypes
{
	internal class CandleStickChart : StockChart
	{
		public override string Name => "Candlestick";

		public CandleStickChart()
			: base(StockOpenCloseMarkStyle.Candlestick)
		{
			forceCandleStick = true;
		}

		public override Image GetImage(ChartTypeRegistry registry)
		{
			return (Image)registry.ResourceManager.GetObject(Name + "ChartType");
		}
	}
}
