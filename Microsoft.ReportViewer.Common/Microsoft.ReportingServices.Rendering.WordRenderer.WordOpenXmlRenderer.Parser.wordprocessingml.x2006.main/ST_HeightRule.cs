namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class ST_HeightRule
	{
		private string _ooxmlEnumerationValue;

		private static ST_HeightRule _auto;

		private static ST_HeightRule _exact;

		private static ST_HeightRule _atLeast;

		public static ST_HeightRule auto
		{
			get
			{
				return _auto;
			}
			private set
			{
				_auto = value;
			}
		}

		public static ST_HeightRule exact
		{
			get
			{
				return _exact;
			}
			private set
			{
				_exact = value;
			}
		}

		public static ST_HeightRule atLeast
		{
			get
			{
				return _atLeast;
			}
			private set
			{
				_atLeast = value;
			}
		}

		static ST_HeightRule()
		{
			auto = new ST_HeightRule("auto");
			exact = new ST_HeightRule("exact");
			atLeast = new ST_HeightRule("atLeast");
		}

		private ST_HeightRule(string val)
		{
			_ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return _ooxmlEnumerationValue;
		}
	}
}
