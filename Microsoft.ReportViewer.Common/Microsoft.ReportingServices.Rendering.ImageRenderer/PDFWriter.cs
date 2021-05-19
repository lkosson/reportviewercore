using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RichText;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.ReportingServices.Rendering.ImageRenderer
{
	internal sealed class PDFWriter : WriterBase
	{
		internal const float GRID_CONVERSION = 2.834646f;

		private const string CRLF = "\r\n";

		private const string INTEGER_FORMAT = "#########0";

		private const string DECIMAL_FORMAT = "#########0.0##";

		private const int PNG_HEADER_SIZE = 8;

		private int m_procSetId;

		private int m_contentsId;

		private List<string> m_pageContentsSection;

		private List<int> m_pageIds = new List<int>();

		private int m_pageId;

		private int m_pagesId = -1;

		private int m_infoId;

		private int m_rootId;

		private int m_outlinesId = -1;

		private RectangleF m_bounds = RectangleF.Empty;

		private Dictionary<int, PDFUriAction> m_actions = new Dictionary<int, PDFUriAction>();

		private Dictionary<string, PDFFont> m_fonts = new Dictionary<string, PDFFont>();

		private int m_nextObjectId = 1;

		private List<long> m_objectOffsets = new List<long>();

		private UnicodeEncoding m_unicodeEncoding = new UnicodeEncoding(bigEndian: true, byteOrderMark: false);

		private const string cp1252_data = "€‚ƒ„…†‡ˆ‰Š‹ŒŽ‘’“”•–—˜™š›œžŸ ¡¢£¤¥¦§¨©ª«¬­®¯°±²³´µ¶·¸¹º»¼½¾¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþÿ";

		private Dictionary<char, byte> m_ansiEncoding;

		private SizeF m_mediaBoxSize;

		private Dictionary<string, PDFImage> m_images = new Dictionary<string, PDFImage>();

		private List<int> m_fontsUsedInCurrentPage = new List<int>();

		private List<int> m_imagesUsedInCurrentPage = new List<int>();

		private MD5 m_md5Hasher;

		internal Dictionary<string, PDFPagePoint> DocumentMapLabelPoints;

		internal PDFLabel DocumentMapRootLabel;

		internal bool HumanReadablePDF;

		internal bool Test;

		internal bool PrintOnOpen;

		internal FontEmbedding EmbedFonts;

		private static readonly string m_assemblyVersionString;

		private static readonly Dictionary<string, string> m_internalFonts;

		private static readonly Dictionary<char, char> m_unicodeToWinAnsi;

		private int m_imageDpiX;

		private int m_imageDpiY;

		private static readonly Hashtable m_pdfDelimiterChars;

		private const string STRING_ITALIC = "Italic";

		private const string STRING_BOLD = "Bold";

		private bool TestOutputEnabled
		{
			get
			{
				if (HumanReadablePDF)
				{
					return Test;
				}
				return false;
			}
		}

		private MD5 Md5Hasher
		{
			get
			{
				if (m_md5Hasher == null)
				{
					m_md5Hasher = MD5.Create();
				}
				return m_md5Hasher;
			}
		}

		internal override float HalfPixelWidthY => SharedRenderer.ConvertToMillimeters(1, m_imageDpiY) / 2f;

		internal override float HalfPixelWidthX => SharedRenderer.ConvertToMillimeters(1, m_imageDpiX) / 2f;

		static PDFWriter()
		{
			m_unicodeToWinAnsi = new Dictionary<char, char>();
			m_unicodeToWinAnsi.Add('Œ', '\u008c');
			m_unicodeToWinAnsi.Add('œ', '\u009c');
			m_unicodeToWinAnsi.Add('Š', '\u008a');
			m_unicodeToWinAnsi.Add('š', '\u009a');
			m_unicodeToWinAnsi.Add('Ÿ', '\u009f');
			m_unicodeToWinAnsi.Add('Ž', '\u008e');
			m_unicodeToWinAnsi.Add('ž', '\u009e');
			m_unicodeToWinAnsi.Add('ƒ', '\u0083');
			m_unicodeToWinAnsi.Add('ˆ', '\u0088');
			m_unicodeToWinAnsi.Add('\u02dc', '\u0098');
			m_unicodeToWinAnsi.Add('–', '\u0096');
			m_unicodeToWinAnsi.Add('—', '\u0097');
			m_unicodeToWinAnsi.Add('‘', '\u0091');
			m_unicodeToWinAnsi.Add('’', '\u0092');
			m_unicodeToWinAnsi.Add('‚', '\u0082');
			m_unicodeToWinAnsi.Add('“', '\u0093');
			m_unicodeToWinAnsi.Add('”', '\u0094');
			m_unicodeToWinAnsi.Add('„', '\u0084');
			m_unicodeToWinAnsi.Add('†', '\u0086');
			m_unicodeToWinAnsi.Add('‡', '\u0087');
			m_unicodeToWinAnsi.Add('•', '\u0095');
			m_unicodeToWinAnsi.Add('…', '\u0085');
			m_unicodeToWinAnsi.Add('‰', '\u0089');
			m_unicodeToWinAnsi.Add('‹', '\u008b');
			m_unicodeToWinAnsi.Add('›', '\u009b');
			m_unicodeToWinAnsi.Add('€', '\u0080');
			m_unicodeToWinAnsi.Add('™', '\u0099');
			m_internalFonts = new Dictionary<string, string>();
			m_internalFonts.Add("Times New Roman", "Times-Roman");
			m_internalFonts.Add("Times New Roman,Bold", "Times-Bold");
			m_internalFonts.Add("Times New Roman,Italic", "Times-Italic");
			m_internalFonts.Add("Times New Roman,BoldItalic", "Times-BoldItalic");
			m_internalFonts.Add("Arial", "Helvetica");
			m_internalFonts.Add("Arial,Bold", "Helvetica-Bold");
			m_internalFonts.Add("Arial,Italic", "Helvetica-Oblique");
			m_internalFonts.Add("Arial,BoldItalic", "Helvetica-BoldOblique");
			m_internalFonts.Add("Courier New", "Courier");
			m_internalFonts.Add("Courier New,Bold", "Courier-Bold");
			m_internalFonts.Add("Courier New,Italic", "Courier-Oblique");
			m_internalFonts.Add("Courier New,BoldItalic", "Courier-BoldOblique");
			m_internalFonts.Add("Symbol", "Symbol");
			m_pdfDelimiterChars = new Hashtable();
			m_pdfDelimiterChars.Add('(', ConvertReservedCharToASCII('('));
			m_pdfDelimiterChars.Add(')', ConvertReservedCharToASCII(')'));
			m_pdfDelimiterChars.Add('<', ConvertReservedCharToASCII('<'));
			m_pdfDelimiterChars.Add('>', ConvertReservedCharToASCII('>'));
			m_pdfDelimiterChars.Add('[', ConvertReservedCharToASCII('['));
			m_pdfDelimiterChars.Add(']', ConvertReservedCharToASCII(']'));
			m_pdfDelimiterChars.Add('{', ConvertReservedCharToASCII('{'));
			m_pdfDelimiterChars.Add('}', ConvertReservedCharToASCII('}'));
			m_pdfDelimiterChars.Add('/', ConvertReservedCharToASCII('/'));
			m_pdfDelimiterChars.Add('%', ConvertReservedCharToASCII('%'));
			m_assemblyVersionString = typeof(PDFRenderer).Assembly.GetName().Version.ToString();
		}

		internal PDFWriter(Renderer renderer, Stream stream, bool disposeRenderer, CreateAndRegisterStream createAndRegisterStream, int imageDpiX, int imageDpiY)
			: base(renderer, stream, disposeRenderer, createAndRegisterStream)
		{
			m_objectOffsets.Add(0L);
			Write("%PDF-1.3");
			m_imageDpiX = imageDpiX;
			m_imageDpiY = imageDpiY;
		}

		internal override void BeginPage(float pageWidth, float pageHeight)
		{
			m_mediaBoxSize = new SizeF(pageWidth * 2.834646f, pageHeight * 2.834646f);
			m_pageContentsSection = new List<string>();
			m_pageId = ReserveObjectId();
		}

		internal override void BeginPageSection(RectangleF bounds)
		{
			base.BeginPageSection(bounds);
			m_bounds = ConvertBoundsToPDFUnits(bounds);
			StringBuilder stringBuilder = new StringBuilder();
			WriteClipBounds(stringBuilder, m_bounds.Left, m_bounds.Top - m_bounds.Height, m_bounds.Size);
			m_pageContentsSection.Add(stringBuilder.ToString());
		}

		internal override void BeginReport(int dpiX, int dpiY)
		{
			m_procSetId = WriteObject("[/PDF /Text /ImageB /ImageC /ImageI]");
			m_commonGraphics = new GraphicsBase(dpiX, dpiY);
		}

		internal override RectangleF CalculateColumnBounds(RPLReportSection reportSection, RPLPageLayout pageLayout, RPLItemMeasurement column, int columnNumber, float top, float columnHeight, float columnWidth)
		{
			return HardPageBreakShared.CalculateColumnBounds(reportSection, pageLayout, columnNumber, top, columnHeight);
		}

		internal override RectangleF CalculateHeaderBounds(RPLReportSection reportSection, RPLPageLayout pageLayout, float top, float width)
		{
			return HardPageBreakShared.CalculateHeaderBounds(reportSection, pageLayout, top, width);
		}

		internal override RectangleF CalculateFooterBounds(RPLReportSection reportSection, RPLPageLayout pageLayout, float top, float width)
		{
			return HardPageBreakShared.CalculateFooterBounds(reportSection, pageLayout, top, width);
		}

		internal override void DrawBackgroundImage(RPLImageData imageData, RPLFormat.BackgroundRepeatTypes repeat, PointF start, RectangleF position)
		{
			PDFImage image = GetImage(imageData.ImageName, imageData.ImageData, imageData.ImageDataOffset, imageData.GDIImageProps);
			if (image == null)
			{
				return;
			}
			SizeF size = new SizeF(ConvertPixelsToPDFUnits(image.GdiProperties.Width, m_imageDpiX), ConvertPixelsToPDFUnits(image.GdiProperties.Height, m_imageDpiY));
			StringBuilder stringBuilder = new StringBuilder();
			if (repeat == RPLFormat.BackgroundRepeatTypes.Clip)
			{
				RectangleF bounds = ConvertToPDFUnits(position);
				WriteImage(stringBuilder, image.ImageId, bounds, bounds.Left + start.X, bounds.Top - size.Height + start.Y, size);
			}
			else
			{
				float num = SharedRenderer.ConvertToMillimeters(image.GdiProperties.Width, m_imageDpiX);
				float num2 = SharedRenderer.ConvertToMillimeters(image.GdiProperties.Height, m_imageDpiY);
				RectangleF bounds2 = ConvertToPDFUnits(position);
				float num3 = position.Width;
				if (repeat == RPLFormat.BackgroundRepeatTypes.RepeatY)
				{
					num3 = num;
				}
				float num4 = position.Height;
				if (repeat == RPLFormat.BackgroundRepeatTypes.RepeatX)
				{
					num4 = num2;
				}
				for (float num5 = start.X; num5 < num3; num5 += num)
				{
					for (float num6 = start.Y; num6 < num4; num6 += num2)
					{
						PointF pointF = ConvertToPDFUnits(position.Left + num5, position.Top + num6);
						WriteImage(stringBuilder, image.ImageId, bounds2, pointF.X, pointF.Y - size.Height, size);
					}
				}
			}
			m_pageContentsSection.Add(stringBuilder.ToString());
		}

		internal override void DrawLine(Color color, float size, RPLFormat.BorderStyles style, float x1, float y1, float x2, float y2)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("\r\n");
			WriteColor(stringBuilder, color, isStroke: true);
			WriteSizeAndStyle(stringBuilder, size, style);
			PointF point = ConvertToPDFUnits(x1, y1);
			Write(stringBuilder, point);
			stringBuilder.Append(" m ");
			point = ConvertToPDFUnits(x2, y2);
			Write(stringBuilder, point);
			stringBuilder.Append(" l ");
			stringBuilder.Append("S");
			m_pageContentsSection.Add(stringBuilder.ToString());
		}

		internal override void DrawDynamicImage(string imageName, Stream imageStream, long imageDataOffset, RectangleF position)
		{
			byte[] array = null;
			if (imageStream != null)
			{
				array = new byte[(int)imageStream.Length];
				imageStream.Position = 0L;
				imageStream.Read(array, 0, (int)imageStream.Length);
			}
			PDFImage pDFImage = GetImage(imageName, array, imageDataOffset, null);
			bool flag = true;
			if (pDFImage == null)
			{
				pDFImage = GetDefaultImage();
				flag = false;
			}
			position = ConvertToPDFUnits(position);
			SizeF size = new SizeF(ConvertPixelsToPDFUnits(pDFImage.GdiProperties.Width, m_imageDpiX), ConvertPixelsToPDFUnits(pDFImage.GdiProperties.Height, m_imageDpiY));
			if (flag)
			{
				size.Width *= position.Width / size.Width;
				size.Height *= position.Height / size.Height;
			}
			StringBuilder stringBuilder = new StringBuilder();
			WriteImage(stringBuilder, pDFImage.ImageId, position, size);
			m_pageContentsSection.Add(stringBuilder.ToString());
		}

		internal override void DrawImage(RectangleF position, RPLImage image, RPLImageProps instanceProperties, RPLImagePropsDef definitionProperties)
		{
			RPLImageData image2 = instanceProperties.Image;
			PDFImage pDFImage = GetImage(image2.ImageName, image2.ImageData, image2.ImageDataOffset, image2.GDIImageProps);
			RPLFormat.Sizings sizings = definitionProperties.Sizing;
			if (pDFImage == null)
			{
				pDFImage = GetDefaultImage();
				sizings = RPLFormat.Sizings.Clip;
			}
			position = ConvertToPDFUnits(position);
			SizeF size = new SizeF(ConvertPixelsToPDFUnits(pDFImage.GdiProperties.Width, m_imageDpiX), ConvertPixelsToPDFUnits(pDFImage.GdiProperties.Height, m_imageDpiY));
			StringBuilder stringBuilder = new StringBuilder();
			switch (sizings)
			{
			case RPLFormat.Sizings.AutoSize:
			case RPLFormat.Sizings.Clip:
				WriteImage(stringBuilder, pDFImage.ImageId, position, position.Left, position.Y - size.Height, size);
				break;
			case RPLFormat.Sizings.FitProportional:
			{
				float num = position.Width / size.Width;
				float num2 = position.Height / size.Height;
				if (num > num2)
				{
					size.Width *= num2;
					size.Height *= num2;
				}
				else
				{
					size.Width *= num;
					size.Height *= num;
				}
				WriteImage(stringBuilder, pDFImage.ImageId, position, position.Left, position.Y - size.Height, size);
				break;
			}
			default:
				size.Width = position.Width;
				size.Height = position.Height;
				WriteImage(stringBuilder, pDFImage.ImageId, position, size);
				break;
			}
			m_pageContentsSection.Add(stringBuilder.ToString());
		}

		internal override void DrawRectangle(Color color, float size, RPLFormat.BorderStyles style, RectangleF rectangle)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("\r\n");
			WriteColor(stringBuilder, color, isStroke: true);
			WriteSizeAndStyle(stringBuilder, size, style);
			rectangle = ConvertToPDFUnits(rectangle);
			Write(stringBuilder, rectangle.Location);
			stringBuilder.Append(" m ");
			Write(stringBuilder, rectangle.X);
			stringBuilder.Append(" ");
			Write(stringBuilder, rectangle.Top - rectangle.Height);
			stringBuilder.Append(" l ");
			Write(stringBuilder, rectangle.Left + rectangle.Width);
			stringBuilder.Append(" ");
			Write(stringBuilder, rectangle.Top - rectangle.Height);
			stringBuilder.Append(" l ");
			Write(stringBuilder, rectangle.Left + rectangle.Width);
			stringBuilder.Append(" ");
			Write(stringBuilder, rectangle.Top);
			stringBuilder.Append(" l ");
			stringBuilder.Append("s");
			m_pageContentsSection.Add(stringBuilder.ToString());
		}

		private void WriteTextRunTestOutput(StringBuilder sb, ReportTextBox textBox, Microsoft.ReportingServices.Rendering.RichText.TextRun run)
		{
			if (TestOutputEnabled)
			{
				string uniqueName = ((ReportTextRun)run.TextRunProperties).UniqueName;
				if (uniqueName == null)
				{
					uniqueName = textBox.UniqueName;
				}
				string comment = string.Format(CultureInfo.InvariantCulture, "#TextRun#UniqueName={0}", uniqueName);
				WriteComment(sb, comment);
			}
		}

		internal override void DrawTextRun(Win32DCSafeHandle hdc, FontCache fontCache, ReportTextBox textBox, Microsoft.ReportingServices.Rendering.RichText.TextRun run, TypeCode typeCode, RPLFormat.TextAlignments textAlign, RPLFormat.VerticalAlignments verticalAlign, RPLFormat.WritingModes writingMode, RPLFormat.Directions direction, Point position, System.Drawing.Rectangle layoutRectangle, int lHeight, int baselineY)
		{
			if (string.IsNullOrEmpty(run.Text))
			{
				return;
			}
			ITextRunProps textRunProperties = run.TextRunProperties;
			bool flag = run.TextRunProperties.TextDecoration == RPLFormat.TextDecorations.Underline;
			bool flag2 = run.TextRunProperties.TextDecoration == RPLFormat.TextDecorations.LineThrough;
			bool flag3 = run.HasEastAsianChars && writingMode == RPLFormat.WritingModes.Vertical;
			PDFFont pDFFont = ProcessDrawStringFont(run, typeCode, textAlign, verticalAlign, writingMode, direction);
			PointF pointF;
			switch (writingMode)
			{
			case RPLFormat.WritingModes.Horizontal:
				pointF = new PointF(SharedRenderer.ConvertToMillimeters(layoutRectangle.Left, fontCache.Dpi), SharedRenderer.ConvertToMillimeters(layoutRectangle.Top, fontCache.Dpi));
				break;
			case RPLFormat.WritingModes.Vertical:
				pointF = new PointF(SharedRenderer.ConvertToMillimeters(layoutRectangle.Right, fontCache.Dpi), SharedRenderer.ConvertToMillimeters(layoutRectangle.Top, fontCache.Dpi));
				break;
			case RPLFormat.WritingModes.Rotate270:
				pointF = new PointF(SharedRenderer.ConvertToMillimeters(layoutRectangle.Left, fontCache.Dpi), SharedRenderer.ConvertToMillimeters(layoutRectangle.Bottom, fontCache.Dpi));
				break;
			default:
				throw new NotSupportedException();
			}
			PointF pointF2 = new PointF(SharedRenderer.ConvertToMillimeters(position.X, fontCache.Dpi), SharedRenderer.ConvertToMillimeters(position.Y, fontCache.Dpi));
			float num = SharedRenderer.ConvertToMillimeters(baselineY, fontCache.Dpi);
			float lineWidth = float.MinValue;
			if (flag || flag2)
			{
				lineWidth = SharedRenderer.ConvertToMillimeters(run.GetWidth(hdc, fontCache), fontCache.Dpi) * 2.834646f;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("\r\n");
			WriteTextRunTestOutput(stringBuilder, textBox, run);
			stringBuilder.Append("BT ");
			stringBuilder.Append("/F");
			stringBuilder.Append(pDFFont.FontId);
			stringBuilder.Append(" ");
			Write(stringBuilder, textRunProperties.FontSize);
			stringBuilder.Append(" Tf ");
			WriteColor(stringBuilder, textRunProperties.Color, isStroke: false);
			float gridHeight = pDFFont.GridHeight;
			Write(stringBuilder, gridHeight);
			stringBuilder.Append(" TL ");
			float num2 = m_bounds.Left + pointF.X * 2.834646f;
			float num3 = m_bounds.Top - pointF.Y * 2.834646f;
			if (writingMode == RPLFormat.WritingModes.Horizontal || flag3)
			{
				if (flag3)
				{
					float num4 = SharedRenderer.ConvertToMillimeters(run.GetAscent(hdc, fontCache), fontCache.Dpi);
					num2 -= pointF2.Y * 2.834646f;
					num3 -= pointF2.X * 2.834646f + num4 * 2.834646f;
					if (pDFFont.SimulateItalic)
					{
						num3 += num4 * 0.16665f * 2.834646f;
					}
				}
				else
				{
					num2 += pointF2.X * 2.834646f;
					num3 -= num * 2.834646f;
				}
				Write(stringBuilder, num2);
				stringBuilder.Append(" ");
				Write(stringBuilder, num3);
				stringBuilder.Append(" Td ");
				if (pDFFont.SimulateItalic)
				{
					if (flag3)
					{
						stringBuilder.Append("1 -0.3333 0 1 ");
					}
					else
					{
						stringBuilder.Append("1 0 0.3333 1 ");
					}
					Write(stringBuilder, num2);
					stringBuilder.Append(" ");
					Write(stringBuilder, num3);
					stringBuilder.Append(" Tm ");
				}
			}
			else
			{
				if (writingMode == RPLFormat.WritingModes.Vertical)
				{
					num2 -= num * 2.834646f;
					num3 -= pointF2.X * 2.834646f;
				}
				else
				{
					num2 += num * 2.834646f;
					num3 += pointF2.X * 2.834646f;
				}
				if (pDFFont.SimulateItalic)
				{
					if (writingMode == RPLFormat.WritingModes.Vertical)
					{
						stringBuilder.Append("0 -1 1 -0.3333 ");
					}
					else
					{
						stringBuilder.Append("0 1 -1 0.3333 ");
					}
				}
				else if (writingMode == RPLFormat.WritingModes.Vertical)
				{
					stringBuilder.Append("0 -1 1 0 ");
				}
				else
				{
					stringBuilder.Append("0 1 -1 0 ");
				}
				Write(stringBuilder, num2);
				stringBuilder.Append(" ");
				Write(stringBuilder, num3);
				stringBuilder.Append(" Tm ");
			}
			if (pDFFont.SimulateBold)
			{
				stringBuilder.Append("2 Tr ");
				float value = run.TextRunProperties.FontSize / 35f;
				Write(stringBuilder, value);
				stringBuilder.Append(" w ");
				WriteColor(stringBuilder, textRunProperties.Color, isStroke: true);
			}
			string text = run.Text;
			if (!pDFFont.IsComposite)
			{
				text = EscapeString(text);
			}
			if (flag3)
			{
				List<int> list = WriteVerticallyStackedText(fontCache, run, stringBuilder, pDFFont);
				num2 = m_bounds.Left + pointF.X * 2.834646f - num * 2.834646f;
				num3 = m_bounds.Top - pointF.Y * 2.834646f - pointF2.X * 2.834646f;
				if (list != null)
				{
					if (pDFFont.SimulateItalic)
					{
						stringBuilder.Append("0 -1 1 -0.3333 ");
					}
					else
					{
						stringBuilder.Append("0 -1 1 0 ");
					}
					float num5 = 0f;
					if (!list.Contains(0))
					{
						float num6 = SharedRenderer.ConvertToMillimeters(run.GetAscent(hdc, fontCache), fontCache.Dpi);
						float num7 = SharedRenderer.ConvertToMillimeters(run.GlyphData.Advances[0], fontCache.Dpi);
						num5 = num6 - num7;
					}
					Write(stringBuilder, num2);
					stringBuilder.Append(" ");
					Write(stringBuilder, num3 - num5 * 2.834646f);
					stringBuilder.Append(" Tm ");
					WriteVerticalText(fontCache, run, stringBuilder, pDFFont, list);
				}
			}
			else
			{
				WriteText(run, stringBuilder, pDFFont, text, HumanReadablePDF);
			}
			stringBuilder.Append("T* ET");
			if (pDFFont.SimulateBold)
			{
				stringBuilder.Append(" 0 Tr");
			}
			if (flag)
			{
				float lineHeight = 0.125f * SharedRenderer.ConvertToMillimeters(run.UnderlineHeight, fontCache.Dpi);
				float lineOffset = pointF2.Y - num;
				DrawLine(stringBuilder, writingMode, num2, num3, lineOffset, lineHeight, lineWidth);
			}
			if (flag2)
			{
				float num8 = SharedRenderer.ConvertToMillimeters(run.GetAscent(hdc, fontCache), fontCache.Dpi) * 2.834646f;
				float num9 = SharedRenderer.ConvertToMillimeters(run.GetDescent(hdc, fontCache), fontCache.Dpi) * 2.834646f;
				float num10 = num8 + num9;
				float num11 = 0.05f * num10;
				float lineOffset2 = 0f - (num8 - num10 / 2f - num11 / 2f);
				DrawLine(stringBuilder, writingMode, num2, num3, lineOffset2, num11, lineWidth);
			}
			m_pageContentsSection.Add(stringBuilder.ToString());
		}

		private void DrawLine(StringBuilder sb, RPLFormat.WritingModes writingMode, float textLineStart, float textBlockStart, float lineOffset, float lineHeight, float lineWidth)
		{
			sb.Append(" ");
			switch (writingMode)
			{
			case RPLFormat.WritingModes.Horizontal:
				Write(sb, textLineStart);
				sb.Append(" ");
				Write(sb, textBlockStart - lineOffset);
				sb.Append(" ");
				Write(sb, lineWidth);
				sb.Append(" ");
				Write(sb, 0f - lineHeight);
				break;
			case RPLFormat.WritingModes.Vertical:
				Write(sb, textLineStart - lineOffset);
				sb.Append(" ");
				Write(sb, textBlockStart);
				sb.Append(" ");
				Write(sb, 0f - lineHeight);
				sb.Append(" ");
				Write(sb, 0f - lineWidth);
				break;
			default:
				Write(sb, textLineStart + lineOffset);
				sb.Append(" ");
				Write(sb, textBlockStart);
				sb.Append(" ");
				Write(sb, lineHeight);
				sb.Append(" ");
				Write(sb, lineWidth);
				break;
			}
			sb.Append(" re f");
		}

		internal override void EndPage()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < m_pageContentsSection.Count; i++)
			{
				stringBuilder.Append(m_pageContentsSection[i]);
			}
			string text = CompressString(stringBuilder.ToString());
			m_contentsId = ReserveObjectId();
			UpdateCrossRefPosition(m_contentsId);
			Write("\r\n");
			Write(m_contentsId);
			Write(" 0 obj");
			Write("\r\n");
			Write("<< /Length ");
			Write(text.Length);
			if (!HumanReadablePDF)
			{
				Write(" /Filter /FlateDecode");
			}
			Write(" >> stream");
			if (HumanReadablePDF)
			{
				Write(" ");
			}
			else
			{
				Write("\r\n");
			}
			Write(text);
			Write("\r\n");
			Write("endstream");
			Write("\r\n");
			Write("endobj");
			if (m_pagesId == -1)
			{
				m_pagesId = ReserveObjectId();
			}
			StringBuilder stringBuilder2 = new StringBuilder();
			stringBuilder2.Append("<< /Type /Page /Parent ");
			stringBuilder2.Append(m_pagesId);
			stringBuilder2.Append(" 0 R /MediaBox [0 0 ");
			Write(stringBuilder2, m_mediaBoxSize);
			stringBuilder2.Append("] /Contents ");
			stringBuilder2.Append(m_contentsId);
			stringBuilder2.Append(" 0 R");
			EndPage_WriteActions(stringBuilder2);
			stringBuilder2.Append(" /Resources << /ProcSet ");
			stringBuilder2.Append(m_procSetId);
			stringBuilder2.Append(" 0 R /XObject <<");
			for (int j = 0; j < m_imagesUsedInCurrentPage.Count; j++)
			{
				stringBuilder2.Append(" /Im");
				Write(stringBuilder2, m_imagesUsedInCurrentPage[j]);
				stringBuilder2.Append(" ");
				Write(stringBuilder2, m_imagesUsedInCurrentPage[j]);
				stringBuilder2.Append(" 0 R");
			}
			m_imagesUsedInCurrentPage.Clear();
			stringBuilder2.Append(" >> /Font <<");
			for (int k = 0; k < m_fontsUsedInCurrentPage.Count; k++)
			{
				stringBuilder2.Append(" /F");
				Write(stringBuilder2, m_fontsUsedInCurrentPage[k]);
				stringBuilder2.Append(" ");
				Write(stringBuilder2, m_fontsUsedInCurrentPage[k]);
				stringBuilder2.Append(" 0 R");
			}
			m_fontsUsedInCurrentPage.Clear();
			stringBuilder2.Append(" >> >> >>");
			WriteObject(m_pageId, stringBuilder2.ToString());
			m_pageIds.Add(m_pageId);
			foreach (string key in m_images.Keys)
			{
				PDFImage pDFImage = m_images[key];
				if (pDFImage.ImageData != null)
				{
					ProcessImage(pDFImage);
				}
			}
		}

		private void EndPage_WriteActions(StringBuilder sb)
		{
			if (m_actions == null || m_actions.Count <= 0)
			{
				return;
			}
			sb.Append(" /Annots [ ");
			foreach (int key in m_actions.Keys)
			{
				PDFUriAction pDFUriAction = m_actions[key];
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("<< /Border [0 0 0] /Subtype /Link /A << /URI (");
				stringBuilder.Append(EscapeString(pDFUriAction.Uri));
				stringBuilder.Append(") /IsMap false /Type /Action /S /URI >> /Type /Annot /Rect [");
				WriteRectangle(stringBuilder, pDFUriAction.Rectangle.Left, pDFUriAction.Rectangle.Top - pDFUriAction.Rectangle.Height, pDFUriAction.Rectangle.Right, pDFUriAction.Rectangle.Top);
				stringBuilder.Append("] >>");
				WriteObject(key, stringBuilder.ToString());
				Write(sb, key);
				sb.Append(" 0 R ");
			}
			sb.Append("]");
			m_actions.Clear();
		}

		internal override void EndPageSection()
		{
			m_pageContentsSection.Add("\r\nQ");
		}

		internal override void EndReport()
		{
			Dictionary<string, EmbeddedFont> dictionary = new Dictionary<string, EmbeddedFont>();
			foreach (string key in m_fonts.Keys)
			{
				ProcessFontForFontEmbedding(m_fonts[key], dictionary);
			}
			foreach (string key2 in dictionary.Keys)
			{
				WriteEmbeddedFont(dictionary[key2]);
				WriteToUnicodeMap(dictionary[key2]);
			}
			dictionary = null;
			foreach (string key3 in m_fonts.Keys)
			{
				WriteFont(m_fonts[key3]);
			}
			m_fonts = null;
			EndReport_WriteDocumentMap();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<< /Type /Pages /Kids [ ");
			for (int i = 0; i < m_pageIds.Count; i++)
			{
				stringBuilder.Append(m_pageIds[i]);
				stringBuilder.Append(" 0 R ");
			}
			stringBuilder.Append("] /Count ");
			stringBuilder.Append(m_pageIds.Count);
			stringBuilder.Append(" >>");
			WriteObject(m_pagesId, stringBuilder.ToString());
			StringBuilder stringBuilder2 = new StringBuilder();
			stringBuilder2.Append("<< /Type /Catalog /Pages ");
			Write(stringBuilder2, m_pagesId);
			stringBuilder2.Append(" 0 R");
			if (m_outlinesId != -1)
			{
				stringBuilder2.Append(" /Outlines ");
				Write(stringBuilder2, m_outlinesId);
				stringBuilder2.Append(" 0 R");
			}
			if (PrintOnOpen)
			{
				stringBuilder2.Append(" /OpenAction <</S /Named /N /Print>>");
			}
			stringBuilder2.Append(" >>");
			m_rootId = WriteObject(stringBuilder2.ToString());
			StringBuilder stringBuilder3 = new StringBuilder();
			stringBuilder3.Append("<< /Title ");
			WriteUnicodeString(stringBuilder3, m_renderer.RplReport.ReportName);
			stringBuilder3.Append("\r\n");
			stringBuilder3.Append("/Author ");
			WriteUnicodeString(stringBuilder3, m_renderer.RplReport.Author);
			stringBuilder3.Append("\r\n");
			stringBuilder3.Append("/Subject ");
			WriteUnicodeString(stringBuilder3, m_renderer.RplReport.Description);
			stringBuilder3.Append("\r\n");
			stringBuilder3.Append("/Creator (Microsoft Reporting Services ");
			stringBuilder3.Append(m_assemblyVersionString);
			stringBuilder3.Append(")");
			stringBuilder3.Append("\r\n");
			stringBuilder3.Append("/Producer (Microsoft Reporting Services PDF Rendering Extension ");
			stringBuilder3.Append(m_assemblyVersionString);
			stringBuilder3.Append(")");
			stringBuilder3.Append("\r\n");
			stringBuilder3.Append("/CreationDate (D:");
			DateTime now = DateTime.Now;
			TimeSpan utcOffset = TimeZone.CurrentTimeZone.GetUtcOffset(now);
			stringBuilder3.Append(now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture));
			if (utcOffset.Hours > 0)
			{
				stringBuilder3.Append("+");
				stringBuilder3.Append(utcOffset.Hours.ToString("00", CultureInfo.InvariantCulture));
				stringBuilder3.Append("'");
			}
			else if (utcOffset.Hours < 0)
			{
				stringBuilder3.Append("-");
				stringBuilder3.Append(Math.Abs(utcOffset.Hours).ToString("00", CultureInfo.InvariantCulture));
				stringBuilder3.Append("'");
			}
			else
			{
				stringBuilder3.Append("Z00'");
			}
			stringBuilder3.Append(utcOffset.Minutes.ToString("00", CultureInfo.InvariantCulture));
			stringBuilder3.Append("')");
			stringBuilder3.Append("\r\n");
			stringBuilder3.Append(">>");
			m_infoId = WriteObject(stringBuilder3.ToString());
			WriteCrossRef();
		}

		private void EndReport_WriteDocumentMap()
		{
			if (DocumentMapRootLabel != null && DocumentMapRootLabel.Children != null && DocumentMapRootLabel.Children.Count != 0)
			{
				m_outlinesId = ReserveObjectId();
				int firstChildId;
				int lastChildId;
				int num = EndReport_WriteDocumentMap_Recursive(DocumentMapRootLabel.Children, m_outlinesId, out firstChildId, out lastChildId);
				if (num > 0)
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append("<< /Type /Outlines /First ");
					Write(stringBuilder, firstChildId);
					stringBuilder.Append(" 0 R /Last ");
					Write(stringBuilder, lastChildId);
					stringBuilder.Append(" 0 R /Count ");
					Write(stringBuilder, num);
					stringBuilder.Append(" >>");
					WriteObject(m_outlinesId, stringBuilder.ToString());
				}
				DocumentMapLabelPoints = null;
				DocumentMapRootLabel = null;
			}
		}

		private int EndReport_WriteDocumentMap_Recursive(List<PDFLabel> labels, int parentId, out int firstChildId, out int lastChildId)
		{
			firstChildId = (lastChildId = -1);
			int num = -1;
			int previousId = -1;
			int num2 = -1;
			int num3 = 0;
			int num4 = -1;
			if (labels.Count == 0)
			{
				return 0;
			}
			PDFPagePoint pagePoint = null;
			PDFPagePoint value;
			for (int i = 0; i < labels.Count; i++)
			{
				PDFLabel pDFLabel = labels[i];
				if (!DocumentMapLabelPoints.TryGetValue(pDFLabel.UniqueName, out value) || value.PageObjectId == -1)
				{
					continue;
				}
				if (num == -1)
				{
					num = (firstChildId = ReserveObjectId());
					num4 = i;
					pagePoint = value;
					continue;
				}
				int lastChildId2;
				int childCount;
				int firstChildId2 = lastChildId2 = (childCount = -1);
				if (labels[num4].Children != null && labels[num4].Children.Count > 0)
				{
					childCount = EndReport_WriteDocumentMap_Recursive(labels[num4].Children, num, out firstChildId2, out lastChildId2);
				}
				num2 = ReserveObjectId();
				WriteDocumentMapEntry(num, labels[num4], pagePoint, parentId, previousId, num2, firstChildId2, lastChildId2, childCount);
				previousId = num;
				num = num2;
				num4 = i;
				pagePoint = value;
				num3++;
			}
			if (num4 != -1)
			{
				PDFLabel pDFLabel = labels[num4];
				if (DocumentMapLabelPoints.TryGetValue(pDFLabel.UniqueName, out value))
				{
					int lastChildId2;
					int childCount;
					int firstChildId2 = lastChildId2 = (childCount = -1);
					if (labels[num4].Children != null && labels[num4].Children.Count > 0)
					{
						childCount = EndReport_WriteDocumentMap_Recursive(labels[num4].Children, num, out firstChildId2, out lastChildId2);
					}
					WriteDocumentMapEntry(num, labels[num4], value, parentId, previousId, -1, firstChildId2, lastChildId2, childCount);
					num3++;
					if (lastChildId == -1)
					{
						lastChildId = num;
					}
				}
			}
			return num3;
		}

		internal override void FillPolygon(Color color, PointF[] polygon)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("\r\n");
			WriteColor(stringBuilder, color, isStroke: false);
			Write(stringBuilder, ConvertToPDFUnits(polygon[0]));
			stringBuilder.Append(" m ");
			for (int i = 1; i < polygon.Length; i++)
			{
				Write(stringBuilder, ConvertToPDFUnits(polygon[i]));
				stringBuilder.Append(" l ");
			}
			stringBuilder.Append("f");
			m_pageContentsSection.Add(stringBuilder.ToString());
		}

		internal override void FillRectangle(Color color, RectangleF rectangle)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("\r\n");
			WriteColor(stringBuilder, color, isStroke: false);
			rectangle = ConvertToPDFUnits(rectangle);
			WriteRectangle(stringBuilder, rectangle.Left, rectangle.Top - rectangle.Height, rectangle.Size);
			stringBuilder.Append(" re f");
			m_pageContentsSection.Add(stringBuilder.ToString());
		}

		internal override void ProcessAction(string uniqueName, RPLActionInfo actionInfo, RectangleF position)
		{
			RPLAction rPLAction = actionInfo.Actions[0];
			if (!string.IsNullOrEmpty(rPLAction.Hyperlink))
			{
				m_actions.Add(ReserveObjectId(), new PDFUriAction(rPLAction.Hyperlink, ConvertToPDFUnits(position)));
			}
		}

		internal override void ProcessLabel(string uniqueName, string label, PointF point)
		{
			if (DocumentMapRootLabel != null && !DocumentMapLabelPoints.ContainsKey(uniqueName))
			{
				PDFPagePoint value = new PDFPagePoint(m_pageId, point);
				DocumentMapLabelPoints.Add(uniqueName, value);
			}
		}

		protected override void Dispose(bool disposing)
		{
			m_fonts = null;
			m_actions = null;
			m_images = null;
			m_pageContentsSection = null;
			DocumentMapLabelPoints = null;
			base.Dispose(disposing);
		}

		~PDFWriter()
		{
			Dispose(disposing: false);
		}

		private string CompressString(string text)
		{
			if (HumanReadablePDF)
			{
				return text;
			}
			byte[] bytes = Encode1252(text);
			byte[] bytes2 = CompressBytes(bytes);
			StringBuilder stringBuilder = new StringBuilder();
			AppendBytes(stringBuilder, bytes2);
			return stringBuilder.ToString();
		}

		private void BuildAnsiEncoding()
		{
			var lookup = new Dictionary<char, byte>();
			for (int i = 0; i < 128; i++) lookup[(char)i] = (byte)i;
			for (int i = 128; i < 256; i++) lookup[cp1252_data[i - 128]] = (byte)i;
			m_ansiEncoding = lookup;
		}

		private byte[] Encode1252(string text)
		{
			if (m_ansiEncoding == null) BuildAnsiEncoding();
			var bytes = new byte[text.Length];
			for (int i = 0; i < text.Length; i++)
			{
				if (!m_ansiEncoding.TryGetValue(text[i], out var value)) value = (byte)'?';
				bytes[i] = value;
			}
			return bytes;
		}

		private byte[] CompressBytes(byte[] bytes)
		{
			return ManagedCompress(bytes);
		}

		private static int CompressBytes(StringBuilder stringBuilder, byte[] bytes, int offset, int count)
		{
			byte[] array = ManagedCompress(bytes, offset, count);
			int num = array.Length;
			AppendBytes(stringBuilder, array, num);
			return num;
		}

		private static void AppendBytes(StringBuilder stringBuilder, byte[] bytes)
		{
			AppendBytes(stringBuilder, bytes, bytes.Length);
		}

		private static void AppendBytes(StringBuilder stringBuilder, byte[] bytes, int count)
		{
			for (int i = 0; i < count; i++)
			{
				stringBuilder.Append((char)bytes[i]);
			}
		}

		private static byte[] ManagedCompress(byte[] bytes)
		{
			return ManagedCompress(bytes, 0, bytes.Length);
		}

		private static byte[] ManagedCompress(byte[] bytes, int offset, int count)
		{
			MemoryStream memoryStream = new MemoryStream();
			memoryStream.WriteByte(88);
			memoryStream.WriteByte(9);
			DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress, leaveOpen: true);
			deflateStream.Write(bytes, offset, count);
			deflateStream.Close();
			uint num = CalculateAdler32Checksum(bytes, offset, count);
			memoryStream.WriteByte((byte)(num >> 24));
			memoryStream.WriteByte((byte)((num & 0xFF0000) >> 16));
			memoryStream.WriteByte((byte)((num & 0xFF00) >> 8));
			memoryStream.WriteByte((byte)num);
			return memoryStream.ToArray();
		}

		private static uint CalculateAdler32Checksum(byte[] bytes, int offset, int count)
		{
			uint num = 1u;
			uint num2 = 0u;
			if (bytes == null)
			{
				return 1u;
			}
			for (int i = offset; i < count; i++)
			{
				num = (num + bytes[i]) % 65521u;
				num2 = (num2 + num) % 65521u;
			}
			return num2 * 65536 + num;
		}

		private PointF ConvertToPDFUnits(PointF point)
		{
			return ConvertToPDFUnits(point.X, point.Y);
		}

		private PointF ConvertToPDFUnits(float x, float y)
		{
			return new PointF(m_bounds.Left + x * 2.834646f, m_bounds.Top - y * 2.834646f);
		}

		private RectangleF ConvertBoundsToPDFUnits(RectangleF rectangle)
		{
			return new RectangleF(rectangle.X * 2.834646f, m_mediaBoxSize.Height - rectangle.Y * 2.834646f, rectangle.Width * 2.834646f, rectangle.Height * 2.834646f);
		}

		private RectangleF ConvertToPDFUnits(RectangleF rectangle)
		{
			return new RectangleF(m_bounds.Left + rectangle.X * 2.834646f, m_bounds.Top - rectangle.Y * 2.834646f, rectangle.Width * 2.834646f, rectangle.Height * 2.834646f);
		}

		private float ConvertPixelsToPDFUnits(int pixels, float dpi)
		{
			return SharedRenderer.ConvertToMillimeters(pixels, dpi) * 2.834646f;
		}

		private static string ConvertReservedCharToASCII(char reservedChar)
		{
			return Convert.ToString(Convert.ToByte(reservedChar), 16);
		}

		private static string EncodePDFName(string literalName)
		{
			StringBuilder stringBuilder = new StringBuilder(literalName);
			for (int i = 0; i < stringBuilder.Length; i++)
			{
				char c = stringBuilder[i];
				string text = null;
				text = (string)m_pdfDelimiterChars[c];
				if (text == null && char.IsWhiteSpace(c))
				{
					text = ConvertReservedCharToASCII(c);
				}
				if (text != null)
				{
					stringBuilder[i] = '#';
					stringBuilder.Insert(i + 1, text);
					i += text.Length;
				}
			}
			return stringBuilder.ToString();
		}

		private static string EscapeString(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder(text);
			for (int i = 0; i < stringBuilder.Length; i++)
			{
				switch (stringBuilder[i])
				{
				case '(':
				case ')':
				case '\\':
					stringBuilder.Insert(i, '\\');
					i++;
					break;
				case '\n':
					stringBuilder[i] = 'n';
					stringBuilder.Insert(i, '\\');
					i++;
					break;
				case '\r':
					stringBuilder[i] = 'r';
					stringBuilder.Insert(i, '\\');
					i++;
					break;
				case '\t':
					stringBuilder[i] = 't';
					stringBuilder.Insert(i, '\\');
					i++;
					break;
				case '\b':
					stringBuilder[i] = 'b';
					stringBuilder.Insert(i, '\\');
					i++;
					break;
				case '\f':
					stringBuilder[i] = 'f';
					stringBuilder.Insert(i, '\\');
					i++;
					break;
				}
			}
			return stringBuilder.ToString();
		}

		private PDFImage GetDefaultImage()
		{
			string key = "__int__InvalidImage";
			if (!m_images.TryGetValue(key, out PDFImage value))
			{
				value = new PDFImage();
				value.ImageId = ReserveObjectId();
				Bitmap bitmap = Renderer.ImageResources["InvalidImage"];
				lock (bitmap)
				{
					GDIImageProps gdiProperties = new GDIImageProps(bitmap);
					using (MemoryStream memoryStream = new MemoryStream())
					{
						bitmap.Save(memoryStream, ImageFormat.Bmp);
						value.ImageData = memoryStream.GetBuffer();
					}
					value.GdiProperties = gdiProperties;
				}
				m_images.Add(key, value);
			}
			if (!m_imagesUsedInCurrentPage.Contains(value.ImageId))
			{
				m_imagesUsedInCurrentPage.Add(value.ImageId);
			}
			return value;
		}

		private PDFImage GetImage(string imageName, byte[] imageData, long imageDataOffset, GDIImageProps gdiImageProps)
		{
			if (string.IsNullOrEmpty(imageName) && imageData != null)
			{
				byte[] array = Md5Hasher.ComputeHash(imageData);
				StringBuilder stringBuilder = new StringBuilder("__int__");
				for (int i = 0; i < array.Length; i++)
				{
					stringBuilder.Append(array[i].ToString("x2"));
				}
				imageName = stringBuilder.ToString();
			}
			if (string.IsNullOrEmpty(imageName) || !m_images.TryGetValue(imageName, out PDFImage value))
			{
				if (!SharedRenderer.GetImage(m_renderer.RplReport, ref imageData, imageDataOffset, ref gdiImageProps))
				{
					return null;
				}
				value = new PDFImage();
				value.ImageId = ReserveObjectId();
				value.ImageData = imageData;
				value.GdiProperties = gdiImageProps;
				if (string.IsNullOrEmpty(imageName))
				{
					imageName = "__int__" + value.ImageId.ToString(CultureInfo.InvariantCulture);
				}
				m_images.Add(imageName, value);
			}
			if (!m_imagesUsedInCurrentPage.Contains(value.ImageId))
			{
				m_imagesUsedInCurrentPage.Add(value.ImageId);
			}
			return value;
		}

		private static int GetInt32(byte[] data, int offset)
		{
			return (data[offset] << 24) + (data[offset + 1] << 16) + (data[offset + 2] << 8) + data[offset + 3];
		}

		private static bool IsUnicode(char character)
		{
			if (character <= '\u007f')
			{
				return false;
			}
			if (m_unicodeToWinAnsi.ContainsKey(character))
			{
				return false;
			}
			return true;
		}

		private int ReserveObjectId()
		{
			return m_nextObjectId++;
		}

		private void UpdateCrossRefPosition(int objectId)
		{
			if (objectId == m_objectOffsets.Count)
			{
				m_objectOffsets.Add(m_outputStream.Position);
				return;
			}
			if (objectId < m_objectOffsets.Count)
			{
				m_objectOffsets[objectId] = m_outputStream.Position;
				return;
			}
			while (objectId > m_objectOffsets.Count)
			{
				m_objectOffsets.Add(0L);
			}
			m_objectOffsets.Add(m_outputStream.Position);
		}

		private bool UnicodeOutput(Microsoft.ReportingServices.Rendering.RichText.TextRun run)
		{
			if (run.IsComplex || run.ScriptAnalysis.fRTL == 1 || run.CachedFont.TextMetric.tmCharSet == 2)
			{
				return true;
			}
			string text = run.Text;
			for (int i = 0; i < text.Length; i++)
			{
				if (IsUnicode(text[i]))
				{
					return true;
				}
			}
			return false;
		}

		private string GetFontKey(Font font)
		{
			StringBuilder stringBuilder = new StringBuilder(font.FontFamily.GetName(1033));
			FontStyle style = font.Style;
			if ((style & FontStyle.Bold) > FontStyle.Regular && (style & FontStyle.Italic) > FontStyle.Regular)
			{
				stringBuilder.Append(",BoldItalic");
			}
			else if ((style & FontStyle.Bold) > FontStyle.Regular)
			{
				stringBuilder.Append(",Bold");
			}
			else if ((style & FontStyle.Italic) > FontStyle.Regular)
			{
				stringBuilder.Append(",Italic");
			}
			return stringBuilder.ToString();
		}

		private PDFFont ProcessDrawStringFont(Microsoft.ReportingServices.Rendering.RichText.TextRun run, TypeCode typeCode, RPLFormat.TextAlignments textAlign, RPLFormat.VerticalAlignments verticalAlign, RPLFormat.WritingModes writingMode, RPLFormat.Directions direction)
		{
			Font font = run.CachedFont.Font;
			bool flag = UnicodeOutput(run);
			string text = GetFontKey(font);
			if (flag)
			{
				text += "+UnicodeFont";
			}
			string value;
			bool flag2 = m_internalFonts.TryGetValue(text, out value);
			if (flag2)
			{
				text = value;
			}
			if (!m_fonts.TryGetValue(text, out PDFFont value2))
			{
				bool simulateItalic = false;
				bool simulateBold = false;
				if (!flag2)
				{
					Win32DCSafeHandle hdc = m_commonGraphics.GetHdc();
					Win32ObjectSafeHandle win32ObjectSafeHandle = Win32ObjectSafeHandle.Zero;
					try
					{
						win32ObjectSafeHandle = Microsoft.ReportingServices.Rendering.RichText.Win32.SelectObject(hdc, run.CachedFont.Hfont);
						FontPackage.CheckSimulatedFontStyles(hdc, run.CachedFont.TextMetric, ref simulateItalic, ref simulateBold);
					}
					finally
					{
						if (!win32ObjectSafeHandle.IsInvalid)
						{
							win32ObjectSafeHandle.SetHandleAsInvalid();
						}
						m_commonGraphics.ReleaseHdc();
					}
				}
				string text2 = text;
				if (!flag2)
				{
					text2 = EncodePDFName(text2);
				}
				FontStyle style = font.Style;
				int emHeight = font.FontFamily.GetEmHeight(style);
				float gridHeight = font.GetHeight(m_commonGraphics.DpiY) * 2.834646f;
				string fontCMap = null;
				string registry = null;
				string ordering = null;
				string supplement = null;
				if (flag)
				{
					fontCMap = "Identity-H";
					registry = "Adobe";
					ordering = "Identity";
					supplement = "0";
				}
				value2 = new PDFFont(run.CachedFont, font.FontFamily.Name, text2, fontCMap, registry, ordering, supplement, style, emHeight, gridHeight, flag2, simulateItalic, simulateBold);
				value2.FontId = ReserveObjectId();
				m_fonts.Add(text, value2);
			}
			if (!m_fontsUsedInCurrentPage.Contains(value2.FontId))
			{
				m_fontsUsedInCurrentPage.Add(value2.FontId);
			}
			return value2;
		}

		private static void WriteGlyph(Microsoft.ReportingServices.Rendering.RichText.TextRun textRun, StringBuilder sb, PDFFont pdfFont, int glyphIndex)
		{
			ushort num = (ushort)textRun.GlyphData.GlyphScriptShapeData.Glyphs[glyphIndex];
			MapGlyphToUnicodeChar(pdfFont.AddUniqueGlyph(num, (float)textRun.GlyphData.ScaledAdvances[glyphIndex] * pdfFont.EMGridConversion), textRun, glyphIndex);
			if (num == ushort.MaxValue)
			{
				num = 34;
			}
			WriteHex(sb, num >> 8);
			WriteHex(sb, num & 0xFF);
		}

		private static List<int> WriteVerticallyStackedText(FontCache fontCache, Microsoft.ReportingServices.Rendering.RichText.TextRun textRun, StringBuilder sb, PDFFont pdfFont)
		{
			List<int> list = null;
			int glyphCount = textRun.GlyphData.GlyphScriptShapeData.GlyphCount;
			for (int i = 0; i < glyphCount; i++)
			{
				if (new ScriptVisAttr(textRun.GlyphData.GlyphScriptShapeData.VisAttrs[i].word1).uJustification == 4)
				{
					continue;
				}
				if (Microsoft.ReportingServices.Rendering.RichText.Utilities.IsEastAsianChar(textRun.Text[i]))
				{
					sb.Append("<");
					WriteGlyph(textRun, sb, pdfFont, i);
					sb.Append("> Tj");
				}
				else
				{
					if (list == null)
					{
						list = new List<int>();
					}
					list.Add(i);
				}
				if (i < glyphCount - 1)
				{
					float num = SharedRenderer.ConvertToMillimeters(textRun.GlyphData.Advances[i], fontCache.Dpi);
					sb.AppendFormat(" 0 {0} TD", (0f - num) * 2.834646f);
				}
				sb.AppendLine();
			}
			return list;
		}

		private static void WriteVerticalText(FontCache fontCache, Microsoft.ReportingServices.Rendering.RichText.TextRun textRun, StringBuilder sb, PDFFont pdfFont, List<int> skippedCharacterIndeces)
		{
			int num = 0;
			foreach (int skippedCharacterIndece in skippedCharacterIndeces)
			{
				if (skippedCharacterIndece > num)
				{
					int num2 = 0;
					for (int i = num; i < skippedCharacterIndece; i++)
					{
						num2 += textRun.GlyphData.Advances[i];
					}
					float num3 = SharedRenderer.ConvertToMillimeters(num2, fontCache.Dpi);
					sb.AppendFormat("{0} 0 TD ", num3 * 2.834646f);
					num = skippedCharacterIndece;
				}
				sb.Append("<");
				WriteGlyph(textRun, sb, pdfFont, skippedCharacterIndece);
				sb.Append("> Tj");
				sb.AppendLine();
			}
		}

		private static void WriteText(Microsoft.ReportingServices.Rendering.RichText.TextRun textRun, StringBuilder sb, PDFFont pdfFont, string text, bool humanReadablePDF)
		{
			if (string.IsNullOrEmpty(text))
			{
				sb.Append("()Tj ");
				return;
			}
			if (pdfFont.IsComposite)
			{
				sb.Append("<");
				for (int i = 0; i < textRun.GlyphData.GlyphScriptShapeData.GlyphCount; i++)
				{
					WriteGlyph(textRun, sb, pdfFont, i);
				}
				sb.Append("> ");
			}
			else
			{
				sb.Append("(");
				if (humanReadablePDF)
				{
					foreach (char c in text)
					{
						if (m_unicodeToWinAnsi.TryGetValue(c, out char value))
						{
							sb.Append(value);
						}
						else
						{
							sb.Append(c);
						}
					}
				}
				else
				{
					sb.Append(text);
				}
				sb.Append(") ");
				if (!pdfFont.InternalFont)
				{
					for (int k = 0; k < textRun.GlyphData.GlyphScriptShapeData.GlyphCount; k++)
					{
						ushort glyph = (ushort)textRun.GlyphData.GlyphScriptShapeData.Glyphs[k];
						MapGlyphToUnicodeChar(pdfFont.AddUniqueGlyph(glyph, (float)textRun.GlyphData.ScaledAdvances[k] * pdfFont.EMGridConversion), textRun, k);
					}
				}
			}
			sb.Append("Tj ");
		}

		private static void MapGlyphToUnicodeChar(PDFFont.GlyphData glyphData, Microsoft.ReportingServices.Rendering.RichText.TextRun textRun, int glyphIndex)
		{
			if (glyphData != null && textRun.ScriptAnalysis.fLayoutRTL == 0 && textRun.ScriptAnalysis.fRTL == 0 && textRun.Text.Length == textRun.GlyphData.GlyphScriptShapeData.GlyphCount && textRun.Text.Length == textRun.GlyphData.GlyphScriptShapeData.Clusters.Length && glyphIndex < textRun.Text.Length && (!textRun.CachedFont.DefaultGlyph.HasValue || textRun.CachedFont.DefaultGlyph != glyphData.Glyph))
			{
				glyphData.Character = textRun.Text[glyphIndex];
			}
		}

		private unsafe int Process32bppArgbImage(StringBuilder sb, StringBuilder imageContent, PDFImage image)
		{
			sb.Append(" /ColorSpace /DeviceRGB /BitsPerComponent 8 ");
			if (!HumanReadablePDF)
			{
				sb.Append("/Filter /FlateDecode ");
			}
			MemoryStream memoryStream = new MemoryStream();
			int i = 0;
			bool flag = false;
			using (Bitmap bitmap = new Bitmap(System.Drawing.Image.FromStream(new MemoryStream(image.ImageData))))
			{
				System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, image.GdiProperties.Width, image.GdiProperties.Height);
				BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
				IntPtr scan = bitmapData.Scan0;
				byte* ptr = null;
				uint num = (uint)Math.Abs(bitmapData.Stride);
				ptr = (byte*)((num == 0) ? ((byte*)scan.ToPointer() + bitmapData.Stride * (image.GdiProperties.Height - 1)) : scan.ToPointer());
				bool flag2 = false;
				bool flag3 = false;
				for (int j = 0; j < image.GdiProperties.Height; j++)
				{
					for (int k = 0; k < image.GdiProperties.Width; k++)
					{
						byte* ptr2 = ptr + j * num + k * 4;
						Color color = Color.FromArgb(ptr2[3], ptr2[2], ptr2[1], *ptr2);
						int num2;
						if (color.A == 0)
						{
							num2 = i;
							flag = true;
						}
						else
						{
							num2 = (color.ToArgb() & 0xFFFFFF);
							if (num2 == i)
							{
								flag3 = true;
							}
						}
						if (flag && flag3)
						{
							flag2 = true;
							break;
						}
						memoryStream.WriteByte((byte)((num2 & 0xFF0000) >> 16));
						memoryStream.WriteByte((byte)((num2 & 0xFF00) >> 8));
						memoryStream.WriteByte((byte)(num2 & 0xFF));
					}
					if (flag2)
					{
						break;
					}
				}
				if (flag2)
				{
					Dictionary<int, byte> dictionary = new Dictionary<int, byte>();
					for (int l = 0; l < image.GdiProperties.Height; l++)
					{
						for (int m = 0; m < image.GdiProperties.Width; m++)
						{
							byte* ptr3 = ptr + l * num + m * 4;
							int num2 = Color.FromArgb(ptr3[3], ptr3[2], ptr3[1], *ptr3).ToArgb() & 0xFFFFFF;
							if (!dictionary.ContainsKey(num2))
							{
								dictionary.Add(num2, 0);
							}
						}
					}
					for (; dictionary.ContainsKey(i); i++)
					{
					}
					memoryStream = new MemoryStream();
					for (int n = 0; n < image.GdiProperties.Height; n++)
					{
						for (int num3 = 0; num3 < image.GdiProperties.Width; num3++)
						{
							byte* ptr4 = ptr + n * num + num3 * 4;
							Color color = Color.FromArgb(ptr4[3], ptr4[2], ptr4[1], *ptr4);
							int num2 = (color.A != 0) ? (color.ToArgb() & 0xFFFFFF) : i;
							memoryStream.WriteByte((byte)((num2 & 0xFF0000) >> 16));
							memoryStream.WriteByte((byte)((num2 & 0xFF00) >> 8));
							memoryStream.WriteByte((byte)(num2 & 0xFF));
						}
					}
				}
				bitmap.UnlockBits(bitmapData);
			}
			if (flag)
			{
				int value = (i & 0xFF0000) >> 16;
				int value2 = (i & 0xFF00) >> 8;
				int value3 = i & 0xFF;
				sb.Append("/Mask [");
				sb.Append(value);
				sb.Append(" ");
				sb.Append(value);
				sb.Append(" ");
				sb.Append(value2);
				sb.Append(" ");
				sb.Append(value2);
				sb.Append(" ");
				sb.Append(value3);
				sb.Append(" ");
				sb.Append(value3);
				sb.Append("]");
			}
			if (HumanReadablePDF)
			{
				Write(imageContent, memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
			}
			else
			{
				CompressBytes(imageContent, memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
			}
			return imageContent.Length;
		}

		private void ProcessImage(PDFImage image)
		{
			if (image.ImageData == null)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<< /Type /XObject /Subtype /Image");
			StringBuilder stringBuilder2 = new StringBuilder();
			int num;
			if (image.GdiProperties.RawFormat.Equals(ImageFormat.Jpeg))
			{
				stringBuilder.Append(" /BitsPerComponent 8 /Filter /DCTDecode /ColorSpace ");
				if (image.IsMonochromeJpeg)
				{
					stringBuilder.Append("/DeviceGray");
				}
				else
				{
					stringBuilder.Append("/DeviceRGB");
				}
				Write(stringBuilder2, image.ImageData);
				num = image.ImageData.Length;
			}
			else if (image.GdiProperties.RawFormat.Equals(ImageFormat.Png))
			{
				num = ProcessPngImage(stringBuilder, stringBuilder2, image);
				if (num == -1)
				{
					num = Process32bppArgbImage(stringBuilder, stringBuilder2, image);
				}
			}
			else
			{
				num = Process32bppArgbImage(stringBuilder, stringBuilder2, image);
			}
			image.ImageData = null;
			stringBuilder.Append(" /Width ");
			Write(stringBuilder, image.GdiProperties.Width);
			stringBuilder.Append(" /Height ");
			Write(stringBuilder, image.GdiProperties.Height);
			stringBuilder.Append(" /Length ");
			Write(stringBuilder, num);
			stringBuilder.Append(" >>");
			UpdateCrossRefPosition(image.ImageId);
			Write("\r\n");
			Write(image.ImageId);
			Write(" 0 obj");
			Write("\r\n");
			Write(stringBuilder.ToString());
			Write("\r\n");
			Write("stream");
			Write("\r\n");
			Write(stringBuilder2.ToString());
			Write("\r\n");
			Write("endstream");
			Write("\r\n");
			Write("endobj");
		}

		private static int ProcessPngImage(StringBuilder sb, StringBuilder imageContent, PDFImage image)
		{
			int num = 8;
			int num2 = 0;
			int num3 = 0;
			bool flag = false;
			byte[] imageData = image.ImageData;
			int num4 = 0;
			if (imageData[0] != 137 || imageData[1] != 80 || imageData[2] != 78)
			{
				throw new InvalidDataException();
			}
			ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
			string @string;
			do
			{
				int @int = GetInt32(imageData, num);
				@string = aSCIIEncoding.GetString(imageData, num + 4, 4);
				switch (@string)
				{
				case "IHDR":
				{
					num2 = imageData[num + 8 + 8];
					num3 = imageData[num + 8 + 9];
					byte b = imageData[num + 8 + 12];
					if (num3 != 3 || b != 0)
					{
						return -1;
					}
					break;
				}
				case "PLTE":
				{
					flag = true;
					sb.Append(" /ColorSpace [/Indexed /DeviceRGB ");
					Write(sb, @int / 3 - 1);
					sb.Append("<");
					sb.Append("\r\n");
					for (int i = 0; i < @int; i++)
					{
						WriteHex(sb, imageData[num + 8 + i]);
					}
					sb.Append(">]");
					sb.Append("\r\n");
					break;
				}
				case "IDAT":
					Write(imageContent, imageData, num + 8, @int);
					num4 += @int;
					break;
				}
				num += @int + 8 + 4;
			}
			while (@string != "IEND" && num < imageData.Length);
			if (!flag)
			{
				if (num3 == 3)
				{
					sb.Append(" /ColorSpace [/Indexed /DeviceRGB 255  <");
					sb.Append("\r\n");
					for (int j = 0; j < 768; j++)
					{
						Write(sb, j & 0xFF);
						Write(sb, j & 0xFF);
						Write(sb, j & 0xFF);
					}
					sb.Append(">]");
					sb.Append("\r\n");
				}
				else
				{
					sb.Append(" /ColorSpace /DeviceRGB");
				}
			}
			sb.Append(" /BitsPerComponent ");
			Write(sb, num2);
			sb.Append(" /Filter /FlateDecode /DecodeParms << /Predictor 15 /Columns ");
			int num5 = image.GdiProperties.Width * num2 / 8 + ((((image.GdiProperties.Width * num2) & 7) != 0) ? 1 : 0);
			if (num3 == 6)
			{
				num5 *= 4;
			}
			Write(sb, num5);
			if (num3 == 2)
			{
				sb.Append(" /Colors 3");
			}
			else
			{
				sb.Append(" /Colors 1");
			}
			sb.Append(" >>");
			return num4;
		}

		private void Write(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			int length = text.Length;
			byte[] array = new byte[length];
			for (int i = 0; i < length; i++)
			{
				char c = text[i];
				if (c <= 'ÿ')
				{
					array[i] = (byte)c;
				}
				else
				{
					array[i] = 63;
				}
			}
			m_outputStream.Write(array, 0, length);
		}

		private void Write(int value)
		{
			Write(value.ToString("#########0", NumberFormatInfo.InvariantInfo));
		}

		private void Write(long value)
		{
			Write(value.ToString("#########0", NumberFormatInfo.InvariantInfo));
		}

		private void Write(byte[] buffer)
		{
			m_outputStream.Write(buffer, 0, buffer.Length);
		}

		private static void Write(StringBuilder sb, ushort value)
		{
			sb.Append(value.ToString("#########0", NumberFormatInfo.InvariantInfo));
		}

		private static void Write(StringBuilder sb, int value)
		{
			sb.Append(value.ToString("#########0", NumberFormatInfo.InvariantInfo));
		}

		private static void Write(StringBuilder sb, float value)
		{
			if ((float)(long)value == value)
			{
				sb.Append(value.ToString("#########0", NumberFormatInfo.InvariantInfo));
			}
			else
			{
				sb.Append(value.ToString("#########0.0##", NumberFormatInfo.InvariantInfo));
			}
		}

		private static void Write(StringBuilder sb, PointF point)
		{
			Write(sb, point.X);
			sb.Append(" ");
			Write(sb, point.Y);
		}

		private static void Write(StringBuilder sb, SizeF size)
		{
			Write(sb, size.Width);
			sb.Append(" ");
			Write(sb, size.Height);
		}

		private static void Write(StringBuilder sb, byte[] data)
		{
			Write(sb, data, 0, data.Length);
		}

		private static void Write(StringBuilder sb, byte[] data, int offset, int length)
		{
			for (int i = offset; i < offset + length; i++)
			{
				sb.Append((char)data[i]);
			}
		}

		private static void WriteClipBounds(StringBuilder sb, float left, float bottom, SizeF size)
		{
			sb.Append("\r\n");
			sb.Append("q ");
			WriteRectangle(sb, left, bottom, size);
			sb.Append(" re W n");
		}

		private static void WriteColor(StringBuilder sb, Color color, bool isStroke)
		{
			Write(sb, (float)(int)color.R / 255f);
			sb.Append(" ");
			Write(sb, (float)(int)color.G / 255f);
			sb.Append(" ");
			Write(sb, (float)(int)color.B / 255f);
			if (!isStroke)
			{
				sb.Append(" rg ");
			}
			else
			{
				sb.Append(" RG ");
			}
		}

		private void WriteCrossRef()
		{
			Write("\r\n");
			long position = m_outputStream.Position;
			Write("xref");
			Write("\r\n");
			Write("0 ");
			Write(m_objectOffsets.Count);
			Write("\r\n");
			Write("0000000000 65535 f");
			Write("\r\n");
			for (int i = 1; i < m_objectOffsets.Count; i++)
			{
				Write((m_objectOffsets[i] + 2).ToString("0000000000", CultureInfo.InvariantCulture));
				Write(" 00000 n");
				Write("\r\n");
			}
			Write("trailer << /Size ");
			Write(m_objectOffsets.Count);
			Write(" /Root ");
			Write(m_rootId);
			Write(" 0 R /Info ");
			Write(m_infoId);
			Write(" 0 R >>");
			Write("\r\n");
			Write("startxref");
			Write("\r\n");
			Write(position);
			Write("\r\n");
			Write("%%EOF");
		}

		private void WriteDocumentMapEntry(int id, PDFLabel label, PDFPagePoint pagePoint, int parentId, int previousId, int nextId, int firstChildId, int lastChildId, int childCount)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<< /Title ");
			WriteUnicodeString(stringBuilder, label.Label);
			stringBuilder.Append(" /Parent ");
			Write(stringBuilder, parentId);
			stringBuilder.Append(" 0 R /Dest [");
			Write(stringBuilder, pagePoint.PageObjectId);
			stringBuilder.Append(" 0 R /XYZ ");
			Write(stringBuilder, ConvertToPDFUnits(pagePoint.Point));
			stringBuilder.Append(" null]");
			if (firstChildId != -1)
			{
				stringBuilder.Append(" /First ");
				Write(stringBuilder, firstChildId);
				stringBuilder.Append(" 0 R /Last ");
				Write(stringBuilder, lastChildId);
				stringBuilder.Append(" 0 R /Count ");
				Write(stringBuilder, childCount);
			}
			if (previousId != -1)
			{
				stringBuilder.Append(" /Prev ");
				Write(stringBuilder, previousId);
				stringBuilder.Append(" 0 R");
			}
			if (nextId != -1)
			{
				stringBuilder.Append(" /Next ");
				Write(stringBuilder, nextId);
				stringBuilder.Append(" 0 R");
			}
			stringBuilder.Append(" >>");
			WriteObject(id, stringBuilder.ToString());
		}

		private void WriteEmbeddedFont(EmbeddedFont embeddedFont)
		{
			Win32DCSafeHandle hdc = m_commonGraphics.GetHdc();
			Win32ObjectSafeHandle win32ObjectSafeHandle = Win32ObjectSafeHandle.Zero;
			try
			{
				PDFFont pDFFont = embeddedFont.PDFFonts[0];
				win32ObjectSafeHandle = Microsoft.ReportingServices.Rendering.RichText.Win32.SelectObject(hdc, pDFFont.CachedFont.Hfont);
				ushort[] glyphIdArray = embeddedFont.GetGlyphIdArray();
				byte[] buffer = FontPackage.Generate(hdc, pDFFont.FontFamily, glyphIdArray);
				WriteFontBuffer(embeddedFont.ObjectId, buffer);
				foreach (PDFFont pDFFont2 in embeddedFont.PDFFonts)
				{
					pDFFont2.FontPDFFamily = "ABCDEE+" + pDFFont2.FontPDFFamily;
				}
			}
			catch (RSException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				foreach (PDFFont pDFFont3 in embeddedFont.PDFFonts)
				{
					pDFFont3.EmbeddedFont = null;
				}
				if (RSTrace.ImageRendererTracer.TraceError)
				{
					RSTrace.ImageRendererTracer.Trace(TraceLevel.Error, "Exception in WriteEmbeddedFont for Font {0}: {1}", embeddedFont.PDFFonts[0].FontFamily, ex2.Message);
				}
			}
			finally
			{
				if (!win32ObjectSafeHandle.IsInvalid)
				{
					win32ObjectSafeHandle.SetHandleAsInvalid();
				}
				m_commonGraphics.ReleaseHdc();
			}
		}

		private void WriteCMapEntry(StringBuilder stringBuilder, ushort value)
		{
			stringBuilder.Append("<");
			WriteHex(stringBuilder, value >> 8);
			WriteHex(stringBuilder, value & 0xFF);
			stringBuilder.Append(">");
		}

		private void WriteCMapMappingRangeEntry(StringBuilder stringBuilder, CMapMappingRange item)
		{
			WriteCMapEntry(stringBuilder, item.Mapping.Source);
			stringBuilder.Append(" ");
			WriteCMapEntry(stringBuilder, (ushort)(item.Mapping.Source + item.Length));
			stringBuilder.Append(" ");
			WriteCMapEntry(stringBuilder, item.Mapping.Destination);
			stringBuilder.AppendLine();
		}

		private void WriteCMapMappingEntry(StringBuilder stringBuilder, CMapMapping item)
		{
			WriteCMapEntry(stringBuilder, item.Source);
			stringBuilder.Append(" ");
			WriteCMapEntry(stringBuilder, item.Destination);
			stringBuilder.AppendLine();
		}

		private void WriteCMapEntries<T>(StringBuilder stringBuilder, List<T> items, Action<StringBuilder, T> writeItem, string cMapEntryStart, string cMapEntryEnd)
		{
			int num = 0;
			foreach (T item in items)
			{
				if (num % 100 == 0)
				{
					int val = items.Count - num;
					int value = Math.Min(100, val);
					stringBuilder.Append(value);
					stringBuilder.AppendLine(" " + cMapEntryStart);
				}
				writeItem(stringBuilder, item);
				num++;
				if (num % 100 == 0)
				{
					stringBuilder.AppendLine(cMapEntryEnd);
				}
			}
			if (num % 100 != 0)
			{
				stringBuilder.AppendLine(cMapEntryEnd);
			}
		}

		private static void FlushToUnicodeRange(List<CMapMapping> singleMappings, List<CMapMappingRange> rangeMappings, List<CMapMapping> range)
		{
			int count = range.Count;
			if (count != 0)
			{
				CMapMapping cMapMapping = range[0];
				if (count == 1)
				{
					singleMappings.Add(cMapMapping);
				}
				else
				{
					CMapMapping cMapMapping2 = range[count - 1];
					rangeMappings.Add(new CMapMappingRange(cMapMapping, (ushort)(cMapMapping2.Source - cMapMapping.Source)));
				}
				range.Clear();
			}
		}

		private void WriteToUnicodeMappingEntries(StringBuilder stringBuilder, IEnumerable<CMapMapping> glyphIdToUnicodeMapping)
		{
			List<CMapMapping> list = new List<CMapMapping>();
			List<CMapMappingRange> list2 = new List<CMapMappingRange>();
			List<CMapMapping> list3 = new List<CMapMapping>();
			foreach (CMapMapping item in glyphIdToUnicodeMapping.OrderBy((CMapMapping cMapMapping) => cMapMapping))
			{
				if (list3.Count > 0)
				{
					CMapMapping cMapMapping2 = list3[list3.Count - 1];
					if (item.GetSourceDelta(cMapMapping2) != item.GetDestinationDelta(cMapMapping2) || cMapMapping2.GetSourceLeftByte() != item.GetSourceLeftByte())
					{
						FlushToUnicodeRange(list, list2, list3);
					}
				}
				list3.Add(item);
			}
			if (list3.Count > 0)
			{
				FlushToUnicodeRange(list, list2, list3);
			}
			WriteCMapEntries(stringBuilder, list, WriteCMapMappingEntry, "beginbfchar", "endbfchar");
			WriteCMapEntries(stringBuilder, list2, WriteCMapMappingRangeEntry, "beginbfrange", "endbfrange");
		}

		private string GetToUnicodeMap(IEnumerable<CMapMapping> glyphIdToUnicodeMapping)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("/CIDInit /ProcSet findresource begin");
			stringBuilder.AppendLine("12 dict begin");
			stringBuilder.AppendLine("begincmap");
			stringBuilder.AppendLine("/CIDSystemInfo << /Registry (Adobe) /Ordering (UCS) /Supplement 0 >> def");
			stringBuilder.AppendLine("/CMapName /Adobe-Identity-UCS def");
			stringBuilder.AppendLine("/CMapType 2 def");
			stringBuilder.AppendLine("1 begincodespacerange");
			stringBuilder.AppendLine("<0000> <FFFF>");
			stringBuilder.AppendLine("endcodespacerange");
			WriteToUnicodeMappingEntries(stringBuilder, glyphIdToUnicodeMapping);
			stringBuilder.AppendLine("endcmap");
			stringBuilder.AppendLine("CMapName currentdict /CMap defineresource pop");
			stringBuilder.AppendLine("end");
			stringBuilder.Append("end");
			return stringBuilder.ToString();
		}

		private void WriteToUnicodeMap(EmbeddedFont embeddedFont)
		{
			IEnumerable<CMapMapping> glyphIdToUnicodeMapping = embeddedFont.GetGlyphIdToUnicodeMapping();
			string toUnicodeMap = GetToUnicodeMap(glyphIdToUnicodeMapping);
			string text = CompressString(toUnicodeMap);
			int toUnicodeId = embeddedFont.ToUnicodeId;
			UpdateCrossRefPosition(toUnicodeId);
			Write("\r\n");
			Write(toUnicodeId);
			Write(" 0 obj");
			Write("\r\n");
			Write("<< /Filter /FlateDecode ");
			Write(" /Length ");
			Write(text.Length);
			Write(" /Length1 ");
			Write(toUnicodeMap.Length);
			Write(" >>");
			Write("\r\n");
			Write("stream");
			Write("\r\n");
			Write(text);
			Write("\r\n");
			Write("endstream");
			Write("\r\n");
			Write("endobj");
		}

		private void WriteFont(PDFFont pdfFont)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<< /Type /Font ");
			if (!pdfFont.IsComposite && pdfFont.InternalFont)
			{
				stringBuilder.Append("/Subtype /Type1 /BaseFont /");
				stringBuilder.Append(pdfFont.FontPDFFamily);
				stringBuilder.Append(" /Encoding /WinAnsiEncoding");
				stringBuilder.Append(" >>");
				WriteObject(pdfFont.FontId, stringBuilder.ToString());
				stringBuilder = null;
				return;
			}
			int num = -1;
			int num2 = -1;
			Win32ObjectSafeHandle win32ObjectSafeHandle = Win32ObjectSafeHandle.Zero;
			Win32DCSafeHandle hdc = m_commonGraphics.GetHdc();
			Win32ObjectSafeHandle win32ObjectSafeHandle2 = Win32ObjectSafeHandle.Zero;
			try
			{
				win32ObjectSafeHandle = new Win32ObjectSafeHandle(new Font(pdfFont.FontFamily, pdfFont.EMHeight, pdfFont.GDIFontStyle, GraphicsUnit.World).ToHfont(), ownsHandle: true);
				win32ObjectSafeHandle2 = Microsoft.ReportingServices.Rendering.RichText.Win32.SelectObject(hdc, win32ObjectSafeHandle);
				if (!string.IsNullOrEmpty(pdfFont.FontCMap))
				{
					stringBuilder.Append("/Subtype /Type0 /BaseFont /");
					stringBuilder.Append(pdfFont.FontPDFFamily);
					stringBuilder.Append(" /Encoding /");
					stringBuilder.Append(pdfFont.FontCMap);
					num = ReserveObjectId();
					stringBuilder.Append(" /DescendantFonts [");
					Write(stringBuilder, num);
					stringBuilder.Append(" 0 R]");
					if (pdfFont.EmbeddedFont != null)
					{
						stringBuilder.Append(" /ToUnicode ");
						Write(stringBuilder, pdfFont.EmbeddedFont.ToUnicodeId);
						stringBuilder.Append(" 0 R");
					}
				}
				else
				{
					stringBuilder.Append("/Subtype /TrueType /FirstChar 0 /LastChar 255 /Widths [");
					Microsoft.ReportingServices.Rendering.RichText.Win32.ABCFloat[] array = new Microsoft.ReportingServices.Rendering.RichText.Win32.ABCFloat[256];
					if (Microsoft.ReportingServices.Rendering.RichText.Win32.GetCharABCWidthsFloat(hdc, 0u, 255u, array) == 0)
					{
						throw new ReportRenderingException(ErrorCode.rrRenderingError);
					}
					for (int i = 0; i <= 255; i++)
					{
						float num3 = (float)Math.Round(array[i].abcfA + array[i].abcfB + array[i].abcfC);
						num3 *= pdfFont.EMGridConversion;
						stringBuilder.Append(" ");
						Write(stringBuilder, (int)num3);
					}
					stringBuilder.Append(" ] /Encoding /WinAnsiEncoding /BaseFont /");
					stringBuilder.Append(pdfFont.FontPDFFamily);
					num2 = ReserveObjectId();
					stringBuilder.Append(" /FontDescriptor ");
					Write(stringBuilder, num2);
					stringBuilder.Append(" 0 R");
				}
				stringBuilder.Append(" >>");
				WriteObject(pdfFont.FontId, stringBuilder.ToString());
				stringBuilder = null;
				if (num != -1)
				{
					StringBuilder stringBuilder2 = new StringBuilder();
					stringBuilder2.Append("<< /Type /Font /Subtype /CIDFontType2 /BaseFont /");
					stringBuilder2.Append(pdfFont.FontPDFFamily);
					stringBuilder2.Append(" /CIDSystemInfo << /Registry (");
					stringBuilder2.Append(pdfFont.Registry);
					stringBuilder2.Append(") /Ordering (");
					stringBuilder2.Append(pdfFont.Ordering);
					stringBuilder2.Append(") /Supplement ");
					stringBuilder2.Append(pdfFont.Supplement);
					stringBuilder2.Append(" >> ");
					if (pdfFont.FontCMap == "Identity-H")
					{
						stringBuilder2.Append("/W [");
						for (int j = 0; j < pdfFont.UniqueGlyphs.Count; j++)
						{
							PDFFont.GlyphData glyphData = pdfFont.UniqueGlyphs[j];
							Write(stringBuilder2, glyphData.Glyph);
							stringBuilder2.Append(" [");
							Write(stringBuilder2, glyphData.Width);
							stringBuilder2.Append("] ");
						}
						stringBuilder2.Append("]");
					}
					num2 = ReserveObjectId();
					stringBuilder2.Append(" /FontDescriptor ");
					Write(stringBuilder2, num2);
					stringBuilder2.Append(" 0 R >>");
					WriteObject(num, stringBuilder2.ToString());
				}
				if (num2 != -1)
				{
					Microsoft.ReportingServices.Rendering.RichText.Win32.OutlineTextMetric lpOTM = default(Microsoft.ReportingServices.Rendering.RichText.Win32.OutlineTextMetric);
					if (Microsoft.ReportingServices.Rendering.RichText.Win32.GetOutlineTextMetrics(hdc, (uint)Marshal.SizeOf(lpOTM), ref lpOTM) == 0)
					{
						throw new ReportRenderingException(ErrorCode.rrRenderingError);
					}
					StringBuilder stringBuilder3 = new StringBuilder();
					stringBuilder3.Append("<< /Type /FontDescriptor /Ascent ");
					Write(stringBuilder3, (int)Math.Round((float)lpOTM.otmAscent * pdfFont.EMGridConversion));
					stringBuilder3.Append(" /CapHeight 0 /Descent ");
					Write(stringBuilder3, (int)Math.Round((float)lpOTM.otmDescent * pdfFont.EMGridConversion));
					stringBuilder3.Append(" /Flags 32 /FontBBox [ ");
					Write(stringBuilder3, (int)Math.Round((float)lpOTM.left * pdfFont.EMGridConversion));
					stringBuilder3.Append(" ");
					Write(stringBuilder3, (int)Math.Round((float)lpOTM.bottom * pdfFont.EMGridConversion));
					stringBuilder3.Append(" ");
					Write(stringBuilder3, (int)Math.Round((float)lpOTM.right * pdfFont.EMGridConversion));
					stringBuilder3.Append(" ");
					Write(stringBuilder3, (int)Math.Round((float)lpOTM.top * pdfFont.EMGridConversion));
					stringBuilder3.Append(" ] /FontName /");
					stringBuilder3.Append(pdfFont.FontPDFFamily);
					stringBuilder3.Append(" /ItalicAngle ");
					Write(stringBuilder3, (int)Math.Round((float)lpOTM.otmItalicAngle * pdfFont.EMGridConversion));
					stringBuilder3.Append(" /StemV 0 ");
					if (pdfFont.EmbeddedFont != null)
					{
						stringBuilder3.Append(" /FontFile2 ");
						Write(stringBuilder3, pdfFont.EmbeddedFont.ObjectId);
						stringBuilder3.Append(" 0 R ");
					}
					stringBuilder3.Append(">>");
					WriteObject(num2, stringBuilder3.ToString());
				}
			}
			finally
			{
				if (!win32ObjectSafeHandle.IsInvalid)
				{
					win32ObjectSafeHandle.Close();
				}
				if (!win32ObjectSafeHandle2.IsInvalid)
				{
					win32ObjectSafeHandle2.SetHandleAsInvalid();
				}
				m_commonGraphics.ReleaseHdc();
			}
		}

		private void ProcessFontForFontEmbedding(PDFFont pdfFont, Dictionary<string, EmbeddedFont> embeddedFonts)
		{
			pdfFont.FontPDFFamily = pdfFont.FontPDFFamily.Replace("+UnicodeFont", "");
			if (pdfFont.SimulateItalic || pdfFont.SimulateBold)
			{
				int num = pdfFont.FontPDFFamily.LastIndexOf(',');
				if (num != -1)
				{
					if (pdfFont.SimulateItalic)
					{
						int num2 = pdfFont.FontPDFFamily.IndexOf("Italic", num, StringComparison.Ordinal);
						if (num2 != -1)
						{
							pdfFont.FontPDFFamily = pdfFont.FontPDFFamily.Remove(num2, "Italic".Length);
						}
					}
					if (pdfFont.SimulateBold)
					{
						int num3 = pdfFont.FontPDFFamily.IndexOf("Bold", num, StringComparison.Ordinal);
						if (num3 != -1)
						{
							pdfFont.FontPDFFamily = pdfFont.FontPDFFamily.Remove(num3, "Bold".Length);
						}
					}
					if (pdfFont.FontPDFFamily.Length - 1 == num)
					{
						pdfFont.FontPDFFamily = pdfFont.FontPDFFamily.Remove(num);
					}
				}
			}
			if (!pdfFont.IsComposite && pdfFont.InternalFont)
			{
				return;
			}
			bool flag = EmbedFonts == FontEmbedding.Subset;
			if (flag)
			{
				flag = (pdfFont.UniqueGlyphs.Count > 0);
				if (flag)
				{
					Win32DCSafeHandle hdc = m_commonGraphics.GetHdc();
					Win32ObjectSafeHandle win32ObjectSafeHandle = Win32ObjectSafeHandle.Zero;
					try
					{
						win32ObjectSafeHandle = Microsoft.ReportingServices.Rendering.RichText.Win32.SelectObject(hdc, pdfFont.CachedFont.Hfont);
						flag = FontPackage.CheckEmbeddingRights(hdc);
					}
					finally
					{
						if (!win32ObjectSafeHandle.IsInvalid)
						{
							win32ObjectSafeHandle.SetHandleAsInvalid();
						}
						m_commonGraphics.ReleaseHdc();
					}
					if (!flag && RSTrace.ImageRendererTracer.TraceVerbose)
					{
						RSTrace.ImageRendererTracer.Trace(TraceLevel.Verbose, "The font {0} cannot be embedded due to privileges", pdfFont.FontFamily);
					}
				}
			}
			if (flag)
			{
				if (!embeddedFonts.TryGetValue(pdfFont.FontPDFFamily, out EmbeddedFont value))
				{
					value = new EmbeddedFont(ReserveObjectId(), ReserveObjectId());
					embeddedFonts.Add(pdfFont.FontPDFFamily, value);
				}
				value.PDFFonts.Add(pdfFont);
				pdfFont.EmbeddedFont = value;
			}
		}

		private static void WriteHex(StringBuilder sb, int value)
		{
			string text = Convert.ToString(value, 16);
			if (text.Length != 2)
			{
				sb.Append("0");
			}
			sb.Append(text);
		}

		private static void WriteImage(StringBuilder sb, int id, RectangleF bounds, SizeF size)
		{
			WriteImage(sb, id, bounds, bounds.Left, bounds.Top - bounds.Height, size);
		}

		private static void WriteImage(StringBuilder sb, int id, RectangleF bounds, float left, float bottom, SizeF size)
		{
			WriteClipBounds(sb, bounds.Left, bounds.Top - bounds.Height, bounds.Size);
			sb.Append(" ");
			Write(sb, size.Width);
			sb.Append(" 0 0 ");
			Write(sb, size.Height);
			sb.Append(" ");
			Write(sb, left);
			sb.Append(" ");
			Write(sb, bottom);
			sb.Append(" cm /Im");
			Write(sb, id);
			sb.Append(" Do Q");
		}

		private int WriteObject(string pdfObject)
		{
			return WriteObject(m_nextObjectId++, pdfObject);
		}

		private int WriteObject(int objectId, string pdfObject)
		{
			UpdateCrossRefPosition(objectId);
			Write("\r\n");
			Write(objectId);
			Write(" 0 obj");
			Write("\r\n");
			Write(pdfObject);
			Write("\r\n");
			Write("endobj");
			return objectId;
		}

		private void WriteFontBuffer(int objectId, byte[] buffer)
		{
			UpdateCrossRefPosition(objectId);
			byte[] array = CompressBytes(buffer);
			Write("\r\n");
			Write(objectId);
			Write(" 0 obj");
			Write("\r\n");
			Write("<< /Filter /FlateDecode ");
			Write(" /Length ");
			Write(array.Length);
			Write(" /Length1 ");
			Write(buffer.Length);
			Write(" >>");
			Write("\r\n");
			Write("stream");
			Write("\r\n");
			Write(array);
			Write("\r\n");
			Write("endstream");
			Write("\r\n");
			Write("endobj");
		}

		private static void WriteRectangle(StringBuilder sb, float left, float bottom, SizeF size)
		{
			Write(sb, left);
			sb.Append(" ");
			Write(sb, bottom);
			sb.Append(" ");
			Write(sb, size.Width);
			sb.Append(" ");
			Write(sb, size.Height);
		}

		private static void WriteRectangle(StringBuilder sb, float left, float bottom, float right, float top)
		{
			Write(sb, left);
			sb.Append(" ");
			Write(sb, bottom);
			sb.Append(" ");
			Write(sb, right);
			sb.Append(" ");
			Write(sb, top);
		}

		private static void WriteSizeAndStyle(StringBuilder sb, float size, RPLFormat.BorderStyles style)
		{
			Write(sb, size * 2.834646f);
			sb.Append(" w ");
			switch (style)
			{
			case RPLFormat.BorderStyles.Dashed:
				sb.Append("[7 4] 0 d ");
				break;
			case RPLFormat.BorderStyles.Dotted:
				sb.Append("[2 3] 0 d ");
				break;
			default:
				sb.Append("[] 0 d ");
				break;
			}
		}

		private void WriteUnicodeString(StringBuilder sb, string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				sb.Append("<>");
				return;
			}
			sb.Append('<');
			WriteHex(sb, 254);
			WriteHex(sb, 255);
			byte[] bytes = m_unicodeEncoding.GetBytes(text);
			for (int i = 0; i < bytes.Length; i++)
			{
				char value = (char)bytes[i];
				WriteHex(sb, value);
			}
			sb.Append('>');
		}

		private void WriteComment(StringBuilder sb, string comment)
		{
			sb.Append('%');
			sb.Append(comment);
			sb.Append("\r\n");
		}

		internal override void ClipTextboxRectangle(Win32DCSafeHandle hdc, RectangleF textposition)
		{
			RectangleF rectangleF = ConvertToPDFUnits(textposition);
			StringBuilder stringBuilder = new StringBuilder();
			WriteClipBounds(stringBuilder, rectangleF.Left, rectangleF.Top - rectangleF.Height, rectangleF.Size);
			m_pageContentsSection.Add(stringBuilder.ToString());
		}

		internal override void UnClipTextboxRectangle(Win32DCSafeHandle hdc)
		{
			m_pageContentsSection.Add("\r\nQ");
		}
	}
}
