using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class CommonReferenceCreator : IReferenceCreator
	{
		private static CommonReferenceCreator m_instance;

		internal static CommonReferenceCreator Instance
		{
			get
			{
				if (m_instance == null)
				{
					m_instance = new CommonReferenceCreator();
				}
				return m_instance;
			}
		}

		private CommonReferenceCreator()
		{
		}

		public bool TryCreateReference(IStorable refTarget, out BaseReference newReference)
		{
			switch (refTarget.GetObjectType())
			{
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StorableArray:
				return TryCreateReference(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StorableArrayReference, out newReference);
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryNode:
				return TryCreateReference(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryNodeReference, out newReference);
			default:
				newReference = null;
				return false;
			}
		}

		public bool TryCreateReference(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType referenceObjectType, out BaseReference reference)
		{
			switch (referenceObjectType)
			{
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Null:
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None:
				Global.Tracer.Assert(condition: false, "Cannot create reference to Nothing or Null");
				reference = null;
				return false;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StorableArrayReference:
				reference = new SimpleReference<StorableArray>(referenceObjectType);
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryNodeReference:
				reference = new ScalableDictionaryNodeReference();
				break;
			default:
				reference = null;
				return false;
			}
			return true;
		}
	}
}
