using System.Drawing;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal static class Constants
	{
		public const string DesignerNamespace = "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner";

		public const string ComponentLibraryNamespace = "http://schemas.microsoft.com/sqlserver/reporting/2010/01/componentdefinition";

		public const string SharedDataSetNamespace = "http://schemas.microsoft.com/sqlserver/reporting/2010/01/shareddatasetdefinition";

		public const string DefinitionNamespace = "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition";

		public const string DefaultFontFamilyNamespace = "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition/defaultfontfamily";

		public const string MustUnderstandAttributeName = "MustUnderstand";

		public const char ExpressionPrefix = '=';

		public static readonly ReportSize DefaultPageHeight = new ReportSize(11.0, SizeTypes.Inch);

		public static readonly ReportSize DefaultPageWidth = new ReportSize(8.5, SizeTypes.Inch);

		public static readonly ReportSize DefaultColumnSpacing = new ReportSize(0.5, SizeTypes.Inch);

		public static readonly ReportSize DefaultEmptySize = default(ReportSize);

		public static readonly ReportSize DefaultZeroSize = new ReportSize(0.0);

		public static readonly ReportColor DefaultEmptyColor = ReportColor.Empty;

		public const string DefaultDefaultFontFamily = "Arial";

		public const string DefaultFontFamily = "Arial";

		public const string DefaultGaugeIndicatorNonNumericString = "-";

		public const string DefaultGaugeIndicatorOutOfRangeString = "#Error";

		public static readonly ReportSize DefaultFontSize = new ReportSize(10.0, SizeTypes.Point);

		public static readonly ReportSize MinimumFontSize = new ReportSize(1.0, SizeTypes.Point);

		public static readonly ReportSize MaximumFontSize = new ReportSize(200.0, SizeTypes.Point);

		public static readonly ReportSize MinimumPadding = new ReportSize(0.0, SizeTypes.Point);

		public static readonly ReportSize MaximumPadding = new ReportSize(1000.0, SizeTypes.Point);

		public static readonly ReportSize MinimumLineHeight = new ReportSize(1.0, SizeTypes.Point);

		public static readonly ReportSize MaximumLineHeight = new ReportSize(1000.0, SizeTypes.Point);

		public static readonly ReportColor DefaultColor = new ReportColor(Color.Black);

		public static readonly ReportColor DefaultBorderColor = new ReportColor(Color.Black);

		public static readonly ReportColor DefaultDigitColor = new ReportColor(Color.SteelBlue);

		public static readonly ReportColor DefaultDecimalDigitColor = new ReportColor(Color.Firebrick);

		public static readonly ReportColor DefaultSeparatorColor = new ReportColor(Color.DimGray);

		public static readonly ReportSize DefaultBorderWidth = new ReportSize(1.0, SizeTypes.Point);

		public static readonly ReportSize MinimumBorderWidth = new ReportSize(0.25, SizeTypes.Point);

		public static readonly ReportSize MaximumBorderWidth = new ReportSize(20.0, SizeTypes.Point);

		public static readonly ReportSize MaximumMargin = new ReportSize(455.0, SizeTypes.Inch);

		public static readonly ReportSize MinimumMargin = new ReportSize(0.0, SizeTypes.Inch);

		public static readonly ReportSize MinimumItemSize = new ReportSize(0.0, SizeTypes.Inch);

		public static readonly ReportSize MaximumItemSize = new ReportSize(455.0, SizeTypes.Inch);
	}
}
