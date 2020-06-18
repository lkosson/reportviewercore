using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models.Relationships;
using Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser;
using Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Packaging;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal sealed class OpenXmlDocumentModel
	{
		private sealed class PartInfo
		{
			public OoxmlPart Part
			{
				get;
				set;
			}

			public string PartName
			{
				get;
				set;
			}

			public Stream Stream
			{
				get;
				set;
			}

			public InterleavingWriter Writer
			{
				get;
				set;
			}

			public TableContext TableContext
			{
				get;
				set;
			}

			public long StartingLocation
			{
				get;
				set;
			}
		}

		private struct TemporaryHeaderFooterReferences : OpenXmlSectionPropertiesModel.IHeaderFooterReferences
		{
			public string Header
			{
				get;
				set;
			}

			public string Footer
			{
				get;
				set;
			}

			public string FirstPageHeader
			{
				get;
				set;
			}

			public string FirstPageFooter
			{
				get;
				set;
			}

			public HeaderFooterReferences Store(int index, long location)
			{
				return new HeaderFooterReferences(index, location, Footer, Header, FirstPageHeader, FirstPageFooter);
			}
		}

		private Package _zipPackage;

		private PartManager _manager;

		private PartInfo _currentPart;

		private PartInfo _documentPart;

		private Stack<OoxmlComplexType> _tags;

		private OpenXmlListNumberingManager _listManager;

		private OpenXmlSectionPropertiesModel _sectionProperties;

		private WordOpenXmlWriter.CreateXmlStream _createXmlStream;

		private int _headerAndFooterIndex;

		private bool _firstSection = true;

		private TemporaryHeaderFooterReferences _currentHeaderFooterReferences;

		internal Package ZipPackage => _zipPackage;

		internal OoxmlPart Part => _currentPart.Part;

		internal OpenXmlListNumberingManager ListManager => _listManager;

		internal OpenXmlSectionPropertiesModel SectionProperties => _sectionProperties;

		public TableContext TableContext => _currentPart.TableContext;

		public bool SectionHasTitlePage
		{
			set
			{
				_sectionProperties.HasTitlePage = value;
			}
		}

		public OpenXmlDocumentModel(Stream output, WordOpenXmlWriter.CreateXmlStream createXmlStream, ScalabilityCache scalabilityCache)
		{
			_createXmlStream = createXmlStream;
			_zipPackage = Package.Open(output, FileMode.Create);
			_documentPart = (_currentPart = new PartInfo());
			_currentPart.Part = new DocumentPart();
			_manager = new PartManager(ZipPackage);
			Relationship relationship = _manager.AddStreamingRootPartToTree("application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument", "word/document.xml");
			WriteStylesheet();
			WriteSettings();
			_currentPart.PartName = relationship.RelatedPart;
			_currentPart.Stream = _createXmlStream("document");
			_currentPart.Writer = new InterleavingWriter(_currentPart.Stream, scalabilityCache);
			_currentHeaderFooterReferences = default(TemporaryHeaderFooterReferences);
			_tags = new Stack<OoxmlComplexType>();
			_currentPart.Writer.TextWriter.Write(OoxmlPart.XmlDeclaration);
			CT_Document cT_Document = new CT_Document();
			cT_Document.WriteOpenTag(_currentPart.Writer.TextWriter, _currentPart.Part.Tag, _currentPart.Part.Namespaces);
			_tags.Push(cT_Document);
			CT_Body ctObject = new CT_Body();
			WriteStartTag(ctObject, CT_Document.BodyElementName);
			_currentPart.TableContext = new TableContext(_currentPart.Writer, inHeaderFooter: false);
			_listManager = new OpenXmlListNumberingManager();
			_sectionProperties = new OpenXmlSectionPropertiesModel();
		}

		private CT_OnOff On()
		{
			return new CT_OnOff
			{
				Val_Attr = true
			};
		}

		private void WriteSettings()
		{
			SettingsPart settingsPart = new SettingsPart();
			((CT_Settings)settingsPart.Root).Compat = new CT_Compat
			{
				FootnoteLayoutLikeWW8 = On(),
				ShapeLayoutLikeWW8 = On(),
				AlignTablesRowByRow = On(),
				ForgetLastTabAlignment = On(),
				DoNotUseHTMLParagraphAutoSpacing = On(),
				LayoutRawTableWidth = On(),
				LayoutTableRowsApart = On(),
				UseWord97LineBreakRules = On(),
				DoNotBreakWrappedTables = On(),
				DoNotSnapToGridInCell = On(),
				SelectFldWithFirstOrLastChar = On(),
				DoNotWrapTextWithPunct = On(),
				DoNotUseEastAsianBreakRules = On(),
				UseWord2002TableStyleRules = On(),
				GrowAutofit = On(),
				UseNormalStyleForList = On(),
				DoNotUseIndentAsNumberingTabStop = On(),
				UseAltKinsokuLineBreakRules = On(),
				AllowSpaceOfSameStyleInTable = On(),
				DoNotSuppressIndentation = On(),
				DoNotAutofitConstrainedTables = On(),
				AutofitToFirstFixedWidthCell = On(),
				UnderlineTabInNumList = On(),
				DisplayHangulFixedWidth = On(),
				SplitPgBreakAndParaMark = On(),
				DoNotVertAlignCellWithSp = On(),
				DoNotBreakConstrainedForcedTable = On(),
				DoNotVertAlignInTxbx = On(),
				UseAnsiKerningPairs = On(),
				CachedColBalance = On()
			};
			_manager.WriteStaticPart(settingsPart, "application/vnd.openxmlformats-officedocument.wordprocessingml.settings+xml", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/settings", "word/settings.xml", _manager.GetRootPart());
		}

		private void WriteNumberingPart()
		{
			_manager.WriteStaticPart(_listManager.CreateNumberingPart(), "application/vnd.openxmlformats-officedocument.wordprocessingml.numbering+xml", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/numbering", "word/numbering.xml", _manager.GetRootPart());
		}

		private void WriteStylesheet()
		{
			CT_RPr rPr = new CT_RPr
			{
				RFonts = new CT_Fonts
				{
					Ascii_Attr = "Times New Roman",
					EastAsia_Attr = "Times New Roman",
					HAnsi_Attr = "Times New Roman",
					Cs_Attr = "Times New Roman"
				}
			};
			CT_DocDefaults docDefaults = new CT_DocDefaults
			{
				RPrDefault = new CT_RPrDefault
				{
					RPr = rPr
				}
			};
			CT_Style item = new CT_Style
			{
				Name = new CT_String
				{
					Val_Attr = "EmptyCellLayoutStyle"
				},
				BasedOn = new CT_String
				{
					Val_Attr = "Normal"
				},
				RPr = new CT_RPr
				{
					Sz = new CT_HpsMeasure
					{
						Val_Attr = 2.ToString(CultureInfo.InvariantCulture)
					}
				}
			};
			StylesPart stylesPart = new StylesPart();
			((CT_Styles)stylesPart.Root).DocDefaults = docDefaults;
			((CT_Styles)stylesPart.Root).Style.Add(item);
			_manager.WriteStaticPart(stylesPart, "application/vnd.openxmlformats-officedocument.wordprocessingml.styles+xml", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles", "word/styles.xml", _manager.GetRootPart());
		}

		private void StartHeaderOrFooter(OoxmlPart part, Relationship relationship)
		{
			_currentPart = new PartInfo();
			_currentPart.Part = part;
			_currentPart.PartName = relationship.RelatedPart;
			_currentPart.Stream = _createXmlStream(_currentPart.PartName);
			_currentPart.Writer = new InterleavingWriter(_currentPart.Stream);
			_currentPart.Writer.TextWriter.Write(OoxmlPart.XmlDeclaration);
			CT_HdrFtr cT_HdrFtr = new CT_HdrFtr();
			cT_HdrFtr.WriteOpenTag(_currentPart.Writer.TextWriter, _currentPart.Part.Tag, _currentPart.Part.Namespaces);
			_tags.Push(cT_HdrFtr);
			_currentPart.TableContext = new TableContext(_currentPart.Writer, inHeaderFooter: true);
			_currentPart.StartingLocation = _currentPart.Writer.Location;
		}

		private void FinishHeaderOrFooter()
		{
			if (_currentPart.Writer.Location == _currentPart.StartingLocation)
			{
				OpenXmlParagraphModel.WriteInvisibleParagraph(_currentPart.Writer.TextWriter);
			}
			WriteCloseTag(_currentPart.Part.Tag);
			Stream stream = ZipPackage.GetPart(new Uri(PartManager.CleanName(_currentPart.PartName), UriKind.Relative)).GetStream();
			_currentPart.Writer.Interleave(stream, WriteInterleaverToHeaderOrFooter);
			_currentPart.Stream.Dispose();
			_currentPart = _documentPart;
		}

		internal Relationship WriteImageData(byte[] imgBuf, ImageHash hash, string extension)
		{
			return _manager.AddImageToTree(new MemoryStream(imgBuf), hash, extension, "http://schemas.openxmlformats.org/officeDocument/2006/relationships/image", "word/media/img{0}.{{0}}", _currentPart.PartName);
		}

		internal void WriteDocumentProperties(string title, string author, string description)
		{
			OpenXmlDocumentPropertiesModel openXmlDocumentPropertiesModel = new OpenXmlDocumentPropertiesModel(author, title, description);
			_manager.WriteStaticRootPart(openXmlDocumentPropertiesModel.PropertiesPart, "application/vnd.openxmlformats-package.core-properties+xml", "http://schemas.openxmlformats.org/package/2006/relationships/metadata/core-properties", "docProps/core.xml");
		}

		internal void WriteParagraph(OpenXmlParagraphModel paragraph)
		{
			paragraph.Write(_currentPart.Writer.TextWriter);
		}

		internal void WriteEmptyParagraph()
		{
			OpenXmlParagraphModel.WriteEmptyParagraph(_currentPart.Writer.TextWriter);
		}

		internal void WritePageBreak()
		{
			OpenXmlParagraphModel.WritePageBreakParagraph(_currentPart.Writer.TextWriter);
		}

		public void WriteSectionBreak()
		{
			_currentPart.Writer.WriteInterleaver(_currentHeaderFooterReferences.Store);
			_currentHeaderFooterReferences = default(TemporaryHeaderFooterReferences);
			if (_firstSection)
			{
				_sectionProperties.Continuous = true;
				_firstSection = false;
			}
		}

		internal void Save()
		{
			_sectionProperties.WriteToBody(_currentPart.Writer.TextWriter, _currentHeaderFooterReferences);
			_firstSection = true;
			_sectionProperties.Continuous = false;
			WriteCloseTag(CT_Document.BodyElementName);
			WriteCloseTag(_currentPart.Part.Tag);
			Stream stream = ZipPackage.GetPart(new Uri(PartManager.CleanName(_currentPart.PartName), UriKind.Relative)).GetStream();
			_currentPart.Writer.Interleave(stream, WriteInterleaverToDocument);
			WriteNumberingPart();
			_manager.Write();
		}

		public void WriteStartTag<T>(T ctObject, string elementName) where T : OoxmlComplexType
		{
			_tags.Push(ctObject);
			ctObject.WriteOpenTag(_currentPart.Writer.TextWriter, elementName, null);
		}

		public void WriteCloseTag(string elementName)
		{
			_tags.Pop().WriteCloseTag(_currentPart.Writer.TextWriter, elementName);
		}

		public void StartHeader()
		{
			string locationHint = string.Format(CultureInfo.InvariantCulture, "word/header{0}.xml", _headerAndFooterIndex++);
			Relationship relationship = _manager.AddStreamingPartToTree("application/vnd.openxmlformats-officedocument.wordprocessingml.header+xml", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/header", locationHint, _manager.GetRootPart());
			_currentHeaderFooterReferences.Header = relationship.RelationshipId;
			StartHeaderOrFooter(new HdrPart(), relationship);
		}

		public void StartFirstPageHeader()
		{
			string locationHint = string.Format(CultureInfo.InvariantCulture, "word/header{0}.xml", _headerAndFooterIndex++);
			Relationship relationship = _manager.AddStreamingPartToTree("application/vnd.openxmlformats-officedocument.wordprocessingml.header+xml", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/header", locationHint, _manager.GetRootPart());
			_currentHeaderFooterReferences.FirstPageHeader = relationship.RelationshipId;
			StartHeaderOrFooter(new HdrPart(), relationship);
		}

		public void StartFooter()
		{
			string locationHint = string.Format(CultureInfo.InvariantCulture, "word/footer{0}.xml", _headerAndFooterIndex++);
			Relationship relationship = _manager.AddStreamingPartToTree("application/vnd.openxmlformats-officedocument.wordprocessingml.footer+xml", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/footer", locationHint, _manager.GetRootPart());
			_currentHeaderFooterReferences.Footer = relationship.RelationshipId;
			StartHeaderOrFooter(new FtrPart(), relationship);
		}

		public void StartFirstPageFooter()
		{
			string locationHint = string.Format(CultureInfo.InvariantCulture, "word/footer{0}.xml", _headerAndFooterIndex++);
			Relationship relationship = _manager.AddStreamingPartToTree("application/vnd.openxmlformats-officedocument.wordprocessingml.footer+xml", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/footer", locationHint, _manager.GetRootPart());
			_currentHeaderFooterReferences.FirstPageFooter = relationship.RelationshipId;
			StartHeaderOrFooter(new FtrPart(), relationship);
		}

		public void FinishHeader()
		{
			FinishHeaderOrFooter();
		}

		public void FinishFooter()
		{
			FinishHeaderOrFooter();
		}

		private void WriteInterleaverToHeaderOrFooter(IInterleave interleaver, TextWriter output)
		{
			interleaver.Write(output);
		}

		private void WriteInterleaverToDocument(IInterleave interleaver, TextWriter output)
		{
			HeaderFooterReferences headerFooterReferences = interleaver as HeaderFooterReferences;
			if (headerFooterReferences != null)
			{
				_sectionProperties.SetHeaderFooterReferences(headerFooterReferences);
				CT_P cT_P = new CT_P();
				cT_P.PPr = new CT_PPr
				{
					SectPr = _sectionProperties.CtSectPr
				};
				cT_P.Write(output, CT_Body.PElementName);
				_sectionProperties.ResetHeadersAndFooters();
				if (_firstSection)
				{
					_sectionProperties.Continuous = true;
					_firstSection = false;
				}
			}
			else
			{
				interleaver.Write(output);
			}
		}
	}
}
