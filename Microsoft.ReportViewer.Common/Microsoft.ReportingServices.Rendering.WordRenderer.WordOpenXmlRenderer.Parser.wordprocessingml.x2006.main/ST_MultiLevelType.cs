namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class ST_MultiLevelType
	{
		private string _ooxmlEnumerationValue;

		private static ST_MultiLevelType _singleLevel;

		private static ST_MultiLevelType _multilevel;

		private static ST_MultiLevelType _hybridMultilevel;

		public static ST_MultiLevelType singleLevel
		{
			get
			{
				return _singleLevel;
			}
			private set
			{
				_singleLevel = value;
			}
		}

		public static ST_MultiLevelType multilevel
		{
			get
			{
				return _multilevel;
			}
			private set
			{
				_multilevel = value;
			}
		}

		public static ST_MultiLevelType hybridMultilevel
		{
			get
			{
				return _hybridMultilevel;
			}
			private set
			{
				_hybridMultilevel = value;
			}
		}

		static ST_MultiLevelType()
		{
			singleLevel = new ST_MultiLevelType("singleLevel");
			multilevel = new ST_MultiLevelType("multilevel");
			hybridMultilevel = new ST_MultiLevelType("hybridMultilevel");
		}

		private ST_MultiLevelType(string val)
		{
			_ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return _ooxmlEnumerationValue;
		}
	}
}
