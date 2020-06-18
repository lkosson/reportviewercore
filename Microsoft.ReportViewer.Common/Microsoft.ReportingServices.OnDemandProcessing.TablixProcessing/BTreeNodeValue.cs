using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal abstract class BTreeNodeValue : IStorable, IPersistable
	{
		protected abstract object Key
		{
			get;
		}

		public abstract int Size
		{
			get;
		}

		internal abstract void AddRow(IHierarchyObj ownerRef);

		internal abstract void Traverse(ProcessingStages operation, ITraversalContext traversalContext);

		internal int CompareTo(object keyValue, OnDemandProcessingContext odpContext)
		{
			return odpContext.ProcessingComparer.Compare(Key, keyValue);
		}

		public abstract void Serialize(IntermediateFormatWriter writer);

		public abstract void Deserialize(IntermediateFormatReader reader);

		public abstract void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems);

		public abstract ObjectType GetObjectType();
	}
}
