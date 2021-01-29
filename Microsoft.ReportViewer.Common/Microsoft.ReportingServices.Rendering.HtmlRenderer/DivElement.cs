using System.Web.Security.AntiXss;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal sealed class DivElement : HtmlElement
	{
		public SizeAttribute Size
		{
			get;
			set;
		}

		public string BackgroundImage
		{
			get;
			set;
		}

		public string BackgroundImageSize
		{
			get;
			set;
		}

		public string Role
		{
			get;
			set;
		}

		public string AriaLabel
		{
			get;
			set;
		}

		public IBackgroundRepeatAttribute BackgroundRepeat
		{
			get;
			set;
		}

		public HtmlElement ChildElement
		{
			get;
			set;
		}

		public string Overflow
		{
			get;
			set;
		}

		public void Render(IOutputStream outputStream)
		{
			outputStream.Write(HTMLElements.m_openDiv);
			if (!string.IsNullOrEmpty(Role))
			{
				outputStream.Write(HTMLElements.m_space);
				outputStream.Write(HTMLElements.m_role);
				outputStream.Write(HTMLElements.m_equal);
				outputStream.Write(HTMLElements.m_quote);
				outputStream.Write(Role);
				outputStream.Write(HTMLElements.m_quote);
			}
			if (!string.IsNullOrEmpty(AriaLabel))
			{
				outputStream.Write(HTMLElements.m_space);
				outputStream.Write(HTMLElements.m_ariaLabel);
				outputStream.Write(HTMLElements.m_equal);
				outputStream.Write(HTMLElements.m_quote);
				outputStream.Write(AntiXssEncoder.XmlAttributeEncode(AriaLabel));
				outputStream.Write(HTMLElements.m_quote);
			}
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
			outputStream.Write(HTMLElements.m_openStyle);
			if (Size != null)
			{
				Size.Render(outputStream);
			}
			if (BackgroundImage != null)
			{
				outputStream.Write(HTMLElements.m_space);
				outputStream.Write(HTMLElements.m_backgroundImage);
				outputStream.Write(BackgroundImage.Replace("(", "%28").Replace(")", "%29"));
				outputStream.Write(HTMLElements.m_closeParenthesis);
				outputStream.Write(HTMLElements.m_semiColon);
			}
			if (BackgroundImageSize != null)
			{
				outputStream.Write(HTMLElements.m_space);
				outputStream.Write(HTMLElements.m_backgroundSize);
				outputStream.Write(BackgroundImageSize);
				outputStream.Write(HTMLElements.m_semiColon);
			}
			if (BackgroundRepeat != null)
			{
				outputStream.Write(HTMLElements.m_space);
				outputStream.Write(HTMLElements.m_backgroundRepeat);
				BackgroundRepeat.Render(outputStream);
				outputStream.Write(HTMLElements.m_semiColon);
			}
			if (Overflow != null)
			{
				outputStream.Write(HTMLElements.m_overflow);
				outputStream.Write(Overflow);
				outputStream.Write(HTMLElements.m_semiColon);
			}
			outputStream.Write(HTMLElements.m_quoteString);
			outputStream.Write(HTMLElements.m_closeBracket);
			if (ChildElement != null)
			{
				ChildElement.Render(outputStream);
			}
			outputStream.Write(HTMLElements.m_closeDiv);
		}
	}
}
