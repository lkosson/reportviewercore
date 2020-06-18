using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace Microsoft.ReportingServices.Rendering.RichText
{
	internal class RevertingDeviceContext : IDisposable
	{
		private Win32DCSafeHandle m_hdc;

		private Matrix m_matrix;

		private Win32.XFORM m_oldXForm;

		private Win32.XFORM m_xForm;

		private int m_oldMode;

		private Graphics m_graphics;

		private GraphicsUnit m_pageUnits;

		private float m_pageScale;

		internal Win32DCSafeHandle Hdc => m_hdc;

		internal Win32.XFORM XForm => m_xForm;

		internal RevertingDeviceContext(Graphics g, float dpi)
		{
			m_graphics = g;
			m_matrix = m_graphics.Transform;
			m_pageUnits = m_graphics.PageUnit;
			m_pageScale = m_graphics.PageScale;
			SetupGraphics(dpi);
		}

		public void Dispose()
		{
			if (m_matrix != null)
			{
				if (!Win32.SetWorldTransform(m_hdc, ref m_oldXForm))
				{
					Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
				}
				if (Win32.SetGraphicsMode(m_hdc, m_oldMode) == 0)
				{
					Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
				}
				m_matrix.Dispose();
				m_matrix = null;
			}
			if (!m_hdc.IsInvalid)
			{
				m_graphics.ReleaseHdc();
			}
			m_graphics.PageScale = m_pageScale;
			m_graphics.PageUnit = m_pageUnits;
			GC.SuppressFinalize(this);
		}

		private void SetupGraphics(float dpi)
		{
			m_graphics.PageUnit = GraphicsUnit.Pixel;
			m_graphics.PageScale = 1f;
			m_hdc = new Win32DCSafeHandle(m_graphics.GetHdc(), ownsHandle: false);
			m_oldXForm = default(Win32.XFORM);
			if (m_matrix != null)
			{
				m_xForm = new Win32.XFORM(m_matrix, m_pageUnits, dpi);
				m_oldMode = Win32.SetGraphicsMode(m_hdc, 2);
				if (!Win32.GetWorldTransform(m_hdc, ref m_oldXForm))
				{
					Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
				}
				if (!Win32.SetWorldTransform(m_hdc, ref m_xForm))
				{
					Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
				}
			}
			else
			{
				m_xForm = Win32.XFORM.Identity;
			}
		}
	}
}
