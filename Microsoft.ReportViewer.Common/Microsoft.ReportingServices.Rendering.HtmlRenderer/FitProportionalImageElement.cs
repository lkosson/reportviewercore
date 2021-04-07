using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal sealed class FitProportionalImageElement : ComboElement
	{
		public FitProportionalImageElement(string url, float width, float height, string role = null, string altText = null, string ariaLabel = null, Dictionary<string, string> customAttributes = null)
			: base(new DivElement
			{
				Role = role,
				AriaLabel = ariaLabel,
				BackgroundImage = url,
				BackgroundImageSize = HTMLElements.m_containString,
				BackgroundRepeat = new NoBackgroundRepeat(),
				Size = new SizeAttribute
				{
					Width = new MillimeterSize(width),
					Height = new MillimeterSize(height)
				}
			})
		{
		}
	}
}
