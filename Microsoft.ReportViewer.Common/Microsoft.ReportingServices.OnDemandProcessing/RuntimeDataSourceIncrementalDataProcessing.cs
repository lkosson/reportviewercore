using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal sealed class RuntimeDataSourceIncrementalDataProcessing : RuntimeIncrementalDataSource
	{
		private RuntimeOnDemandIncrementalDataSet m_runtimeDataSet;

		public IOnDemandScopeInstance GroupTreeRoot => m_runtimeDataSet.GroupTreeRoot;

		protected override RuntimeIncrementalDataSet RuntimeDataSet => m_runtimeDataSet;

		internal RuntimeDataSourceIncrementalDataProcessing(DataSet dataSet, OnDemandProcessingContext odpContext)
			: base(odpContext.ReportDefinition, dataSet, odpContext)
		{
		}

		protected override List<RuntimeDataSet> CreateRuntimeDataSets()
		{
			DataSetInstance dataSetInstance = new DataSetInstance(m_dataSet);
			m_odpContext.CurrentReportInstance.SetDataSetInstance(dataSetInstance);
			m_runtimeDataSet = new RuntimeOnDemandIncrementalDataSet(base.DataSourceDefinition, m_dataSet, dataSetInstance, base.OdpContext);
			return new List<RuntimeDataSet>(1)
			{
				m_runtimeDataSet
			};
		}

		public void Advance()
		{
			try
			{
				m_runtimeDataSet.Advance();
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
