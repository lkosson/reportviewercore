using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.Reporting.WinForms
{
	internal class DropDownResizeGlyph : Control
	{
		private Point m_mouseLocation;

		private bool m_resize;

		private Size m_minimumSize = new Size(100, 100);

		private IContainer components;

		private ImageList imageList;

		private Rectangle ResizableImageArea
		{
			get
			{
				Size size = imageList.Images[0].Size;
				Rectangle result = new Rectangle(new Point(1, 1), size);
				if (RightToLeft != RightToLeft.Yes)
				{
					result.Offset(base.Size.Width - size.Width, 0);
				}
				return result;
			}
		}

		internal event EventHandler<ResizeEventArgs> Inflate;

		public DropDownResizeGlyph()
		{
			InitializeComponent();
			base.Height = imageList.Images[0].Height + 2;
			BackColor = Color.FromKnownColor(KnownColor.Window);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (ResizableImageArea.Contains(e.Location))
			{
				m_mouseLocation = Control.MousePosition;
				m_resize = true;
			}
			SetResizeCursor(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (m_resize && this.Inflate != null)
			{
				Point mousePosition = Control.MousePosition;
				int num = mousePosition.X - m_mouseLocation.X;
				if (RightToLeft == RightToLeft.Yes)
				{
					num *= -1;
				}
				ResizeEventArgs resizeEventArgs = new ResizeEventArgs(num, mousePosition.Y - m_mouseLocation.Y);
				if (Math.Abs(resizeEventArgs.DeltaX) > 1 || Math.Abs(resizeEventArgs.DeltaY) > 1)
				{
					this.Inflate(this, resizeEventArgs);
					m_mouseLocation = mousePosition;
				}
			}
			SetResizeCursor(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			m_resize = false;
			SetResizeCursor(e);
		}

		private void SetResizeCursor(MouseEventArgs e)
		{
			if (m_resize || ResizableImageArea.Contains(e.Location))
			{
				Cursor cursor = (RightToLeft == RightToLeft.Yes) ? Cursors.SizeNESW : Cursors.SizeNWSE;
				if (Cursor.Current != cursor)
				{
					Cursor.Current = cursor;
				}
			}
			else
			{
				Cursor.Current = Cursors.Default;
			}
		}

		protected override void OnPaint(PaintEventArgs pe)
		{
			base.OnPaint(pe);
			Rectangle clientRectangle = base.ClientRectangle;
			Size size = imageList.Images[0].Size;
			using (Pen pen = new Pen(Color.FromKnownColor(KnownColor.ControlDark), 1f))
			{
				pe.Graphics.DrawLine(pen, clientRectangle.Left, clientRectangle.Top, clientRectangle.Right, clientRectangle.Top);
			}
			if (RightToLeft == RightToLeft.Yes)
			{
				imageList.Draw(pe.Graphics, new Point(clientRectangle.Left + 1, clientRectangle.Top + 1), 1);
			}
			else
			{
				imageList.Draw(pe.Graphics, new Point(clientRectangle.Width - size.Width - 1, clientRectangle.Top + 1), 0);
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Microsoft.Reporting.WinForms.DropDownResizeGlyph));
			imageList = new System.Windows.Forms.ImageList(components);
			SuspendLayout();
			imageList.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("imageList.ImageStream");
			imageList.TransparentColor = System.Drawing.Color.Transparent;
			imageList.Images.SetKeyName(0, "HandleGrip.gif");
			imageList.Images.SetKeyName(1, "HandleGripRTL.gif");
			ResumeLayout(false);
		}
	}
}
