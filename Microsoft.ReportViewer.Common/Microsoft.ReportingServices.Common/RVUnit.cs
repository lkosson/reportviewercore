using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;

namespace Microsoft.ReportingServices.Common
{
	internal struct RVUnit
	{
		public static readonly RVUnit Empty = default(RVUnit);

		private double m_value;

		private RVUnitType m_type;

		private static RVUnitType DefaultType = RVUnitType.Inch;

		internal const double Pt1 = 0.35277777777777775;

		internal const double Pc1 = 4.2333333333333325;

		public bool IsEmpty => m_type == (RVUnitType)0;

		public RVUnitType Type
		{
			get
			{
				if (!IsEmpty)
				{
					return m_type;
				}
				return DefaultType;
			}
		}

		public double Value => m_value;

		public RVUnit(double value, RVUnitType type)
		{
			m_value = value;
			m_type = type;
		}

		public RVUnit(string value)
			: this(value, CultureInfo.CurrentCulture, DefaultType)
		{
		}

		public RVUnit(string value, CultureInfo culture)
			: this(value, culture, DefaultType)
		{
		}

		public RVUnit(string value, CultureInfo culture, RVUnitType defaultType)
		{
			m_value = 0.0;
			m_type = defaultType;
			if (value != null && value.Length != 0)
			{
				Init(value, culture, defaultType);
			}
		}

		private void Init(string value, CultureInfo culture, RVUnitType defaultType)
		{
			if (culture == null)
			{
				culture = CultureInfo.CurrentCulture;
			}
			string text = value.Trim().ToLower(culture);
			int length = text.Length;
			int num = -1;
			for (int i = 0; i < length; i++)
			{
				char c = text[i];
				if ((c < '0' || c > '9') && c != '-' && c != '.' && c != ',')
				{
					break;
				}
				num = i;
			}
			if (num == -1)
			{
				throw new FormatException();
			}
			if (num < length - 1)
			{
				m_type = GetTypeFromString(text.Substring(num + 1).Trim());
			}
			else
			{
				if (defaultType == (RVUnitType)0)
				{
					throw new FormatException();
				}
				m_type = defaultType;
			}
			string text2 = text.Substring(0, num + 1);
			try
			{
				TypeConverter typeConverter = new SingleConverter();
				m_value = (float)typeConverter.ConvertFromString(null, culture, text2);
			}
			catch (Exception ex)
			{
				Exception ex2 = FindStoppingException(ex);
				if (ex2 == ex)
				{
					throw;
				}
				if (ex2 != null)
				{
					throw ex2;
				}
				throw new FormatException();
			}
		}

		private static Exception FindStoppingException(Exception e)
		{
			if (e is OutOfMemoryException || e is ExecutionEngineException || e is StackOverflowException || e is ThreadAbortException)
			{
				return e;
			}
			Exception innerException = e.InnerException;
			if (innerException != null)
			{
				return FindStoppingException(innerException);
			}
			return null;
		}

		private static string GetStringFromType(RVUnitType type)
		{
			switch (type)
			{
			case RVUnitType.Point:
				return "pt";
			case RVUnitType.Pica:
				return "pc";
			case RVUnitType.Inch:
				return "in";
			case RVUnitType.Mm:
				return "mm";
			case RVUnitType.Cm:
				return "cm";
			case RVUnitType.Percentage:
				return "%";
			case RVUnitType.Em:
				return "em";
			case RVUnitType.Ex:
				return "ex";
			default:
				return string.Empty;
			}
		}

		public static RVUnitType GetTypeFromString(string value)
		{
			if (value != null && value.Length > 0)
			{
				if (value.Equals("pt"))
				{
					return RVUnitType.Point;
				}
				if (value.Equals("pc"))
				{
					return RVUnitType.Pica;
				}
				if (value.Equals("in"))
				{
					return RVUnitType.Inch;
				}
				if (value.Equals("mm"))
				{
					return RVUnitType.Mm;
				}
				if (value.Equals("cm"))
				{
					return RVUnitType.Cm;
				}
				throw new ArgumentOutOfRangeException("value");
			}
			return DefaultType;
		}

		internal static RVUnit Parse(string s)
		{
			return new RVUnit(s, null);
		}

		public static RVUnit Parse(string s, CultureInfo culture)
		{
			return new RVUnit(s, culture);
		}

		internal static bool TryParse(string s, out RVUnit rvUnit)
		{
			return TryParse(s, null, out rvUnit);
		}

		public static bool TryParse(string s, CultureInfo culture, out RVUnit rvUnit)
		{
			try
			{
				rvUnit = new RVUnit(s, culture);
				return true;
			}
			catch (Exception ex)
			{
				Exception ex2 = FindStoppingException(ex);
				if (ex2 == ex)
				{
					throw;
				}
				if (ex2 != null)
				{
					throw ex2;
				}
				rvUnit = Empty;
				return false;
			}
		}

		public override string ToString()
		{
			return ToString(CultureInfo.CurrentCulture);
		}

		public string ToString(CultureInfo culture)
		{
			if (IsEmpty)
			{
				return string.Empty;
			}
			return m_value.ToString("0.#####", culture) + GetStringFromType(m_type);
		}

		public double ToMillimeters()
		{
			double num = Value;
			switch (Type)
			{
			case RVUnitType.Cm:
				num *= 10.0;
				break;
			case RVUnitType.Inch:
				num *= 25.4;
				break;
			case RVUnitType.Pica:
				num *= 4.2333333333333325;
				break;
			case RVUnitType.Point:
				num *= 0.35277777777777775;
				break;
			}
			return num;
		}
	}
}
