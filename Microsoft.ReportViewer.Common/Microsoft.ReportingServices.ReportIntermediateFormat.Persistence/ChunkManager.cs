using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.ReportingServices.ReportIntermediateFormat.Persistence
{
	internal sealed class ChunkManager
	{
		internal sealed class DataChunkWriter : PersistenceHelper
		{
			private static List<Declaration> m_DataChunkDeclarations = GetDataChunkDeclarations();

			private IChunkFactory m_reportChunkFactory;

			private string m_dataSetChunkName;

			private RecordSetInfo m_recordSetInfo;

			private IntermediateFormatWriter? m_chunkWriter;

			private Stream m_chunkStream;

			private OnDemandProcessingContext m_odpContext;

			internal DataChunkWriter(RecordSetInfo recordSetInfo, DataSetInstance dataSetInstance, OnDemandProcessingContext odpContext)
			{
				Global.Tracer.Assert(odpContext.ChunkFactory != null, "(null != context.ChunkFactory)");
				m_reportChunkFactory = odpContext.ChunkFactory;
				m_recordSetInfo = recordSetInfo;
				m_odpContext = odpContext;
				if (odpContext.IsSharedDataSetExecutionOnly)
				{
					m_dataSetChunkName = (odpContext.ExternalDataSetContext.TargetChunkNameInSnapshot ?? "SharedDataSet");
					return;
				}
				m_dataSetChunkName = GenerateDataChunkName(dataSetInstance, odpContext);
				odpContext.OdpMetadata.AddDataChunk(m_dataSetChunkName, dataSetInstance);
			}

			internal DataChunkWriter(DataSetInstance dataSetInstance, OnDemandProcessingContext context)
			{
				Global.Tracer.Assert(context.ChunkFactory != null, "(null != context.ChunkFactory)");
				m_odpContext = context;
				m_dataSetChunkName = GenerateDataChunkName(dataSetInstance, context);
				m_reportChunkFactory = context.ChunkFactory;
			}

			internal void Close()
			{
				m_chunkWriter = null;
				if (m_chunkStream != null)
				{
					m_chunkStream.Close();
					m_chunkStream = null;
				}
			}

			internal void CloseAndEraseChunk()
			{
				Close();
				if (m_reportChunkFactory != null)
				{
					m_reportChunkFactory.Erase(m_dataSetChunkName, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Data);
					if (!m_odpContext.IsSharedDataSetExecutionOnly)
					{
						m_odpContext.OdpMetadata.DeleteDataChunk(m_dataSetChunkName);
					}
				}
			}

			internal void CreateDataChunkAndWriteHeader(RecordSetInfo recordSetInfo)
			{
				if (m_chunkStream == null)
				{
					m_recordSetInfo = recordSetInfo;
					m_chunkStream = m_reportChunkFactory.CreateChunk(m_dataSetChunkName, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Data, null);
					m_chunkWriter = new IntermediateFormatWriter(m_chunkStream, m_DataChunkDeclarations, this, m_odpContext.GetActiveCompatibilityVersion(), m_odpContext.ProhibitSerializableValues);
					m_chunkWriter.Value.Write(m_recordSetInfo);
				}
			}

			internal void WriteRecordRow(RecordRow recordRow)
			{
				try
				{
					if (m_chunkStream == null)
					{
						CreateDataChunkAndWriteHeader(m_recordSetInfo);
					}
					recordRow.StreamPosition = m_chunkStream.Position;
					m_chunkWriter.Value.Write(recordRow);
				}
				catch (Exception)
				{
					Close();
					throw;
				}
			}

			private static List<Declaration> GetDataChunkDeclarations()
			{
				return new List<Declaration>(4)
				{
					RecordSetInfo.GetDeclaration(),
					RecordRow.GetDeclaration(),
					RecordField.GetDeclaration(),
					RecordSetPropertyNames.GetDeclaration()
				};
			}
		}

		internal sealed class DataChunkReader : IRecordRowReader, IDisposable
		{
			private Stream m_chunkStream;

			private IntermediateFormatReader? m_chunkReader;

			private RecordSetInfo m_recordSetInfo;

			private RecordRow m_recordRow;

			private int m_recordSetSize = -1;

			private int m_currentRow = -1;

			private long m_streamLength = -1L;

			private long m_previousStreamOffset = -1L;

			private long m_firstRow = -1L;

			private int[] m_mappingDataSetFieldIndexesToDataChunk;

			private bool m_mappingIdentical;

			internal bool ReaderExtensionsSupported
			{
				get
				{
					if (m_chunkStream == null)
					{
						return false;
					}
					return m_recordSetInfo.ReaderExtensionsSupported;
				}
			}

			internal bool ReaderFieldProperties
			{
				get
				{
					if (m_recordSetInfo != null)
					{
						return m_recordSetInfo.FieldPropertyNames != null;
					}
					return false;
				}
			}

			internal bool ValidCompareOptions
			{
				get
				{
					if (m_chunkStream == null)
					{
						return false;
					}
					return m_recordSetInfo.ValidCompareOptions;
				}
			}

			internal CompareOptions CompareOptions => m_recordSetInfo.CompareOptions;

			internal RecordSetInfo RecordSetInfo => m_recordSetInfo;

			public RecordRow RecordRow => m_recordRow;

			internal bool IsAggregateRow => m_recordRow.IsAggregateRow;

			internal int AggregationFieldCount => m_recordRow.AggregationFieldCount;

			internal RecordSetPropertyNamesList FieldPropertyNames => m_recordSetInfo.FieldPropertyNames;

			internal DataChunkReader(DataSetInstance dataSetInstance, OnDemandProcessingContext context, string chunkName)
			{
				m_recordSetSize = dataSetInstance.RecordSetSize;
				Global.Tracer.Assert(context.ChunkFactory != null && !string.IsNullOrEmpty(chunkName), "null != context.ChunkFactory && !String.IsNullOrEmpty(chunkName)");
				m_chunkStream = context.ChunkFactory.GetChunk(chunkName, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Data, ChunkMode.Open, out string _);
				Global.Tracer.Assert(m_chunkStream != null, "Missing Expected DataChunk with name: {0}", chunkName);
				m_chunkReader = new IntermediateFormatReader(m_chunkStream, default(DataReaderRIFObjectCreator));
				m_recordSetInfo = (RecordSetInfo)m_chunkReader.Value.ReadRIFObject();
				if (context.IsSharedDataSetExecutionOnly || dataSetInstance.DataSetDef.IsReferenceToSharedDataSet)
				{
					CreateDataChunkFieldMapping(dataSetInstance, m_recordSetInfo, context.IsSharedDataSetExecutionOnly, out m_mappingIdentical, out m_mappingDataSetFieldIndexesToDataChunk);
				}
				m_firstRow = m_chunkStream.Position;
				if (-1 == m_recordSetSize)
				{
					m_streamLength = m_chunkStream.Length;
					Global.Tracer.Assert(m_streamLength >= m_firstRow, "(m_streamLength >= m_firstRow)");
				}
			}

			internal static void OverrideWithDataReaderSettings(RecordSetInfo recordSetInfo, OnDemandProcessingContext odpContext, DataSetInstance dataSetInstance, DataSetCore dataSetCore)
			{
				if (recordSetInfo != null)
				{
					dataSetCore.MergeCollationSettings(null, null, recordSetInfo.CultureName, (recordSetInfo.CompareOptions & CompareOptions.IgnoreCase) == 0, (recordSetInfo.CompareOptions & CompareOptions.IgnoreNonSpace) == 0, (recordSetInfo.CompareOptions & CompareOptions.IgnoreKanaType) == 0, (recordSetInfo.CompareOptions & CompareOptions.IgnoreWidth) == 0);
					odpContext.SetComparisonInformation(dataSetCore);
					odpContext.ReportObjectModel.FieldsImpl.ReaderExtensionsSupported = recordSetInfo.ReaderExtensionsSupported;
					odpContext.ReportObjectModel.FieldsImpl.ReaderFieldProperties = (recordSetInfo != null && recordSetInfo.FieldPropertyNames != null);
					dataSetInstance.CommandText = recordSetInfo.CommandText;
					dataSetInstance.RewrittenCommandText = recordSetInfo.RewrittenCommandText;
					dataSetInstance.SetQueryExecutionTime(recordSetInfo.ExecutionTime);
				}
			}

			internal static void CreateDataChunkFieldMapping(DataSetInstance currentDataSetInstance, RecordSetInfo recordSetInfo, bool isSharedDataSetExecutionReader, out bool mappingIdentical, out int[] mappingDataSetFieldIndexesToDataChunk)
			{
				mappingDataSetFieldIndexesToDataChunk = null;
				mappingIdentical = true;
				string[] fieldNames = recordSetInfo.FieldNames;
				RecordSetPropertyNamesList fieldPropertyNames = recordSetInfo.FieldPropertyNames;
				List<Field> fields = currentDataSetInstance.DataSetDef.Fields;
				bool flag = isSharedDataSetExecutionReader;
				if (fieldNames == null || fields == null)
				{
					return;
				}
				int num = flag ? currentDataSetInstance.DataSetDef.Fields.Count : currentDataSetInstance.DataSetDef.NonCalculatedFieldCount;
				if (fieldPropertyNames != null && fieldPropertyNames.Count > 0)
				{
					currentDataSetInstance.FieldInfos = new FieldInfo[num];
				}
				mappingIdentical = (fieldNames.Length == num);
				Dictionary<string, int> dictionary = new Dictionary<string, int>(StringComparer.Ordinal);
				for (int i = 0; i < fieldNames.Length; i++)
				{
					dictionary.Add(fieldNames[i], i);
				}
				int count = fields.Count;
				int num2 = 0;
				mappingDataSetFieldIndexesToDataChunk = new int[num];
				for (int j = 0; j < count; j++)
				{
					if (fields[j].IsCalculatedField && !flag)
					{
						continue;
					}
					string key = fields[j].DataField;
					if (isSharedDataSetExecutionReader || fields[j].IsCalculatedField)
					{
						key = fields[j].Name;
					}
					if (dictionary.TryGetValue(key, out int value))
					{
						mappingDataSetFieldIndexesToDataChunk[num2] = value;
						if (fieldPropertyNames != null && value < fieldPropertyNames.Count && fieldPropertyNames[value] != null)
						{
							List<string> propertyNames = fieldPropertyNames.GetPropertyNames(value);
							if (propertyNames != null)
							{
								currentDataSetInstance.FieldInfos[num2] = new FieldInfo(CreateSequentialIndexList(propertyNames.Count), propertyNames);
							}
						}
						if (num2 != value)
						{
							mappingIdentical = false;
						}
					}
					else
					{
						mappingDataSetFieldIndexesToDataChunk[num2] = -1;
						mappingIdentical = false;
					}
					num2++;
				}
			}

			private static List<int> CreateSequentialIndexList(int capacity)
			{
				List<int> list = new List<int>(capacity);
				for (int i = 0; i < capacity; i++)
				{
					list.Add(i);
				}
				return list;
			}

			public bool MoveToFirstRow()
			{
				if (m_chunkStream == null || !m_chunkStream.CanSeek)
				{
					return false;
				}
				m_chunkReader.Value.Seek(m_firstRow, SeekOrigin.Begin);
				m_currentRow = -1;
				m_previousStreamOffset = -1L;
				m_recordRow = null;
				return true;
			}

			internal void ResetCachedStreamOffset()
			{
				m_previousStreamOffset = -1L;
			}

			public bool GetNextRow()
			{
				if (m_chunkStream == null)
				{
					return false;
				}
				bool flag = false;
				if (-1 == m_recordSetSize)
				{
					if (m_chunkStream.Position < m_streamLength - 1)
					{
						flag = true;
					}
				}
				else if (m_currentRow < m_recordSetSize - 1)
				{
					flag = true;
				}
				if (flag)
				{
					m_previousStreamOffset = m_chunkStream.Position;
					m_currentRow++;
					ReadNextRow();
				}
				return flag;
			}

			internal bool ReadOneRowAtPosition(long offset)
			{
				if (m_chunkStream == null)
				{
					return false;
				}
				if (m_previousStreamOffset == offset)
				{
					return false;
				}
				m_previousStreamOffset = offset;
				m_chunkReader.Value.Seek(offset, SeekOrigin.Begin);
				ReadNextRow();
				return true;
			}

			internal object GetFieldValue(int aliasIndex)
			{
				object obj = null;
				if (m_recordRow.RecordFields[aliasIndex] == null)
				{
					throw new ReportProcessingException_FieldError(DataFieldStatus.IsMissing, null);
				}
				return m_recordRow.GetFieldValue(aliasIndex);
			}

			internal bool IsAggregationField(int aliasIndex)
			{
				return m_recordRow.IsAggregationField(aliasIndex);
			}

			internal object GetPropertyValue(int aliasIndex, int propertyIndex)
			{
				if (m_recordSetInfo.FieldPropertyNames != null && m_recordRow.RecordFields[aliasIndex] != null)
				{
					List<object> fieldPropertyValues = m_recordRow.RecordFields[aliasIndex].FieldPropertyValues;
					if (fieldPropertyValues != null && propertyIndex >= 0 && propertyIndex < fieldPropertyValues.Count)
					{
						return fieldPropertyValues[propertyIndex];
					}
				}
				return null;
			}

			internal int GetPropertyCount(int aliasIndex)
			{
				if (m_recordSetInfo.FieldPropertyNames != null && m_recordRow.RecordFields[aliasIndex] != null && m_recordRow.RecordFields[aliasIndex].FieldPropertyValues != null)
				{
					return m_recordRow.RecordFields[aliasIndex].FieldPropertyValues.Count;
				}
				return 0;
			}

			internal string GetPropertyName(int aliasIndex, int propertyIndex)
			{
				if (m_recordSetInfo.FieldPropertyNames != null && m_recordSetInfo.FieldPropertyNames[aliasIndex] != null)
				{
					return m_recordSetInfo.FieldPropertyNames[aliasIndex].PropertyNames[propertyIndex];
				}
				return null;
			}

			public void Close()
			{
				Dispose(disposing: true);
			}

			public void Dispose()
			{
				Dispose(disposing: true);
			}

			private void Dispose(bool disposing)
			{
				if (disposing)
				{
					if (m_chunkReader.HasValue)
					{
						m_chunkReader = null;
					}
					if (m_chunkStream != null)
					{
						m_chunkStream.Close();
						m_chunkStream = null;
					}
				}
				m_recordRow = null;
				m_recordSetInfo = null;
			}

			private void ReadNextRow()
			{
				m_recordRow = (RecordRow)m_chunkReader.Value.ReadRIFObject();
				if (!m_mappingIdentical)
				{
					m_recordRow.ApplyFieldMapping(m_mappingDataSetFieldIndexesToDataChunk);
				}
			}
		}

		internal sealed class OnDemandProcessingManager
		{
			private static List<Declaration> m_ChunkDeclarations;

			private OnDemandProcessingContext m_odpContext;

			internal OnDemandProcessingManager()
			{
			}

			internal void SetOdpContext(OnDemandProcessingContext odpContext)
			{
				m_odpContext = odpContext;
			}

			internal static GlobalIDOwnerCollection DeserializeOdpReportSnapshot(ProcessingContext pc, IChunkFactory originalSnapshotChunks, ProcessingErrorContext errorContext, bool fetchSubreports, bool deserializeGroupTree, IConfiguration configuration, ref OnDemandMetadata odpMetadata, out Report report)
			{
				GlobalIDOwnerCollection globalIDOwnerCollection = new GlobalIDOwnerCollection();
				report = Microsoft.ReportingServices.ReportProcessing.ReportProcessing.DeserializeKatmaiReport(pc.ChunkFactory, keepReferences: true, globalIDOwnerCollection);
				IChunkFactory chunkFactory = originalSnapshotChunks ?? pc.ChunkFactory;
				if (odpMetadata == null)
				{
					odpMetadata = DeserializeOnDemandMetadata(chunkFactory, globalIDOwnerCollection);
				}
				if (pc.Parameters != null)
				{
					pc.Parameters.StoreLabels();
				}
				if (fetchSubreports)
				{
					Microsoft.ReportingServices.ReportProcessing.ReportProcessing.FetchSubReports(report, pc.ChunkFactory, errorContext, odpMetadata, pc.ReportContext, pc.OnDemandSubReportCallback, 0, snapshotProcessing: true, processWithCachedData: false, globalIDOwnerCollection, pc.QueryParameters);
					if (deserializeGroupTree)
					{
						DeserializeGroupTree(report, chunkFactory, globalIDOwnerCollection, configuration, ref odpMetadata);
					}
				}
				odpMetadata.GlobalIDOwnerCollection = globalIDOwnerCollection;
				return globalIDOwnerCollection;
			}

			internal static void DeserializeGroupTree(Report report, IChunkFactory chunkFactory, GlobalIDOwnerCollection globalIDOwnerCollection, IConfiguration configuration, ref OnDemandMetadata odpMetadata)
			{
				bool prohibitSerializableValues = configuration?.ProhibitSerializableValues ?? false;
				EnsureGroupTreeStorageSetup(odpMetadata, chunkFactory, globalIDOwnerCollection, openExisting: true, ReportProcessingCompatibilityVersion.GetCompatibilityVersion(configuration), prohibitSerializableValues);
				GroupTreePartition groupTreePartition = (GroupTreePartition)odpMetadata.GroupTreeScalabilityCache.Storage.Retrieve(odpMetadata.GroupTreeRootOffset);
				Global.Tracer.Assert(groupTreePartition.TopLevelScopeInstances[0].GetObjectType() == ObjectType.ReportInstanceReference, "GroupTree root partition did not contain a ReportInstance");
				odpMetadata.ReportInstance = (groupTreePartition.TopLevelScopeInstances[0] as IReference<ReportInstance>);
				odpMetadata.Report = report;
				odpMetadata.ReportSnapshot.Report = report;
			}

			internal static void EnsureGroupTreeStorageSetup(OnDemandMetadata odpMetadata, IChunkFactory chunkFactory, GlobalIDOwnerCollection globalIDOwnerCollection, bool openExisting, int rifCompatVersion, bool prohibitSerializableValues)
			{
				if (odpMetadata.GroupTreeScalabilityCache == null)
				{
					IStorage storage = new RIFAppendOnlyStorage(BuildChunkStreamHandler("GroupTree", Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Main, chunkFactory, openExisting), default(GroupTreeRIFObjectCreator), GroupTreeReferenceCreator.Instance, globalIDOwnerCollection, openExisting, rifCompatVersion, prohibitSerializableValues);
					odpMetadata.GroupTreeScalabilityCache = new GroupTreeScalabilityCache(odpMetadata.GroupTreePartitionManager, storage);
				}
			}

			internal static void EnsureLookupStorageSetup(OnDemandMetadata odpMetadata, IChunkFactory chunkFactory, bool openExisting, int rifCompatVersion, bool prohibitSerializableValues)
			{
				if (odpMetadata.LookupScalabilityCache == null)
				{
					new AppendOnlySpaceManager();
					IStorage storage = new RIFAppendOnlyStorage(BuildChunkStreamHandler("LookupInfo", Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.LookupInfo, chunkFactory, openExisting), default(LookupRIFObjectCreator), LookupReferenceCreator.Instance, null, openExisting, rifCompatVersion, prohibitSerializableValues);
					odpMetadata.LookupScalabilityCache = new LookupScalabilityCache(odpMetadata.LookupPartitionManager, storage);
				}
			}

			private static IStreamHandler BuildChunkStreamHandler(string chunkName, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes chunkType, IChunkFactory chunkFactory, bool openExisting)
			{
				return new ChunkFactoryStreamHandler(chunkName, chunkType, chunkFactory, openExisting);
			}

			internal static void PreparePartitionedTreesForAsyncSerialization(OnDemandProcessingContext odpContext)
			{
				PreparePartitionedTreeForAsyncSerialization(odpContext.OdpMetadata.GroupTreeScalabilityCache, odpContext, "GroupTree", Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Main);
				PreparePartitionedTreeForAsyncSerialization(odpContext.OdpMetadata.LookupScalabilityCache, odpContext, "LookupInfo", Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.LookupInfo);
			}

			private static void PreparePartitionedTreeForAsyncSerialization(PartitionedTreeScalabilityCache scaleCache, OnDemandProcessingContext odpContext, string chunkName, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes chunkType)
			{
				if (scaleCache != null)
				{
					RIFAppendOnlyStorage rIFAppendOnlyStorage = scaleCache.Storage as RIFAppendOnlyStorage;
					if (rIFAppendOnlyStorage != null)
					{
						IStreamHandler streamHandler = BuildChunkStreamHandler(chunkName, chunkType, odpContext.ChunkFactory, rIFAppendOnlyStorage.FromExistingStream);
						rIFAppendOnlyStorage.Reset(streamHandler);
					}
					scaleCache.PrepareForFlush();
				}
			}

			internal static void PreparePartitionedTreesForSyncSerialization(OnDemandProcessingContext odpContext)
			{
				OnDemandMetadata odpMetadata = odpContext.OdpMetadata;
				if (odpMetadata.GroupTreeScalabilityCache != null)
				{
					odpMetadata.GroupTreeScalabilityCache.PrepareForFlush();
				}
				if (odpMetadata.LookupScalabilityCache != null)
				{
					odpMetadata.LookupScalabilityCache.PrepareForFlush();
				}
			}

			internal static OnDemandMetadata DeserializeOnDemandMetadata(IChunkFactory chunkFactory, GlobalIDOwnerCollection globalIDOwnerCollection)
			{
				Stream stream = null;
				try
				{
					stream = chunkFactory.GetChunk("Metadata", Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Main, ChunkMode.Open, out string _);
					IntermediateFormatReader intermediateFormatReader = new IntermediateFormatReader(stream, default(GroupTreeRIFObjectCreator), globalIDOwnerCollection);
					OnDemandMetadata onDemandMetadata = (OnDemandMetadata)intermediateFormatReader.ReadRIFObject();
					Global.Tracer.Assert(onDemandMetadata != null, "(null != odpMetadata)");
					stream.Close();
					stream = null;
					onDemandMetadata.OdpChunkManager = new OnDemandProcessingManager();
					return onDemandMetadata;
				}
				finally
				{
					stream?.Close();
				}
			}

			internal void SerializeSnapshot()
			{
				Global.Tracer.Assert(m_odpContext != null, "OnDemandProcessingContext is unavailable");
				OnDemandMetadata odpMetadata = m_odpContext.OdpMetadata;
				if (!odpMetadata.SnapshotHasChanged)
				{
					return;
				}
				try
				{
					IReference<ReportInstance> reportInstance = odpMetadata.ReportInstance;
					Global.Tracer.Assert(reportInstance != null, "Missing GroupTreeRoot");
					if (odpMetadata.IsInitialProcessingRequest)
					{
						reportInstance.UnPinValue();
					}
					if (odpMetadata.GroupTreeHasChanged || odpMetadata.IsInitialProcessingRequest)
					{
						GroupTreeScalabilityCache groupTreeScalabilityCache = m_odpContext.OdpMetadata.GroupTreeScalabilityCache;
						groupTreeScalabilityCache.Flush();
						if (odpMetadata.IsInitialProcessingRequest)
						{
							GroupTreePartition groupTreePartition = new GroupTreePartition();
							groupTreePartition.AddTopLevelScopeInstance((IReference<ScopeInstance>)reportInstance);
							long groupTreeRootOffset = groupTreeScalabilityCache.Storage.Allocate(groupTreePartition);
							groupTreeScalabilityCache.Storage.Flush();
							odpMetadata.GroupTreeRootOffset = groupTreeRootOffset;
						}
					}
					if (odpMetadata.LookupInfoHasChanged)
					{
						m_odpContext.OdpMetadata.LookupScalabilityCache.Flush();
					}
					SerializeMetadata(m_odpContext.ChunkFactory, m_odpContext.OdpMetadata, m_odpContext.GetActiveCompatibilityVersion(), m_odpContext.ProhibitSerializableValues);
					SerializeSortFilterEventInfo(m_odpContext);
				}
				finally
				{
					odpMetadata?.DisposePersistedTreeScalability();
				}
			}

			internal static void SerializeMetadata(IChunkFactory chunkFactory, OnDemandMetadata odpMetadata, int compatVersion, bool prohibitSerializableValues)
			{
				odpMetadata.UpdateLastAssignedGlobalID();
				using (Stream str = chunkFactory.CreateChunk("Metadata", Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Main, null))
				{
					new IntermediateFormatWriter(str, compatVersion, prohibitSerializableValues).Write(odpMetadata);
				}
			}

			private static void SerializeSortFilterEventInfo(OnDemandProcessingContext odpContext)
			{
				ReportSnapshot reportSnapshot = odpContext.OdpMetadata.ReportSnapshot;
				if (reportSnapshot != null && reportSnapshot.SortFilterEventInfo != null)
				{
					Stream stream = null;
					try
					{
						stream = odpContext.ChunkFactory.GetChunk("SortFilterEventInfo", Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Interactivity, ChunkMode.OpenOrCreate, out string _);
						stream.Seek(0L, SeekOrigin.End);
						new IntermediateFormatWriter(stream, odpContext.GetActiveCompatibilityVersion(), odpContext.ProhibitSerializableValues).Write(reportSnapshot.SortFilterEventInfo);
						reportSnapshot.SortFilterEventInfo = null;
					}
					finally
					{
						stream?.Close();
					}
				}
			}

			internal static SortFilterEventInfoMap DeserializeSortFilterEventInfo(IChunkFactory originalSnapshotChunks, GlobalIDOwnerCollection globalIDOwnerCollection)
			{
				Stream stream = null;
				SortFilterEventInfoMap sortFilterEventInfoMap = null;
				try
				{
					stream = originalSnapshotChunks.GetChunk("SortFilterEventInfo", Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Interactivity, ChunkMode.Open, out string _);
					if (stream != null)
					{
						IntermediateFormatReader intermediateFormatReader;
						do
						{
							intermediateFormatReader = new IntermediateFormatReader(stream, new ProcessingRIFObjectCreator(null, null), globalIDOwnerCollection);
							SortFilterEventInfoMap sortFilterEventInfoMap2 = (SortFilterEventInfoMap)intermediateFormatReader.ReadRIFObject();
							Global.Tracer.Assert(sortFilterEventInfoMap2 != null, "(null != newInfo)");
							if (sortFilterEventInfoMap == null)
							{
								sortFilterEventInfoMap = sortFilterEventInfoMap2;
							}
							else
							{
								sortFilterEventInfoMap.Merge(sortFilterEventInfoMap2);
							}
						}
						while (!intermediateFormatReader.EOS);
						return sortFilterEventInfoMap;
					}
					return sortFilterEventInfoMap;
				}
				finally
				{
					stream?.Close();
				}
			}

			internal static List<Declaration> GetChunkDeclarations()
			{
				if (m_ChunkDeclarations == null)
				{
					return new List<Declaration>(21)
					{
						ScopeInstance.GetDeclaration(),
						ReportInstance.GetDeclaration(),
						DataSetInstance.GetDeclaration(),
						DataRegionInstance.GetDeclaration(),
						DataRegionMemberInstance.GetDeclaration(),
						DataCellInstance.GetDeclaration(),
						DataAggregateObjResult.GetDeclaration(),
						SubReportInstance.GetDeclaration(),
						GroupTreePartition.GetDeclaration(),
						ReportSnapshot.GetDeclaration(),
						ParametersImplWrapper.GetDeclaration(),
						ParameterImplWrapper.GetDeclaration(),
						SubReportInfo.GetDeclaration(),
						ParameterInfo.GetNewDeclaration(),
						ParameterInfoCollection.GetDeclaration(),
						ParameterBase.GetNewDeclaration(),
						ValidValue.GetNewDeclaration(),
						FieldInfo.GetDeclaration(),
						TreePartitionManager.GetDeclaration(),
						LookupObjResult.GetDeclaration(),
						DataCellInstanceList.GetDeclaration()
					};
				}
				return m_ChunkDeclarations;
			}

			internal static Stream OpenExistingDocumentMapStream(OnDemandMetadata odpMetadata, ICatalogItemContext reportContext, IChunkFactory chunkFactory)
			{
				Stream stream = null;
				if (!odpMetadata.ReportSnapshot.CanUseExistingDocumentMapChunk(reportContext))
				{
					return null;
				}
				string mimeType;
				return chunkFactory.GetChunk("DocumentMap", Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Interactivity, ChunkMode.Open, out mimeType);
			}
		}

		internal const string Definition = "CompiledDefinition";

		internal const string DocumentMap = "DocumentMap";

		internal const string ShowHideInfo = "ShowHideInfo";

		internal const string Bookmarks = "Bookmarks";

		internal const string Drillthrough = "Drillthrough";

		internal const string QuickFind = "QuickFind";

		internal const string SortFilterEventInfo = "SortFilterEventInfo";

		internal const string DataChunkPrefix = "DataChunk";

		internal const string GroupTree = "GroupTree";

		internal const string LookupInfo = "LookupInfo";

		internal const string Metadata = "Metadata";

		internal const string SharedDataSet = "SharedDataSet";

		internal const char Delimiter = 'x';

		internal static string GenerateDataChunkName(OnDemandProcessingContext context, int dataSetID, bool isInSubReport)
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			stringBuilder.Append("DataChunk");
			stringBuilder.Append('x');
			if (isInSubReport)
			{
				stringBuilder.Append(context.SubReportUniqueName);
				stringBuilder.Append('x');
				stringBuilder.Append(context.SubReportDataChunkNameModifier);
				stringBuilder.Append('x');
			}
			stringBuilder.Append(dataSetID.ToString(CultureInfo.InvariantCulture));
			return stringBuilder.ToString();
		}

		internal static string GenerateLegacySharedSubReportDataChunkName(OnDemandProcessingContext context, int dataSetID)
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			stringBuilder.Append("DataChunk");
			stringBuilder.Append('x');
			stringBuilder.Append(context.SubReportUniqueName);
			stringBuilder.Append('x');
			stringBuilder.Append(dataSetID.ToString(CultureInfo.InvariantCulture));
			return stringBuilder.ToString();
		}

		private static string GenerateDataChunkName(DataSetInstance dataSetInstance, OnDemandProcessingContext context)
		{
			string text = null;
			DataSet dataSetDef = dataSetInstance.DataSetDef;
			if (context.InSubreport)
			{
				return GenerateDataChunkName(context, dataSetDef.ID, isInSubReport: true);
			}
			return GenerateDataChunkName(null, dataSetDef.ID, isInSubReport: false);
		}

		internal static void SerializeReport(Report report, Stream stream, IConfiguration configuration)
		{
			int compatibilityVersion = ReportProcessingCompatibilityVersion.GetCompatibilityVersion(configuration);
			new IntermediateFormatWriter(stream, compatibilityVersion).Write(report);
		}

		internal static Report DeserializeReport(bool keepReferences, GlobalIDOwnerCollection globalIDOwnerCollection, IDOwner parentIDOwner, ReportItem parentReportItem, Stream stream)
		{
			IntermediateFormatReader intermediateFormatReader = new IntermediateFormatReader(stream, new ProcessingRIFObjectCreator(parentIDOwner, parentReportItem), globalIDOwnerCollection);
			Report obj = (Report)intermediateFormatReader.ReadRIFObject();
			obj.ReportOrDescendentHasUserSortFilter = obj.HasUserSortFilter;
			if (!keepReferences)
			{
				intermediateFormatReader.ClearReferences();
			}
			return obj;
		}
	}
}
