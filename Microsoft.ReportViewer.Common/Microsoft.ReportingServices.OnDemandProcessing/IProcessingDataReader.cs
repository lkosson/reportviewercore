using Microsoft.ReportingServices.ReportIntermediateFormat;
using System;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal interface IProcessingDataReader : IDisposable
	{
		int AggregationFieldCount
		{
			get;
		}

		bool ReaderExtensionsSupported
		{
			get;
		}

		bool ReaderFieldProperties
		{
			get;
		}

		bool IsAggregateRow
		{
			get;
		}

		RecordSetInfo RecordSetInfo
		{
			get;
		}

		object GetColumn(int aliasIndex);

		bool GetNextRow();

		int GetPropertyCount(int aliasIndex);

		string GetPropertyName(int aliasIndex, int propertyIndex);

		object GetPropertyValue(int aliasIndex, int propertyIndex);

		RecordRow GetUnderlyingRecordRowObject();

		bool IsAggregationField(int aliasIndex);

		void OverrideDataCacheCompareOptions(ref OnDemandProcessingContext context);

		void OverrideWithDataReaderSettings(OnDemandProcessingContext odpContext, DataSetInstance dataSetInstance, DataSetCore dataSetCore);

		void GetDataReaderMappingForRowConsumer(DataSetInstance dataSetInstance, out bool mappingIdentical, out int[] mappingDataSetFieldIndexesToDataChunk);
	}
}
