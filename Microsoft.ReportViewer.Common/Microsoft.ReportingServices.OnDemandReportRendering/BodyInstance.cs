using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System.Globalization;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class BodyInstance : ReportElementInstance
	{
		public string UniqueName
		{
			get
			{
				if (m_reportElementDef.IsOldSnapshot)
				{
					ReportInstanceInfo instanceInfo = BodyDefinition.RenderReport.InstanceInfo;
					if (instanceInfo != null)
					{
						return instanceInfo.BodyUniqueName.ToString(CultureInfo.InvariantCulture);
					}
					return string.Empty;
				}
				Microsoft.ReportingServices.ReportIntermediateFormat.ReportSection sectionDef = BodyDefinition.SectionDef;
				return InstancePathItem.GenerateUniqueNameString(sectionDef.ID, sectionDef.InstancePath) + "xB";
			}
		}

		internal Body BodyDefinition => (Body)m_reportElementDef;

		internal BodyInstance(Body bodyDef)
			: base(bodyDef)
		{
		}
	}
}
