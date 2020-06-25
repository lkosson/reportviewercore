using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.ReportingServices.Diagnostics
{
	internal sealed class MonitoredScope : IDisposable
	{
		private readonly Stack<KeyValuePair<string, DateTime>> stack = new Stack<KeyValuePair<string, DateTime>>();

		private const string IndentationTag = "--";

		private string indent = "--";

		private static bool? traceMonitoredScope = null;

		private static readonly MonitoredScope dummyInstance = new MonitoredScope();

		[ThreadStatic]
		private static MonitoredScope instance;

		private static bool TraceMonitoredScope
		{
			get
			{
				if (!traceMonitoredScope.HasValue)
				{
					bool value = false;
					traceMonitoredScope = value;
				}
				return traceMonitoredScope.Value;
			}
		}

		private MonitoredScope()
		{
		}

		internal static MonitoredScope New(string name)
		{
			if (!TraceMonitoredScope)
			{
				return dummyInstance;
			}
			MonitoredScope monitoredScope = instance;
			if (monitoredScope == null)
			{
				monitoredScope = (instance = new MonitoredScope());
			}
			monitoredScope.Start(name);
			return monitoredScope;
		}

		internal static MonitoredScope NewFormat(string format, object arg0)
		{
			if (!TraceMonitoredScope)
			{
				return dummyInstance;
			}
			return New(string.Format(CultureInfo.InvariantCulture, format, arg0));
		}

		internal static MonitoredScope NewFormat(string format, object arg0, object arg1)
		{
			if (!TraceMonitoredScope)
			{
				return dummyInstance;
			}
			return New(string.Format(CultureInfo.InvariantCulture, format, arg0, arg1));
		}

		internal static MonitoredScope NewFormat(string format, object arg0, object arg1, object arg2)
		{
			if (!TraceMonitoredScope)
			{
				return dummyInstance;
			}
			return New(string.Format(CultureInfo.InvariantCulture, format, arg0, arg1, arg2));
		}

		internal static MonitoredScope NewConcat(string arg0, object arg1)
		{
			if (!TraceMonitoredScope)
			{
				return dummyInstance;
			}
			if (arg1 != null)
			{
				return New(arg0 + arg1);
			}
			return New(arg0);
		}

		internal static void End(string name)
		{
			if (TraceMonitoredScope)
			{
				MonitoredScope monitoredScope = instance;
				if (!string.Equals(monitoredScope.stack.Peek().Key, name, StringComparison.InvariantCulture))
				{
					throw new Exception("MonitoredScope cannot be ended because the start and end scope names do not match!");
				}
				monitoredScope.Dispose();
			}
		}

		private void Start(string name)
		{
			if (TraceMonitoredScope)
			{
				stack.Push(new KeyValuePair<string, DateTime>(name, DateTime.UtcNow));
				string message = string.Format(CultureInfo.InvariantCulture, "< {0} {1}", indent, name);
				RSTrace.MonitoredScope.Trace(message);
				indent += "--";
			}
		}

		public void Dispose()
		{
			if (TraceMonitoredScope)
			{
				KeyValuePair<string, DateTime> keyValuePair = stack.Pop();
				indent = indent.Substring(0, indent.Length - 2);
				double totalMilliseconds = (DateTime.UtcNow - keyValuePair.Value).TotalMilliseconds;
				string message = string.Format(CultureInfo.InvariantCulture, "> {0} {1} - {2}", indent, keyValuePair.Key, totalMilliseconds);
				RSTrace.MonitoredScope.Trace(message);
			}
		}
	}
}
