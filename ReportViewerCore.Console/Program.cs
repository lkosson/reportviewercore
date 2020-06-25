using System;
using System.IO;
using Microsoft.Reporting.WinForms;

namespace ReportViewerCore.Console
{
	class Program
	{
		static void Main(string[] args)
		{
			var report = new LocalReport();
			Report.Load(report);
			var pdf = report.Render("PDF");
			File.WriteAllBytes("report.pdf", pdf);
		}
	}
}
