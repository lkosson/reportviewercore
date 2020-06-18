using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Net;
using System.Reflection;
using System.Resources;

namespace Microsoft.Reporting.Chart.WebForms.Utilities
{
	internal class ImageLoader : IDisposable, IServiceProvider
	{
		private Hashtable imageData;

		private IServiceContainer serviceContainer;

		private ImageLoader()
		{
		}

		public ImageLoader(IServiceContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException(SR.ExceptionImageLoaderInvalidServiceContainer);
			}
			serviceContainer = container;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(ImageLoader))
			{
				return this;
			}
			throw new ArgumentException(SR.ExceptionImageLoaderUnsupportedType(serviceType.ToString()));
		}

		public void Dispose()
		{
			if (imageData == null)
			{
				return;
			}
			foreach (DictionaryEntry imageDatum in imageData)
			{
				((Image)imageDatum.Value).Dispose();
			}
			imageData = null;
			GC.SuppressFinalize(this);
		}

		public Image LoadImage(string imageURL)
		{
			return LoadImage(imageURL, saveImage: true);
		}

		public Image LoadImage(string imageURL, bool saveImage)
		{
			Image image = null;
			if (serviceContainer != null)
			{
				Chart chart = (Chart)serviceContainer.GetService(typeof(Chart));
				if (chart != null)
				{
					foreach (NamedImage image2 in chart.Images)
					{
						if (image2.Name == imageURL)
						{
							return image2.Image;
						}
					}
				}
			}
			if (imageData == null)
			{
				imageData = new Hashtable(StringComparer.OrdinalIgnoreCase);
			}
			if (imageData.Contains(imageURL))
			{
				image = (Image)imageData[imageURL];
			}
			if (image == null)
			{
				try
				{
					int num = imageURL.IndexOf("::", StringComparison.Ordinal);
					if (num > 0)
					{
						string baseName = imageURL.Substring(0, num);
						string name = imageURL.Substring(num + 2);
						image = (Image)new ResourceManager(baseName, Assembly.GetExecutingAssembly()).GetObject(name);
					}
				}
				catch (Exception)
				{
				}
			}
			if (image == null)
			{
				Uri uri = null;
				try
				{
					uri = new Uri(imageURL);
				}
				catch (Exception)
				{
				}
				if (uri != null)
				{
					try
					{
						image = Image.FromStream(WebRequest.Create(uri).GetResponse().GetResponseStream());
					}
					catch (Exception)
					{
					}
				}
			}
			if (image == null)
			{
				image = LoadFromFile(imageURL);
			}
			if (image == null)
			{
				throw new ArgumentException(SR.ExceptionImageLoaderIncorrectImageUrl(imageURL));
			}
			if (saveImage)
			{
				imageData[imageURL] = image;
			}
			return image;
		}

		private Image LoadFromFile(string fileName)
		{
			try
			{
				return Image.FromFile(fileName);
			}
			catch (Exception)
			{
				return null;
			}
		}

		internal bool GetAdjustedImageSize(string name, Graphics graphics, ref SizeF size)
		{
			Image image = LoadImage(name);
			if (image == null)
			{
				return false;
			}
			GetAdjustedImageSize(image, graphics, ref size);
			return true;
		}

		internal static void GetAdjustedImageSize(Image image, Graphics graphics, ref SizeF size)
		{
			if (graphics != null)
			{
				size.Width = (float)image.Width * graphics.DpiX / image.HorizontalResolution;
				size.Height = (float)image.Height * graphics.DpiY / image.VerticalResolution;
			}
			else
			{
				size.Width = image.Width;
				size.Height = image.Height;
			}
		}

		internal static bool DoDpisMatch(Image image, Graphics graphics)
		{
			if (graphics.DpiX == image.HorizontalResolution)
			{
				return graphics.DpiY == image.VerticalResolution;
			}
			return false;
		}

		internal static Image GetScaledImage(Image image, Graphics graphics)
		{
			return new Bitmap(image, new Size((int)((float)image.Width * graphics.DpiX / image.HorizontalResolution), (int)((float)image.Height * graphics.DpiY / image.VerticalResolution)));
		}
	}
}
