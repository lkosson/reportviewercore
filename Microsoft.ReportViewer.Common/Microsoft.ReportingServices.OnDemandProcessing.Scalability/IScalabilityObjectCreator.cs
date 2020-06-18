using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal interface IScalabilityObjectCreator
	{
		bool TryCreateObject(ObjectType objectType, out IPersistable newObject);

		List<Declaration> GetDeclarations();
	}
}
