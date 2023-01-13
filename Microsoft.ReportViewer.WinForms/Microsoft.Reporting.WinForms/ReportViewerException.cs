using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Reporting.WinForms
{
	[Serializable]
	public abstract class ReportViewerException : Exception
	{
		protected ReportViewerException(string message)
			: base(message)
		{
		}

		protected ReportViewerException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected ReportViewerException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
