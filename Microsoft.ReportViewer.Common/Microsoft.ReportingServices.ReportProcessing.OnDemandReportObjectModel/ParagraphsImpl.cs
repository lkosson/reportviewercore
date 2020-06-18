using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	internal sealed class ParagraphsImpl : Paragraphs
	{
		private Microsoft.ReportingServices.ReportIntermediateFormat.TextBox m_textBoxDef;

		private ParagraphImpl[] m_paragraphs;

		private Microsoft.ReportingServices.RdlExpressions.ReportRuntime m_reportRT;

		private IErrorContext m_iErrorContext;

		private IScope m_scope;

		public override Paragraph this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				ParagraphImpl paragraphImpl = m_paragraphs[index];
				if (paragraphImpl == null)
				{
					paragraphImpl = new ParagraphImpl(m_textBoxDef.Paragraphs[index], m_reportRT, m_iErrorContext, m_scope);
					m_paragraphs[index] = paragraphImpl;
				}
				return paragraphImpl;
			}
		}

		internal int Count => m_paragraphs.Length;

		internal ParagraphsImpl(Microsoft.ReportingServices.ReportIntermediateFormat.TextBox textBoxDef, Microsoft.ReportingServices.RdlExpressions.ReportRuntime reportRT, IErrorContext iErrorContext, IScope scope)
		{
			m_textBoxDef = textBoxDef;
			m_reportRT = reportRT;
			m_iErrorContext = iErrorContext;
			m_scope = scope;
			List<Microsoft.ReportingServices.ReportIntermediateFormat.Paragraph> paragraphs = m_textBoxDef.Paragraphs;
			if (paragraphs != null)
			{
				m_paragraphs = new ParagraphImpl[paragraphs.Count];
			}
			else
			{
				m_paragraphs = new ParagraphImpl[0];
			}
		}

		internal void Reset()
		{
			for (int i = 0; i < m_paragraphs.Length; i++)
			{
				m_paragraphs[i]?.Reset();
			}
		}
	}
}
