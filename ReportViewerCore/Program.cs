using Microsoft.Reporting.WinForms;
using System;
using System.IO;

namespace ReportViewerCore
{
	class Program
	{
		static void Main(string[] args)
		{
			var items = new[] { new ReportItem { Description = "Widget 6000", Price = 104.99m, Qty = 1 }, new ReportItem { Description = "Gizmo MAX", Price = 1.41m, Qty = 25 } };
			var parameters = new[] { new ReportParameter("Title", "Invoice 4/2020") };
			using var fs = new FileStream("Report.rdlc", FileMode.Open);
			var report = new LocalReport();
			report.LoadReportDefinition(fs);
			report.DataSources.Add(new ReportDataSource("Items", items));
			report.SetParameters(parameters);
			var pdf = report.Render("PDF");
			File.WriteAllBytes("report.pdf", pdf);
		}
	}
}
