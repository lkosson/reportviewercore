using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_DefinedName : OoxmlComplexType
	{
		private string _name_attr;

		private OoxmlBool _hidden_attr;

		private OoxmlBool _function_attr;

		private OoxmlBool _vbProcedure_attr;

		private OoxmlBool _xlm_attr;

		private OoxmlBool _publishToServer_attr;

		private OoxmlBool _workbookParameter_attr;

		private string _comment_attr;

		private bool _comment_attr_is_specified;

		private string _customMenu_attr;

		private bool _customMenu_attr_is_specified;

		private string _description_attr;

		private bool _description_attr_is_specified;

		private string _help_attr;

		private bool _help_attr_is_specified;

		private string _statusBar_attr;

		private bool _statusBar_attr_is_specified;

		private uint _localSheetId_attr;

		private bool _localSheetId_attr_is_specified;

		private uint _functionGroupId_attr;

		private bool _functionGroupId_attr_is_specified;

		private string _shortcutKey_attr;

		private bool _shortcutKey_attr_is_specified;

		private string _content;

		public string Name_Attr
		{
			get
			{
				return _name_attr;
			}
			set
			{
				_name_attr = value;
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
			}
		}

		public OoxmlBool Function_Attr
		{
			get
			{
				return _function_attr;
			}
			set
			{
				_function_attr = value;
			}
		}

		public OoxmlBool VbProcedure_Attr
		{
			get
			{
				return _vbProcedure_attr;
			}
			set
			{
				_vbProcedure_attr = value;
			}
		}

		public OoxmlBool Xlm_Attr
		{
			get
			{
				return _xlm_attr;
			}
			set
			{
				_xlm_attr = value;
			}
		}

		public OoxmlBool PublishToServer_Attr
		{
			get
			{
				return _publishToServer_attr;
			}
			set
			{
				_publishToServer_attr = value;
			}
		}

		public OoxmlBool WorkbookParameter_Attr
		{
			get
			{
				return _workbookParameter_attr;
			}
			set
			{
				_workbookParameter_attr = value;
			}
		}

		public uint LocalSheetId_Attr
		{
			get
			{
				return _localSheetId_attr;
			}
			set
			{
				_localSheetId_attr = value;
				_localSheetId_attr_is_specified = true;
			}
		}

		public bool LocalSheetId_Attr_Is_Specified
		{
			get
			{
				return _localSheetId_attr_is_specified;
			}
			set
			{
				_localSheetId_attr_is_specified = value;
			}
		}

		public uint FunctionGroupId_Attr
		{
			get
			{
				return _functionGroupId_attr;
			}
			set
			{
				_functionGroupId_attr = value;
				_functionGroupId_attr_is_specified = true;
			}
		}

		public bool FunctionGroupId_Attr_Is_Specified
		{
			get
			{
				return _functionGroupId_attr_is_specified;
			}
			set
			{
				_functionGroupId_attr_is_specified = value;
			}
		}

		public string Comment_Attr
		{
			get
			{
				return _comment_attr;
			}
			set
			{
				_comment_attr = value;
				_comment_attr_is_specified = (value != null);
			}
		}

		public string CustomMenu_Attr
		{
			get
			{
				return _customMenu_attr;
			}
			set
			{
				_customMenu_attr = value;
				_customMenu_attr_is_specified = (value != null);
			}
		}

		public string Description_Attr
		{
			get
			{
				return _description_attr;
			}
			set
			{
				_description_attr = value;
				_description_attr_is_specified = (value != null);
			}
		}

		public string Help_Attr
		{
			get
			{
				return _help_attr;
			}
			set
			{
				_help_attr = value;
				_help_attr_is_specified = (value != null);
			}
		}

		public string StatusBar_Attr
		{
			get
			{
				return _statusBar_attr;
			}
			set
			{
				_statusBar_attr = value;
				_statusBar_attr_is_specified = (value != null);
			}
		}

		public string ShortcutKey_Attr
		{
			get
			{
				return _shortcutKey_attr;
			}
			set
			{
				_shortcutKey_attr = value;
				_shortcutKey_attr_is_specified = (value != null);
			}
		}

		public string Content
		{
			get
			{
				return _content;
			}
			set
			{
				_content = value;
			}
		}

		protected override void InitAttributes()
		{
			_hidden_attr = OoxmlBool.OoxmlFalse;
			_function_attr = OoxmlBool.OoxmlFalse;
			_vbProcedure_attr = OoxmlBool.OoxmlFalse;
			_xlm_attr = OoxmlBool.OoxmlFalse;
			_publishToServer_attr = OoxmlBool.OoxmlFalse;
			_workbookParameter_attr = OoxmlBool.OoxmlFalse;
			_comment_attr_is_specified = false;
			_customMenu_attr_is_specified = false;
			_description_attr_is_specified = false;
			_help_attr_is_specified = false;
			_statusBar_attr_is_specified = false;
			_localSheetId_attr_is_specified = false;
			_functionGroupId_attr_is_specified = false;
			_shortcutKey_attr_is_specified = false;
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
			s.Write(" name=\"");
			OoxmlComplexType.WriteData(s, _name_attr);
			s.Write("\"");
			if ((bool)(_hidden_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" hidden=\"");
				OoxmlComplexType.WriteData(s, _hidden_attr);
				s.Write("\"");
			}
			if ((bool)(_function_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" function=\"");
				OoxmlComplexType.WriteData(s, _function_attr);
				s.Write("\"");
			}
			if ((bool)(_vbProcedure_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" vbProcedure=\"");
				OoxmlComplexType.WriteData(s, _vbProcedure_attr);
				s.Write("\"");
			}
			if ((bool)(_xlm_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" xlm=\"");
				OoxmlComplexType.WriteData(s, _xlm_attr);
				s.Write("\"");
			}
			if ((bool)(_publishToServer_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" publishToServer=\"");
				OoxmlComplexType.WriteData(s, _publishToServer_attr);
				s.Write("\"");
			}
			if ((bool)(_workbookParameter_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" workbookParameter=\"");
				OoxmlComplexType.WriteData(s, _workbookParameter_attr);
				s.Write("\"");
			}
			if (_comment_attr_is_specified)
			{
				s.Write(" comment=\"");
				OoxmlComplexType.WriteData(s, _comment_attr);
				s.Write("\"");
			}
			if (_customMenu_attr_is_specified)
			{
				s.Write(" customMenu=\"");
				OoxmlComplexType.WriteData(s, _customMenu_attr);
				s.Write("\"");
			}
			if (_description_attr_is_specified)
			{
				s.Write(" description=\"");
				OoxmlComplexType.WriteData(s, _description_attr);
				s.Write("\"");
			}
			if (_help_attr_is_specified)
			{
				s.Write(" help=\"");
				OoxmlComplexType.WriteData(s, _help_attr);
				s.Write("\"");
			}
			if (_statusBar_attr_is_specified)
			{
				s.Write(" statusBar=\"");
				OoxmlComplexType.WriteData(s, _statusBar_attr);
				s.Write("\"");
			}
			if (_localSheetId_attr_is_specified)
			{
				s.Write(" localSheetId=\"");
				OoxmlComplexType.WriteData(s, _localSheetId_attr);
				s.Write("\"");
			}
			if (_functionGroupId_attr_is_specified)
			{
				s.Write(" functionGroupId=\"");
				OoxmlComplexType.WriteData(s, _functionGroupId_attr);
				s.Write("\"");
			}
			if (_shortcutKey_attr_is_specified)
			{
				s.Write(" shortcutKey=\"");
				OoxmlComplexType.WriteData(s, _shortcutKey_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			OoxmlComplexType.WriteData(s, _content);
		}
	}
}
