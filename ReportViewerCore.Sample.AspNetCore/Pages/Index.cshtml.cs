using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Reporting.NETCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReportViewerCore.Sample.AspNetCore.Pages
{
	public class IndexModel : PageModel
	{
		[FromForm] public decimal WidgetPrice { get; set; }
		[FromForm] public decimal GizmoPrice { get; set; }

		public void OnGet()
		{
			WidgetPrice = 104.99m;
			GizmoPrice = 1.41m;
		}

		public IActionResult OnPostGetPDF() => PrepareReport("PDF", "pdf", "application/pdf");
		public IActionResult OnPostGetHTML() => PrepareReport("HTML5", "html", "text/html");
		public IActionResult OnPostGetDOCX() => PrepareReport("WORDOPENXML", "docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
		public IActionResult OnPostGetXLSX() => PrepareReport("EXCELOPENXML", "xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

		private IActionResult PrepareReport(string renderFormat, string extension, string mimeType)
		{
			using var report = new LocalReport();
			Report.Load(report, WidgetPrice, GizmoPrice);
			var pdf = report.Render(renderFormat);
			return File(pdf, mimeType, "report." + extension);
		}
	}
}
