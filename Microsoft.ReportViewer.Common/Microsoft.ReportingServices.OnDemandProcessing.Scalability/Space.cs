namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal struct Space
	{
		internal long Offset;

		internal long Size;

		internal Space(long freeOffset, long freeSize)
		{
			Offset = freeOffset;
			Size = freeSize;
		}
	}
}
