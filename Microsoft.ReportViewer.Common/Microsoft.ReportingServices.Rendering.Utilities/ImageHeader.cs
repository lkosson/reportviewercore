using System;
using System.Drawing.Imaging;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.Utilities
{
	/// <summary>
	/// Reads an image's pixel size, resolution and format from its header, in managed code.
	///
	/// The Excel and Word renderers only need these to size the picture; the bytes themselves are
	/// written to the document untouched. Getting them through System.Drawing.Image.FromStream loads
	/// GDI+, which System.Drawing.Common 7+ refuses to do outside Windows, so on Linux any report with
	/// an image failed to export to Excel or Word. PNG, JPEG, GIF and BMP are read here; anything else
	/// (EMF, TIFF, ...) returns false and the caller falls back to System.Drawing as before.
	///
	/// Resolution defaults to 96 dpi when the file does not state one, which is what GDI+ reports.
	/// </summary>
	internal static class ImageHeader
	{
		private const float DefaultDpi = 96f;

		internal static bool TryRead(Stream stream, out int width, out int height, out float horizontalDpi, out float verticalDpi, out ImageFormat format)
		{
			width = 0;
			height = 0;
			horizontalDpi = DefaultDpi;
			verticalDpi = DefaultDpi;
			format = null;
			if (stream == null || !stream.CanSeek || !stream.CanRead)
			{
				return false;
			}
			long start = stream.Position;
			try
			{
				byte[] head = new byte[64];
				int read = ReadFully(stream, head, 0, head.Length);
				if (read >= 24 && head[0] == 0x89 && head[1] == (byte)'P' && head[2] == (byte)'N' && head[3] == (byte)'G')
				{
					format = ImageFormat.Png;
					return ReadPng(stream, start, out width, out height, ref horizontalDpi, ref verticalDpi);
				}
				if (read >= 4 && head[0] == 0xFF && head[1] == 0xD8)
				{
					format = ImageFormat.Jpeg;
					return ReadJpeg(stream, start, out width, out height, ref horizontalDpi, ref verticalDpi);
				}
				if (read >= 10 && head[0] == (byte)'G' && head[1] == (byte)'I' && head[2] == (byte)'F')
				{
					format = ImageFormat.Gif;
					width = head[6] | (head[7] << 8);
					height = head[8] | (head[9] << 8);
					return width > 0 && height > 0;
				}
				if (read >= 26 && head[0] == (byte)'B' && head[1] == (byte)'M')
				{
					format = ImageFormat.Bmp;
					return ReadBmp(head, read, out width, out height, ref horizontalDpi, ref verticalDpi);
				}
				return false;
			}
			catch (EndOfStreamException)
			{
				return false;
			}
			finally
			{
				stream.Position = start;
			}
		}

		private static bool ReadPng(Stream stream, long start, out int width, out int height, ref float horizontalDpi, ref float verticalDpi)
		{
			// Signature (8), then IHDR: length (4), "IHDR" (4), width (4), height (4), all big-endian.
			stream.Position = start + 16;
			byte[] buffer = new byte[9];
			ReadExactly(stream, buffer, 8);
			width = BigEndian32(buffer, 0);
			height = BigEndian32(buffer, 4);
			// pHYs, when present, comes before the first IDAT.
			stream.Position = start + 8;
			while (true)
			{
				ReadExactly(stream, buffer, 8);
				int length = BigEndian32(buffer, 0);
				string type = new string(new[] { (char)buffer[4], (char)buffer[5], (char)buffer[6], (char)buffer[7] });
				if (type == "pHYs" && length >= 9)
				{
					ReadExactly(stream, buffer, 9);
					if (buffer[8] == 1)
					{
						// Pixels per metre.
						horizontalDpi = PerMetreToDpi(BigEndian32(buffer, 0));
						verticalDpi = PerMetreToDpi(BigEndian32(buffer, 4));
					}
					break;
				}
				if (type == "IDAT" || type == "IEND" || length < 0)
				{
					break;
				}
				stream.Position += (long)length + 4;
			}
			return width > 0 && height > 0;
		}

		private static bool ReadJpeg(Stream stream, long start, out int width, out int height, ref float horizontalDpi, ref float verticalDpi)
		{
			width = 0;
			height = 0;
			stream.Position = start + 2;
			byte[] buffer = new byte[16];
			while (true)
			{
				int b = stream.ReadByte();
				if (b < 0)
				{
					return false;
				}
				if (b != 0xFF)
				{
					continue;
				}
				int marker = stream.ReadByte();
				while (marker == 0xFF)
				{
					marker = stream.ReadByte();
				}
				if (marker < 0 || marker == 0xD9 || marker == 0xDA)
				{
					// End of image, or start of scan with no frame header seen.
					return false;
				}
				if (marker == 0x01 || (marker >= 0xD0 && marker <= 0xD7))
				{
					// Standalone markers carry no length.
					continue;
				}
				ReadExactly(stream, buffer, 2);
				int length = (buffer[0] << 8) | buffer[1];
				if (length < 2)
				{
					return false;
				}
				long next = stream.Position + length - 2;
				if (marker == 0xE0 && length >= 16)
				{
					// APP0 "JFIF\0": version (2), units (1), X density (2), Y density (2).
					ReadExactly(stream, buffer, 12);
					if (buffer[0] == (byte)'J' && buffer[1] == (byte)'F' && buffer[2] == (byte)'I' && buffer[3] == (byte)'F' && buffer[4] == 0)
					{
						int units = buffer[7];
						int x = (buffer[8] << 8) | buffer[9];
						int y = (buffer[10] << 8) | buffer[11];
						if (x > 0 && y > 0 && units == 1)
						{
							horizontalDpi = x;
							verticalDpi = y;
						}
						else if (x > 0 && y > 0 && units == 2)
						{
							horizontalDpi = x * 2.54f;
							verticalDpi = y * 2.54f;
						}
					}
				}
				else if (marker >= 0xC0 && marker <= 0xCF && marker != 0xC4 && marker != 0xC8 && marker != 0xCC)
				{
					// SOFn: precision (1), height (2), width (2).
					ReadExactly(stream, buffer, 5);
					height = (buffer[1] << 8) | buffer[2];
					width = (buffer[3] << 8) | buffer[4];
					return width > 0 && height > 0;
				}
				stream.Position = next;
			}
		}

		private static bool ReadBmp(byte[] head, int read, out int width, out int height, ref float horizontalDpi, ref float verticalDpi)
		{
			int dibSize = LittleEndian32(head, 14);
			if (dibSize == 12)
			{
				// BITMAPCOREHEADER: 16-bit width and height.
				width = head[18] | (head[19] << 8);
				height = head[20] | (head[21] << 8);
				return width > 0 && height > 0;
			}
			width = LittleEndian32(head, 18);
			height = Math.Abs(LittleEndian32(head, 22));
			if (read >= 46)
			{
				int x = LittleEndian32(head, 38);
				int y = LittleEndian32(head, 42);
				if (x > 0 && y > 0)
				{
					horizontalDpi = PerMetreToDpi(x);
					verticalDpi = PerMetreToDpi(y);
				}
			}
			return width > 0 && height > 0;
		}

		private static float PerMetreToDpi(int perMetre)
		{
			return perMetre > 0 ? (float)Math.Round(perMetre * 0.0254, 2) : DefaultDpi;
		}

		private static int BigEndian32(byte[] b, int i)
		{
			return (b[i] << 24) | (b[i + 1] << 16) | (b[i + 2] << 8) | b[i + 3];
		}

		private static int LittleEndian32(byte[] b, int i)
		{
			return b[i] | (b[i + 1] << 8) | (b[i + 2] << 16) | (b[i + 3] << 24);
		}

		private static void ReadExactly(Stream stream, byte[] buffer, int count)
		{
			if (ReadFully(stream, buffer, 0, count) < count)
			{
				throw new EndOfStreamException();
			}
		}

		private static int ReadFully(Stream stream, byte[] buffer, int offset, int count)
		{
			int total = 0;
			while (total < count)
			{
				int n = stream.Read(buffer, offset + total, count - total);
				if (n <= 0)
				{
					break;
				}
				total += n;
			}
			return total;
		}
	}
}
