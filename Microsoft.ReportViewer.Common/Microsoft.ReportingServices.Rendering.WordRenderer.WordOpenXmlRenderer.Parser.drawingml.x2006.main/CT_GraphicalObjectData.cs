using Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.drawingml.x2006.picture;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.drawingml.x2006.main
{
	internal class CT_GraphicalObjectData : OoxmlComplexType, IOoxmlComplexType
	{
		private string _uri_attr;

		private CT_Picture _pic;

		private List<XmlElement> _any;

		public string Uri_Attr
		{
			get
			{
				return _uri_attr;
			}
			set
			{
				_uri_attr = value;
			}
		}

		public CT_Picture Pic
		{
			get
			{
				return _pic;
			}
			set
			{
				_pic = value;
			}
		}

		public List<XmlElement> Any
		{
			get
			{
				return _any;
			}
			set
			{
				_any = value;
			}
		}

		public static string PicElementName => "pic";

		protected override void InitAttributes()
		{
		}

		protected override void InitElements()
		{
			_pic = new CT_Picture();
		}

		protected override void InitCollections()
		{
			_any = new List<XmlElement>();
		}

		public override void Write(TextWriter s, string tagName)
		{
			WriteOpenTag(s, tagName, null);
			WriteElements(s);
			WriteCloseTag(s, tagName);
		}

		public override void WriteOpenTag(TextWriter s, string tagName, Dictionary<string, string> namespaces)
		{
			WriteOpenTag(s, tagName, "a", namespaces);
		}

		public override void WriteCloseTag(TextWriter s, string tagName)
		{
			s.Write("</a:");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
			s.Write(" uri=\"");
			OoxmlComplexType.WriteData(s, _uri_attr);
			s.Write("\"");
		}

		public override void WriteElements(TextWriter s)
		{
			Write_pic(s);
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.OmitXmlDeclaration = true;
			xmlWriterSettings.ConformanceLevel = ConformanceLevel.Fragment;
			XmlWriter xmlWriter = XmlWriter.Create(s, xmlWriterSettings);
			foreach (XmlElement item in _any)
			{
				item.WriteTo(xmlWriter);
			}
			xmlWriter.Flush();
		}

		public void Write_pic(TextWriter s)
		{
			if (_pic != null)
			{
				_pic.Write(s, "pic");
			}
		}
	}
}
