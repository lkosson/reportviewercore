namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class ST_Visibility
	{
		private string _ooxmlEnumerationValue;

		private static ST_Visibility _visible;

		private static ST_Visibility _hidden;

		private static ST_Visibility _veryHidden;

		public static ST_Visibility visible
		{
			get
			{
				return _visible;
			}
			private set
			{
				_visible = value;
			}
		}

		public static ST_Visibility hidden
		{
			get
			{
				return _hidden;
			}
			private set
			{
				_hidden = value;
			}
		}

		public static ST_Visibility veryHidden
		{
			get
			{
				return _veryHidden;
			}
			private set
			{
				_veryHidden = value;
			}
		}

		static ST_Visibility()
		{
			visible = new ST_Visibility("visible");
			hidden = new ST_Visibility("hidden");
			veryHidden = new ST_Visibility("veryHidden");
		}

		private ST_Visibility(string val)
		{
			_ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return _ooxmlEnumerationValue;
		}

		public bool Equals(ST_Visibility other)
		{
			if (other == null)
			{
				return false;
			}
			return _ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_Visibility one, ST_Visibility two)
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

		public static bool operator !=(ST_Visibility one, ST_Visibility two)
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
