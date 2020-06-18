using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal sealed class DataCellInstance : ScopeInstance
	{
		[NonSerialized]
		private Cell m_cellDef;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType ObjectType => Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataCellInstance;

		internal override IRIFReportScope RIFReportScope => m_cellDef;

		internal Cell CellDef => m_cellDef;

		public override int Size => base.Size + ItemSizes.ReferenceSize;

		private DataCellInstance(OnDemandProcessingContext odpContext, Cell cellDef, DataAggregateObjResult[] runningValueValues, DataAggregateObjResult[] runningValueOfAggregateValues, long firstRowOffset)
			: base(firstRowOffset)
		{
			m_cellDef = cellDef;
			DataRegion dataRegionDef = m_cellDef.DataRegionDef;
			if (cellDef.AggregateIndexes != null)
			{
				StoreAggregates(odpContext, dataRegionDef.CellAggregates, cellDef.AggregateIndexes);
			}
			if (cellDef.PostSortAggregateIndexes != null)
			{
				StoreAggregates(odpContext, dataRegionDef.CellPostSortAggregates, cellDef.PostSortAggregateIndexes);
			}
			if (runningValueValues == null)
			{
				if (cellDef.RunningValueIndexes != null)
				{
					StoreAggregates(odpContext, dataRegionDef.CellRunningValues, cellDef.RunningValueIndexes);
				}
			}
			else if (runningValueValues != null)
			{
				StoreAggregates(runningValueValues);
			}
			if (cellDef.DataScopeInfo != null)
			{
				DataScopeInfo dataScopeInfo = cellDef.DataScopeInfo;
				if (dataScopeInfo.AggregatesOfAggregates != null)
				{
					StoreAggregates(odpContext, dataScopeInfo.AggregatesOfAggregates);
				}
				if (dataScopeInfo.PostSortAggregatesOfAggregates != null)
				{
					StoreAggregates(odpContext, dataScopeInfo.PostSortAggregatesOfAggregates);
				}
				if (runningValueOfAggregateValues != null)
				{
					StoreAggregates(runningValueOfAggregateValues);
				}
			}
		}

		internal DataCellInstance()
		{
		}

		internal static DataCellInstance CreateInstance(IMemberHierarchy dataRegionOrRowMemberInstance, OnDemandProcessingContext odpContext, Cell cellDef, long firstRowOffset, int columnMemberSequenceId)
		{
			return CreateInstance(dataRegionOrRowMemberInstance, odpContext, cellDef, null, null, firstRowOffset, columnMemberSequenceId);
		}

		internal static DataCellInstance CreateInstance(IMemberHierarchy dataRegionOrRowMemberInstance, OnDemandProcessingContext odpContext, Cell cellDef, DataAggregateObjResult[] runningValueValues, DataAggregateObjResult[] runningValueOfAggregateValues, long firstRowOffset, int columnMemberSequenceId)
		{
			DataCellInstance dataCellInstance = new DataCellInstance(odpContext, cellDef, runningValueValues, runningValueOfAggregateValues, firstRowOffset);
			dataCellInstance.m_cleanupRef = dataRegionOrRowMemberInstance.AddCellInstance(columnMemberSequenceId, cellDef.IndexInCollection, dataCellInstance, odpContext.OdpMetadata.GroupTreeScalabilityCache);
			return dataCellInstance;
		}

		internal void SetupEnvironment(OnDemandProcessingContext odpContext, int dataSetIndex)
		{
			SetupFields(odpContext, dataSetIndex);
			DataRegion dataRegionDef = m_cellDef.DataRegionDef;
			int aggregateValueOffset = 0;
			if (m_cellDef.AggregateIndexes != null)
			{
				SetupAggregates(odpContext, dataRegionDef.CellAggregates, m_cellDef.AggregateIndexes, ref aggregateValueOffset);
			}
			if (m_cellDef.PostSortAggregateIndexes != null)
			{
				SetupAggregates(odpContext, dataRegionDef.CellPostSortAggregates, m_cellDef.PostSortAggregateIndexes, ref aggregateValueOffset);
			}
			if (m_cellDef.RunningValueIndexes != null)
			{
				SetupAggregates(odpContext, dataRegionDef.CellRunningValues, m_cellDef.RunningValueIndexes, ref aggregateValueOffset);
			}
			if (m_cellDef.DataScopeInfo != null)
			{
				DataScopeInfo dataScopeInfo = m_cellDef.DataScopeInfo;
				if (dataScopeInfo.AggregatesOfAggregates != null)
				{
					SetupAggregates(odpContext, dataScopeInfo.AggregatesOfAggregates, ref aggregateValueOffset);
				}
				if (dataScopeInfo.PostSortAggregatesOfAggregates != null)
				{
					SetupAggregates(odpContext, dataScopeInfo.PostSortAggregatesOfAggregates, ref aggregateValueOffset);
				}
				if (dataScopeInfo.RunningValuesOfAggregates != null)
				{
					SetupAggregates(odpContext, dataScopeInfo.RunningValuesOfAggregates, ref aggregateValueOffset);
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ID, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Cell, Token.GlobalReference));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataCellInstance, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScopeInstance, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				if (writer.CurrentMember.MemberName == MemberName.ID)
				{
					Global.Tracer.Assert(m_cellDef != null, "(null != m_cellDef)");
					writer.WriteGlobalReference(m_cellDef);
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				if (reader.CurrentMember.MemberName == MemberName.ID)
				{
					m_cellDef = reader.ReadGlobalReference<Cell>();
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataCellInstance;
		}
	}
}
