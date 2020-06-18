using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.officeDocument.x2006.extended_properties
{
	internal class CT_Properties : OoxmlComplexType
	{
		private string _Template;

		private string _Manager;

		private string _Company;

		private int _Pages;

		private int _Words;

		private int _Characters;

		private string _PresentationFormat;

		private int _Lines;

		private int _Paragraphs;

		private int _Slides;

		private int _Notes;

		private int _TotalTime;

		private int _HiddenSlides;

		private int _MMClips;

		private OoxmlBool _ScaleCrop;

		private CT_VectorVariant _HeadingPairs;

		private CT_VectorLpstr _TitlesOfParts;

		private OoxmlBool _LinksUpToDate;

		private int _CharactersWithSpaces;

		private OoxmlBool _SharedDoc;

		private string _HyperlinkBase;

		private CT_VectorVariant _HLinks;

		private OoxmlBool _HyperlinksChanged;

		private string _Application;

		private string _AppVersion;

		private int _DocSecurity;

		public string Template
		{
			get
			{
				return _Template;
			}
			set
			{
				_Template = value;
			}
		}

		public string Manager
		{
			get
			{
				return _Manager;
			}
			set
			{
				_Manager = value;
			}
		}

		public string Company
		{
			get
			{
				return _Company;
			}
			set
			{
				_Company = value;
			}
		}

		public int Pages
		{
			get
			{
				return _Pages;
			}
			set
			{
				_Pages = value;
			}
		}

		public int Words
		{
			get
			{
				return _Words;
			}
			set
			{
				_Words = value;
			}
		}

		public int Characters
		{
			get
			{
				return _Characters;
			}
			set
			{
				_Characters = value;
			}
		}

		public string PresentationFormat
		{
			get
			{
				return _PresentationFormat;
			}
			set
			{
				_PresentationFormat = value;
			}
		}

		public int Lines
		{
			get
			{
				return _Lines;
			}
			set
			{
				_Lines = value;
			}
		}

		public int Paragraphs
		{
			get
			{
				return _Paragraphs;
			}
			set
			{
				_Paragraphs = value;
			}
		}

		public int Slides
		{
			get
			{
				return _Slides;
			}
			set
			{
				_Slides = value;
			}
		}

		public int Notes
		{
			get
			{
				return _Notes;
			}
			set
			{
				_Notes = value;
			}
		}

		public int TotalTime
		{
			get
			{
				return _TotalTime;
			}
			set
			{
				_TotalTime = value;
			}
		}

		public int HiddenSlides
		{
			get
			{
				return _HiddenSlides;
			}
			set
			{
				_HiddenSlides = value;
			}
		}

		public int MMClips
		{
			get
			{
				return _MMClips;
			}
			set
			{
				_MMClips = value;
			}
		}

		public OoxmlBool ScaleCrop
		{
			get
			{
				return _ScaleCrop;
			}
			set
			{
				_ScaleCrop = value;
			}
		}

		public CT_VectorVariant HeadingPairs
		{
			get
			{
				return _HeadingPairs;
			}
			set
			{
				_HeadingPairs = value;
			}
		}

		public CT_VectorLpstr TitlesOfParts
		{
			get
			{
				return _TitlesOfParts;
			}
			set
			{
				_TitlesOfParts = value;
			}
		}

		public OoxmlBool LinksUpToDate
		{
			get
			{
				return _LinksUpToDate;
			}
			set
			{
				_LinksUpToDate = value;
			}
		}

		public int CharactersWithSpaces
		{
			get
			{
				return _CharactersWithSpaces;
			}
			set
			{
				_CharactersWithSpaces = value;
			}
		}

		public OoxmlBool SharedDoc
		{
			get
			{
				return _SharedDoc;
			}
			set
			{
				_SharedDoc = value;
			}
		}

		public string HyperlinkBase
		{
			get
			{
				return _HyperlinkBase;
			}
			set
			{
				_HyperlinkBase = value;
			}
		}

		public CT_VectorVariant HLinks
		{
			get
			{
				return _HLinks;
			}
			set
			{
				_HLinks = value;
			}
		}

		public OoxmlBool HyperlinksChanged
		{
			get
			{
				return _HyperlinksChanged;
			}
			set
			{
				_HyperlinksChanged = value;
			}
		}

		public string Application
		{
			get
			{
				return _Application;
			}
			set
			{
				_Application = value;
			}
		}

		public string AppVersion
		{
			get
			{
				return _AppVersion;
			}
			set
			{
				_AppVersion = value;
			}
		}

		public int DocSecurity
		{
			get
			{
				return _DocSecurity;
			}
			set
			{
				_DocSecurity = value;
			}
		}

		public static string HeadingPairsElementName => "HeadingPairs";

		public static string TitlesOfPartsElementName => "TitlesOfParts";

		public static string HLinksElementName => "HLinks";

		public static string PagesElementName => "Pages";

		public static string WordsElementName => "Words";

		public static string CharactersElementName => "Characters";

		public static string LinesElementName => "Lines";

		public static string ParagraphsElementName => "Paragraphs";

		public static string SlidesElementName => "Slides";

		public static string NotesElementName => "Notes";

		public static string TotalTimeElementName => "TotalTime";

		public static string HiddenSlidesElementName => "HiddenSlides";

		public static string MMClipsElementName => "MMClips";

		public static string ScaleCropElementName => "ScaleCrop";

		public static string LinksUpToDateElementName => "LinksUpToDate";

		public static string CharactersWithSpacesElementName => "CharactersWithSpaces";

		public static string SharedDocElementName => "SharedDoc";

		public static string HyperlinksChangedElementName => "HyperlinksChanged";

		public static string DocSecurityElementName => "DocSecurity";

		public static string TemplateElementName => "Template";

		public static string ManagerElementName => "Manager";

		public static string CompanyElementName => "Company";

		public static string PresentationFormatElementName => "PresentationFormat";

		public static string HyperlinkBaseElementName => "HyperlinkBase";

		public static string ApplicationElementName => "Application";

		public static string AppVersionElementName => "AppVersion";

		protected override void InitAttributes()
		{
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
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties");
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
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			Write_Template(s, depth, namespaces);
			Write_Manager(s, depth, namespaces);
			Write_Company(s, depth, namespaces);
			Write_Pages(s, depth, namespaces);
			Write_Words(s, depth, namespaces);
			Write_Characters(s, depth, namespaces);
			Write_PresentationFormat(s, depth, namespaces);
			Write_Lines(s, depth, namespaces);
			Write_Paragraphs(s, depth, namespaces);
			Write_Slides(s, depth, namespaces);
			Write_Notes(s, depth, namespaces);
			Write_TotalTime(s, depth, namespaces);
			Write_HiddenSlides(s, depth, namespaces);
			Write_MMClips(s, depth, namespaces);
			Write_ScaleCrop(s, depth, namespaces);
			Write_HeadingPairs(s, depth, namespaces);
			Write_TitlesOfParts(s, depth, namespaces);
			Write_LinksUpToDate(s, depth, namespaces);
			Write_CharactersWithSpaces(s, depth, namespaces);
			Write_SharedDoc(s, depth, namespaces);
			Write_HyperlinkBase(s, depth, namespaces);
			Write_HLinks(s, depth, namespaces);
			Write_HyperlinksChanged(s, depth, namespaces);
			Write_Application(s, depth, namespaces);
			Write_AppVersion(s, depth, namespaces);
			Write_DocSecurity(s, depth, namespaces);
		}

		public void Write_HeadingPairs(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_HeadingPairs != null)
			{
				_HeadingPairs.Write(s, "HeadingPairs", depth + 1, namespaces);
			}
		}

		public void Write_TitlesOfParts(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_TitlesOfParts != null)
			{
				_TitlesOfParts.Write(s, "TitlesOfParts", depth + 1, namespaces);
			}
		}

		public void Write_HLinks(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_HLinks != null)
			{
				_HLinks.Write(s, "HLinks", depth + 1, namespaces);
			}
		}

		public void Write_Pages(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			OoxmlComplexType.WriteRawTag(s, depth, namespaces, "Pages", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", _Pages);
		}

		public void Write_Words(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			OoxmlComplexType.WriteRawTag(s, depth, namespaces, "Words", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", _Words);
		}

		public void Write_Characters(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			OoxmlComplexType.WriteRawTag(s, depth, namespaces, "Characters", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", _Characters);
		}

		public void Write_Lines(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			OoxmlComplexType.WriteRawTag(s, depth, namespaces, "Lines", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", _Lines);
		}

		public void Write_Paragraphs(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			OoxmlComplexType.WriteRawTag(s, depth, namespaces, "Paragraphs", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", _Paragraphs);
		}

		public void Write_Slides(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			OoxmlComplexType.WriteRawTag(s, depth, namespaces, "Slides", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", _Slides);
		}

		public void Write_Notes(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			OoxmlComplexType.WriteRawTag(s, depth, namespaces, "Notes", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", _Notes);
		}

		public void Write_TotalTime(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			OoxmlComplexType.WriteRawTag(s, depth, namespaces, "TotalTime", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", _TotalTime);
		}

		public void Write_HiddenSlides(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			OoxmlComplexType.WriteRawTag(s, depth, namespaces, "HiddenSlides", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", _HiddenSlides);
		}

		public void Write_MMClips(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			OoxmlComplexType.WriteRawTag(s, depth, namespaces, "MMClips", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", _MMClips);
		}

		public void Write_ScaleCrop(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			OoxmlComplexType.WriteRawTag(s, depth, namespaces, "ScaleCrop", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", _ScaleCrop);
		}

		public void Write_LinksUpToDate(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			OoxmlComplexType.WriteRawTag(s, depth, namespaces, "LinksUpToDate", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", _LinksUpToDate);
		}

		public void Write_CharactersWithSpaces(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			OoxmlComplexType.WriteRawTag(s, depth, namespaces, "CharactersWithSpaces", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", _CharactersWithSpaces);
		}

		public void Write_SharedDoc(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			OoxmlComplexType.WriteRawTag(s, depth, namespaces, "SharedDoc", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", _SharedDoc);
		}

		public void Write_HyperlinksChanged(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			OoxmlComplexType.WriteRawTag(s, depth, namespaces, "HyperlinksChanged", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", _HyperlinksChanged);
		}

		public void Write_DocSecurity(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			OoxmlComplexType.WriteRawTag(s, depth, namespaces, "DocSecurity", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", _DocSecurity);
		}

		public void Write_Template(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_Template != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "Template", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", _Template);
			}
		}

		public void Write_Manager(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_Manager != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "Manager", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", _Manager);
			}
		}

		public void Write_Company(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_Company != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "Company", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", _Company);
			}
		}

		public void Write_PresentationFormat(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_PresentationFormat != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "PresentationFormat", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", _PresentationFormat);
			}
		}

		public void Write_HyperlinkBase(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_HyperlinkBase != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "HyperlinkBase", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", _HyperlinkBase);
			}
		}

		public void Write_Application(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_Application != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "Application", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", _Application);
			}
		}

		public void Write_AppVersion(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_AppVersion != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "AppVersion", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", _AppVersion);
			}
		}
	}
}
