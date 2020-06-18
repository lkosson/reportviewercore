using Microsoft.ReportingServices.ReportIntermediateFormat;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal sealed class RuntimeLiveReaderDataSource : RuntimeIncrementalDataSource
	{
		private DataSetInstance m_dataSetInstance;

		private RuntimeLiveDataReaderDataSet m_runtimeDataSet;

		public DataSetInstance DataSetInstance => m_dataSetInstance;

		protected override RuntimeIncrementalDataSet RuntimeDataSet => m_runtimeDataSet;

		internal RuntimeLiveReaderDataSource(Report report, DataSet dataSet, OnDemandProcessingContext odpContext)
			: base(report, dataSet, odpContext)
		{
		}

		protected override List<RuntimeDataSet> CreateRuntimeDataSets()
		{
			m_dataSetInstance = new DataSetInstance(m_dataSet);
			m_odpContext.CurrentReportInstance.SetDataSetInstance(m_dataSetInstance);
			m_runtimeDataSet = new RuntimeLiveDataReaderDataSet(base.DataSourceDefinition, m_dataSet, m_dataSetInstance, base.OdpContext);
			return new List<RuntimeDataSet>(1)
			{
				m_runtimeDataSet
			};
		}

		public RecordRow ReadNextRow()
		{
			try
			{
				return m_runtimeDataSet.ReadNextRow();
			}
			catch (Exception e)
			{
				HandleException(e);
				FinalCleanup();
				throw;
			}
		}
	}
}
