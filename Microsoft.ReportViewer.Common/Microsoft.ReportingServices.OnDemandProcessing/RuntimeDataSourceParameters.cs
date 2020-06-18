using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal sealed class RuntimeDataSourceParameters : RuntimeAtomicDataSource
	{
		private RuntimeParameterDataSet m_runtimeDataSet;

		private readonly int m_parameterDataSetIndex;

		private readonly ReportParameterDataSetCache m_paramDataCache;

		internal override bool NoRows => CheckNoRows(m_runtimeDataSet);

		internal RuntimeDataSourceParameters(Microsoft.ReportingServices.ReportIntermediateFormat.Report report, Microsoft.ReportingServices.ReportIntermediateFormat.DataSource dataSource, OnDemandProcessingContext processingContext, int parameterDataSetIndex, ReportParameterDataSetCache aCache)
			: base(report, dataSource, processingContext, mergeTransactions: false)
		{
			Global.Tracer.Assert(parameterDataSetIndex != -1, "Parameter DataSet index must be specified when processing parameters");
			m_parameterDataSetIndex = parameterDataSetIndex;
			m_paramDataCache = aCache;
		}

		protected override List<RuntimeDataSet> CreateRuntimeDataSets()
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet = base.DataSourceDefinition.DataSets[m_parameterDataSetIndex];
			DataSetInstance dataSetInstance = new DataSetInstance(dataSet);
			m_runtimeDataSet = new RuntimeParameterDataSet(base.DataSourceDefinition, dataSet, dataSetInstance, base.OdpContext, mustEvaluateThroughReportObjectModel: true, m_paramDataCache);
			return new List<RuntimeDataSet>(1)
			{
				m_runtimeDataSet
			};
		}
	}
}
