using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Microsoft.ReportingServices.Diagnostics
{
	internal sealed class SafeLocalFree : SafeHandleZeroOrMinusOneIsInvalid
	{
		internal const int LMEM_FIXED = 0;

		internal const int LMEM_ZEROINIT = 64;

		internal const int LPTR = 64;

		private const int NULL = 0;

		internal static readonly SafeLocalFree Zero = new SafeLocalFree(ownsHandle: false);

		private SafeLocalFree()
			: base(ownsHandle: true)
		{
		}

		private SafeLocalFree(bool ownsHandle)
			: base(ownsHandle)
		{
		}

		internal static SafeLocalFree LocalAlloc(int cb)
		{
			return LocalAlloc(0, cb);
		}

		internal static SafeLocalFree LocalAlloc(int flags, int cb)
		{
			SafeLocalFree safeLocalFree = NativeMemoryMethods.LocalAlloc(flags, (UIntPtr)(ulong)cb);
			if (safeLocalFree.IsInvalid)
			{
				safeLocalFree.SetHandleAsInvalid();
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}
			return safeLocalFree;
		}

		protected override bool ReleaseHandle()
		{
			return NativeMemoryMethods.LocalFree(handle) == IntPtr.Zero;
		}
	}
}
