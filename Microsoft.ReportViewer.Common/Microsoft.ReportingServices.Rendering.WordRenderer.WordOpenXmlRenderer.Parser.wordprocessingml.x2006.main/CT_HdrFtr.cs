using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_HdrFtr : OoxmlComplexType, IOoxmlComplexType
	{
		public enum ChoiceBucket_0
		{
			customXml,
			sdt,
			p,
			tbl,
			proofErr,
			permStart,
			permEnd,
			bookmarkStart,
			bookmarkEnd,
			moveFromRangeStart,
			moveFromRangeEnd,
			moveToRangeStart,
			moveToRangeEnd,
			commentRangeStart,
			commentRangeEnd,
			customXmlInsRangeStart,
			customXmlInsRangeEnd,
			customXmlDelRangeStart,
			customXmlDelRangeEnd,
			customXmlMoveFromRangeStart,
			customXmlMoveFromRangeEnd,
			customXmlMoveToRangeStart,
			customXmlMoveToRangeEnd,
			ins,
			del,
			moveFrom,
			moveTo,
			oMathPara,
			oMath,
			altChunk
		}

		private List<CT_P> _p;

		private List<CT_Tbl> _tbl;

		private ChoiceBucket_0 _choice_0;

		public List<CT_P> P
		{
			get
			{
				return _p;
			}
			set
			{
				_p = value;
			}
		}

		public List<CT_Tbl> Tbl
		{
			get
			{
				return _tbl;
			}
			set
			{
				_tbl = value;
			}
		}

		public ChoiceBucket_0 Choice_0
		{
			get
			{
				return _choice_0;
			}
			set
			{
				_choice_0 = value;
			}
		}

		public static string PElementName => "p";

		public static string TblElementName => "tbl";

		protected override void InitAttributes()
		{
		}

		protected override void InitElements()
		{
		}

		protected override void InitCollections()
		{
			_p = new List<CT_P>();
			_tbl = new List<CT_Tbl>();
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
			Write_p(s);
			Write_tbl(s);
		}

		public void Write_p(TextWriter s)
		{
			if (_choice_0 != ChoiceBucket_0.p || _p == null)
			{
				return;
			}
			foreach (CT_P item in _p)
			{
				item?.Write(s, "p");
			}
		}

		public void Write_tbl(TextWriter s)
		{
			if (_choice_0 != ChoiceBucket_0.tbl || _tbl == null)
			{
				return;
			}
			foreach (CT_Tbl item in _tbl)
			{
				item?.Write(s, "tbl");
			}
		}
	}
}
