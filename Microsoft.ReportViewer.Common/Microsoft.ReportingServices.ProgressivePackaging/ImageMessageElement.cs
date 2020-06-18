using System;

namespace Microsoft.ReportingServices.ProgressivePackaging
{
	internal abstract class ImageMessageElement
	{
		public string ImageUrl
		{
			get;
			protected set;
		}

		public string ImageWidth
		{
			get;
			protected set;
		}

		public string ImageHeight
		{
			get;
			protected set;
		}

		public ImageMessageElement()
		{
		}

		public ImageMessageElement(string imageUrl, string imageWidth, string imageHeight)
		{
			ImageUrl = imageUrl;
			ImageWidth = imageWidth;
			ImageHeight = imageHeight;
		}

		public override bool Equals(object obj)
		{
			ImageMessageElement imageMessageElement = obj as ImageMessageElement;
			if (imageMessageElement == null)
			{
				return false;
			}
			if (string.Compare(ImageUrl, imageMessageElement.ImageUrl, StringComparison.Ordinal) == 0 && string.Compare(ImageWidth, imageMessageElement.ImageWidth, StringComparison.Ordinal) == 0)
			{
				return string.Compare(ImageHeight, imageMessageElement.ImageHeight, StringComparison.Ordinal) == 0;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return ((ImageUrl != null) ? ImageUrl.GetHashCode() : 0) ^ ((ImageWidth != null) ? ImageWidth.GetHashCode() : 0) ^ ((ImageHeight != null) ? ImageHeight.GetHashCode() : 0);
		}
	}
}
