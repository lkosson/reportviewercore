using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RPLProcessing;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class Chart : DynamicImage
	{
		protected override PaginationInfoItems PaginationInfoEnum => PaginationInfoItems.Chart;

		protected override bool SpecialBorderHandling => ((Microsoft.ReportingServices.OnDemandReportRendering.Chart)m_source).SpecialBorderHandling;

		protected override PageBreak PageBreak => ((Microsoft.ReportingServices.OnDemandReportRendering.Chart)m_source).PageBreak;

		protected override string PageName => ((ChartInstance)m_source.Instance).PageName;

		internal override double SourceWidthInMM => ((ChartInstance)m_source.Instance).DynamicWidth.ToMillimeters();

		internal Chart(Microsoft.ReportingServices.OnDemandReportRendering.Chart source, PageContext pageContext, bool createForRepeat)
			: base(source, pageContext, createForRepeat)
		{
			ChartInstance chartInstance = (ChartInstance)m_source.Instance;
			m_itemPageSizes.AdjustHeightTo(chartInstance.DynamicHeight.ToMillimeters());
			m_itemPageSizes.AdjustWidthTo(chartInstance.DynamicWidth.ToMillimeters());
		}

		protected override byte GetElementToken(PageContext pageContext)
		{
			return 11;
		}

		protected override string GenerateStreamName(PageContext pageContext)
		{
			return pageContext.GenerateStreamName((ChartInstance)m_source.Instance);
		}

		protected override RPLItem CreateRPLItem()
		{
			return new RPLChart();
		}
	}
}
