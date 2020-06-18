namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal interface ICompiledTextRunInstance
	{
		ICompiledStyleInstance Style
		{
			get;
			set;
		}

		string Value
		{
			get;
			set;
		}

		string Label
		{
			get;
			set;
		}

		string ToolTip
		{
			get;
			set;
		}

		MarkupType MarkupType
		{
			get;
			set;
		}

		IActionInstance ActionInstance
		{
			get;
			set;
		}
	}
}
