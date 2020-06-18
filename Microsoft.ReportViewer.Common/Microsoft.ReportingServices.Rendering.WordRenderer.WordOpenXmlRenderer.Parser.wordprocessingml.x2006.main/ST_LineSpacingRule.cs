namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class ST_LineSpacingRule
	{
		private string _ooxmlEnumerationValue;

		private static ST_LineSpacingRule _auto;

		private static ST_LineSpacingRule _exact;

		private static ST_LineSpacingRule _atLeast;

		public static ST_LineSpacingRule auto
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

		public static ST_LineSpacingRule exact
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

		public static ST_LineSpacingRule atLeast
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

		static ST_LineSpacingRule()
		{
			auto = new ST_LineSpacingRule("auto");
			exact = new ST_LineSpacingRule("exact");
			atLeast = new ST_LineSpacingRule("atLeast");
		}

		private ST_LineSpacingRule(string val)
		{
			_ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return _ooxmlEnumerationValue;
		}
	}
}
