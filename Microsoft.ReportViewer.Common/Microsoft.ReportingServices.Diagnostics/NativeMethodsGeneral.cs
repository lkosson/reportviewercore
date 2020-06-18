using System.Runtime.InteropServices;

namespace Microsoft.ReportingServices.Diagnostics
{
	internal static class NativeMethodsGeneral
	{
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		private struct MEMORYSTATUSEX
		{
			internal int dwLength;

			internal int dwMemoryLoad;

			internal long ullTotalPhys;

			internal long ullAvailPhys;

			internal long ullTotalPageFile;

			internal long ullAvailPageFile;

			internal long ullTotalVirtual;

			internal long ullAvailVirtual;

			internal long ullAvailExtendedVirtual;

			internal void Init()
			{
				dwLength = Marshal.SizeOf(typeof(MEMORYSTATUSEX));
			}
		}

		public static bool GlobalMemoryStatusEx(out long ullAvailPhys, out long ullAvailVirtual)
		{
			MEMORYSTATUSEX memoryStatusEx = default(MEMORYSTATUSEX);
			memoryStatusEx.Init();
			if (GlobalMemoryStatusEx(ref memoryStatusEx) != 0)
			{
				ullAvailPhys = memoryStatusEx.ullAvailPhys;
				ullAvailVirtual = memoryStatusEx.ullAvailVirtual;
				return true;
			}
			ullAvailPhys = 0L;
			ullAvailVirtual = 0L;
			return false;
		}

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		private static extern int GlobalMemoryStatusEx(ref MEMORYSTATUSEX memoryStatusEx);
	}
}
