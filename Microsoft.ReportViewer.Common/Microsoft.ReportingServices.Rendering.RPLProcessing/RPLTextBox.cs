using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLTextBox : RPLItem
	{
		private int m_paragraphCount;

		private Queue<RPLParagraph> m_paragraphs;

		private long m_paragraphOffsets = -1L;

		internal long ParagraphOffsets
		{
			get
			{
				return m_paragraphOffsets;
			}
			set
			{
				m_paragraphOffsets = value;
			}
		}

		internal int ParagraphCount
		{
			set
			{
				m_paragraphCount = value;
			}
		}

		internal Queue<RPLParagraph> Paragraphs
		{
			set
			{
				m_paragraphs = value;
				if (m_paragraphs != null)
				{
					m_paragraphCount = m_paragraphs.Count;
				}
			}
		}

		internal RPLTextBox()
		{
			m_rplElementProps = new RPLTextBoxProps();
			m_rplElementProps.Definition = new RPLTextBoxPropsDef();
		}

		internal RPLTextBox(long startOffset, RPLContext context)
			: base(startOffset, context)
		{
		}

		internal RPLTextBox(RPLItemProps rplElementProps)
			: base(rplElementProps)
		{
			RPLTextBoxProps rPLTextBoxProps = rplElementProps as RPLTextBoxProps;
			if (rPLTextBoxProps != null)
			{
				rPLTextBoxProps.Value = null;
				m_paragraphs = null;
				m_paragraphCount = 0;
			}
		}

		public RPLParagraph GetNextParagraph()
		{
			if (m_paragraphs != null)
			{
				if (m_paragraphs.Count == 0)
				{
					m_paragraphs = null;
					return null;
				}
				return m_paragraphs.Dequeue();
			}
			if (m_paragraphOffsets >= 0 && m_paragraphCount > 0)
			{
				m_context.BinaryReader.BaseStream.Seek(m_paragraphOffsets, SeekOrigin.Begin);
				long num = m_context.BinaryReader.ReadInt64();
				if (num == -1)
				{
					m_paragraphOffsets = -1L;
					return null;
				}
				m_paragraphCount--;
				m_paragraphOffsets += 8L;
				return RPLReader.ReadParagraph(num, m_context);
			}
			return null;
		}

		internal void AddParagraph(RPLParagraph paragraph)
		{
			if (m_paragraphs == null)
			{
				m_paragraphs = new Queue<RPLParagraph>();
			}
			m_paragraphs.Enqueue(paragraph);
			m_paragraphCount++;
		}

		public void GetSimpleStyles(out RPLStyleProps nonShared, out RPLStyleProps shared, RPLParagraph paragraph, RPLTextRun textRun)
		{
			shared = new RPLStyleProps();
			nonShared = new RPLStyleProps();
			shared.AddAll(ElementPropsDef.SharedStyle);
			nonShared.AddAll(ElementProps.NonSharedStyle);
			nonShared.AddAll(paragraph.ElementProps.NonSharedStyle);
			if (paragraph.ElementProps.Definition != null)
			{
				shared.AddAll(paragraph.ElementProps.Definition.SharedStyle);
			}
			nonShared.AddAll(textRun.ElementProps.NonSharedStyle);
			if (textRun.ElementProps.Definition != null)
			{
				shared.AddAll(textRun.ElementProps.Definition.SharedStyle);
			}
		}

		public RPLElementStyle GetSimpleStyles(RPLParagraph paragraph, RPLTextRun textRun)
		{
			RPLStyleProps shared = null;
			RPLStyleProps nonShared = null;
			GetSimpleStyles(out nonShared, out shared, paragraph, textRun);
			return new RPLElementStyle(nonShared, shared);
		}
	}
}
