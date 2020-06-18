namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class ST_PageOrder
	{
		private string _ooxmlEnumerationValue;

		private static ST_PageOrder _downThenOver;

		private static ST_PageOrder _overThenDown;

		public static ST_PageOrder downThenOver
		{
			get
			{
				return _downThenOver;
			}
			private set
			{
				_downThenOver = value;
			}
		}

		public static ST_PageOrder overThenDown
		{
			get
			{
				return _overThenDown;
			}
			private set
			{
				_overThenDown = value;
			}
		}

		static ST_PageOrder()
		{
			downThenOver = new ST_PageOrder("downThenOver");
			overThenDown = new ST_PageOrder("overThenDown");
		}

		private ST_PageOrder(string val)
		{
			_ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return _ooxmlEnumerationValue;
		}

		public bool Equals(ST_PageOrder other)
		{
			if (other == null)
			{
				return false;
			}
			return _ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_PageOrder one, ST_PageOrder two)
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

		public static bool operator !=(ST_PageOrder one, ST_PageOrder two)
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
