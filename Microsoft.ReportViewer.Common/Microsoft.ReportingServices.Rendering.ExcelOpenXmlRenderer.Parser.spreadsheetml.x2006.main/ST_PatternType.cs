namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class ST_PatternType
	{
		private string _ooxmlEnumerationValue;

		private static ST_PatternType _none;

		private static ST_PatternType _solid;

		private static ST_PatternType _mediumGray;

		private static ST_PatternType _darkGray;

		private static ST_PatternType _lightGray;

		private static ST_PatternType _darkHorizontal;

		private static ST_PatternType _darkVertical;

		private static ST_PatternType _darkDown;

		private static ST_PatternType _darkUp;

		private static ST_PatternType _darkGrid;

		private static ST_PatternType _darkTrellis;

		private static ST_PatternType _lightHorizontal;

		private static ST_PatternType _lightVertical;

		private static ST_PatternType _lightDown;

		private static ST_PatternType _lightUp;

		private static ST_PatternType _lightGrid;

		private static ST_PatternType _lightTrellis;

		private static ST_PatternType _gray125;

		private static ST_PatternType _gray0625;

		public static ST_PatternType none
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

		public static ST_PatternType solid
		{
			get
			{
				return _solid;
			}
			private set
			{
				_solid = value;
			}
		}

		public static ST_PatternType mediumGray
		{
			get
			{
				return _mediumGray;
			}
			private set
			{
				_mediumGray = value;
			}
		}

		public static ST_PatternType darkGray
		{
			get
			{
				return _darkGray;
			}
			private set
			{
				_darkGray = value;
			}
		}

		public static ST_PatternType lightGray
		{
			get
			{
				return _lightGray;
			}
			private set
			{
				_lightGray = value;
			}
		}

		public static ST_PatternType darkHorizontal
		{
			get
			{
				return _darkHorizontal;
			}
			private set
			{
				_darkHorizontal = value;
			}
		}

		public static ST_PatternType darkVertical
		{
			get
			{
				return _darkVertical;
			}
			private set
			{
				_darkVertical = value;
			}
		}

		public static ST_PatternType darkDown
		{
			get
			{
				return _darkDown;
			}
			private set
			{
				_darkDown = value;
			}
		}

		public static ST_PatternType darkUp
		{
			get
			{
				return _darkUp;
			}
			private set
			{
				_darkUp = value;
			}
		}

		public static ST_PatternType darkGrid
		{
			get
			{
				return _darkGrid;
			}
			private set
			{
				_darkGrid = value;
			}
		}

		public static ST_PatternType darkTrellis
		{
			get
			{
				return _darkTrellis;
			}
			private set
			{
				_darkTrellis = value;
			}
		}

		public static ST_PatternType lightHorizontal
		{
			get
			{
				return _lightHorizontal;
			}
			private set
			{
				_lightHorizontal = value;
			}
		}

		public static ST_PatternType lightVertical
		{
			get
			{
				return _lightVertical;
			}
			private set
			{
				_lightVertical = value;
			}
		}

		public static ST_PatternType lightDown
		{
			get
			{
				return _lightDown;
			}
			private set
			{
				_lightDown = value;
			}
		}

		public static ST_PatternType lightUp
		{
			get
			{
				return _lightUp;
			}
			private set
			{
				_lightUp = value;
			}
		}

		public static ST_PatternType lightGrid
		{
			get
			{
				return _lightGrid;
			}
			private set
			{
				_lightGrid = value;
			}
		}

		public static ST_PatternType lightTrellis
		{
			get
			{
				return _lightTrellis;
			}
			private set
			{
				_lightTrellis = value;
			}
		}

		public static ST_PatternType gray125
		{
			get
			{
				return _gray125;
			}
			private set
			{
				_gray125 = value;
			}
		}

		public static ST_PatternType gray0625
		{
			get
			{
				return _gray0625;
			}
			private set
			{
				_gray0625 = value;
			}
		}

		static ST_PatternType()
		{
			none = new ST_PatternType("none");
			solid = new ST_PatternType("solid");
			mediumGray = new ST_PatternType("mediumGray");
			darkGray = new ST_PatternType("darkGray");
			lightGray = new ST_PatternType("lightGray");
			darkHorizontal = new ST_PatternType("darkHorizontal");
			darkVertical = new ST_PatternType("darkVertical");
			darkDown = new ST_PatternType("darkDown");
			darkUp = new ST_PatternType("darkUp");
			darkGrid = new ST_PatternType("darkGrid");
			darkTrellis = new ST_PatternType("darkTrellis");
			lightHorizontal = new ST_PatternType("lightHorizontal");
			lightVertical = new ST_PatternType("lightVertical");
			lightDown = new ST_PatternType("lightDown");
			lightUp = new ST_PatternType("lightUp");
			lightGrid = new ST_PatternType("lightGrid");
			lightTrellis = new ST_PatternType("lightTrellis");
			gray125 = new ST_PatternType("gray125");
			gray0625 = new ST_PatternType("gray0625");
		}

		private ST_PatternType(string val)
		{
			_ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return _ooxmlEnumerationValue;
		}

		public bool Equals(ST_PatternType other)
		{
			if (other == null)
			{
				return false;
			}
			return _ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_PatternType one, ST_PatternType two)
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

		public static bool operator !=(ST_PatternType one, ST_PatternType two)
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
