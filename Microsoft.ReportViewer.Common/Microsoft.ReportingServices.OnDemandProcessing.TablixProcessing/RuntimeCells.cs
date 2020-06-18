using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class RuntimeCells : IStorable, IPersistable
	{
		private int m_firstCellKey = -1;

		private int m_lastCellKey = -1;

		private ScalableList<IStorable> m_collection;

		private static Declaration m_declaration = GetDeclaration();

		public int Count => m_collection.Count;

		public int Size => 8 + ItemSizes.SizeOf(m_collection);

		internal RuntimeCells()
		{
		}

		internal RuntimeCells(int priority, IScalabilityCache cache)
		{
			m_collection = new ScalableList<IStorable>(priority, cache, 200, 10);
		}

		internal void AddCell(int key, RuntimeCell cell)
		{
			InternalAdd(key, cell);
		}

		internal void AddCell(int key, IReference<RuntimeCell> cellRef)
		{
			InternalAdd(key, cellRef);
		}

		private void InternalAdd(int key, IStorable cell)
		{
			if (Count == 0)
			{
				m_firstCellKey = key;
			}
			else
			{
				GetAndPinCell(m_lastCellKey, out IDisposable cleanupRef).NextCell = key;
				cleanupRef?.Dispose();
			}
			m_lastCellKey = key;
			m_collection.SetValueWithExtension(key, cell);
		}

		internal RuntimeCell GetCell(int key, out RuntimeCellReference cellRef)
		{
			RuntimeCell result = null;
			cellRef = null;
			if (key < m_collection.Count)
			{
				IStorable storable = m_collection[key];
				if (storable != null)
				{
					if (IsCellReference(storable))
					{
						cellRef = (RuntimeCellReference)storable;
						result = cellRef.Value();
					}
					else
					{
						result = (RuntimeCell)storable;
					}
				}
			}
			return result;
		}

		internal RuntimeCell GetAndPinCell(int key, out IDisposable cleanupRef)
		{
			cleanupRef = null;
			IStorable item;
			bool flag;
			if (key < m_collection.Count)
			{
				cleanupRef = m_collection.GetAndPin(key, out item);
				flag = (item != null);
			}
			else
			{
				item = null;
				flag = false;
			}
			if (flag)
			{
				if (IsCellReference(item))
				{
					if (cleanupRef != null)
					{
						cleanupRef.Dispose();
					}
					IReference<RuntimeCell> reference = (IReference<RuntimeCell>)item;
					reference.PinValue();
					cleanupRef = (IDisposable)reference;
					return reference.Value();
				}
				return (RuntimeCell)item;
			}
			return null;
		}

		private bool IsCellReference(IStorable cellOrReference)
		{
			switch (cellOrReference.GetObjectType())
			{
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixCellReference:
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriCellReference:
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCellReference:
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataShapeIntersectionReference:
				return true;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixCell:
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriCell:
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCell:
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataShapeIntersection:
				return false;
			default:
				Global.Tracer.Assert(condition: false, "Unexpected type in RuntimeCells collection");
				throw new InvalidOperationException();
			}
		}

		internal RuntimeCell GetOrCreateCell(Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef, IReference<RuntimeDataTablixGroupLeafObj> ownerRef, IReference<RuntimeDataTablixGroupRootObj> currOuterGroupRootRef, out IDisposable cleanupRef)
		{
			RuntimeDataTablixGroupRootObj runtimeDataTablixGroupRootObj = currOuterGroupRootRef.Value();
			int groupLeafIndex = dataRegionDef.OuterGroupingIndexes[runtimeDataTablixGroupRootObj.HierarchyDef.HierarchyDynamicIndex];
			return GetOrCreateCellByIndex(groupLeafIndex, dataRegionDef, ownerRef, runtimeDataTablixGroupRootObj, out cleanupRef);
		}

		internal RuntimeCell GetOrCreateCell(Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef, IReference<RuntimeDataTablixGroupLeafObj> ownerRef, IReference<RuntimeDataTablixGroupRootObj> currOuterGroupRootRef, int groupLeafIndex, out IDisposable cleanupRef)
		{
			RuntimeDataTablixGroupRootObj currOuterGroupRoot = currOuterGroupRootRef.Value();
			return GetOrCreateCellByIndex(groupLeafIndex, dataRegionDef, ownerRef, currOuterGroupRoot, out cleanupRef);
		}

		private RuntimeCell GetOrCreateCellByIndex(int groupLeafIndex, Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef, IReference<RuntimeDataTablixGroupLeafObj> ownerRef, RuntimeDataTablixGroupRootObj currOuterGroupRoot, out IDisposable cleanupRef)
		{
			RuntimeCell andPinCell = GetAndPinCell(groupLeafIndex, out cleanupRef);
			if (andPinCell == null)
			{
				using (ownerRef.PinValue())
				{
					RuntimeDataTablixGroupLeafObj runtimeDataTablixGroupLeafObj = ownerRef.Value();
					if (!RuntimeCell.HasOnlySimpleGroupTreeCells(currOuterGroupRoot.HierarchyDef, runtimeDataTablixGroupLeafObj.MemberDef, dataRegionDef))
					{
						runtimeDataTablixGroupLeafObj.CreateCell(this, groupLeafIndex, currOuterGroupRoot.HierarchyDef, runtimeDataTablixGroupLeafObj.MemberDef, dataRegionDef);
					}
				}
				andPinCell = GetAndPinCell(groupLeafIndex, out cleanupRef);
			}
			return andPinCell;
		}

		internal void SortAndFilter(AggregateUpdateContext aggContext)
		{
			Traverse(ProcessingStages.SortAndFilter, aggContext);
		}

		private void Traverse(ProcessingStages operation, AggregateUpdateContext context)
		{
			if (Count == 0)
			{
				return;
			}
			int num = m_firstCellKey;
			int num2;
			do
			{
				num2 = num;
				IDisposable cleanupRef;
				RuntimeCell andPinCell = GetAndPinCell(num2, out cleanupRef);
				switch (operation)
				{
				case ProcessingStages.SortAndFilter:
					andPinCell.SortAndFilter(context);
					break;
				case ProcessingStages.UpdateAggregates:
					andPinCell.UpdateAggregates(context);
					break;
				default:
					Global.Tracer.Assert(condition: false, "Unknown operation in Traverse");
					break;
				}
				num = andPinCell.NextCell;
				cleanupRef?.Dispose();
			}
			while (num2 != m_lastCellKey);
		}

		internal void UpdateAggregates(AggregateUpdateContext aggContext)
		{
			Traverse(ProcessingStages.UpdateAggregates, aggContext);
		}

		internal void CalculateRunningValues(Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef, Dictionary<string, IReference<RuntimeGroupRootObj>> groupCol, IReference<RuntimeGroupRootObj> lastGroup, IReference<RuntimeDataTablixGroupLeafObj> owner, AggregateUpdateContext aggContext)
		{
			IDisposable cleanupRef;
			RuntimeCell orCreateCell = GetOrCreateCell(dataRegionDef, owner, dataRegionDef.CurrentOuterGroupRoot, out cleanupRef);
			if (orCreateCell != null)
			{
				orCreateCell.CalculateRunningValues(groupCol, lastGroup, aggContext);
				cleanupRef?.Dispose();
			}
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.FirstCell:
					writer.Write(m_firstCellKey);
					break;
				case MemberName.LastCell:
					writer.Write(m_lastCellKey);
					break;
				case MemberName.Collection:
					writer.Write(m_collection);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.FirstCell:
					m_firstCellKey = reader.ReadInt32();
					break;
				case MemberName.LastCell:
					m_lastCellKey = reader.ReadInt32();
					break;
				case MemberName.Collection:
					m_collection = reader.ReadRIFObject<ScalableList<IStorable>>();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCells;
		}

		public static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.FirstCell, Token.Int32));
				list.Add(new MemberInfo(MemberName.LastCell, Token.Int32));
				list.Add(new MemberInfo(MemberName.Collection, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableList));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCells, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return m_declaration;
		}
	}
}
