using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_Numbering : OoxmlComplexType, IOoxmlComplexType
	{
		private CT_DecimalNumber _numIdMacAtCleanup;

		private List<CT_AbstractNum> _abstractNum;

		private List<CT_Num> _num;

		public CT_DecimalNumber NumIdMacAtCleanup
		{
			get
			{
				return _numIdMacAtCleanup;
			}
			set
			{
				_numIdMacAtCleanup = value;
			}
		}

		public List<CT_AbstractNum> AbstractNum
		{
			get
			{
				return _abstractNum;
			}
			set
			{
				_abstractNum = value;
			}
		}

		public List<CT_Num> Num
		{
			get
			{
				return _num;
			}
			set
			{
				_num = value;
			}
		}

		public static string NumIdMacAtCleanupElementName => "numIdMacAtCleanup";

		public static string AbstractNumElementName => "abstractNum";

		public static string NumElementName => "num";

		protected override void InitAttributes()
		{
		}

		protected override void InitElements()
		{
		}

		protected override void InitCollections()
		{
			_abstractNum = new List<CT_AbstractNum>();
			_num = new List<CT_Num>();
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
			Write_abstractNum(s);
			Write_num(s);
			Write_numIdMacAtCleanup(s);
		}

		public void Write_numIdMacAtCleanup(TextWriter s)
		{
			if (_numIdMacAtCleanup != null)
			{
				_numIdMacAtCleanup.Write(s, "numIdMacAtCleanup");
			}
		}

		public void Write_abstractNum(TextWriter s)
		{
			if (_abstractNum == null)
			{
				return;
			}
			foreach (CT_AbstractNum item in _abstractNum)
			{
				item?.Write(s, "abstractNum");
			}
		}

		public void Write_num(TextWriter s)
		{
			if (_num == null)
			{
				return;
			}
			foreach (CT_Num item in _num)
			{
				item?.Write(s, "num");
			}
		}
	}
}
