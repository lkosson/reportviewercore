using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal interface IInScopeEventSource : IReferenceable, IGloballyReferenceable, IGlobalIDOwner
	{
		Microsoft.ReportingServices.ReportProcessing.ObjectType ObjectType
		{
			get;
		}

		string Name
		{
			get;
		}

		ReportItem Parent
		{
			get;
		}

		EndUserSort UserSort
		{
			get;
		}

		List<InstancePathItem> InstancePath
		{
			get;
		}

		GroupingList ContainingScopes
		{
			get;
			set;
		}

		string Scope
		{
			get;
			set;
		}

		bool IsTablixCellScope
		{
			get;
			set;
		}

		bool IsDetailScope
		{
			get;
			set;
		}

		bool IsSubReportTopLevelScope
		{
			get;
			set;
		}

		InitializationContext.ScopeChainInfo ScopeChainInfo
		{
			get;
			set;
		}

		List<int> GetPeerSortFilters(bool create);

		InScopeSortFilterHashtable GetSortFiltersInScope(bool create, bool inDetail);
	}
}
