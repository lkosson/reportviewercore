using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.Rendering.ImageRenderer;
using Microsoft.ReportingServices.Rendering.RichText;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;

namespace Extensions.ITextSharpRendering
{
	internal class PDFWriter : WriterBase
	{
		public PDFWriter(Renderer renderer, Stream stream, bool disposeRenderer, CreateAndRegisterStream createAndRegisterStream) : base(renderer, stream, disposeRenderer, createAndRegisterStream)
		{
		}

		internal override void BeginPage(float pageWidth, float pageHeight)
		{
			base.BeginPage(pageWidth, pageHeight);
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
		}

		internal override void DrawLine(Color color, float size, RPLFormat.BorderStyles style, float x1, float y1, float x2, float y2)
		{
		}

		internal override void DrawDynamicImage(string imageName, Stream imageStream, long imageDataOffset, RectangleF position)
		{
		}

		internal override void DrawImage(RectangleF position, RPLImage image, RPLImageProps instanceProperties, RPLImagePropsDef definitionProperties)
		{
		}

		internal override void DrawRectangle(Color color, float size, RPLFormat.BorderStyles style, RectangleF rectangle)
		{
		}

		internal override void DrawTextRun(Win32DCSafeHandle hdc, FontCache fontCache, ReportTextBox textBox, TextRun run, TypeCode typeCode, RPLFormat.TextAlignments textAlign, RPLFormat.VerticalAlignments verticalAlign, RPLFormat.WritingModes writingMode, RPLFormat.Directions direction, Point position, System.Drawing.Rectangle layoutRectangle, int lHeight, int baselineY)
		{
			;
		}

		internal override void EndPage()
		{
		}

		internal override void EndReport()
		{
		}

		internal override void FillPolygon(Color color, PointF[] polygon)
		{
		}

		internal override void FillRectangle(Color color, RectangleF rectangle)
		{
		}

		internal override void UnClipTextboxRectangle(Win32DCSafeHandle hdc)
		{
		}
	}
}
