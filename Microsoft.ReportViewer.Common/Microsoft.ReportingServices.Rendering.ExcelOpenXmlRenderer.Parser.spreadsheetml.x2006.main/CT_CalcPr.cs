using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_CalcPr : OoxmlComplexType
	{
		private uint _calcId_attr;

		private ST_CalcMode _calcMode_attr;

		private OoxmlBool _fullCalcOnLoad_attr;

		private ST_RefMode _refMode_attr;

		private OoxmlBool _iterate_attr;

		private uint _iterateCount_attr;

		private double _iterateDelta_attr;

		private OoxmlBool _fullPrecision_attr;

		private OoxmlBool _calcCompleted_attr;

		private OoxmlBool _calcOnSave_attr;

		private OoxmlBool _concurrentCalc_attr;

		private uint _concurrentManualCount_attr;

		private bool _concurrentManualCount_attr_is_specified;

		private OoxmlBool _forceFullCalc_attr;

		private bool _forceFullCalc_attr_is_specified;

		public uint CalcId_Attr
		{
			get
			{
				return _calcId_attr;
			}
			set
			{
				_calcId_attr = value;
			}
		}

		public ST_CalcMode CalcMode_Attr
		{
			get
			{
				return _calcMode_attr;
			}
			set
			{
				_calcMode_attr = value;
			}
		}

		public OoxmlBool FullCalcOnLoad_Attr
		{
			get
			{
				return _fullCalcOnLoad_attr;
			}
			set
			{
				_fullCalcOnLoad_attr = value;
			}
		}

		public ST_RefMode RefMode_Attr
		{
			get
			{
				return _refMode_attr;
			}
			set
			{
				_refMode_attr = value;
			}
		}

		public OoxmlBool Iterate_Attr
		{
			get
			{
				return _iterate_attr;
			}
			set
			{
				_iterate_attr = value;
			}
		}

		public uint IterateCount_Attr
		{
			get
			{
				return _iterateCount_attr;
			}
			set
			{
				_iterateCount_attr = value;
			}
		}

		public double IterateDelta_Attr
		{
			get
			{
				return _iterateDelta_attr;
			}
			set
			{
				_iterateDelta_attr = value;
			}
		}

		public OoxmlBool FullPrecision_Attr
		{
			get
			{
				return _fullPrecision_attr;
			}
			set
			{
				_fullPrecision_attr = value;
			}
		}

		public OoxmlBool CalcCompleted_Attr
		{
			get
			{
				return _calcCompleted_attr;
			}
			set
			{
				_calcCompleted_attr = value;
			}
		}

		public OoxmlBool CalcOnSave_Attr
		{
			get
			{
				return _calcOnSave_attr;
			}
			set
			{
				_calcOnSave_attr = value;
			}
		}

		public OoxmlBool ConcurrentCalc_Attr
		{
			get
			{
				return _concurrentCalc_attr;
			}
			set
			{
				_concurrentCalc_attr = value;
			}
		}

		public uint ConcurrentManualCount_Attr
		{
			get
			{
				return _concurrentManualCount_attr;
			}
			set
			{
				_concurrentManualCount_attr = value;
				_concurrentManualCount_attr_is_specified = true;
			}
		}

		public bool ConcurrentManualCount_Attr_Is_Specified
		{
			get
			{
				return _concurrentManualCount_attr_is_specified;
			}
			set
			{
				_concurrentManualCount_attr_is_specified = value;
			}
		}

		public OoxmlBool ForceFullCalc_Attr
		{
			get
			{
				return _forceFullCalc_attr;
			}
			set
			{
				_forceFullCalc_attr = value;
				_forceFullCalc_attr_is_specified = true;
			}
		}

		public bool ForceFullCalc_Attr_Is_Specified
		{
			get
			{
				return _forceFullCalc_attr_is_specified;
			}
			set
			{
				_forceFullCalc_attr_is_specified = value;
			}
		}

		protected override void InitAttributes()
		{
			_calcMode_attr = ST_CalcMode.auto;
			_fullCalcOnLoad_attr = OoxmlBool.OoxmlFalse;
			_refMode_attr = ST_RefMode.A1;
			_iterate_attr = OoxmlBool.OoxmlFalse;
			_iterateCount_attr = Convert.ToUInt32("100", CultureInfo.InvariantCulture);
			_iterateDelta_attr = Convert.ToDouble("0.001", CultureInfo.InvariantCulture);
			_fullPrecision_attr = OoxmlBool.OoxmlTrue;
			_calcCompleted_attr = OoxmlBool.OoxmlTrue;
			_calcOnSave_attr = OoxmlBool.OoxmlTrue;
			_concurrentCalc_attr = OoxmlBool.OoxmlTrue;
			_concurrentManualCount_attr_is_specified = false;
			_forceFullCalc_attr_is_specified = false;
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
			s.Write(" calcId=\"");
			OoxmlComplexType.WriteData(s, _calcId_attr);
			s.Write("\"");
			if (_calcMode_attr != ST_CalcMode.auto)
			{
				s.Write(" calcMode=\"");
				OoxmlComplexType.WriteData(s, _calcMode_attr);
				s.Write("\"");
			}
			if ((bool)(_fullCalcOnLoad_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" fullCalcOnLoad=\"");
				OoxmlComplexType.WriteData(s, _fullCalcOnLoad_attr);
				s.Write("\"");
			}
			if (_refMode_attr != ST_RefMode.A1)
			{
				s.Write(" refMode=\"");
				OoxmlComplexType.WriteData(s, _refMode_attr);
				s.Write("\"");
			}
			if ((bool)(_iterate_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" iterate=\"");
				OoxmlComplexType.WriteData(s, _iterate_attr);
				s.Write("\"");
			}
			if (_iterateCount_attr != Convert.ToUInt32("100", CultureInfo.InvariantCulture))
			{
				s.Write(" iterateCount=\"");
				OoxmlComplexType.WriteData(s, _iterateCount_attr);
				s.Write("\"");
			}
			if (_iterateDelta_attr != Convert.ToDouble("0.001", CultureInfo.InvariantCulture))
			{
				s.Write(" iterateDelta=\"");
				OoxmlComplexType.WriteData(s, _iterateDelta_attr);
				s.Write("\"");
			}
			if ((bool)(_fullPrecision_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" fullPrecision=\"");
				OoxmlComplexType.WriteData(s, _fullPrecision_attr);
				s.Write("\"");
			}
			if ((bool)(_calcCompleted_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" calcCompleted=\"");
				OoxmlComplexType.WriteData(s, _calcCompleted_attr);
				s.Write("\"");
			}
			if ((bool)(_calcOnSave_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" calcOnSave=\"");
				OoxmlComplexType.WriteData(s, _calcOnSave_attr);
				s.Write("\"");
			}
			if ((bool)(_concurrentCalc_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" concurrentCalc=\"");
				OoxmlComplexType.WriteData(s, _concurrentCalc_attr);
				s.Write("\"");
			}
			if (_concurrentManualCount_attr_is_specified)
			{
				s.Write(" concurrentManualCount=\"");
				OoxmlComplexType.WriteData(s, _concurrentManualCount_attr);
				s.Write("\"");
			}
			if (_forceFullCalc_attr_is_specified)
			{
				s.Write(" forceFullCalc=\"");
				OoxmlComplexType.WriteData(s, _forceFullCalc_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
		}
	}
}
