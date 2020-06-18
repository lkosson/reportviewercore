using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ParagraphInstanceCollection : IEnumerable<ParagraphInstance>, IEnumerable
	{
		private TextBox m_textbox;

		internal ParagraphInstanceCollection(TextBox textbox)
		{
			m_textbox = textbox;
		}

		public IEnumerator<ParagraphInstance> GetEnumerator()
		{
			return new ParagraphInstanceEnumerator(m_textbox);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
