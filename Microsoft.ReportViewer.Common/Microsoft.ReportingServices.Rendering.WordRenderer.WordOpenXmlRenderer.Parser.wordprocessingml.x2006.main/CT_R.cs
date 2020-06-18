using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_R : OoxmlComplexType, IOoxmlComplexType, IEG_PContent
	{
		private string _rsidRPr_attr;

		private string _rsidDel_attr;

		private string _rsidR_attr;

		private CT_RPr _rPr;

		private List<IEG_RunInnerContent> _EG_RunInnerContents;

		public override GeneratedType GroupInterfaceType => GeneratedType.CT_R;

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

		public List<IEG_RunInnerContent> EG_RunInnerContents
		{
			get
			{
				return _EG_RunInnerContents;
			}
			set
			{
				_EG_RunInnerContents = value;
			}
		}

		public static string RPrElementName => "rPr";

		public static string BrElementName => "br";

		public static string TElementName => "t";

		public static string ContentPartElementName => "contentPart";

		public static string InstrTextElementName => "instrText";

		public static string NoBreakHyphenElementName => "noBreakHyphen";

		public static string SymElementName => "sym";

		public static string ObjectElementName => "object";

		public static string FldCharElementName => "fldChar";

		public static string RubyElementName => "ruby";

		public static string FootnoteReferenceElementName => "footnoteReference";

		public static string CommentReferenceElementName => "commentReference";

		public static string DrawingElementName => "drawing";

		public static string PtabElementName => "ptab";

		protected override void InitAttributes()
		{
		}

		protected override void InitElements()
		{
		}

		protected override void InitCollections()
		{
			_EG_RunInnerContents = new List<IEG_RunInnerContent>();
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
		}

		public override void WriteElements(TextWriter s)
		{
			Write_rPr(s);
			Write_EG_RunInnerContents(s);
		}

		public void Write_rPr(TextWriter s)
		{
			if (_rPr != null)
			{
				_rPr.Write(s, "rPr");
			}
		}

		public void Write_EG_RunInnerContents(TextWriter s)
		{
			for (int i = 0; i < _EG_RunInnerContents.Count; i++)
			{
				string tagName = null;
				switch (_EG_RunInnerContents[i].GroupInterfaceType)
				{
				case GeneratedType.CT_Br:
					tagName = "br";
					break;
				case GeneratedType.CT_Text:
					tagName = "t";
					break;
				case GeneratedType.CT_Rel:
					tagName = "contentPart";
					break;
				case GeneratedType.CT_InstrText:
					tagName = "instrText";
					break;
				case GeneratedType.CT_Empty:
					tagName = "noBreakHyphen";
					break;
				case GeneratedType.CT_Sym:
					tagName = "sym";
					break;
				case GeneratedType.CT_Object:
					tagName = "object";
					break;
				case GeneratedType.CT_FldChar:
					tagName = "fldChar";
					break;
				case GeneratedType.CT_Ruby:
					tagName = "ruby";
					break;
				case GeneratedType.CT_FtnEdnRef:
					tagName = "footnoteReference";
					break;
				case GeneratedType.CT_Markup:
					tagName = "commentReference";
					break;
				case GeneratedType.CT_Drawing:
					tagName = "drawing";
					break;
				case GeneratedType.CT_PTab:
					tagName = "ptab";
					break;
				}
				_EG_RunInnerContents[i].Write(s, tagName);
			}
		}
	}
}
