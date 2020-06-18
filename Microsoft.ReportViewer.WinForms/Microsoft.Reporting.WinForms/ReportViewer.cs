using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.Rendering.SPBProcessing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Microsoft.Reporting.WinForms
{
	[Designer("Microsoft.Reporting.WinForms.ReportViewerDesigner, Microsoft.ReportViewer.Design, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91", typeof(IDesigner))]
	[Docking(DockingBehavior.Ask)]
	[SRDescription("ReportViewerDescription")]
	public class ReportViewer : UserControl, IRenderable
	{
		private class AsyncReportOperationWrapper : AsyncReportOperation
		{
			private AsyncReportOperation m_operation;

			public AsyncReportOperationWrapper(AsyncReportOperation operation)
				: base(operation.Report)
			{
				m_operation = operation;
			}

			public void EndWrappedOperationExecution(Exception e)
			{
				m_operation.EndAsyncExecution(e);
			}

			protected override void PerformOperation()
			{
				m_operation.BeginAsyncExecution();
			}
		}

		public const int MaximumPageCount = int.MaxValue;

		private bool m_showToolbar = true;

		private bool m_showParameterPrompts = true;

		private bool m_showCredentialPrompts = true;

		private bool m_promptAreaCollapsed;

		private bool m_docMapCollapsed;

		private bool m_showProgress = true;

		private int m_toolbarVisibility = -1;

		private UIState m_lastUIState;

		private SearchState m_searchState;

		private bool m_userChangedSplitter;

		private PageCountMode m_pageCountMode = PageCountMode.Estimate;

		private ZoomMode m_zoomMode = ZoomMode.Percent;

		private int m_zoomPercent = 100;

		private ProcessingMode m_processingMode;

		private PrinterSettings m_printerSettings = new PrinterSettings();

		private DisplayMode m_viewMode;

		private Timer m_autoRefreshTimer = new Timer();

		private ProcessingThread m_processingThread = new ProcessingThread();

		private RVSplitContainer paramsSplitContainer;

		private AsyncWaitControl m_asyncWaitControl;

		private Timer m_asyncWaitControlTimer;

		private int m_waitControlDisplayAfter = 1000;

		private bool m_canRenderForWaitControl;

		private RSParams rsParams;

		private ReportToolBar reportToolBar;

		private RVSplitContainer dmSplitContainer;

		private RSDocMap rsDocMap;

		private WinRSviewer winRSviewer;

		private bool m_disposing;

		private ReportHierarchy m_reportHierarchy = new ReportHierarchy();

		private IReportViewerMessages m_reportViewerMessages;

		private Queue<MethodInvoker> m_pendingAsyncInvokes = new Queue<MethodInvoker>();

		private ToolStripRenderer m_toolStripRenderer = new ToolStripProfessionalRenderer();

		private ReportViewerStatus m_status;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRDescription("ServerReportDesc")]
		public ServerReport ServerReport => CurrentReport.ServerReport;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRDescription("LocalReportDesc")]
		public LocalReport LocalReport => CurrentReport.LocalReport;

		internal Report Report
		{
			get
			{
				if (ProcessingMode == ProcessingMode.Remote)
				{
					return ServerReport;
				}
				return LocalReport;
			}
		}

		[DefaultValue(BorderStyle.FixedSingle)]
		public new BorderStyle BorderStyle
		{
			get
			{
				return base.BorderStyle;
			}
			set
			{
				base.BorderStyle = value;
			}
		}

		[DefaultValue(typeof(Color), "White")]
		public override Color BackColor
		{
			get
			{
				return base.BackColor;
			}
			set
			{
				base.BackColor = value;
				winRSviewer.BackColor = value;
			}
		}

		public override System.Drawing.Image BackgroundImage
		{
			get
			{
				return base.BackgroundImage;
			}
			set
			{
				base.BackgroundImage = value;
				winRSviewer.BackgroundImage = value;
			}
		}

		public override ImageLayout BackgroundImageLayout
		{
			get
			{
				return base.BackgroundImageLayout;
			}
			set
			{
				base.BackgroundImageLayout = value;
				winRSviewer.BackgroundImageLayout = value;
			}
		}

		[Category("Appearance")]
		[DefaultValue(100)]
		[SRDescription("DocMapWidthDesc")]
		public int DocumentMapWidth
		{
			get
			{
				return dmSplitContainer.SplitterDistance;
			}
			set
			{
				dmSplitContainer.SplitterDistance = value;
			}
		}

		[Category("Appearance")]
		[DefaultValue(false)]
		[SRDescription("DocMapWidthFixedDesc")]
		public bool IsDocumentMapWidthFixed
		{
			get
			{
				return dmSplitContainer.FixedSize;
			}
			set
			{
				dmSplitContainer.FixedSize = value;
			}
		}

		[Category("Appearance")]
		[DefaultValue(false)]
		[SRDescription("DocMapCollapsedDesc")]
		public bool DocumentMapCollapsed
		{
			get
			{
				return m_docMapCollapsed;
			}
			set
			{
				if (m_docMapCollapsed != value)
				{
					m_docMapCollapsed = value;
					UpdateUIState(m_lastUIState);
				}
			}
		}

		[Category("Appearance")]
		[DefaultValue(false)]
		[SRDescription("PromptAreaCollapsedDesc")]
		public bool PromptAreaCollapsed
		{
			get
			{
				return m_promptAreaCollapsed;
			}
			set
			{
				m_promptAreaCollapsed = value;
				UpdateUIState(m_lastUIState);
			}
		}

		[Category("Appearance")]
		[DefaultValue(true)]
		[SRDescription("ShowParameterPromptsDesc")]
		public bool ShowParameterPrompts
		{
			get
			{
				return m_showParameterPrompts;
			}
			set
			{
				m_showParameterPrompts = value;
				UpdateUIState(m_lastUIState);
			}
		}

		[Category("Appearance")]
		[DefaultValue(true)]
		[SRDescription("ShowCredentialPromptsDesc")]
		public bool ShowCredentialPrompts
		{
			get
			{
				return m_showCredentialPrompts;
			}
			set
			{
				m_showCredentialPrompts = value;
				UpdateUIState(m_lastUIState);
			}
		}

		[Category("Appearance")]
		[DefaultValue(true)]
		[SRDescription("ShowToolBarDesc")]
		public bool ShowToolBar
		{
			get
			{
				return m_showToolbar;
			}
			set
			{
				m_showToolbar = value;
				reportToolBar.Visible = value;
			}
		}

		[Category("Appearance")]
		[DefaultValue(true)]
		[SRDescription("ShowProgressDesc")]
		public bool ShowProgress
		{
			get
			{
				return m_showProgress;
			}
			set
			{
				m_showProgress = value;
			}
		}

		[SRDescription("WaitControlDisplayAfterDesc")]
		[DefaultValue(1000)]
		public int WaitControlDisplayAfter
		{
			get
			{
				return m_waitControlDisplayAfter;
			}
			set
			{
				m_waitControlDisplayAfter = value;
			}
		}

		[Category("Appearance")]
		[DefaultValue(true)]
		[SRDescription("ShowContextMenuDesc")]
		public bool ShowContextMenu
		{
			get
			{
				return winRSviewer.ShowContextMenu;
			}
			set
			{
				winRSviewer.ShowContextMenu = value;
			}
		}

		[DefaultValue(true)]
		[SRDescription("ShowDocumentMapButtonDesc")]
		public bool ShowDocumentMapButton
		{
			get
			{
				return IsVisibilityFlagSet(ToolbarFlags.DocMap);
			}
			set
			{
				SetVisibilityFlag(ToolbarFlags.DocMap, value);
				UpdateUIState(m_lastUIState);
			}
		}

		[DefaultValue(true)]
		[SRDescription("ShowPromptAreaButtonDesc")]
		public bool ShowPromptAreaButton
		{
			get
			{
				return IsVisibilityFlagSet(ToolbarFlags.Params);
			}
			set
			{
				SetVisibilityFlag(ToolbarFlags.Params, value);
				UpdateUIState(m_lastUIState);
			}
		}

		[SRCategory("ToolBarCategoryDesc")]
		[DefaultValue(true)]
		[SRDescription("ShowPageNavigationDesc")]
		public bool ShowPageNavigationControls
		{
			get
			{
				return IsVisibilityFlagSet(ToolbarFlags.PageNav);
			}
			set
			{
				SetVisibilityFlag(ToolbarFlags.PageNav, value);
				UpdateUIState(m_lastUIState);
			}
		}

		[SRCategory("ToolBarCategoryDesc")]
		[DefaultValue(true)]
		[SRDescription("ShowBackButtonDesc")]
		public bool ShowBackButton
		{
			get
			{
				return IsVisibilityFlagSet(ToolbarFlags.Back);
			}
			set
			{
				SetVisibilityFlag(ToolbarFlags.Back, value);
				UpdateUIState(m_lastUIState);
			}
		}

		[SRCategory("ToolBarCategoryDesc")]
		[DefaultValue(true)]
		[SRDescription("ShowStopButtonDesc")]
		public bool ShowStopButton
		{
			get
			{
				return IsVisibilityFlagSet(ToolbarFlags.Stop);
			}
			set
			{
				SetVisibilityFlag(ToolbarFlags.Stop, value);
				UpdateUIState(m_lastUIState);
			}
		}

		[SRCategory("ToolBarCategoryDesc")]
		[DefaultValue(true)]
		[SRDescription("ShowRefreshButtonDesc")]
		public bool ShowRefreshButton
		{
			get
			{
				return IsVisibilityFlagSet(ToolbarFlags.Refresh);
			}
			set
			{
				SetVisibilityFlag(ToolbarFlags.Refresh, value);
				UpdateUIState(m_lastUIState);
			}
		}

		[SRCategory("ToolBarCategoryDesc")]
		[DefaultValue(true)]
		[SRDescription("ShowPrintButtonDesc")]
		public bool ShowPrintButton
		{
			get
			{
				return IsVisibilityFlagSet(ToolbarFlags.Print);
			}
			set
			{
				SetVisibilityFlag(ToolbarFlags.Print, value);
				UpdateUIState(m_lastUIState);
			}
		}

		[SRCategory("ToolBarCategoryDesc")]
		[DefaultValue(true)]
		[SRDescription("ShowExportButtonDesc")]
		public bool ShowExportButton
		{
			get
			{
				return IsVisibilityFlagSet(ToolbarFlags.Export);
			}
			set
			{
				SetVisibilityFlag(ToolbarFlags.Export, value);
				UpdateUIState(m_lastUIState);
			}
		}

		[SRCategory("ToolBarCategoryDesc")]
		[DefaultValue(true)]
		[SRDescription("ShowZoomButtonDesc")]
		public bool ShowZoomControl
		{
			get
			{
				return IsVisibilityFlagSet(ToolbarFlags.Zoom);
			}
			set
			{
				SetVisibilityFlag(ToolbarFlags.Zoom, value);
				UpdateUIState(m_lastUIState);
			}
		}

		[SRCategory("ToolBarCategoryDesc")]
		[DefaultValue(true)]
		[SRDescription("ShowFindButtonDesc")]
		public bool ShowFindControls
		{
			get
			{
				return IsVisibilityFlagSet(ToolbarFlags.Find);
			}
			set
			{
				SetVisibilityFlag(ToolbarFlags.Find, value);
				UpdateUIState(m_lastUIState);
			}
		}

		[DefaultValue(ProcessingMode.Local)]
		[SRDescription("ProcessingModeDesc")]
		public ProcessingMode ProcessingMode
		{
			get
			{
				return m_processingMode;
			}
			set
			{
				if (m_processingMode != value)
				{
					if (Report.IsDrillthroughReport)
					{
						throw new InvalidOperationException();
					}
					Clear();
					m_processingMode = value;
				}
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IReportViewerMessages Messages
		{
			get
			{
				return m_reportViewerMessages;
			}
			set
			{
				m_reportViewerMessages = value;
				(LocalizationHelper.Current as LocalizationHelper).SetWinformsViewerMessages(m_reportViewerMessages);
				reportToolBar.SuspendLayout();
				reportToolBar.ApplyCustomResources();
				reportToolBar.SetZoom();
				reportToolBar.ResumeLayout(performLayout: true);
				rsParams.SuspendLayout();
				rsParams.ApplyCustomResources();
				rsParams.ResumeLayout(performLayout: true);
				m_asyncWaitControl.SuspendLayout();
				m_asyncWaitControl.ApplyCustomResources();
				m_asyncWaitControl.ResumeLayout(performLayout: true);
				winRSviewer.SuspendLayout();
				winRSviewer.ApplyCustomResources();
				winRSviewer.SetZoom();
				winRSviewer.ResumeLayout(performLayout: true);
				ApplySplitterResources(allResources: false);
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int CurrentPage
		{
			get
			{
				return CurrentReport.CurrentPage;
			}
			set
			{
				try
				{
					SetCurrentPage(value, ActionType.None, null);
				}
				catch (ArgumentOutOfRangeException)
				{
					throw new ArgumentOutOfRangeException("value");
				}
			}
		}

		[Browsable(false)]
		public DisplayMode DisplayMode => m_viewMode;

		[DefaultValue(PageCountMode.Estimate)]
		[SRDescription("PageCountModeDesc")]
		public PageCountMode PageCountMode
		{
			get
			{
				return m_pageCountMode;
			}
			set
			{
				if (value != m_pageCountMode)
				{
					m_pageCountMode = value;
				}
			}
		}

		[Category("Appearance")]
		[DefaultValue(ZoomMode.Percent)]
		[SRDescription("ZoomModeDesc")]
		public ZoomMode ZoomMode
		{
			get
			{
				return m_zoomMode;
			}
			set
			{
				if (value != ZoomMode)
				{
					m_zoomMode = value;
					SetZoom();
				}
			}
		}

		[Category("Appearance")]
		[DefaultValue(100)]
		[SRDescription("ZoomPercentDesc")]
		public int ZoomPercent
		{
			get
			{
				return m_zoomPercent;
			}
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				if (value != ZoomPercent)
				{
					m_zoomPercent = value;
					if (ZoomMode == ZoomMode.Percent)
					{
						SetZoom();
					}
				}
			}
		}

		[Browsable(false)]
		public int ZoomCalculated => Math.Max((int)Math.Round(winRSviewer.ZoomCalculated * 100f), 1);

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ToolStripRenderer ToolStripRenderer
		{
			get
			{
				return m_toolStripRenderer;
			}
			set
			{
				m_toolStripRenderer = value;
				reportToolBar.SetToolStripRenderer(value);
				winRSviewer.SetToolStripRenderer(value);
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ReportViewerStatus CurrentStatus => m_status;

		[SRDescription("KeepSessionAliveDesc")]
		[DefaultValue(true)]
		public bool KeepSessionAlive
		{
			get
			{
				return m_reportHierarchy.KeepSessionAlive;
			}
			set
			{
				m_reportHierarchy.KeepSessionAlive = value;
			}
		}

		internal ProcessingThread BackgroundThread => m_processingThread;

		internal ReportInfo CurrentReport
		{
			get
			{
				if (m_reportHierarchy.Count == 0)
				{
					throw new ObjectDisposedException(GetType().Name);
				}
				return m_reportHierarchy.Peek();
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SearchState SearchState => m_searchState;

		private PageSettings PageSettings
		{
			get
			{
				PageSettings pageSettings = CurrentReport.PageSettings;
				if (pageSettings == null)
				{
					return ResetAndGetPageSettings();
				}
				return pageSettings;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public PrinterSettings PrinterSettings
		{
			get
			{
				if (!m_printerSettings.IsValid)
				{
					m_printerSettings = new PrinterSettings();
				}
				return m_printerSettings;
			}
			set
			{
				m_printerSettings = value;
				ReportPageSettings.UpdatePageSettingsForPrinter(PageSettings, m_printerSettings);
			}
		}

		private CreateAndRegisterStream PrintCreateAndRegisterStream => CreateStreamEMF;

		private bool ParametersAreaSupported => ProcessingMode == ProcessingMode.Remote;

		bool IRenderable.CanRender => m_canRenderForWaitControl;

		[SRDescription("ZoomEventDesc")]
		public event ZoomChangedEventHandler ZoomChange;

		[SRDescription("PageNavigationEventDesc")]
		public event PageNavigationEventHandler PageNavigation;

		[SRDescription("ExportEventDesc")]
		public event ExportEventHandler ReportExport;

		[SRDescription("RefreshEventDesc")]
		public event CancelEventHandler ReportRefresh;

		[SRDescription("PrintEventDesc")]
		public event ReportPrintEventHandler Print;

		[SRDescription("PrintingBegingEventDesc")]
		public event ReportPrintEventHandler PrintingBegin;

		[SRDescription("BackEventDesc")]
		public event BackEventHandler Back;

		[SRDescription("BookmarkEventDesc")]
		public event BookmarkNavigationEventHandler BookmarkNavigation;

		[SRDescription("ToggleEventDesc")]
		public event CancelEventHandler Toggle;

		[SRDescription("DrillthroughEventDesc")]
		public event DrillthroughEventHandler Drillthrough;

		[SRDescription("ViewReportEventDesc")]
		public event CancelEventHandler ViewButtonClick;

		[SRDescription("SortEventDesc")]
		public event SortEventHandler Sort;

		[SRDescription("HyperlinkEventDesc")]
		public event HyperlinkEventHandler Hyperlink;

		[SRDescription("DocMapEventDesc")]
		public event DocumentMapNavigationEventHandler DocumentMapNavigation;

		[SRDescription("RenderCompleteEventDesc")]
		public event RenderingCompleteEventHandler RenderingComplete;

		[SRDescription("RenderBeginEventDesc")]
		public event CancelEventHandler RenderingBegin;

		[SRDescription("SearchEventDesc")]
		public event SearchEventHandler Search;

		[SRDescription("ErrorEventDesc")]
		public event ReportErrorEventHandler ReportError;

		[SRDescription("StateChangedEventDesc")]
		public event EventHandler<EventArgs> StatusChanged;

		[SRDescription("SubmittingDataSourceCredentialsEventDesc")]
		public event ReportCredentialsEventHandler SubmittingDataSourceCredentials;

		[SRDescription("SubmittingParameterValuesEventDesc")]
		public event ReportParametersEventHandler SubmittingParameterValues;

		[SRDescription("PageSettingsChangedEventDesc")]
		public event EventHandler PageSettingsChanged;

		public ReportViewer()
		{
			InitializeComponent();
			reportToolBar.ViewerControl = this;
			rsParams.ViewerControl = this;
			winRSviewer.ViewerControl = this;
			m_autoRefreshTimer.Tick += OnRefresh;
			Reset();
			SetZoom();
		}

		private void OnZoomChanged(object sender, ZoomChangeEventArgs e)
		{
			try
			{
				if (this.ZoomChange != null)
				{
					this.ZoomChange(this, e);
				}
				if (!e.Cancel)
				{
					ZoomPercent = e.ZoomPercent;
					ZoomMode = e.ZoomMode;
				}
				else if (sender == reportToolBar)
				{
					reportToolBar.SetZoom();
				}
			}
			catch (Exception e2)
			{
				UpdateUIState(e2);
			}
		}

		private void OnPageNavigation(object sender, PageNavigationEventArgs e)
		{
			OnPageNavigation(sender, e, ActionType.None);
		}

		private void OnPageNavigation(object sender, PageNavigationEventArgs e, ActionType postRenderAction)
		{
			try
			{
				if (this.PageNavigation != null)
				{
					this.PageNavigation(this, e);
				}
				if (!e.Cancel)
				{
					SetCurrentPage(e.NewPage, postRenderAction, null);
				}
				else if (sender == reportToolBar)
				{
					UpdateUIState(m_lastUIState);
				}
			}
			catch (Exception e2)
			{
				UpdateUIState(e2);
			}
		}

		private bool OnPageNavigation(int targetPage)
		{
			if (this.PageNavigation == null)
			{
				return true;
			}
			PageNavigationEventArgs pageNavigationEventArgs = new PageNavigationEventArgs(targetPage);
			this.PageNavigation(this, pageNavigationEventArgs);
			return !pageNavigationEventArgs.Cancel;
		}

		private void OnExport(object sender, ReportExportEventArgs e)
		{
			try
			{
				if (this.ReportExport != null)
				{
					this.ReportExport(this, e);
				}
				if (!e.Cancel)
				{
					ExportDialog(e.Extension, e.DeviceInfo);
				}
			}
			catch (Exception e2)
			{
				UpdateUIState(e2);
			}
		}

		private void OnRefresh(object sender, EventArgs e)
		{
			try
			{
				CancelEventArgs cancelEventArgs = new CancelEventArgs();
				if (this.ReportRefresh != null)
				{
					this.ReportRefresh(this, cancelEventArgs);
				}
				if (!cancelEventArgs.Cancel)
				{
					int targetPage = 1;
					PostRenderArgs postRenderArgs = null;
					if (sender == m_autoRefreshTimer)
					{
						targetPage = CurrentPage;
						postRenderArgs = new PostRenderArgs(isDifferentReport: true, isPartialRendering: false, winRSviewer.ReportPanelAutoScrollPosition);
					}
					RefreshReport(targetPage, postRenderArgs);
				}
			}
			catch (Exception e2)
			{
				UpdateUIState(e2);
			}
		}

		private void OnPrint(object sender, EventArgs e)
		{
			try
			{
				ReportPrintEventArgs reportPrintEventArgs = new ReportPrintEventArgs(CreateDefaultPrintSettings());
				if (this.Print != null)
				{
					this.Print(this, reportPrintEventArgs);
				}
				if (!reportPrintEventArgs.Cancel)
				{
					PrintDialog(reportPrintEventArgs.PrinterSettings);
				}
			}
			catch (Exception e2)
			{
				UpdateUIState(e2);
			}
		}

		private bool OnPrintingBegin(object sender, PrinterSettings printerSettings)
		{
			try
			{
				ReportPrintEventArgs reportPrintEventArgs = new ReportPrintEventArgs(printerSettings);
				if (this.PrintingBegin != null)
				{
					this.PrintingBegin(this, reportPrintEventArgs);
					return !reportPrintEventArgs.Cancel;
				}
				return true;
			}
			catch (Exception e)
			{
				UpdateUIState(e);
				return false;
			}
		}

		private void OnBack(object sender, EventArgs e)
		{
			try
			{
				if (m_reportHierarchy.Count < 2)
				{
					throw new InvalidOperationException("Back call without drillthrough report");
				}
				bool flag = false;
				if (this.Back != null)
				{
					ReportInfo reportInfo = m_reportHierarchy.ToArray()[1];
					Report parentReport = (Report)((ProcessingMode != 0) ? ((object)reportInfo.ServerReport) : ((object)reportInfo.LocalReport));
					BackEventArgs backEventArgs = new BackEventArgs(parentReport);
					this.Back(this, backEventArgs);
					flag = backEventArgs.Cancel;
				}
				if (!flag)
				{
					PerformBack();
				}
			}
			catch (Exception e2)
			{
				UpdateUIState(e2);
			}
		}

		private void OnDocumentMapNavigation(object sender, DocumentMapNavigationEventArgs e)
		{
			try
			{
				if (this.DocumentMapNavigation != null)
				{
					this.DocumentMapNavigation(this, e);
				}
				if (!e.Cancel)
				{
					int num = Report.PerformDocumentMapNavigation(e.DocumentMapId);
					if (num != 0 && (num == CurrentPage || OnPageNavigation(num)))
					{
						SetCurrentPage(num, ActionType.DocumentMap, e.DocumentMapId);
					}
				}
			}
			catch (Exception e2)
			{
				UpdateUIState(e2);
			}
		}

		private void OnSearch(object sender, SearchEventArgs se)
		{
			try
			{
				if (this.Search != null)
				{
					this.Search(this, se);
				}
				if (se.Cancel)
				{
					return;
				}
				if (!se.IsFindNext)
				{
					if (Find(se.SearchString, se.StartPage) == 0)
					{
						MessageBoxWrappers.ShowMessageBox(this, LocalizationHelper.Current.TextNotFound, LocalizationHelper.Current.MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
					}
				}
				else if (FindNext() == 0)
				{
					MessageBoxWrappers.ShowMessageBox(this, LocalizationHelper.Current.NoMoreMatches, LocalizationHelper.Current.MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				}
			}
			catch (Exception e)
			{
				UpdateUIState(e);
			}
		}

		private bool OnError(Exception e)
		{
			if (this.ReportError != null)
			{
				ReportErrorEventArgs reportErrorEventArgs = new ReportErrorEventArgs(e);
				this.ReportError(this, reportErrorEventArgs);
				return reportErrorEventArgs.Handled;
			}
			return false;
		}

		private void OnStatusChanged(object sender, EventArgs e)
		{
			TriggerWaitControl();
			if (this.StatusChanged != null)
			{
				this.StatusChanged(sender, e);
			}
		}

		private void TriggerWaitControl()
		{
			if (CurrentStatus.CanInteractWithReportPage == ReportViewerStatus.DoesStateAllowInteractWithReportPage(m_lastUIState))
			{
				return;
			}
			if (!CurrentStatus.CanInteractWithReportPage)
			{
				if (m_showProgress)
				{
					if (m_waitControlDisplayAfter > 0)
					{
						m_asyncWaitControlTimer.Interval = m_waitControlDisplayAfter;
						m_asyncWaitControlTimer.Start();
					}
					else
					{
						SetWaitControlVisibility(visible: true);
					}
				}
			}
			else
			{
				m_asyncWaitControlTimer.Stop();
				SetWaitControlVisibility(visible: false);
			}
		}

		private void OnWaitPanelTimerTick(object sender, EventArgs e)
		{
			m_asyncWaitControlTimer.Stop();
			SetWaitControlVisibility(m_showProgress);
		}

		private void SetWaitControlVisibility(bool visible)
		{
			dmSplitContainer.SuspendLayout();
			rsDocMap.Enabled = !visible;
			rsDocMap.BackColor = (visible ? SystemColors.Control : SystemColors.Window);
			if (visible)
			{
				m_asyncWaitControl.Font = new Font(Font.FontFamily, Font.Size * 1.75f, FontStyle.Bold | Font.Style);
				m_asyncWaitControl.BringToFront();
			}
			m_asyncWaitControl.Visible = visible;
			dmSplitContainer.ResumeLayout(performLayout: true);
		}

		private void OnSubmittingDataSourceCredentials(object sender, ReportCredentialsEventArgs credentialArgs)
		{
			if (this.SubmittingDataSourceCredentials != null)
			{
				this.SubmittingDataSourceCredentials(this, credentialArgs);
			}
		}

		private void OnSubmittingParameterValues(object sender, ReportParametersEventArgs parameterArgs)
		{
			if (this.SubmittingParameterValues != null)
			{
				this.SubmittingParameterValues(this, parameterArgs);
			}
		}

		private bool IsVisibilityFlagSet(ToolbarFlags flag)
		{
			return (m_toolbarVisibility & (int)flag) != 0;
		}

		private void SetVisibilityFlag(ToolbarFlags flag, bool shouldSet)
		{
			if (shouldSet)
			{
				m_toolbarVisibility |= (int)flag;
			}
			else
			{
				m_toolbarVisibility &= (int)(~flag);
			}
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Microsoft.Reporting.WinForms.ReportViewer));
			paramsSplitContainer = new Microsoft.Reporting.WinForms.RVSplitContainer();
			rsParams = new Microsoft.Reporting.WinForms.RSParams();
			dmSplitContainer = new Microsoft.Reporting.WinForms.RVSplitContainer();
			rsDocMap = new Microsoft.Reporting.WinForms.RSDocMap();
			winRSviewer = new Microsoft.Reporting.WinForms.WinRSviewer();
			reportToolBar = new Microsoft.Reporting.WinForms.ReportToolBar();
			paramsSplitContainer.Panel1.SuspendLayout();
			paramsSplitContainer.Panel2.SuspendLayout();
			paramsSplitContainer.SuspendLayout();
			dmSplitContainer.Panel1.SuspendLayout();
			dmSplitContainer.Panel2.SuspendLayout();
			dmSplitContainer.SuspendLayout();
			SuspendLayout();
			componentResourceManager.ApplyResources(paramsSplitContainer, "paramsSplitContainer");
			paramsSplitContainer.BackColor = System.Drawing.SystemColors.ScrollBar;
			paramsSplitContainer.Name = "paramsSplitContainer";
			componentResourceManager.ApplyResources(paramsSplitContainer.Panel1, "paramsSplitContainer.Panel1");
			paramsSplitContainer.Panel1.Controls.Add(rsParams);
			paramsSplitContainer.Panel1Visible = false;
			componentResourceManager.ApplyResources(paramsSplitContainer.Panel2, "paramsSplitContainer.Panel2");
			paramsSplitContainer.Panel2.Controls.Add(dmSplitContainer);
			paramsSplitContainer.Panel2.Controls.Add(reportToolBar);
			paramsSplitContainer.SplitterMoving += new System.EventHandler(OnParamsSplitterMoving);
			paramsSplitContainer.CollapsedChanged += new System.EventHandler(OnPromptAreaCollapse);
			componentResourceManager.ApplyResources(rsParams, "rsParams");
			rsParams.BackColor = System.Drawing.SystemColors.Control;
			rsParams.Name = "rsParams";
			rsParams.ViewButtonClick += new System.EventHandler(OnViewButtonClick);
			rsParams.SubmitDataSourceCredentials += new Microsoft.Reporting.WinForms.ReportCredentialsEventHandler(OnSubmittingDataSourceCredentials);
			rsParams.SubmitParameters += new Microsoft.Reporting.WinForms.ReportParametersEventHandler(OnSubmittingParameterValues);
			rsParams.PreferredHeightChanged += new System.EventHandler(OnPreferredPromptAreaHeightChanged);
			componentResourceManager.ApplyResources(dmSplitContainer, "dmSplitContainer");
			dmSplitContainer.BackColor = System.Drawing.Color.LightGray;
			dmSplitContainer.Name = "dmSplitContainer";
			componentResourceManager.ApplyResources(dmSplitContainer.Panel1, "dmSplitContainer.Panel1");
			dmSplitContainer.Panel1.Controls.Add(rsDocMap);
			dmSplitContainer.Panel1Visible = false;
			dmSplitContainer.CollapsedChanged += new System.EventHandler(OnDocumentMapCollapse);
			componentResourceManager.ApplyResources(dmSplitContainer.Panel2, "dmSplitContainer.Panel2");
			dmSplitContainer.Panel2.Controls.Add(winRSviewer);
			componentResourceManager.ApplyResources(rsDocMap, "rsDocMap");
			rsDocMap.BackColor = System.Drawing.Color.White;
			rsDocMap.BorderStyle = System.Windows.Forms.BorderStyle.None;
			rsDocMap.HotTracking = true;
			rsDocMap.Name = "rsDocMap";
			rsDocMap.RightToLeftLayout = true;
			rsDocMap.DocumentMapNavigation += new Microsoft.Reporting.WinForms.DocumentMapNavigationEventHandler(OnDocumentMapNavigation);
			componentResourceManager.ApplyResources(winRSviewer, "winRSviewer");
			winRSviewer.BackColor = System.Drawing.Color.White;
			winRSviewer.CausesValidation = false;
			winRSviewer.Name = "winRSviewer";
			winRSviewer.ShowContextMenu = true;
			winRSviewer.PageNavigation += new Microsoft.Reporting.WinForms.InternalPageNavigationEventHandler(OnPageNavigation);
			winRSviewer.ZoomChange += new Microsoft.Reporting.WinForms.ZoomChangedEventHandler(OnZoomChanged);
			winRSviewer.ReportRefresh += new System.EventHandler(OnRefresh);
			winRSviewer.PageSettings += new System.EventHandler(OnPageSetup);
			winRSviewer.Print += new System.EventHandler(OnPrint);
			winRSviewer.Export += new Microsoft.Reporting.WinForms.ExportEventHandler(OnExport);
			winRSviewer.Back += new System.EventHandler(OnBack);
			componentResourceManager.ApplyResources(reportToolBar, "reportToolBar");
			reportToolBar.BackColor = System.Drawing.SystemColors.Control;
			reportToolBar.Name = "reportToolBar";
			reportToolBar.ZoomChange += new Microsoft.Reporting.WinForms.ZoomChangedEventHandler(OnZoomChanged);
			reportToolBar.ReportRefresh += new System.EventHandler(OnRefresh);
			reportToolBar.PageSetup += new System.EventHandler(OnPageSetup);
			reportToolBar.Print += new System.EventHandler(OnPrint);
			reportToolBar.Export += new Microsoft.Reporting.WinForms.ExportEventHandler(OnExport);
			reportToolBar.Search += new Microsoft.Reporting.WinForms.SearchEventHandler(OnSearch);
			reportToolBar.Back += new System.EventHandler(OnBack);
			reportToolBar.PageNavigation += new Microsoft.Reporting.WinForms.PageNavigationEventHandler(OnPageNavigation);
			BackColor = System.Drawing.Color.White;
			BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			base.Controls.Add(paramsSplitContainer);
			base.Name = "ReportViewer";
			componentResourceManager.ApplyResources(this, "$this");
			m_asyncWaitControl = new Microsoft.Reporting.WinForms.AsyncWaitControl(this);
			m_asyncWaitControl.Dock = System.Windows.Forms.DockStyle.Fill;
			m_asyncWaitControl.Visible = false;
			m_asyncWaitControlTimer = new System.Windows.Forms.Timer();
			m_asyncWaitControlTimer.Tick += new System.EventHandler(OnWaitPanelTimerTick);
			dmSplitContainer.Panel2.Controls.Add(m_asyncWaitControl);
			paramsSplitContainer.Panel1.ResumeLayout(false);
			paramsSplitContainer.Panel2.ResumeLayout(false);
			paramsSplitContainer.ResumeLayout(false);
			dmSplitContainer.Panel1.ResumeLayout(false);
			dmSplitContainer.Panel2.ResumeLayout(false);
			dmSplitContainer.ResumeLayout(false);
			ResumeLayout(false);
		}

		public int GetTotalPages()
		{
			PageCountMode pageCountMode;
			return GetTotalPages(out pageCountMode);
		}

		public int GetTotalPages(out PageCountMode pageCountMode)
		{
			if (m_viewMode == DisplayMode.PrintLayout)
			{
				pageCountMode = PageCountMode.Actual;
				if (CurrentReport.FileManager.Status == FileManagerStatus.InProgress)
				{
					return Math.Max(CurrentReport.FileManager.Count - 1, 0);
				}
				return CurrentReport.FileManager.Count;
			}
			return Report.GetTotalPages(out pageCountMode);
		}

		private void SetCurrentPage(int page, ActionType postActionType, string actionID)
		{
			if (page < 1)
			{
				throw new ArgumentOutOfRangeException("page");
			}
			CurrentReport.CurrentPage = page;
			PostRenderArgs postRenderArgs = new PostRenderArgs(postActionType, actionID);
			if (m_viewMode == DisplayMode.PrintLayout)
			{
				SetViewForCurrentPage(m_lastUIState, postRenderArgs);
			}
			else
			{
				RenderForPreview(postRenderArgs, invalidateCache: false);
			}
		}

		private void SetViewForCurrentPage(UIState state, PostRenderArgs args)
		{
			if (m_viewMode == DisplayMode.PrintLayout)
			{
				winRSviewer.SetNewPage(new MetaFilePage(CurrentReport.FileManager.Get(CurrentPage), PageSettings));
			}
			else
			{
				if (args.PostActionType == ActionType.Sort || args.PostActionType == ActionType.Toggle)
				{
					args.PreActionScrollPosition = winRSviewer.ReportPanelAutoScrollPosition;
				}
				winRSviewer.SetNewPage(new GdiPage(CurrentReport.GdiRenderer));
				if (args.IsDifferentReport)
				{
					rsDocMap.PopulateTree(Report.GetDocumentMap());
				}
				rsDocMap.UpdateTreeForPage(CurrentReport.GdiRenderer.Report.Labels);
			}
			PerformPostRenderAction(args);
			UpdateUIState(state);
		}

		internal bool CanMoveToPage(int page)
		{
			PageCountMode pageCountMode;
			int totalPages = GetTotalPages(out pageCountMode);
			if (pageCountMode != 0)
			{
				return page >= 1;
			}
			if (page <= totalPages)
			{
				return page >= 1;
			}
			return false;
		}

		private void CancelAllRenderingRequests()
		{
			CancelRendering(-1);
			CancelAutoRefreshTimer();
			ProcessAsyncInvokes();
		}

		public bool CancelRendering(int millisecondsTimeout)
		{
			return BackgroundThread.Cancel(millisecondsTimeout);
		}

		private void RenderDrillthrough(DrillthroughAction action)
		{
			if (!string.IsNullOrEmpty(action.ReportId))
			{
				string reportPath = null;
				Report report = Report.PerformDrillthrough(action.ReportId, out reportPath);
				LocalReport localReport = null;
				ServerReport serverReport = null;
				if (ProcessingMode == ProcessingMode.Local)
				{
					serverReport = new ServerReport(ServerReport);
					localReport = (LocalReport)report;
				}
				else
				{
					localReport = CreateLocalReport();
					serverReport = (ServerReport)report;
				}
				DrillthroughEventArgs drillthroughEventArgs = new DrillthroughEventArgs(reportPath, report);
				if (this.Drillthrough != null)
				{
					this.Drillthrough(this, drillthroughEventArgs);
				}
				if (!drillthroughEventArgs.Cancel)
				{
					PushReport(localReport, serverReport);
					RenderReportWithNewParameters(1, null);
				}
			}
		}

		public void PerformBack()
		{
			try
			{
				if (!Report.IsDrillthroughReport)
				{
					throw new InvalidOperationException(CommonStrings.NotInDrillthrough);
				}
				m_reportHierarchy.Pop();
				rsParams.EnsureParamsLoaded();
				if (m_viewMode == DisplayMode.Normal || CurrentReport.FileManager.Status == FileManagerStatus.Complete)
				{
					SetViewForCurrentPage(UIState.ProcessingSuccess, new PostRenderArgs(isDifferentReport: true, isPartialRendering: false));
				}
				else
				{
					RenderForPreview(new PostRenderArgs(isDifferentReport: true, isPartialRendering: false), invalidateCache: false);
				}
			}
			catch (ObjectDisposedException)
			{
				throw;
			}
			catch (Exception e)
			{
				UpdateUIState(e);
			}
		}

		private void PushReport(LocalReport localReport, ServerReport serverReport)
		{
			m_reportHierarchy.Push(new ReportInfo(localReport, serverReport));
		}

		public void Clear()
		{
			CancelAllRenderingRequests();
			CurrentReport.ClearGdiPage();
			CurrentReport.CurrentPage = 0;
			m_searchState = null;
			rsDocMap.Clear();
			CurrentReport.FileManager.Clean();
			UpdateUIState(UIState.NoReport);
		}

		public void Reset()
		{
			CancelAllRenderingRequests();
			m_reportHierarchy.Clear();
			PushReport(CreateLocalReport(), new ServerReport());
			rsParams.Clear();
			Clear();
		}

		private LocalReport CreateLocalReport()
		{
			return new LocalReport();
		}

		private void OnPrintPreviewPageAvailableUI(int pageNumber, bool isLastPage)
		{
			try
			{
				UIState uIState = (!isLastPage) ? UIState.ProcessingPartial : UIState.ProcessingSuccess;
				if (pageNumber == CurrentPage)
				{
					SetViewForCurrentPage(uIState, new PostRenderArgs(isDifferentReport: true, isPartialRendering: false));
				}
				else
				{
					UpdateUIState(uIState);
				}
			}
			catch (Exception e)
			{
				UpdateUIState(e);
			}
		}

		private void OnRenderingComplete(object sender, AsyncCompletedEventArgs args)
		{
			if (m_disposing)
			{
				return;
			}
			AsyncRenderingOperation asyncRenderingOperation = (AsyncRenderingOperation)sender;
			if (m_viewMode == DisplayMode.PrintLayout)
			{
				if (args.Error != null)
				{
					CurrentReport.FileManager.Status = FileManagerStatus.Aborted;
				}
				else
				{
					CurrentReport.FileManager.Status = FileManagerStatus.Complete;
				}
			}
			if (args.Error != null)
			{
				if (args.Cancelled)
				{
					winRSviewer.ShowMessage(LocalizationHelper.Current.ProcessingStopped, enabled: false);
					UpdateUIState(UIState.ProcessingFailure);
				}
				else
				{
					UpdateUIState(args.Error);
				}
				CurrentReport.ClearGdiPage();
			}
			else
			{
				try
				{
					if (m_viewMode == DisplayMode.Normal)
					{
						AsyncMainStreamRenderingOperation asyncMainStreamRenderingOperation = (AsyncMainStreamRenderingOperation)sender;
						int totalPages = Report.GetTotalPages();
						if (CurrentPage > totalPages)
						{
							CurrentReport.CurrentPage = totalPages;
						}
						if (asyncMainStreamRenderingOperation.ReportBytes == null || asyncMainStreamRenderingOperation.ReportBytes.LongLength == 0L)
						{
							PostRenderArgs postRenderArgs = asyncRenderingOperation.PostRenderArgs;
							postRenderArgs.PreActionScrollPosition = Point.Empty;
							RenderRPLPage(CurrentReport.CurrentPage, postRenderArgs);
							return;
						}
						CurrentReport.SetNewGdiPage(asyncMainStreamRenderingOperation.ReportBytes);
						SetViewForCurrentPage(UIState.ProcessingSuccess, asyncRenderingOperation.PostRenderArgs);
						StartAutoRefreshTimer(Report.AutoRefreshInterval);
					}
					else
					{
						int count = CurrentReport.FileManager.Count;
						if (CurrentPage > count)
						{
							CurrentReport.CurrentPage = count;
						}
						OnPrintPreviewPageAvailableUI(CurrentReport.FileManager.Count, isLastPage: true);
					}
				}
				catch (Exception e)
				{
					UpdateUIState(e);
				}
			}
			if (this.RenderingComplete != null)
			{
				RenderingCompleteEventArgs e2 = new RenderingCompleteEventArgs(asyncRenderingOperation.Warnings, args.Error);
				this.RenderingComplete(this, e2);
			}
		}

		private void PerformPostRenderAction(PostRenderArgs args)
		{
			if (args.PreActionScrollPosition != Point.Empty)
			{
				winRSviewer.ReportPanelAutoScrollPosition = args.PreActionScrollPosition;
			}
			if (args.ActionID != null)
			{
				if (args.PostActionType == ActionType.DocumentMap)
				{
					winRSviewer.SetFocusPoint(rsDocMap.GetFocusPoint(args.ActionID), WinRSviewer.FocusMode.AlignToTop);
				}
				else if (args.PostActionType == ActionType.BookmarkLink)
				{
					winRSviewer.SetBookmarkFocusPoint(args.ActionID);
				}
				else if (args.PostActionType == ActionType.Search)
				{
					winRSviewer.RenderToGraphics(winRSviewer.CreateGraphics(), testMode: false);
					CurrentReport.GdiRenderer.Search(args.ActionID);
					List<SearchMatch> searchMatches = CurrentReport.GdiRenderer.Context.SearchMatches;
					winRSviewer.SetFocusPointMm((searchMatches != null && searchMatches.Count > 0) ? searchMatches[0].Point : new PointF(0f, 0f), WinRSviewer.FocusMode.AvoidScrolling);
				}
				else
				{
					winRSviewer.SetActionFocusPoint(args.PostActionType, args.ActionID);
				}
			}
			else if (args.PostActionType == ActionType.ScrollToBottomOfPage)
			{
				winRSviewer.SetActionFocusPoint(args.PostActionType, args.ActionID);
			}
		}

		private void ExportDialogClosed(object sender, EventArgs e)
		{
			UpdateUIState(UIState.ProcessingSuccess);
		}

		internal void DisplayErrorMsgBox(Exception ex, string title)
		{
			string text = ex.Message;
			for (ex = ex.InnerException; ex != null; ex = ex.InnerException)
			{
				text = text + "\r\n" + ex.Message;
			}
			MessageBoxWrappers.ShowMessageBox(this, text, title, MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}

		private AsyncReportOperationWrapper WrapAsyncOperationForUIThreadNotification(AsyncReportOperation operation)
		{
			AsyncReportOperationWrapper asyncReportOperationWrapper = new AsyncReportOperationWrapper(operation);
			asyncReportOperationWrapper.Completed += OnBackgroundThreadCompleted;
			return asyncReportOperationWrapper;
		}

		private void OnBackgroundThreadCompleted(object sender, AsyncCompletedEventArgs e)
		{
			AsyncReportOperationWrapper operationWrapper = (AsyncReportOperationWrapper)sender;
			RegisterAsyncInvoke(delegate
			{
				operationWrapper.EndWrappedOperationExecution(e.Error);
			});
		}

		private void ProcessAsyncInvokes()
		{
			lock (m_pendingAsyncInvokes)
			{
				while (m_pendingAsyncInvokes.Count > 0)
				{
					m_pendingAsyncInvokes.Dequeue()();
				}
			}
		}

		private void RegisterAsyncInvoke(MethodInvoker method)
		{
			lock (m_pendingAsyncInvokes)
			{
				m_pendingAsyncInvokes.Enqueue(method);
				if (base.IsHandleCreated)
				{
					BeginInvoke(new MethodInvoker(ProcessAsyncInvokes));
				}
			}
		}

		private void OnRenderingCompletePrintOnly(object sender, AsyncCompletedEventArgs args)
		{
			if (((AsyncRenderingOperation)sender).PostRenderArgs.IsPartialRendering || args.Error != null)
			{
				CurrentReport.FileManager.Status = FileManagerStatus.Aborted;
			}
			else
			{
				CurrentReport.FileManager.Status = FileManagerStatus.Complete;
			}
		}

		private void OnAsyncLoadCompleted(object sender, AsyncCompletedEventArgs args)
		{
			if (args.Error != null)
			{
				UpdateUIState(args.Error);
				return;
			}
			try
			{
				RefreshReport();
			}
			catch (Exception e)
			{
				OnError(e);
			}
		}

		internal void LoadAndRefreshReportAsync(Stream reportDefinition)
		{
			Clear();
			AsyncLoadOperation asyncLoadOperation = new AsyncLoadOperation(Report, reportDefinition);
			asyncLoadOperation.Completed += OnAsyncLoadCompleted;
			AsyncReportOperationWrapper operation = WrapAsyncOperationForUIThreadNotification(asyncLoadOperation);
			UpdateUIState(UIState.LongRunningAction);
			BackgroundThread.BeginBackgroundOperation(operation);
		}

		public void RefreshReport()
		{
			RefreshReport(1, null);
		}

		private void RefreshReport(int targetPage, PostRenderArgs postRenderArgs)
		{
			try
			{
				Clear();
				Report.Refresh();
				RenderReportWithNewParameters(targetPage, postRenderArgs);
			}
			catch (ObjectDisposedException)
			{
				throw;
			}
			catch (Exception e)
			{
				UpdateUIState(e);
			}
		}

		public int Find(string searchString, int startPage)
		{
			int defaultEndPageForStartPage = GetDefaultEndPageForStartPage(startPage);
			m_searchState = null;
			return Find(searchString, startPage, defaultEndPageForStartPage);
		}

		private int Find(string searchString, int startPage, int endPage)
		{
			if (!ReportViewerStatus.DoesStateAllowSearch(this, m_lastUIState))
			{
				throw new InvalidOperationException();
			}
			if (CurrentReport.GdiRenderer != null)
			{
				CurrentReport.GdiRenderer.ClearSearchResults();
			}
			int num = (m_processingMode != 0) ? CurrentReport.ServerReport.PerformSearch(searchString, startPage, endPage) : CurrentReport.LocalReport.PerformSearch(searchString, startPage, endPage);
			if (num != 0)
			{
				if (m_searchState == null)
				{
					m_searchState = new SearchState(searchString, startPage);
				}
				if (num != CurrentPage || CurrentReport.GdiRenderer == null)
				{
					SearchState searchState = m_searchState;
					SetCurrentPage(num, ActionType.Search, searchString);
					m_searchState = searchState;
				}
				else
				{
					SetViewForCurrentPage(UIState.ProcessingSuccess, new PostRenderArgs(ActionType.Search, searchString, winRSviewer.ReportPanelAutoScrollPosition));
				}
			}
			return num;
		}

		private int GetDefaultEndPageForStartPage(int startPage)
		{
			if (startPage == 1)
			{
				PageCountMode pageCountMode;
				int totalPages = GetTotalPages(out pageCountMode);
				if (pageCountMode != 0)
				{
					return int.MaxValue;
				}
				return totalPages;
			}
			return startPage - 1;
		}

		public int FindNext()
		{
			if (CurrentReport.GdiRenderer == null || m_searchState == null)
			{
				throw new InvalidOperationException(ReportPreviewStrings.NotSearching);
			}
			if (CurrentReport.GdiRenderer.FindNext())
			{
				SetViewForCurrentPage(UIState.ProcessingSuccess, new PostRenderArgs(isDifferentReport: false, isPartialRendering: false, winRSviewer.ReportPanelAutoScrollPosition));
				winRSviewer.SetFocusPointMm(CurrentReport.GdiRenderer.Context.SearchMatches[CurrentReport.GdiRenderer.Context.SearchMatchIndex].Point, WinRSviewer.FocusMode.AvoidScrolling);
				return CurrentPage;
			}
			int defaultEndPageForStartPage = GetDefaultEndPageForStartPage(m_searchState.StartPage);
			int num = 0;
			if (CurrentPage != defaultEndPageForStartPage)
			{
				PageCountMode pageCountMode;
				int totalPages = GetTotalPages(out pageCountMode);
				num = Find(startPage: (CurrentPage == totalPages && pageCountMode == PageCountMode.Actual) ? 1 : (CurrentPage + 1), searchString: CurrentReport.GdiRenderer.Context.SearchText, endPage: defaultEndPageForStartPage);
			}
			if (num == 0)
			{
				m_searchState = null;
				UpdateUIState(m_lastUIState);
			}
			return num;
		}

		public void JumpToBookmark(string bookmarkId)
		{
			string uniqueName;
			int num = Report.PerformBookmarkNavigation(bookmarkId, out uniqueName);
			if (num > 0)
			{
				SetCurrentPage(num, ActionType.BookmarkLink, uniqueName);
			}
		}

		public void JumpToDocumentMapId(string documentMapId)
		{
			int num = Report.PerformDocumentMapNavigation(documentMapId);
			if (num > 0)
			{
				SetCurrentPage(num, ActionType.DocumentMap, documentMapId);
			}
		}

		private void RenderReportWithNewParameters(int pageNumber, PostRenderArgs postRenderArgs)
		{
			try
			{
				CurrentReport.CurrentPage = pageNumber;
				rsParams.EnsureParamsLoaded();
				if (Report.PrepareForRender())
				{
					if (postRenderArgs == null)
					{
						postRenderArgs = new PostRenderArgs(isDifferentReport: true, isPartialRendering: false);
					}
					RenderForPreview(postRenderArgs, invalidateCache: true);
					return;
				}
				if (!Report.IsReadyForConnection)
				{
					throw new MissingReportSourceException();
				}
				if (!ParametersAreaSupported || (!ShowPromptAreaButton && PromptAreaCollapsed))
				{
					rsParams.ValidateReportInputsSatisfied();
					if (ProcessingMode == ProcessingMode.Local)
					{
						foreach (string dataSourceName in LocalReport.GetDataSourceNames())
						{
							if (LocalReport.DataSources[dataSourceName] == null)
							{
								throw new MissingDataSourceException(dataSourceName);
							}
						}
					}
					throw new Exception(CommonStrings.ReportNotReadyException);
				}
				UpdateUIState(UIState.NoReport);
			}
			catch (Exception e)
			{
				UpdateUIState(e);
			}
		}

		public DialogResult PrintDialog()
		{
			return PrintDialog(CreateDefaultPrintSettings());
		}

		public DialogResult PrintDialog(PrinterSettings printerSettings)
		{
			if (!ReportViewerStatus.DoesStateAllowPrinting(m_lastUIState))
			{
				throw new InvalidOperationException();
			}
			DialogResult dialogResult = DialogResult.Cancel;
			using (PrintDialog printDialog = new PrintDialog())
			{
				printDialog.PrinterSettings = printerSettings;
				printDialog.AllowSelection = false;
				printDialog.AllowSomePages = true;
				printDialog.UseEXDialog = true;
				dialogResult = printDialog.ShowDialog(this);
				if (dialogResult == DialogResult.OK)
				{
					if (OnPrintingBegin(this, printerSettings))
					{
						string displayNameForUse = Report.DisplayNameForUse;
						if (CurrentReport.FileManager.Status == FileManagerStatus.Aborted || CurrentReport.FileManager.Status == FileManagerStatus.NotStarted)
						{
							bool flag = printDialog.PrinterSettings.PrintRange == PrintRange.AllPages;
							int startPage;
							int endPage;
							if (!flag)
							{
								startPage = 1;
								endPage = printDialog.PrinterSettings.ToPage;
							}
							else
							{
								startPage = 0;
								endPage = 0;
							}
							string deviceInfo = CreateEMFDeviceInfo(startPage, endPage);
							ProcessAsyncInvokes();
							BeginAsyncRender("IMAGE", allowInternalRenderers: true, deviceInfo, PageCountMode.Estimate, CreateStreamEMFPrintOnly, OnRenderingCompletePrintOnly, new PostRenderArgs(isDifferentReport: false, !flag), requireCompletionOnUIThread: false);
						}
						ReportPrintDocument reportPrintDocument = new ReportPrintDocument(CurrentReport.FileManager, (PageSettings)PageSettings.Clone());
						reportPrintDocument.DocumentName = displayNameForUse;
						reportPrintDocument.PrinterSettings = printDialog.PrinterSettings;
						reportPrintDocument.Print();
						return dialogResult;
					}
					return dialogResult;
				}
				return dialogResult;
			}
		}

		private PrinterSettings CreateDefaultPrintSettings()
		{
			PrinterSettings printerSettings = PrinterSettings;
			printerSettings.PrintRange = PrintRange.AllPages;
			printerSettings.MinimumPage = 1;
			printerSettings.FromPage = 1;
			FileManager fileManager = CurrentReport.FileManager;
			if (fileManager.Status == FileManagerStatus.Complete)
			{
				printerSettings.MaximumPage = fileManager.Count;
				printerSettings.ToPage = fileManager.Count;
			}
			else
			{
				printerSettings.ToPage = 1;
			}
			return printerSettings;
		}

		public PageSettings GetPageSettings()
		{
			return ReportViewerUtils.DeepClonePageSettings(PageSettings);
		}

		public void ResetPageSettings()
		{
			ResetAndGetPageSettings();
		}

		private PageSettings ResetAndGetPageSettings()
		{
			PageSettings pageSettings = null;
			try
			{
				pageSettings = Report.GetDefaultPageSettings().ToPageSettings(PrinterSettings);
			}
			catch (MissingReportSourceException)
			{
				pageSettings = null;
			}
			m_reportHierarchy.Peek().PageSettings = pageSettings;
			return pageSettings;
		}

		public void SetPageSettings(PageSettings pageSettings)
		{
			if (pageSettings == null)
			{
				throw new ArgumentNullException("pageSettings");
			}
			pageSettings = ReportViewerUtils.DeepClonePageSettings(pageSettings);
			if (DisplayMode == DisplayMode.PrintLayout)
			{
				try
				{
					CancelAllRenderingRequests();
					CurrentReport.PageSettings = pageSettings;
					RenderForPreview(new PostRenderArgs(isDifferentReport: true, isPartialRendering: false), invalidateCache: false);
				}
				catch (ObjectDisposedException)
				{
					throw;
				}
				catch (Exception e)
				{
					UpdateUIState(e);
				}
			}
			else
			{
				CurrentReport.PageSettings = pageSettings;
			}
		}

		private void RenderForPreview(PostRenderArgs postRenderArgs, bool invalidateCache)
		{
			if (this.RenderingBegin != null)
			{
				CancelEventArgs cancelEventArgs = new CancelEventArgs();
				this.RenderingBegin(this, cancelEventArgs);
				if (cancelEventArgs.Cancel)
				{
					return;
				}
			}
			m_searchState = null;
			CancelAllRenderingRequests();
			if (invalidateCache)
			{
				CurrentReport.FileManager.Clean();
			}
			if (m_viewMode == DisplayMode.PrintLayout && CurrentReport.FileManager.Status == FileManagerStatus.Complete)
			{
				SetViewForCurrentPage(UIState.ProcessingSuccess, postRenderArgs);
				if (this.RenderingComplete != null)
				{
					this.RenderingComplete(this, new RenderingCompleteEventArgs(null, null));
				}
				return;
			}
			UpdateUIState(UIState.LongRunningAction);
			if (!base.IsHandleCreated)
			{
				CreateHandle();
			}
			if (m_viewMode == DisplayMode.PrintLayout)
			{
				string deviceInfo = CreateEMFDeviceInfo(0, 0);
				BeginAsyncRender("IMAGE", allowInternalRenderers: true, deviceInfo, PageCountMode.Actual, PrintCreateAndRegisterStream, OnRenderingComplete, postRenderArgs, requireCompletionOnUIThread: true);
			}
			else
			{
				RenderRPLPage(CurrentPage, postRenderArgs);
			}
		}

		private void BeginAsyncRender(string format, bool allowInternalRenderers, string deviceInfo, PageCountMode pageCountMode, CreateAndRegisterStream createStreamCallback, AsyncCompletedEventHandler onCompleteCallback, PostRenderArgs postRenderArgs, bool requireCompletionOnUIThread)
		{
			AsyncReportOperation asyncReportOperation = (createStreamCallback != null) ? ((AsyncRenderingOperation)new AsyncAllStreamsRenderingOperation(Report, pageCountMode, format, deviceInfo, allowInternalRenderers, postRenderArgs, createStreamCallback)) : ((AsyncRenderingOperation)new AsyncMainStreamRenderingOperation(Report, pageCountMode, format, deviceInfo, allowInternalRenderers, postRenderArgs));
			asyncReportOperation.Completed += onCompleteCallback;
			if (requireCompletionOnUIThread)
			{
				asyncReportOperation = WrapAsyncOperationForUIThreadNotification(asyncReportOperation);
			}
			BackgroundThread.BeginBackgroundOperation(asyncReportOperation);
		}

		private void RenderRPLPage(int page, PostRenderArgs postRenderArgs)
		{
			string format = "RPL";
			string text = "10.6";
			string format2 = "<DeviceInfo><StartPage>{0}</StartPage><EndPage>{1}</EndPage><MeasureItems>{2}</MeasureItems><SecondaryStreams>{3}</SecondaryStreams><StreamNames>{4}</StreamNames><RPLVersion>{5}</RPLVersion></DeviceInfo>";
			string deviceInfo = string.Format(CultureInfo.InvariantCulture, format2, page, page, true, SecondaryStreams.Embedded, false, text);
			BeginAsyncRender(format, allowInternalRenderers: true, deviceInfo, PageCountMode, null, OnRenderingComplete, postRenderArgs, requireCompletionOnUIThread: true);
		}

		private string CreateEMFDeviceInfo(int startPage, int endPage)
		{
			string text = "";
			PageSettings pageSettings = PageSettings;
			int hundrethsOfInch = pageSettings.Landscape ? pageSettings.PaperSize.Height : pageSettings.PaperSize.Width;
			int hundrethsOfInch2 = pageSettings.Landscape ? pageSettings.PaperSize.Width : pageSettings.PaperSize.Height;
			text = string.Format(CultureInfo.InvariantCulture, "<MarginTop>{0}</MarginTop><MarginLeft>{1}</MarginLeft><MarginRight>{2}</MarginRight><MarginBottom>{3}</MarginBottom><PageHeight>{4}</PageHeight><PageWidth>{5}</PageWidth>", ToInches(pageSettings.Margins.Top), ToInches(pageSettings.Margins.Left), ToInches(pageSettings.Margins.Right), ToInches(pageSettings.Margins.Bottom), ToInches(hundrethsOfInch2), ToInches(hundrethsOfInch));
			return string.Format(CultureInfo.InvariantCulture, "<DeviceInfo><OutputFormat>emf</OutputFormat><StartPage>{0}</StartPage><EndPage>{1}</EndPage>{2}</DeviceInfo>", startPage, endPage, text);
		}

		private static string ToInches(int hundrethsOfInch)
		{
			return ((double)hundrethsOfInch / 100.0).ToString(CultureInfo.InvariantCulture) + "in";
		}

		private Stream CreateStreamEMF(string name, string extension, Encoding encoding, string mimeType, bool useChunking, StreamOper operation)
		{
			Stream result = CreateStreamEMFPrintOnly(name, extension, encoding, mimeType, useChunking, operation);
			PageCountMode pageCountMode;
			int totalPages = GetTotalPages(out pageCountMode);
			if (totalPages > 0 && base.IsHandleCreated)
			{
				RegisterAsyncInvoke(delegate
				{
					OnPrintPreviewPageAvailableUI(totalPages, isLastPage: false);
				});
			}
			return result;
		}

		private Stream CreateStreamEMFPrintOnly(string name, string extension, Encoding encoding, string mimeType, bool useChunking, StreamOper operation)
		{
			return CurrentReport.FileManager.CreatePage(operation == StreamOper.CreateAndRegister || operation == StreamOper.CreateForPersistedStreams);
		}

		protected override void Dispose(bool disposing)
		{
			m_disposing = true;
			CancelAllRenderingRequests();
			m_reportHierarchy.Dispose();
			base.Dispose(disposing);
		}

		protected override void OnLayout(LayoutEventArgs e)
		{
			paramsSplitContainer.SuspendLayout();
			try
			{
				base.OnLayout(e);
				OnPreferredPromptAreaHeightChanged(this, EventArgs.Empty);
			}
			finally
			{
				paramsSplitContainer.ResumeLayout();
			}
		}

		private void OnPreferredPromptAreaHeightChanged(object sender, EventArgs e)
		{
			if (!m_userChangedSplitter)
			{
				int val = paramsSplitContainer.Height - paramsSplitContainer.SplitterWidth - paramsSplitContainer.Panel2MinSize;
				int num = Math.Min(rsParams.PreferredHeight, val);
				if (num >= paramsSplitContainer.Panel1MinSize)
				{
					paramsSplitContainer.SplitterDistance = num;
				}
			}
		}

		private void OnParamsSplitterMoving(object sender, EventArgs e)
		{
			m_userChangedSplitter = true;
		}

		private void OnPromptAreaCollapse(object sender, EventArgs e)
		{
			PromptAreaCollapsed = paramsSplitContainer.Collapsed;
		}

		private void OnDocumentMapCollapse(object sender, EventArgs e)
		{
			DocumentMapCollapsed = dmSplitContainer.Collapsed;
		}

		protected override void OnLoad(EventArgs e)
		{
			if (!base.DesignMode)
			{
				ApplySplitterResources(allResources: true);
			}
			base.OnLoad(e);
		}

		private void ApplySplitterResources(bool allResources)
		{
			if (allResources)
			{
				paramsSplitContainer.Splitter.AccessibleName = ReportPreviewStrings.ShowParamsAccessibleName;
				paramsSplitContainer.Splitter.ButtonAreaAccessibleName = ReportPreviewStrings.ShowParamsHotAreaAccessibleName;
				dmSplitContainer.Splitter.AccessibleName = ReportPreviewStrings.DocMapAccessibleName;
				dmSplitContainer.Splitter.ButtonAreaAccessibleName = ReportPreviewStrings.DocMapHotAreaAccessibleName;
			}
			paramsSplitContainer.ToolTip = LocalizationHelper.Current.ParameterAreaButtonToolTip;
			dmSplitContainer.ToolTip = LocalizationHelper.Current.DocumentMapButtonToolTip;
		}

		public void SetDisplayMode(DisplayMode mode)
		{
			try
			{
				CancelAllRenderingRequests();
				m_viewMode = mode;
				ZoomPercent = 100;
				ZoomMode = ((m_viewMode != DisplayMode.PrintLayout) ? ZoomMode.Percent : ZoomMode.FullPage);
				CurrentReport.CurrentPage = 1;
				RenderForPreview(new PostRenderArgs(isDifferentReport: true, isPartialRendering: false), invalidateCache: false);
			}
			catch (ObjectDisposedException)
			{
				throw;
			}
			catch (Exception e)
			{
				UpdateUIState(e);
			}
		}

		private void OnViewButtonClick(object sender, EventArgs e)
		{
			if (this.ViewButtonClick != null)
			{
				CancelEventArgs cancelEventArgs = new CancelEventArgs();
				this.ViewButtonClick(this, cancelEventArgs);
				if (cancelEventArgs.Cancel)
				{
					return;
				}
			}
			RenderReportWithNewParameters(1, null);
		}

		public DialogResult ExportDialog(RenderingExtension extension)
		{
			return ExportDialog(extension, null, null);
		}

		public DialogResult ExportDialog(RenderingExtension extension, string deviceInfo)
		{
			return ExportDialog(extension, deviceInfo, null);
		}

		public DialogResult ExportDialog(RenderingExtension extension, string deviceInfo, string fileName)
		{
			if (!ReportViewerStatus.DoesStateAllowExport(m_lastUIState))
			{
				throw new InvalidOperationException();
			}
			if (extension == null)
			{
				throw new ArgumentNullException("extension");
			}
			bool flag = false;
			RenderingExtension[] array = Report.ListRenderingExtensions();
			for (int i = 0; i < array.Length; i++)
			{
				if (string.Equals(array[i].Name, extension.Name, StringComparison.Ordinal))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				throw new ArgumentOutOfRangeException("extension");
			}
			ExportDialog exportDialog = new ExportDialog(this, extension, deviceInfo, fileName);
			exportDialog.Closed += ExportDialogClosed;
			exportDialog.Font = Font;
			return exportDialog.ShowDialog(this);
		}

		private void OnPageSetup(object sender, EventArgs e)
		{
			PageSetupDialog();
		}

		public DialogResult PageSetupDialog()
		{
			if (!PrinterSettings.IsValid)
			{
				DisplayErrorMsgBox(new InvalidPrinterException(new PrinterSettings()), LocalizationHelper.Current.MessageBoxTitle);
				return DialogResult.Abort;
			}
			using (PageSetupDialog pageSetupDialog = new PageSetupDialog())
			{
				pageSetupDialog.AllowPrinter = true;
				pageSetupDialog.EnableMetric = true;
				pageSetupDialog.PrinterSettings = PrinterSettings;
				pageSetupDialog.PageSettings = (PageSettings)PageSettings.Clone();
				DialogResult num = pageSetupDialog.ShowDialog(this);
				if (num == DialogResult.OK)
				{
					bool num2 = pageSetupDialog.PageSettings.Margins != PageSettings.Margins || pageSetupDialog.PageSettings.Bounds != PageSettings.Bounds;
					CurrentReport.PageSettings = pageSetupDialog.PageSettings;
					PrinterSettings = pageSetupDialog.PrinterSettings;
					if (num2)
					{
						if (this.PageSettingsChanged != null)
						{
							this.PageSettingsChanged(this, EventArgs.Empty);
						}
						CurrentReport.FileManager.Clean();
						if (m_viewMode == DisplayMode.PrintLayout)
						{
							RenderForPreview(new PostRenderArgs(isDifferentReport: true, isPartialRendering: false), invalidateCache: false);
						}
					}
				}
				return num;
			}
		}

		private void CancelAutoRefreshTimer()
		{
			m_autoRefreshTimer.Stop();
		}

		private void StartAutoRefreshTimer(int autoRefreshSeconds)
		{
			if (autoRefreshSeconds > 0)
			{
				m_autoRefreshTimer.Interval = autoRefreshSeconds * 1000;
				m_autoRefreshTimer.Start();
			}
		}

		private void SetZoom()
		{
			winRSviewer.SetZoom();
			reportToolBar.SetZoom();
			UpdateUIState(m_lastUIState);
		}

		internal void FireAnAction(Action action, bool shiftKeyDown)
		{
			try
			{
				if (action == null)
				{
					return;
				}
				bool flag = false;
				if (ActionType.BookmarkLink == action.Type)
				{
					BookmarkNavigationEventArgs bookmarkNavigationEventArgs = new BookmarkNavigationEventArgs(((BookmarkLinkAction)action).ActionLink);
					if (this.BookmarkNavigation != null)
					{
						this.BookmarkNavigation(this, bookmarkNavigationEventArgs);
					}
					if (!bookmarkNavigationEventArgs.Cancel)
					{
						string uniqueName;
						int num = Report.PerformBookmarkNavigation(bookmarkNavigationEventArgs.BookmarkId, out uniqueName);
						if (num > 0 && (num == CurrentPage || OnPageNavigation(num)))
						{
							SetCurrentPage(num, ActionType.BookmarkLink, uniqueName);
						}
					}
				}
				else if (ActionType.HyperLink == action.Type)
				{
					HyperLinkAction hyperLinkAction = (HyperLinkAction)action;
					HyperlinkEventArgs hyperlinkEventArgs = new HyperlinkEventArgs(hyperLinkAction.Url);
					if (this.Hyperlink != null)
					{
						this.Hyperlink(this, hyperlinkEventArgs);
					}
					if (!hyperlinkEventArgs.Cancel)
					{
						SpawnHyperLink(hyperLinkAction.Url);
					}
				}
				else if (ActionType.DrillThrough == action.Type)
				{
					RenderDrillthrough((DrillthroughAction)action);
				}
				else if (ActionType.Toggle == action.Type)
				{
					CancelEventArgs cancelEventArgs = new CancelEventArgs();
					if (this.Toggle != null)
					{
						this.Toggle(this, cancelEventArgs);
					}
					if (!cancelEventArgs.Cancel)
					{
						Report.PerformToggle(action.Id);
						flag = true;
						RenderForPreview(new PostRenderArgs(ActionType.Toggle, action.Id), invalidateCache: true);
					}
				}
				else if (ActionType.Sort == action.Type)
				{
					SortAction sortAction = (SortAction)action;
					bool clearSort = (!shiftKeyDown) ? true : false;
					SortEventArgs sortEventArgs = new SortEventArgs(action.Id, sortAction.Direction, clearSort);
					if (this.Sort != null)
					{
						this.Sort(this, sortEventArgs);
					}
					if (!sortEventArgs.Cancel)
					{
						string uniqueName2 = null;
						int num2 = Report.PerformSort(action.Id, sortAction.Direction, clearSort, PageCountMode, out uniqueName2);
						if (num2 == CurrentPage || OnPageNavigation(num2))
						{
							CurrentReport.CurrentPage = num2;
						}
						else
						{
							int totalPages = Report.GetTotalPages();
							if (CurrentPage > totalPages)
							{
								CurrentReport.CurrentPage = totalPages;
							}
						}
						flag = true;
						RenderForPreview(new PostRenderArgs(ActionType.Sort, uniqueName2), invalidateCache: true);
					}
				}
				if (flag)
				{
					CurrentReport.FileManager.Clean();
				}
			}
			catch (Exception e)
			{
				UpdateUIState(e);
			}
		}

		private void SpawnHyperLink(string url)
		{
			try
			{
				if (url != null && (url.StartsWith(Uri.UriSchemeHttp + Uri.SchemeDelimiter, StringComparison.OrdinalIgnoreCase) || url.StartsWith(Uri.UriSchemeHttps + Uri.SchemeDelimiter, StringComparison.OrdinalIgnoreCase) || url.StartsWith(Uri.UriSchemeMailto + ":", StringComparison.OrdinalIgnoreCase)))
				{
					Process.Start(url);
				}
			}
			catch (Win32Exception ex)
			{
				DisplayErrorMsgBox(ex, LocalizationHelper.Current.HyperlinkErrorTitle);
			}
		}

		internal void UpdateUIState(Exception e)
		{
			if (OnError(e))
			{
				winRSviewer.SetNewPage(null);
			}
			else
			{
				winRSviewer.ShowMessage(e);
			}
			UpdateUIState(UIState.ProcessingFailure);
		}

		private void UpdateUIState(UIState newState)
		{
			if (newState != UIState.ProcessingSuccess)
			{
				CancelAutoRefreshTimer();
			}
			if (newState == UIState.NoReport)
			{
				winRSviewer.SetNewPage(null);
			}
			m_status = new ReportViewerStatus(this, newState, m_searchState != null, ParametersAreaSupported, rsParams.HaveContent, rsDocMap.HasDocMap);
			bool flag = rsDocMap.HasDocMap && DisplayMode == DisplayMode.Normal;
			rsDocMap.Visible = flag;
			dmSplitContainer.Panel1Visible = flag;
			dmSplitContainer.CanCollapse = ShowDocumentMapButton;
			dmSplitContainer.Collapsed = DocumentMapCollapsed;
			bool flag2 = rsParams.HaveContent && ParametersAreaSupported;
			rsParams.Visible = flag2;
			paramsSplitContainer.Panel1Visible = flag2;
			paramsSplitContainer.SplitterVisible = ShowPromptAreaButton;
			paramsSplitContainer.Collapsed = PromptAreaCollapsed;
			rsParams.Enabled = (m_status.CanSubmitPromptAreaValues && m_status.IsPromptingSupported);
			OnStatusChanged(this, EventArgs.Empty);
			m_canRenderForWaitControl = (m_lastUIState == UIState.ProcessingSuccess);
			m_lastUIState = newState;
		}

		private void RenderToGraphics(Graphics g)
		{
			winRSviewer.RenderToGraphics(g, testMode: true);
		}

		void IRenderable.RenderToGraphics(Graphics g)
		{
			RenderToGraphics(g);
		}
	}
}
