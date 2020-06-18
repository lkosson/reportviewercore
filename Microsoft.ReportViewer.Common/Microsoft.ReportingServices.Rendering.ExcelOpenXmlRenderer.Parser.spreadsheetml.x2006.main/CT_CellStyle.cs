using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_CellStyle : OoxmlComplexType
	{
		private uint _xfId_attr;

		private string _name_attr;

		private bool _name_attr_is_specified;

		private uint _builtinId_attr;

		private bool _builtinId_attr_is_specified;

		private uint _iLevel_attr;

		private bool _iLevel_attr_is_specified;

		private OoxmlBool _hidden_attr;

		private bool _hidden_attr_is_specified;

		private OoxmlBool _customBuiltin_attr;

		private bool _customBuiltin_attr_is_specified;

		public uint XfId_Attr
		{
			get
			{
				return _xfId_attr;
			}
			set
			{
				_xfId_attr = value;
			}
		}

		public uint BuiltinId_Attr
		{
			get
			{
				return _builtinId_attr;
			}
			set
			{
				_builtinId_attr = value;
				_builtinId_attr_is_specified = true;
			}
		}

		public bool BuiltinId_Attr_Is_Specified
		{
			get
			{
				return _builtinId_attr_is_specified;
			}
			set
			{
				_builtinId_attr_is_specified = value;
			}
		}

		public uint ILevel_Attr
		{
			get
			{
				return _iLevel_attr;
			}
			set
			{
				_iLevel_attr = value;
				_iLevel_attr_is_specified = true;
			}
		}

		public bool ILevel_Attr_Is_Specified
		{
			get
			{
				return _iLevel_attr_is_specified;
			}
			set
			{
				_iLevel_attr_is_specified = value;
			}
		}

		public OoxmlBool Hidden_Attr
		{
			get
			{
				return _hidden_attr;
			}
			set
			{
				_hidden_attr = value;
				_hidden_attr_is_specified = true;
			}
		}

		public bool Hidden_Attr_Is_Specified
		{
			get
			{
				return _hidden_attr_is_specified;
			}
			set
			{
				_hidden_attr_is_specified = value;
			}
		}

		public OoxmlBool CustomBuiltin_Attr
		{
			get
			{
				return _customBuiltin_attr;
			}
			set
			{
				_customBuiltin_attr = value;
				_customBuiltin_attr_is_specified = true;
			}
		}

		public bool CustomBuiltin_Attr_Is_Specified
		{
			get
			{
				return _customBuiltin_attr_is_specified;
			}
			set
			{
				_customBuiltin_attr_is_specified = value;
			}
		}

		public string Name_Attr
		{
			get
			{
				return _name_attr;
			}
			set
			{
				_name_attr = value;
				_name_attr_is_specified = (value != null);
			}
		}

		protected override void InitAttributes()
		{
			_name_attr_is_specified = false;
			_builtinId_attr_is_specified = false;
			_iLevel_attr_is_specified = false;
			_hidden_attr_is_specified = false;
			_customBuiltin_attr_is_specified = false;
		}

		protected override void InitElements()
		{
		}

		protected override void InitCollections()
		{
		}

		public override void WriteAsRoot(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces)
		{
			WriteOpenTag(s, tagName, depth, namespaces, root: true);
			WriteElements(s, depth, namespaces);
			WriteCloseTag(s, tagName, depth, namespaces);
		}

		public override void Write(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces)
		{
			WriteOpenTag(s, tagName, depth, namespaces, root: false);
			WriteElements(s, depth, namespaces);
			WriteCloseTag(s, tagName, depth, namespaces);
		}

		public override void WriteOpenTag(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces, bool root)
		{
			s.Write("<");
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
			s.Write(tagName);
			WriteAttributes(s);
			if (root)
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

		public override void WriteCloseTag(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces)
		{
			s.Write("</");
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
			s.Write(" xfId=\"");
			OoxmlComplexType.WriteData(s, _xfId_attr);
			s.Write("\"");
			if (_name_attr_is_specified)
			{
				s.Write(" name=\"");
				OoxmlComplexType.WriteData(s, _name_attr);
				s.Write("\"");
			}
			if (_builtinId_attr_is_specified)
			{
				s.Write(" builtinId=\"");
				OoxmlComplexType.WriteData(s, _builtinId_attr);
				s.Write("\"");
			}
			if (_iLevel_attr_is_specified)
			{
				s.Write(" iLevel=\"");
				OoxmlComplexType.WriteData(s, _iLevel_attr);
				s.Write("\"");
			}
			if (_hidden_attr_is_specified)
			{
				s.Write(" hidden=\"");
				OoxmlComplexType.WriteData(s, _hidden_attr);
				s.Write("\"");
			}
			if (_customBuiltin_attr_is_specified)
			{
				s.Write(" customBuiltin=\"");
				OoxmlComplexType.WriteData(s, _customBuiltin_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
		}
	}
}
