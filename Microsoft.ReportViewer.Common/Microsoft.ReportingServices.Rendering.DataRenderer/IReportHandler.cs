using Microsoft.ReportingServices.OnDemandReportRendering;

namespace Microsoft.ReportingServices.Rendering.DataRenderer;

internal interface IReportHandler
{
	bool Done { get; }

	void OnReportBegin(Report report, ref bool walkChildren);

	void OnReportEnd(Report report);

	void OnSubReportBegin(SubReport subReport, ref bool walkSubreport);

	void OnSubReportEnd(SubReport subReport);

	void OnRectangleBegin(Rectangle rectangle, ref bool walkChildren);

	void OnRectangleEnd(Rectangle rectangle);

	void OnTextBoxBegin(TextBox textBox, bool output, ref bool render);

	void OnTextBoxEnd(TextBox textBox);

	void OnTopLevelDataRegionOrMap(ReportItem reportItem);
}
