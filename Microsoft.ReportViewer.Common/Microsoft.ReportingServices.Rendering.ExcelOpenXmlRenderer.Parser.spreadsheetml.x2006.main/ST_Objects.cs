namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class ST_Objects
	{
		private string _ooxmlEnumerationValue;

		private static ST_Objects _all;

		private static ST_Objects _placeholders;

		private static ST_Objects _none;

		public static ST_Objects all
		{
			get
			{
				return _all;
			}
			private set
			{
				_all = value;
			}
		}

		public static ST_Objects placeholders
		{
			get
			{
				return _placeholders;
			}
			private set
			{
				_placeholders = value;
			}
		}

		public static ST_Objects none
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

		static ST_Objects()
		{
			all = new ST_Objects("all");
			placeholders = new ST_Objects("placeholders");
			none = new ST_Objects("none");
		}

		private ST_Objects(string val)
		{
			_ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return _ooxmlEnumerationValue;
		}

		public bool Equals(ST_Objects other)
		{
			if (other == null)
			{
				return false;
			}
			return _ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_Objects one, ST_Objects two)
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

		public static bool operator !=(ST_Objects one, ST_Objects two)
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
