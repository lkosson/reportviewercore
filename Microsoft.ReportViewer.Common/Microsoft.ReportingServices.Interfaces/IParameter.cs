namespace Microsoft.ReportingServices.Interfaces
{
	public interface IParameter
	{
		string Name
		{
			get;
		}

		bool IsMultiValue
		{
			get;
		}

		object[] Values
		{
			get;
		}
	}
}
