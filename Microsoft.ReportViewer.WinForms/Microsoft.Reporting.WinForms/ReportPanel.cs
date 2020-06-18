using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Microsoft.Reporting.WinForms
{
	internal class ReportPanel : MirrorPanel
	{
		private abstract class AccessibleObjectBase : AccessibleObject
		{
			private ReportPanel m_control;

			private AccessibleObject m_parent;

			private IList<AccessibleObject> m_children;

			public override AccessibleObject Parent => m_parent;

			protected ReportPanel Control => m_control;

			public override AccessibleRole Role
			{
				get
				{
					EnsureChildren();
					if (m_children.Count > 0)
					{
						return AccessibleRole.Grouping;
					}
					return AccessibleRole.Graphic;
				}
			}

			public override AccessibleStates State
			{
				get
				{
					AccessibleStates accessibleStates = AccessibleStates.None;
					Rectangle bounds = Bounds;
					Rectangle rect = Control.RectangleToScreen(Control.ClientRectangle);
					if (!bounds.IntersectsWith(rect))
					{
						accessibleStates |= (AccessibleStates.Invisible | AccessibleStates.Offscreen);
					}
					return accessibleStates;
				}
			}

			protected virtual Action Action => null;

			public override string DefaultAction
			{
				get
				{
					if (Action != null)
					{
						return Action.LocalizedTypeName;
					}
					return null;
				}
			}

			public AccessibleObjectBase(ReportPanel control, AccessibleObject parent)
			{
				m_control = control;
				m_parent = parent;
			}

			public override AccessibleObject Navigate(AccessibleNavigation navdir)
			{
				return (m_parent as AccessibleObjectBase)?.NavigateFromChild(this, navdir);
			}

			public override void DoDefaultAction()
			{
				Action action = Action;
				if (action == null)
				{
					return;
				}
				GdiPage gdiPage = Control.CurrentPage as GdiPage;
				if (gdiPage == null)
				{
					return;
				}
				List<int> sortedActionItemIndices = gdiPage.SortedActionItemIndices;
				int num = 0;
				while (true)
				{
					if (num < sortedActionItemIndices.Count)
					{
						if (action == gdiPage.Actions[sortedActionItemIndices[num]])
						{
							break;
						}
						num++;
						continue;
					}
					return;
				}
				gdiPage.ActionIndex = num;
				Control.ProcessCurrentAction();
			}

			public override int GetChildCount()
			{
				EnsureChildren();
				return m_children.Count;
			}

			public override AccessibleObject GetChild(int index)
			{
				EnsureChildren();
				if (index < 0 || index >= m_children.Count)
				{
					return null;
				}
				return m_children[index];
			}

			protected virtual void EnsureChildren()
			{
				if (m_children == null)
				{
					m_children = CreateChildren();
				}
			}

			protected virtual IList<AccessibleObject> CreateChildren()
			{
				return new List<AccessibleObject>();
			}

			private AccessibleObject NavigateFromChild(AccessibleObject child, AccessibleNavigation navdir)
			{
				int num = m_children.IndexOf(child);
				if (num >= 0)
				{
					switch (navdir)
					{
					case AccessibleNavigation.Next:
						return GetChild(num + 1);
					case AccessibleNavigation.Previous:
						return GetChild(num - 1);
					}
				}
				return null;
			}
		}

		private abstract class RenderingElementBaseAccessibleObject : AccessibleObjectBase
		{
			private RenderingElementBase m_renderingElementBase;

			public override string Name
			{
				get
				{
					return m_renderingElementBase.AccessibleName;
				}
				set
				{
				}
			}

			public override Rectangle Bounds => GetBounds(base.Control, m_renderingElementBase.Position);

			public RenderingElementBaseAccessibleObject(ReportPanel control, AccessibleObject parent, RenderingElementBase element)
				: base(control, parent)
			{
				m_renderingElementBase = element;
			}

			internal static Rectangle GetBounds(ReportPanel reportPanel, RectangleF position)
			{
				float dpiX;
				float dpiY;
				using (Graphics graphics = reportPanel.CreateGraphics())
				{
					dpiX = graphics.DpiX;
					dpiY = graphics.DpiY;
				}
				float zoomRate = reportPanel.GetZoomRate();
				Rectangle r = new Rectangle(Global.ToPixels(zoomRate * position.Left, dpiX) + reportPanel.AutoScrollPosition.X, Global.ToPixels(zoomRate * position.Top, dpiY) + reportPanel.AutoScrollPosition.Y, Global.ToPixels(zoomRate * position.Width, dpiX), Global.ToPixels(zoomRate * position.Height, dpiY));
				return reportPanel.RectangleToScreen(r);
			}
		}

		private class RenderingItemAccessibleObject : RenderingElementBaseAccessibleObject
		{
			private RenderingItem m_renderingItem;

			protected override Action Action
			{
				get
				{
					IList<Action> actions = m_renderingItem.Actions;
					if (actions.Count > 0)
					{
						return actions[0];
					}
					return null;
				}
			}

			public RenderingItemAccessibleObject(ReportPanel control, AccessibleObject parent, RenderingItem item)
				: base(control, parent, item)
			{
				m_renderingItem = item;
			}
		}

		private class RenderingReportSectionAccessibleObject : RenderingElementBaseAccessibleObject
		{
			private RenderingReportSection m_reportSection;

			public RenderingReportSectionAccessibleObject(ReportPanel control, AccessibleObject parent, RenderingReportSection section)
				: base(control, parent, section)
			{
				m_reportSection = section;
			}

			protected override IList<AccessibleObject> CreateChildren()
			{
				List<AccessibleObject> list = new List<AccessibleObject>(3);
				if (m_reportSection.Header != null)
				{
					list.Add(new RenderingItemContainerAccessibleObject(base.Control, this, m_reportSection.Header));
				}
				if (m_reportSection.Body != null)
				{
					list.Add(new RenderingItemContainerAccessibleObject(base.Control, this, m_reportSection.Body));
				}
				if (m_reportSection.Footer != null)
				{
					list.Add(new RenderingItemContainerAccessibleObject(base.Control, this, m_reportSection.Footer));
				}
				return list;
			}
		}

		private class RenderingTextBoxAccessibleObject : RenderingItemAccessibleObject
		{
			private RenderingTextBox m_textBox;

			private string m_text;

			private List<Action> m_childActions = new List<Action>();

			private Action m_nonChildAction;

			public override AccessibleRole Role => AccessibleRole.Text;

			protected override Action Action => m_nonChildAction;

			public override string Value
			{
				get
				{
					EnsureText();
					return m_text;
				}
				set
				{
				}
			}

			public override AccessibleStates State => base.State | AccessibleStates.ReadOnly;

			public RenderingTextBoxAccessibleObject(ReportPanel control, AccessibleObject parent, RenderingTextBox item)
				: base(control, parent, item)
			{
				m_textBox = item;
				foreach (Action action in item.Actions)
				{
					if (action.IsChildAction)
					{
						m_childActions.Add(action);
					}
					else if (m_nonChildAction == null)
					{
						m_nonChildAction = action;
					}
				}
			}

			public override int GetChildCount()
			{
				return m_childActions.Count;
			}

			public override AccessibleObject GetChild(int index)
			{
				if (index < 0 || index > m_childActions.Count)
				{
					return null;
				}
				return new ActionAccessibleObject(base.Control, this, m_childActions[index]);
			}

			private void EnsureText()
			{
				if (m_text == null)
				{
					string text = m_textBox.GetText();
					m_text = (text ?? string.Empty);
				}
			}
		}

		private class RenderingItemContainerAccessibleObject : RenderingItemAccessibleObject
		{
			private RenderingItemContainer m_container;

			public RenderingItemContainerAccessibleObject(ReportPanel control, AccessibleObject parent, RenderingItemContainer container)
				: base(control, parent, container)
			{
				m_container = container;
			}

			protected override IList<AccessibleObject> CreateChildren()
			{
				List<AccessibleObject> list = new List<AccessibleObject>();
				if (m_container == null || m_container.Children == null)
				{
					return list;
				}
				List<int> sortedRenderingItemIndices = m_container.SortedRenderingItemIndices;
				for (int i = 0; i < m_container.Children.Count; i++)
				{
					RenderingItem renderingItem = m_container.Children[sortedRenderingItemIndices[i]];
					AccessibleObjectBase item = (!(renderingItem is RenderingItemContainer)) ? ((!(renderingItem is RenderingTextBox)) ? new RenderingItemAccessibleObject(base.Control, this, renderingItem) : new RenderingTextBoxAccessibleObject(base.Control, this, (RenderingTextBox)renderingItem)) : new RenderingItemContainerAccessibleObject(base.Control, this, (RenderingItemContainer)renderingItem);
					list.Add(item);
				}
				return list;
			}
		}

		private class ActionAccessibleObject : AccessibleObjectBase
		{
			private Action m_action;

			protected override Action Action => m_action;

			public override string Name
			{
				get
				{
					return Action.LocalizedTypeName;
				}
				set
				{
				}
			}

			public override Rectangle Bounds
			{
				get
				{
					using (base.Control.CreateGraphics())
					{
						float zoomRate = base.Control.GetZoomRate();
						Rectangle r = new Rectangle((int)(zoomRate * (float)(Action.Position.Left + Action.XOffset)) + base.Control.AutoScrollPosition.X, (int)(zoomRate * (float)(Action.Position.Top + Action.YOffset)) + base.Control.AutoScrollPosition.Y, (int)(zoomRate * (float)Action.Position.Width), (int)(zoomRate * (float)Action.Position.Height));
						return base.Control.RectangleToScreen(r);
					}
				}
			}

			public ActionAccessibleObject(ReportPanel control, AccessibleObject parent, Action action)
				: base(control, parent)
			{
				m_action = action;
			}
		}

		private class ReportPanelAccessibleObject : ControlAccessibleObject
		{
			private ReportPanel m_control;

			public override string Name
			{
				get
				{
					return ReportPreviewStrings.ReportAccessibleName;
				}
				set
				{
				}
			}

			public override Rectangle Bounds
			{
				get
				{
					RenderingReport report = Report;
					if (report != null)
					{
						return RenderingElementBaseAccessibleObject.GetBounds(m_control, report.Position);
					}
					return Rectangle.Empty;
				}
			}

			public override AccessibleRole Role => AccessibleRole.Pane;

			private RenderingReport Report => m_control.ViewerControl.CurrentReport.GdiRenderer?.Report;

			public ReportPanelAccessibleObject(ReportPanel control)
				: base(control)
			{
				if (control == null)
				{
					throw new ArgumentNullException("control");
				}
				m_control = control;
			}

			private IList<AccessibleObject> CreateChildren()
			{
				List<AccessibleObject> list = new List<AccessibleObject>();
				RenderingReport report = Report;
				if (report == null)
				{
					return list;
				}
				foreach (RenderingReportSection reportSection in report.ReportSections)
				{
					list.Add(new RenderingReportSectionAccessibleObject(m_control, this, reportSection));
				}
				return list;
			}

			public override int GetChildCount()
			{
				return CreateChildren().Count;
			}

			public override AccessibleObject GetChild(int index)
			{
				IList<AccessibleObject> list = CreateChildren();
				if (index < 0 || index >= list.Count)
				{
					return null;
				}
				return list[index];
			}
		}

		private class RenderingPanel : Panel
		{
			private ReportPanel m_host;

			private ReportToolTip m_lastToolTip;

			private ToolTip m_toolTipControl;

			private ReportViewer ViewerControl => m_host.ViewerControl;

			private bool InLongRunningAction => m_host.m_inLongRunningAction;

			private DrawablePage CurrentPage => m_host.CurrentPage;

			private Size PanelSize => m_host.Size;

			public RenderingPanel(ReportPanel host)
			{
				if (host == null)
				{
					throw new ArgumentNullException("host");
				}
				m_host = host;
				SetStyle(ControlStyles.DoubleBuffer, value: true);
				SetStyle(ControlStyles.AllPaintingInWmPaint, value: true);
				SetStyle(ControlStyles.UserPaint, value: true);
				m_toolTipControl = new ToolTip();
				m_toolTipControl.UseFading = true;
				m_toolTipControl.UseAnimation = true;
			}

			private float GetZoomRate()
			{
				return m_host.GetZoomRate();
			}

			private Action GetActionAtPoint(ReportActions actions, int left, int top)
			{
				if (actions == null || actions.Count == 0)
				{
					return null;
				}
				float zoomRate = GetZoomRate();
				left = (int)((float)left / zoomRate);
				top = (int)((float)top / zoomRate);
				foreach (Action action in actions)
				{
					int num = action.Position.Left + action.XOffset;
					int num2 = num + action.Position.Width;
					int num3 = action.Position.Top + action.YOffset;
					int num4 = num3 + action.Position.Height;
					if (left < num || top < num3 || left > num2 || top > num4)
					{
						continue;
					}
					if (action.Shape == ActionShape.Rectangle)
					{
						return action;
					}
					using (GraphicsPath graphicsPath = new GraphicsPath())
					{
						if (action.Shape == ActionShape.Circle)
						{
							graphicsPath.AddEllipse(num, num3, action.Position.Width, action.Position.Height);
						}
						else if (num > 0 || num3 > 0)
						{
							PointF[] array = new PointF[action.Points.Length];
							for (int i = 0; i < array.Length; i++)
							{
								PointF pointF = action.Points[i];
								pointF.X += num;
								pointF.Y += num3;
								array[i] = pointF;
							}
							graphicsPath.AddPolygon(array);
						}
						else
						{
							graphicsPath.AddPolygon(action.Points);
						}
						using (Region region = new Region(graphicsPath))
						{
							if (region.IsVisible(left, top))
							{
								return action;
							}
						}
					}
				}
				return null;
			}

			private void DrawPageFrame(Graphics g, float width, float height)
			{
				float num = -1f;
				float num2 = -1f;
				Pen black = Pens.Black;
				g.DrawLine(black, num, num2, width, num2);
				g.DrawLine(black, num, num2, num, height);
				g.DrawLine(black, width, num2, width, height + 1f);
				g.DrawLine(black, width + 1f, num2 + 1f, width + 1f, height + 1f);
				g.DrawLine(black, num, height, width, height);
				g.DrawLine(black, num + 1f, height + 1f, width + 1f, height + 1f);
			}

			public void RecalculateSize()
			{
				if (CurrentPage != null)
				{
					SizeF zoomedSize = GetZoomedSize();
					int width;
					int height;
					if (CurrentPage.NeedsFrame)
					{
						width = Math.Max(PanelSize.Width, Convert.ToInt32(zoomedSize.Width));
						height = Math.Max(PanelSize.Height, Convert.ToInt32(zoomedSize.Height));
					}
					else
					{
						width = Convert.ToInt32(zoomedSize.Width);
						height = Convert.ToInt32(zoomedSize.Height);
					}
					base.Size = new Size(width, height);
				}
				else
				{
					base.Size = default(Size);
				}
				Invalidate();
			}

			private SizeF GetZoomedSize()
			{
				float zoomRate;
				SizeF unzoomedSize;
				return GetZoomedSize(out zoomRate, out unzoomedSize);
			}

			private SizeF GetZoomedSize(out float zoomRate, out SizeF unzoomedSize)
			{
				float width;
				float height;
				using (Graphics g = CreateGraphics())
				{
					CurrentPage.GetPageSize(g, out width, out height);
				}
				unzoomedSize = new SizeF(width, height);
				zoomRate = GetZoomRate();
				return new SizeF(width * zoomRate, height * zoomRate);
			}

			public void RenderToGraphics(Graphics g, bool testMode)
			{
				float zoomRate;
				SizeF unzoomedSize;
				SizeF zoomedSize = GetZoomedSize(out zoomRate, out unzoomedSize);
				g.ResetTransform();
				g.ScaleTransform(zoomRate, zoomRate);
				Point point = default(Point);
				float num = CurrentPage.ExternalMargin;
				int num4 = point.Y = (point.X = Convert.ToInt32(zoomRate * num));
				if (CurrentPage.NeedsFrame)
				{
					if ((float)PanelSize.Height > zoomedSize.Height)
					{
						point.Y += (int)(((float)PanelSize.Height - zoomedSize.Height) / 2f);
					}
					if ((float)PanelSize.Width > zoomedSize.Width)
					{
						point.X += (int)(((float)PanelSize.Width - zoomedSize.Width) / 2f);
					}
				}
				PointF pointF = default(PointF);
				pointF.X = (float)point.X / zoomRate;
				pointF.Y = (float)point.Y / zoomRate;
				if (CurrentPage.DrawInPixels)
				{
					g.TranslateTransform(pointF.X, pointF.Y);
				}
				else
				{
					g.TranslateTransform(Global.ToMillimeters(pointF.X, g.DpiX), Global.ToMillimeters(pointF.Y, g.DpiY));
				}
				if (CurrentPage.NeedsFrame)
				{
					g.Clear(Color.Gray);
				}
				PointF scrollOffset = new PointF(m_host.AutoScrollPosition.X, m_host.AutoScrollPosition.Y);
				if (!CurrentPage.DrawInPixels)
				{
					scrollOffset.X = Global.ToMillimeters(scrollOffset.X, g.DpiX);
					scrollOffset.Y = Global.ToMillimeters(scrollOffset.Y, g.DpiY);
				}
				CurrentPage.Draw(g, scrollOffset, testMode);
				if (CurrentPage.NeedsFrame)
				{
					g.ResetTransform();
					g.ScaleTransform(zoomRate, zoomRate);
					g.TranslateTransform(pointF.X, pointF.Y);
					DrawPageFrame(g, unzoomedSize.Width - 2f * num, unzoomedSize.Height - 2f * num);
				}
			}

			protected override void OnPaint(PaintEventArgs e)
			{
				if (!base.DesignMode && CurrentPage != null)
				{
					RenderToGraphics(e.Graphics, testMode: false);
				}
			}

			protected override void OnMouseWheel(MouseEventArgs e)
			{
				m_host.OnMouseWheel(e);
			}

			protected override void OnMouseUp(MouseEventArgs e)
			{
				try
				{
					if (!InLongRunningAction && CurrentPage != null && e.Button == MouseButtons.Left)
					{
						bool shiftKeyDown = (Control.ModifierKeys & Keys.Shift) > Keys.None;
						Action actionAtPoint = GetActionAtPoint(CurrentPage.Actions, e.X, e.Y);
						if (actionAtPoint != null)
						{
							ViewerControl.FireAnAction(actionAtPoint, shiftKeyDown);
						}
					}
				}
				catch (Exception e2)
				{
					ViewerControl.UpdateUIState(e2);
				}
			}

			protected override void OnMouseMove(MouseEventArgs e)
			{
				try
				{
					if (!InLongRunningAction && CurrentPage != null)
					{
						Action actionAtPoint = GetActionAtPoint(CurrentPage.Actions, e.X, e.Y);
						if (actionAtPoint != null)
						{
							Cursor.Current = Cursors.Hand;
						}
						else
						{
							Cursor.Current = Cursors.Default;
						}
						ReportToolTip reportToolTip = (ReportToolTip)GetActionAtPoint(CurrentPage.ToolTips, e.X, e.Y);
						if (reportToolTip == null)
						{
							m_toolTipControl.RemoveAll();
						}
						else if (!reportToolTip.Equals(m_lastToolTip))
						{
							m_toolTipControl.RemoveAll();
							m_toolTipControl.SetToolTip(this, reportToolTip.Caption);
						}
						m_lastToolTip = reportToolTip;
					}
				}
				catch (Exception e2)
				{
					ViewerControl.UpdateUIState(e2);
				}
			}
		}

		private bool m_showContextMenu = true;

		private bool m_inLongRunningAction;

		private IContainer components;

		private DrawablePage m_currentPage;

		private RenderingPanel m_renderPanel;

		private ContextMenuStrip m_contextMenu;

		private ToolStripMenuItem m_documentMapToolStripMenuItem;

		private ToolStripMenuItem m_backToolStripMenuItem;

		private ToolStripMenuItem m_refreshToolStripMenuItem;

		private ToolStripMenuItem m_printToolStripMenuItem;

		private ToolStripMenuItem m_printLayoutToolStripMenuItem;

		private ToolStripMenuItem m_exportToolStripMenuItem;

		private ToolStripMenuItem m_stopToolStripMenuItem;

		private ToolStripMenuItem m_zoomToolStripMenuItem;

		private ToolStripMenuItem m_pageSetupToolStripMenuItem;

		private ReportViewer m_viewerControl;

		private Dictionary<string, Point> m_previousChildActionPoints;

		private int m_previousPageWidth;

		public bool ShowContextMenu
		{
			get
			{
				return m_showContextMenu;
			}
			set
			{
				m_showContextMenu = value;
			}
		}

		public DrawablePage CurrentPage
		{
			get
			{
				return m_currentPage;
			}
			set
			{
				m_previousChildActionPoints.Clear();
				ReportActions currentPageActions = GetCurrentPageActions();
				if (currentPageActions != null)
				{
					foreach (Action item in currentPageActions)
					{
						if (item.IsChildAction && !m_previousChildActionPoints.ContainsKey(item.Id))
						{
							m_previousChildActionPoints.Add(item.Id, item.Position.Location);
						}
					}
				}
				m_previousPageWidth = GetCurrentPageWidth();
				m_currentPage = value;
				base.AutoScrollPosition = Point.Empty;
				m_renderPanel.RecalculateSize();
			}
		}

		public ReportViewer ViewerControl
		{
			get
			{
				return m_viewerControl;
			}
			set
			{
				if (m_viewerControl != null)
				{
					m_viewerControl.StatusChanged -= OnReportViewerStateChanged;
				}
				m_viewerControl = value;
				if (m_viewerControl != null)
				{
					m_viewerControl.StatusChanged += OnReportViewerStateChanged;
				}
			}
		}

		public event InternalPageNavigationEventHandler PageNavigation;

		public event EventHandler Back;

		public event EventHandler ReportRefresh;

		public event EventHandler Print;

		public event EventHandler PageSettings;

		public event ExportEventHandler Export;

		public event ZoomChangedEventHandler ZoomChange;

		public ReportPanel()
		{
			InitializeComponent();
			InitializeContextMenu();
			AutoScroll = true;
			m_renderPanel = new RenderingPanel(this);
			m_renderPanel.Dock = DockStyle.None;
			m_renderPanel.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
			m_renderPanel.Location = new Point(0, 0);
			m_renderPanel.SizeChanged += OnRenderingPanelSizeChanged;
			m_renderPanel.MouseClick += OnMouseClick;
			base.MouseClick += OnMouseClick;
			m_previousChildActionPoints = new Dictionary<string, Point>();
			base.Controls.Add(m_renderPanel);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				m_contextMenu.Dispose();
			}
			base.Dispose(disposing);
		}

		private void OnDocumentMapClick(object sender, EventArgs e)
		{
			try
			{
				ViewerControl.DocumentMapCollapsed = !ViewerControl.DocumentMapCollapsed;
			}
			catch (Exception e2)
			{
				ViewerControl.UpdateUIState(e2);
			}
		}

		private void OnBackClick(object sender, EventArgs e)
		{
			if (this.Back != null)
			{
				this.Back(this, e);
			}
		}

		private void OnReportRefreshClick(object sender, EventArgs e)
		{
			if (this.ReportRefresh != null)
			{
				this.ReportRefresh(this, e);
			}
		}

		private void OnPrintClick(object sender, EventArgs e)
		{
			if (this.Print != null)
			{
				this.Print(this, e);
			}
		}

		private void OnPrintLayoutClick(object sender, EventArgs e)
		{
			try
			{
				ViewerControl.SetDisplayMode(m_printLayoutToolStripMenuItem.Checked ? DisplayMode.PrintLayout : DisplayMode.Normal);
			}
			catch (Exception e2)
			{
				ViewerControl.UpdateUIState(e2);
			}
		}

		private void OnPageSettingsClick(object sender, EventArgs e)
		{
			if (this.PageSettings != null)
			{
				this.PageSettings(this, e);
			}
		}

		private void OnExportClick(object sender, EventArgs e)
		{
			if (this.Export != null)
			{
				ReportExportEventArgs e2 = new ReportExportEventArgs((sender as ToolStripMenuItem).Tag as RenderingExtension);
				this.Export(this, e2);
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

		private void OnZoomClick(object sender, EventArgs e)
		{
			if (this.ZoomChange != null)
			{
				ZoomItem zoomItem = (sender as ToolStripMenuItem).Tag as ZoomItem;
				ZoomChangeEventArgs e2 = new ZoomChangeEventArgs(zoomItem.ZoomMode, zoomItem.ZoomPercent);
				this.ZoomChange(this, e2);
			}
		}

		private void OnContextMenuOpening(object sender, CancelEventArgs e)
		{
			e.Cancel = !m_showContextMenu;
		}

		private void OnContextMenuOpened(object sender, EventArgs e)
		{
			m_exportToolStripMenuItem.DropDownItems.Clear();
			if (m_exportToolStripMenuItem.Enabled && m_exportToolStripMenuItem.Visible)
			{
				try
				{
					RenderingExtension[] extensions = ViewerControl.Report.ListRenderingExtensions();
					EventHandler handler = OnExportClick;
					RenderingExtensionsHelper.Populate(m_exportToolStripMenuItem, handler, extensions);
				}
				catch
				{
				}
			}
		}

		private void OnContextMenuClosed(object sender, EventArgs e)
		{
			Select();
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			if (!m_inLongRunningAction && m_currentPage != null)
			{
				ScrollReport(e.Delta < 0);
				if (m_currentPage.IsRequireFullRedraw)
				{
					m_renderPanel.Invalidate();
				}
			}
		}

		private void OnMouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				if (m_showContextMenu)
				{
					Control control = sender as Control;
					m_contextMenu.Show(control, e.X, e.Y);
				}
			}
			else
			{
				Focus();
			}
		}

		private void InitializeContextMenu()
		{
			ApplyContextMenuCustomResources();
			m_documentMapToolStripMenuItem.Click += OnDocumentMapClick;
			m_backToolStripMenuItem.Click += OnBackClick;
			m_refreshToolStripMenuItem.Click += OnReportRefreshClick;
			m_printToolStripMenuItem.Click += OnPrintClick;
			m_printLayoutToolStripMenuItem.Click += OnPrintLayoutClick;
			m_stopToolStripMenuItem.Click += OnStopClick;
			m_pageSetupToolStripMenuItem.Click += OnPageSettingsClick;
		}

		private void ApplyContextMenuCustomResources()
		{
			string documentMapMenuItemText = LocalizationHelper.Current.DocumentMapMenuItemText;
			if (documentMapMenuItemText != null)
			{
				m_documentMapToolStripMenuItem.Text = documentMapMenuItemText;
			}
			documentMapMenuItemText = LocalizationHelper.Current.BackMenuItemText;
			if (documentMapMenuItemText != null)
			{
				m_backToolStripMenuItem.Text = documentMapMenuItemText;
			}
			documentMapMenuItemText = LocalizationHelper.Current.RefreshMenuItemText;
			if (documentMapMenuItemText != null)
			{
				m_refreshToolStripMenuItem.Text = documentMapMenuItemText;
			}
			documentMapMenuItemText = LocalizationHelper.Current.PrintMenuItemText;
			if (documentMapMenuItemText != null)
			{
				m_printToolStripMenuItem.Text = documentMapMenuItemText;
			}
			documentMapMenuItemText = LocalizationHelper.Current.PrintLayoutMenuItemText;
			if (documentMapMenuItemText != null)
			{
				m_printLayoutToolStripMenuItem.Text = documentMapMenuItemText;
			}
			documentMapMenuItemText = LocalizationHelper.Current.ExportMenuItemText;
			if (documentMapMenuItemText != null)
			{
				m_exportToolStripMenuItem.Text = documentMapMenuItemText;
			}
			documentMapMenuItemText = LocalizationHelper.Current.StopMenuItemText;
			if (documentMapMenuItemText != null)
			{
				m_stopToolStripMenuItem.Text = documentMapMenuItemText;
			}
			documentMapMenuItemText = LocalizationHelper.Current.ZoomMenuItemText;
			if (documentMapMenuItemText != null)
			{
				m_zoomToolStripMenuItem.Text = documentMapMenuItemText;
			}
			documentMapMenuItemText = LocalizationHelper.Current.PageSetupMenuItemText;
			if (documentMapMenuItemText != null)
			{
				m_pageSetupToolStripMenuItem.Text = documentMapMenuItemText;
			}
		}

		private void UpdateScrollBarIncrements()
		{
			if (m_currentPage == null)
			{
				base.AutoScrollMinSize = default(Size);
				return;
			}
			int width = m_renderPanel.Width;
			int height = m_renderPanel.Height;
			base.AutoScrollMinSize = new Size(width, height);
			base.HorizontalScroll.LargeChange = Math.Max(1, base.ClientRectangle.Width * 19 / 20);
			base.HorizontalScroll.SmallChange = Math.Max(1, base.ClientRectangle.Width / 20);
			base.VerticalScroll.LargeChange = Math.Max(1, base.ClientRectangle.Height * 19 / 20);
			base.VerticalScroll.SmallChange = Math.Max(1, base.ClientRectangle.Height / 20);
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			UpdateScrollBarIncrements();
			m_renderPanel.RecalculateSize();
			base.OnSizeChanged(e);
		}

		protected override void OnScroll(ScrollEventArgs se)
		{
			base.OnScroll(se);
			if (m_currentPage != null && m_currentPage.IsRequireFullRedraw)
			{
				m_renderPanel.Invalidate();
			}
		}

		private void OnRenderingPanelSizeChanged(object sender, EventArgs e)
		{
			UpdateScrollBarIncrements();
		}

		internal void SetToolStripRenderer(ToolStripRenderer renderer)
		{
			m_contextMenu.Renderer = renderer;
		}

		internal void ApplyCustomResources()
		{
			ApplyContextMenuCustomResources();
		}

		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			m_contextMenu = new System.Windows.Forms.ContextMenuStrip(components);
			m_documentMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			m_backToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			m_refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			m_printToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			m_printLayoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			m_pageSetupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			m_exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			m_stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			m_zoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			m_contextMenu.SuspendLayout();
			SuspendLayout();
			m_contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[9]
			{
				m_documentMapToolStripMenuItem,
				m_backToolStripMenuItem,
				m_refreshToolStripMenuItem,
				m_printToolStripMenuItem,
				m_printLayoutToolStripMenuItem,
				m_pageSetupToolStripMenuItem,
				m_exportToolStripMenuItem,
				m_stopToolStripMenuItem,
				m_zoomToolStripMenuItem
			});
			m_contextMenu.Name = "contextMenu";
			m_contextMenu.Size = new System.Drawing.Size(158, 202);
			m_contextMenu.Opened += new System.EventHandler(OnContextMenuOpened);
			m_contextMenu.Opening += new System.ComponentModel.CancelEventHandler(OnContextMenuOpening);
			m_contextMenu.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(OnContextMenuClosed);
			m_documentMapToolStripMenuItem.CheckOnClick = true;
			m_documentMapToolStripMenuItem.Name = "m_documentMapToolStripMenuItem";
			m_documentMapToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
			m_documentMapToolStripMenuItem.Text = "Document Map <replaced by resource>";
			m_backToolStripMenuItem.Name = "m_backToolStripMenuItem";
			m_backToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
			m_backToolStripMenuItem.Text = "Back <replaced by resource>";
			m_refreshToolStripMenuItem.Name = "m_refreshToolStripMenuItem";
			m_refreshToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
			m_refreshToolStripMenuItem.Text = "Refresh <replaced by resource>";
			m_printToolStripMenuItem.Name = "m_printToolStripMenuItem";
			m_printToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
			m_printToolStripMenuItem.Text = "Print <replaced by resource>";
			m_printLayoutToolStripMenuItem.CheckOnClick = true;
			m_printLayoutToolStripMenuItem.Name = "m_printLayoutToolStripMenuItem";
			m_printLayoutToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
			m_printLayoutToolStripMenuItem.Text = "Print Layout <replaced by resource>";
			m_pageSetupToolStripMenuItem.Name = "m_pageSetupToolStripMenuItem";
			m_pageSetupToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
			m_pageSetupToolStripMenuItem.Text = "Page Setup <replaced by resource>";
			m_exportToolStripMenuItem.Name = "m_exportToolStripMenuItem";
			m_exportToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
			m_exportToolStripMenuItem.Text = "Export <replaced by resource>";
			m_stopToolStripMenuItem.Name = "m_stopToolStripMenuItem";
			m_stopToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
			m_stopToolStripMenuItem.Text = "Stop <replaced by resource>";
			m_zoomToolStripMenuItem.Name = "m_zoomToolStripMenuItem";
			m_zoomToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
			m_zoomToolStripMenuItem.Text = "Zoom <replaced by resource>";
			ContextMenuStrip = m_contextMenu;
			m_contextMenu.ResumeLayout(false);
			ResumeLayout(false);
		}

		private ReportActions GetCurrentPageActions()
		{
			if (m_currentPage != null)
			{
				using (Graphics g = CreateGraphics())
				{
					m_currentPage.BuildInteractivityInfo(g);
				}
				return m_currentPage.Actions;
			}
			return null;
		}

		private int GetCurrentPageWidth()
		{
			if (m_currentPage != null)
			{
				float width;
				using (Graphics g = CreateGraphics())
				{
					m_currentPage.GetPageSize(g, out width, out float _);
				}
				return Convert.ToInt32(width);
			}
			return 0;
		}

		public void ProcessCurrentAction()
		{
			if (m_currentPage == null || m_currentPage.Actions.Count == 0)
			{
				return;
			}
			GdiPage gdiPage = m_currentPage as GdiPage;
			if (gdiPage != null && gdiPage.ActionIndex != -1)
			{
				Action currentAction = gdiPage.CurrentAction;
				if (currentAction != null)
				{
					bool shiftKeyDown = (Control.ModifierKeys & Keys.Shift) > Keys.None;
					ViewerControl.FireAnAction(currentAction, shiftKeyDown);
				}
			}
		}

		private void MoveToNextAction(bool reverse)
		{
			if (m_currentPage == null || m_currentPage.Actions.Count == 0)
			{
				return;
			}
			GdiPage gdiPage = m_currentPage as GdiPage;
			if (gdiPage == null)
			{
				return;
			}
			if (!reverse)
			{
				gdiPage.ActionIndex++;
				if (gdiPage.ActionIndex >= gdiPage.Actions.Count)
				{
					gdiPage.ActionIndex = 0;
				}
			}
			else
			{
				gdiPage.ActionIndex--;
				if (gdiPage.ActionIndex < 0)
				{
					gdiPage.ActionIndex = gdiPage.Actions.Count - 1;
				}
			}
			Action currentAction = gdiPage.CurrentAction;
			SetActionFocusPoint(currentAction.Type, currentAction.Id);
			Invalidate(invalidateChildren: true);
		}

		protected override bool IsInputKey(Keys keyData)
		{
			if ((keyData & Keys.Alt) == Keys.Alt)
			{
				return false;
			}
			Keys keys = keyData & Keys.KeyCode;
			if ((uint)(keys - 37) <= 3u)
			{
				return true;
			}
			return false;
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			try
			{
				if (m_inLongRunningAction && e.KeyCode != Keys.Escape)
				{
					return;
				}
				switch (e.KeyCode)
				{
				case Keys.Home:
					if (e.Control)
					{
						if (ViewerControl.CurrentPage == 1)
						{
							SetAutoScrollLocation(new Point(-base.AutoScrollPosition.X, 0));
						}
						else
						{
							OnPageNavigation(1, ActionType.None);
						}
					}
					else
					{
						SetAutoScrollLocation(new Point(0, -base.AutoScrollPosition.Y));
					}
					break;
				case Keys.End:
					if (e.Control)
					{
						PageCountMode pageCountMode;
						int totalPages = ViewerControl.GetTotalPages(out pageCountMode);
						if (ViewerControl.CurrentPage == totalPages && pageCountMode == PageCountMode.Actual)
						{
							SetAutoScrollLocation(new Point(-base.AutoScrollPosition.X, base.AutoScrollMinSize.Height));
							break;
						}
						int newPage = totalPages;
						if (pageCountMode != 0)
						{
							newPage = int.MaxValue;
						}
						OnPageNavigation(newPage, ActionType.ScrollToBottomOfPage);
					}
					else
					{
						SetAutoScrollLocation(new Point(base.AutoScrollMinSize.Width, -base.AutoScrollPosition.Y));
					}
					break;
				case Keys.Prior:
					if (e.Control)
					{
						if (IsExtentVisible(checkVerticalExtent: true, checkPositiveDirection: false))
						{
							OnPageNavigation(ViewerControl.CurrentPage - 1, ActionType.None);
						}
						else
						{
							SetAutoScrollLocation(new Point(0, 0));
						}
					}
					else
					{
						ScrollReportView(isVerticalScroll: true, scrollByViewSize: true, isPositiveMovement: false);
					}
					break;
				case Keys.Next:
					if (e.Control)
					{
						PageCountMode pageCountMode2;
						int totalPages2 = ViewerControl.GetTotalPages(out pageCountMode2);
						if (ViewerControl.CurrentPage < totalPages2 || pageCountMode2 != 0)
						{
							OnPageNavigation(ViewerControl.CurrentPage + 1, ActionType.None);
						}
						else
						{
							SetFocusPoint(new Point(-base.AutoScrollPosition.X, base.AutoScrollMinSize.Height), WinRSviewer.FocusMode.AlignToTop);
						}
					}
					else
					{
						ScrollReportView(isVerticalScroll: true, scrollByViewSize: true, isPositiveMovement: true);
					}
					break;
				case Keys.Up:
					if (e.Control)
					{
						MoveToNextAction(reverse: true);
					}
					else
					{
						ScrollReportView(isVerticalScroll: true, scrollByViewSize: false, isPositiveMovement: false);
					}
					break;
				case Keys.Down:
					if (e.Control)
					{
						MoveToNextAction(reverse: false);
					}
					else
					{
						ScrollReportView(isVerticalScroll: true, scrollByViewSize: false, isPositiveMovement: true);
					}
					break;
				case Keys.Left:
					if (e.Control)
					{
						MoveToNextAction(reverse: true);
					}
					else
					{
						ScrollReportView(isVerticalScroll: false, scrollByViewSize: false, isPositiveMovement: false);
					}
					break;
				case Keys.Right:
					if (e.Control)
					{
						MoveToNextAction(reverse: false);
					}
					else
					{
						ScrollReportView(isVerticalScroll: false, scrollByViewSize: false, isPositiveMovement: true);
					}
					break;
				case Keys.Back:
					OnBackClick(this, EventArgs.Empty);
					break;
				case Keys.Escape:
					ViewerControl.CancelRendering(0);
					break;
				case Keys.Return:
					ProcessCurrentAction();
					break;
				}
			}
			catch (Exception e2)
			{
				ViewerControl.UpdateUIState(e2);
			}
		}

		private void OnPageNavigation(int newPage, ActionType postRenderAction)
		{
			if (ViewerControl.CanMoveToPage(newPage) && this.PageNavigation != null)
			{
				PageNavigationEventArgs e = new PageNavigationEventArgs(newPage);
				this.PageNavigation(this, e, postRenderAction);
			}
		}

		private void ScrollReportView(bool isVerticalScroll, bool scrollByViewSize, bool isPositiveMovement)
		{
			if (!AutoScroll)
			{
				return;
			}
			if (IsExtentVisible(isVerticalScroll, isPositiveMovement))
			{
				if (isVerticalScroll)
				{
					if (isPositiveMovement)
					{
						OnPageNavigation(ViewerControl.CurrentPage + 1, ActionType.None);
					}
					else
					{
						OnPageNavigation(ViewerControl.CurrentPage - 1, ActionType.ScrollToBottomOfPage);
					}
				}
				return;
			}
			ScrollProperties scrollProperties = (!isVerticalScroll) ? ((ScrollProperties)base.HorizontalScroll) : ((ScrollProperties)base.VerticalScroll);
			int num = scrollByViewSize ? scrollProperties.LargeChange : scrollProperties.SmallChange;
			if (!isPositiveMovement)
			{
				num = -num;
			}
			Point autoScrollLocation = new Point(-base.AutoScrollPosition.X, -base.AutoScrollPosition.Y);
			if (isVerticalScroll)
			{
				autoScrollLocation.Y += num;
			}
			else
			{
				autoScrollLocation.X += num;
			}
			SetAutoScrollLocation(autoScrollLocation);
		}

		private Point MirrorPointForRTL(Point focusPoint)
		{
			if (RightToLeft == RightToLeft.Yes)
			{
				focusPoint.X = GetCurrentPageWidth() - focusPoint.X;
			}
			return focusPoint;
		}

		private void MaintainChildActionView(Point newPoint, Point origPoint)
		{
			Point point = MirrorPointForRTL(newPoint);
			Point point2 = origPoint;
			if (RightToLeft == RightToLeft.Yes)
			{
				point2.X = m_previousPageWidth - point2.X;
			}
			float zoomRate = GetZoomRate();
			int num = Convert.ToInt32((float)(point.X - point2.X) * zoomRate);
			int num2 = Convert.ToInt32((float)(point.Y - point2.Y) * zoomRate);
			int x = -base.AutoScrollPosition.X + num;
			int y = -base.AutoScrollPosition.Y + num2;
			SetAutoScrollLocation(new Point(x, y));
		}

		private void BringItemToView(Point focusPoint, WinRSviewer.FocusMode focusMode)
		{
			Point point = MirrorPointForRTL(focusPoint);
			float zoomRate = GetZoomRate();
			int x = 0;
			int y = 0;
			int num = Convert.ToInt32((float)point.X * zoomRate);
			int num2 = Convert.ToInt32((float)point.Y * zoomRate);
			bool flag = true;
			if (focusMode == WinRSviewer.FocusMode.AlignToTop)
			{
				x = num;
				y = num2;
			}
			else
			{
				bool flag2 = num >= -base.AutoScrollPosition.X && num <= -base.AutoScrollPosition.X + base.ClientRectangle.Width;
				bool flag3 = num2 >= -base.AutoScrollPosition.Y && num2 <= -base.AutoScrollPosition.Y + base.ClientRectangle.Height;
				if (focusMode == WinRSviewer.FocusMode.AvoidScrolling)
				{
					if (!flag2 || !flag3)
					{
						focusMode = WinRSviewer.FocusMode.Central;
					}
					else
					{
						flag = false;
					}
				}
				if (focusMode == WinRSviewer.FocusMode.Central)
				{
					if (!flag2)
					{
						if (num > base.Width / 2)
						{
							x = num - base.Width / 2;
						}
					}
					else
					{
						x = -base.AutoScrollPosition.X;
					}
					if (!flag3)
					{
						if (num2 > base.Height / 3)
						{
							y = num2 - base.Height / 3;
						}
					}
					else
					{
						y = -base.AutoScrollPosition.Y;
					}
				}
			}
			if (flag)
			{
				SetAutoScrollLocation(new Point(x, y));
			}
		}

		public void SetAutoScrollLocation(Point point)
		{
			base.AutoScrollPosition = new Point(point.X, point.Y);
			m_renderPanel.Invalidate();
		}

		public void SetFocusPoint(Point focusPoint, WinRSviewer.FocusMode focusMode)
		{
			if (m_currentPage != null)
			{
				BringItemToView(focusPoint, focusMode);
			}
		}

		public void SetFocusPointMm(PointF focusPoint, WinRSviewer.FocusMode focusMode)
		{
			if (m_currentPage != null)
			{
				using (Graphics graphics = CreateGraphics())
				{
					BringItemToView(new Point(Global.ToPixels(focusPoint.X, graphics.DpiX), Global.ToPixels(focusPoint.Y, graphics.DpiY)), focusMode);
				}
			}
		}

		private bool IsExtentVisible(bool checkVerticalExtent, bool checkPositiveDirection)
		{
			int num = checkVerticalExtent ? base.AutoScrollPosition.Y : base.AutoScrollPosition.X;
			if (checkPositiveDirection)
			{
				int num2 = (!checkVerticalExtent) ? (base.ClientRectangle.Width - base.AutoScrollMinSize.Width) : (base.ClientRectangle.Height - base.AutoScrollMinSize.Height);
				return num <= num2;
			}
			return num == 0;
		}

		public void SetBookmarkFocusPoint(string bookmarkId)
		{
			if (m_currentPage != null)
			{
				using (Graphics g = CreateGraphics())
				{
					m_currentPage.BuildInteractivityInfo(g);
				}
				if (m_currentPage.Bookmarks.TryGetValue(bookmarkId, out BookmarkPoint value))
				{
					SetFocusPoint(value.Point, WinRSviewer.FocusMode.AlignToTop);
				}
			}
		}

		public void SetActionFocusPoint(ActionType actionType, string actionId)
		{
			if (m_currentPage == null)
			{
				return;
			}
			if (actionType == ActionType.ScrollToBottomOfPage)
			{
				SetAutoScrollLocation(new Point(0, base.AutoScrollMinSize.Height));
				return;
			}
			ReportActions currentPageActions = GetCurrentPageActions();
			if (currentPageActions == null)
			{
				return;
			}
			Point actionPoint = currentPageActions.GetActionPoint(actionType, actionId);
			if (actionType == ActionType.Sort || actionType == ActionType.Toggle)
			{
				if (m_previousChildActionPoints.ContainsKey(actionId))
				{
					MaintainChildActionView(actionPoint, m_previousChildActionPoints[actionId]);
				}
			}
			else
			{
				SetFocusPoint(actionPoint, WinRSviewer.FocusMode.AvoidScrolling);
			}
		}

		public float GetZoomRate()
		{
			float num;
			if (ViewerControl.ZoomMode == ZoomMode.Percent)
			{
				num = (float)ViewerControl.ZoomPercent / 100f;
			}
			else
			{
				if (m_currentPage == null)
				{
					return 1f;
				}
				float width;
				float height;
				using (Graphics g = CreateGraphics())
				{
					m_currentPage.GetPageSize(g, out width, out height);
				}
				if (0f == width || 0f == height)
				{
					return 1f;
				}
				float num2 = (float)base.ClientSize.Width / width;
				if (ViewerControl.ZoomMode == ZoomMode.PageWidth)
				{
					num = num2;
				}
				else
				{
					float val = (float)base.ClientSize.Height / height;
					num = Math.Min(num2, val);
				}
				if (num < 0.01f)
				{
					num = 0.01f;
				}
			}
			return num;
		}

		public void ScrollReport(bool positiveDirection)
		{
			try
			{
				ScrollReportView(isVerticalScroll: true, scrollByViewSize: false, positiveDirection);
			}
			catch (Exception e)
			{
				ViewerControl.UpdateUIState(e);
			}
		}

		public void SetZoom()
		{
			if (ViewerControl != null)
			{
				ZoomMenuHelper.Populate(m_zoomToolStripMenuItem, OnZoomClick, ViewerControl.ZoomMode, ViewerControl.ZoomPercent);
				m_renderPanel.RecalculateSize();
			}
		}

		private void OnReportViewerStateChanged(object sender, EventArgs e)
		{
			ReportViewer reportViewer = (ReportViewer)sender;
			ReportViewerStatus currentStatus = reportViewer.CurrentStatus;
			m_inLongRunningAction = !currentStatus.CanInteractWithReportPage;
			m_documentMapToolStripMenuItem.Enabled = currentStatus.HasDocumentMapToDisplay;
			m_documentMapToolStripMenuItem.Checked = currentStatus.IsDocumentMapVisible;
			m_backToolStripMenuItem.Enabled = currentStatus.CanNavigateBack;
			m_refreshToolStripMenuItem.Enabled = currentStatus.CanRefreshData;
			m_printToolStripMenuItem.Enabled = currentStatus.CanPrint;
			m_printLayoutToolStripMenuItem.Enabled = currentStatus.CanChangeDisplayModes;
			m_printLayoutToolStripMenuItem.Checked = (reportViewer.DisplayMode == DisplayMode.PrintLayout);
			m_pageSetupToolStripMenuItem.Enabled = m_printToolStripMenuItem.Enabled;
			m_exportToolStripMenuItem.Enabled = currentStatus.CanExport;
			m_stopToolStripMenuItem.Enabled = currentStatus.InCancelableOperation;
			m_zoomToolStripMenuItem.Enabled = currentStatus.CanChangeZoom;
			m_documentMapToolStripMenuItem.Visible = reportViewer.ShowDocumentMapButton;
			m_backToolStripMenuItem.Visible = reportViewer.ShowBackButton;
			m_refreshToolStripMenuItem.Visible = reportViewer.ShowRefreshButton;
			m_printToolStripMenuItem.Visible = reportViewer.ShowPrintButton;
			m_printLayoutToolStripMenuItem.Visible = reportViewer.ShowPrintButton;
			m_pageSetupToolStripMenuItem.Visible = (m_printToolStripMenuItem.Enabled || m_printLayoutToolStripMenuItem.Enabled);
			m_exportToolStripMenuItem.Visible = reportViewer.ShowExportButton;
			m_stopToolStripMenuItem.Visible = reportViewer.ShowStopButton;
			m_zoomToolStripMenuItem.Visible = reportViewer.ShowZoomControl;
		}

		public void RenderToGraphics(Graphics g, bool testMode)
		{
			m_renderPanel.RenderToGraphics(g, testMode);
		}

		protected override AccessibleObject CreateAccessibilityInstance()
		{
			return new ReportPanelAccessibleObject(this);
		}
	}
}
