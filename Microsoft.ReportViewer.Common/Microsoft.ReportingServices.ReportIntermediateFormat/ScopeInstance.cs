using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal abstract class ScopeInstance : IStorable, IPersistable
	{
		protected long m_firstRowOffset = DataFieldRow.UnInitializedStreamOffset;

		protected List<IReference<DataRegionInstance>> m_dataRegionInstances;

		protected List<IReference<SubReportInstance>> m_subReportInstances;

		protected List<DataAggregateObjResult> m_aggregateValues;

		[NonSerialized]
		private int m_serializationDataRegionIndexInCollection = -1;

		[NonSerialized]
		protected IDisposable m_cleanupRef;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal abstract Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType ObjectType
		{
			get;
		}

		internal virtual IRIFReportScope RIFReportScope => null;

		internal long FirstRowOffset
		{
			get
			{
				return m_firstRowOffset;
			}
			set
			{
				m_firstRowOffset = value;
			}
		}

		internal List<IReference<DataRegionInstance>> DataRegionInstances => m_dataRegionInstances;

		internal List<IReference<SubReportInstance>> SubreportInstances => m_subReportInstances;

		internal List<DataAggregateObjResult> AggregateValues => m_aggregateValues;

		internal bool IsReadOnly => m_cleanupRef == null;

		public virtual int Size => 8 + ItemSizes.SizeOf(m_dataRegionInstances) + ItemSizes.SizeOf(m_subReportInstances) + ItemSizes.SizeOf(m_aggregateValues) + 4;

		protected ScopeInstance()
		{
		}

		protected ScopeInstance(long firstRowOffset)
		{
			m_firstRowOffset = firstRowOffset;
		}

		internal virtual void InstanceComplete()
		{
			m_cleanupRef.Dispose();
			m_cleanupRef = null;
		}

		protected void UnPinList<T>(List<ScalableList<T>> listOfLists)
		{
			if (listOfLists != null)
			{
				int count = listOfLists.Count;
				for (int i = 0; i < count; i++)
				{
					listOfLists[i]?.UnPinAll();
				}
			}
		}

		protected void SetReadOnlyList<T>(List<ScalableList<T>> listOfLists)
		{
			if (listOfLists != null)
			{
				int count = listOfLists.Count;
				for (int i = 0; i < count; i++)
				{
					listOfLists[i]?.SetReadOnly();
				}
			}
		}

		protected static void AdjustLength<T>(ScalableList<T> instances, int indexInCollection) where T : class
		{
			for (int i = instances.Count; i <= indexInCollection; i++)
			{
				instances.Add(null);
			}
		}

		protected static IDisposable AddAndPinItemAt<T>(ScalableList<T> list, int index, T item) where T : class
		{
			for (int i = list.Count; i < index; i++)
			{
				list.Add(null);
			}
			return list.AddAndPin(item);
		}

		internal virtual void AddChildScope(IReference<ScopeInstance> childRef, int indexInCollection)
		{
			switch (childRef.Value().ObjectType)
			{
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegionInstance:
				if (m_dataRegionInstances == null)
				{
					m_dataRegionInstances = new List<IReference<DataRegionInstance>>();
				}
				ListUtils.AdjustLength(m_dataRegionInstances, indexInCollection);
				Global.Tracer.Assert(m_dataRegionInstances[indexInCollection] == null, "(null == m_dataRegionInstances[indexInCollection])");
				m_dataRegionInstances[indexInCollection] = (childRef as IReference<DataRegionInstance>);
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SubReportInstance:
				if (m_subReportInstances == null)
				{
					m_subReportInstances = new List<IReference<SubReportInstance>>();
				}
				ListUtils.AdjustLength(m_subReportInstances, indexInCollection);
				Global.Tracer.Assert(m_subReportInstances[indexInCollection] == null, "(null == m_subReportInstances[indexInCollection])");
				m_subReportInstances[indexInCollection] = (childRef as IReference<SubReportInstance>);
				break;
			default:
				Global.Tracer.Assert(condition: false, childRef.Value().ToString());
				break;
			}
		}

		internal void StoreAggregates<AggregateType>(OnDemandProcessingContext odpContext, List<AggregateType> aggregateDefs) where AggregateType : DataAggregateInfo
		{
			if (aggregateDefs != null)
			{
				int count = aggregateDefs.Count;
				if (m_aggregateValues == null)
				{
					m_aggregateValues = new List<DataAggregateObjResult>();
				}
				for (int i = 0; i < count; i++)
				{
					StoreAggregate(odpContext, aggregateDefs[i], ref m_aggregateValues);
				}
			}
		}

		internal void StoreAggregates<AggregateType>(OnDemandProcessingContext odpContext, BucketedAggregatesCollection<AggregateType> aggregateDefs) where AggregateType : DataAggregateInfo
		{
			if (aggregateDefs == null || aggregateDefs.IsEmpty)
			{
				return;
			}
			if (m_aggregateValues == null)
			{
				m_aggregateValues = new List<DataAggregateObjResult>();
			}
			foreach (AggregateType aggregateDef in aggregateDefs)
			{
				StoreAggregate(odpContext, aggregateDef, ref m_aggregateValues);
			}
		}

		internal void StoreAggregates(DataAggregateObjResult[] aggregateObjResults)
		{
			if (aggregateObjResults != null)
			{
				int num = aggregateObjResults.Length;
				if (m_aggregateValues == null)
				{
					m_aggregateValues = new List<DataAggregateObjResult>();
				}
				for (int i = 0; i < num; i++)
				{
					m_aggregateValues.Add(aggregateObjResults[i]);
				}
			}
		}

		internal void StoreAggregates<AggregateType>(OnDemandProcessingContext odpContext, List<AggregateType> aggregateDefs, List<int> aggregateIndexes) where AggregateType : DataAggregateInfo
		{
			if (aggregateIndexes != null)
			{
				int count = aggregateIndexes.Count;
				if (m_aggregateValues == null)
				{
					m_aggregateValues = new List<DataAggregateObjResult>();
				}
				for (int i = 0; i < count; i++)
				{
					int index = aggregateIndexes[i];
					StoreAggregate(odpContext, aggregateDefs[index], ref m_aggregateValues);
				}
			}
		}

		private static void StoreAggregate<AggregateType>(OnDemandProcessingContext odpContext, AggregateType aggregateDef, ref List<DataAggregateObjResult> aggregateValues) where AggregateType : DataAggregateInfo
		{
			DataAggregateObjResult item = odpContext.ReportObjectModel.AggregatesImpl.GetAggregateObj(aggregateDef.Name).AggregateResult();
			aggregateValues.Add(item);
		}

		protected static IList<DataRegionMemberInstance> GetChildMemberInstances(List<ScalableList<DataRegionMemberInstance>> members, int memberIndexInCollection)
		{
			if (members == null || members.Count <= memberIndexInCollection)
			{
				return null;
			}
			return members[memberIndexInCollection];
		}

		internal void SetupFields(OnDemandProcessingContext odpContext, int dataSetIndex)
		{
			DataSetInstance dataSetInstance = odpContext.CurrentReportInstance.GetDataSetInstance(dataSetIndex, odpContext);
			SetupFields(odpContext, dataSetInstance);
		}

		internal void SetupFields(OnDemandProcessingContext odpContext, DataSetInstance dataSetInstance)
		{
			if (!dataSetInstance.NoRows)
			{
				if (0 < m_firstRowOffset)
				{
					odpContext.ReportObjectModel.RegisterOnDemandFieldValueUpdate(m_firstRowOffset, dataSetInstance, odpContext.GetDataChunkReader(dataSetInstance.DataSetDef.IndexInCollection));
				}
				else
				{
					odpContext.ReportObjectModel.ResetFieldValues();
				}
			}
		}

		internal void SetupAggregates<AggregateType>(OnDemandProcessingContext odpContext, List<AggregateType> aggregateDefs, ref int aggregateValueOffset) where AggregateType : DataAggregateInfo
		{
			if (m_aggregateValues != null && aggregateDefs != null)
			{
				int count = aggregateDefs.Count;
				for (int i = 0; i < count; i++)
				{
					SetupAggregate(odpContext, aggregateDefs[i], m_aggregateValues[aggregateValueOffset]);
					aggregateValueOffset++;
				}
			}
		}

		internal void SetupAggregates<AggregateType>(OnDemandProcessingContext odpContext, BucketedAggregatesCollection<AggregateType> aggregateDefs, ref int aggregateValueOffset) where AggregateType : DataAggregateInfo
		{
			if (m_aggregateValues == null || aggregateDefs == null)
			{
				return;
			}
			foreach (AggregateType aggregateDef in aggregateDefs)
			{
				SetupAggregate(odpContext, aggregateDef, m_aggregateValues[aggregateValueOffset]);
				aggregateValueOffset++;
			}
		}

		internal void SetupAggregates<AggregateType>(OnDemandProcessingContext odpContext, List<AggregateType> aggregateDefs, List<int> aggregateIndexes, ref int aggregateValueOffset) where AggregateType : DataAggregateInfo
		{
			int num = aggregateIndexes?.Count ?? 0;
			for (int i = 0; i < num; i++)
			{
				int index = aggregateIndexes[i];
				SetupAggregate(odpContext, aggregateDefs[index], m_aggregateValues[aggregateValueOffset]);
				aggregateValueOffset++;
			}
		}

		private static void SetupAggregate<AggregateType>(OnDemandProcessingContext odpContext, AggregateType aggregateDef, DataAggregateObjResult aggregateObj) where AggregateType : DataAggregateInfo
		{
			odpContext.ReportObjectModel.AggregatesImpl.Set(aggregateDef.Name, aggregateDef, aggregateDef.DuplicateNames, aggregateObj);
		}

		internal static void CalculateVariables(OnDemandProcessingContext odpContext, List<Variable> variableDefs, out object[] variableValues)
		{
			variableValues = null;
			if (variableDefs != null && variableDefs.Count != 0)
			{
				int count = variableDefs.Count;
				variableValues = new object[count];
				for (int i = 0; i < count; i++)
				{
					VariableImpl cachedVariableObj = variableDefs[i].GetCachedVariableObj(odpContext);
					variableValues[i] = cachedVariableObj.GetResult();
				}
			}
		}

		internal static void ResetVariables(OnDemandProcessingContext odpContext, List<Variable> variableDefs)
		{
			if (variableDefs != null)
			{
				for (int i = 0; i < variableDefs.Count; i++)
				{
					variableDefs[i].GetCachedVariableObj(odpContext).Reset();
				}
			}
		}

		internal static void SetupVariables(OnDemandProcessingContext odpContext, List<Variable> variableDefs, object[] variableValues)
		{
			if (variableDefs != null)
			{
				for (int i = 0; i < variableValues.Length; i++)
				{
					variableDefs[i].GetCachedVariableObj(odpContext).SetValue(variableValues[i], internalSet: true);
				}
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.FirstRowIndex, Token.Int64));
			list.Add(new MemberInfo(MemberName.DataRegionInstances, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegionInstanceReference));
			list.Add(new MemberInfo(MemberName.SubReportInstances, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SubReportInstanceReference));
			list.Add(new MemberInfo(MemberName.AggregateValues, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObjResult));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScopeInstance, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.FirstRowIndex:
					writer.Write(m_firstRowOffset);
					break;
				case MemberName.DataRegionInstances:
				{
					if (m_serializationDataRegionIndexInCollection < 0 || m_dataRegionInstances == null)
					{
						writer.Write(m_dataRegionInstances);
						break;
					}
					List<IReference<DataRegionInstance>> list = new List<IReference<DataRegionInstance>>(m_dataRegionInstances.Count);
					ListUtils.AdjustLength(list, m_dataRegionInstances.Count - 1);
					list[m_serializationDataRegionIndexInCollection] = m_dataRegionInstances[m_serializationDataRegionIndexInCollection];
					writer.Write(list);
					break;
				}
				case MemberName.SubReportInstances:
					writer.Write(m_subReportInstances);
					break;
				case MemberName.AggregateValues:
					writer.Write(m_aggregateValues);
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
				case MemberName.FirstRowIndex:
					m_firstRowOffset = reader.ReadInt64();
					break;
				case MemberName.DataRegionInstances:
					m_dataRegionInstances = reader.ReadGenericListOfRIFObjects<IReference<DataRegionInstance>>();
					break;
				case MemberName.SubReportInstances:
					m_subReportInstances = reader.ReadGenericListOfRIFObjects<IReference<SubReportInstance>>();
					break;
				case MemberName.AggregateValues:
					m_aggregateValues = reader.ReadGenericListOfRIFObjects<DataAggregateObjResult>();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScopeInstance;
		}
	}
}
