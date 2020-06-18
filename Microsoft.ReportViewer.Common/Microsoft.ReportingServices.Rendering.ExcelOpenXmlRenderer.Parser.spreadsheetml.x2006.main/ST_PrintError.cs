namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class ST_PrintError
	{
		private string _ooxmlEnumerationValue;

		private static ST_PrintError _displayed;

		private static ST_PrintError _blank;

		private static ST_PrintError _dash;

		private static ST_PrintError _NA;

		public static ST_PrintError displayed
		{
			get
			{
				return _displayed;
			}
			private set
			{
				_displayed = value;
			}
		}

		public static ST_PrintError blank
		{
			get
			{
				return _blank;
			}
			private set
			{
				_blank = value;
			}
		}

		public static ST_PrintError dash
		{
			get
			{
				return _dash;
			}
			private set
			{
				_dash = value;
			}
		}

		public static ST_PrintError NA
		{
			get
			{
				return _NA;
			}
			private set
			{
				_NA = value;
			}
		}

		static ST_PrintError()
		{
			displayed = new ST_PrintError("displayed");
			blank = new ST_PrintError("blank");
			dash = new ST_PrintError("dash");
			NA = new ST_PrintError("NA");
		}

		private ST_PrintError(string val)
		{
			_ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return _ooxmlEnumerationValue;
		}

		public bool Equals(ST_PrintError other)
		{
			if (other == null)
			{
				return false;
			}
			return _ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_PrintError one, ST_PrintError two)
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

		public static bool operator !=(ST_PrintError one, ST_PrintError two)
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
