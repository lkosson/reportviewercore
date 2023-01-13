using System.Security.Permissions;

namespace Microsoft.ReportingServices.Interfaces
{
	public interface ISubscriptionBaseUIUserControl : IExtension
	{
		Setting[] UserData
		{
			get;
			set;
		}

		string Description
		{
			get;
		}

		IDeliveryReportServerInformation ReportServerInformation
		{
			set;
		}

		bool IsPrivilegedUser
		{
			set;
		}

		bool Validate();
	}
}
