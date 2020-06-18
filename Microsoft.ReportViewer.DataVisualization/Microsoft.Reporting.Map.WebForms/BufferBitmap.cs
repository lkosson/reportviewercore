using System;
using System.Drawing;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class BufferBitmap : IDisposable
	{
		private Bitmap bitmap;

		private Graphics graphics;

		private Size size;

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

		public BufferBitmap()
		{
			Size = new Size(5, 5);
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
