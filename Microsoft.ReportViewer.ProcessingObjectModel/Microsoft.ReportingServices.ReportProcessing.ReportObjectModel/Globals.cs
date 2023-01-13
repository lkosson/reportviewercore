using System;
using System.Security.Permissions;

namespace Microsoft.ReportingServices.ReportProcessing.ReportObjectModel
{
	public abstract class Globals : MarshalByRefObject
	{
		public abstract object this[string key]
		{
			get;
		}

		public abstract string ReportName
		{
			get;
		}

		public abstract int PageNumber
		{
			get;
		}

		public abstract int TotalPages
		{
			get;
		}

		public abstract int OverallPageNumber
		{
			get;
		}

		public abstract int OverallTotalPages
		{
			get;
		}

		public abstract DateTime ExecutionTime
		{
			get;
		}

		public abstract string ReportServerUrl
		{
			get;
		}

		public abstract string ReportFolder
		{
			get;
		}

		public abstract string PageName
		{
			get;
		}

		public abstract RenderFormat RenderFormat
		{
			get;
		}
	}
}
