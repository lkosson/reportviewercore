using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_TrPrBase : OoxmlComplexType, IOoxmlComplexType
	{
		private CT_DecimalNumber _divId;

		private CT_DecimalNumber _gridBefore;

		private CT_DecimalNumber _gridAfter;

		private CT_TblWidth _wBefore;

		private CT_TblWidth _wAfter;

		private CT_OnOff _cantSplit;

		private CT_Height _trHeight;

		private CT_OnOff _tblHeader;

		private CT_TblWidth _tblCellSpacing;

		private CT_OnOff _hidden;

		public CT_DecimalNumber DivId
		{
			get
			{
				return _divId;
			}
			set
			{
				_divId = value;
			}
		}

		public CT_DecimalNumber GridBefore
		{
			get
			{
				return _gridBefore;
			}
			set
			{
				_gridBefore = value;
			}
		}

		public CT_DecimalNumber GridAfter
		{
			get
			{
				return _gridAfter;
			}
			set
			{
				_gridAfter = value;
			}
		}

		public CT_TblWidth WBefore
		{
			get
			{
				return _wBefore;
			}
			set
			{
				_wBefore = value;
			}
		}

		public CT_TblWidth WAfter
		{
			get
			{
				return _wAfter;
			}
			set
			{
				_wAfter = value;
			}
		}

		public CT_OnOff CantSplit
		{
			get
			{
				return _cantSplit;
			}
			set
			{
				_cantSplit = value;
			}
		}

		public CT_Height TrHeight
		{
			get
			{
				return _trHeight;
			}
			set
			{
				_trHeight = value;
			}
		}

		public CT_OnOff TblHeader
		{
			get
			{
				return _tblHeader;
			}
			set
			{
				_tblHeader = value;
			}
		}

		public CT_TblWidth TblCellSpacing
		{
			get
			{
				return _tblCellSpacing;
			}
			set
			{
				_tblCellSpacing = value;
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

		public static string DivIdElementName => "divId";

		public static string GridBeforeElementName => "gridBefore";

		public static string GridAfterElementName => "gridAfter";

		public static string WBeforeElementName => "wBefore";

		public static string WAfterElementName => "wAfter";

		public static string CantSplitElementName => "cantSplit";

		public static string TrHeightElementName => "trHeight";

		public static string TblHeaderElementName => "tblHeader";

		public static string TblCellSpacingElementName => "tblCellSpacing";

		public static string HiddenElementName => "hidden";

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
		}

		public override void WriteElements(TextWriter s)
		{
			Write_divId(s);
			Write_gridBefore(s);
			Write_gridAfter(s);
			Write_wBefore(s);
			Write_wAfter(s);
			Write_cantSplit(s);
			Write_trHeight(s);
			Write_tblHeader(s);
			Write_tblCellSpacing(s);
			Write_hidden(s);
		}

		public void Write_divId(TextWriter s)
		{
			if (_divId != null)
			{
				_divId.Write(s, "divId");
			}
		}

		public void Write_gridBefore(TextWriter s)
		{
			if (_gridBefore != null)
			{
				_gridBefore.Write(s, "gridBefore");
			}
		}

		public void Write_gridAfter(TextWriter s)
		{
			if (_gridAfter != null)
			{
				_gridAfter.Write(s, "gridAfter");
			}
		}

		public void Write_wBefore(TextWriter s)
		{
			if (_wBefore != null)
			{
				_wBefore.Write(s, "wBefore");
			}
		}

		public void Write_wAfter(TextWriter s)
		{
			if (_wAfter != null)
			{
				_wAfter.Write(s, "wAfter");
			}
		}

		public void Write_cantSplit(TextWriter s)
		{
			if (_cantSplit != null)
			{
				_cantSplit.Write(s, "cantSplit");
			}
		}

		public void Write_trHeight(TextWriter s)
		{
			if (_trHeight != null)
			{
				_trHeight.Write(s, "trHeight");
			}
		}

		public void Write_tblHeader(TextWriter s)
		{
			if (_tblHeader != null)
			{
				_tblHeader.Write(s, "tblHeader");
			}
		}

		public void Write_tblCellSpacing(TextWriter s)
		{
			if (_tblCellSpacing != null)
			{
				_tblCellSpacing.Write(s, "tblCellSpacing");
			}
		}

		public void Write_hidden(TextWriter s)
		{
			if (_hidden != null)
			{
				_hidden.Write(s, "hidden");
			}
		}
	}
}
