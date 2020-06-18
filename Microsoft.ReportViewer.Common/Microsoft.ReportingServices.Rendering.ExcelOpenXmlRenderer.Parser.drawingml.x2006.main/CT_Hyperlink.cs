using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.drawingml.x2006.main
{
	internal class CT_Hyperlink : OoxmlComplexType
	{
		private string _invalidUrl_attr;

		private string _action_attr;

		private string _tgtFrame_attr;

		private string _tooltip_attr;

		private OoxmlBool _history_attr;

		private OoxmlBool _highlightClick_attr;

		private OoxmlBool _endSnd_attr;

		private string _id_attr;

		private bool _id_attr_is_specified;

		public string InvalidUrl_Attr
		{
			get
			{
				return _invalidUrl_attr;
			}
			set
			{
				_invalidUrl_attr = value;
			}
		}

		public string Action_Attr
		{
			get
			{
				return _action_attr;
			}
			set
			{
				_action_attr = value;
			}
		}

		public string TgtFrame_Attr
		{
			get
			{
				return _tgtFrame_attr;
			}
			set
			{
				_tgtFrame_attr = value;
			}
		}

		public string Tooltip_Attr
		{
			get
			{
				return _tooltip_attr;
			}
			set
			{
				_tooltip_attr = value;
			}
		}

		public OoxmlBool History_Attr
		{
			get
			{
				return _history_attr;
			}
			set
			{
				_history_attr = value;
			}
		}

		public OoxmlBool HighlightClick_Attr
		{
			get
			{
				return _highlightClick_attr;
			}
			set
			{
				_highlightClick_attr = value;
			}
		}

		public OoxmlBool EndSnd_Attr
		{
			get
			{
				return _endSnd_attr;
			}
			set
			{
				_endSnd_attr = value;
			}
		}

		public string Id_Attr
		{
			get
			{
				return _id_attr;
			}
			set
			{
				_id_attr = value;
				_id_attr_is_specified = (value != null);
			}
		}

		protected override void InitAttributes()
		{
			_invalidUrl_attr = Convert.ToString("", CultureInfo.InvariantCulture);
			_action_attr = Convert.ToString("", CultureInfo.InvariantCulture);
			_tgtFrame_attr = Convert.ToString("", CultureInfo.InvariantCulture);
			_tooltip_attr = Convert.ToString("", CultureInfo.InvariantCulture);
			_history_attr = OoxmlBool.OoxmlTrue;
			_highlightClick_attr = OoxmlBool.OoxmlFalse;
			_endSnd_attr = OoxmlBool.OoxmlFalse;
			_id_attr_is_specified = false;
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
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/drawingml/2006/main");
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
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/drawingml/2006/main");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
			if (_invalidUrl_attr != Convert.ToString("", CultureInfo.InvariantCulture))
			{
				s.Write(" invalidUrl=\"");
				OoxmlComplexType.WriteData(s, _invalidUrl_attr);
				s.Write("\"");
			}
			if (_action_attr != Convert.ToString("", CultureInfo.InvariantCulture))
			{
				s.Write(" action=\"");
				OoxmlComplexType.WriteData(s, _action_attr);
				s.Write("\"");
			}
			if (_tgtFrame_attr != Convert.ToString("", CultureInfo.InvariantCulture))
			{
				s.Write(" tgtFrame=\"");
				OoxmlComplexType.WriteData(s, _tgtFrame_attr);
				s.Write("\"");
			}
			if (_tooltip_attr != Convert.ToString("", CultureInfo.InvariantCulture))
			{
				s.Write(" tooltip=\"");
				OoxmlComplexType.WriteData(s, _tooltip_attr);
				s.Write("\"");
			}
			if ((bool)(_history_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" history=\"");
				OoxmlComplexType.WriteData(s, _history_attr);
				s.Write("\"");
			}
			if ((bool)(_highlightClick_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" highlightClick=\"");
				OoxmlComplexType.WriteData(s, _highlightClick_attr);
				s.Write("\"");
			}
			if ((bool)(_endSnd_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" endSnd=\"");
				OoxmlComplexType.WriteData(s, _endSnd_attr);
				s.Write("\"");
			}
			if (_id_attr_is_specified)
			{
				s.Write(" r:id=\"");
				OoxmlComplexType.WriteData(s, _id_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
		}
	}
}
