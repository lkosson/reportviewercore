using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapColorScaleInstance : MapDockableSubItemInstance
	{
		private MapColorScale m_defObject;

		private ReportSize m_tickMarkLength;

		private ReportColor m_colorBarBorderColor;

		private int? m_labelInterval;

		private string m_labelFormat;

		private MapLabelPlacement? m_labelPlacement;

		private MapLabelBehavior? m_labelBehavior;

		private bool? m_hideEndLabels;

		private ReportColor m_rangeGapColor;

		private string m_noDataText;

		public ReportSize TickMarkLength
		{
			get
			{
				if (m_tickMarkLength == null)
				{
					m_tickMarkLength = new ReportSize(((Microsoft.ReportingServices.ReportIntermediateFormat.MapColorScale)m_defObject.MapDockableSubItemDef).EvaluateTickMarkLength(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return m_tickMarkLength;
			}
		}

		public ReportColor ColorBarBorderColor
		{
			get
			{
				if (m_colorBarBorderColor == null)
				{
					m_colorBarBorderColor = new ReportColor(((Microsoft.ReportingServices.ReportIntermediateFormat.MapColorScale)m_defObject.MapDockableSubItemDef).EvaluateColorBarBorderColor(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return m_colorBarBorderColor;
			}
		}

		public int LabelInterval
		{
			get
			{
				if (!m_labelInterval.HasValue)
				{
					m_labelInterval = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapColorScale)m_defObject.MapDockableSubItemDef).EvaluateLabelInterval(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_labelInterval.Value;
			}
		}

		public string LabelFormat
		{
			get
			{
				if (m_labelFormat == null)
				{
					m_labelFormat = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapColorScale)m_defObject.MapDockableSubItemDef).EvaluateLabelFormat(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_labelFormat;
			}
		}

		public MapLabelPlacement LabelPlacement
		{
			get
			{
				if (!m_labelPlacement.HasValue)
				{
					m_labelPlacement = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapColorScale)m_defObject.MapDockableSubItemDef).EvaluateLabelPlacement(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_labelPlacement.Value;
			}
		}

		public MapLabelBehavior LabelBehavior
		{
			get
			{
				if (!m_labelBehavior.HasValue)
				{
					m_labelBehavior = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapColorScale)m_defObject.MapDockableSubItemDef).EvaluateLabelBehavior(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_labelBehavior.Value;
			}
		}

		public bool HideEndLabels
		{
			get
			{
				if (!m_hideEndLabels.HasValue)
				{
					m_hideEndLabels = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapColorScale)m_defObject.MapDockableSubItemDef).EvaluateHideEndLabels(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_hideEndLabels.Value;
			}
		}

		public ReportColor RangeGapColor
		{
			get
			{
				if (m_rangeGapColor == null)
				{
					m_rangeGapColor = new ReportColor(((Microsoft.ReportingServices.ReportIntermediateFormat.MapColorScale)m_defObject.MapDockableSubItemDef).EvaluateRangeGapColor(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return m_rangeGapColor;
			}
		}

		public string NoDataText
		{
			get
			{
				if (m_noDataText == null)
				{
					m_noDataText = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapColorScale)m_defObject.MapDockableSubItemDef).EvaluateNoDataText(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_noDataText;
			}
		}

		internal MapColorScaleInstance(MapColorScale defObject)
			: base(defObject)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_tickMarkLength = null;
			m_colorBarBorderColor = null;
			m_labelInterval = null;
			m_labelFormat = null;
			m_labelPlacement = null;
			m_labelBehavior = null;
			m_hideEndLabels = null;
			m_rangeGapColor = null;
			m_noDataText = null;
		}
	}
}
