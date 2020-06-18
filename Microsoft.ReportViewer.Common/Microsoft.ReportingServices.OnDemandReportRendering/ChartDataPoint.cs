using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class ChartDataPoint : IReportScope, IDataRegionCell, IROMStyleDefinitionContainer
	{
		protected Chart m_owner;

		protected int m_rowIndex;

		protected int m_columnIndex;

		protected ChartDataPointValues m_dataPointValues;

		protected ActionInfo m_actionInfo;

		protected CustomPropertyCollection m_customProperties;

		protected bool m_customPropertiesReady;

		protected ChartDataPointInstance m_instance;

		protected Style m_style;

		protected ChartMarker m_marker;

		protected ChartDataLabel m_dataLabel;

		public abstract DataElementOutputTypes DataElementOutput
		{
			get;
		}

		public abstract string DataElementName
		{
			get;
		}

		public abstract ChartDataPointValues DataPointValues
		{
			get;
		}

		public abstract ChartItemInLegend ItemInLegend
		{
			get;
		}

		public abstract ActionInfo ActionInfo
		{
			get;
		}

		public abstract CustomPropertyCollection CustomProperties
		{
			get;
		}

		public abstract Style Style
		{
			get;
		}

		public abstract ChartMarker Marker
		{
			get;
		}

		public abstract ChartDataLabel DataLabel
		{
			get;
		}

		public abstract ReportVariantProperty AxisLabel
		{
			get;
		}

		public abstract ReportStringProperty ToolTip
		{
			get;
		}

		internal abstract Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint DataPointDef
		{
			get;
		}

		internal abstract Microsoft.ReportingServices.ReportRendering.ChartDataPoint RenderItem
		{
			get;
		}

		internal abstract Microsoft.ReportingServices.ReportProcessing.ChartDataPoint RenderDataPointDef
		{
			get;
		}

		internal Chart ChartDef => m_owner;

		public ChartDataPointInstance Instance
		{
			get
			{
				if (m_owner.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new ChartDataPointInstance(this);
				}
				return m_instance;
			}
		}

		IReportScopeInstance IReportScope.ReportScopeInstance => Instance;

		IRIFReportScope IReportScope.RIFReportScope => RIFReportScope;

		internal virtual IRIFReportScope RIFReportScope => null;

		internal ChartDataPoint(Chart owner, int rowIndex, int colIndex)
		{
			m_owner = owner;
			m_rowIndex = rowIndex;
			m_columnIndex = colIndex;
			m_dataPointValues = null;
			m_actionInfo = null;
			m_customProperties = null;
		}

		void IDataRegionCell.SetNewContext()
		{
			SetNewContext();
		}

		internal virtual void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			m_customPropertiesReady = false;
			if (m_actionInfo != null)
			{
				m_actionInfo.SetNewContext();
			}
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
			if (m_marker != null)
			{
				m_marker.SetNewContext();
			}
			if (m_dataLabel != null)
			{
				m_dataLabel.SetNewContext();
			}
			if (m_dataPointValues != null)
			{
				m_dataPointValues.SetNewContext();
			}
		}
	}
}
