using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimChartDataPoint : ChartDataPoint
	{
		private Microsoft.ReportingServices.ReportRendering.ChartDataPoint m_renderDataPoint;

		private bool m_dataValueUpdateNeeded;

		private DataValueCollection m_dataValues;

		private ShimChartMember m_seriesParentMember;

		private ShimChartMember m_categoryParentMember;

		private Microsoft.ReportingServices.ReportProcessing.ChartDataPoint m_cachedDataPoint;

		public override string DataElementName => CachedRenderDataPoint.DataElementName;

		public override DataElementOutputTypes DataElementOutput
		{
			get
			{
				if (CachedRenderDataPoint.DataElementOutput == Microsoft.ReportingServices.ReportRendering.DataElementOutputTypes.Output)
				{
					return DataElementOutputTypes.ContentsOnly;
				}
				return DataElementOutputTypes.NoOutput;
			}
		}

		internal DataValueCollection DataValues
		{
			get
			{
				if (m_dataValues == null)
				{
					m_dataValues = new DataValueCollection(m_owner.RenderingContext, CachedRenderDataPoint);
				}
				else if (m_dataValueUpdateNeeded)
				{
					m_dataValueUpdateNeeded = false;
					m_dataValues.UpdateChartDataValues(CachedRenderDataPoint.DataValues);
				}
				return m_dataValues;
			}
		}

		public override ChartDataPointValues DataPointValues
		{
			get
			{
				if (m_dataPointValues == null)
				{
					m_dataPointValues = new ChartDataPointValues(this, base.ChartDef);
				}
				return m_dataPointValues;
			}
		}

		public override ChartItemInLegend ItemInLegend => null;

		public override ActionInfo ActionInfo
		{
			get
			{
				if (m_actionInfo == null && CachedRenderDataPoint.ActionInfo != null)
				{
					m_actionInfo = new ActionInfo(m_owner.RenderingContext, CachedRenderDataPoint.ActionInfo);
				}
				return m_actionInfo;
			}
		}

		public override CustomPropertyCollection CustomProperties
		{
			get
			{
				if (m_customProperties == null && CachedRenderDataPoint.CustomProperties != null && 0 < CachedRenderDataPoint.CustomProperties.Count)
				{
					m_customProperties = new CustomPropertyCollection(m_owner.RenderingContext, CachedRenderDataPoint.CustomProperties);
				}
				return m_customProperties;
			}
		}

		public override Style Style
		{
			get
			{
				if (m_style == null)
				{
					m_style = new Style(CachedDataPoint.StyleClass, CachedRenderDataPoint.InstanceInfo.StyleAttributeValues, base.ChartDef.RenderingContext);
				}
				return m_style;
			}
		}

		public override ChartMarker Marker
		{
			get
			{
				if (m_marker == null)
				{
					m_marker = new ChartMarker(this, base.ChartDef);
				}
				return m_marker;
			}
		}

		public override ChartDataLabel DataLabel
		{
			get
			{
				if (m_dataLabel == null)
				{
					m_dataLabel = new ChartDataLabel(this, base.ChartDef);
				}
				return m_dataLabel;
			}
		}

		public override ReportVariantProperty AxisLabel => null;

		public override ReportStringProperty ToolTip => null;

		internal override Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint DataPointDef => null;

		internal override Microsoft.ReportingServices.ReportRendering.ChartDataPoint RenderItem => CachedRenderDataPoint;

		internal override Microsoft.ReportingServices.ReportProcessing.ChartDataPoint RenderDataPointDef => CachedDataPoint;

		private Microsoft.ReportingServices.ReportRendering.ChartDataPoint CachedRenderDataPoint
		{
			get
			{
				if (m_renderDataPoint == null)
				{
					int cachedMemberDataPointIndex = m_seriesParentMember.CurrentRenderChartMember.CachedMemberDataPointIndex;
					int cachedMemberDataPointIndex2 = m_categoryParentMember.CurrentRenderChartMember.CachedMemberDataPointIndex;
					m_renderDataPoint = m_owner.RenderChart.DataPointCollection[cachedMemberDataPointIndex, cachedMemberDataPointIndex2];
					if (m_actionInfo != null)
					{
						m_actionInfo.Update(m_renderDataPoint.ActionInfo);
					}
					if (m_customProperties != null)
					{
						m_customProperties.UpdateCustomProperties(m_renderDataPoint.CustomProperties);
					}
				}
				return m_renderDataPoint;
			}
		}

		private Microsoft.ReportingServices.ReportProcessing.ChartDataPoint CachedDataPoint
		{
			get
			{
				if (m_cachedDataPoint == null)
				{
					int memberCellIndex = m_seriesParentMember.MemberCellIndex;
					int memberCellIndex2 = m_categoryParentMember.MemberCellIndex;
					m_cachedDataPoint = ((Microsoft.ReportingServices.ReportProcessing.Chart)m_owner.RenderChart.ReportItemDef).GetDataPoint(memberCellIndex, memberCellIndex2);
				}
				return m_cachedDataPoint;
			}
		}

		internal ShimChartDataPoint(Chart owner, int rowIndex, int colIndex, ShimChartMember seriesParentMember, ShimChartMember categoryParentMember)
			: base(owner, rowIndex, colIndex)
		{
			m_dataValues = null;
			m_seriesParentMember = seriesParentMember;
			m_categoryParentMember = categoryParentMember;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (m_dataValues != null)
			{
				m_dataValues.SetNewContext();
			}
			m_renderDataPoint = null;
			m_dataValueUpdateNeeded = true;
			m_cachedDataPoint = null;
		}
	}
}
