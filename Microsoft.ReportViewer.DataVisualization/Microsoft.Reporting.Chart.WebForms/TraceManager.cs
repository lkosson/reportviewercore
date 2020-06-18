using System;
using System.ComponentModel.Design;

namespace Microsoft.Reporting.Chart.WebForms
{
	internal class TraceManager : IServiceProvider
	{
		internal IServiceContainer serviceContainer;

		private ITraceContext traceContext;

		internal ITraceContext TraceContext
		{
			get
			{
				_ = traceContext;
				return traceContext;
			}
			set
			{
				traceContext = value;
			}
		}

		public bool TraceEnabled
		{
			get
			{
				if (TraceContext == null)
				{
					return false;
				}
				return TraceContext.TraceEnabled;
			}
		}

		private TraceManager()
		{
		}

		public TraceManager(IServiceContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException(SR.ExceptionInvalidServiceContainer);
			}
			serviceContainer = container;
		}

		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(TraceManager))
			{
				return this;
			}
			throw new ArgumentException(SR.ExceptionTraceManagerUnsupportedType(serviceType.ToString()));
		}

		public void Write(string category, string message)
		{
			if (TraceContext != null)
			{
				TraceContext.Write(category, message);
			}
		}
	}
}
