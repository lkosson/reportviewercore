using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.Reporting.WinForms
{
	internal class WinRSviewer : UserControl
	{
		private enum ViewType
		{
			Message,
			Report
		}

		public enum FocusMode
		{
			AlignToTop,
			Central,
			AvoidScrolling
		}

		private ReportViewer m_currentViewerControl;

		private TextBox m_errorBox;

		private ReportPanel m_reportPanel;

		private DrawablePage m_currentPage;

		public bool ShowContextMenu
		{
			get
			{
				return m_reportPanel.ShowContextMenu;
			}
			set
			{
				m_reportPanel.ShowContextMenu = value;
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
				m_currentViewerControl.StatusChanged += OnReportViewerStateChanged;
				if (m_currentViewerControl != null)
				{
					m_reportPanel.ViewerControl = value;
				}
			}
		}

		public override Color BackColor
		{
			set
			{
				base.BackColor = value;
				m_errorBox.BackColor = value;
			}
		}

		public Point ReportPanelAutoScrollPosition
		{
			get
			{
				return m_reportPanel.AutoScrollPosition;
			}
			set
			{
				m_reportPanel.SetAutoScrollLocation(value);
			}
		}

		public float ZoomCalculated => m_reportPanel.GetZoomRate();

		public event InternalPageNavigationEventHandler PageNavigation
		{
			add
			{
				m_reportPanel.PageNavigation += value;
			}
			remove
			{
				m_reportPanel.PageNavigation -= value;
			}
		}

		public event EventHandler Back
		{
			add
			{
				m_reportPanel.Back += value;
			}
			remove
			{
				m_reportPanel.Back -= value;
			}
		}

		public event EventHandler ReportRefresh
		{
			add
			{
				m_reportPanel.ReportRefresh += value;
			}
			remove
			{
				m_reportPanel.ReportRefresh -= value;
			}
		}

		public event EventHandler Print
		{
			add
			{
				m_reportPanel.Print += value;
			}
			remove
			{
				m_reportPanel.Print -= value;
			}
		}

		public event EventHandler PageSettings
		{
			add
			{
				m_reportPanel.PageSettings += value;
			}
			remove
			{
				m_reportPanel.PageSettings -= value;
			}
		}

		public event ExportEventHandler Export
		{
			add
			{
				m_reportPanel.Export += value;
			}
			remove
			{
				m_reportPanel.Export -= value;
			}
		}

		public event ZoomChangedEventHandler ZoomChange
		{
			add
			{
				m_reportPanel.ZoomChange += value;
			}
			remove
			{
				m_reportPanel.ZoomChange -= value;
			}
		}

		public WinRSviewer()
		{
			InitializeComponent();
			InitializeReportPanel();
			ApplyCustomResources();
		}

		protected override void OnGotFocus(EventArgs e)
		{
			if (m_reportPanel != null && m_reportPanel.Visible)
			{
				m_reportPanel.Focus();
			}
		}

		private void InitializeReportPanel()
		{
			m_reportPanel = new ReportPanel();
			m_reportPanel.Dock = DockStyle.Fill;
			m_reportPanel.ViewerControl = m_currentViewerControl;
			m_reportPanel.CurrentPage = m_currentPage;
			m_reportPanel.SetZoom();
			base.Controls.Add(m_reportPanel);
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Microsoft.Reporting.WinForms.WinRSviewer));
			m_errorBox = new System.Windows.Forms.TextBox();
			SuspendLayout();
			componentResourceManager.ApplyResources(m_errorBox, "errorBox");
			m_errorBox.BackColor = System.Drawing.SystemColors.Control;
			m_errorBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			m_errorBox.Cursor = System.Windows.Forms.Cursors.Arrow;
			m_errorBox.Name = "errorBox";
			m_errorBox.ReadOnly = true;
			componentResourceManager.ApplyResources(this, "$this");
			BackColor = System.Drawing.Color.White;
			base.CausesValidation = false;
			base.Controls.Add(m_errorBox);
			base.Name = "WinRSviewer";
			ResumeLayout(false);
			PerformLayout();
		}

		internal void ApplyCustomResources()
		{
			m_reportPanel.ApplyCustomResources();
		}

		public void ShowMessage(Exception e)
		{
			string text = "";
			for (Exception ex = e; ex != null; ex = ex.InnerException)
			{
				if (text.Length > 0)
				{
					text += "\r\n";
				}
				text += ex.Message;
			}
			ShowMessage(text, enabled: false);
		}

		public void ShowMessage(string text, bool enabled)
		{
			m_errorBox.Text = "\r\n\r\n" + text;
			m_errorBox.Enabled = enabled;
			m_errorBox.SelectionStart = 0;
			m_errorBox.SelectionLength = 0;
			ChangeView(ViewType.Message);
		}

		private void ChangeView(ViewType viewType)
		{
			m_errorBox.Visible = (viewType == ViewType.Message);
			m_reportPanel.Visible = (viewType == ViewType.Report);
		}

		public void SetFocusPoint(Point point, FocusMode focusMode)
		{
			m_reportPanel.SetFocusPoint(point, focusMode);
		}

		public void SetBookmarkFocusPoint(string bookMarkId)
		{
			m_reportPanel.SetBookmarkFocusPoint(bookMarkId);
		}

		public void SetFocusPointMm(PointF pointF, FocusMode focusMode)
		{
			m_reportPanel.SetFocusPointMm(pointF, focusMode);
		}

		public void SetActionFocusPoint(ActionType actionType, string actionId)
		{
			m_reportPanel.SetActionFocusPoint(actionType, actionId);
		}

		public void ScrollReport(bool positiveDirection)
		{
			m_reportPanel.ScrollReport(positiveDirection);
		}

		public void SetZoom()
		{
			m_reportPanel.SetZoom();
		}

		public void SetNewPage(DrawablePage currentPage)
		{
			m_currentPage = currentPage;
			m_reportPanel.CurrentPage = currentPage;
			ChangeView(ViewType.Report);
		}

		private void OnReportViewerStateChanged(object sender, EventArgs e)
		{
			Cursor = ((sender as ReportViewer).CurrentStatus.CanInteractWithReportPage ? Cursors.Default : Cursors.WaitCursor);
		}

		public void RenderToGraphics(Graphics g, bool testMode)
		{
			if (m_reportPanel != null)
			{
				m_reportPanel.RenderToGraphics(g, testMode);
			}
		}

		internal void SetToolStripRenderer(ToolStripRenderer renderer)
		{
			m_reportPanel.SetToolStripRenderer(renderer);
		}
	}
}
