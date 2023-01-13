using Microsoft.ReportingServices.ReportRendering;
using System;
using System.Collections.Generic;
using System.Security.Permissions;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class TextRun : ReportElement, IROMActionOwner
	{
		protected ReportStringProperty m_value;

		protected ReportEnumProperty<MarkupType> m_markupType;

		private string m_definitionPath;

		protected int m_indexIntoParentCollectionDef;

		protected TextRunInstance m_instance;

		protected Paragraph m_paragraph;

		protected bool? m_formattedValueExpressionBased;

		public override string DefinitionPath
		{
			get
			{
				if (m_definitionPath == null)
				{
					m_definitionPath = DefinitionPathConstants.GetCollectionDefinitionPath(m_parentDefinitionPath, m_indexIntoParentCollectionDef);
				}
				return m_definitionPath;
			}
		}

		internal override string InstanceUniqueName
		{
			get
			{
				if (base.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				return Instance.UniqueName;
			}
		}

		public virtual string Label => null;

		public abstract ReportStringProperty Value
		{
			get;
		}

		string IROMActionOwner.UniqueName => null;

		public virtual ActionInfo ActionInfo => null;

		public virtual ReportStringProperty ToolTip => null;

		public abstract ReportEnumProperty<MarkupType> MarkupType
		{
			get;
		}

		public abstract TypeCode SharedTypeCode
		{
			get;
		}

		internal TextBox TextBox => m_paragraph.TextBox;

		public abstract bool FormattedValueExpressionBased
		{
			get;
		}

		internal override ReportElementInstance ReportElementInstance => Instance;

		public new abstract TextRunInstance Instance
		{
			get;
		}

		public virtual CompiledRichTextInstance CompiledInstance => null;

		List<string> IROMActionOwner.FieldsUsedInValueExpression => FieldsUsedInValueExpression;

		internal virtual List<string> FieldsUsedInValueExpression => null;

		internal TextRun(Paragraph paragraph, int indexIntoParentCollectionDef, RenderingContext renderingContext)
			: base(paragraph.ReportScope, paragraph, paragraph.TextBox.ReportItemDef, renderingContext)
		{
			m_paragraph = paragraph;
			m_indexIntoParentCollectionDef = indexIntoParentCollectionDef;
		}

		internal TextRun(Paragraph paragraph, RenderingContext renderingContext)
			: base(paragraph, paragraph.TextBox.RenderReportItem, renderingContext)
		{
			m_paragraph = paragraph;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
		}

		internal override void SetNewContextChildren()
		{
		}

		internal virtual void UpdateRenderReportItem(Microsoft.ReportingServices.ReportRendering.ReportItem renderReportItem)
		{
		}
	}
}
