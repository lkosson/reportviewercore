using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal interface IStaticReferenceable
	{
		int ID
		{
			get;
		}

		void SetID(int id);

		ObjectType GetObjectType();
	}
}
