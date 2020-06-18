using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal class RuntimeDataTablixGroupRootObj : RuntimeGroupRootObj
	{
		private List<int> m_recursiveParentIndexes;

		private IReference<RuntimeMemberObj>[] m_innerGroupings;

		private List<string> m_cellRVs;

		private List<string> m_staticCellRVs;

		private List<string> m_cellPreviousValues;

		private List<string> m_staticCellPreviousValues;

		private int m_headingLevel;

		private bool m_outermostStatics;

		private bool m_hasLeafCells;

		private bool m_processOutermostStaticCells;

		private bool m_processStaticCellsForRVs;

		private int m_currentMemberIndexWithinScopeLevel = -1;

		[NonSerialized]
		private DataRegionMemberInstance m_currentMemberInstance;

		[NonSerialized]
		private static Declaration m_declaration = GetDeclaration();

		internal IReference<RuntimeMemberObj>[] InnerGroupings => m_innerGroupings;

		internal int HeadingLevel => m_headingLevel;

		internal bool OutermostStatics => m_outermostStatics;

		internal bool ProcessOutermostStaticCells => m_processOutermostStaticCells;

		internal bool HasLeafCells => m_hasLeafCells;

		internal object CurrentGroupExpressionValue => m_currentGroupExprValue;

		internal int CurrentMemberIndexWithinScopeLevel
		{
			get
			{
				return m_currentMemberIndexWithinScopeLevel;
			}
			set
			{
				m_currentMemberIndexWithinScopeLevel = value;
			}
		}

		internal DataRegionMemberInstance CurrentMemberInstance
		{
			get
			{
				return m_currentMemberInstance;
			}
			set
			{
				m_currentMemberInstance = value;
			}
		}

		public override int Size => base.Size + ItemSizes.SizeOf(m_innerGroupings) + ItemSizes.SizeOf(m_cellRVs) + ItemSizes.SizeOf(m_staticCellRVs) + ItemSizes.SizeOf(m_cellPreviousValues) + ItemSizes.SizeOf(m_staticCellPreviousValues) + ItemSizes.SizeOf(m_recursiveParentIndexes) + 4 + 1 + 1 + 1 + 4 + 1 + ItemSizes.ReferenceSize;

		internal RuntimeDataTablixGroupRootObj()
		{
		}

		internal RuntimeDataTablixGroupRootObj(IReference<IScope> outerScope, Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode dynamicMember, ref DataActions dataAction, OnDemandProcessingContext odpContext, IReference<RuntimeMemberObj>[] innerGroupings, bool outermostStatics, int headingLevel, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType)
			: base(outerScope, dynamicMember, dataAction, odpContext, objectType)
		{
			m_innerGroupings = innerGroupings;
			m_headingLevel = headingLevel;
			m_outermostStatics = outermostStatics;
			m_hasLeafCells = false;
			HierarchyNodeList innerStaticMembersInSameScope = dynamicMember.InnerStaticMembersInSameScope;
			m_hasLeafCells = (!dynamicMember.HasInnerDynamic || (innerStaticMembersInSameScope != null && innerStaticMembersInSameScope.LeafCellIndexes != null));
			if (((innerGroupings == null && innerStaticMembersInSameScope != null && innerStaticMembersInSameScope.LeafCellIndexes != null) || (innerGroupings != null && outermostStatics)) && m_hasLeafCells)
			{
				m_processStaticCellsForRVs = true;
			}
			if (m_hasLeafCells && outermostStatics)
			{
				m_processOutermostStaticCells = true;
			}
			NeedProcessDataActions(dynamicMember);
			NeedProcessDataActions(dynamicMember.InnerStaticMembersInSameScope);
			if (dynamicMember.Grouping.Filters == null)
			{
				dataAction = DataActions.None;
			}
			if ((m_processOutermostStaticCells || m_processStaticCellsForRVs) && (dynamicMember.DataRegionDef.CellPostSortAggregates != null || dynamicMember.DataRegionDef.CellRunningValues != null))
			{
				m_dataAction |= DataActions.PostSortAggregates;
			}
		}

		private void NeedProcessDataActions(HierarchyNodeList members)
		{
			if (members != null)
			{
				for (int i = 0; i < members.Count; i++)
				{
					NeedProcessDataActions(members[i]);
				}
			}
		}

		private void NeedProcessDataActions(Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode memberDefinition)
		{
			if (memberDefinition != null)
			{
				NeedProcessDataActions(memberDefinition.RunningValues);
			}
		}

		private void NeedProcessDataActions(List<Microsoft.ReportingServices.ReportIntermediateFormat.RunningValueInfo> runningValues)
		{
			if ((m_dataAction & DataActions.PostSortAggregates) == 0 && runningValues != null && 0 < runningValues.Count)
			{
				m_dataAction |= DataActions.PostSortAggregates;
			}
		}

		protected override void UpdateDataRegionGroupRootInfo()
		{
			if (m_innerGroupings != null)
			{
				base.HierarchyDef.DataRegionDef.CurrentOuterGroupRootObjs[m_hierarchyDef.HierarchyDynamicIndex] = (RuntimeDataTablixGroupRootObjReference)base.SelfReference;
			}
		}

		internal virtual void PrepareCalculateRunningValues()
		{
			TraverseGroupOrSortTree(ProcessingStages.PreparePeerGroupRunningValues, null);
		}

		internal override void CalculateRunningValues(Dictionary<string, IReference<RuntimeGroupRootObj>> groupCol, IReference<RuntimeGroupRootObj> lastGroup, AggregateUpdateContext aggContext)
		{
			base.CalculateRunningValues(groupCol, lastGroup, aggContext);
			if (m_processStaticCellsForRVs || m_processOutermostStaticCells)
			{
				m_hierarchyDef.DataRegionDef.ProcessOutermostStaticCellRunningValues = true;
				if (m_innerGroupings != null)
				{
					m_hierarchyDef.DataRegionDef.CurrentOuterGroupRoot = (RuntimeDataTablixGroupRootObjReference)base.SelfReference;
				}
				AddCellRunningValues(groupCol, ref m_staticCellRVs, ref m_staticCellPreviousValues, outermostStatics: true);
				m_hierarchyDef.DataRegionDef.ProcessOutermostStaticCellRunningValues = false;
			}
			if (m_innerGroupings == null)
			{
				IReference<RuntimeDataTablixGroupRootObj> currentOuterGroupRoot = m_hierarchyDef.DataRegionDef.CurrentOuterGroupRoot;
				if (currentOuterGroupRoot != null)
				{
					m_hierarchyDef.DataRegionDef.ProcessCellRunningValues = true;
					m_cellRVs = null;
					m_cellPreviousValues = null;
					AddCellRunningValues(groupCol, ref m_cellRVs, ref m_cellPreviousValues, outermostStatics: false);
					m_hierarchyDef.DataRegionDef.ProcessCellRunningValues = false;
				}
			}
			AddRunningValues(m_hierarchyDef.RunningValues);
			AddRunningValuesOfAggregates();
			TraverseGroupOrSortTree(ProcessingStages.RunningValues, aggContext);
			if (m_hierarchyDef.Grouping.Name != null)
			{
				groupCol.Remove(m_hierarchyDef.Grouping.Name);
			}
		}

		private void AddCellRunningValues(Dictionary<string, IReference<RuntimeGroupRootObj>> groupCol, ref List<string> runningValues, ref List<string> previousValues, bool outermostStatics)
		{
			if (m_hierarchyDef.DataRegionDef.CellRunningValues != null && 0 < m_hierarchyDef.DataRegionDef.CellRunningValues.Count && AddRunningValues(m_hierarchyDef.DataRegionDef.CellRunningValues, ref runningValues, ref previousValues, groupCol, cellRunningValues: true, outermostStatics))
			{
				m_dataAction |= DataActions.PostSortAggregates;
			}
		}

		internal override void CalculatePreviousAggregates()
		{
			if (!FlagUtils.HasFlag(m_dataAction, DataActions.PostSortAggregates))
			{
				return;
			}
			AggregatesImpl aggregatesImpl = m_odpContext.ReportObjectModel.AggregatesImpl;
			if (m_hierarchyDef.DataRegionDef.ProcessCellRunningValues)
			{
				if (m_cellPreviousValues != null)
				{
					for (int i = 0; i < m_cellPreviousValues.Count; i++)
					{
						string text = m_cellPreviousValues[i];
						Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregateObj = aggregatesImpl.GetAggregateObj(text);
						Global.Tracer.Assert(aggregateObj != null, "Missing expected previous aggregate: {0}", text);
						aggregateObj.Update();
					}
				}
				if (m_outerScope != null && (m_outerDataAction & DataActions.PostSortAggregates) != 0)
				{
					using (m_outerScope.PinValue())
					{
						m_outerScope.Value().CalculatePreviousAggregates();
					}
				}
				return;
			}
			if (m_staticCellPreviousValues != null)
			{
				for (int j = 0; j < m_staticCellPreviousValues.Count; j++)
				{
					string text2 = m_staticCellPreviousValues[j];
					Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregateObj2 = aggregatesImpl.GetAggregateObj(text2);
					Global.Tracer.Assert(aggregateObj2 != null, "Missing expected previous aggregate: {0}", text2);
					aggregateObj2.Update();
				}
			}
			base.CalculatePreviousAggregates();
		}

		public override void ReadRow(DataActions dataAction, ITraversalContext context)
		{
			if (!FlagUtils.HasFlag(dataAction, DataActions.PostSortAggregates) || !FlagUtils.HasFlag(m_dataAction, DataActions.PostSortAggregates))
			{
				return;
			}
			AggregatesImpl aggregatesImpl = m_odpContext.ReportObjectModel.AggregatesImpl;
			if (m_hierarchyDef.DataRegionDef.ProcessCellRunningValues)
			{
				if (m_cellRVs != null)
				{
					for (int i = 0; i < m_cellRVs.Count; i++)
					{
						string text = m_cellRVs[i];
						Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregateObj = aggregatesImpl.GetAggregateObj(text);
						Global.Tracer.Assert(aggregateObj != null, "Missing expected running value: {0}", text);
						aggregateObj.Update();
					}
				}
				if (m_outerScope != null && m_hierarchyDef.DataRegionDef.CellPostSortAggregates != null)
				{
					using (m_outerScope.PinValue())
					{
						m_outerScope.Value().ReadRow(dataAction, context);
					}
				}
				return;
			}
			if (m_staticCellRVs != null)
			{
				for (int j = 0; j < m_staticCellRVs.Count; j++)
				{
					string text2 = m_staticCellRVs[j];
					Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregateObj2 = aggregatesImpl.GetAggregateObj(text2);
					Global.Tracer.Assert(aggregateObj2 != null, "Missing expected running value: {0}", text2);
					aggregateObj2.Update();
				}
			}
			base.ReadRow(dataAction, context);
		}

		internal void DoneReadingRows(ref Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[] runningValueValues, ref Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[] runningValueOfAggregateValues, ref Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[] cellRunningValueValues)
		{
			AggregatesImpl aggregatesImpl = m_odpContext.ReportObjectModel.AggregatesImpl;
			RuntimeRICollection.StoreRunningValues(aggregatesImpl, m_hierarchyDef.RunningValues, ref runningValueValues);
			if (m_hierarchyDef.DataScopeInfo != null)
			{
				RuntimeRICollection.StoreRunningValues(aggregatesImpl, m_hierarchyDef.DataScopeInfo.RunningValuesOfAggregates, ref runningValueOfAggregateValues);
			}
			int num = (m_staticCellPreviousValues != null) ? m_staticCellPreviousValues.Count : 0;
			int num2 = (m_staticCellRVs != null) ? m_staticCellRVs.Count : 0;
			if (num2 > 0)
			{
				cellRunningValueValues = new Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[num2 + num];
				for (int i = 0; i < num2; i++)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregateObj = aggregatesImpl.GetAggregateObj(m_staticCellRVs[i]);
					cellRunningValueValues[i] = aggregateObj.AggregateResult();
				}
			}
			if (num > 0)
			{
				if (cellRunningValueValues == null)
				{
					cellRunningValueValues = new Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[num];
				}
				for (int j = 0; j < num; j++)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregateObj2 = aggregatesImpl.GetAggregateObj(m_staticCellPreviousValues[j]);
					cellRunningValueValues[num2 + j] = aggregateObj2.AggregateResult();
				}
			}
		}

		internal void SetupCellRunningValues(Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[] cellRunningValueValues)
		{
			if (cellRunningValueValues == null)
			{
				return;
			}
			AggregatesImpl aggregatesImpl = m_odpContext.ReportObjectModel.AggregatesImpl;
			int num = (m_staticCellPreviousValues != null) ? m_staticCellPreviousValues.Count : 0;
			int num2 = (m_staticCellRVs != null) ? m_staticCellRVs.Count : 0;
			if (num2 > 0)
			{
				for (int i = 0; i < num2; i++)
				{
					string text = m_staticCellRVs[i];
					Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregateObj = aggregatesImpl.GetAggregateObj(text);
					Global.Tracer.Assert(aggregateObj != null, "Missing expected running value: {0}", text);
					aggregateObj.Set(cellRunningValueValues[i]);
				}
			}
			if (num > 0)
			{
				for (int j = 0; j < num; j++)
				{
					string text2 = m_staticCellPreviousValues[j];
					Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregateObj2 = aggregatesImpl.GetAggregateObj(text2);
					Global.Tracer.Assert(aggregateObj2 != null, "Missing expected running value: {0}", text2);
					aggregateObj2.Set(cellRunningValueValues[num2 + j]);
				}
			}
		}

		internal bool GetCellTargetForNonDetailSort()
		{
			using (m_outerScope.PinValue())
			{
				IScope scope = m_outerScope.Value();
				if (scope is RuntimeTablixObj)
				{
					return scope.TargetForNonDetailSort;
				}
				Global.Tracer.Assert(scope is RuntimeTablixGroupLeafObj, "(outerScopeObj is RuntimeTablixGroupLeafObj)");
				return ((RuntimeTablixGroupLeafObj)scope).GetCellTargetForNonDetailSort();
			}
		}

		internal bool GetCellTargetForSort(int index, bool detailSort)
		{
			using (m_outerScope.PinValue())
			{
				IScope scope = m_outerScope.Value();
				if (scope is RuntimeTablixObj)
				{
					return scope.IsTargetForSort(index, detailSort);
				}
				Global.Tracer.Assert(scope is RuntimeTablixGroupLeafObj, "(outerScopeObj is RuntimeTablixGroupLeafObj)");
				return ((RuntimeTablixGroupLeafObj)scope).GetCellTargetForSort(index, detailSort);
			}
		}

		internal int GetRecursiveParentIndex(int recursiveLevel)
		{
			return m_recursiveParentIndexes[recursiveLevel];
		}

		internal void SetRecursiveParentIndex(int instanceIndex, int recursiveLevel)
		{
			if (m_recursiveParentIndexes == null)
			{
				m_recursiveParentIndexes = new List<int>();
			}
			while (recursiveLevel >= m_recursiveParentIndexes.Count)
			{
				m_recursiveParentIndexes.Add(-1);
			}
			m_recursiveParentIndexes[recursiveLevel] = instanceIndex;
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.InnerGroupings:
					writer.Write(m_innerGroupings);
					break;
				case MemberName.CellRunningValues:
					writer.WriteListOfPrimitives(m_cellRVs);
					break;
				case MemberName.StaticCellRunningValues:
					writer.WriteListOfPrimitives(m_staticCellRVs);
					break;
				case MemberName.CellPreviousValues:
					writer.WriteListOfPrimitives(m_cellPreviousValues);
					break;
				case MemberName.StaticCellPreviousValues:
					writer.WriteListOfPrimitives(m_staticCellPreviousValues);
					break;
				case MemberName.HeadingLevel:
					writer.Write(m_headingLevel);
					break;
				case MemberName.OutermostStatics:
					writer.Write(m_outermostStatics);
					break;
				case MemberName.HasLeafCells:
					writer.Write(m_hasLeafCells);
					break;
				case MemberName.ProcessOutermostStaticCells:
					writer.Write(m_processOutermostStaticCells);
					break;
				case MemberName.CurrentMemberIndexWithinScopeLevel:
					writer.Write(m_currentMemberIndexWithinScopeLevel);
					break;
				case MemberName.RecursiveParentIndexes:
					writer.WriteListOfPrimitives(m_recursiveParentIndexes);
					break;
				case MemberName.ProcessStaticCellsForRVs:
					writer.Write(m_processStaticCellsForRVs);
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
			reader.RegisterDeclaration(m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.InnerGroupings:
					m_innerGroupings = reader.ReadArrayOfRIFObjects<IReference<RuntimeMemberObj>>();
					break;
				case MemberName.CellRunningValues:
					m_cellRVs = reader.ReadListOfPrimitives<string>();
					break;
				case MemberName.StaticCellRunningValues:
					m_staticCellRVs = reader.ReadListOfPrimitives<string>();
					break;
				case MemberName.CellPreviousValues:
					m_cellPreviousValues = reader.ReadListOfPrimitives<string>();
					break;
				case MemberName.StaticCellPreviousValues:
					m_staticCellPreviousValues = reader.ReadListOfPrimitives<string>();
					break;
				case MemberName.HeadingLevel:
					m_headingLevel = reader.ReadInt32();
					break;
				case MemberName.OutermostStatics:
					m_outermostStatics = reader.ReadBoolean();
					break;
				case MemberName.HasLeafCells:
					m_hasLeafCells = reader.ReadBoolean();
					break;
				case MemberName.ProcessOutermostStaticCells:
					m_processOutermostStaticCells = reader.ReadBoolean();
					break;
				case MemberName.CurrentMemberIndexWithinScopeLevel:
					m_currentMemberIndexWithinScopeLevel = reader.ReadInt32();
					break;
				case MemberName.RecursiveParentIndexes:
					m_recursiveParentIndexes = reader.ReadListOfPrimitives<int>();
					break;
				case MemberName.ProcessStaticCellsForRVs:
					m_processStaticCellsForRVs = reader.ReadBoolean();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixGroupRootObj;
		}

		public new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.InnerGroupings, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeMemberObjReference));
				list.Add(new MemberInfo(MemberName.CellRunningValues, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.String));
				list.Add(new MemberInfo(MemberName.StaticCellRunningValues, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.String));
				list.Add(new MemberInfo(MemberName.CellPreviousValues, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.String));
				list.Add(new MemberInfo(MemberName.StaticCellPreviousValues, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.String));
				list.Add(new MemberInfo(MemberName.HeadingLevel, Token.Int32));
				list.Add(new MemberInfo(MemberName.OutermostStatics, Token.Boolean));
				list.Add(new MemberInfo(MemberName.HasLeafCells, Token.Boolean));
				list.Add(new MemberInfo(MemberName.ProcessOutermostStaticCells, Token.Boolean));
				list.Add(new MemberInfo(MemberName.CurrentMemberIndexWithinScopeLevel, Token.Int32));
				list.Add(new MemberInfo(MemberName.RecursiveParentIndexes, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Int32));
				list.Add(new MemberInfo(MemberName.ProcessStaticCellsForRVs, Token.Boolean));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixGroupRootObj, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupRootObj, list);
			}
			return m_declaration;
		}
	}
}
