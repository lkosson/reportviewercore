using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.officeDocument.x2006.sharedTypes;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using System;
using System.IO.Packaging;
using System.Text.RegularExpressions;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal static class XMLConstants
	{
		internal static class ContentTypes
		{
			internal static class Http
			{
				public const string Xlsx = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

				public const string Xlsm = "application/vnd.ms-excel.sheet.macroEnabled.12";

				public const string Xltx = "application/vnd.openxmlformats-officedocument.spreadsheetml.template";

				public const string Xltm = "application/vnd.ms-excel.template.macroEnabled.12";
			}

			public const string Workbook = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml";

			public const string MacroEnabledWorkbook = "application/vnd.ms-excel.sheet.macroEnabled.main+xml";

			public const string TemplateWorkbook = "application/vnd.openxmlformats-officedocument.spreadsheetml.template.main+xml";

			public const string MacroEnabledTemplateWorkbook = "application/vnd.ms-excel.template.macroEnabled.main+xml";

			public const string Styles = "application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml";

			public const string Themes = "application/vnd.openxmlformats-officedocument.theme+xml";

			public const string Worksheets = "application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml";

			public const string SharedStrings = "application/vnd.openxmlformats-officedocument.spreadsheetml.sharedStrings+xml";

			public const string ExtendedProperties = "application/vnd.openxmlformats-officedocument.extended-properties+xml";

			public const string OpcCoreProperties = "application/vnd.openxmlformats-package.core-properties+xml";

			public const string VbaMacro = "application/vnd.ms-office.vbaProject";

			public const string CustomProperties = "application/vnd.openxmlformats-officedocument.custom-properties+xml";

			public const string Drawings = "application/vnd.openxmlformats-officedocument.drawing+xml";

			public const string CalculationChain = "application/vnd.openxmlformats-officedocument.spreadsheetml.calcChain+xml";

			public const string Relationship = "application/vnd.openxmlformats-package.relationships+xml";

			public const string Xml = "application/xml";
		}

		internal static class RelationshipTypes
		{
			public const string ExtendedProperties = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/extended-properties";

			public const string OpcCoreProperties = "http://schemas.openxmlformats.org/package/2006/relationships/meatadata/core-properties";

			public const string Workbook = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument";

			public const string Styles = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles";

			public const string Themes = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/theme";

			public const string Worksheets = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet";

			public const string SharedStrings = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/sharedStrings";

			public const string VbaMacro = "http://schemas.microsoft.com/office/2006/relationships/vbaProject";

			public const string CustomProperties = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/custom-properties";

			public const string Drawings = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/drawing";

			public const string Hyperlink = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/hyperlink";

			public const string Image = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/image";

			public const string CalculationChain = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/calcChain";
		}

		internal static class DefaultPaths
		{
			public const string Styles = "xl/styles.xml";

			public const string Themes = "xl/theme/theme.xml";

			public const string Workbook = "xl/workbook.xml";

			public const string Worksheets = "xl/worksheets/sheet{0}.xml";

			public const string SharedStrings = "xl/sharedStrings.xml";

			public const string OpcCoreProperties = "docProps/core.xml";

			public const string ExtendedProperties = "docProps/app.xml";

			public const string CustomProperties = "docProps/custom.xml";

			public const string Drawings = "xl/drawings/drawing{0}.xml";

			public const string Images = "xl/media/image{0}.";
		}

		internal static class StyleIds
		{
			public const uint FirstNumberFormat = 82u;

			public const uint FirstFont = 0u;

			public const uint FirstFill = 0u;

			public const uint FirstBorder = 0u;
		}

		internal static class Margins
		{
			internal static class Default
			{
				public const double LeftRight = 0.7;

				public const double TopBottom = 0.75;

				public const double HeaderFooter = 0.3;
			}

			public const double MinInclusive = 0.0;

			public const double MaxExclusive = 49.0;
		}

		internal static class ZoomSettings
		{
			public const uint DefaultZoom = 100u;

			public const uint DefaultPageWidthFit = 1u;

			public const uint DefaultPageHeightFit = 1u;

			public const bool DefaultUseZoom = true;

			public const int FitToHeightMax = 32767;

			public const int FitToWidthMax = 32767;

			public const int FitToHeightMin = 0;

			public const int FitToWidthMin = 0;

			public const int ZoomMin = 10;

			public const int ZoomMax = 400;
		}

		internal static class DefinedNames
		{
			public const int NameMinLength = 1;

			public const int NameMaxLength = 255;

			public static readonly Regex DefinedNameValidationRe = new Regex("^(.+!)?[a-zA-Z_\\\\][\\w\\\\.]+$");

			public const string PrintTitlesRangeName = "_xlnm.Print_Titles";
		}

		public static class Font
		{
			public static class Default
			{
				public const double FontSize = 11.0;

				public const string FontName = "Calibri";

				public static readonly ST_UnderlineValues Underline = ST_UnderlineValues.none;

				public static readonly ST_VerticalAlignRun VerticalAlignment = ST_VerticalAlignRun.baseline;
			}

			public const double MinFontSize = 1.0;

			public const double MaxFontSize = 409.55;
		}

		public static class Style
		{
			public const int MinIndentLevel = 0;

			public const int MaxIndentLevel = 255;
		}

		public const int MaximumRow = 1048575;

		public const int MaximumColumn = 16383;

		public static readonly ST_Orientation DefaultWorksheetOrientation = ST_Orientation.portrait;

		public const int DefaultHorizontalDpi = 300;

		public const int DefaultVerticalDpi = 300;

		public const int HeaderFooterMaxLength = 255;

		public static readonly DateTime ExcelDateEpoch = new DateTime(1900, 1, 1);

		public const CompressionOption CompressionLevel = CompressionOption.Normal;
	}
}
