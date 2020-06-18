namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLTextRunPropsDef : RPLElementPropsDef
	{
		private string m_value;

		private string m_label;

		private string m_toolTip;

		private RPLFormat.MarkupStyles m_markup;

		private string m_formula;

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

		public string Label
		{
			get
			{
				return m_label;
			}
			set
			{
				m_label = value;
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

		public string Formula
		{
			get
			{
				return m_formula;
			}
			set
			{
				m_formula = value;
			}
		}

		internal RPLTextRunPropsDef()
		{
		}
	}
}
