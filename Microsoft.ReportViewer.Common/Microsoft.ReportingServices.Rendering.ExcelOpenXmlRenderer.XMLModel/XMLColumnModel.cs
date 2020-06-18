using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Models;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.OXML;
using System;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class XMLColumnModel : IColumnModel, IOoxmlCtWrapperModel
	{
		private readonly ColumnProperties _interface;

		private readonly CT_Col _col;

		private int _colnumber;

		public ColumnProperties Interface => _interface;

		public double Width
		{
			set
			{
				double num = ConvertPointsToCharacterWidths(value);
				if (num < 0.0)
				{
					num = 0.0;
				}
				else if (num > 255.0)
				{
					num = 255.0;
				}
				_col.Width_Attr = num;
				_col.CustomWidth_Attr = true;
			}
		}

		public bool Hidden
		{
			set
			{
				_col.Hidden_Attr = value;
			}
		}

		public int OutlineLevel
		{
			set
			{
				if (value < 0 || value > 7)
				{
					throw new FatalException();
				}
				_col.OutlineLevel_Attr = (byte)value;
			}
		}

		public bool OutlineCollapsed
		{
			set
			{
				_col.Collapsed_Attr = value;
			}
		}

		public OoxmlComplexType OoxmlTag => _col;

		public XMLColumnModel(CT_Col col, int number)
		{
			_col = col;
			_colnumber = number;
			_interface = new ColumnProperties(this);
			_col.Width_Attr = 9.140625;
		}

		private double ConvertPointsToCharacterWidths(double points)
		{
			double num = points * 96.0 / 72.0;
			double num2 = 7.0;
			return Math.Truncate((Math.Truncate((num - 5.0) / num2 * 100.0 + 0.5) / 100.0 * num2 + 5.0) / num2 * 256.0) / 256.0;
		}

		public void Cleanup()
		{
			_col.Min_Attr = (uint)_colnumber;
			_col.Max_Attr = (uint)_colnumber;
		}
	}
}
