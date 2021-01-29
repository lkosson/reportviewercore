using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Security.Permissions;
using System.Text;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	[StrongNameIdentityPermission(SecurityAction.LinkDemand, PublicKey = "0024000004800000940000000602000000240000525341310004000001000100272736ad6e5f9586bac2d531eabc3acc666c2f8ec879fa94f8f7b0327d2ff2ed523448f83c3d5c5dd2dfc7bc99c5286b2c125117bf5cbe242b9d41750732b2bdffe649c6efb8e5526d526fdd130095ecdb7bf210809c6cdad8824faa9ac0310ac3cba2aa0523567b2dfa7fe250b30facbd62d4ec99b94ac47c7d3b28f1f6e4c8")]
	public class MhtmlWriter
	{
		private const string MIMEVERSION = "MIME-Version: 1.0";

		private const string CONTENTTYPEMULIPARTRELATED = "Content-Type: multipart/related";

		private const string BOUNDARYVALUE = "----=_NextPart_01C35DB7.4B204430";

		private const string MIMEDESCRIPTION = "This is a multi-part message in MIME format.";

		private const string CONTENTTYPE = "Content-Type: ";

		private const string CONTENTDISPOSITION = "Content-Disposition: inline";

		private const string CONTENTID = "Content-ID: ";

		private const string XREPORTSERVER = "X-MSSQLRS-ProducerVersion: V{0}";

		private const string CONTENTTRANSFERENCODING = "Content-Transfer-Encoding: base64";

		private const string CHARSETDEF = "charset";

		private const string NAMEDEF = "name";

		private const string FILENAMEDEF = "filename";

		private const string BOUNDARYDEF = "boundary";

		private const string _FoldedHeaderFormat = "{0};\r\n\t{1}=\"{2}\"";

		private const string _MimeBoundaryFormat = "--{0}";

		private const string _MimeEndBoundaryFormat = "--{0}--";

		private const string _ContentIDValueFormat = "<{0}>";

		private const int _readBase64BufferSize = 57;

		private StreamWriter m_writer;

		private bool m_wroteBodyParts;

		private static bool IsAtom(string val)
		{
			if (val == null || val.Length == 0)
			{
				return false;
			}
			bool result = true;
			char[] array = val.ToCharArray();
			char[] array2 = array;
			foreach (char c in array2)
			{
				if (!IsCHAR(c) || IsSpecial(c) || IsCTL(c) || IsSpace(c))
				{
					result = false;
					break;
				}
			}
			return result;
		}

		private static bool IsAsciiParameterValue(string val)
		{
			if (val == null || val.Length == 0)
			{
				return false;
			}
			bool result = true;
			char[] array = val.ToCharArray();
			char[] array2 = array;
			foreach (char c in array2)
			{
				if (!IsCHAR(c) || IsCTL(c))
				{
					result = false;
					break;
				}
			}
			return result;
		}

		private static bool IsCHAR(char c)
		{
			int num = Convert.ToInt32(c);
			if (num >= 0)
			{
				return num <= 127;
			}
			return false;
		}

		private static bool IsSpecial(char c)
		{
			bool result = false;
			switch (c)
			{
			case '"':
			case '\'':
			case '(':
			case ')':
			case ',':
			case '.':
			case ':':
			case ';':
			case '<':
			case '>':
			case '@':
			case '[':
			case ']':
				result = true;
				break;
			}
			return result;
		}

		private static bool IsTSpecial(char c)
		{
			bool result = true;
			if (!IsSpecial(c) && c != '?' && c != '=')
			{
				result = false;
			}
			return result;
		}

		private static bool IsSpace(char c)
		{
			return c == ' ';
		}

		private static bool IsCTL(char c)
		{
			int num = Convert.ToInt32(c);
			if (num >= 31)
			{
				return num == 127;
			}
			return true;
		}

		public void StartMimeDocument(Stream outputStream)
		{
			m_writer = new StreamWriter(outputStream, new ASCIIEncoding());
			m_writer.WriteLine("MIME-Version: 1.0");
			string originalHeader = "Content-Type: multipart/related";
			originalHeader = AppendFoldedParam(originalHeader, "boundary", "----=_NextPart_01C35DB7.4B204430", ensureCompliance: false);
			m_writer.WriteLine(originalHeader);
			m_writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "X-MSSQLRS-ProducerVersion: V{0}", "0.0.0.0"));
			m_writer.WriteLine(string.Empty);
			m_writer.WriteLine("This is a multi-part message in MIME format.");
			m_writer.WriteLine(string.Empty);
			m_writer.Flush();
		}

		private static string AppendFoldedParam(string originalHeader, string param, string val, bool ensureCompliance)
		{
			if (!ensureCompliance)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0};\r\n\t{1}=\"{2}\"", originalHeader, param, val);
			}
			if (val.Length <= 76 && IsAsciiParameterValue(val))
			{
				return AppendFoldedParam(originalHeader, param, val, ensureCompliance: false);
			}
			int num = 78;
			ArrayList arrayList = new ArrayList();
			StringBuilder stringBuilder = new StringBuilder(78);
			char[] array = new char[1];
			bool flag = true;
			int num2 = 1;
			int length = val.Length;
			for (int i = 0; i < length; i++)
			{
				char c = val[i];
				if (flag)
				{
					stringBuilder.Append("\t");
					stringBuilder.Append(param);
					stringBuilder.Append("*");
					stringBuilder.Append(num2);
					stringBuilder.Append("*");
					stringBuilder.Append("=");
					if (num2 == 1)
					{
						stringBuilder.Append("utf-8''");
					}
					flag = false;
					num2++;
				}
				array[0] = c;
				byte[] bytes = Encoding.UTF8.GetBytes(array);
				for (int j = 0; j < bytes.Length; j++)
				{
					byte b = bytes[j];
					stringBuilder.Append("%");
					stringBuilder.Append(b.ToString("X", CultureInfo.InvariantCulture));
					if (stringBuilder.Length >= num - 5 && (j < bytes.Length - 1 || i < length - 1))
					{
						stringBuilder.Append("\r\n");
						flag = true;
						num = stringBuilder.Length + 78;
					}
				}
			}
			return originalHeader + ";\r\n" + stringBuilder.ToString();
		}

		public void WriteMimeBodyPart(Stream bodyPartStream, string name, string mimeType, Encoding streamEncoding)
		{
			RSTrace.RenderingTracer.Assert(m_writer != null, "WriteMimeBodyPart must be called after StartMimeDocument");
			m_wroteBodyParts = true;
			m_writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "--{0}", "----=_NextPart_01C35DB7.4B204430"));
			if (IsAtom(name) && name.Length < 78)
			{
				m_writer.Write("Content-ID: ");
				m_writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "<{0}>", name));
			}
			string text = "Content-Disposition: inline";
			if (!string.IsNullOrEmpty(name))
			{
				text = AppendFoldedParam(text, "filename", name, ensureCompliance: true);
			}
			m_writer.WriteLine(text);
			string text2 = "Content-Type: " + mimeType;
			if (!string.IsNullOrEmpty(name))
			{
				text2 = AppendFoldedParam(text2, "name", name, ensureCompliance: true);
			}
			if (streamEncoding != null)
			{
				text2 = AppendFoldedParam(text2, "charset", streamEncoding.BodyName, ensureCompliance: false);
			}
			m_writer.WriteLine(text2);
			m_writer.WriteLine("Content-Transfer-Encoding: base64");
			m_writer.WriteLine(string.Empty);
			bodyPartStream.Seek(0L, SeekOrigin.Begin);
			byte[] array = new byte[57];
			int num = 0;
			while ((num = bodyPartStream.Read(array, 0, 57)) > 0)
			{
				m_writer.Write(Convert.ToBase64String(array, 0, num));
				m_writer.WriteLine(string.Empty);
			}
			m_writer.Flush();
			m_writer.WriteLine(string.Empty);
		}

		public void EndMimeDocument()
		{
			RSTrace.RenderingTracer.Assert(m_writer != null, "EndMimeDocument must be called after StartMimeDocument");
			if (m_wroteBodyParts)
			{
				m_writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "--{0}--", "----=_NextPart_01C35DB7.4B204430"));
			}
			m_writer.Flush();
			m_writer = null;
		}
	}
}
