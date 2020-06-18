using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_TblPrBase : OoxmlComplexType, IOoxmlComplexType
	{
		private CT_String _tblStyle;

		private CT_OnOff _bidiVisual;

		private CT_DecimalNumber _tblStyleRowBandSize;

		private CT_DecimalNumber _tblStyleColBandSize;

		private CT_TblWidth _tblW;

		private CT_TblWidth _tblCellSpacing;

		private CT_TblWidth _tblInd;

		private CT_TblCellMar _tblCellMar;

		private CT_String _tblCaption;

		private CT_String _tblDescription;

		public CT_String TblStyle
		{
			get
			{
				return _tblStyle;
			}
			set
			{
				_tblStyle = value;
			}
		}

		public CT_OnOff BidiVisual
		{
			get
			{
				return _bidiVisual;
			}
			set
			{
				_bidiVisual = value;
			}
		}

		public CT_DecimalNumber TblStyleRowBandSize
		{
			get
			{
				return _tblStyleRowBandSize;
			}
			set
			{
				_tblStyleRowBandSize = value;
			}
		}

		public CT_DecimalNumber TblStyleColBandSize
		{
			get
			{
				return _tblStyleColBandSize;
			}
			set
			{
				_tblStyleColBandSize = value;
			}
		}

		public CT_TblWidth TblW
		{
			get
			{
				return _tblW;
			}
			set
			{
				_tblW = value;
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

		public CT_TblWidth TblInd
		{
			get
			{
				return _tblInd;
			}
			set
			{
				_tblInd = value;
			}
		}

		public CT_TblCellMar TblCellMar
		{
			get
			{
				return _tblCellMar;
			}
			set
			{
				_tblCellMar = value;
			}
		}

		public CT_String TblCaption
		{
			get
			{
				return _tblCaption;
			}
			set
			{
				_tblCaption = value;
			}
		}

		public CT_String TblDescription
		{
			get
			{
				return _tblDescription;
			}
			set
			{
				_tblDescription = value;
			}
		}

		public static string TblStyleElementName => "tblStyle";

		public static string BidiVisualElementName => "bidiVisual";

		public static string TblStyleRowBandSizeElementName => "tblStyleRowBandSize";

		public static string TblStyleColBandSizeElementName => "tblStyleColBandSize";

		public static string TblWElementName => "tblW";

		public static string TblCellSpacingElementName => "tblCellSpacing";

		public static string TblIndElementName => "tblInd";

		public static string TblCellMarElementName => "tblCellMar";

		public static string TblCaptionElementName => "tblCaption";

		public static string TblDescriptionElementName => "tblDescription";

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
			Write_tblStyle(s);
			Write_bidiVisual(s);
			Write_tblStyleRowBandSize(s);
			Write_tblStyleColBandSize(s);
			Write_tblW(s);
			Write_tblCellSpacing(s);
			Write_tblInd(s);
			Write_tblCellMar(s);
			Write_tblCaption(s);
			Write_tblDescription(s);
		}

		public void Write_tblStyle(TextWriter s)
		{
			if (_tblStyle != null)
			{
				_tblStyle.Write(s, "tblStyle");
			}
		}

		public void Write_bidiVisual(TextWriter s)
		{
			if (_bidiVisual != null)
			{
				_bidiVisual.Write(s, "bidiVisual");
			}
		}

		public void Write_tblStyleRowBandSize(TextWriter s)
		{
			if (_tblStyleRowBandSize != null)
			{
				_tblStyleRowBandSize.Write(s, "tblStyleRowBandSize");
			}
		}

		public void Write_tblStyleColBandSize(TextWriter s)
		{
			if (_tblStyleColBandSize != null)
			{
				_tblStyleColBandSize.Write(s, "tblStyleColBandSize");
			}
		}

		public void Write_tblW(TextWriter s)
		{
			if (_tblW != null)
			{
				_tblW.Write(s, "tblW");
			}
		}

		public void Write_tblCellSpacing(TextWriter s)
		{
			if (_tblCellSpacing != null)
			{
				_tblCellSpacing.Write(s, "tblCellSpacing");
			}
		}

		public void Write_tblInd(TextWriter s)
		{
			if (_tblInd != null)
			{
				_tblInd.Write(s, "tblInd");
			}
		}

		public void Write_tblCellMar(TextWriter s)
		{
			if (_tblCellMar != null)
			{
				_tblCellMar.Write(s, "tblCellMar");
			}
		}

		public void Write_tblCaption(TextWriter s)
		{
			if (_tblCaption != null)
			{
				_tblCaption.Write(s, "tblCaption");
			}
		}

		public void Write_tblDescription(TextWriter s)
		{
			if (_tblDescription != null)
			{
				_tblDescription.Write(s, "tblDescription");
			}
		}
	}
}
