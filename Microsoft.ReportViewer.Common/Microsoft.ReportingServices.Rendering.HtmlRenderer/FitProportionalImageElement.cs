using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal sealed class FitProportionalImageElement : ComboElement
	{
		public FitProportionalImageElement(string url, float width, string role = null, string altText = null, string ariaLabel = null, Dictionary<string, string> customAttributes = null)
			: base(new DivElement
			{
				Role = role,
				AriaLabel = ariaLabel,
				BackgroundImage = url,
				BackgroundImageSize = HTMLElements.m_containString,
				BackgroundRepeat = new NoBackgroundRepeat(),
				ChildElement = new ImgElement
				{
					AltText = altText,
					Image = url,
					Opacity = 0f,
					Size = new SizeAttribute
					{
						Width = new AutoScaleTo100Percent(),
						Height = new AutoScaleTo100Percent()
					},
					CustomAttributes = customAttributes
				},
				Size = new SizeAttribute
				{
					Width = new MillimeterSize(width),
					Height = new AutoScaleTo100Percent()
				}
			})
		{
		}
	}
}
