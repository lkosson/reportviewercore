using System;
using System.Runtime.Serialization;

namespace Microsoft.Reporting.WinForms
{
	[Serializable]
	public sealed class ClientRenderingException : ReportViewerException
	{
		internal ClientRenderingException(Exception renderingException)
			: base(CommonStrings.ClientRenderingErrors, renderingException)
		{
		}

		private ClientRenderingException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
