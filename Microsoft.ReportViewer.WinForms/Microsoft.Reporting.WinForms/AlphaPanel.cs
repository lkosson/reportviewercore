using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Microsoft.Reporting.WinForms
{
	internal class AlphaPanel : Panel
	{
		private IRenderable m_background;

		private double m_opacity = 0.5;

		private Brush m_alphaFillBrush;

		public AlphaPanel(IRenderable background)
		{
			if (background == null)
			{
				throw new ArgumentNullException("background");
			}
			m_background = background;
			SetStyle(ControlStyles.AllPaintingInWmPaint, value: true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, value: true);
			SetStyle(ControlStyles.ResizeRedraw, value: true);
			SetStyle(ControlStyles.UserPaint, value: true);
			SetAlphaFillBrush();
		}

		private void SetAlphaFillBrush()
		{
			if (m_alphaFillBrush != null)
			{
				m_alphaFillBrush.Dispose();
			}
			m_alphaFillBrush = new SolidBrush(Color.FromArgb(Convert.ToInt32(m_opacity * 255.0), Color.White));
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				m_alphaFillBrush.Dispose();
			}
			base.Dispose(disposing);
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			e.Graphics.FillRectangle(Brushes.White, e.ClipRectangle);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (m_background.CanRender)
			{
				e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
				e.Graphics.CompositingMode = CompositingMode.SourceOver;
				try
				{
					m_background.RenderToGraphics(e.Graphics);
				}
				catch
				{
					e.Graphics.FillRectangle(Brushes.White, base.ClientRectangle);
					return;
				}
				e.Graphics.FillRectangle(m_alphaFillBrush, base.ClientRectangle);
			}
		}
	}
}
