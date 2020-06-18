namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class ST_SectionMark
	{
		private string _ooxmlEnumerationValue;

		private static ST_SectionMark _nextPage;

		private static ST_SectionMark _nextColumn;

		private static ST_SectionMark _continuous;

		private static ST_SectionMark _evenPage;

		private static ST_SectionMark _oddPage;

		public static ST_SectionMark nextPage
		{
			get
			{
				return _nextPage;
			}
			private set
			{
				_nextPage = value;
			}
		}

		public static ST_SectionMark nextColumn
		{
			get
			{
				return _nextColumn;
			}
			private set
			{
				_nextColumn = value;
			}
		}

		public static ST_SectionMark continuous
		{
			get
			{
				return _continuous;
			}
			private set
			{
				_continuous = value;
			}
		}

		public static ST_SectionMark evenPage
		{
			get
			{
				return _evenPage;
			}
			private set
			{
				_evenPage = value;
			}
		}

		public static ST_SectionMark oddPage
		{
			get
			{
				return _oddPage;
			}
			private set
			{
				_oddPage = value;
			}
		}

		static ST_SectionMark()
		{
			nextPage = new ST_SectionMark("nextPage");
			nextColumn = new ST_SectionMark("nextColumn");
			continuous = new ST_SectionMark("continuous");
			evenPage = new ST_SectionMark("evenPage");
			oddPage = new ST_SectionMark("oddPage");
		}

		private ST_SectionMark(string val)
		{
			_ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return _ooxmlEnumerationValue;
		}
	}
}
