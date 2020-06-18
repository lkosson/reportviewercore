using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ParagraphInstanceEnumerator : IEnumerator<ParagraphInstance>, IDisposable, IEnumerator
	{
		private TextBox m_textbox;

		private ParagraphInstance m_currentParagraphInstance;

		private int m_currentCompiledIndex;

		private int m_currentIndex;

		private CompiledParagraphInstanceCollection m_paragraphs;

		public ParagraphInstance Current => m_currentParagraphInstance;

		object IEnumerator.Current => m_currentParagraphInstance;

		internal ParagraphInstanceEnumerator(TextBox textbox)
		{
			m_textbox = textbox;
		}

		public void Dispose()
		{
			Reset();
		}

		public bool MoveNext()
		{
			if (m_currentIndex < m_textbox.Paragraphs.Count)
			{
				Paragraph paragraph = m_textbox.Paragraphs[m_currentIndex];
				if (paragraph.TextRuns.Count == 1 && paragraph.TextRuns[0].Instance.MarkupType != 0)
				{
					if (m_paragraphs == null)
					{
						m_paragraphs = paragraph.TextRuns[0].CompiledInstance.CompiledParagraphInstances;
					}
					if (m_currentCompiledIndex >= m_paragraphs.Count)
					{
						m_paragraphs = null;
						m_currentCompiledIndex = 0;
						m_currentIndex++;
						return MoveNext();
					}
					m_currentParagraphInstance = m_paragraphs[m_currentCompiledIndex];
					m_currentCompiledIndex++;
				}
				else
				{
					m_currentParagraphInstance = paragraph.Instance;
					m_currentIndex++;
				}
				return true;
			}
			return false;
		}

		public void Reset()
		{
			m_paragraphs = null;
			m_currentParagraphInstance = null;
			m_currentIndex = 0;
			m_currentCompiledIndex = 0;
		}
	}
}
