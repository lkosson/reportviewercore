namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLParagraphProps : RPLElementProps
	{
		private RPLFormat.ListStyles? m_listStyle;

		private int? m_listLevel;

		private int m_paragraphNumber;

		private RPLReportSize m_leftIndent;

		private RPLReportSize m_rightIndent;

		private RPLReportSize m_hangingIndent;

		private RPLReportSize m_spaceBefore;

		private RPLReportSize m_spaceAfter;

		private bool m_firstLine = true;

		public RPLReportSize LeftIndent
		{
			get
			{
				return m_leftIndent;
			}
			set
			{
				m_leftIndent = value;
			}
		}

		public RPLReportSize RightIndent
		{
			get
			{
				return m_rightIndent;
			}
			set
			{
				m_rightIndent = value;
			}
		}

		public RPLReportSize HangingIndent
		{
			get
			{
				return m_hangingIndent;
			}
			set
			{
				m_hangingIndent = value;
			}
		}

		public RPLFormat.ListStyles? ListStyle
		{
			get
			{
				return m_listStyle;
			}
			set
			{
				m_listStyle = value;
			}
		}

		public int? ListLevel
		{
			get
			{
				return m_listLevel;
			}
			set
			{
				m_listLevel = value;
			}
		}

		public int ParagraphNumber
		{
			get
			{
				return m_paragraphNumber;
			}
			set
			{
				m_paragraphNumber = value;
			}
		}

		public RPLReportSize SpaceBefore
		{
			get
			{
				return m_spaceBefore;
			}
			set
			{
				m_spaceBefore = value;
			}
		}

		public RPLReportSize SpaceAfter
		{
			get
			{
				return m_spaceAfter;
			}
			set
			{
				m_spaceAfter = value;
			}
		}

		public bool FirstLine
		{
			get
			{
				return m_firstLine;
			}
			set
			{
				m_firstLine = value;
			}
		}

		internal RPLParagraphProps()
		{
		}
	}
}
