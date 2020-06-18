using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Drawing;

namespace Microsoft.ReportingServices.Rendering.RichText
{
	internal class Underline
	{
		private Point m_startPoint;

		private Point m_endPoint;

		internal const double UnderlineScale = 0.085;

		private Underline()
		{
		}

		internal Underline(TextRun run, Win32DCSafeHandle hdc, FontCache fontCache, Rectangle layoutRectangle, int x, int baselineY, RPLFormat.WritingModes writingMode)
		{
			int width = run.GetWidth(hdc, fontCache);
			int num = (int)((double)(int)((double)run.UnderlineHeight * 0.085) * 1.5);
			switch (writingMode)
			{
			case RPLFormat.WritingModes.Horizontal:
				m_startPoint = new Point(layoutRectangle.X + x, layoutRectangle.Y + baselineY + num);
				m_endPoint = new Point(Math.Min(m_startPoint.X + width, layoutRectangle.Right), m_startPoint.Y);
				break;
			case RPLFormat.WritingModes.Vertical:
				m_startPoint = new Point(layoutRectangle.Right - baselineY - num - 1, layoutRectangle.Y + x);
				m_endPoint = new Point(m_startPoint.X, Math.Min(m_startPoint.Y + width, layoutRectangle.Bottom));
				break;
			case RPLFormat.WritingModes.Rotate270:
				m_startPoint = new Point(layoutRectangle.X + baselineY + num, layoutRectangle.Bottom - x);
				m_endPoint = new Point(m_startPoint.X, Math.Max(m_startPoint.Y - width, layoutRectangle.Top));
				break;
			}
		}

		internal void Draw(Win32DCSafeHandle hdc, int lineThickness, uint rgbColor)
		{
			if (lineThickness < 1)
			{
				lineThickness = 1;
			}
			Win32.LOGBRUSH lplb = default(Win32.LOGBRUSH);
			lplb.lbColor = rgbColor;
			lplb.lbHatch = 0;
			lplb.lbStyle = 0u;
			Win32ObjectSafeHandle win32ObjectSafeHandle = Win32.ExtCreatePen(66048u, (uint)lineThickness, ref lplb, 0u, null);
			Win32ObjectSafeHandle win32ObjectSafeHandle2 = Win32ObjectSafeHandle.Zero;
			try
			{
				win32ObjectSafeHandle2 = Win32.SelectObject(hdc, win32ObjectSafeHandle);
				Win32.MoveToEx(hdc, m_startPoint.X, m_startPoint.Y, IntPtr.Zero);
				Win32.LineTo(hdc, m_endPoint.X, m_endPoint.Y);
			}
			finally
			{
				if (!win32ObjectSafeHandle2.IsInvalid)
				{
					Win32.SelectObject(hdc, win32ObjectSafeHandle2).SetHandleAsInvalid();
					win32ObjectSafeHandle2.SetHandleAsInvalid();
				}
				if (!win32ObjectSafeHandle.IsInvalid)
				{
					win32ObjectSafeHandle.Close();
					win32ObjectSafeHandle = null;
				}
			}
		}
	}
}
