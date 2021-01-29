namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal sealed class SizeAttribute
	{
		public ISize Width
		{
			get;
			set;
		}

		public ISize MinWidth
		{
			get;
			set;
		}

		public ISize MaxWidth
		{
			get;
			set;
		}

		public ISize Height
		{
			get;
			set;
		}

		public ISize MinHeight
		{
			get;
			set;
		}

		public ISize MaxHeight
		{
			get;
			set;
		}

		public void Render(IOutputStream outputStream)
		{
			if (MinWidth != null)
			{
				outputStream.Write(HTMLElements.m_space);
				outputStream.Write(HTMLElements.m_styleMinWidth);
				outputStream.Write(HTMLElements.m_space);
				MinWidth.Render(outputStream);
				outputStream.Write(HTMLElements.m_semiColon);
			}
			if (MinHeight != null)
			{
				outputStream.Write(HTMLElements.m_space);
				outputStream.Write(HTMLElements.m_styleMinHeight);
				outputStream.Write(HTMLElements.m_space);
				MinHeight.Render(outputStream);
				outputStream.Write(HTMLElements.m_semiColon);
			}
			if (MaxHeight != null)
			{
				outputStream.Write(HTMLElements.m_space);
				outputStream.Write(HTMLElements.m_styleMaxHeight);
				outputStream.Write(HTMLElements.m_space);
				MaxHeight.Render(outputStream);
				outputStream.Write(HTMLElements.m_semiColon);
			}
			if (MaxWidth != null)
			{
				outputStream.Write(HTMLElements.m_space);
				outputStream.Write(HTMLElements.m_styleMaxWidth);
				outputStream.Write(HTMLElements.m_space);
				MaxWidth.Render(outputStream);
				outputStream.Write(HTMLElements.m_semiColon);
			}
			if (Width != null)
			{
				outputStream.Write(HTMLElements.m_space);
				outputStream.Write(HTMLElements.m_styleWidth);
				outputStream.Write(HTMLElements.m_space);
				Width.Render(outputStream);
				outputStream.Write(HTMLElements.m_semiColon);
			}
			if (Height != null)
			{
				outputStream.Write(HTMLElements.m_space);
				outputStream.Write(HTMLElements.m_styleHeight);
				outputStream.Write(HTMLElements.m_space);
				Height.Render(outputStream);
				outputStream.Write(HTMLElements.m_semiColon);
			}
		}
	}
}
