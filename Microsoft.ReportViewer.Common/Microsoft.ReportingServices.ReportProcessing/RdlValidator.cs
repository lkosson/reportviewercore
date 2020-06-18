using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class RdlValidator
	{
		private sealed class RdlElementStack : ArrayList
		{
			internal new Hashtable this[int index]
			{
				get
				{
					return (Hashtable)base[index];
				}
				set
				{
					base[index] = value;
				}
			}
		}

		private const string MustUnderstandAttributeName = "MustUnderstand";

		private readonly XmlReader m_reader;

		private RdlElementStack m_rdlElementStack;

		private readonly HashSet<string> m_validationNamespaces;

		public RdlValidator(XmlReader xmlReader, IEnumerable<string> validationNamespaces)
		{
			m_reader = xmlReader;
			m_validationNamespaces = new HashSet<string>(validationNamespaces);
		}

		public bool ValidateStartElement(out string message)
		{
			message = null;
			XmlSchemaComplexType xmlSchemaComplexType = null;
			ArrayList arrayList = null;
			if (m_rdlElementStack == null)
			{
				m_rdlElementStack = new RdlElementStack();
			}
			if (m_reader.SchemaInfo != null && m_validationNamespaces.Contains(m_reader.NamespaceURI))
			{
				xmlSchemaComplexType = (m_reader.SchemaInfo.SchemaType as XmlSchemaComplexType);
			}
			if (xmlSchemaComplexType != null)
			{
				arrayList = new ArrayList();
				TraverseParticle(xmlSchemaComplexType.ContentTypeParticle, arrayList);
			}
			if (xmlSchemaComplexType != null && 1 < arrayList.Count && "MapLayersType" != xmlSchemaComplexType.Name && "ReportItemsType" != xmlSchemaComplexType.Name)
			{
				Hashtable hashtable = new Hashtable(arrayList.Count);
				hashtable.Add("_ParentName", m_reader.LocalName);
				hashtable.Add("_Type", xmlSchemaComplexType);
				m_rdlElementStack.Add(hashtable);
			}
			else
			{
				m_rdlElementStack.Add(null);
			}
			if (0 < m_reader.Depth && m_rdlElementStack != null)
			{
				Hashtable hashtable2 = m_rdlElementStack[m_reader.Depth - 1];
				if (hashtable2 != null)
				{
					if (hashtable2.ContainsKey(m_reader.LocalName))
					{
						message = ValidationMessage("rdlValidationInvalidElement", (string)hashtable2["_ParentName"], m_reader.LocalName);
						return false;
					}
					hashtable2.Add(m_reader.LocalName, null);
				}
			}
			string text = (m_reader.GetAttribute("MustUnderstand") ?? string.Empty).Trim();
			if (!string.IsNullOrEmpty(text))
			{
				string[] array = text.Split();
				foreach (string text2 in array)
				{
					string text3 = m_reader.LookupNamespace(text2);
					if (!m_validationNamespaces.Contains(text3))
					{
						int num = 0;
						int num2 = 0;
						IXmlLineInfo obj = (IXmlLineInfo)m_reader;
						num = obj.LineNumber;
						message = RDLValidatingReaderStrings.rdlValidationUnknownRequiredNamespaces(position: obj.LinePosition.ToString(CultureInfo.InvariantCulture.NumberFormat), xmlns: text2, prefix: text3, sqlServerVersionName: "Microsoft SQL Server 2019 Community Technology Preview 2.3", linenumber: num.ToString(CultureInfo.InvariantCulture.NumberFormat));
						return false;
					}
				}
			}
			return true;
		}

		public bool ValidateEndElement(out string message)
		{
			message = null;
			bool result = true;
			if (m_rdlElementStack != null)
			{
				Hashtable hashtable = m_rdlElementStack[m_rdlElementStack.Count - 1];
				if (hashtable != null)
				{
					XmlSchemaComplexType obj = hashtable["_Type"] as XmlSchemaComplexType;
					ArrayList arrayList = new ArrayList();
					TraverseParticle(obj.ContentTypeParticle, arrayList);
					for (int i = 0; i < arrayList.Count; i++)
					{
						XmlSchemaElement xmlSchemaElement = arrayList[i] as XmlSchemaElement;
						if (xmlSchemaElement.MinOccurs > 0m && !hashtable.ContainsKey(xmlSchemaElement.Name))
						{
							result = false;
							message = ValidationMessage("rdlValidationMissingChildElement", hashtable["_ParentName"] as string, xmlSchemaElement.Name);
						}
					}
					m_rdlElementStack[m_rdlElementStack.Count - 1] = null;
				}
				m_rdlElementStack.RemoveAt(m_rdlElementStack.Count - 1);
			}
			return result;
		}

		private static void TraverseParticle(XmlSchemaParticle particle, ArrayList elementDeclsInContentModel)
		{
			if (particle is XmlSchemaElement)
			{
				XmlSchemaElement value = particle as XmlSchemaElement;
				elementDeclsInContentModel.Add(value);
			}
			else
			{
				if (!(particle is XmlSchemaGroupBase))
				{
					return;
				}
				foreach (XmlSchemaParticle item in (particle as XmlSchemaGroupBase).Items)
				{
					TraverseParticle(item, elementDeclsInContentModel);
				}
			}
		}

		private string ValidationMessage(string id, string parentType, string childType)
		{
			int num = 0;
			int num2 = 0;
			IXmlLineInfo xmlLineInfo = m_reader as IXmlLineInfo;
			if (xmlLineInfo != null)
			{
				num = xmlLineInfo.LineNumber;
				num2 = xmlLineInfo.LinePosition;
			}
			return RDLValidatingReaderStrings.Keys.GetString(id, parentType, childType, num.ToString(CultureInfo.InvariantCulture.NumberFormat), num2.ToString(CultureInfo.InvariantCulture.NumberFormat));
		}
	}
}
