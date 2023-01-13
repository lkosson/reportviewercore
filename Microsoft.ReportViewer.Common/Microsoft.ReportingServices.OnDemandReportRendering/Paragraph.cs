using Microsoft.ReportingServices.ReportRendering;
using System.Security.Permissions;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class Paragraph : ReportElement
	{
		protected ReportIntProperty m_listLevel;

		protected ReportEnumProperty<ListStyle> m_listStyle;

		private string m_definitionPath;

		protected int m_indexIntoParentCollectionDef;

		protected ParagraphInstance m_instance;

		protected TextRunCollection m_textRunCollection;

		protected TextBox m_textBox;

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

		public TextRunCollection TextRuns
		{
			get
			{
				if (m_textRunCollection == null)
				{
					m_textRunCollection = new TextRunCollection(this);
				}
				return m_textRunCollection;
			}
		}

		public virtual ReportSizeProperty LeftIndent => null;

		public virtual ReportSizeProperty RightIndent => null;

		public virtual ReportSizeProperty HangingIndent => null;

		public abstract ReportEnumProperty<ListStyle> ListStyle
		{
			get;
		}

		public abstract ReportIntProperty ListLevel
		{
			get;
		}

		public virtual ReportSizeProperty SpaceBefore => null;

		public virtual ReportSizeProperty SpaceAfter => null;

		internal TextBox TextBox => m_textBox;

		internal override ReportElementInstance ReportElementInstance => Instance;

		public new abstract ParagraphInstance Instance
		{
			get;
		}

		internal Paragraph(TextBox textBox, int indexIntoParentCollectionDef, RenderingContext renderingContext)
			: base(textBox.ReportScope, textBox, textBox.ReportItemDef, renderingContext)
		{
			m_textBox = textBox;
			m_indexIntoParentCollectionDef = indexIntoParentCollectionDef;
		}

		internal Paragraph(TextBox textBox, RenderingContext renderingContext)
			: base(textBox, textBox.RenderReportItem, renderingContext)
		{
			m_textBox = textBox;
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
			if (m_textRunCollection != null)
			{
				m_textRunCollection.SetNewContext();
			}
		}

		internal virtual void UpdateRenderReportItem(Microsoft.ReportingServices.ReportRendering.ReportItem renderReportItem)
		{
			if (m_textRunCollection != null && m_textRunCollection[0] != null)
			{
				m_textRunCollection[0].UpdateRenderReportItem(renderReportItem);
			}
		}
	}
}
