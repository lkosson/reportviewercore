using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal abstract class RuntimeDataSourceDataProcessing : RuntimeAtomicDataSource
	{
		protected readonly DataSet m_dataSet;

		private RuntimeOnDemandDataSet m_runtimeDataSet;

		internal RuntimeOnDemandDataSet RuntimeDataSet => m_runtimeDataSet;

		internal override bool NoRows => CheckNoRows(m_runtimeDataSet);

		internal RuntimeDataSourceDataProcessing(DataSet dataSet, OnDemandProcessingContext processingContext)
			: base(processingContext.ReportDefinition, dataSet.DataSource, processingContext, mergeTransactions: false)
		{
			m_dataSet = dataSet;
		}

		internal void ProcessSingleOdp()
		{
			ExecutedQuery query = null;
			try
			{
				m_odpContext.StateManager.ExecutedQueryCache?.Extract(m_dataSet, out query);
				if (InitializeDataSource(query))
				{
					m_runtimeDataSet.InitProcessingParams(m_connection, m_transaction);
					m_runtimeDataSet.ProcessInline(query);
					m_executionMetrics.Add(m_runtimeDataSet.DataSetExecutionMetrics);
					if (m_totalDurationFromExistingQuery != null)
					{
						m_executionMetrics.TotalDuration.Subtract(m_totalDurationFromExistingQuery);
					}
					TeardownDataSource();
				}
			}
			catch (Exception e)
			{
				HandleException(e);
				throw;
			}
			finally
			{
				FinalCleanup();
				query?.Close();
			}
		}

		protected override List<RuntimeDataSet> CreateRuntimeDataSets()
		{
			m_runtimeDataSet = CreateRuntimeDataSet();
			return new List<RuntimeDataSet>(1)
			{
				m_runtimeDataSet
			};
		}

		protected abstract RuntimeOnDemandDataSet CreateRuntimeDataSet();
	}
}
