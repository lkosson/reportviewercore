namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class ST_SheetState
	{
		private string _ooxmlEnumerationValue;

		private static ST_SheetState _visible;

		private static ST_SheetState _hidden;

		private static ST_SheetState _veryHidden;

		public static ST_SheetState visible
		{
			get
			{
				return _visible;
			}
			private set
			{
				_visible = value;
			}
		}

		public static ST_SheetState hidden
		{
			get
			{
				return _hidden;
			}
			private set
			{
				_hidden = value;
			}
		}

		public static ST_SheetState veryHidden
		{
			get
			{
				return _veryHidden;
			}
			private set
			{
				_veryHidden = value;
			}
		}

		static ST_SheetState()
		{
			visible = new ST_SheetState("visible");
			hidden = new ST_SheetState("hidden");
			veryHidden = new ST_SheetState("veryHidden");
		}

		private ST_SheetState(string val)
		{
			_ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return _ooxmlEnumerationValue;
		}

		public bool Equals(ST_SheetState other)
		{
			if (other == null)
			{
				return false;
			}
			return _ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_SheetState one, ST_SheetState two)
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

		public static bool operator !=(ST_SheetState one, ST_SheetState two)
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
