using System;
using System.Collections;
using System.Security.Permissions;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ProcessingMessageList : ArrayList
	{
		public new ProcessingMessage this[int index] => (ProcessingMessage)base[index];

		public ProcessingMessageList()
		{
		}

		internal ProcessingMessageList(int capacity)
			: base(capacity)
		{
		}
	}
}
