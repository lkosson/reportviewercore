using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_P : OoxmlComplexType, IOoxmlComplexType, IEG_BlockLevelElts
	{
		private string _rsidRPr_attr;

		private string _rsidR_attr;

		private string _rsidDel_attr;

		private string _rsidP_attr;

		private string _rsidRDefault_attr;

		private CT_PPr _pPr;

		private List<IEG_PContent> _EG_PContents;

		public override GeneratedType GroupInterfaceType => GeneratedType.CT_P;

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

		public string RsidP_Attr
		{
			get
			{
				return _rsidP_attr;
			}
			set
			{
				_rsidP_attr = value;
			}
		}

		public string RsidRDefault_Attr
		{
			get
			{
				return _rsidRDefault_attr;
			}
			set
			{
				_rsidRDefault_attr = value;
			}
		}

		public CT_PPr PPr
		{
			get
			{
				return _pPr;
			}
			set
			{
				_pPr = value;
			}
		}

		public List<IEG_PContent> EG_PContents
		{
			get
			{
				return _EG_PContents;
			}
			set
			{
				_EG_PContents = value;
			}
		}

		public static string PPrElementName => "pPr";

		public static string CustomXmlElementName => "customXml";

		public static string SmartTagElementName => "smartTag";

		public static string SdtElementName => "sdt";

		public static string DirElementName => "dir";

		public static string BdoElementName => "bdo";

		public static string RElementName => "r";

		public static string ProofErrElementName => "proofErr";

		public static string PermStartElementName => "permStart";

		public static string PermEndElementName => "permEnd";

		public static string BookmarkStartElementName => "bookmarkStart";

		public static string BookmarkEndElementName => "bookmarkEnd";

		public static string MoveFromRangeStartElementName => "moveFromRangeStart";

		public static string CustomXmlInsRangeStartElementName => "customXmlInsRangeStart";

		public static string CustomXmlInsRangeEndElementName => "customXmlInsRangeEnd";

		public static string InsElementName => "ins";

		public static string FldSimpleElementName => "fldSimple";

		public static string HyperlinkElementName => "hyperlink";

		public static string SubDocElementName => "subDoc";

		protected override void InitAttributes()
		{
		}

		protected override void InitElements()
		{
		}

		protected override void InitCollections()
		{
			_EG_PContents = new List<IEG_PContent>();
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
			s.Write(" w:rsidR=\"");
			OoxmlComplexType.WriteData(s, _rsidR_attr);
			s.Write("\"");
			s.Write(" w:rsidDel=\"");
			OoxmlComplexType.WriteData(s, _rsidDel_attr);
			s.Write("\"");
			s.Write(" w:rsidP=\"");
			OoxmlComplexType.WriteData(s, _rsidP_attr);
			s.Write("\"");
			s.Write(" w:rsidRDefault=\"");
			OoxmlComplexType.WriteData(s, _rsidRDefault_attr);
			s.Write("\"");
		}

		public override void WriteElements(TextWriter s)
		{
			Write_pPr(s);
			Write_EG_PContents(s);
		}

		public void Write_pPr(TextWriter s)
		{
			if (_pPr != null)
			{
				_pPr.Write(s, "pPr");
			}
		}

		public void Write_EG_PContents(TextWriter s)
		{
			for (int i = 0; i < _EG_PContents.Count; i++)
			{
				string tagName = null;
				switch (_EG_PContents[i].GroupInterfaceType)
				{
				case GeneratedType.CT_CustomXmlRun:
					tagName = "customXml";
					break;
				case GeneratedType.CT_SmartTagRun:
					tagName = "smartTag";
					break;
				case GeneratedType.CT_SdtRun:
					tagName = "sdt";
					break;
				case GeneratedType.CT_DirContentRun:
					tagName = "dir";
					break;
				case GeneratedType.CT_BdoContentRun:
					tagName = "bdo";
					break;
				case GeneratedType.CT_R:
					tagName = "r";
					break;
				case GeneratedType.CT_ProofErr:
					tagName = "proofErr";
					break;
				case GeneratedType.CT_PermStart:
					tagName = "permStart";
					break;
				case GeneratedType.CT_Perm:
					tagName = "permEnd";
					break;
				case GeneratedType.CT_Bookmark:
					tagName = "bookmarkStart";
					break;
				case GeneratedType.CT_MarkupRange:
					tagName = "bookmarkEnd";
					break;
				case GeneratedType.CT_MoveBookmark:
					tagName = "moveFromRangeStart";
					break;
				case GeneratedType.CT_TrackChange:
					tagName = "customXmlInsRangeStart";
					break;
				case GeneratedType.CT_Markup:
					tagName = "customXmlInsRangeEnd";
					break;
				case GeneratedType.CT_RunTrackChange:
					tagName = "ins";
					break;
				case GeneratedType.CT_SimpleField:
					tagName = "fldSimple";
					break;
				case GeneratedType.CT_Hyperlink:
					tagName = "hyperlink";
					break;
				case GeneratedType.CT_Rel:
					tagName = "subDoc";
					break;
				}
				_EG_PContents[i].Write(s, tagName);
			}
		}
	}
}
