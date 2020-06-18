using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal class LookupReferenceCreator : IReferenceCreator
	{
		private static LookupReferenceCreator m_instance;

		internal static LookupReferenceCreator Instance
		{
			get
			{
				if (m_instance == null)
				{
					m_instance = new LookupReferenceCreator();
				}
				return m_instance;
			}
		}

		private LookupReferenceCreator()
		{
		}

		public bool TryCreateReference(IStorable refTarget, out BaseReference newReference)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType objectType = refTarget.GetObjectType();
			if (TryMapObjectTypeToReferenceType(objectType, out Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType referenceType))
			{
				return TryCreateReference(referenceType, out newReference);
			}
			newReference = null;
			return false;
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
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupTableReference:
				reference = new SimpleReference<LookupTable>(referenceObjectType);
				return true;
			default:
				reference = null;
				return false;
			}
		}

		private bool TryMapObjectTypeToReferenceType(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType targetType, out Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType referenceType)
		{
			if (targetType == Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupTable)
			{
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupTableReference;
				return true;
			}
			referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None;
			return false;
		}
	}
}
