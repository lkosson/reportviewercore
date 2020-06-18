using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_Row : OoxmlComplexType, IOoxmlComplexType, IEG_ContentRowContent
	{
		private string _rsidRPr_attr;

		private string _rsidR_attr;

		private string _rsidDel_attr;

		private string _rsidTr_attr;

		private CT_TrPr _trPr;

		private List<IEG_ContentCellContent> _EG_ContentCellContents;

		public override GeneratedType GroupInterfaceType => GeneratedType.CT_Row;

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

		public string RsidTr_Attr
		{
			get
			{
				return _rsidTr_attr;
			}
			set
			{
				_rsidTr_attr = value;
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

		public List<IEG_ContentCellContent> EG_ContentCellContents
		{
			get
			{
				return _EG_ContentCellContents;
			}
			set
			{
				_EG_ContentCellContents = value;
			}
		}

		public static string TrPrElementName => "trPr";

		public static string TcElementName => "tc";

		public static string CustomXmlElementName => "customXml";

		public static string SdtElementName => "sdt";

		public static string ProofErrElementName => "proofErr";

		public static string PermStartElementName => "permStart";

		public static string PermEndElementName => "permEnd";

		public static string BookmarkStartElementName => "bookmarkStart";

		public static string BookmarkEndElementName => "bookmarkEnd";

		public static string MoveFromRangeStartElementName => "moveFromRangeStart";

		public static string CustomXmlInsRangeStartElementName => "customXmlInsRangeStart";

		public static string CustomXmlInsRangeEndElementName => "customXmlInsRangeEnd";

		public static string InsElementName => "ins";

		protected override void InitAttributes()
		{
		}

		protected override void InitElements()
		{
		}

		protected override void InitCollections()
		{
			_EG_ContentCellContents = new List<IEG_ContentCellContent>();
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
			s.Write(" w:rsidTr=\"");
			OoxmlComplexType.WriteData(s, _rsidTr_attr);
			s.Write("\"");
		}

		public override void WriteElements(TextWriter s)
		{
			Write_trPr(s);
			Write_EG_ContentCellContents(s);
		}

		public void Write_trPr(TextWriter s)
		{
			if (_trPr != null)
			{
				_trPr.Write(s, "trPr");
			}
		}

		public void Write_EG_ContentCellContents(TextWriter s)
		{
			for (int i = 0; i < _EG_ContentCellContents.Count; i++)
			{
				string tagName = null;
				switch (_EG_ContentCellContents[i].GroupInterfaceType)
				{
				case GeneratedType.CT_Tc:
					tagName = "tc";
					break;
				case GeneratedType.CT_CustomXmlCell:
					tagName = "customXml";
					break;
				case GeneratedType.CT_SdtCell:
					tagName = "sdt";
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
				}
				_EG_ContentCellContents[i].Write(s, tagName);
			}
		}
	}
}
