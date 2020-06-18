namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class ST_RefMode
	{
		private string _ooxmlEnumerationValue;

		private static ST_RefMode _A1;

		private static ST_RefMode _R1C1;

		public static ST_RefMode A1
		{
			get
			{
				return _A1;
			}
			private set
			{
				_A1 = value;
			}
		}

		public static ST_RefMode R1C1
		{
			get
			{
				return _R1C1;
			}
			private set
			{
				_R1C1 = value;
			}
		}

		static ST_RefMode()
		{
			A1 = new ST_RefMode("A1");
			R1C1 = new ST_RefMode("R1C1");
		}

		private ST_RefMode(string val)
		{
			_ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return _ooxmlEnumerationValue;
		}

		public bool Equals(ST_RefMode other)
		{
			if (other == null)
			{
				return false;
			}
			return _ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_RefMode one, ST_RefMode two)
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

		public static bool operator !=(ST_RefMode one, ST_RefMode two)
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
