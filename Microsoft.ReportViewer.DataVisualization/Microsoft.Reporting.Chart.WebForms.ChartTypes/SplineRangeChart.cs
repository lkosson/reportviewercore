using System.Drawing;

namespace Microsoft.Reporting.Chart.WebForms.ChartTypes
{
	internal class SplineRangeChart : RangeChart
	{
		public override string Name => "SplineRange";

		public SplineRangeChart()
		{
			lineTension = 0.5f;
		}

		public override Image GetImage(ChartTypeRegistry registry)
		{
			return (Image)registry.ResourceManager.GetObject(Name + "ChartType");
		}

		protected override float GetDefaultTension()
		{
			return 0.5f;
		}

		protected override bool IsLineTensionSupported()
		{
			return true;
		}
	}
}
