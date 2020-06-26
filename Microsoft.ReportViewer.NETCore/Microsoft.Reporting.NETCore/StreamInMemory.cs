using System.IO;

namespace Microsoft.Reporting.NETCore
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
