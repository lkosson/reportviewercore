using Microsoft.ReportingServices.ReportIntermediateFormat;
using System;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal sealed class RuntimeAggregationIncrementalDataSet : RuntimeIncrementalDataSetWithProcessingController
	{
		protected override bool ShouldCancelCommandDuringCleanup => false;

		public RuntimeAggregationIncrementalDataSet(DataSource dataSource, DataSet dataSet, DataSetInstance dataSetInstance, OnDemandProcessingContext odpContext)
			: base(dataSource, dataSet, dataSetInstance, odpContext)
		{
		}

		public void CalculateDataSetAggregates()
		{
			try
			{
				int rowIndex;
				RecordRow recordRow = ReadOneRow(out rowIndex);
				if (recordRow != null)
				{
					m_dataProcessingController.NextRow(recordRow, rowIndex, useRowOffset: true, base.HasServerAggregateMetadata);
				}
				m_dataProcessingController.AllRowsRead();
			}
			catch (Exception)
			{
				CleanupForException();
				FinalCleanup();
				throw;
			}
		}
	}
}
