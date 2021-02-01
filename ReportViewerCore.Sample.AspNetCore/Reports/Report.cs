using Microsoft.Reporting.NETCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace ReportViewerCore
{
	class Report
	{
		public static void Load(LocalReport report, decimal widgetPrice, decimal gizmoPrice)
		{
			var items = new[] { new ReportItem { Description = "Widget 6000", Price = widgetPrice, Qty = 1 }, new ReportItem { Description = "Gizmo MAX", Price = gizmoPrice, Qty = 25 } };
			var parameters = new[] { new ReportParameter("Title", "Invoice 4/2020") };
			using var rs = Assembly.GetExecutingAssembly().GetManifestResourceStream("ReportViewerCore.Sample.AspNetCore.Reports.Report.rdlc");
			report.LoadReportDefinition(rs);
			report.DataSources.Add(new ReportDataSource("Items", items));
			report.SetParameters(parameters);
		}
	}
}
