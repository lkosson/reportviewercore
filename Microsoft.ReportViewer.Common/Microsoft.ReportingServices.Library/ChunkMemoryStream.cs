using System;
using System.IO;

namespace Microsoft.ReportingServices.Library
{
	[Serializable]
	internal sealed class ChunkMemoryStream : MemoryStream
	{
		[NonSerialized]
		public bool CanBeClosed;

		public override void Close()
		{
			if (CanBeClosed)
			{
				base.Close();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (CanBeClosed)
			{
				base.Dispose(disposing);
			}
		}
	}
}
