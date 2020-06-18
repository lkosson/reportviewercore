using Microsoft.ReportingServices.ReportIntermediateFormat;
using System;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal sealed class RuntimeOnDemandIncrementalDataSet : RuntimeIncrementalDataSetWithProcessingController
	{
		public RuntimeOnDemandIncrementalDataSet(DataSource dataSource, DataSet dataSet, DataSetInstance dataSetInstance, OnDemandProcessingContext odpContext)
			: base(dataSource, dataSet, dataSetInstance, odpContext)
		{
		}

		public void Advance()
		{
			try
			{
				bool isAggregateRow;
				while (ReadAndProcessOneRow(out isAggregateRow) && !m_odpContext.StateManager.ShouldStopPipelineAdvance(!isAggregateRow))
				{
				}
			}
			catch (Exception)
			{
				CleanupForException();
				FinalCleanup();
				throw;
			}
		}

		private bool ReadAndProcessOneRow(out bool isAggregateRow)
		{
			isAggregateRow = false;
			int rowIndex;
			RecordRow recordRow = ReadOneRow(out rowIndex);
			if (recordRow == null)
			{
				return false;
			}
			isAggregateRow = recordRow.IsAggregateRow;
			m_dataProcessingController.NextRow(recordRow, rowIndex, useRowOffset: true, base.HasServerAggregateMetadata);
			return true;
		}
	}
}
