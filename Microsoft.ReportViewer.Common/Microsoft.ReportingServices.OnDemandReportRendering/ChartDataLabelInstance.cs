using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartDataLabelInstance : BaseInstance
	{
		private ChartDataLabel m_chartDataLabelDef;

		private StyleInstance m_style;

		private string m_formattedValue;

		private Microsoft.ReportingServices.RdlExpressions.VariantResult? m_originalValue;

		private bool? m_useValueAsLabel;

		private ChartDataLabelPositions? m_position;

		private int? m_rotation;

		private bool? m_visible;

		private string m_toolTip;

		public StyleInstance Style
		{
			get
			{
				if (m_style == null)
				{
					m_style = new StyleInstance(m_chartDataLabelDef, m_chartDataLabelDef.ReportScope, m_chartDataLabelDef.ChartDef.RenderingContext);
				}
				return m_style;
			}
		}

		public object OriginalValue => GetOriginalValue().Value;

		public string Label
		{
			get
			{
				if (m_formattedValue == null)
				{
					if (m_chartDataLabelDef.ChartDef.IsOldSnapshot)
					{
						m_formattedValue = (GetOriginalValue().Value as string);
					}
					else
					{
						m_formattedValue = m_chartDataLabelDef.ChartDataLabelDef.GetFormattedValue(GetOriginalValue(), ReportScopeInstance, m_chartDataLabelDef.ChartDef.RenderingContext.OdpContext);
					}
				}
				return m_formattedValue;
			}
		}

		public bool UseValueAsLabel
		{
			get
			{
				if (!m_useValueAsLabel.HasValue && !m_chartDataLabelDef.ChartDef.IsOldSnapshot)
				{
					m_useValueAsLabel = m_chartDataLabelDef.ChartDataLabelDef.EvaluateUseValueAsLabel(ReportScopeInstance, m_chartDataLabelDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_useValueAsLabel.Value;
			}
		}

		public string ToolTip
		{
			get
			{
				if (m_toolTip == null)
				{
					m_toolTip = m_chartDataLabelDef.ChartDataLabelDef.EvaluateToolTip(ReportScopeInstance, m_chartDataLabelDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_toolTip;
			}
		}

		public ChartDataLabelPositions Position
		{
			get
			{
				if (!m_position.HasValue && !m_chartDataLabelDef.ChartDef.IsOldSnapshot)
				{
					m_position = m_chartDataLabelDef.ChartDataLabelDef.EvaluatePosition(ReportScopeInstance, m_chartDataLabelDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_position.Value;
			}
		}

		public int Rotation
		{
			get
			{
				if (!m_rotation.HasValue && !m_chartDataLabelDef.ChartDef.IsOldSnapshot)
				{
					m_rotation = m_chartDataLabelDef.ChartDataLabelDef.EvaluateRotation(ReportScopeInstance, m_chartDataLabelDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_rotation.Value;
			}
		}

		public bool Visible
		{
			get
			{
				if (!m_visible.HasValue && !m_chartDataLabelDef.ChartDef.IsOldSnapshot)
				{
					m_visible = m_chartDataLabelDef.ChartDataLabelDef.EvaluateVisible(ReportScopeInstance, m_chartDataLabelDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_visible.Value;
			}
		}

		internal ChartDataLabelInstance(ChartDataLabel chartDataLabelDef)
			: base(chartDataLabelDef.ReportScope)
		{
			m_chartDataLabelDef = chartDataLabelDef;
		}

		private Microsoft.ReportingServices.RdlExpressions.VariantResult GetOriginalValue()
		{
			if (!m_originalValue.HasValue)
			{
				if (m_chartDataLabelDef.ChartDef.IsOldSnapshot)
				{
					Microsoft.ReportingServices.ReportProcessing.ChartDataLabel dataLabel = m_chartDataLabelDef.ChartDataPoint.RenderDataPointDef.DataLabel;
					if (dataLabel != null)
					{
						if (dataLabel.Value != null && !dataLabel.Value.IsExpression)
						{
							_ = dataLabel.Value.Value;
						}
						else
						{
							_ = m_chartDataLabelDef.ChartDataPoint.RenderItem.InstanceInfo.DataLabelValue;
						}
					}
					m_originalValue = new Microsoft.ReportingServices.RdlExpressions.VariantResult(errorOccurred: false, dataLabel);
				}
				else
				{
					m_originalValue = m_chartDataLabelDef.ChartDataLabelDef.EvaluateLabel(ReportScopeInstance, m_chartDataLabelDef.ChartDef.RenderingContext.OdpContext);
				}
			}
			return m_originalValue.Value;
		}

		protected override void ResetInstanceCache()
		{
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
			m_originalValue = null;
			m_formattedValue = null;
			m_useValueAsLabel = null;
			m_position = null;
			m_rotation = null;
			m_visible = null;
			m_toolTip = null;
		}
	}
}
