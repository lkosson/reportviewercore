using System;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.ReportingServices.Common
{
	internal sealed class ScopeValue : SerializableValue
	{
		private readonly string m_key;

		private ScopeIDType m_scopeType = ScopeIDType.GroupValues;

		internal const string SCOPEVALUE = "ScopeValue";

		internal const string SCOPETYPE = "ScopeType";

		internal ScopeIDType ScopeType => m_scopeType;

		internal string Key => m_key;

		internal bool IsGroupExprValue
		{
			get
			{
				if (m_scopeType != ScopeIDType.GroupValues)
				{
					return m_scopeType == ScopeIDType.SortGroup;
				}
				return true;
			}
		}

		internal bool IsSortExprValue
		{
			get
			{
				if (m_scopeType != ScopeIDType.SortValues)
				{
					return m_scopeType == ScopeIDType.SortGroup;
				}
				return true;
			}
		}

		internal ScopeValue()
		{
		}

		internal ScopeValue(object value, ScopeIDType scopeType)
			: base(value)
		{
			m_scopeType = scopeType;
		}

		internal ScopeValue(object value, ScopeIDType scopeType, string key)
			: this(value, scopeType)
		{
			m_key = key;
		}

		internal ScopeValue(object value, ScopeIDType scopeType, DataTypeCode dataTypeCode)
			: base(value, dataTypeCode)
		{
			m_scopeType = scopeType;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as ScopeValue);
		}

		internal bool Equals(ScopeValue scopeValue)
		{
			if ((object)this == scopeValue)
			{
				return true;
			}
			if ((object)scopeValue == null)
			{
				return false;
			}
			return Equals(scopeValue, null);
		}

		public static bool operator ==(ScopeValue scopeValue1, ScopeValue scopeValue2)
		{
			if ((object)scopeValue1 == scopeValue2)
			{
				return true;
			}
			return scopeValue1?.Equals(scopeValue2) ?? false;
		}

		public static bool operator !=(ScopeValue scopeValue1, ScopeValue scopeValue2)
		{
			return !(scopeValue1 == scopeValue2);
		}

		internal bool Equals(ScopeValue scopeValue, IEqualityComparer<object> comparer)
		{
			if ((object)scopeValue == null)
			{
				return false;
			}
			if (ScopeType != scopeValue.ScopeType)
			{
				return false;
			}
			return comparer?.Equals(base.Value, scopeValue.Value) ?? ObjectSerializer.Equals(base.Value, scopeValue.Value, base.TypeCode, scopeValue.TypeCode);
		}

		internal int GetHashCode(IEqualityComparer<object> comparer)
		{
			int num = 0;
			if (base.Value != null)
			{
				num = comparer.GetHashCode(base.Value);
				num <<= 20;
			}
			return num | (((int)ScopeType << 8) & (int)base.TypeCode);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		protected override void ReadDerivedXml(XmlReader xmlReader)
		{
			XmlNodeType nodeType = xmlReader.NodeType;
			if (nodeType == XmlNodeType.Element)
			{
				string name = xmlReader.Name;
				if (name == "ScopeValue")
				{
					ReadAttributes(xmlReader);
				}
			}
		}

		private void ReadAttributes(XmlReader xmlReader)
		{
			for (int i = 0; i < xmlReader.AttributeCount; i++)
			{
				xmlReader.MoveToAttribute(i);
				string name = xmlReader.Name;
				if (name == "ScopeType")
				{
					m_scopeType = (ScopeIDType)Enum.Parse(typeof(ScopeIDType), xmlReader.Value, ignoreCase: false);
				}
			}
		}

		public override void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("ScopeValue");
			writer.WriteAttributeString("ScopeType", m_scopeType.ToString());
			WriteBaseXml(writer);
			writer.WriteEndElement();
		}
	}
}
