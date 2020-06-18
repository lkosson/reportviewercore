using Microsoft.ReportingServices.DataExtensions;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal sealed class RuntimeDataSourceSharedDataSet : RuntimeAtomicDataSource
	{
		private RuntimeSharedDataSet m_runtimeDataSet;

		private readonly DataSetDefinition m_dataSetDefinition;

		protected override bool CreatesDataChunks => true;

		internal override bool NoRows => CheckNoRows(m_runtimeDataSet);

		internal RuntimeDataSourceSharedDataSet(DataSetDefinition dataSetDefinition, OnDemandProcessingContext odpContext)
			: base(null, new DataSource(-1, dataSetDefinition.SharedDataSourceReferenceId, dataSetDefinition.DataSetCore), odpContext, mergeTransactions: false)
		{
			m_dataSetDefinition = dataSetDefinition;
		}

		protected override List<RuntimeDataSet> CreateRuntimeDataSets()
		{
			List<RuntimeDataSet> list = new List<RuntimeDataSet>(1);
			DataSet dataSet = base.DataSourceDefinition.DataSets[0];
			m_runtimeDataSet = new RuntimeSharedDataSet(dataSetInstance: new DataSetInstance(dataSet), dataSource: base.DataSourceDefinition, dataSet: dataSet, processingContext: base.OdpContext);
			list.Add(m_runtimeDataSet);
			return list;
		}

		protected override void OpenInitialConnectionAndTransaction()
		{
		}
	}
}
