using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalChartDataPoint : ChartDataPoint, IROMActionOwner
	{
		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint m_dataPointDef;

		private ReportVariantProperty m_axisLabel;

		private ChartItemInLegend m_itemInLegend;

		private ReportStringProperty m_toolTip;

		public override string DataElementName => m_dataPointDef.DataElementName;

		public override DataElementOutputTypes DataElementOutput => m_dataPointDef.DataElementOutput;

		public override ChartDataPointValues DataPointValues
		{
			get
			{
				if (m_dataPointValues == null && m_dataPointDef.DataPointValues != null)
				{
					m_dataPointValues = new ChartDataPointValues(this, m_dataPointDef.DataPointValues, m_owner);
				}
				return m_dataPointValues;
			}
		}

		public override ChartItemInLegend ItemInLegend
		{
			get
			{
				if (m_dataPointDef.ItemInLegend != null)
				{
					m_itemInLegend = new ChartItemInLegend(this, m_dataPointDef.ItemInLegend, m_owner);
				}
				return m_itemInLegend;
			}
		}

		public string UniqueName => m_dataPointDef.UniqueName;

		public override ActionInfo ActionInfo
		{
			get
			{
				if (m_actionInfo == null && m_dataPointDef.Action != null)
				{
					m_actionInfo = new ActionInfo(m_owner.RenderingContext, this, m_dataPointDef.Action, m_dataPointDef, m_owner, ObjectType.Chart, m_dataPointDef.Name, this);
				}
				return m_actionInfo;
			}
		}

		public List<string> FieldsUsedInValueExpression => null;

		public override CustomPropertyCollection CustomProperties
		{
			get
			{
				if (m_customProperties == null)
				{
					m_customPropertiesReady = true;
					m_customProperties = new CustomPropertyCollection(base.Instance, m_owner.RenderingContext, null, m_dataPointDef, ObjectType.Chart, m_owner.Name);
				}
				else if (!m_customPropertiesReady)
				{
					m_customPropertiesReady = true;
					m_customProperties.UpdateCustomProperties(base.Instance, m_dataPointDef, m_owner.RenderingContext.OdpContext, ObjectType.Chart, m_owner.Name);
				}
				return m_customProperties;
			}
		}

		public override Style Style
		{
			get
			{
				if (m_style == null && m_dataPointDef.StyleClass != null)
				{
					m_style = new Style(m_owner, this, m_dataPointDef, base.ChartDef.RenderingContext);
				}
				return m_style;
			}
		}

		public override ChartMarker Marker
		{
			get
			{
				if (m_marker == null && m_dataPointDef.Marker != null)
				{
					m_marker = new ChartMarker(this, m_dataPointDef.Marker, m_owner);
				}
				return m_marker;
			}
		}

		public override ChartDataLabel DataLabel
		{
			get
			{
				if (m_dataLabel == null && m_dataPointDef.DataLabel != null)
				{
					m_dataLabel = new ChartDataLabel(this, m_dataPointDef.DataLabel, m_owner);
				}
				return m_dataLabel;
			}
		}

		public override ReportVariantProperty AxisLabel
		{
			get
			{
				if (m_axisLabel == null && m_dataPointDef.AxisLabel != null)
				{
					m_axisLabel = new ReportVariantProperty(m_dataPointDef.AxisLabel);
				}
				return m_axisLabel;
			}
		}

		public override ReportStringProperty ToolTip
		{
			get
			{
				if (m_toolTip == null && m_dataPointDef.ToolTip != null)
				{
					m_toolTip = new ReportStringProperty(m_dataPointDef.ToolTip);
				}
				return m_toolTip;
			}
		}

		internal override Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint DataPointDef => m_dataPointDef;

		internal override Microsoft.ReportingServices.ReportRendering.ChartDataPoint RenderItem => null;

		internal override Microsoft.ReportingServices.ReportProcessing.ChartDataPoint RenderDataPointDef => null;

		internal override IRIFReportScope RIFReportScope => m_dataPointDef;

		internal InternalChartDataPoint(Chart owner, int rowIndex, int colIndex, Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPointDef)
			: base(owner, rowIndex, colIndex)
		{
			m_dataPointDef = dataPointDef;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (m_itemInLegend != null)
			{
				m_itemInLegend.SetNewContext();
			}
			if (m_dataPointDef != null)
			{
				m_dataPointDef.ClearStreamingScopeInstanceBinding();
			}
		}
	}
}
