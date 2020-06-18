using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal sealed class DataRegionMemberInstance : ScopeInstance, IMemberHierarchy
	{
		private int m_memberInstanceIndexWithinScopeLevel = -1;

		private List<ScalableList<DataRegionMemberInstance>> m_children;

		private ScalableList<DataCellInstanceList> m_cells;

		[NonSerialized]
		private List<ScalableList<DataCellInstance>> m_upgradedSnapshotCells;

		private object[] m_variables;

		private int m_recursiveLevel;

		private object[] m_groupExprValues;

		private int m_parentInstanceIndex = -1;

		private bool? m_hasRecursiveChildren;

		[Reference]
		private ReportHierarchyNode m_memberDef;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType ObjectType => Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegionMemberInstance;

		internal override IRIFReportScope RIFReportScope => m_memberDef;

		internal int MemberInstanceIndexWithinScopeLevel => m_memberInstanceIndexWithinScopeLevel;

		internal int RecursiveLevel => m_recursiveLevel;

		internal object[] GroupVariables => m_variables;

		internal object[] GroupExprValues => m_groupExprValues;

		internal List<ScalableList<DataRegionMemberInstance>> Children => m_children;

		internal ScalableList<DataCellInstanceList> Cells => m_cells;

		internal ReportHierarchyNode MemberDef => m_memberDef;

		internal int RecursiveParentIndex
		{
			get
			{
				return m_parentInstanceIndex;
			}
			set
			{
				m_parentInstanceIndex = value;
			}
		}

		internal bool? HasRecursiveChildren
		{
			get
			{
				return m_hasRecursiveChildren;
			}
			set
			{
				m_hasRecursiveChildren = value;
			}
		}

		public override int Size => base.Size + ItemSizes.SizeOf(m_children) + ItemSizes.SizeOf(m_cells) + ItemSizes.SizeOf(m_upgradedSnapshotCells) + ItemSizes.SizeOf(m_variables) + ItemSizes.SizeOf(m_recursiveLevel) + ItemSizes.SizeOf(m_groupExprValues) + 4 + ItemSizes.NullableBoolSize + ItemSizes.ReferenceSize;

		private DataRegionMemberInstance(OnDemandProcessingContext odpContext, ReportHierarchyNode memberDef, long firstRowOffset, int memberInstanceIndexWithinScopeLevel, int recursiveLevel, List<object> groupExpressionValues, object[] groupVariableValues)
			: base(firstRowOffset)
		{
			m_memberDef = memberDef;
			m_memberInstanceIndexWithinScopeLevel = memberInstanceIndexWithinScopeLevel;
			m_recursiveLevel = recursiveLevel;
			if (groupExpressionValues != null && groupExpressionValues.Count != 0)
			{
				m_groupExprValues = new object[groupExpressionValues.Count];
				for (int i = 0; i < m_groupExprValues.Length; i++)
				{
					object obj = groupExpressionValues[i];
					if (obj == DBNull.Value)
					{
						obj = null;
					}
					m_groupExprValues[i] = obj;
				}
			}
			StoreAggregates(odpContext, memberDef.Grouping.Aggregates);
			StoreAggregates(odpContext, memberDef.Grouping.RecursiveAggregates);
			StoreAggregates(odpContext, memberDef.Grouping.PostSortAggregates);
			StoreAggregates(odpContext, memberDef.RunningValues);
			if (memberDef.DataScopeInfo != null)
			{
				DataScopeInfo dataScopeInfo = memberDef.DataScopeInfo;
				StoreAggregates(odpContext, dataScopeInfo.AggregatesOfAggregates);
				StoreAggregates(odpContext, dataScopeInfo.PostSortAggregatesOfAggregates);
				StoreAggregates(odpContext, dataScopeInfo.RunningValuesOfAggregates);
			}
			m_variables = groupVariableValues;
		}

		internal DataRegionMemberInstance()
		{
		}

		internal static DataRegionMemberInstance CreateInstance(IMemberHierarchy parentInstance, OnDemandProcessingContext odpContext, ReportHierarchyNode memberDef, long firstRowOffset, int memberInstanceIndexWithinScopeLevel, int recursiveLevel, List<object> groupExpressionValues, object[] groupVariableValues, out int instanceIndex)
		{
			DataRegionMemberInstance dataRegionMemberInstance = new DataRegionMemberInstance(odpContext, memberDef, firstRowOffset, memberInstanceIndexWithinScopeLevel, recursiveLevel, groupExpressionValues, groupVariableValues);
			dataRegionMemberInstance.m_cleanupRef = parentInstance.AddMemberInstance(dataRegionMemberInstance, memberDef.IndexInCollection, odpContext.OdpMetadata.GroupTreeScalabilityCache, out instanceIndex);
			return dataRegionMemberInstance;
		}

		internal override void InstanceComplete()
		{
			if (m_cells != null)
			{
				m_cells.UnPinAll();
			}
			UnPinList(m_children);
			base.InstanceComplete();
		}

		IDisposable IMemberHierarchy.AddMemberInstance(DataRegionMemberInstance instance, int indexInCollection, IScalabilityCache cache, out int instanceIndex)
		{
			bool flag = false;
			if (m_children == null)
			{
				m_children = new List<ScalableList<DataRegionMemberInstance>>();
				flag = true;
			}
			ListUtils.AdjustLength(m_children, indexInCollection);
			ScalableList<DataRegionMemberInstance> scalableList = m_children[indexInCollection];
			if (flag || scalableList == null)
			{
				scalableList = new ScalableList<DataRegionMemberInstance>(0, cache, 100, 5);
				m_children[indexInCollection] = scalableList;
			}
			instanceIndex = scalableList.Count;
			return scalableList.AddAndPin(instance);
		}

		IDisposable IMemberHierarchy.AddCellInstance(int columnMemberSequenceId, int cellIndexInCollection, DataCellInstance cellInstance, IScalabilityCache cache)
		{
			if (m_cells == null)
			{
				m_cells = new ScalableList<DataCellInstanceList>(0, cache, 100, 5, keepAllBucketsPinned: true);
			}
			return DataRegionInstance.AddCellInstance(m_cells, columnMemberSequenceId, cellIndexInCollection, cellInstance, cache);
		}

		internal void SetupEnvironment(OnDemandProcessingContext odpContext, int dataSetIndex)
		{
			SetupFields(odpContext, dataSetIndex);
			int aggregateValueOffset = 0;
			SetupAggregates(odpContext, m_memberDef.Grouping.Aggregates, ref aggregateValueOffset);
			SetupAggregates(odpContext, m_memberDef.Grouping.RecursiveAggregates, ref aggregateValueOffset);
			SetupAggregates(odpContext, m_memberDef.Grouping.PostSortAggregates, ref aggregateValueOffset);
			SetupAggregates(odpContext, m_memberDef.RunningValues, ref aggregateValueOffset);
			if (m_memberDef.DataScopeInfo != null)
			{
				DataScopeInfo dataScopeInfo = m_memberDef.DataScopeInfo;
				SetupAggregates(odpContext, dataScopeInfo.AggregatesOfAggregates, ref aggregateValueOffset);
				SetupAggregates(odpContext, dataScopeInfo.PostSortAggregatesOfAggregates, ref aggregateValueOffset);
				SetupAggregates(odpContext, dataScopeInfo.RunningValuesOfAggregates, ref aggregateValueOffset);
			}
			if (m_variables != null)
			{
				ScopeInstance.SetupVariables(odpContext, m_memberDef.Grouping.Variables, m_variables);
			}
		}

		IList<DataRegionMemberInstance> IMemberHierarchy.GetChildMemberInstances(bool isRowMember, int memberIndexInCollection)
		{
			return ScopeInstance.GetChildMemberInstances(m_children, memberIndexInCollection);
		}

		IList<DataCellInstance> IMemberHierarchy.GetCellInstances(int columnMemberSequenceId)
		{
			if (m_cells != null && columnMemberSequenceId < m_cells.Count)
			{
				return m_cells[columnMemberSequenceId];
			}
			if (m_upgradedSnapshotCells != null && columnMemberSequenceId < m_upgradedSnapshotCells.Count)
			{
				return m_upgradedSnapshotCells[columnMemberSequenceId];
			}
			return null;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ID, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportHierarchyNode, Token.GlobalReference));
			list.Add(new MemberInfo(MemberName.MemberInstanceIndexWithinScopeLevel, Token.Int32));
			list.Add(new MemberInfo(MemberName.Children, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableList));
			list.Add(new ReadOnlyMemberInfo(MemberName.Cells, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableList));
			list.Add(new ReadOnlyMemberInfo(MemberName.Variables, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveArray, Token.Object));
			list.Add(new MemberInfo(MemberName.SerializableVariables, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SerializableArray, Token.Serializable));
			list.Add(new MemberInfo(MemberName.RecursiveLevel, Token.Int32));
			list.Add(new MemberInfo(MemberName.GroupExpressionValues, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveArray, Token.Object));
			list.Add(new MemberInfo(MemberName.ParentInstanceIndex, Token.Int32));
			list.Add(new MemberInfo(MemberName.HasRecursiveChildren, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Nullable, Token.Boolean));
			list.Add(new MemberInfo(MemberName.Cells2, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataCellInstanceList));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegionMemberInstance, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScopeInstance, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ID:
					Global.Tracer.Assert(m_memberDef != null, "(null != m_memberDef)");
					writer.WriteGlobalReference(m_memberDef);
					break;
				case MemberName.MemberInstanceIndexWithinScopeLevel:
					writer.Write7BitEncodedInt(m_memberInstanceIndexWithinScopeLevel);
					break;
				case MemberName.Children:
					writer.Write(m_children);
					break;
				case MemberName.Cells2:
					writer.Write(m_cells);
					break;
				case MemberName.SerializableVariables:
					writer.WriteSerializableArray(m_variables);
					break;
				case MemberName.RecursiveLevel:
					writer.Write7BitEncodedInt(m_recursiveLevel);
					break;
				case MemberName.GroupExpressionValues:
					writer.Write(m_groupExprValues);
					break;
				case MemberName.ParentInstanceIndex:
					writer.Write(m_parentInstanceIndex);
					break;
				case MemberName.HasRecursiveChildren:
					writer.Write(m_hasRecursiveChildren);
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
				case MemberName.ID:
					m_memberDef = reader.ReadGlobalReference<ReportHierarchyNode>();
					break;
				case MemberName.MemberInstanceIndexWithinScopeLevel:
					m_memberInstanceIndexWithinScopeLevel = reader.Read7BitEncodedInt();
					break;
				case MemberName.Children:
					m_children = reader.ReadGenericListOfRIFObjectsUsingNew<ScalableList<DataRegionMemberInstance>>();
					SetReadOnlyList(m_children);
					break;
				case MemberName.Cells2:
					m_cells = reader.ReadRIFObject<ScalableList<DataCellInstanceList>>();
					break;
				case MemberName.Cells:
					m_upgradedSnapshotCells = reader.ReadGenericListOfRIFObjectsUsingNew<ScalableList<DataCellInstance>>();
					SetReadOnlyList(m_upgradedSnapshotCells);
					break;
				case MemberName.Variables:
					m_variables = reader.ReadVariantArray();
					break;
				case MemberName.SerializableVariables:
					m_variables = reader.ReadSerializableArray();
					break;
				case MemberName.RecursiveLevel:
					m_recursiveLevel = reader.Read7BitEncodedInt();
					break;
				case MemberName.GroupExpressionValues:
					m_groupExprValues = reader.ReadVariantArray();
					break;
				case MemberName.ParentInstanceIndex:
					m_parentInstanceIndex = reader.ReadInt32();
					break;
				case MemberName.HasRecursiveChildren:
				{
					object obj = reader.ReadVariant();
					if (obj != null)
					{
						m_hasRecursiveChildren = (bool)obj;
					}
					break;
				}
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegionMemberInstance;
		}
	}
}
