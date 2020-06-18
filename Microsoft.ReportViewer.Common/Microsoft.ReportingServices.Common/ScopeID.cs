using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.Common
{
	internal class ScopeID : IXmlSerializable
	{
		private ScopeValue[] m_scopeValues;

		internal const string SCOPEID = "ScopeID";

		internal const string SCOPEVALUES = "ScopeValues";

		public int ScopeValueCount
		{
			get
			{
				if (m_scopeValues != null)
				{
					return m_scopeValues.Length;
				}
				return 0;
			}
		}

		public IEnumerable<ScopeValue> InstanceID
		{
			get
			{
				int count = ScopeValueCount;
				for (int cursor = 0; cursor < count; cursor++)
				{
					ScopeValue scopeValue = m_scopeValues[cursor];
					if (scopeValue.IsGroupExprValue)
					{
						yield return scopeValue;
					}
				}
			}
		}

		public IEnumerable<ScopeValue> QueryRestartPosition
		{
			get
			{
				int count = ScopeValueCount;
				for (int cursor = 0; cursor < count; cursor++)
				{
					ScopeValue scopeValue = m_scopeValues[cursor];
					if (scopeValue.IsSortExprValue)
					{
						yield return scopeValue;
					}
				}
			}
		}

		internal ScopeID()
		{
		}

		internal ScopeID(ScopeValue[] scopeValues)
		{
			m_scopeValues = scopeValues;
		}

		internal ScopeID(ScopeID scopeID)
			: this(scopeID.m_scopeValues)
		{
		}

		public ScopeValue GetScopeValue(int index)
		{
			return m_scopeValues[index];
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("{ ");
			string value = string.Empty;
			int scopeValueCount = ScopeValueCount;
			for (int i = 0; i < scopeValueCount; i++)
			{
				stringBuilder.Append(value);
				stringBuilder.Append(Convert.ToString(m_scopeValues[i].Value, CultureInfo.InvariantCulture));
				stringBuilder.Append("[");
				stringBuilder.Append(m_scopeValues[i].ScopeType.ToString());
				stringBuilder.Append("]");
				value = ", ";
			}
			stringBuilder.Append(" }");
			return stringBuilder.ToString();
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as ScopeID);
		}

		internal bool Equals(ScopeID scopeID)
		{
			if ((object)this == scopeID)
			{
				return true;
			}
			if ((object)scopeID == null)
			{
				return false;
			}
			return Equals(scopeID, null);
		}

		internal bool Equals(ScopeID scopeID, IEqualityComparer<object> comparer)
		{
			if ((object)scopeID == null)
			{
				return false;
			}
			if (m_scopeValues == scopeID.m_scopeValues)
			{
				return true;
			}
			int scopeValueCount = ScopeValueCount;
			if (scopeValueCount != scopeID.ScopeValueCount)
			{
				return false;
			}
			for (int i = 0; i < scopeValueCount; i++)
			{
				if (!m_scopeValues[i].Equals(scopeID.m_scopeValues[i], comparer))
				{
					return false;
				}
			}
			return true;
		}

		internal int Compare(ScopeID scopeID, IComparer<object> comparer)
		{
			int scopeValueCount = ScopeValueCount;
			if (scopeValueCount == 0)
			{
				if ((object)scopeID == null || scopeID.ScopeValueCount == 0)
				{
					return 0;
				}
				return -1;
			}
			if ((object)scopeID == null || scopeID.ScopeValueCount == 0)
			{
				return 1;
			}
			if (scopeValueCount != scopeID.ScopeValueCount)
			{
				throw new ArgumentException();
			}
			for (int i = 0; i < scopeValueCount; i++)
			{
				int num = comparer.Compare(m_scopeValues[i].Value, scopeID.m_scopeValues[i].Value);
				if (num != 0)
				{
					return num;
				}
			}
			return 0;
		}

		internal int GetHashCode(IEqualityComparer<object> comparer)
		{
			int scopeValueCount = ScopeValueCount;
			int num = scopeValueCount;
			for (int i = 0; i < scopeValueCount; i++)
			{
				num ^= m_scopeValues[i].GetHashCode(comparer);
			}
			return num;
		}

		public static bool operator ==(ScopeID scopeID1, ScopeID scopeID2)
		{
			if ((object)scopeID1 == scopeID2)
			{
				return true;
			}
			return scopeID1?.Equals(scopeID2) ?? false;
		}

		public static bool operator !=(ScopeID scopeID1, ScopeID scopeID2)
		{
			return !(scopeID1 == scopeID2);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public virtual XmlSchema GetSchema()
		{
			throw new NotImplementedException();
		}

		public virtual void ReadXml(XmlReader xmlReader)
		{
			while (xmlReader.Read())
			{
				if (xmlReader.NodeType == XmlNodeType.Element)
				{
					ReadXmlElement(xmlReader);
				}
			}
		}

		protected virtual void ReadXmlElement(XmlReader xmlReader)
		{
			if (xmlReader.Name != "ScopeValues")
			{
				return;
			}
			List<ScopeValue> list = new List<ScopeValue>();
			using (XmlReader xmlReader2 = xmlReader.ReadSubtree())
			{
				while (xmlReader2.Read())
				{
					if (xmlReader2.NodeType == XmlNodeType.Element && xmlReader2.Name == "ScopeValue")
					{
						using (XmlReader xmlReader3 = xmlReader.ReadSubtree())
						{
							ScopeValue scopeValue = new ScopeValue();
							scopeValue.ReadXml(xmlReader3);
							list.Add(scopeValue);
						}
					}
				}
			}
			if (list.Count > 0)
			{
				m_scopeValues = list.ToArray();
			}
			else
			{
				m_scopeValues = null;
			}
		}

		public virtual void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("ScopeID");
			WriteBaseXml(writer);
			writer.WriteEndElement();
		}

		protected void WriteBaseXml(XmlWriter writer)
		{
			writer.WriteStartElement("ScopeValues");
			ScopeValue[] scopeValues = m_scopeValues;
			for (int i = 0; i < scopeValues.Length; i++)
			{
				scopeValues[i].WriteXml(writer);
			}
			writer.WriteEndElement();
		}
	}
}
