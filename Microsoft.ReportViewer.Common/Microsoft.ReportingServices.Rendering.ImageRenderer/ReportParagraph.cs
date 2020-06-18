using Microsoft.ReportingServices.Rendering.RichText;
using Microsoft.ReportingServices.Rendering.RPLProcessing;

namespace Microsoft.ReportingServices.Rendering.ImageRenderer
{
	internal class ReportParagraph : IParagraphProps
	{
		private RPLParagraphProps m_source;

		private RPLElementStyle m_style;

		private string m_uniqueName;

		private int m_paragraphNumber;

		public RPLFormat.TextAlignments Alignment
		{
			get
			{
				RPLElementStyle style = m_style;
				if (style == null && m_source != null)
				{
					style = m_source.Style;
				}
				RPLFormat.TextAlignments result = RPLFormat.TextAlignments.General;
				if (style != null)
				{
					object obj = style[25];
					if (obj != null)
					{
						result = (RPLFormat.TextAlignments)obj;
					}
				}
				return result;
			}
		}

		public float SpaceBefore
		{
			get
			{
				if (m_source != null)
				{
					RPLReportSize spaceBefore = m_source.SpaceBefore;
					if (spaceBefore == null)
					{
						spaceBefore = ((RPLParagraphPropsDef)m_source.Definition).SpaceBefore;
					}
					if (spaceBefore != null)
					{
						return (float)spaceBefore.ToMillimeters();
					}
				}
				return 0f;
			}
		}

		public float SpaceAfter
		{
			get
			{
				if (m_source != null)
				{
					RPLReportSize spaceAfter = m_source.SpaceAfter;
					if (spaceAfter == null)
					{
						spaceAfter = ((RPLParagraphPropsDef)m_source.Definition).SpaceAfter;
					}
					if (spaceAfter != null)
					{
						return (float)spaceAfter.ToMillimeters();
					}
				}
				return 0f;
			}
		}

		public float LeftIndent
		{
			get
			{
				if (m_source != null)
				{
					RPLReportSize leftIndent = m_source.LeftIndent;
					if (leftIndent == null)
					{
						leftIndent = ((RPLParagraphPropsDef)m_source.Definition).LeftIndent;
					}
					if (leftIndent != null)
					{
						return (float)leftIndent.ToMillimeters();
					}
				}
				return 0f;
			}
		}

		public float RightIndent
		{
			get
			{
				if (m_source != null)
				{
					RPLReportSize rightIndent = m_source.RightIndent;
					if (rightIndent == null)
					{
						rightIndent = ((RPLParagraphPropsDef)m_source.Definition).RightIndent;
					}
					if (rightIndent != null)
					{
						return (float)rightIndent.ToMillimeters();
					}
				}
				return 0f;
			}
		}

		public float HangingIndent
		{
			get
			{
				if (m_source != null)
				{
					RPLReportSize hangingIndent = m_source.HangingIndent;
					if (hangingIndent == null)
					{
						hangingIndent = ((RPLParagraphPropsDef)m_source.Definition).HangingIndent;
					}
					if (hangingIndent != null)
					{
						return (float)hangingIndent.ToMillimeters();
					}
				}
				return 0f;
			}
		}

		public int ListLevel
		{
			get
			{
				int result = 0;
				if (m_source != null)
				{
					int? num = m_source.ListLevel;
					if (!num.HasValue)
					{
						num = ((RPLParagraphPropsDef)m_source.Definition).ListLevel;
					}
					if (num.HasValue)
					{
						result = num.Value;
					}
				}
				return result;
			}
		}

		public RPLFormat.ListStyles ListStyle
		{
			get
			{
				RPLFormat.ListStyles result = RPLFormat.ListStyles.None;
				if (m_source != null)
				{
					RPLFormat.ListStyles? listStyles = m_source.ListStyle;
					if (!listStyles.HasValue)
					{
						listStyles = ((RPLParagraphPropsDef)m_source.Definition).ListStyle;
					}
					if (listStyles.HasValue)
					{
						result = listStyles.Value;
					}
				}
				return result;
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

		internal string UniqueName => m_uniqueName;

		internal ReportParagraph(RPLParagraphProps source)
		{
			m_source = source;
			m_uniqueName = source.UniqueName;
			ParagraphNumber = source.ParagraphNumber;
		}

		internal ReportParagraph(RPLElementStyle style, string uniqueName)
		{
			m_style = style;
			m_uniqueName = uniqueName;
		}
	}
}
