namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.officeDocument.x2006.docPropsVTypes
{
	internal class ST_VectorBaseType
	{
		private string _ooxmlEnumerationValue;

		private static ST_VectorBaseType _variant;

		private static ST_VectorBaseType _i1;

		private static ST_VectorBaseType _i2;

		private static ST_VectorBaseType _i4;

		private static ST_VectorBaseType _i8;

		private static ST_VectorBaseType _ui1;

		private static ST_VectorBaseType _ui2;

		private static ST_VectorBaseType _ui4;

		private static ST_VectorBaseType _ui8;

		private static ST_VectorBaseType _r4;

		private static ST_VectorBaseType _r8;

		private static ST_VectorBaseType _lpstr;

		private static ST_VectorBaseType _lpwstr;

		private static ST_VectorBaseType _bstr;

		private static ST_VectorBaseType _date;

		private static ST_VectorBaseType _filetime;

		private static ST_VectorBaseType __bool;

		private static ST_VectorBaseType _cy;

		private static ST_VectorBaseType _error;

		private static ST_VectorBaseType _clsid;

		private static ST_VectorBaseType _cf;

		public static ST_VectorBaseType variant
		{
			get
			{
				return _variant;
			}
			private set
			{
				_variant = value;
			}
		}

		public static ST_VectorBaseType i1
		{
			get
			{
				return _i1;
			}
			private set
			{
				_i1 = value;
			}
		}

		public static ST_VectorBaseType i2
		{
			get
			{
				return _i2;
			}
			private set
			{
				_i2 = value;
			}
		}

		public static ST_VectorBaseType i4
		{
			get
			{
				return _i4;
			}
			private set
			{
				_i4 = value;
			}
		}

		public static ST_VectorBaseType i8
		{
			get
			{
				return _i8;
			}
			private set
			{
				_i8 = value;
			}
		}

		public static ST_VectorBaseType ui1
		{
			get
			{
				return _ui1;
			}
			private set
			{
				_ui1 = value;
			}
		}

		public static ST_VectorBaseType ui2
		{
			get
			{
				return _ui2;
			}
			private set
			{
				_ui2 = value;
			}
		}

		public static ST_VectorBaseType ui4
		{
			get
			{
				return _ui4;
			}
			private set
			{
				_ui4 = value;
			}
		}

		public static ST_VectorBaseType ui8
		{
			get
			{
				return _ui8;
			}
			private set
			{
				_ui8 = value;
			}
		}

		public static ST_VectorBaseType r4
		{
			get
			{
				return _r4;
			}
			private set
			{
				_r4 = value;
			}
		}

		public static ST_VectorBaseType r8
		{
			get
			{
				return _r8;
			}
			private set
			{
				_r8 = value;
			}
		}

		public static ST_VectorBaseType lpstr
		{
			get
			{
				return _lpstr;
			}
			private set
			{
				_lpstr = value;
			}
		}

		public static ST_VectorBaseType lpwstr
		{
			get
			{
				return _lpwstr;
			}
			private set
			{
				_lpwstr = value;
			}
		}

		public static ST_VectorBaseType bstr
		{
			get
			{
				return _bstr;
			}
			private set
			{
				_bstr = value;
			}
		}

		public static ST_VectorBaseType date
		{
			get
			{
				return _date;
			}
			private set
			{
				_date = value;
			}
		}

		public static ST_VectorBaseType filetime
		{
			get
			{
				return _filetime;
			}
			private set
			{
				_filetime = value;
			}
		}

		public static ST_VectorBaseType _bool
		{
			get
			{
				return __bool;
			}
			private set
			{
				__bool = value;
			}
		}

		public static ST_VectorBaseType cy
		{
			get
			{
				return _cy;
			}
			private set
			{
				_cy = value;
			}
		}

		public static ST_VectorBaseType error
		{
			get
			{
				return _error;
			}
			private set
			{
				_error = value;
			}
		}

		public static ST_VectorBaseType clsid
		{
			get
			{
				return _clsid;
			}
			private set
			{
				_clsid = value;
			}
		}

		public static ST_VectorBaseType cf
		{
			get
			{
				return _cf;
			}
			private set
			{
				_cf = value;
			}
		}

		static ST_VectorBaseType()
		{
			variant = new ST_VectorBaseType("variant");
			i1 = new ST_VectorBaseType("i1");
			i2 = new ST_VectorBaseType("i2");
			i4 = new ST_VectorBaseType("i4");
			i8 = new ST_VectorBaseType("i8");
			ui1 = new ST_VectorBaseType("ui1");
			ui2 = new ST_VectorBaseType("ui2");
			ui4 = new ST_VectorBaseType("ui4");
			ui8 = new ST_VectorBaseType("ui8");
			r4 = new ST_VectorBaseType("r4");
			r8 = new ST_VectorBaseType("r8");
			lpstr = new ST_VectorBaseType("lpstr");
			lpwstr = new ST_VectorBaseType("lpwstr");
			bstr = new ST_VectorBaseType("bstr");
			date = new ST_VectorBaseType("date");
			filetime = new ST_VectorBaseType("filetime");
			_bool = new ST_VectorBaseType("bool");
			cy = new ST_VectorBaseType("cy");
			error = new ST_VectorBaseType("error");
			clsid = new ST_VectorBaseType("clsid");
			cf = new ST_VectorBaseType("cf");
		}

		private ST_VectorBaseType(string val)
		{
			_ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return _ooxmlEnumerationValue;
		}

		public bool Equals(ST_VectorBaseType other)
		{
			if (other == null)
			{
				return false;
			}
			return _ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_VectorBaseType one, ST_VectorBaseType two)
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

		public static bool operator !=(ST_VectorBaseType one, ST_VectorBaseType two)
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
