using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_SheetView : OoxmlComplexType
	{
		private OoxmlBool _windowProtection_attr;

		private OoxmlBool _showFormulas_attr;

		private OoxmlBool _showGridLines_attr;

		private OoxmlBool _showRowColHeaders_attr;

		private OoxmlBool _showZeros_attr;

		private OoxmlBool _rightToLeft_attr;

		private OoxmlBool _tabSelected_attr;

		private OoxmlBool _showRuler_attr;

		private OoxmlBool _showOutlineSymbols_attr;

		private OoxmlBool _defaultGridColor_attr;

		private OoxmlBool _showWhiteSpace_attr;

		private ST_SheetViewType _view_attr;

		private uint _colorId_attr;

		private uint _zoomScale_attr;

		private uint _zoomScaleNormal_attr;

		private uint _zoomScaleSheetLayoutView_attr;

		private uint _zoomScalePageLayoutView_attr;

		private uint _workbookViewId_attr;

		private string _topLeftCell_attr;

		private bool _topLeftCell_attr_is_specified;

		private CT_Pane _pane;

		public OoxmlBool WindowProtection_Attr
		{
			get
			{
				return _windowProtection_attr;
			}
			set
			{
				_windowProtection_attr = value;
			}
		}

		public OoxmlBool ShowFormulas_Attr
		{
			get
			{
				return _showFormulas_attr;
			}
			set
			{
				_showFormulas_attr = value;
			}
		}

		public OoxmlBool ShowGridLines_Attr
		{
			get
			{
				return _showGridLines_attr;
			}
			set
			{
				_showGridLines_attr = value;
			}
		}

		public OoxmlBool ShowRowColHeaders_Attr
		{
			get
			{
				return _showRowColHeaders_attr;
			}
			set
			{
				_showRowColHeaders_attr = value;
			}
		}

		public OoxmlBool ShowZeros_Attr
		{
			get
			{
				return _showZeros_attr;
			}
			set
			{
				_showZeros_attr = value;
			}
		}

		public OoxmlBool RightToLeft_Attr
		{
			get
			{
				return _rightToLeft_attr;
			}
			set
			{
				_rightToLeft_attr = value;
			}
		}

		public OoxmlBool TabSelected_Attr
		{
			get
			{
				return _tabSelected_attr;
			}
			set
			{
				_tabSelected_attr = value;
			}
		}

		public OoxmlBool ShowRuler_Attr
		{
			get
			{
				return _showRuler_attr;
			}
			set
			{
				_showRuler_attr = value;
			}
		}

		public OoxmlBool ShowOutlineSymbols_Attr
		{
			get
			{
				return _showOutlineSymbols_attr;
			}
			set
			{
				_showOutlineSymbols_attr = value;
			}
		}

		public OoxmlBool DefaultGridColor_Attr
		{
			get
			{
				return _defaultGridColor_attr;
			}
			set
			{
				_defaultGridColor_attr = value;
			}
		}

		public OoxmlBool ShowWhiteSpace_Attr
		{
			get
			{
				return _showWhiteSpace_attr;
			}
			set
			{
				_showWhiteSpace_attr = value;
			}
		}

		public ST_SheetViewType View_Attr
		{
			get
			{
				return _view_attr;
			}
			set
			{
				_view_attr = value;
			}
		}

		public uint ColorId_Attr
		{
			get
			{
				return _colorId_attr;
			}
			set
			{
				_colorId_attr = value;
			}
		}

		public uint ZoomScale_Attr
		{
			get
			{
				return _zoomScale_attr;
			}
			set
			{
				_zoomScale_attr = value;
			}
		}

		public uint ZoomScaleNormal_Attr
		{
			get
			{
				return _zoomScaleNormal_attr;
			}
			set
			{
				_zoomScaleNormal_attr = value;
			}
		}

		public uint ZoomScaleSheetLayoutView_Attr
		{
			get
			{
				return _zoomScaleSheetLayoutView_attr;
			}
			set
			{
				_zoomScaleSheetLayoutView_attr = value;
			}
		}

		public uint ZoomScalePageLayoutView_Attr
		{
			get
			{
				return _zoomScalePageLayoutView_attr;
			}
			set
			{
				_zoomScalePageLayoutView_attr = value;
			}
		}

		public uint WorkbookViewId_Attr
		{
			get
			{
				return _workbookViewId_attr;
			}
			set
			{
				_workbookViewId_attr = value;
			}
		}

		public string TopLeftCell_Attr
		{
			get
			{
				return _topLeftCell_attr;
			}
			set
			{
				_topLeftCell_attr = value;
				_topLeftCell_attr_is_specified = (value != null);
			}
		}

		public CT_Pane Pane
		{
			get
			{
				return _pane;
			}
			set
			{
				_pane = value;
			}
		}

		public static string PaneElementName => "pane";

		protected override void InitAttributes()
		{
			_windowProtection_attr = OoxmlBool.OoxmlFalse;
			_showFormulas_attr = OoxmlBool.OoxmlFalse;
			_showGridLines_attr = OoxmlBool.OoxmlTrue;
			_showRowColHeaders_attr = OoxmlBool.OoxmlTrue;
			_showZeros_attr = OoxmlBool.OoxmlTrue;
			_rightToLeft_attr = OoxmlBool.OoxmlFalse;
			_tabSelected_attr = OoxmlBool.OoxmlFalse;
			_showRuler_attr = OoxmlBool.OoxmlTrue;
			_showOutlineSymbols_attr = OoxmlBool.OoxmlTrue;
			_defaultGridColor_attr = OoxmlBool.OoxmlTrue;
			_showWhiteSpace_attr = OoxmlBool.OoxmlTrue;
			_view_attr = ST_SheetViewType.normal;
			_colorId_attr = Convert.ToUInt32("64", CultureInfo.InvariantCulture);
			_zoomScale_attr = Convert.ToUInt32("100", CultureInfo.InvariantCulture);
			_zoomScaleNormal_attr = Convert.ToUInt32("0", CultureInfo.InvariantCulture);
			_zoomScaleSheetLayoutView_attr = Convert.ToUInt32("0", CultureInfo.InvariantCulture);
			_zoomScalePageLayoutView_attr = Convert.ToUInt32("0", CultureInfo.InvariantCulture);
			_topLeftCell_attr_is_specified = false;
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
			s.Write(" workbookViewId=\"");
			OoxmlComplexType.WriteData(s, _workbookViewId_attr);
			s.Write("\"");
			if ((bool)(_windowProtection_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" windowProtection=\"");
				OoxmlComplexType.WriteData(s, _windowProtection_attr);
				s.Write("\"");
			}
			if ((bool)(_showFormulas_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" showFormulas=\"");
				OoxmlComplexType.WriteData(s, _showFormulas_attr);
				s.Write("\"");
			}
			if ((bool)(_showGridLines_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" showGridLines=\"");
				OoxmlComplexType.WriteData(s, _showGridLines_attr);
				s.Write("\"");
			}
			if ((bool)(_showRowColHeaders_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" showRowColHeaders=\"");
				OoxmlComplexType.WriteData(s, _showRowColHeaders_attr);
				s.Write("\"");
			}
			if ((bool)(_showZeros_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" showZeros=\"");
				OoxmlComplexType.WriteData(s, _showZeros_attr);
				s.Write("\"");
			}
			if ((bool)(_rightToLeft_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" rightToLeft=\"");
				OoxmlComplexType.WriteData(s, _rightToLeft_attr);
				s.Write("\"");
			}
			if ((bool)(_tabSelected_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" tabSelected=\"");
				OoxmlComplexType.WriteData(s, _tabSelected_attr);
				s.Write("\"");
			}
			if ((bool)(_showRuler_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" showRuler=\"");
				OoxmlComplexType.WriteData(s, _showRuler_attr);
				s.Write("\"");
			}
			if ((bool)(_showOutlineSymbols_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" showOutlineSymbols=\"");
				OoxmlComplexType.WriteData(s, _showOutlineSymbols_attr);
				s.Write("\"");
			}
			if ((bool)(_defaultGridColor_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" defaultGridColor=\"");
				OoxmlComplexType.WriteData(s, _defaultGridColor_attr);
				s.Write("\"");
			}
			if ((bool)(_showWhiteSpace_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" showWhiteSpace=\"");
				OoxmlComplexType.WriteData(s, _showWhiteSpace_attr);
				s.Write("\"");
			}
			if (_view_attr != ST_SheetViewType.normal)
			{
				s.Write(" view=\"");
				OoxmlComplexType.WriteData(s, _view_attr);
				s.Write("\"");
			}
			if (_colorId_attr != Convert.ToUInt32("64", CultureInfo.InvariantCulture))
			{
				s.Write(" colorId=\"");
				OoxmlComplexType.WriteData(s, _colorId_attr);
				s.Write("\"");
			}
			if (_zoomScale_attr != Convert.ToUInt32("100", CultureInfo.InvariantCulture))
			{
				s.Write(" zoomScale=\"");
				OoxmlComplexType.WriteData(s, _zoomScale_attr);
				s.Write("\"");
			}
			if (_zoomScaleNormal_attr != Convert.ToUInt32("0", CultureInfo.InvariantCulture))
			{
				s.Write(" zoomScaleNormal=\"");
				OoxmlComplexType.WriteData(s, _zoomScaleNormal_attr);
				s.Write("\"");
			}
			if (_zoomScaleSheetLayoutView_attr != Convert.ToUInt32("0", CultureInfo.InvariantCulture))
			{
				s.Write(" zoomScaleSheetLayoutView=\"");
				OoxmlComplexType.WriteData(s, _zoomScaleSheetLayoutView_attr);
				s.Write("\"");
			}
			if (_zoomScalePageLayoutView_attr != Convert.ToUInt32("0", CultureInfo.InvariantCulture))
			{
				s.Write(" zoomScalePageLayoutView=\"");
				OoxmlComplexType.WriteData(s, _zoomScalePageLayoutView_attr);
				s.Write("\"");
			}
			if (_topLeftCell_attr_is_specified)
			{
				s.Write(" topLeftCell=\"");
				OoxmlComplexType.WriteData(s, _topLeftCell_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			Write_pane(s, depth, namespaces);
		}

		public void Write_pane(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_pane != null)
			{
				_pane.Write(s, "pane", depth + 1, namespaces);
			}
		}
	}
}
