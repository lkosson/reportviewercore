using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel;
using System;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model
{
	internal interface IStyleModel : ICloneable
	{
		Style Interface
		{
			get;
		}

		GlobalStyle GlobalInterface
		{
			get;
		}

		ColorModel BackgroundColor
		{
			get;
			set;
		}

		XMLFontModel Font
		{
			get;
			set;
		}

		IBorderModel BorderModel
		{
			get;
		}

		ST_VerticalAlignment VerticalAlignment
		{
			get;
			set;
		}

		ST_HorizontalAlignment HorizontalAlignment
		{
			get;
			set;
		}

		uint? TextDirection
		{
			get;
			set;
		}

		bool? WrapText
		{
			get;
			set;
		}

		string NumberFormat
		{
			get;
			set;
		}

		int? IndentLevel
		{
			get;
			set;
		}

		int? Orientation
		{
			get;
			set;
		}

		bool HasBeenModified
		{
			get;
		}

		IStyleModel cloneStyle(bool cellStyle);

		new bool Equals(object o);
	}
}
