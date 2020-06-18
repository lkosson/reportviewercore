using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.ConstrainedExecution;

namespace Microsoft.ReportingServices.Rendering.RichText
{
	internal sealed class Win32DCSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		private static Win32DCSafeHandle m_zero = new Win32DCSafeHandle(IntPtr.Zero, ownsHandle: false);

		public IntPtr Handle => handle;

		public static Win32DCSafeHandle Zero => m_zero;

		public Win32DCSafeHandle()
			: base(ownsHandle: true)
		{
		}

		public Win32DCSafeHandle(IntPtr hdc, bool ownsHandle)
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
				result = Win32.DeleteDC(handle);
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
