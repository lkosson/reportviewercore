using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLParagraph : RPLElement
	{
		private int m_textRunCount;

		private RPLSizes m_contentSizes;

		private Queue<RPLTextRun> m_textRuns;

		private long m_textRunOffsets = -1L;

		public RPLSizes ContentSizes
		{
			get
			{
				return m_contentSizes;
			}
			set
			{
				m_contentSizes = value;
			}
		}

		public int TextRunCount
		{
			get
			{
				return m_textRunCount;
			}
			set
			{
				m_textRunCount = value;
			}
		}

		internal Queue<RPLTextRun> TextRuns
		{
			set
			{
				m_textRuns = value;
				if (m_textRuns != null)
				{
					m_textRunCount = m_textRuns.Count;
				}
			}
		}

		internal RPLParagraph()
		{
			m_rplElementProps = new RPLParagraphProps();
			m_rplElementProps.Definition = new RPLParagraphPropsDef();
		}

		internal RPLParagraph(long textRunOffsets, RPLContext context)
			: base(context)
		{
			m_textRunOffsets = textRunOffsets;
		}

		internal RPLParagraph(Queue<RPLTextRun> textRuns, RPLParagraphProps rplElementProps)
			: base(rplElementProps)
		{
			m_textRuns = textRuns;
		}

		internal void AddTextRun(RPLTextRun textRun)
		{
			if (m_textRuns == null)
			{
				m_textRuns = new Queue<RPLTextRun>();
			}
			m_textRuns.Enqueue(textRun);
			m_textRunCount++;
		}

		public RPLTextRun GetNextTextRun()
		{
			if (m_textRuns != null)
			{
				if (m_textRuns.Count == 0)
				{
					m_textRuns = null;
					return null;
				}
				return m_textRuns.Dequeue();
			}
			if (m_textRunOffsets >= 0 && m_textRunCount > 0)
			{
				m_context.BinaryReader.BaseStream.Seek(m_textRunOffsets, SeekOrigin.Begin);
				long num = m_context.BinaryReader.ReadInt64();
				if (num == -1)
				{
					m_textRunOffsets = -1L;
					return null;
				}
				m_textRunCount--;
				m_textRunOffsets += 8L;
				return RPLReader.ReadTextRun(num, m_context);
			}
			return null;
		}
	}
}
