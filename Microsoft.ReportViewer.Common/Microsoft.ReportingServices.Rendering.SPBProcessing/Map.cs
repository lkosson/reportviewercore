using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RPLProcessing;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class Map : DynamicImage
	{
		protected override PaginationInfoItems PaginationInfoEnum => PaginationInfoItems.Map;

		protected override bool SpecialBorderHandling => ((Microsoft.ReportingServices.OnDemandReportRendering.Map)m_source).SpecialBorderHandling;

		protected override PageBreak PageBreak => ((Microsoft.ReportingServices.OnDemandReportRendering.Map)m_source).PageBreak;

		protected override string PageName => ((MapInstance)m_source.Instance).PageName;

		internal Map(Microsoft.ReportingServices.OnDemandReportRendering.Map source, PageContext pageContext, bool createForRepeat)
			: base(source, pageContext, createForRepeat)
		{
		}

		protected override byte GetElementToken(PageContext pageContext)
		{
			if (pageContext.VersionPicker == RPLVersionEnum.RPL2008 || pageContext.VersionPicker == RPLVersionEnum.RPLAccess || pageContext.VersionPicker == RPLVersionEnum.RPL2008WithImageConsolidation)
			{
				return 11;
			}
			return 21;
		}

		protected override string GenerateStreamName(PageContext pageContext)
		{
			return pageContext.GenerateStreamName((MapInstance)m_source.Instance);
		}

		protected override RPLItem CreateRPLItem()
		{
			return new RPLMap();
		}
	}
}
