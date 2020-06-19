using Microsoft.Reporting.WinForms;
using System;
using System.IO;

namespace ReportViewerCore
{
	class Program
	{
		static void Main(string[] args)
		{
			using var form = new ReportViewerForm();
			form.ShowDialog();

			//var report = new LocalReport();
			//Report.Load(report);
			//var pdf = report.Render("PDF");
			//File.WriteAllBytes("report.pdf", pdf);
		}
	}
}
