using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_Lvl : OoxmlComplexType, IOoxmlComplexType
	{
		private int _ilvl_attr;

		private CT_DecimalNumber _start;

		private CT_NumFmt _numFmt;

		private CT_DecimalNumber _lvlRestart;

		private CT_String _pStyle;

		private CT_OnOff _isLgl;

		private CT_LevelText _lvlText;

		private CT_DecimalNumber _lvlPicBulletId;

		private CT_Jc _lvlJc;

		private CT_RPr _rPr;

		public int Ilvl_Attr
		{
			get
			{
				return _ilvl_attr;
			}
			set
			{
				_ilvl_attr = value;
			}
		}

		public CT_DecimalNumber Start
		{
			get
			{
				return _start;
			}
			set
			{
				_start = value;
			}
		}

		public CT_NumFmt NumFmt
		{
			get
			{
				return _numFmt;
			}
			set
			{
				_numFmt = value;
			}
		}

		public CT_DecimalNumber LvlRestart
		{
			get
			{
				return _lvlRestart;
			}
			set
			{
				_lvlRestart = value;
			}
		}

		public CT_String PStyle
		{
			get
			{
				return _pStyle;
			}
			set
			{
				_pStyle = value;
			}
		}

		public CT_OnOff IsLgl
		{
			get
			{
				return _isLgl;
			}
			set
			{
				_isLgl = value;
			}
		}

		public CT_LevelText LvlText
		{
			get
			{
				return _lvlText;
			}
			set
			{
				_lvlText = value;
			}
		}

		public CT_DecimalNumber LvlPicBulletId
		{
			get
			{
				return _lvlPicBulletId;
			}
			set
			{
				_lvlPicBulletId = value;
			}
		}

		public CT_Jc LvlJc
		{
			get
			{
				return _lvlJc;
			}
			set
			{
				_lvlJc = value;
			}
		}

		public CT_RPr RPr
		{
			get
			{
				return _rPr;
			}
			set
			{
				_rPr = value;
			}
		}

		public static string StartElementName => "start";

		public static string NumFmtElementName => "numFmt";

		public static string LvlRestartElementName => "lvlRestart";

		public static string PStyleElementName => "pStyle";

		public static string IsLglElementName => "isLgl";

		public static string LvlTextElementName => "lvlText";

		public static string LvlPicBulletIdElementName => "lvlPicBulletId";

		public static string LvlJcElementName => "lvlJc";

		public static string RPrElementName => "rPr";

		protected override void InitAttributes()
		{
		}

		protected override void InitElements()
		{
		}

		protected override void InitCollections()
		{
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
			s.Write(" w:ilvl=\"");
			OoxmlComplexType.WriteData(s, _ilvl_attr);
			s.Write("\"");
		}

		public override void WriteElements(TextWriter s)
		{
			Write_start(s);
			Write_numFmt(s);
			Write_lvlRestart(s);
			Write_pStyle(s);
			Write_isLgl(s);
			Write_lvlText(s);
			Write_lvlPicBulletId(s);
			Write_lvlJc(s);
			Write_rPr(s);
		}

		public void Write_start(TextWriter s)
		{
			if (_start != null)
			{
				_start.Write(s, "start");
			}
		}

		public void Write_numFmt(TextWriter s)
		{
			if (_numFmt != null)
			{
				_numFmt.Write(s, "numFmt");
			}
		}

		public void Write_lvlRestart(TextWriter s)
		{
			if (_lvlRestart != null)
			{
				_lvlRestart.Write(s, "lvlRestart");
			}
		}

		public void Write_pStyle(TextWriter s)
		{
			if (_pStyle != null)
			{
				_pStyle.Write(s, "pStyle");
			}
		}

		public void Write_isLgl(TextWriter s)
		{
			if (_isLgl != null)
			{
				_isLgl.Write(s, "isLgl");
			}
		}

		public void Write_lvlText(TextWriter s)
		{
			if (_lvlText != null)
			{
				_lvlText.Write(s, "lvlText");
			}
		}

		public void Write_lvlPicBulletId(TextWriter s)
		{
			if (_lvlPicBulletId != null)
			{
				_lvlPicBulletId.Write(s, "lvlPicBulletId");
			}
		}

		public void Write_lvlJc(TextWriter s)
		{
			if (_lvlJc != null)
			{
				_lvlJc.Write(s, "lvlJc");
			}
		}

		public void Write_rPr(TextWriter s)
		{
			if (_rPr != null)
			{
				_rPr.Write(s, "rPr");
			}
		}
	}
}
