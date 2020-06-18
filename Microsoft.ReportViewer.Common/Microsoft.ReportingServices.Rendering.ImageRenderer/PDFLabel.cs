using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.ImageRenderer
{
	internal sealed class PDFLabel
	{
		internal readonly string UniqueName;

		internal readonly string Label;

		internal List<PDFLabel> Children;

		internal PDFLabel Parent;

		internal PDFLabel(string uniqueName, string label)
		{
			UniqueName = uniqueName;
			Label = label;
		}
	}
}
