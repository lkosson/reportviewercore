namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.drawingml.x2006.spreadsheetDrawing
{
	internal class ST_EditAs
	{
		private string _ooxmlEnumerationValue;

		private static ST_EditAs _twoCell;

		private static ST_EditAs _oneCell;

		private static ST_EditAs _absolute;

		public static ST_EditAs twoCell
		{
			get
			{
				return _twoCell;
			}
			private set
			{
				_twoCell = value;
			}
		}

		public static ST_EditAs oneCell
		{
			get
			{
				return _oneCell;
			}
			private set
			{
				_oneCell = value;
			}
		}

		public static ST_EditAs absolute
		{
			get
			{
				return _absolute;
			}
			private set
			{
				_absolute = value;
			}
		}

		static ST_EditAs()
		{
			twoCell = new ST_EditAs("twoCell");
			oneCell = new ST_EditAs("oneCell");
			absolute = new ST_EditAs("absolute");
		}

		private ST_EditAs(string val)
		{
			_ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return _ooxmlEnumerationValue;
		}

		public bool Equals(ST_EditAs other)
		{
			if (other == null)
			{
				return false;
			}
			return _ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_EditAs one, ST_EditAs two)
		{
			if ((object)one == null && (object)two == null)
			{
				return true;
			}
			if ((object)one == null || (object)two == null)
			{
				return false;
			}
			return one._ooxmlEnumerationValue == two._ooxmlEnumerationValue;
		}

		public static bool operator !=(ST_EditAs one, ST_EditAs two)
		{
			return !(one == two);
		}

		public override int GetHashCode()
		{
			return _ooxmlEnumerationValue.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}
	}
}
