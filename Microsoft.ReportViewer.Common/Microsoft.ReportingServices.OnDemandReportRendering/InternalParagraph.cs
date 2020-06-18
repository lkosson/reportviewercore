using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalParagraph : Paragraph
	{
		private ReportSizeProperty m_leftIndent;

		private ReportSizeProperty m_rightIndent;

		private ReportSizeProperty m_hangingIndent;

		private ReportSizeProperty m_spaceBefore;

		private ReportSizeProperty m_spaceAfter;

		private Microsoft.ReportingServices.ReportIntermediateFormat.Paragraph m_paragraphDef;

		internal override IStyleContainer StyleContainer => m_paragraphDef;

		public override string ID => m_paragraphDef.RenderingModelID;

		public override ReportSizeProperty LeftIndent
		{
			get
			{
				if (m_leftIndent == null && m_paragraphDef.LeftIndent != null)
				{
					m_leftIndent = new ReportSizeProperty(m_paragraphDef.LeftIndent);
				}
				return m_leftIndent;
			}
		}

		public override ReportSizeProperty RightIndent
		{
			get
			{
				if (m_rightIndent == null && m_paragraphDef.RightIndent != null)
				{
					m_rightIndent = new ReportSizeProperty(m_paragraphDef.RightIndent);
				}
				return m_rightIndent;
			}
		}

		public override ReportSizeProperty HangingIndent
		{
			get
			{
				if (m_hangingIndent == null && m_paragraphDef.HangingIndent != null)
				{
					m_hangingIndent = new ReportSizeProperty(m_paragraphDef.HangingIndent, allowNegative: true);
				}
				return m_hangingIndent;
			}
		}

		public override ReportEnumProperty<ListStyle> ListStyle
		{
			get
			{
				if (m_listStyle == null)
				{
					ExpressionInfo listStyle = m_paragraphDef.ListStyle;
					if (listStyle != null)
					{
						ListStyle value = Microsoft.ReportingServices.OnDemandReportRendering.ListStyle.None;
						if (!listStyle.IsExpression)
						{
							value = RichTextHelpers.TranslateListStyle(listStyle.StringValue);
						}
						m_listStyle = new ReportEnumProperty<ListStyle>(listStyle.IsExpression, listStyle.OriginalText, value);
					}
					else
					{
						m_listStyle = new ReportEnumProperty<ListStyle>(Microsoft.ReportingServices.OnDemandReportRendering.ListStyle.None);
					}
				}
				return m_listStyle;
			}
		}

		public override ReportIntProperty ListLevel
		{
			get
			{
				if (m_listLevel == null)
				{
					if (m_paragraphDef.ListLevel != null)
					{
						m_listLevel = new ReportIntProperty(m_paragraphDef.ListLevel);
					}
					else
					{
						m_listLevel = new ReportIntProperty((Instance.ListStyle != 0) ? 1 : 0);
					}
				}
				return m_listLevel;
			}
		}

		public override ReportSizeProperty SpaceBefore
		{
			get
			{
				if (m_spaceBefore == null && m_paragraphDef.SpaceBefore != null)
				{
					m_spaceBefore = new ReportSizeProperty(m_paragraphDef.SpaceBefore);
				}
				return m_spaceBefore;
			}
		}

		public override ReportSizeProperty SpaceAfter
		{
			get
			{
				if (m_spaceAfter == null && m_paragraphDef.SpaceAfter != null)
				{
					m_spaceAfter = new ReportSizeProperty(m_paragraphDef.SpaceAfter);
				}
				return m_spaceAfter;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.Paragraph ParagraphDef => m_paragraphDef;

		public override ParagraphInstance Instance
		{
			get
			{
				if (base.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new InternalParagraphInstance(this);
				}
				return m_instance;
			}
		}

		internal InternalParagraph(TextBox textBox, int indexIntoParentCollectionDef, Microsoft.ReportingServices.ReportIntermediateFormat.Paragraph paragraph, RenderingContext renderingContext)
			: base(textBox, indexIntoParentCollectionDef, renderingContext)
		{
			m_paragraphDef = paragraph;
		}
	}
}
