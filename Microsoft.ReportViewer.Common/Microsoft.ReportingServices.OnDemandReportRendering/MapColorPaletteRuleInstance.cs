using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapColorPaletteRuleInstance : MapColorRuleInstance
	{
		private MapColorPaletteRule m_defObject;

		private MapPalette? m_palette;

		public MapPalette Palette
		{
			get
			{
				if (!m_palette.HasValue)
				{
					m_palette = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapColorPaletteRule)m_defObject.MapColorRuleDef).EvaluatePalette(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_palette.Value;
			}
		}

		internal MapColorPaletteRuleInstance(MapColorPaletteRule defObject)
			: base(defObject)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_palette = null;
		}
	}
}
