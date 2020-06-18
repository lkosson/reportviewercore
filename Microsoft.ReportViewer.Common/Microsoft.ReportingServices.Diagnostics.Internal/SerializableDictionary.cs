using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Internal
{
	public sealed class SerializableDictionary : Dictionary<string, int>, IXmlSerializable
	{
		internal SerializableDictionary()
		{
		}

		internal SerializableDictionary(IDictionary<string, int> dictionary)
			: base(dictionary)
		{
		}

		internal SerializableDictionary(IEqualityComparer<string> comparer)
			: base(comparer)
		{
		}

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, int> current = enumerator.Current;
					writer.WriteStartElement(current.Key);
					writer.WriteValue(current.Value);
					writer.WriteEndElement();
				}
			}
		}

		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			throw new NotImplementedException();
		}

		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}
	}
}
