namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLTextRunProps : RPLElementProps
	{
		private RPLActionInfo m_actionInfo;

		private string m_value;

		private string m_toolTip;

		private RPLFormat.MarkupStyles m_markup;

		private bool m_processedWithError;

		public string Value
		{
			get
			{
				return m_value;
			}
			set
			{
				m_value = value;
			}
		}

		public string ToolTip
		{
			get
			{
				return m_toolTip;
			}
			set
			{
				m_toolTip = value;
			}
		}

		public RPLActionInfo ActionInfo
		{
			get
			{
				return m_actionInfo;
			}
			set
			{
				m_actionInfo = value;
			}
		}

		public RPLFormat.MarkupStyles Markup
		{
			get
			{
				return m_markup;
			}
			set
			{
				m_markup = value;
			}
		}

		public bool ProcessedWithError
		{
			get
			{
				return m_processedWithError;
			}
			set
			{
				m_processedWithError = value;
			}
		}

		internal RPLTextRunProps()
		{
		}
	}
}
