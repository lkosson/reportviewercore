using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.OXML;
using System.Globalization;
using System.Text;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class XMLPageSetupModel : IPageSetupModel
	{
		private readonly PageSetup _interface;

		private readonly CT_Worksheet _sheet;

		private readonly XMLWorksheetModel _sheetModel;

		private string _rightHeader;

		private string _centerHeader;

		private string _leftHeader;

		private string _rightFooter;

		private string _centerFooter;

		private string _leftFooter;

		private bool _useZoom;

		public CT_Worksheet BackingSheet => _sheet;

		public PageSetup Interface => _interface;

		public PageSetup.PageOrientation Orientation
		{
			set
			{
				if (value == PageSetup.PageOrientation.Landscape)
				{
					_sheet.PageSetup.Orientation_Attr = ST_Orientation.landscape;
					return;
				}
				if (value == PageSetup.PageOrientation.Portrait)
				{
					_sheet.PageSetup.Orientation_Attr = ST_Orientation.portrait;
					return;
				}
				throw new FatalException();
			}
		}

		public PageSetup.PagePaperSize PaperSize
		{
			set
			{
				_sheet.PageSetup.PaperSize_Attr = (uint)value.Value;
			}
		}

		public double LeftMargin
		{
			set
			{
				TryValidMargin(value);
				_sheet.PageMargins.Left_Attr = value;
			}
		}

		public double RightMargin
		{
			set
			{
				TryValidMargin(value);
				_sheet.PageMargins.Right_Attr = value;
			}
		}

		public double TopMargin
		{
			set
			{
				TryValidMargin(value);
				_sheet.PageMargins.Top_Attr = value;
			}
		}

		public double BottomMargin
		{
			set
			{
				TryValidMargin(value);
				_sheet.PageMargins.Bottom_Attr = value;
			}
		}

		public double FooterMargin
		{
			set
			{
				TryValidMargin(value);
				_sheet.PageMargins.Footer_Attr = value;
			}
		}

		public double HeaderMargin
		{
			set
			{
				TryValidMargin(value);
				_sheet.PageMargins.Header_Attr = value;
			}
		}

		public string RightFooter
		{
			set
			{
				TryValidHeaderFooter(_leftFooter, _centerFooter, value);
				_rightFooter = value;
			}
		}

		public string CenterFooter
		{
			set
			{
				TryValidHeaderFooter(_leftFooter, value, _rightFooter);
				_centerFooter = value;
			}
		}

		public string LeftFooter
		{
			set
			{
				TryValidHeaderFooter(value, _centerFooter, _rightFooter);
				_leftFooter = value;
			}
		}

		public string RightHeader
		{
			set
			{
				TryValidHeaderFooter(_leftHeader, _centerHeader, value);
				_rightHeader = value;
			}
		}

		public string CenterHeader
		{
			set
			{
				TryValidHeaderFooter(_leftHeader, value, _rightHeader);
				_centerHeader = value;
			}
		}

		public string LeftHeader
		{
			set
			{
				TryValidHeaderFooter(value, _centerHeader, _rightHeader);
				_leftHeader = value;
			}
		}

		public bool UseZoom
		{
			get
			{
				return _useZoom;
			}
			set
			{
				_useZoom = value;
			}
		}

		public bool SummaryRowsBelow
		{
			set
			{
				GetOutlineProperties().SummaryBelow_Attr = value;
			}
		}

		public bool SummaryColumnsRight
		{
			set
			{
				GetOutlineProperties().SummaryRight_Attr = value;
			}
		}

		public XMLPageSetupModel(CT_Worksheet sheet, XMLWorksheetModel sheetModel)
		{
			if (sheet == null || sheetModel == null)
			{
				throw new FatalException();
			}
			_sheet = sheet;
			_sheetModel = sheetModel;
			_interface = new PageSetup(this);
			if (_sheet.PageMargins == null)
			{
				SetDefaultMargins();
			}
			if (_sheet.PageSetup == null)
			{
				SetDefaultPageSetup();
			}
			SetupUseZoom();
		}

		private void SetDefaultMargins()
		{
			if (_sheet.PageMargins == null)
			{
				_sheet.PageMargins = new CT_PageMargins();
			}
			_sheet.PageMargins.Bottom_Attr = 0.75;
			_sheet.PageMargins.Footer_Attr = 0.3;
			_sheet.PageMargins.Header_Attr = 0.3;
			_sheet.PageMargins.Left_Attr = 0.7;
			_sheet.PageMargins.Right_Attr = 0.7;
			_sheet.PageMargins.Top_Attr = 0.75;
		}

		private void SetDefaultPageSetup()
		{
			if (_sheet.PageSetup == null)
			{
				_sheet.PageSetup = new CT_PageSetup();
			}
			_sheet.PageSetup.Orientation_Attr = XMLConstants.DefaultWorksheetOrientation;
			_sheet.PageSetup.HorizontalDpi_Attr = 300u;
			_sheet.PageSetup.VerticalDpi_Attr = 300u;
		}

		private void SetupUseZoom()
		{
			if (_sheet.PageSetup.FitToHeight_Attr != 1 || _sheet.PageSetup.FitToWidth_Attr != 1)
			{
				UseZoom = false;
			}
			else if (_sheet.PageSetup.Scale_Attr != 100)
			{
				UseZoom = true;
			}
			else
			{
				UseZoom = true;
			}
		}

		private static void TryValidMargin(double margin)
		{
			if (margin < 0.0 || margin >= 49.0)
			{
				throw new FatalException();
			}
		}

		private static void TryValidHeaderFooter(string left, string center, string right)
		{
			int num = 0;
			string[] array = new string[3]
			{
				left,
				center,
				right
			};
			foreach (string text in array)
			{
				if (!string.IsNullOrEmpty(text))
				{
					num += 2;
					num += text.Length;
				}
			}
			if (num > 255)
			{
				throw new FatalException();
			}
		}

		private CT_OutlinePr GetOutlineProperties()
		{
			if (_sheet.SheetPr == null)
			{
				_sheet.SheetPr = new CT_SheetPr();
			}
			if (_sheet.SheetPr.OutlinePr == null)
			{
				_sheet.SheetPr.OutlinePr = new CT_OutlinePr();
			}
			return _sheet.SheetPr.OutlinePr;
		}

		public void SetPrintTitleToRows(int firstRow, int lastRow)
		{
			XMLDefinedName xMLDefinedName = _sheetModel.NameManager.CreateDefinedName("_xlnm.Print_Titles");
			xMLDefinedName.Content = string.Format(CultureInfo.InvariantCulture, "'{0}'!${1}:${2}", _sheetModel.Name.Replace("'", "''"), firstRow + 1, lastRow + 1);
			xMLDefinedName.SheetIndex = _sheetModel.Position;
		}

		private static string BuildHeaderFooter(string left, string center, string right)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (!string.IsNullOrEmpty(left))
			{
				stringBuilder.AppendFormat("&L{0}", left);
			}
			if (!string.IsNullOrEmpty(center))
			{
				stringBuilder.AppendFormat("&C{0}", center);
			}
			if (!string.IsNullOrEmpty(right))
			{
				stringBuilder.AppendFormat("&R{0}", right);
			}
			return stringBuilder.ToString();
		}

		public void Cleanup()
		{
			if (_leftHeader != null || _centerHeader != null || _rightHeader != null)
			{
				if (_sheet.HeaderFooter == null)
				{
					_sheet.HeaderFooter = new CT_HeaderFooter();
				}
				_sheet.HeaderFooter.OddHeader = BuildHeaderFooter(_leftHeader, _centerHeader, _rightHeader);
			}
			if (_leftFooter != null || _centerFooter != null || _rightFooter != null)
			{
				if (_sheet.HeaderFooter == null)
				{
					_sheet.HeaderFooter = new CT_HeaderFooter();
				}
				_sheet.HeaderFooter.OddFooter = BuildHeaderFooter(_leftFooter, _centerFooter, _rightFooter);
			}
			if (UseZoom)
			{
				_sheet.PageSetup.FitToHeight_Attr = 1u;
				_sheet.PageSetup.FitToWidth_Attr = 1u;
				return;
			}
			if (_sheet.SheetPr == null)
			{
				_sheet.SheetPr = new CT_SheetPr();
			}
			_sheet.PageSetup.Scale_Attr = 100u;
		}
	}
}
