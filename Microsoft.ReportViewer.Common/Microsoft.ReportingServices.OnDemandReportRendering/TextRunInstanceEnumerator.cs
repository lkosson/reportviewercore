using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class TextRunInstanceEnumerator : IEnumerator<TextRunInstance>, IDisposable, IEnumerator
	{
		private ParagraphInstance m_paragraphInstance;

		private TextRunInstance m_textRunInstance;

		private int m_currentIndex;

		private int m_currentCompiledIndex;

		private CompiledTextRunInstanceCollection m_textRunInstances;

		public TextRunInstance Current => m_textRunInstance;

		object IEnumerator.Current => m_textRunInstance;

		internal TextRunInstanceEnumerator(ParagraphInstance paragraphInstance)
		{
			m_paragraphInstance = paragraphInstance;
		}

		public void Dispose()
		{
			Reset();
		}

		public bool MoveNext()
		{
			TextRunCollection textRuns = m_paragraphInstance.Definition.TextRuns;
			if (m_currentIndex < textRuns.Count)
			{
				TextRun textRun = textRuns[m_currentIndex];
				if (textRun.Instance.MarkupType != 0)
				{
					if (m_textRunInstances == null)
					{
						if (textRuns.Count > 1)
						{
							m_textRunInstances = textRun.CompiledInstance.CompiledParagraphInstances[0].CompiledTextRunInstances;
						}
						else
						{
							m_textRunInstances = ((CompiledParagraphInstance)m_paragraphInstance).CompiledTextRunInstances;
						}
					}
					if (m_currentCompiledIndex >= m_textRunInstances.Count)
					{
						m_textRunInstances = null;
						m_currentCompiledIndex = 0;
						m_currentIndex++;
						return MoveNext();
					}
					m_textRunInstance = m_textRunInstances[m_currentCompiledIndex];
					m_currentCompiledIndex++;
				}
				else
				{
					m_textRunInstance = textRun.Instance;
					m_currentIndex++;
				}
				return true;
			}
			return false;
		}

		public void Reset()
		{
			m_textRunInstance = null;
			m_currentIndex = 0;
			m_currentCompiledIndex = 0;
		}
	}
}
