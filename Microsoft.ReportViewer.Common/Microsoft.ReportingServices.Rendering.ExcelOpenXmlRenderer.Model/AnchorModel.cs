using Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.OXML;
using System;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model
{
	internal sealed class AnchorModel
	{
		private Anchor _interface;

		private readonly int _row;

		private readonly int _column;

		private readonly double _offsetX;

		private readonly double _offsetY;

		public Anchor Interface
		{
			get
			{
				if (_interface == null)
				{
					_interface = new Anchor(this);
				}
				return _interface;
			}
		}

		public int Row => _row;

		public int Column => _column;

		public double OffsetY => _offsetY;

		public double OffsetX => _offsetX;

		public AnchorModel(int row, int column, double offsetX, double offsetY)
		{
			if (row < 0 || column < 0)
			{
				throw new FatalException();
			}
			ValidateOffsetParam(offsetX);
			ValidateOffsetParam(offsetY);
			_row = row;
			_column = column;
			_offsetX = Math.Min(offsetX, 100.0);
			_offsetX = Math.Max(_offsetX, 0.0);
			_offsetY = Math.Min(offsetY, 100.0);
			_offsetY = Math.Max(_offsetY, 0.0);
		}

		private static void ValidateOffsetParam(double param)
		{
			if (param < 0.0 || param > 100.0)
			{
				throw new FatalException();
			}
		}
	}
}
