using Microsoft.ReportingServices.ReportRendering;
using System;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimTextRunInstance : TextRunInstance
	{
		private TextBoxInstance m_textBoxInstance;

		public override string UniqueName
		{
			get
			{
				if (m_uniqueName == null)
				{
					Microsoft.ReportingServices.ReportRendering.ReportItem renderReportItem = m_reportElementDef.RenderReportItem;
					m_uniqueName = renderReportItem.ID + "i1i" + renderReportItem.UniqueName;
				}
				return m_uniqueName;
			}
		}

		public override MarkupType MarkupType => MarkupType.None;

		public override string Value => m_textBoxInstance.Value;

		public override object OriginalValue => m_textBoxInstance.OriginalValue;

		public override TypeCode TypeCode => m_textBoxInstance.TypeCode;

		public override bool IsCompiled => false;

		public override bool ProcessedWithError
		{
			get
			{
				if (OriginalValue == null && !string.IsNullOrEmpty(Value))
				{
					return true;
				}
				return false;
			}
		}

		internal ShimTextRunInstance(TextRun textRunDef, TextBoxInstance textBoxInstance)
			: base(textRunDef)
		{
			m_textBoxInstance = textBoxInstance;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
		}
	}
}
