using Microsoft.ReportingServices.Rendering.RichText;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Microsoft.ReportingServices.Rendering.ImageRenderer
{
	internal class GraphicsBase : IDisposable
	{
		internal delegate void SynchronizedOperation();

		protected System.Drawing.Graphics m_graphicsBase;

		protected Bitmap m_imageBase;

		private int m_dpiX = 96;

		private int m_dpiY = 96;

		private Win32DCSafeHandle m_hdc = Win32DCSafeHandle.Zero;

		internal int DpiX
		{
			get
			{
				return m_dpiX;
			}
			set
			{
				m_dpiX = value;
			}
		}

		internal int DpiY
		{
			get
			{
				return m_dpiY;
			}
			set
			{
				m_dpiY = value;
			}
		}

		internal Win32DCSafeHandle Hdc
		{
			get
			{
				return m_hdc;
			}
			set
			{
				m_hdc = value;
			}
		}

		internal System.Drawing.Graphics SystemGraphics => m_graphicsBase;

		internal GraphicsBase(float dpiX, float dpiY)
		{
			m_dpiX = (int)dpiX;
			m_dpiY = (int)dpiY;
			m_imageBase = new Bitmap(2, 2);
			m_imageBase.SetResolution(dpiX, dpiY);
			m_graphicsBase = System.Drawing.Graphics.FromImage(m_imageBase);
			m_graphicsBase.CompositingMode = CompositingMode.SourceOver;
			m_graphicsBase.PageUnit = GraphicsUnit.Millimeter;
			m_graphicsBase.PixelOffsetMode = PixelOffsetMode.Default;
			m_graphicsBase.SmoothingMode = SmoothingMode.Default;
			m_graphicsBase.TextRenderingHint = TextRenderingHint.SystemDefault;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (m_graphicsBase != null)
				{
					ReleaseCachedHdc(releaseHdc: true);
					m_graphicsBase.Dispose();
					m_graphicsBase = null;
				}
				if (m_imageBase != null)
				{
					m_imageBase.Dispose();
					m_imageBase = null;
				}
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		~GraphicsBase()
		{
			Dispose(disposing: false);
		}

		internal virtual void CacheHdc(bool createNewHdc)
		{
			if (createNewHdc || Hdc == Win32DCSafeHandle.Zero)
			{
				GetHdc();
			}
		}

		internal virtual void ReleaseCachedHdc(bool releaseHdc)
		{
			if (releaseHdc)
			{
				ReleaseHdc();
			}
		}

		internal virtual void ExecuteSync(SynchronizedOperation synchronizedOperation)
		{
			synchronizedOperation();
		}

		internal Win32DCSafeHandle GetHdc()
		{
			ReleaseHdc();
			Hdc = new Win32DCSafeHandle(m_graphicsBase.GetHdc(), ownsHandle: false);
			return Hdc;
		}

		internal void ReleaseHdc()
		{
			if (!Hdc.IsInvalid)
			{
				m_graphicsBase.ReleaseHdc();
				Hdc = Win32DCSafeHandle.Zero;
			}
		}

		internal float ConvertToMillimeters(int pixels)
		{
			return SharedRenderer.ConvertToMillimeters(pixels, m_dpiX);
		}

		internal int ConvertToPixels(float mm)
		{
			return SharedRenderer.ConvertToPixels(mm, m_dpiX);
		}
	}
}
