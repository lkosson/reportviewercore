using System;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.Reporting.WinForms
{
	internal class ResizableToolStripPanel : Panel
	{
		private Control m_control;

		private DropDownResizeGlyph m_resizeControl;

		private Size m_minimumSize;

		private Point parentLocation = Point.Empty;

		public ResizableToolStripPanel(Control c)
		{
			DoubleBuffered = true;
			m_control = c;
			m_resizeControl = new DropDownResizeGlyph();
			AutoSize = false;
			base.Size = new Size(m_control.Width, m_control.Height + m_resizeControl.Height);
			MinimumSize = base.Size;
			MaximumSize = base.Size;
			m_minimumSize = new Size(m_control.Width, m_control.Height / 2);
			m_control.Dock = DockStyle.Fill;
			m_resizeControl.Dock = DockStyle.Bottom;
			m_resizeControl.Inflate += ResizeControl_Inflate;
			base.Controls.Add(m_control);
			base.Controls.Add(m_resizeControl);
		}

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			m_control.Focus();
		}

		private void ResizeControl_Inflate(object sender, ResizeEventArgs e)
		{
			if (parentLocation.IsEmpty)
			{
				parentLocation = base.Parent.Location;
			}
			Size size3 = MaximumSize = (MinimumSize = new Size(Math.Max(m_minimumSize.Width, base.Size.Width + e.DeltaX), Math.Max(m_minimumSize.Height, base.Size.Height + e.DeltaY)));
			if (RightToLeft == RightToLeft.Yes)
			{
				base.Parent.Location = new Point(parentLocation.X - (size3.Width - m_minimumSize.Width), parentLocation.Y);
			}
			m_resizeControl.Refresh();
		}
	}
}
