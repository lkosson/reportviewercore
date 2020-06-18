using Microsoft.Cloud.Platform.Utils;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Diagnostics;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal abstract class RuntimeIncrementalDataSource : RuntimeDataSource
	{
		protected readonly Microsoft.ReportingServices.ReportIntermediateFormat.DataSet m_dataSet;

		protected abstract RuntimeIncrementalDataSet RuntimeDataSet
		{
			get;
		}

		internal override bool NoRows => CheckNoRows(RuntimeDataSet);

		protected RuntimeIncrementalDataSource(Microsoft.ReportingServices.ReportIntermediateFormat.Report report, Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet, OnDemandProcessingContext odpContext)
			: base(report, dataSet.DataSource, odpContext, mergeTransactions: false)
		{
			m_dataSet = dataSet;
		}

		internal void Initialize()
		{
			ExecutedQuery query = null;
			try
			{
				m_odpContext.StateManager.ExecutedQueryCache?.Extract(m_dataSet, out query);
				InitializeDataSource(query);
				InitializeDataSet(query);
			}
			catch (Exception e)
			{
				HandleException(e);
				FinalCleanup();
				query?.Close();
				throw;
			}
		}

		internal override void Abort()
		{
			if (Global.Tracer.TraceVerbose)
			{
				Global.Tracer.Trace(TraceLevel.Verbose, "Data source '{0}': Abort handler called.", m_dataSource.Name.MarkAsModelInfo());
			}
			if (RuntimeDataSet != null)
			{
				RuntimeDataSet.Abort();
			}
		}

		protected void InitializeDataSet(ExecutedQuery existingQuery)
		{
			RuntimeDataSet.InitProcessingParams(m_connection, m_transaction);
			RuntimeDataSet.Initialize(existingQuery);
		}

		internal void Teardown()
		{
			try
			{
				TeardownDataSet();
				TeardownDataSource();
			}
			catch (Exception e)
			{
				HandleException(e);
				throw;
			}
			finally
			{
				FinalCleanup();
			}
		}

		protected override void FinalCleanup()
		{
			base.FinalCleanup();
			if (RuntimeDataSet != null)
			{
				TimeMetric timeMetric = RuntimeDataSet.DataSetExecutionMetrics.TotalDuration;
				if (m_totalDurationFromExistingQuery != null)
				{
					timeMetric = new TimeMetric(timeMetric);
					timeMetric.Subtract(m_totalDurationFromExistingQuery);
				}
				m_odpContext.ExecutionLogContext.AddDataProcessingTime(timeMetric);
				m_executionMetrics.Add(RuntimeDataSet.DataSetExecutionMetrics);
			}
		}

		protected virtual void TeardownDataSet()
		{
			RuntimeDataSet.Teardown();
		}

		internal void RecordSkippedRowCount(long rowCount)
		{
			RuntimeDataSet.RecordSkippedRowCount(rowCount);
		}
	}
}
