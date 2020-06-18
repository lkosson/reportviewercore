using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace Microsoft.ReportingServices.Diagnostics
{
	internal sealed class SafeSidPtr : SafeHandleZeroOrMinusOneIsInvalid
	{
		private struct SID_IDENTIFIER_AUTHORITY
		{
			internal byte m_Value0;

			internal byte m_Value1;

			internal byte m_Value2;

			internal byte m_Value3;

			internal byte m_Value4;

			internal byte m_Value5;
		}

		internal static readonly SafeSidPtr Zero = new SafeSidPtr(ownsHandle: false);

		private static byte[] SECURITY_NT_AUTHORITY = new byte[6]
		{
			0,
			0,
			0,
			0,
			0,
			5
		};

		internal const uint SECURITY_BUILTIN_DOMAIN_RID = 32u;

		internal const uint DOMAIN_ALIAS_RID_ADMINS = 544u;

		internal const uint SECURITY_LOCAL_SYSTEM_RID = 18u;

		private SafeSidPtr()
			: base(ownsHandle: true)
		{
		}

		private SafeSidPtr(bool ownsHandle)
			: base(ownsHandle)
		{
		}

		internal static bool AllocateAndInitializeSid(byte nSubAuthorityCount, uint nSubAuthority0, uint nSubAuthority1, uint nSubAuthority2, uint nSubAuthority3, uint nSubAuthority4, uint nSubAuthority5, uint nSubAuthority6, uint nSubAuthority7, out SafeSidPtr pSid)
		{
			SafeLocalFree safeLocalFree = null;
			try
			{
				SID_IDENTIFIER_AUTHORITY structure = default(SID_IDENTIFIER_AUTHORITY);
				structure.m_Value0 = SECURITY_NT_AUTHORITY[0];
				structure.m_Value1 = SECURITY_NT_AUTHORITY[1];
				structure.m_Value2 = SECURITY_NT_AUTHORITY[2];
				structure.m_Value3 = SECURITY_NT_AUTHORITY[3];
				structure.m_Value4 = SECURITY_NT_AUTHORITY[4];
				structure.m_Value5 = SECURITY_NT_AUTHORITY[5];
				safeLocalFree = SafeLocalFree.LocalAlloc(Marshal.SizeOf(structure));
				Marshal.StructureToPtr(structure, safeLocalFree.DangerousGetHandle(), fDeleteOld: true);
				return NativeMemoryMethods.AllocateAndInitializeSid(safeLocalFree, nSubAuthorityCount, nSubAuthority0, nSubAuthority1, nSubAuthority2, nSubAuthority3, nSubAuthority4, nSubAuthority5, nSubAuthority6, nSubAuthority7, out pSid);
			}
			finally
			{
				safeLocalFree?.Close();
			}
		}

		protected override bool ReleaseHandle()
		{
			return NativeMemoryMethods.FreeSid(handle) == IntPtr.Zero;
		}
	}
}
