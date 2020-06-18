using Microsoft.ReportingServices.Rendering.RPLProcessing;

namespace Microsoft.ReportingServices.Rendering.RichText
{
	internal interface IParagraphProps
	{
		float SpaceBefore
		{
			get;
		}

		float SpaceAfter
		{
			get;
		}

		float LeftIndent
		{
			get;
		}

		float RightIndent
		{
			get;
		}

		float HangingIndent
		{
			get;
		}

		int ListLevel
		{
			get;
		}

		RPLFormat.ListStyles ListStyle
		{
			get;
		}

		RPLFormat.TextAlignments Alignment
		{
			get;
		}

		int ParagraphNumber
		{
			get;
			set;
		}
	}
}
