using Microsoft.Cloud.Platform.Utils;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal class OnDemandMetadata : IReportInstanceContainer, IPersistable
	{
		private Dictionary<string, SubReportInfo> m_subReportInfoMap;

		private Dictionary<string, CommonSubReportInfo> m_commonSubReportInfoMap;

		private Microsoft.ReportingServices.ReportIntermediateFormat.ReportSnapshot m_reportSnapshot;

		private Dictionary<string, DataSetInstance> m_dataChunkMap = new Dictionary<string, DataSetInstance>();

		private Dictionary<string, bool[]> m_tablixProcessingComplete;

		private Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.ImageInfo> m_cachedExternalImages;

		private Dictionary<string, ShapefileInfo> m_cachedShapefiles;

		private string m_transparentImageChunkName;

		private long m_groupTreeRootOffset = TreePartitionManager.EmptyTreePartitionOffset;

		private TreePartitionManager m_groupTreePartitions;

		private TreePartitionManager m_lookupPartitions;

		private int m_lastAssignedGlobalID = -1;

		private Dictionary<string, UpdatedVariableValues> m_updatedVariableValues;

		[NonSerialized]
		private IReference<Microsoft.ReportingServices.ReportIntermediateFormat.ReportInstance> m_reportInstance;

		[NonSerialized]
		private Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager m_odpChunkManager;

		[NonSerialized]
		private Microsoft.ReportingServices.ReportIntermediateFormat.Report m_report;

		[NonSerialized]
		private bool m_isInitialProcessingRequest;

		[NonSerialized]
		private bool m_metaDataChanged;

		[NonSerialized]
		private List<OnDemandProcessingContext> m_odpContexts = new List<OnDemandProcessingContext>();

		[NonSerialized]
		private GroupTreeScalabilityCache m_groupTreeScalabilityCache;

		[NonSerialized]
		private LookupScalabilityCache m_lookupScalabilityCache;

		[NonSerialized]
		private GlobalIDOwnerCollection m_globalIDOwnerCollection;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal bool IsInitialProcessingRequest => m_isInitialProcessingRequest;

		internal bool GroupTreeHasChanged
		{
			get
			{
				if (m_groupTreePartitions != null)
				{
					return m_groupTreePartitions.TreeHasChanged;
				}
				return false;
			}
			set
			{
				GroupTreePartitionManager.TreeHasChanged = value;
			}
		}

		internal bool LookupInfoHasChanged
		{
			get
			{
				if (m_lookupPartitions != null)
				{
					return m_lookupPartitions.TreeHasChanged;
				}
				return false;
			}
			set
			{
				LookupPartitionManager.TreeHasChanged = value;
			}
		}

		internal bool SnapshotHasChanged
		{
			get
			{
				if (!GroupTreeHasChanged && !LookupInfoHasChanged && !m_metaDataChanged)
				{
					return m_reportSnapshot.CachedDataChanged;
				}
				return true;
			}
		}

		internal bool MetadataHasChanged
		{
			get
			{
				return m_metaDataChanged;
			}
			set
			{
				m_metaDataChanged = value;
			}
		}

		internal TreePartitionManager GroupTreePartitionManager
		{
			get
			{
				if (m_groupTreePartitions == null)
				{
					m_groupTreePartitions = new TreePartitionManager();
					m_groupTreePartitions.TreeHasChanged = true;
				}
				return m_groupTreePartitions;
			}
		}

		internal TreePartitionManager LookupPartitionManager
		{
			get
			{
				if (m_lookupPartitions == null)
				{
					m_lookupPartitions = new TreePartitionManager();
					m_lookupPartitions.TreeHasChanged = true;
				}
				return m_lookupPartitions;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.Report Report
		{
			get
			{
				return m_report;
			}
			set
			{
				m_report = value;
			}
		}

		public IReference<Microsoft.ReportingServices.ReportIntermediateFormat.ReportInstance> ReportInstance
		{
			get
			{
				return m_reportInstance;
			}
			set
			{
				m_reportInstance = value;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ReportSnapshot ReportSnapshot
		{
			get
			{
				return m_reportSnapshot;
			}
			set
			{
				m_reportSnapshot = value;
				m_metaDataChanged = true;
			}
		}

		internal Dictionary<string, DataSetInstance> DataChunkMap => m_dataChunkMap;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager OdpChunkManager
		{
			get
			{
				return m_odpChunkManager;
			}
			set
			{
				m_odpChunkManager = value;
			}
		}

		internal List<OnDemandProcessingContext> OdpContexts => m_odpContexts;

		internal string TransparentImageChunkName
		{
			get
			{
				return m_transparentImageChunkName;
			}
			set
			{
				m_transparentImageChunkName = value;
				m_metaDataChanged = true;
			}
		}

		internal GroupTreeScalabilityCache GroupTreeScalabilityCache
		{
			get
			{
				return m_groupTreeScalabilityCache;
			}
			set
			{
				m_groupTreeScalabilityCache = value;
			}
		}

		internal LookupScalabilityCache LookupScalabilityCache
		{
			get
			{
				return m_lookupScalabilityCache;
			}
			set
			{
				m_lookupScalabilityCache = value;
			}
		}

		internal GlobalIDOwnerCollection GlobalIDOwnerCollection
		{
			get
			{
				return m_globalIDOwnerCollection;
			}
			set
			{
				m_globalIDOwnerCollection = value;
			}
		}

		internal long GroupTreeRootOffset
		{
			get
			{
				return m_groupTreeRootOffset;
			}
			set
			{
				m_groupTreeRootOffset = value;
				m_metaDataChanged = true;
			}
		}

		internal int LastAssignedGlobalID => m_lastAssignedGlobalID;

		internal OnDemandMetadata()
		{
			m_isInitialProcessingRequest = false;
			m_metaDataChanged = false;
		}

		internal OnDemandMetadata(Microsoft.ReportingServices.ReportIntermediateFormat.Report report)
		{
			m_report = report;
			m_odpChunkManager = new Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager();
			m_isInitialProcessingRequest = true;
			m_metaDataChanged = true;
			m_tablixProcessingComplete = new Dictionary<string, bool[]>();
		}

		internal OnDemandMetadata(OnDemandMetadata metadataFromOldSnapshot, Microsoft.ReportingServices.ReportIntermediateFormat.Report report)
		{
			m_isInitialProcessingRequest = true;
			m_metaDataChanged = true;
			m_report = report;
			m_odpChunkManager = metadataFromOldSnapshot.m_odpChunkManager;
			m_subReportInfoMap = metadataFromOldSnapshot.m_subReportInfoMap;
			m_commonSubReportInfoMap = metadataFromOldSnapshot.m_commonSubReportInfoMap;
			m_dataChunkMap = metadataFromOldSnapshot.m_dataChunkMap;
			m_lastAssignedGlobalID = metadataFromOldSnapshot.m_lastAssignedGlobalID;
			CommonPrepareForReprocessing();
		}

		public IReference<Microsoft.ReportingServices.ReportIntermediateFormat.ReportInstance> SetReportInstance(Microsoft.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance, OnDemandMetadata odpMetadata)
		{
			m_reportInstance = m_groupTreeScalabilityCache.AllocateAndPin(reportInstance, 0);
			return m_reportInstance;
		}

		internal void ResetUserSortFilterContexts()
		{
			foreach (OnDemandProcessingContext odpContext in m_odpContexts)
			{
				odpContext.ResetUserSortFilterContext();
			}
		}

		internal void UpdateLastAssignedGlobalID()
		{
			if (m_globalIDOwnerCollection != null)
			{
				int lastAssignedID = m_globalIDOwnerCollection.LastAssignedID;
				if (lastAssignedID > m_lastAssignedGlobalID)
				{
					m_lastAssignedGlobalID = lastAssignedID;
					m_metaDataChanged = true;
				}
			}
		}

		private void CommonPrepareForReprocessing()
		{
			m_tablixProcessingComplete = new Dictionary<string, bool[]>();
			if (m_dataChunkMap == null)
			{
				return;
			}
			foreach (DataSetInstance value in m_dataChunkMap.Values)
			{
				value.InitializeForReprocessing();
			}
		}

		internal void PrepareForCachedDataProcessing(OnDemandMetadata odpMetadata)
		{
			m_subReportInfoMap = odpMetadata.m_subReportInfoMap;
			m_commonSubReportInfoMap = odpMetadata.m_commonSubReportInfoMap;
			m_dataChunkMap = odpMetadata.m_dataChunkMap;
			CommonPrepareForReprocessing();
		}

		internal bool IsTablixProcessingComplete(OnDemandProcessingContext odpContext, int dataSetIndexInCollection)
		{
			if (m_tablixProcessingComplete == null)
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet = odpContext.ReportDefinition.MappingDataSetIndexToDataSet[dataSetIndexInCollection];
				return odpContext.GetDataSetInstance(dataSet)?.OldSnapshotTablixProcessingComplete ?? false;
			}
			if (m_tablixProcessingComplete.TryGetValue(GetUniqueIdFromContext(odpContext), out bool[] value))
			{
				return value[dataSetIndexInCollection];
			}
			return false;
		}

		internal void SetTablixProcessingComplete(OnDemandProcessingContext odpContext, int dataSetIndexInCollection)
		{
			if (m_tablixProcessingComplete == null)
			{
				m_tablixProcessingComplete = new Dictionary<string, bool[]>();
			}
			string uniqueIdFromContext = GetUniqueIdFromContext(odpContext);
			if (!m_tablixProcessingComplete.TryGetValue(uniqueIdFromContext, out bool[] value))
			{
				value = new bool[odpContext.ReportDefinition.DataSetCount];
				m_tablixProcessingComplete[uniqueIdFromContext] = value;
			}
			value[dataSetIndexInCollection] = true;
			m_metaDataChanged = true;
		}

		private string GetUniqueIdFromContext(OnDemandProcessingContext odpContext)
		{
			if (odpContext.InSubreport)
			{
				string processingAbortItemUniqueIdentifier = odpContext.ProcessingAbortItemUniqueIdentifier;
				Global.Tracer.Assert(!string.IsNullOrEmpty(processingAbortItemUniqueIdentifier), "Subreport ID must not be null or empty");
				return processingAbortItemUniqueIdentifier;
			}
			return string.Empty;
		}

		internal void DisposePersistedTreeScalability()
		{
			if (m_groupTreeScalabilityCache != null)
			{
				m_groupTreeScalabilityCache.Dispose();
				m_groupTreeScalabilityCache = null;
			}
			if (m_lookupScalabilityCache != null)
			{
				m_lookupScalabilityCache.Dispose();
				m_lookupScalabilityCache = null;
			}
		}

		internal void EnsureLookupScalabilitySetup(IChunkFactory chunkFactory, int rifCompatVersion, bool prohibitSerializableValues)
		{
			if (m_lookupScalabilityCache == null)
			{
				bool openExisting = m_lookupPartitions != null;
				Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager.EnsureLookupStorageSetup(this, chunkFactory, openExisting, rifCompatVersion, prohibitSerializableValues);
			}
		}

		internal SubReportInfo AddSubReportInfo(bool isTopLevelSubreport, string definitionPath, string reportPath, string originalCatalogReportPath)
		{
			m_metaDataChanged = true;
			if (m_subReportInfoMap == null)
			{
				m_subReportInfoMap = new Dictionary<string, SubReportInfo>(EqualityComparers.StringComparerInstance);
			}
			Global.Tracer.Assert(!m_subReportInfoMap.ContainsKey(definitionPath), "(!m_subReportInfoMap.ContainsKey(definitionPath))");
			SubReportInfo subReportInfo = new SubReportInfo(Guid.NewGuid());
			m_subReportInfoMap.Add(definitionPath, subReportInfo);
			string reportPath2 = isTopLevelSubreport ? reportPath : (definitionPath + "_" + reportPath);
			subReportInfo.CommonSubReportInfo = GetOrCreateCommonSubReportInfo(reportPath2, out bool created);
			if (created)
			{
				subReportInfo.CommonSubReportInfo.DefinitionUniqueName = subReportInfo.UniqueName;
				subReportInfo.CommonSubReportInfo.OriginalCatalogPath = originalCatalogReportPath;
			}
			return subReportInfo;
		}

		private CommonSubReportInfo GetOrCreateCommonSubReportInfo(string reportPath, out bool created)
		{
			created = false;
			if (m_commonSubReportInfoMap == null)
			{
				m_commonSubReportInfoMap = new Dictionary<string, CommonSubReportInfo>(EqualityComparers.StringComparerInstance);
			}
			if (!m_commonSubReportInfoMap.TryGetValue(reportPath, out CommonSubReportInfo value))
			{
				created = true;
				value = new CommonSubReportInfo();
				value.ReportPath = reportPath;
				m_commonSubReportInfoMap.Add(reportPath, value);
			}
			return value;
		}

		internal bool TryGetSubReportInfo(bool isTopLevelSubreport, string definitionPath, string reportPath, out SubReportInfo subReportInfo)
		{
			subReportInfo = null;
			if (m_subReportInfoMap != null && m_subReportInfoMap.TryGetValue(definitionPath, out subReportInfo))
			{
				if (subReportInfo.CommonSubReportInfo == null)
				{
					string key = isTopLevelSubreport ? reportPath : (definitionPath + "_" + reportPath);
					if (m_commonSubReportInfoMap == null)
					{
						subReportInfo = null;
						return false;
					}
					if (m_commonSubReportInfoMap.TryGetValue(key, out CommonSubReportInfo value))
					{
						subReportInfo.CommonSubReportInfo = value;
						return true;
					}
					int length = reportPath.Length;
					foreach (string key2 in m_commonSubReportInfoMap.Keys)
					{
						if (key2.Length >= length)
						{
							int num = key2.LastIndexOf(reportPath, StringComparison.OrdinalIgnoreCase);
							if (num >= 0 && num + length == key2.Length)
							{
								subReportInfo.CommonSubReportInfo = m_commonSubReportInfoMap[key2];
								return true;
							}
						}
					}
				}
				return true;
			}
			return false;
		}

		internal SubReportInfo GetSubReportInfo(bool isTopLevelSubreport, string definitionPath, string reportPath)
		{
			SubReportInfo subReportInfo = null;
			bool condition = TryGetSubReportInfo(isTopLevelSubreport, definitionPath, reportPath, out subReportInfo);
			Global.Tracer.Assert(condition, "Missing expected SubReportInfo: {0}_{1}", definitionPath, reportPath.MarkAsPrivate());
			return subReportInfo;
		}

		internal void AddDataChunk(string dataSetChunkName, DataSetInstance dataSetInstance)
		{
			m_metaDataChanged = true;
			dataSetInstance.DataChunkName = dataSetChunkName;
			lock (m_dataChunkMap)
			{
				m_dataChunkMap.Add(dataSetChunkName, dataSetInstance);
			}
		}

		internal void DeleteDataChunk(string dataSetChunkName)
		{
			m_metaDataChanged = true;
			lock (m_dataChunkMap)
			{
				m_dataChunkMap.Remove(dataSetChunkName);
			}
		}

		internal void AddExternalImage(string value, Microsoft.ReportingServices.ReportIntermediateFormat.ImageInfo imageInfo)
		{
			m_metaDataChanged = true;
			if (m_cachedExternalImages == null)
			{
				m_cachedExternalImages = new Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.ImageInfo>(EqualityComparers.StringComparerInstance);
			}
			m_cachedExternalImages.Add(value, imageInfo);
		}

		internal bool TryGetExternalImage(string value, out Microsoft.ReportingServices.ReportIntermediateFormat.ImageInfo imageInfo)
		{
			if (m_cachedExternalImages != null)
			{
				return m_cachedExternalImages.TryGetValue(value, out imageInfo);
			}
			imageInfo = null;
			return false;
		}

		internal void AddShapefile(string value, ShapefileInfo shapefileInfo)
		{
			m_metaDataChanged = true;
			if (m_cachedShapefiles == null)
			{
				m_cachedShapefiles = new Dictionary<string, ShapefileInfo>(EqualityComparers.StringComparerInstance);
			}
			m_cachedShapefiles.Add(value, shapefileInfo);
		}

		internal bool TryGetShapefile(string value, out ShapefileInfo shapefileInfo)
		{
			if (m_cachedShapefiles != null)
			{
				return m_cachedShapefiles.TryGetValue(value, out shapefileInfo);
			}
			shapefileInfo = null;
			return false;
		}

		internal bool StoreUpdatedVariableValue(OnDemandProcessingContext odpContext, Microsoft.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance, int index, object value)
		{
			m_metaDataChanged = true;
			if (m_updatedVariableValues == null)
			{
				m_updatedVariableValues = new Dictionary<string, UpdatedVariableValues>();
			}
			string key = odpContext.SubReportUniqueName ?? "Report";
			Dictionary<int, object> dictionary;
			if (m_updatedVariableValues.TryGetValue(key, out UpdatedVariableValues value2))
			{
				dictionary = value2.VariableValues;
			}
			else
			{
				dictionary = new Dictionary<int, object>();
				value2 = new UpdatedVariableValues();
				value2.VariableValues = dictionary;
				m_updatedVariableValues.Add(key, value2);
			}
			if (reportInstance != null && reportInstance.VariableValues != null)
			{
				reportInstance.VariableValues[index] = value;
			}
			dictionary[index] = value;
			return true;
		}

		internal void SetUpdatedVariableValues(OnDemandProcessingContext odpContext, Microsoft.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance)
		{
			if (m_updatedVariableValues == null)
			{
				return;
			}
			string key = odpContext.SubReportUniqueName ?? "Report";
			if (!m_updatedVariableValues.TryGetValue(key, out UpdatedVariableValues value))
			{
				return;
			}
			Dictionary<int, object> variableValues = value.VariableValues;
			List<Variable> variables = odpContext.ReportDefinition.Variables;
			foreach (KeyValuePair<int, object> item in variableValues)
			{
				reportInstance.VariableValues[item.Key] = item.Value;
				variables[item.Key].GetCachedVariableObj(odpContext).SetValue(item.Value, internalSet: true);
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.CommonSubReportInfos, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StringRIFObjectDictionary, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.CommonSubReportInfo));
			list.Add(new MemberInfo(MemberName.SubReportInfos, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StringRIFObjectDictionary, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SubReportInfo));
			list.Add(new MemberInfo(MemberName.ReportSnapshot, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportSnapshot));
			list.Add(new ReadOnlyMemberInfo(MemberName.GroupTreePartitionOffsets, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Int64));
			list.Add(new MemberInfo(MemberName.DataChunkMap, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StringRIFObjectDictionary, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSetInstance));
			list.Add(new MemberInfo(MemberName.CachedExternalImages, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StringRIFObjectDictionary, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ImageInfo));
			list.Add(new MemberInfo(MemberName.TransparentImageChunkName, Token.String));
			list.Add(new MemberInfo(MemberName.GroupTreeRootOffset, Token.Int64));
			list.Add(new MemberInfo(MemberName.TablixProcessingComplete, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StringBoolArrayDictionary, Token.Boolean));
			list.Add(new MemberInfo(MemberName.GroupTreePartitions, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TreePartitionManager));
			list.Add(new MemberInfo(MemberName.LookupPartitions, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TreePartitionManager));
			list.Add(new MemberInfo(MemberName.LastAssignedGlobalID, Token.Int32));
			list.Add(new MemberInfo(MemberName.CachedShapefiles, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StringRIFObjectDictionary, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ShapefileInfo));
			list.Add(new MemberInfo(MemberName.UpdatedVariableValues, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StringRIFObjectDictionary, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.UpdatedVariableValues));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.OnDemandMetadata, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.CommonSubReportInfos:
					writer.WriteStringRIFObjectDictionary(m_commonSubReportInfoMap);
					break;
				case MemberName.SubReportInfos:
					writer.WriteStringRIFObjectDictionary(m_subReportInfoMap);
					break;
				case MemberName.ReportSnapshot:
					writer.Write(m_reportSnapshot);
					break;
				case MemberName.DataChunkMap:
					writer.WriteStringRIFObjectDictionary(m_dataChunkMap);
					break;
				case MemberName.CachedExternalImages:
					writer.WriteStringRIFObjectDictionary(m_cachedExternalImages);
					break;
				case MemberName.CachedShapefiles:
					writer.WriteStringRIFObjectDictionary(m_cachedShapefiles);
					break;
				case MemberName.TransparentImageChunkName:
					writer.Write(m_transparentImageChunkName);
					break;
				case MemberName.GroupTreeRootOffset:
					writer.Write(m_groupTreeRootOffset);
					break;
				case MemberName.TablixProcessingComplete:
					writer.WriteStringBoolArrayDictionary(m_tablixProcessingComplete);
					break;
				case MemberName.GroupTreePartitions:
					writer.Write(m_groupTreePartitions);
					break;
				case MemberName.LookupPartitions:
					writer.Write(m_lookupPartitions);
					break;
				case MemberName.LastAssignedGlobalID:
					writer.Write(m_lastAssignedGlobalID);
					break;
				case MemberName.UpdatedVariableValues:
					writer.WriteStringRIFObjectDictionary(m_updatedVariableValues);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.CommonSubReportInfos:
					m_commonSubReportInfoMap = reader.ReadStringRIFObjectDictionary<CommonSubReportInfo>();
					break;
				case MemberName.SubReportInfos:
					m_subReportInfoMap = reader.ReadStringRIFObjectDictionary<SubReportInfo>();
					break;
				case MemberName.ReportSnapshot:
					m_reportSnapshot = (Microsoft.ReportingServices.ReportIntermediateFormat.ReportSnapshot)reader.ReadRIFObject();
					break;
				case MemberName.GroupTreePartitionOffsets:
				{
					List<long> list = reader.ReadListOfPrimitives<long>();
					if (list != null)
					{
						m_groupTreePartitions = new TreePartitionManager(list);
					}
					break;
				}
				case MemberName.DataChunkMap:
					m_dataChunkMap = reader.ReadStringRIFObjectDictionary<DataSetInstance>();
					break;
				case MemberName.CachedExternalImages:
					m_cachedExternalImages = reader.ReadStringRIFObjectDictionary<Microsoft.ReportingServices.ReportIntermediateFormat.ImageInfo>();
					break;
				case MemberName.CachedShapefiles:
					m_cachedShapefiles = reader.ReadStringRIFObjectDictionary<ShapefileInfo>();
					break;
				case MemberName.TransparentImageChunkName:
					m_transparentImageChunkName = reader.ReadString();
					break;
				case MemberName.GroupTreeRootOffset:
					m_groupTreeRootOffset = reader.ReadInt64();
					break;
				case MemberName.TablixProcessingComplete:
					m_tablixProcessingComplete = reader.ReadStringBoolArrayDictionary();
					break;
				case MemberName.GroupTreePartitions:
					m_groupTreePartitions = (TreePartitionManager)reader.ReadRIFObject();
					break;
				case MemberName.LookupPartitions:
					m_lookupPartitions = (TreePartitionManager)reader.ReadRIFObject();
					break;
				case MemberName.LastAssignedGlobalID:
					m_lastAssignedGlobalID = reader.ReadInt32();
					break;
				case MemberName.UpdatedVariableValues:
					m_updatedVariableValues = reader.ReadStringRIFObjectDictionary<UpdatedVariableValues>();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public virtual void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		public virtual Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.OnDemandMetadata;
		}
	}
}
