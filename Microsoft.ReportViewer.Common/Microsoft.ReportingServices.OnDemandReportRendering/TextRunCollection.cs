using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class TextRunCollection : ReportElementCollectionBase<TextRun>
	{
		private Paragraph m_paragraph;

		private TextRun[] m_textRuns;

		public override TextRun this[int i]
		{
			get
			{
				if (i < 0 || i >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, i, 0, Count);
				}
				TextRun textRun = m_textRuns[i];
				if (textRun == null)
				{
					if (m_paragraph.IsOldSnapshot)
					{
						textRun = new ShimTextRun(m_paragraph, m_paragraph.RenderingContext);
					}
					else
					{
						Microsoft.ReportingServices.ReportIntermediateFormat.TextRun textRun2 = ((InternalParagraph)m_paragraph).ParagraphDef.TextRuns[i];
						textRun = new InternalTextRun(m_paragraph, i, textRun2, m_paragraph.RenderingContext);
					}
					m_textRuns[i] = textRun;
				}
				return textRun;
			}
		}

		public override int Count => m_textRuns.Length;

		internal TextRunCollection(Paragraph paragraph)
		{
			m_paragraph = paragraph;
			if (m_paragraph.IsOldSnapshot)
			{
				m_textRuns = new TextRun[1];
				return;
			}
			List<Microsoft.ReportingServices.ReportIntermediateFormat.TextRun> textRuns = ((InternalParagraph)m_paragraph).ParagraphDef.TextRuns;
			if (textRuns != null)
			{
				m_textRuns = new TextRun[textRuns.Count];
			}
			else
			{
				m_textRuns = new TextRun[0];
			}
		}

		internal void SetNewContext()
		{
			for (int i = 0; i < m_textRuns.Length; i++)
			{
				if (m_textRuns[i] != null)
				{
					m_textRuns[i].SetNewContext();
				}
			}
		}
	}
}
