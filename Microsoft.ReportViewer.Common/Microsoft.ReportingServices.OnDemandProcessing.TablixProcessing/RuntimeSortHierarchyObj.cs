using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class RuntimeSortHierarchyObj : IHierarchyObj, IStorable, IPersistable
	{
		[PersistedWithinRequestOnly]
		internal class SortHierarchyStructure : IStorable, IPersistable
		{
			internal IReference<RuntimeSortFilterEventInfo> SortInfo;

			internal int SortIndex;

			internal BTree SortTree;

			[NonSerialized]
			private static Declaration m_declaration = GetDeclaration();

			public int Size => ItemSizes.SizeOf(SortInfo) + 4 + ItemSizes.SizeOf(SortTree);

			internal SortHierarchyStructure()
			{
			}

			internal SortHierarchyStructure(IHierarchyObj owner, int sortIndex, List<IReference<RuntimeSortFilterEventInfo>> sortInfoList, List<int> sortInfoIndices)
			{
				SortIndex = sortIndex;
				SortInfo = sortInfoList[sortInfoIndices[sortIndex]];
				SortTree = new BTree(owner, owner.OdpContext, owner.Depth);
			}

			public void Serialize(IntermediateFormatWriter writer)
			{
				writer.RegisterDeclaration(m_declaration);
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.SortInfo:
						writer.Write(SortInfo);
						break;
					case MemberName.SortIndex:
						writer.Write(SortIndex);
						break;
					case MemberName.SortTree:
						writer.Write(SortTree);
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
					case MemberName.SortInfo:
						SortInfo = (IReference<RuntimeSortFilterEventInfo>)reader.ReadRIFObject();
						break;
					case MemberName.SortIndex:
						SortIndex = reader.ReadInt32();
						break;
					case MemberName.SortTree:
						SortTree = (BTree)reader.ReadRIFObject();
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
				return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortHierarchyStruct;
			}

			public static Declaration GetDeclaration()
			{
				if (m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.SortInfo, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeSortFilterEventInfoReference));
					list.Add(new MemberInfo(MemberName.SortIndex, Token.Int32));
					list.Add(new MemberInfo(MemberName.SortTree, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BTreeNode));
					return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortHierarchyStruct, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
				}
				return m_declaration;
			}
		}

		private IReference<IHierarchyObj> m_hierarchyRoot;

		private OnDemandProcessingContext m_odpContext;

		private SortHierarchyStructure m_sortHierarchyStruct;

		private IReference<ISortDataHolder> m_dataHolder;

		private RuntimeSortDataHolder m_dataRowHolder;

		private static readonly Declaration m_declaration = GetDeclaration();

		public int Depth => m_hierarchyRoot.Value().Depth + 1;

		IReference<IHierarchyObj> IHierarchyObj.HierarchyRoot => m_hierarchyRoot;

		OnDemandProcessingContext IHierarchyObj.OdpContext => m_hierarchyRoot.Value().OdpContext;

		BTree IHierarchyObj.SortTree
		{
			get
			{
				if (m_sortHierarchyStruct != null)
				{
					return m_sortHierarchyStruct.SortTree;
				}
				return null;
			}
		}

		int IHierarchyObj.ExpressionIndex
		{
			get
			{
				if (m_sortHierarchyStruct != null)
				{
					return m_sortHierarchyStruct.SortIndex;
				}
				return -1;
			}
		}

		List<int> IHierarchyObj.SortFilterInfoIndices => m_hierarchyRoot.Value().SortFilterInfoIndices;

		bool IHierarchyObj.IsDetail => false;

		bool IHierarchyObj.InDataRowSortPhase => false;

		public int Size => ItemSizes.SizeOf(m_hierarchyRoot) + ItemSizes.ReferenceSize + ItemSizes.SizeOf(m_sortHierarchyStruct) + ItemSizes.SizeOf(m_dataHolder) + ItemSizes.SizeOf(m_dataRowHolder);

		internal RuntimeSortHierarchyObj()
		{
		}

		internal RuntimeSortHierarchyObj(IHierarchyObj outerHierarchy, int depth)
		{
			m_hierarchyRoot = outerHierarchy.HierarchyRoot;
			m_odpContext = m_hierarchyRoot.Value().OdpContext;
			List<int> sortFilterInfoIndices = m_hierarchyRoot.Value().SortFilterInfoIndices;
			int num = outerHierarchy.ExpressionIndex + 1;
			if (sortFilterInfoIndices == null || num >= sortFilterInfoIndices.Count)
			{
				RuntimeDataTablixGroupRootObjReference runtimeDataTablixGroupRootObjReference = m_hierarchyRoot as RuntimeDataTablixGroupRootObjReference;
				if (null != runtimeDataTablixGroupRootObjReference)
				{
					using (runtimeDataTablixGroupRootObjReference.PinValue())
					{
						RuntimeDataTablixGroupRootObj runtimeDataTablixGroupRootObj = runtimeDataTablixGroupRootObjReference.Value();
						m_dataHolder = (IReference<ISortDataHolder>)runtimeDataTablixGroupRootObj.CreateGroupLeaf();
						if (!runtimeDataTablixGroupRootObj.HasParent)
						{
							runtimeDataTablixGroupRootObj.AddChildWithNoParent((RuntimeGroupLeafObjReference)m_dataHolder);
						}
					}
				}
				else
				{
					m_dataRowHolder = new RuntimeSortDataHolder();
				}
			}
			else
			{
				m_sortHierarchyStruct = new SortHierarchyStructure(this, num, m_odpContext.RuntimeSortFilterInfo, sortFilterInfoIndices);
			}
		}

		IHierarchyObj IHierarchyObj.CreateHierarchyObjForSortTree()
		{
			return new RuntimeSortHierarchyObj(this, Depth + 1);
		}

		ProcessingMessageList IHierarchyObj.RegisterComparisonError(string propertyName)
		{
			return m_odpContext.RegisterComparisonErrorForSortFilterEvent(propertyName);
		}

		void IHierarchyObj.NextRow(IHierarchyObj owner)
		{
			if (m_dataHolder != null)
			{
				using (m_dataHolder.PinValue())
				{
					m_dataHolder.Value().NextRow();
				}
			}
			else if (m_sortHierarchyStruct != null)
			{
				IReference<RuntimeSortFilterEventInfo> sortInfo = m_sortHierarchyStruct.SortInfo;
				object sortOrder;
				using (sortInfo.PinValue())
				{
					sortOrder = sortInfo.Value().GetSortOrder(m_odpContext.ReportRuntime);
				}
				m_sortHierarchyStruct.SortTree.NextRow(sortOrder, this);
			}
			else if (m_dataRowHolder != null)
			{
				m_dataRowHolder.NextRow(m_odpContext, Depth + 1);
			}
		}

		void IHierarchyObj.Traverse(ProcessingStages operation, ITraversalContext traversalContext)
		{
			if (m_sortHierarchyStruct != null)
			{
				bool ascending = true;
				RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = m_sortHierarchyStruct.SortInfo.Value();
				if (runtimeSortFilterEventInfo.EventSource.UserSort.SortExpressionScope == null)
				{
					ascending = runtimeSortFilterEventInfo.SortDirection;
				}
				m_sortHierarchyStruct.SortTree.Traverse(operation, ascending, traversalContext);
			}
			if (m_dataHolder != null)
			{
				using (m_dataHolder.PinValue())
				{
					m_dataHolder.Value().Traverse(operation, traversalContext);
				}
			}
			if (m_dataRowHolder != null)
			{
				using (m_hierarchyRoot.PinValue())
				{
					m_dataRowHolder.Traverse(operation, traversalContext, m_hierarchyRoot.Value());
				}
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
			IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.HierarchyRoot:
					writer.Write(m_hierarchyRoot);
					break;
				case MemberName.OdpContext:
				{
					int value = scalabilityCache.StoreStaticReference(m_odpContext);
					writer.Write(value);
					break;
				}
				case MemberName.SortHierarchyStruct:
					writer.Write(m_sortHierarchyStruct);
					break;
				case MemberName.DataHolder:
					writer.Write(m_dataHolder);
					break;
				case MemberName.DataRowHolder:
					writer.Write(m_dataRowHolder);
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
			IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.HierarchyRoot:
					m_hierarchyRoot = (IReference<IHierarchyObj>)reader.ReadRIFObject();
					break;
				case MemberName.OdpContext:
				{
					int id = reader.ReadInt32();
					m_odpContext = (OnDemandProcessingContext)scalabilityCache.FetchStaticReference(id);
					break;
				}
				case MemberName.SortHierarchyStruct:
					m_sortHierarchyStruct = (SortHierarchyStructure)reader.ReadRIFObject();
					break;
				case MemberName.DataHolder:
					m_dataHolder = (IReference<ISortDataHolder>)reader.ReadRIFObject();
					break;
				case MemberName.DataRowHolder:
					m_dataRowHolder = (RuntimeSortDataHolder)reader.ReadRIFObject();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeSortHierarchyObj;
		}

		internal static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.HierarchyRoot, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IHierarchyObjReference));
				list.Add(new MemberInfo(MemberName.OdpContext, Token.Int32));
				list.Add(new MemberInfo(MemberName.SortHierarchyStruct, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortHierarchyStruct));
				list.Add(new MemberInfo(MemberName.DataHolder, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ISortDataHolderReference));
				list.Add(new MemberInfo(MemberName.DataRowHolder, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeSortDataHolder));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeSortHierarchyObj, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return m_declaration;
		}
	}
}
