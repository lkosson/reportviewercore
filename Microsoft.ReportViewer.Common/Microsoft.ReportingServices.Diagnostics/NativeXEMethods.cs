using System.Runtime.InteropServices;

namespace Microsoft.ReportingServices.Diagnostics
{
	internal class NativeXEMethods
	{
		private const string ReportingServiceExe = "ReportingServicesService.exe";

		[DllImport("ReportingServicesService.exe", CharSet = CharSet.Unicode)]
		public static extern void AlterXESessions();
	}
}
