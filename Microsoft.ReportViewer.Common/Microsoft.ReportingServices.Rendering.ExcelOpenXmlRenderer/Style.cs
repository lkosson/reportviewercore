using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.officeDocument.x2006.sharedTypes;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer
{
	internal class Style : IStyle, IFont
	{
		private IStyleModel mModel;

		public IColor BackgroundColor
		{
			set
			{
				if (value != null || mModel.BackgroundColor != null)
				{
					ColorModel colorModel = (value == null) ? null : ((Color)value).Model;
					Clone(mModel.BackgroundColor, colorModel);
					mModel.BackgroundColor = colorModel;
				}
			}
		}

		public Font Font => mModel.Font.Interface;

		public HorizontalAlignment HorizontalAlignment
		{
			set
			{
				ST_HorizontalAlignment sT_HorizontalAlignment = ToInternalHorizontalAlignment(value);
				Clone(mModel.HorizontalAlignment, sT_HorizontalAlignment);
				mModel.HorizontalAlignment = sT_HorizontalAlignment;
			}
		}

		public int IndentLevel
		{
			set
			{
				if (value < 0)
				{
					CloneNullable(mModel.IndentLevel, 0);
					mModel.IndentLevel = 0;
				}
				else if (value > 255)
				{
					CloneNullable(mModel.IndentLevel, 255);
					mModel.IndentLevel = 255;
				}
				else
				{
					CloneNullable(mModel.IndentLevel, value);
					mModel.IndentLevel = value;
				}
			}
		}

		internal IStyleModel Model => mModel;

		public string NumberFormat
		{
			set
			{
				Clone(mModel.NumberFormat, value);
				mModel.NumberFormat = value;
			}
		}

		public int Orientation
		{
			set
			{
				if (value == Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel.Orientation.Rotate90ClockWise)
				{
					CloneNullable(mModel.Orientation, 180);
					mModel.Orientation = 180;
				}
				else
				{
					CloneNullable(mModel.Orientation, value);
					mModel.Orientation = value;
				}
			}
		}

		public TextDirection TextDirection
		{
			set
			{
				byte b = ToInternalTextDirection(value);
				CloneNullable(mModel.TextDirection, b);
				mModel.TextDirection = b;
			}
		}

		public VerticalAlignment VerticalAlignment
		{
			set
			{
				ST_VerticalAlignment sT_VerticalAlignment = ToInternalVerticalAlignment(value);
				Clone(mModel.VerticalAlignment, sT_VerticalAlignment);
				mModel.VerticalAlignment = sT_VerticalAlignment;
			}
		}

		public bool WrapText
		{
			set
			{
				CloneNullable(mModel.WrapText, value);
				mModel.WrapText = value;
			}
		}

		public int Bold
		{
			set
			{
				bool flag = Font.ToInternalBold(value);
				CloneValueType(mModel.Font.Bold, flag);
				mModel.Font.Bold = flag;
			}
		}

		public bool Italic
		{
			set
			{
				CloneValueType(mModel.Font.Italic, value);
				mModel.Font.Italic = value;
			}
		}

		public bool Strikethrough
		{
			set
			{
				CloneValueType(mModel.Font.Strikethrough, value);
				mModel.Font.Strikethrough = value;
			}
		}

		public ScriptStyle ScriptStyle
		{
			set
			{
				ST_VerticalAlignRun sT_VerticalAlignRun = Font.ToInternalScriptStyle(value);
				Clone(mModel.Font.ScriptStyle, sT_VerticalAlignRun);
				mModel.Font.ScriptStyle = sT_VerticalAlignRun;
			}
		}

		public IColor Color
		{
			set
			{
				ColorModel colorModel = Font.ToInternalColor(value);
				Clone(mModel.Font.Color, colorModel);
				mModel.Font.Color = colorModel;
			}
		}

		public Underline Underline
		{
			set
			{
				ST_UnderlineValues sT_UnderlineValues = Font.ToInternalUnderline(value);
				Clone(mModel.Font.Underline, sT_UnderlineValues);
				mModel.Font.Underline = sT_UnderlineValues;
			}
		}

		public string Name
		{
			set
			{
				Clone(mModel.Font.Name, value);
				mModel.Font.Name = value;
			}
		}

		public double Size
		{
			set
			{
				value = Font.ToInternalSize(value);
				CloneValueType(mModel.Font.Size, value);
				mModel.Font.Size = value;
			}
		}

		public ExcelBorderStyle BorderLeftStyle
		{
			set
			{
				ST_BorderStyle sT_BorderStyle = ToInternalBorderStyle(value);
				Clone(mModel.BorderModel.LeftBorder.Style, sT_BorderStyle);
				mModel.BorderModel.LeftBorder.Style = sT_BorderStyle;
			}
		}

		public ExcelBorderStyle BorderRightStyle
		{
			set
			{
				ST_BorderStyle sT_BorderStyle = ToInternalBorderStyle(value);
				Clone(mModel.BorderModel.RightBorder.Style, sT_BorderStyle);
				mModel.BorderModel.RightBorder.Style = sT_BorderStyle;
			}
		}

		public ExcelBorderStyle BorderTopStyle
		{
			set
			{
				ST_BorderStyle sT_BorderStyle = ToInternalBorderStyle(value);
				Clone(mModel.BorderModel.TopBorder.Style, sT_BorderStyle);
				mModel.BorderModel.TopBorder.Style = sT_BorderStyle;
			}
		}

		public ExcelBorderStyle BorderBottomStyle
		{
			set
			{
				ST_BorderStyle sT_BorderStyle = ToInternalBorderStyle(value);
				Clone(mModel.BorderModel.BottomBorder.Style, sT_BorderStyle);
				mModel.BorderModel.BottomBorder.Style = sT_BorderStyle;
			}
		}

		public ExcelBorderStyle BorderDiagStyle
		{
			set
			{
				ST_BorderStyle sT_BorderStyle = ToInternalBorderStyle(value);
				Clone(mModel.BorderModel.DiagonalBorder.Style, sT_BorderStyle);
				mModel.BorderModel.DiagonalBorder.Style = sT_BorderStyle;
			}
		}

		public ExcelBorderPart BorderDiagPart
		{
			set
			{
				mModel.BorderModel.DiagonalPartDirection = value;
			}
		}

		public IColor BorderLeftColor
		{
			set
			{
				ColorModel colorModel = (value == null) ? null : ((Color)value).Model;
				Clone(mModel.BorderModel.LeftBorder.Color, colorModel);
				mModel.BorderModel.LeftBorder.Color = colorModel;
			}
		}

		public IColor BorderRightColor
		{
			set
			{
				ColorModel colorModel = (value == null) ? null : ((Color)value).Model;
				Clone(mModel.BorderModel.RightBorder.Color, colorModel);
				mModel.BorderModel.RightBorder.Color = colorModel;
			}
		}

		public IColor BorderTopColor
		{
			set
			{
				ColorModel colorModel = (value == null) ? null : ((Color)value).Model;
				Clone(mModel.BorderModel.TopBorder.Color, colorModel);
				mModel.BorderModel.TopBorder.Color = colorModel;
			}
		}

		public IColor BorderBottomColor
		{
			set
			{
				ColorModel colorModel = (value == null) ? null : ((Color)value).Model;
				Clone(mModel.BorderModel.BottomBorder.Color, colorModel);
				mModel.BorderModel.BottomBorder.Color = colorModel;
			}
		}

		public IColor BorderDiagColor
		{
			set
			{
				ColorModel colorModel = (value == null) ? null : ((Color)value).Model;
				Clone(mModel.BorderModel.DiagonalBorder.Color, colorModel);
				mModel.BorderModel.DiagonalBorder.Color = colorModel;
			}
		}

		public bool HasBeenModified => mModel.HasBeenModified;

		internal Style(IStyleModel model)
		{
			mModel = model;
		}

		private void Clone<T>(T currentValue, T newValue) where T : class
		{
			if (((newValue == null) ^ (currentValue == null)) || (newValue != null && !newValue.Equals(currentValue)))
			{
				mModel = mModel.cloneStyle(cellStyle: true);
			}
		}

		private void CloneNullable<T>(T? currentValue, T newValue) where T : struct
		{
			if (!currentValue.HasValue || !newValue.Equals(currentValue))
			{
				mModel = mModel.cloneStyle(cellStyle: true);
			}
		}

		private void CloneValueType<T>(T currentValue, T newValue) where T : struct
		{
			if (!newValue.Equals(currentValue))
			{
				mModel = mModel.cloneStyle(cellStyle: true);
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is Style))
			{
				return false;
			}
			if (obj == this)
			{
				return true;
			}
			return ((Style)obj).mModel.Equals(mModel);
		}

		public override int GetHashCode()
		{
			return mModel.GetHashCode();
		}

		private static ST_BorderStyle ToInternalBorderStyle(ExcelBorderStyle borderStyle)
		{
			switch (borderStyle)
			{
			case ExcelBorderStyle.None:
				return ST_BorderStyle.none;
			case ExcelBorderStyle.Thin:
				return ST_BorderStyle.thin;
			case ExcelBorderStyle.Medium:
				return ST_BorderStyle.medium;
			case ExcelBorderStyle.Dashed:
				return ST_BorderStyle.dashed;
			case ExcelBorderStyle.Dotted:
				return ST_BorderStyle.dotted;
			case ExcelBorderStyle.Thick:
				return ST_BorderStyle.thick;
			case ExcelBorderStyle.Double:
				return ST_BorderStyle._double;
			case ExcelBorderStyle.Hair:
				return ST_BorderStyle.hair;
			case ExcelBorderStyle.MedDashed:
				return ST_BorderStyle.mediumDashed;
			case ExcelBorderStyle.DashDot:
				return ST_BorderStyle.dashDot;
			case ExcelBorderStyle.MedDashDot:
				return ST_BorderStyle.mediumDashDot;
			case ExcelBorderStyle.DashDotDot:
				return ST_BorderStyle.dashDotDot;
			case ExcelBorderStyle.MedDashDotDot:
				return ST_BorderStyle.mediumDashDotDot;
			case ExcelBorderStyle.SlantedDashDot:
				return ST_BorderStyle.slantDashDot;
			default:
				return ST_BorderStyle.none;
			}
		}

		private static ST_HorizontalAlignment ToInternalHorizontalAlignment(HorizontalAlignment horizontalAlignment)
		{
			switch (horizontalAlignment)
			{
			case HorizontalAlignment.Left:
				return ST_HorizontalAlignment.left;
			case HorizontalAlignment.Center:
				return ST_HorizontalAlignment.center;
			case HorizontalAlignment.Right:
				return ST_HorizontalAlignment.right;
			case HorizontalAlignment.Fill:
				return ST_HorizontalAlignment.fill;
			case HorizontalAlignment.Justify:
				return ST_HorizontalAlignment.justify;
			case HorizontalAlignment.CenterAcrossSelection:
				return ST_HorizontalAlignment.centerContinuous;
			case HorizontalAlignment.Distributed:
				return ST_HorizontalAlignment.distributed;
			default:
				return ST_HorizontalAlignment.general;
			}
		}

		private static byte ToInternalTextDirection(TextDirection textDirection)
		{
			switch (textDirection)
			{
			case TextDirection.LeftToRight:
				return 1;
			case TextDirection.RightToLeft:
				return 2;
			default:
				return 0;
			}
		}

		private static ST_VerticalAlignment ToInternalVerticalAlignment(VerticalAlignment verticalAlignment)
		{
			switch (verticalAlignment)
			{
			case VerticalAlignment.Center:
				return ST_VerticalAlignment.center;
			case VerticalAlignment.Distributed:
				return ST_VerticalAlignment.distributed;
			case VerticalAlignment.Justify:
				return ST_VerticalAlignment.justify;
			case VerticalAlignment.Top:
				return ST_VerticalAlignment.top;
			default:
				return ST_VerticalAlignment.bottom;
			}
		}
	}
}
