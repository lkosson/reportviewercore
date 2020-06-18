namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.drawingml.x2006.main
{
	internal class ST_BlipCompression
	{
		private string _ooxmlEnumerationValue;

		private static ST_BlipCompression _email;

		private static ST_BlipCompression _screen;

		private static ST_BlipCompression _print;

		private static ST_BlipCompression _hqprint;

		private static ST_BlipCompression _none;

		public static ST_BlipCompression email
		{
			get
			{
				return _email;
			}
			private set
			{
				_email = value;
			}
		}

		public static ST_BlipCompression screen
		{
			get
			{
				return _screen;
			}
			private set
			{
				_screen = value;
			}
		}

		public static ST_BlipCompression print
		{
			get
			{
				return _print;
			}
			private set
			{
				_print = value;
			}
		}

		public static ST_BlipCompression hqprint
		{
			get
			{
				return _hqprint;
			}
			private set
			{
				_hqprint = value;
			}
		}

		public static ST_BlipCompression none
		{
			get
			{
				return _none;
			}
			private set
			{
				_none = value;
			}
		}

		static ST_BlipCompression()
		{
			email = new ST_BlipCompression("email");
			screen = new ST_BlipCompression("screen");
			print = new ST_BlipCompression("print");
			hqprint = new ST_BlipCompression("hqprint");
			none = new ST_BlipCompression("none");
		}

		private ST_BlipCompression(string val)
		{
			_ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return _ooxmlEnumerationValue;
		}

		public bool Equals(ST_BlipCompression other)
		{
			if (other == null)
			{
				return false;
			}
			return _ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_BlipCompression one, ST_BlipCompression two)
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

		public static bool operator !=(ST_BlipCompression one, ST_BlipCompression two)
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
