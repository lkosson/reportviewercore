using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Globalization;
using System.Net;

namespace Microsoft.Reporting.Gauge.WebForms
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
				throw new ArgumentNullException(Utils.SRGetStr("ExceptionImageLoaderMissingSerivice"));
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
			throw new ArgumentException(Utils.SRGetStr("ExceptionImageLoaderMissingSerivice", serviceType.ToString()));
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
				GaugeCore gaugeCore = (GaugeCore)serviceContainer.GetService(typeof(GaugeCore));
				if (gaugeCore != null)
				{
					foreach (NamedImage namedImage in gaugeCore.NamedImages)
					{
						if (namedImage.Name == imageURL)
						{
							if (namedImage.Image == null)
							{
								throw new ArgumentException(Utils.SRGetStr("ExceptionImageLoaderInvalidUrl", imageURL));
							}
							return namedImage.Image;
						}
					}
				}
			}
			if (imageData == null)
			{
				imageData = new Hashtable(new CaseInsensitiveHashCodeProvider(CultureInfo.InvariantCulture), StringComparer.OrdinalIgnoreCase);
			}
			if (imageData.Contains(imageURL))
			{
				image = (Image)imageData[imageURL];
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
				if (uri == null)
				{
					GaugeContainer gaugeContainer = (GaugeContainer)serviceContainer.GetService(typeof(GaugeContainer));
					if (gaugeContainer != null)
					{
						if (imageURL.StartsWith("~", StringComparison.Ordinal) && gaugeContainer.applicationDocumentURL.Length > 0)
						{
							try
							{
								uri = new Uri(gaugeContainer.applicationDocumentURL + imageURL.Substring(1));
							}
							catch (Exception)
							{
							}
						}
						else
						{
							string text = gaugeContainer.webFormDocumentURL;
							int num = text.LastIndexOf('/');
							if (num != -1)
							{
								text = text.Substring(0, num + 1);
							}
							try
							{
								uri = new Uri(new Uri(text), imageURL);
							}
							catch (Exception)
							{
							}
						}
					}
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
				throw new ArgumentException(Utils.SRGetStr("ExceptionImageLoaderInvalidUrl", imageURL));
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
	}
}
