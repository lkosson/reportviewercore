using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Security.Permissions;
using System.Windows.Forms;

namespace Microsoft.Reporting.WinForms
{
	internal class RVSplitContainer : UserControl
	{
		private enum ImageTypes
		{
			VerticalCollapsed,
			VerticalCollapsedHover,
			VerticalExpanded,
			VerticalExpandedHover,
			HorizontalCollapsed,
			HorizontalCollapsedHover,
			HorizontalExpanded,
			HorizontalExpandedHover
		}

		internal class SplitterButton : Button
		{
			private class SplitterButtonAccessibleObject : ButtonBaseAccessibleObject
			{
				private ButtonAreaAccessibleObject m_buttonAccessibleObject;

				public override AccessibleRole Role => AccessibleRole.Separator;

				public SplitterButtonAccessibleObject(SplitterButton owner)
					: base(owner)
				{
				}

				public override int GetChildCount()
				{
					return 1;
				}

				public override AccessibleObject GetChild(int index)
				{
					if (index != 0)
					{
						return null;
					}
					if (m_buttonAccessibleObject == null)
					{
						m_buttonAccessibleObject = new ButtonAreaAccessibleObject((SplitterButton)base.Owner, this);
					}
					return m_buttonAccessibleObject;
				}
			}

			private class ButtonAreaAccessibleObject : AccessibleObject
			{
				private SplitterButton m_owner;

				private AccessibleObject m_parentAccessibleObject;

				public override Rectangle Bounds
				{
					get
					{
						if (!m_owner.SplitContainer.ButtonRectangle.IsEmpty)
						{
							return m_owner.RectangleToScreen(m_owner.SplitContainer.ButtonRectangle);
						}
						return Rectangle.Empty;
					}
				}

				public override AccessibleRole Role => AccessibleRole.Indicator;

				public override AccessibleObject Parent => m_parentAccessibleObject;

				public override string Name
				{
					get
					{
						return m_owner.ButtonAreaAccessibleName;
					}
					set
					{
					}
				}

				public ButtonAreaAccessibleObject(SplitterButton owner, AccessibleObject parentAccessibleObject)
				{
					m_owner = owner;
					m_parentAccessibleObject = parentAccessibleObject;
				}
			}

			private Point m_mouseClick = Point.Empty;

			private RVSplitContainer m_SplitContainer;

			private string m_buttonAreaAccessibleName;

			internal RVSplitContainer SplitContainer
			{
				get
				{
					return m_SplitContainer;
				}
				set
				{
					m_SplitContainer = value;
				}
			}

			public string ButtonAreaAccessibleName
			{
				get
				{
					return m_buttonAreaAccessibleName;
				}
				set
				{
					m_buttonAreaAccessibleName = value;
				}
			}

			public SplitterButton()
			{
				Dock = DockStyle.Fill;
			}

			protected override void OnPaint(PaintEventArgs e)
			{
				bool style = GetStyle(ControlStyles.UserPaint);
				SetStyle(ControlStyles.UserPaint, value: false);
				using (Brush brush = new SolidBrush(BackColor))
				{
					e.Graphics.FillRectangle(brush, e.ClipRectangle);
				}
				base.OnPaint(e);
				if (Focused && ShowFocusCues)
				{
					ControlPaint.DrawFocusRectangle(e.Graphics, e.ClipRectangle);
				}
				SetStyle(ControlStyles.UserPaint, style);
			}

			[UIPermission(SecurityAction.LinkDemand, Window = UIPermissionWindow.AllWindows)]
			protected override bool ProcessDialogKey(Keys keyData)
			{
				if ((keyData & (Keys.Control | Keys.Alt)) == 0 && Focused && ShowFocusCues)
				{
					Keys keys = keyData & Keys.KeyCode;
					if ((uint)(keys - 37) <= 3u)
					{
						return false;
					}
				}
				return base.ProcessDialogKey(keyData);
			}

			protected override void OnClick(EventArgs e)
			{
				if (m_mouseClick.IsEmpty)
				{
					base.OnClick(e);
				}
				else if (SplitContainer.CanClickSplitterButton(m_mouseClick))
				{
					base.OnClick(e);
				}
			}

