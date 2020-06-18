namespace Microsoft.ReportingServices.Diagnostics.Internal
{
	public sealed class DataSource
	{
		public string Name
		{
			get;
			set;
		}

		public string DataSourceReference
		{
			get;
			set;
		}

		public string ConnectionString
		{
			get;
			set;
		}

		public string DataExtension
		{
			get;
			set;
		}

		internal DataSource()
		{
		}
	}
}
