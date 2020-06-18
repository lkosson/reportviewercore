using Microsoft.Win32.SafeHandles;
using System;

namespace Microsoft.ReportingServices.Diagnostics
{
	internal sealed class SafeCoTaskMem : SafeHandleZeroOrMinusOneIsInvalid
	{
		private SafeCoTaskMem()
			: base(ownsHandle: true)
		{
		}

		private SafeCoTaskMem(bool ownsHandle)
			: base(ownsHandle)
		{
		}

		internal SafeCoTaskMem Alloc(int cb)
		{
			SafeCoTaskMem safeCoTaskMem = NativeMemoryMethods.CoTaskMemAlloc(cb);
			if (safeCoTaskMem.IsInvalid)
			{
				safeCoTaskMem.SetHandleAsInvalid();
				throw new OutOfMemoryException();
			}
			return safeCoTaskMem;
		}

		protected override bool ReleaseHandle()
		{
			NativeMemoryMethods.CoTaskMemFree(handle);
			return true;
		}
	}
}
