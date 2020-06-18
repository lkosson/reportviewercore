using System;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.ReportingServices.CommonControls
{
	internal class GenericDropDown : UserControl
	{
		private Control m_topControl;

		private Control m_dropDownControl;

		private ToolStripDropDown m_dropDown;

		public Control TopControl
		{
			get
			{
				return m_topControl;
			}
			set
			{
				base.Controls.Clear();
				m_topControl = value;
				if (value != null)
				{
					base.Controls.Add(m_topControl);
				}
			}
		}

		public Control DropDownControl
		{
			get
			{
				return m_dropDownControl;
			}
			set
			{
				m_dropDown.Items.Clear();
				m_dropDownControl = value;
				m_dropDownControl.CreateControl();
				if (value != null)
				{
					ToolStripControlHost toolStripControlHost = new ToolStripControlHost(value);
					toolStripControlHost.Margin = new Padding(0);
					m_dropDown.Items.Add(toolStripControlHost);
				}
			}
		}

		public event EventHandler DropDownClosed;

		public GenericDropDown()
		{
			AutoSize = true;
			m_dropDown = new ToolStripDropDown();
			m_dropDown.AutoSize = true;
			m_dropDown.Padding = new Padding(1);
			m_dropDown.Closed += OnDropDownClosed;
		}

		public void OpenDropDown()
		{
			if (TopControl == null || DropDownControl == null)
			{
				throw new InvalidOperationException("GenericDropDown controls not set");
			}
			ToolStripDropDownDirection direction = ToolStripDropDownDirection.Right;
			if (RightToLeft == RightToLeft.Yes)
			{
				direction = ToolStripDropDownDirection.Left;
			}
			m_dropDown.Show(TopControl, new Point(0, TopControl.Height + 1), direction);
			DropDownControl.Focus();
		}

		public void CloseDropDown()
		{
			if (TopControl == null || DropDownControl == null)
			{
				throw new InvalidOperationException("GenericDropDown controls not set");
			}
			m_dropDown.Close();
		}

		protected override void OnRightToLeftChanged(EventArgs e)
		{
			base.OnRightToLeftChanged(e);
			m_dropDown.RightToLeft = RightToLeft;
		}

		private void OnDropDownClosed(object sender, ToolStripDropDownClosedEventArgs e)
		{
			if (this.DropDownClosed != null)
			{
				this.DropDownClosed(this, EventArgs.Empty);
			}
		}
	}
}
