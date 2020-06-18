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
	public class RenderStreamCompletedEventArgs : AsyncCompletedEventArgs
	{
		private object[] results;

		public byte[] Result
		{
			get
			{
				RaiseExceptionIfNecessary();
				return (byte[])results[0];
			}
		}

		public string Encoding
		{
			get
			{
				RaiseExceptionIfNecessary();
				return (string)results[1];
			}
		}

		public string MimeType
		{
			get
			{
				RaiseExceptionIfNecessary();
				return (string)results[2];
			}
		}

		internal RenderStreamCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState)
			: base(exception, cancelled, userState)
		{
			this.results = results;
		}
	}
}
