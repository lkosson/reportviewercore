using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser
{
	internal abstract class OoxmlComplexType : IOoxmlComplexType
	{
		public virtual GeneratedType GroupInterfaceType => GeneratedType.NoType;

		protected OoxmlComplexType()
		{
			InitAttributes();
			InitElements();
			InitCollections();
		}

		public abstract void WriteOpenTag(TextWriter s, string tagName, Dictionary<string, string> namespaces);

		public abstract void WriteCloseTag(TextWriter s, string tagName);

		public abstract void WriteAttributes(TextWriter s);

		public abstract void WriteElements(TextWriter s);

		public abstract void Write(TextWriter s, string tagName);

		protected abstract void InitAttributes();

		protected abstract void InitElements();

		protected abstract void InitCollections();

		protected static void WriteData(TextWriter s, object data)
		{
			if (data != null)
			{
				if (data is DateTime)
				{
					s.Write(((DateTime)data).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture));
				}
				else if (data is bool)
				{
					s.Write(((bool)data) ? 1 : 0);
				}
				else if (data is float)
				{
					s.Write(((float)data).ToString(CultureInfo.InvariantCulture));
				}
				else if (data is double)
				{
					s.Write(((double)data).ToString(CultureInfo.InvariantCulture));
				}
				else if (data is decimal)
				{
					s.Write(((decimal)data).ToString(CultureInfo.InvariantCulture));
				}
				else
				{
					s.Write(data.ToString());
				}
			}
		}

		protected static void WriteRawTag(TextWriter s, string tagname, string tagNamespacePrefix, object data)
		{
			WriteRawTag(s, tagname, preserveWhitespace: false, tagNamespacePrefix, data);
		}

		protected static void WriteRawTag(TextWriter s, string tagname, bool preserveWhitespace, string tagNamespacePrefix, object data)
		{
			s.Write("<");
			s.Write(tagNamespacePrefix);
			s.Write(":");
			s.Write(tagname);
			if (preserveWhitespace)
			{
				s.Write(" xml:space=\"preserve\"");
			}
			s.Write(">");
			WriteData(s, data);
			s.Write("</");
			s.Write(tagNamespacePrefix);
			s.Write(":");
			s.Write(tagname);
			s.Write(">");
		}

		public void WriteAsRoot(TextWriter s, string tagName, Dictionary<string, string> namespaces)
		{
			WriteOpenTag(s, tagName, namespaces);
			WriteElements(s);
			WriteCloseTag(s, tagName);
		}

		protected void WriteEmptyTag(TextWriter s, string tagName, string nsPrefix)
		{
			s.Write("<");
			s.Write(nsPrefix);
			s.Write(":");
			s.Write(tagName);
			WriteAttributes(s);
			s.Write("/>");
		}

		public void WriteOpenTag(TextWriter s, string tagName, string nsPrefix, Dictionary<string, string> namespaces)
		{
			s.Write("<");
			s.Write(nsPrefix);
			s.Write(":");
			s.Write(tagName);
			WriteAttributes(s);
			if (namespaces != null)
			{
				foreach (string key in namespaces.Keys)
				{
					s.Write(" xmlns");
					if (namespaces[key] != "")
					{
						s.Write(":");
						s.Write(namespaces[key]);
					}
					s.Write("=\"");
					s.Write(key);
					s.Write("\"");
				}
			}
			s.Write(">");
		}
	}
}
