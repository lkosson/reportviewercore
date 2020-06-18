using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_Body : OoxmlComplexType, IOoxmlComplexType
	{
		private CT_SectPr _sectPr;

		private List<IEG_BlockLevelElts> _EG_BlockLevelEltss;

		public CT_SectPr SectPr
		{
			get
			{
				return _sectPr;
			}
			set
			{
				_sectPr = value;
			}
		}

		public List<IEG_BlockLevelElts> EG_BlockLevelEltss
		{
			get
			{
				return _EG_BlockLevelEltss;
			}
			set
			{
				_EG_BlockLevelEltss = value;
			}
		}

		public static string SectPrElementName => "sectPr";

		public static string CustomXmlElementName => "customXml";

		public static string SdtElementName => "sdt";

		public static string PElementName => "p";

		public static string TblElementName => "tbl";

		public static string ProofErrElementName => "proofErr";

		public static string PermStartElementName => "permStart";

		public static string PermEndElementName => "permEnd";

		public static string BookmarkStartElementName => "bookmarkStart";

		public static string BookmarkEndElementName => "bookmarkEnd";

		public static string MoveFromRangeStartElementName => "moveFromRangeStart";

		public static string CustomXmlInsRangeStartElementName => "customXmlInsRangeStart";

		public static string CustomXmlInsRangeEndElementName => "customXmlInsRangeEnd";

		public static string InsElementName => "ins";

		public static string AltChunkElementName => "altChunk";

		protected override void InitAttributes()
		{
		}

		protected override void InitElements()
		{
		}

		protected override void InitCollections()
		{
			_EG_BlockLevelEltss = new List<IEG_BlockLevelElts>();
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
			Write_EG_BlockLevelEltss(s);
			Write_sectPr(s);
		}

		public void Write_sectPr(TextWriter s)
		{
			if (_sectPr != null)
			{
				_sectPr.Write(s, "sectPr");
			}
		}

		public void Write_EG_BlockLevelEltss(TextWriter s)
		{
			for (int i = 0; i < _EG_BlockLevelEltss.Count; i++)
			{
				string tagName = null;
				switch (_EG_BlockLevelEltss[i].GroupInterfaceType)
				{
				case GeneratedType.CT_CustomXmlBlock:
					tagName = "customXml";
					break;
				case GeneratedType.CT_SdtBlock:
					tagName = "sdt";
					break;
				case GeneratedType.CT_P:
					tagName = "p";
					break;
				case GeneratedType.CT_Tbl:
					tagName = "tbl";
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
				case GeneratedType.CT_AltChunk:
					tagName = "altChunk";
					break;
				}
				_EG_BlockLevelEltss[i].Write(s, tagName);
			}
		}
	}
}
