using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class RuntimeDataRowSortHierarchyObj : IHierarchyObj, IStorable, IPersistable
	{
		private IReference<IHierarchyObj> m_hierarchyRoot;

		private RuntimeSortDataHolder m_dataRowHolder;

		private RuntimeExpressionInfo m_sortExpression;

		private BTree m_sortTree;

		private static readonly Declaration m_declaration = GetDeclaration();

		public int Depth => m_hierarchyRoot.Value().Depth + 1;

		IReference<IHierarchyObj> IHierarchyObj.HierarchyRoot => m_hierarchyRoot;

		public OnDemandProcessingContext OdpContext => m_hierarchyRoot.Value().OdpContext;

		BTree IHierarchyObj.SortTree => m_sortTree;

		int IHierarchyObj.ExpressionIndex
		{
			get
			{
				Global.Tracer.Assert(m_sortExpression != null, "m_sortExpression != null");
				return m_sortExpression.ExpressionIndex;
			}
		}

		List<int> IHierarchyObj.SortFilterInfoIndices
		{
			get
			{
				Global.Tracer.Assert(condition: false, "SortFilterInfoIndices should not be called on this type");
				return null;
			}
		}

		bool IHierarchyObj.IsDetail => false;

		bool IHierarchyObj.InDataRowSortPhase => true;

		public int Size => ItemSizes.SizeOf(m_hierarchyRoot) + ItemSizes.SizeOf(m_dataRowHolder) + ItemSizes.SizeOf(m_sortExpression) + ItemSizes.SizeOf(m_sortTree);

		internal RuntimeDataRowSortHierarchyObj()
		{
		}

		internal RuntimeDataRowSortHierarchyObj(IHierarchyObj outerHierarchy, int depth)
		{
			m_hierarchyRoot = outerHierarchy.HierarchyRoot;
			int num = outerHierarchy.ExpressionIndex + 1;
			Microsoft.ReportingServices.ReportIntermediateFormat.Sorting sortingDef = ((IDataRowSortOwner)m_hierarchyRoot.Value()).SortingDef;
			if (sortingDef.SortExpressions == null || num >= sortingDef.SortExpressions.Count)
			{
				m_dataRowHolder = new RuntimeSortDataHolder();
				return;
			}
			m_sortExpression = new RuntimeExpressionInfo(sortingDef.SortExpressions, sortingDef.ExprHost, sortingDef.SortDirections, num);
			m_sortTree = new BTree(this, OdpContext, depth);
		}

		IHierarchyObj IHierarchyObj.CreateHierarchyObjForSortTree()
		{
			return new RuntimeDataRowSortHierarchyObj(this, Depth + 1);
		}

		ProcessingMessageList IHierarchyObj.RegisterComparisonError(string propertyName)
		{
			return m_hierarchyRoot.Value().RegisterComparisonError(propertyName);
		}

		void IHierarchyObj.NextRow(IHierarchyObj owner)
		{
			if (m_dataRowHolder != null)
			{
				m_dataRowHolder.NextRow(owner.OdpContext, owner.Depth);
				return;
			}
			object keyValue = ((IDataRowSortOwner)m_hierarchyRoot.Value()).EvaluateDataRowSortExpression(m_sortExpression);
			m_sortTree.NextRow(keyValue, this);
		}

		void IHierarchyObj.Traverse(ProcessingStages operation, ITraversalContext traversalContext)
		{
			if (m_dataRowHolder != null)
			{
				m_dataRowHolder.Traverse(operation, traversalContext, this);
			}
			else
			{
				m_sortTree.Traverse(operation, m_sortExpression.Direction, traversalContext);
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

		void IHierarchyObj.AddSortInfoIndex(int sortInfoIndex, IReference<RuntimeSortFilterEventInfo> sortInfo)
		{
			Global.Tracer.Assert(condition: false);
		}

		void IPersistable.Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_declaration);
			_ = writer.PersistenceHelper;
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.HierarchyRoot:
					writer.Write(m_hierarchyRoot);
					break;
				case MemberName.DataRowHolder:
					writer.Write(m_dataRowHolder);
					break;
				case MemberName.Expression:
					writer.Write(m_sortExpression);
					break;
				case MemberName.SortTree:
					writer.Write(m_sortTree);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		void IPersistable.Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_declaration);
			_ = reader.PersistenceHelper;
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.HierarchyRoot:
					m_hierarchyRoot = (IReference<IHierarchyObj>)reader.ReadRIFObject();
					break;
				case MemberName.DataRowHolder:
					m_dataRowHolder = (RuntimeSortDataHolder)reader.ReadRIFObject();
					break;
				case MemberName.Expression:
					m_sortExpression = (RuntimeExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.SortTree:
					m_sortTree = (BTree)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		void IPersistable.ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IPersistable.GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataRowSortHierarchyObj;
		}

		internal static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.HierarchyRoot, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IHierarchyObjReference));
				list.Add(new MemberInfo(MemberName.DataRowHolder, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeSortDataHolder));
				list.Add(new MemberInfo(MemberName.Expression, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeExpressionInfo));
				list.Add(new MemberInfo(MemberName.SortTree, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BTree));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataRowSortHierarchyObj, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return m_declaration;
		}
	}
}
