namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	internal static class WordRendererStrings
	{
		public const string ColumnsError = "The rendered report contains a table that has more than 63 columns.";

		public const string WidthError = "The maximum page width of the report exceeds 22 inches (55.88 centimeters).";

		public const string WidthMarginsAdjusted = "The left or right margin is either <0 or the sum exceeds the page width.";

		public const string HeightMarginsAdjusted = "The top or bottom margin is either <0 or the sum exceeds the page height.";

		public const string PageWidthExceeded = "The maximum page width exceeded:{0}";

		public const string PageHeightExceeded = "The maximum page height exceeded:{0}";

		public const string InvalidAutofit = "AutoFit value is not valid";
	}
}
