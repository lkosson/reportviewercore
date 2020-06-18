using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Microsoft.ReportingServices.RdlObjectModel.Serialization
{
	internal sealed class NamespaceUpdater
	{
		private sealed class ExtensionNamespaceCounter
		{
			public ExtensionNamespace ExtensionNamespace
			{
				get;
				private set;
			}

			public int Count
			{
				get;
				set;
			}

			public ExtensionNamespaceCounter(ExtensionNamespace extensionNamespace)
			{
				ExtensionNamespace = extensionNamespace;
				Count = 0;
			}
		}

		public string[] Update(XmlWriter writer, string xml, ISerializerHost host = null)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(xml);
			string[] result = new NamespaceUpdater().Update(xmlDocument, host);
			xmlDocument.Save(writer);
			return result;
		}

		public string[] Update(XmlDocument document, ISerializerHost host = null)
		{
			if (document == null)
			{
				throw new ArgumentNullException("document");
			}
			Dictionary<string, ExtensionNamespaceCounter> dictionary = (host == null) ? new Dictionary<string, ExtensionNamespaceCounter>() : host.GetExtensionNamespaces().ToDictionary((ExtensionNamespace v) => v.Namespace, (ExtensionNamespace v) => new ExtensionNamespaceCounter(v));
			XmlElement documentElement = document.DocumentElement;
			if (documentElement == null)
			{
				throw new InvalidOperationException("Document contains no elements");
			}
			AddMustUnderstandNamespaces(dictionary, documentElement);
			AddXmlnsNamespaces(dictionary, documentElement);
			UpdateLocalNames(documentElement, dictionary);
			ExtensionNamespace[] array = (from v in dictionary
				where v.Value.Count > 0
				select v.Value.ExtensionNamespace).ToArray();
			UpdateRootNamespaces(array, documentElement);
			string[] array2 = (from v in array
				where v.MustUnderstand
				select v.LocalName).ToArray();
			string text = string.Join(" ", array2);
			if (text.Any())
			{
				documentElement.SetAttribute("MustUnderstand", text);
			}
			else
			{
				documentElement.RemoveAttribute("MustUnderstand");
			}
			return array2;
		}

		private void AddXmlnsNamespaces(Dictionary<string, ExtensionNamespaceCounter> xmlnsDictionary, XmlElement rootElem)
		{
			foreach (XmlAttribute item in from XmlAttribute a in rootElem.Attributes
				where a.Prefix == "xmlns" && !xmlnsDictionary.ContainsKey(a.Value)
				select a)
			{
				ExtensionNamespace extensionNamespace = new ExtensionNamespace(item.LocalName, item.Value);
				xmlnsDictionary.Add(item.Value, new ExtensionNamespaceCounter(extensionNamespace));
			}
		}

		private static void UpdateRootNamespaces(IEnumerable<ExtensionNamespace> namespaces, XmlElement rootElem)
		{
			int count = rootElem.Attributes.Count;
			while (count-- > 0)
			{
				if (rootElem.Attributes[count].Prefix == "xmlns")
				{
					rootElem.Attributes.RemoveAt(count);
				}
			}
			foreach (ExtensionNamespace @namespace in namespaces)
			{
				rootElem.SetAttribute($"xmlns:{@namespace.LocalName}", @namespace.Namespace);
			}
		}

		private void AddMustUnderstandNamespaces(Dictionary<string, ExtensionNamespaceCounter> xmlnsDictionary, XmlElement rootElem)
		{
			XmlNode namedItem = rootElem.Attributes.GetNamedItem("MustUnderstand");
			if (namedItem == null || string.IsNullOrEmpty(namedItem.Value))
			{
				return;
			}
			string[] array = namedItem.Value.Split();
			foreach (string text in array)
			{
				string namespaceOfPrefix = rootElem.GetNamespaceOfPrefix(text);
				if (!string.IsNullOrEmpty(namespaceOfPrefix) && !xmlnsDictionary.ContainsKey(namespaceOfPrefix))
				{
					xmlnsDictionary.Add(namespaceOfPrefix, new ExtensionNamespaceCounter(new ExtensionNamespace(text, namespaceOfPrefix, mustUnderstand: true)));
				}
			}
		}

		private void UpdateLocalNames(XmlElement xmlElement, Dictionary<string, ExtensionNamespaceCounter> xmlnsDictionary)
		{
			Stack<XmlNode> stack = new Stack<XmlNode>(new XmlElement[1]
			{
				xmlElement
			});
			while (stack.Count != 0)
			{
				XmlNode xmlNode = stack.Pop();
				if (xmlnsDictionary.TryGetValue(xmlNode.NamespaceURI, out ExtensionNamespaceCounter value))
				{
					xmlNode.Prefix = value.ExtensionNamespace.LocalName;
					(xmlNode as XmlElement)?.RemoveAttribute("xmlns");
					value.Count++;
				}
				IEnumerable<XmlNode> first;
				if (xmlNode.Attributes != null)
				{
					first = xmlNode.Attributes.Cast<XmlNode>();
				}
				else
				{
					IEnumerable<XmlNode> enumerable = new XmlNode[0];
					first = enumerable;
				}
				IEnumerable<XmlNode> second = xmlNode.ChildNodes.Cast<XmlNode>();
				foreach (XmlNode item in first.Concat(second))
				{
					stack.Push(item);
				}
			}
		}
	}
}
