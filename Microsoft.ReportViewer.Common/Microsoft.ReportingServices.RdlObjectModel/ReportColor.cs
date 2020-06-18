using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.RdlObjectModel.Serialization;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	[TypeConverter(typeof(ReportColorConverter))]
	internal struct ReportColor : IXmlSerializable, IFormattable, IShouldSerialize
	{
		private Color m_color;

		private static readonly ReportColor m_empty;

		public static ReportColor Empty => m_empty;

		public Color Color
		{
			get
			{
				return m_color;
			}
			set
			{
				m_color = value;
			}
		}

		public bool IsEmpty => Color.Empty == m_color;

		public ReportColor(Color color)
		{
			m_color = color;
		}

		public ReportColor(string value)
		{
			m_color = Color.Empty;
			Init(value);
		}

		private void Init(string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				Color = RdlStringToColor(value);
			}
		}

		internal static Color RdlStringToColor(string value)
		{
			if (value[0] == '#')
			{
				return RgbStringToColor(value);
			}
			Color result = FromName(value);
			if (!result.IsKnownColor)
			{
				throw new ArgumentException(SRErrors.InvalidColor(value));
			}
			return result;
		}

		private static Color RgbStringToColor(string value)
		{
			byte alpha = byte.MaxValue;
			if (value == "#00ffffff")
			{
				return Color.Transparent;
			}
			if (value == "#00000000")
			{
				return Color.Empty;
			}
			bool flag = true;
			if ((value.Length != 7 && value.Length != 9) || value[0] != '#')
			{
				flag = false;
			}
			else
			{
				string text = "abcdefABCDEF";
				for (int i = 1; i < value.Length; i++)
				{
					if (!char.IsDigit(value[i]) && -1 == text.IndexOf(value[i]))
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				int num = 1;
				if (value.Length == 9)
				{
					alpha = Convert.ToByte(value.Substring(num, 2), 16);
					num += 2;
				}
				byte red = Convert.ToByte(value.Substring(num, 2), 16);
				byte green = Convert.ToByte(value.Substring(num + 2, 2), 16);
				byte blue = Convert.ToByte(value.Substring(num + 4, 2), 16);
				return Color.FromArgb(alpha, red, green, blue);
			}
			throw new ArgumentException(SRErrors.InvalidColor(value));
		}

		public static string ColorToRdlString(Color c)
		{
			if (c.IsEmpty)
			{
				return "";
			}
			if (c == Color.Transparent)
			{
				return "#00ffffff";
			}
			if (c.IsNamedColor && !c.IsSystemColor)
			{
				return ToName(c);
			}
			if (c.A == byte.MaxValue)
			{
				return StringUtil.FormatInvariant("#{0:x6}", c.ToArgb() & 0xFFFFFF);
			}
			return StringUtil.FormatInvariant("#{0:x8}", c.ToArgb());
		}

		public static ReportColor Parse(string s, IFormatProvider provider)
		{
			return new ReportColor(s);
		}

		public void SetEmpty()
		{
			m_color = Color.Empty;
		}

		internal static Color FromName(string name)
		{
			if (string.Equals(name, "LightGrey", StringComparison.OrdinalIgnoreCase))
			{
				name = "LightGray";
			}
			return Color.FromName(name);
		}

		internal static string ToName(Color color)
		{
			string text = color.Name;
			if (text == "LightGray")
			{
				text = "LightGrey";
			}
			return text;
		}

		public override string ToString()
		{
			return ColorToRdlString(Color);
		}

		public string ToString(string format, IFormatProvider provider)
		{
			return ToString();
		}

		public override int GetHashCode()
		{
			return m_color.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is ReportColor))
			{
				return false;
			}
			return this == (ReportColor)obj;
		}

		public static bool operator ==(ReportColor left, ReportColor right)
		{
			return left.Color == right.Color;
		}

		public static bool operator !=(ReportColor left, ReportColor right)
		{
			return left.Color != right.Color;
		}

		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			string text = reader.ReadString();
			Init(text.Trim());
			reader.Skip();
		}

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			string text = ToString();
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
