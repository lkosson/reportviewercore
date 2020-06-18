using Microsoft.ReportingServices.RdlObjectModel.Serialization;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	[TypeConverter(typeof(ReportSizeConverter))]
	internal struct ReportSize : IComparable, IXmlSerializable, IFormattable, IShouldSerialize
	{
		internal const double CentimetersPerInch = 2.54;

		internal const double MillimetersPerInch = 25.4;

		internal const double PicasPerInch = 6.0;

		internal const double PointsPerInch = 72.0;

		internal const int DefaultDecimalDigits = 5;

		private static float m_dotsPerInch;

		private static readonly ReportSize m_empty;

		private static string m_serializationFormat;

		private static int m_serializedDecimalDigits;

		private static SizeTypes m_defaultType;

		private SizeTypes m_type;

		private double m_value;

		public static SizeTypes DefaultType
		{
			get
			{
				return m_defaultType;
			}
			set
			{
				m_defaultType = value;
			}
		}

		public static int SerializedDecimalDigits
		{
			get
			{
				return m_serializedDecimalDigits;
			}
			set
			{
				if (value <= 0 || value > 99)
				{
					throw new ArgumentException("SerializedDecimalDigits");
				}
				m_serializedDecimalDigits = value;
				m_serializationFormat = "{0:0." + new string('#', value) + "}{1}";
			}
		}

		public static float DotsPerInch
		{
			get
			{
				if (m_dotsPerInch == 0f)
				{
					using (Bitmap image = new Bitmap(1, 1))
					{
						using (Graphics graphics = Graphics.FromImage(image))
						{
							m_dotsPerInch = graphics.DpiX;
						}
					}
				}
				return m_dotsPerInch;
			}
		}

		public static ReportSize Empty => m_empty;

		public SizeTypes Type
		{
			get
			{
				if (m_type == SizeTypes.Invalid)
				{
					return DefaultType;
				}
				return m_type;
			}
		}

		public double Value => m_value;

		public double SerializedValue => Math.Round(m_value, m_serializedDecimalDigits);

		public bool IsEmpty => m_type == SizeTypes.Invalid;

		static ReportSize()
		{
			m_empty = default(ReportSize);
			m_defaultType = SizeTypes.Inch;
			SerializedDecimalDigits = 5;
		}

		public ReportSize(double value, SizeTypes type)
		{
			m_value = value;
			m_type = type;
		}

		public ReportSize(string value)
			: this(value, CultureInfo.CurrentCulture)
		{
		}

		public ReportSize(double value)
		{
			m_value = value;
			m_type = DefaultType;
		}

		public ReportSize(string value, IFormatProvider provider)
			: this(value, provider, DefaultType)
		{
		}

		public ReportSize(string value, IFormatProvider provider, SizeTypes defaultType)
		{
			m_value = 0.0;
			m_type = SizeTypes.Invalid;
			if (!string.IsNullOrEmpty(value))
			{
				Init(value, provider, defaultType);
			}
		}

		private void Init(string value, IFormatProvider provider, SizeTypes defaultType)
		{
			if (provider == null)
			{
				provider = CultureInfo.CurrentCulture;
			}
			string text = value.Trim();
			int length = text.Length;
			NumberFormatInfo numberFormatInfo = provider.GetFormat(typeof(NumberFormatInfo)) as NumberFormatInfo;
			if (numberFormatInfo == null)
			{
				numberFormatInfo = CultureInfo.InvariantCulture.NumberFormat;
			}
			int num = -1;
			for (int i = 0; i < length; i++)
			{
				char c = text[i];
				if (!char.IsDigit(c) && c != numberFormatInfo.NegativeSign[0] && c != numberFormatInfo.NumberDecimalSeparator[0] && c != numberFormatInfo.NumberGroupSeparator[0])
				{
					break;
				}
				num = i;
			}
			if (num == -1)
			{
				throw new FormatException(SRErrors.UnitParseNoDigits(value));
			}
			if (num < length - 1)
			{
				try
				{
					m_type = GetTypeFromString(text.Substring(num + 1).Trim().ToLowerInvariant());
				}
				catch (ArgumentException ex)
				{
					throw new FormatException(ex.Message);
				}
			}
			else
			{
				if (defaultType == SizeTypes.Invalid)
				{
					throw new FormatException(SRErrors.UnitParseNoUnit(value));
				}
				m_type = defaultType;
			}
			string text2 = text.Substring(0, num + 1);
			try
			{
				m_value = double.Parse(text2, provider);
			}
			catch
			{
				throw new FormatException(SRErrors.UnitParseNumericPart(value, text2, m_type.ToString("G")));
			}
		}

		public static ReportSize Parse(string s, IFormatProvider provider)
		{
			return new ReportSize(s, provider);
		}

		public override int GetHashCode()
		{
			return (m_type.GetHashCode() << 2) ^ m_value.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is ReportSize))
			{
				return false;
			}
			ReportSize reportSize = (ReportSize)obj;
			if (reportSize.Value == Value && reportSize.m_type == m_type)
			{
				return true;
			}
			return false;
		}

		public static bool operator ==(ReportSize left, ReportSize right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(ReportSize left, ReportSize right)
		{
			return !left.Equals(right);
		}

		public static bool operator <(ReportSize left, ReportSize right)
		{
			return left.ToMillimeters() < right.ToMillimeters();
		}

		public static bool operator >(ReportSize left, ReportSize right)
		{
			return left.ToMillimeters() > right.ToMillimeters();
		}

		public static ReportSize operator +(ReportSize size1, ReportSize size2)
		{
			if (size1.IsEmpty)
			{
				size1 = new ReportSize(0.0);
			}
			size1.SetPixels(size1.ToPixels() + size2.ToPixels());
			return size1;
		}

		public static ReportSize operator -(ReportSize size1, ReportSize size2)
		{
			if (size1.IsEmpty)
			{
				size1 = new ReportSize(0.0);
			}
			size1.SetPixels(size1.ToPixels() - size2.ToPixels());
			return size1;
		}

		private static string GetStringFromType(SizeTypes type)
		{
			switch (type)
			{
			case SizeTypes.Point:
				return "pt";
			case SizeTypes.Pica:
				return "pc";
			case SizeTypes.Inch:
				return "in";
			case SizeTypes.Mm:
				return "mm";
			case SizeTypes.Cm:
				return "cm";
			default:
				return string.Empty;
			}
		}

		internal static SizeTypes GetTypeFromString(string value)
		{
			if (value != null && value.Length > 0)
			{
				if (value.Equals("pt"))
				{
					return SizeTypes.Point;
				}
				if (value.Equals("pc"))
				{
					return SizeTypes.Pica;
				}
				if (value.Equals("in"))
				{
					return SizeTypes.Inch;
				}
				if (value.Equals("mm"))
				{
					return SizeTypes.Mm;
				}
				if (value.Equals("cm"))
				{
					return SizeTypes.Cm;
				}
				throw new ArgumentException(SRErrors.InvalidUnitType(value));
			}
			return DefaultType;
		}

		public int ToIntPixels()
		{
			return Convert.ToInt32(ConvertToPixels(m_value, m_type));
		}

		public double ToPixels()
		{
			return ConvertToPixels(m_value, m_type);
		}

		public void SetPixels(double pixels)
		{
			m_value = ConvertToUnits(pixels, m_type);
		}

		public static ReportSize FromPixels(double pixels, SizeTypes type)
		{
			return new ReportSize(ConvertToUnits(pixels, type), type);
		}

		public double ToMillimeters()
		{
			return ConvertToMillimeters(m_value, m_type);
		}

		public double ToCentimeters()
		{
			return 0.1 * ConvertToMillimeters(m_value, m_type);
		}

		public double ToInches()
		{
			return ConvertToMillimeters(m_value, m_type) / 25.4;
		}

		public double ToPoints()
		{
			if (m_type == SizeTypes.Point)
			{
				return m_value;
			}
			return ToInches() * 72.0;
		}

		public override string ToString()
		{
			return ToString(null, CultureInfo.CurrentCulture);
		}

		public string ToString(string format, IFormatProvider provider)
		{
			if (IsEmpty)
			{
				return string.Empty;
			}
			return string.Format(provider, m_serializationFormat, SerializedValue, GetStringFromType(m_type));
		}

		internal ReportSize ChangeType(SizeTypes type)
		{
			if (type == m_type)
			{
				return this;
			}
			return new ReportSize(ConvertToUnits(ConvertToPixels(m_value, m_type), type), type);
		}

		internal double ConvertToPixels(double value, SizeTypes type)
		{
			switch (type)
			{
			case SizeTypes.Cm:
				value *= (double)DotsPerInch / 2.54;
				break;
			case SizeTypes.Inch:
				value *= (double)DotsPerInch;
				break;
			case SizeTypes.Mm:
				value *= (double)DotsPerInch / 25.4;
				break;
			case SizeTypes.Pica:
				value *= (double)DotsPerInch / 6.0;
				break;
			case SizeTypes.Point:
				value *= (double)DotsPerInch / 72.0;
				break;
			}
			return value;
		}

		internal double ConvertToMillimeters(double value, SizeTypes type)
		{
			switch (type)
			{
			case SizeTypes.Cm:
				value *= 10.0;
				break;
			case SizeTypes.Inch:
				value *= 25.4;
				break;
			case SizeTypes.Pica:
				value *= 4.2333333333333334;
				break;
			case SizeTypes.Point:
				value *= 0.35277777777777775;
				break;
			}
			return value;
		}

		internal static double ConvertToUnits(double pixels, SizeTypes type)
		{
			double num = pixels;
			switch (type)
			{
			case SizeTypes.Cm:
				num /= (double)DotsPerInch / 2.54;
				break;
			case SizeTypes.Inch:
				num /= (double)DotsPerInch;
				break;
			case SizeTypes.Mm:
				num /= (double)DotsPerInch / 25.4;
				break;
			case SizeTypes.Pica:
				num /= (double)DotsPerInch / 6.0;
				break;
			case SizeTypes.Point:
				num /= (double)DotsPerInch / 72.0;
				break;
			}
			return num;
		}

		int IComparable.CompareTo(object value)
		{
			if (!(value is ReportSize))
			{
				throw new ArgumentException("value is not a RdlSize");
			}
			double num = ToMillimeters();
			double num2 = ((ReportSize)value).ToMillimeters();
			if (!(num < num2))
			{
				if (!(num > num2))
				{
					return 0;
				}
				return 1;
			}
			return -1;
		}

		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			string value = reader.ReadString();
			Init(value, CultureInfo.InvariantCulture, SizeTypes.Invalid);
			reader.Skip();
		}

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			string text = ToString(null, CultureInfo.InvariantCulture);
			writer.WriteString(text);
		}

		bool IShouldSerialize.ShouldSerializeThis()
		{
			return !IsEmpty;
		}

		SerializationMethod IShouldSerialize.ShouldSerializeProperty(string name)
		{
			return SerializationMethod.Auto;
		}
	}
}
