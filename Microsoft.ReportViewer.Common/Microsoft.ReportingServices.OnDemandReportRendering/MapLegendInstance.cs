using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapLegendInstance : MapDockableSubItemInstance
	{
		private MapLegend m_defObject;

		private MapLegendLayout? m_layout;

		private bool? m_autoFitTextDisabled;

		private ReportSize m_minFontSize;

		private bool? m_interlacedRows;

		private ReportColor m_interlacedRowsColor;

		private bool? m_equallySpacedItems;

		private int? m_textWrapThreshold;

		public MapLegendLayout Layout
		{
			get
			{
				if (!m_layout.HasValue)
				{
					m_layout = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapLegend)m_defObject.MapDockableSubItemDef).EvaluateLayout(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_layout.Value;
			}
		}

		public bool AutoFitTextDisabled
		{
			get
			{
				if (!m_autoFitTextDisabled.HasValue)
				{
					m_autoFitTextDisabled = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapLegend)m_defObject.MapDockableSubItemDef).EvaluateAutoFitTextDisabled(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_autoFitTextDisabled.Value;
			}
		}

		public ReportSize MinFontSize
		{
			get
			{
				if (m_minFontSize == null)
				{
					m_minFontSize = new ReportSize(((Microsoft.ReportingServices.ReportIntermediateFormat.MapLegend)m_defObject.MapDockableSubItemDef).EvaluateMinFontSize(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return m_minFontSize;
			}
		}

		public bool InterlacedRows
		{
			get
			{
				if (!m_interlacedRows.HasValue)
				{
					m_interlacedRows = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapLegend)m_defObject.MapDockableSubItemDef).EvaluateInterlacedRows(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_interlacedRows.Value;
			}
		}

		public ReportColor InterlacedRowsColor
		{
			get
			{
				if (m_interlacedRowsColor == null)
				{
					m_interlacedRowsColor = new ReportColor(((Microsoft.ReportingServices.ReportIntermediateFormat.MapLegend)m_defObject.MapDockableSubItemDef).EvaluateInterlacedRowsColor(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return m_interlacedRowsColor;
			}
		}

		public bool EquallySpacedItems
		{
			get
			{
				if (!m_equallySpacedItems.HasValue)
				{
					m_equallySpacedItems = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapLegend)m_defObject.MapDockableSubItemDef).EvaluateEquallySpacedItems(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_equallySpacedItems.Value;
			}
		}

		public int TextWrapThreshold
		{
			get
			{
				if (!m_textWrapThreshold.HasValue)
				{
					m_textWrapThreshold = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapLegend)m_defObject.MapDockableSubItemDef).EvaluateTextWrapThreshold(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_textWrapThreshold.Value;
			}
		}

		internal MapLegendInstance(MapLegend defObject)
			: base(defObject)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_layout = null;
			m_autoFitTextDisabled = null;
			m_minFontSize = null;
			m_interlacedRows = null;
			m_interlacedRowsColor = null;
			m_equallySpacedItems = null;
			m_textWrapThreshold = null;
		}
	}
}
