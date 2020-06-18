using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_Cell : OoxmlComplexType
	{
		private uint _s_attr;

		private ST_CellType _t_attr;

		private uint _cm_attr;

		private uint _vm_attr;

		private OoxmlBool _ph_attr;

		private string _r_attr;

		private bool _r_attr_is_specified;

		private string _v;

		private CT_Rst _is;

		public uint S_Attr
		{
			get
			{
				return _s_attr;
			}
			set
			{
				_s_attr = value;
			}
		}

		public ST_CellType T_Attr
		{
			get
			{
				return _t_attr;
			}
			set
			{
				_t_attr = value;
			}
		}

		public uint Cm_Attr
		{
			get
			{
				return _cm_attr;
			}
			set
			{
				_cm_attr = value;
			}
		}

		public uint Vm_Attr
		{
			get
			{
				return _vm_attr;
			}
			set
			{
				_vm_attr = value;
			}
		}

		public OoxmlBool Ph_Attr
		{
			get
			{
				return _ph_attr;
			}
			set
			{
				_ph_attr = value;
			}
		}

		public string R_Attr
		{
			get
			{
				return _r_attr;
			}
			set
			{
				_r_attr = value;
				_r_attr_is_specified = (value != null);
			}
		}

		public string V
		{
			get
			{
				return _v;
			}
			set
			{
				_v = value;
			}
		}

		public CT_Rst Is
		{
			get
			{
				return _is;
			}
			set
			{
				_is = value;
			}
		}

		public static string IsElementName => "is";

		public static string VElementName => "v";

		protected override void InitAttributes()
		{
			_s_attr = Convert.ToUInt32("0", CultureInfo.InvariantCulture);
			_t_attr = ST_CellType.n;
			_cm_attr = Convert.ToUInt32("0", CultureInfo.InvariantCulture);
			_vm_attr = Convert.ToUInt32("0", CultureInfo.InvariantCulture);
			_ph_attr = OoxmlBool.OoxmlFalse;
			_r_attr_is_specified = false;
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
			if (_s_attr != Convert.ToUInt32("0", CultureInfo.InvariantCulture))
			{
				s.Write(" s=\"");
				OoxmlComplexType.WriteData(s, _s_attr);
				s.Write("\"");
			}
			if (_t_attr != ST_CellType.n)
			{
				s.Write(" t=\"");
				OoxmlComplexType.WriteData(s, _t_attr);
				s.Write("\"");
			}
			if (_cm_attr != Convert.ToUInt32("0", CultureInfo.InvariantCulture))
			{
				s.Write(" cm=\"");
				OoxmlComplexType.WriteData(s, _cm_attr);
				s.Write("\"");
			}
			if (_vm_attr != Convert.ToUInt32("0", CultureInfo.InvariantCulture))
			{
				s.Write(" vm=\"");
				OoxmlComplexType.WriteData(s, _vm_attr);
				s.Write("\"");
			}
			if ((bool)(_ph_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" ph=\"");
				OoxmlComplexType.WriteData(s, _ph_attr);
				s.Write("\"");
			}
			if (_r_attr_is_specified)
			{
				s.Write(" r=\"");
				OoxmlComplexType.WriteData(s, _r_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			Write_v(s, depth, namespaces);
			Write_is(s, depth, namespaces);
		}

		public void Write_is(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_is != null)
			{
				_is.Write(s, "is", depth + 1, namespaces);
			}
		}

		public void Write_v(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_v != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "v", "http://schemas.openxmlformats.org/spreadsheetml/2006/main", _v);
			}
		}
	}
}
