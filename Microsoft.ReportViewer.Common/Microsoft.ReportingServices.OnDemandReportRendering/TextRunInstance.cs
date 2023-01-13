using System;
using System.Security.Permissions;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class TextRunInstance : ReportElementInstance
	{
		protected string m_uniqueName;

		public abstract string UniqueName
		{
			get;
		}

		public abstract string Value
		{
			get;
		}

		public abstract object OriginalValue
		{
			get;
		}

		public virtual string ToolTip => null;

		public TextRun Definition => (TextRun)m_reportElementDef;

		public abstract MarkupType MarkupType
		{
			get;
		}

		public abstract TypeCode TypeCode
		{
			get;
		}

		public abstract bool IsCompiled
		{
			get;
		}

		public abstract bool ProcessedWithError
		{
			get;
		}

		internal TextRunInstance(TextRun textRunDef)
			: base(textRunDef)
		{
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_uniqueName = null;
		}
	}
}
