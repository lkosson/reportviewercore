using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.OXML;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class XMLCellController : ICellController
	{
		private readonly CT_Cell _cell;

		private readonly XMLWorkbookModel _workbook;

		private readonly XMLWorksheetModel _sheet;

		private readonly PartManager _manager;

		private XMLCharacterRunManager _charManager;

		private XMLStyleModel _style;

		private bool _holdsOwnStyle;

		public object Value
		{
			get
			{
				if (_cell.T_Attr == ST_CellType.inlineStr)
				{
					if (_cell.Is != null && _cell.Is.R != null && _cell.Is.R.Count > 0)
					{
						string text = "";
						{
							foreach (CT_RElt item in _cell.Is.R)
							{
								text += item.T;
							}
							return text;
						}
					}
					if (_cell.Is == null)
					{
						return null;
					}
					return _cell.Is.T;
				}
				if (_cell.V == null)
				{
					return null;
				}
				if (_cell.T_Attr == ST_CellType.b)
				{
					switch (_cell.V.ToUpper(CultureInfo.InvariantCulture).Trim())
					{
					case "TRUE":
						return true;
					case "FALSE":
						return false;
					case "1":
						return true;
					case "0":
						return false;
					default:
						throw new FatalException();
					}
				}
				if (_cell.T_Attr == ST_CellType.d)
				{
					return DateTime.FromOADate(double.Parse(_cell.V, CultureInfo.InvariantCulture));
				}
				if (_cell.T_Attr == ST_CellType.n)
				{
					if (long.TryParse(_cell.V, out long result))
					{
						return result;
					}
					if (double.TryParse(_cell.V, out double result2))
					{
						return result2;
					}
					throw new FatalException();
				}
				return null;
			}
			set
			{
				_cell.V = null;
				_cell.Is = null;
				if (value == null)
				{
					_cell.V = null;
				}
				else if (value is bool)
				{
					_cell.T_Attr = ST_CellType.b;
					_cell.V = (((bool)value) ? "1" : "0");
				}
				else if (value is DateTime)
				{
					_cell.T_Attr = ST_CellType.n;
					double num = ((DateTime)value).ToOADate();
					if (num <= 60.0)
					{
						num -= 1.0;
					}
					_cell.V = num.ToString(CultureInfo.InvariantCulture);
				}
				else if (value is ExcelErrorCode)
				{
					_cell.T_Attr = ST_CellType.e;
					switch ((ExcelErrorCode)value)
					{
					case ExcelErrorCode.DivByZeroError:
						_cell.V = "#DIV/0!";
						break;
					case ExcelErrorCode.NameError:
						_cell.V = "#NAME?";
						break;
					case ExcelErrorCode.ValueError:
						_cell.V = "#VALUE!";
						break;
					case ExcelErrorCode.NullError:
						_cell.V = "#NULL!";
						break;
					case ExcelErrorCode.NumError:
						_cell.V = "#NUM!";
						break;
					case ExcelErrorCode.RefError:
						_cell.V = "#REF!";
						break;
					case ExcelErrorCode.NAError:
						_cell.V = "#N/A";
						break;
					default:
						_cell.V = "#VALUE!";
						break;
					}
				}
				else if (value is string)
				{
					_cell.T_Attr = ST_CellType.inlineStr;
					_cell.Is = new CT_Rst();
					_cell.Is.T = value.ToString();
				}
				else if (value is sbyte || value is byte || value is char || value is short || value is ushort || value is int || value is uint || value is long || value is ulong || value is float || value is double || value is decimal)
				{
					_cell.T_Attr = ST_CellType.n;
					if (value is float)
					{
						if (IsNotValidFloat((float)value))
						{
							SetDivByZeroError();
						}
						else
						{
							_cell.V = ((float)value).ToString(CultureInfo.InvariantCulture);
						}
					}
					else if (value is double)
					{
						if (IsNotValidDouble((double)value))
						{
							SetDivByZeroError();
						}
						else
						{
							_cell.V = ((double)value).ToString(CultureInfo.InvariantCulture);
						}
					}
					else if (value is decimal)
					{
						_cell.V = ((decimal)value).ToString(CultureInfo.InvariantCulture);
					}
					else
					{
						_cell.V = value.ToString();
					}
				}
				else
				{
					Value = value.ToString();
				}
			}
		}

		public Cell.CellValueType ValueType
		{
			get
			{
				if (_cell.T_Attr == ST_CellType.b)
				{
					return Cell.CellValueType.Boolean;
				}
				if (_cell.T_Attr == ST_CellType.d)
				{
					return Cell.CellValueType.Date;
				}
				if (_cell.T_Attr == ST_CellType.e)
				{
					return Cell.CellValueType.Error;
				}
				if (_cell.T_Attr == ST_CellType.inlineStr)
				{
					return Cell.CellValueType.Text;
				}
				if (_cell.T_Attr == ST_CellType.n)
				{
					if (_cell.V != null && _cell.V.Contains("."))
					{
						return Cell.CellValueType.Double;
					}
					return Cell.CellValueType.Integer;
				}
				return Cell.CellValueType.Text;
			}
		}

		public IStyleModel Style
		{
			get
			{
				if (_style == null)
				{
					_style = _manager.StyleSheet.CreateStyle(_cell.S_Attr);
				}
				else if (!_holdsOwnStyle)
				{
					_style = _manager.StyleSheet.CreateStyle(_style);
				}
				_holdsOwnStyle = true;
				return _style;
			}
			set
			{
				_style = (XMLStyleModel)value;
				_holdsOwnStyle = false;
			}
		}

		public XMLCharacterRunManager CharManager
		{
			get
			{
				if (_charManager == null)
				{
					if (_cell.Is == null)
					{
						_cell.Is = new CT_Rst();
						_cell.Is.R = new List<CT_RElt>();
						_cell.Is.R.Add(new CT_RElt());
						_cell.Is.R[0].T = (Value.ToString() ?? "");
					}
					else if (_cell.Is.T != null)
					{
						_cell.Is.R = new List<CT_RElt>();
						_cell.Is.R.Add(new CT_RElt());
						_cell.Is.R[0].T = _cell.Is.T;
						_cell.Is.T = null;
					}
					_cell.T_Attr = ST_CellType.inlineStr;
					_charManager = new XMLCharacterRunManager(_cell.Is, _manager.StyleSheet.Palette);
				}
				return _charManager;
			}
		}

		public XMLCellController(CT_Cell cell, XMLWorksheetModel sheet, PartManager manager)
		{
			_cell = cell;
			_workbook = (XMLWorkbookModel)sheet.Workbook;
			_sheet = sheet;
			_manager = manager;
		}

		public void Cleanup()
		{
			if (_style != null)
			{
				if (_style.Index == 0)
				{
					_cell.S_Attr = _manager.StyleSheet.CommitStyle(_style);
				}
				else
				{
					_cell.S_Attr = _style.Index;
				}
			}
		}

		public bool IsNotValidDouble(double value)
		{
			if (!double.IsNaN(value))
			{
				return double.IsInfinity(value);
			}
			return true;
		}

		public bool IsNotValidFloat(float value)
		{
			if (!float.IsNaN(value))
			{
				return float.IsInfinity(value);
			}
			return true;
		}

		private void SetDivByZeroError()
		{
			_cell.T_Attr = ST_CellType.e;
			_cell.V = "#DIV/0!";
		}
	}
}
