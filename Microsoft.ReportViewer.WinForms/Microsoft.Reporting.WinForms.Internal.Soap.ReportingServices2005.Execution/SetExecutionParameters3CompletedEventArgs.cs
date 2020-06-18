using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Reporting.WinForms.Internal.Soap.ReportingServices2005.Execution
{
	[GeneratedCode("wsdl", "2.0.50727.42")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	public class SetExecutionParameters3CompletedEventArgs : AsyncCompletedEventArgs
	{
		private object[] results;

		public ExecutionInfo3 Result
		{
			get
			{
				RaiseExceptionIfNecessary();
				return (ExecutionInfo3)results[0];
			}
		}

		internal SetExecutionParameters3CompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState)
			: base(exception, cancelled, userState)
		{
			this.results = results;
		}
	}
}
