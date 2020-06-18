namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class ST_Pane
	{
		private string _ooxmlEnumerationValue;

		private static ST_Pane _bottomRight;

		private static ST_Pane _topRight;

		private static ST_Pane _bottomLeft;

		private static ST_Pane _topLeft;

		public static ST_Pane bottomRight
		{
			get
			{
				return _bottomRight;
			}
			private set
			{
				_bottomRight = value;
			}
		}

		public static ST_Pane topRight
		{
			get
			{
				return _topRight;
			}
			private set
			{
				_topRight = value;
			}
		}

		public static ST_Pane bottomLeft
		{
			get
			{
				return _bottomLeft;
			}
			private set
			{
				_bottomLeft = value;
			}
		}

		public static ST_Pane topLeft
		{
			get
			{
				return _topLeft;
			}
			private set
			{
				_topLeft = value;
			}
		}

		static ST_Pane()
		{
			bottomRight = new ST_Pane("bottomRight");
			topRight = new ST_Pane("topRight");
			bottomLeft = new ST_Pane("bottomLeft");
			topLeft = new ST_Pane("topLeft");
		}

		private ST_Pane(string val)
		{
			_ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return _ooxmlEnumerationValue;
		}

		public bool Equals(ST_Pane other)
		{
			if (other == null)
			{
				return false;
			}
			return _ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_Pane one, ST_Pane two)
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

		public static bool operator !=(ST_Pane one, ST_Pane two)
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
