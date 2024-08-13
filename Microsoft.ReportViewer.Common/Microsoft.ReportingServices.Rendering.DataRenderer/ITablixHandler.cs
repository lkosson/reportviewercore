using Microsoft.ReportingServices.OnDemandReportRendering;

namespace Microsoft.ReportingServices.Rendering.DataRenderer;

internal interface ITablixHandler
{
	void OnTablixBegin(Tablix tablix, ref bool walkChildren, bool outputTablix);

	void OnTablixEnd(Tablix tablix);

	void OnTablixMemberBegin(TablixMember tablixMember, ref bool walkThisMember, bool outputThisMember);

	void OnTablixMemberEnd(TablixMember tablixMember);

	void OnTablixCellBegin(TablixCell cell, ref bool walkCell);

	void OnTablixCellEnd(TablixCell cell);
}
