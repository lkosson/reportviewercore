using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Microsoft.ReportingServices.Diagnostics
{
	internal static class NativeLoggingMethods
	{
		private const string ReportingServiceExe = "ReportingServicesService.exe";

		[DllImport("ReportingServicesService.exe", CharSet = CharSet.Unicode)]
		public static extern SafeNativeLoggingPointer CreateNativeLoggingObject(string AppDomain);

		[DllImport("ReportingServicesService.exe")]
		public static extern void ReleaseNativeLoggingObject(IntPtr nativeLoggingObject);

		[DllImport("ReportingServicesService.exe", CharSet = CharSet.Unicode)]
		public static extern void NativeLoggingTrace(SafeNativeLoggingPointer nativeLoggingObject, TraceLevel traceLevel, string componentName, string message, bool isAssert, bool isException, bool allowEventLogWrite);

		[DllImport("ReportingServicesService.exe", CharSet = CharSet.Unicode)]
		public static extern IntPtr GetNativeLoggingTraceDirectory(SafeNativeLoggingPointer nativeLoggingObject);

		[DllImport("ReportingServicesService.exe", CharSet = CharSet.Unicode)]
		public static extern IntPtr GetNativeLoggingTracePath(SafeNativeLoggingPointer nativeLoggingObject);

		[DllImport("ReportingServicesService.exe", CharSet = CharSet.Unicode)]
		public static extern bool GetNativeTraceLevel(SafeNativeLoggingPointer nativeLoggingObject, string component, out TraceLevel traceLevel);

		[DllImport("ReportingServicesService.exe", CharSet = CharSet.Unicode)]
		public static extern IntPtr GetDefaultTraceLevel(SafeNativeLoggingPointer nativeLoggingObject);
	}
}
