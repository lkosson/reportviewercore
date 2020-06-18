using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class DataSet
	{
		private FieldCollection m_fields;

		private DataSetInstance m_instance;

		private Microsoft.ReportingServices.ReportIntermediateFormat.DataSet m_dataSetDef;

		private RenderingContext m_renderingContext;

		public string Name => m_dataSetDef.Name;

		public DataSetInstance Instance
		{
			get
			{
				if (m_renderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new DataSetInstance(this);
				}
				return m_instance;
			}
		}

		public FieldCollection NonCalculatedFields
		{
			get
			{
				if (m_fields == null)
				{
					m_fields = new FieldCollection(m_dataSetDef);
				}
				return m_fields;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.DataSet DataSetDef => m_dataSetDef;

		internal RenderingContext RenderingContext => m_renderingContext;

		internal DataSet(Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSetDef, RenderingContext renderingContext)
		{
			m_dataSetDef = dataSetDef;
			m_renderingContext = renderingContext;
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
