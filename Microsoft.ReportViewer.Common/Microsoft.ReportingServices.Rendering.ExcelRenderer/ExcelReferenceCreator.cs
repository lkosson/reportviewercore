using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer
{
	internal class ExcelReferenceCreator : IReferenceCreator
	{
		private static ExcelReferenceCreator m_instance = new ExcelReferenceCreator();

		internal static ExcelReferenceCreator Instance => m_instance;

		private ExcelReferenceCreator()
		{
		}

		public bool TryCreateReference(IStorable refTarget, out BaseReference reference)
		{
			reference = null;
			return false;
		}

		public bool TryCreateReference(ObjectType referenceObjectType, out BaseReference reference)
		{
			reference = null;
			return false;
		}
	}
}
