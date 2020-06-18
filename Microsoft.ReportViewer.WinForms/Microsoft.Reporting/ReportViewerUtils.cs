using System.Drawing.Printing;

namespace Microsoft.Reporting
{
	internal static class ReportViewerUtils
	{
		public static PageSettings DeepClonePageSettings(PageSettings pageSettings)
		{
			if (pageSettings == null)
			{
				return null;
			}
			PageSettings pageSettings2 = (PageSettings)pageSettings.Clone();
			Margins margins = pageSettings.Margins;
			if (margins != null)
			{
				pageSettings2.Margins = (Margins)margins.Clone();
			}
			pageSettings2.PaperSize = new PaperSize();
			pageSettings2.PaperSize.Height = pageSettings.PaperSize.Height;
			pageSettings2.PaperSize.PaperName = pageSettings.PaperSize.PaperName;
			pageSettings2.PaperSize.RawKind = pageSettings.PaperSize.RawKind;
			pageSettings2.PaperSize.Width = pageSettings.PaperSize.Width;
			pageSettings2.PaperSource = new PaperSource();
			pageSettings2.PaperSource.RawKind = pageSettings.PaperSource.RawKind;
			pageSettings2.PaperSource.SourceName = pageSettings.PaperSource.SourceName;
			pageSettings2.PrinterResolution = new PrinterResolution();
			pageSettings2.PrinterResolution.Kind = pageSettings.PrinterResolution.Kind;
			pageSettings2.PrinterResolution.X = pageSettings.PrinterResolution.X;
			pageSettings2.PrinterResolution.Y = pageSettings.PrinterResolution.Y;
			if (pageSettings.PrinterSettings != null)
			{
				pageSettings2.PrinterSettings = pageSettings.PrinterSettings;
			}
			return pageSettings2;
		}
	}
}
