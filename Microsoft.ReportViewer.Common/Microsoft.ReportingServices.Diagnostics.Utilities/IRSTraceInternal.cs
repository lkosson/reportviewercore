using System.Diagnostics;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	internal interface IRSTraceInternal
	{
		string CurrentTraceFilePath
		{
			get;
		}

		bool BufferOutput
		{
			get;
			set;
		}

		bool IsTraceInitialized
		{
			get;
		}

		string TraceDirectory
		{
			get;
		}

		void ClearBuffer();

		void WriteBuffer();

		void Trace(string componentName, string message);

		void Trace(string componentName, string format, params object[] arg);

		void Trace(TraceLevel traceLevel, string componentName, string message);

		void Trace(TraceLevel traceLevel, string componentName, string format, params object[] arg);

		void TraceWithDetails(TraceLevel traceLevel, string componentName, string message, string details);

		void TraceException(TraceLevel traceLevel, string componentName, string message);

		void TraceWithNoEventLog(TraceLevel traceLevel, string componentName, string format, params object[] arg);

		void Fail(string componentName);

		void Fail(string componentName, string message);

		string GetDefaultTraceLevel();

		bool GetTraceLevel(string componentName, out TraceLevel componentTraceLevel);
	}
}
