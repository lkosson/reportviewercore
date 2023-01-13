using System;
using System.Collections.Specialized;
using System.Security.Permissions;

namespace Microsoft.ReportingServices.Interfaces
{
	[CLSCompliant(false)]
	public interface IAuthorizationExtension : IExtension
	{
		byte[] CreateSecurityDescriptor(AceCollection acl, SecurityItemType itemType, out string stringSecDesc);

		bool CheckAccess(string userName, IntPtr userToken, byte[] secDesc, CatalogOperation requiredOperation);

		bool CheckAccess(string userName, IntPtr userToken, byte[] secDesc, CatalogOperation[] requiredOperations);

		bool CheckAccess(string userName, IntPtr userToken, byte[] secDesc, ReportOperation requiredOperation);

		bool CheckAccess(string userName, IntPtr userToken, byte[] secDesc, FolderOperation requiredOperation);

		bool CheckAccess(string userName, IntPtr userToken, byte[] secDesc, FolderOperation[] requiredOperations);

		bool CheckAccess(string userName, IntPtr userToken, byte[] secDesc, ResourceOperation requiredOperation);

		bool CheckAccess(string userName, IntPtr userToken, byte[] secDesc, ResourceOperation[] requiredOperations);

		bool CheckAccess(string userName, IntPtr userToken, byte[] secDesc, DatasourceOperation requiredOperation);

		bool CheckAccess(string userName, IntPtr userToken, byte[] secDesc, ModelOperation requiredOperation);

		bool CheckAccess(string userName, IntPtr userToken, byte[] secDesc, ModelItemOperation requiredOperation);

		StringCollection GetPermissions(string userName, IntPtr userToken, SecurityItemType itemType, byte[] secDesc);
	}
}
