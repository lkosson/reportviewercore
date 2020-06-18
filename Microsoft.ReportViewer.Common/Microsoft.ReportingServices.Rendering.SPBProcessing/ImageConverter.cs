using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal class ImageConverter
	{
		public static bool Convert(ref byte[] imageData, ref string imageMimeType)
		{
			MemoryStream stream = new MemoryStream(imageData);
			try
			{
				using (System.Drawing.Image image = System.Drawing.Image.FromStream(stream))
				{
					if (NeedsToConvert(image.RawFormat))
					{
						MemoryStream memoryStream = new MemoryStream();
						image.Save(memoryStream, ImageFormat.Png);
						imageData = memoryStream.ToArray();
						imageMimeType = PageContext.PNG_MIME_TYPE;
						return true;
					}
					return false;
				}
			}
			catch (ExternalException)
			{
				throw new ArgumentOutOfRangeException();
			}
			catch (ArgumentException)
			{
				throw new ArgumentOutOfRangeException();
			}
			catch (Exception)
			{
				throw;
			}
		}

		public static bool NeedsToConvert(ImageFormat rawFormat)
		{
			if (!rawFormat.Equals(ImageFormat.Png))
			{
				return !rawFormat.Equals(ImageFormat.Jpeg);
			}
			return false;
		}
	}
}
