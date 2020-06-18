using System;
using System.Globalization;
using System.IO;

namespace Microsoft.ReportingServices.ProgressivePackaging
{
	internal class MessageFormatterFactory
	{
		internal const string ProgressivePackagingMimeType = "application/progressive-report";

		internal const uint FileMarker = 1179510781u;

		internal static IMessageReader CreateReader(Stream s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			BinaryReader binaryReader = new BinaryReader(s, MessageUtil.StringEncoding);
			try
			{
				uint num = binaryReader.ReadUInt32();
				if (num != 1179510781)
				{
					throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Stream is not a valid package."));
				}
				string format = binaryReader.ReadString();
				int majorVersion = binaryReader.ReadInt32();
				int minorVersion = binaryReader.ReadInt32();
				return InternalCreateReader(binaryReader, num, format, majorVersion, minorVersion);
			}
			catch (IOException)
			{
				throw;
			}
			catch (NotSupportedException)
			{
				throw;
			}
			catch (Exception innerException)
			{
				throw new IOException("header", innerException);
			}
		}

		internal static IMessageWriter CreateWriter(Stream s, string format, int majorVersion, int minorVersion)
		{
			return CreateWriter(s, format, majorVersion, minorVersion, writePackagePrefix: true);
		}

		internal static IMessageWriter CreateWriter(Stream s, string format, int majorVersion, int minorVersion, bool writePackagePrefix)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			if (string.IsNullOrEmpty(format))
			{
				throw new ArgumentException("format");
			}
			BinaryWriter binaryWriter = new BinaryWriter(s, MessageUtil.StringEncoding);
			if (writePackagePrefix)
			{
				binaryWriter.Write(1179510781u);
				binaryWriter.Write(format);
				binaryWriter.Write(majorVersion);
				binaryWriter.Write(minorVersion);
			}
			return InternalCreateWriter(binaryWriter, format, majorVersion, minorVersion);
		}

		private static IMessageReader InternalCreateReader(BinaryReader reader, uint marker, string format, int majorVersion, int minorVersion)
		{
			if (format.Equals("Progressive", StringComparison.Ordinal) && majorVersion == 1 && minorVersion == 0)
			{
				return new ProgressiveReader(reader);
			}
			throw NotSupportedException(format, majorVersion, minorVersion);
		}

		private static IMessageWriter InternalCreateWriter(BinaryWriter writer, string format, int majorVersion, int minorVersion)
		{
			if (format.Equals("Progressive", StringComparison.Ordinal) && majorVersion == 1 && minorVersion == 0)
			{
				return new ProgressiveWriter(writer);
			}
			throw NotSupportedException(format, majorVersion, minorVersion);
		}

		private static NotSupportedException NotSupportedException(string format, int majorVersion, int minorVersion)
		{
			return new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "{0} {1}.{2}", format, majorVersion, minorVersion));
		}
	}
}
