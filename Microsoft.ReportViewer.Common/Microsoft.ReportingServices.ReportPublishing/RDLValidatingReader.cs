using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Schema;

namespace Microsoft.ReportingServices.ReportPublishing
{
	internal class RDLValidatingReader : XmlReader
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

			internal RdlElementStack()
			{
			}
		}

		private sealed class XmlNullResolver : XmlUrlResolver
		{
			public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
			{
				throw new XmlException("Can't resolve URI reference.", null);
			}
		}

		private RdlElementStack m_rdlElementStack;

		protected List<string> m_validationNamespaceList;

		protected XmlReader m_reader;

		protected static XmlSchemaContentProcessing m_processContent;

		internal int LineNumber => (m_reader as IXmlLineInfo)?.LineNumber ?? 0;

		internal int LinePosition => (m_reader as IXmlLineInfo)?.LinePosition ?? 0;

		public override XmlReaderSettings Settings => m_reader.Settings;

		public override int AttributeCount => m_reader.AttributeCount;

		public override string BaseURI => m_reader.BaseURI;

		public override int Depth => m_reader.Depth;

		public override bool EOF => m_reader.EOF;

		public override bool HasValue => m_reader.HasValue;

		public override bool IsEmptyElement => m_reader.IsEmptyElement;

		public override string LocalName => m_reader.LocalName;

		public override XmlNameTable NameTable => m_reader.NameTable;

		public override string NamespaceURI => m_reader.NamespaceURI;

		public override XmlNodeType NodeType => m_reader.NodeType;

		public override string Prefix => m_reader.Prefix;

		public override ReadState ReadState => m_reader.ReadState;

		public override string Value => m_reader.Value;

		public event ValidationEventHandler ValidationEventHandler;

		internal RDLValidatingReader(Stream stream, List<Pair<string, Stream>> namespaceSchemaStreamMap)
		{
			try
			{
				m_validationNamespaceList = new List<string>();
				XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
				foreach (Pair<string, Stream> item in namespaceSchemaStreamMap)
				{
					m_validationNamespaceList.Add(item.First);
					xmlReaderSettings.Schemas.Add(item.First, XmlReader.Create(item.Second));
				}
				xmlReaderSettings.ValidationType = ValidationType.Schema;
				xmlReaderSettings.ValidationEventHandler += ValidationCallBack;
				xmlReaderSettings.ProhibitDtd = true;
				xmlReaderSettings.CloseInput = true;
				xmlReaderSettings.XmlResolver = new XmlNullResolver();
				m_reader = XmlReader.Create(stream, xmlReaderSettings);
			}
			catch (SynchronizationLockException innerException)
			{
				throw new ReportProcessingException(RPRes.rsProcessingAbortedByError, ErrorCode.rsProcessingError, innerException);
			}
		}

		private static int CompareWithInvariantCulture(string x, string y, bool ignoreCase)
		{
			return string.Compare(x, y, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}

		public bool Validate(out string message)
		{
			message = null;
			if (!ListUtils.ContainsWithOrdinalComparer(m_reader.NamespaceURI, m_validationNamespaceList))
			{
				return true;
			}
			XmlSchemaComplexType xmlSchemaComplexType = null;
			bool result = true;
			ArrayList arrayList = new ArrayList();
			switch (m_reader.NodeType)
			{
			case XmlNodeType.Element:
			{
				if (m_rdlElementStack == null)
				{
					m_rdlElementStack = new RdlElementStack();
				}
				xmlSchemaComplexType = (m_reader.SchemaInfo.SchemaType as XmlSchemaComplexType);
				if (xmlSchemaComplexType != null)
				{
					TraverseParticle(xmlSchemaComplexType.ContentTypeParticle, arrayList);
				}
				if (!m_reader.IsEmptyElement)
				{
					if (xmlSchemaComplexType != null && 1 < arrayList.Count && CompareWithInvariantCulture("ReportItemsType", xmlSchemaComplexType.Name, ignoreCase: false) != 0 && CompareWithInvariantCulture("MapLayersType", xmlSchemaComplexType.Name, ignoreCase: false) != 0)
					{
						Hashtable hashtable2 = new Hashtable(arrayList.Count);
						hashtable2.Add("_ParentName", m_reader.LocalName);
						hashtable2.Add("_Type", xmlSchemaComplexType);
						m_rdlElementStack.Add(hashtable2);
					}
					else
					{
						m_rdlElementStack.Add(null);
					}
				}
				else if (xmlSchemaComplexType != null)
				{
					for (int j = 0; j < arrayList.Count; j++)
					{
						XmlSchemaElement xmlSchemaElement2 = arrayList[j] as XmlSchemaElement;
						if (xmlSchemaElement2.MinOccurs > 0m)
						{
							result = false;
							message = RDLValidatingReaderStrings.rdlValidationMissingChildElement(m_reader.LocalName, xmlSchemaElement2.Name, LineNumber.ToString(CultureInfo.InvariantCulture.NumberFormat), LinePosition.ToString(CultureInfo.InvariantCulture.NumberFormat));
						}
					}
				}
				if (0 >= m_reader.Depth || m_rdlElementStack == null)
				{
					break;
				}
				Hashtable hashtable3 = m_rdlElementStack[m_reader.Depth - 1];
				if (hashtable3 == null)
				{
					break;
				}
				string text = (string)hashtable3[m_reader.LocalName];
				if (text == null)
				{
					hashtable3.Add(m_reader.LocalName, m_reader.NamespaceURI);
					break;
				}
				if (CompareWithInvariantCulture(text, m_reader.NamespaceURI, ignoreCase: false) == 0)
				{
					result = false;
					message = RDLValidatingReaderStrings.rdlValidationInvalidElement(hashtable3["_ParentName"] as string, m_reader.LocalName, LineNumber.ToString(CultureInfo.InvariantCulture.NumberFormat), LinePosition.ToString(CultureInfo.InvariantCulture.NumberFormat));
					break;
				}
				string key = m_reader.LocalName + "$" + m_reader.NamespaceURI;
				if (hashtable3.ContainsKey(key))
				{
					result = false;
					message = RDLValidatingReaderStrings.rdlValidationInvalidElement(hashtable3["_ParentName"] as string, m_reader.LocalName, LineNumber.ToString(CultureInfo.InvariantCulture.NumberFormat), LinePosition.ToString(CultureInfo.InvariantCulture.NumberFormat));
				}
				else
				{
					hashtable3.Add(key, m_reader.LocalName);
				}
				break;
			}
			case XmlNodeType.EndElement:
			{
				if (m_rdlElementStack == null)
				{
					break;
				}
				Hashtable hashtable = m_rdlElementStack[m_rdlElementStack.Count - 1];
				if (hashtable != null)
				{
					xmlSchemaComplexType = (hashtable["_Type"] as XmlSchemaComplexType);
					TraverseParticle(xmlSchemaComplexType.ContentTypeParticle, arrayList);
					for (int i = 0; i < arrayList.Count; i++)
					{
						XmlSchemaElement xmlSchemaElement = arrayList[i] as XmlSchemaElement;
						if (xmlSchemaElement.MinOccurs > 0m && !hashtable.ContainsKey(xmlSchemaElement.Name))
						{
							result = false;
							message = RDLValidatingReaderStrings.rdlValidationMissingChildElement(m_reader.LocalName, xmlSchemaElement.Name, LineNumber.ToString(CultureInfo.InvariantCulture.NumberFormat), LinePosition.ToString(CultureInfo.InvariantCulture.NumberFormat));
						}
					}
					m_rdlElementStack[m_rdlElementStack.Count - 1] = null;
				}
				m_rdlElementStack.RemoveAt(m_rdlElementStack.Count - 1);
				break;
			}
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
			else if (particle is XmlSchemaGroupBase)
			{
				foreach (XmlSchemaParticle item in (particle as XmlSchemaGroupBase).Items)
				{
					TraverseParticle(item, elementDeclsInContentModel);
				}
			}
			else if (particle is XmlSchemaAny)
			{
				m_processContent = (particle as XmlSchemaAny).ProcessContents;
			}
		}

		private void ValidationCallBack(object sender, ValidationEventArgs args)
		{
			if (this.ValidationEventHandler != null)
			{
				this.ValidationEventHandler(sender, args);
			}
		}

		public override void Close()
		{
			m_reader.Close();
		}

		public override string GetAttribute(int i)
		{
			return m_reader.GetAttribute(i);
		}

		public override string GetAttribute(string name, string namespaceURI)
		{
			return m_reader.GetAttribute(name, namespaceURI);
		}

		public override string GetAttribute(string name)
		{
			return m_reader.GetAttribute(name);
		}

		internal string GetAttributeLocalName(string name)
		{
			string result = null;
			if (m_reader.HasAttributes)
			{
				while (m_reader.MoveToNextAttribute())
				{
					if (m_reader.LocalName.Equals(name, StringComparison.Ordinal))
					{
						result = m_reader.Value;
						break;
					}
				}
				m_reader.MoveToElement();
			}
			return result;
		}

		public override string LookupNamespace(string prefix)
		{
			return m_reader.LookupNamespace(prefix);
		}

		public override bool MoveToAttribute(string name, string ns)
		{
			return m_reader.MoveToAttribute(name, ns);
		}

		public override bool MoveToAttribute(string name)
		{
			return m_reader.MoveToAttribute(name);
		}

		public override bool MoveToElement()
		{
			return m_reader.MoveToElement();
		}

		public override bool MoveToFirstAttribute()
		{
			return m_reader.MoveToFirstAttribute();
		}

		public override bool MoveToNextAttribute()
		{
			return m_reader.MoveToNextAttribute();
		}

		public override bool Read()
		{
			return m_reader.Read();
		}

		public override bool ReadAttributeValue()
		{
			return m_reader.ReadAttributeValue();
		}

		public override void ResolveEntity()
		{
			m_reader.ResolveEntity();
		}
	}
}
