namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class ST_HdrFtr
	{
		private string _ooxmlEnumerationValue;

		private static ST_HdrFtr _even;

		private static ST_HdrFtr __default;

		private static ST_HdrFtr _first;

		public static ST_HdrFtr even
		{
			get
			{
				return _even;
			}
			private set
			{
				_even = value;
			}
		}

		public static ST_HdrFtr _default
		{
			get
			{
				return __default;
			}
			private set
			{
				__default = value;
			}
		}

		public static ST_HdrFtr first
		{
			get
			{
				return _first;
			}
			private set
			{
				_first = value;
			}
		}

		static ST_HdrFtr()
		{
			even = new ST_HdrFtr("even");
			_default = new ST_HdrFtr("default");
			first = new ST_HdrFtr("first");
		}

		private ST_HdrFtr(string val)
		{
			_ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return _ooxmlEnumerationValue;
		}
	}
}
