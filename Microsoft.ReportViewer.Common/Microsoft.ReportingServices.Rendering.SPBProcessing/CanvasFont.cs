using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RichText;
using System;
using System.Drawing;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal class CanvasFont : IDisposable
	{
		private Font m_gdiFont;

		private StringFormat m_stringFormat;

		private bool m_writingModeTopBottom;

		internal Font GDIFont => m_gdiFont;

		internal StringFormat TrimStringFormat => m_stringFormat;

		internal bool WritingModeTopBottom => m_writingModeTopBottom;

		internal CanvasFont(CanvasFont copyFont)
		{
			m_gdiFont = copyFont.GDIFont;
			m_stringFormat = copyFont.TrimStringFormat;
			m_writingModeTopBottom = copyFont.WritingModeTopBottom;
		}

		internal CanvasFont(string family, ReportSize size, FontStyles style, FontWeights weight, TextDecorations decoration, TextAlignments alignment, VerticalAlignments verticalAlignment, Directions direction, WritingModes writingMode)
		{
			CreateFont(family, size, style, weight, decoration, alignment, verticalAlignment, direction, writingMode);
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (m_gdiFont != null)
				{
					m_gdiFont.Dispose();
					m_gdiFont = null;
				}
				if (m_stringFormat != null)
				{
					m_stringFormat.Dispose();
					m_stringFormat = null;
				}
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		private void CreateFont(string family, ReportSize size, FontStyles style, FontWeights weight, TextDecorations decoration, TextAlignments alignment, VerticalAlignments verticalAlignment, Directions direction, WritingModes writingMode)
		{
			CreateGDIFont(family, size, style, weight, decoration);
			StringAlignment textStringAlignment = CreateTextStringAlignment(alignment);
			bool directionRightToLeft = false;
			if (direction == Directions.RTL)
			{
				directionRightToLeft = true;
			}
			SetWritingMode(writingMode);
			StringAlignment lineStringAlignment = CreateLineStringAlignment(verticalAlignment);
			CreateFormatString(textStringAlignment, lineStringAlignment, directionRightToLeft);
		}

		private StringAlignment CreateLineStringAlignment(VerticalAlignments verticalAlignment)
		{
			switch (verticalAlignment)
			{
			case VerticalAlignments.Middle:
				return StringAlignment.Center;
			case VerticalAlignments.Bottom:
				if (m_writingModeTopBottom)
				{
					return StringAlignment.Near;
				}
				return StringAlignment.Far;
			default:
				if (m_writingModeTopBottom)
				{
					return StringAlignment.Far;
				}
				return StringAlignment.Near;
			}
		}

		internal void NewFormatStrings(bool newFormatStrings)
		{
			if (newFormatStrings)
			{
				m_stringFormat = new StringFormat(m_stringFormat);
			}
		}

		internal void SetWritingMode(WritingModes writingMode)
		{
			if (writingMode == WritingModes.Vertical || writingMode == WritingModes.Rotate270)
			{
				m_writingModeTopBottom = true;
			}
		}

		internal void SetTextStringAlignment(TextAlignments alignment, bool newFormatStrings)
		{
			StringAlignment alignment2 = CreateTextStringAlignment(alignment);
			NewFormatStrings(newFormatStrings);
			m_stringFormat.Alignment = alignment2;
		}

		internal void SetLineStringAlignment(VerticalAlignments verticalAlignment, bool newFormatStrings)
		{
			StringAlignment lineAlignment = CreateLineStringAlignment(verticalAlignment);
			NewFormatStrings(newFormatStrings);
			m_stringFormat.LineAlignment = lineAlignment;
		}

		internal void SetFormatFlags(Directions direction, bool setWritingMode, bool newFormatStrings)
		{
			bool directionRightToLeft = false;
			if (direction == Directions.RTL)
			{
				directionRightToLeft = true;
			}
			NewFormatStrings(newFormatStrings);
			StringFormatFlags formatFlags = m_stringFormat.FormatFlags;
			UpdateFormatFlags(ref formatFlags, setWritingMode, directionRightToLeft);
			m_stringFormat.FormatFlags = formatFlags;
		}

		private StringAlignment CreateTextStringAlignment(TextAlignments alignment)
		{
			StringAlignment stringAlignment = StringAlignment.Near;
			switch (alignment)
			{
			case TextAlignments.Center:
				return StringAlignment.Center;
			case TextAlignments.Right:
				return StringAlignment.Far;
			default:
				return StringAlignment.Near;
			}
		}

		internal void CreateGDIFont(string family, ReportSize size, FontStyles style, FontWeights weight, TextDecorations decoration)
		{
			double num = 12.0;
			bool bold = IsBold(weight);
			bool italic = style == FontStyles.Italic;
			if (size != null)
			{
				num = size.ToPoints();
			}
			string fontFamilyName = "Arial";
			if (family != null)
			{
				fontFamilyName = family;
			}
			bool lineThrough = false;
			bool underLine = false;
			switch (decoration)
			{
			case TextDecorations.Underline:
				underLine = true;
				break;
			case TextDecorations.LineThrough:
				lineThrough = true;
				break;
			}
			m_gdiFont = FontCache.CreateGdiPlusFont(fontFamilyName, (float)num, ref bold, ref italic, lineThrough, underLine);
		}

		private void CreateFormatString(StringAlignment textStringAlignment, StringAlignment lineStringAlignment, bool directionRightToLeft)
		{
			m_stringFormat = new StringFormat(StringFormat.GenericDefault);
			m_stringFormat.Alignment = textStringAlignment;
			m_stringFormat.LineAlignment = lineStringAlignment;
			m_stringFormat.Trimming = StringTrimming.Word;
			StringFormatFlags formatFlags = m_stringFormat.FormatFlags;
			formatFlags &= ~StringFormatFlags.NoWrap;
			formatFlags |= StringFormatFlags.LineLimit;
			UpdateFormatFlags(ref formatFlags, setWritingMode: true, directionRightToLeft);
			m_stringFormat.FormatFlags = formatFlags;
		}

		private void UpdateFormatFlags(ref StringFormatFlags formatFlags, bool setWritingMode, bool directionRightToLeft)
		{
			if (setWritingMode)
			{
				if (m_writingModeTopBottom)
				{
					formatFlags |= StringFormatFlags.DirectionVertical;
				}
				else
				{
					formatFlags &= ~StringFormatFlags.DirectionVertical;
				}
			}
			if (directionRightToLeft)
			{
				formatFlags |= StringFormatFlags.DirectionRightToLeft;
			}
			else
			{
				formatFlags &= ~StringFormatFlags.DirectionRightToLeft;
			}
		}

		private bool IsBold(FontWeights fontWeight)
		{
			if (fontWeight == FontWeights.Bold || fontWeight == FontWeights.ExtraBold || fontWeight == FontWeights.Heavy)
			{
				return true;
			}
			return false;
		}
	}
}
