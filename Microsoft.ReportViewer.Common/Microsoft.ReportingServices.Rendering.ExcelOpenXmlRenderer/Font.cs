using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.officeDocument.x2006.sharedTypes;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel;
using System;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer
{
	internal sealed class Font : IFont
	{
		private readonly IFontModel mModel;

		public int Bold
		{
			set
			{
				mModel.Bold = ToInternalBold(value);
			}
		}

		public IColor Color
		{
			set
			{
				mModel.Color = ToInternalColor(value);
			}
		}

		public bool Italic
		{
			set
			{
				mModel.Italic = value;
			}
		}

		internal IFontModel Model => mModel;

		public string Name
		{
			set
			{
				mModel.Name = value;
			}
		}

		public ScriptStyle ScriptStyle
		{
			set
			{
				mModel.ScriptStyle = ToInternalScriptStyle(value);
			}
		}

		public double Size
		{
			set
			{
				mModel.Size = ToInternalSize(value);
			}
		}

		public bool Strikethrough
		{
			set
			{
				mModel.Strikethrough = value;
			}
		}

		public Underline Underline
		{
			set
			{
				mModel.Underline = ToInternalUnderline(value);
			}
		}

		internal Font(IFontModel model)
		{
			mModel = model;
		}

		public void SetFont(Font font)
		{
			mModel.SetFont(font.Model);
		}

		public static bool ToInternalBold(int bold)
		{
			return bold > 500;
		}

		public static ColorModel ToInternalColor(IColor color)
		{
			return ((Color)color).Model;
		}

		public static ST_VerticalAlignRun ToInternalScriptStyle(ScriptStyle scriptStyle)
		{
			switch (scriptStyle)
			{
			case ScriptStyle.Superscript:
				return ST_VerticalAlignRun.superscript;
			case ScriptStyle.Subscript:
				return ST_VerticalAlignRun.subscript;
			default:
				return ST_VerticalAlignRun.baseline;
			}
		}

		public static double ToInternalSize(double size)
		{
			return Math.Max(1.0, Math.Min(409.55, size));
		}

		public static ST_UnderlineValues ToInternalUnderline(Underline underline)
		{
			switch (underline)
			{
			case Underline.Single:
				return ST_UnderlineValues.single;
			case Underline.Double:
				return ST_UnderlineValues._double;
			case Underline.Accounting:
				return ST_UnderlineValues.singleAccounting;
			case Underline.DoubleAccounting:
				return ST_UnderlineValues.doubleAccounting;
			default:
				return ST_UnderlineValues.none;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is Font))
			{
				return false;
			}
			if (obj == this)
			{
				return true;
			}
			return ((Font)obj).mModel.Equals(mModel);
		}

		public override int GetHashCode()
		{
			return mModel.GetHashCode();
		}
	}
}
