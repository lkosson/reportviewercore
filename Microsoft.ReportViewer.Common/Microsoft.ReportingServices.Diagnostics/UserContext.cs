using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.Interfaces;
using System;
using Microsoft.Reporting;

namespace Microsoft.ReportingServices.Diagnostics
{
	internal class UserContext
	{
		protected string m_userName;

		protected object m_userToken;

		protected AuthenticationType m_authType;

		protected bool m_initialized;

		protected byte[] m_additionalUserToken;

		public string UserName => m_userName;

		public object UserToken => m_userToken;

		public AuthenticationType AuthenticationType => m_authType;

		public bool IsInitialized => m_initialized;

		public byte[] AdditionalUserToken
		{
			get
			{
				return m_additionalUserToken;
			}
			set
			{
				m_additionalUserToken = value;
			}
		}

		internal virtual bool UseAdditionalToken => true;

		internal virtual WindowsIdentity GetWindowsIdentity()
		{
			if (m_authType != AuthenticationType.Windows)
			{
				throw new WindowsIntegratedSecurityDisabledException();
			}
			RSTrace.SecurityTracer.Assert(m_userToken != null && m_userToken is IntPtr);
			IntPtr intPtr = (IntPtr)m_userToken;
			if (intPtr != IntPtr.Zero)
			{
				return new WindowsIdentity(intPtr);
			}
			return null;
		}

		public UserContext(string userName, object token, AuthenticationType authType)
		{
			m_userName = userName;
			m_userToken = token;
			m_authType = authType;
			m_initialized = true;
			m_additionalUserToken = null;
		}

		public UserContext()
		{
			m_userName = string.Empty;
			m_userToken = null;
			m_authType = AuthenticationType.None;
			m_additionalUserToken = null;
		}

		public UserContext(AuthenticationType authType)
		{
			m_userName = string.Empty;
			m_userToken = null;
			m_authType = authType;
			m_initialized = false;
			m_additionalUserToken = null;
		}
	}
}
