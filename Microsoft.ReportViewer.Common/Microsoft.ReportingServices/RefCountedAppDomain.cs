using System;

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

		private AppDomain m_appDomain;

		public AppDomain AppDomain => m_appDomain;

		public RefCountedAppDomain(AppDomain appDomain)
			: this(appDomain, new AppDomainRefCount())
		{
		}

		private RefCountedAppDomain(AppDomain appDomain, AppDomainRefCount refCount)
		{
			m_appDomain = appDomain;
			m_refCount = refCount;
			m_refCount.IncrementRefCount();
		}

		public RefCountedAppDomain CreateNewReference()
		{
			return new RefCountedAppDomain(m_appDomain, m_refCount);
		}

		public void Dispose()
		{
			if (m_appDomain == null)
			{
				return;
			}
			try
			{
				if (m_refCount.DecrementRefCount() == 0)
				{
					AppDomain.Unload(m_appDomain);
				}
			}
			catch (CannotUnloadAppDomainException)
			{
			}
			finally
			{
				m_appDomain = null;
				m_refCount = null;
			}
		}
	}
}
