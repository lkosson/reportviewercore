using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class ScalableDictionaryNodeReference : Reference<ScalableDictionaryNode>, IScalableDictionaryEntry, IStorable, IPersistable
	{
		internal ScalableDictionaryNodeReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.ScalableDictionaryNodeReference;
		}

		public ScalableDictionaryNode Value()
		{
			return ((IReference<ScalableDictionaryNode>)this).Value();
		}
	}
}
