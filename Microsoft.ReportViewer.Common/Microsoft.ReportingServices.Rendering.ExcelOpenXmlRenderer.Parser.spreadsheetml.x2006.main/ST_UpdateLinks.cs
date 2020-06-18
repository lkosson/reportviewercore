namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class ST_UpdateLinks
	{
		private string _ooxmlEnumerationValue;

		private static ST_UpdateLinks _userSet;

		private static ST_UpdateLinks _never;

		private static ST_UpdateLinks _always;

		public static ST_UpdateLinks userSet
		{
			get
			{
				return _userSet;
			}
			private set
			{
				_userSet = value;
			}
		}

		public static ST_UpdateLinks never
		{
			get
			{
				return _never;
			}
			private set
			{
				_never = value;
			}
		}

		public static ST_UpdateLinks always
		{
			get
			{
				return _always;
			}
			private set
			{
				_always = value;
			}
		}

		static ST_UpdateLinks()
		{
			userSet = new ST_UpdateLinks("userSet");
			never = new ST_UpdateLinks("never");
			always = new ST_UpdateLinks("always");
		}

		private ST_UpdateLinks(string val)
		{
			_ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return _ooxmlEnumerationValue;
		}

		public bool Equals(ST_UpdateLinks other)
		{
			if (other == null)
			{
				return false;
			}
			return _ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_UpdateLinks one, ST_UpdateLinks two)
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

		public static bool operator !=(ST_UpdateLinks one, ST_UpdateLinks two)
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
