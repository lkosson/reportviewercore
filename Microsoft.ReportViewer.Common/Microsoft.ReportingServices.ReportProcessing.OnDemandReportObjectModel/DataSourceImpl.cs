using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;

namespace Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	internal sealed class DataSourceImpl : Microsoft.ReportingServices.ReportProcessing.ReportObjectModel.DataSource
	{
		private Microsoft.ReportingServices.ReportIntermediateFormat.DataSource m_dataSource;

		public override string DataSourceReference
		{
			get
			{
				if (m_dataSource.SharedDataSourceReferencePath == null)
				{
					return m_dataSource.DataSourceReference;
				}
				return m_dataSource.SharedDataSourceReferencePath;
			}
		}

		public override string Type => m_dataSource.Type;

		internal DataSourceImpl(Microsoft.ReportingServices.ReportIntermediateFormat.DataSource dataSourceDef)
		{
			m_dataSource = dataSourceDef;
		}
	}
}
