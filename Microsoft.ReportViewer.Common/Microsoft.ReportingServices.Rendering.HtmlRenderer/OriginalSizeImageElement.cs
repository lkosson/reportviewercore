using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal sealed class OriginalSizeImageElement : ComboElement
	{
		public OriginalSizeImageElement(string url, string role = null, string altText = null, string ariaLabel = null, Dictionary<string, string> customAttributes = null)
			: base(new DivElement
			{
				Role = role,
				AriaLabel = ariaLabel,
				ChildElement = new ImgElement
				{
					AltText = altText,
					Image = url,
					CustomAttributes = customAttributes
				}
			})
		{
		}
	}
}
