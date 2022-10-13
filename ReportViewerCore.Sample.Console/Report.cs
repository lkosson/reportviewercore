using Microsoft.Reporting.NETCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ReportViewerCore
{
	class Report
	{
		public static void Load(LocalReport report)
		{
			var items = new[] { new ReportItem { Description = "Widget 6000", Price = 104.99m, Qty = 1 }, new ReportItem { Description = "Gizmo MAX", Price = 1.41m, Qty = 25 } };
			var parameters = new[] { new ReportParameter("Title", "Invoice 4/2020\nabcdefghijklmnopqrstuvwxyz\nABCDEFGHIJKLMNOPQRSTUVWXYZ") };
			using var fs = new FileStream("Report.rdlc", FileMode.Open);
			report.LoadReportDefinition(fs);
			report.DataSources.Add(new ReportDataSource("Items", Enumerable.Range(1, 100).SelectMany(n => items)));
			report.SetParameters(parameters);
		}
	}
}
