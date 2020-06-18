using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_PageSetup : OoxmlComplexType
	{
		private uint _paperSize_attr;

		private uint _scale_attr;

		private uint _firstPageNumber_attr;

		private uint _fitToWidth_attr;

		private uint _fitToHeight_attr;

		private ST_PageOrder _pageOrder_attr;

		private ST_Orientation _orientation_attr;

		private OoxmlBool _usePrinterDefaults_attr;

		private OoxmlBool _blackAndWhite_attr;

		private OoxmlBool _draft_attr;

		private ST_CellComments _cellComments_attr;

		private OoxmlBool _useFirstPageNumber_attr;

		private ST_PrintError _errors_attr;

		private uint _horizontalDpi_attr;

		private uint _verticalDpi_attr;

		private uint _copies_attr;

		private string _paperHeight_attr;

		private bool _paperHeight_attr_is_specified;

		private string _paperWidth_attr;

		private bool _paperWidth_attr_is_specified;

		private string _id_attr;

		private bool _id_attr_is_specified;

		public uint PaperSize_Attr
		{
			get
			{
				return _paperSize_attr;
			}
			set
			{
				_paperSize_attr = value;
			}
		}

		public uint Scale_Attr
		{
			get
			{
				return _scale_attr;
			}
			set
			{
				_scale_attr = value;
			}
		}

		public uint FirstPageNumber_Attr
		{
			get
			{
				return _firstPageNumber_attr;
			}
			set
			{
				_firstPageNumber_attr = value;
			}
		}

		public uint FitToWidth_Attr
		{
			get
			{
				return _fitToWidth_attr;
			}
			set
			{
				_fitToWidth_attr = value;
			}
		}

		public uint FitToHeight_Attr
		{
			get
			{
				return _fitToHeight_attr;
			}
			set
			{
				_fitToHeight_attr = value;
			}
		}

		public ST_PageOrder PageOrder_Attr
		{
			get
			{
				return _pageOrder_attr;
			}
			set
			{
				_pageOrder_attr = value;
			}
		}

		public ST_Orientation Orientation_Attr
		{
			get
			{
				return _orientation_attr;
			}
			set
			{
				_orientation_attr = value;
			}
		}

		public OoxmlBool UsePrinterDefaults_Attr
		{
			get
			{
				return _usePrinterDefaults_attr;
			}
			set
			{
				_usePrinterDefaults_attr = value;
			}
		}

		public OoxmlBool BlackAndWhite_Attr
		{
			get
			{
				return _blackAndWhite_attr;
			}
			set
			{
				_blackAndWhite_attr = value;
			}
		}

		public OoxmlBool Draft_Attr
		{
			get
			{
				return _draft_attr;
			}
			set
			{
				_draft_attr = value;
			}
		}

		public ST_CellComments CellComments_Attr
		{
			get
			{
				return _cellComments_attr;
			}
			set
			{
				_cellComments_attr = value;
			}
		}

		public OoxmlBool UseFirstPageNumber_Attr
		{
			get
			{
				return _useFirstPageNumber_attr;
			}
			set
			{
				_useFirstPageNumber_attr = value;
			}
		}

		public ST_PrintError Errors_Attr
		{
			get
			{
				return _errors_attr;
			}
			set
			{
				_errors_attr = value;
			}
		}

		public uint HorizontalDpi_Attr
		{
			get
			{
				return _horizontalDpi_attr;
			}
			set
			{
				_horizontalDpi_attr = value;
			}
		}

		public uint VerticalDpi_Attr
		{
			get
			{
				return _verticalDpi_attr;
			}
			set
			{
				_verticalDpi_attr = value;
			}
		}

		public uint Copies_Attr
		{
			get
			{
				return _copies_attr;
			}
			set
			{
				_copies_attr = value;
			}
		}

		public string PaperHeight_Attr
		{
			get
			{
				return _paperHeight_attr;
			}
			set
			{
				_paperHeight_attr = value;
				_paperHeight_attr_is_specified = (value != null);
			}
		}

		public string PaperWidth_Attr
		{
			get
			{
				return _paperWidth_attr;
			}
			set
			{
				_paperWidth_attr = value;
				_paperWidth_attr_is_specified = (value != null);
			}
		}

		public string Id_Attr
		{
			get
			{
				return _id_attr;
			}
			set
			{
				_id_attr = value;
				_id_attr_is_specified = (value != null);
			}
		}

		protected override void InitAttributes()
		{
			_paperSize_attr = Convert.ToUInt32("1", CultureInfo.InvariantCulture);
			_scale_attr = Convert.ToUInt32("100", CultureInfo.InvariantCulture);
			_firstPageNumber_attr = Convert.ToUInt32("1", CultureInfo.InvariantCulture);
			_fitToWidth_attr = Convert.ToUInt32("1", CultureInfo.InvariantCulture);
			_fitToHeight_attr = Convert.ToUInt32("1", CultureInfo.InvariantCulture);
			_pageOrder_attr = ST_PageOrder.downThenOver;
			_orientation_attr = ST_Orientation._default;
			_usePrinterDefaults_attr = OoxmlBool.OoxmlTrue;
			_blackAndWhite_attr = OoxmlBool.OoxmlFalse;
			_draft_attr = OoxmlBool.OoxmlFalse;
			_cellComments_attr = ST_CellComments.none;
			_useFirstPageNumber_attr = OoxmlBool.OoxmlFalse;
			_errors_attr = ST_PrintError.displayed;
			_horizontalDpi_attr = Convert.ToUInt32("600", CultureInfo.InvariantCulture);
			_verticalDpi_attr = Convert.ToUInt32("600", CultureInfo.InvariantCulture);
			_copies_attr = Convert.ToUInt32("1", CultureInfo.InvariantCulture);
			_paperHeight_attr_is_specified = false;
			_paperWidth_attr_is_specified = false;
			_id_attr_is_specified = false;
		}

		protected override void InitElements()
		{
		}

		protected override void InitCollections()
		{
		}

		public override void WriteAsRoot(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces)
		{
			WriteOpenTag(s, tagName, depth, namespaces, root: true);
			WriteElements(s, depth, namespaces);
			WriteCloseTag(s, tagName, depth, namespaces);
		}

		public override void Write(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces)
		{
			WriteOpenTag(s, tagName, depth, namespaces, root: false);
			WriteElements(s, depth, namespaces);
			WriteCloseTag(s, tagName, depth, namespaces);
		}

		public override void WriteOpenTag(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces, bool root)
		{
			s.Write("<");
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
			s.Write(tagName);
			WriteAttributes(s);
			if (root)
			{
				foreach (string key in namespaces.Keys)
				{
					s.Write(" xmlns");
					if (namespaces[key] != "")
					{
						s.Write(":");
						s.Write(namespaces[key]);
					}
					s.Write("=\"");
					s.Write(key);
					s.Write("\"");
				}
			}
			s.Write(">");
		}

		public override void WriteCloseTag(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces)
		{
			s.Write("</");
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
			if (_paperSize_attr != Convert.ToUInt32("1", CultureInfo.InvariantCulture))
			{
				s.Write(" paperSize=\"");
				OoxmlComplexType.WriteData(s, _paperSize_attr);
				s.Write("\"");
			}
			if (_scale_attr != Convert.ToUInt32("100", CultureInfo.InvariantCulture))
			{
				s.Write(" scale=\"");
				OoxmlComplexType.WriteData(s, _scale_attr);
				s.Write("\"");
			}
			if (_firstPageNumber_attr != Convert.ToUInt32("1", CultureInfo.InvariantCulture))
			{
				s.Write(" firstPageNumber=\"");
				OoxmlComplexType.WriteData(s, _firstPageNumber_attr);
				s.Write("\"");
			}
			if (_fitToWidth_attr != Convert.ToUInt32("1", CultureInfo.InvariantCulture))
			{
				s.Write(" fitToWidth=\"");
				OoxmlComplexType.WriteData(s, _fitToWidth_attr);
				s.Write("\"");
			}
			if (_fitToHeight_attr != Convert.ToUInt32("1", CultureInfo.InvariantCulture))
			{
				s.Write(" fitToHeight=\"");
				OoxmlComplexType.WriteData(s, _fitToHeight_attr);
				s.Write("\"");
			}
			if (_pageOrder_attr != ST_PageOrder.downThenOver)
			{
				s.Write(" pageOrder=\"");
				OoxmlComplexType.WriteData(s, _pageOrder_attr);
				s.Write("\"");
			}
			if (_orientation_attr != ST_Orientation._default)
			{
				s.Write(" orientation=\"");
				OoxmlComplexType.WriteData(s, _orientation_attr);
				s.Write("\"");
			}
			if ((bool)(_usePrinterDefaults_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" usePrinterDefaults=\"");
				OoxmlComplexType.WriteData(s, _usePrinterDefaults_attr);
				s.Write("\"");
			}
			if ((bool)(_blackAndWhite_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" blackAndWhite=\"");
				OoxmlComplexType.WriteData(s, _blackAndWhite_attr);
				s.Write("\"");
			}
			if ((bool)(_draft_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" draft=\"");
				OoxmlComplexType.WriteData(s, _draft_attr);
				s.Write("\"");
			}
			if (_cellComments_attr != ST_CellComments.none)
			{
				s.Write(" cellComments=\"");
				OoxmlComplexType.WriteData(s, _cellComments_attr);
				s.Write("\"");
			}
			if ((bool)(_useFirstPageNumber_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" useFirstPageNumber=\"");
				OoxmlComplexType.WriteData(s, _useFirstPageNumber_attr);
				s.Write("\"");
			}
			if (_errors_attr != ST_PrintError.displayed)
			{
				s.Write(" errors=\"");
				OoxmlComplexType.WriteData(s, _errors_attr);
				s.Write("\"");
			}
			if (_horizontalDpi_attr != Convert.ToUInt32("600", CultureInfo.InvariantCulture))
			{
				s.Write(" horizontalDpi=\"");
				OoxmlComplexType.WriteData(s, _horizontalDpi_attr);
				s.Write("\"");
			}
			if (_verticalDpi_attr != Convert.ToUInt32("600", CultureInfo.InvariantCulture))
			{
				s.Write(" verticalDpi=\"");
				OoxmlComplexType.WriteData(s, _verticalDpi_attr);
				s.Write("\"");
			}
			if (_copies_attr != Convert.ToUInt32("1", CultureInfo.InvariantCulture))
			{
				s.Write(" copies=\"");
				OoxmlComplexType.WriteData(s, _copies_attr);
				s.Write("\"");
			}
			if (_paperHeight_attr_is_specified)
			{
				s.Write(" paperHeight=\"");
				OoxmlComplexType.WriteData(s, _paperHeight_attr);
				s.Write("\"");
			}
			if (_paperWidth_attr_is_specified)
			{
				s.Write(" paperWidth=\"");
				OoxmlComplexType.WriteData(s, _paperWidth_attr);
				s.Write("\"");
			}
			if (_id_attr_is_specified)
			{
				s.Write(" r:id=\"");
				OoxmlComplexType.WriteData(s, _id_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
		}
	}
}
