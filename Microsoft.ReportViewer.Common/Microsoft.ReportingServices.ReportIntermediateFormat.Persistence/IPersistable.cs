using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat.Persistence
{
	internal interface IPersistable
	{
		void Serialize(IntermediateFormatWriter writer);

		void Deserialize(IntermediateFormatReader reader);

		void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems);

		ObjectType GetObjectType();
	}
}
