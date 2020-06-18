using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Xml;

namespace Microsoft.Reporting.Map.WebForms
{
	[TypeConverter(typeof(FieldConverter))]
	internal class Field : NamedElement
	{
		private Type type;

		private bool uniqueIdentifier;

		private bool isTemporary;

		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeField_Name")]
		public override string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				if (FieldHasData())
				{
					throw new ArgumentException(SR.ExceptionCannotRenameField);
				}
				base.Name = value;
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeField_Type")]
		[TypeConverter(typeof(DataAttributeType_TypeConverter))]
		public Type Type
		{
			get
			{
				return type;
			}
			set
			{
				type = ConvertToSupportedType(value);
			}
		}

		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeField_UniqueIdentifier")]
		[DefaultValue(false)]
		public bool UniqueIdentifier
		{
			get
			{
				return uniqueIdentifier;
			}
			set
			{
				uniqueIdentifier = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public override object Tag
		{
			get
			{
				return base.Tag;
			}
			set
			{
				base.Tag = value;
			}
		}

		[Browsable(false)]
		[DefaultValue(false)]
		public bool IsTemporary
		{
			get
			{
				return isTemporary;
			}
			set
			{
				isTemporary = value;
			}
		}

		public Field()
			: this(null)
		{
		}

		internal Field(CommonElements common)
			: base(common)
		{
			type = typeof(string);
		}

		public override string ToString()
		{
			return Name;
		}

		public bool IsNumeric()
		{
			if (Type == typeof(int) || Type == typeof(double) || Type == typeof(decimal) || Type == typeof(DateTime) || Type == typeof(TimeSpan))
			{
				return true;
			}
			return false;
		}

		public string GetKeyword()
		{
			string text = Name;
			if (common == null || common.MapCore.UppercaseFieldKeywords)
			{
				text = text.ToUpper(CultureInfo.InvariantCulture);
			}
			text = text.Replace(' ', '_');
			return "#" + text;
		}

		internal MapCore GetMapCore()
		{
			return (MapCore)ParentElement;
		}

		private static bool IsValid(Type type)
		{
			if (type == typeof(string) || type == typeof(int) || type == typeof(double) || type == typeof(decimal) || type == typeof(bool) || type == typeof(DateTime) || type == typeof(TimeSpan))
			{
				return true;
			}
			return false;
		}

		internal string FormatValue(object value)
		{
			if (value == null)
			{
				return null;
			}
			if (Type == typeof(string))
			{
				return XmlConvert.EncodeName(Convert.ToString(value, CultureInfo.InvariantCulture));
			}
			if (Type == typeof(int))
			{
				return XmlConvert.ToString(Convert.ToInt32(value, CultureInfo.InvariantCulture));
			}
			if (Type == typeof(double))
			{
				return XmlConvert.ToString(Convert.ToDouble(value, CultureInfo.InvariantCulture));
			}
			if (Type == typeof(decimal))
			{
				return XmlConvert.ToString(Convert.ToDecimal(value, CultureInfo.InvariantCulture));
			}
			if (Type == typeof(bool))
			{
				return XmlConvert.ToString(Convert.ToBoolean(value, CultureInfo.InvariantCulture));
			}
			if (Type == typeof(DateTime))
			{
				return XmlConvert.ToString(Convert.ToDateTime(value, CultureInfo.InvariantCulture));
			}
			return XmlConvert.ToString((TimeSpan)value);
		}

		internal void ParseValue(string fieldValue, Hashtable fields)
		{
			if (Type == typeof(string))
			{
				fields[Name] = XmlConvert.DecodeName(fieldValue);
			}
			else if (Type == typeof(int))
			{
				fields[Name] = XmlConvert.ToInt32(fieldValue);
			}
			else if (Type == typeof(double))
			{
				fields[Name] = XmlConvert.ToDouble(fieldValue);
			}
			else if (Type == typeof(decimal))
			{
				fields[Name] = XmlConvert.ToDecimal(fieldValue);
			}
			else if (Type == typeof(bool))
			{
				fields[Name] = XmlConvert.ToBoolean(fieldValue);
			}
			else if (Type == typeof(DateTime))
			{
				fields[Name] = XmlConvert.ToDateTime(fieldValue);
			}
			else
			{
				fields[Name] = XmlConvert.ToTimeSpan(fieldValue);
			}
		}

		internal void SetValue(object value, Hashtable fields)
		{
			if (Type == typeof(string))
			{
				fields[Name] = Convert.ToString(value, CultureInfo.InvariantCulture);
			}
			else if (Type == typeof(int))
			{
				fields[Name] = Convert.ToInt32(value, CultureInfo.InvariantCulture);
			}
			else if (Type == typeof(double))
			{
				fields[Name] = Convert.ToDouble(value, CultureInfo.InvariantCulture);
			}
			else if (Type == typeof(decimal))
			{
				fields[Name] = Convert.ToDecimal(value, CultureInfo.InvariantCulture);
			}
			else if (Type == typeof(bool))
			{
				fields[Name] = Convert.ToBoolean(value, CultureInfo.InvariantCulture);
			}
			else if (Type == typeof(DateTime))
			{
				fields[Name] = Convert.ToDateTime(value, CultureInfo.InvariantCulture);
			}
			else
			{
				fields[Name] = (TimeSpan)value;
			}
		}

		internal object Parse(string stringValue)
		{
			try
			{
				if (Type == typeof(string))
				{
					return stringValue;
				}
				if (string.IsNullOrEmpty(stringValue))
				{
					return null;
				}
				if (Type == typeof(int))
				{
					return int.Parse(stringValue, CultureInfo.InvariantCulture);
				}
				if (Type == typeof(double))
				{
					return double.Parse(stringValue, CultureInfo.InvariantCulture);
				}
				if (Type == typeof(decimal))
				{
					return decimal.Parse(stringValue, CultureInfo.InvariantCulture);
				}
				if (Type == typeof(bool))
				{
					return bool.Parse(stringValue);
				}
				if (Type == typeof(DateTime))
				{
					return DateTime.Parse(stringValue, CultureInfo.InvariantCulture);
				}
				return TimeSpan.Parse(stringValue);
			}
			catch
			{
				return null;
			}
		}

		internal static object ConvertToSupportedValue(object value)
		{
			Type obj = value.GetType();
			Type type = ConvertToSupportedType(obj);
			if (obj == type)
			{
				return value;
			}
			return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
		}

		internal static Type ConvertToSupportedType(Type valueType)
		{
			if (IsValid(valueType))
			{
				return valueType;
			}
			if (valueType == typeof(char))
			{
				return typeof(string);
			}
			if (valueType == typeof(float))
			{
				return typeof(double);
			}
			if (valueType == typeof(byte) || valueType == typeof(sbyte) || valueType == typeof(short) || valueType == typeof(ushort))
			{
				return typeof(int);
			}
			if (valueType == typeof(uint) || valueType == typeof(long) || valueType == typeof(ulong) || valueType == typeof(ushort))
			{
				return typeof(decimal);
			}
			return typeof(string);
		}

		internal double ConvertToDouble(object fieldValue)
		{
			if (fieldValue == null)
			{
				return double.NaN;
			}
			if (Type == typeof(int))
			{
				return (int)fieldValue;
			}
			if (Type == typeof(double))
			{
				return (double)fieldValue;
			}
			if (Type == typeof(decimal))
			{
				return (double)(decimal)fieldValue;
			}
			if (Type == typeof(DateTime))
			{
				TimeSpan timeSpan = new TimeSpan(((DateTime)fieldValue).Ticks);
				return timeSpan.TotalMilliseconds;
			}
			if (Type == typeof(TimeSpan))
			{
				return ((TimeSpan)fieldValue).TotalMilliseconds;
			}
			throw new Exception(fieldValue.ToString() + " is not a supported numeric type.");
		}

		internal static string ToStringInvariant(object fieldValue)
		{
			if (fieldValue == null)
			{
				return string.Empty;
			}
			if (fieldValue is IConvertible)
			{
				return ((string)((IConvertible)fieldValue).ToType(typeof(string), CultureInfo.InvariantCulture)).ToString(CultureInfo.InvariantCulture);
			}
			return fieldValue.ToString();
		}

		internal bool FieldHasData()
		{
			MapCore mapCore = GetMapCore();
			if (mapCore != null && mapCore.IsDesignMode())
			{
				if (Collection == mapCore.GroupFields)
				{
					foreach (Group group in mapCore.Groups)
					{
						if (group[Name] != null)
						{
							return true;
						}
					}
				}
				else if (Collection == mapCore.ShapeFields)
				{
					foreach (Shape shape in mapCore.Shapes)
					{
						if (shape[Name] != null)
						{
							return true;
						}
					}
				}
				else if (Collection == mapCore.PathFields)
				{
					foreach (Path path in mapCore.Paths)
					{
						if (path[Name] != null)
						{
							return true;
						}
					}
				}
				else if (Collection == mapCore.SymbolFields)
				{
					foreach (Symbol symbol in mapCore.Symbols)
					{
						if (symbol[Name] != null)
						{
							return true;
						}
					}
				}
			}
			return false;
		}
	}
}
