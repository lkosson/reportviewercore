namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class ST_PaneState
	{
		private string _ooxmlEnumerationValue;

		private static ST_PaneState _split;

		private static ST_PaneState _frozen;

		private static ST_PaneState _frozenSplit;

		public static ST_PaneState split
		{
			get
			{
				return _split;
			}
			private set
			{
				_split = value;
			}
		}

		public static ST_PaneState frozen
		{
			get
			{
				return _frozen;
			}
			private set
			{
				_frozen = value;
			}
		}

		public static ST_PaneState frozenSplit
		{
			get
			{
				return _frozenSplit;
			}
			private set
			{
				_frozenSplit = value;
			}
		}

		static ST_PaneState()
		{
			split = new ST_PaneState("split");
			frozen = new ST_PaneState("frozen");
			frozenSplit = new ST_PaneState("frozenSplit");
		}

		private ST_PaneState(string val)
		{
			_ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return _ooxmlEnumerationValue;
		}

		public bool Equals(ST_PaneState other)
		{
			if (other == null)
			{
				return false;
			}
			return _ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_PaneState one, ST_PaneState two)
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

		public static bool operator !=(ST_PaneState one, ST_PaneState two)
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
