using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalDocumentMap : DocumentMap, IDocumentMap, IEnumerator<OnDemandDocumentMapNode>, IDisposable, IEnumerator
	{
		private DocumentMapReader m_reader;

		OnDemandDocumentMapNode IEnumerator<OnDemandDocumentMapNode>.Current => Current;

		public override DocumentMapNode Current => m_reader.Current;

		object IEnumerator.Current => Current;

		internal InternalDocumentMap(DocumentMapReader aReader)
		{
			m_reader = aReader;
		}

		public override void Close()
		{
			if (m_reader != null)
			{
				m_reader.Close();
			}
			m_reader = null;
			m_isClosed = true;
		}

		public override void Dispose()
		{
			Close();
		}

		public override bool MoveNext()
		{
			return m_reader.MoveNext();
		}

		public override void Reset()
		{
			m_reader.Reset();
		}
	}
}
