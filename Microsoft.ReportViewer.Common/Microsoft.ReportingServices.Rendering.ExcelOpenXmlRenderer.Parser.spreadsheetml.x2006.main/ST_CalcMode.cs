namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class ST_CalcMode
	{
		private string _ooxmlEnumerationValue;

		private static ST_CalcMode _manual;

		private static ST_CalcMode _auto;

		private static ST_CalcMode _autoNoTable;

		public static ST_CalcMode manual
		{
			get
			{
				return _manual;
			}
			private set
			{
				_manual = value;
			}
		}

		public static ST_CalcMode auto
		{
			get
			{
				return _auto;
			}
			private set
			{
				_auto = value;
			}
		}

		public static ST_CalcMode autoNoTable
		{
			get
			{
				return _autoNoTable;
			}
			private set
			{
				_autoNoTable = value;
			}
		}

		static ST_CalcMode()
		{
			manual = new ST_CalcMode("manual");
			auto = new ST_CalcMode("auto");
			autoNoTable = new ST_CalcMode("autoNoTable");
		}

		private ST_CalcMode(string val)
		{
			_ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return _ooxmlEnumerationValue;
		}

		public bool Equals(ST_CalcMode other)
		{
			if (other == null)
			{
				return false;
			}
			return _ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_CalcMode one, ST_CalcMode two)
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

		public static bool operator !=(ST_CalcMode one, ST_CalcMode two)
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
