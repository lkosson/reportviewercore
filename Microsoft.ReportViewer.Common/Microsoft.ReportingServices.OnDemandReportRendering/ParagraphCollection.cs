using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ParagraphCollection : ReportElementCollectionBase<Paragraph>
	{
		private TextBox m_textBox;

		private Paragraph[] m_paragraphs;

		public override Paragraph this[int i]
		{
			get
			{
				if (i < 0 || i >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, i, 0, Count);
				}
				Paragraph paragraph = m_paragraphs[i];
				if (paragraph == null)
				{
					if (m_textBox.IsOldSnapshot)
					{
						paragraph = new ShimParagraph(m_textBox, m_textBox.RenderingContext);
					}
					else
					{
						Microsoft.ReportingServices.ReportIntermediateFormat.Paragraph paragraph2 = m_textBox.TexBoxDef.Paragraphs[i];
						paragraph = new InternalParagraph(m_textBox, i, paragraph2, m_textBox.RenderingContext);
					}
					m_paragraphs[i] = paragraph;
				}
				return paragraph;
			}
		}

		public override int Count => m_paragraphs.Length;

		internal ParagraphCollection(TextBox textBox)
		{
			m_textBox = textBox;
			if (m_textBox.IsOldSnapshot)
			{
				m_paragraphs = new Paragraph[1];
				return;
			}
			List<Microsoft.ReportingServices.ReportIntermediateFormat.Paragraph> paragraphs = m_textBox.TexBoxDef.Paragraphs;
			if (paragraphs != null)
			{
				m_paragraphs = new Paragraph[paragraphs.Count];
			}
			else
			{
				m_paragraphs = new Paragraph[0];
			}
		}

		internal void SetNewContext()
		{
			for (int i = 0; i < m_paragraphs.Length; i++)
			{
				if (m_paragraphs[i] != null)
				{
					m_paragraphs[i].SetNewContext();
				}
			}
		}
	}
}
