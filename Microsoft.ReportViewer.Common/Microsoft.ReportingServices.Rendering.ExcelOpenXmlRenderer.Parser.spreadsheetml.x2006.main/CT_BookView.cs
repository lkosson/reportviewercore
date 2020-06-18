using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_BookView : OoxmlComplexType
	{
		private ST_Visibility _visibility_attr;

		private OoxmlBool _minimized_attr;

		private OoxmlBool _showHorizontalScroll_attr;

		private OoxmlBool _showVerticalScroll_attr;

		private OoxmlBool _showSheetTabs_attr;

		private uint _tabRatio_attr;

		private uint _firstSheet_attr;

		private uint _activeTab_attr;

		private OoxmlBool _autoFilterDateGrouping_attr;

		private int _xWindow_attr;

		private bool _xWindow_attr_is_specified;

		private int _yWindow_attr;

		private bool _yWindow_attr_is_specified;

		private uint _windowWidth_attr;

		private bool _windowWidth_attr_is_specified;

		private uint _windowHeight_attr;

		private bool _windowHeight_attr_is_specified;

		public ST_Visibility Visibility_Attr
		{
			get
			{
				return _visibility_attr;
			}
			set
			{
				_visibility_attr = value;
			}
		}

		public OoxmlBool Minimized_Attr
		{
			get
			{
				return _minimized_attr;
			}
			set
			{
				_minimized_attr = value;
			}
		}

		public OoxmlBool ShowHorizontalScroll_Attr
		{
			get
			{
				return _showHorizontalScroll_attr;
			}
			set
			{
				_showHorizontalScroll_attr = value;
			}
		}

		public OoxmlBool ShowVerticalScroll_Attr
		{
			get
			{
				return _showVerticalScroll_attr;
			}
			set
			{
				_showVerticalScroll_attr = value;
			}
		}

		public OoxmlBool ShowSheetTabs_Attr
		{
			get
			{
				return _showSheetTabs_attr;
			}
			set
			{
				_showSheetTabs_attr = value;
			}
		}

		public uint TabRatio_Attr
		{
			get
			{
				return _tabRatio_attr;
			}
			set
			{
				_tabRatio_attr = value;
			}
		}

		public uint FirstSheet_Attr
		{
			get
			{
				return _firstSheet_attr;
			}
			set
			{
				_firstSheet_attr = value;
			}
		}

		public uint ActiveTab_Attr
		{
			get
			{
				return _activeTab_attr;
			}
			set
			{
				_activeTab_attr = value;
			}
		}

		public OoxmlBool AutoFilterDateGrouping_Attr
		{
			get
			{
				return _autoFilterDateGrouping_attr;
			}
			set
			{
				_autoFilterDateGrouping_attr = value;
			}
		}

		public int XWindow_Attr
		{
			get
			{
				return _xWindow_attr;
			}
			set
			{
				_xWindow_attr = value;
				_xWindow_attr_is_specified = true;
			}
		}

		public bool XWindow_Attr_Is_Specified
		{
			get
			{
				return _xWindow_attr_is_specified;
			}
			set
			{
				_xWindow_attr_is_specified = value;
			}
		}

		public int YWindow_Attr
		{
			get
			{
				return _yWindow_attr;
			}
			set
			{
				_yWindow_attr = value;
				_yWindow_attr_is_specified = true;
			}
		}

		public bool YWindow_Attr_Is_Specified
		{
			get
			{
				return _yWindow_attr_is_specified;
			}
			set
			{
				_yWindow_attr_is_specified = value;
			}
		}

		public uint WindowWidth_Attr
		{
			get
			{
				return _windowWidth_attr;
			}
			set
			{
				_windowWidth_attr = value;
				_windowWidth_attr_is_specified = true;
			}
		}

		public bool WindowWidth_Attr_Is_Specified
		{
			get
			{
				return _windowWidth_attr_is_specified;
			}
			set
			{
				_windowWidth_attr_is_specified = value;
			}
		}

		public uint WindowHeight_Attr
		{
			get
			{
				return _windowHeight_attr;
			}
			set
			{
				_windowHeight_attr = value;
				_windowHeight_attr_is_specified = true;
			}
		}

		public bool WindowHeight_Attr_Is_Specified
		{
			get
			{
				return _windowHeight_attr_is_specified;
			}
			set
			{
				_windowHeight_attr_is_specified = value;
			}
		}

		protected override void InitAttributes()
		{
			_visibility_attr = ST_Visibility.visible;
			_minimized_attr = OoxmlBool.OoxmlFalse;
			_showHorizontalScroll_attr = OoxmlBool.OoxmlTrue;
			_showVerticalScroll_attr = OoxmlBool.OoxmlTrue;
			_showSheetTabs_attr = OoxmlBool.OoxmlTrue;
			_tabRatio_attr = Convert.ToUInt32("600", CultureInfo.InvariantCulture);
			_firstSheet_attr = Convert.ToUInt32("0", CultureInfo.InvariantCulture);
			_activeTab_attr = Convert.ToUInt32("0", CultureInfo.InvariantCulture);
			_autoFilterDateGrouping_attr = OoxmlBool.OoxmlTrue;
			_xWindow_attr_is_specified = false;
			_yWindow_attr_is_specified = false;
			_windowWidth_attr_is_specified = false;
			_windowHeight_attr_is_specified = false;
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
			if (_visibility_attr != ST_Visibility.visible)
			{
				s.Write(" visibility=\"");
				OoxmlComplexType.WriteData(s, _visibility_attr);
				s.Write("\"");
			}
			if ((bool)(_minimized_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" minimized=\"");
				OoxmlComplexType.WriteData(s, _minimized_attr);
				s.Write("\"");
			}
			if ((bool)(_showHorizontalScroll_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" showHorizontalScroll=\"");
				OoxmlComplexType.WriteData(s, _showHorizontalScroll_attr);
				s.Write("\"");
			}
			if ((bool)(_showVerticalScroll_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" showVerticalScroll=\"");
				OoxmlComplexType.WriteData(s, _showVerticalScroll_attr);
				s.Write("\"");
			}
			if ((bool)(_showSheetTabs_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" showSheetTabs=\"");
				OoxmlComplexType.WriteData(s, _showSheetTabs_attr);
				s.Write("\"");
			}
			if (_tabRatio_attr != Convert.ToUInt32("600", CultureInfo.InvariantCulture))
			{
				s.Write(" tabRatio=\"");
				OoxmlComplexType.WriteData(s, _tabRatio_attr);
				s.Write("\"");
			}
			if (_firstSheet_attr != Convert.ToUInt32("0", CultureInfo.InvariantCulture))
			{
				s.Write(" firstSheet=\"");
				OoxmlComplexType.WriteData(s, _firstSheet_attr);
				s.Write("\"");
			}
			if (_activeTab_attr != Convert.ToUInt32("0", CultureInfo.InvariantCulture))
			{
				s.Write(" activeTab=\"");
				OoxmlComplexType.WriteData(s, _activeTab_attr);
				s.Write("\"");
			}
			if ((bool)(_autoFilterDateGrouping_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" autoFilterDateGrouping=\"");
				OoxmlComplexType.WriteData(s, _autoFilterDateGrouping_attr);
				s.Write("\"");
			}
			if (_xWindow_attr_is_specified)
			{
				s.Write(" xWindow=\"");
				OoxmlComplexType.WriteData(s, _xWindow_attr);
				s.Write("\"");
			}
			if (_yWindow_attr_is_specified)
			{
				s.Write(" yWindow=\"");
				OoxmlComplexType.WriteData(s, _yWindow_attr);
				s.Write("\"");
			}
			if (_windowWidth_attr_is_specified)
			{
				s.Write(" windowWidth=\"");
				OoxmlComplexType.WriteData(s, _windowWidth_attr);
				s.Write("\"");
			}
			if (_windowHeight_attr_is_specified)
			{
				s.Write(" windowHeight=\"");
				OoxmlComplexType.WriteData(s, _windowHeight_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
		}
	}
}
