using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class RichTextParser
	{
		protected CompiledStyleInfo m_currentStyle;

		protected CompiledParagraphInfo m_currentParagraph;

		protected static ReportSize DefaultParagraphSpacing = new ReportSize("10pt");

		internal const int ParagraphListLevelMin = 0;

		internal const int ParagraphListLevelMax = 9;

		protected bool m_allowMultipleParagraphs;

		protected ICompiledParagraphInstance m_currentParagraphInstance;

		protected ICompiledTextRunInstance m_currentTextRunInstance;

		protected IRichTextInstanceCreator m_IRichTextInstanceCreator;

		protected IList<ICompiledParagraphInstance> m_paragraphInstanceCollection;

		protected ICompiledParagraphInstance m_onlyParagraphInstance;

		protected IRichTextLogger m_richTextLogger;

		private bool m_loggedListLevelWarning;

		private const string m_propertyListLevel = "ListLevel";

		internal RichTextParser(bool allowMultipleParagraphs, IRichTextInstanceCreator iRichTextInstanceCreator, IRichTextLogger richTextLogger)
		{
			m_allowMultipleParagraphs = allowMultipleParagraphs;
			m_IRichTextInstanceCreator = iRichTextInstanceCreator;
			m_richTextLogger = richTextLogger;
		}

		internal virtual IList<ICompiledParagraphInstance> Parse(string richText)
		{
			m_currentStyle = new CompiledStyleInfo();
			m_currentParagraph = new CompiledParagraphInfo();
			m_paragraphInstanceCollection = m_IRichTextInstanceCreator.CreateParagraphInstanceCollection();
			if (!string.IsNullOrEmpty(richText))
			{
				InternalParse(richText);
			}
			m_currentParagraph = new CompiledParagraphInfo();
			if (m_paragraphInstanceCollection.Count == 0)
			{
				m_currentParagraphInstance = CreateParagraphInstance();
				m_currentTextRunInstance = CreateTextRunInstance();
				m_currentParagraphInstance.Style = m_IRichTextInstanceCreator.CreateStyleInstance(isParagraph: true);
				m_currentTextRunInstance.Style = m_IRichTextInstanceCreator.CreateStyleInstance(isParagraph: false);
			}
			else
			{
				for (int i = 0; i < m_paragraphInstanceCollection.Count; i++)
				{
					m_currentParagraphInstance = m_paragraphInstanceCollection[i];
					if (m_currentParagraphInstance.CompiledTextRunInstances == null || m_currentParagraphInstance.CompiledTextRunInstances.Count == 0)
					{
						m_currentTextRunInstance = CreateTextRunInstance();
						m_currentTextRunInstance.Style = m_IRichTextInstanceCreator.CreateStyleInstance(isParagraph: true);
					}
				}
			}
			CloseParagraph();
			return m_paragraphInstanceCollection;
		}

		protected abstract void InternalParse(string richText);

		protected virtual bool AppendText(string value)
		{
			return AppendText(value, onlyIfValueExists: false);
		}

		protected virtual bool AppendText(string value, bool onlyIfValueExists)
		{
			if (m_currentParagraphInstance != null)
			{
				IList<ICompiledTextRunInstance> compiledTextRunInstances = m_currentParagraphInstance.CompiledTextRunInstances;
				if (compiledTextRunInstances.Count > 0)
				{
					m_currentTextRunInstance = compiledTextRunInstances[compiledTextRunInstances.Count - 1];
					if (onlyIfValueExists && string.IsNullOrEmpty(m_currentTextRunInstance.Value))
					{
						m_currentTextRunInstance = null;
						return false;
					}
				}
			}
			SetTextRunValue(value);
			return true;
		}

		protected virtual void SetTextRunValue(string value)
		{
			if (m_currentTextRunInstance == null)
			{
				m_currentTextRunInstance = CreateTextRunInstance();
			}
			m_currentTextRunInstance.Value += value;
			if (m_currentTextRunInstance.Style == null)
			{
				ICompiledStyleInstance compiledStyleInstance = m_IRichTextInstanceCreator.CreateStyleInstance(isParagraph: false);
				m_currentStyle.PopulateStyleInstance(compiledStyleInstance, isParagraphStyle: false);
				m_currentTextRunInstance.Style = compiledStyleInstance;
			}
			if (m_currentParagraphInstance.Style == null)
			{
				m_currentParagraphInstance.Style = m_IRichTextInstanceCreator.CreateStyleInstance(isParagraph: true);
			}
			m_currentTextRunInstance = null;
		}

		protected virtual ICompiledParagraphInstance CreateParagraphInstance()
		{
			if (!m_allowMultipleParagraphs && m_onlyParagraphInstance != null)
			{
				m_currentParagraphInstance = m_onlyParagraphInstance;
				AppendText(Environment.NewLine, onlyIfValueExists: true);
				return m_onlyParagraphInstance;
			}
			ICompiledParagraphInstance compiledParagraphInstance = m_IRichTextInstanceCreator.CreateParagraphInstance();
			if (m_allowMultipleParagraphs)
			{
				m_currentParagraph.PopulateParagraph(compiledParagraphInstance);
				int listLevel = compiledParagraphInstance.ListLevel;
				if (listLevel > 9)
				{
					if (!m_loggedListLevelWarning)
					{
						m_richTextLogger.RegisterOutOfRangeSizeWarning("ListLevel", Convert.ToString(listLevel, CultureInfo.InvariantCulture), Convert.ToString(0, CultureInfo.InvariantCulture), Convert.ToString(9, CultureInfo.InvariantCulture));
						m_loggedListLevelWarning = true;
					}
					compiledParagraphInstance.ListLevel = 9;
				}
			}
			else
			{
				m_onlyParagraphInstance = compiledParagraphInstance;
			}
			ICompiledStyleInstance compiledStyleInstance = m_IRichTextInstanceCreator.CreateStyleInstance(isParagraph: true);
			m_currentStyle.PopulateStyleInstance(compiledStyleInstance, isParagraphStyle: true);
			compiledParagraphInstance.Style = compiledStyleInstance;
			IList<ICompiledTextRunInstance> list2 = compiledParagraphInstance.CompiledTextRunInstances = m_IRichTextInstanceCreator.CreateTextRunInstanceCollection();
			m_paragraphInstanceCollection.Add(compiledParagraphInstance);
			return compiledParagraphInstance;
		}

		protected virtual ICompiledTextRunInstance CreateTextRunInstance()
		{
			if (m_currentParagraphInstance == null)
			{
				m_currentParagraphInstance = CreateParagraphInstance();
			}
			IList<ICompiledTextRunInstance> compiledTextRunInstances = m_currentParagraphInstance.CompiledTextRunInstances;
			ICompiledTextRunInstance compiledTextRunInstance = m_IRichTextInstanceCreator.CreateTextRunInstance();
			ICompiledStyleInstance styleInstance = compiledTextRunInstance.Style = m_IRichTextInstanceCreator.CreateStyleInstance(isParagraph: false);
			m_currentStyle.PopulateStyleInstance(styleInstance, isParagraphStyle: false);
			compiledTextRunInstances.Add(compiledTextRunInstance);
			return compiledTextRunInstance;
		}

		protected virtual void CloseParagraph()
		{
			m_currentParagraphInstance = null;
			m_currentTextRunInstance = null;
		}
	}
}
