using Microsoft.ReportingServices.ReportIntermediateFormat;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal sealed class RuntimeIdcIncrementalDataSource : RuntimeIncrementalDataSource
	{
		private RuntimeIdcIncrementalDataSet m_runtimeDataSet;

		protected override RuntimeIncrementalDataSet RuntimeDataSet => m_runtimeDataSet;

		internal RuntimeIdcIncrementalDataSource(DataSet dataSet, OnDemandProcessingContext odpContext)
			: base(odpContext.ReportDefinition, dataSet, odpContext)
		{
		}

		protected override List<RuntimeDataSet> CreateRuntimeDataSets()
		{
			DataSetInstance dataSetInstance = new DataSetInstance(m_dataSet);
			m_runtimeDataSet = new RuntimeIdcIncrementalDataSet(base.DataSourceDefinition, m_dataSet, dataSetInstance, base.OdpContext);
			return new List<RuntimeDataSet>(1)
			{
				m_runtimeDataSet
			};
		}

		public bool SetupNextRow()
		{
			try
			{
				return m_runtimeDataSet.GetNextRow() != null;
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
