using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportRendering;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class TextBox : ReportItem, IROMActionOwner
	{
		private ActionInfo m_actionInfo;

		private Microsoft.ReportingServices.ReportRendering.TextBox m_renderTextBox;

		private ParagraphCollection m_paragraphCollection;

		private Microsoft.ReportingServices.ReportIntermediateFormat.TextBox m_textBoxDef;

		public override Style Style
		{
			get
			{
				if (m_isOldSnapshot)
				{
					if (m_style == null)
					{
						m_style = new TextBoxFilteredStyle(RenderReportItem, base.RenderingContext, UseRenderStyle);
					}
					return m_style;
				}
				return base.Style;
			}
		}

		public bool CanScrollVertically
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return false;
				}
				return m_textBoxDef.CanScrollVertically;
			}
		}

		public bool CanGrow
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return m_renderTextBox.CanGrow;
				}
				return m_textBoxDef.CanGrow;
			}
		}

		public bool CanShrink
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return m_renderTextBox.CanShrink;
				}
				return m_textBoxDef.CanShrink;
			}
		}

		public bool HideDuplicates
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return m_renderTextBox.HideDuplicates;
				}
				if (m_textBoxDef.IsSimple)
				{
					return m_textBoxDef.HideDuplicates != null;
				}
				return false;
			}
		}

		public string UniqueName => m_reportItemDef.UniqueName;

		public ActionInfo ActionInfo
		{
			get
			{
				if (m_actionInfo == null)
				{
					if (m_isOldSnapshot)
					{
						if (m_renderTextBox.ActionInfo != null)
						{
							m_actionInfo = new ActionInfo(base.RenderingContext, m_renderTextBox.ActionInfo);
						}
					}
					else if (m_textBoxDef.Action != null)
					{
						m_actionInfo = new ActionInfo(base.RenderingContext, ReportScope, m_textBoxDef.Action, m_reportItemDef, this, m_reportItemDef.ObjectType, m_reportItemDef.Name, this);
					}
				}
				return m_actionInfo;
			}
		}

		public TypeCode SharedTypeCode
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return m_renderTextBox.SharedTypeCode;
				}
				if (IsSimple)
				{
					return Paragraphs[0].TextRuns[0].SharedTypeCode;
				}
				return TypeCode.String;
			}
		}

		public bool IsToggleParent
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return m_renderTextBox.IsSharedToggleParent;
				}
				return m_textBoxDef.IsToggle;
			}
		}

		public bool CanSort
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return m_renderTextBox.CanSort;
				}
				return m_textBoxDef.UserSort != null;
			}
		}

		public Report.DataElementStyles DataElementStyle
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return (Report.DataElementStyles)m_renderTextBox.DataElementStyle;
				}
				if (!m_textBoxDef.DataElementStyleAttribute)
				{
					return Report.DataElementStyles.Element;
				}
				return Report.DataElementStyles.Attribute;
			}
		}

		public bool KeepTogether
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return true;
				}
				return m_textBoxDef.KeepTogether;
			}
		}

		public ParagraphCollection Paragraphs
		{
			get
			{
				if (m_paragraphCollection == null)
				{
					m_paragraphCollection = new ParagraphCollection(this);
				}
				return m_paragraphCollection;
			}
		}

		public bool IsSimple
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return true;
				}
				return m_textBoxDef.IsSimple;
			}
		}

		public bool FormattedValueExpressionBased
		{
			get
			{
				if (IsSimple)
				{
					return Paragraphs[0].TextRuns[0].FormattedValueExpressionBased;
				}
				return false;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.TextBox TexBoxDef => m_textBoxDef;

		List<string> IROMActionOwner.FieldsUsedInValueExpression
		{
			get
			{
				if (base.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				return ((TextBoxInstance)GetOrCreateInstance()).GetFieldsUsedInValueExpression();
			}
		}

		internal TextBox(IReportScope reportScope, IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, Microsoft.ReportingServices.ReportIntermediateFormat.TextBox reportItemDef, RenderingContext renderingContext)
			: base(reportScope, parentDefinitionPath, indexIntoParentCollectionDef, reportItemDef, renderingContext)
		{
			m_textBoxDef = reportItemDef;
		}

		internal TextBox(IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, bool inSubtotal, Microsoft.ReportingServices.ReportRendering.TextBox renderTextBox, RenderingContext renderingContext)
			: base(parentDefinitionPath, indexIntoParentCollectionDef, inSubtotal, renderTextBox, renderingContext)
		{
			m_renderTextBox = renderTextBox;
		}

		internal override ReportItemInstance GetOrCreateInstance()
		{
			if (m_instance == null)
			{
				m_instance = new TextBoxInstance(this);
			}
			return m_instance;
		}

		internal override void UpdateRenderReportItem(Microsoft.ReportingServices.ReportRendering.ReportItem renderReportItem)
		{
			base.UpdateRenderReportItem(renderReportItem);
			m_renderTextBox = (Microsoft.ReportingServices.ReportRendering.TextBox)m_renderReportItem;
			if (m_actionInfo != null)
			{
				m_actionInfo.Update(m_renderTextBox.ActionInfo);
			}
			if (m_paragraphCollection != null && m_paragraphCollection[0] != null)
			{
				m_paragraphCollection[0].UpdateRenderReportItem(renderReportItem);
			}
		}

		internal override void SetNewContextChildren()
		{
			base.SetNewContextChildren();
			if (m_actionInfo != null)
			{
				m_actionInfo.SetNewContext();
			}
			if (m_paragraphCollection != null)
			{
				m_paragraphCollection.SetNewContext();
			}
		}
	}
}
