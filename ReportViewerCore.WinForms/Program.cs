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
		}
	}
}
