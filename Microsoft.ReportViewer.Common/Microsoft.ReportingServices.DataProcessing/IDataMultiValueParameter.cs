namespace Microsoft.ReportingServices.DataProcessing
{
	public interface IDataMultiValueParameter : IDataParameter
	{
		object[] Values
		{
			get;
			set;
		}
	}
}
