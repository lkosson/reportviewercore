namespace Microsoft.Reporting.Gauge.WebForms
{
	internal interface ITraceContext
	{
		bool TraceEnabled
		{
			get;
		}

		void Write(string category, string message);
	}
}
