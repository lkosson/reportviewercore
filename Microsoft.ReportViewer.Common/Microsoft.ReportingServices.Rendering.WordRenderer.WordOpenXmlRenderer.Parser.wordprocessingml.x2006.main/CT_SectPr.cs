using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_SectPr : OoxmlComplexType, IOoxmlComplexType
	{
		private string _rsidRPr_attr;

		private string _rsidDel_attr;

		private string _rsidR_attr;

		private string _rsidSect_attr;

		private CT_SectType _type;

		private CT_PageSz _pgSz;

		private CT_PageMar _pgMar;

		private CT_OnOff _formProt;

		private CT_OnOff _noEndnote;

		private CT_OnOff _titlePg;

		private CT_OnOff _bidi;

		private CT_OnOff _rtlGutter;

		private CT_Rel _printerSettings;

		private List<IEG_HdrFtrReferences> _EG_HdrFtrReferencess;

		public string RsidRPr_Attr
		{
			get
			{
				return _rsidRPr_attr;
			}
			set
			{
				_rsidRPr_attr = value;
			}
		}

		public string RsidDel_Attr
		{
			get
			{
				return _rsidDel_attr;
			}
			set
			{
				_rsidDel_attr = value;
			}
		}

		public string RsidR_Attr
		{
			get
			{
				return _rsidR_attr;
			}
			set
			{
				_rsidR_attr = value;
			}
		}

		public string RsidSect_Attr
		{
			get
			{
				return _rsidSect_attr;
			}
			set
			{
				_rsidSect_attr = value;
			}
		}

		public CT_SectType Type
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
			}
		}

		public CT_PageSz PgSz
		{
			get
			{
				return _pgSz;
			}
			set
			{
				_pgSz = value;
			}
		}

		public CT_PageMar PgMar
		{
			get
			{
				return _pgMar;
			}
			set
			{
				_pgMar = value;
			}
		}

		public CT_OnOff FormProt
		{
			get
			{
				return _formProt;
			}
			set
			{
				_formProt = value;
			}
		}

		public CT_OnOff NoEndnote
		{
			get
			{
				return _noEndnote;
			}
			set
			{
				_noEndnote = value;
			}
		}

		public CT_OnOff TitlePg
		{
			get
			{
				return _titlePg;
			}
			set
			{
				_titlePg = value;
			}
		}

		public CT_OnOff Bidi
		{
			get
			{
				return _bidi;
			}
			set
			{
				_bidi = value;
			}
		}

		public CT_OnOff RtlGutter
		{
			get
			{
				return _rtlGutter;
			}
			set
			{
				_rtlGutter = value;
			}
		}

		public CT_Rel PrinterSettings
		{
			get
			{
				return _printerSettings;
			}
			set
			{
				_printerSettings = value;
			}
		}

		public List<IEG_HdrFtrReferences> EG_HdrFtrReferencess
		{
			get
			{
				return _EG_HdrFtrReferencess;
			}
			set
			{
				_EG_HdrFtrReferencess = value;
			}
		}

		public static string TypeElementName => "type";

		public static string PgSzElementName => "pgSz";

		public static string PgMarElementName => "pgMar";

		public static string FormProtElementName => "formProt";

		public static string NoEndnoteElementName => "noEndnote";

		public static string TitlePgElementName => "titlePg";

		public static string BidiElementName => "bidi";

		public static string RtlGutterElementName => "rtlGutter";

		public static string PrinterSettingsElementName => "printerSettings";

		public static string HeaderReferenceElementName => "headerReference";

		public static string FooterReferenceElementName => "footerReference";

		protected override void InitAttributes()
		{
		}

		protected override void InitElements()
		{
		}

		protected override void InitCollections()
		{
			_EG_HdrFtrReferencess = new List<IEG_HdrFtrReferences>();
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
			s.Write(" w:rsidRPr=\"");
			OoxmlComplexType.WriteData(s, _rsidRPr_attr);
			s.Write("\"");
			s.Write(" w:rsidDel=\"");
			OoxmlComplexType.WriteData(s, _rsidDel_attr);
			s.Write("\"");
			s.Write(" w:rsidR=\"");
			OoxmlComplexType.WriteData(s, _rsidR_attr);
			s.Write("\"");
			s.Write(" w:rsidSect=\"");
			OoxmlComplexType.WriteData(s, _rsidSect_attr);
			s.Write("\"");
		}

		public override void WriteElements(TextWriter s)
		{
			Write_EG_HdrFtrReferencess(s);
			Write_type(s);
			Write_pgSz(s);
			Write_pgMar(s);
			Write_formProt(s);
			Write_noEndnote(s);
			Write_titlePg(s);
			Write_bidi(s);
			Write_rtlGutter(s);
			Write_printerSettings(s);
		}

		public void Write_type(TextWriter s)
		{
			if (_type != null)
			{
				_type.Write(s, "type");
			}
		}

		public void Write_pgSz(TextWriter s)
		{
			if (_pgSz != null)
			{
				_pgSz.Write(s, "pgSz");
			}
		}

		public void Write_pgMar(TextWriter s)
		{
			if (_pgMar != null)
			{
				_pgMar.Write(s, "pgMar");
			}
		}

		public void Write_formProt(TextWriter s)
		{
			if (_formProt != null)
			{
				_formProt.Write(s, "formProt");
			}
		}

		public void Write_noEndnote(TextWriter s)
		{
			if (_noEndnote != null)
			{
				_noEndnote.Write(s, "noEndnote");
			}
		}

		public void Write_titlePg(TextWriter s)
		{
			if (_titlePg != null)
			{
				_titlePg.Write(s, "titlePg");
			}
		}

		public void Write_bidi(TextWriter s)
		{
			if (_bidi != null)
			{
				_bidi.Write(s, "bidi");
			}
		}

		public void Write_rtlGutter(TextWriter s)
		{
			if (_rtlGutter != null)
			{
				_rtlGutter.Write(s, "rtlGutter");
			}
		}

		public void Write_printerSettings(TextWriter s)
		{
			if (_printerSettings != null)
			{
				_printerSettings.Write(s, "printerSettings");
			}
		}

		public void Write_EG_HdrFtrReferencess(TextWriter s)
		{
			for (int i = 0; i < _EG_HdrFtrReferencess.Count; i++)
			{
				string tagName = null;
				switch (_EG_HdrFtrReferencess[i].GroupInterfaceType)
				{
				case GeneratedType.CT_HdrRef:
					tagName = "headerReference";
					break;
				case GeneratedType.CT_FtrRef:
					tagName = "footerReference";
					break;
				}
				_EG_HdrFtrReferencess[i].Write(s, tagName);
			}
		}
	}
}
