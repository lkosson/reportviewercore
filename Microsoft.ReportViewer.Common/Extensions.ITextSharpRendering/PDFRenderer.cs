using Microsoft.ReportingServices.Rendering.ImageRenderer;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Extensions.ITextSharpRendering
{
	internal class PDFRenderer : Microsoft.ReportingServices.Rendering.ImageRenderer.Renderer
	{
		internal PDFRenderer() : base(true)
		{
		}

		protected override void ProcessSimpleTextBox(string value, RectangleF textPosition, ReportTextBox rptTextBox, ReportParagraph reportParagraph, ReportTextRun reportTextRun, PointF offset)
		{
			base.ProcessSimpleTextBox(value, textPosition, rptTextBox, reportParagraph, reportTextRun, offset);
		}

		protected override void ProcessRichTextBox(RectangleF textPosition, RPLTextBox textbox, ReportTextBox rptTextBox, PointF offset)
		{
			base.ProcessRichTextBox(textPosition, textbox, rptTextBox, offset);
		}
	}
}
