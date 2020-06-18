using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.ConstrainedExecution;

namespace Microsoft.ReportingServices.Rendering.RichText
{
	internal sealed class Win32ObjectSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		private static Win32ObjectSafeHandle m_zero = new Win32ObjectSafeHandle(IntPtr.Zero, ownsHandle: false);

		public IntPtr Handle => handle;

		public static Win32ObjectSafeHandle Zero => m_zero;

		public Win32ObjectSafeHandle()
			: base(ownsHandle: true)
		{
		}

		public Win32ObjectSafeHandle(IntPtr hdc, bool ownsHandle)
			: base(ownsHandle)
		{
			handle = hdc;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		protected override bool ReleaseHandle()
		{
			bool result;
			try
			{
				result = (Win32.DeleteObject(handle) != 0);
			}
			catch
			{
				result = false;
			}
			handle = IntPtr.Zero;
			return result;
		}
	}
}
