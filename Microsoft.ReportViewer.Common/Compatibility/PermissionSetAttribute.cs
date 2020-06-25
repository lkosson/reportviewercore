using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Text;

namespace Microsoft.Reporting
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
	public class PermissionSetAttribute : Attribute
	{
		public bool Unrestricted { get; set; }

		public PermissionSetAttribute(SecurityAction securityAction)
		{
		}
	}
}
