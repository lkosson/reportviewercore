using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal interface IStorable : IPersistable
	{
		int Size
		{
			get;
		}
	}
}