			protected override void OnMouseUp(MouseEventArgs mevent)
			{
				m_mouseClick = mevent.Location;
				try
				{
					base.OnMouseUp(mevent);
				}
				finally
				{
					m_mouseClick = Point.Empty;
				}
			}

			protected override AccessibleObject CreateAccessibilityInstance()
			{
				return new SplitterButtonAccessibleObject(this);
			}
		}

		private bool m_collapsed;

		private bool m_fixed;

		private bool m_tooltipSet;

		private bool m_trackingMouse;

		private bool m_mouseOverCollapseRegion;

		private Color m_splitterHoverColor = Color.FromKnownColor(KnownColor.ControlDark);

		private Color m_splitterNormalColor = Color.FromKnownColor(KnownColor.ControlLight);

		private Orientation m_orienation = Orientation.Vertical;

		private bool m_panel1Visible = true;

		private bool m_splitterVisble = true;

		private int m_splitterDistance = 100;

		private bool m_isSplitterFixed;

		private bool m_canCollapse = true;

		private static Bitmap[] m_bitmaps;

		private int m_panel1MinSize;

		private int m_panel2MinSize;

		private string m_toolTip = string.Empty;

		private IContainer components;

		private TableLayoutPanel tableLayoutPanel1;

		private Panel panel1;

		private Panel panel2;

		private SplitterButton splitter;

		private ToolTip toolTip1;

		public string ToolTip
		{
			get
			{
				return m_toolTip;
			}
			set
			{
				m_toolTip = value;
			}
		}

		public int Panel1MinSize
		{
			get
			{
				return m_panel1MinSize;
			}
			set
			{
				m_panel1MinSize = value;
				SplitterDistance = m_splitterDistance;
			}
		}

		public int Panel2MinSize
		{
			get
			{
				return m_panel2MinSize;
			}
			set
			{
				m_panel2MinSize = value;
				SplitterDistance = m_splitterDistance;
			}
		}

		public int SplitterDistance
		{
			get
			{
				return m_splitterDistance;
			}
			set
			{
				if (Orientation == Orientation.Vertical)
				{
					m_splitterDistance = Math.Max(Math.Min(value, base.Width - Panel2MinSize), Panel1MinSize);
					m_splitterDistance = Math.Max(0, Math.Min(base.Width - SplitterWidth, m_splitterDistance));
				}
				else
				{
					m_splitterDistance = Math.Max(Math.Min(value, base.Height - Panel2MinSize), Panel1MinSize);
					m_splitterDistance = Math.Max(0, Math.Min(base.Height - SplitterWidth, m_splitterDistance));
				}
				AdjustSplitterDistance(m_splitterDistance);
			}
		}

		public bool Panel1Visible
		{
			get
			{
				return m_panel1Visible;
			}
			set
			{
				if (m_panel1Visible != value)
				{
					m_panel1Visible = value;
					EnsureVisibility();
				}
			}
		}

		public bool SplitterVisible
		{
			get
			{
				return m_splitterVisble;
			}
			set
			{
				if (m_splitterVisble != value)
				{
					m_splitterVisble = value;
					EnsureVisibility();
				}
			}
		}

		public bool FixedSize
		{
			get
			{
				return m_fixed;
			}
			set
			{
				if (m_fixed != value)
				{
					m_fixed = value;
					EnsureFixedSize();
				}
			}
		}

		public bool Collapsed
		{
			get
			{
				return m_collapsed;
			}
			set
			{
				if (m_collapsed != value)
				{
					m_collapsed = value;
					PerformCollapse();
					OnCollapsedChanged(EventArgs.Empty);
					splitter.Invalidate();
				}
			}
		}

		public bool IsSplitterFixed
		{
			get
			{
				return m_isSplitterFixed;
			}
			set
			{
				if (m_isSplitterFixed != value)
				{
					m_isSplitterFixed = value;
					EnsureVisibility();
				}
			}
		}

		public bool CanCollapse
		{
			get
			{
				return m_canCollapse;
			}
			set
			{
				if (m_canCollapse != value)
				{
					m_canCollapse = value;
					EnsureVisibility();
				}
			}
		}

