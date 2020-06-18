namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser
{
	internal struct OoxmlBool
	{
		public static readonly OoxmlBool OoxmlTrue = new OoxmlBool(b: true);

		public static readonly OoxmlBool OoxmlFalse = new OoxmlBool(b: false);

		private bool _value;

		private OoxmlBool(bool b)
		{
			_value = b;
		}

		public static implicit operator OoxmlBool(bool b)
		{
			if (!b)
			{
				return OoxmlFalse;
			}
			return OoxmlTrue;
		}

		public static implicit operator bool(OoxmlBool b)
		{
			return b._value;
		}

		public static OoxmlBool operator ==(OoxmlBool b1, OoxmlBool b2)
		{
			if (b1._value != b2._value)
			{
				return OoxmlFalse;
			}
			return OoxmlTrue;
		}

		public static OoxmlBool operator !=(OoxmlBool b1, OoxmlBool b2)
		{
			if (b1._value == b2._value)
			{
				return OoxmlFalse;
			}
			return OoxmlTrue;
		}

		public static OoxmlBool operator !(OoxmlBool b)
		{
			if (!(bool)b)
			{
				return OoxmlTrue;
			}
			return OoxmlFalse;
		}

		public override string ToString()
		{
			if (!_value)
			{
				return "0";
			}
			return "1";
		}

		public bool Equals(OoxmlBool other)
		{
			return other._value.Equals(_value);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj.GetType() != typeof(OoxmlBool))
			{
				return false;
			}
			return Equals((OoxmlBool)obj);
		}

		public override int GetHashCode()
		{
			return _value.GetHashCode();
		}
	}
}
