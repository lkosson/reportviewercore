using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace Microsoft.ReportingServices.ReportPublishing
{
	internal sealed class RmlValidatingReader : RDLValidatingReader
	{
		internal enum CustomFlags
		{
			None,
			InCustomElement,
			AfterCustomElement
		}

		internal enum ItemType
		{
			Rdl,
			Rdlx,
			Rsd
		}

		private CustomFlags m_custom;

		private PublishingErrorContext m_errorContext;

		private ItemType m_itemType;

		private readonly NameValueCollection m_microversioningValidationStructureElements;

		private readonly NameValueCollection m_microversioningValidationStructureAttributes;

		private readonly Stack<Pair<string, string>> m_rdlElementHierarchy;

		private List<string> m_serverSupportedSchemas;

		internal RmlValidatingReader(Stream stream, List<Pair<string, Stream>> namespaceSchemaStreamMap, PublishingErrorContext errorContext, ItemType itemType)
			: base(stream, namespaceSchemaStreamMap)
		{
			m_rdlElementHierarchy = new Stack<Pair<string, string>>();
			m_microversioningValidationStructureElements = SetupMicroVersioningValidationStructureForElements();
			m_microversioningValidationStructureAttributes = SetupMicroVersioningValidationStructureForAttributes();
			SetupMicroVersioningSchemas();
			base.ValidationEventHandler += ValidationCallBack;
			m_errorContext = errorContext;
			m_itemType = itemType;
		}

		public override bool Read()
		{
			try
			{
				if (CustomFlags.AfterCustomElement != m_custom)
				{
					base.Read();
					if (!Validate(out string message))
					{
						RegisterErrorAndThrow(message);
					}
					if (m_itemType == ItemType.Rdl || m_itemType == ItemType.Rdlx)
					{
						if (!RdlAdditionElementLocationValidation(out message))
						{
							RegisterErrorAndThrow(message);
						}
						if (!RdlAdditionAttributeLocationValidation(out message))
						{
							RegisterErrorAndThrow(message);
						}
						if (!ForceLaxSkippedValidation(out message))
						{
							RegisterErrorAndThrow(message);
						}
					}
				}
				else
				{
					m_custom = CustomFlags.None;
				}
				if (CustomFlags.InCustomElement != m_custom)
				{
					while (!base.EOF && XmlNodeType.Element == base.NodeType && !ListUtils.ContainsWithOrdinalComparer(base.NamespaceURI, m_validationNamespaceList))
					{
						Skip();
					}
				}
				return !base.EOF;
			}
			catch (ArgumentException ex)
			{
				RegisterErrorAndThrow(ex.Message);
				return false;
			}
		}

		private static NameValueCollection SetupMicroVersioningValidationStructureForElements()
		{
			NameValueCollection nameValueCollection = new NameValueCollection(StringComparer.Ordinal);
			SetMicroVersionValidationStructure(nameValueCollection, "http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition");
			SetMicroVersionValidationStructure(nameValueCollection, "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition");
			return nameValueCollection;
		}

		private static void SetMicroVersionValidationStructure(NameValueCollection validationStructure, string expandToThisNamespace)
		{
			validationStructure.Add(GetExpandedName("CanScroll", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), GetExpandedName("Tablix", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("CanScrollVertically", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), GetExpandedName("Textbox", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("NaturalGroup", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), GetExpandedName("Group", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("NaturalSort", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), GetExpandedName("SortExpression", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("DeferredSort", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), GetExpandedName("SortExpression", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("Relationship", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), GetExpandedName("Tablix", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("Relationship", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), GetExpandedName("Chart", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("Relationship", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), GetExpandedName("MapDataRegion", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("Relationship", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), GetExpandedName("GaugePanel", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("Relationship", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), GetExpandedName("CustomData", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("Relationship", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), GetExpandedName("Group", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("Relationship", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), GetExpandedName("Relationships", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"));
			validationStructure.Add(GetExpandedName("Relationships", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), GetExpandedName("TablixCell", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("Relationships", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), GetExpandedName("ChartDataPoint", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("Relationships", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), GetExpandedName("DataCell", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("DataSetName", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), GetExpandedName("Group", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("DataSetName", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), GetExpandedName("TablixCell", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("DataSetName", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), GetExpandedName("ChartDataPoint", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("DataSetName", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), GetExpandedName("DataCell", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("DefaultRelationships", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), GetExpandedName("DataSet", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("BandLayoutOptions", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), GetExpandedName("Tablix", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("LeftMargin", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), GetExpandedName("Tablix", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("RightMargin", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), GetExpandedName("Tablix", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("TopMargin", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), GetExpandedName("Tablix", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("BottomMargin", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), GetExpandedName("Tablix", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("DataSetName", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), GetExpandedName("LabelData", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"));
			validationStructure.Add(GetExpandedName("HighlightX", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), GetExpandedName("ChartDataPointValues", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("HighlightY", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), GetExpandedName("ChartDataPointValues", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("HighlightSize", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), GetExpandedName("ChartDataPointValues", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("AggregateIndicatorField", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), GetExpandedName("Field", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("NullsAsBlanks", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), GetExpandedName("DataSet", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("CollationCulture", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), GetExpandedName("DataSet", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("Tag", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), GetExpandedName("Image", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("Subtype", "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition"), GetExpandedName("ChartSeries", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("EmbeddingMode", "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition"), GetExpandedName("Image", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("EmbeddingMode", "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition"), GetExpandedName("BackgroundImage", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("LayoutDirection", "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition"), GetExpandedName("ReportSection", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("FontFamily", "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition"), GetExpandedName("Style", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("EnableDrilldown", "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition"), GetExpandedName("TablixRowHierarchy", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("EnableDrilldown", "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition"), GetExpandedName("TablixColumnHierarchy", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("BackgroundColor", "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition"), GetExpandedName("Style", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("Color", "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition"), GetExpandedName("Style", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("EnableDrilldown", "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition"), GetExpandedName("ChartCategoryHierarchy", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("BackgroundRepeat", "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition"), GetExpandedName("BackgroundImage", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("Transparency", "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition"), GetExpandedName("BackgroundImage", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("KeyFields", "http://schemas.microsoft.com/sqlserver/reporting/2013/01/reportdefinition"), GetExpandedName("LabelData", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"));
			validationStructure.Add(GetExpandedName("Tags", "http://schemas.microsoft.com/sqlserver/reporting/2013/01/reportdefinition"), GetExpandedName("Image", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("FormatX", "http://schemas.microsoft.com/sqlserver/reporting/2013/01/reportdefinition"), GetExpandedName("ChartDataPointValues", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("FormatY", "http://schemas.microsoft.com/sqlserver/reporting/2013/01/reportdefinition"), GetExpandedName("ChartDataPointValues", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("FormatSize", "http://schemas.microsoft.com/sqlserver/reporting/2013/01/reportdefinition"), GetExpandedName("ChartDataPointValues", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("CurrencyLanguageX", "http://schemas.microsoft.com/sqlserver/reporting/2013/01/reportdefinition"), GetExpandedName("ChartDataPointValues", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("CurrencyLanguageY", "http://schemas.microsoft.com/sqlserver/reporting/2013/01/reportdefinition"), GetExpandedName("ChartDataPointValues", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("CurrencyLanguageSize", "http://schemas.microsoft.com/sqlserver/reporting/2013/01/reportdefinition"), GetExpandedName("ChartDataPointValues", expandToThisNamespace));
			validationStructure.Add(GetExpandedName("CurrencyLanguage", "http://schemas.microsoft.com/sqlserver/reporting/2013/01/reportdefinition"), GetExpandedName("Style", expandToThisNamespace));
			if (expandToThisNamespace == "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition")
			{
				SetMicroVersionValidationStructureForRDL2016(validationStructure);
			}
		}

		private static void SetMicroVersionValidationStructureForRDL2016(NameValueCollection validationStructure)
		{
			validationStructure.Add(GetExpandedName("DefaultFontFamily", "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition/defaultfontfamily"), GetExpandedName("Report", "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition"));
		}

		private static NameValueCollection SetupMicroVersioningValidationStructureForAttributes()
		{
			return new NameValueCollection(StringComparer.Ordinal)
			{
				{
					GetExpandedName("Name", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"),
					GetExpandedName("ReportSection", "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition")
				},
				{
					GetExpandedName("Name", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"),
					GetExpandedName("ReportSection", "http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition")
				},
				{
					GetExpandedName("ValueType", "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition"),
					GetExpandedName("FontFamily", "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition")
				},
				{
					GetExpandedName("ValueType", "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition"),
					GetExpandedName("Color", "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition")
				},
				{
					GetExpandedName("ValueType", "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition"),
					GetExpandedName("BackgroundColor", "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition")
				}
			};
		}

		private void SetupMicroVersioningSchemas()
		{
			m_serverSupportedSchemas = new List<string>
			{
				"http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition",
				"http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition/defaultfontfamily"
			};
		}

		private static string GetExpandedName(string localName, string namespaceURI)
		{
			StringBuilder stringBuilder = new StringBuilder(namespaceURI);
			stringBuilder.Append(":");
			stringBuilder.Append(localName);
			return stringBuilder.ToString();
		}

		private bool RdlAdditionElementLocationValidation(out string message)
		{
			Pair<string, string>? pair = null;
			string text = null;
			bool flag = false;
			message = null;
			if (ListUtils.ContainsWithOrdinalComparer(NamespaceURI, m_validationNamespaceList))
			{
				switch (NodeType)
				{
				case XmlNodeType.Element:
				{
					text = GetExpandedName(LocalName, NamespaceURI);
					bool flag2 = IsPowerViewMicroVersionedNamespace();
					if ((m_itemType == ItemType.Rdl || m_itemType == ItemType.Rsd) && flag2)
					{
						message = RDLValidatingReaderStrings.rdlValidationInvalidNamespaceElement(text, NamespaceURI, base.LineNumber.ToString(CultureInfo.InvariantCulture.NumberFormat), base.LinePosition.ToString(CultureInfo.InvariantCulture.NumberFormat));
						return false;
					}
					if (m_rdlElementHierarchy.Count > 0)
					{
						pair = m_rdlElementHierarchy.Peek();
					}
					if (!pair.HasValue)
					{
						Global.Tracer.Assert(LocalName == "Report", "(this.LocalName == Constants.Report)");
						Global.Tracer.Assert(NamespaceURI == "http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition" || NamespaceURI == "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition", "(this.NamespaceURI == Constants.RDL2010NamespaceURI) || (this.NamespaceURI == Constants.RDL2016NamespaceURI)");
					}
					if (!IsEmptyElement)
					{
						m_rdlElementHierarchy.Push(new Pair<string, string>(text, NamespaceURI));
					}
					if (!flag2)
					{
						break;
					}
					string[] values = m_microversioningValidationStructureElements.GetValues(text);
					if (values != null)
					{
						for (int i = 0; i < values.Length; i++)
						{
							if (pair.Value.First.Equals(values[i], StringComparison.Ordinal))
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							message = RDLValidatingReaderStrings.rdlValidationInvalidParent(Name, NamespaceURI, pair.Value.First, base.LineNumber.ToString(CultureInfo.InvariantCulture.NumberFormat), base.LinePosition.ToString(CultureInfo.InvariantCulture.NumberFormat));
							return false;
						}
					}
					else if (!pair.Value.Second.Equals(NamespaceURI, StringComparison.Ordinal))
					{
						message = RDLValidatingReaderStrings.rdlValidationInvalidMicroVersionedElement(Name, pair.Value.First, base.LineNumber.ToString(CultureInfo.InvariantCulture.NumberFormat), base.LinePosition.ToString(CultureInfo.InvariantCulture.NumberFormat));
						return false;
					}
					break;
				}
				case XmlNodeType.EndElement:
					m_rdlElementHierarchy.Pop();
					break;
				}
			}
			return true;
		}

		private bool RdlAdditionAttributeLocationValidation(out string message)
		{
			message = null;
			HashSet<string> hashSet = null;
			if (NodeType == XmlNodeType.Element && HasAttributes)
			{
				string expandedName = GetExpandedName(LocalName, NamespaceURI);
				string namespaceURI = NamespaceURI;
				if (string.CompareOrdinal(expandedName, GetExpandedName("Report", "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition")) == 0 && GetAttributeLocalName("MustUnderstand") != null)
				{
					hashSet = new HashSet<string>(GetAttributeLocalName("MustUnderstand").Split());
				}
				while (MoveToNextAttribute())
				{
					string text = NamespaceURI;
					if (string.IsNullOrEmpty(text))
					{
						text = namespaceURI;
					}
					if (IsMicroVersionedAttributeNamespace(text))
					{
						string expandedName2 = GetExpandedName(LocalName, text);
						if (m_itemType == ItemType.Rdl || m_itemType == ItemType.Rsd)
						{
							message = RDLValidatingReaderStrings.rdlValidationInvalidNamespaceAttribute(expandedName2, text, base.LineNumber.ToString(CultureInfo.InvariantCulture.NumberFormat), base.LinePosition.ToString(CultureInfo.InvariantCulture.NumberFormat));
						}
						else
						{
							string[] values = m_microversioningValidationStructureAttributes.GetValues(expandedName2);
							if (values != null)
							{
								for (int i = 0; i < values.Length; i++)
								{
									if (values[i].Equals(expandedName, StringComparison.Ordinal))
									{
										MoveToElement();
										return true;
									}
								}
							}
							message = RDLValidatingReaderStrings.rdlValidationInvalidMicroVersionedAttribute(expandedName2, expandedName, base.LineNumber.ToString(CultureInfo.InvariantCulture.NumberFormat), base.LinePosition.ToString(CultureInfo.InvariantCulture.NumberFormat));
						}
					}
					if (hashSet != null && Prefix == "xmlns" && hashSet.Contains(LocalName))
					{
						hashSet.Remove(LocalName);
						if (!m_serverSupportedSchemas.Contains(Value))
						{
							message = RDLValidatingReaderStrings.rdlValidationUnsupportedSchema(Value, LocalName, base.LineNumber.ToString(CultureInfo.InvariantCulture.NumberFormat), base.LinePosition.ToString(CultureInfo.InvariantCulture.NumberFormat));
							m_errorContext.Register(ProcessingErrorCode.rsInvalidMustUnderstandNamespaces, Severity.Error, ObjectType.Report, null, "MustUnderstand", message);
							throw new ReportProcessingException(m_errorContext.Messages);
						}
					}
				}
				if (hashSet != null && hashSet.Count != 0)
				{
					if (hashSet.Count == 1)
					{
						message = RDLValidatingReaderStrings.rdlValidationUndefinedSchemaNamespace(hashSet.First(), "MustUnderstand", base.LineNumber.ToString(CultureInfo.InvariantCulture.NumberFormat), base.LinePosition.ToString(CultureInfo.InvariantCulture.NumberFormat));
						m_errorContext.Register(ProcessingErrorCode.rsInvalidMustUnderstandNamespaces, Severity.Error, ObjectType.Report, null, "MustUnderstand", message);
						throw new ReportProcessingException(m_errorContext.Messages);
					}
					message = RDLValidatingReaderStrings.rdlValidationMultipleUndefinedSchemaNamespaces(string.Join(", ", hashSet.ToArray()), "MustUnderstand", base.LineNumber.ToString(CultureInfo.InvariantCulture.NumberFormat), base.LinePosition.ToString(CultureInfo.InvariantCulture.NumberFormat));
					m_errorContext.Register(ProcessingErrorCode.rsInvalidMustUnderstandNamespaces, Severity.Error, ObjectType.Report, null, "MustUnderstand", message);
					throw new ReportProcessingException(m_errorContext.Messages);
				}
				MoveToElement();
			}
			return message == null;
		}

		private bool IsPowerViewMicroVersionedNamespace()
		{
			Global.Tracer.Assert(ListUtils.ContainsWithOrdinalComparer(NamespaceURI, m_validationNamespaceList), "Not rdl namespace: " + NamespaceURI);
			if (string.CompareOrdinal(NamespaceURI, "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition") != 0 && string.CompareOrdinal(NamespaceURI, "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition") != 0)
			{
				return string.CompareOrdinal(NamespaceURI, "http://schemas.microsoft.com/sqlserver/reporting/2013/01/reportdefinition") == 0;
			}
			return true;
		}

		private bool IsMicroVersionedAttributeNamespace(string namespaceUri)
		{
			return string.CompareOrdinal(namespaceUri, "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition") == 0;
		}

		private bool ForceLaxSkippedValidation(out string message)
		{
			bool result = true;
			message = null;
			if (RDLValidatingReader.m_processContent == XmlSchemaContentProcessing.Lax && m_reader.NodeType == XmlNodeType.EndElement && m_reader.SchemaInfo != null && m_reader.SchemaInfo.Validity == XmlSchemaValidity.NotKnown && ListUtils.ContainsWithOrdinalComparer(m_reader.NamespaceURI, m_validationNamespaceList))
			{
				result = false;
				message = RDLValidatingReaderStrings.rdlValidationNoElementDecl(GetExpandedName(m_reader.LocalName, m_reader.NamespaceURI), m_reader.LocalName, m_reader.NamespaceURI, base.LineNumber.ToString(CultureInfo.InvariantCulture.NumberFormat));
			}
			return result;
		}

		public override string ReadString()
		{
			if (base.IsEmptyElement)
			{
				return string.Empty;
			}
			return base.ReadString();
		}

		internal bool ReadBoolean(ObjectType objectType, string objectName, string propertyName)
		{
			bool result = false;
			if (base.IsEmptyElement)
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidBooleanConstant, Severity.Error, objectType, objectName, propertyName, string.Empty);
				return result;
			}
			string text = base.ReadString();
			try
			{
				result = XmlConvert.ToBoolean(text);
				return result;
			}
			catch
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidBooleanConstant, Severity.Error, objectType, objectName, propertyName, text);
				return result;
			}
		}

		internal int ReadInteger(ObjectType objectType, string objectName, string propertyName)
		{
			int result = 0;
			if (base.IsEmptyElement)
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidIntegerConstant, Severity.Error, objectType, objectName, propertyName, string.Empty);
				return result;
			}
			string text = base.ReadString();
			try
			{
				result = XmlConvert.ToInt32(text);
				return result;
			}
			catch
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidIntegerConstant, Severity.Error, objectType, objectName, propertyName, text);
				return result;
			}
		}

		internal string ReadCustomXml()
		{
			Global.Tracer.Assert(m_custom == CustomFlags.None);
			if (base.IsEmptyElement)
			{
				return string.Empty;
			}
			m_custom = CustomFlags.InCustomElement;
			string result = base.ReadInnerXml();
			m_custom = CustomFlags.AfterCustomElement;
			return result;
		}

		private void ValidationCallBack(object sender, ValidationEventArgs args)
		{
			if (ListUtils.ContainsWithOrdinalComparer(base.NamespaceURI, m_validationNamespaceList))
			{
				RegisterErrorAndThrow(args.Message);
				return;
			}
			_ = base.NodeType;
			_ = 3;
		}

		private void RegisterErrorAndThrow(string message)
		{
			if (m_itemType == ItemType.Rdl || m_itemType == ItemType.Rdlx)
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidReportDefinition, Severity.Error, ObjectType.Report, null, null, message);
				throw new ReportProcessingException(m_errorContext.Messages);
			}
			m_errorContext.Register(ProcessingErrorCode.rsInvalidSharedDataSetDefinition, Severity.Error, ObjectType.SharedDataSet, null, null, message);
			throw new DataSetPublishingException(m_errorContext.Messages);
		}
	}
}
