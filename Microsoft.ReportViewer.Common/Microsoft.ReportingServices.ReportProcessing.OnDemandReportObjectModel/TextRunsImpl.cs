using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	internal sealed class TextRunsImpl : TextRuns
	{
		private Microsoft.ReportingServices.ReportIntermediateFormat.TextBox m_textBoxDef;

		private Microsoft.ReportingServices.ReportIntermediateFormat.Paragraph m_paragraphDef;

		private TextRunImpl[] m_textRuns;

		private Microsoft.ReportingServices.RdlExpressions.ReportRuntime m_reportRT;

		private IErrorContext m_iErrorContext;

		private IScope m_scope;

		public override TextRun this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				TextRunImpl textRunImpl = m_textRuns[index];
				if (textRunImpl == null)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.TextRun textRunDef = m_paragraphDef.TextRuns[index];
					textRunImpl = new TextRunImpl(m_textBoxDef, textRunDef, m_reportRT, m_iErrorContext, m_scope);
					m_textRuns[index] = textRunImpl;
				}
				return textRunImpl;
			}
		}

		internal int Count => m_textRuns.Length;

		internal TextRunsImpl(Microsoft.ReportingServices.ReportIntermediateFormat.Paragraph paragraphDef, Microsoft.ReportingServices.RdlExpressions.ReportRuntime reportRT, IErrorContext iErrorContext, IScope scope)
		{
			m_textBoxDef = paragraphDef.TextBox;
			m_paragraphDef = paragraphDef;
			m_reportRT = reportRT;
			m_iErrorContext = iErrorContext;
			m_scope = scope;
			List<Microsoft.ReportingServices.ReportIntermediateFormat.TextRun> textRuns = m_paragraphDef.TextRuns;
			if (textRuns != null)
			{
				m_textRuns = new TextRunImpl[textRuns.Count];
			}
			else
			{
				m_textRuns = new TextRunImpl[0];
			}
		}

		internal void Reset()
		{
			for (int i = 0; i < m_textRuns.Length; i++)
			{
				m_textRuns[i]?.Reset();
			}
		}
	}
}
