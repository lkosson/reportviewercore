using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Microsoft.Reporting.WinForms
{
	internal class AsyncWaitMessage : UserControl
	{
		private const string SpinningWheelResourceName = "Microsoft.Reporting.WinForms.Resources.SpinningWheel.gif";

		private Stream m_imageStream;

		private IContainer components;

		private TableLayoutPanel tableLayoutPanel1;

		private Panel SpinningWheelPanel;

		private PictureBox PictSpinningWheel;

		private Label LblLoading;

		public override Font Font
		{
			get
			{
				return LblLoading.Font;
			}
			set
			{
				LblLoading.Font = value;
			}
		}

		public AsyncWaitMessage()
		{
			InitializeComponent();
			PictSpinningWheel.SizeMode = PictureBoxSizeMode.StretchImage;
		}

		~AsyncWaitMessage()
		{
			if (m_imageStream != null)
			{
				m_imageStream.Close();
			}
		}

		public void CenterToParent()
		{
			if (base.Parent != null)
			{
				base.Left = base.Parent.Width / 2 - base.Width / 2;
				base.Top = base.Parent.Height / 2 - base.Height / 2;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			if (!base.DesignMode)
			{
				ApplyCustomResources();
				m_imageStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Microsoft.Reporting.WinForms.Resources.SpinningWheel.gif");
				Image image = new Bitmap(m_imageStream);
				PictSpinningWheel.Image = image;
			}
			base.OnLoad(e);
		}

		internal void ApplyCustomResources()
		{
			string progressText = LocalizationHelper.Current.ProgressText;
			if (progressText != null)
			{
				LblLoading.Text = progressText;
			}
		}

		private void CenterSpinningWheel()
		{
			int top = SpinningWheelPanel.Height / 2 - PictSpinningWheel.Height / 2;
			PictSpinningWheel.Top = top;
			int left = SpinningWheelPanel.Width / 2 - PictSpinningWheel.Width / 2;
			PictSpinningWheel.Left = left;
		}

		protected override void OnLayout(LayoutEventArgs levent)
		{
			CenterSpinningWheel();
			base.OnLayout(levent);
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
			tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			SpinningWheelPanel = new System.Windows.Forms.Panel();
			PictSpinningWheel = new System.Windows.Forms.PictureBox();
			LblLoading = new System.Windows.Forms.Label();
			tableLayoutPanel1.SuspendLayout();
			SpinningWheelPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)PictSpinningWheel).BeginInit();
			SuspendLayout();
			tableLayoutPanel1.AutoSize = true;
			tableLayoutPanel1.ColumnCount = 2;
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 52f));
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100f));
			tableLayoutPanel1.Controls.Add(SpinningWheelPanel, 0, 0);
			tableLayoutPanel1.Controls.Add(LblLoading, 1, 0);
			tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			tableLayoutPanel1.Name = "tableLayoutPanel1";
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20f));
			tableLayoutPanel1.Size = new System.Drawing.Size(241, 134);
			tableLayoutPanel1.TabIndex = 0;
			SpinningWheelPanel.Controls.Add(PictSpinningWheel);
			SpinningWheelPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			SpinningWheelPanel.Location = new System.Drawing.Point(3, 3);
			SpinningWheelPanel.Name = "SpinningWheelPanel";
			SpinningWheelPanel.Size = new System.Drawing.Size(46, 128);
			SpinningWheelPanel.TabIndex = 1;
			PictSpinningWheel.Location = new System.Drawing.Point(3, 49);
			PictSpinningWheel.Name = "PictSpinningWheel";
			PictSpinningWheel.Size = new System.Drawing.Size(32, 32);
			PictSpinningWheel.TabIndex = 0;
			PictSpinningWheel.TabStop = false;
			LblLoading.AutoSize = true;
			LblLoading.Dock = System.Windows.Forms.DockStyle.Fill;
			LblLoading.Font = new System.Drawing.Font("Microsoft Sans Serif", 14f);
			LblLoading.Location = new System.Drawing.Point(55, 0);
			LblLoading.Name = "LblLoading";
			LblLoading.Size = new System.Drawing.Size(183, 134);
			LblLoading.TabIndex = 2;
			LblLoading.Text = "Loading...";
			LblLoading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			base.Controls.Add(tableLayoutPanel1);
			base.Name = "AsyncWaitMessage";
			base.Size = new System.Drawing.Size(200, 80);
			tableLayoutPanel1.ResumeLayout(false);
			tableLayoutPanel1.PerformLayout();
			SpinningWheelPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)PictSpinningWheel).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
