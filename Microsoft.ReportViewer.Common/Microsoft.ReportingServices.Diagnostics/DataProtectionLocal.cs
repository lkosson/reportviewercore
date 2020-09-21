using Microsoft.ReportingServices.Diagnostics.Utilities;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.ReportingServices.Diagnostics
{
	internal static class DataProtectionLocal
	{
		private sealed class DataProtectionLocalInstance : IDataProtection
		{
			public byte[] ProtectData(string unprotectedData, string tag)
			{
				if (unprotectedData == null)
				{
					return null;
				}
				return LocalProtectData(Encoding.Unicode.GetBytes(unprotectedData));
			}

			public string UnprotectDataToString(byte[] protectedData, string tag)
			{
				if (protectedData == null)
				{
					return null;
				}
				byte[] array = LocalUnprotectData(protectedData);
				if (array == null)
				{
					return null;
				}
				if (protectedData.Length == 0)
				{
					return string.Empty;
				}
				return Encoding.Unicode.GetString(array);
			}
		}

		private static int m_dwProtectionFlags = 4;

		private static IDataProtection m_dpInstance;

		public static ProtectionMode GlobalProtectionMode
		{
			set
			{
				switch (value)
				{
				case ProtectionMode.LocalSystemEncryption:
					m_dwProtectionFlags = 4;
					break;
				case ProtectionMode.CurrentUserEncryption:
					m_dwProtectionFlags = 0;
					break;
				}
			}
		}

		public static IDataProtection Instance
		{
			[DebuggerStepThrough]
			get
			{
				if (m_dpInstance == null)
				{
					m_dpInstance = new DataProtectionLocalInstance();
				}
				return m_dpInstance;
			}
		}

		public static byte[] LocalProtectData(byte[] data)
		{
			// No need to protect data for local reports (no connection)
			return data;
		}

		public static byte[] LocalUnprotectData(byte[] data)
		{
			// No need to protect data for local reports (no connection)
			return data;
		}
	}
}
