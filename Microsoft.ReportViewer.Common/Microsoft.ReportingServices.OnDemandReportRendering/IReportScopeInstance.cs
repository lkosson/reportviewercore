namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal interface IReportScopeInstance
	{
		IReportScope ReportScope
		{
			get;
		}

		string UniqueName
		{
			get;
		}

		bool IsNewContext
		{
			get;
			set;
		}
	}
}
