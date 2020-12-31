using System;
using System.Runtime.Loader;

namespace Microsoft.ReportingServices
{
	internal sealed class RefCountedAppDomain : IDisposable
	{
		private class AppDomainRefCount
		{
			private int m_refCount;

			public void IncrementRefCount()
			{
				lock (this)
				{
					m_refCount++;
				}
			}

			public int DecrementRefCount()
			{
				lock (this)
				{
					return --m_refCount;
				}
			}
		}

		private AppDomainRefCount m_refCount;

		private AssemblyLoadContext m_assemblyLoadContext;

		public AssemblyLoadContext AssemblyLoadContext => m_assemblyLoadContext;

		public RefCountedAppDomain(AssemblyLoadContext assemblyLoadContext)
			: this(assemblyLoadContext, new AppDomainRefCount())
		{
		}

		private RefCountedAppDomain(AssemblyLoadContext assemblyLoadContext, AppDomainRefCount refCount)
		{
			m_assemblyLoadContext = assemblyLoadContext;
			m_refCount = refCount;
			m_refCount.IncrementRefCount();
		}

		public RefCountedAppDomain CreateNewReference()
		{
			return new RefCountedAppDomain(m_assemblyLoadContext, m_refCount);
		}

		public void Dispose()
		{
			if (m_assemblyLoadContext == null)
			{
				return;
			}
			try
			{
				if (m_refCount.DecrementRefCount() == 0)
				{
					m_assemblyLoadContext.Unload();
				}
			}
			catch (CannotUnloadAppDomainException)
			{
			}
			finally
			{
				m_assemblyLoadContext = null;
				m_refCount = null;
			}
		}
	}
}
