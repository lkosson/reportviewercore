using System;
using System.Security.Permissions;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	[StrongNameIdentityPermission(SecurityAction.InheritanceDemand, PublicKey = "0024000004800000940000000602000000240000525341310004000001000100272736ad6e5f9586bac2d531eabc3acc666c2f8ec879fa94f8f7b0327d2ff2ed523448f83c3d5c5dd2dfc7bc99c5286b2c125117bf5cbe242b9d41750732b2bdffe649c6efb8e5526d526fdd130095ecdb7bf210809c6cdad8824faa9ac0310ac3cba2aa0523567b2dfa7fe250b30facbd62d4ec99b94ac47c7d3b28f1f6e4c8")]
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
