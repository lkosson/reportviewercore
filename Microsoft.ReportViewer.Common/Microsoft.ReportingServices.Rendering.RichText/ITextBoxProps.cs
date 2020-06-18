using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System.Drawing;

namespace Microsoft.ReportingServices.Rendering.RichText
{
	internal interface ITextBoxProps
	{
		RPLFormat.WritingModes WritingMode
		{
			get;
		}

		RPLFormat.TextAlignments DefaultAlignment
		{
			get;
		}

		RPLFormat.Directions Direction
		{
			get;
		}

		Color BackgroundColor
		{
			get;
		}

		bool CanGrow
		{
			get;
		}

		void DrawTextRun(TextRun run, Paragraph paragraph, Win32DCSafeHandle hdc, float dpiX, FontCache fontCache, int x, int y, int baselineY, int lineHeight, Rectangle layoutRectangle);

		void DrawClippedTextRun(TextRun run, Paragraph paragraph, Win32DCSafeHandle hdc, float dpiX, FontCache fontCache, int x, int y, int baselineY, int lineHeight, Rectangle layoutRectangle, uint fontColorOverride, Rectangle clipRect);
	}
}
