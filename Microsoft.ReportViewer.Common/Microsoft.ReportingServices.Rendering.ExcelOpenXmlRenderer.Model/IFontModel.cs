using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.officeDocument.x2006.sharedTypes;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using System;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model
{
	internal interface IFontModel : ICloneable
	{
		Font Interface
		{
			get;
		}

		bool Bold
		{
			set;
		}

		bool Italic
		{
			set;
		}

		bool Strikethrough
		{
			set;
		}

		string Name
		{
			set;
		}

		double Size
		{
			set;
		}

		ST_UnderlineValues Underline
		{
			set;
		}

		ST_VerticalAlignRun ScriptStyle
		{
			set;
		}

		ColorModel Color
		{
			set;
		}

		void SetFont(IFontModel font);
	}
}
