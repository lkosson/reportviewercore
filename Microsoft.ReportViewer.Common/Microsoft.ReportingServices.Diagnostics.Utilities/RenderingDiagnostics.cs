using System.Diagnostics;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	internal sealed class RenderingDiagnostics
	{
		public static bool Enabled => RSTrace.RenderingTracer.TraceVerbose;

		public static void Trace(RenderingArea renderingArea, TraceLevel traceLevel, string message)
		{
			if (Enabled)
			{
				RSTrace.RenderingTracer.Trace(TraceLevel.Verbose, message);
			}
		}

		public static void Trace(RenderingArea renderingArea, TraceLevel traceLevel, string format, params object[] arg)
		{
			if (Enabled)
			{
				RSTrace.RenderingTracer.Trace(TraceLevel.Verbose, format, arg);
			}
		}
	}
}
