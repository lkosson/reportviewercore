using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal interface ICompiledParagraphInstance
	{
		IList<ICompiledTextRunInstance> CompiledTextRunInstances
		{
			get;
			set;
		}

		ICompiledStyleInstance Style
		{
			get;
			set;
		}

		ListStyle ListStyle
		{
			get;
			set;
		}

		int ListLevel
		{
			get;
			set;
		}

		ReportSize LeftIndent
		{
			get;
			set;
		}

		ReportSize RightIndent
		{
			get;
			set;
		}

		ReportSize HangingIndent
		{
			get;
			set;
		}

		ReportSize SpaceBefore
		{
			get;
			set;
		}

		ReportSize SpaceAfter
		{
			get;
			set;
		}
	}
}
