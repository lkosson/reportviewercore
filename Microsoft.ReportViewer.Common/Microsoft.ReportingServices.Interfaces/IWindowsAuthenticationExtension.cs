using System.Security.Permissions;

namespace Microsoft.ReportingServices.Interfaces
{
	public interface IWindowsAuthenticationExtension : IAuthenticationExtension, IExtension
	{
		byte[] PrincipalNameToSid(string name);

		string SidToPrincipalName(byte[] sid);
	}
}
