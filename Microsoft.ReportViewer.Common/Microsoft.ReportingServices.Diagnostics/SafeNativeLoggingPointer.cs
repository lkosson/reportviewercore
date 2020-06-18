using Microsoft.Win32.SafeHandles;
using System.Runtime.ConstrainedExecution;

namespace Microsoft.ReportingServices.Diagnostics
{
	internal class SafeNativeLoggingPointer : SafeHandleZeroOrMinusOneIsInvalid
	{
		private SafeNativeLoggingPointer()
			: base(ownsHandle: true)
		{
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		protected override bool ReleaseHandle()
		{
			NativeLoggingMethods.ReleaseNativeLoggingObject(handle);
			return true;
		}
	}
}
