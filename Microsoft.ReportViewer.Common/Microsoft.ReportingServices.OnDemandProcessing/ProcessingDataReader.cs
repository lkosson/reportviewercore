using Microsoft.ReportingServices.DataExtensions;
using Microsoft.ReportingServices.DataProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal sealed class ProcessingDataReader : IProcessingDataReader, IDisposable
	{
		private RecordSetInfo m_recordSetInfo;

		private MappingDataReader m_dataSourceDataReader;

		private ChunkManager.DataChunkReader m_dataSnapshotReader;

		public RecordSetInfo RecordSetInfo => m_recordSetInfo;

		public bool ReaderExtensionsSupported
		{
			get
			{
				if (m_dataSourceDataReader != null)
				{
					return m_dataSourceDataReader.ReaderExtensionsSupported;
				}
				return m_dataSnapshotReader.ReaderExtensionsSupported;
			}
		}

		public bool ReaderFieldProperties
		{
			get
			{
				if (m_dataSourceDataReader != null)
				{
					return m_dataSourceDataReader.ReaderFieldProperties;
				}
				return m_dataSnapshotReader.ReaderFieldProperties;
			}
		}

		public bool IsAggregateRow
		{
			get
			{
				if (m_dataSourceDataReader != null)
				{
					return m_dataSourceDataReader.IsAggregateRow;
				}
				return m_dataSnapshotReader.IsAggregateRow;
			}
		}

		public int AggregationFieldCount
		{
			get
			{
				if (m_dataSourceDataReader != null)
				{
					return m_dataSourceDataReader.AggregationFieldCount;
				}
				return m_dataSnapshotReader.AggregationFieldCount;
			}
		}

		internal ProcessingDataReader(OnDemandProcessingContext odpContext, DataSetInstance dataSetInstance, string dataSetName, IDataReader sourceReader, bool hasServerAggregateMetadata, string[] aliases, string[] names, DataSourceErrorInspector errorInspector)
		{
			m_recordSetInfo = new RecordSetInfo(hasServerAggregateMetadata, odpContext.IsSharedDataSetExecutionOnly, dataSetInstance, odpContext.ExecutionTime);
			m_dataSourceDataReader = new MappingDataReader(dataSetName, sourceReader, aliases, names, errorInspector);
		}

		internal ProcessingDataReader(DataSetInstance dataSetInstance, DataSet dataSet, OnDemandProcessingContext odpContext, bool overrideWithSharedDataSetChunkSettings)
		{
			if (odpContext.IsSharedDataSetExecutionOnly)
			{
				m_dataSnapshotReader = new ChunkManager.DataChunkReader(dataSetInstance, odpContext, odpContext.ExternalDataSetContext.CachedDataChunkName);
			}
			else
			{
				m_dataSnapshotReader = odpContext.GetDataChunkReader(dataSet.IndexInCollection);
			}
			m_recordSetInfo = m_dataSnapshotReader.RecordSetInfo;
			m_dataSnapshotReader.MoveToFirstRow();
			if (overrideWithSharedDataSetChunkSettings)
			{
				OverrideWithDataReaderSettings(odpContext, dataSetInstance, dataSet.DataSetCore);
			}
			else
			{
				OverrideDataCacheCompareOptions(ref odpContext);
			}
		}

		public void Dispose()
		{
			if (m_dataSourceDataReader != null)
			{
				((IDisposable)m_dataSourceDataReader).Dispose();
			}
			else
			{
				((IDisposable)m_dataSnapshotReader).Dispose();
			}
		}

		public void OverrideDataCacheCompareOptions(ref OnDemandProcessingContext context)
		{
			if (m_dataSnapshotReader != null && (context.ProcessWithCachedData || context.SnapshotProcessing) && m_dataSnapshotReader.ValidCompareOptions)
			{
				context.ClrCompareOptions = m_dataSnapshotReader.CompareOptions;
			}
		}

		public bool GetNextRow()
		{
			if (m_dataSourceDataReader != null)
			{
				return m_dataSourceDataReader.GetNextRow();
			}
			return m_dataSnapshotReader.GetNextRow();
		}

		public RecordRow GetUnderlyingRecordRowObject()
		{
			if (m_dataSnapshotReader != null)
			{
				return m_dataSnapshotReader.RecordRow;
			}
			return null;
		}

		public object GetColumn(int aliasIndex)
		{
			object obj = null;
			obj = ((m_dataSourceDataReader == null) ? m_dataSnapshotReader.GetFieldValue(aliasIndex) : m_dataSourceDataReader.GetFieldValue(aliasIndex));
			if (obj is DBNull)
			{
				return null;
			}
			return obj;
		}

		public bool IsAggregationField(int aliasIndex)
		{
			if (m_dataSourceDataReader != null)
			{
				return m_dataSourceDataReader.IsAggregationField(aliasIndex);
			}
			return m_dataSnapshotReader.IsAggregationField(aliasIndex);
		}

		public int GetPropertyCount(int aliasIndex)
		{
			if (m_dataSourceDataReader != null)
			{
				return m_dataSourceDataReader.GetPropertyCount(aliasIndex);
			}
			if (m_dataSnapshotReader != null && m_dataSnapshotReader.FieldPropertyNames != null && m_dataSnapshotReader.FieldPropertyNames[aliasIndex] != null)
			{
				List<string> propertyNames = m_dataSnapshotReader.FieldPropertyNames.GetPropertyNames(aliasIndex);
				if (propertyNames != null)
				{
					return propertyNames.Count;
				}
			}
			return 0;
		}

		public string GetPropertyName(int aliasIndex, int propertyIndex)
		{
			if (m_dataSourceDataReader != null)
			{
				return m_dataSourceDataReader.GetPropertyName(aliasIndex, propertyIndex);
			}
			if (m_dataSnapshotReader != null && m_dataSnapshotReader.FieldPropertyNames != null)
			{
				return m_dataSnapshotReader.FieldPropertyNames.GetPropertyName(aliasIndex, propertyIndex);
			}
			return null;
		}

		public object GetPropertyValue(int aliasIndex, int propertyIndex)
		{
			object obj = null;
			if (m_dataSourceDataReader != null)
			{
				obj = m_dataSourceDataReader.GetPropertyValue(aliasIndex, propertyIndex);
			}
			else if (m_dataSnapshotReader != null)
			{
				obj = m_dataSnapshotReader.GetPropertyValue(aliasIndex, propertyIndex);
			}
			if (obj is DBNull)
			{
				return null;
			}
			return obj;
		}

		public void OverrideWithDataReaderSettings(OnDemandProcessingContext odpContext, DataSetInstance dataSetInstance, DataSetCore dataSetCore)
		{
			ChunkManager.DataChunkReader.OverrideWithDataReaderSettings(m_recordSetInfo, odpContext, dataSetInstance, dataSetCore);
		}

		public void GetDataReaderMappingForRowConsumer(DataSetInstance dataSetInstance, out bool mappingIdentical, out int[] mappingDataSetFieldIndexesToDataChunk)
		{
			mappingIdentical = true;
			mappingDataSetFieldIndexesToDataChunk = null;
			ChunkManager.DataChunkReader.CreateDataChunkFieldMapping(dataSetInstance, m_recordSetInfo, isSharedDataSetExecutionReader: false, out mappingIdentical, out mappingDataSetFieldIndexesToDataChunk);
		}
	}
}
