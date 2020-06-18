namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class ST_FontScheme
	{
		private string _ooxmlEnumerationValue;

		private static ST_FontScheme _none;

		private static ST_FontScheme _major;

		private static ST_FontScheme _minor;

		public static ST_FontScheme none
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

		public static ST_FontScheme major
		{
			get
			{
				return _major;
			}
			private set
			{
				_major = value;
			}
		}

		public static ST_FontScheme minor
		{
			get
			{
				return _minor;
			}
			private set
			{
				_minor = value;
			}
		}

		static ST_FontScheme()
		{
			none = new ST_FontScheme("none");
			major = new ST_FontScheme("major");
			minor = new ST_FontScheme("minor");
		}

		private ST_FontScheme(string val)
		{
			_ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return _ooxmlEnumerationValue;
		}

		public bool Equals(ST_FontScheme other)
		{
			if (other == null)
			{
				return false;
			}
			return _ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_FontScheme one, ST_FontScheme two)
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

		public static bool operator !=(ST_FontScheme one, ST_FontScheme two)
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
