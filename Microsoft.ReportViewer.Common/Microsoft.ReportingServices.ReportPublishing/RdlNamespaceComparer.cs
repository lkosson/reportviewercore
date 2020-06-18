using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportPublishing
{
	internal sealed class RdlNamespaceComparer : IComparer<string>
	{
		private static readonly RdlNamespaceComparer m_instance = new RdlNamespaceComparer();

		public static RdlNamespaceComparer Instance => m_instance;

		private RdlNamespaceComparer()
		{
		}

		private static int GetNamespaceComparisonNumber(string nsString)
		{
			if (string.Equals(nsString, "http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition"))
			{
				return 0;
			}
			if (string.Equals(nsString, "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"))
			{
				return 1;
			}
			if (string.Equals(nsString, "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition"))
			{
				return 2;
			}
			if (string.Equals(nsString, "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition"))
			{
				return 3;
			}
			Global.Tracer.Assert(false, "Invalid RDL namespace: {0}", nsString);
			throw new InvalidOperationException("Invalid RDL namespace: " + nsString);
		}

		public int Compare(string x, string y)
		{
			return GetNamespaceComparisonNumber(x).CompareTo(GetNamespaceComparisonNumber(y));
		}
	}
}
