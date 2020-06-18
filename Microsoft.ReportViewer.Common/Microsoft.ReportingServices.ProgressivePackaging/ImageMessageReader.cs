using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ProgressivePackaging
{
	internal abstract class ImageMessageReader<T> : IImageMessageReader, IEnumerable, IDisposable, IEnumerable<T> where T : ImageMessageElement
	{
		private bool m_disposed;

		public abstract IEnumerator<T> GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<T>)this).GetEnumerator();
		}

		public abstract void InternalDispose();

		public void Dispose()
		{
			if (!m_disposed)
			{
				InternalDispose();
				m_disposed = true;
			}
		}
	}
}
