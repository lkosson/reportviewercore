using Microsoft.Reporting.WinForms;
using System;
using System.IO;

namespace ReportViewerCore
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			using var form = new ReportViewerForm();
			form.ShowDialog();
		}
	}
}
