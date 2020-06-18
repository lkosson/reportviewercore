using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal class RuntimeHierarchyObj : RuntimeDataRegionObj, IHierarchyObj, IStorable, IPersistable
	{
		protected RuntimeGroupingObj m_grouping;

		protected RuntimeExpressionInfo m_expression;

		protected RuntimeHierarchyObjReference m_hierarchyRoot;

		protected List<IReference<RuntimeHierarchyObj>> m_hierarchyObjs;

		private static Declaration m_declaration = GetDeclaration();

		internal List<IReference<RuntimeHierarchyObj>> HierarchyObjs => m_hierarchyObjs;

		protected override IReference<IScope> OuterScope
		{
			get
			{
				Global.Tracer.Assert(condition: false);
				return null;
			}
		}

		protected virtual IReference<IHierarchyObj> HierarchyRoot => m_hierarchyRoot;

		internal RuntimeGroupingObj Grouping => m_grouping;

		protected virtual BTree SortTree => m_grouping.Tree;

		protected virtual int ExpressionIndex
		{
			get
			{
				if (m_expression != null)
				{
					return m_expression.ExpressionIndex;
				}
				return 0;
			}
		}

		protected virtual List<DataFieldRow> SortDataRows
		{
			get
			{
				Global.Tracer.Assert(condition: false);
				return null;
			}
		}

		protected virtual List<int> SortFilterInfoIndices
		{
			get
			{
				Global.Tracer.Assert(condition: false);
				return null;
			}
		}

		protected virtual bool IsDetail => false;

		IReference<IHierarchyObj> IHierarchyObj.HierarchyRoot => HierarchyRoot;

		OnDemandProcessingContext IHierarchyObj.OdpContext => m_odpContext;

		BTree IHierarchyObj.SortTree => SortTree;

		int IHierarchyObj.ExpressionIndex => ExpressionIndex;

		List<int> IHierarchyObj.SortFilterInfoIndices => SortFilterInfoIndices;

		bool IHierarchyObj.IsDetail => IsDetail;

		bool IHierarchyObj.InDataRowSortPhase => false;

		public override int Size => base.Size + ItemSizes.SizeOf(m_grouping) + ItemSizes.SizeOf(m_expression) + ItemSizes.SizeOf(m_hierarchyRoot) + ItemSizes.SizeOf(m_hierarchyObjs);

		internal RuntimeHierarchyObj()
		{
		}

		protected RuntimeHierarchyObj(OnDemandProcessingContext odpContext, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, int level)
			: base(odpContext, objectType, level)
		{
		}

		internal RuntimeHierarchyObj(RuntimeHierarchyObj outerHierarchy, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, int level)
			: base(outerHierarchy.OdpContext, objectType, level)
		{
			if (outerHierarchy.m_expression != null)
			{
				ConstructorHelper(outerHierarchy.m_expression.ExpressionIndex + 1, outerHierarchy.m_hierarchyRoot);
			}
			else
			{
				ConstructorHelper(-1, outerHierarchy.m_hierarchyRoot);
			}
		}

		ProcessingMessageList IHierarchyObj.RegisterComparisonError(string propertyName)
		{
			return RegisterComparisonError(propertyName);
		}

		void IHierarchyObj.NextRow(IHierarchyObj owner)
		{
			NextRow();
		}

		void IHierarchyObj.Traverse(ProcessingStages operation, ITraversalContext traversalContext)
		{
			switch (operation)
			{
			case ProcessingStages.SortAndFilter:
				SortAndFilter((AggregateUpdateContext)traversalContext);
				break;
			case ProcessingStages.RunningValues:
				CalculateRunningValues((AggregateUpdateContext)traversalContext);
				break;
			case ProcessingStages.CreateGroupTree:
				CreateInstances((CreateInstancesTraversalContext)traversalContext);
				break;
			case ProcessingStages.UpdateAggregates:
				UpdateAggregates((AggregateUpdateContext)traversalContext);
				break;
			default:
				Global.Tracer.Assert(condition: false);
				break;
			}
		}

		void IHierarchyObj.ReadRow()
		{
			ReadRow(DataActions.UserSort, null);
		}

		void IHierarchyObj.ProcessUserSort()
		{
			ProcessUserSort();
		}

		void IHierarchyObj.MarkSortInfoProcessed(List<IReference<RuntimeSortFilterEventInfo>> runtimeSortFilterInfo)
		{
			MarkSortInfoProcessed(runtimeSortFilterInfo);
		}

		void IHierarchyObj.AddSortInfoIndex(int sortInfoIndex, IReference<RuntimeSortFilterEventInfo> sortInfo)
		{
			AddSortInfoIndex(sortInfoIndex, sortInfo);
		}

		private void ConstructorHelper(int exprIndex, RuntimeHierarchyObjReference hierarchyRoot)
		{
			m_hierarchyRoot = hierarchyRoot;
			using (m_hierarchyRoot.PinValue())
			{
				RuntimeGroupRootObj runtimeGroupRootObj = m_hierarchyRoot.Value() as RuntimeGroupRootObj;
				Global.Tracer.Assert(runtimeGroupRootObj != null, "(null != groupRoot)");
				List<Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo> list;
				IndexedExprHost expressionsHost;
				List<bool> directions;
				if (ProcessingStages.Grouping == runtimeGroupRootObj.ProcessingStage)
				{
					list = runtimeGroupRootObj.GroupExpressions;
					expressionsHost = runtimeGroupRootObj.GroupExpressionHost;
					directions = runtimeGroupRootObj.GroupDirections;
				}
				else
				{
					Global.Tracer.Assert(ProcessingStages.SortAndFilter == runtimeGroupRootObj.ProcessingStage, "(ProcessingStages.SortAndFilter == groupRoot.ProcessingStage)");
					list = runtimeGroupRootObj.SortExpressions;
					expressionsHost = runtimeGroupRootObj.SortExpressionHost;
					directions = runtimeGroupRootObj.SortDirections;
				}
				if (exprIndex == -1 || exprIndex >= list.Count)
				{
					m_hierarchyObjs = new List<IReference<RuntimeHierarchyObj>>();
					RuntimeGroupLeafObjReference runtimeGroupLeafObjReference = null;
					_ = m_odpContext.TablixProcessingScalabilityCache;
					if (ProcessingStages.Grouping == runtimeGroupRootObj.ProcessingStage)
					{
						runtimeGroupLeafObjReference = runtimeGroupRootObj.CreateGroupLeaf();
						if (!runtimeGroupRootObj.HasParent)
						{
							runtimeGroupRootObj.AddChildWithNoParent(runtimeGroupLeafObjReference);
						}
					}
					if (null != runtimeGroupLeafObjReference)
					{
						m_hierarchyObjs.Add(runtimeGroupLeafObjReference);
					}
				}
				else
				{
					m_expression = new RuntimeExpressionInfo(list, expressionsHost, directions, exprIndex);
					m_grouping = RuntimeGroupingObj.CreateGroupingObj(runtimeGroupRootObj.GroupingType, this, m_objectType);
				}
			}
		}

		internal ProcessingMessageList RegisterComparisonError(string propertyName)
		{
			return RegisterComparisonError(propertyName, null);
		}

		internal ProcessingMessageList RegisterComparisonError(string propertyName, ReportProcessingException_ComparisonError e)
		{
			Microsoft.ReportingServices.ReportProcessing.ObjectType objectType;
			string name;
			using (m_hierarchyRoot.PinValue())
			{
				RuntimeGroupRootObj obj = (RuntimeGroupRootObj)m_hierarchyRoot.Value();
				objectType = obj.HierarchyDef.DataRegionDef.ObjectType;
				name = obj.HierarchyDef.DataRegionDef.Name;
			}
			return m_odpContext.RegisterComparisonError(e, objectType, name, propertyName);
		}

		internal ProcessingMessageList RegisterSpatialTypeComparisonError(string type)
		{
			Microsoft.ReportingServices.ReportProcessing.ObjectType objectType;
			string name;
			using (m_hierarchyRoot.PinValue())
			{
				RuntimeGroupRootObj obj = (RuntimeGroupRootObj)m_hierarchyRoot.Value();
				objectType = obj.HierarchyDef.DataRegionDef.ObjectType;
				name = obj.HierarchyDef.DataRegionDef.Name;
			}
			return m_odpContext.RegisterSpatialTypeComparisonError(objectType, name, type);
		}

		internal override void NextRow()
		{
			bool flag = true;
			RuntimeGroupRootObj runtimeGroupRootObj = null;
			using (m_hierarchyRoot.PinValue())
			{
				if (m_hierarchyRoot is RuntimeGroupRootObjReference)
				{
					runtimeGroupRootObj = (RuntimeGroupRootObj)m_hierarchyRoot.Value();
					if (ProcessingStages.SortAndFilter == runtimeGroupRootObj.ProcessingStage)
					{
						flag = false;
					}
				}
				if (m_hierarchyObjs != null)
				{
					if (flag)
					{
						IReference<RuntimeHierarchyObj> reference = m_hierarchyObjs[0];
						Global.Tracer.Assert(reference != null, "(null != hierarchyObj)");
						using (reference.PinValue())
						{
							reference.Value().NextRow();
						}
					}
					else if (runtimeGroupRootObj != null)
					{
						RuntimeGroupLeafObjReference lastChild = runtimeGroupRootObj.LastChild;
						Global.Tracer.Assert(null != lastChild, "(null != groupLastChild)");
						m_hierarchyObjs.Add(lastChild);
					}
				}
				else
				{
					if (m_grouping == null)
					{
						return;
					}
					Microsoft.ReportingServices.ReportProcessing.ObjectType objectType = runtimeGroupRootObj.HierarchyDef.DataRegionDef.ObjectType;
					string name = runtimeGroupRootObj.HierarchyDef.DataRegionDef.Name;
					string propertyName = "GroupExpression";
					DomainScopeContext domainScopeContext = base.OdpContext.DomainScopeContext;
					DomainScopeContext.DomainScopeInfo domainScopeInfo = null;
					if (domainScopeContext != null)
					{
						domainScopeInfo = domainScopeContext.CurrentDomainScope;
					}
					object obj;
					if (domainScopeInfo == null)
					{
						obj = ((m_expression != null) ? m_odpContext.ReportRuntime.EvaluateRuntimeExpression(m_expression, objectType, name, propertyName) : ((object)m_odpContext.ReportObjectModel.FieldsImpl.GetRowIndex()));
					}
					else
					{
						domainScopeInfo.MoveNext();
						obj = domainScopeInfo.CurrentKey;
					}
					if (runtimeGroupRootObj != null && flag)
					{
						Microsoft.ReportingServices.ReportIntermediateFormat.Grouping grouping = runtimeGroupRootObj.HierarchyDef.Grouping;
						if (runtimeGroupRootObj.SaveGroupExprValues)
						{
							grouping.CurrentGroupExpressionValues.Add(obj);
						}
						MatchSortFilterScope(runtimeGroupRootObj.SelfReference, grouping, obj, m_expression.ExpressionIndex);
					}
					m_grouping.NextRow(obj);
					domainScopeInfo?.MovePrevious();
					return;
				}
			}
		}

		internal override bool SortAndFilter(AggregateUpdateContext aggContext)
		{
			Traverse(ProcessingStages.SortAndFilter, aggContext);
			return true;
		}

		public override void UpdateAggregates(AggregateUpdateContext aggContext)
		{
			Traverse(ProcessingStages.UpdateAggregates, aggContext);
		}

		private void Traverse(ProcessingStages operation, AggregateUpdateContext aggContext)
		{
			if (m_grouping != null)
			{
				m_grouping.Traverse(ProcessingStages.SortAndFilter, ascending: true, aggContext);
			}
			if (m_hierarchyObjs == null)
			{
				return;
			}
			for (int i = 0; i < m_hierarchyObjs.Count; i++)
			{
				IReference<RuntimeHierarchyObj> reference = m_hierarchyObjs[i];
				using (reference.PinValue())
				{
					switch (operation)
					{
					case ProcessingStages.SortAndFilter:
						reference.Value().SortAndFilter(aggContext);
						break;
					case ProcessingStages.UpdateAggregates:
						reference.Value().UpdateAggregates(aggContext);
						break;
					}
				}
			}
		}

		internal virtual void CalculateRunningValues(AggregateUpdateContext aggContext)
		{
			if (m_grouping != null)
			{
				m_grouping.Traverse(ProcessingStages.RunningValues, m_expression == null || m_expression.Direction, aggContext);
			}
			if (m_hierarchyObjs == null)
			{
				return;
			}
			bool flag = true;
			for (int i = 0; i < m_hierarchyObjs.Count; i++)
			{
				IReference<RuntimeHierarchyObj> reference = m_hierarchyObjs[i];
				using (reference.PinValue())
				{
					RuntimeHierarchyObj runtimeHierarchyObj = reference.Value();
					if (!flag || runtimeHierarchyObj is RuntimeGroupLeafObj)
					{
						((RuntimeGroupLeafObj)runtimeHierarchyObj).TraverseAllLeafNodes(ProcessingStages.RunningValues, aggContext);
						flag = false;
					}
				}
			}
		}

		internal override void CalculateRunningValues(Dictionary<string, IReference<RuntimeGroupRootObj>> groupCol, IReference<RuntimeGroupRootObj> lastGroup, AggregateUpdateContext aggContext)
		{
			Global.Tracer.Assert(condition: false);
		}

		internal override void CalculatePreviousAggregates()
		{
			Global.Tracer.Assert(condition: false);
		}

		internal void CreateInstances(CreateInstancesTraversalContext traversalContext)
		{
			if (m_grouping != null)
			{
				m_grouping.Traverse(ProcessingStages.CreateGroupTree, m_expression == null || m_expression.Direction, traversalContext);
			}
			if (m_hierarchyObjs == null)
			{
				return;
			}
			bool flag = true;
			for (int i = 0; i < m_hierarchyObjs.Count; i++)
			{
				IReference<RuntimeHierarchyObj> reference = m_hierarchyObjs[i];
				using (reference.PinValue())
				{
					RuntimeHierarchyObj runtimeHierarchyObj = reference.Value();
					if (!flag || runtimeHierarchyObj is RuntimeGroupLeafObj)
					{
						((RuntimeGroupLeafObj)runtimeHierarchyObj).TraverseAllLeafNodes(ProcessingStages.CreateGroupTree, traversalContext);
						flag = false;
					}
					else
					{
						((RuntimeDetailObj)runtimeHierarchyObj).CreateInstance(traversalContext);
					}
				}
			}
		}

		internal virtual void CreateInstance(CreateInstancesTraversalContext traversalContext)
		{
			Global.Tracer.Assert(condition: false);
		}

		public override void SetupEnvironment()
		{
		}

		public override void ReadRow(DataActions dataAction, ITraversalContext context)
		{
			Global.Tracer.Assert(condition: false);
		}

		internal override bool InScope(string scope)
		{
			Global.Tracer.Assert(condition: false);
			return false;
		}

		public virtual IHierarchyObj CreateHierarchyObjForSortTree()
		{
			return new RuntimeHierarchyObj(this, m_objectType, m_depth + 1);
		}

		protected void MatchSortFilterScope(IReference<IScope> outerScope, Microsoft.ReportingServices.ReportIntermediateFormat.Grouping groupDef, object groupExprValue, int groupExprIndex)
		{
			if (m_odpContext.RuntimeSortFilterInfo == null || groupDef.SortFilterScopeInfo == null)
			{
				return;
			}
			List<IReference<RuntimeSortFilterEventInfo>> runtimeSortFilterInfo = m_odpContext.RuntimeSortFilterInfo;
			for (int i = 0; i < runtimeSortFilterInfo.Count; i++)
			{
				List<object> list = groupDef.SortFilterScopeInfo[i];
				if (list != null && outerScope.Value().TargetScopeMatched(i, detailSort: false))
				{
					if (m_odpContext.ProcessingComparer.Compare(list[groupExprIndex], groupExprValue) != 0)
					{
						groupDef.SortFilterScopeMatched[i] = false;
					}
				}
				else
				{
					groupDef.SortFilterScopeMatched[i] = false;
				}
			}
		}

		protected virtual void ProcessUserSort()
		{
			Global.Tracer.Assert(condition: false);
		}

		protected virtual void MarkSortInfoProcessed(List<IReference<RuntimeSortFilterEventInfo>> runtimeSortFilterInfo)
		{
			Global.Tracer.Assert(condition: false);
		}

		protected virtual void AddSortInfoIndex(int sortInfoIndex, IReference<RuntimeSortFilterEventInfo> sortInfo)
		{
			Global.Tracer.Assert(condition: false);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Grouping:
					writer.Write(m_grouping);
					break;
				case MemberName.Expression:
					writer.Write(m_expression);
					break;
				case MemberName.HierarchyRoot:
					writer.Write(m_hierarchyRoot);
					break;
				case MemberName.HierarchyObjs:
					writer.Write(m_hierarchyObjs);
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
				case MemberName.Grouping:
					m_grouping = (RuntimeGroupingObj)reader.ReadRIFObject();
					if (m_grouping != null)
					{
						m_grouping.SetOwner(this);
					}
					break;
				case MemberName.Expression:
					m_expression = (RuntimeExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.HierarchyRoot:
					m_hierarchyRoot = (RuntimeHierarchyObjReference)reader.ReadRIFObject();
					break;
				case MemberName.HierarchyObjs:
					m_hierarchyObjs = reader.ReadListOfRIFObjects<List<IReference<RuntimeHierarchyObj>>>();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeHierarchyObj;
		}

		public new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.Grouping, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupingObj));
				list.Add(new MemberInfo(MemberName.Expression, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeExpressionInfo));
				list.Add(new MemberInfo(MemberName.HierarchyRoot, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeHierarchyObjReference));
				list.Add(new MemberInfo(MemberName.HierarchyObjs, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeHierarchyObjReference));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeHierarchyObj, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataRegionObj, list);
			}
			return m_declaration;
		}
	}
}
