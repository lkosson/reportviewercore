namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class ST_CellComments
	{
		private string _ooxmlEnumerationValue;

		private static ST_CellComments _none;

		private static ST_CellComments _asDisplayed;

		private static ST_CellComments _atEnd;

		public static ST_CellComments none
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

		public static ST_CellComments asDisplayed
		{
			get
			{
				return _asDisplayed;
			}
			private set
			{
				_asDisplayed = value;
			}
		}

		public static ST_CellComments atEnd
		{
			get
			{
				return _atEnd;
			}
			private set
			{
				_atEnd = value;
			}
		}

		static ST_CellComments()
		{
			none = new ST_CellComments("none");
			asDisplayed = new ST_CellComments("asDisplayed");
			atEnd = new ST_CellComments("atEnd");
		}

		private ST_CellComments(string val)
		{
			_ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return _ooxmlEnumerationValue;
		}

		public bool Equals(ST_CellComments other)
		{
			if (other == null)
			{
				return false;
			}
			return _ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_CellComments one, ST_CellComments two)
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

		public static bool operator !=(ST_CellComments one, ST_CellComments two)
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
