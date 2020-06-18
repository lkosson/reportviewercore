using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_WorkbookPr : OoxmlComplexType
	{
		private OoxmlBool _date1904_attr;

		private OoxmlBool _dateCompatibility_attr;

		private ST_Objects _showObjects_attr;

		private OoxmlBool _showBorderUnselectedTables_attr;

		private OoxmlBool _filterPrivacy_attr;

		private OoxmlBool _promptedSolutions_attr;

		private OoxmlBool _showInkAnnotation_attr;

		private OoxmlBool _backupFile_attr;

		private OoxmlBool _saveExternalLinkValues_attr;

		private ST_UpdateLinks _updateLinks_attr;

		private OoxmlBool _hidePivotFieldList_attr;

		private OoxmlBool _showPivotChartFilter_attr;

		private OoxmlBool _allowRefreshQuery_attr;

		private OoxmlBool _publishItems_attr;

		private OoxmlBool _checkCompatibility_attr;

		private OoxmlBool _autoCompressPictures_attr;

		private OoxmlBool _refreshAllConnections_attr;

		private string _codeName_attr;

		private bool _codeName_attr_is_specified;

		private uint _defaultThemeVersion_attr;

		private bool _defaultThemeVersion_attr_is_specified;

		public OoxmlBool Date1904_Attr
		{
			get
			{
				return _date1904_attr;
			}
			set
			{
				_date1904_attr = value;
			}
		}

		public OoxmlBool DateCompatibility_Attr
		{
			get
			{
				return _dateCompatibility_attr;
			}
			set
			{
				_dateCompatibility_attr = value;
			}
		}

		public ST_Objects ShowObjects_Attr
		{
			get
			{
				return _showObjects_attr;
			}
			set
			{
				_showObjects_attr = value;
			}
		}

		public OoxmlBool ShowBorderUnselectedTables_Attr
		{
			get
			{
				return _showBorderUnselectedTables_attr;
			}
			set
			{
				_showBorderUnselectedTables_attr = value;
			}
		}

		public OoxmlBool FilterPrivacy_Attr
		{
			get
			{
				return _filterPrivacy_attr;
			}
			set
			{
				_filterPrivacy_attr = value;
			}
		}

		public OoxmlBool PromptedSolutions_Attr
		{
			get
			{
				return _promptedSolutions_attr;
			}
			set
			{
				_promptedSolutions_attr = value;
			}
		}

		public OoxmlBool ShowInkAnnotation_Attr
		{
			get
			{
				return _showInkAnnotation_attr;
			}
			set
			{
				_showInkAnnotation_attr = value;
			}
		}

		public OoxmlBool BackupFile_Attr
		{
			get
			{
				return _backupFile_attr;
			}
			set
			{
				_backupFile_attr = value;
			}
		}

		public OoxmlBool SaveExternalLinkValues_Attr
		{
			get
			{
				return _saveExternalLinkValues_attr;
			}
			set
			{
				_saveExternalLinkValues_attr = value;
			}
		}

		public ST_UpdateLinks UpdateLinks_Attr
		{
			get
			{
				return _updateLinks_attr;
			}
			set
			{
				_updateLinks_attr = value;
			}
		}

		public OoxmlBool HidePivotFieldList_Attr
		{
			get
			{
				return _hidePivotFieldList_attr;
			}
			set
			{
				_hidePivotFieldList_attr = value;
			}
		}

		public OoxmlBool ShowPivotChartFilter_Attr
		{
			get
			{
				return _showPivotChartFilter_attr;
			}
			set
			{
				_showPivotChartFilter_attr = value;
			}
		}

		public OoxmlBool AllowRefreshQuery_Attr
		{
			get
			{
				return _allowRefreshQuery_attr;
			}
			set
			{
				_allowRefreshQuery_attr = value;
			}
		}

		public OoxmlBool PublishItems_Attr
		{
			get
			{
				return _publishItems_attr;
			}
			set
			{
				_publishItems_attr = value;
			}
		}

		public OoxmlBool CheckCompatibility_Attr
		{
			get
			{
				return _checkCompatibility_attr;
			}
			set
			{
				_checkCompatibility_attr = value;
			}
		}

		public OoxmlBool AutoCompressPictures_Attr
		{
			get
			{
				return _autoCompressPictures_attr;
			}
			set
			{
				_autoCompressPictures_attr = value;
			}
		}

		public OoxmlBool RefreshAllConnections_Attr
		{
			get
			{
				return _refreshAllConnections_attr;
			}
			set
			{
				_refreshAllConnections_attr = value;
			}
		}

		public uint DefaultThemeVersion_Attr
		{
			get
			{
				return _defaultThemeVersion_attr;
			}
			set
			{
				_defaultThemeVersion_attr = value;
				_defaultThemeVersion_attr_is_specified = true;
			}
		}

		public bool DefaultThemeVersion_Attr_Is_Specified
		{
			get
			{
				return _defaultThemeVersion_attr_is_specified;
			}
			set
			{
				_defaultThemeVersion_attr_is_specified = value;
			}
		}

		public string CodeName_Attr
		{
			get
			{
				return _codeName_attr;
			}
			set
			{
				_codeName_attr = value;
				_codeName_attr_is_specified = (value != null);
			}
		}

		protected override void InitAttributes()
		{
			_date1904_attr = OoxmlBool.OoxmlFalse;
			_dateCompatibility_attr = OoxmlBool.OoxmlTrue;
			_showObjects_attr = ST_Objects.all;
			_showBorderUnselectedTables_attr = OoxmlBool.OoxmlTrue;
			_filterPrivacy_attr = OoxmlBool.OoxmlFalse;
			_promptedSolutions_attr = OoxmlBool.OoxmlFalse;
			_showInkAnnotation_attr = OoxmlBool.OoxmlTrue;
			_backupFile_attr = OoxmlBool.OoxmlFalse;
			_saveExternalLinkValues_attr = OoxmlBool.OoxmlTrue;
			_updateLinks_attr = ST_UpdateLinks.userSet;
			_hidePivotFieldList_attr = OoxmlBool.OoxmlFalse;
			_showPivotChartFilter_attr = OoxmlBool.OoxmlFalse;
			_allowRefreshQuery_attr = OoxmlBool.OoxmlFalse;
			_publishItems_attr = OoxmlBool.OoxmlFalse;
			_checkCompatibility_attr = OoxmlBool.OoxmlFalse;
			_autoCompressPictures_attr = OoxmlBool.OoxmlTrue;
			_refreshAllConnections_attr = OoxmlBool.OoxmlFalse;
			_codeName_attr_is_specified = false;
			_defaultThemeVersion_attr_is_specified = false;
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
			if ((bool)(_date1904_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" date1904=\"");
				OoxmlComplexType.WriteData(s, _date1904_attr);
				s.Write("\"");
			}
			if ((bool)(_dateCompatibility_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" dateCompatibility=\"");
				OoxmlComplexType.WriteData(s, _dateCompatibility_attr);
				s.Write("\"");
			}
			if (_showObjects_attr != ST_Objects.all)
			{
				s.Write(" showObjects=\"");
				OoxmlComplexType.WriteData(s, _showObjects_attr);
				s.Write("\"");
			}
			if ((bool)(_showBorderUnselectedTables_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" showBorderUnselectedTables=\"");
				OoxmlComplexType.WriteData(s, _showBorderUnselectedTables_attr);
				s.Write("\"");
			}
			if ((bool)(_filterPrivacy_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" filterPrivacy=\"");
				OoxmlComplexType.WriteData(s, _filterPrivacy_attr);
				s.Write("\"");
			}
			if ((bool)(_promptedSolutions_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" promptedSolutions=\"");
				OoxmlComplexType.WriteData(s, _promptedSolutions_attr);
				s.Write("\"");
			}
			if ((bool)(_showInkAnnotation_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" showInkAnnotation=\"");
				OoxmlComplexType.WriteData(s, _showInkAnnotation_attr);
				s.Write("\"");
			}
			if ((bool)(_backupFile_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" backupFile=\"");
				OoxmlComplexType.WriteData(s, _backupFile_attr);
				s.Write("\"");
			}
			if ((bool)(_saveExternalLinkValues_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" saveExternalLinkValues=\"");
				OoxmlComplexType.WriteData(s, _saveExternalLinkValues_attr);
				s.Write("\"");
			}
			if (_updateLinks_attr != ST_UpdateLinks.userSet)
			{
				s.Write(" updateLinks=\"");
				OoxmlComplexType.WriteData(s, _updateLinks_attr);
				s.Write("\"");
			}
			if ((bool)(_hidePivotFieldList_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" hidePivotFieldList=\"");
				OoxmlComplexType.WriteData(s, _hidePivotFieldList_attr);
				s.Write("\"");
			}
			if ((bool)(_showPivotChartFilter_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" showPivotChartFilter=\"");
				OoxmlComplexType.WriteData(s, _showPivotChartFilter_attr);
				s.Write("\"");
			}
			if ((bool)(_allowRefreshQuery_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" allowRefreshQuery=\"");
				OoxmlComplexType.WriteData(s, _allowRefreshQuery_attr);
				s.Write("\"");
			}
			if ((bool)(_publishItems_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" publishItems=\"");
				OoxmlComplexType.WriteData(s, _publishItems_attr);
				s.Write("\"");
			}
			if ((bool)(_checkCompatibility_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" checkCompatibility=\"");
				OoxmlComplexType.WriteData(s, _checkCompatibility_attr);
				s.Write("\"");
			}
			if ((bool)(_autoCompressPictures_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" autoCompressPictures=\"");
				OoxmlComplexType.WriteData(s, _autoCompressPictures_attr);
				s.Write("\"");
			}
			if ((bool)(_refreshAllConnections_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" refreshAllConnections=\"");
				OoxmlComplexType.WriteData(s, _refreshAllConnections_attr);
				s.Write("\"");
			}
			if (_codeName_attr_is_specified)
			{
				s.Write(" codeName=\"");
				OoxmlComplexType.WriteData(s, _codeName_attr);
				s.Write("\"");
			}
			if (_defaultThemeVersion_attr_is_specified)
			{
				s.Write(" defaultThemeVersion=\"");
				OoxmlComplexType.WriteData(s, _defaultThemeVersion_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
		}
	}
}
