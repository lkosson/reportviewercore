using System.Security.Permissions;

namespace Microsoft.ReportingServices.Interfaces
{
	public interface IExtension
	{
		string LocalizedName
		{
			get;
		}

		void SetConfiguration(string configuration);
	}
}
