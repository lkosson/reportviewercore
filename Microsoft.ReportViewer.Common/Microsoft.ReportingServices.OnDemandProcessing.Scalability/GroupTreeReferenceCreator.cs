using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal class GroupTreeReferenceCreator : IReferenceCreator
	{
		private static GroupTreeReferenceCreator m_instance;

		internal static GroupTreeReferenceCreator Instance
		{
			get
			{
				if (m_instance == null)
				{
					m_instance = new GroupTreeReferenceCreator();
				}
				return m_instance;
			}
		}

		private GroupTreeReferenceCreator()
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
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegionInstanceReference:
				reference = new DataRegionInstanceReference();
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SubReportInstanceReference:
				reference = new SubReportInstanceReference();
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportInstanceReference:
				reference = new ReportInstanceReference();
				break;
			default:
				reference = null;
				return false;
			}
			return true;
		}

		private bool TryMapObjectTypeToReferenceType(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType targetType, out Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType referenceType)
		{
			switch (targetType)
			{
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegionInstance:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegionInstanceReference;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SubReportInstance:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SubReportInstanceReference;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportInstance:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportInstanceReference;
				break;
			default:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None;
				return false;
			}
			return true;
		}
	}
}
