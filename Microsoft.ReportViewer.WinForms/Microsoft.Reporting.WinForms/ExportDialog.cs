using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Security;
using System.Text;
using System.Windows.Forms;

namespace Microsoft.Reporting.WinForms
{
	internal sealed class ExportDialog : Form
	{
		private Button cancelButton;

		private Label exportLabel;

		private Container components;

		private ReportViewer m_viewerControl;

		private RenderingExtension m_format;

		private string m_deviceInfo;

		private string m_fileName;

		internal ExportDialog(ReportViewer viewer, RenderingExtension extension, string deviceInfo, string fileName)
		{
			InitializeComponent();
			Text = LocalizationHelper.Current.ExportDialogTitle;
			cancelButton.Text = LocalizationHelper.Current.ExportDialogCancelButton;
			exportLabel.Text = LocalizationHelper.Current.ExportDialogStatusText;
			m_viewerControl = viewer;
			m_format = extension;
			m_deviceInfo = deviceInfo;
			if (m_deviceInfo == null)
			{
				m_deviceInfo = "";
			}
			m_fileName = fileName;
			exportLabel.MinimumSize = TextRenderer.MeasureText(exportLabel.Text, exportLabel.Font);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Microsoft.Reporting.WinForms.ExportDialog));
			exportLabel = new System.Windows.Forms.Label();
			cancelButton = new System.Windows.Forms.Button();
			SuspendLayout();
			componentResourceManager.ApplyResources(exportLabel, "exportLabel");
			exportLabel.Name = "exportLabel";
			componentResourceManager.ApplyResources(cancelButton, "cancelButton");
			cancelButton.Name = "cancelButton";
			cancelButton.Click += new System.EventHandler(CancelButton_Click);
			componentResourceManager.ApplyResources(this, "$this");
			base.Controls.Add(cancelButton);
			base.Controls.Add(exportLabel);
			Cursor = System.Windows.Forms.Cursors.Default;
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ExportDialog";
			base.ShowInTaskbar = false;
			ResumeLayout(false);
			PerformLayout();
		}

		protected override void OnLoad(EventArgs e)
		{
			cancelButton.Font = Font;
			try
			{
				m_viewerControl.CancelRendering(-1);
				AsyncReportOperation asyncReportOperation = new AsyncMainStreamRenderingOperation(m_viewerControl.Report, PageCountMode.Estimate, m_format.Name, m_deviceInfo, allowInternalRenderers: false, null);
				asyncReportOperation.Completed += OnExportComplete;
				m_viewerControl.BackgroundThread.BeginBackgroundOperation(asyncReportOperation);
			}
			catch (Exception ex)
			{
				ProcessOnLoadException(ex);
			}
			Point point = m_viewerControl.PointToScreen(Point.Empty);
			base.Left = point.X + Math.Max(0, (m_viewerControl.Width - base.Width) / 2);
			base.Top = point.Y + Math.Max(0, (m_viewerControl.Height - base.Height) / 2);
		}

		private void ProcessOnLoadException(Exception ex)
		{
			m_viewerControl.DisplayErrorMsgBox(ex, LocalizationHelper.Current.ExportErrorTitle);
			Close();
		}

		private void CancelButton_Click(object sender, EventArgs e)
		{
			m_viewerControl.CancelRendering(0);
		}

		private void OnExportCompleteUI(object sender, AsyncCompletedEventArgs args)
		{
			AsyncMainStreamRenderingOperation asyncMainStreamRenderingOperation = (AsyncMainStreamRenderingOperation)sender;
			if (args.Error != null)
			{
				if (!args.Cancelled)
				{
					m_viewerControl.DisplayErrorMsgBox(args.Error, LocalizationHelper.Current.ExportErrorTitle);
				}
			}
			else if (asyncMainStreamRenderingOperation.ReportBytes != null)
			{
				try
				{
					using (Stream stream = PromptFileName(asyncMainStreamRenderingOperation.FileNameExtension))
					{
						if (stream != null)
						{
							stream.Write(asyncMainStreamRenderingOperation.ReportBytes, 0, asyncMainStreamRenderingOperation.ReportBytes.Length);
							base.DialogResult = DialogResult.OK;
						}
					}
				}
				catch (Exception ex)
				{
					m_viewerControl.DisplayErrorMsgBox(ex, LocalizationHelper.Current.ExportErrorTitle);
				}
			}
			Close();
		}

		private void OnExportComplete(object sender, AsyncCompletedEventArgs args)
		{
			AsyncCompletedEventHandler method = OnExportCompleteUI;
			BeginInvoke(method, sender, args);
		}

		private Stream PromptFileName(string fileExtension)
		{
			_ = m_format.Name;
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			string str = "";
			if (fileExtension != null)
			{
				str = m_format.LocalizedName + " (*." + fileExtension + ")|*." + fileExtension + "|";
			}
			saveFileDialog.Filter = saveFileDialog.Filter + str + LocalizationHelper.Current.AllFilesFilter + " (*.*)|*.*";
			saveFileDialog.RestoreDirectory = true;
			bool flag = !string.IsNullOrEmpty(m_fileName);
			string text = m_fileName;
			if (!flag)
			{
				text = m_viewerControl.Report.DisplayNameForUse;
				text = ReplaceReservedCharacters(text);
				if (fileExtension != null)
				{
					text = text + "." + fileExtension;
				}
			}
			try
			{
				saveFileDialog.FileName = text;
			}
			catch (SecurityException)
			{
			}
			bool flag2 = flag;
			if (!flag && saveFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				flag2 = true;
			}
			if (flag2)
			{
				return saveFileDialog.OpenFile();
			}
			return null;
		}

		private string ReplaceReservedCharacters(string original)
		{
			StringBuilder stringBuilder = new StringBuilder(original.Length);
			char[] array = original.ToCharArray();
			foreach (char c in array)
			{
				bool flag = false;
				char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
				foreach (char c2 in invalidFileNameChars)
				{
					if (c == c2)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}
	}
}
