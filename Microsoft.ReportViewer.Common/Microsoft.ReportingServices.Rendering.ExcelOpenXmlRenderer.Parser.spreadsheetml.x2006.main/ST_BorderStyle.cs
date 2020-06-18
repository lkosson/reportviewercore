namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class ST_BorderStyle
	{
		private string _ooxmlEnumerationValue;

		private static ST_BorderStyle _none;

		private static ST_BorderStyle _thin;

		private static ST_BorderStyle _medium;

		private static ST_BorderStyle _dashed;

		private static ST_BorderStyle _dotted;

		private static ST_BorderStyle _thick;

		private static ST_BorderStyle __double;

		private static ST_BorderStyle _hair;

		private static ST_BorderStyle _mediumDashed;

		private static ST_BorderStyle _dashDot;

		private static ST_BorderStyle _mediumDashDot;

		private static ST_BorderStyle _dashDotDot;

		private static ST_BorderStyle _mediumDashDotDot;

		private static ST_BorderStyle _slantDashDot;

		public static ST_BorderStyle none
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

		public static ST_BorderStyle thin
		{
			get
			{
				return _thin;
			}
			private set
			{
				_thin = value;
			}
		}

		public static ST_BorderStyle medium
		{
			get
			{
				return _medium;
			}
			private set
			{
				_medium = value;
			}
		}

		public static ST_BorderStyle dashed
		{
			get
			{
				return _dashed;
			}
			private set
			{
				_dashed = value;
			}
		}

		public static ST_BorderStyle dotted
		{
			get
			{
				return _dotted;
			}
			private set
			{
				_dotted = value;
			}
		}

		public static ST_BorderStyle thick
		{
			get
			{
				return _thick;
			}
			private set
			{
				_thick = value;
			}
		}

		public static ST_BorderStyle _double
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

		public static ST_BorderStyle hair
		{
			get
			{
				return _hair;
			}
			private set
			{
				_hair = value;
			}
		}

		public static ST_BorderStyle mediumDashed
		{
			get
			{
				return _mediumDashed;
			}
			private set
			{
				_mediumDashed = value;
			}
		}

		public static ST_BorderStyle dashDot
		{
			get
			{
				return _dashDot;
			}
			private set
			{
				_dashDot = value;
			}
		}

		public static ST_BorderStyle mediumDashDot
		{
			get
			{
				return _mediumDashDot;
			}
			private set
			{
				_mediumDashDot = value;
			}
		}

		public static ST_BorderStyle dashDotDot
		{
			get
			{
				return _dashDotDot;
			}
			private set
			{
				_dashDotDot = value;
			}
		}

		public static ST_BorderStyle mediumDashDotDot
		{
			get
			{
				return _mediumDashDotDot;
			}
			private set
			{
				_mediumDashDotDot = value;
			}
		}

		public static ST_BorderStyle slantDashDot
		{
			get
			{
				return _slantDashDot;
			}
			private set
			{
				_slantDashDot = value;
			}
		}

		static ST_BorderStyle()
		{
			none = new ST_BorderStyle("none");
			thin = new ST_BorderStyle("thin");
			medium = new ST_BorderStyle("medium");
			dashed = new ST_BorderStyle("dashed");
			dotted = new ST_BorderStyle("dotted");
			thick = new ST_BorderStyle("thick");
			_double = new ST_BorderStyle("double");
			hair = new ST_BorderStyle("hair");
			mediumDashed = new ST_BorderStyle("mediumDashed");
			dashDot = new ST_BorderStyle("dashDot");
			mediumDashDot = new ST_BorderStyle("mediumDashDot");
			dashDotDot = new ST_BorderStyle("dashDotDot");
			mediumDashDotDot = new ST_BorderStyle("mediumDashDotDot");
			slantDashDot = new ST_BorderStyle("slantDashDot");
		}

		private ST_BorderStyle(string val)
		{
			_ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return _ooxmlEnumerationValue;
		}

		public bool Equals(ST_BorderStyle other)
		{
			if (other == null)
			{
				return false;
			}
			return _ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_BorderStyle one, ST_BorderStyle two)
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

		public static bool operator !=(ST_BorderStyle one, ST_BorderStyle two)
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
