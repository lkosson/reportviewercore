namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class ST_TblWidth
	{
		private string _ooxmlEnumerationValue;

		private static ST_TblWidth _nil;

		private static ST_TblWidth _pct;

		private static ST_TblWidth _dxa;

		private static ST_TblWidth _auto;

		public static ST_TblWidth nil
		{
			get
			{
				return _nil;
			}
			private set
			{
				_nil = value;
			}
		}

		public static ST_TblWidth pct
		{
			get
			{
				return _pct;
			}
			private set
			{
				_pct = value;
			}
		}

		public static ST_TblWidth dxa
		{
			get
			{
				return _dxa;
			}
			private set
			{
				_dxa = value;
			}
		}

		public static ST_TblWidth auto
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

		static ST_TblWidth()
		{
			nil = new ST_TblWidth("nil");
			pct = new ST_TblWidth("pct");
			dxa = new ST_TblWidth("dxa");
			auto = new ST_TblWidth("auto");
		}

		private ST_TblWidth(string val)
		{
			_ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return _ooxmlEnumerationValue;
		}
	}
}
