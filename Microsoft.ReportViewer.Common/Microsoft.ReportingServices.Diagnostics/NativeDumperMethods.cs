using System;
using System.Runtime.InteropServices;

namespace Microsoft.ReportingServices.Diagnostics
{
	internal static class NativeDumperMethods
	{
		[Flags]
		public enum DumperFlags
		{
			Default = 0x0,
			NoMiniDump = 0x2,
			ReferencedMemory = 0x8,
			AllMemory = 0x10,
			AllThreads = 0x20,
			MatchFilename = 0x40,
			Verbose = 0x100,
			WaitAtExit = 0x200,
			SendToWatson = 0x400,
			UseDefault = 0x800,
			MaximumDump = 0x1000,
			DoubleDump = 0x2000,
			ForceWatson = 0x4000
		}

		private const string ReportingServiceExe = "ReportingServicesService.exe";

		[DllImport("ReportingServicesService.exe", CharSet = CharSet.Unicode, EntryPoint = "RsDumpDump")]
		public static extern void Dump();

		[DllImport("ReportingServicesService.exe", CharSet = CharSet.Unicode, EntryPoint = "RsDumpSetErrorText")]
		public static extern void SetErrorText(string errorText);

		[DllImport("ReportingServicesService.exe", CharSet = CharSet.Unicode, EntryPoint = "RsDumpSetErrorDetails")]
		public static extern void SetErrorDetails(string errorDetails);

		[DllImport("ReportingServicesService.exe", CharSet = CharSet.Unicode, EntryPoint = "RsDumpSetErrorSignature")]
		public static extern void SetErrorSignature(int signature);

		[DllImport("ReportingServicesService.exe", CharSet = CharSet.Unicode, EntryPoint = "RsDumpAddMemory")]
		public static extern void AddMemory(IntPtr pData, int size);
	}
}
