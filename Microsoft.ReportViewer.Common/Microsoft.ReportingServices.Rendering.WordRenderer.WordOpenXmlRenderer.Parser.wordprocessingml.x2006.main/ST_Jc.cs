namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class ST_Jc
	{
		private string _ooxmlEnumerationValue;

		private static ST_Jc _left;

		private static ST_Jc _center;

		private static ST_Jc _right;

		private static ST_Jc _both;

		private static ST_Jc _mediumKashida;

		private static ST_Jc _distribute;

		private static ST_Jc _numTab;

		private static ST_Jc _highKashida;

		private static ST_Jc _lowKashida;

		private static ST_Jc _thaiDistribute;

		public static ST_Jc left
		{
			get
			{
				return _left;
			}
			private set
			{
				_left = value;
			}
		}

		public static ST_Jc center
		{
			get
			{
				return _center;
			}
			private set
			{
				_center = value;
			}
		}

		public static ST_Jc right
		{
			get
			{
				return _right;
			}
			private set
			{
				_right = value;
			}
		}

		public static ST_Jc both
		{
			get
			{
				return _both;
			}
			private set
			{
				_both = value;
			}
		}

		public static ST_Jc mediumKashida
		{
			get
			{
				return _mediumKashida;
			}
			private set
			{
				_mediumKashida = value;
			}
		}

		public static ST_Jc distribute
		{
			get
			{
				return _distribute;
			}
			private set
			{
				_distribute = value;
			}
		}

		public static ST_Jc numTab
		{
			get
			{
				return _numTab;
			}
			private set
			{
				_numTab = value;
			}
		}

		public static ST_Jc highKashida
		{
			get
			{
				return _highKashida;
			}
			private set
			{
				_highKashida = value;
			}
		}

		public static ST_Jc lowKashida
		{
			get
			{
				return _lowKashida;
			}
			private set
			{
				_lowKashida = value;
			}
		}

		public static ST_Jc thaiDistribute
		{
			get
			{
				return _thaiDistribute;
			}
			private set
			{
				_thaiDistribute = value;
			}
		}

		static ST_Jc()
		{
			left = new ST_Jc("left");
			center = new ST_Jc("center");
			right = new ST_Jc("right");
			both = new ST_Jc("both");
			mediumKashida = new ST_Jc("mediumKashida");
			distribute = new ST_Jc("distribute");
			numTab = new ST_Jc("numTab");
			highKashida = new ST_Jc("highKashida");
			lowKashida = new ST_Jc("lowKashida");
			thaiDistribute = new ST_Jc("thaiDistribute");
		}

		private ST_Jc(string val)
		{
			_ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return _ooxmlEnumerationValue;
		}
	}
}
