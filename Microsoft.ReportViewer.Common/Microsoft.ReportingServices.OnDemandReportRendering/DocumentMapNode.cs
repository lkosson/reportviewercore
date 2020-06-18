using Microsoft.ReportingServices.ReportProcessing;
using System;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	[Serializable]
	internal sealed class DocumentMapNode : OnDemandDocumentMapNode
	{
		internal DocumentMapNode(string aLabel, string aId, int aLevel)
			: base(aLabel, aId, aLevel)
		{
		}
	}
}
