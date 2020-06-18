namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel
{
	internal interface IStyle : IFont
	{
		ExcelBorderStyle BorderLeftStyle
		{
			set;
		}

		ExcelBorderStyle BorderRightStyle
		{
			set;
		}

		ExcelBorderStyle BorderTopStyle
		{
			set;
		}

		ExcelBorderStyle BorderBottomStyle
		{
			set;
		}

		ExcelBorderStyle BorderDiagStyle
		{
			set;
		}

		IColor BorderLeftColor
		{
			set;
		}

		IColor BorderRightColor
		{
			set;
		}

		IColor BorderTopColor
		{
			set;
		}

		IColor BorderBottomColor
		{
			set;
		}

		IColor BorderDiagColor
		{
			set;
		}

		ExcelBorderPart BorderDiagPart
		{
			set;
		}

		IColor BackgroundColor
		{
			set;
		}

		int IndentLevel
		{
			set;
		}

		bool WrapText
		{
			set;
		}

		int Orientation
		{
			set;
		}

		string NumberFormat
		{
			set;
		}

		HorizontalAlignment HorizontalAlignment
		{
			set;
		}

		VerticalAlignment VerticalAlignment
		{
			set;
		}

		TextDirection TextDirection
		{
			set;
		}
	}
}
