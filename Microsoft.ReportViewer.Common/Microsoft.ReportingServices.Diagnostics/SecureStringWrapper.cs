using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.ReportingServices.Diagnostics
{
	internal sealed class SecureStringWrapper : IDisposable
	{
		private SecureString m_secStr;

		public SecureStringWrapper(string regularString)
		{
			m_secStr = new SecureString();
			if (regularString != null)
			{
				for (int i = 0; i < regularString.Length; i++)
				{
					m_secStr.AppendChar(regularString[i]);
				}
				m_secStr.MakeReadOnly();
			}
		}

		public SecureStringWrapper(SecureString secureString)
		{
			m_secStr = ((secureString == null) ? new SecureString() : secureString.Copy());
		}

		public SecureStringWrapper(SecureStringWrapper secureStringWrapper)
		{
			if (secureStringWrapper != null && secureStringWrapper.m_secStr != null)
			{
				m_secStr = secureStringWrapper.m_secStr.Copy();
			}
			else
			{
				m_secStr = new SecureString();
			}
		}

		public static string GetDecryptedString(SecureString secureString)
		{
			string empty = string.Empty;
			IntPtr intPtr = IntPtr.Zero;
			if (secureString.Length != 0)
			{
				RuntimeHelpers.PrepareConstrainedRegions();
				try
				{
					RuntimeHelpers.PrepareConstrainedRegions();
					try
					{
					}
					finally
					{
						intPtr = Marshal.SecureStringToBSTR(secureString);
					}
					return Marshal.PtrToStringBSTR(intPtr);
				}
				finally
				{
					if (IntPtr.Zero != intPtr)
					{
						Marshal.ZeroFreeBSTR(intPtr);
					}
				}
			}
			return empty;
		}

		public override string ToString()
		{
			return GetDecryptedString(m_secStr);
		}

		internal SecureString GetUnderlyingSecureString()
		{
			return m_secStr;
		}

		void IDisposable.Dispose()
		{
			Dispose(disposing: true);
		}

		private void Dispose(bool disposing)
		{
			if (disposing && m_secStr != null)
			{
				m_secStr.Dispose();
				m_secStr = null;
			}
		}
	}
}
