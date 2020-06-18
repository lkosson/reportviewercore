using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal class SubReportInstanceReference : ScopeInstanceReference, IReference<SubReportInstance>, IReference, IStorable, IPersistable
	{
		internal SubReportInstanceReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.SubReportInstanceReference;
		}

		public new SubReportInstance Value()
		{
			return (SubReportInstance)InternalValue();
		}
	}
}
