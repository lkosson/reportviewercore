using System;

namespace Microsoft.ReportingServices.ReportProcessing.ReportObjectModel
{
	internal sealed class DataSetImpl : DataSet
	{
		private Microsoft.ReportingServices.ReportProcessing.DataSet m_dataSet;

		public override string CommandText => m_dataSet.Query.CommandTextValue;

		public override string RewrittenCommandText => m_dataSet.Query.RewrittenCommandText;

		public override DateTime ExecutionTime
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		internal DataSetImpl(Microsoft.ReportingServices.ReportProcessing.DataSet dataSetDef)
		{
			m_dataSet = dataSetDef;
		}
	}
}
