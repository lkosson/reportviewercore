using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class ParamValues : Hashtable
	{
		internal Hashtable m_fields = new Hashtable();

		private const string _ParameterValues = "ParameterValues";

		public ParamValueList this[string name] => (ParamValueList)base[name];

		public Hashtable Fields => m_fields;

		public string[] FieldKeys
		{
			get
			{
				string[] array = new string[m_fields.Keys.Count];
				int num = 0;
				foreach (string key in m_fields.Keys)
				{
					array[num++] = key;
				}
				return array;
			}
		}

		public NameValueCollection AsNameValueCollection
		{
			get
			{
				NameValueCollection nameValueCollection = new NameValueCollection(Count, StringComparer.CurrentCulture);
				foreach (ParamValueList value in Values)
				{
					for (int i = 0; i < value.Count; i++)
					{
						nameValueCollection.Add(value[i].Name, value[i].Value);
					}
				}
				return nameValueCollection;
			}
		}

		internal void AddField(string fieldName)
		{
			if (m_fields[fieldName] == null)
			{
				m_fields.Add(fieldName, fieldName);
			}
		}

		internal string GetFieldValue(string fieldName)
		{
			return (string)m_fields[fieldName];
		}

		internal void AddFieldValue(string fieldName, string fieldValue)
		{
			if (m_fields[fieldName] != null)
			{
				m_fields[fieldName] = fieldValue;
			}
		}

		public void FromXml(string xml)
		{
			if (xml == null || xml == "")
			{
				return;
			}
			XmlReader xmlReader = XmlUtil.SafeCreateXmlTextReader(xml);
			try
			{
				if (!xmlReader.Read() || xmlReader.Name != "ParameterValues")
				{
					throw new InvalidXmlException();
				}
				while (true)
				{
					if (!xmlReader.Read())
					{
						return;
					}
					if (xmlReader.IsStartElement("ParameterValue"))
					{
						ParamValue paramValue = new ParamValue(xmlReader, this);
						bool flag = false;
						ParamValueList paramValueList = this[paramValue.Name];
						if (paramValueList == null)
						{
							flag = true;
							paramValueList = new ParamValueList();
						}
						paramValueList.Add(paramValue);
						if (flag)
						{
							Add(paramValue.Name, paramValueList);
						}
					}
					else if (xmlReader.NodeType == XmlNodeType.Element)
					{
						break;
					}
				}
				throw new InvalidXmlException();
			}
			catch (XmlException ex)
			{
				throw new MalformedXmlException(ex);
			}
		}

		public string ToXml(bool outputFieldElements)
		{
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
			try
			{
				xmlTextWriter.WriteStartElement("ParameterValues");
				foreach (ParamValueList value in Values)
				{
					for (int i = 0; i < value.Count; i++)
					{
						value[i].ToXml(xmlTextWriter, outputFieldElements);
					}
				}
				xmlTextWriter.WriteEndElement();
				return stringWriter.ToString();
			}
			finally
			{
				xmlTextWriter.Close();
				stringWriter.Close();
			}
		}

		public string ToOldParameterXml()
		{
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
			try
			{
				xmlTextWriter.WriteStartElement("Parameters");
				foreach (ParamValueList value in Values)
				{
					for (int i = 0; i < value.Count; i++)
					{
						value[i].ToOldParameterXml(xmlTextWriter);
					}
				}
				xmlTextWriter.WriteEndElement();
				return stringWriter.ToString();
			}
			finally
			{
				xmlTextWriter.Close();
				stringWriter.Close();
			}
		}
	}
}
