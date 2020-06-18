using Microsoft.ReportingServices.ReportProcessing;
using System.IO;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class IndexTablePage
	{
		internal byte[] Buffer;

		internal bool Dirty;

		internal int PageNumber;

		internal IndexTablePage PreviousPage;

		internal IndexTablePage NextPage;

		public IndexTablePage(int size)
		{
			Buffer = new byte[size];
			Dirty = false;
			PreviousPage = null;
			NextPage = null;
		}

		public void Read(Stream stream)
		{
			int num = stream.Read(Buffer, 0, Buffer.Length);
			if (num == 0)
			{
				for (int i = 0; i < Buffer.Length; i++)
				{
					Buffer[i] = 0;
				}
			}
			else if (num < Buffer.Length)
			{
				Global.Tracer.Assert(condition: false);
			}
			Dirty = false;
		}

		public void Write(Stream stream)
		{
			stream.Write(Buffer, 0, Buffer.Length);
			Dirty = false;
		}
	}
}
