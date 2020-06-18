namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class ST_HorizontalAlignment
	{
		private string _ooxmlEnumerationValue;

		private static ST_HorizontalAlignment _general;

		private static ST_HorizontalAlignment _left;

		private static ST_HorizontalAlignment _center;

		private static ST_HorizontalAlignment _right;

		private static ST_HorizontalAlignment _fill;

		private static ST_HorizontalAlignment _justify;

		private static ST_HorizontalAlignment _centerContinuous;

		private static ST_HorizontalAlignment _distributed;

		public static ST_HorizontalAlignment general
		{
			get
			{
				return _general;
			}
			private set
			{
				_general = value;
			}
		}

		public static ST_HorizontalAlignment left
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

		public static ST_HorizontalAlignment center
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

		public static ST_HorizontalAlignment right
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

		public static ST_HorizontalAlignment fill
		{
			get
			{
				return _fill;
			}
			private set
			{
				_fill = value;
			}
		}

		public static ST_HorizontalAlignment justify
		{
			get
			{
				return _justify;
			}
			private set
			{
				_justify = value;
			}
		}

		public static ST_HorizontalAlignment centerContinuous
		{
			get
			{
				return _centerContinuous;
			}
			private set
			{
				_centerContinuous = value;
			}
		}

		public static ST_HorizontalAlignment distributed
		{
			get
			{
				return _distributed;
			}
			private set
			{
				_distributed = value;
			}
		}

		static ST_HorizontalAlignment()
		{
			general = new ST_HorizontalAlignment("general");
			left = new ST_HorizontalAlignment("left");
			center = new ST_HorizontalAlignment("center");
			right = new ST_HorizontalAlignment("right");
			fill = new ST_HorizontalAlignment("fill");
			justify = new ST_HorizontalAlignment("justify");
			centerContinuous = new ST_HorizontalAlignment("centerContinuous");
			distributed = new ST_HorizontalAlignment("distributed");
		}

		private ST_HorizontalAlignment(string val)
		{
			_ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return _ooxmlEnumerationValue;
		}

		public bool Equals(ST_HorizontalAlignment other)
		{
			if (other == null)
			{
				return false;
			}
			return _ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_HorizontalAlignment one, ST_HorizontalAlignment two)
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

		public static bool operator !=(ST_HorizontalAlignment one, ST_HorizontalAlignment two)
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
