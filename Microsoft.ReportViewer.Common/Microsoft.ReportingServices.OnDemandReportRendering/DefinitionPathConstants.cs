using System.Globalization;
using System.Text;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal static class DefinitionPathConstants
	{
		public const string TablixCorner = "xT";

		public const string TablixRowHierarchy = "xR";

		public const string TablixColHierarchy = "xC";

		public const string TablixBody = "xD";

		public const string TablixHeader = "xH";

		public const string TablixSubMembers = "xM";

		public const string Report = "xA";

		public const string ReportBody = "xB";

		public const string SubReportBody = "xS";

		public const string Page = "xP";

		public const string PageHeader = "xH";

		public const string PageFooter = "xF";

		public const string TextRun = "xL";

		public const string Paragraph = "xK";

		public const string ReportSection = "xE";

		public const char DefinitionPathDelimiter = 'x';

		internal static string GetCollectionDefinitionPath(IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef)
		{
			if (parentDefinitionPath == null || parentDefinitionPath.DefinitionPath == null)
			{
				return indexIntoParentCollectionDef.ToString(CultureInfo.InvariantCulture);
			}
			return parentDefinitionPath.DefinitionPath + "x" + indexIntoParentCollectionDef.ToString(CultureInfo.InvariantCulture);
		}

		internal static string GetTablixHierarchyDefinitionPath(IDefinitionPath parentDefinitionPath, bool isColumn)
		{
			if (isColumn)
			{
				return parentDefinitionPath.DefinitionPath + "xC";
			}
			return parentDefinitionPath.DefinitionPath + "xR";
		}

		internal static string GetTablixCellDefinitionPath(IDefinitionPath parentDefinitionPath, int rowIndex, int colIndex, bool isTablixBodyCell)
		{
			StringBuilder stringBuilder = new StringBuilder(parentDefinitionPath.DefinitionPath);
			if (isTablixBodyCell)
			{
				stringBuilder.Append("xD");
			}
			else
			{
				stringBuilder.Append("xT");
			}
			stringBuilder.Append('x');
			stringBuilder.Append(rowIndex.ToString(CultureInfo.InvariantCulture));
			stringBuilder.Append('x');
			stringBuilder.Append(colIndex.ToString(CultureInfo.InvariantCulture));
			return stringBuilder.ToString();
		}
	}
}
