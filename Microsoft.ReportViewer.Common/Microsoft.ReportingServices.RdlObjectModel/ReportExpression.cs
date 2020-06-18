using Microsoft.ReportingServices.RdlObjectModel.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal struct ReportExpression : IExpression, IXmlSerializable, IFormattable
	{
		private string m_value;

		private DataTypes m_dataType;

		private EvaluationMode m_evaluationMode;

		private static Regex m_nonConstantRegex = new Regex("^\\s*=", RegexOptions.Compiled);

		public string Value
		{
			get
			{
				return m_value ?? "";
			}
			set
			{
				m_value = value;
			}
		}

		public DataTypes DataType
		{
			get
			{
				return m_dataType;
			}
			set
			{
				m_dataType = value;
			}
		}

		object IExpression.Value
		{
			get
			{
				return Value;
			}
			set
			{
				Value = (string)value;
			}
		}

		public string Expression
		{
			get
			{
				return Value;
			}
			set
			{
				Value = value;
			}
		}

		public EvaluationMode EvaluationMode
		{
			get
			{
				return m_evaluationMode;
			}
			set
			{
				m_evaluationMode = value;
			}
		}

		public bool IsExpression
		{
			get
			{
				if (EvaluationMode == EvaluationMode.Auto)
				{
					return IsExpressionString(m_value);
				}
				return false;
			}
		}

		public ReportExpression(string value)
		{
			m_value = value;
			m_dataType = DataTypes.String;
			m_evaluationMode = EvaluationMode.Auto;
		}

		public ReportExpression(string value, EvaluationMode evaluationMode)
		{
			m_value = value;
			m_dataType = DataTypes.String;
			m_evaluationMode = evaluationMode;
		}

		public override string ToString()
		{
			return Value;
		}

		public string ToString(string format, IFormatProvider provider)
		{
			return Value;
		}

		public void GetDependencies(IList<ReportObject> dependencies, ReportObject parent)
		{
			ReportExpressionUtils.GetDependencies(dependencies, parent, Expression);
		}

		public static bool IsExpressionString(string value)
		{
			if (value != null)
			{
				return m_nonConstantRegex.IsMatch(value);
			}
			return false;
		}

		public override bool Equals(object value)
		{
			if (value is ReportExpression)
			{
				ReportExpression reportExpression = (ReportExpression)value;
				if (Value == reportExpression.Value && IsExpression == reportExpression.IsExpression)
				{
					return DataType == reportExpression.DataType;
				}
				return false;
			}
			if (value is string)
			{
				return Equals(new ReportExpression(((string)value) ?? ""));
			}
			if (value == null)
			{
				return Value == "";
			}
			return false;
		}

		public static bool operator ==(ReportExpression left, ReportExpression right)
		{
			return left.Equals(right);
		}

		public static bool operator ==(ReportExpression left, string right)
		{
			return left.Equals(right);
		}

		public static bool operator ==(string left, ReportExpression right)
		{
			return right.Equals(left);
		}

		public static bool operator !=(ReportExpression left, ReportExpression right)
		{
			return !left.Equals(right);
		}

		public static bool operator !=(ReportExpression left, string right)
		{
			return !left.Equals(right);
		}

		public static bool operator !=(string left, ReportExpression right)
		{
			return !right.Equals(left);
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public static explicit operator string(ReportExpression value)
		{
			return value.Value;
		}

		public static implicit operator ReportExpression(string value)
		{
			return new ReportExpression(value);
		}

		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			string attribute = reader.GetAttribute("DataType");
			if (attribute != null)
			{
				DataType = (DataTypes)ParseEnum(typeof(DataTypes), attribute);
			}
			string attribute2 = reader.GetAttribute("EvaluationMode");
			if (attribute2 != null)
			{
				EvaluationMode = (EvaluationMode)ParseEnum(typeof(EvaluationMode), attribute2);
			}
			m_value = reader.ReadString();
			reader.Skip();
		}

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			if (DataType != 0)
			{
				writer.WriteAttributeString("DataType", DataType.ToString());
			}
			if (EvaluationMode != 0)
			{
				writer.WriteAttributeString("EvaluationMode", EvaluationMode.ToString());
			}
			if (Value.Length > 0)
			{
				if (Value.Trim().Length == 0)
				{
					writer.WriteAttributeString("xml", "space", null, "preserve");
				}
				writer.WriteString(Value);
			}
		}

		internal static object ParseEnum(Type type, string value)
		{
			int num = Array.IndexOf(Enum.GetNames(type), value);
			if (num < 0)
			{
				throw new ArgumentException(SRErrors.InvalidValue(value));
			}
			return Enum.GetValues(type).GetValue(num);
		}
	}
	internal struct ReportExpression<T> : IExpression, IXmlSerializable, IFormattable, IShouldSerialize where T : struct
	{
		private T m_value;

		private string m_expression;

		private static MethodInfo m_parseMethod;

		private static int m_parseMethodArgs;

		public T Value
		{
			get
			{
				return m_value;
			}
			set
			{
				m_value = value;
				m_expression = null;
			}
		}

		object IExpression.Value
		{
			get
			{
				return m_value;
			}
			set
			{
				m_value = (T)value;
			}
		}

		public string Expression
		{
			get
			{
				return m_expression;
			}
			set
			{
				m_expression = value;
				m_value = default(T);
			}
		}

		public bool IsExpression => m_expression != null;

		public ReportExpression(T value)
		{
			m_value = value;
			m_expression = null;
		}

		public ReportExpression(string value)
			: this(value, CultureInfo.CurrentCulture)
		{
		}

		public ReportExpression(string value, IFormatProvider provider)
		{
			m_value = default(T);
			m_expression = null;
			if (!string.IsNullOrEmpty(value))
			{
				Init(value, provider);
			}
		}

		private void Init(string value, IFormatProvider provider)
		{
			if (ReportExpression.IsExpressionString(value))
			{
				Expression = value;
				return;
			}
			if (typeof(T).IsSubclassOf(typeof(Enum)))
			{
				Value = (T)ReportExpression.ParseEnum(typeof(T), value);
				return;
			}
			if (typeof(T) == typeof(ReportSize))
			{
				Value = (T)(object)ReportSize.Parse(value, provider);
				return;
			}
			if (typeof(T) == typeof(ReportColor))
			{
				Value = (T)(object)ReportColor.Parse(value, provider);
				return;
			}
			try
			{
				if (typeof(T) == typeof(bool))
				{
					Value = (T)(object)XmlConvert.ToBoolean(value.ToLowerInvariant());
					return;
				}
				Value = (T)GetParseMethod().Invoke(null, (m_parseMethodArgs != 1) ? new object[2]
				{
					value,
					provider
				} : new object[1]
				{
					value
				});
			}
			catch (TargetInvocationException ex)
			{
				if (ex.InnerException != null)
				{
					throw ex.InnerException;
				}
				throw ex;
			}
		}

		public static ReportExpression<T> Parse(string value, IFormatProvider provider)
		{
			return new ReportExpression<T>(value, provider);
		}

		private MethodInfo GetParseMethod()
		{
			if (m_parseMethodArgs == 0)
			{
				m_parseMethod = typeof(T).GetMethod("Parse", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[2]
				{
					typeof(string),
					typeof(IFormatProvider)
				}, null);
				m_parseMethodArgs = 2;
				if (m_parseMethod == null)
				{
					m_parseMethod = typeof(T).GetMethod("Parse", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[1]
					{
						typeof(string)
					}, null);
					m_parseMethodArgs = 1;
				}
			}
			return m_parseMethod;
		}

		public override string ToString()
		{
			return ToString(null, CultureInfo.CurrentCulture);
		}

		public string ToString(string format, IFormatProvider provider)
		{
			if (IsExpression)
			{
				return m_expression;
			}
			if (typeof(T) == typeof(bool) && provider == CultureInfo.InvariantCulture)
			{
				if (!true.Equals(m_value))
				{
					return "false";
				}
				return "true";
			}
			if (typeof(IFormattable).IsAssignableFrom(typeof(T)))
			{
				return ((IFormattable)(object)m_value).ToString(format, provider);
			}
			return m_value.ToString();
		}

		public override bool Equals(object value)
		{
			if (value is ReportExpression<T>)
			{
				if (m_value.Equals(((ReportExpression<T>)value).Value))
				{
					return m_expression == ((ReportExpression<T>)value).Expression;
				}
				return false;
			}
			if (IsExpression)
			{
				if (value is string)
				{
					return m_expression == (string)value;
				}
				return false;
			}
			return m_value.Equals(value);
		}

		public override int GetHashCode()
		{
			int num = m_value.GetHashCode();
			if (m_expression != null)
			{
				num ^= m_expression.GetHashCode();
			}
			return num;
		}

		public void GetDependencies(IList<ReportObject> dependencies, ReportObject parent)
		{
			ReportExpressionUtils.GetDependencies(dependencies, parent, Expression);
		}

		public static bool operator ==(ReportExpression<T> left, ReportExpression<T> right)
		{
			if (left.Value.Equals(right.Value))
			{
				return left.Expression == right.Expression;
			}
			return false;
		}

		public static bool operator ==(ReportExpression<T> left, T right)
		{
			if (!left.IsExpression)
			{
				return left.Value.Equals(right);
			}
			return false;
		}

		public static bool operator ==(T left, ReportExpression<T> right)
		{
			if (!right.IsExpression)
			{
				return right.Value.Equals(left);
			}
			return false;
		}

		public static bool operator ==(ReportExpression<T> left, string right)
		{
			if (left.IsExpression)
			{
				return left.Expression == right;
			}
			return false;
		}

		public static bool operator ==(string left, ReportExpression<T> right)
		{
			if (right.IsExpression)
			{
				return right.Expression == left;
			}
			return false;
		}

		public static bool operator !=(ReportExpression<T> left, ReportExpression<T> right)
		{
			return !(left == right);
		}

		public static bool operator !=(ReportExpression<T> left, T right)
		{
			return !(left == right);
		}

		public static bool operator !=(T left, ReportExpression<T> right)
		{
			return !(left == right);
		}

		public static bool operator !=(ReportExpression<T> left, string right)
		{
			return !(left == right);
		}

		public static bool operator !=(string left, ReportExpression<T> right)
		{
			return !(left == right);
		}

		public static explicit operator T(ReportExpression<T> value)
		{
			return value.Value;
		}

		public static implicit operator ReportExpression<T>(T value)
		{
			return new ReportExpression<T>(value);
		}

		public static implicit operator ReportExpression<T>(T? value)
		{
			if (value.HasValue)
			{
				return new ReportExpression<T>(value.Value);
			}
			return new ReportExpression<T>(null, CultureInfo.InvariantCulture);
		}

		public static explicit operator string(ReportExpression<T> value)
		{
			return value.ToString();
		}

		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			string value = reader.ReadString();
			Init(value, CultureInfo.InvariantCulture);
			reader.Skip();
		}

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			writer.WriteString(ToString(null, CultureInfo.InvariantCulture));
		}

		bool IShouldSerialize.ShouldSerializeThis()
		{
			if (!IsExpression && typeof(IShouldSerialize).IsAssignableFrom(typeof(T)))
			{
				return ((IShouldSerialize)(object)m_value).ShouldSerializeThis();
			}
			return true;
		}

		SerializationMethod IShouldSerialize.ShouldSerializeProperty(string name)
		{
			return SerializationMethod.Auto;
		}
	}
}
