namespace Microsoft.ReportingServices.Diagnostics.Internal
{
	public sealed class ExternalImageCategory
	{
		public string Count
		{
			get;
			set;
		}

		public string ByteCount
		{
			get;
			set;
		}

		public string ResourceFetchTime
		{
			get;
			set;
		}

		internal ExternalImageCategory()
		{
		}
	}
}
