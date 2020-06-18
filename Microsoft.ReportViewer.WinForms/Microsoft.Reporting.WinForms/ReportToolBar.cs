using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Microsoft.Reporting.WinForms
{
	internal class ReportToolBar : UserControl
	{
		public class ToolStripButtonOverride : ToolStripButton
		{
			public override bool CanSelect => Enabled;
		}

		private bool m_ignoreZoomEvents;

		private ReportViewer m_currentViewerControl;

		private ToolStripButton firstPage;

		private ToolStripButton previousPage;

		private ToolStripTextBox currentPage;

		private ToolStripLabel labelOf;

		private ToolStripLabel totalPages;

		private ToolStripButton nextPage;

		private ToolStripButton lastPage;

		private ToolStripSeparator toolStripSeparator2;

		private ToolStripButton back;

		private ToolStripButton stop;

		private ToolStripButton refresh;

		private ToolStripSeparator toolStripSeparator3;

		private ToolStripButton print;

		private ToolStripButton printPreview;

		private ToolStripSeparator separator4;

		private ToolStrip toolStrip1;

		private ToolStripComboBox zoom;

		private ToolStripTextBox textToFind;

		private ToolStripButton find;

		private ToolStripSeparator toolStripSeparator4;

		private ToolStripButton findNext;

		private ToolStripButton pageSetup;

		private ToolStripDropDownButton export;

		public override Size MinimumSize
		{
			get
			{
				return GetIdealSize();
			}
			set
			{
			}
		}

		public override Size MaximumSize
		{
			get
			{
				return GetIdealSize();
			}
			set
			{
			}
		}

		internal ReportViewer ViewerControl
		{
			get
			{
				return m_currentViewerControl;
			}
			set
			{
				if (m_currentViewerControl != null)
				{
					m_currentViewerControl.StatusChanged -= OnReportViewerStateChanged;
				}
				m_currentViewerControl = value;
				if (m_currentViewerControl != null)
				{
					m_currentViewerControl.StatusChanged += OnReportViewerStateChanged;
				}
			}
		}

		public event ZoomChangedEventHandler ZoomChange;

		public event PageNavigationEventHandler PageNavigation;

		public event ExportEventHandler Export;

		public event SearchEventHandler Search;

		public event EventHandler ReportRefresh;

		public event EventHandler Print;

		public event EventHandler Back;

		public event EventHandler PageSetup;

		public ReportToolBar()
		{
			InitializeComponent();
			using (Bitmap image = new Bitmap(1, 1))
			{
				using (Graphics graphics = Graphics.FromImage(image))
				{
					currentPage.Width = graphics.MeasureString("12345", currentPage.Font).ToSize().Width;
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				currentPage.TextBox.Visible = false;
				textToFind.TextBox.Visible = false;
			}
			base.Dispose(disposing);
		}

		protected override void OnLoad(EventArgs e)
		{
			if (!base.DesignMode)
			{
				ApplyLocalizedResources();
			}
			base.OnLoad(e);
		}

		internal void ApplyCustomResources()
		{
			firstPage.ToolTipText = LocalizationHelper.Current.FirstPageButtonToolTip;
			previousPage.ToolTipText = LocalizationHelper.Current.PreviousPageButtonToolTip;
			currentPage.ToolTipText = LocalizationHelper.Current.CurrentPageTextBoxToolTip;
			labelOf.Text = LocalizationHelper.Current.PageOf;
			totalPages.ToolTipText = LocalizationHelper.Current.TotalPagesToolTip;
			nextPage.ToolTipText = LocalizationHelper.Current.NextPageButtonToolTip;
			lastPage.ToolTipText = LocalizationHelper.Current.LastPageButtonToolTip;
			back.ToolTipText = LocalizationHelper.Current.BackButtonToolTip;
			stop.ToolTipText = LocalizationHelper.Current.StopButtonToolTip;
			refresh.ToolTipText = LocalizationHelper.Current.RefreshButtonToolTip;
			print.ToolTipText = LocalizationHelper.Current.PrintButtonToolTip;
			printPreview.ToolTipText = LocalizationHelper.Current.PrintLayoutButtonToolTip;
			pageSetup.ToolTipText = LocalizationHelper.Current.PageSetupButtonToolTip;
			export.ToolTipText = LocalizationHelper.Current.ExportButtonToolTip;
			zoom.ToolTipText = LocalizationHelper.Current.ZoomControlToolTip;
			textToFind.ToolTipText = LocalizationHelper.Current.SearchTextBoxToolTip;
			find.Text = LocalizationHelper.Current.FindButtonText;
			find.ToolTipText = LocalizationHelper.Current.FindButtonToolTip;
			findNext.Text = LocalizationHelper.Current.FindNextButtonText;
			findNext.ToolTipText = LocalizationHelper.Current.FindNextButtonToolTip;
		}

		private void ApplyLocalizedResources()
		{
			firstPage.AccessibleName = ReportPreviewStrings.FirstPageAccessibleName;
			previousPage.AccessibleName = ReportPreviewStrings.PreviousPageAccessibleName;
			currentPage.AccessibleName = ReportPreviewStrings.CurrentPageAccessibleName;
			nextPage.AccessibleName = ReportPreviewStrings.NextPageAccessibleName;
			lastPage.AccessibleName = ReportPreviewStrings.LastPageAccessibleName;
			back.AccessibleName = ReportPreviewStrings.BackAccessibleName;
			stop.AccessibleName = ReportPreviewStrings.StopAccessibleName;
			refresh.AccessibleName = ReportPreviewStrings.RefreshAccessibleName;
			print.AccessibleName = ReportPreviewStrings.PrintAccessibleName;
			printPreview.AccessibleName = ReportPreviewStrings.PrintPreviewAccessibleName;
			pageSetup.AccessibleName = ReportPreviewStrings.PageSetupAccessibleName;
			export.AccessibleDescription = ReportPreviewStrings.ExportAccessibleDescription;
			export.AccessibleName = ReportPreviewStrings.ExportAccessibleName;
			zoom.AccessibleName = ReportPreviewStrings.ZoomAccessibleName;
			textToFind.AccessibleName = ReportPreviewStrings.SearchTextBoxAccessibleName;
			find.AccessibleName = ReportPreviewStrings.FindAccessibleName;
			findNext.AccessibleName = ReportPreviewStrings.FindNextAccessibleName;
			base.AccessibleName = ReportPreviewStrings.ReportToolBarAccessibleName;
			ApplyCustomResources();
		}

		internal void SetToolStripRenderer(ToolStripRenderer renderer)
		{
			toolStrip1.Renderer = renderer;
		}

		private Size GetIdealSize()
		{
			Size result = base.Size;
			if (toolStrip1 != null && base.Parent != null)
			{
				result = new Size(base.Parent.Width, toolStrip1.PreferredSize.Height);
			}
			return result;
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Microsoft.Reporting.WinForms.ReportToolBar));
			firstPage = new Microsoft.Reporting.WinForms.ReportToolBar.ToolStripButtonOverride();
			previousPage = new Microsoft.Reporting.WinForms.ReportToolBar.ToolStripButtonOverride();
			currentPage = new System.Windows.Forms.ToolStripTextBox();
			labelOf = new System.Windows.Forms.ToolStripLabel();
			totalPages = new System.Windows.Forms.ToolStripLabel();
			nextPage = new Microsoft.Reporting.WinForms.ReportToolBar.ToolStripButtonOverride();
			lastPage = new Microsoft.Reporting.WinForms.ReportToolBar.ToolStripButtonOverride();
			toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			back = new Microsoft.Reporting.WinForms.ReportToolBar.ToolStripButtonOverride();
			stop = new Microsoft.Reporting.WinForms.ReportToolBar.ToolStripButtonOverride();
			refresh = new Microsoft.Reporting.WinForms.ReportToolBar.ToolStripButtonOverride();
			toolStrip1 = new System.Windows.Forms.ToolStrip();
			toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			print = new Microsoft.Reporting.WinForms.ReportToolBar.ToolStripButtonOverride();
			printPreview = new Microsoft.Reporting.WinForms.ReportToolBar.ToolStripButtonOverride();
			pageSetup = new Microsoft.Reporting.WinForms.ReportToolBar.ToolStripButtonOverride();
			export = new System.Windows.Forms.ToolStripDropDownButton();
			separator4 = new System.Windows.Forms.ToolStripSeparator();
			zoom = new System.Windows.Forms.ToolStripComboBox();
			textToFind = new System.Windows.Forms.ToolStripTextBox();
			find = new Microsoft.Reporting.WinForms.ReportToolBar.ToolStripButtonOverride();
			toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			findNext = new Microsoft.Reporting.WinForms.ReportToolBar.ToolStripButtonOverride();
			toolStrip1.SuspendLayout();
			SuspendLayout();
			firstPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			firstPage.Image = (System.Drawing.Image)resources.GetObject("firstPage.Image");
			firstPage.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			firstPage.Name = "firstPage";
			firstPage.RightToLeftAutoMirrorImage = true;
			firstPage.Size = new System.Drawing.Size(23, 22);
			firstPage.Click += new System.EventHandler(OnPageNavButtonClick);
			previousPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			previousPage.Image = (System.Drawing.Image)resources.GetObject("previousPage.Image");
			previousPage.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			previousPage.Name = "previousPage";
			previousPage.RightToLeftAutoMirrorImage = true;
			previousPage.Size = new System.Drawing.Size(23, 22);
			previousPage.Click += new System.EventHandler(OnPageNavButtonClick);
			currentPage.AcceptsReturn = true;
			currentPage.AcceptsTab = true;
			currentPage.MaxLength = 10;
			currentPage.Name = "currentPage";
			currentPage.Size = new System.Drawing.Size(40, 25);
			currentPage.WordWrap = false;
			currentPage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(CurrentPage_KeyPress);
			labelOf.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			labelOf.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			labelOf.Name = "labelOf";
			labelOf.Size = new System.Drawing.Size(0, 22);
			totalPages.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			totalPages.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			totalPages.Name = "totalPages";
			totalPages.Size = new System.Drawing.Size(0, 22);
			totalPages.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			nextPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			nextPage.Image = (System.Drawing.Image)resources.GetObject("nextPage.Image");
			nextPage.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			nextPage.Name = "nextPage";
			nextPage.RightToLeftAutoMirrorImage = true;
			nextPage.Size = new System.Drawing.Size(23, 22);
			nextPage.Click += new System.EventHandler(OnPageNavButtonClick);
			lastPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			lastPage.Image = (System.Drawing.Image)resources.GetObject("lastPage.Image");
			lastPage.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			lastPage.Name = "lastPage";
			lastPage.RightToLeftAutoMirrorImage = true;
			lastPage.Size = new System.Drawing.Size(23, 22);
			lastPage.Click += new System.EventHandler(OnPageNavButtonClick);
			toolStripSeparator2.Name = "toolStripSeparator2";
			toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
			back.Image = (System.Drawing.Image)resources.GetObject("back.Image");
			back.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			back.Name = "back";
			back.RightToLeftAutoMirrorImage = true;
			back.Size = new System.Drawing.Size(23, 22);
			back.Click += new System.EventHandler(OnBack);
			stop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			stop.Image = (System.Drawing.Image)resources.GetObject("stop.Image");
			stop.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			stop.Name = "stop";
			stop.Size = new System.Drawing.Size(23, 22);
			stop.Click += new System.EventHandler(OnStopClick);
			refresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			refresh.Image = (System.Drawing.Image)resources.GetObject("refresh.Image");
			refresh.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			refresh.Name = "refresh";
			refresh.Size = new System.Drawing.Size(23, 22);
			refresh.Click += new System.EventHandler(OnRefresh);
			toolStrip1.AccessibleName = "Toolstrip";
			toolStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
			toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[22]
			{
				firstPage,
				previousPage,
				currentPage,
				labelOf,
				totalPages,
				nextPage,
				lastPage,
				toolStripSeparator2,
				back,
				stop,
				refresh,
				toolStripSeparator3,
				print,
				printPreview,
				pageSetup,
				export,
				separator4,
				zoom,
				textToFind,
				find,
				toolStripSeparator4,
				findNext
			});
			toolStrip1.Location = new System.Drawing.Point(0, 0);
			toolStrip1.Name = "toolStrip1";
			toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			toolStrip1.Size = new System.Drawing.Size(714, 25);
			toolStrip1.TabIndex = 3;
			toolStrip1.TabStop = true;
			toolStripSeparator3.Name = "toolStripSeparator3";
			toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
			print.Image = (System.Drawing.Image)resources.GetObject("print.Image");
			print.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			print.Name = "print";
			print.Size = new System.Drawing.Size(23, 22);
			print.Click += new System.EventHandler(OnPrint);
			printPreview.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			printPreview.Image = (System.Drawing.Image)resources.GetObject("printPreview.Image");
			printPreview.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			printPreview.Name = "printPreview";
			printPreview.Size = new System.Drawing.Size(23, 22);
			printPreview.Click += new System.EventHandler(OnPrintPreviewClick);
			pageSetup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			pageSetup.Image = (System.Drawing.Image)resources.GetObject("pageSetup.Image");
			pageSetup.ImageTransparentColor = System.Drawing.Color.Magenta;
			pageSetup.Name = "pageSetup";
			pageSetup.Size = new System.Drawing.Size(23, 22);
			pageSetup.Click += new System.EventHandler(OnPageSetupClick);
			export.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			export.Image = (System.Drawing.Image)resources.GetObject("export.Image");
			export.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			export.Name = "export";
			export.Size = new System.Drawing.Size(29, 22);
			export.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(OnExport);
			separator4.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
			separator4.Name = "separator4";
			separator4.Size = new System.Drawing.Size(6, 25);
			zoom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			zoom.Margin = new System.Windows.Forms.Padding(7, 0, 1, 0);
			zoom.MaxDropDownItems = 9;
			zoom.Name = "zoom";
			zoom.Size = new System.Drawing.Size(110, 25);
			zoom.SelectedIndexChanged += new System.EventHandler(OnZoomChanged);
			textToFind.AcceptsReturn = true;
			textToFind.Margin = new System.Windows.Forms.Padding(10, 0, 1, 0);
			textToFind.Name = "textToFind";
			textToFind.Size = new System.Drawing.Size(75, 25);
			textToFind.KeyPress += new System.Windows.Forms.KeyPressEventHandler(textToFind_KeyPress);
			textToFind.TextChanged += new System.EventHandler(textToFind_TextChanged);
			find.Enabled = false;
			find.ForeColor = System.Drawing.Color.Blue;
			find.Margin = new System.Windows.Forms.Padding(3, 1, 1, 2);
			find.Name = "find";
			find.Size = new System.Drawing.Size(23, 22);
			find.Click += new System.EventHandler(find_Click);
			toolStripSeparator4.AutoSize = false;
			toolStripSeparator4.Name = "toolStripSeparator4";
			toolStripSeparator4.Size = new System.Drawing.Size(6, 20);
			findNext.Enabled = false;
			findNext.ForeColor = System.Drawing.Color.Blue;
			findNext.Margin = new System.Windows.Forms.Padding(2, 1, 0, 2);
			findNext.Name = "findNext";
			findNext.Size = new System.Drawing.Size(23, 22);
			findNext.Click += new System.EventHandler(findNext_Click);
			BackColor = System.Drawing.SystemColors.Control;
			base.Controls.Add(toolStrip1);
			base.Name = "ReportToolBar";
			base.Size = new System.Drawing.Size(714, 25);
			toolStrip1.ResumeLayout(false);
			toolStrip1.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		private void OnZoomChanged(object sender, EventArgs e)
		{
			if (!m_ignoreZoomEvents && this.ZoomChange != null)
			{
				ZoomItem zoomItem = (ZoomItem)zoom.SelectedItem;
				ZoomChangeEventArgs e2 = new ZoomChangeEventArgs(zoomItem.ZoomMode, zoomItem.ZoomPercent);
				this.ZoomChange(this, e2);
			}
		}

		private void OnPageNavigation(int newPage)
		{
			if (this.PageNavigation != null)
			{
				PageNavigationEventArgs e = new PageNavigationEventArgs(newPage);
				this.PageNavigation(this, e);
			}
		}

		private void OnExport(object sender, ToolStripItemClickedEventArgs e)
		{
			if (this.Export != null)
			{
				ReportExportEventArgs e2 = new ReportExportEventArgs((RenderingExtension)e.ClickedItem.Tag);
				this.Export(this, e2);
			}
		}

		private void OnSearch(object sender, SearchEventArgs se)
		{
			if (this.Search != null)
			{
				this.Search(this, se);
			}
		}

		private void OnRefresh(object sender, EventArgs e)
		{
			if (this.ReportRefresh != null)
			{
				this.ReportRefresh(this, EventArgs.Empty);
			}
		}

		private void OnPrint(object sender, EventArgs e)
		{
			if (this.Print != null)
			{
				toolStrip1.Capture = false;
				this.Print(this, EventArgs.Empty);
			}
		}

		private void OnBack(object sender, EventArgs e)
		{
			if (this.Back != null)
			{
				this.Back(this, e);
			}
		}

		private void OnPageSetupClick(object sender, EventArgs e)
		{
			if (this.PageSetup != null)
			{
				this.PageSetup(this, EventArgs.Empty);
			}
		}

		public void SetZoom()
		{
			try
			{
				m_ignoreZoomEvents = true;
				ZoomMenuHelper.Populate(zoom, ViewerControl.ZoomMode, ViewerControl.ZoomPercent);
			}
			finally
			{
				m_ignoreZoomEvents = false;
			}
		}

		private void PopulateExportList()
		{
			RenderingExtension[] extensions = ViewerControl.Report.ListRenderingExtensions();
			RenderingExtensionsHelper.Populate(export, null, extensions);
		}

		private void OnPageNavButtonClick(object sender, EventArgs e)
		{
			if (sender == firstPage)
			{
				OnPageNavigation(1);
			}
			else if (sender == previousPage)
			{
				OnPageNavigation(ViewerControl.CurrentPage - 1);
			}
			else if (sender == nextPage)
			{
				OnPageNavigation(ViewerControl.CurrentPage + 1);
			}
			else if (sender == lastPage)
			{
				PageCountMode pageCountMode;
				int newPage = ViewerControl.GetTotalPages(out pageCountMode);
				if (pageCountMode != 0)
				{
					OnPageNavigation(int.MaxValue);
				}
				else
				{
					OnPageNavigation(newPage);
				}
			}
		}

		private void find_Click(object sender, EventArgs e)
		{
			OnSearch(sender, new SearchEventArgs(textToFind.Text, ViewerControl.CurrentPage, isFindNext: false));
		}

		private void findNext_Click(object sender, EventArgs e)
		{
			OnSearch(sender, new SearchEventArgs(textToFind.Text, ViewerControl.CurrentPage, isFindNext: true));
		}

		private void textToFind_TextChanged(object sender, EventArgs e)
		{
			find.Enabled = !string.IsNullOrEmpty(textToFind.Text);
			findNext.Enabled = false;
		}

		private void textToFind_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r' && textToFind.Text.Length > 0)
			{
				if (!findNext.Enabled)
				{
					find_Click(sender, null);
				}
				else
				{
					findNext_Click(sender, null);
				}
			}
		}

		private void OnPrintPreviewClick(object sender, EventArgs e)
		{
			try
			{
				ViewerControl.SetDisplayMode((!printPreview.Checked) ? DisplayMode.PrintLayout : DisplayMode.Normal);
			}
			catch (Exception e2)
			{
				ViewerControl.UpdateUIState(e2);
			}
		}

		private void OnStopClick(object sender, EventArgs e)
		{
			try
			{
				ViewerControl.CancelRendering(0);
			}
			catch (Exception e2)
			{
				ViewerControl.UpdateUIState(e2);
			}
		}

		private void OnReportViewerStateChanged(object sender, EventArgs e)
		{
			ReportViewer reportViewer = (ReportViewer)sender;
			PageCountMode pageCountMode = PageCountMode.Actual;
			int num = 0;
			try
			{
				num = ViewerControl.GetTotalPages(out pageCountMode);
			}
			catch (Exception e2)
			{
				if (!ViewerControl.CurrentStatus.IsInFailedState)
				{
					ViewerControl.UpdateUIState(e2);
				}
			}
			if (num < 1)
			{
				totalPages.Text = "";
			}
			else
			{
				totalPages.Text = LocalizationHelper.Current.TotalPages(num, pageCountMode);
			}
			ReportViewerStatus currentStatus = reportViewer.CurrentStatus;
			if (currentStatus.CanNavigatePages)
			{
				currentPage.Text = ViewerControl.CurrentPage.ToString(CultureInfo.CurrentCulture);
			}
			bool flag = ViewerControl.CurrentPage <= 1;
			bool flag2 = ViewerControl.CurrentPage >= num && pageCountMode != PageCountMode.Estimate;
			firstPage.Enabled = (currentStatus.CanNavigatePages && !flag);
			previousPage.Enabled = (currentStatus.CanNavigatePages && !flag);
			currentPage.Enabled = currentStatus.CanNavigatePages;
			nextPage.Enabled = (currentStatus.CanNavigatePages && !flag2);
			lastPage.Enabled = (currentStatus.CanNavigatePages && !flag2);
			back.Enabled = currentStatus.CanNavigateBack;
			stop.Enabled = currentStatus.InCancelableOperation;
			refresh.Enabled = currentStatus.CanRefreshData;
			print.Enabled = currentStatus.CanPrint;
			printPreview.Enabled = currentStatus.CanChangeDisplayModes;
			printPreview.Checked = (reportViewer.DisplayMode == DisplayMode.PrintLayout);
			pageSetup.Enabled = print.Enabled;
			export.Enabled = currentStatus.CanExport;
			zoom.Enabled = currentStatus.CanChangeZoom;
			textToFind.Enabled = currentStatus.CanSearch;
			find.Enabled = (textToFind.Enabled && !string.IsNullOrEmpty(textToFind.Text));
			findNext.Enabled = currentStatus.CanContinueSearch;
			if (currentStatus.CanSearch && ViewerControl.SearchState != null)
			{
				textToFind.Text = ViewerControl.SearchState.Text;
			}
			bool showPageNavigationControls = reportViewer.ShowPageNavigationControls;
			firstPage.Visible = showPageNavigationControls;
			previousPage.Visible = showPageNavigationControls;
			currentPage.Visible = showPageNavigationControls;
			labelOf.Visible = showPageNavigationControls;
			totalPages.Visible = showPageNavigationControls;
			nextPage.Visible = showPageNavigationControls;
			lastPage.Visible = showPageNavigationControls;
			toolStripSeparator2.Visible = showPageNavigationControls;
			back.Visible = reportViewer.ShowBackButton;
			stop.Visible = reportViewer.ShowStopButton;
			refresh.Visible = reportViewer.ShowRefreshButton;
			toolStripSeparator3.Visible = (back.Visible || stop.Visible || refresh.Visible);
			print.Visible = reportViewer.ShowPrintButton;
			printPreview.Visible = reportViewer.ShowPrintButton;
			pageSetup.Visible = (print.Visible || printPreview.Visible);
			export.Visible = reportViewer.ShowExportButton;
			separator4.Visible = (print.Visible || printPreview.Visible || export.Visible);
			zoom.Visible = reportViewer.ShowZoomControl;
			bool showFindControls = reportViewer.ShowFindControls;
			toolStripSeparator4.Visible = showFindControls;
			find.Visible = showFindControls;
			findNext.Visible = showFindControls;
			textToFind.Visible = showFindControls;
			if (export.Visible && export.Enabled)
			{
				PopulateExportList();
			}
		}

		private void CurrentPage_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r' && currentPage.Text.Length > 0)
			{
				bool flag = false;
				if (int.TryParse(currentPage.Text, out int result) && ViewerControl.CanMoveToPage(result))
				{
					flag = true;
					OnPageNavigation(result);
				}
				if (!flag)
				{
					currentPage.Text = ViewerControl.CurrentPage.ToString(CultureInfo.CurrentCulture);
				}
				e.Handled = true;
			}
			else if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
			{
				e.Handled = true;
			}
		}
	}
}
