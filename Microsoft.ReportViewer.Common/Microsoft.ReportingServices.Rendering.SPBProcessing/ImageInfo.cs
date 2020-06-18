using System;
using System.Drawing;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal class ImageInfo : IDisposable
	{
		public Stream ImageData;

		public int Width;

		public int Height;

		public void RenderAndDispose(Graphics g, int x, int y)
		{
			using (System.Drawing.Image image = System.Drawing.Image.FromStream(ImageData))
			{
				g.DrawImage(image, new Point(x, y));
			}
			Dispose();
		}

		public void Dispose()
		{
			if (ImageData != null)
			{
				ImageData.Dispose();
				ImageData = null;
			}
			GC.SuppressFinalize(this);
		}
	}
}
