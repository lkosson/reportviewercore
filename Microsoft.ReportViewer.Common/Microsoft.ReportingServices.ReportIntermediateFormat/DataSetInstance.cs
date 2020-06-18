using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal sealed class DataSetInstance : ScopeInstance
	{
		private int m_recordSetSize = -1;

		private string m_rewrittenCommandText;

		private string m_commandText;

		private DateTime m_executionTime = DateTime.MinValue;

		private FieldInfo[] m_fieldInfos;

		private uint m_lcid;

		private DataSet.TriState m_caseSensitivity;

		private DataSet.TriState m_accentSensitivity;

		private DataSet.TriState m_kanatypeSensitivity;

		private DataSet.TriState m_widthSensitivity;

		[NonSerialized]
		private bool m_oldSnapshotTablixProcessingComplete;

		private string m_dataChunkName;

		private List<LookupObjResult> m_lookupResults;

		[NonSerialized]
		private CompareInfo m_cachedCompareInfo;

		[NonSerialized]
		private CompareOptions m_cachedCompareOptions;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private DataSet m_dataSetDef;

		internal override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType ObjectType => Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSetInstance;

		internal DataSet DataSetDef
		{
			get
			{
				return m_dataSetDef;
			}
			set
			{
				m_dataSetDef = value;
			}
		}

		internal int RecordSetSize
		{
			get
			{
				return m_recordSetSize;
			}
			set
			{
				m_recordSetSize = value;
			}
		}

		internal bool NoRows => m_recordSetSize <= 0;

		internal FieldInfo[] FieldInfos
		{
			get
			{
				return m_fieldInfos;
			}
			set
			{
				m_fieldInfos = value;
			}
		}

		internal string RewrittenCommandText
		{
			get
			{
				return m_rewrittenCommandText;
			}
			set
			{
				m_rewrittenCommandText = value;
			}
		}

		internal string CommandText
		{
			get
			{
				return m_commandText;
			}
			set
			{
				m_commandText = value;
			}
		}

		internal DataSet.TriState CaseSensitivity
		{
			get
			{
				return m_caseSensitivity;
			}
			set
			{
				m_caseSensitivity = value;
			}
		}

		internal DataSet.TriState AccentSensitivity
		{
			get
			{
				return m_accentSensitivity;
			}
			set
			{
				m_accentSensitivity = value;
			}
		}

		internal DataSet.TriState KanatypeSensitivity
		{
			get
			{
				return m_kanatypeSensitivity;
			}
			set
			{
				m_kanatypeSensitivity = value;
			}
		}

		internal DataSet.TriState WidthSensitivity
		{
			get
			{
				return m_widthSensitivity;
			}
			set
			{
				m_widthSensitivity = value;
			}
		}

		internal uint LCID
		{
			get
			{
				return m_lcid;
			}
			set
			{
				m_lcid = value;
			}
		}

		internal List<LookupObjResult> LookupResults
		{
			get
			{
				return m_lookupResults;
			}
			set
			{
				m_lookupResults = value;
			}
		}

		internal bool OldSnapshotTablixProcessingComplete
		{
			get
			{
				return m_oldSnapshotTablixProcessingComplete;
			}
			set
			{
				m_oldSnapshotTablixProcessingComplete = value;
			}
		}

		internal string DataChunkName
		{
			get
			{
				return m_dataChunkName;
			}
			set
			{
				m_dataChunkName = value;
			}
		}

		internal CompareInfo CompareInfo
		{
			get
			{
				if (m_cachedCompareInfo == null)
				{
					CreateCompareInfo();
				}
				return m_cachedCompareInfo;
			}
		}

		internal CompareOptions ClrCompareOptions
		{
			get
			{
				if (m_cachedCompareInfo == null)
				{
					CreateCompareInfo();
				}
				return m_cachedCompareOptions;
			}
		}

		internal DataSetInstance(DataSet dataSetDef)
		{
			m_dataSetDef = dataSetDef;
		}

		internal DataSetInstance()
		{
		}

		internal void InitializeForReprocessing()
		{
			m_oldSnapshotTablixProcessingComplete = false;
			m_aggregateValues = null;
			m_lookupResults = null;
			m_firstRowOffset = -1L;
		}

		internal override void AddChildScope(IReference<ScopeInstance> child, int indexInCollection)
		{
			Global.Tracer.Assert(condition: false);
		}

		internal void SetupEnvironment(OnDemandProcessingContext odpContext, bool newDataSetDefinition)
		{
			if (newDataSetDefinition)
			{
				odpContext.SetupFieldsForNewDataSet(m_dataSetDef, this, addRowIndex: false, NoRows);
			}
			if (!NoRows)
			{
				if (m_firstRowOffset == DataFieldRow.UnInitializedStreamOffset)
				{
					odpContext.ReportObjectModel.CreateNoRows();
					return;
				}
				Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DataChunkReader dataChunkReader = odpContext.GetDataChunkReader(m_dataSetDef.IndexInCollection);
				dataChunkReader.ReadOneRowAtPosition(m_firstRowOffset);
				odpContext.ReportObjectModel.FieldsImpl.NewRow(m_firstRowOffset);
				odpContext.ReportObjectModel.UpdateFieldValues(!newDataSetDefinition, dataChunkReader.RecordRow, this, dataChunkReader.ReaderExtensionsSupported);
			}
		}

		internal void SetupDataSetLevelAggregates(OnDemandProcessingContext odpContext)
		{
			int aggregateValueOffset = 0;
			SetupAggregates(odpContext, m_dataSetDef.Aggregates, ref aggregateValueOffset);
			SetupAggregates(odpContext, m_dataSetDef.PostSortAggregates, ref aggregateValueOffset);
		}

		internal void SetupCollationSettings(OnDemandProcessingContext odpContext)
		{
			odpContext.SetComparisonInformation(CompareInfo, ClrCompareOptions, m_dataSetDef.NullsAsBlanks, m_dataSetDef.UseOrdinalStringKeyGeneration);
		}

		internal void SaveCollationSettings(DataSet dataSet)
		{
			LCID = dataSet.LCID;
			CaseSensitivity = dataSet.CaseSensitivity;
			WidthSensitivity = dataSet.WidthSensitivity;
			AccentSensitivity = dataSet.AccentSensitivity;
			KanatypeSensitivity = dataSet.KanatypeSensitivity;
		}

		private void CreateCompareInfo()
		{
			if (m_dataSetDef.NeedAutoDetectCollation())
			{
				m_dataSetDef.LCID = m_lcid;
				m_dataSetDef.CaseSensitivity = m_caseSensitivity;
				m_dataSetDef.AccentSensitivity = m_accentSensitivity;
				m_dataSetDef.KanatypeSensitivity = m_kanatypeSensitivity;
				m_dataSetDef.WidthSensitivity = m_widthSensitivity;
			}
			m_cachedCompareInfo = m_dataSetDef.CreateCultureInfoFromLcid().CompareInfo;
			m_cachedCompareOptions = m_dataSetDef.GetCLRCompareOptions();
		}

		internal IDataComparer CreateProcessingComparer(OnDemandProcessingContext odpContext)
		{
			if (odpContext.ContextMode == OnDemandProcessingContext.Mode.Streaming)
			{
				return new CommonDataComparer(CompareInfo, ClrCompareOptions, m_dataSetDef.NullsAsBlanks);
			}
			return new Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ProcessingComparer(CompareInfo, ClrCompareOptions, m_dataSetDef.NullsAsBlanks);
		}

		internal DateTime GetQueryExecutionTime(DateTime reportExecutionTime)
		{
			if (!(m_executionTime == DateTime.MinValue))
			{
				return m_executionTime;
			}
			return reportExecutionTime;
		}

		internal void SetQueryExecutionTime(DateTime queryExecutionTime)
		{
			m_executionTime = queryExecutionTime;
		}

		internal FieldInfo GetOrCreateFieldInfo(int aIndex)
		{
			if (m_fieldInfos == null)
			{
				m_fieldInfos = new FieldInfo[m_dataSetDef.NonCalculatedFieldCount];
			}
			if (m_fieldInfos[aIndex] == null)
			{
				m_fieldInfos[aIndex] = new FieldInfo();
			}
			return m_fieldInfos[aIndex];
		}

		internal bool IsFieldMissing(int index)
		{
			if (m_fieldInfos == null || m_fieldInfos[index] == null)
			{
				return false;
			}
			return m_fieldInfos[index].Missing;
		}

		internal int GetFieldPropertyCount(int index)
		{
			if (m_fieldInfos == null || m_fieldInfos[index] == null)
			{
				return 0;
			}
			return m_fieldInfos[index].PropertyCount;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.RecordSetSize, Token.Int32));
			list.Add(new MemberInfo(MemberName.CommandText, Token.String));
			list.Add(new MemberInfo(MemberName.RewrittenCommandText, Token.String));
			list.Add(new MemberInfo(MemberName.Fields, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.FieldInfo));
			list.Add(new MemberInfo(MemberName.CaseSensitivity, Token.Enum));
			list.Add(new MemberInfo(MemberName.AccentSensitivity, Token.Enum));
			list.Add(new MemberInfo(MemberName.KanatypeSensitivity, Token.Enum));
			list.Add(new MemberInfo(MemberName.WidthSensitivity, Token.Enum));
			list.Add(new MemberInfo(MemberName.LCID, Token.UInt32));
			list.Add(new ReadOnlyMemberInfo(MemberName.TablixProcessingComplete, Token.Boolean));
			list.Add(new MemberInfo(MemberName.DataChunkName, Token.String));
			list.Add(new MemberInfo(MemberName.LookupResults, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupObjResult));
			list.Add(new MemberInfo(MemberName.ExecutionTime, Token.DateTime));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSetInstance, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScopeInstance, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.RecordSetSize:
					writer.Write(m_recordSetSize);
					break;
				case MemberName.CommandText:
					writer.Write(m_commandText);
					break;
				case MemberName.RewrittenCommandText:
					writer.Write(m_rewrittenCommandText);
					break;
				case MemberName.Fields:
					writer.Write(m_fieldInfos);
					break;
				case MemberName.CaseSensitivity:
					writer.WriteEnum((int)m_caseSensitivity);
					break;
				case MemberName.AccentSensitivity:
					writer.WriteEnum((int)m_accentSensitivity);
					break;
				case MemberName.KanatypeSensitivity:
					writer.WriteEnum((int)m_kanatypeSensitivity);
					break;
				case MemberName.WidthSensitivity:
					writer.WriteEnum((int)m_widthSensitivity);
					break;
				case MemberName.LCID:
					writer.Write(m_lcid);
					break;
				case MemberName.DataChunkName:
					writer.Write(m_dataChunkName);
					break;
				case MemberName.LookupResults:
					writer.Write(m_lookupResults);
					break;
				case MemberName.ExecutionTime:
					writer.Write(m_executionTime);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.RecordSetSize:
					m_recordSetSize = reader.ReadInt32();
					break;
				case MemberName.CommandText:
					m_commandText = reader.ReadString();
					break;
				case MemberName.RewrittenCommandText:
					m_rewrittenCommandText = reader.ReadString();
					break;
				case MemberName.Fields:
					m_fieldInfos = reader.ReadArrayOfRIFObjects<FieldInfo>();
					break;
				case MemberName.CaseSensitivity:
					m_caseSensitivity = (DataSet.TriState)reader.ReadEnum();
					break;
				case MemberName.AccentSensitivity:
					m_accentSensitivity = (DataSet.TriState)reader.ReadEnum();
					break;
				case MemberName.KanatypeSensitivity:
					m_kanatypeSensitivity = (DataSet.TriState)reader.ReadEnum();
					break;
				case MemberName.WidthSensitivity:
					m_widthSensitivity = (DataSet.TriState)reader.ReadEnum();
					break;
				case MemberName.LCID:
					m_lcid = reader.ReadUInt32();
					break;
				case MemberName.TablixProcessingComplete:
					m_oldSnapshotTablixProcessingComplete = reader.ReadBoolean();
					break;
				case MemberName.DataChunkName:
					m_dataChunkName = reader.ReadString();
					break;
				case MemberName.LookupResults:
					m_lookupResults = reader.ReadListOfRIFObjects<List<LookupObjResult>>();
					break;
				case MemberName.ExecutionTime:
					m_executionTime = reader.ReadDateTime();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSetInstance;
		}
	}
}
