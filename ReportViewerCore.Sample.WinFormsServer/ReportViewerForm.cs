using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ReportViewerCore
{
	class ReportViewerForm : Form
	{
		private readonly ReportViewer reportViewer;

		public ReportViewerForm()
		{
			Text = "Report viewer";
			WindowState = FormWindowState.Maximized;
			reportViewer = new ReportViewer();
			reportViewer.Dock = DockStyle.Fill;
			Controls.Add(reportViewer);
		}

		protected override void OnLoad(EventArgs e)
		{
			reportViewer.ServerReport.ReportServerUrl = new Uri("http://localhost/ReportServer");
			reportViewer.ServerReport.ReportPath = "Invoice";
			reportViewer.ServerReport.SetParameters(new[] { new ReportParameter("Title", "Invoice 4/2020") });
			reportViewer.RefreshReport();
			base.OnLoad(e);
		}
	}
}
