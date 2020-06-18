using Microsoft.ReportingServices.Diagnostics.Utilities;
using System;

namespace Microsoft.ReportingServices.DataExtensions
{
	internal abstract class BaseDataWrapper : IDisposable
	{
		private object m_underlyingObject;

		protected object UnderlyingObject
		{
			get
			{
				return m_underlyingObject;
			}
			set
			{
				RSTrace.DataExtensionTracer.Assert(m_underlyingObject == null, "Should never replace the underlying connection");
				m_underlyingObject = value;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			if (m_underlyingObject == null)
			{
				return ((BaseDataWrapper)obj).m_underlyingObject == null;
			}
			return m_underlyingObject.Equals(((BaseDataWrapper)obj).m_underlyingObject);
		}

		public override int GetHashCode()
		{
			if (m_underlyingObject != null)
			{
				return m_underlyingObject.GetHashCode();
			}
			return base.GetHashCode();
		}

		public void Dispose()
		{
			Dispose(disposing: true);
		}

		protected BaseDataWrapper(object underlyingObject)
		{
			m_underlyingObject = underlyingObject;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				(m_underlyingObject as IDisposable)?.Dispose();
			}
			m_underlyingObject = null;
		}
	}
}
