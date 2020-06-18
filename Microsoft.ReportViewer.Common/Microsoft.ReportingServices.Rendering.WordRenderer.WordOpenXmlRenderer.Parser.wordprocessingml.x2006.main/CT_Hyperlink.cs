using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_Hyperlink : OoxmlComplexType, IOoxmlComplexType, IEG_PContent
	{
		public enum ChoiceBucket_0
		{
			customXml,
			smartTag,
			sdt,
			dir,
			bdo,
			r,
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
			fldSimple,
			hyperlink,
			subDoc
		}

		private string _id_attr;

		private CT_R _r;

		private CT_Hyperlink _hyperlink;

		private CT_Rel _subDoc;

		private ChoiceBucket_0 _choice_0;

		public override GeneratedType GroupInterfaceType => GeneratedType.CT_Hyperlink;

		public string Id_Attr
		{
			get
			{
				return _id_attr;
			}
			set
			{
				_id_attr = value;
			}
		}

		public CT_R R
		{
			get
			{
				return _r;
			}
			set
			{
				_r = value;
			}
		}

		public CT_Hyperlink Hyperlink
		{
			get
			{
				return _hyperlink;
			}
			set
			{
				_hyperlink = value;
			}
		}

		public CT_Rel SubDoc
		{
			get
			{
				return _subDoc;
			}
			set
			{
				_subDoc = value;
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

		public static string RElementName => "r";

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
			s.Write(" r:id=\"");
			OoxmlComplexType.WriteData(s, _id_attr);
			s.Write("\"");
		}

		public override void WriteElements(TextWriter s)
		{
			Write_r(s);
			Write_hyperlink(s);
			Write_subDoc(s);
		}

		public void Write_r(TextWriter s)
		{
			if (_choice_0 == ChoiceBucket_0.r && _r != null)
			{
				_r.Write(s, "r");
			}
		}

		public void Write_hyperlink(TextWriter s)
		{
			if (_choice_0 == ChoiceBucket_0.hyperlink && _hyperlink != null)
			{
				_hyperlink.Write(s, "hyperlink");
			}
		}

		public void Write_subDoc(TextWriter s)
		{
			if (_choice_0 == ChoiceBucket_0.subDoc && _subDoc != null)
			{
				_subDoc.Write(s, "subDoc");
			}
		}
	}
}
