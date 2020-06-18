using System.IO;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal interface ISpaceManager
	{
		long StreamEnd
		{
			get;
			set;
		}

		void Free(long offset, long size);

		long AllocateSpace(long size);

		long Resize(long offset, long oldSize, long newSize);

		void Seek(long offset, SeekOrigin origin);

		void TraceStats();
	}
}
