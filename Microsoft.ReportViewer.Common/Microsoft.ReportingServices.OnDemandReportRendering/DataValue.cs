using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportRendering;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class DataValue
	{
		private bool m_isChartValue;

		private Microsoft.ReportingServices.ReportIntermediateFormat.DataValue m_dataValue;

		private ReportStringProperty m_name;

		private ReportVariantProperty m_value;

		private DataValueInstance m_instance;

		private IInstancePath m_instancePath;

		private RenderingContext m_renderingContext;

		private string m_objectName;

		public ReportStringProperty Name => m_name;

		public ReportVariantProperty Value => m_value;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.DataValue DataValueDef => m_dataValue;

		internal RenderingContext RenderingContext => m_renderingContext;

		internal bool IsChart => m_isChartValue;

		internal IInstancePath InstancePath => m_instancePath;

		internal string ObjectName => m_objectName;

		public DataValueInstance Instance
		{
			get
			{
				if (m_renderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				return m_instance;
			}
		}

		internal DataValue(RenderingContext renderingContext, object chartDataValue)
		{
			m_isChartValue = true;
			m_name = new ReportStringProperty();
			m_value = new ReportVariantProperty(isExpression: true);
			m_instance = new ShimDataValueInstance(null, chartDataValue);
			m_renderingContext = renderingContext;
		}

		internal DataValue(RenderingContext renderingContext, Microsoft.ReportingServices.ReportRendering.DataValue dataValue)
		{
			m_isChartValue = false;
			string name = dataValue?.Name;
			object value = dataValue?.Value;
			m_name = new ReportStringProperty(isExpression: true, null, null);
			m_value = new ReportVariantProperty(isExpression: true);
			m_instance = new ShimDataValueInstance(name, value);
			m_renderingContext = renderingContext;
		}

		internal DataValue(IReportScope reportScope, RenderingContext renderingContext, Microsoft.ReportingServices.ReportIntermediateFormat.DataValue dataValue, bool isChart, IInstancePath instancePath, string objectName)
		{
			m_isChartValue = isChart;
			m_instancePath = instancePath;
			m_dataValue = dataValue;
			m_name = new ReportStringProperty(dataValue.Name);
			m_value = new ReportVariantProperty(dataValue.Value);
			m_instance = new InternalDataValueInstance(reportScope, this);
			m_renderingContext = renderingContext;
			m_objectName = objectName;
		}

		internal void UpdateChartDataValue(object dataValue)
		{
			if (m_dataValue == null && m_isChartValue)
			{
				((ShimDataValueInstance)m_instance).Update(null, dataValue);
			}
		}

		internal void UpdateDataCellValue(Microsoft.ReportingServices.ReportRendering.DataValue dataValue)
		{
			if (m_dataValue == null && !m_isChartValue)
			{
				string name = dataValue?.Name;
				object value = dataValue?.Value;
				((ShimDataValueInstance)m_instance).Update(name, value);
			}
		}

		internal void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
		}
	}
}
