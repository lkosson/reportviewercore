using Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.dc.elements.x1_1;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.package.x2006.metadata.core_properties
{
	internal class CT_CoreProperties : OoxmlComplexType, IOoxmlComplexType
	{
		private string _category;

		private string _contentStatus;

		private string _contentType;

		private SimpleLiteral _creator;

		private SimpleLiteral _description;

		private SimpleLiteral _identifier;

		private string _keywords;

		private SimpleLiteral _language;

		private string _lastModifiedBy;

		private string _revision;

		private SimpleLiteral _subject;

		private SimpleLiteral _title;

		private string _version;

		public string Category
		{
			get
			{
				return _category;
			}
			set
			{
				_category = value;
			}
		}

		public string ContentStatus
		{
			get
			{
				return _contentStatus;
			}
			set
			{
				_contentStatus = value;
			}
		}

		public string ContentType
		{
			get
			{
				return _contentType;
			}
			set
			{
				_contentType = value;
			}
		}

		public SimpleLiteral Creator
		{
			get
			{
				return _creator;
			}
			set
			{
				_creator = value;
			}
		}

		public SimpleLiteral Description
		{
			get
			{
				return _description;
			}
			set
			{
				_description = value;
			}
		}

		public SimpleLiteral Identifier
		{
			get
			{
				return _identifier;
			}
			set
			{
				_identifier = value;
			}
		}

		public string Keywords
		{
			get
			{
				return _keywords;
			}
			set
			{
				_keywords = value;
			}
		}

		public SimpleLiteral Language
		{
			get
			{
				return _language;
			}
			set
			{
				_language = value;
			}
		}

		public string LastModifiedBy
		{
			get
			{
				return _lastModifiedBy;
			}
			set
			{
				_lastModifiedBy = value;
			}
		}

		public string Revision
		{
			get
			{
				return _revision;
			}
			set
			{
				_revision = value;
			}
		}

		public SimpleLiteral Subject
		{
			get
			{
				return _subject;
			}
			set
			{
				_subject = value;
			}
		}

		public SimpleLiteral Title
		{
			get
			{
				return _title;
			}
			set
			{
				_title = value;
			}
		}

		public string Version
		{
			get
			{
				return _version;
			}
			set
			{
				_version = value;
			}
		}

		public static string CreatorElementName => "creator";

		public static string DescriptionElementName => "description";

		public static string IdentifierElementName => "identifier";

		public static string LanguageElementName => "language";

		public static string SubjectElementName => "subject";

		public static string TitleElementName => "title";

		public static string CategoryElementName => "category";

		public static string ContentStatusElementName => "contentStatus";

		public static string ContentTypeElementName => "contentType";

		public static string KeywordsElementName => "keywords";

		public static string LastModifiedByElementName => "lastModifiedBy";

		public static string RevisionElementName => "revision";

		public static string VersionElementName => "version";

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
			WriteOpenTag(s, tagName, "cp", namespaces);
		}

		public override void WriteCloseTag(TextWriter s, string tagName)
		{
			s.Write("</cp:");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
		}

		public override void WriteElements(TextWriter s)
		{
			Write_category(s);
			Write_contentStatus(s);
			Write_contentType(s);
			Write_creator(s);
			Write_description(s);
			Write_identifier(s);
			Write_keywords(s);
			Write_language(s);
			Write_lastModifiedBy(s);
			Write_revision(s);
			Write_subject(s);
			Write_title(s);
			Write_version(s);
		}

		public void Write_creator(TextWriter s)
		{
			if (_creator != null)
			{
				_creator.Write(s, "creator");
			}
		}

		public void Write_description(TextWriter s)
		{
			if (_description != null)
			{
				_description.Write(s, "description");
			}
		}

		public void Write_identifier(TextWriter s)
		{
			if (_identifier != null)
			{
				_identifier.Write(s, "identifier");
			}
		}

		public void Write_language(TextWriter s)
		{
			if (_language != null)
			{
				_language.Write(s, "language");
			}
		}

		public void Write_subject(TextWriter s)
		{
			if (_subject != null)
			{
				_subject.Write(s, "subject");
			}
		}

		public void Write_title(TextWriter s)
		{
			if (_title != null)
			{
				_title.Write(s, "title");
			}
		}

		public void Write_category(TextWriter s)
		{
			if (_category != null)
			{
				OoxmlComplexType.WriteRawTag(s, "category", "cp", _category);
			}
		}

		public void Write_contentStatus(TextWriter s)
		{
			if (_contentStatus != null)
			{
				OoxmlComplexType.WriteRawTag(s, "contentStatus", "cp", _contentStatus);
			}
		}

		public void Write_contentType(TextWriter s)
		{
			if (_contentType != null)
			{
				OoxmlComplexType.WriteRawTag(s, "contentType", "cp", _contentType);
			}
		}

		public void Write_keywords(TextWriter s)
		{
			if (_keywords != null)
			{
				OoxmlComplexType.WriteRawTag(s, "keywords", "cp", _keywords);
			}
		}

		public void Write_lastModifiedBy(TextWriter s)
		{
			if (_lastModifiedBy != null)
			{
				OoxmlComplexType.WriteRawTag(s, "lastModifiedBy", "cp", _lastModifiedBy);
			}
		}

		public void Write_revision(TextWriter s)
		{
			if (_revision != null)
			{
				OoxmlComplexType.WriteRawTag(s, "revision", "cp", _revision);
			}
		}

		public void Write_version(TextWriter s)
		{
			if (_version != null)
			{
				OoxmlComplexType.WriteRawTag(s, "version", "cp", _version);
			}
		}
	}
}
