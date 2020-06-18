using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.ConstrainedExecution;

namespace Microsoft.ReportingServices.Rendering.RichText
{
	internal sealed class ScriptCacheSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		public ScriptCacheSafeHandle()
			: base(ownsHandle: true)
		{
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		protected override bool ReleaseHandle()
		{
			int hr = Win32.ScriptFreeCache(ref handle);
			handle = IntPtr.Zero;
			return Win32.Succeeded(hr);
		}
	}
}
