using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_Pane : OoxmlComplexType
	{
		private double _xSplit_attr;

		private double _ySplit_attr;

		private ST_Pane _activePane_attr;

		private ST_PaneState _state_attr;

		private string _topLeftCell_attr;

		private bool _topLeftCell_attr_is_specified;

		public double XSplit_Attr
		{
			get
			{
				return _xSplit_attr;
			}
			set
			{
				_xSplit_attr = value;
			}
		}

		public double YSplit_Attr
		{
			get
			{
				return _ySplit_attr;
			}
			set
			{
				_ySplit_attr = value;
			}
		}

		public ST_Pane ActivePane_Attr
		{
			get
			{
				return _activePane_attr;
			}
			set
			{
				_activePane_attr = value;
			}
		}

		public ST_PaneState State_Attr
		{
			get
			{
				return _state_attr;
			}
			set
			{
				_state_attr = value;
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

		protected override void InitAttributes()
		{
			_xSplit_attr = Convert.ToDouble("0", CultureInfo.InvariantCulture);
			_ySplit_attr = Convert.ToDouble("0", CultureInfo.InvariantCulture);
			_activePane_attr = ST_Pane.topLeft;
			_state_attr = ST_PaneState.split;
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
			if (_xSplit_attr != Convert.ToDouble("0", CultureInfo.InvariantCulture))
			{
				s.Write(" xSplit=\"");
				OoxmlComplexType.WriteData(s, _xSplit_attr);
				s.Write("\"");
			}
			if (_ySplit_attr != Convert.ToDouble("0", CultureInfo.InvariantCulture))
			{
				s.Write(" ySplit=\"");
				OoxmlComplexType.WriteData(s, _ySplit_attr);
				s.Write("\"");
			}
			if (_activePane_attr != ST_Pane.topLeft)
			{
				s.Write(" activePane=\"");
				OoxmlComplexType.WriteData(s, _activePane_attr);
				s.Write("\"");
			}
			if (_state_attr != ST_PaneState.split)
			{
				s.Write(" state=\"");
				OoxmlComplexType.WriteData(s, _state_attr);
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
		}
	}
}
