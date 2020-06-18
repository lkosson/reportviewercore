using Microsoft.ReportingServices.ReportPublishing;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal interface IDomainScopeMemberCreator
	{
		void CreateDomainScopeMember(ReportHierarchyNode parentNode, Grouping grouping, AutomaticSubtotalContext context);
	}
}
