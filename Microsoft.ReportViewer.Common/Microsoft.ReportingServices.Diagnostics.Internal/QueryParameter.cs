namespace Microsoft.ReportingServices.Diagnostics.Internal
{
	public sealed class QueryParameter
	{
		public string Name
		{
			get;
			set;
		}

		public string Value
		{
			get;
			set;
		}

		public string TypeName
		{
			get;
			set;
		}

		internal QueryParameter()
		{
		}
	}
}
