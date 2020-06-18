using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace Microsoft.ReportingServices.Diagnostics
{
	internal class NativeMemoryMethods
	{
		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		internal static extern SafeLocalFree LocalAlloc(int uFlags, UIntPtr sizetdwBytes);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		internal static extern IntPtr LocalFree(IntPtr handle);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern bool AllocateAndInitializeSid(SafeLocalFree pSidAuthPtr, byte nSubAuthorityCount, uint nSubAuthority0, uint nSubAuthority1, uint nSubAuthority2, uint nSubAuthority3, uint nSubAuthority4, uint nSubAuthority5, uint nSubAuthority6, uint nSubAuthority7, out SafeSidPtr pSid);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern IntPtr FreeSid(IntPtr pSid);

		[DllImport("ole32.dll")]
		internal static extern SafeCoTaskMem CoTaskMemAlloc(int cb);

		[DllImport("ole32.dll")]
		internal static extern void CoTaskMemFree(IntPtr ptr);
	}
}
