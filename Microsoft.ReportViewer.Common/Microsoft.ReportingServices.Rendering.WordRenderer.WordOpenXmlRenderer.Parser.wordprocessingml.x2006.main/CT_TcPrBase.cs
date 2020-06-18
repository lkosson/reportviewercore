using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_TcPrBase : OoxmlComplexType, IOoxmlComplexType
	{
		private CT_TblWidth _tcW;

		private CT_DecimalNumber _gridSpan;

		private CT_OnOff _noWrap;

		private CT_OnOff _tcFitText;

		private CT_OnOff _hideMark;

		public CT_TblWidth TcW
		{
			get
			{
				return _tcW;
			}
			set
			{
				_tcW = value;
			}
		}

		public CT_DecimalNumber GridSpan
		{
			get
			{
				return _gridSpan;
			}
			set
			{
				_gridSpan = value;
			}
		}

		public CT_OnOff NoWrap
		{
			get
			{
				return _noWrap;
			}
			set
			{
				_noWrap = value;
			}
		}

		public CT_OnOff TcFitText
		{
			get
			{
				return _tcFitText;
			}
			set
			{
				_tcFitText = value;
			}
		}

		public CT_OnOff HideMark
		{
			get
			{
				return _hideMark;
			}
			set
			{
				_hideMark = value;
			}
		}

		public static string TcWElementName => "tcW";

		public static string GridSpanElementName => "gridSpan";

		public static string NoWrapElementName => "noWrap";

		public static string TcFitTextElementName => "tcFitText";

		public static string HideMarkElementName => "hideMark";

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
			Write_tcW(s);
			Write_gridSpan(s);
			Write_noWrap(s);
			Write_tcFitText(s);
			Write_hideMark(s);
		}

		public void Write_tcW(TextWriter s)
		{
			if (_tcW != null)
			{
				_tcW.Write(s, "tcW");
			}
		}

		public void Write_gridSpan(TextWriter s)
		{
			if (_gridSpan != null)
			{
				_gridSpan.Write(s, "gridSpan");
			}
		}

		public void Write_noWrap(TextWriter s)
		{
			if (_noWrap != null)
			{
				_noWrap.Write(s, "noWrap");
			}
		}

		public void Write_tcFitText(TextWriter s)
		{
			if (_tcFitText != null)
			{
				_tcFitText.Write(s, "tcFitText");
			}
		}

		public void Write_hideMark(TextWriter s)
		{
			if (_hideMark != null)
			{
				_hideMark.Write(s, "hideMark");
			}
		}
	}
}
