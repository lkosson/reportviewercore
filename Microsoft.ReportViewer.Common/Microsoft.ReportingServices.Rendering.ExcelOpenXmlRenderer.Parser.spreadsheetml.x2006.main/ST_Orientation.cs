namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class ST_Orientation
	{
		private string _ooxmlEnumerationValue;

		private static ST_Orientation __default;

		private static ST_Orientation _portrait;

		private static ST_Orientation _landscape;

		public static ST_Orientation _default
		{
			get
			{
				return __default;
			}
			private set
			{
				__default = value;
			}
		}

		public static ST_Orientation portrait
		{
			get
			{
				return _portrait;
			}
			private set
			{
				_portrait = value;
			}
		}

		public static ST_Orientation landscape
		{
			get
			{
				return _landscape;
			}
			private set
			{
				_landscape = value;
			}
		}

		static ST_Orientation()
		{
			_default = new ST_Orientation("default");
			portrait = new ST_Orientation("portrait");
			landscape = new ST_Orientation("landscape");
		}

		private ST_Orientation(string val)
		{
			_ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return _ooxmlEnumerationValue;
		}

		public bool Equals(ST_Orientation other)
		{
			if (other == null)
			{
				return false;
			}
			return _ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_Orientation one, ST_Orientation two)
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

		public static bool operator !=(ST_Orientation one, ST_Orientation two)
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
