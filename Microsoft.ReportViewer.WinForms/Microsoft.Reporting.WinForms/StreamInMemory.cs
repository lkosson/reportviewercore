using System.IO;

namespace Microsoft.Reporting.WinForms
{
	internal sealed class StreamInMemory : FileManagerStream
	{
		public StreamInMemory()
		{
			base.Stream = new MemoryStream();
		}

		public override void Delete()
		{
			base.Stream.Close();
		}
	}
}
