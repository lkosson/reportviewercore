using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal interface IHierarchyObj : IStorable, IPersistable
	{
		IReference<IHierarchyObj> HierarchyRoot
		{
			get;
		}

		OnDemandProcessingContext OdpContext
		{
			get;
		}

		BTree SortTree
		{
			get;
		}

		int ExpressionIndex
		{
			get;
		}

		int Depth
		{
			get;
		}

		List<int> SortFilterInfoIndices
		{
			get;
		}

		bool IsDetail
		{
			get;
		}

		bool InDataRowSortPhase
		{
			get;
		}

		IHierarchyObj CreateHierarchyObjForSortTree();

		ProcessingMessageList RegisterComparisonError(string propertyName);

		void NextRow(IHierarchyObj owner);

		void Traverse(ProcessingStages operation, ITraversalContext traversalContext);

		void ReadRow();

		void ProcessUserSort();

		void MarkSortInfoProcessed(List<IReference<RuntimeSortFilterEventInfo>> runtimeSortFilterInfo);

		void AddSortInfoIndex(int sortInfoIndex, IReference<RuntimeSortFilterEventInfo> sortInfo);
	}
}
