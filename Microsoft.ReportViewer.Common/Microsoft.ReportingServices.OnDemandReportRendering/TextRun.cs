using Microsoft.ReportingServices.ReportRendering;
using System;
using System.Collections.Generic;
using System.Security.Permissions;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	[StrongNameIdentityPermission(SecurityAction.InheritanceDemand, PublicKey = "0024000004800000940000000602000000240000525341310004000001000100272736ad6e5f9586bac2d531eabc3acc666c2f8ec879fa94f8f7b0327d2ff2ed523448f83c3d5c5dd2dfc7bc99c5286b2c125117bf5cbe242b9d41750732b2bdffe649c6efb8e5526d526fdd130095ecdb7bf210809c6cdad8824faa9ac0310ac3cba2aa0523567b2dfa7fe250b30facbd62d4ec99b94ac47c7d3b28f1f6e4c8")]
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
