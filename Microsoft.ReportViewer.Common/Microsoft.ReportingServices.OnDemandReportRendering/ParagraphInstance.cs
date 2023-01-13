using System.Security.Permissions;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class ParagraphInstance : ReportElementInstance
	{
		protected TextRunInstanceCollection m_textRunInstances;

		protected string m_uniqueName;

		public abstract string UniqueName
		{
			get;
		}

		public virtual ReportSize LeftIndent => null;

		public virtual ReportSize RightIndent => null;

		public virtual ReportSize HangingIndent => null;

		public virtual ListStyle ListStyle => ListStyle.None;

		public virtual int ListLevel => 0;

		public virtual ReportSize SpaceBefore => null;

		public virtual ReportSize SpaceAfter => null;

		public Paragraph Definition => (Paragraph)m_reportElementDef;

		public TextRunInstanceCollection TextRunInstances
		{
			get
			{
				if (m_textRunInstances == null)
				{
					m_textRunInstances = new TextRunInstanceCollection(this);
				}
				return m_textRunInstances;
			}
		}

		public abstract bool IsCompiled
		{
			get;
		}

		internal ParagraphInstance(Paragraph paragraphDef)
			: base(paragraphDef)
		{
		}

		protected ParagraphInstance(ReportElement reportElementDef)
			: base(reportElementDef)
		{
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_uniqueName = null;
		}
	}
}
