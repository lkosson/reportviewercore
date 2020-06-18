namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.officeDocument.x2006.sharedTypes
{
	internal class ST_VerticalAlignRun
	{
		private string _ooxmlEnumerationValue;

		private static ST_VerticalAlignRun _baseline;

		private static ST_VerticalAlignRun _superscript;

		private static ST_VerticalAlignRun _subscript;

		public static ST_VerticalAlignRun baseline
		{
			get
			{
				return _baseline;
			}
			private set
			{
				_baseline = value;
			}
		}

		public static ST_VerticalAlignRun superscript
		{
			get
			{
				return _superscript;
			}
			private set
			{
				_superscript = value;
			}
		}

		public static ST_VerticalAlignRun subscript
		{
			get
			{
				return _subscript;
			}
			private set
			{
				_subscript = value;
			}
		}

		static ST_VerticalAlignRun()
		{
			baseline = new ST_VerticalAlignRun("baseline");
			superscript = new ST_VerticalAlignRun("superscript");
			subscript = new ST_VerticalAlignRun("subscript");
		}

		private ST_VerticalAlignRun(string val)
		{
			_ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return _ooxmlEnumerationValue;
		}

		public bool Equals(ST_VerticalAlignRun other)
		{
			if (other == null)
			{
				return false;
			}
			return _ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_VerticalAlignRun one, ST_VerticalAlignRun two)
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

		public static bool operator !=(ST_VerticalAlignRun one, ST_VerticalAlignRun two)
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
