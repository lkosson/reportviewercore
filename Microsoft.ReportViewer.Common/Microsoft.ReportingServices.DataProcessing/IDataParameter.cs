namespace Microsoft.ReportingServices.DataProcessing
{
	public interface IDataParameter
	{
		string ParameterName
		{
			get;
			set;
		}

		object Value
		{
			get;
			set;
		}
	}
}
