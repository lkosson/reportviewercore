using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_Settings : OoxmlComplexType, IOoxmlComplexType
	{
		private CT_OnOff _removePersonalInformation;

		private CT_OnOff _removeDateAndTime;

		private CT_OnOff _doNotDisplayPageBoundaries;

		private CT_OnOff _displayBackgroundShape;

		private CT_OnOff _printPostScriptOverText;

		private CT_OnOff _printFractionalCharacterWidth;

		private CT_OnOff _printFormsData;

		private CT_OnOff _embedTrueTypeFonts;

		private CT_OnOff _embedSystemFonts;

		private CT_OnOff _saveSubsetFonts;

		private CT_OnOff _saveFormsData;

		private CT_OnOff _mirrorMargins;

		private CT_OnOff _alignBordersAndEdges;

		private CT_OnOff _bordersDoNotSurroundHeader;

		private CT_OnOff _bordersDoNotSurroundFooter;

		private CT_OnOff _gutterAtTop;

		private CT_OnOff _hideSpellingErrors;

		private CT_OnOff _hideGrammaticalErrors;

		private CT_OnOff _formsDesign;

		private CT_Rel _attachedTemplate;

		private CT_OnOff _linkStyles;

		private CT_OnOff _trackRevisions;

		private CT_OnOff _doNotTrackMoves;

		private CT_OnOff _doNotTrackFormatting;

		private CT_OnOff _autoFormatOverride;

		private CT_OnOff _styleLockTheme;

		private CT_OnOff _styleLockQFSet;

		private CT_OnOff _autoHyphenation;

		private CT_DecimalNumber _consecutiveHyphenLimit;

		private CT_OnOff _doNotHyphenateCaps;

		private CT_OnOff _showEnvelope;

		private CT_String _clickAndTypeStyle;

		private CT_String _defaultTableStyle;

		private CT_OnOff _evenAndOddHeaders;

		private CT_OnOff _bookFoldRevPrinting;

		private CT_OnOff _bookFoldPrinting;

		private CT_DecimalNumber _bookFoldPrintingSheets;

		private CT_DecimalNumber _displayHorizontalDrawingGridEvery;

		private CT_DecimalNumber _displayVerticalDrawingGridEvery;

		private CT_OnOff _doNotUseMarginsForDrawingGridOrigin;

		private CT_OnOff _doNotShadeFormData;

		private CT_OnOff _noPunctuationKerning;

		private CT_OnOff _printTwoOnOne;

		private CT_OnOff _strictFirstAndLastChars;

		private CT_OnOff _savePreviewPicture;

		private CT_OnOff _doNotValidateAgainstSchema;

		private CT_OnOff _saveInvalidXml;

		private CT_OnOff _ignoreMixedContent;

		private CT_OnOff _alwaysShowPlaceholderText;

		private CT_OnOff _doNotDemarcateInvalidXml;

		private CT_OnOff _saveXmlDataOnly;

		private CT_OnOff _useXSLTWhenSaving;

		private CT_OnOff _showXMLTags;

		private CT_OnOff _alwaysMergeEmptyNamespace;

		private CT_OnOff _updateFields;

		private CT_Compat _compat;

		private CT_OnOff _doNotIncludeSubdocsInStats;

		private CT_OnOff _doNotAutoCompressPictures;

		private CT_OnOff _doNotEmbedSmartTags;

		private CT_String _decimalSymbol;

		private CT_String _listSeparator;

		private List<CT_String> _attachedSchema;

		public CT_OnOff RemovePersonalInformation
		{
			get
			{
				return _removePersonalInformation;
			}
			set
			{
				_removePersonalInformation = value;
			}
		}

		public CT_OnOff RemoveDateAndTime
		{
			get
			{
				return _removeDateAndTime;
			}
			set
			{
				_removeDateAndTime = value;
			}
		}

		public CT_OnOff DoNotDisplayPageBoundaries
		{
			get
			{
				return _doNotDisplayPageBoundaries;
			}
			set
			{
				_doNotDisplayPageBoundaries = value;
			}
		}

		public CT_OnOff DisplayBackgroundShape
		{
			get
			{
				return _displayBackgroundShape;
			}
			set
			{
				_displayBackgroundShape = value;
			}
		}

		public CT_OnOff PrintPostScriptOverText
		{
			get
			{
				return _printPostScriptOverText;
			}
			set
			{
				_printPostScriptOverText = value;
			}
		}

		public CT_OnOff PrintFractionalCharacterWidth
		{
			get
			{
				return _printFractionalCharacterWidth;
			}
			set
			{
				_printFractionalCharacterWidth = value;
			}
		}

		public CT_OnOff PrintFormsData
		{
			get
			{
				return _printFormsData;
			}
			set
			{
				_printFormsData = value;
			}
		}

		public CT_OnOff EmbedTrueTypeFonts
		{
			get
			{
				return _embedTrueTypeFonts;
			}
			set
			{
				_embedTrueTypeFonts = value;
			}
		}

		public CT_OnOff EmbedSystemFonts
		{
			get
			{
				return _embedSystemFonts;
			}
			set
			{
				_embedSystemFonts = value;
			}
		}

		public CT_OnOff SaveSubsetFonts
		{
			get
			{
				return _saveSubsetFonts;
			}
			set
			{
				_saveSubsetFonts = value;
			}
		}

		public CT_OnOff SaveFormsData
		{
			get
			{
				return _saveFormsData;
			}
			set
			{
				_saveFormsData = value;
			}
		}

		public CT_OnOff MirrorMargins
		{
			get
			{
				return _mirrorMargins;
			}
			set
			{
				_mirrorMargins = value;
			}
		}

		public CT_OnOff AlignBordersAndEdges
		{
			get
			{
				return _alignBordersAndEdges;
			}
			set
			{
				_alignBordersAndEdges = value;
			}
		}

		public CT_OnOff BordersDoNotSurroundHeader
		{
			get
			{
				return _bordersDoNotSurroundHeader;
			}
			set
			{
				_bordersDoNotSurroundHeader = value;
			}
		}

		public CT_OnOff BordersDoNotSurroundFooter
		{
			get
			{
				return _bordersDoNotSurroundFooter;
			}
			set
			{
				_bordersDoNotSurroundFooter = value;
			}
		}

		public CT_OnOff GutterAtTop
		{
			get
			{
				return _gutterAtTop;
			}
			set
			{
				_gutterAtTop = value;
			}
		}

		public CT_OnOff HideSpellingErrors
		{
			get
			{
				return _hideSpellingErrors;
			}
			set
			{
				_hideSpellingErrors = value;
			}
		}

		public CT_OnOff HideGrammaticalErrors
		{
			get
			{
				return _hideGrammaticalErrors;
			}
			set
			{
				_hideGrammaticalErrors = value;
			}
		}

		public CT_OnOff FormsDesign
		{
			get
			{
				return _formsDesign;
			}
			set
			{
				_formsDesign = value;
			}
		}

		public CT_Rel AttachedTemplate
		{
			get
			{
				return _attachedTemplate;
			}
			set
			{
				_attachedTemplate = value;
			}
		}

		public CT_OnOff LinkStyles
		{
			get
			{
				return _linkStyles;
			}
			set
			{
				_linkStyles = value;
			}
		}

		public CT_OnOff TrackRevisions
		{
			get
			{
				return _trackRevisions;
			}
			set
			{
				_trackRevisions = value;
			}
		}

		public CT_OnOff DoNotTrackMoves
		{
			get
			{
				return _doNotTrackMoves;
			}
			set
			{
				_doNotTrackMoves = value;
			}
		}

		public CT_OnOff DoNotTrackFormatting
		{
			get
			{
				return _doNotTrackFormatting;
			}
			set
			{
				_doNotTrackFormatting = value;
			}
		}

		public CT_OnOff AutoFormatOverride
		{
			get
			{
				return _autoFormatOverride;
			}
			set
			{
				_autoFormatOverride = value;
			}
		}

		public CT_OnOff StyleLockTheme
		{
			get
			{
				return _styleLockTheme;
			}
			set
			{
				_styleLockTheme = value;
			}
		}

		public CT_OnOff StyleLockQFSet
		{
			get
			{
				return _styleLockQFSet;
			}
			set
			{
				_styleLockQFSet = value;
			}
		}

		public CT_OnOff AutoHyphenation
		{
			get
			{
				return _autoHyphenation;
			}
			set
			{
				_autoHyphenation = value;
			}
		}

		public CT_DecimalNumber ConsecutiveHyphenLimit
		{
			get
			{
				return _consecutiveHyphenLimit;
			}
			set
			{
				_consecutiveHyphenLimit = value;
			}
		}

		public CT_OnOff DoNotHyphenateCaps
		{
			get
			{
				return _doNotHyphenateCaps;
			}
			set
			{
				_doNotHyphenateCaps = value;
			}
		}

		public CT_OnOff ShowEnvelope
		{
			get
			{
				return _showEnvelope;
			}
			set
			{
				_showEnvelope = value;
			}
		}

		public CT_String ClickAndTypeStyle
		{
			get
			{
				return _clickAndTypeStyle;
			}
			set
			{
				_clickAndTypeStyle = value;
			}
		}

		public CT_String DefaultTableStyle
		{
			get
			{
				return _defaultTableStyle;
			}
			set
			{
				_defaultTableStyle = value;
			}
		}

		public CT_OnOff EvenAndOddHeaders
		{
			get
			{
				return _evenAndOddHeaders;
			}
			set
			{
				_evenAndOddHeaders = value;
			}
		}

		public CT_OnOff BookFoldRevPrinting
		{
			get
			{
				return _bookFoldRevPrinting;
			}
			set
			{
				_bookFoldRevPrinting = value;
			}
		}

		public CT_OnOff BookFoldPrinting
		{
			get
			{
				return _bookFoldPrinting;
			}
			set
			{
				_bookFoldPrinting = value;
			}
		}

		public CT_DecimalNumber BookFoldPrintingSheets
		{
			get
			{
				return _bookFoldPrintingSheets;
			}
			set
			{
				_bookFoldPrintingSheets = value;
			}
		}

		public CT_DecimalNumber DisplayHorizontalDrawingGridEvery
		{
			get
			{
				return _displayHorizontalDrawingGridEvery;
			}
			set
			{
				_displayHorizontalDrawingGridEvery = value;
			}
		}

		public CT_DecimalNumber DisplayVerticalDrawingGridEvery
		{
			get
			{
				return _displayVerticalDrawingGridEvery;
			}
			set
			{
				_displayVerticalDrawingGridEvery = value;
			}
		}

		public CT_OnOff DoNotUseMarginsForDrawingGridOrigin
		{
			get
			{
				return _doNotUseMarginsForDrawingGridOrigin;
			}
			set
			{
				_doNotUseMarginsForDrawingGridOrigin = value;
			}
		}

		public CT_OnOff DoNotShadeFormData
		{
			get
			{
				return _doNotShadeFormData;
			}
			set
			{
				_doNotShadeFormData = value;
			}
		}

		public CT_OnOff NoPunctuationKerning
		{
			get
			{
				return _noPunctuationKerning;
			}
			set
			{
				_noPunctuationKerning = value;
			}
		}

		public CT_OnOff PrintTwoOnOne
		{
			get
			{
				return _printTwoOnOne;
			}
			set
			{
				_printTwoOnOne = value;
			}
		}

		public CT_OnOff StrictFirstAndLastChars
		{
			get
			{
				return _strictFirstAndLastChars;
			}
			set
			{
				_strictFirstAndLastChars = value;
			}
		}

		public CT_OnOff SavePreviewPicture
		{
			get
			{
				return _savePreviewPicture;
			}
			set
			{
				_savePreviewPicture = value;
			}
		}

		public CT_OnOff DoNotValidateAgainstSchema
		{
			get
			{
				return _doNotValidateAgainstSchema;
			}
			set
			{
				_doNotValidateAgainstSchema = value;
			}
		}

		public CT_OnOff SaveInvalidXml
		{
			get
			{
				return _saveInvalidXml;
			}
			set
			{
				_saveInvalidXml = value;
			}
		}

		public CT_OnOff IgnoreMixedContent
		{
			get
			{
				return _ignoreMixedContent;
			}
			set
			{
				_ignoreMixedContent = value;
			}
		}

		public CT_OnOff AlwaysShowPlaceholderText
		{
			get
			{
				return _alwaysShowPlaceholderText;
			}
			set
			{
				_alwaysShowPlaceholderText = value;
			}
		}

		public CT_OnOff DoNotDemarcateInvalidXml
		{
			get
			{
				return _doNotDemarcateInvalidXml;
			}
			set
			{
				_doNotDemarcateInvalidXml = value;
			}
		}

		public CT_OnOff SaveXmlDataOnly
		{
			get
			{
				return _saveXmlDataOnly;
			}
			set
			{
				_saveXmlDataOnly = value;
			}
		}

		public CT_OnOff UseXSLTWhenSaving
		{
			get
			{
				return _useXSLTWhenSaving;
			}
			set
			{
				_useXSLTWhenSaving = value;
			}
		}

		public CT_OnOff ShowXMLTags
		{
			get
			{
				return _showXMLTags;
			}
			set
			{
				_showXMLTags = value;
			}
		}

		public CT_OnOff AlwaysMergeEmptyNamespace
		{
			get
			{
				return _alwaysMergeEmptyNamespace;
			}
			set
			{
				_alwaysMergeEmptyNamespace = value;
			}
		}

		public CT_OnOff UpdateFields
		{
			get
			{
				return _updateFields;
			}
			set
			{
				_updateFields = value;
			}
		}

		public CT_Compat Compat
		{
			get
			{
				return _compat;
			}
			set
			{
				_compat = value;
			}
		}

		public CT_OnOff DoNotIncludeSubdocsInStats
		{
			get
			{
				return _doNotIncludeSubdocsInStats;
			}
			set
			{
				_doNotIncludeSubdocsInStats = value;
			}
		}

		public CT_OnOff DoNotAutoCompressPictures
		{
			get
			{
				return _doNotAutoCompressPictures;
			}
			set
			{
				_doNotAutoCompressPictures = value;
			}
		}

		public CT_OnOff DoNotEmbedSmartTags
		{
			get
			{
				return _doNotEmbedSmartTags;
			}
			set
			{
				_doNotEmbedSmartTags = value;
			}
		}

		public CT_String DecimalSymbol
		{
			get
			{
				return _decimalSymbol;
			}
			set
			{
				_decimalSymbol = value;
			}
		}

		public CT_String ListSeparator
		{
			get
			{
				return _listSeparator;
			}
			set
			{
				_listSeparator = value;
			}
		}

		public List<CT_String> AttachedSchema
		{
			get
			{
				return _attachedSchema;
			}
			set
			{
				_attachedSchema = value;
			}
		}

		public static string RemovePersonalInformationElementName => "removePersonalInformation";

		public static string RemoveDateAndTimeElementName => "removeDateAndTime";

		public static string DoNotDisplayPageBoundariesElementName => "doNotDisplayPageBoundaries";

		public static string DisplayBackgroundShapeElementName => "displayBackgroundShape";

		public static string PrintPostScriptOverTextElementName => "printPostScriptOverText";

		public static string PrintFractionalCharacterWidthElementName => "printFractionalCharacterWidth";

		public static string PrintFormsDataElementName => "printFormsData";

		public static string EmbedTrueTypeFontsElementName => "embedTrueTypeFonts";

		public static string EmbedSystemFontsElementName => "embedSystemFonts";

		public static string SaveSubsetFontsElementName => "saveSubsetFonts";

		public static string SaveFormsDataElementName => "saveFormsData";

		public static string MirrorMarginsElementName => "mirrorMargins";

		public static string AlignBordersAndEdgesElementName => "alignBordersAndEdges";

		public static string BordersDoNotSurroundHeaderElementName => "bordersDoNotSurroundHeader";

		public static string BordersDoNotSurroundFooterElementName => "bordersDoNotSurroundFooter";

		public static string GutterAtTopElementName => "gutterAtTop";

		public static string HideSpellingErrorsElementName => "hideSpellingErrors";

		public static string HideGrammaticalErrorsElementName => "hideGrammaticalErrors";

		public static string FormsDesignElementName => "formsDesign";

		public static string AttachedTemplateElementName => "attachedTemplate";

		public static string LinkStylesElementName => "linkStyles";

		public static string TrackRevisionsElementName => "trackRevisions";

		public static string DoNotTrackMovesElementName => "doNotTrackMoves";

		public static string DoNotTrackFormattingElementName => "doNotTrackFormatting";

		public static string AutoFormatOverrideElementName => "autoFormatOverride";

		public static string StyleLockThemeElementName => "styleLockTheme";

		public static string StyleLockQFSetElementName => "styleLockQFSet";

		public static string AutoHyphenationElementName => "autoHyphenation";

		public static string ConsecutiveHyphenLimitElementName => "consecutiveHyphenLimit";

		public static string DoNotHyphenateCapsElementName => "doNotHyphenateCaps";

		public static string ShowEnvelopeElementName => "showEnvelope";

		public static string ClickAndTypeStyleElementName => "clickAndTypeStyle";

		public static string DefaultTableStyleElementName => "defaultTableStyle";

		public static string EvenAndOddHeadersElementName => "evenAndOddHeaders";

		public static string BookFoldRevPrintingElementName => "bookFoldRevPrinting";

		public static string BookFoldPrintingElementName => "bookFoldPrinting";

		public static string BookFoldPrintingSheetsElementName => "bookFoldPrintingSheets";

		public static string DisplayHorizontalDrawingGridEveryElementName => "displayHorizontalDrawingGridEvery";

		public static string DisplayVerticalDrawingGridEveryElementName => "displayVerticalDrawingGridEvery";

		public static string DoNotUseMarginsForDrawingGridOriginElementName => "doNotUseMarginsForDrawingGridOrigin";

		public static string DoNotShadeFormDataElementName => "doNotShadeFormData";

		public static string NoPunctuationKerningElementName => "noPunctuationKerning";

		public static string PrintTwoOnOneElementName => "printTwoOnOne";

		public static string StrictFirstAndLastCharsElementName => "strictFirstAndLastChars";

		public static string SavePreviewPictureElementName => "savePreviewPicture";

		public static string DoNotValidateAgainstSchemaElementName => "doNotValidateAgainstSchema";

		public static string SaveInvalidXmlElementName => "saveInvalidXml";

		public static string IgnoreMixedContentElementName => "ignoreMixedContent";

		public static string AlwaysShowPlaceholderTextElementName => "alwaysShowPlaceholderText";

		public static string DoNotDemarcateInvalidXmlElementName => "doNotDemarcateInvalidXml";

		public static string SaveXmlDataOnlyElementName => "saveXmlDataOnly";

		public static string UseXSLTWhenSavingElementName => "useXSLTWhenSaving";

		public static string ShowXMLTagsElementName => "showXMLTags";

		public static string AlwaysMergeEmptyNamespaceElementName => "alwaysMergeEmptyNamespace";

		public static string UpdateFieldsElementName => "updateFields";

		public static string CompatElementName => "compat";

		public static string DoNotIncludeSubdocsInStatsElementName => "doNotIncludeSubdocsInStats";

		public static string DoNotAutoCompressPicturesElementName => "doNotAutoCompressPictures";

		public static string DoNotEmbedSmartTagsElementName => "doNotEmbedSmartTags";

		public static string DecimalSymbolElementName => "decimalSymbol";

		public static string ListSeparatorElementName => "listSeparator";

		public static string AttachedSchemaElementName => "attachedSchema";

		protected override void InitAttributes()
		{
		}

		protected override void InitElements()
		{
		}

		protected override void InitCollections()
		{
			_attachedSchema = new List<CT_String>();
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
			Write_removePersonalInformation(s);
			Write_removeDateAndTime(s);
			Write_doNotDisplayPageBoundaries(s);
			Write_displayBackgroundShape(s);
			Write_printPostScriptOverText(s);
			Write_printFractionalCharacterWidth(s);
			Write_printFormsData(s);
			Write_embedTrueTypeFonts(s);
			Write_embedSystemFonts(s);
			Write_saveSubsetFonts(s);
			Write_saveFormsData(s);
			Write_mirrorMargins(s);
			Write_alignBordersAndEdges(s);
			Write_bordersDoNotSurroundHeader(s);
			Write_bordersDoNotSurroundFooter(s);
			Write_gutterAtTop(s);
			Write_hideSpellingErrors(s);
			Write_hideGrammaticalErrors(s);
			Write_formsDesign(s);
			Write_attachedTemplate(s);
			Write_linkStyles(s);
			Write_trackRevisions(s);
			Write_doNotTrackMoves(s);
			Write_doNotTrackFormatting(s);
			Write_autoFormatOverride(s);
			Write_styleLockTheme(s);
			Write_styleLockQFSet(s);
			Write_autoHyphenation(s);
			Write_consecutiveHyphenLimit(s);
			Write_doNotHyphenateCaps(s);
			Write_showEnvelope(s);
			Write_clickAndTypeStyle(s);
			Write_defaultTableStyle(s);
			Write_evenAndOddHeaders(s);
			Write_bookFoldRevPrinting(s);
			Write_bookFoldPrinting(s);
			Write_bookFoldPrintingSheets(s);
			Write_displayHorizontalDrawingGridEvery(s);
			Write_displayVerticalDrawingGridEvery(s);
			Write_doNotUseMarginsForDrawingGridOrigin(s);
			Write_doNotShadeFormData(s);
			Write_noPunctuationKerning(s);
			Write_printTwoOnOne(s);
			Write_strictFirstAndLastChars(s);
			Write_savePreviewPicture(s);
			Write_doNotValidateAgainstSchema(s);
			Write_saveInvalidXml(s);
			Write_ignoreMixedContent(s);
			Write_alwaysShowPlaceholderText(s);
			Write_doNotDemarcateInvalidXml(s);
			Write_saveXmlDataOnly(s);
			Write_useXSLTWhenSaving(s);
			Write_showXMLTags(s);
			Write_alwaysMergeEmptyNamespace(s);
			Write_updateFields(s);
			Write_compat(s);
			Write_attachedSchema(s);
			Write_doNotIncludeSubdocsInStats(s);
			Write_doNotAutoCompressPictures(s);
			Write_doNotEmbedSmartTags(s);
			Write_decimalSymbol(s);
			Write_listSeparator(s);
		}

		public void Write_removePersonalInformation(TextWriter s)
		{
			if (_removePersonalInformation != null)
			{
				_removePersonalInformation.Write(s, "removePersonalInformation");
			}
		}

		public void Write_removeDateAndTime(TextWriter s)
		{
			if (_removeDateAndTime != null)
			{
				_removeDateAndTime.Write(s, "removeDateAndTime");
			}
		}

		public void Write_doNotDisplayPageBoundaries(TextWriter s)
		{
			if (_doNotDisplayPageBoundaries != null)
			{
				_doNotDisplayPageBoundaries.Write(s, "doNotDisplayPageBoundaries");
			}
		}

		public void Write_displayBackgroundShape(TextWriter s)
		{
			if (_displayBackgroundShape != null)
			{
				_displayBackgroundShape.Write(s, "displayBackgroundShape");
			}
		}

		public void Write_printPostScriptOverText(TextWriter s)
		{
			if (_printPostScriptOverText != null)
			{
				_printPostScriptOverText.Write(s, "printPostScriptOverText");
			}
		}

		public void Write_printFractionalCharacterWidth(TextWriter s)
		{
			if (_printFractionalCharacterWidth != null)
			{
				_printFractionalCharacterWidth.Write(s, "printFractionalCharacterWidth");
			}
		}

		public void Write_printFormsData(TextWriter s)
		{
			if (_printFormsData != null)
			{
				_printFormsData.Write(s, "printFormsData");
			}
		}

		public void Write_embedTrueTypeFonts(TextWriter s)
		{
			if (_embedTrueTypeFonts != null)
			{
				_embedTrueTypeFonts.Write(s, "embedTrueTypeFonts");
			}
		}

		public void Write_embedSystemFonts(TextWriter s)
		{
			if (_embedSystemFonts != null)
			{
				_embedSystemFonts.Write(s, "embedSystemFonts");
			}
		}

		public void Write_saveSubsetFonts(TextWriter s)
		{
			if (_saveSubsetFonts != null)
			{
				_saveSubsetFonts.Write(s, "saveSubsetFonts");
			}
		}

		public void Write_saveFormsData(TextWriter s)
		{
			if (_saveFormsData != null)
			{
				_saveFormsData.Write(s, "saveFormsData");
			}
		}

		public void Write_mirrorMargins(TextWriter s)
		{
			if (_mirrorMargins != null)
			{
				_mirrorMargins.Write(s, "mirrorMargins");
			}
		}

		public void Write_alignBordersAndEdges(TextWriter s)
		{
			if (_alignBordersAndEdges != null)
			{
				_alignBordersAndEdges.Write(s, "alignBordersAndEdges");
			}
		}

		public void Write_bordersDoNotSurroundHeader(TextWriter s)
		{
			if (_bordersDoNotSurroundHeader != null)
			{
				_bordersDoNotSurroundHeader.Write(s, "bordersDoNotSurroundHeader");
			}
		}

		public void Write_bordersDoNotSurroundFooter(TextWriter s)
		{
			if (_bordersDoNotSurroundFooter != null)
			{
				_bordersDoNotSurroundFooter.Write(s, "bordersDoNotSurroundFooter");
			}
		}

		public void Write_gutterAtTop(TextWriter s)
		{
			if (_gutterAtTop != null)
			{
				_gutterAtTop.Write(s, "gutterAtTop");
			}
		}

		public void Write_hideSpellingErrors(TextWriter s)
		{
			if (_hideSpellingErrors != null)
			{
				_hideSpellingErrors.Write(s, "hideSpellingErrors");
			}
		}

		public void Write_hideGrammaticalErrors(TextWriter s)
		{
			if (_hideGrammaticalErrors != null)
			{
				_hideGrammaticalErrors.Write(s, "hideGrammaticalErrors");
			}
		}

		public void Write_formsDesign(TextWriter s)
		{
			if (_formsDesign != null)
			{
				_formsDesign.Write(s, "formsDesign");
			}
		}

		public void Write_attachedTemplate(TextWriter s)
		{
			if (_attachedTemplate != null)
			{
				_attachedTemplate.Write(s, "attachedTemplate");
			}
		}

		public void Write_linkStyles(TextWriter s)
		{
			if (_linkStyles != null)
			{
				_linkStyles.Write(s, "linkStyles");
			}
		}

		public void Write_trackRevisions(TextWriter s)
		{
			if (_trackRevisions != null)
			{
				_trackRevisions.Write(s, "trackRevisions");
			}
		}

		public void Write_doNotTrackMoves(TextWriter s)
		{
			if (_doNotTrackMoves != null)
			{
				_doNotTrackMoves.Write(s, "doNotTrackMoves");
			}
		}

		public void Write_doNotTrackFormatting(TextWriter s)
		{
			if (_doNotTrackFormatting != null)
			{
				_doNotTrackFormatting.Write(s, "doNotTrackFormatting");
			}
		}

		public void Write_autoFormatOverride(TextWriter s)
		{
			if (_autoFormatOverride != null)
			{
				_autoFormatOverride.Write(s, "autoFormatOverride");
			}
		}

		public void Write_styleLockTheme(TextWriter s)
		{
			if (_styleLockTheme != null)
			{
				_styleLockTheme.Write(s, "styleLockTheme");
			}
		}

		public void Write_styleLockQFSet(TextWriter s)
		{
			if (_styleLockQFSet != null)
			{
				_styleLockQFSet.Write(s, "styleLockQFSet");
			}
		}

		public void Write_autoHyphenation(TextWriter s)
		{
			if (_autoHyphenation != null)
			{
				_autoHyphenation.Write(s, "autoHyphenation");
			}
		}

		public void Write_consecutiveHyphenLimit(TextWriter s)
		{
			if (_consecutiveHyphenLimit != null)
			{
				_consecutiveHyphenLimit.Write(s, "consecutiveHyphenLimit");
			}
		}

		public void Write_doNotHyphenateCaps(TextWriter s)
		{
			if (_doNotHyphenateCaps != null)
			{
				_doNotHyphenateCaps.Write(s, "doNotHyphenateCaps");
			}
		}

		public void Write_showEnvelope(TextWriter s)
		{
			if (_showEnvelope != null)
			{
				_showEnvelope.Write(s, "showEnvelope");
			}
		}

		public void Write_clickAndTypeStyle(TextWriter s)
		{
			if (_clickAndTypeStyle != null)
			{
				_clickAndTypeStyle.Write(s, "clickAndTypeStyle");
			}
		}

		public void Write_defaultTableStyle(TextWriter s)
		{
			if (_defaultTableStyle != null)
			{
				_defaultTableStyle.Write(s, "defaultTableStyle");
			}
		}

		public void Write_evenAndOddHeaders(TextWriter s)
		{
			if (_evenAndOddHeaders != null)
			{
				_evenAndOddHeaders.Write(s, "evenAndOddHeaders");
			}
		}

		public void Write_bookFoldRevPrinting(TextWriter s)
		{
			if (_bookFoldRevPrinting != null)
			{
				_bookFoldRevPrinting.Write(s, "bookFoldRevPrinting");
			}
		}

		public void Write_bookFoldPrinting(TextWriter s)
		{
			if (_bookFoldPrinting != null)
			{
				_bookFoldPrinting.Write(s, "bookFoldPrinting");
			}
		}

		public void Write_bookFoldPrintingSheets(TextWriter s)
		{
			if (_bookFoldPrintingSheets != null)
			{
				_bookFoldPrintingSheets.Write(s, "bookFoldPrintingSheets");
			}
		}

		public void Write_displayHorizontalDrawingGridEvery(TextWriter s)
		{
			if (_displayHorizontalDrawingGridEvery != null)
			{
				_displayHorizontalDrawingGridEvery.Write(s, "displayHorizontalDrawingGridEvery");
			}
		}

		public void Write_displayVerticalDrawingGridEvery(TextWriter s)
		{
			if (_displayVerticalDrawingGridEvery != null)
			{
				_displayVerticalDrawingGridEvery.Write(s, "displayVerticalDrawingGridEvery");
			}
		}

		public void Write_doNotUseMarginsForDrawingGridOrigin(TextWriter s)
		{
			if (_doNotUseMarginsForDrawingGridOrigin != null)
			{
				_doNotUseMarginsForDrawingGridOrigin.Write(s, "doNotUseMarginsForDrawingGridOrigin");
			}
		}

		public void Write_doNotShadeFormData(TextWriter s)
		{
			if (_doNotShadeFormData != null)
			{
				_doNotShadeFormData.Write(s, "doNotShadeFormData");
			}
		}

		public void Write_noPunctuationKerning(TextWriter s)
		{
			if (_noPunctuationKerning != null)
			{
				_noPunctuationKerning.Write(s, "noPunctuationKerning");
			}
		}

		public void Write_printTwoOnOne(TextWriter s)
		{
			if (_printTwoOnOne != null)
			{
				_printTwoOnOne.Write(s, "printTwoOnOne");
			}
		}

		public void Write_strictFirstAndLastChars(TextWriter s)
		{
			if (_strictFirstAndLastChars != null)
			{
				_strictFirstAndLastChars.Write(s, "strictFirstAndLastChars");
			}
		}

		public void Write_savePreviewPicture(TextWriter s)
		{
			if (_savePreviewPicture != null)
			{
				_savePreviewPicture.Write(s, "savePreviewPicture");
			}
		}

		public void Write_doNotValidateAgainstSchema(TextWriter s)
		{
			if (_doNotValidateAgainstSchema != null)
			{
				_doNotValidateAgainstSchema.Write(s, "doNotValidateAgainstSchema");
			}
		}

		public void Write_saveInvalidXml(TextWriter s)
		{
			if (_saveInvalidXml != null)
			{
				_saveInvalidXml.Write(s, "saveInvalidXml");
			}
		}

		public void Write_ignoreMixedContent(TextWriter s)
		{
			if (_ignoreMixedContent != null)
			{
				_ignoreMixedContent.Write(s, "ignoreMixedContent");
			}
		}

		public void Write_alwaysShowPlaceholderText(TextWriter s)
		{
			if (_alwaysShowPlaceholderText != null)
			{
				_alwaysShowPlaceholderText.Write(s, "alwaysShowPlaceholderText");
			}
		}

		public void Write_doNotDemarcateInvalidXml(TextWriter s)
		{
			if (_doNotDemarcateInvalidXml != null)
			{
				_doNotDemarcateInvalidXml.Write(s, "doNotDemarcateInvalidXml");
			}
		}

		public void Write_saveXmlDataOnly(TextWriter s)
		{
			if (_saveXmlDataOnly != null)
			{
				_saveXmlDataOnly.Write(s, "saveXmlDataOnly");
			}
		}

		public void Write_useXSLTWhenSaving(TextWriter s)
		{
			if (_useXSLTWhenSaving != null)
			{
				_useXSLTWhenSaving.Write(s, "useXSLTWhenSaving");
			}
		}

		public void Write_showXMLTags(TextWriter s)
		{
			if (_showXMLTags != null)
			{
				_showXMLTags.Write(s, "showXMLTags");
			}
		}

		public void Write_alwaysMergeEmptyNamespace(TextWriter s)
		{
			if (_alwaysMergeEmptyNamespace != null)
			{
				_alwaysMergeEmptyNamespace.Write(s, "alwaysMergeEmptyNamespace");
			}
		}

		public void Write_updateFields(TextWriter s)
		{
			if (_updateFields != null)
			{
				_updateFields.Write(s, "updateFields");
			}
		}

		public void Write_compat(TextWriter s)
		{
			if (_compat != null)
			{
				_compat.Write(s, "compat");
			}
		}

		public void Write_doNotIncludeSubdocsInStats(TextWriter s)
		{
			if (_doNotIncludeSubdocsInStats != null)
			{
				_doNotIncludeSubdocsInStats.Write(s, "doNotIncludeSubdocsInStats");
			}
		}

		public void Write_doNotAutoCompressPictures(TextWriter s)
		{
			if (_doNotAutoCompressPictures != null)
			{
				_doNotAutoCompressPictures.Write(s, "doNotAutoCompressPictures");
			}
		}

		public void Write_doNotEmbedSmartTags(TextWriter s)
		{
			if (_doNotEmbedSmartTags != null)
			{
				_doNotEmbedSmartTags.Write(s, "doNotEmbedSmartTags");
			}
		}

		public void Write_decimalSymbol(TextWriter s)
		{
			if (_decimalSymbol != null)
			{
				_decimalSymbol.Write(s, "decimalSymbol");
			}
		}

		public void Write_listSeparator(TextWriter s)
		{
			if (_listSeparator != null)
			{
				_listSeparator.Write(s, "listSeparator");
			}
		}

		public void Write_attachedSchema(TextWriter s)
		{
			if (_attachedSchema == null)
			{
				return;
			}
			foreach (CT_String item in _attachedSchema)
			{
				item?.Write(s, "attachedSchema");
			}
		}
	}
}
