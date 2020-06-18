using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_SheetPr : OoxmlComplexType
	{
		private OoxmlBool _syncHorizontal_attr;

		private OoxmlBool _syncVertical_attr;

		private OoxmlBool _transitionEvaluation_attr;

		private OoxmlBool _transitionEntry_attr;

		private OoxmlBool _published_attr;

		private OoxmlBool _filterMode_attr;

		private OoxmlBool _enableFormatConditionsCalculation_attr;

		private string _syncRef_attr;

		private bool _syncRef_attr_is_specified;

		private string _codeName_attr;

		private bool _codeName_attr_is_specified;

		private CT_Color _tabColor;

		private CT_OutlinePr _outlinePr;

		public OoxmlBool SyncHorizontal_Attr
		{
			get
			{
				return _syncHorizontal_attr;
			}
			set
			{
				_syncHorizontal_attr = value;
			}
		}

		public OoxmlBool SyncVertical_Attr
		{
			get
			{
				return _syncVertical_attr;
			}
			set
			{
				_syncVertical_attr = value;
			}
		}

		public OoxmlBool TransitionEvaluation_Attr
		{
			get
			{
				return _transitionEvaluation_attr;
			}
			set
			{
				_transitionEvaluation_attr = value;
			}
		}

		public OoxmlBool TransitionEntry_Attr
		{
			get
			{
				return _transitionEntry_attr;
			}
			set
			{
				_transitionEntry_attr = value;
			}
		}

		public OoxmlBool Published_Attr
		{
			get
			{
				return _published_attr;
			}
			set
			{
				_published_attr = value;
			}
		}

		public OoxmlBool FilterMode_Attr
		{
			get
			{
				return _filterMode_attr;
			}
			set
			{
				_filterMode_attr = value;
			}
		}

		public OoxmlBool EnableFormatConditionsCalculation_Attr
		{
			get
			{
				return _enableFormatConditionsCalculation_attr;
			}
			set
			{
				_enableFormatConditionsCalculation_attr = value;
			}
		}

		public string SyncRef_Attr
		{
			get
			{
				return _syncRef_attr;
			}
			set
			{
				_syncRef_attr = value;
				_syncRef_attr_is_specified = (value != null);
			}
		}

		public string CodeName_Attr
		{
			get
			{
				return _codeName_attr;
			}
			set
			{
				_codeName_attr = value;
				_codeName_attr_is_specified = (value != null);
			}
		}

		public CT_Color TabColor
		{
			get
			{
				return _tabColor;
			}
			set
			{
				_tabColor = value;
			}
		}

		public CT_OutlinePr OutlinePr
		{
			get
			{
				return _outlinePr;
			}
			set
			{
				_outlinePr = value;
			}
		}

		public static string TabColorElementName => "tabColor";

		public static string OutlinePrElementName => "outlinePr";

		protected override void InitAttributes()
		{
			_syncHorizontal_attr = OoxmlBool.OoxmlFalse;
			_syncVertical_attr = OoxmlBool.OoxmlFalse;
			_transitionEvaluation_attr = OoxmlBool.OoxmlFalse;
			_transitionEntry_attr = OoxmlBool.OoxmlFalse;
			_published_attr = OoxmlBool.OoxmlTrue;
			_filterMode_attr = OoxmlBool.OoxmlFalse;
			_enableFormatConditionsCalculation_attr = OoxmlBool.OoxmlTrue;
			_syncRef_attr_is_specified = false;
			_codeName_attr_is_specified = false;
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
			if ((bool)(_syncHorizontal_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" syncHorizontal=\"");
				OoxmlComplexType.WriteData(s, _syncHorizontal_attr);
				s.Write("\"");
			}
			if ((bool)(_syncVertical_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" syncVertical=\"");
				OoxmlComplexType.WriteData(s, _syncVertical_attr);
				s.Write("\"");
			}
			if ((bool)(_transitionEvaluation_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" transitionEvaluation=\"");
				OoxmlComplexType.WriteData(s, _transitionEvaluation_attr);
				s.Write("\"");
			}
			if ((bool)(_transitionEntry_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" transitionEntry=\"");
				OoxmlComplexType.WriteData(s, _transitionEntry_attr);
				s.Write("\"");
			}
			if ((bool)(_published_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" published=\"");
				OoxmlComplexType.WriteData(s, _published_attr);
				s.Write("\"");
			}
			if ((bool)(_filterMode_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" filterMode=\"");
				OoxmlComplexType.WriteData(s, _filterMode_attr);
				s.Write("\"");
			}
			if ((bool)(_enableFormatConditionsCalculation_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" enableFormatConditionsCalculation=\"");
				OoxmlComplexType.WriteData(s, _enableFormatConditionsCalculation_attr);
				s.Write("\"");
			}
			if (_syncRef_attr_is_specified)
			{
				s.Write(" syncRef=\"");
				OoxmlComplexType.WriteData(s, _syncRef_attr);
				s.Write("\"");
			}
			if (_codeName_attr_is_specified)
			{
				s.Write(" codeName=\"");
				OoxmlComplexType.WriteData(s, _codeName_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			Write_tabColor(s, depth, namespaces);
			Write_outlinePr(s, depth, namespaces);
		}

		public void Write_tabColor(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_tabColor != null)
			{
				_tabColor.Write(s, "tabColor", depth + 1, namespaces);
			}
		}

		public void Write_outlinePr(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_outlinePr != null)
			{
				_outlinePr.Write(s, "outlinePr", depth + 1, namespaces);
			}
		}
	}
}
