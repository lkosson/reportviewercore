using System;
using System.Drawing;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class BufferBitmap : IDisposable
	{
		private Bitmap bitmap;

		private Graphics graphics;

		private Size size;

		private float dpiX = 96f;

		private float dpiY = 96f;

		private bool disposed;

		public Size Size
		{
			get
			{
				return size;
			}
			set
			{
				if (size != value)
				{
					size = value;
					Invalidate();
				}
			}
		}

		public Bitmap Bitmap
		{
			get
			{
				if (bitmap == null)
				{
					Invalidate();
				}
				return bitmap;
			}
		}

		public Graphics Graphics
		{
			get
			{
				if (graphics == null)
				{
					Invalidate();
				}
				return graphics;
			}
		}

		public BufferBitmap(float dpiX, float dpiY)
		{
			Size = new Size(5, 5);
			this.dpiX = dpiX;
			this.dpiY = dpiY;
		}

		private void DisposeObjects()
		{
			if (graphics != null)
			{
				graphics.Dispose();
				graphics = null;
			}
			if (bitmap != null)
			{
				bitmap.Dispose();
				bitmap = null;
			}
		}

		public void Invalidate()
		{
			DisposeObjects();
			bitmap = new Bitmap(size.Width, size.Height);
			bitmap.SetResolution(dpiX, dpiY);
			graphics = Graphics.FromImage(bitmap);
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed && disposing)
			{
				DisposeObjects();
			}
			disposed = true;
		}

		~BufferBitmap()
		{
			Dispose(disposing: false);
		}
	}
}
