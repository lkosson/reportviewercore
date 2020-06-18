using Microsoft.ReportingServices.ReportIntermediateFormat;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalTextRun : TextRun, IROMActionOwner
	{
		private ReportStringProperty m_toolTip;

		private ActionInfo m_actionInfo;

		private CompiledRichTextInstance m_compiledRichTextInstance;

		private Microsoft.ReportingServices.ReportIntermediateFormat.TextRun m_textRunDef;

		internal override IStyleContainer StyleContainer => m_textRunDef;

		public override string ID => m_textRunDef.RenderingModelID;

		public override string Label => m_textRunDef.Label;

		public override ReportStringProperty Value
		{
			get
			{
				if (m_value == null)
				{
					m_value = new ReportStringProperty(m_textRunDef.Value);
				}
				return m_value;
			}
		}

		string IROMActionOwner.UniqueName => TextRunDef.UniqueName;

		public override ActionInfo ActionInfo
		{
			get
			{
				if (m_actionInfo == null && m_textRunDef.Action != null)
				{
					m_actionInfo = new ActionInfo(base.RenderingContext, ReportScope, m_textRunDef.Action, TextRunDef, this, m_textRunDef.ObjectType, m_textRunDef.Name, this);
				}
				return m_actionInfo;
			}
		}

		public override ReportStringProperty ToolTip
		{
			get
			{
				if (m_toolTip == null && m_textRunDef.ToolTip != null)
				{
					m_toolTip = new ReportStringProperty(m_textRunDef.ToolTip);
				}
				return m_toolTip;
			}
		}

		public override ReportEnumProperty<MarkupType> MarkupType
		{
			get
			{
				if (m_markupType == null)
				{
					ExpressionInfo markupType = m_textRunDef.MarkupType;
					if (markupType != null)
					{
						MarkupType value = Microsoft.ReportingServices.OnDemandReportRendering.MarkupType.None;
						if (!markupType.IsExpression)
						{
							value = RichTextHelpers.TranslateMarkupType(markupType.StringValue);
						}
						m_markupType = new ReportEnumProperty<MarkupType>(markupType.IsExpression, markupType.OriginalText, value);
					}
					else
					{
						m_markupType = new ReportEnumProperty<MarkupType>(Microsoft.ReportingServices.OnDemandReportRendering.MarkupType.None);
					}
				}
				return m_markupType;
			}
		}

		public override TypeCode SharedTypeCode => m_textRunDef.ValueTypeCode;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.TextRun TextRunDef => m_textRunDef;

		public override bool FormattedValueExpressionBased
		{
			get
			{
				if (!m_formattedValueExpressionBased.HasValue)
				{
					if (!MarkupType.IsExpression && MarkupType.Value == Microsoft.ReportingServices.OnDemandReportRendering.MarkupType.None)
					{
						if (m_textRunDef.Value != null && m_textRunDef.ValueTypeCode != TypeCode.String)
						{
							Microsoft.ReportingServices.ReportIntermediateFormat.Style styleClass = m_textRunDef.StyleClass;
							m_formattedValueExpressionBased = (m_textRunDef.Value.IsExpression || (styleClass != null && (StyleAttributeExpressionBased(styleClass, "Language") || StyleAttributeExpressionBased(styleClass, "Format") || StyleAttributeExpressionBased(styleClass, "Calendar") || StyleAttributeExpressionBased(styleClass, "NumeralLanguage") || StyleAttributeExpressionBased(styleClass, "NumeralVariant"))));
						}
						else
						{
							m_formattedValueExpressionBased = false;
						}
					}
					else
					{
						m_formattedValueExpressionBased = true;
					}
				}
				return m_formattedValueExpressionBased.Value;
			}
		}

		public override TextRunInstance Instance
		{
			get
			{
				if (base.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new InternalTextRunInstance(this);
				}
				return m_instance;
			}
		}

		internal override List<string> FieldsUsedInValueExpression
		{
			get
			{
				if (base.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				return ((InternalTextRunInstance)Instance).GetFieldsUsedInValueExpression();
			}
		}

		public override CompiledRichTextInstance CompiledInstance
		{
			get
			{
				if (Instance.MarkupType == Microsoft.ReportingServices.OnDemandReportRendering.MarkupType.None)
				{
					return null;
				}
				if (m_compiledRichTextInstance == null)
				{
					m_compiledRichTextInstance = new CompiledRichTextInstance(ReportScope, this, m_paragraph, m_paragraph.TextRuns.Count == 1);
				}
				return m_compiledRichTextInstance;
			}
		}

		internal InternalTextRun(Paragraph paragraph, int indexIntoParentCollectionDef, Microsoft.ReportingServices.ReportIntermediateFormat.TextRun textRun, RenderingContext renderingContext)
			: base(paragraph, indexIntoParentCollectionDef, renderingContext)
		{
			m_textRunDef = textRun;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
		}

		internal override void SetNewContextChildren()
		{
			base.SetNewContextChildren();
			if (m_compiledRichTextInstance != null)
			{
				m_compiledRichTextInstance.SetNewContext();
			}
			if (m_actionInfo != null)
			{
				m_actionInfo.SetNewContext();
			}
		}

		private bool StyleAttributeExpressionBased(Microsoft.ReportingServices.ReportIntermediateFormat.Style style, string styleName)
		{
			if (style.GetAttributeInfo(styleName, out AttributeInfo styleAttribute))
			{
				return styleAttribute.IsExpression;
			}
			return false;
		}
	}
}
