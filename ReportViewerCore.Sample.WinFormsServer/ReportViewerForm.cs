using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Net;
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
			reportViewer = new();
			reportViewer.Dock = DockStyle.Fill;
			Controls.Add(reportViewer);
		}

		protected override void OnLoad(EventArgs e)
		{
			reportViewer.ProcessingMode = ProcessingMode.Remote;
			reportViewer.ServerReport.ReportServerCredentials.NetworkCredentials = new NetworkCredential("user", "password", "Domain");
			reportViewer.ServerReport.ReportServerUrl = new Uri("http://192.168.39.11/ReportServer");
			reportViewer.ServerReport.ReportPath = "/PHC/Facturação/Análise das Notas de Crédito por período";
			//reportViewer.ServerReport.SetParameters(new[] { new ReportParameter("Title", "Invoice 4/2020") });
			reportViewer.RefreshReport();
			base.OnLoad(e);
		}
	}
}
