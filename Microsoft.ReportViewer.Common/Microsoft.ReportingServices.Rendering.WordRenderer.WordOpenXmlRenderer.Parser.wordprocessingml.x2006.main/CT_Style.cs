using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_Style : OoxmlComplexType, IOoxmlComplexType
	{
		private bool __default_attr;

		private bool __default_attr_is_specified;

		private CT_String _name;

		private CT_String _aliases;

		private CT_String _basedOn;

		private CT_String _next;

		private CT_String _link;

		private CT_OnOff _autoRedefine;

		private CT_OnOff _hidden;

		private CT_DecimalNumber _uiPriority;

		private CT_OnOff _semiHidden;

		private CT_OnOff _unhideWhenUsed;

		private CT_OnOff _qFormat;

		private CT_OnOff _locked;

		private CT_OnOff _personal;

		private CT_OnOff _personalCompose;

		private CT_OnOff _personalReply;

		private CT_LongHexNumber _rsid;

		private CT_RPr _rPr;

		private CT_TblPrBase _tblPr;

		private CT_TrPr _trPr;

		private CT_TcPr _tcPr;

		public bool _default_Attr
		{
			get
			{
				return __default_attr;
			}
			set
			{
				__default_attr = value;
				__default_attr_is_specified = true;
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

		public CT_String Aliases
		{
			get
			{
				return _aliases;
			}
			set
			{
				_aliases = value;
			}
		}

		public CT_String BasedOn
		{
			get
			{
				return _basedOn;
			}
			set
			{
				_basedOn = value;
			}
		}

		public CT_String Next
		{
			get
			{
				return _next;
			}
			set
			{
				_next = value;
			}
		}

		public CT_String Link
		{
			get
			{
				return _link;
			}
			set
			{
				_link = value;
			}
		}

		public CT_OnOff AutoRedefine
		{
			get
			{
				return _autoRedefine;
			}
			set
			{
				_autoRedefine = value;
			}
		}

		public CT_OnOff Hidden
		{
			get
			{
				return _hidden;
			}
			set
			{
				_hidden = value;
			}
		}

		public CT_DecimalNumber UiPriority
		{
			get
			{
				return _uiPriority;
			}
			set
			{
				_uiPriority = value;
			}
		}

		public CT_OnOff SemiHidden
		{
			get
			{
				return _semiHidden;
			}
			set
			{
				_semiHidden = value;
			}
		}

		public CT_OnOff UnhideWhenUsed
		{
			get
			{
				return _unhideWhenUsed;
			}
			set
			{
				_unhideWhenUsed = value;
			}
		}

		public CT_OnOff QFormat
		{
			get
			{
				return _qFormat;
			}
			set
			{
				_qFormat = value;
			}
		}

		public CT_OnOff Locked
		{
			get
			{
				return _locked;
			}
			set
			{
				_locked = value;
			}
		}

		public CT_OnOff Personal
		{
			get
			{
				return _personal;
			}
			set
			{
				_personal = value;
			}
		}

		public CT_OnOff PersonalCompose
		{
			get
			{
				return _personalCompose;
			}
			set
			{
				_personalCompose = value;
			}
		}

		public CT_OnOff PersonalReply
		{
			get
			{
				return _personalReply;
			}
			set
			{
				_personalReply = value;
			}
		}

		public CT_LongHexNumber Rsid
		{
			get
			{
				return _rsid;
			}
			set
			{
				_rsid = value;
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

		public CT_TblPrBase TblPr
		{
			get
			{
				return _tblPr;
			}
			set
			{
				_tblPr = value;
			}
		}

		public CT_TrPr TrPr
		{
			get
			{
				return _trPr;
			}
			set
			{
				_trPr = value;
			}
		}

		public CT_TcPr TcPr
		{
			get
			{
				return _tcPr;
			}
			set
			{
				_tcPr = value;
			}
		}

		public static string NameElementName => "name";

		public static string AliasesElementName => "aliases";

		public static string BasedOnElementName => "basedOn";

		public static string NextElementName => "next";

		public static string LinkElementName => "link";

		public static string AutoRedefineElementName => "autoRedefine";

		public static string HiddenElementName => "hidden";

		public static string UiPriorityElementName => "uiPriority";

		public static string SemiHiddenElementName => "semiHidden";

		public static string UnhideWhenUsedElementName => "unhideWhenUsed";

		public static string QFormatElementName => "qFormat";

		public static string LockedElementName => "locked";

		public static string PersonalElementName => "personal";

		public static string PersonalComposeElementName => "personalCompose";

		public static string PersonalReplyElementName => "personalReply";

		public static string RsidElementName => "rsid";

		public static string RPrElementName => "rPr";

		public static string TblPrElementName => "tblPr";

		public static string TrPrElementName => "trPr";

		public static string TcPrElementName => "tcPr";

		protected override void InitAttributes()
		{
			__default_attr_is_specified = false;
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
			if (__default_attr_is_specified)
			{
				s.Write(" w:default=\"");
				OoxmlComplexType.WriteData(s, __default_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s)
		{
			Write_name(s);
			Write_aliases(s);
			Write_basedOn(s);
			Write_next(s);
			Write_link(s);
			Write_autoRedefine(s);
			Write_hidden(s);
			Write_uiPriority(s);
			Write_semiHidden(s);
			Write_unhideWhenUsed(s);
			Write_qFormat(s);
			Write_locked(s);
			Write_personal(s);
			Write_personalCompose(s);
			Write_personalReply(s);
			Write_rsid(s);
			Write_rPr(s);
			Write_tblPr(s);
			Write_trPr(s);
			Write_tcPr(s);
		}

		public void Write_name(TextWriter s)
		{
			if (_name != null)
			{
				_name.Write(s, "name");
			}
		}

		public void Write_aliases(TextWriter s)
		{
			if (_aliases != null)
			{
				_aliases.Write(s, "aliases");
			}
		}

		public void Write_basedOn(TextWriter s)
		{
			if (_basedOn != null)
			{
				_basedOn.Write(s, "basedOn");
			}
		}

		public void Write_next(TextWriter s)
		{
			if (_next != null)
			{
				_next.Write(s, "next");
			}
		}

		public void Write_link(TextWriter s)
		{
			if (_link != null)
			{
				_link.Write(s, "link");
			}
		}

		public void Write_autoRedefine(TextWriter s)
		{
			if (_autoRedefine != null)
			{
				_autoRedefine.Write(s, "autoRedefine");
			}
		}

		public void Write_hidden(TextWriter s)
		{
			if (_hidden != null)
			{
				_hidden.Write(s, "hidden");
			}
		}

		public void Write_uiPriority(TextWriter s)
		{
			if (_uiPriority != null)
			{
				_uiPriority.Write(s, "uiPriority");
			}
		}

		public void Write_semiHidden(TextWriter s)
		{
			if (_semiHidden != null)
			{
				_semiHidden.Write(s, "semiHidden");
			}
		}

		public void Write_unhideWhenUsed(TextWriter s)
		{
			if (_unhideWhenUsed != null)
			{
				_unhideWhenUsed.Write(s, "unhideWhenUsed");
			}
		}

		public void Write_qFormat(TextWriter s)
		{
			if (_qFormat != null)
			{
				_qFormat.Write(s, "qFormat");
			}
		}

		public void Write_locked(TextWriter s)
		{
			if (_locked != null)
			{
				_locked.Write(s, "locked");
			}
		}

		public void Write_personal(TextWriter s)
		{
			if (_personal != null)
			{
				_personal.Write(s, "personal");
			}
		}

		public void Write_personalCompose(TextWriter s)
		{
			if (_personalCompose != null)
			{
				_personalCompose.Write(s, "personalCompose");
			}
		}

		public void Write_personalReply(TextWriter s)
		{
			if (_personalReply != null)
			{
				_personalReply.Write(s, "personalReply");
			}
		}

		public void Write_rsid(TextWriter s)
		{
			if (_rsid != null)
			{
				_rsid.Write(s, "rsid");
			}
		}

		public void Write_rPr(TextWriter s)
		{
			if (_rPr != null)
			{
				_rPr.Write(s, "rPr");
			}
		}

		public void Write_tblPr(TextWriter s)
		{
			if (_tblPr != null)
			{
				_tblPr.Write(s, "tblPr");
			}
		}

		public void Write_trPr(TextWriter s)
		{
			if (_trPr != null)
			{
				_trPr.Write(s, "trPr");
			}
		}

		public void Write_tcPr(TextWriter s)
		{
			if (_tcPr != null)
			{
				_tcPr.Write(s, "tcPr");
			}
		}
	}
}
