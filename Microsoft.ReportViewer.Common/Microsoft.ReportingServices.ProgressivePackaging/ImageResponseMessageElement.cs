using System.IO;

namespace Microsoft.ReportingServices.ProgressivePackaging
{
	internal sealed class ImageResponseMessageElement : ImageMessageElement
	{
		public string ServerErrorCode
		{
			get;
			private set;
		}

		public byte[] ImageBytes
		{
			get;
			private set;
		}

		public ImageResponseMessageElement()
		{
		}

		public ImageResponseMessageElement(string imageUrl, string imageWidth, string imageHeight, byte[] imageBytes, string serverErrorCode)
			: base(imageUrl, imageWidth, imageHeight)
		{
			ServerErrorCode = serverErrorCode;
			ImageBytes = imageBytes;
		}

		public void Write(BinaryWriter writer)
		{
			WriteStringValue(base.ImageUrl, writer);
			WriteStringValue(base.ImageWidth, writer);
			WriteStringValue(base.ImageHeight, writer);
			WriteStringValue(ServerErrorCode, writer);
			if (ImageBytes != null)
			{
				writer.Write(ImageBytes.Length);
				writer.Write(ImageBytes);
			}
			else
			{
				writer.Write(0);
			}
		}

		private void WriteStringValue(string value, BinaryWriter writer)
		{
			if (value == null)
			{
				writer.Write(string.Empty);
			}
			else
			{
				writer.Write(value);
			}
		}

		public void Read(BinaryReader reader)
		{
			base.ImageUrl = reader.ReadString();
			base.ImageWidth = reader.ReadString();
			base.ImageHeight = reader.ReadString();
			ServerErrorCode = reader.ReadString();
			int count = reader.ReadInt32();
			ImageBytes = reader.ReadBytes(count);
		}
	}
}
