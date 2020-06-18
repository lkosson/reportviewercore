namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class ST_UnderlineValues
	{
		private string _ooxmlEnumerationValue;

		private static ST_UnderlineValues _single;

		private static ST_UnderlineValues __double;

		private static ST_UnderlineValues _singleAccounting;

		private static ST_UnderlineValues _doubleAccounting;

		private static ST_UnderlineValues _none;

		public static ST_UnderlineValues single
		{
			get
			{
				return _single;
			}
			private set
			{
				_single = value;
			}
		}

		public static ST_UnderlineValues _double
		{
			get
			{
				return __double;
			}
			private set
			{
				__double = value;
			}
		}

		public static ST_UnderlineValues singleAccounting
		{
			get
			{
				return _singleAccounting;
			}
			private set
			{
				_singleAccounting = value;
			}
		}

		public static ST_UnderlineValues doubleAccounting
		{
			get
			{
				return _doubleAccounting;
			}
			private set
			{
				_doubleAccounting = value;
			}
		}

		public static ST_UnderlineValues none
		{
			get
			{
				return _none;
			}
			private set
			{
				_none = value;
			}
		}

		static ST_UnderlineValues()
		{
			single = new ST_UnderlineValues("single");
			_double = new ST_UnderlineValues("double");
			singleAccounting = new ST_UnderlineValues("singleAccounting");
			doubleAccounting = new ST_UnderlineValues("doubleAccounting");
			none = new ST_UnderlineValues("none");
		}

		private ST_UnderlineValues(string val)
		{
			_ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return _ooxmlEnumerationValue;
		}

		public bool Equals(ST_UnderlineValues other)
		{
			if (other == null)
			{
				return false;
			}
			return _ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_UnderlineValues one, ST_UnderlineValues two)
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

		public static bool operator !=(ST_UnderlineValues one, ST_UnderlineValues two)
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
