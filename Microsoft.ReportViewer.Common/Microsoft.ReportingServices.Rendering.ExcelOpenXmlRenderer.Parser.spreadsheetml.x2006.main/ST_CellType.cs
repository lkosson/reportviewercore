namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class ST_CellType
	{
		private string _ooxmlEnumerationValue;

		private static ST_CellType _b;

		private static ST_CellType _d;

		private static ST_CellType _n;

		private static ST_CellType _e;

		private static ST_CellType _s;

		private static ST_CellType _str;

		private static ST_CellType _inlineStr;

		public static ST_CellType b
		{
			get
			{
				return _b;
			}
			private set
			{
				_b = value;
			}
		}

		public static ST_CellType d
		{
			get
			{
				return _d;
			}
			private set
			{
				_d = value;
			}
		}

		public static ST_CellType n
		{
			get
			{
				return _n;
			}
			private set
			{
				_n = value;
			}
		}

		public static ST_CellType e
		{
			get
			{
				return _e;
			}
			private set
			{
				_e = value;
			}
		}

		public static ST_CellType s
		{
			get
			{
				return _s;
			}
			private set
			{
				_s = value;
			}
		}

		public static ST_CellType str
		{
			get
			{
				return _str;
			}
			private set
			{
				_str = value;
			}
		}

		public static ST_CellType inlineStr
		{
			get
			{
				return _inlineStr;
			}
			private set
			{
				_inlineStr = value;
			}
		}

		static ST_CellType()
		{
			b = new ST_CellType("b");
			d = new ST_CellType("d");
			n = new ST_CellType("n");
			e = new ST_CellType("e");
			s = new ST_CellType("s");
			str = new ST_CellType("str");
			inlineStr = new ST_CellType("inlineStr");
		}

		private ST_CellType(string val)
		{
			_ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return _ooxmlEnumerationValue;
		}

		public bool Equals(ST_CellType other)
		{
			if (other == null)
			{
				return false;
			}
			return _ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_CellType one, ST_CellType two)
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

		public static bool operator !=(ST_CellType one, ST_CellType two)
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
