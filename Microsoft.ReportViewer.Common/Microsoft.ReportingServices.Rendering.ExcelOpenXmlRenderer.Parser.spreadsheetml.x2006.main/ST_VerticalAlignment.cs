namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class ST_VerticalAlignment
	{
		private string _ooxmlEnumerationValue;

		private static ST_VerticalAlignment _top;

		private static ST_VerticalAlignment _center;

		private static ST_VerticalAlignment _bottom;

		private static ST_VerticalAlignment _justify;

		private static ST_VerticalAlignment _distributed;

		public static ST_VerticalAlignment top
		{
			get
			{
				return _top;
			}
			private set
			{
				_top = value;
			}
		}

		public static ST_VerticalAlignment center
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

		public static ST_VerticalAlignment bottom
		{
			get
			{
				return _bottom;
			}
			private set
			{
				_bottom = value;
			}
		}

		public static ST_VerticalAlignment justify
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

		public static ST_VerticalAlignment distributed
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

		static ST_VerticalAlignment()
		{
			top = new ST_VerticalAlignment("top");
			center = new ST_VerticalAlignment("center");
			bottom = new ST_VerticalAlignment("bottom");
			justify = new ST_VerticalAlignment("justify");
			distributed = new ST_VerticalAlignment("distributed");
		}

		private ST_VerticalAlignment(string val)
		{
			_ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return _ooxmlEnumerationValue;
		}

		public bool Equals(ST_VerticalAlignment other)
		{
			if (other == null)
			{
				return false;
			}
			return _ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_VerticalAlignment one, ST_VerticalAlignment two)
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

		public static bool operator !=(ST_VerticalAlignment one, ST_VerticalAlignment two)
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
