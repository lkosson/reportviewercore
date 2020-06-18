using System.Drawing;

namespace Microsoft.Reporting.Chart.WebForms.ChartTypes
{
	internal class SplineChart : LineChart
	{
		public override string Name => "Spline";

		public SplineChart()
		{
			lineTension = 0.5f;
		}

		public override Image GetImage(ChartTypeRegistry registry)
		{
			return (Image)registry.ResourceManager.GetObject(Name + "ChartType");
		}

		protected override bool IsLineTensionSupported()
		{
			return true;
		}

		protected override PointF[] GetPointsPosition(ChartGraphics graph, Series series, bool indexedSeries)
		{
			lineTension = GetDefaultTension();
			if (IsLineTensionSupported() && series.IsAttributeSet("LineTension"))
			{
				lineTension = CommonElements.ParseFloat(series["LineTension"]);
			}
			return base.GetPointsPosition(graph, series, indexedSeries);
		}

		protected override float GetDefaultTension()
		{
			return 0.5f;
		}
	}
}
