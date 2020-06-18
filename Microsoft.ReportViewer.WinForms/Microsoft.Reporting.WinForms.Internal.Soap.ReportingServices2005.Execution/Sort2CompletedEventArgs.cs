using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Reporting.WinForms.Internal.Soap.ReportingServices2005.Execution
{
	[GeneratedCode("wsdl", "2.0.50727.42")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class Sort2CompletedEventArgs : AsyncCompletedEventArgs
	{
		private object[] results;

		public int Result
		{
			get
			{
				RaiseExceptionIfNecessary();
				return (int)results[0];
			}
		}

		public string ReportItem
		{
			get
			{
				RaiseExceptionIfNecessary();
				return (string)results[1];
			}
		}

		public ExecutionInfo2 ExecutionInfo
		{
			get
			{
				RaiseExceptionIfNecessary();
				return (ExecutionInfo2)results[2];
			}
		}

		internal Sort2CompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState)
			: base(exception, cancelled, userState)
		{
			this.results = results;
		}
	}
}
