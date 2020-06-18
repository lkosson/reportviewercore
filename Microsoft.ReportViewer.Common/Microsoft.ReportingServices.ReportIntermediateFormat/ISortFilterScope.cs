using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal interface ISortFilterScope : IPersistable, IReferenceable
	{
		new int ID
		{
			get;
		}

		string ScopeName
		{
			get;
		}

		bool[] IsSortFilterTarget
		{
			get;
			set;
		}

		bool[] IsSortFilterExpressionScope
		{
			get;
			set;
		}

		List<ExpressionInfo> UserSortExpressions
		{
			get;
			set;
		}

		IndexedExprHost UserSortExpressionsHost
		{
			get;
		}
	}
}
