using System;
using System.Drawing.Printing;

namespace Microsoft.Reporting.WinForms
{
	[Serializable]
	public sealed class ReportPageSettings
	{
		private int m_pageWidth;

		private int m_pageHeight;

		private Margins m_margins;

		public PaperSize PaperSize
		{
			get
			{
				PageSettings customPageSettings = CustomPageSettings;
				UpdatePageSettingsForPrinter(customPageSettings, new PrinterSettings());
				return customPageSettings.PaperSize;
			}
		}

		public Margins Margins => (Margins)m_margins.Clone();

		public bool IsLandscape => m_pageWidth > m_pageHeight;

		internal PageSettings CustomPageSettings
		{
			get
			{
				PageSettings pageSettings = new PageSettings();
				int width = Math.Min(m_pageWidth, m_pageHeight);
				int height = Math.Max(m_pageWidth, m_pageHeight);
				pageSettings.PaperSize = new PaperSize("", width, height);
				pageSettings.Landscape = IsLandscape;
				pageSettings.Margins = Margins;
				return pageSettings;
			}
		}

		internal ReportPageSettings(double pageHeight, double pageWidth, double leftMargin, double rightMargin, double topMargin, double bottomMargin)
		{
			m_pageWidth = ConvertMmTo100thInch(pageWidth);
			m_pageHeight = ConvertMmTo100thInch(pageHeight);
			m_margins = new Margins(ConvertMmTo100thInch(leftMargin), ConvertMmTo100thInch(rightMargin), ConvertMmTo100thInch(topMargin), ConvertMmTo100thInch(bottomMargin));
		}

		internal ReportPageSettings()
			: this(1100.0, 850.0, 50.0, 50.0, 50.0, 50.0)
		{
		}

		private static int ConvertMmTo100thInch(double mm)
		{
			return (int)Math.Round(mm / 25.4 * 100.0);
		}

		internal static void UpdatePageSettingsForPrinter(PageSettings pageSettings, PrinterSettings printerSettings)
		{
			if (!printerSettings.IsValid)
			{
				return;
			}
			int num = pageSettings.Landscape ? pageSettings.PaperSize.Height : pageSettings.PaperSize.Width;
			int num2 = pageSettings.Landscape ? pageSettings.PaperSize.Width : pageSettings.PaperSize.Height;
			foreach (PaperSize paperSize in printerSettings.PaperSizes)
			{
				if (num == paperSize.Width && num2 == paperSize.Height)
				{
					pageSettings.Landscape = false;
					pageSettings.PaperSize = paperSize;
					break;
				}
				if (num == paperSize.Height && num2 == paperSize.Width)
				{
					pageSettings.Landscape = true;
					pageSettings.PaperSize = paperSize;
					break;
				}
			}
			pageSettings.PrinterSettings = printerSettings;
		}

		internal PageSettings ToPageSettings(PrinterSettings currentPrinter)
		{
			PageSettings customPageSettings = CustomPageSettings;
			UpdatePageSettingsForPrinter(customPageSettings, currentPrinter);
			customPageSettings.Margins = Margins;
			return customPageSettings;
		}
	}
}
