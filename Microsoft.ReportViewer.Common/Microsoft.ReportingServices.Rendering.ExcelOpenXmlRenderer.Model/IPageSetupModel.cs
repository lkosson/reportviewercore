namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model
{
	internal interface IPageSetupModel
	{
		PageSetup Interface
		{
			get;
		}

		PageSetup.PageOrientation Orientation
		{
			set;
		}

		PageSetup.PagePaperSize PaperSize
		{
			set;
		}

		double LeftMargin
		{
			set;
		}

		double RightMargin
		{
			set;
		}

		double TopMargin
		{
			set;
		}

		double BottomMargin
		{
			set;
		}

		double FooterMargin
		{
			set;
		}

		double HeaderMargin
		{
			set;
		}

		string RightFooter
		{
			set;
		}

		string CenterFooter
		{
			set;
		}

		string LeftFooter
		{
			set;
		}

		string RightHeader
		{
			set;
		}

		string CenterHeader
		{
			set;
		}

		string LeftHeader
		{
			set;
		}

		bool UseZoom
		{
			get;
			set;
		}

		bool SummaryRowsBelow
		{
			set;
		}

		bool SummaryColumnsRight
		{
			set;
		}

		void SetPrintTitleToRows(int firstRow, int lastRow);
	}
}
