using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class DocumentMap : IEnumerator<DocumentMapNode>, IDisposable, IEnumerator
	{
		protected bool m_isClosed;

		internal bool IsClosed => m_isClosed;

		public abstract DocumentMapNode Current
		{
			get;
		}

		object IEnumerator.Current => Current;

		public abstract void Close();

		public abstract void Dispose();

		public abstract bool MoveNext();

		public abstract void Reset();
	}
}
