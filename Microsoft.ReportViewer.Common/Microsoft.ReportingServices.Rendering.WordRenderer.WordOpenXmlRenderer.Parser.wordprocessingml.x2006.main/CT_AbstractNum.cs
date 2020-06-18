using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_AbstractNum : OoxmlComplexType, IOoxmlComplexType
	{
		private int _abstractNumId_attr;

		private CT_LongHexNumber _nsid;

		private CT_MultiLevelType _multiLevelType;

		private CT_LongHexNumber _tmpl;

		private CT_String _name;

		private CT_String _styleLink;

		private CT_String _numStyleLink;

		private List<CT_Lvl> _lvl;

		public int AbstractNumId_Attr
		{
			get
			{
				return _abstractNumId_attr;
			}
			set
			{
				_abstractNumId_attr = value;
			}
		}

		public CT_LongHexNumber Nsid
		{
			get
			{
				return _nsid;
			}
			set
			{
				_nsid = value;
			}
		}

		public CT_MultiLevelType MultiLevelType
		{
			get
			{
				return _multiLevelType;
			}
			set
			{
				_multiLevelType = value;
			}
		}

		public CT_LongHexNumber Tmpl
		{
			get
			{
				return _tmpl;
			}
			set
			{
				_tmpl = value;
			}
		}

		public CT_String Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		public CT_String StyleLink
		{
			get
			{
				return _styleLink;
			}
			set
			{
				_styleLink = value;
			}
		}

		public CT_String NumStyleLink
		{
			get
			{
				return _numStyleLink;
			}
			set
			{
				_numStyleLink = value;
			}
		}

		public List<CT_Lvl> Lvl
		{
			get
			{
				return _lvl;
			}
			set
			{
				_lvl = value;
			}
		}

		public static string NsidElementName => "nsid";

		public static string MultiLevelTypeElementName => "multiLevelType";

		public static string TmplElementName => "tmpl";

		public static string NameElementName => "name";

		public static string StyleLinkElementName => "styleLink";

		public static string NumStyleLinkElementName => "numStyleLink";

		public static string LvlElementName => "lvl";

		protected override void InitAttributes()
		{
		}

		protected override void InitElements()
		{
		}

		protected override void InitCollections()
		{
			_lvl = new List<CT_Lvl>();
		}

		public override void Write(TextWriter s, string tagName)
		{
			WriteOpenTag(s, tagName, null);
			WriteElements(s);
			WriteCloseTag(s, tagName);
		}

		public override void WriteOpenTag(TextWriter s, string tagName, Dictionary<string, string> namespaces)
		{
			WriteOpenTag(s, tagName, "w", namespaces);
		}

		public override void WriteCloseTag(TextWriter s, string tagName)
		{
			s.Write("</w:");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
			s.Write(" w:abstractNumId=\"");
			OoxmlComplexType.WriteData(s, _abstractNumId_attr);
			s.Write("\"");
		}

		public override void WriteElements(TextWriter s)
		{
			Write_nsid(s);
			Write_multiLevelType(s);
			Write_tmpl(s);
			Write_name(s);
			Write_styleLink(s);
			Write_numStyleLink(s);
			Write_lvl(s);
		}

		public void Write_nsid(TextWriter s)
		{
			if (_nsid != null)
			{
				_nsid.Write(s, "nsid");
			}
		}

		public void Write_multiLevelType(TextWriter s)
		{
			if (_multiLevelType != null)
			{
				_multiLevelType.Write(s, "multiLevelType");
			}
		}

		public void Write_tmpl(TextWriter s)
		{
			if (_tmpl != null)
			{
				_tmpl.Write(s, "tmpl");
			}
		}

		public void Write_name(TextWriter s)
		{
			if (_name != null)
			{
				_name.Write(s, "name");
			}
		}

		public void Write_styleLink(TextWriter s)
		{
			if (_styleLink != null)
			{
				_styleLink.Write(s, "styleLink");
			}
		}

		public void Write_numStyleLink(TextWriter s)
		{
			if (_numStyleLink != null)
			{
				_numStyleLink.Write(s, "numStyleLink");
			}
		}

		public void Write_lvl(TextWriter s)
		{
			if (_lvl == null)
			{
				return;
			}
			foreach (CT_Lvl item in _lvl)
			{
				item?.Write(s, "lvl");
			}
		}
	}
}
