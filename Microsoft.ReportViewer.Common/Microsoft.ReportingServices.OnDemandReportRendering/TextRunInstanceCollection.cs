using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class TextRunInstanceCollection : IEnumerable<TextRunInstance>, IEnumerable
	{
		private ParagraphInstance m_paragraphInstance;

		internal TextRunInstanceCollection(ParagraphInstance paragraphInstance)
		{
			m_paragraphInstance = paragraphInstance;
		}

		public IEnumerator<TextRunInstance> GetEnumerator()
		{
			return new TextRunInstanceEnumerator(m_paragraphInstance);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
