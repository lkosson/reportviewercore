using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Text;

namespace Microsoft.ReportingServices
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
	public class StrongNameIdentityPermissionAttribute : Attribute
	{
		public string PublicKey { get; set; }

		public StrongNameIdentityPermissionAttribute(SecurityAction securityAction)
		{
		}
	}
}
