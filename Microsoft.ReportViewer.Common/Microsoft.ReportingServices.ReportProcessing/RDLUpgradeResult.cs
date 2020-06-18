namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class RDLUpgradeResult
	{
		private bool hasUnsupportedDundasChartFeatures;

		private bool hasUnsupportedDundasGaugeFeatures;

		public bool HasUnsupportedDundasCRIFeatures
		{
			get
			{
				if (!HasUnsupportedDundasChartFeatures)
				{
					return HasUnsupportedDundasGaugeFeatures;
				}
				return true;
			}
		}

		public bool HasUnsupportedDundasChartFeatures
		{
			get
			{
				return hasUnsupportedDundasChartFeatures;
			}
			internal set
			{
				hasUnsupportedDundasChartFeatures = value;
			}
		}

		public bool HasUnsupportedDundasGaugeFeatures
		{
			get
			{
				return hasUnsupportedDundasGaugeFeatures;
			}
			internal set
			{
				hasUnsupportedDundasGaugeFeatures = value;
			}
		}
	}
}
