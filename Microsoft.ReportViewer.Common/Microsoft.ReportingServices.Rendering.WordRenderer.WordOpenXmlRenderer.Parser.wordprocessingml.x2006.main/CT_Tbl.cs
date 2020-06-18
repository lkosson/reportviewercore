using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_Tbl : OoxmlComplexType, IOoxmlComplexType, IEG_BlockLevelElts
	{
		private CT_TblPr _tblPr;

		private CT_TblGrid _tblGrid;

		private List<IEG_RangeMarkupElements> _EG_RangeMarkupElementss;

		private List<IEG_ContentRowContent> _EG_ContentRowContents;

		public override GeneratedType GroupInterfaceType => GeneratedType.CT_Tbl;

		public CT_TblPr TblPr
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

		public CT_TblGrid TblGrid
		{
			get
			{
				return _tblGrid;
			}
			set
			{
				_tblGrid = value;
			}
		}

		public List<IEG_RangeMarkupElements> EG_RangeMarkupElementss
		{
			get
			{
				return _EG_RangeMarkupElementss;
			}
			set
			{
				_EG_RangeMarkupElementss = value;
			}
		}

		public List<IEG_ContentRowContent> EG_ContentRowContents
		{
			get
			{
				return _EG_ContentRowContents;
			}
			set
			{
				_EG_ContentRowContents = value;
			}
		}

		public static string TblPrElementName => "tblPr";

		public static string TblGridElementName => "tblGrid";

		public static string BookmarkStartElementName => "bookmarkStart";

		public static string BookmarkEndElementName => "bookmarkEnd";

		public static string MoveFromRangeStartElementName => "moveFromRangeStart";

		public static string CustomXmlInsRangeStartElementName => "customXmlInsRangeStart";

		public static string CustomXmlInsRangeEndElementName => "customXmlInsRangeEnd";

		public static string TrElementName => "tr";

		public static string CustomXmlElementName => "customXml";

		public static string SdtElementName => "sdt";

		public static string ProofErrElementName => "proofErr";

		public static string PermStartElementName => "permStart";

		public static string PermEndElementName => "permEnd";

		public static string InsElementName => "ins";

		protected override void InitAttributes()
		{
		}

		protected override void InitElements()
		{
			_tblPr = new CT_TblPr();
			_tblGrid = new CT_TblGrid();
		}

		protected override void InitCollections()
		{
			_EG_RangeMarkupElementss = new List<IEG_RangeMarkupElements>();
			_EG_ContentRowContents = new List<IEG_ContentRowContent>();
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
		}

		public override void WriteElements(TextWriter s)
		{
			Write_EG_RangeMarkupElementss(s);
			Write_tblPr(s);
			Write_tblGrid(s);
			Write_EG_ContentRowContents(s);
		}

		public void Write_tblPr(TextWriter s)
		{
			if (_tblPr != null)
			{
				_tblPr.Write(s, "tblPr");
			}
		}

		public void Write_tblGrid(TextWriter s)
		{
			if (_tblGrid != null)
			{
				_tblGrid.Write(s, "tblGrid");
			}
		}

		public void Write_EG_RangeMarkupElementss(TextWriter s)
		{
			for (int i = 0; i < _EG_RangeMarkupElementss.Count; i++)
			{
				string tagName = null;
				switch (_EG_RangeMarkupElementss[i].GroupInterfaceType)
				{
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
				}
				_EG_RangeMarkupElementss[i].Write(s, tagName);
			}
		}

		public void Write_EG_ContentRowContents(TextWriter s)
		{
			for (int i = 0; i < _EG_ContentRowContents.Count; i++)
			{
				string tagName = null;
				switch (_EG_ContentRowContents[i].GroupInterfaceType)
				{
				case GeneratedType.CT_Row:
					tagName = "tr";
					break;
				case GeneratedType.CT_CustomXmlRow:
					tagName = "customXml";
					break;
				case GeneratedType.CT_SdtRow:
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
				_EG_ContentRowContents[i].Write(s, tagName);
			}
		}
	}
}
