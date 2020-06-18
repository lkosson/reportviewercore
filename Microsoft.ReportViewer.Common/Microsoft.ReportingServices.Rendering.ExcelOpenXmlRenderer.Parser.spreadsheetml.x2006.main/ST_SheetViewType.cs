namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class ST_SheetViewType
	{
		private string _ooxmlEnumerationValue;

		private static ST_SheetViewType _normal;

		private static ST_SheetViewType _pageBreakPreview;

		private static ST_SheetViewType _pageLayout;

		public static ST_SheetViewType normal
		{
			get
			{
				return _normal;
			}
			private set
			{
				_normal = value;
			}
		}

		public static ST_SheetViewType pageBreakPreview
		{
			get
			{
				return _pageBreakPreview;
			}
			private set
			{
				_pageBreakPreview = value;
			}
		}

		public static ST_SheetViewType pageLayout
		{
			get
			{
				return _pageLayout;
			}
			private set
			{
				_pageLayout = value;
			}
		}

		static ST_SheetViewType()
		{
			normal = new ST_SheetViewType("normal");
			pageBreakPreview = new ST_SheetViewType("pageBreakPreview");
			pageLayout = new ST_SheetViewType("pageLayout");
		}

		private ST_SheetViewType(string val)
		{
			_ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return _ooxmlEnumerationValue;
		}

		public bool Equals(ST_SheetViewType other)
		{
			if (other == null)
			{
				return false;
			}
			return _ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_SheetViewType one, ST_SheetViewType two)
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

		public static bool operator !=(ST_SheetViewType one, ST_SheetViewType two)
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