		public Orientation Orientation
		{
			get
			{
				return m_orienation;
			}
			set
			{
				if (m_orienation != value)
				{
					m_orienation = value;
					EnsureOrientation();
				}
			}
		}

		public int SplitterWidth
		{
			get
			{
				if (!Panel1Visible)
				{
					return 0;
				}
				if (!SplitterVisible)
				{
					return 2;
				}
				if (Orientation == Orientation.Vertical)
				{
					return ButtonBitmap.Width + 2;
				}
				return ButtonBitmap.Height + 2;
			}
		}

		public Color SplitterNormalColor
		{
			get
			{
				return m_splitterNormalColor;
			}
			set
			{
				m_splitterNormalColor = value;
				splitter.BackColor = SplitterNormalColor;
			}
		}

		public Color SplitterHoverColor
		{
			get
			{
				return m_splitterHoverColor;
			}
			set
			{
				m_splitterHoverColor = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Panel Panel1 => panel1;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Panel Panel2 => panel2;

		internal SplitterButton Splitter => splitter;

		private SizeType CurrentSizeType
		{
			get
			{
				if (Orientation == Orientation.Vertical)
				{
					return tableLayoutPanel1.ColumnStyles[0].SizeType;
				}
				return tableLayoutPanel1.RowStyles[0].SizeType;
			}
		}

		private Rectangle ButtonRectangle
		{
			get
			{
				if (CanCollapse && SplitterVisible)
				{
					Rectangle clientRectangle = splitter.ClientRectangle;
					Bitmap buttonBitmap = ButtonBitmap;
					clientRectangle.Offset(clientRectangle.Width / 2 - buttonBitmap.Width / 2, clientRectangle.Height / 2 - buttonBitmap.Height / 2);
					clientRectangle.Width = buttonBitmap.Width;
					clientRectangle.Height = buttonBitmap.Height;
					clientRectangle.Inflate(1, 1);
					return clientRectangle;
				}
				return Rectangle.Empty;
			}
		}

		private Bitmap ButtonBitmap
		{
			get
			{
				int num = (Orientation != Orientation.Vertical) ? (Collapsed ? 4 : 6) : ((RightToLeft != RightToLeft.Yes) ? ((!Collapsed) ? 2 : 0) : (Collapsed ? 2 : 0));
				if (m_mouseOverCollapseRegion)
				{
					num++;
				}
				return m_bitmaps[num];
			}
		}

		public event EventHandler SplitterMoving;

		public event EventHandler CollapsedChanged;

		private void PerformCollapse()
		{
			if (!Panel1Visible)
			{
				Panel1.Visible = false;
				Splitter.Visible = false;
				AdjustSplitterDistance(0);
			}
			else if (Collapsed)
			{
				Panel1.Visible = false;
				Splitter.Visible = true;
				AdjustSplitterDistance(0);
			}
			else
			{
				Panel1.Visible = true;
				Splitter.Visible = true;
				AdjustSplitterDistance(m_splitterDistance);
				EnsureFixedSize();
			}
		}

		public RVSplitContainer()
		{
			InitializeComponent();
			splitter.SplitContainer = this;
			splitter.BackColor = SplitterNormalColor;
			EnsureOrientation();
			EnsureFixedSize();
			EnsureVisibility();
		}

		static RVSplitContainer()
		{
			m_bitmaps = new Bitmap[Enum.GetValues(typeof(ImageTypes)).Length];
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			string str = typeof(RVSplitContainer).Namespace + ".Resources.";
			m_bitmaps[4] = new Bitmap(executingAssembly.GetManifestResourceStream(str + "SplitterHorizExpand.png"));
			m_bitmaps[5] = new Bitmap(executingAssembly.GetManifestResourceStream(str + "SplitterHorizExpandHover.png"));
			m_bitmaps[6] = new Bitmap(executingAssembly.GetManifestResourceStream(str + "SplitterHorizCollapse.png"));
			m_bitmaps[7] = new Bitmap(executingAssembly.GetManifestResourceStream(str + "SplitterHorizCollapseHover.png"));
			m_bitmaps[0] = new Bitmap(executingAssembly.GetManifestResourceStream(str + "SplitterVertExpand.png"));
			m_bitmaps[1] = new Bitmap(executingAssembly.GetManifestResourceStream(str + "SplitterVertExpandHover.png"));
			m_bitmaps[2] = new Bitmap(executingAssembly.GetManifestResourceStream(str + "SplitterVertCollapse.png"));
			m_bitmaps[3] = new Bitmap(executingAssembly.GetManifestResourceStream(str + "SplitterVertCollapseHover.png"));
		}

		private void splitter_Paint(object sender, PaintEventArgs e)
		{
			if (!ButtonRectangle.IsEmpty)
			{
				Rectangle buttonRectangle = ButtonRectangle;
				buttonRectangle.Inflate(-1, -1);
				e.Graphics.DrawImage(ButtonBitmap, buttonRectangle);
			}
		}

		private void splitter_Click(object sender, EventArgs e)
		{
			if (CanCollapse && SplitterVisible)
			{
				Collapsed = !Collapsed;
			}
		}

		private bool IsMouseOverExpandCollapseRegion(Point mouseLocation)
		{
			if (!ButtonRectangle.Contains(mouseLocation) && !Collapsed)
			{
				return IsSplitterFixed;
			}
			return true;
		}

		private bool CanClickSplitterButton(Point mouseLocation)
		{
			if (IsMouseOverExpandCollapseRegion(mouseLocation))
			{
				return !m_trackingMouse;
			}
			return false;
		}

		private void splitter_KeyDown(object sender, KeyEventArgs e)
		{
			if ((e.KeyCode & (Keys.Control | Keys.Alt)) == 0 && !IsSplitterFixed)
			{
				int num = 0;
				if (e.KeyCode == (Keys)((Orientation == Orientation.Vertical) ? 37 : 38))
				{
					num = -1;
				}
				else if (e.KeyCode == (Keys)((Orientation == Orientation.Vertical) ? 39 : 40))
				{
					num = 1;
				}
				if (Orientation == Orientation.Vertical && RightToLeft == RightToLeft.Yes)
				{
					num = -num;
				}
				MoveSplitterBy(num);
			}
		}

		private void splitter_MouseLeave(object sender, EventArgs e)
		{
			if (SplitterVisible)
			{
				splitter.BackColor = SplitterNormalColor;
				m_mouseOverCollapseRegion = false;
			}
		}

		private void splitter_MouseDown(object sender, MouseEventArgs e)
		{
			if (SplitterVisible && !ButtonRectangle.Contains(e.Location) && !Collapsed && !IsSplitterFixed)
			{
				m_trackingMouse = true;
			}
		}

		private void splitter_MouseUp(object sender, MouseEventArgs e)
		{
			if (m_trackingMouse)
			{
				EnsureFixedSize();
			}
			m_trackingMouse = false;
		}

		private void splitter_MouseMove(object sender, MouseEventArgs e)
		{
			if (!SplitterVisible)
			{
				splitter.Cursor = Cursor;
				return;
			}
			if (IsMouseOverExpandCollapseRegion(e.Location))
			{
				m_mouseOverCollapseRegion = true;
				splitter.Cursor = Cursors.Hand;
				splitter.BackColor = SplitterHoverColor;
				if (!m_tooltipSet)
				{
					toolTip1.SetToolTip(splitter, ToolTip);
					m_tooltipSet = true;
				}
				return;
			}
			m_mouseOverCollapseRegion = false;
			toolTip1.SetToolTip(splitter, string.Empty);
			m_tooltipSet = false;
			splitter.Cursor = ((Orientation == Orientation.Vertical) ? Cursors.VSplit : Cursors.HSplit);
			if (!m_trackingMouse)
			{
				return;
			}
			Point mousePosition = Control.MousePosition;
			Point point = PointToScreen(base.Location);
			int num;
			if (Orientation == Orientation.Vertical)
			{
				num = mousePosition.X - point.X;
				if (RightToLeft == RightToLeft.Yes)
				{
					num = base.Width - num;
				}
			}
			else
			{
				num = mousePosition.Y - point.Y;
			}
			MoveSplitterTo(num);
		}

		private void MoveSplitterBy(int delta)
		{
			if (delta != 0)
			{
				MoveSplitterTo(m_splitterDistance + delta);
			}
		}

		private void MoveSplitterTo(int newSplitterLocation)
		{
			int val = (Orientation != Orientation.Vertical) ? (base.Height - splitter.Height) : (base.Width - splitter.Width);
			newSplitterLocation = Math.Max(0, newSplitterLocation);
			newSplitterLocation = Math.Min(val, newSplitterLocation);
			AdjustSplitterDistance(newSplitterLocation);
			m_splitterDistance = newSplitterLocation;
			OnSplitterMoving();
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			if (base.Height > 0 && base.Width > 0)
			{
				SplitterDistance = m_splitterDistance;
			}
		}

		protected override void OnRightToLeftChanged(EventArgs e)
		{
			base.OnRightToLeftChanged(e);
			splitter.Invalidate();
		}

		private void AdjustSplitterDistance(int value)
		{
			if (Orientation == Orientation.Vertical)
			{
				tableLayoutPanel1.ColumnStyles[0] = new ColumnStyle(SizeType.Absolute, (!Collapsed && Panel1Visible) ? value : 0);
			}
			else
			{
				tableLayoutPanel1.RowStyles[0] = new RowStyle(SizeType.Absolute, (!Collapsed && Panel1Visible) ? value : 0);
			}
		}

		private void EnsureVisibility()
		{
			bool flag = m_panel1Visible && (CanCollapse || !IsSplitterFixed);
			if (Orientation == Orientation.Vertical)
			{
				tableLayoutPanel1.ColumnStyles[1].Width = (flag ? SplitterWidth : 0);
			}
			else
			{
				tableLayoutPanel1.RowStyles[1].Height = (flag ? SplitterWidth : 0);
			}
			PerformCollapse();
			splitter.Invalidate();
		}

		private void EnsureOrientation()
		{
			if (Orientation == Orientation.Vertical && tableLayoutPanel1.ColumnStyles.Count < 3)
			{
				tableLayoutPanel1.SuspendLayout();
				tableLayoutPanel1.RowStyles.Clear();
				tableLayoutPanel1.ColumnStyles.Clear();
				tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
				tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, (Panel1Visible && !Collapsed) ? SplitterDistance : 0));
				tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, Panel1Visible ? SplitterWidth : 0));
				tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
				tableLayoutPanel1.SetCellPosition(panel1, new TableLayoutPanelCellPosition(0, 0));
				tableLayoutPanel1.SetCellPosition(splitter, new TableLayoutPanelCellPosition(1, 0));
				tableLayoutPanel1.SetCellPosition(panel2, new TableLayoutPanelCellPosition(2, 0));
				EnsureFixedSize();
				tableLayoutPanel1.ResumeLayout();
			}
			else if (Orientation == Orientation.Horizontal && tableLayoutPanel1.RowStyles.Count < 3)
			{
				tableLayoutPanel1.SuspendLayout();
				tableLayoutPanel1.RowStyles.Clear();
				tableLayoutPanel1.ColumnStyles.Clear();
				tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
				tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, (Panel1Visible && !Collapsed) ? SplitterDistance : 0));
				tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, Panel1Visible ? SplitterWidth : 0));
				tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
				tableLayoutPanel1.SetCellPosition(panel1, new TableLayoutPanelCellPosition(0, 0));
				tableLayoutPanel1.SetCellPosition(splitter, new TableLayoutPanelCellPosition(0, 1));
				tableLayoutPanel1.SetCellPosition(panel2, new TableLayoutPanelCellPosition(0, 2));
				EnsureFixedSize();
				tableLayoutPanel1.ResumeLayout();
			}
		}

		private void EnsureFixedSize()
		{
			if (FixedSize && CurrentSizeType == SizeType.Percent)
			{
				if (Orientation == Orientation.Vertical)
				{
					tableLayoutPanel1.ColumnStyles[0] = new ColumnStyle(SizeType.Absolute, panel1.Size.Width);
				}
				else
				{
					tableLayoutPanel1.RowStyles[0] = new RowStyle(SizeType.Absolute, panel1.Size.Height);
				}
			}
			else if (!FixedSize && CurrentSizeType == SizeType.Absolute)
			{
				tableLayoutPanel1.SuspendLayout();
				int num = (Orientation == Orientation.Vertical) ? panel1.Size.Width : panel1.Size.Height;
				int num2 = (Orientation == Orientation.Vertical) ? panel2.Size.Width : panel2.Size.Height;
				float num3 = (float)num / (float)(num + num2) * 100f;
				if (Orientation == Orientation.Vertical)
				{
					tableLayoutPanel1.ColumnStyles[0] = new ColumnStyle(SizeType.Percent, num3);
					tableLayoutPanel1.ColumnStyles[2] = new ColumnStyle(SizeType.Percent, 100f - num3);
				}
				else
				{
					tableLayoutPanel1.RowStyles[0] = new RowStyle(SizeType.Percent, num3);
					tableLayoutPanel1.RowStyles[2] = new RowStyle(SizeType.Percent, 100f - num3);
				}
				tableLayoutPanel1.ResumeLayout();
			}
		}

		public void OnSplitterMoving()
		{
			if (this.SplitterMoving != null)
			{
				this.SplitterMoving(this, EventArgs.Empty);
			}
		}

		public void OnCollapsedChanged(EventArgs e)
		{
			if (this.CollapsedChanged != null)
			{
				this.CollapsedChanged(this, e);
			}
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
			components = new System.ComponentModel.Container();
			tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			panel1 = new System.Windows.Forms.Panel();
			panel2 = new System.Windows.Forms.Panel();
			splitter = new Microsoft.Reporting.WinForms.RVSplitContainer.SplitterButton();
			toolTip1 = new System.Windows.Forms.ToolTip(components);
			tableLayoutPanel1.SuspendLayout();
			SuspendLayout();
			tableLayoutPanel1.ColumnCount = 3;
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 107f));
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 5f));
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			tableLayoutPanel1.Controls.Add(panel1, 0, 0);
			tableLayoutPanel1.Controls.Add(splitter, 1, 0);
			tableLayoutPanel1.Controls.Add(panel2, 2, 0);
			tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(6);
			tableLayoutPanel1.Name = "tableLayoutPanel1";
			tableLayoutPanel1.RowCount = 1;
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100f));
			tableLayoutPanel1.Size = new System.Drawing.Size(647, 441);
			tableLayoutPanel1.TabIndex = 0;
			panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			panel1.Location = new System.Drawing.Point(0, 0);
			panel1.Margin = new System.Windows.Forms.Padding(0);
			panel1.Name = "panel1";
			panel1.Size = new System.Drawing.Size(107, 441);
			panel1.TabIndex = 0;
			panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			panel2.Location = new System.Drawing.Point(112, 0);
			panel2.Margin = new System.Windows.Forms.Padding(0);
			panel2.Name = "panel2";
			panel2.Size = new System.Drawing.Size(535, 441);
			panel2.TabIndex = 1;
			splitter.Dock = System.Windows.Forms.DockStyle.Fill;
			splitter.Location = new System.Drawing.Point(107, 0);
			splitter.Margin = new System.Windows.Forms.Padding(0);
			splitter.Name = "splitter";
			splitter.Size = new System.Drawing.Size(5, 441);
			splitter.MouseLeave += new System.EventHandler(splitter_MouseLeave);
			splitter.Paint += new System.Windows.Forms.PaintEventHandler(splitter_Paint);
			splitter.MouseMove += new System.Windows.Forms.MouseEventHandler(splitter_MouseMove);
			splitter.MouseDown += new System.Windows.Forms.MouseEventHandler(splitter_MouseDown);
			splitter.MouseUp += new System.Windows.Forms.MouseEventHandler(splitter_MouseUp);
			splitter.KeyDown += new System.Windows.Forms.KeyEventHandler(splitter_KeyDown);
			splitter.Click += new System.EventHandler(splitter_Click);
			base.Controls.Add(tableLayoutPanel1);
			base.Name = "RVSplitContainer";
			base.Size = new System.Drawing.Size(647, 441);
			tableLayoutPanel1.ResumeLayout(false);
			ResumeLayout(false);
		}
	}
}
