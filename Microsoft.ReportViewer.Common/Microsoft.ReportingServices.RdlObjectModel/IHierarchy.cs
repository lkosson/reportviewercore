using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal interface IHierarchy
	{
		IEnumerable<IHierarchyMember> Members
		{
			get;
		}
	}
}
