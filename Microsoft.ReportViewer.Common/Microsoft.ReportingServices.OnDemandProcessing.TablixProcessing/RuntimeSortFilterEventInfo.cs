using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class RuntimeSortFilterEventInfo : IStorable, IPersistable, ISelfReferential
	{
		[PersistedWithinRequestOnly]
		internal class SortFilterExpressionScopeObj : IHierarchyObj, IStorable, IPersistable
		{
			private ScalableList<IReference<RuntimeDataRegionObj>> m_scopeInstances;

			private ScalableList<SortScopeValuesHolder> m_scopeValuesList;

			private BTree m_sortTree;

			private int m_currentScopeInstanceIndex = -1;

			[NonSerialized]
			private static Declaration m_declaration = GetDeclaration();

			public int Depth => -1;

			internal int CurrentScopeInstanceIndex => m_currentScopeInstanceIndex;

			IReference<IHierarchyObj> IHierarchyObj.HierarchyRoot => null;

			OnDemandProcessingContext IHierarchyObj.OdpContext
			{
				get
				{
					Global.Tracer.Assert(0 < m_scopeInstances.Count, "(0 < m_scopeInstances.Count)");
					return m_scopeInstances[0].Value().OdpContext;
				}
			}

			BTree IHierarchyObj.SortTree => m_sortTree;

			int IHierarchyObj.ExpressionIndex => 0;

			List<int> IHierarchyObj.SortFilterInfoIndices => null;

			bool IHierarchyObj.IsDetail => false;

			bool IHierarchyObj.InDataRowSortPhase => false;

			public int Size => ItemSizes.SizeOf(m_scopeInstances) + ItemSizes.SizeOf(m_scopeValuesList) + ItemSizes.SizeOf(m_sortTree) + 4;

			internal SortFilterExpressionScopeObj()
			{
			}

			internal SortFilterExpressionScopeObj(IReference<RuntimeSortFilterEventInfo> owner, OnDemandProcessingContext odpContext, int depth)
			{
				m_scopeInstances = new ScalableList<IReference<RuntimeDataRegionObj>>(depth, odpContext.TablixProcessingScalabilityCache);
				m_scopeValuesList = new ScalableList<SortScopeValuesHolder>(depth, odpContext.TablixProcessingScalabilityCache);
			}

			IHierarchyObj IHierarchyObj.CreateHierarchyObjForSortTree()
			{
				return new SortExpressionScopeInstanceHolder(null);
			}

			ProcessingMessageList IHierarchyObj.RegisterComparisonError(string propertyName)
			{
				return ((IHierarchyObj)this).OdpContext.RegisterComparisonErrorForSortFilterEvent(propertyName);
			}

			void IHierarchyObj.NextRow(IHierarchyObj owner)
			{
				Global.Tracer.Assert(condition: false);
			}

			public void Traverse(ProcessingStages operation, ITraversalContext traversalContext)
			{
				UserSortFilterTraversalContext userSortFilterTraversalContext = (UserSortFilterTraversalContext)traversalContext;
				userSortFilterTraversalContext.ExpressionScope = this;
				if (m_sortTree != null)
				{
					m_sortTree.Traverse(operation, userSortFilterTraversalContext.EventInfo.SortDirection, traversalContext);
				}
			}

			void IHierarchyObj.ReadRow()
			{
				Global.Tracer.Assert(condition: false);
			}

			void IHierarchyObj.ProcessUserSort()
			{
				Global.Tracer.Assert(condition: false);
			}

			void IHierarchyObj.MarkSortInfoProcessed(List<IReference<RuntimeSortFilterEventInfo>> runtimeSortFilterInfo)
			{
				Global.Tracer.Assert(condition: false);
			}

			void IHierarchyObj.AddSortInfoIndex(int sortFilterInfoIndex, IReference<RuntimeSortFilterEventInfo> sortInfo)
			{
				Global.Tracer.Assert(condition: false);
			}

			internal void RegisterScopeInstance(IReference<RuntimeDataRegionObj> scopeObj, List<object>[] scopeValues)
			{
				m_scopeInstances.Add(scopeObj);
				m_scopeValuesList.Add(new SortScopeValuesHolder(scopeValues));
			}

			internal void SortSEScopes(OnDemandProcessingContext odpContext, IInScopeEventSource eventSource)
			{
				m_sortTree = new BTree(this, odpContext, Depth + 1);
				for (int i = 0; i < m_scopeInstances.Count; i++)
				{
					IReference<RuntimeDataRegionObj> reference = m_scopeInstances[i];
					m_currentScopeInstanceIndex = i;
					using (reference.PinValue())
					{
						reference.Value().SetupEnvironment();
					}
					m_sortTree.NextRow(odpContext.ReportRuntime.EvaluateUserSortExpression(eventSource), this);
				}
			}

			internal void AddSortOrder(RuntimeSortFilterEventInfo owner, int scopeInstanceIndex, bool incrementCounter)
			{
				owner.AddSortOrder(m_scopeValuesList[scopeInstanceIndex].Values, incrementCounter);
			}

			public void Serialize(IntermediateFormatWriter writer)
			{
				writer.RegisterDeclaration(m_declaration);
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.ScopeInstances:
						writer.Write(m_scopeInstances);
						break;
					case MemberName.SortTree:
						writer.Write(m_sortTree);
						break;
					case MemberName.ScopeValuesList:
						writer.Write(m_scopeValuesList);
						break;
					case MemberName.CurrentScopeIndex:
						writer.Write(m_currentScopeInstanceIndex);
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
					case MemberName.ScopeInstances:
						m_scopeInstances = reader.ReadRIFObject<ScalableList<IReference<RuntimeDataRegionObj>>>();
						break;
					case MemberName.ScopeValuesList:
						m_scopeValuesList = reader.ReadRIFObject<ScalableList<SortScopeValuesHolder>>();
						break;
					case MemberName.SortTree:
						m_sortTree = (BTree)reader.ReadRIFObject();
						break;
					case MemberName.CurrentScopeIndex:
						m_currentScopeInstanceIndex = reader.ReadInt32();
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
				return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortFilterExpressionScopeObj;
			}

			internal static Declaration GetDeclaration()
			{
				if (m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.ScopeInstances, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataRegionObjReference));
					list.Add(new MemberInfo(MemberName.ScopeValuesList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortScopeValuesHolder));
					list.Add(new MemberInfo(MemberName.SortTree, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BTree));
					list.Add(new MemberInfo(MemberName.CurrentScopeIndex, Token.Int32));
					return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortFilterExpressionScopeObj, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
				}
				return m_declaration;
			}
		}

		[PersistedWithinRequestOnly]
		internal class SortScopeValuesHolder : IStorable, IPersistable
		{
			private List<object>[] m_values;

			[NonSerialized]
			private static Declaration m_declaration = GetDeclaration();

			public List<object>[] Values => m_values;

			public int Size => ItemSizes.SizeOf(m_values);

			public SortScopeValuesHolder()
			{
			}

			public SortScopeValuesHolder(List<object>[] values)
			{
				m_values = values;
			}

			public void Serialize(IntermediateFormatWriter writer)
			{
				writer.RegisterDeclaration(m_declaration);
				while (writer.NextMember())
				{
					MemberName memberName = writer.CurrentMember.MemberName;
					if (memberName == MemberName.Values)
					{
						writer.WriteArrayOfListsOfPrimitives(m_values);
					}
					else
					{
						Global.Tracer.Assert(condition: false);
					}
				}
			}

			public void Deserialize(IntermediateFormatReader reader)
			{
				reader.RegisterDeclaration(m_declaration);
				while (reader.NextMember())
				{
					MemberName memberName = reader.CurrentMember.MemberName;
					if (memberName == MemberName.Values)
					{
						m_values = reader.ReadArrayOfListsOfPrimitives<object>();
					}
					else
					{
						Global.Tracer.Assert(condition: false);
					}
				}
			}

			public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
			{
			}

			public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
			{
				return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortScopeValuesHolder;
			}

			internal static Declaration GetDeclaration()
			{
				if (m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.Values, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveArray, Token.Object, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList));
					return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortScopeValuesHolder, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
				}
				return m_declaration;
			}
		}

		[PersistedWithinRequestOnly]
		internal class SortExpressionScopeInstanceHolder : IHierarchyObj, IStorable, IPersistable
		{
			private List<int> m_scopeInstanceIndices;

			[NonSerialized]
			private static Declaration m_declaration = GetDeclaration();

			public int Depth => -1;

			IReference<IHierarchyObj> IHierarchyObj.HierarchyRoot => null;

			OnDemandProcessingContext IHierarchyObj.OdpContext => null;

			BTree IHierarchyObj.SortTree => null;

			int IHierarchyObj.ExpressionIndex => -1;

			List<int> IHierarchyObj.SortFilterInfoIndices => null;

			bool IHierarchyObj.IsDetail => false;

			bool IHierarchyObj.InDataRowSortPhase => false;

			public int Size => ItemSizes.SizeOf(m_scopeInstanceIndices);

			internal SortExpressionScopeInstanceHolder()
			{
			}

			internal SortExpressionScopeInstanceHolder(OnDemandProcessingContext odpContext)
			{
				m_scopeInstanceIndices = new List<int>();
			}

			IHierarchyObj IHierarchyObj.CreateHierarchyObjForSortTree()
			{
				Global.Tracer.Assert(condition: false);
				return null;
			}

			ProcessingMessageList IHierarchyObj.RegisterComparisonError(string propertyName)
			{
				Global.Tracer.Assert(condition: false);
				return null;
			}

			void IHierarchyObj.NextRow(IHierarchyObj owner)
			{
				m_scopeInstanceIndices.Add(((SortFilterExpressionScopeObj)owner).CurrentScopeInstanceIndex);
			}

			void IHierarchyObj.Traverse(ProcessingStages operation, ITraversalContext traversalContext)
			{
				UserSortFilterTraversalContext obj = (UserSortFilterTraversalContext)traversalContext;
				RuntimeSortFilterEventInfo eventInfo = obj.EventInfo;
				SortFilterExpressionScopeObj expressionScope = obj.ExpressionScope;
				if (eventInfo.SortDirection)
				{
					for (int i = 0; i < m_scopeInstanceIndices.Count; i++)
					{
						expressionScope.AddSortOrder(eventInfo, m_scopeInstanceIndices[i], i == m_scopeInstanceIndices.Count - 1);
					}
					return;
				}
				for (int num = m_scopeInstanceIndices.Count - 1; num >= 0; num--)
				{
					expressionScope.AddSortOrder(eventInfo, m_scopeInstanceIndices[num], num == 0);
				}
			}

			void IHierarchyObj.ReadRow()
			{
				Global.Tracer.Assert(condition: false);
			}

			void IHierarchyObj.ProcessUserSort()
			{
				Global.Tracer.Assert(condition: false);
			}

			void IHierarchyObj.MarkSortInfoProcessed(List<IReference<RuntimeSortFilterEventInfo>> runtimeSortFilterInfo)
			{
				Global.Tracer.Assert(condition: false);
			}

			void IHierarchyObj.AddSortInfoIndex(int sortFilterInfoIndex, IReference<RuntimeSortFilterEventInfo> sortInfo)
			{
				Global.Tracer.Assert(condition: false);
			}

			void IPersistable.Serialize(IntermediateFormatWriter writer)
			{
				writer.RegisterDeclaration(m_declaration);
				while (writer.NextMember())
				{
					MemberName memberName = writer.CurrentMember.MemberName;
					if (memberName == MemberName.ScopeInstanceIndices)
					{
						writer.WriteListOfPrimitives(m_scopeInstanceIndices);
					}
					else
					{
						Global.Tracer.Assert(condition: false);
					}
				}
			}

			void IPersistable.Deserialize(IntermediateFormatReader reader)
			{
				reader.RegisterDeclaration(m_declaration);
				while (reader.NextMember())
				{
					MemberName memberName = reader.CurrentMember.MemberName;
					if (memberName == MemberName.ScopeInstanceIndices)
					{
						m_scopeInstanceIndices = reader.ReadListOfPrimitives<int>();
					}
					else
					{
						Global.Tracer.Assert(condition: false);
					}
				}
			}

			void IPersistable.ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
			{
			}

			Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IPersistable.GetObjectType()
			{
				return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortExpressionScopeInstanceHolder;
			}

			internal static Declaration GetDeclaration()
			{
				if (m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.ScopeInstanceIndices, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Int32));
					return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortExpressionScopeInstanceHolder, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
				}
				return m_declaration;
			}
		}

		[StaticReference]
		private IInScopeEventSource m_eventSource;

		private string m_oldUniqueName;

		private List<object>[] m_sortSourceScopeInfo;

		private bool m_sortDirection;

		private IReference<IScope> m_eventSourceRowScope;

		private IReference<IScope> m_eventSourceColScope;

		private int m_eventSourceColDetailIndex = -1;

		private int m_eventSourceRowDetailIndex = -1;

		private List<IReference<RuntimeDataRegionObj>> m_detailRowScopes;

		private List<IReference<RuntimeDataRegionObj>> m_detailColScopes;

		private List<int> m_detailRowScopeIndices;

		private List<int> m_detailColScopeIndices;

		private IReference<IHierarchyObj> m_eventTarget;

		private bool m_targetSortFilterInfoAdded;

		private List<RuntimeExpressionInfo> m_groupExpressionsInSortTarget;

		private List<SortFilterExpressionScopeObj> m_sortFilterExpressionScopeObjects;

		private int m_currentSortIndex = 1;

		private int m_currentInstanceIndex;

		private Microsoft.ReportingServices.ReportIntermediateFormat.ScopeLookupTable m_sortOrders;

		private bool m_processed;

		private int m_nullScopeCount;

		private string m_newUniqueName;

		private int m_depth;

		private Hashtable m_peerSortFilters;

		[NonSerialized]
		private IReference<RuntimeSortFilterEventInfo> m_selfReference;

		private static Declaration m_declaration = GetDeclaration();

		internal IInScopeEventSource EventSource => m_eventSource;

		internal bool HasEventSourceScope
		{
			get
			{
				if (m_eventSourceColScope == null)
				{
					return m_eventSourceRowScope != null;
				}
				return true;
			}
		}

		internal bool HasDetailScopeInfo
		{
			get
			{
				if (m_detailColScopes == null)
				{
					return m_detailRowScopes != null;
				}
				return true;
			}
		}

		internal int EventSourceColDetailIndex
		{
			get
			{
				return m_eventSourceColDetailIndex;
			}
			set
			{
				m_eventSourceColDetailIndex = value;
			}
		}

		internal int EventSourceRowDetailIndex
		{
			get
			{
				return m_eventSourceRowDetailIndex;
			}
			set
			{
				m_eventSourceRowDetailIndex = value;
			}
		}

		internal List<IReference<RuntimeDataRegionObj>> DetailRowScopes
		{
			get
			{
				return m_detailRowScopes;
			}
			set
			{
				m_detailRowScopes = value;
			}
		}

		internal List<IReference<RuntimeDataRegionObj>> DetailColScopes
		{
			get
			{
				return m_detailColScopes;
			}
			set
			{
				m_detailColScopes = value;
			}
		}

		internal List<int> DetailRowScopeIndices
		{
			get
			{
				return m_detailRowScopeIndices;
			}
			set
			{
				m_detailRowScopeIndices = value;
			}
		}

		internal List<int> DetailColScopeIndices
		{
			get
			{
				return m_detailColScopeIndices;
			}
			set
			{
				m_detailColScopeIndices = value;
			}
		}

		internal bool SortDirection
		{
			get
			{
				return m_sortDirection;
			}
			set
			{
				m_sortDirection = value;
			}
		}

		internal List<object>[] SortSourceScopeInfo => m_sortSourceScopeInfo;

		internal IReference<IHierarchyObj> EventTarget
		{
			get
			{
				return m_eventTarget;
			}
			set
			{
				m_eventTarget = value;
			}
		}

		internal bool TargetSortFilterInfoAdded
		{
			get
			{
				return m_targetSortFilterInfoAdded;
			}
			set
			{
				m_targetSortFilterInfoAdded = value;
			}
		}

		internal bool Processed
		{
			get
			{
				return m_processed;
			}
			set
			{
				m_processed = value;
			}
		}

		internal string OldUniqueName => m_oldUniqueName;

		internal string NewUniqueName
		{
			get
			{
				return m_newUniqueName;
			}
			set
			{
				m_newUniqueName = value;
			}
		}

		internal Hashtable PeerSortFilters
		{
			get
			{
				return m_peerSortFilters;
			}
			set
			{
				m_peerSortFilters = value;
			}
		}

		internal IReference<RuntimeSortFilterEventInfo> SelfReference => m_selfReference;

		public int Size => ItemSizes.ReferenceSize + 4 + ItemSizes.SizeOf(m_sortSourceScopeInfo) + 1 + ItemSizes.SizeOf(m_eventSourceRowScope) + ItemSizes.SizeOf(m_eventSourceColScope) + 4 + 4 + ItemSizes.SizeOf(m_detailRowScopes) + ItemSizes.SizeOf(m_detailColScopes) + ItemSizes.SizeOf(m_detailRowScopeIndices) + ItemSizes.SizeOf(m_detailColScopeIndices) + ItemSizes.SizeOf(m_eventTarget) + 1 + ItemSizes.SizeOf(m_groupExpressionsInSortTarget) + ItemSizes.SizeOf(m_sortFilterExpressionScopeObjects) + 4 + 4 + ItemSizes.SizeOf(m_sortOrders) + 1 + 4 + 4 + 4 + ItemSizes.SizeOf(m_peerSortFilters) + 4 + ItemSizes.SizeOf(m_selfReference);

		internal RuntimeSortFilterEventInfo()
		{
		}

		internal RuntimeSortFilterEventInfo(IInScopeEventSource eventSource, string oldUniqueName, bool sortDirection, List<object>[] sortSourceScopeInfo, OnDemandProcessingContext odpContext, int depth)
		{
			m_depth = depth;
			odpContext.TablixProcessingScalabilityCache.AllocateAndPin(this, m_depth);
			m_eventSource = eventSource;
			m_oldUniqueName = oldUniqueName;
			m_sortDirection = sortDirection;
			m_sortSourceScopeInfo = sortSourceScopeInfo;
		}

		internal IReference<IScope> GetEventSourceScope(bool isColumnAxis)
		{
			if (!isColumnAxis)
			{
				return m_eventSourceRowScope;
			}
			return m_eventSourceColScope;
		}

		internal void SetEventSourceScope(bool isColumnAxis, IReference<IScope> eventSourceScope, int rowIndex)
		{
			if (isColumnAxis)
			{
				Global.Tracer.Assert(m_eventSourceColScope == null);
				m_eventSourceColScope = eventSourceScope;
				m_eventSourceColDetailIndex = rowIndex;
			}
			else
			{
				Global.Tracer.Assert(m_eventSourceRowScope == null);
				m_eventSourceRowScope = eventSourceScope;
				m_eventSourceRowDetailIndex = rowIndex;
			}
		}

		internal void UpdateEventSourceScope(bool isColumnAxis, IReference<IScope> eventSourceScope, int rootRowCount)
		{
			if (isColumnAxis)
			{
				m_eventSourceColScope = eventSourceScope;
				m_eventSourceColDetailIndex += rootRowCount;
			}
			else
			{
				m_eventSourceRowScope = eventSourceScope;
				m_eventSourceRowDetailIndex += rootRowCount;
			}
		}

		internal void AddDetailScopeInfo(bool isColumnAxis, RuntimeDataRegionObjReference dataRegionReference, int detailRowIndex)
		{
			if (m_detailRowScopes == null)
			{
				m_detailRowScopes = new List<IReference<RuntimeDataRegionObj>>();
				m_detailRowScopeIndices = new List<int>();
				m_detailColScopes = new List<IReference<RuntimeDataRegionObj>>();
				m_detailColScopeIndices = new List<int>();
			}
			if (isColumnAxis)
			{
				m_detailColScopes.Add(dataRegionReference);
				m_detailColScopeIndices.Add(detailRowIndex);
			}
			else
			{
				m_detailRowScopes.Add(dataRegionReference);
				m_detailRowScopeIndices.Add(detailRowIndex);
			}
		}

		internal void UpdateDetailScopeInfo(RuntimeGroupRootObj detailRoot, bool isColumnAxis, int rootRowCount, RuntimeDataRegionObjReference selfReference)
		{
			List<IReference<RuntimeDataRegionObj>> list;
			List<int> list2;
			if (isColumnAxis)
			{
				list = m_detailColScopes;
				list2 = m_detailColScopeIndices;
			}
			else
			{
				list = m_detailRowScopes;
				list2 = m_detailRowScopeIndices;
			}
			for (int i = 0; i < list.Count; i++)
			{
				if (selfReference == list[i])
				{
					list[i] = detailRoot.SelfReference;
					list2[i] += rootRowCount;
				}
			}
		}

		internal void RegisterSortFilterExpressionScope(ref int containerSortFilterExprScopeIndex, IReference<RuntimeDataRegionObj> scopeObj, List<object>[] scopeValues, int sortFilterInfoIndex)
		{
			if (m_eventTarget != null && !m_targetSortFilterInfoAdded)
			{
				using (m_eventTarget.PinValue())
				{
					m_eventTarget.Value().AddSortInfoIndex(sortFilterInfoIndex, SelfReference);
				}
			}
			SortFilterExpressionScopeObj sortFilterExpressionScopeObj = null;
			if (-1 != containerSortFilterExprScopeIndex)
			{
				sortFilterExpressionScopeObj = m_sortFilterExpressionScopeObjects[containerSortFilterExprScopeIndex];
			}
			else
			{
				if (m_sortFilterExpressionScopeObjects == null)
				{
					m_sortFilterExpressionScopeObjects = new List<SortFilterExpressionScopeObj>();
				}
				containerSortFilterExprScopeIndex = m_sortFilterExpressionScopeObjects.Count;
				sortFilterExpressionScopeObj = new SortFilterExpressionScopeObj(m_selfReference, scopeObj.Value().OdpContext, m_depth + 1);
				m_sortFilterExpressionScopeObjects.Add(sortFilterExpressionScopeObj);
			}
			sortFilterExpressionScopeObj.RegisterScopeInstance(scopeObj, scopeValues);
		}

		internal void PrepareForSorting(OnDemandProcessingContext odpContext)
		{
			Global.Tracer.Assert(!m_processed, "(!m_processed)");
			if (m_eventTarget == null || m_sortFilterExpressionScopeObjects == null)
			{
				return;
			}
			odpContext.UserSortFilterContext.CurrentSortFilterEventSource = m_eventSource;
			for (int i = 0; i < m_sortFilterExpressionScopeObjects.Count; i++)
			{
				m_sortFilterExpressionScopeObjects[i].SortSEScopes(odpContext, m_eventSource);
			}
			Microsoft.ReportingServices.ReportIntermediateFormat.GroupingList groupsInSortTarget = m_eventSource.UserSort.GroupsInSortTarget;
			if (groupsInSortTarget != null && 0 < groupsInSortTarget.Count)
			{
				m_groupExpressionsInSortTarget = new List<RuntimeExpressionInfo>();
				for (int j = 0; j < groupsInSortTarget.Count; j++)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.Grouping grouping = groupsInSortTarget[j];
					for (int k = 0; k < grouping.GroupExpressions.Count; k++)
					{
						m_groupExpressionsInSortTarget.Add(new RuntimeExpressionInfo(grouping.GroupExpressions, grouping.ExprHost, null, k));
					}
				}
			}
			CollectSortOrders();
		}

		private void CollectSortOrders()
		{
			m_currentSortIndex = 1;
			UserSortFilterTraversalContext traversalContext = new UserSortFilterTraversalContext(this);
			for (int i = 0; i < m_sortFilterExpressionScopeObjects.Count; i++)
			{
				m_sortFilterExpressionScopeObjects[i].Traverse(ProcessingStages.UserSortFilter, traversalContext);
			}
			m_sortFilterExpressionScopeObjects = null;
		}

		internal bool ProcessSorting(OnDemandProcessingContext odpContext)
		{
			Global.Tracer.Assert(!m_processed, "(!m_processed)");
			if (m_eventTarget == null)
			{
				return false;
			}
			using (m_eventTarget.PinValue())
			{
				m_eventTarget.Value().ProcessUserSort();
			}
			m_sortOrders = null;
			return true;
		}

		private void AddSortOrder(List<object>[] scopeValues, bool incrementCounter)
		{
			if (m_sortOrders == null)
			{
				m_sortOrders = new Microsoft.ReportingServices.ReportIntermediateFormat.ScopeLookupTable();
			}
			if (scopeValues == null || scopeValues.Length == 0)
			{
				m_sortOrders.Add(m_eventSource.UserSort.GroupsInSortTarget, scopeValues, m_currentSortIndex);
			}
			else
			{
				int num = 0;
				for (int i = 0; i < scopeValues.Length; i++)
				{
					if (scopeValues[i] == null)
					{
						num++;
					}
				}
				if (num >= m_nullScopeCount)
				{
					if (num > m_nullScopeCount)
					{
						m_sortOrders.Clear();
						m_nullScopeCount = num;
					}
					m_sortOrders.Add(m_eventSource.UserSort.GroupsInSortTarget, scopeValues, m_currentSortIndex);
				}
			}
			if (incrementCounter)
			{
				m_currentSortIndex++;
			}
		}

		internal object GetSortOrder(Microsoft.ReportingServices.RdlExpressions.ReportRuntime runtime)
		{
			object obj = null;
			if (m_eventSource.UserSort.SortExpressionScope == null)
			{
				obj = runtime.EvaluateUserSortExpression(m_eventSource);
			}
			else if (m_sortOrders == null)
			{
				obj = null;
			}
			else
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.GroupingList groupsInSortTarget = m_eventSource.UserSort.GroupsInSortTarget;
				if (groupsInSortTarget == null || groupsInSortTarget.Count == 0)
				{
					obj = m_sortOrders.LookupTable;
				}
				else
				{
					bool flag = true;
					bool flag2 = false;
					int num = 0;
					Hashtable hashtable = m_sortOrders.LookupTable;
					int num2 = 0;
					int num3 = 0;
					while (num3 < groupsInSortTarget.Count)
					{
						IEnumerator enumerator = hashtable.Keys.GetEnumerator();
						enumerator.MoveNext();
						num2 = (int)enumerator.Current;
						for (int i = 0; i < num2; i++)
						{
							num += groupsInSortTarget[num3++].GroupExpressions.Count;
						}
						hashtable = (Hashtable)hashtable[num2];
						if (num3 < groupsInSortTarget.Count)
						{
							Microsoft.ReportingServices.ReportIntermediateFormat.Grouping grouping = groupsInSortTarget[num3];
							for (int j = 0; j < grouping.GroupExpressions.Count; j++)
							{
								object key = runtime.EvaluateRuntimeExpression(m_groupExpressionsInSortTarget[num], Microsoft.ReportingServices.ReportProcessing.ObjectType.Grouping, grouping.Name, "GroupExpression");
								num++;
								obj = hashtable[key];
								if (obj == null)
								{
									flag = false;
									break;
								}
								if (num < m_groupExpressionsInSortTarget.Count)
								{
									hashtable = (Hashtable)obj;
								}
							}
							num3++;
							if (!flag)
							{
								break;
							}
							continue;
						}
						flag2 = true;
						break;
					}
					if (flag && flag2)
					{
						obj = hashtable[1];
						if (obj == null)
						{
							flag = false;
						}
					}
					if (flag)
					{
						m_currentInstanceIndex = m_currentSortIndex + 1;
					}
					else
					{
						obj = m_currentInstanceIndex;
					}
				}
			}
			if (obj == null)
			{
				obj = DBNull.Value;
			}
			return obj;
		}

		internal void MatchEventSource(IInScopeEventSource eventSource, string eventSourceUniqueNameString, IScope containingScope, OnDemandProcessingContext odpContext)
		{
			bool flag = false;
			if (!(containingScope is RuntimeCell))
			{
				while (containingScope != null && !(containingScope is RuntimeGroupLeafObj) && !(containingScope is RuntimeDetailObj))
				{
					containingScope = containingScope.GetOuterScope(includeSubReportContainingScope: true)?.Value();
				}
			}
			if (containingScope == null)
			{
				if (m_eventSource.ContainingScopes == null || m_eventSource.ContainingScopes.Count == 0)
				{
					flag = true;
				}
			}
			else if ((m_eventSourceRowScope != null && m_eventSourceRowScope.Value() == containingScope) || (m_eventSourceColScope != null && m_eventSourceColScope.Value() == containingScope))
			{
				flag = true;
				Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataRegion = null;
				bool flag2 = false;
				RuntimeGroupLeafObj runtimeGroupLeafObj = containingScope as RuntimeGroupLeafObj;
				if (runtimeGroupLeafObj != null && runtimeGroupLeafObj.MemberDef.Grouping.IsDetail)
				{
					dataRegion = runtimeGroupLeafObj.MemberDef.DataRegionDef;
					flag2 = runtimeGroupLeafObj.MemberDef.IsColumn;
				}
				if (dataRegion != null)
				{
					if (flag2 && dataRegion.CurrentColDetailIndex != m_eventSourceColDetailIndex)
					{
						flag = false;
					}
					else if (!flag2 && dataRegion.CurrentRowDetailIndex != m_eventSourceRowDetailIndex)
					{
						flag = false;
					}
				}
			}
			if (flag)
			{
				if (eventSource == m_eventSource)
				{
					m_newUniqueName = eventSourceUniqueNameString;
				}
				else if (m_peerSortFilters != null && m_peerSortFilters.Contains(eventSource.ID))
				{
					m_peerSortFilters[eventSource.ID] = eventSourceUniqueNameString;
				}
			}
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_declaration);
			IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.EventSource:
				{
					int value = scalabilityCache.StoreStaticReference(m_eventSource);
					writer.Write(value);
					break;
				}
				case MemberName.OldUniqueName:
					writer.Write(m_oldUniqueName);
					break;
				case MemberName.SortSourceScopeInfo:
					writer.WriteArrayOfListsOfPrimitives(m_sortSourceScopeInfo);
					break;
				case MemberName.SortDirection:
					writer.Write(m_sortDirection);
					break;
				case MemberName.EventSourceRowScope:
					writer.Write(m_eventSourceRowScope);
					break;
				case MemberName.EventSourceColScope:
					writer.Write(m_eventSourceColScope);
					break;
				case MemberName.EventSourceColDetailIndex:
					writer.Write(m_eventSourceColDetailIndex);
					break;
				case MemberName.EventSourceRowDetailIndex:
					writer.Write(m_eventSourceRowDetailIndex);
					break;
				case MemberName.DetailRowScopes:
					writer.Write(m_detailRowScopes);
					break;
				case MemberName.DetailColScopes:
					writer.Write(m_detailColScopes);
					break;
				case MemberName.DetailRowScopeIndices:
					writer.WriteListOfPrimitives(m_detailRowScopeIndices);
					break;
				case MemberName.DetailColScopeIndices:
					writer.WriteListOfPrimitives(m_detailColScopeIndices);
					break;
				case MemberName.EventTarget:
					writer.Write(m_eventTarget);
					break;
				case MemberName.TargetSortFilterInfoAdded:
					writer.Write(m_targetSortFilterInfoAdded);
					break;
				case MemberName.GroupExpressionsInSortTarget:
					writer.Write(m_groupExpressionsInSortTarget);
					break;
				case MemberName.SortFilterExpressionScopeObjects:
					writer.Write(m_sortFilterExpressionScopeObjects);
					break;
				case MemberName.CurrentSortIndex:
					writer.Write(m_currentSortIndex);
					break;
				case MemberName.CurrentInstanceIndex:
					writer.Write(m_currentInstanceIndex);
					break;
				case MemberName.SortOrders:
					writer.Write(m_sortOrders);
					break;
				case MemberName.Processed:
					writer.Write(m_processed);
					break;
				case MemberName.NullScopeCount:
					writer.Write(m_nullScopeCount);
					break;
				case MemberName.NewUniqueName:
					writer.Write(m_newUniqueName);
					break;
				case MemberName.PeerSortFilters:
					writer.WriteInt32StringHashtable(m_peerSortFilters);
					break;
				case MemberName.Depth:
					writer.Write(m_depth);
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
			IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.EventSource:
				{
					int id = reader.ReadInt32();
					m_eventSource = (IInScopeEventSource)scalabilityCache.FetchStaticReference(id);
					break;
				}
				case MemberName.OldUniqueName:
					m_oldUniqueName = reader.ReadString();
					break;
				case MemberName.SortSourceScopeInfo:
					m_sortSourceScopeInfo = reader.ReadArrayOfListsOfPrimitives<object>();
					break;
				case MemberName.SortDirection:
					m_sortDirection = reader.ReadBoolean();
					break;
				case MemberName.EventSourceRowScope:
					m_eventSourceRowScope = (IReference<IScope>)reader.ReadRIFObject();
					break;
				case MemberName.EventSourceColScope:
					m_eventSourceColScope = (IReference<IScope>)reader.ReadRIFObject();
					break;
				case MemberName.EventSourceColDetailIndex:
					m_eventSourceColDetailIndex = reader.ReadInt32();
					break;
				case MemberName.EventSourceRowDetailIndex:
					m_eventSourceRowDetailIndex = reader.ReadInt32();
					break;
				case MemberName.DetailRowScopes:
					m_detailRowScopes = reader.ReadListOfRIFObjects<List<IReference<RuntimeDataRegionObj>>>();
					break;
				case MemberName.DetailColScopes:
					m_detailColScopes = reader.ReadListOfRIFObjects<List<IReference<RuntimeDataRegionObj>>>();
					break;
				case MemberName.DetailRowScopeIndices:
					m_detailRowScopeIndices = reader.ReadListOfPrimitives<int>();
					break;
				case MemberName.DetailColScopeIndices:
					m_detailColScopeIndices = reader.ReadListOfPrimitives<int>();
					break;
				case MemberName.EventTarget:
					m_eventTarget = (IReference<IHierarchyObj>)reader.ReadRIFObject();
					break;
				case MemberName.TargetSortFilterInfoAdded:
					m_targetSortFilterInfoAdded = reader.ReadBoolean();
					break;
				case MemberName.GroupExpressionsInSortTarget:
					m_groupExpressionsInSortTarget = reader.ReadListOfRIFObjects<List<RuntimeExpressionInfo>>();
					break;
				case MemberName.SortFilterExpressionScopeObjects:
					m_sortFilterExpressionScopeObjects = reader.ReadListOfRIFObjects<List<SortFilterExpressionScopeObj>>();
					break;
				case MemberName.CurrentSortIndex:
					m_currentSortIndex = reader.ReadInt32();
					break;
				case MemberName.CurrentInstanceIndex:
					m_currentInstanceIndex = reader.ReadInt32();
					break;
				case MemberName.SortOrders:
					m_sortOrders = (Microsoft.ReportingServices.ReportIntermediateFormat.ScopeLookupTable)reader.ReadRIFObject();
					break;
				case MemberName.Processed:
					m_processed = reader.ReadBoolean();
					break;
				case MemberName.NullScopeCount:
					m_nullScopeCount = reader.ReadInt32();
					break;
				case MemberName.NewUniqueName:
					m_newUniqueName = reader.ReadString();
					break;
				case MemberName.PeerSortFilters:
					m_peerSortFilters = reader.ReadInt32StringHashtable();
					break;
				case MemberName.Depth:
					m_depth = reader.ReadInt32();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeSortFilterEventInfo;
		}

		public static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.EventSource, Token.Int32));
				list.Add(new MemberInfo(MemberName.OldUniqueName, Token.String));
				list.Add(new MemberInfo(MemberName.SortSourceScopeInfo, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveArray, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList));
				list.Add(new MemberInfo(MemberName.SortDirection, Token.Boolean));
				list.Add(new MemberInfo(MemberName.EventSourceRowScope, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IScopeReference));
				list.Add(new MemberInfo(MemberName.EventSourceColScope, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IScopeReference));
				list.Add(new MemberInfo(MemberName.EventSourceRowDetailIndex, Token.Int32));
				list.Add(new MemberInfo(MemberName.EventSourceColDetailIndex, Token.Int32));
				list.Add(new MemberInfo(MemberName.DetailRowScopes, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataRegionObjReference));
				list.Add(new MemberInfo(MemberName.DetailColScopes, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataRegionObjReference));
				list.Add(new MemberInfo(MemberName.DetailRowScopeIndices, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Int32));
				list.Add(new MemberInfo(MemberName.DetailColScopeIndices, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Int32));
				list.Add(new MemberInfo(MemberName.EventTarget, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IHierarchyObjReference));
				list.Add(new MemberInfo(MemberName.TargetSortFilterInfoAdded, Token.Boolean));
				list.Add(new MemberInfo(MemberName.GroupExpressionsInSortTarget, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeExpressionInfo));
				list.Add(new MemberInfo(MemberName.SortFilterExpressionScopeObjects, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortFilterExpressionScopeObj));
				list.Add(new MemberInfo(MemberName.CurrentSortIndex, Token.Int32));
				list.Add(new MemberInfo(MemberName.CurrentInstanceIndex, Token.Int32));
				list.Add(new MemberInfo(MemberName.SortOrders, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScopeLookupTable));
				list.Add(new MemberInfo(MemberName.Processed, Token.Boolean));
				list.Add(new MemberInfo(MemberName.NullScopeCount, Token.Int32));
				list.Add(new MemberInfo(MemberName.NewUniqueName, Token.String));
				list.Add(new MemberInfo(MemberName.PeerSortFilters, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Int32StringHashtable));
				list.Add(new MemberInfo(MemberName.Depth, Token.Int32));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeSortFilterEventInfo, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return m_declaration;
		}

		public void SetReference(IReference selfRef)
		{
			m_selfReference = (IReference<RuntimeSortFilterEventInfo>)selfRef;
		}
	}
}
