using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal sealed class ImgElement : HtmlElement
	{
		public string AltText
		{
			get;
			set;
		}

		public SizeAttribute Size
		{
			get;
			set;
		}

		public string Image
		{
			get;
			set;
		}

		public float? Opacity
		{
			get;
			set;
		}

		public Dictionary<string, string> CustomAttributes
		{
			get;
			set;
		}

		public void Render(IOutputStream outputStream)
		{
			outputStream.Write(HTMLElements.m_img);
			string text = string.Empty;
			if (Size != null && Size.Width != null && Size.Width.GetType() == typeof(AutoScaleTo100Percent))
			{
				text = HTMLElements.m_resize100WidthClassName;
			}
			if (Size != null && Size.Height != null && Size.Height.GetType() == typeof(AutoScaleTo100Percent))
			{
				text = text + HTMLElements.m_spaceString + HTMLElements.m_resize100HeightClassName;
			}
			if (!string.IsNullOrEmpty(text))
			{
				outputStream.Write(HTMLElements.m_classStyle);
				outputStream.Write(text);
				outputStream.Write(HTMLElements.m_quoteString);
			}
			if (!string.IsNullOrEmpty(AltText))
			{
				outputStream.Write(HTMLElements.m_alt);
				outputStream.Write(AltText);
				outputStream.Write(HTMLElements.m_quoteString);
				outputStream.Write(HTMLElements.m_title);
				outputStream.Write(AltText);
				outputStream.Write(HTMLElements.m_quoteString);
			}
			outputStream.Write(HTMLElements.m_openStyle);
			if (Size != null)
			{
				Size.Render(outputStream);
			}
			if (Opacity.HasValue)
			{
				outputStream.Write(HTMLElements.m_space);
				outputStream.Write(HTMLElements.m_opacity);
				outputStream.Write(Opacity.ToString());
			}
			outputStream.Write(HTMLElements.m_quoteString);
			if (CustomAttributes != null)
			{
				foreach (KeyValuePair<string, string> customAttribute in CustomAttributes)
				{
					outputStream.Write(HTMLElements.m_space);
					outputStream.Write(customAttribute.Key);
					outputStream.Write(HTMLElements.m_equal);
					outputStream.Write(HTMLElements.m_quoteString);
					outputStream.Write(customAttribute.Value);
					outputStream.Write(HTMLElements.m_quoteString);
				}
			}
			outputStream.Write(HTMLElements.m_src);
			if (Image != null)
			{
				outputStream.Write(Image);
			}
			outputStream.Write(HTMLElements.m_quoteString);
			outputStream.Write(HTMLElements.m_space);
			outputStream.Write(HTMLElements.m_closeSingleTag);
		}
	}
}
