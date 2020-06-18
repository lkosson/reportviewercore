using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal sealed class DataRegionInstance : ScopeInstance, IMemberHierarchy
	{
		private int m_dataSetIndexInCollection = -1;

		private List<ScalableList<DataRegionMemberInstance>> m_rowMembers;

		private List<ScalableList<DataRegionMemberInstance>> m_columnMembers;

		private ScalableList<DataCellInstanceList> m_cells;

		[NonSerialized]
		private List<ScalableList<DataCellInstance>> m_upgradedSnapshotCells;

		[Reference]
		private DataRegion m_dataRegionDef;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType ObjectType => Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegionInstance;

		internal override IRIFReportScope RIFReportScope => m_dataRegionDef;

		internal DataRegion DataRegionDef => m_dataRegionDef;

		internal bool NoRows => m_firstRowOffset <= 0;

		internal int DataSetIndexInCollection => m_dataSetIndexInCollection;

		internal List<ScalableList<DataRegionMemberInstance>> TopLevelRowMembers => m_rowMembers;

		internal List<ScalableList<DataRegionMemberInstance>> TopLevelColumnMembers => m_columnMembers;

		internal ScalableList<DataCellInstanceList> Cells => m_cells;

		public override int Size => base.Size + 4 + ItemSizes.SizeOf(m_rowMembers) + ItemSizes.SizeOf(m_columnMembers) + ItemSizes.SizeOf(m_cells) + ItemSizes.SizeOf(m_upgradedSnapshotCells) + ItemSizes.ReferenceSize;

		private DataRegionInstance(DataRegion dataRegionDef, int dataSetIndex)
		{
			m_dataRegionDef = dataRegionDef;
			m_dataSetIndexInCollection = dataSetIndex;
		}

		internal DataRegionInstance()
		{
		}

		internal static IReference<DataRegionInstance> CreateInstance(ScopeInstance parentInstance, OnDemandMetadata odpMetadata, DataRegion dataRegionDef, int dataSetIndex)
		{
			DataRegionInstance dataRegionInstance = new DataRegionInstance(dataRegionDef, dataSetIndex);
			GroupTreeScalabilityCache groupTreeScalabilityCache = odpMetadata.GroupTreeScalabilityCache;
			IReference<DataRegionInstance> reference;
			if (parentInstance.ObjectType == Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportInstance)
			{
				reference = ((ReportInstance)parentInstance).GetTopLevelDataRegionReference(dataRegionDef.IndexInCollection);
				groupTreeScalabilityCache.SetTreePartitionContentsAndPin(reference, dataRegionInstance);
			}
			else
			{
				reference = groupTreeScalabilityCache.AllocateAndPin(dataRegionInstance, 0);
				parentInstance.AddChildScope((IReference<ScopeInstance>)reference, dataRegionDef.IndexInCollection);
			}
			dataRegionInstance.m_cleanupRef = (IDisposable)reference;
			return reference;
		}

		internal override void InstanceComplete()
		{
			if (m_cells != null)
			{
				m_cells.UnPinAll();
			}
			UnPinList(m_rowMembers);
			UnPinList(m_columnMembers);
			base.InstanceComplete();
		}

		IDisposable IMemberHierarchy.AddMemberInstance(DataRegionMemberInstance instance, int indexInCollection, IScalabilityCache cache, out int instanceIndex)
		{
			List<ScalableList<DataRegionMemberInstance>> list = instance.MemberDef.IsColumn ? m_columnMembers : m_rowMembers;
			bool flag = false;
			if (list == null)
			{
				flag = true;
				list = new List<ScalableList<DataRegionMemberInstance>>();
				if (instance.MemberDef.IsColumn)
				{
					m_columnMembers = list;
				}
				else
				{
					m_rowMembers = list;
				}
			}
			ListUtils.AdjustLength(list, indexInCollection);
			ScalableList<DataRegionMemberInstance> scalableList = list[indexInCollection];
			if (flag || scalableList == null)
			{
				scalableList = (list[indexInCollection] = new ScalableList<DataRegionMemberInstance>(0, cache, 100, 5));
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
			return AddCellInstance(m_cells, columnMemberSequenceId, cellIndexInCollection, cellInstance, cache);
		}

		internal static IDisposable AddCellInstance(ScalableList<DataCellInstanceList> cells, int columnMemberSequenceId, int cellIndexInCollection, DataCellInstance cellInstance, IScalabilityCache cache)
		{
			ScopeInstance.AdjustLength(cells, columnMemberSequenceId);
			DataCellInstanceList item;
			IDisposable andPin = cells.GetAndPin(columnMemberSequenceId, out item);
			if (item == null)
			{
				item = (cells[columnMemberSequenceId] = new DataCellInstanceList());
			}
			ListUtils.AdjustLength(item, cellIndexInCollection);
			item[cellIndexInCollection] = cellInstance;
			return andPin;
		}

		internal void SetupEnvironment(OnDemandProcessingContext odpContext)
		{
			SetupFields(odpContext, m_dataSetIndexInCollection);
			int aggregateValueOffset = 0;
			SetupAggregates(odpContext, m_dataRegionDef.Aggregates, ref aggregateValueOffset);
			SetupAggregates(odpContext, m_dataRegionDef.PostSortAggregates, ref aggregateValueOffset);
			SetupAggregates(odpContext, m_dataRegionDef.RunningValues, ref aggregateValueOffset);
			if (m_dataRegionDef.DataScopeInfo != null)
			{
				DataScopeInfo dataScopeInfo = m_dataRegionDef.DataScopeInfo;
				SetupAggregates(odpContext, dataScopeInfo.AggregatesOfAggregates, ref aggregateValueOffset);
				SetupAggregates(odpContext, dataScopeInfo.PostSortAggregatesOfAggregates, ref aggregateValueOffset);
				SetupAggregates(odpContext, dataScopeInfo.RunningValuesOfAggregates, ref aggregateValueOffset);
			}
		}

		IList<DataRegionMemberInstance> IMemberHierarchy.GetChildMemberInstances(bool isRowMember, int memberIndexInCollection)
		{
			return ScopeInstance.GetChildMemberInstances(isRowMember ? m_rowMembers : m_columnMembers, memberIndexInCollection);
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
			list.Add(new MemberInfo(MemberName.ID, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegion, Token.GlobalReference));
			list.Add(new MemberInfo(MemberName.DataSetIndexInCollection, Token.Int32));
			list.Add(new MemberInfo(MemberName.RowMembers, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableList));
			list.Add(new MemberInfo(MemberName.ColumnMembers, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableList));
			list.Add(new ReadOnlyMemberInfo(MemberName.Cells, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableList));
			list.Add(new MemberInfo(MemberName.Cells2, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataCellInstanceList));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegionInstance, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScopeInstance, list);
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
					Global.Tracer.Assert(m_dataRegionDef != null, "(null != m_dataRegionDef)");
					writer.WriteGlobalReference(m_dataRegionDef);
					break;
				case MemberName.DataSetIndexInCollection:
					writer.Write7BitEncodedInt(m_dataSetIndexInCollection);
					break;
				case MemberName.RowMembers:
					writer.Write(m_rowMembers);
					break;
				case MemberName.ColumnMembers:
					writer.Write(m_columnMembers);
					break;
				case MemberName.Cells2:
					writer.Write(m_cells);
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
					m_dataRegionDef = reader.ReadGlobalReference<DataRegion>();
					break;
				case MemberName.DataSetIndexInCollection:
					m_dataSetIndexInCollection = reader.Read7BitEncodedInt();
					break;
				case MemberName.RowMembers:
					m_rowMembers = reader.ReadGenericListOfRIFObjectsUsingNew<ScalableList<DataRegionMemberInstance>>();
					SetReadOnlyList(m_rowMembers);
					break;
				case MemberName.ColumnMembers:
					m_columnMembers = reader.ReadGenericListOfRIFObjectsUsingNew<ScalableList<DataRegionMemberInstance>>();
					SetReadOnlyList(m_columnMembers);
					break;
				case MemberName.Cells2:
					m_cells = reader.ReadRIFObject<ScalableList<DataCellInstanceList>>();
					break;
				case MemberName.Cells:
					m_upgradedSnapshotCells = reader.ReadGenericListOfRIFObjectsUsingNew<ScalableList<DataCellInstance>>();
					SetReadOnlyList(m_upgradedSnapshotCells);
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegionInstance;
		}
	}
}
