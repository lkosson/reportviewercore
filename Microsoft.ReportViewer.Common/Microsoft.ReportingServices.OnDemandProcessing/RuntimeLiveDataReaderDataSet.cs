using Microsoft.ReportingServices.ReportIntermediateFormat;
using System;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal sealed class RuntimeLiveDataReaderDataSet : RuntimeIncrementalDataSet
	{
		internal RuntimeLiveDataReaderDataSet(DataSource dataSource, DataSet dataSet, DataSetInstance dataSetInstance, OnDemandProcessingContext odpContext)
			: base(dataSource, dataSet, dataSetInstance, odpContext)
		{
		}

		internal RecordRow ReadNextRow()
		{
			try
			{
				int rowIndex;
				return ReadOneRow(out rowIndex);
			}
			catch (Exception)
			{
				CleanupForException();
				FinalCleanup();
				throw;
			}
		}

		protected override void InitializeBeforeProcessingRows(bool aReaderExtensionsSupported)
		{
		}

		protected override void ProcessExtendedPropertyMappings()
		{
		}
	}
}
