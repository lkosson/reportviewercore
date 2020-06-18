namespace Microsoft.ReportingServices.Interfaces
{
	public interface IUserContext
	{
		string UserName
		{
			get;
		}

		object Token
		{
			get;
		}

		AuthenticationType AuthenticationType
		{
			get;
		}
	}
}
