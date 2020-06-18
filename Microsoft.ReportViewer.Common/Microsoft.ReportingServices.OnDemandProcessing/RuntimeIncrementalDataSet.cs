using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal abstract class RuntimeIncrementalDataSet : RuntimeDataSet
	{
		protected override bool ShouldCancelCommandDuringCleanup => true;

		protected RuntimeIncrementalDataSet(Microsoft.ReportingServices.ReportIntermediateFormat.DataSource dataSource, Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet, DataSetInstance dataSetInstance, OnDemandProcessingContext odpContext)
			: base(dataSource, dataSet, dataSetInstance, odpContext, processRetrievedData: true)
		{
		}

		internal void Initialize(ExecutedQuery existingQuery)
		{
			try
			{
				InitializeDataSet();
				if (m_dataSet.IsReferenceToSharedDataSet)
				{
					Global.Tracer.Assert(condition: false, "Shared data sets cannot be used with a RuntimeIncrementalDataSet");
				}
				else if (existingQuery != null)
				{
					InitializeAndRunFromExistingQuery(existingQuery);
				}
				else
				{
					InitializeAndRunLiveQuery();
				}
			}
			catch (Exception)
			{
				CleanupForException();
				FinalCleanup();
				throw;
			}
		}

		internal void Teardown()
		{
			try
			{
				CleanupProcess();
				TeardownDataSet();
			}
			catch (Exception)
			{
				CleanupForException();
				throw;
			}
			finally
			{
				FinalCleanup();
			}
		}

		internal void RecordSkippedRowCount(long rowCount)
		{
			m_executionMetrics.AddSkippedRowCount(rowCount);
		}
	}
}
