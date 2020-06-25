using System;
using System.Net;
using System.Security.Principal;

namespace Microsoft.Reporting.WinForms
{
	public sealed class ReportServerCredentials : IReportServerCredentials
	{
		private object m_syncObject;

		private System.Security.Principal.WindowsIdentity m_impersonationUser;

		private ICredentials m_networkCredentials;

		private Cookie m_formsCookie;

		private string m_formsUserName;

		private string m_formsPassword;

		private string m_formsAuthority;

		public System.Security.Principal.WindowsIdentity ImpersonationUser
		{
			get
			{
				return m_impersonationUser;
			}
			set
			{
				lock (m_syncObject)
				{
					m_impersonationUser = value;
					OnChange();
				}
			}
		}

		public ICredentials NetworkCredentials
		{
			get
			{
				return m_networkCredentials;
			}
			set
			{
				lock (m_syncObject)
				{
					m_networkCredentials = value;
					OnChange();
				}
			}
		}

		internal event EventHandler Change;

		internal ReportServerCredentials(object syncObject)
		{
			m_syncObject = syncObject;
		}

		internal void CopyFrom(ReportServerCredentials other)
		{
			m_impersonationUser = other.m_impersonationUser;
			m_networkCredentials = other.m_networkCredentials;
			m_formsCookie = other.m_formsCookie;
			m_formsUserName = other.m_formsUserName;
			m_formsPassword = other.m_formsPassword;
			m_formsAuthority = other.m_formsAuthority;
		}

		private void OnChange()
		{
			if (this.Change != null)
			{
				this.Change(this, EventArgs.Empty);
			}
		}

		public bool GetFormsCredentials(out Cookie authCookie, out string userName, out string password, out string authority)
		{
			lock (m_syncObject)
			{
				authCookie = m_formsCookie;
				userName = m_formsUserName;
				password = m_formsPassword;
				authority = m_formsAuthority;
				return authCookie != null || userName != null || password != null || m_formsAuthority != null;
			}
		}

		public void SetFormsCredentials(Cookie authCookie, string userName, string password, string authority)
		{
			lock (m_syncObject)
			{
				m_formsCookie = authCookie;
				m_formsUserName = userName;
				m_formsPassword = password;
				m_formsAuthority = authority;
				OnChange();
			}
		}
	}
}
