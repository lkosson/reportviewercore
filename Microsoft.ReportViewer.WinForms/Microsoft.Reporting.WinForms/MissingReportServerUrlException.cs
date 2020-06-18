using System;
using System.Runtime.Serialization;

namespace Microsoft.Reporting.WinForms
{
	[Serializable]
	public sealed class MissingReportServerUrlException : ReportViewerException
	{
		public MissingReportServerUrlException()
			: base(CommonStrings.MissingReportServerUrl)
		{
		}

		private MissingReportServerUrlException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
