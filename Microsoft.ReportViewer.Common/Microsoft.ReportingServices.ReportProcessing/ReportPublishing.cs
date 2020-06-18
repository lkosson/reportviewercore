using Microsoft.ReportingServices.DataExtensions;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.ReportPublishing;
using Microsoft.ReportingServices.ReportRendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class ReportPublishing
	{
		private enum StyleOwnerType
		{
			Line = 1,
			Rectangle,
			Checkbox,
			Image,
			ActiveXControl,
			List,
			Matrix,
			Table,
			OWCChart,
			Body,
			Chart,
			Textbox,
			SubReport,
			Subtotal,
			PageSection
		}

		private struct PublishingContextStruct
		{
			private LocationFlags m_location;

			private ObjectType m_objectType;

			private string m_objectName;

			internal LocationFlags Location
			{
				get
				{
					return m_location;
				}
				set
				{
					m_location = value;
				}
			}

			internal ObjectType ObjectType
			{
				get
				{
					return m_objectType;
				}
				set
				{
					m_objectType = value;
				}
			}

			internal string ObjectName
			{
				get
				{
					return m_objectName;
				}
				set
				{
					m_objectName = value;
				}
			}

			internal PublishingContextStruct(LocationFlags location, ObjectType objectType, string objectName)
			{
				m_location = location;
				m_objectType = objectType;
				m_objectName = objectName;
			}

			internal ExpressionParser.ExpressionContext CreateExpressionContext(ExpressionParser.ExpressionType expressionType, ExpressionParser.ConstantType constantType, string propertyName, string dataSetName)
			{
				return new ExpressionParser.ExpressionContext(expressionType, constantType, m_location, m_objectType, m_objectName, propertyName, dataSetName, parseExtended: false);
			}

			internal ExpressionParser.ExpressionContext CreateExpressionContext(ExpressionParser.ExpressionType expressionType, ExpressionParser.ConstantType constantType, string propertyName, string dataSetName, bool parseExtended)
			{
				return new ExpressionParser.ExpressionContext(expressionType, constantType, m_location, m_objectType, m_objectName, propertyName, dataSetName, parseExtended);
			}
		}

		private sealed class StyleInformation
		{
			private StringList m_names = new StringList();

			private ExpressionInfoList m_values = new ExpressionInfoList();

			private static Hashtable StyleNameIndexes;

			private static bool[,] AllowStyleAttributeByType;

			internal StringList Names => m_names;

			internal ExpressionInfoList Values => m_values;

			static StyleInformation()
			{
				AllowStyleAttributeByType = new bool[43, 12]
				{
					{
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true
					},
					{
						true,
						false,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true
					},
					{
						true,
						false,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true
					},
					{
						true,
						false,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true
					},
					{
						true,
						false,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true
					},
					{
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true
					},
					{
						true,
						false,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true
					},
					{
						true,
						false,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true
					},
					{
						true,
						false,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true
					},
					{
						true,
						false,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true
					},
					{
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true
					},
					{
						true,
						false,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true
					},
					{
						true,
						false,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true
					},
					{
						true,
						false,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true
					},
					{
						true,
						false,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true
					},
					{
						true,
						false,
						true,
						false,
						false,
						false,
						true,
						true,
						true,
						false,
						true,
						true
					},
					{
						true,
						false,
						true,
						false,
						false,
						false,
						true,
						true,
						true,
						false,
						true,
						true
					},
					{
						true,
						false,
						true,
						false,
						false,
						false,
						true,
						true,
						true,
						false,
						true,
						true
					},
					{
						true,
						false,
						true,
						false,
						false,
						false,
						true,
						true,
						true,
						false,
						true,
						true
					},
					{
						true,
						false,
						true,
						false,
						false,
						false,
						true,
						true,
						true,
						false,
						true,
						true
					},
					{
						true,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						true
					},
					{
						true,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						true
					},
					{
						true,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						true
					},
					{
						true,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						true
					},
					{
						true,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						true
					},
					{
						true,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						true
					},
					{
						true,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false
					},
					{
						true,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false
					},
					{
						true,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						true
					},
					{
						true,
						false,
						false,
						false,
						true,
						false,
						false,
						false,
						false,
						false,
						false,
						false
					},
					{
						true,
						false,
						false,
						false,
						true,
						false,
						false,
						false,
						false,
						false,
						false,
						false
					},
					{
						true,
						false,
						false,
						false,
						true,
						false,
						false,
						false,
						false,
						false,
						false,
						false
					},
					{
						true,
						false,
						false,
						false,
						true,
						false,
						false,
						false,
						false,
						false,
						false,
						false
					},
					{
						true,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false
					},
					{
						true,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						true
					},
					{
						true,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						true
					},
					{
						true,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false
					},
					{
						true,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						true
					},
					{
						true,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						true
					},
					{
						true,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						true
					},
					{
						true,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						true
					},
					{
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						true
					},
					{
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						true
					}
				};
				StyleNameIndexes = new Hashtable();
				StyleNameIndexes.Add("BorderColor", 0);
				StyleNameIndexes.Add("BorderColorLeft", 1);
				StyleNameIndexes.Add("BorderColorRight", 2);
				StyleNameIndexes.Add("BorderColorTop", 3);
				StyleNameIndexes.Add("BorderColorBottom", 4);
				StyleNameIndexes.Add("BorderStyle", 5);
				StyleNameIndexes.Add("BorderStyleLeft", 6);
				StyleNameIndexes.Add("BorderStyleRight", 7);
				StyleNameIndexes.Add("BorderStyleTop", 8);
				StyleNameIndexes.Add("BorderStyleBottom", 9);
				StyleNameIndexes.Add("BorderWidth", 10);
				StyleNameIndexes.Add("BorderWidthLeft", 11);
				StyleNameIndexes.Add("BorderWidthRight", 12);
				StyleNameIndexes.Add("BorderWidthTop", 13);
				StyleNameIndexes.Add("BorderWidthBottom", 14);
				StyleNameIndexes.Add("BackgroundColor", 15);
				StyleNameIndexes.Add("BackgroundImageSource", 16);
				StyleNameIndexes.Add("BackgroundImageValue", 17);
				StyleNameIndexes.Add("BackgroundImageMIMEType", 18);
				StyleNameIndexes.Add("BackgroundRepeat", 19);
				StyleNameIndexes.Add("FontStyle", 20);
				StyleNameIndexes.Add("FontFamily", 21);
				StyleNameIndexes.Add("FontSize", 22);
				StyleNameIndexes.Add("FontWeight", 23);
				StyleNameIndexes.Add("Format", 24);
				StyleNameIndexes.Add("TextDecoration", 25);
				StyleNameIndexes.Add("TextAlign", 26);
				StyleNameIndexes.Add("VerticalAlign", 27);
				StyleNameIndexes.Add("Color", 28);
				StyleNameIndexes.Add("PaddingLeft", 29);
				StyleNameIndexes.Add("PaddingRight", 30);
				StyleNameIndexes.Add("PaddingTop", 31);
				StyleNameIndexes.Add("PaddingBottom", 32);
				StyleNameIndexes.Add("LineHeight", 33);
				StyleNameIndexes.Add("Direction", 34);
				StyleNameIndexes.Add("Language", 35);
				StyleNameIndexes.Add("UnicodeBiDi", 36);
				StyleNameIndexes.Add("Calendar", 37);
				StyleNameIndexes.Add("NumeralLanguage", 38);
				StyleNameIndexes.Add("NumeralVariant", 39);
				StyleNameIndexes.Add("WritingMode", 40);
				StyleNameIndexes.Add("BackgroundGradientType", 41);
				StyleNameIndexes.Add("BackgroundGradientEndColor", 42);
			}

			internal void AddAttribute(string name, ExpressionInfo expression)
			{
				Global.Tracer.Assert(name != null);
				Global.Tracer.Assert(expression != null);
				m_names.Add(name);
				m_values.Add(expression);
			}

			internal void Filter(StyleOwnerType ownerType, bool hasNoRows)
			{
				Global.Tracer.Assert(m_names.Count == m_values.Count);
				int ownerType2 = MapStyleOwnerTypeToIndex(ownerType, hasNoRows);
				for (int num = m_names.Count - 1; num >= 0; num--)
				{
					if (!Allow(MapStyleNameToIndex(m_names[num]), ownerType2))
					{
						m_names.RemoveAt(num);
						m_values.RemoveAt(num);
					}
				}
			}

			private int MapStyleOwnerTypeToIndex(StyleOwnerType ownerType, bool hasNoRows)
			{
				if (hasNoRows)
				{
					return 0;
				}
				switch (ownerType)
				{
				case StyleOwnerType.PageSection:
					return 2;
				case StyleOwnerType.Textbox:
				case StyleOwnerType.SubReport:
				case StyleOwnerType.Subtotal:
					return 0;
				default:
					return (int)ownerType;
				}
			}

			private int MapStyleNameToIndex(string name)
			{
				return (int)StyleNameIndexes[name];
			}

			private bool Allow(int styleName, int ownerType)
			{
				return AllowStyleAttributeByType[styleName, ownerType];
			}
		}

		private sealed class RmlValidatingReader : RDLValidatingReader
		{
			internal enum CustomFlags
			{
				None,
				InCustomElement,
				AfterCustomElement
			}

			private const string XsdResourceID = "Microsoft.ReportingServices.ReportProcessing.ReportDefinition.xsd";

			private CustomFlags m_custom;

			private PublishingErrorContext m_errorContext;

			private string m_targetRDLNamespace;

			private RmlValidatingReader(XmlTextReader textReader, PublishingErrorContext errorContext, string targetRDLNamespace)
				: base(textReader, targetRDLNamespace)
			{
				base.Schemas.Add(XmlSchema.Read(Assembly.GetExecutingAssembly().GetManifestResourceStream("Microsoft.ReportingServices.ReportProcessing.ReportDefinition.xsd"), null));
				base.ValidationEventHandler += ValidationCallBack;
				base.ValidationType = ValidationType.Schema;
				m_errorContext = errorContext;
				m_targetRDLNamespace = targetRDLNamespace;
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
							m_errorContext.Register(ProcessingErrorCode.rsInvalidReportDefinition, Severity.Error, ObjectType.Report, null, null, message);
							throw new ReportProcessingException(m_errorContext.Messages);
						}
					}
					else
					{
						m_custom = CustomFlags.None;
					}
					if (CustomFlags.InCustomElement != m_custom)
					{
						while (!base.EOF && XmlNodeType.Element == base.NodeType && m_targetRDLNamespace != base.NamespaceURI)
						{
							Skip();
						}
					}
					return !base.EOF;
				}
				catch (ArgumentException ex)
				{
					m_errorContext.Register(ProcessingErrorCode.rsInvalidReportDefinition, Severity.Error, ObjectType.Report, null, null, ex.Message);
					throw new ReportProcessingException(m_errorContext.Messages);
				}
			}

			public override string ReadString()
			{
				if (base.IsEmptyElement)
				{
					return string.Empty;
				}
				return base.ReadString();
			}

			internal static RmlValidatingReader CreateReader(XmlTextReader upgradedRDLReader, PublishingErrorContext errorContext, string targetRDLNamespace)
			{
				Global.Tracer.Assert(upgradedRDLReader != null);
				upgradedRDLReader.WhitespaceHandling = WhitespaceHandling.None;
				upgradedRDLReader.XmlResolver = new XmlNullResolver();
				return new RmlValidatingReader(upgradedRDLReader, errorContext, targetRDLNamespace);
			}

			internal bool ReadBoolean()
			{
				if (base.IsEmptyElement)
				{
					Global.Tracer.Assert(condition: false);
					return false;
				}
				return XmlConvert.ToBoolean(base.ReadString());
			}

			internal int ReadInteger()
			{
				if (base.IsEmptyElement)
				{
					Global.Tracer.Assert(condition: false);
					return 0;
				}
				return XmlConvert.ToInt32(base.ReadString());
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
				if (ReportProcessing.CompareWithInvariantCulture(m_targetRDLNamespace, base.NamespaceURI, ignoreCase: false) == 0)
				{
					m_errorContext.Register(ProcessingErrorCode.rsInvalidReportDefinition, Severity.Error, ObjectType.Report, null, null, args.Message);
					throw new ReportProcessingException(m_errorContext.Messages);
				}
				_ = base.NodeType;
				_ = 3;
			}
		}

		private sealed class XmlNullResolver : XmlUrlResolver
		{
			public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
			{
				throw new XmlException("Can't resolve URI reference.", null);
			}
		}

		private sealed class AllowNullKeyHashtable
		{
			private Hashtable m_hashtable = new Hashtable();

			private object m_nullValue;

			internal object this[string name]
			{
				get
				{
					if (name == null)
					{
						return m_nullValue;
					}
					return m_hashtable[name];
				}
				set
				{
					if (name == null)
					{
						m_nullValue = value;
					}
					else
					{
						m_hashtable[name] = value;
					}
				}
			}
		}

		private bool m_static;

		private bool m_interactive;

		private int m_idCounter;

		private RmlValidatingReader m_reader;

		private CLSUniqueNameValidator m_reportItemNames;

		private ScopeNameValidator m_scopeNames;

		private ImageStreamNames m_imageStreamNames;

		private ICatalogItemContext m_reportContext;

		private ReportProcessing.CreateReportChunk m_createChunkCallback;

		private ReportProcessing.CheckSharedDataSource m_checkDataSourceCallback;

		private string m_description;

		private DataSourceInfoCollection m_dataSources;

		private SubReportList m_subReports;

		private UserLocationFlags m_reportLocationFlags = UserLocationFlags.ReportBody;

		private UserLocationFlags m_userReferenceLocation = UserLocationFlags.None;

		private bool m_hasExternalImages;

		private bool m_hasHyperlinks;

		private bool m_pageSectionDrillthroughs;

		private bool m_hasGrouping;

		private bool m_hasSorting;

		private bool m_hasUserSort;

		private bool m_hasGroupFilters;

		private bool m_hasSpecialRecursiveAggregates;

		private bool m_aggregateInDetailSections;

		private bool m_subReportMergeTransactions;

		private ReportCompileTime m_reportCT;

		private bool m_hasImageStreams;

		private bool m_hasLabels;

		private bool m_hasBookmarks;

		private TextBoxList m_textBoxesWithUserSortTarget = new TextBoxList();

		private bool m_hasFilters;

		private DataSetList m_dataSets = new DataSetList();

		private bool m_parametersNotUsedInQuery = true;

		private Hashtable m_usedInQueryInfos = new Hashtable();

		private Hashtable m_reportParamUserProfile = new Hashtable();

		private Hashtable m_dataSetQueryInfo = new Hashtable();

		private ArrayList m_dynamicParameters = new ArrayList();

		private CultureInfo m_reportLanguage;

		private bool m_hasUserSortPeerScopes;

		private Hashtable m_reportScopes = new Hashtable();

		private StringDictionary m_dataSourceNames = new StringDictionary();

		private int m_dataRegionCount;

		private ArrayList m_reportItemCollectionList = new ArrayList();

		private ArrayList m_aggregateHolderList = new ArrayList();

		private ArrayList m_runningValueHolderList = new ArrayList();

		private string m_targetRDLNamespace;

		private Report m_report;

		private PublishingErrorContext m_errorContext;

		internal Report CreateIntermediateFormat(ICatalogItemContext reportContext, byte[] definition, ReportProcessing.CreateReportChunk createChunkCallback, ReportProcessing.CheckSharedDataSource checkDataSourceCallback, ReportProcessing.ResolveTemporaryDataSource resolveTemporaryDataSourceCallback, DataSourceInfoCollection originalDataSources, PublishingErrorContext errorContext, AppDomain compilationTempAppDomain, bool generateExpressionHostWithRefusedPermissions, IDataProtection dataProtection, out string description, out string language, out ParameterInfoCollection parameters, out DataSourceInfoCollection dataSources, out UserLocationFlags userReferenceLocation, out ArrayList dataSetsName, out bool hasExternalImages, out bool hasHyperlinks)
		{
			try
			{
				m_report = null;
				m_errorContext = errorContext;
				if (definition == null)
				{
					m_errorContext.Register(ProcessingErrorCode.rsNotAReportDefinition, Severity.Error, ObjectType.Report, null, null);
					throw new ReportProcessingException(m_errorContext.Messages);
				}
				Phase1(reportContext, definition, createChunkCallback, checkDataSourceCallback, resolveTemporaryDataSourceCallback, originalDataSources, dataProtection, out description, out language, out dataSources, out userReferenceLocation, out hasExternalImages, out hasHyperlinks);
				Phase2();
				Phase3(reportContext, out parameters, compilationTempAppDomain, generateExpressionHostWithRefusedPermissions);
				Phase4();
				if (m_errorContext.HasError)
				{
					throw new ReportProcessingException(m_errorContext.Messages);
				}
				CalculateChildrenPostions(m_report);
				CalculateChildrenDependencies(m_report);
				dataSetsName = null;
				for (int i = 0; i < m_dataSets.Count; i++)
				{
					if (!m_dataSets[i].UsedOnlyInParameters)
					{
						if (dataSetsName == null)
						{
							dataSetsName = new ArrayList();
						}
						dataSetsName.Add(m_dataSets[i].Name);
					}
				}
				return m_report;
			}
			finally
			{
				m_report = null;
				m_errorContext = null;
			}
		}

		private int GenerateID()
		{
			return ++m_idCounter;
		}

		private void Phase1(ICatalogItemContext reportContext, byte[] definition, ReportProcessing.CreateReportChunk createChunkCallback, ReportProcessing.CheckSharedDataSource checkDataSourceCallback, ReportProcessing.ResolveTemporaryDataSource resolveTemporaryDataSourceCallback, DataSourceInfoCollection originalDataSources, IDataProtection dataProtection, out string description, out string language, out DataSourceInfoCollection dataSources, out UserLocationFlags userReferenceLocation, out bool hasExternalImages, out bool hasHyperlinks)
		{
			try
			{
				XmlTextReader xmlTextReader = new XmlTextReader(new MemoryStream(definition, writable: false));
				XmlUtil.ApplyDtdDosDefense(xmlTextReader);
				m_reader = RmlValidatingReader.CreateReader(xmlTextReader, m_errorContext, "http://schemas.microsoft.com/sqlserver/reporting/2005/01/reportdefinition");
				m_reportItemNames = new CLSUniqueNameValidator(ProcessingErrorCode.rsInvalidNameNotCLSCompliant, ProcessingErrorCode.rsDuplicateReportItemName);
				m_scopeNames = new ScopeNameValidator();
				m_imageStreamNames = new ImageStreamNames();
				m_reportContext = reportContext;
				m_createChunkCallback = createChunkCallback;
				m_checkDataSourceCallback = checkDataSourceCallback;
				m_dataSources = new DataSourceInfoCollection();
				m_subReports = new SubReportList();
				while (m_reader.Read())
				{
					if (XmlNodeType.Element == m_reader.NodeType && "Report" == m_reader.LocalName)
					{
						m_reportCT = new ReportCompileTime(new VBExpressionParser(m_errorContext), m_errorContext);
						m_report = ReadReport(resolveTemporaryDataSourceCallback, originalDataSources, dataProtection);
					}
				}
				if (m_report == null)
				{
					m_errorContext.Register(ProcessingErrorCode.rsNotACurrentReportDefinition, Severity.Error, ObjectType.Report, null, "Namespace", m_targetRDLNamespace);
					throw new ReportProcessingException(m_errorContext.Messages);
				}
			}
			catch (XmlException ex)
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidReportDefinition, Severity.Error, ObjectType.Report, null, null, ex.Message);
				throw new ReportProcessingException(m_errorContext.Messages);
			}
			finally
			{
				if (m_reader != null)
				{
					m_reader.Close();
					m_reader = null;
				}
				m_reportItemNames = null;
				m_scopeNames = null;
				m_imageStreamNames = null;
				m_reportContext = null;
				m_createChunkCallback = null;
				m_checkDataSourceCallback = null;
				description = m_description;
				language = null;
				if (m_reportLanguage != null)
				{
					language = m_reportLanguage.Name;
				}
				dataSources = m_dataSources;
				userReferenceLocation = m_userReferenceLocation;
				hasExternalImages = m_hasExternalImages;
				hasHyperlinks = m_hasHyperlinks;
				m_description = null;
				m_dataSources = null;
			}
		}

		private Report ReadReport(ReportProcessing.ResolveTemporaryDataSource resolveTemporaryDataSourceCallback, DataSourceInfoCollection originalDataSources, IDataProtection dataProtection)
		{
			Report report = new Report(GenerateID(), GenerateID());
			PublishingContextStruct context = new PublishingContextStruct(LocationFlags.None, report.ObjectType, null);
			ExpressionInfo expressionInfo = null;
			m_reportItemCollectionList.Add(report.ReportItems);
			m_aggregateHolderList.Add(report);
			m_runningValueHolderList.Add(report.ReportItems);
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (m_reader.LocalName)
					{
					case "Description":
						m_description = m_reader.ReadString();
						break;
					case "Author":
						report.Author = m_reader.ReadString();
						break;
					case "AutoRefresh":
						report.AutoRefresh = m_reader.ReadInteger();
						break;
					case "DataSources":
						report.DataSources = ReadDataSources(context, resolveTemporaryDataSourceCallback, originalDataSources, dataProtection);
						break;
					case "DataSets":
						ReadDataSets(context);
						break;
					case "Body":
						ReadBody(report, context);
						break;
					case "ReportParameters":
						report.Parameters = ReadReportParameters(context);
						break;
					case "Custom":
						report.Custom = m_reader.ReadCustomXml();
						break;
					case "CustomProperties":
						report.CustomProperties = ReadCustomProperties(context);
						break;
					case "Code":
						report.Code = m_reader.ReadString();
						m_reportCT.Builder.SetCustomCode();
						break;
					case "Width":
						report.Width = ReadSize();
						break;
					case "PageHeader":
						report.PageHeader = ReadPageSection(isHeader: true, report, context);
						break;
					case "PageFooter":
						report.PageFooter = ReadPageSection(isHeader: false, report, context);
						break;
					case "PageHeight":
						report.PageHeight = ReadSize();
						break;
					case "PageWidth":
						report.PageWidth = ReadSize();
						break;
					case "InteractiveHeight":
						report.InteractiveHeight = ReadSize();
						break;
					case "InteractiveWidth":
						report.InteractiveWidth = ReadSize();
						break;
					case "LeftMargin":
						report.LeftMargin = ReadSize();
						break;
					case "RightMargin":
						report.RightMargin = ReadSize();
						break;
					case "TopMargin":
						report.TopMargin = ReadSize();
						break;
					case "BottomMargin":
						report.BottomMargin = ReadSize();
						break;
					case "EmbeddedImages":
						report.EmbeddedImages = ReadEmbeddedImages(context);
						break;
					case "Language":
						expressionInfo = (report.Language = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.ReportLanguage, ExpressionParser.ConstantType.String, context));
						break;
					case "CodeModules":
						report.CodeModules = ReadCodeModules(context);
						break;
					case "Classes":
						report.CodeClasses = ReadClasses(context);
						break;
					case "DataTransform":
						report.DataTransform = m_reader.ReadString();
						break;
					case "DataSchema":
						report.DataSchema = m_reader.ReadString();
						break;
					case "DataElementName":
						report.DataElementName = m_reader.ReadString();
						break;
					case "DataElementStyle":
						report.DataElementStyleAttribute = ReadDataElementStyle();
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("Report" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (expressionInfo == null)
			{
				m_reportLanguage = Localization.DefaultReportServerSpecificCulture;
			}
			else if (ExpressionInfo.Types.Constant == expressionInfo.Type)
			{
				PublishingValidator.ValidateSpecificLanguage(expressionInfo, ObjectType.Report, null, "Language", m_errorContext, out m_reportLanguage);
			}
			if (m_interactive)
			{
				report.ShowHideType = Report.ShowHideTypes.Interactive;
			}
			else if (m_static)
			{
				report.ShowHideType = Report.ShowHideTypes.Static;
			}
			else
			{
				report.ShowHideType = Report.ShowHideTypes.None;
			}
			report.ImageStreamNames = m_imageStreamNames;
			report.SubReports = m_subReports;
			report.BodyID = GenerateID();
			report.LastID = m_idCounter;
			return report;
		}

		private EmbeddedImageHashtable ReadEmbeddedImages(PublishingContextStruct context)
		{
			EmbeddedImageHashtable embeddedImageHashtable = new EmbeddedImageHashtable();
			CLSUniqueNameValidator embeddedImageNames = new CLSUniqueNameValidator(ProcessingErrorCode.rsInvalidNameNotCLSCompliant, ProcessingErrorCode.rsDuplicateEmbeddedImageName);
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "EmbeddedImage")
					{
						ReadEmbeddedImage(embeddedImageHashtable, embeddedImageNames, context);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("EmbeddedImages" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return embeddedImageHashtable;
		}

		private void ReadEmbeddedImage(EmbeddedImageHashtable embeddedImages, CLSUniqueNameValidator embeddedImageNames, PublishingContextStruct context)
		{
			string attribute = m_reader.GetAttribute("Name");
			context.ObjectType = ObjectType.EmbeddedImage;
			context.ObjectName = attribute;
			embeddedImageNames.Validate(context.ObjectType, context.ObjectName, m_errorContext);
			bool flag = false;
			byte[] array = null;
			string text = null;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (!(localName == "MIMEType"))
					{
						if (localName == "ImageData")
						{
							string s = m_reader.ReadString();
							try
							{
								array = Convert.FromBase64String(s);
							}
							catch
							{
								m_errorContext.Register(ProcessingErrorCode.rsInvalidEmbeddedImage, Severity.Error, context.ObjectType, context.ObjectName, "ImageData");
							}
						}
					}
					else
					{
						text = m_reader.ReadString();
						if (!PublishingValidator.ValidateMimeType(text, context.ObjectType, context.ObjectName, m_reader.LocalName, m_errorContext))
						{
							text = null;
						}
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("EmbeddedImage" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			string text2 = Guid.NewGuid().ToString();
			embeddedImages.Add(attribute, new ImageInfo(text2, text));
			if (array != null && text != null && m_createChunkCallback != null)
			{
				using (Stream stream = m_createChunkCallback(text2, ReportProcessing.ReportChunkTypes.Image, text))
				{
					stream.Write(array, 0, array.Length);
				}
			}
		}

		private DataSourceList ReadDataSources(PublishingContextStruct context, ReportProcessing.ResolveTemporaryDataSource resolveTemporaryDataSourceCallback, DataSourceInfoCollection originalDataSources, IDataProtection dataProtection)
		{
			DataSourceList dataSourceList = new DataSourceList();
			DataSourceNameValidator dataSourceNames = new DataSourceNameValidator();
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "DataSource")
					{
						dataSourceList.Add(ReadDataSource(dataSourceNames, context, resolveTemporaryDataSourceCallback, originalDataSources, dataProtection));
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("DataSources" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return dataSourceList;
		}

		private DataSource ReadDataSource(DataSourceNameValidator dataSourceNames, PublishingContextStruct context, ReportProcessing.ResolveTemporaryDataSource resolveTemporaryDataSourceCallback, DataSourceInfoCollection originalDataSources, IDataProtection dataProtection)
		{
			DataSource dataSource = new DataSource();
			dataSource.Name = m_reader.GetAttribute("Name");
			context.ObjectType = ObjectType.DataSource;
			context.ObjectName = dataSource.Name;
			bool flag = false;
			if (dataSourceNames.Validate(context.ObjectType, context.ObjectName, m_errorContext))
			{
				flag = true;
			}
			bool flag2 = false;
			bool flag3 = false;
			bool hasComplexParams = false;
			StringList parametersInQuery = null;
			if (!m_reader.IsEmptyElement)
			{
				bool flag4 = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (m_reader.LocalName)
						{
						case "Transaction":
							dataSource.Transaction = m_reader.ReadBoolean();
							break;
						case "ConnectionProperties":
							flag2 = true;
							ReadConnectionProperties(dataSource, context, ref hasComplexParams, ref parametersInQuery);
							break;
						case "DataSourceReference":
							flag3 = true;
							dataSource.DataSourceReference = m_reader.ReadString();
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("DataSource" == m_reader.LocalName)
						{
							flag4 = true;
						}
						break;
					}
				}
				while (!flag4);
			}
			if ((!flag3 && !flag2) || (flag3 && flag2))
			{
				flag = false;
				m_errorContext.Register(ProcessingErrorCode.rsInvalidDataSource, Severity.Error, context.ObjectType, context.ObjectName, null);
			}
			if (flag && !m_dataSourceNames.ContainsKey(dataSource.Name))
			{
				m_dataSourceNames.Add(dataSource.Name, null);
			}
			DataSourceInfo dataSourceInfo = null;
			if (flag2)
			{
				dataSource.IsComplex = hasComplexParams;
				dataSource.ParameterNames = parametersInQuery;
				bool flag5 = false;
				if (dataSource.ConnectStringExpression.Type != ExpressionInfo.Types.Constant)
				{
					flag5 = true;
				}
				dataSourceInfo = new DataSourceInfo(dataSource.Name, dataSource.Type, flag5 ? null : dataSource.ConnectStringExpression.OriginalText, flag5, dataSource.IntegratedSecurity, dataSource.Prompt, dataProtection);
			}
			else if (flag3)
			{
				string text = (m_reportContext == null) ? dataSource.DataSourceReference : m_reportContext.MapUserProvidedPath(dataSource.DataSourceReference);
				if (m_checkDataSourceCallback == null)
				{
					dataSourceInfo = new DataSourceInfo(dataSource.Name, text, Guid.Empty);
				}
				else
				{
					Guid catalogItemId = Guid.Empty;
					DataSourceInfo dataSourceInfo2 = m_checkDataSourceCallback(text, out catalogItemId);
					if (dataSourceInfo2 == null)
					{
						dataSourceInfo = new DataSourceInfo(dataSource.Name);
						m_errorContext.Register(ProcessingErrorCode.rsDataSourceReferenceNotPublished, Severity.Warning, context.ObjectType, context.ObjectName, "Report", dataSource.Name);
					}
					else
					{
						dataSourceInfo = new DataSourceInfo(dataSource.Name, text, catalogItemId, dataSourceInfo2);
					}
				}
			}
			if (dataSourceInfo != null)
			{
				resolveTemporaryDataSourceCallback?.Invoke(dataSourceInfo, originalDataSources);
				dataSource.ID = dataSourceInfo.ID;
				m_dataSources.Add(dataSourceInfo);
			}
			return dataSource;
		}

		private StringList ReadCodeModules(PublishingContextStruct context)
		{
			StringList stringList = new StringList();
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "CodeModule")
					{
						stringList.Add(m_reader.ReadString());
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("CodeModules" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return stringList;
		}

		private CodeClassList ReadClasses(PublishingContextStruct context)
		{
			CodeClassList codeClassList = new CodeClassList();
			CLSUniqueNameValidator instanceNameValidator = new CLSUniqueNameValidator(ProcessingErrorCode.rsInvalidNameNotCLSCompliant, ProcessingErrorCode.rsDuplicateClassInstanceName);
			context.ObjectType = ObjectType.CodeClass;
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "Class")
					{
						ReadClass(codeClassList, instanceNameValidator, context);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("Classes" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			m_reportCT.Builder.SetCustomCode();
			return codeClassList;
		}

		private void ReadClass(CodeClassList codeClasses, CLSUniqueNameValidator instanceNameValidator, PublishingContextStruct context)
		{
			bool flag = false;
			CodeClass codeClass = default(CodeClass);
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (!(localName == "ClassName"))
					{
						if (localName == "InstanceName")
						{
							codeClass.InstanceName = m_reader.ReadString();
							if (!instanceNameValidator.Validate(context.ObjectType, codeClass.InstanceName, m_errorContext))
							{
								codeClass.InstanceName = null;
							}
						}
					}
					else
					{
						codeClass.ClassName = m_reader.ReadString();
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("Class" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			codeClasses.Add(codeClass);
		}

		private void ReadConnectionProperties(DataSource dataSource, PublishingContextStruct context, ref bool hasComplexParams, ref StringList parametersInQuery)
		{
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (m_reader.LocalName)
					{
					case "DataProvider":
						dataSource.Type = m_reader.ReadString();
						break;
					case "ConnectString":
						Global.Tracer.Assert(ObjectType.DataSource == context.ObjectType);
						dataSource.ConnectStringExpression = ReadQueryOrParameterExpression(context, ref hasComplexParams, ref parametersInQuery);
						break;
					case "IntegratedSecurity":
						dataSource.IntegratedSecurity = m_reader.ReadBoolean();
						break;
					case "Prompt":
						dataSource.Prompt = m_reader.ReadString();
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("ConnectionProperties" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
		}

		private void ReadDataSets(PublishingContextStruct context)
		{
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "DataSet")
					{
						m_dataSets.Add(ReadDataSet(context));
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("DataSets" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
		}

		private DataSet ReadDataSet(PublishingContextStruct context)
		{
			DataSet dataSet = new DataSet(GenerateID());
			YukonDataSetInfo queryDataSetInfo = null;
			dataSet.Name = m_reader.GetAttribute("Name");
			context.Location |= LocationFlags.InDataSet;
			context.ObjectType = dataSet.ObjectType;
			context.ObjectName = dataSet.Name;
			if (m_scopeNames.Validate(isGrouping: false, context.ObjectName, context.ObjectType, context.ObjectName, m_errorContext))
			{
				m_reportScopes.Add(dataSet.Name, dataSet);
			}
			m_aggregateHolderList.Add(dataSet);
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (m_reader.LocalName)
					{
					case "Fields":
					{
						dataSet.Fields = ReadFields(context, out int calculatedFieldStartIndex);
						dataSet.NonCalculatedFieldCount = calculatedFieldStartIndex;
						break;
					}
					case "Query":
						dataSet.Query = ReadQuery(context, out queryDataSetInfo);
						break;
					case "CaseSensitivity":
						dataSet.CaseSensitivity = ReadSensitivity();
						break;
					case "Collation":
					{
						dataSet.Collation = m_reader.ReadString();
						if (DataSetValidator.ValidateCollation(dataSet.Collation, out uint lcid))
						{
							dataSet.LCID = lcid;
						}
						break;
					}
					case "AccentSensitivity":
						dataSet.AccentSensitivity = ReadSensitivity();
						break;
					case "KanatypeSensitivity":
						dataSet.KanatypeSensitivity = ReadSensitivity();
						break;
					case "WidthSensitivity":
						dataSet.WidthSensitivity = ReadSensitivity();
						break;
					case "Filters":
						dataSet.Filters = ReadFilters(ExpressionParser.ExpressionType.DataSetFilters, context);
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("DataSet" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (queryDataSetInfo != null && !m_dataSetQueryInfo.ContainsKey(context.ObjectName))
			{
				m_dataSetQueryInfo.Add(context.ObjectName, queryDataSetInfo);
				int num = (dataSet.Fields != null) ? dataSet.Fields.Count : 0;
				while (num > 0 && dataSet.Fields[num - 1].IsCalculatedField)
				{
					num--;
				}
				queryDataSetInfo.CalculatedFieldIndex = num;
			}
			return dataSet;
		}

		private ReportQuery ReadQuery(PublishingContextStruct context, out YukonDataSetInfo queryDataSetInfo)
		{
			ReportQuery reportQuery = new ReportQuery();
			bool flag = false;
			bool hasComplexParams = false;
			StringList parametersInQuery = null;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (m_reader.LocalName)
					{
					case "DataSourceName":
						reportQuery.DataSourceName = m_reader.ReadString();
						break;
					case "CommandType":
						reportQuery.CommandType = ReadCommandType();
						break;
					case "CommandText":
						Global.Tracer.Assert(ObjectType.DataSet == context.ObjectType);
						context.ObjectType = ObjectType.Query;
						reportQuery.CommandText = ReadQueryOrParameterExpression(context, ref hasComplexParams, ref parametersInQuery);
						context.ObjectType = ObjectType.DataSet;
						break;
					case "QueryParameters":
						reportQuery.Parameters = ReadQueryParameters(context, ref hasComplexParams, ref parametersInQuery);
						break;
					case "Timeout":
						reportQuery.TimeOut = m_reader.ReadInteger();
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("Query" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			queryDataSetInfo = new YukonDataSetInfo(m_dataSets.Count, hasComplexParams, parametersInQuery);
			return reportQuery;
		}

		private ExpressionInfo ReadQueryOrParameterExpression(PublishingContextStruct context, ref bool isComplex, ref StringList parametersInQuery)
		{
			Global.Tracer.Assert(ObjectType.QueryParameter == context.ObjectType || ObjectType.Query == context.ObjectType || ObjectType.DataSource == context.ObjectType);
			ExpressionParser.DetectionFlags detectionFlags = (ExpressionParser.DetectionFlags)0;
			if (m_parametersNotUsedInQuery || !isComplex)
			{
				detectionFlags |= ExpressionParser.DetectionFlags.ParameterReference;
			}
			m_reportLocationFlags = UserLocationFlags.ReportQueries;
			bool reportParameterReferenced;
			string reportParameterName;
			ExpressionInfo result = ReadExpression(m_reader.LocalName, context.ObjectName, ExpressionParser.ExpressionType.QueryParameter, ExpressionParser.ConstantType.String, context, detectionFlags, out reportParameterReferenced, out reportParameterName);
			if ((m_parametersNotUsedInQuery || !isComplex) && reportParameterReferenced)
			{
				if (reportParameterName == null)
				{
					m_parametersNotUsedInQuery = false;
					isComplex = true;
				}
				else
				{
					if (!m_usedInQueryInfos.Contains(reportParameterName))
					{
						m_usedInQueryInfos.Add(reportParameterName, true);
					}
					if (!isComplex)
					{
						if (parametersInQuery == null)
						{
							parametersInQuery = new StringList();
						}
						parametersInQuery.Add(reportParameterName);
					}
				}
			}
			m_reportLocationFlags = UserLocationFlags.ReportBody;
			return result;
		}

		private ParameterValueList ReadQueryParameters(PublishingContextStruct context, ref bool hasComplexParams, ref StringList parametersInQuery)
		{
			ParameterValueList parameterValueList = new ParameterValueList();
			bool flag = false;
			string objectName = context.ObjectName;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "QueryParameter")
					{
						parameterValueList.Add(ReadQueryParameter(context, ref hasComplexParams, ref parametersInQuery));
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("QueryParameters" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			context.ObjectName = objectName;
			return parameterValueList;
		}

		private ParameterValue ReadQueryParameter(PublishingContextStruct context, ref bool isComplex, ref StringList parametersInQuery)
		{
			Global.Tracer.Assert(ObjectType.DataSet == context.ObjectType);
			ParameterValue parameterValue = new ParameterValue();
			parameterValue.Name = m_reader.GetAttribute("Name");
			context.ObjectType = ObjectType.QueryParameter;
			context.ObjectName = parameterValue.Name;
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "Value")
					{
						parameterValue.Value = ReadQueryOrParameterExpression(context, ref isComplex, ref parametersInQuery);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("QueryParameter" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return parameterValue;
		}

		private DataFieldList ReadFields(PublishingContextStruct context, out int calculatedFieldStartIndex)
		{
			DataFieldList dataFieldList = new DataFieldList();
			CLSUniqueNameValidator names = new CLSUniqueNameValidator(ProcessingErrorCode.rsInvalidFieldNameNotCLSCompliant, ProcessingErrorCode.rsDuplicateFieldName);
			Field field = null;
			bool flag = false;
			calculatedFieldStartIndex = -1;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (!(localName == "Field"))
					{
						break;
					}
					field = ReadField(names, context);
					if (field.IsCalculatedField)
					{
						if (calculatedFieldStartIndex < 0)
						{
							calculatedFieldStartIndex = dataFieldList.Count;
						}
						dataFieldList.Add(field);
					}
					else if (calculatedFieldStartIndex < 0)
					{
						dataFieldList.Add(field);
					}
					else
					{
						dataFieldList.Insert(calculatedFieldStartIndex, field);
						calculatedFieldStartIndex++;
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("Fields" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (0 > calculatedFieldStartIndex)
			{
				calculatedFieldStartIndex = dataFieldList.Count;
			}
			return dataFieldList;
		}

		private Field ReadField(CLSUniqueNameValidator names, PublishingContextStruct context)
		{
			Global.Tracer.Assert(ObjectType.DataSet == context.ObjectType);
			string objectName = context.ObjectName;
			Field field = new Field();
			context.ObjectType = ObjectType.Field;
			string text = null;
			field.Name = m_reader.GetAttribute("Name");
			Global.Tracer.Assert(field.Name != null, "Name is a mandatory attribute of field elements");
			context.ObjectName = field.Name;
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName = m_reader.LocalName;
						if (!(localName == "DataField"))
						{
							if (localName == "Value")
							{
								text = m_reader.ReadString();
								if (text != null)
								{
									context.ObjectName = text;
									field.Value = ReadExpression(parseExtended: true, text, m_reader.LocalName, objectName, ExpressionParser.ExpressionType.FieldValue, ExpressionParser.ConstantType.String, context);
								}
							}
						}
						else
						{
							field.DataField = m_reader.ReadString();
							names.Validate(field.Name, field.DataField, objectName, m_errorContext);
						}
						break;
					}
					case XmlNodeType.EndElement:
						if ("Field" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			if (field.DataField != null == (text != null))
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidField, Severity.Error, context.ObjectType, field.Name, null, objectName);
			}
			return field;
		}

		private FilterList ReadFilters(ExpressionParser.ExpressionType expressionType, PublishingContextStruct context)
		{
			FilterList filterList = new FilterList();
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "Filter")
					{
						filterList.Add(ReadFilter(expressionType, context));
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("Filters" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return filterList;
		}

		private Filter ReadFilter(ExpressionParser.ExpressionType expressionType, PublishingContextStruct context)
		{
			m_hasFilters = true;
			Filter filter = new Filter();
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (m_reader.LocalName)
					{
					case "FilterExpression":
						filter.Expression = ReadExpression(m_reader.LocalName, expressionType, ExpressionParser.ConstantType.String, context);
						break;
					case "Operator":
						filter.Operator = ReadOperator();
						break;
					case "FilterValues":
						filter.Values = ReadFilterValues(expressionType, context);
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("Filter" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			int num = (filter.Values != null) ? filter.Values.Count : 0;
			switch (filter.Operator)
			{
			case Filter.Operators.Equal:
			case Filter.Operators.Like:
			case Filter.Operators.GreaterThan:
			case Filter.Operators.GreaterThanOrEqual:
			case Filter.Operators.LessThan:
			case Filter.Operators.LessThanOrEqual:
			case Filter.Operators.TopN:
			case Filter.Operators.BottomN:
			case Filter.Operators.TopPercent:
			case Filter.Operators.BottomPercent:
			case Filter.Operators.NotEqual:
				if (1 != num)
				{
					m_errorContext.Register(ProcessingErrorCode.rsInvalidNumberOfFilterValues, Severity.Error, context.ObjectType, context.ObjectName, "FilterValues", filter.Operator.ToString(), Convert.ToString(1, CultureInfo.InvariantCulture));
				}
				break;
			case Filter.Operators.Between:
				if (2 != num)
				{
					m_errorContext.Register(ProcessingErrorCode.rsInvalidNumberOfFilterValues, Severity.Error, context.ObjectType, context.ObjectName, "FilterValues", filter.Operator.ToString(), Convert.ToString(2, CultureInfo.InvariantCulture));
				}
				break;
			}
			if (ExpressionParser.ExpressionType.GroupingFilters == expressionType && filter.Expression.HasRecursiveAggregates())
			{
				m_hasSpecialRecursiveAggregates = true;
			}
			return filter;
		}

		private ExpressionInfoList ReadFilterValues(ExpressionParser.ExpressionType expressionType, PublishingContextStruct context)
		{
			ExpressionInfoList expressionInfoList = new ExpressionInfoList();
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "FilterValue")
					{
						ExpressionInfo expressionInfo = ReadExpression(m_reader.LocalName, expressionType, ExpressionParser.ConstantType.String, context);
						expressionInfoList.Add(expressionInfo);
						if (ExpressionParser.ExpressionType.GroupingFilters == expressionType && expressionInfo.HasRecursiveAggregates())
						{
							m_hasSpecialRecursiveAggregates = true;
						}
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("FilterValues" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return expressionInfoList;
		}

		private void ReadBody(Report report, PublishingContextStruct context)
		{
			if (m_reader.IsEmptyElement)
			{
				return;
			}
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (m_reader.LocalName)
					{
					case "ReportItems":
						ReadReportItems(null, report, report.ReportItems, context, null);
						break;
					case "Height":
						report.Height = ReadSize();
						break;
					case "Columns":
					{
						int columns = m_reader.ReadInteger();
						if (PublishingValidator.ValidateColumns(columns, context.ObjectType, context.ObjectName, "Columns", m_errorContext))
						{
							report.Columns = columns;
						}
						break;
					}
					case "ColumnSpacing":
						report.ColumnSpacing = ReadSize();
						break;
					case "Style":
					{
						StyleInformation styleInformation = ReadStyle(context);
						styleInformation.Filter(StyleOwnerType.Body, hasNoRows: false);
						report.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, m_errorContext);
						break;
					}
					}
					break;
				case XmlNodeType.EndElement:
					if ("Body" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
		}

		private ParameterDefList ReadReportParameters(PublishingContextStruct context)
		{
			ParameterDefList parameterDefList = new ParameterDefList();
			CLSUniqueNameValidator reportParameterNames = new CLSUniqueNameValidator(ProcessingErrorCode.rsInvalidNameNotCLSCompliant, ProcessingErrorCode.rsDuplicateReportParameterName);
			bool flag = false;
			int num = 0;
			Hashtable parameterNames = new Hashtable();
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "ReportParameter")
					{
						parameterDefList.Add(ReadReportParameter(reportParameterNames, parameterNames, context, num));
						num++;
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("ReportParameters" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return parameterDefList;
		}

		private ParameterDef ReadReportParameter(CLSUniqueNameValidator reportParameterNames, Hashtable parameterNames, PublishingContextStruct context, int count)
		{
			ParameterDef parameterDef = new ParameterDef();
			parameterDef.Name = m_reader.GetAttribute("Name");
			context.ObjectType = ObjectType.ReportParameter;
			context.ObjectName = parameterDef.Name;
			reportParameterNames.Validate(context.ObjectType, context.ObjectName, m_errorContext);
			string type = null;
			string nullable = null;
			bool flag = false;
			string allowBlank = null;
			bool flag2 = false;
			string prompt = null;
			List<string> list = null;
			string multiValue = null;
			string usedInQuery = null;
			bool isComplex = false;
			bool flag3 = false;
			DataSetReference validValueDataSet = null;
			DataSetReference defaultDataSet = null;
			bool flag4 = false;
			bool flag5 = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (m_reader.LocalName)
					{
					case "DataType":
						type = m_reader.ReadString();
						break;
					case "Nullable":
						nullable = m_reader.ReadString();
						break;
					case "DefaultValue":
						flag = true;
						list = ReadDefaultValue(context, parameterDef, parameterNames, ref isComplex, out defaultDataSet);
						break;
					case "AllowBlank":
						allowBlank = m_reader.ReadString();
						break;
					case "Prompt":
						flag2 = true;
						prompt = m_reader.ReadString();
						break;
					case "ValidValues":
						flag3 = ReadValidValues(context, parameterDef, parameterNames, ref isComplex, out validValueDataSet);
						break;
					case "Hidden":
						flag4 = m_reader.ReadBoolean();
						break;
					case "MultiValue":
						multiValue = m_reader.ReadString();
						break;
					case "UsedInQuery":
						usedInQuery = m_reader.ReadString();
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("ReportParameter" == m_reader.LocalName)
					{
						flag5 = true;
					}
					break;
				}
			}
			while (!flag5);
			parameterDef.Parse(parameterDef.Name, list, type, nullable, prompt, null, allowBlank, multiValue, usedInQuery, flag4, m_errorContext, CultureInfo.InvariantCulture);
			if (parameterDef.Nullable && !flag)
			{
				parameterDef.DefaultValues = new object[1];
				parameterDef.DefaultValues[0] = null;
			}
			if (parameterDef.DataType == DataType.Boolean)
			{
				validValueDataSet = null;
			}
			if (!flag2 && !flag && !flag4 && (!parameterDef.Nullable || (parameterDef.ValidValuesValueExpressions != null && !flag3)))
			{
				m_errorContext.Register(ProcessingErrorCode.rsMissingParameterDefault, Severity.Error, context.ObjectType, context.ObjectName, null);
			}
			if (parameterDef.Nullable && parameterDef.MultiValue)
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidMultiValueParameter, Severity.Error, context.ObjectType, context.ObjectName, null);
			}
			if (!parameterDef.MultiValue && list != null && list.Count > 1)
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidDefaultValue, Severity.Error, context.ObjectType, context.ObjectName, null);
			}
			if (defaultDataSet != null || validValueDataSet != null)
			{
				m_dynamicParameters.Add(new DynamicParameter(validValueDataSet, defaultDataSet, count, isComplex));
			}
			if (!parameterNames.ContainsKey(parameterDef.Name))
			{
				parameterNames.Add(parameterDef.Name, count);
			}
			return parameterDef;
		}

		private List<string> ReadDefaultValue(PublishingContextStruct context, ParameterDef parameter, Hashtable parameterNames, ref bool isComplex, out DataSetReference defaultDataSet)
		{
			bool flag = false;
			bool flag2 = false;
			List<string> result = null;
			defaultDataSet = null;
			if (!m_reader.IsEmptyElement)
			{
				bool flag3 = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName = m_reader.LocalName;
						if (!(localName == "DataSetReference"))
						{
							if (localName == "Values")
							{
								flag2 = true;
								result = ReadValues(context, parameter, parameterNames, out isComplex);
							}
						}
						else
						{
							flag = true;
							defaultDataSet = ReadDataSetReference();
						}
						break;
					}
					case XmlNodeType.EndElement:
						if ("DefaultValue" == m_reader.LocalName)
						{
							flag3 = true;
						}
						break;
					}
				}
				while (!flag3);
			}
			if ((!flag && !flag2) || (flag && flag2))
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidDefaultValue, Severity.Error, context.ObjectType, context.ObjectName, "DefaultValue");
			}
			return result;
		}

		private List<string> ReadValues(PublishingContextStruct context, ParameterDef parameter, Hashtable parameterNames, out bool isComplex)
		{
			List<string> list = null;
			ExpressionInfoList expressionInfoList = new ExpressionInfoList();
			ExpressionInfo expressionInfo = null;
			bool dynamic = false;
			Hashtable dependencies = null;
			bool flag = false;
			isComplex = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "Value")
					{
						expressionInfo = ReadParameterExpression(m_reader.LocalName, context, parameter, parameterNames, ref dependencies, ref dynamic, ref isComplex);
						expressionInfoList.Add(expressionInfo);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("Values" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (isComplex && parameterNames.Count > 0)
			{
				dependencies = (Hashtable)parameterNames.Clone();
			}
			if (dynamic)
			{
				parameter.DefaultExpressions = expressionInfoList;
			}
			else
			{
				list = new List<string>(expressionInfoList.Count);
				for (int i = 0; i < expressionInfoList.Count; i++)
				{
					list.Add(expressionInfoList[i].Value);
				}
			}
			parameter.Dependencies = dependencies;
			return list;
		}

		private bool ReadValidValues(PublishingContextStruct context, ParameterDef parameter, Hashtable parameterNames, ref bool isComplex, out DataSetReference validValueDataSet)
		{
			bool flag = false;
			bool flag2 = false;
			bool containsExplicitNull = false;
			validValueDataSet = null;
			if (!m_reader.IsEmptyElement)
			{
				bool flag3 = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName = m_reader.LocalName;
						if (!(localName == "DataSetReference"))
						{
							if (localName == "ParameterValues")
							{
								flag2 = true;
								ReadParameterValues(context, parameter, parameterNames, ref isComplex, ref containsExplicitNull);
							}
						}
						else
						{
							flag = true;
							validValueDataSet = ReadDataSetReference();
						}
						break;
					}
					case XmlNodeType.EndElement:
						if ("ValidValues" == m_reader.LocalName)
						{
							flag3 = true;
						}
						break;
					}
				}
				while (!flag3);
			}
			if ((!flag && !flag2) || (flag && flag2))
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidValidValues, Severity.Error, context.ObjectType, context.ObjectName, "ValidValues");
			}
			return containsExplicitNull;
		}

		private DataSetReference ReadDataSetReference()
		{
			string dataSet = null;
			string valueAlias = null;
			string labelAlias = null;
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (m_reader.LocalName)
					{
					case "DataSetName":
						dataSet = m_reader.ReadString();
						break;
					case "ValueField":
						valueAlias = m_reader.ReadString();
						break;
					case "LabelField":
						labelAlias = m_reader.ReadString();
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("DataSetReference" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return new DataSetReference(dataSet, valueAlias, labelAlias);
		}

		private void ReadParameterValues(PublishingContextStruct context, ParameterDef parameter, Hashtable parameterNames, ref bool isComplex, ref bool containsExplicitNull)
		{
			ExpressionInfoList expressionInfoList = new ExpressionInfoList();
			ExpressionInfoList expressionInfoList2 = new ExpressionInfoList();
			Hashtable dependencies = null;
			bool dynamic = isComplex;
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (!(localName == "ParameterValue"))
					{
						break;
					}
					ExpressionInfo expressionInfo = null;
					ExpressionInfo value = null;
					if (!m_reader.IsEmptyElement)
					{
						bool flag2 = false;
						do
						{
							m_reader.Read();
							switch (m_reader.NodeType)
							{
							case XmlNodeType.Element:
								localName = m_reader.LocalName;
								if (!(localName == "Value"))
								{
									if (localName == "Label")
									{
										value = ReadParameterExpression(m_reader.LocalName, context, parameter, parameterNames, ref dependencies, ref dynamic, ref isComplex);
									}
								}
								else
								{
									expressionInfo = ReadParameterExpression(m_reader.LocalName, context, parameter, parameterNames, ref dependencies, ref dynamic, ref isComplex);
								}
								break;
							case XmlNodeType.EndElement:
								if ("ParameterValue" == m_reader.LocalName)
								{
									flag2 = true;
								}
								break;
							}
						}
						while (!flag2);
					}
					containsExplicitNull |= (expressionInfo == null);
					expressionInfoList.Add(expressionInfo);
					expressionInfoList2.Add(value);
					break;
				}
				case XmlNodeType.EndElement:
					if ("ParameterValues" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (isComplex && parameterNames.Count > 0)
			{
				dependencies = (Hashtable)parameterNames.Clone();
			}
			parameter.ValidValuesValueExpressions = expressionInfoList;
			parameter.ValidValuesLabelExpressions = expressionInfoList2;
			parameter.Dependencies = dependencies;
		}

		private ExpressionInfo ReadParameterExpression(string propertyName, PublishingContextStruct context, ParameterDef parameter, Hashtable parameterNames, ref Hashtable dependencies, ref bool dynamic, ref bool isComplex)
		{
			ExpressionInfo expressionInfo = null;
			string reportParameterName = null;
			bool flag = false;
			bool userCollectionReferenced;
			if (isComplex)
			{
				dynamic = true;
				expressionInfo = ReadExpression(propertyName, null, ExpressionParser.ExpressionType.ReportParameter, ExpressionParser.ConstantType.String, context, out userCollectionReferenced);
			}
			else
			{
				ExpressionParser.DetectionFlags detectionFlags = ExpressionParser.DetectionFlags.ParameterReference;
				detectionFlags |= ExpressionParser.DetectionFlags.UserReference;
				expressionInfo = ReadExpression(propertyName, null, ExpressionParser.ExpressionType.ReportParameter, ExpressionParser.ConstantType.String, context, detectionFlags, out bool reportParameterReferenced, out reportParameterName, out userCollectionReferenced);
				if (reportParameterReferenced)
				{
					dynamic = true;
					if (reportParameterName == null)
					{
						isComplex = true;
					}
					else if (!parameterNames.ContainsKey(reportParameterName))
					{
						flag = true;
					}
					else
					{
						if (dependencies == null)
						{
							dependencies = new Hashtable();
						}
						dependencies.Add(reportParameterName, parameterNames[reportParameterName]);
					}
				}
			}
			if (userCollectionReferenced)
			{
				if (parameter.Name != null && !m_reportParamUserProfile.Contains(parameter.Name))
				{
					m_reportParamUserProfile.Add(parameter.Name, true);
				}
				m_userReferenceLocation |= UserLocationFlags.ReportBody;
			}
			if (flag)
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidReportParameterDependency, Severity.Error, ObjectType.ReportParameter, parameter.Name, "ValidValues", reportParameterName);
			}
			return expressionInfo;
		}

		private ParameterValueList ReadParameters(PublishingContextStruct context, bool doClsValidation)
		{
			bool computed;
			return ReadParameters(context, omitAllowed: false, doClsValidation, out computed);
		}

		private ParameterValueList ReadParameters(PublishingContextStruct context, bool omitAllowed, bool doClsValidation, out bool computed)
		{
			computed = false;
			ParameterValueList parameterValueList = new ParameterValueList();
			ParameterNameValidator parameterNames = new ParameterNameValidator();
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "Parameter")
					{
						parameterValueList.Add(ReadParameter(parameterNames, context, omitAllowed, doClsValidation, out bool computed2));
						computed |= computed2;
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("Parameters" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return parameterValueList;
		}

		private ParameterValue ReadParameter(ParameterNameValidator parameterNames, PublishingContextStruct context, bool omitAllowed, bool doClsValidation, out bool computed)
		{
			computed = false;
			bool computed2 = false;
			bool computed3 = false;
			ParameterValue parameterValue = new ParameterValue();
			parameterValue.Name = m_reader.GetAttribute("Name");
			if (doClsValidation)
			{
				parameterNames.Validate(parameterValue.Name, context.ObjectType, context.ObjectName, m_errorContext);
			}
			parameterValue.Value = null;
			parameterValue.Omit = null;
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (!(localName == "Value"))
					{
						if (localName == "Omit" && omitAllowed)
						{
							parameterValue.Omit = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.Boolean, context, out computed3);
						}
					}
					else
					{
						parameterValue.Value = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out computed2);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("Parameter" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			computed = (computed2 || computed3);
			return parameterValue;
		}

		private PageSection ReadPageSection(bool isHeader, Report report, PublishingContextStruct context)
		{
			PageSection pageSection = new PageSection(isHeader, GenerateID(), GenerateID(), report);
			context.Location |= LocationFlags.InPageSection;
			context.ObjectType = pageSection.ObjectType;
			context.ObjectName = null;
			m_reportItemCollectionList.Add(pageSection.ReportItems);
			m_runningValueHolderList.Add(pageSection.ReportItems);
			m_reportLocationFlags = UserLocationFlags.ReportPageSection;
			bool computed = false;
			bool computed2 = false;
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (m_reader.LocalName)
					{
					case "Height":
						pageSection.Height = ReadSize();
						break;
					case "PrintOnFirstPage":
						pageSection.PrintOnFirstPage = m_reader.ReadBoolean();
						break;
					case "PrintOnLastPage":
						pageSection.PrintOnLastPage = m_reader.ReadBoolean();
						break;
					case "ReportItems":
						ReadReportItems(null, pageSection, pageSection.ReportItems, context, null, out computed);
						break;
					case "Style":
					{
						StyleInformation styleInformation = ReadStyle(context, out computed2);
						styleInformation.Filter(StyleOwnerType.PageSection, hasNoRows: false);
						pageSection.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, m_errorContext);
						break;
					}
					}
					break;
				case XmlNodeType.EndElement:
					if (isHeader)
					{
						if ("PageHeader" == m_reader.LocalName)
						{
							flag = true;
						}
					}
					else if ("PageFooter" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			pageSection.PostProcessEvaluate = ((computed || computed2) | m_pageSectionDrillthroughs);
			m_pageSectionDrillthroughs = false;
			m_reportLocationFlags = UserLocationFlags.ReportBody;
			return pageSection;
		}

		private void ReadReportItems(string propertyName, ReportItem parent, ReportItemCollection parentCollection, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget, out bool computed)
		{
			computed = false;
			int num = 0;
			bool flag = parent is Matrix;
			bool flag2 = parent is Table;
			bool flag3 = parent is CustomReportItem;
			bool flag4 = false;
			do
			{
				ReportItem reportItem = null;
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (m_reader.LocalName)
					{
					case "Line":
						num++;
						reportItem = ReadLine(parent, context);
						break;
					case "Rectangle":
						num++;
						reportItem = ReadRectangle(parent, context, textBoxesWithDefaultSortTarget);
						break;
					case "CustomReportItem":
						num++;
						if (!flag3)
						{
							reportItem = ReadCustomReportItem(parent, context, textBoxesWithDefaultSortTarget);
						}
						else
						{
							m_errorContext.Register(ProcessingErrorCode.rsInvalidAltReportItem, Severity.Error, context.ObjectType, context.ObjectName, propertyName);
						}
						break;
					case "Checkbox":
						num++;
						reportItem = ReadCheckbox(parent, context);
						break;
					case "Textbox":
						num++;
						reportItem = ReadTextbox(parent, context, textBoxesWithDefaultSortTarget);
						break;
					case "Image":
						num++;
						reportItem = ReadImage(parent, context);
						break;
					case "Subreport":
						num++;
						reportItem = ReadSubreport(parent, context);
						break;
					case "ActiveXControl":
						num++;
						reportItem = ReadActiveXControl(parent, context);
						break;
					case "List":
						num++;
						reportItem = ReadList(parent, context);
						break;
					case "Matrix":
						num++;
						reportItem = ReadMatrix(parent, context);
						break;
					case "Table":
						num++;
						reportItem = ReadTable(parent, context);
						break;
					case "OWCChart":
						num++;
						reportItem = ReadOWCChart(parent, context);
						break;
					case "Chart":
						num++;
						reportItem = ReadChart(parent, context);
						break;
					}
					if (flag && num > 1)
					{
						m_errorContext.Register(ProcessingErrorCode.rsMultiReportItemsInMatrixSection, Severity.Error, context.ObjectType, context.ObjectName, propertyName);
					}
					if (flag && (LocationFlags.InMatrixSubtotal & context.Location) != 0 && reportItem != null && !(reportItem is TextBox))
					{
						m_errorContext.Register(ProcessingErrorCode.rsInvalidMatrixSubtotalReportItem, Severity.Error, context.ObjectType, context.ObjectName, "Subtotal");
					}
					if (flag2 && num > 1)
					{
						m_errorContext.Register(ProcessingErrorCode.rsMultiReportItemsInTableCell, Severity.Error, context.ObjectType, context.ObjectName, propertyName);
					}
					if (flag3 && num > 1)
					{
						m_errorContext.Register(ProcessingErrorCode.rsMultiReportItemsInCustomReportItem, Severity.Error, context.ObjectType, context.ObjectName, propertyName);
					}
					if (reportItem != null)
					{
						computed |= reportItem.Computed;
						parentCollection.AddReportItem(reportItem);
						if (flag || flag2)
						{
							reportItem.IsFullSize = true;
						}
						if (flag3 && (parent.Parent is Matrix || parent.Parent is Table))
						{
							reportItem.IsFullSize = true;
						}
					}
					break;
				case XmlNodeType.EndElement:
					if ("ReportItems" == m_reader.LocalName || (flag3 && "AltReportItem" == m_reader.LocalName))
					{
						flag4 = true;
					}
					break;
				}
			}
			while (!flag4);
		}

		private void ReadReportItems(string propertyName, ReportItem parent, ReportItemCollection parentCollection, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget)
		{
			ReadReportItems(propertyName, parent, parentCollection, context, textBoxesWithDefaultSortTarget, out bool _);
		}

		private CustomReportItem ReadCustomReportItem(ReportItem parent, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget)
		{
			CustomReportItem customReportItem = new CustomReportItem(GenerateID(), GenerateID(), parent);
			customReportItem.Name = m_reader.GetAttribute("Name");
			context.ObjectType = customReportItem.ObjectType;
			context.ObjectName = customReportItem.Name;
			m_dataRegionCount++;
			m_reportItemNames.Validate(context.ObjectType, context.ObjectName, m_errorContext);
			bool flag = true;
			if ((context.Location & LocationFlags.InPageSection) != 0)
			{
				m_errorContext.Register(ProcessingErrorCode.rsCRIInPageSection, Severity.Error, context.ObjectType, context.ObjectName, null);
				flag = false;
			}
			m_reportItemCollectionList.Add(customReportItem.AltReportItem);
			m_aggregateHolderList.Add(customReportItem);
			m_runningValueHolderList.Add(customReportItem);
			m_runningValueHolderList.Add(customReportItem.AltReportItem);
			bool computed = false;
			bool computed2 = false;
			bool computed3 = false;
			bool computed4 = false;
			bool computed5 = false;
			bool computed6 = false;
			bool flag2 = false;
			ExpressionInfo expressionInfo = null;
			ExpressionInfo expressionInfo2 = null;
			TextBoxList textBoxList = new TextBoxList();
			if (!m_reader.IsEmptyElement)
			{
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (m_reader.LocalName)
						{
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context, out computed);
							customReportItem.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, m_errorContext);
							break;
						}
						case "Top":
							customReportItem.Top = ReadSize();
							break;
						case "Left":
							customReportItem.Left = ReadSize();
							break;
						case "Height":
							customReportItem.Height = ReadSize();
							break;
						case "Width":
							customReportItem.Width = ReadSize();
							break;
						case "ZIndex":
							customReportItem.ZIndex = m_reader.ReadInteger();
							break;
						case "Visibility":
							customReportItem.Visibility = ReadVisibility(context, out computed2);
							break;
						case "Label":
							expressionInfo = (customReportItem.Label = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out computed5));
							break;
						case "Bookmark":
							expressionInfo2 = (customReportItem.Bookmark = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out computed6));
							break;
						case "RepeatWith":
							customReportItem.RepeatedSibling = true;
							customReportItem.RepeatWith = m_reader.ReadString();
							break;
						case "Type":
							customReportItem.Type = m_reader.ReadString();
							break;
						case "AltReportItem":
							ReadReportItems("AltReportItem", customReportItem, customReportItem.AltReportItem, context, textBoxList, out computed3);
							Global.Tracer.Assert(1 <= customReportItem.AltReportItem.Count);
							break;
						case "CustomData":
							ReadCustomData(customReportItem, context);
							break;
						case "CustomProperties":
							customReportItem.CustomProperties = ReadCustomProperties(context, out computed4);
							break;
						case "DataElementName":
							customReportItem.DataElementName = m_reader.ReadString();
							break;
						case "DataElementOutput":
							customReportItem.DataElementOutputRDL = ReadDataElementOutputRDL();
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("CustomReportItem" == m_reader.LocalName)
						{
							flag2 = true;
						}
						break;
					}
				}
				while (!flag2);
			}
			customReportItem.Computed = true;
			if (!computed5 && expressionInfo != null && expressionInfo.Value != null)
			{
				m_hasLabels = true;
			}
			if (!computed6 && expressionInfo2 != null && expressionInfo2.Value != null)
			{
				m_hasBookmarks = true;
			}
			Global.Tracer.Assert(customReportItem.AltReportItem != null);
			if (customReportItem.AltReportItem.Count == 0)
			{
				Rectangle rectangle = new Rectangle(GenerateID(), GenerateID(), parent);
				rectangle.Name = customReportItem.Name + "_" + customReportItem.ID + "_" + rectangle.ID;
				m_reportItemNames.Validate(rectangle.ObjectType, rectangle.Name, m_errorContext);
				rectangle.Computed = false;
				Visibility visibility = new Visibility();
				ExpressionParser.ExpressionContext context2 = context.CreateExpressionContext(ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.Boolean, "Hidden", null);
				visibility.Hidden = m_reportCT.ParseExpression("true", context2, out bool userCollectionReferenced);
				Global.Tracer.Assert(!userCollectionReferenced);
				rectangle.Visibility = visibility;
				m_reportItemCollectionList.Add(rectangle.ReportItems);
				if (parent is Matrix || parent is Table)
				{
					rectangle.IsFullSize = true;
				}
				customReportItem.AltReportItem.AddReportItem(rectangle);
			}
			if (customReportItem.DataSetName != null)
			{
				SetSortTargetForTextBoxes(textBoxList, customReportItem);
			}
			else
			{
				textBoxesWithDefaultSortTarget?.AddRange(textBoxList);
			}
			if (!flag)
			{
				return null;
			}
			return customReportItem;
		}

		private void ReadCustomData(CustomReportItem crItem, PublishingContextStruct context)
		{
			_ = context.Location;
			context.Location = (context.Location | LocationFlags.InDataSet | LocationFlags.InDataRegion);
			if (m_scopeNames.Validate(isGrouping: false, context.ObjectName, context.ObjectType, context.ObjectName, m_errorContext))
			{
				m_reportScopes.Add(crItem.Name, crItem);
			}
			if ((context.Location & LocationFlags.InPageSection) != 0)
			{
				m_errorContext.Register(ProcessingErrorCode.rsDataRegionInPageSection, Severity.Error, context.ObjectType, context.ObjectName, null);
			}
			if ((context.Location & LocationFlags.InDetail) != 0)
			{
				m_errorContext.Register(ProcessingErrorCode.rsDataRegionInTableDetailRow, Severity.Error, context.ObjectType, context.ObjectName, null);
			}
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (m_reader.LocalName)
					{
					case "DataSetName":
						crItem.DataSetName = m_reader.ReadString();
						break;
					case "DataColumnGroupings":
						crItem.Columns = ReadCustomDataColumnOrRowGroupings(isColumn: true, crItem, context);
						break;
					case "DataRowGroupings":
						crItem.Rows = ReadCustomDataColumnOrRowGroupings(isColumn: false, crItem, context);
						break;
					case "DataRows":
						crItem.DataRowCells = ReadCustomDataRows(crItem, context);
						break;
					case "Filters":
						crItem.Filters = ReadFilters(ExpressionParser.ExpressionType.DataRegionFilters, context);
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("CustomData" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
		}

		private CustomReportItemHeadingList ReadCustomDataColumnOrRowGroupings(bool isColumn, CustomReportItem crItem, PublishingContextStruct context)
		{
			CustomReportItemHeadingList customReportItemHeadingList = new CustomReportItemHeadingList();
			int num = -1;
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "DataGroupings")
					{
						num = ReadCustomDataGroupings(isColumn, crItem, customReportItemHeadingList, context);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ((isColumn && "DataColumnGroupings" == m_reader.LocalName) || (!isColumn && "DataRowGroupings" == m_reader.LocalName))
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (isColumn)
			{
				crItem.ExpectedColumns = num;
			}
			else
			{
				crItem.ExpectedRows = num;
			}
			return customReportItemHeadingList;
		}

		private int ReadCustomDataGroupings(bool isColumn, CustomReportItem crItem, CustomReportItemHeadingList crGroupingList, PublishingContextStruct context)
		{
			bool flag = false;
			int num = 0;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "DataGrouping")
					{
						crGroupingList.Add(ReadCustomDataGrouping(isColumn, crItem, context, out int groupingLeafs));
						num += groupingLeafs;
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("DataGroupings" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return num;
		}

		private CustomReportItemHeading ReadCustomDataGrouping(bool isColumn, CustomReportItem crItem, PublishingContextStruct context, out int groupingLeafs)
		{
			CustomReportItemHeading customReportItemHeading = new CustomReportItemHeading(GenerateID(), crItem);
			m_runningValueHolderList.Add(customReportItemHeading);
			customReportItemHeading.IsColumn = isColumn;
			groupingLeafs = 1;
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (m_reader.LocalName)
						{
						case "Static":
							customReportItemHeading.Static = m_reader.ReadBoolean();
							break;
						case "Grouping":
							customReportItemHeading.Grouping = ReadGrouping(context);
							break;
						case "Sorting":
							customReportItemHeading.Sorting = ReadSorting(context);
							break;
						case "Subtotal":
							customReportItemHeading.Subtotal = m_reader.ReadBoolean();
							break;
						case "CustomProperties":
							customReportItemHeading.CustomProperties = ReadCustomProperties(context);
							break;
						case "DataGroupings":
							customReportItemHeading.InnerHeadings = new CustomReportItemHeadingList();
							groupingLeafs = ReadCustomDataGroupings(isColumn, crItem, customReportItemHeading.InnerHeadings, context);
							customReportItemHeading.HeadingSpan = groupingLeafs;
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("DataGrouping" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			if (CanMergeGroupingAndSorting(customReportItemHeading.Grouping, customReportItemHeading.Sorting))
			{
				customReportItemHeading.Grouping.GroupAndSort = true;
				customReportItemHeading.Grouping.SortDirections = customReportItemHeading.Sorting.SortDirections;
				customReportItemHeading.Sorting = null;
			}
			if (customReportItemHeading.Sorting != null)
			{
				m_hasSorting = true;
			}
			return customReportItemHeading;
		}

		private DataCellsList ReadCustomDataRows(CustomReportItem crItem, PublishingContextStruct context)
		{
			DataCellsList dataCellsList = new DataCellsList();
			bool flag = false;
			int num = 0;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "DataRow")
					{
						dataCellsList.Add(ReadCustomDataRow(crItem, num++, context));
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("DataRows" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return dataCellsList;
		}

		private DataCellList ReadCustomDataRow(CustomReportItem crItem, int rowIndex, PublishingContextStruct context)
		{
			DataCellList dataCellList = new DataCellList();
			bool flag = false;
			int num = 0;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "DataCell")
					{
						dataCellList.Add(ReadCustomDataCell(crItem, rowIndex, num++, context));
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("DataRow" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return dataCellList;
		}

		private DataValueCRIList ReadCustomDataCell(CustomReportItem crItem, int rowIndex, int columnIndex, PublishingContextStruct context)
		{
			DataValueCRIList dataValueCRIList = new DataValueCRIList();
			dataValueCRIList.RDLRowIndex = rowIndex;
			dataValueCRIList.RDLColumnIndex = columnIndex;
			int num = 0;
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "DataValue")
					{
						dataValueCRIList.Add(ReadDataValue(isCustomProperty: false, ++num, context));
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("DataCell" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return dataValueCRIList;
		}

		private Line ReadLine(ReportItem parent, PublishingContextStruct context)
		{
			Line line = new Line(GenerateID(), parent);
			line.Name = m_reader.GetAttribute("Name");
			context.ObjectType = line.ObjectType;
			context.ObjectName = line.Name;
			m_reportItemNames.Validate(context.ObjectType, context.ObjectName, m_errorContext);
			bool computed = false;
			bool computed2 = false;
			bool computed3 = false;
			bool computedBookmark = false;
			bool computed4 = false;
			bool flag = false;
			ExpressionInfo expressionInfo = null;
			ExpressionInfo expressionInfo2 = null;
			if (!m_reader.IsEmptyElement)
			{
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (m_reader.LocalName)
						{
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context, out computed);
							styleInformation.Filter(StyleOwnerType.Line, hasNoRows: false);
							line.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, m_errorContext);
							break;
						}
						case "Top":
							line.Top = ReadSize();
							break;
						case "Left":
							line.Left = ReadSize();
							break;
						case "Height":
							line.Height = ReadSize();
							break;
						case "Width":
							line.Width = ReadSize();
							break;
						case "ZIndex":
							line.ZIndex = m_reader.ReadInteger();
							break;
						case "Visibility":
							line.Visibility = ReadVisibility(context, out computed2);
							break;
						case "Label":
							expressionInfo = (line.Label = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out computed3));
							break;
						case "Bookmark":
							expressionInfo2 = (line.Bookmark = ReadBookmarkExpression(context, out computedBookmark));
							break;
						case "RepeatWith":
							line.RepeatedSibling = true;
							line.RepeatWith = m_reader.ReadString();
							break;
						case "Custom":
							line.Custom = m_reader.ReadCustomXml();
							break;
						case "CustomProperties":
							line.CustomProperties = ReadCustomProperties(context, out computed4);
							break;
						case "DataElementName":
							line.DataElementName = m_reader.ReadString();
							break;
						case "DataElementOutput":
							line.DataElementOutputRDL = ReadDataElementOutputRDL();
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("Line" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			line.Computed = (computed || computed2 || computed3 || computedBookmark || computed4);
			if (!computed3 && expressionInfo != null && expressionInfo.Value != null)
			{
				m_hasLabels = true;
			}
			if (!computedBookmark && expressionInfo2 != null && expressionInfo2.Value != null)
			{
				m_hasBookmarks = true;
			}
			return line;
		}

		private Rectangle ReadRectangle(ReportItem parent, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget)
		{
			Rectangle rectangle = new Rectangle(GenerateID(), GenerateID(), parent);
			rectangle.Name = m_reader.GetAttribute("Name");
			context.ObjectType = rectangle.ObjectType;
			context.ObjectName = rectangle.Name;
			m_reportItemNames.Validate(context.ObjectType, context.ObjectName, m_errorContext);
			m_reportItemCollectionList.Add(rectangle.ReportItems);
			m_runningValueHolderList.Add(rectangle.ReportItems);
			bool computed = false;
			bool computed2 = false;
			bool computed3 = false;
			bool computedBookmark = false;
			bool computed4 = false;
			bool computed5 = false;
			bool computed6 = false;
			bool flag = false;
			string text = null;
			ExpressionInfo expressionInfo = null;
			ExpressionInfo expressionInfo2 = null;
			if (!m_reader.IsEmptyElement)
			{
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (m_reader.LocalName)
						{
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context, out computed);
							styleInformation.Filter(StyleOwnerType.Rectangle, hasNoRows: false);
							rectangle.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, m_errorContext);
							break;
						}
						case "Top":
							rectangle.Top = ReadSize();
							break;
						case "Left":
							rectangle.Left = ReadSize();
							break;
						case "Height":
							rectangle.Height = ReadSize();
							break;
						case "Width":
							rectangle.Width = ReadSize();
							break;
						case "ZIndex":
							rectangle.ZIndex = m_reader.ReadInteger();
							break;
						case "Visibility":
							rectangle.Visibility = ReadVisibility(context, out computed2);
							break;
						case "ToolTip":
							rectangle.ToolTip = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out computed4);
							break;
						case "Label":
							expressionInfo = (rectangle.Label = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out computed3));
							break;
						case "LinkToChild":
							text = m_reader.ReadString();
							break;
						case "Bookmark":
							expressionInfo2 = (rectangle.Bookmark = ReadBookmarkExpression(context, out computedBookmark));
							break;
						case "RepeatWith":
							rectangle.RepeatedSibling = true;
							rectangle.RepeatWith = m_reader.ReadString();
							break;
						case "Custom":
							rectangle.Custom = m_reader.ReadCustomXml();
							break;
						case "CustomProperties":
							rectangle.CustomProperties = ReadCustomProperties(context, out computed6);
							break;
						case "ReportItems":
							ReadReportItems(null, rectangle, rectangle.ReportItems, context, textBoxesWithDefaultSortTarget, out computed5);
							break;
						case "PageBreakAtStart":
							rectangle.PageBreakAtStart = m_reader.ReadBoolean();
							break;
						case "PageBreakAtEnd":
							rectangle.PageBreakAtEnd = m_reader.ReadBoolean();
							break;
						case "DataElementName":
							rectangle.DataElementName = m_reader.ReadString();
							break;
						case "DataElementOutput":
							rectangle.DataElementOutputRDL = ReadDataElementOutputRDL();
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("Rectangle" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			rectangle.Computed = ((computed || computed2 || computed3 || computedBookmark || computed5 || computed4 || computed6) | rectangle.PageBreakAtStart | rectangle.PageBreakAtEnd);
			if (!computed3 && expressionInfo != null && expressionInfo.Value != null)
			{
				m_hasLabels = true;
			}
			if (!computedBookmark && expressionInfo2 != null && expressionInfo2.Value != null)
			{
				m_hasBookmarks = true;
			}
			if (expressionInfo != null && text != null)
			{
				rectangle.ReportItems.LinkToChild = text;
			}
			return rectangle;
		}

		private CheckBox ReadCheckbox(ReportItem parent, PublishingContextStruct context)
		{
			CheckBox checkBox = new CheckBox(GenerateID(), parent);
			checkBox.Name = m_reader.GetAttribute("Name");
			context.ObjectType = checkBox.ObjectType;
			context.ObjectName = checkBox.Name;
			m_reportItemNames.Validate(context.ObjectType, context.ObjectName, m_errorContext);
			bool computed = false;
			bool computed2 = false;
			bool computed3 = false;
			bool computedBookmark = false;
			bool computed4 = false;
			bool computed5 = false;
			bool computed6 = false;
			bool flag = false;
			ExpressionInfo expressionInfo = null;
			ExpressionInfo expressionInfo2 = null;
			if (!m_reader.IsEmptyElement)
			{
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (m_reader.LocalName)
						{
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context, out computed);
							styleInformation.Filter(StyleOwnerType.Checkbox, hasNoRows: false);
							checkBox.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, m_errorContext);
							break;
						}
						case "Top":
							checkBox.Top = ReadSize();
							break;
						case "Left":
							checkBox.Left = ReadSize();
							break;
						case "ZIndex":
							checkBox.ZIndex = m_reader.ReadInteger();
							break;
						case "Visibility":
							checkBox.Visibility = ReadVisibility(context, out computed2);
							break;
						case "ToolTip":
							checkBox.ToolTip = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out computed4);
							break;
						case "Label":
							expressionInfo = (checkBox.Label = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out computed3));
							break;
						case "Bookmark":
							expressionInfo2 = (checkBox.Bookmark = ReadBookmarkExpression(context, out computedBookmark));
							break;
						case "RepeatWith":
							checkBox.RepeatedSibling = true;
							checkBox.RepeatWith = m_reader.ReadString();
							break;
						case "Custom":
							checkBox.Custom = m_reader.ReadCustomXml();
							break;
						case "CustomProperties":
							checkBox.CustomProperties = ReadCustomProperties(context, out computed6);
							break;
						case "Value":
							checkBox.Value = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.Boolean, context, out computed5);
							break;
						case "HideDuplicates":
						{
							string text = m_reader.ReadString();
							if ((context.Location & LocationFlags.InPageSection) != 0 || text == null || text.Length <= 0)
							{
								checkBox.HideDuplicates = null;
							}
							else
							{
								checkBox.HideDuplicates = text;
							}
							break;
						}
						case "DataElementName":
							checkBox.DataElementName = m_reader.ReadString();
							break;
						case "DataElementOutput":
							checkBox.DataElementOutputRDL = ReadDataElementOutputRDL();
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("Checkbox" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			checkBox.Computed = ((computed || computed2 || computed3 || computed4 || computedBookmark || computed5 || computed6) | (checkBox.HideDuplicates != null));
			if (!computed3 && expressionInfo != null && expressionInfo.Value != null)
			{
				m_hasLabels = true;
			}
			if (!computedBookmark && expressionInfo2 != null && expressionInfo2.Value != null)
			{
				m_hasBookmarks = true;
			}
			return checkBox;
		}

		private TextBox ReadTextbox(ReportItem parent, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget)
		{
			TextBox textBox = new TextBox(GenerateID(), parent);
			textBox.Name = m_reader.GetAttribute("Name");
			context.ObjectType = textBox.ObjectType;
			context.ObjectName = textBox.Name;
			m_reportItemNames.Validate(context.ObjectType, context.ObjectName, m_errorContext);
			Global.Tracer.Assert(!m_reportCT.ValueReferenced);
			bool computed = false;
			bool computed2 = false;
			bool computed3 = false;
			bool computed4 = false;
			bool computedBookmark = false;
			bool computed5 = false;
			bool computed6 = false;
			bool computed7 = false;
			bool computed8 = false;
			bool flag = false;
			ExpressionInfo expressionInfo = null;
			ExpressionInfo expressionInfo2 = null;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (m_reader.LocalName)
					{
					case "Style":
					{
						StyleInformation styleInformation = ReadStyle(context, out computed);
						styleInformation.Filter(StyleOwnerType.Textbox, hasNoRows: false);
						textBox.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, m_errorContext);
						break;
					}
					case "Action":
					{
						int computedIndex = -1;
						bool missingLabel = false;
						ActionItem actionItem = ReadActionItem(context, out computed2, ref computedIndex, ref missingLabel);
						textBox.Action = new Action(actionItem, computed2);
						break;
					}
					case "ActionInfo":
						textBox.Action = ReadAction(context, StyleOwnerType.Textbox, out computed2);
						break;
					case "Top":
						textBox.Top = ReadSize();
						break;
					case "Left":
						textBox.Left = ReadSize();
						break;
					case "Height":
						textBox.Height = ReadSize();
						break;
					case "Width":
						textBox.Width = ReadSize();
						break;
					case "ZIndex":
						textBox.ZIndex = m_reader.ReadInteger();
						break;
					case "Visibility":
						textBox.Visibility = ReadVisibility(context, out computed3);
						break;
					case "ToolTip":
						textBox.ToolTip = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out computed5);
						break;
					case "Label":
						expressionInfo = (textBox.Label = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out computed4));
						break;
					case "Bookmark":
						expressionInfo2 = (textBox.Bookmark = ReadBookmarkExpression(context, out computedBookmark));
						break;
					case "RepeatWith":
						textBox.RepeatedSibling = true;
						textBox.RepeatWith = m_reader.ReadString();
						break;
					case "Custom":
						textBox.Custom = m_reader.ReadCustomXml();
						break;
					case "CustomProperties":
						textBox.CustomProperties = ReadCustomProperties(context, out computed8);
						break;
					case "Value":
					{
						ExpressionInfo expressionInfo3 = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out computed6);
						if (expressionInfo3 != null)
						{
							textBox.Value = expressionInfo3;
							if (expressionInfo3.Type != ExpressionInfo.Types.Constant)
							{
								textBox.Formula = textBox.Value.OriginalText;
							}
						}
						break;
					}
					case "CanGrow":
						textBox.CanGrow = m_reader.ReadBoolean();
						break;
					case "CanShrink":
						textBox.CanShrink = m_reader.ReadBoolean();
						break;
					case "HideDuplicates":
					{
						string text = m_reader.ReadString();
						if ((context.Location & LocationFlags.InPageSection) != 0 || text == null || text.Length <= 0)
						{
							textBox.HideDuplicates = null;
						}
						else
						{
							textBox.HideDuplicates = text;
						}
						break;
					}
					case "ToggleImage":
						textBox.InitialToggleState = ReadToggleImage(context, out computed7);
						break;
					case "UserSort":
						ReadUserSort(context, textBox, textBoxesWithDefaultSortTarget);
						m_hasUserSort = true;
						break;
					case "DataElementName":
						textBox.DataElementName = m_reader.ReadString();
						break;
					case "DataElementOutput":
						textBox.DataElementOutputRDL = ReadDataElementOutputRDL();
						break;
					case "DataElementStyle":
					{
						ReportItem.DataElementStylesRDL dataElementStylesRDL = ReadDataElementStyleRDL();
						if (ReportItem.DataElementStylesRDL.Auto != dataElementStylesRDL)
						{
							textBox.OverrideReportDataElementStyle = true;
							Global.Tracer.Assert(dataElementStylesRDL == ReportItem.DataElementStylesRDL.AttributeNormal || ReportItem.DataElementStylesRDL.ElementNormal == dataElementStylesRDL);
							textBox.DataElementStyleAttribute = (dataElementStylesRDL == ReportItem.DataElementStylesRDL.AttributeNormal);
						}
						break;
					}
					}
					break;
				case XmlNodeType.EndElement:
					if ("Textbox" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			textBox.Computed = ((computed || computed2 || computed3 || computed8 || computed4 || computedBookmark || computed5 || computed6 || computed7) | (textBox.UserSort != null) | (textBox.HideDuplicates != null));
			textBox.ValueReferenced = m_reportCT.ValueReferenced;
			m_reportCT.ResetValueReferencedFlag();
			if (!computed4 && expressionInfo != null && expressionInfo.Value != null)
			{
				m_hasLabels = true;
			}
			if (!computedBookmark && expressionInfo2 != null && expressionInfo2.Value != null)
			{
				m_hasBookmarks = true;
			}
			return textBox;
		}

		private void ReadUserSort(PublishingContextStruct context, TextBox textbox, TextBoxList textBoxesWithDefaultSortTarget)
		{
			bool flag = (context.Location & LocationFlags.InPageSection) != 0;
			bool flag2 = false;
			EndUserSort endUserSort = new EndUserSort();
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (m_reader.LocalName)
					{
					case "SortExpression":
					{
						endUserSort.SortExpression = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.SortExpression, ExpressionParser.ConstantType.String, context, out bool _);
						break;
					}
					case "SortExpressionScope":
						endUserSort.SortExpressionScopeString = m_reader.ReadString();
						break;
					case "SortTarget":
						m_hasUserSortPeerScopes = true;
						endUserSort.SortTargetString = m_reader.ReadString();
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("UserSort" == m_reader.LocalName)
					{
						flag2 = true;
					}
					break;
				}
			}
			while (!flag2);
			if (flag)
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidTextboxInPageSection, Severity.Error, textbox.ObjectType, textbox.Name, "UserSort");
				return;
			}
			textbox.UserSort = endUserSort;
			if (endUserSort.SortTargetString == null)
			{
				textBoxesWithDefaultSortTarget?.Add(textbox);
			}
			else
			{
				m_textBoxesWithUserSortTarget.Add(textbox);
			}
		}

		private void SetSortTargetForTextBoxes(TextBoxList textBoxes, ISortFilterScope target)
		{
			if (textBoxes != null)
			{
				for (int i = 0; i < textBoxes.Count; i++)
				{
					textBoxes[i].UserSort.SetSortTarget(target);
				}
			}
		}

		private ExpressionInfo ReadToggleImage(PublishingContextStruct context, out bool computed)
		{
			computed = false;
			m_static = true;
			ExpressionInfo result = null;
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "InitialState")
					{
						result = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.Boolean, context, out computed);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("ToggleImage" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return result;
		}

		private Image ReadImage(ReportItem parent, PublishingContextStruct context)
		{
			Image image = new Image(GenerateID(), parent);
			image.Name = m_reader.GetAttribute("Name");
			context.ObjectType = image.ObjectType;
			context.ObjectName = image.Name;
			m_reportItemNames.Validate(context.ObjectType, context.ObjectName, m_errorContext);
			bool computed = false;
			bool computed2 = false;
			bool computed3 = false;
			bool computed4 = false;
			bool computedBookmark = false;
			bool computed5 = false;
			bool computed6 = false;
			bool computed7 = false;
			bool computed8 = false;
			bool flag = false;
			ExpressionInfo expressionInfo = null;
			ExpressionInfo expressionInfo2 = null;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (m_reader.LocalName)
					{
					case "Style":
					{
						StyleInformation styleInformation = ReadStyle(context, out computed);
						styleInformation.Filter(StyleOwnerType.Image, hasNoRows: false);
						image.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, m_errorContext);
						break;
					}
					case "Action":
					{
						int computedIndex = -1;
						bool missingLabel = false;
						ActionItem actionItem = ReadActionItem(context, out computed2, ref computedIndex, ref missingLabel);
						image.Action = new Action(actionItem, computed2);
						break;
					}
					case "ActionInfo":
						image.Action = ReadAction(context, StyleOwnerType.Image, out computed2);
						break;
					case "Top":
						image.Top = ReadSize();
						break;
					case "Left":
						image.Left = ReadSize();
						break;
					case "Height":
						image.Height = ReadSize();
						break;
					case "Width":
						image.Width = ReadSize();
						break;
					case "ZIndex":
						image.ZIndex = m_reader.ReadInteger();
						break;
					case "Visibility":
						image.Visibility = ReadVisibility(context, out computed3);
						break;
					case "ToolTip":
						image.ToolTip = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out computed5);
						break;
					case "Label":
						expressionInfo = (image.Label = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out computed4));
						break;
					case "Bookmark":
						expressionInfo2 = (image.Bookmark = ReadBookmarkExpression(context, out computedBookmark));
						break;
					case "RepeatWith":
						image.RepeatedSibling = true;
						image.RepeatWith = m_reader.ReadString();
						break;
					case "Custom":
						image.Custom = m_reader.ReadCustomXml();
						break;
					case "CustomProperties":
						image.CustomProperties = ReadCustomProperties(context, out computed8);
						break;
					case "Source":
						image.Source = ReadSource();
						break;
					case "Value":
						image.Value = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out computed6);
						break;
					case "MIMEType":
						image.MIMEType = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out computed7);
						break;
					case "Sizing":
						image.Sizing = ReadSizing();
						break;
					case "DataElementName":
						image.DataElementName = m_reader.ReadString();
						break;
					case "DataElementOutput":
						image.DataElementOutputRDL = ReadDataElementOutputRDL();
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("Image" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (Image.SourceType.Database == image.Source)
			{
				Global.Tracer.Assert(image.Value != null);
				if (ExpressionInfo.Types.Constant == image.Value.Type)
				{
					m_errorContext.Register(ProcessingErrorCode.rsBinaryConstant, Severity.Error, context.ObjectType, context.ObjectName, "Value");
				}
				if (!PublishingValidator.ValidateMimeType(image.MIMEType, context.ObjectType, context.ObjectName, "MIMEType", m_errorContext))
				{
					image.MIMEType = null;
				}
			}
			else
			{
				if (image.Source == Image.SourceType.External && ExpressionInfo.Types.Constant == image.Value.Type && image.Value.Value != null && image.Value.Value.Trim().Length == 0)
				{
					m_errorContext.Register(ProcessingErrorCode.rsInvalidEmptyImageReference, Severity.Error, context.ObjectType, context.ObjectName, "Value");
				}
				image.MIMEType = null;
				if (image.Source == Image.SourceType.External && !computed6)
				{
					m_imageStreamNames[image.Value.Value] = new ImageInfo(image.Name, null);
				}
			}
			image.Computed = (computed || computed2 || computed3 || computed8 || computed4 || computedBookmark || computed5 || computed6 || computed7);
			m_hasImageStreams = true;
			if (!computed4 && expressionInfo != null && expressionInfo.Value != null)
			{
				m_hasLabels = true;
			}
			if (!computedBookmark && expressionInfo2 != null && expressionInfo2.Value != null)
			{
				m_hasBookmarks = true;
			}
			if (image.Source == Image.SourceType.External)
			{
				m_hasExternalImages = true;
			}
			return image;
		}

		private SubReport ReadSubreport(ReportItem parent, PublishingContextStruct context)
		{
			SubReport subReport = new SubReport(GenerateID(), parent);
			subReport.Name = m_reader.GetAttribute("Name");
			context.ObjectType = subReport.ObjectType;
			context.ObjectName = subReport.Name;
			m_reportItemNames.Validate(context.ObjectType, context.ObjectName, m_errorContext);
			bool flag = true;
			if ((context.Location & LocationFlags.InPageSection) != 0)
			{
				m_errorContext.Register(ProcessingErrorCode.rsDataRegionInPageSection, Severity.Error, context.ObjectType, context.ObjectName, null);
				flag = false;
			}
			bool flag2 = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (m_reader.LocalName)
					{
					case "Style":
					{
						StyleInformation styleInformation = ReadStyle(context);
						styleInformation.Filter(StyleOwnerType.SubReport, hasNoRows: false);
						subReport.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, m_errorContext);
						break;
					}
					case "Top":
						subReport.Top = ReadSize();
						break;
					case "Left":
						subReport.Left = ReadSize();
						break;
					case "Height":
						subReport.Height = ReadSize();
						break;
					case "Width":
						subReport.Width = ReadSize();
						break;
					case "ZIndex":
						subReport.ZIndex = m_reader.ReadInteger();
						break;
					case "Visibility":
						subReport.Visibility = ReadVisibility(context);
						break;
					case "ToolTip":
						subReport.ToolTip = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
						break;
					case "Label":
						subReport.Label = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
						break;
					case "Bookmark":
						subReport.Bookmark = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
						break;
					case "RepeatWith":
						subReport.RepeatedSibling = true;
						subReport.RepeatWith = m_reader.ReadString();
						break;
					case "Custom":
						subReport.Custom = m_reader.ReadCustomXml();
						break;
					case "CustomProperties":
						subReport.CustomProperties = ReadCustomProperties(context);
						break;
					case "ReportName":
						subReport.ReportPath = PublishingValidator.ValidateReportName(m_reportContext, m_reader.ReadString(), context.ObjectType, context.ObjectName, "ReportName", m_errorContext);
						break;
					case "Parameters":
						subReport.Parameters = ReadParameters(context, doClsValidation: true);
						break;
					case "NoRows":
						subReport.NoRows = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
						break;
					case "MergeTransactions":
						subReport.MergeTransactions = m_reader.ReadBoolean();
						if (subReport.MergeTransactions)
						{
							m_subReportMergeTransactions = true;
						}
						break;
					case "DataElementName":
						subReport.DataElementName = m_reader.ReadString();
						break;
					case "DataElementOutput":
						subReport.DataElementOutputRDL = ReadDataElementOutputRDL();
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("Subreport" == m_reader.LocalName)
					{
						flag2 = true;
					}
					break;
				}
			}
			while (!flag2);
			subReport.Computed = true;
			if (flag)
			{
				m_subReports.Add(subReport);
				m_parametersNotUsedInQuery = false;
				return subReport;
			}
			return null;
		}

		private ActiveXControl ReadActiveXControl(ReportItem parent, PublishingContextStruct context)
		{
			ActiveXControl activeXControl = new ActiveXControl(GenerateID(), parent);
			activeXControl.Name = m_reader.GetAttribute("Name");
			context.ObjectType = activeXControl.ObjectType;
			context.ObjectName = activeXControl.Name;
			m_reportItemNames.Validate(context.ObjectType, context.ObjectName, m_errorContext);
			bool computed = false;
			bool computed2 = false;
			bool computed3 = false;
			bool computedBookmark = false;
			bool computed4 = false;
			bool computed5 = false;
			bool computed6 = false;
			bool flag = false;
			ExpressionInfo expressionInfo = null;
			ExpressionInfo expressionInfo2 = null;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (m_reader.LocalName)
					{
					case "Style":
					{
						StyleInformation styleInformation = ReadStyle(context, out computed);
						styleInformation.Filter(StyleOwnerType.ActiveXControl, hasNoRows: false);
						activeXControl.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, m_errorContext);
						break;
					}
					case "Top":
						activeXControl.Top = ReadSize();
						break;
					case "Left":
						activeXControl.Left = ReadSize();
						break;
					case "Height":
						activeXControl.Height = ReadSize();
						break;
					case "Width":
						activeXControl.Width = ReadSize();
						break;
					case "ZIndex":
						activeXControl.ZIndex = m_reader.ReadInteger();
						break;
					case "Visibility":
						activeXControl.Visibility = ReadVisibility(context, out computed2);
						break;
					case "ToolTip":
						activeXControl.ToolTip = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out computed4);
						break;
					case "Label":
						expressionInfo = (activeXControl.Label = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out computed3));
						break;
					case "Bookmark":
						expressionInfo2 = (activeXControl.Bookmark = ReadBookmarkExpression(context, out computedBookmark));
						break;
					case "RepeatWith":
						activeXControl.RepeatedSibling = true;
						activeXControl.RepeatWith = m_reader.ReadString();
						break;
					case "Custom":
						activeXControl.Custom = m_reader.ReadCustomXml();
						break;
					case "CustomProperties":
						activeXControl.CustomProperties = ReadCustomProperties(context, out computed6);
						break;
					case "ClassID":
						activeXControl.ClassID = m_reader.ReadString();
						break;
					case "CodeBase":
						activeXControl.CodeBase = m_reader.ReadString();
						break;
					case "Parameters":
						activeXControl.Parameters = ReadParameters(context, omitAllowed: false, doClsValidation: true, out computed5);
						break;
					case "DataElementName":
						activeXControl.DataElementName = m_reader.ReadString();
						break;
					case "DataElementOutput":
						activeXControl.DataElementOutputRDL = ReadDataElementOutputRDL();
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("ActiveXControl" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			activeXControl.Computed = (computed || computed2 || computed3 || computed6 || computedBookmark || computed4 || computed5);
			if (!computed3 && expressionInfo != null && expressionInfo.Value != null)
			{
				m_hasLabels = true;
			}
			if (!computedBookmark && expressionInfo2 != null && expressionInfo2.Value != null)
			{
				m_hasBookmarks = true;
			}
			return activeXControl;
		}

		private ExpressionInfo ReadBookmarkExpression(PublishingContextStruct context, out bool computedBookmark)
		{
			ExpressionInfo result = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out computedBookmark);
			if ((context.Location & LocationFlags.InPageSection) > (LocationFlags)0)
			{
				m_errorContext.Register(ProcessingErrorCode.rsBookmarkInPageSection, Severity.Warning, context.ObjectType, context.ObjectName, null);
			}
			return result;
		}

		private List ReadList(ReportItem parent, PublishingContextStruct context)
		{
			List list = new List(GenerateID(), GenerateID(), GenerateID(), parent);
			list.Name = m_reader.GetAttribute("Name");
			bool flag = (context.Location & LocationFlags.InDataRegion) == 0;
			context.Location = (context.Location | LocationFlags.InDataSet | LocationFlags.InDataRegion);
			context.ObjectType = list.ObjectType;
			context.ObjectName = list.Name;
			TextBoxList textBoxList = new TextBoxList();
			m_dataRegionCount++;
			m_reportItemNames.Validate(context.ObjectType, context.ObjectName, m_errorContext);
			if (m_scopeNames.Validate(isGrouping: false, context.ObjectName, context.ObjectType, context.ObjectName, m_errorContext))
			{
				m_reportScopes.Add(list.Name, list);
			}
			bool flag2 = true;
			if ((context.Location & LocationFlags.InPageSection) != 0)
			{
				m_errorContext.Register(ProcessingErrorCode.rsDataRegionInPageSection, Severity.Error, context.ObjectType, context.ObjectName, null);
				flag2 = false;
			}
			if ((context.Location & LocationFlags.InDetail) != 0)
			{
				m_errorContext.Register(ProcessingErrorCode.rsDataRegionInTableDetailRow, Severity.Error, context.ObjectType, context.ObjectName, null);
				flag2 = false;
			}
			m_reportItemCollectionList.Add(list.ReportItems);
			m_aggregateHolderList.Add(list);
			m_runningValueHolderList.Add(list.ReportItems);
			StyleInformation styleInformation = null;
			int numberOfAggregates = m_reportCT.NumberOfAggregates;
			if (!m_reader.IsEmptyElement)
			{
				bool flag3 = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (m_reader.LocalName)
						{
						case "Style":
							styleInformation = ReadStyle(context);
							break;
						case "Top":
							list.Top = ReadSize();
							break;
						case "Left":
							list.Left = ReadSize();
							break;
						case "Height":
							list.Height = ReadSize();
							break;
						case "Width":
							list.Width = ReadSize();
							break;
						case "ZIndex":
							list.ZIndex = m_reader.ReadInteger();
							break;
						case "Visibility":
							list.Visibility = ReadVisibility(context);
							break;
						case "ToolTip":
							list.ToolTip = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
							break;
						case "Label":
							list.Label = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
							break;
						case "Bookmark":
							list.Bookmark = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
							break;
						case "RepeatWith":
							list.RepeatedSibling = true;
							list.RepeatWith = m_reader.ReadString();
							break;
						case "Custom":
							list.Custom = m_reader.ReadCustomXml();
							break;
						case "CustomProperties":
							list.CustomProperties = ReadCustomProperties(context);
							break;
						case "KeepTogether":
							list.KeepTogether = m_reader.ReadBoolean();
							break;
						case "NoRows":
							list.NoRows = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
							break;
						case "DataSetName":
						{
							string dataSetName = m_reader.ReadString();
							if (flag)
							{
								list.DataSetName = dataSetName;
							}
							break;
						}
						case "PageBreakAtStart":
							list.PageBreakAtStart = m_reader.ReadBoolean();
							break;
						case "PageBreakAtEnd":
							list.PageBreakAtEnd = m_reader.ReadBoolean();
							break;
						case "Filters":
							list.Filters = ReadFilters(ExpressionParser.ExpressionType.DataRegionFilters, context);
							break;
						case "Grouping":
							list.Grouping = ReadGrouping(context);
							break;
						case "Sorting":
							list.Sorting = ReadSorting(context);
							break;
						case "ReportItems":
							ReadReportItems(null, list, list.ReportItems, context, textBoxList);
							break;
						case "FillPage":
							list.FillPage = m_reader.ReadBoolean();
							break;
						case "DataElementName":
							list.DataElementName = m_reader.ReadString();
							break;
						case "DataElementOutput":
							list.DataElementOutputRDL = ReadDataElementOutputRDL();
							break;
						case "DataInstanceName":
							list.DataInstanceName = m_reader.ReadString();
							break;
						case "DataInstanceElementOutput":
							list.DataInstanceElementOutput = ReadDataElementOutput();
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("List" == m_reader.LocalName)
						{
							flag3 = true;
						}
						break;
					}
				}
				while (!flag3);
			}
			if (list.Grouping == null)
			{
				if (m_reportCT.NumberOfAggregates > numberOfAggregates)
				{
					m_aggregateInDetailSections = true;
				}
				SetSortTargetForTextBoxes(textBoxList, list);
			}
			else
			{
				SetSortTargetForTextBoxes(textBoxList, list.Grouping);
			}
			if (CanMergeGroupingAndSorting(list.Grouping, list.Sorting))
			{
				list.Grouping.GroupAndSort = true;
				list.Grouping.SortDirections = list.Sorting.SortDirections;
				list.Sorting = null;
			}
			if (list.Sorting != null)
			{
				m_hasSorting = true;
			}
			if (styleInformation != null)
			{
				styleInformation.Filter(StyleOwnerType.List, list.NoRows != null);
				list.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, m_errorContext);
			}
			list.Computed = true;
			if (!flag2)
			{
				return null;
			}
			return list;
		}

		private Matrix ReadMatrix(ReportItem parent, PublishingContextStruct context)
		{
			Matrix matrix = new Matrix(GenerateID(), GenerateID(), GenerateID(), parent);
			matrix.Name = m_reader.GetAttribute("Name");
			bool flag = (context.Location & LocationFlags.InDataRegion) == 0;
			context.Location = (context.Location | LocationFlags.InDataSet | LocationFlags.InDataRegion);
			context.ObjectType = matrix.ObjectType;
			context.ObjectName = matrix.Name;
			m_dataRegionCount++;
			m_reportItemNames.Validate(context.ObjectType, context.ObjectName, m_errorContext);
			if (m_scopeNames.Validate(isGrouping: false, context.ObjectName, context.ObjectType, context.ObjectName, m_errorContext))
			{
				m_reportScopes.Add(matrix.Name, matrix);
			}
			bool flag2 = true;
			if ((context.Location & LocationFlags.InPageSection) != 0)
			{
				m_errorContext.Register(ProcessingErrorCode.rsDataRegionInPageSection, Severity.Error, context.ObjectType, context.ObjectName, null);
				flag2 = false;
			}
			if ((context.Location & LocationFlags.InDetail) != 0)
			{
				m_errorContext.Register(ProcessingErrorCode.rsDataRegionInTableDetailRow, Severity.Error, context.ObjectType, context.ObjectName, null);
				flag2 = false;
			}
			m_reportItemCollectionList.Add(matrix.CornerReportItems);
			m_reportItemCollectionList.Add(matrix.CellReportItems);
			m_aggregateHolderList.Add(matrix);
			m_runningValueHolderList.Add(matrix);
			m_runningValueHolderList.Add(matrix.CornerReportItems);
			m_runningValueHolderList.Add(matrix.CellReportItems);
			StyleInformation styleInformation = null;
			bool flag3 = false;
			TextBoxList textBoxList = new TextBoxList();
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (m_reader.LocalName)
					{
					case "Style":
						styleInformation = ReadStyle(context);
						break;
					case "Top":
						matrix.Top = ReadSize();
						break;
					case "Left":
						matrix.Left = ReadSize();
						break;
					case "Height":
						matrix.Height = ReadSize();
						break;
					case "Width":
						matrix.Width = ReadSize();
						break;
					case "ZIndex":
						matrix.ZIndex = m_reader.ReadInteger();
						break;
					case "Visibility":
						matrix.Visibility = ReadVisibility(context);
						break;
					case "ToolTip":
						matrix.ToolTip = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
						break;
					case "Label":
						matrix.Label = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
						break;
					case "Bookmark":
						matrix.Bookmark = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
						break;
					case "RepeatWith":
						matrix.RepeatedSibling = true;
						matrix.RepeatWith = m_reader.ReadString();
						break;
					case "Custom":
						matrix.Custom = m_reader.ReadCustomXml();
						break;
					case "CustomProperties":
						matrix.CustomProperties = ReadCustomProperties(context);
						break;
					case "DataElementName":
						matrix.DataElementName = m_reader.ReadString();
						break;
					case "DataElementOutput":
						matrix.DataElementOutputRDL = ReadDataElementOutputRDL();
						break;
					case "CellDataElementName":
						matrix.CellDataElementName = m_reader.ReadString();
						break;
					case "CellDataElementOutput":
						matrix.CellDataElementOutput = ReadDataElementOutput();
						break;
					case "KeepTogether":
						matrix.KeepTogether = m_reader.ReadBoolean();
						break;
					case "NoRows":
						matrix.NoRows = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
						break;
					case "DataSetName":
					{
						string dataSetName = m_reader.ReadString();
						if (flag)
						{
							matrix.DataSetName = dataSetName;
						}
						break;
					}
					case "PageBreakAtStart":
						matrix.PageBreakAtStart = m_reader.ReadBoolean();
						break;
					case "PageBreakAtEnd":
						matrix.PageBreakAtEnd = m_reader.ReadBoolean();
						break;
					case "Filters":
						matrix.Filters = ReadFilters(ExpressionParser.ExpressionType.DataRegionFilters, context);
						break;
					case "Corner":
						ReadCorner(matrix, context, textBoxList);
						break;
					case "ColumnGroupings":
						ReadColumnGroupings(matrix, context, textBoxList);
						break;
					case "RowGroupings":
						ReadRowGroupings(matrix, context, textBoxList);
						break;
					case "MatrixRows":
						matrix.MatrixRows = ReadMatrixRows(matrix, context, textBoxList);
						break;
					case "MatrixColumns":
						matrix.MatrixColumns = ReadMatrixColumns();
						break;
					case "LayoutDirection":
						matrix.LayoutDirection = ReadLayoutDirection();
						break;
					case "GroupsBeforeRowHeaders":
						matrix.GroupsBeforeRowHeaders = m_reader.ReadInteger();
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("Matrix" == m_reader.LocalName)
					{
						flag3 = true;
					}
					break;
				}
			}
			while (!flag3);
			matrix.CalculatePropagatedFlags();
			if (!flag && (matrix.RowGroupingFixedHeader || matrix.ColumnGroupingFixedHeader))
			{
				m_errorContext.Register(ProcessingErrorCode.rsFixedHeadersInInnerDataRegion, Severity.Error, context.ObjectType, context.ObjectName, "FixedHeader");
			}
			if (styleInformation != null)
			{
				styleInformation.Filter(StyleOwnerType.Matrix, matrix.NoRows != null);
				matrix.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, m_errorContext);
			}
			SetSortTargetForTextBoxes(textBoxList, matrix);
			matrix.Computed = true;
			if (!flag2)
			{
				return null;
			}
			return matrix;
		}

		private void ReadCorner(Matrix matrix, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget)
		{
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "ReportItems")
					{
						ReadReportItems("Corner", matrix, matrix.CornerReportItems, context, textBoxesWithDefaultSortTarget);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("Corner" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
		}

		private void ReadColumnGroupings(Matrix matrix, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget)
		{
			MatrixHeading matrixHeading = null;
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (!(localName == "ColumnGrouping"))
					{
						break;
					}
					TextBoxList textBoxList = new TextBoxList();
					bool flag2 = true;
					MatrixHeading matrixHeading2 = ReadColumnOrRowGrouping(isColumn: true, matrix, context, textBoxList);
					if (matrixHeading != null)
					{
						matrixHeading.SubHeading = matrixHeading2;
						if (matrixHeading.Grouping != null)
						{
							SetSortTargetForTextBoxes(textBoxList, matrixHeading.Grouping);
							flag2 = false;
						}
					}
					else
					{
						matrix.Columns = matrixHeading2;
					}
					if (flag2)
					{
						textBoxesWithDefaultSortTarget.AddRange(textBoxList);
					}
					matrixHeading = matrixHeading2;
					matrix.ColumnCount++;
					break;
				}
				case XmlNodeType.EndElement:
					if ("ColumnGroupings" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
		}

		private MatrixHeading ReadColumnOrRowGrouping(bool isColumn, Matrix matrix, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget)
		{
			MatrixHeading matrixHeading = new MatrixHeading(GenerateID(), GenerateID(), matrix);
			m_reportItemCollectionList.Add(matrixHeading.ReportItems);
			m_runningValueHolderList.Add(matrixHeading.ReportItems);
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (m_reader.LocalName)
					{
					case "FixedHeader":
						if (isColumn)
						{
							matrix.ColumnGroupingFixedHeader = m_reader.ReadBoolean();
						}
						else
						{
							matrix.RowGroupingFixedHeader = m_reader.ReadBoolean();
						}
						break;
					case "Height":
						Global.Tracer.Assert(isColumn);
						matrixHeading.Size = ReadSize();
						break;
					case "Width":
						Global.Tracer.Assert(!isColumn);
						matrixHeading.Size = ReadSize();
						break;
					case "DynamicColumns":
						flag = true;
						Global.Tracer.Assert(isColumn);
						ReadDynamicColumnsOrRows(isColumn, matrix, matrixHeading, context, textBoxesWithDefaultSortTarget);
						break;
					case "DynamicRows":
						flag = true;
						Global.Tracer.Assert(!isColumn);
						ReadDynamicColumnsOrRows(isColumn, matrix, matrixHeading, context, textBoxesWithDefaultSortTarget);
						break;
					case "StaticColumns":
						flag2 = true;
						Global.Tracer.Assert(isColumn);
						ReadStaticColumnsOrRows(isColumn, matrix, matrixHeading, context, textBoxesWithDefaultSortTarget);
						break;
					case "StaticRows":
						flag2 = true;
						Global.Tracer.Assert(!isColumn);
						ReadStaticColumnsOrRows(isColumn, matrix, matrixHeading, context, textBoxesWithDefaultSortTarget);
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ((isColumn && "ColumnGrouping" == m_reader.LocalName) || (!isColumn && "RowGrouping" == m_reader.LocalName))
					{
						flag3 = true;
					}
					break;
				}
			}
			while (!flag3);
			if (flag == flag2)
			{
				if (isColumn)
				{
					m_errorContext.Register(ProcessingErrorCode.rsInvalidColumnGrouping, Severity.Error, context.ObjectType, context.ObjectName, "ColumnGrouping");
				}
				else
				{
					m_errorContext.Register(ProcessingErrorCode.rsInvalidRowGrouping, Severity.Error, context.ObjectType, context.ObjectName, "RowGrouping");
				}
			}
			if (isColumn && matrixHeading.Grouping != null && (matrixHeading.Grouping.PageBreakAtStart || matrixHeading.Grouping.PageBreakAtEnd))
			{
				m_errorContext.Register(ProcessingErrorCode.rsPageBreakOnMatrixColumnGroup, Severity.Warning, context.ObjectType, context.ObjectName, "ColumnGrouping", matrixHeading.Grouping.Name);
			}
			return matrixHeading;
		}

		private void ReadRowGroupings(Matrix matrix, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget)
		{
			MatrixHeading matrixHeading = null;
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (!(localName == "RowGrouping"))
					{
						break;
					}
					TextBoxList textBoxList = new TextBoxList();
					bool flag2 = true;
					MatrixHeading matrixHeading2 = ReadColumnOrRowGrouping(isColumn: false, matrix, context, textBoxList);
					if (matrixHeading != null)
					{
						matrixHeading.SubHeading = matrixHeading2;
						if (matrixHeading.Grouping != null)
						{
							SetSortTargetForTextBoxes(textBoxList, matrixHeading.Grouping);
							flag2 = false;
						}
					}
					else
					{
						matrix.Rows = matrixHeading2;
					}
					if (flag2)
					{
						textBoxesWithDefaultSortTarget.AddRange(textBoxList);
					}
					matrixHeading = matrixHeading2;
					matrix.RowCount++;
					break;
				}
				case XmlNodeType.EndElement:
					if ("RowGroupings" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
		}

		private void ReadDynamicColumnsOrRows(bool isColumns, Matrix matrix, MatrixHeading heading, PublishingContextStruct context, TextBoxList subtotalTextBoxesWithDefaultSortTarget)
		{
			bool flag = false;
			TextBoxList textBoxList = new TextBoxList();
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (m_reader.LocalName)
					{
					case "Grouping":
						heading.Grouping = ReadGrouping(context);
						break;
					case "Sorting":
						heading.Sorting = ReadSorting(context);
						break;
					case "Subtotal":
						heading.Subtotal = ReadSubtotal(matrix, context, subtotalTextBoxesWithDefaultSortTarget);
						break;
					case "ReportItems":
						ReadReportItems(isColumns ? "DynamicColumns" : "DynamicRows", matrix, heading.ReportItems, context, textBoxList);
						break;
					case "Visibility":
						heading.Visibility = ReadVisibility(context);
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("DynamicColumns" == m_reader.LocalName || "DynamicRows" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (CanMergeGroupingAndSorting(heading.Grouping, heading.Sorting))
			{
				heading.Grouping.GroupAndSort = true;
				heading.Grouping.SortDirections = heading.Sorting.SortDirections;
				heading.Sorting = null;
			}
			if (heading.Sorting != null)
			{
				m_hasSorting = true;
			}
			if (heading.Subtotal == null && heading.Visibility != null)
			{
				heading.Subtotal = new Subtotal(GenerateID(), GenerateID(), autoDerived: true);
				m_reportItemCollectionList.Add(heading.Subtotal.ReportItems);
				m_runningValueHolderList.Add(heading.Subtotal.ReportItems);
			}
			Global.Tracer.Assert(heading.Grouping != null);
			SetSortTargetForTextBoxes(textBoxList, heading.Grouping);
		}

		private void ReadStaticColumnsOrRows(bool isColumn, Matrix matrix, MatrixHeading heading, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget)
		{
			if (isColumn)
			{
				if (matrix.StaticColumns == null)
				{
					matrix.StaticColumns = heading;
				}
				else
				{
					m_errorContext.Register(ProcessingErrorCode.rsMultiStaticColumnsOrRows, Severity.Error, context.ObjectType, context.ObjectName, "StaticColumns");
				}
			}
			else if (matrix.StaticRows == null)
			{
				matrix.StaticRows = heading;
			}
			else
			{
				m_errorContext.Register(ProcessingErrorCode.rsMultiStaticColumnsOrRows, Severity.Error, context.ObjectType, context.ObjectName, "StaticRows");
			}
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (!(localName == "StaticColumn"))
					{
						if (localName == "StaticRow")
						{
							Global.Tracer.Assert(!isColumn);
							ReadStaticRow(matrix, heading, context, textBoxesWithDefaultSortTarget);
							heading.NumberOfStatics++;
						}
					}
					else
					{
						Global.Tracer.Assert(isColumn);
						ReadStaticColumn(matrix, heading, context, textBoxesWithDefaultSortTarget);
						heading.NumberOfStatics++;
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("StaticColumns" == m_reader.LocalName || "StaticRows" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			heading.IDs = new IntList();
			for (int i = 0; i < heading.ReportItems.Count; i++)
			{
				heading.IDs.Add(GenerateID());
			}
		}

		private void ReadStaticColumn(Matrix matrix, MatrixHeading heading, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget)
		{
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "ReportItems")
					{
						ReadReportItems("StaticColumn", matrix, heading.ReportItems, context, textBoxesWithDefaultSortTarget);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("StaticColumn" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
		}

		private void ReadStaticRow(Matrix matrix, MatrixHeading heading, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget)
		{
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "ReportItems")
					{
						ReadReportItems("StaticRow", matrix, heading.ReportItems, context, textBoxesWithDefaultSortTarget);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("StaticRow" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
		}

		private Subtotal ReadSubtotal(Matrix matrix, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget)
		{
			bool computed = false;
			Subtotal subtotal = new Subtotal(GenerateID(), GenerateID(), autoDerived: false);
			m_reportItemCollectionList.Add(subtotal.ReportItems);
			m_runningValueHolderList.Add(subtotal.ReportItems);
			context.Location |= LocationFlags.InMatrixSubtotal;
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (m_reader.LocalName)
					{
					case "ReportItems":
						ReadReportItems("Subtotal", matrix, subtotal.ReportItems, context, textBoxesWithDefaultSortTarget);
						break;
					case "Style":
					{
						StyleInformation styleInformation = ReadStyle(context, out computed);
						styleInformation.Filter(StyleOwnerType.Subtotal, hasNoRows: false);
						subtotal.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, m_errorContext);
						subtotal.Computed = computed;
						break;
					}
					case "Position":
						subtotal.Position = ReadPosition();
						break;
					case "DataElementName":
						subtotal.DataElementName = m_reader.ReadString();
						break;
					case "DataElementOutput":
						subtotal.DataElementOutput = ReadDataElementOutput();
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("Subtotal" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return subtotal;
		}

		private MatrixRowList ReadMatrixRows(Matrix matrix, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget)
		{
			MatrixRowList matrixRowList = new MatrixRowList();
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "MatrixRow")
					{
						matrixRowList.Add(ReadMatrixRow(matrix, context, textBoxesWithDefaultSortTarget));
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("MatrixRows" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			matrix.CellIDs = new IntList();
			for (int i = 0; i < matrix.CellReportItems.Count; i++)
			{
				matrix.CellIDs.Add(GenerateID());
			}
			return matrixRowList;
		}

		private MatrixRow ReadMatrixRow(Matrix matrix, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget)
		{
			MatrixRow matrixRow = new MatrixRow();
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (!(localName == "Height"))
					{
						if (localName == "MatrixCells")
						{
							matrixRow.NumberOfMatrixCells = ReadMatrixCells(matrix, context, textBoxesWithDefaultSortTarget);
						}
					}
					else
					{
						matrixRow.Height = ReadSize();
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("MatrixRow" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return matrixRow;
		}

		private int ReadMatrixCells(Matrix matrix, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget)
		{
			int num = 0;
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "MatrixCell")
					{
						ReadMatrixCell(matrix, context, textBoxesWithDefaultSortTarget);
						num++;
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("MatrixCells" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return num;
		}

		private void ReadMatrixCell(Matrix matrix, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget)
		{
			context.Location |= LocationFlags.InMatrixCell;
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "ReportItems")
					{
						ReadReportItems("MatrixCell", matrix, matrix.CellReportItems, context, textBoxesWithDefaultSortTarget);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("MatrixCell" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
		}

		private MatrixColumnList ReadMatrixColumns()
		{
			MatrixColumnList matrixColumnList = new MatrixColumnList();
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "MatrixColumn")
					{
						matrixColumnList.Add(ReadMatrixColumn());
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("MatrixColumns" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return matrixColumnList;
		}

		private MatrixColumn ReadMatrixColumn()
		{
			MatrixColumn matrixColumn = new MatrixColumn();
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "Width")
					{
						matrixColumn.Width = ReadSize();
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("MatrixColumn" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return matrixColumn;
		}

		private Chart ReadChart(ReportItem parent, PublishingContextStruct context)
		{
			Chart chart = new Chart(GenerateID(), parent);
			chart.Name = m_reader.GetAttribute("Name");
			bool flag = (context.Location & LocationFlags.InDataRegion) == 0;
			context.Location = (context.Location | LocationFlags.InDataSet | LocationFlags.InDataRegion);
			context.ObjectType = chart.ObjectType;
			context.ObjectName = chart.Name;
			m_dataRegionCount++;
			m_reportItemNames.Validate(context.ObjectType, context.ObjectName, m_errorContext);
			if (m_scopeNames.Validate(isGrouping: false, context.ObjectName, context.ObjectType, context.ObjectName, m_errorContext))
			{
				m_reportScopes.Add(chart.Name, chart);
			}
			bool flag2 = true;
			if ((context.Location & LocationFlags.InPageSection) != 0)
			{
				m_errorContext.Register(ProcessingErrorCode.rsDataRegionInPageSection, Severity.Error, context.ObjectType, context.ObjectName, null);
				flag2 = false;
			}
			m_aggregateHolderList.Add(chart);
			m_runningValueHolderList.Add(chart);
			StyleInformation styleInformation = null;
			if (!m_reader.IsEmptyElement)
			{
				bool flag3 = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (m_reader.LocalName)
						{
						case "Style":
							styleInformation = ReadStyle(context);
							break;
						case "Top":
							chart.Top = ReadSize();
							break;
						case "Left":
							chart.Left = ReadSize();
							break;
						case "Height":
							chart.Height = ReadSize();
							break;
						case "Width":
							chart.Width = ReadSize();
							break;
						case "ZIndex":
							chart.ZIndex = m_reader.ReadInteger();
							break;
						case "Visibility":
							chart.Visibility = ReadVisibility(context);
							break;
						case "ToolTip":
							chart.ToolTip = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
							break;
						case "Label":
							chart.Label = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
							break;
						case "Bookmark":
							chart.Bookmark = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
							break;
						case "RepeatWith":
							chart.RepeatedSibling = true;
							chart.RepeatWith = m_reader.ReadString();
							break;
						case "Custom":
							chart.Custom = m_reader.ReadCustomXml();
							break;
						case "CustomProperties":
							chart.CustomProperties = ReadCustomProperties(context);
							break;
						case "DataElementName":
							chart.DataElementName = m_reader.ReadString();
							break;
						case "DataElementOutput":
							chart.DataElementOutputRDL = ReadDataElementOutputRDL();
							break;
						case "ChartElementOutput":
							chart.CellDataElementOutput = ReadDataElementOutput();
							break;
						case "KeepTogether":
							chart.KeepTogether = m_reader.ReadBoolean();
							break;
						case "NoRows":
							chart.NoRows = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
							break;
						case "DataSetName":
						{
							string dataSetName = m_reader.ReadString();
							if (flag)
							{
								chart.DataSetName = dataSetName;
							}
							break;
						}
						case "PageBreakAtStart":
							chart.PageBreakAtStart = m_reader.ReadBoolean();
							break;
						case "PageBreakAtEnd":
							chart.PageBreakAtEnd = m_reader.ReadBoolean();
							break;
						case "Filters":
							chart.Filters = ReadFilters(ExpressionParser.ExpressionType.DataRegionFilters, context);
							break;
						case "Type":
							chart.Type = ReadChartType();
							break;
						case "Subtype":
							chart.SubType = ReadChartSubType();
							break;
						case "SeriesGroupings":
							ReadSeriesGroupings(chart, context);
							break;
						case "CategoryGroupings":
							ReadCategoryGroupings(chart, context);
							break;
						case "ChartData":
							ReadChartData(chart, context);
							break;
						case "Legend":
							chart.Legend = ReadLegend(context);
							break;
						case "CategoryAxis":
							chart.CategoryAxis = ReadCategoryOrValueAxis(chart, context);
							break;
						case "ValueAxis":
							chart.ValueAxis = ReadCategoryOrValueAxis(chart, context);
							break;
						case "MultiChart":
							chart.MultiChart = ReadMultiChart(chart, context);
							break;
						case "Title":
							chart.Title = ReadChartTitle(context);
							break;
						case "PointWidth":
							chart.PointWidth = m_reader.ReadInteger();
							break;
						case "Palette":
							chart.Palette = ReadChartPalette();
							break;
						case "ThreeDProperties":
							chart.ThreeDProperties = ReadThreeDProperties(chart, context);
							break;
						case "PlotArea":
							chart.PlotArea = ReadPlotArea(chart, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("Chart" == m_reader.LocalName)
						{
							flag3 = true;
						}
						break;
					}
				}
				while (!flag3);
			}
			if (!chart.IsValidChartSubType())
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidChartSubType, Severity.Error, context.ObjectType, context.ObjectName, null, Enum.GetName(typeof(Chart.ChartTypes), chart.Type), Enum.GetName(typeof(Chart.ChartSubTypes), chart.SubType));
			}
			if (Chart.ChartTypes.Pie == chart.Type || Chart.ChartTypes.Doughnut == chart.Type)
			{
				chart.CategoryAxis = null;
				chart.ValueAxis = null;
				if (chart.Rows != null)
				{
					if (chart.StaticRows != null && chart.StaticColumns != null)
					{
						m_errorContext.Register(ProcessingErrorCode.rsInvalidChartGroupings, Severity.Error, context.ObjectType, context.ObjectName, null);
					}
					else
					{
						ChartHeading chartHeading = chart.Columns;
						while (chartHeading != null && chartHeading.SubHeading != null)
						{
							chartHeading = chartHeading.SubHeading;
						}
						if (chartHeading == null)
						{
							chart.Columns = chart.Rows;
						}
						else
						{
							chartHeading.SubHeading = chart.Rows;
						}
						if (chart.StaticRows != null)
						{
							chart.StaticColumns = chart.StaticRows;
							chart.StaticRows = null;
						}
						Global.Tracer.Assert(chart.NumberOfSeriesDataPoints != null);
						int num = 0;
						for (int i = 0; i < chart.NumberOfSeriesDataPoints.Count; i++)
						{
							num += chart.NumberOfSeriesDataPoints[i];
						}
						chart.NumberOfSeriesDataPoints = new IntList(1);
						chart.NumberOfSeriesDataPoints.Add(num);
						chart.ColumnCount += chart.RowCount;
						chart.RowCount = 0;
						chart.Rows = null;
					}
				}
			}
			if (styleInformation != null)
			{
				styleInformation.Filter(StyleOwnerType.Chart, chart.NoRows != null);
				chart.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, m_errorContext);
			}
			if (chart.Columns == null)
			{
				if ((chart.Type == Chart.ChartTypes.Bubble || chart.Type == Chart.ChartTypes.Scatter) && !chart.HasDataValueAggregates)
				{
					ChartAddRowNumberCategory(chart, context);
				}
				else
				{
					ChartFakeStaticCategory(chart);
				}
			}
			if (chart.Rows == null)
			{
				ChartFakeStaticSeries(chart);
			}
			chart.Computed = true;
			if (flag2)
			{
				m_hasImageStreams = true;
				return chart;
			}
			return null;
		}

		private void ReadCategoryGroupings(Chart chart, PublishingContextStruct context)
		{
			ChartHeading chartHeading = null;
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "CategoryGrouping")
					{
						ChartHeading chartHeading2 = ReadCategoryOrSeriesGrouping(isCategory: true, chart, context);
						if (chartHeading != null)
						{
							chartHeading.SubHeading = chartHeading2;
						}
						else
						{
							chart.Columns = chartHeading2;
						}
						chartHeading = chartHeading2;
						chart.ColumnCount++;
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("CategoryGroupings" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
		}

		private void ChartAddRowNumberCategory(Chart chart, PublishingContextStruct context)
		{
			Global.Tracer.Assert(chart != null);
			Global.Tracer.Assert(chart.ColumnCount == 0);
			chart.ColumnCount++;
			chart.Columns = new ChartHeading(GenerateID(), chart);
			m_hasGrouping = true;
			Grouping grouping = new Grouping(ConstructionPhase.Publishing);
			grouping.Name = "0_" + chart.Name + "_AutoGenerated_RowNumber_Category";
			if (m_scopeNames.Validate(isGrouping: true, grouping.Name, context.ObjectType, context.ObjectName, m_errorContext, enforceCLSCompliance: false))
			{
				m_reportScopes.Add(grouping.Name, grouping);
			}
			m_aggregateHolderList.Add(grouping);
			chart.Columns.Grouping = grouping;
			m_runningValueHolderList.Add(chart.Columns);
			ExpressionParser.ExpressionContext context2 = context.CreateExpressionContext(ExpressionParser.ExpressionType.GroupExpression, ExpressionParser.ConstantType.String, "CategoryGrouping", null);
			grouping.GroupExpressions.Add(m_reportCT.ParseExpression("=RowNumber(\"" + chart.Name + "\")", context2, out bool userCollectionReferenced));
			Global.Tracer.Assert(!userCollectionReferenced);
		}

		private void ChartFakeStaticSeries(Chart chart)
		{
			Global.Tracer.Assert(chart != null);
			Global.Tracer.Assert(chart.RowCount == 0);
			chart.RowCount++;
			chart.Rows = new ChartHeading(GenerateID(), chart);
			chart.Rows.NumberOfStatics++;
			chart.StaticRows = chart.Rows;
		}

		private void ChartFakeStaticCategory(Chart chart)
		{
			Global.Tracer.Assert(chart != null);
			Global.Tracer.Assert(chart.ColumnCount == 0);
			chart.ColumnCount++;
			chart.Columns = new ChartHeading(GenerateID(), chart);
			chart.Columns.NumberOfStatics++;
			chart.StaticColumns = chart.Columns;
		}

		private ChartHeading ReadCategoryOrSeriesGrouping(bool isCategory, Chart chart, PublishingContextStruct context)
		{
			ChartHeading chartHeading = new ChartHeading(GenerateID(), chart);
			m_runningValueHolderList.Add(chartHeading);
			bool flag = false;
			bool flag2 = false;
			if (!m_reader.IsEmptyElement)
			{
				bool flag3 = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (m_reader.LocalName)
						{
						case "DynamicCategories":
							flag = true;
							Global.Tracer.Assert(isCategory);
							ReadDynamicCategoriesOrSeries(isCategory, chart, chartHeading, context);
							break;
						case "DynamicSeries":
							flag = true;
							Global.Tracer.Assert(!isCategory);
							ReadDynamicCategoriesOrSeries(isCategory, chart, chartHeading, context);
							break;
						case "StaticCategories":
							flag2 = true;
							Global.Tracer.Assert(isCategory);
							ReadStaticCategoriesOrSeries(isCategory, chart, chartHeading, context);
							break;
						case "StaticSeries":
							flag2 = true;
							Global.Tracer.Assert(!isCategory);
							ReadStaticCategoriesOrSeries(isCategory, chart, chartHeading, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ((isCategory && "CategoryGrouping" == m_reader.LocalName) || (!isCategory && "SeriesGrouping" == m_reader.LocalName))
						{
							flag3 = true;
						}
						break;
					}
				}
				while (!flag3);
			}
			if (flag == flag2)
			{
				if (isCategory)
				{
					m_errorContext.Register(ProcessingErrorCode.rsInvalidCategoryGrouping, Severity.Error, context.ObjectType, context.ObjectName, "CategoryGrouping");
				}
				else
				{
					m_errorContext.Register(ProcessingErrorCode.rsInvalidSeriesGrouping, Severity.Error, context.ObjectType, context.ObjectName, "SeriesGrouping");
				}
			}
			if (chartHeading.Grouping != null && (chartHeading.Grouping.PageBreakAtStart || chartHeading.Grouping.PageBreakAtEnd))
			{
				m_errorContext.Register(ProcessingErrorCode.rsPageBreakOnChartGroup, Severity.Warning, context.ObjectType, context.ObjectName, isCategory ? "CategoryGroupings" : "SeriesGroupings", chartHeading.Grouping.Name);
			}
			return chartHeading;
		}

		private void ReadSeriesGroupings(Chart chart, PublishingContextStruct context)
		{
			ChartHeading chartHeading = null;
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "SeriesGrouping")
					{
						ChartHeading chartHeading2 = ReadCategoryOrSeriesGrouping(isCategory: false, chart, context);
						if (chartHeading != null)
						{
							chartHeading.SubHeading = chartHeading2;
						}
						else
						{
							chart.Rows = chartHeading2;
						}
						chartHeading = chartHeading2;
						chart.RowCount++;
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("SeriesGroupings" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
		}

		private void ReadDynamicCategoriesOrSeries(bool isCategory, Chart chart, ChartHeading heading, PublishingContextStruct context)
		{
			bool flag = false;
			ExpressionInfo expressionInfo = null;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (m_reader.LocalName)
					{
					case "Grouping":
						heading.Grouping = ReadGrouping(context);
						break;
					case "Sorting":
						heading.Sorting = ReadSorting(context);
						break;
					case "Label":
						expressionInfo = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ((isCategory && "DynamicCategories" == m_reader.LocalName) || (!isCategory && "DynamicSeries" == m_reader.LocalName))
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (CanMergeGroupingAndSorting(heading.Grouping, heading.Sorting))
			{
				heading.Grouping.GroupAndSort = true;
				heading.Grouping.SortDirections = heading.Sorting.SortDirections;
				heading.Sorting = null;
			}
			if (heading.Sorting != null)
			{
				m_hasSorting = true;
			}
			if (expressionInfo != null && (ExpressionInfo.Types.Constant != expressionInfo.Type || expressionInfo.Value.Length != 0))
			{
				if (heading.Labels == null)
				{
					heading.Labels = new ExpressionInfoList();
				}
				heading.Labels.Add(expressionInfo);
				return;
			}
			if (heading.Labels == null)
			{
				heading.Labels = new ExpressionInfoList();
			}
			Global.Tracer.Assert(heading.Grouping.GroupExpressions != null && heading.Grouping.GroupExpressions[0] != null);
			heading.Labels.Add(heading.Grouping.GroupExpressions[0]);
		}

		private void ReadStaticCategoriesOrSeries(bool isColumn, Chart chart, ChartHeading heading, PublishingContextStruct context)
		{
			if (isColumn)
			{
				if (chart.StaticColumns == null)
				{
					chart.StaticColumns = heading;
				}
				else
				{
					m_errorContext.Register(ProcessingErrorCode.rsMultiStaticCategoriesOrSeries, Severity.Error, context.ObjectType, context.ObjectName, "StaticCategories");
				}
			}
			else if (chart.StaticRows == null)
			{
				chart.StaticRows = heading;
			}
			else
			{
				m_errorContext.Register(ProcessingErrorCode.rsMultiStaticCategoriesOrSeries, Severity.Error, context.ObjectType, context.ObjectName, "StaticSeries");
			}
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "StaticMember")
					{
						ReadStaticMember(chart, heading, context);
						heading.NumberOfStatics++;
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("StaticCategories" == m_reader.LocalName || "StaticSeries" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
		}

		private void ReadStaticMember(Chart chart, ChartHeading heading, PublishingContextStruct context)
		{
			bool flag = false;
			ExpressionInfo expressionInfo = null;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (!(localName == "Label"))
					{
						break;
					}
					expressionInfo = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
					if (expressionInfo != null)
					{
						if (heading.Labels == null)
						{
							heading.Labels = new ExpressionInfoList();
						}
						if (ExpressionInfo.Types.Constant == expressionInfo.Type && expressionInfo.Value.Length == 0)
						{
							expressionInfo.Value = null;
						}
						heading.Labels.Add(expressionInfo);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("StaticMember" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
		}

		private ChartTitle ReadChartTitle(PublishingContextStruct context)
		{
			ChartTitle chartTitle = new ChartTitle();
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (m_reader.LocalName)
						{
						case "Caption":
							chartTitle.Caption = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
							break;
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Textbox, hasNoRows: false);
							chartTitle.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, m_errorContext);
							break;
						}
						case "Position":
							chartTitle.Position = ReadChartTitlePosition();
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("Title" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartTitle;
		}

		private Axis ReadCategoryOrValueAxis(Chart chart, PublishingContextStruct context)
		{
			Axis result = null;
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName = m_reader.LocalName;
						if (localName == "Axis")
						{
							result = ReadAxis(chart, context);
						}
						break;
					}
					case XmlNodeType.EndElement:
						if ("CategoryAxis" == m_reader.LocalName || "ValueAxis" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return result;
		}

		private Axis ReadAxis(Chart chart, PublishingContextStruct context)
		{
			Axis axis = new Axis();
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (m_reader.LocalName)
						{
						case "Visible":
							axis.Visible = m_reader.ReadBoolean();
							break;
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Textbox, hasNoRows: false);
							axis.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, m_errorContext);
							break;
						}
						case "Title":
							axis.Title = ReadChartTitle(context);
							break;
						case "Margin":
							axis.Margin = m_reader.ReadBoolean();
							break;
						case "MajorTickMarks":
							axis.MajorTickMarks = ReadAxisTickMarks();
							break;
						case "MinorTickMarks":
							axis.MinorTickMarks = ReadAxisTickMarks();
							break;
						case "MajorGridLines":
							axis.MajorGridLines = ReadGridLines(context);
							break;
						case "MinorGridLines":
							axis.MinorGridLines = ReadGridLines(context);
							break;
						case "MajorInterval":
							axis.MajorInterval = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
							break;
						case "MinorInterval":
							axis.MinorInterval = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
							break;
						case "Reverse":
							axis.Reverse = m_reader.ReadBoolean();
							break;
						case "CrossAt":
							axis.AutoCrossAt = false;
							axis.CrossAt = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
							break;
						case "Interlaced":
							axis.Interlaced = m_reader.ReadBoolean();
							break;
						case "Scalar":
							axis.Scalar = m_reader.ReadBoolean();
							break;
						case "Min":
							axis.AutoScaleMin = false;
							axis.Min = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
							break;
						case "Max":
							axis.AutoScaleMax = false;
							axis.Max = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
							break;
						case "LogScale":
							axis.LogScale = m_reader.ReadBoolean();
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("Axis" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return axis;
		}

		private Legend ReadLegend(PublishingContextStruct context)
		{
			Legend legend = new Legend();
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (m_reader.LocalName)
						{
						case "Visible":
							legend.Visible = m_reader.ReadBoolean();
							break;
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Chart, hasNoRows: false);
							legend.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, m_errorContext);
							break;
						}
						case "Position":
							legend.Position = ReadLegendPosition();
							break;
						case "Layout":
							legend.Layout = ReadLegendLayout();
							break;
						case "InsidePlotArea":
							legend.InsidePlotArea = m_reader.ReadBoolean();
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("Legend" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return legend;
		}

		private GridLines ReadGridLines(PublishingContextStruct context)
		{
			GridLines gridLines = new GridLines();
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName = m_reader.LocalName;
						if (!(localName == "Style"))
						{
							if (localName == "ShowGridLines")
							{
								gridLines.ShowGridLines = m_reader.ReadBoolean();
							}
						}
						else
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Chart, hasNoRows: false);
							gridLines.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, m_errorContext);
						}
						break;
					}
					case XmlNodeType.EndElement:
						if ("MajorGridLines" == m_reader.LocalName || "MinorGridLines" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return gridLines;
		}

		private int ReadChartData(Chart chart, PublishingContextStruct context)
		{
			if (!m_reader.IsEmptyElement)
			{
				chart.NumberOfSeriesDataPoints = new IntList();
				chart.SeriesPlotType = new BoolList();
				int num = 0;
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName = m_reader.LocalName;
						if (localName == "ChartSeries")
						{
							ReadChartSeries(chart, context);
							num++;
						}
						break;
					}
					case XmlNodeType.EndElement:
						if ("ChartData" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
				return num;
			}
			return 0;
		}

		private void ReadChartSeries(Chart chart, PublishingContextStruct context)
		{
			bool flag = false;
			bool flag2 = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (!(localName == "DataPoints"))
					{
						if (localName == "PlotType")
						{
							if (ReadPlotType())
							{
								chart.SeriesPlotType.Add(true);
								chart.HasSeriesPlotTypeLine = true;
							}
							else
							{
								chart.SeriesPlotType.Add(false);
							}
							flag2 = true;
						}
					}
					else
					{
						chart.NumberOfSeriesDataPoints.Add(ReadChartDataPoints(chart, context));
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("ChartSeries" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (!flag2)
			{
				chart.SeriesPlotType.Add(false);
			}
		}

		private int ReadChartDataPoints(Chart chart, PublishingContextStruct context)
		{
			int num = 0;
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "DataPoint")
					{
						chart.ChartDataPoints.Add(ReadChartDataPoint(chart, context));
						num++;
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("DataPoints" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return num;
		}

		private ChartDataPoint ReadChartDataPoint(Chart chart, PublishingContextStruct context)
		{
			context.Location |= LocationFlags.InMatrixCell;
			ChartDataPoint chartDataPoint = new ChartDataPoint();
			bool flag = false;
			bool computed = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (m_reader.LocalName)
					{
					case "DataValues":
					{
						ReadChartDataValues(chartDataPoint, context, out bool hasAggregates);
						if (hasAggregates)
						{
							chart.HasDataValueAggregates = true;
						}
						break;
					}
					case "DataLabel":
						chartDataPoint.DataLabel = ReadChartDataLabel(context);
						break;
					case "Action":
					{
						int computedIndex = -1;
						bool missingLabel = false;
						ActionItem actionItem = ReadActionItem(context, out computed, ref computedIndex, ref missingLabel);
						chartDataPoint.Action = new Action(actionItem, computed);
						break;
					}
					case "ActionInfo":
						chartDataPoint.Action = ReadAction(context, StyleOwnerType.Chart, out computed);
						break;
					case "Style":
					{
						StyleInformation styleInformation = ReadStyle(context);
						styleInformation.Filter(StyleOwnerType.Chart, hasNoRows: false);
						chartDataPoint.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, m_errorContext);
						break;
					}
					case "Marker":
						ReadDataPointMarker(chartDataPoint, context);
						break;
					case "DataElementName":
						chartDataPoint.DataElementName = m_reader.ReadString();
						break;
					case "DataElementOutput":
						chartDataPoint.DataElementOutput = ReadDataElementOutput();
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("DataPoint" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			Global.Tracer.Assert(chart.ChartDataPoints != null);
			return chartDataPoint;
		}

		private void ReadDataPointMarker(ChartDataPoint dataPoint, PublishingContextStruct context)
		{
			if (m_reader.IsEmptyElement)
			{
				return;
			}
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (m_reader.LocalName)
					{
					case "Type":
						dataPoint.MarkerType = ReadMarkerType();
						break;
					case "Size":
						dataPoint.MarkerSize = ReadSize();
						break;
					case "Style":
					{
						StyleInformation styleInformation = ReadStyle(context);
						styleInformation.Filter(StyleOwnerType.Chart, hasNoRows: false);
						dataPoint.MarkerStyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, m_errorContext);
						break;
					}
					}
					break;
				case XmlNodeType.EndElement:
					if ("Marker" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
		}

		private void ReadChartDataValues(ChartDataPoint dataPoint, PublishingContextStruct context, out bool hasAggregates)
		{
			hasAggregates = false;
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "DataValue")
					{
						dataPoint.DataValues.Add(ReadChartDataValue(context, ref hasAggregates));
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("DataValues" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
		}

		private ExpressionInfo ReadChartDataValue(PublishingContextStruct context, ref bool hasAggregates)
		{
			ExpressionInfo expressionInfo = null;
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "Value")
					{
						expressionInfo = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
						if (!hasAggregates && (expressionInfo.Aggregates != null || expressionInfo.RunningValues != null))
						{
							hasAggregates = true;
						}
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("DataValue" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return expressionInfo;
		}

		private ChartDataLabel ReadChartDataLabel(PublishingContextStruct context)
		{
			ChartDataLabel chartDataLabel = new ChartDataLabel();
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (m_reader.LocalName)
						{
						case "Visible":
							chartDataLabel.Visible = m_reader.ReadBoolean();
							break;
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Textbox, hasNoRows: false);
							chartDataLabel.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, m_errorContext);
							break;
						}
						case "Value":
							chartDataLabel.Value = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
							break;
						case "Position":
							chartDataLabel.Position = ReadDataLabelPosition();
							break;
						case "Rotation":
							chartDataLabel.Rotation = m_reader.ReadInteger();
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("DataLabel" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartDataLabel;
		}

		private MultiChart ReadMultiChart(Chart chart, PublishingContextStruct context)
		{
			MultiChart multiChart = new MultiChart();
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (m_reader.LocalName)
						{
						case "Grouping":
							multiChart.Grouping = ReadGrouping(context);
							break;
						case "Layout":
							multiChart.Layout = ReadLayout();
							break;
						case "MaxCount":
							multiChart.MaxCount = m_reader.ReadInteger();
							break;
						case "SyncScale":
							multiChart.SyncScale = m_reader.ReadBoolean();
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("MultiChart" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return multiChart;
		}

		private PlotArea ReadPlotArea(Chart chart, PublishingContextStruct context)
		{
			PlotArea plotArea = new PlotArea();
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName = m_reader.LocalName;
						if (localName == "Style")
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Chart, hasNoRows: false);
							plotArea.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, m_errorContext);
						}
						break;
					}
					case XmlNodeType.EndElement:
						if ("PlotArea" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return plotArea;
		}

		private ThreeDProperties ReadThreeDProperties(Chart chart, PublishingContextStruct context)
		{
			ThreeDProperties threeDProperties = new ThreeDProperties();
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (m_reader.LocalName)
						{
						case "Enabled":
							threeDProperties.Enabled = m_reader.ReadBoolean();
							break;
						case "ProjectionMode":
							threeDProperties.PerspectiveProjectionMode = ReadProjectionMode();
							break;
						case "Rotation":
							threeDProperties.Rotation = m_reader.ReadInteger();
							break;
						case "Inclination":
							threeDProperties.Inclination = m_reader.ReadInteger();
							break;
						case "Perspective":
							threeDProperties.Perspective = m_reader.ReadInteger();
							break;
						case "HeightRatio":
							threeDProperties.HeightRatio = m_reader.ReadInteger();
							break;
						case "DepthRatio":
							threeDProperties.DepthRatio = m_reader.ReadInteger();
							break;
						case "Shading":
							threeDProperties.Shading = ReadShading();
							break;
						case "GapDepth":
							threeDProperties.GapDepth = m_reader.ReadInteger();
							break;
						case "WallThickness":
							threeDProperties.WallThickness = m_reader.ReadInteger();
							break;
						case "DrawingStyle":
							threeDProperties.DrawingStyleCube = ReadDrawingStyle();
							break;
						case "Clustered":
							threeDProperties.Clustered = m_reader.ReadBoolean();
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ThreeDProperties" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return threeDProperties;
		}

		private Table ReadTable(ReportItem parent, PublishingContextStruct context)
		{
			Table table = new Table(GenerateID(), parent);
			table.Name = m_reader.GetAttribute("Name");
			bool flag = (context.Location & LocationFlags.InDataRegion) == 0;
			context.Location = (context.Location | LocationFlags.InDataSet | LocationFlags.InDataRegion);
			context.ObjectType = table.ObjectType;
			context.ObjectName = table.Name;
			m_dataRegionCount++;
			m_reportItemNames.Validate(context.ObjectType, context.ObjectName, m_errorContext);
			if (m_scopeNames.Validate(isGrouping: false, context.ObjectName, context.ObjectType, context.ObjectName, m_errorContext))
			{
				m_reportScopes.Add(table.Name, table);
			}
			bool flag2 = true;
			if ((context.Location & LocationFlags.InPageSection) != 0)
			{
				m_errorContext.Register(ProcessingErrorCode.rsDataRegionInPageSection, Severity.Error, context.ObjectType, context.ObjectName, null);
				flag2 = false;
			}
			if ((context.Location & LocationFlags.InDetail) != 0)
			{
				m_errorContext.Register(ProcessingErrorCode.rsDataRegionInTableDetailRow, Severity.Error, context.ObjectType, context.ObjectName, null);
				flag2 = false;
			}
			m_aggregateHolderList.Add(table);
			m_runningValueHolderList.Add(table);
			StyleInformation styleInformation = null;
			bool flag3 = false;
			TextBoxList textBoxList = new TextBoxList();
			TextBoxList textBoxList2 = new TextBoxList();
			TableGroup tableGroup = null;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (m_reader.LocalName)
					{
					case "Style":
						styleInformation = ReadStyle(context);
						break;
					case "Top":
						table.Top = ReadSize();
						break;
					case "Left":
						table.Left = ReadSize();
						break;
					case "ZIndex":
						table.ZIndex = m_reader.ReadInteger();
						break;
					case "Visibility":
						table.Visibility = ReadVisibility(context);
						break;
					case "ToolTip":
						table.ToolTip = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
						break;
					case "Label":
						table.Label = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
						break;
					case "Bookmark":
						table.Bookmark = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
						break;
					case "RepeatWith":
						table.RepeatedSibling = true;
						table.RepeatWith = m_reader.ReadString();
						break;
					case "Custom":
						table.Custom = m_reader.ReadCustomXml();
						break;
					case "CustomProperties":
						table.CustomProperties = ReadCustomProperties(context);
						break;
					case "KeepTogether":
						table.KeepTogether = m_reader.ReadBoolean();
						break;
					case "NoRows":
						table.NoRows = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
						break;
					case "DataSetName":
					{
						string dataSetName = m_reader.ReadString();
						if (flag)
						{
							table.DataSetName = dataSetName;
						}
						break;
					}
					case "PageBreakAtStart":
						table.PageBreakAtStart = m_reader.ReadBoolean();
						break;
					case "PageBreakAtEnd":
						table.PageBreakAtEnd = m_reader.ReadBoolean();
						break;
					case "Filters":
						table.Filters = ReadFilters(ExpressionParser.ExpressionType.DataRegionFilters, context);
						break;
					case "TableColumns":
						table.TableColumns = ReadTableColumns(context, table);
						break;
					case "Header":
					{
						ReadHeaderOrFooter(table, context, textBoxList, allowFixedHeaders: true, out TableRowList tableRows2, out bool repeatOnNewPage2);
						table.HeaderRows = tableRows2;
						table.HeaderRepeatOnNewPage = repeatOnNewPage2;
						break;
					}
					case "TableGroups":
						tableGroup = ReadTableGroups(table, context);
						break;
					case "Details":
					{
						ReadDetails(table, context, textBoxList2, out TableDetail tableDetail, out TableGroup detailGroup);
						if (detailGroup != null)
						{
							table.DetailGroup = detailGroup;
						}
						else
						{
							table.TableDetail = tableDetail;
						}
						break;
					}
					case "Footer":
					{
						ReadHeaderOrFooter(table, context, textBoxList, allowFixedHeaders: false, out TableRowList tableRows, out bool repeatOnNewPage);
						table.FooterRows = tableRows;
						table.FooterRepeatOnNewPage = repeatOnNewPage;
						break;
					}
					case "FillPage":
						table.FillPage = m_reader.ReadBoolean();
						break;
					case "DataElementName":
						table.DataElementName = m_reader.ReadString();
						break;
					case "DataElementOutput":
						table.DataElementOutputRDL = ReadDataElementOutputRDL();
						break;
					case "DetailDataElementName":
						table.DetailDataElementName = m_reader.ReadString();
						break;
					case "DetailDataCollectionName":
						table.DetailDataCollectionName = m_reader.ReadString();
						break;
					case "DetailDataElementOutput":
						table.DetailDataElementOutput = ReadDataElementOutput();
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("Table" == m_reader.LocalName)
					{
						flag3 = true;
					}
					break;
				}
			}
			while (!flag3);
			if (!flag && (table.FixedHeader || table.HasFixedColumnHeaders))
			{
				m_errorContext.Register(ProcessingErrorCode.rsFixedHeadersInInnerDataRegion, Severity.Error, context.ObjectType, context.ObjectName, "FixedHeader");
			}
			table.CalculatePropagatedFlags();
			if (styleInformation != null)
			{
				styleInformation.Filter(StyleOwnerType.Table, table.NoRows != null);
				table.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, m_errorContext);
			}
			SetSortTargetForTextBoxes(textBoxList, table);
			ISortFilterScope target = (ISortFilterScope)((table.DetailGroup != null) ? table.DetailGroup.Grouping : ((tableGroup == null) ? ((object)table) : ((object)tableGroup.Grouping)));
			SetSortTargetForTextBoxes(textBoxList2, target);
			table.Computed = true;
			if (!flag2)
			{
				return null;
			}
			return table;
		}

		private void ReadHeaderOrFooter(Table parent, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget, bool allowFixedHeaders, out TableRowList tableRows, out bool repeatOnNewPage)
		{
			tableRows = null;
			repeatOnNewPage = false;
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (m_reader.LocalName)
					{
					case "FixedHeader":
					{
						bool fixedHeader = m_reader.ReadBoolean();
						if (allowFixedHeaders)
						{
							parent.FixedHeader = fixedHeader;
						}
						else
						{
							m_errorContext.Register(ProcessingErrorCode.rsCantMakeTableGroupHeadersFixed, Severity.Error, context.ObjectType, context.ObjectName, "FixedHeader");
						}
						break;
					}
					case "TableRows":
						tableRows = ReadTableRowList(parent, context, textBoxesWithDefaultSortTarget);
						break;
					case "RepeatOnNewPage":
						repeatOnNewPage = m_reader.ReadBoolean();
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("Header" == m_reader.LocalName || "Footer" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
		}

		private TableRowList ReadTableRowList(Table parent, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget)
		{
			TableRowList tableRowList = new TableRowList();
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "TableRow")
					{
						tableRowList.Add(ReadTableRow(parent, context, textBoxesWithDefaultSortTarget));
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("TableRows" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return tableRowList;
		}

		private TableRow ReadTableRow(Table parent, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget)
		{
			TableRow tableRow = new TableRow(GenerateID(), GenerateID());
			m_reportItemCollectionList.Add(tableRow.ReportItems);
			m_runningValueHolderList.Add(tableRow.ReportItems);
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (m_reader.LocalName)
					{
					case "TableCells":
						ReadTableCells(parent, tableRow.ReportItems, tableRow.ColSpans, context, textBoxesWithDefaultSortTarget);
						break;
					case "Height":
						tableRow.Height = m_reader.ReadString();
						break;
					case "Visibility":
						tableRow.Visibility = ReadVisibility(context);
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("TableRow" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			tableRow.IDs = new IntList();
			for (int i = 0; i < tableRow.ReportItems.Count; i++)
			{
				tableRow.IDs.Add(GenerateID());
			}
			return tableRow;
		}

		private void ReadDetails(Table parent, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget, out TableDetail tableDetail, out TableGroup detailGroup)
		{
			context.Location |= LocationFlags.InDetail;
			tableDetail = null;
			detailGroup = null;
			int numberOfAggregates = m_reportCT.NumberOfAggregates;
			bool flag = false;
			Grouping grouping = null;
			TableRowList tableRowList = null;
			Sorting sorting = null;
			Visibility visibility = null;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (m_reader.LocalName)
					{
					case "TableRows":
						tableRowList = ReadTableRowList(parent, context, textBoxesWithDefaultSortTarget);
						break;
					case "Grouping":
						grouping = ReadGrouping(context);
						break;
					case "Sorting":
						sorting = ReadSorting(context);
						break;
					case "Visibility":
						visibility = ReadVisibility(context);
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("Details" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (grouping != null)
			{
				detailGroup = new TableGroup(GenerateID(), parent);
				m_runningValueHolderList.Add(detailGroup);
				detailGroup.Grouping = grouping;
				detailGroup.HeaderRows = tableRowList;
				detailGroup.Visibility = visibility;
				if (sorting != null)
				{
					if (CanMergeGroupingAndSorting(grouping, sorting))
					{
						detailGroup.Grouping.GroupAndSort = true;
						detailGroup.Grouping.SortDirections = sorting.SortDirections;
						detailGroup.Sorting = null;
						sorting = null;
					}
					else
					{
						detailGroup.Sorting = sorting;
					}
				}
			}
			else
			{
				tableDetail = new TableDetail(GenerateID());
				m_runningValueHolderList.Add(tableDetail);
				tableDetail.DetailRows = tableRowList;
				tableDetail.Sorting = sorting;
				tableDetail.Visibility = visibility;
			}
			if (sorting != null)
			{
				m_hasSorting = true;
			}
			if (m_reportCT.NumberOfAggregates > numberOfAggregates)
			{
				m_aggregateInDetailSections = true;
			}
		}

		private void ReadTableCells(Table parent, ReportItemCollection reportItems, IntList colSpans, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget)
		{
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "TableCell")
					{
						ReadTableCell(parent, reportItems, colSpans, context, textBoxesWithDefaultSortTarget);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("TableCells" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
		}

		private void ReadTableCell(Table parent, ReportItemCollection reportItems, IntList colSpans, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget)
		{
			int num = 1;
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (!(localName == "ReportItems"))
					{
						if (localName == "ColSpan")
						{
							num = m_reader.ReadInteger();
						}
					}
					else
					{
						ReadReportItems("TableCell", parent, reportItems, context, textBoxesWithDefaultSortTarget);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("TableCell" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			colSpans.Add(num);
		}

		private TableColumnList ReadTableColumns(PublishingContextStruct context, Table table)
		{
			TableColumnList tableColumnList = new TableColumnList();
			bool flag = false;
			bool flag2 = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "TableColumn")
					{
						TableColumn tableColumn = ReadTableColumn(context);
						tableColumnList.Add(tableColumn);
						if (tableColumn.FixedHeader)
						{
							flag2 = true;
						}
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("TableColumns" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (flag2)
			{
				bool flag3 = false;
				bool flag4 = false;
				bool flag5 = false;
				for (int i = 0; i < tableColumnList.Count; i++)
				{
					if (tableColumnList[i].FixedHeader)
					{
						if (flag4)
						{
							flag5 = true;
							break;
						}
						flag3 = true;
					}
					else if (flag3)
					{
						flag4 = true;
					}
				}
				if (!flag5 && !tableColumnList[0].FixedHeader && !tableColumnList[tableColumnList.Count - 1].FixedHeader)
				{
					flag5 = true;
				}
				if (!flag5 && !flag4 && tableColumnList[0].FixedHeader && tableColumnList[tableColumnList.Count - 1].FixedHeader)
				{
					flag5 = true;
				}
				if (flag5)
				{
					m_errorContext.Register(ProcessingErrorCode.rsInvalidFixedTableColumnHeaderSpacing, Severity.Error, context.ObjectType, context.ObjectName, "FixedHeader");
				}
				table.HasFixedColumnHeaders = true;
			}
			return tableColumnList;
		}

		private TableColumn ReadTableColumn(PublishingContextStruct context)
		{
			TableColumn tableColumn = new TableColumn();
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (m_reader.LocalName)
					{
					case "FixedHeader":
						tableColumn.FixedHeader = m_reader.ReadBoolean();
						break;
					case "Width":
						tableColumn.Width = ReadSize();
						break;
					case "Visibility":
						tableColumn.Visibility = ReadVisibility(context);
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("TableColumn" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return tableColumn;
		}

		private TableGroup ReadTableGroups(Table table, PublishingContextStruct context)
		{
			TableGroup tableGroup = null;
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "TableGroup")
					{
						TableGroup tableGroup2 = ReadTableGroup(table, context);
						if (tableGroup != null)
						{
							tableGroup.SubGroup = tableGroup2;
						}
						else
						{
							table.TableGroups = tableGroup2;
						}
						tableGroup = tableGroup2;
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("TableGroups" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return tableGroup;
		}

		private TableGroup ReadTableGroup(Table table, PublishingContextStruct context)
		{
			TableGroup tableGroup = new TableGroup(GenerateID(), table);
			m_runningValueHolderList.Add(tableGroup);
			bool flag = false;
			TextBoxList textBoxList = new TextBoxList();
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (m_reader.LocalName)
					{
					case "Grouping":
						tableGroup.Grouping = ReadGrouping(context);
						break;
					case "Sorting":
						tableGroup.Sorting = ReadSorting(context);
						break;
					case "Header":
					{
						ReadHeaderOrFooter(table, context, textBoxList, allowFixedHeaders: false, out TableRowList tableRows2, out bool repeatOnNewPage2);
						tableGroup.HeaderRows = tableRows2;
						tableGroup.HeaderRepeatOnNewPage = repeatOnNewPage2;
						break;
					}
					case "Footer":
					{
						ReadHeaderOrFooter(table, context, textBoxList, allowFixedHeaders: false, out TableRowList tableRows, out bool repeatOnNewPage);
						tableGroup.FooterRows = tableRows;
						tableGroup.FooterRepeatOnNewPage = repeatOnNewPage;
						break;
					}
					case "Visibility":
						tableGroup.Visibility = ReadVisibility(context);
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("TableGroup" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (CanMergeGroupingAndSorting(tableGroup.Grouping, tableGroup.Sorting))
			{
				tableGroup.Grouping.GroupAndSort = true;
				tableGroup.Grouping.SortDirections = tableGroup.Sorting.SortDirections;
				tableGroup.Sorting = null;
			}
			if (tableGroup.Sorting != null)
			{
				m_hasSorting = true;
			}
			Global.Tracer.Assert(tableGroup.Grouping != null);
			SetSortTargetForTextBoxes(textBoxList, tableGroup.Grouping);
			return tableGroup;
		}

		private OWCChart ReadOWCChart(ReportItem parent, PublishingContextStruct context)
		{
			OWCChart oWCChart = new OWCChart(GenerateID(), parent);
			oWCChart.Name = m_reader.GetAttribute("Name");
			bool flag = (context.Location & LocationFlags.InDataRegion) == 0;
			context.Location = (context.Location | LocationFlags.InDataSet | LocationFlags.InDataRegion);
			context.ObjectType = oWCChart.ObjectType;
			context.ObjectName = oWCChart.Name;
			m_dataRegionCount++;
			m_reportItemNames.Validate(context.ObjectType, context.ObjectName, m_errorContext);
			if (m_scopeNames.Validate(isGrouping: false, context.ObjectName, context.ObjectType, context.ObjectName, m_errorContext))
			{
				m_reportScopes.Add(oWCChart.Name, oWCChart);
			}
			bool flag2 = true;
			if ((context.Location & LocationFlags.InPageSection) != 0)
			{
				m_errorContext.Register(ProcessingErrorCode.rsDataRegionInPageSection, Severity.Error, context.ObjectType, context.ObjectName, null);
				flag2 = false;
			}
			if ((context.Location & LocationFlags.InDetail) != 0)
			{
				m_errorContext.Register(ProcessingErrorCode.rsDataRegionInTableDetailRow, Severity.Error, context.ObjectType, context.ObjectName, null);
				flag2 = false;
			}
			m_aggregateHolderList.Add(oWCChart);
			m_runningValueHolderList.Add(oWCChart);
			StyleInformation styleInformation = null;
			bool flag3 = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (m_reader.LocalName)
					{
					case "Style":
						styleInformation = ReadStyle(context);
						break;
					case "Top":
						oWCChart.Top = ReadSize();
						break;
					case "Left":
						oWCChart.Left = ReadSize();
						break;
					case "Height":
						oWCChart.Height = ReadSize();
						break;
					case "Width":
						oWCChart.Width = ReadSize();
						break;
					case "ZIndex":
						oWCChart.ZIndex = m_reader.ReadInteger();
						break;
					case "Visibility":
						oWCChart.Visibility = ReadVisibility(context);
						break;
					case "ToolTip":
						oWCChart.ToolTip = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
						break;
					case "Label":
						oWCChart.Label = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
						break;
					case "Bookmark":
						oWCChart.Bookmark = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
						break;
					case "RepeatWith":
						oWCChart.RepeatedSibling = true;
						oWCChart.RepeatWith = m_reader.ReadString();
						break;
					case "Custom":
						oWCChart.Custom = m_reader.ReadCustomXml();
						break;
					case "CustomProperties":
						oWCChart.CustomProperties = ReadCustomProperties(context);
						break;
					case "KeepTogether":
						oWCChart.KeepTogether = m_reader.ReadBoolean();
						break;
					case "NoRows":
						oWCChart.NoRows = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
						break;
					case "DataSetName":
					{
						string dataSetName = m_reader.ReadString();
						if (flag)
						{
							oWCChart.DataSetName = dataSetName;
						}
						break;
					}
					case "PageBreakAtStart":
						oWCChart.PageBreakAtStart = m_reader.ReadBoolean();
						break;
					case "PageBreakAtEnd":
						oWCChart.PageBreakAtEnd = m_reader.ReadBoolean();
						break;
					case "Filters":
						oWCChart.Filters = ReadFilters(ExpressionParser.ExpressionType.DataRegionFilters, context);
						break;
					case "OWCColumns":
						oWCChart.ChartData = ReadChartColumns(context);
						break;
					case "OWCDefinition":
						oWCChart.ChartDefinition = m_reader.ReadString();
						break;
					case "DataElementName":
						oWCChart.DataElementName = m_reader.ReadString();
						break;
					case "DataElementOutput":
						oWCChart.DataElementOutputRDL = ReadDataElementOutputRDL();
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("OWCChart" == m_reader.LocalName)
					{
						flag3 = true;
					}
					break;
				}
			}
			while (!flag3);
			if (styleInformation != null)
			{
				styleInformation.Filter(StyleOwnerType.OWCChart, oWCChart.NoRows != null);
				oWCChart.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, m_errorContext);
			}
			oWCChart.Computed = true;
			if (flag2)
			{
				m_hasImageStreams = true;
				return oWCChart;
			}
			return null;
		}

		private ChartColumnList ReadChartColumns(PublishingContextStruct context)
		{
			ChartColumnList chartColumnList = new ChartColumnList();
			CLSUniqueNameValidator chartColumnNames = new CLSUniqueNameValidator(ProcessingErrorCode.rsInvalidChartColumnNameNotCLSCompliant, ProcessingErrorCode.rsDuplicateItemName);
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "OWCColumn")
					{
						chartColumnList.Add(ReadChartColumn(chartColumnNames, context));
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("OWCColumns" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return chartColumnList;
		}

		private ChartColumn ReadChartColumn(CLSUniqueNameValidator chartColumnNames, PublishingContextStruct context)
		{
			ChartColumn chartColumn = new ChartColumn();
			chartColumn.Name = m_reader.GetAttribute("Name");
			chartColumnNames.Validate(chartColumn.Name, context.ObjectType, context.ObjectName, m_errorContext);
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "Value")
					{
						chartColumn.Value = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("OWCColumn" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return chartColumn;
		}

		private Sorting ReadSorting(PublishingContextStruct context)
		{
			Sorting sorting = new Sorting(ConstructionPhase.Publishing);
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "SortBy")
					{
						ReadSortBy(sorting, context);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("Sorting" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return sorting;
		}

		private void ReadSortBy(Sorting sorting, PublishingContextStruct context)
		{
			ExpressionInfo expressionInfo = null;
			bool flag = true;
			bool flag2 = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (!(localName == "SortExpression"))
					{
						if (localName == "Direction")
						{
							flag = ReadDirection();
						}
					}
					else
					{
						expressionInfo = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.SortExpression, ExpressionParser.ConstantType.String, context);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("SortBy" == m_reader.LocalName)
					{
						flag2 = true;
					}
					break;
				}
			}
			while (!flag2);
			sorting.SortExpressions.Add(expressionInfo);
			sorting.SortDirections.Add(flag);
			if (expressionInfo.HasRecursiveAggregates())
			{
				m_hasSpecialRecursiveAggregates = true;
			}
		}

		private bool CanMergeGroupingAndSorting(Grouping grouping, Sorting sorting)
		{
			if (grouping != null && grouping.Parent == null && sorting != null && grouping.GroupExpressions != null && sorting.SortExpressions != null && grouping.GroupExpressions.Count == sorting.SortExpressions.Count)
			{
				for (int i = 0; i < grouping.GroupExpressions.Count; i++)
				{
					if (grouping.GroupExpressions[i].OriginalText != sorting.SortExpressions[i].OriginalText)
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		private Grouping ReadGrouping(PublishingContextStruct context)
		{
			m_hasGrouping = true;
			Grouping grouping = new Grouping(ConstructionPhase.Publishing);
			grouping.Name = m_reader.GetAttribute("Name");
			if (m_scopeNames.Validate(isGrouping: true, grouping.Name, context.ObjectType, context.ObjectName, m_errorContext))
			{
				m_reportScopes.Add(grouping.Name, grouping);
			}
			m_aggregateHolderList.Add(grouping);
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (m_reader.LocalName)
					{
					case "Label":
						grouping.GroupLabel = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
						break;
					case "GroupExpressions":
						ReadGroupExpressions(grouping, context);
						break;
					case "PageBreakAtStart":
						grouping.PageBreakAtStart = m_reader.ReadBoolean();
						break;
					case "PageBreakAtEnd":
						grouping.PageBreakAtEnd = m_reader.ReadBoolean();
						break;
					case "Custom":
						grouping.Custom = m_reader.ReadCustomXml();
						break;
					case "CustomProperties":
						grouping.CustomProperties = ReadCustomProperties(context);
						break;
					case "Filters":
						grouping.Filters = ReadFilters(ExpressionParser.ExpressionType.GroupingFilters, context);
						m_hasGroupFilters = true;
						break;
					case "Parent":
						grouping.Parent = new ExpressionInfoList();
						grouping.Parent.Add(ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.GroupExpression, ExpressionParser.ConstantType.String, context));
						break;
					case "DataElementName":
						grouping.DataElementName = m_reader.ReadString();
						break;
					case "DataCollectionName":
						grouping.DataCollectionName = m_reader.ReadString();
						break;
					case "DataElementOutput":
						grouping.DataElementOutput = ReadDataElementOutput();
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("Grouping" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (grouping.Parent != null && 1 != grouping.GroupExpressions.Count)
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidGroupingParent, Severity.Error, context.ObjectType, context.ObjectName, "Parent");
			}
			return grouping;
		}

		private void ReadGroupExpressions(Grouping grouping, PublishingContextStruct context)
		{
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "GroupExpression")
					{
						grouping.GroupExpressions.Add(ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.GroupExpression, ExpressionParser.ConstantType.String, context));
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("GroupExpressions" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
		}

		private Action ReadAction(PublishingContextStruct context, StyleOwnerType styleOwnerType, out bool computed)
		{
			Action action = new Action();
			bool computed2 = false;
			bool flag = false;
			if (!m_reader.IsEmptyElement)
			{
				bool flag2 = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName = m_reader.LocalName;
						if (!(localName == "Style"))
						{
							if (localName == "Actions")
							{
								ReadActionItemList(action, context);
								if (action.ComputedActionItemsCount > 0)
								{
									flag = true;
								}
							}
						}
						else
						{
							StyleInformation styleInformation = ReadStyle(context, out computed2);
							styleInformation.Filter(styleOwnerType, hasNoRows: false);
							action.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, m_errorContext);
						}
						break;
					}
					case XmlNodeType.EndElement:
						if ("ActionInfo" == m_reader.LocalName)
						{
							flag2 = true;
						}
						break;
					}
				}
				while (!flag2);
			}
			computed = (computed2 || flag);
			return action;
		}

		private void ReadActionItemList(Action actionInfo, PublishingContextStruct context)
		{
			int computedIndex = -1;
			bool flag = false;
			bool computed = false;
			bool missingLabel = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "Action")
					{
						actionInfo.ActionItems.Add(ReadActionItem(context, out computed, ref computedIndex, ref missingLabel));
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("Actions" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			computedIndex = (actionInfo.ComputedActionItemsCount = computedIndex + 1);
			if (missingLabel && actionInfo.ActionItems.Count > 1)
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidActionLabel, Severity.Error, context.ObjectType, context.ObjectName, "Actions");
			}
		}

		private ActionItem ReadActionItem(PublishingContextStruct context, out bool computed, ref int computedIndex, ref bool missingLabel)
		{
			ActionItem actionItem = new ActionItem();
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool computed2 = false;
			bool computed3 = false;
			bool computed4 = false;
			bool computed5 = false;
			if (!m_reader.IsEmptyElement)
			{
				bool flag5 = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (m_reader.LocalName)
						{
						case "Hyperlink":
							m_hasHyperlinks = true;
							flag = true;
							actionItem.HyperLinkURL = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out computed2);
							break;
						case "Drillthrough":
							flag2 = true;
							ReadDrillthrough(context, actionItem, out computed3);
							break;
						case "BookmarkLink":
							flag3 = true;
							actionItem.BookmarkLink = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out computed4);
							break;
						case "Label":
							flag4 = true;
							actionItem.Label = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out computed5);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("Action" == m_reader.LocalName)
						{
							flag5 = true;
						}
						break;
					}
				}
				while (!flag5);
			}
			int num = 0;
			if (flag)
			{
				num++;
			}
			if (flag2)
			{
				num++;
				if ((context.Location & LocationFlags.InPageSection) > (LocationFlags)0)
				{
					m_pageSectionDrillthroughs = true;
				}
			}
			if (flag3)
			{
				num++;
			}
			if (1 != num)
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidAction, Severity.Error, context.ObjectType, context.ObjectName, "Action");
			}
			if (!flag4)
			{
				missingLabel = true;
			}
			computed = (computed2 || computed3 || computed4 || computed5);
			if (computed)
			{
				computedIndex++;
				actionItem.ComputedIndex = computedIndex;
			}
			return actionItem;
		}

		private void ReadDrillthrough(PublishingContextStruct context, ActionItem actionItem, out bool computed)
		{
			computed = false;
			bool computed2 = false;
			bool computed3 = false;
			bool computed4 = false;
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (m_reader.LocalName)
					{
					case "ReportName":
						actionItem.DrillthroughReportName = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out computed4);
						if (ExpressionInfo.Types.Constant == actionItem.DrillthroughReportName.Type)
						{
							actionItem.DrillthroughReportName.Value = PublishingValidator.ValidateReportName(m_reportContext, actionItem.DrillthroughReportName.Value, context.ObjectType, context.ObjectName, "DrillthroughReportName", m_errorContext);
						}
						break;
					case "Parameters":
						actionItem.DrillthroughParameters = ReadParameters(context, omitAllowed: true, doClsValidation: false, out computed2);
						break;
					case "BookmarkLink":
						actionItem.DrillthroughBookmarkLink = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out computed3);
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("Drillthrough" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			computed = (computed2 || computed3 || computed4);
		}

		private Visibility ReadVisibility(PublishingContextStruct context, out bool computed)
		{
			m_static = true;
			Visibility visibility = new Visibility();
			bool computed2 = false;
			bool flag = false;
			if (!m_reader.IsEmptyElement)
			{
				bool flag2 = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName = m_reader.LocalName;
						if (!(localName == "Hidden"))
						{
							if (localName == "ToggleItem")
							{
								flag = true;
								if ((context.Location & LocationFlags.InPageSection) != 0)
								{
									m_errorContext.Register(ProcessingErrorCode.rsToggleInPageSection, Severity.Error, context.ObjectType, context.ObjectName, "ToggleItem");
								}
								m_interactive = true;
								visibility.Toggle = m_reader.ReadString();
							}
						}
						else
						{
							visibility.Hidden = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.Boolean, context, out computed2);
						}
						break;
					}
					case XmlNodeType.EndElement:
						if ("Visibility" == m_reader.LocalName)
						{
							flag2 = true;
						}
						break;
					}
				}
				while (!flag2);
			}
			computed = (computed2 || flag);
			return visibility;
		}

		private Visibility ReadVisibility(PublishingContextStruct context)
		{
			bool computed;
			return ReadVisibility(context, out computed);
		}

		private DataValueList ReadCustomProperties(PublishingContextStruct context)
		{
			bool computed;
			return ReadCustomProperties(context, out computed);
		}

		private DataValueList ReadCustomProperties(PublishingContextStruct context, out bool computed)
		{
			bool flag = false;
			computed = false;
			int num = 0;
			DataValueList dataValueList = new DataValueList();
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "CustomProperty")
					{
						dataValueList.Add(ReadDataValue(isCustomProperty: true, ++num, ref computed, context));
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("CustomProperties" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return dataValueList;
		}

		private DataValue ReadDataValue(bool isCustomProperty, int index, PublishingContextStruct context)
		{
			bool isComputed = false;
			return ReadDataValue(isCustomProperty, index, ref isComputed, context);
		}

		private DataValue ReadDataValue(bool isCustomProperty, int index, ref bool isComputed, PublishingContextStruct context)
		{
			DataValue dataValue = new DataValue();
			bool computed = false;
			bool computed2 = false;
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (!(localName == "Name"))
					{
						if (localName == "Value")
						{
							dataValue.Value = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out computed2);
						}
					}
					else
					{
						dataValue.Name = ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out computed);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ((isCustomProperty && "CustomProperty" == m_reader.LocalName) || (!isCustomProperty && "DataValue" == m_reader.LocalName))
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			Global.Tracer.Assert(dataValue.Value != null);
			if (dataValue.Name == null && isCustomProperty)
			{
				m_errorContext.Register(ProcessingErrorCode.rsMissingCustomPropertyName, Severity.Error, context.ObjectType, context.ObjectName, "Name", index.ToString(CultureInfo.CurrentCulture));
			}
			isComputed |= (computed2 || computed);
			return dataValue;
		}

		private bool CheckUserProfileDependency()
		{
			bool result = false;
			if (m_reportLocationFlags == UserLocationFlags.ReportBody)
			{
				if ((m_userReferenceLocation & UserLocationFlags.ReportBody) == 0)
				{
					result = true;
				}
			}
			else if (m_reportLocationFlags == UserLocationFlags.ReportPageSection)
			{
				if ((m_userReferenceLocation & UserLocationFlags.ReportPageSection) == 0)
				{
					result = true;
				}
			}
			else if (m_reportLocationFlags == UserLocationFlags.ReportQueries && (m_userReferenceLocation & UserLocationFlags.ReportQueries) == 0)
			{
				result = true;
			}
			return result;
		}

		private void SetUserProfileDependency()
		{
			if (m_reportLocationFlags == UserLocationFlags.ReportBody)
			{
				m_userReferenceLocation |= UserLocationFlags.ReportBody;
			}
			else if (m_reportLocationFlags == UserLocationFlags.ReportPageSection)
			{
				m_userReferenceLocation |= UserLocationFlags.ReportPageSection;
			}
			else if (m_reportLocationFlags == UserLocationFlags.ReportQueries)
			{
				m_userReferenceLocation |= UserLocationFlags.ReportQueries;
			}
		}

		private ExpressionInfo ReadExpression(string expression, string propertyName, string dataSetName, ExpressionParser.ExpressionType expressionType, ExpressionParser.ConstantType constantType, PublishingContextStruct context)
		{
			return ReadExpression(parseExtended: false, expression, propertyName, dataSetName, expressionType, constantType, context);
		}

		private ExpressionInfo ReadExpression(bool parseExtended, string expression, string propertyName, string dataSetName, ExpressionParser.ExpressionType expressionType, ExpressionParser.ConstantType constantType, PublishingContextStruct context)
		{
			ExpressionParser.ExpressionContext context2 = context.CreateExpressionContext(expressionType, constantType, propertyName, dataSetName, parseExtended);
			if (!CheckUserProfileDependency())
			{
				return m_reportCT.ParseExpression(expression, context2);
			}
			bool userCollectionReferenced;
			ExpressionInfo result = m_reportCT.ParseExpression(expression, context2, out userCollectionReferenced);
			if (userCollectionReferenced)
			{
				SetUserProfileDependency();
			}
			return result;
		}

		private ExpressionInfo ReadExpression(string propertyName, string dataSetName, ExpressionParser.ExpressionType expressionType, ExpressionParser.ConstantType constantType, PublishingContextStruct context, out bool userCollectionReferenced)
		{
			ExpressionParser.ExpressionContext context2 = context.CreateExpressionContext(expressionType, constantType, propertyName, dataSetName);
			return m_reportCT.ParseExpression(m_reader.ReadString(), context2, out userCollectionReferenced);
		}

		private ExpressionInfo ReadExpression(string propertyName, string dataSetName, ExpressionParser.ExpressionType expressionType, ExpressionParser.ConstantType constantType, PublishingContextStruct context)
		{
			return ReadExpression(m_reader.ReadString(), propertyName, dataSetName, expressionType, constantType, context);
		}

		private ExpressionInfo ReadExpression(string propertyName, ExpressionParser.ExpressionType expressionType, ExpressionParser.ConstantType constantType, PublishingContextStruct context)
		{
			return ReadExpression(propertyName, null, expressionType, constantType, context);
		}

		private ExpressionInfo ReadExpression(string propertyName, ExpressionParser.ExpressionType expressionType, ExpressionParser.ConstantType constantType, PublishingContextStruct context, out bool computed)
		{
			ExpressionInfo expressionInfo = ReadExpression(propertyName, expressionType, constantType, context);
			if (ExpressionInfo.Types.Constant == expressionInfo.Type)
			{
				computed = false;
			}
			else
			{
				computed = true;
			}
			return expressionInfo;
		}

		private ExpressionInfo ReadExpression(string propertyName, string dataSetName, ExpressionParser.ExpressionType expressionType, ExpressionParser.ConstantType constantType, PublishingContextStruct context, ExpressionParser.DetectionFlags flag, out bool reportParameterReferenced, out string reportParameterName)
		{
			ExpressionParser.ExpressionContext context2 = context.CreateExpressionContext(expressionType, constantType, propertyName, dataSetName);
			if (CheckUserProfileDependency())
			{
				flag |= ExpressionParser.DetectionFlags.UserReference;
			}
			bool userCollectionReferenced;
			ExpressionInfo result = m_reportCT.ParseExpression(m_reader.ReadString(), context2, flag, out reportParameterReferenced, out reportParameterName, out userCollectionReferenced);
			if (userCollectionReferenced)
			{
				SetUserProfileDependency();
			}
			return result;
		}

		private ExpressionInfo ReadExpression(string propertyName, string dataSetName, ExpressionParser.ExpressionType expressionType, ExpressionParser.ConstantType constantType, PublishingContextStruct context, ExpressionParser.DetectionFlags flag, out bool reportParameterReferenced, out string reportParameterName, out bool userCollectionReferenced)
		{
			ExpressionParser.ExpressionContext context2 = context.CreateExpressionContext(expressionType, constantType, propertyName, dataSetName);
			return m_reportCT.ParseExpression(m_reader.ReadString(), context2, flag, out reportParameterReferenced, out reportParameterName, out userCollectionReferenced);
		}

		private DataSet.Sensitivity ReadSensitivity()
		{
			string value = m_reader.ReadString();
			return (DataSet.Sensitivity)Enum.Parse(typeof(DataSet.Sensitivity), value, ignoreCase: false);
		}

		private CommandType ReadCommandType()
		{
			string value = m_reader.ReadString();
			return (CommandType)Enum.Parse(typeof(CommandType), value, ignoreCase: false);
		}

		private Filter.Operators ReadOperator()
		{
			string value = m_reader.ReadString();
			return (Filter.Operators)Enum.Parse(typeof(Filter.Operators), value, ignoreCase: false);
		}

		private bool ReadDirection()
		{
			string x = m_reader.ReadString();
			return ReportProcessing.CompareWithInvariantCulture(x, "Ascending", ignoreCase: false) == 0;
		}

		private bool ReadLayoutDirection()
		{
			string x = m_reader.ReadString();
			return ReportProcessing.CompareWithInvariantCulture(x, "RTL", ignoreCase: false) == 0;
		}

		private bool ReadProjectionMode()
		{
			string x = m_reader.ReadString();
			return ReportProcessing.CompareWithInvariantCulture(x, "Perspective", ignoreCase: false) == 0;
		}

		private bool ReadDrawingStyle()
		{
			string x = m_reader.ReadString();
			return ReportProcessing.CompareWithInvariantCulture(x, "Cube", ignoreCase: false) == 0;
		}

		private Image.SourceType ReadSource()
		{
			string value = m_reader.ReadString();
			return (Image.SourceType)Enum.Parse(typeof(Image.SourceType), value, ignoreCase: false);
		}

		private Image.Sizings ReadSizing()
		{
			string value = m_reader.ReadString();
			return (Image.Sizings)Enum.Parse(typeof(Image.Sizings), value, ignoreCase: false);
		}

		private bool ReadDataElementStyle()
		{
			string x = m_reader.ReadString();
			return ReportProcessing.CompareWithInvariantCulture(x, "AttributeNormal", ignoreCase: false) == 0;
		}

		private ReportItem.DataElementStylesRDL ReadDataElementStyleRDL()
		{
			string value = m_reader.ReadString();
			return (ReportItem.DataElementStylesRDL)Enum.Parse(typeof(ReportItem.DataElementStylesRDL), value, ignoreCase: false);
		}

		private ReportItem.DataElementOutputTypesRDL ReadDataElementOutputRDL()
		{
			string value = m_reader.ReadString();
			return (ReportItem.DataElementOutputTypesRDL)Enum.Parse(typeof(ReportItem.DataElementOutputTypesRDL), value, ignoreCase: false);
		}

		private DataElementOutputTypes ReadDataElementOutput()
		{
			string value = m_reader.ReadString();
			return (DataElementOutputTypes)Enum.Parse(typeof(DataElementOutputTypes), value, ignoreCase: false);
		}

		private Subtotal.PositionType ReadPosition()
		{
			string value = m_reader.ReadString();
			return (Subtotal.PositionType)Enum.Parse(typeof(Subtotal.PositionType), value, ignoreCase: false);
		}

		private ChartDataLabel.Positions ReadDataLabelPosition()
		{
			string value = m_reader.ReadString();
			return (ChartDataLabel.Positions)Enum.Parse(typeof(ChartDataLabel.Positions), value, ignoreCase: false);
		}

		private Chart.ChartTypes ReadChartType()
		{
			string value = m_reader.ReadString();
			return (Chart.ChartTypes)Enum.Parse(typeof(Chart.ChartTypes), value, ignoreCase: false);
		}

		private Chart.ChartSubTypes ReadChartSubType()
		{
			string value = m_reader.ReadString();
			return (Chart.ChartSubTypes)Enum.Parse(typeof(Chart.ChartSubTypes), value, ignoreCase: false);
		}

		private Chart.ChartPalette ReadChartPalette()
		{
			string value = m_reader.ReadString();
			return (Chart.ChartPalette)Enum.Parse(typeof(Chart.ChartPalette), value, ignoreCase: false);
		}

		private ChartTitle.Positions ReadChartTitlePosition()
		{
			string value = m_reader.ReadString();
			return (ChartTitle.Positions)Enum.Parse(typeof(ChartTitle.Positions), value, ignoreCase: false);
		}

		private Legend.Positions ReadLegendPosition()
		{
			string value = m_reader.ReadString();
			return (Legend.Positions)Enum.Parse(typeof(Legend.Positions), value, ignoreCase: false);
		}

		private Legend.LegendLayout ReadLegendLayout()
		{
			string value = m_reader.ReadString();
			return (Legend.LegendLayout)Enum.Parse(typeof(Legend.LegendLayout), value, ignoreCase: false);
		}

		private MultiChart.Layouts ReadLayout()
		{
			string value = m_reader.ReadString();
			return (MultiChart.Layouts)Enum.Parse(typeof(MultiChart.Layouts), value, ignoreCase: false);
		}

		private Axis.TickMarks ReadAxisTickMarks()
		{
			string value = m_reader.ReadString();
			return (Axis.TickMarks)Enum.Parse(typeof(Axis.TickMarks), value, ignoreCase: false);
		}

		private ThreeDProperties.ShadingTypes ReadShading()
		{
			string value = m_reader.ReadString();
			return (ThreeDProperties.ShadingTypes)Enum.Parse(typeof(ThreeDProperties.ShadingTypes), value, ignoreCase: false);
		}

		private ChartDataPoint.MarkerTypes ReadMarkerType()
		{
			string value = m_reader.ReadString();
			return (ChartDataPoint.MarkerTypes)Enum.Parse(typeof(ChartDataPoint.MarkerTypes), value, ignoreCase: false);
		}

		private bool ReadPlotType()
		{
			string x = m_reader.ReadString();
			return ReportProcessing.CompareWithInvariantCulture(x, "Line", ignoreCase: false) == 0;
		}

		private StyleInformation ReadStyle(PublishingContextStruct context, out bool computed)
		{
			computed = false;
			StyleInformation styleInformation = new StyleInformation();
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						bool computed2 = false;
						switch (m_reader.LocalName)
						{
						case "BorderColor":
						case "BorderStyle":
						case "BorderWidth":
							ReadBorderAttribute(m_reader.LocalName, styleInformation, context, out computed2);
							break;
						case "BackgroundImage":
							ReadBackgroundImage(styleInformation, context, out computed2);
							break;
						case "Format":
						case "Language":
						case "Calendar":
						case "NumeralLanguage":
						case "BackgroundColor":
						case "BackgroundGradientType":
						case "BackgroundGradientEndColor":
						case "FontStyle":
						case "FontFamily":
						case "FontSize":
						case "FontWeight":
						case "TextDecoration":
						case "TextAlign":
						case "VerticalAlign":
						case "Color":
						case "PaddingLeft":
						case "PaddingRight":
						case "PaddingTop":
						case "PaddingBottom":
						case "LineHeight":
						case "Direction":
						case "WritingMode":
						case "UnicodeBiDi":
							styleInformation.AddAttribute(m_reader.LocalName, ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out computed2));
							break;
						case "NumeralVariant":
							styleInformation.AddAttribute(m_reader.LocalName, ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.Integer, context, out computed2));
							break;
						}
						computed |= computed2;
						break;
					}
					case XmlNodeType.EndElement:
						if ("Style" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return styleInformation;
		}

		private StyleInformation ReadStyle(PublishingContextStruct context)
		{
			bool computed;
			return ReadStyle(context, out computed);
		}

		private void ReadBorderAttribute(string borderAttribute, StyleInformation styleInfo, PublishingContextStruct context, out bool computed)
		{
			computed = false;
			if (m_reader.IsEmptyElement)
			{
				return;
			}
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string text = null;
					bool computed2 = false;
					switch (m_reader.LocalName)
					{
					case "Default":
					case "Left":
					case "Right":
					case "Top":
					case "Bottom":
						text = ((!("Default" == m_reader.LocalName)) ? m_reader.LocalName : string.Empty);
						styleInfo.AddAttribute(borderAttribute + text, ReadExpression(borderAttribute, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out computed2));
						break;
					}
					computed |= computed2;
					break;
				}
				case XmlNodeType.EndElement:
					if (borderAttribute == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
		}

		private void ReadBackgroundImage(StyleInformation styleInfo, PublishingContextStruct context, out bool computed)
		{
			bool computed2 = false;
			bool computed3 = false;
			bool computed4 = false;
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (m_reader.LocalName)
					{
					case "Source":
					{
						ExpressionInfo expressionInfo = new ExpressionInfo();
						expressionInfo.Type = ExpressionInfo.Types.Constant;
						expressionInfo.IntValue = (int)ReadSource();
						styleInfo.AddAttribute("BackgroundImageSource", expressionInfo);
						if (expressionInfo.IntValue == 0)
						{
							m_hasExternalImages = true;
						}
						break;
					}
					case "Value":
						styleInfo.AddAttribute("BackgroundImageValue", ReadExpression("BackgroundImageValue", ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out computed2));
						break;
					case "MIMEType":
						styleInfo.AddAttribute("BackgroundImageMIMEType", ReadExpression("BackgroundImageMIMEType", ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out computed3));
						break;
					case "BackgroundRepeat":
						styleInfo.AddAttribute(m_reader.LocalName, ReadExpression(m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out computed4));
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("BackgroundImage" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			computed = (computed2 || computed3 || computed4);
			m_hasImageStreams = true;
		}

		private string ReadSize()
		{
			return m_reader.ReadString();
		}

		private void Phase2()
		{
			if (1 < m_dataSets.Count)
			{
				m_reportCT.ConvertFields2ComplexExpr();
			}
			else
			{
				m_report.OneDataSetName = ((m_dataSets.Count == 1) ? m_dataSets[0].Name : null);
			}
			if (0 < m_textBoxesWithUserSortTarget.Count)
			{
				for (int i = 0; i < m_textBoxesWithUserSortTarget.Count; i++)
				{
					EndUserSort userSort = m_textBoxesWithUserSortTarget[i].UserSort;
					ISortFilterScope sortFilterScope = m_reportScopes[userSort.SortTargetString] as ISortFilterScope;
					if (sortFilterScope != null)
					{
						userSort.SetSortTarget(sortFilterScope);
					}
				}
			}
			m_report.MergeOnePass = (!m_hasGrouping && !m_hasSorting && !m_aggregateInDetailSections && !m_reportCT.BodyRefersToReportItems && !m_reportCT.ValueReferencedGlobal && !m_subReportMergeTransactions && !m_hasUserSort);
			m_report.PageMergeOnePass = (m_report.PageAggregates.Count == 0 && !m_reportCT.PageSectionRefersToReportItems);
			m_report.SubReportMergeTransactions = m_subReportMergeTransactions;
			m_report.NeedPostGroupProcessing = (m_hasSorting | m_hasGroupFilters);
			m_report.HasSpecialRecursiveAggregates = m_hasSpecialRecursiveAggregates;
			m_report.HasReportItemReferences = m_reportCT.BodyRefersToReportItems;
			m_report.HasImageStreams = m_hasImageStreams;
			m_report.HasBookmarks = m_hasBookmarks;
			m_report.HasLabels = m_hasLabels;
			m_report.HasUserSortFilter = m_hasUserSort;
		}

		private void Phase3(ICatalogItemContext reportContext, out ParameterInfoCollection parameters, AppDomain compilationTempAppDomain, bool generateExpressionHostWithRefusedPermissions)
		{
			try
			{
				m_reportCT.Builder.ReportStart();
				m_report.LastAggregateID = m_reportCT.LastAggregateID;
				InitializationContext context = new InitializationContext(reportContext, m_hasFilters, m_dataSourceNames, m_dataSets, m_dynamicParameters, m_dataSetQueryInfo, m_errorContext, m_reportCT.Builder, m_report, m_reportLanguage, m_reportScopes, m_hasUserSortPeerScopes, m_dataRegionCount);
				m_report.Initialize(context);
				bool parametersNotUsedInQuery = false;
				ParameterInfo parameterInfo = null;
				parameters = new ParameterInfoCollection();
				ParameterDefList parameters2 = m_report.Parameters;
				if (parameters2 != null && parameters2.Count > 0)
				{
					context.InitializeParameters(m_report.Parameters, m_dataSets);
					for (int i = 0; i < parameters2.Count; i++)
					{
						ParameterDef parameterDef = parameters2[i];
						if (parameterDef.UsedInQueryAsDefined == ParameterBase.UsedInQueryType.Auto)
						{
							if (m_parametersNotUsedInQuery)
							{
								if (m_usedInQueryInfos.Contains(parameterDef.Name))
								{
									parameterDef.UsedInQuery = true;
								}
								else
								{
									parameterDef.UsedInQuery = false;
									parametersNotUsedInQuery = true;
								}
							}
							else
							{
								parameterDef.UsedInQuery = true;
							}
						}
						else if (parameterDef.UsedInQueryAsDefined == ParameterBase.UsedInQueryType.False)
						{
							parametersNotUsedInQuery = true;
							parameterDef.UsedInQuery = false;
						}
						if (parameterDef.UsedInQuery && (m_userReferenceLocation & UserLocationFlags.ReportQueries) == 0 && m_reportParamUserProfile.Contains(parameterDef.Name))
						{
							m_userReferenceLocation |= UserLocationFlags.ReportQueries;
						}
						parameterDef.Initialize(context);
						parameterInfo = new ParameterInfo(parameterDef);
						if (parameterDef.Dependencies != null && parameterDef.Dependencies.Count > 0)
						{
							int num = 0;
							IDictionaryEnumerator enumerator = parameterDef.Dependencies.GetEnumerator();
							ParameterDefList parameterDefList = new ParameterDefList();
							ParameterInfoCollection parameterInfoCollection = new ParameterInfoCollection();
							while (enumerator.MoveNext())
							{
								num = (int)enumerator.Value;
								parameterDefList.Add(parameters2[num]);
								parameterInfoCollection.Add(parameters[num]);
								if (parameterDef.UsedInQuery)
								{
									parameters[num].UsedInQuery = true;
								}
							}
							parameterDef.DependencyList = parameterDefList;
							parameterInfo.DependencyList = parameterInfoCollection;
						}
						if (parameterDef.ValidValuesDataSource != null)
						{
							parameterInfo.DynamicValidValues = true;
						}
						else if (parameterDef.ValidValuesValueExpressions != null)
						{
							int count = parameterDef.ValidValuesValueExpressions.Count;
							for (int j = 0; j < count; j++)
							{
								if (parameterInfo.DynamicValidValues)
								{
									break;
								}
								ExpressionInfo expressionInfo = parameterDef.ValidValuesValueExpressions[j];
								ExpressionInfo expressionInfo2 = parameterDef.ValidValuesLabelExpressions[j];
								if ((expressionInfo != null && ExpressionInfo.Types.Constant != expressionInfo.Type) || (expressionInfo2 != null && ExpressionInfo.Types.Constant != expressionInfo2.Type))
								{
									parameterInfo.DynamicValidValues = true;
								}
							}
							if (!parameterInfo.DynamicValidValues)
							{
								parameterInfo.ValidValues = new ValidValueList(count);
								for (int k = 0; k < count; k++)
								{
									ExpressionInfo expressionInfo3 = parameterDef.ValidValuesValueExpressions[k];
									ExpressionInfo expressionInfo4 = parameterDef.ValidValuesLabelExpressions[k];
									parameterInfo.AddValidValue(expressionInfo3?.Value, expressionInfo4?.Value, m_errorContext, CultureInfo.InvariantCulture);
								}
							}
						}
						parameterInfo.DynamicDefaultValue = (parameterDef.DefaultDataSource != null || parameterDef.DefaultExpressions != null);
						parameterInfo.Values = parameterDef.DefaultValues;
						parameters.Add(parameterInfo);
					}
				}
				m_parametersNotUsedInQuery = parametersNotUsedInQuery;
				m_report.ParametersNotUsedInQuery = m_parametersNotUsedInQuery;
				m_reportCT.Builder.ReportEnd();
				if (!m_errorContext.HasError)
				{
					m_report.CompiledCode = m_reportCT.Compile(m_report, compilationTempAppDomain, generateExpressionHostWithRefusedPermissions);
				}
				if (!m_report.MergeOnePass)
				{
					return;
				}
				int num2 = 0;
				int num3 = 0;
				while (true)
				{
					if (num3 >= m_dataSets.Count)
					{
						return;
					}
					if (!m_dataSets[num3].UsedOnlyInParameters)
					{
						num2++;
						if (1 < num2)
						{
							break;
						}
					}
					num3++;
				}
				m_report.MergeOnePass = false;
			}
			finally
			{
				m_reportCT = null;
			}
		}

		private void Phase4()
		{
			PopulateReportItemCollections();
			CompactAggregates();
			CompactRunningValues();
		}

		private void PopulateReportItemCollections()
		{
			try
			{
				Global.Tracer.Assert(m_reportItemCollectionList != null);
				for (int i = 0; i < m_reportItemCollectionList.Count; i++)
				{
					((ReportItemCollection)m_reportItemCollectionList[i]).Populate(m_errorContext);
				}
			}
			finally
			{
				m_reportItemCollectionList = null;
			}
		}

		private void CompactAggregates()
		{
			try
			{
				Hashtable aggregateHashByType = new Hashtable();
				for (int i = 0; i < m_aggregateHolderList.Count; i++)
				{
					IAggregateHolder aggregateHolder = (IAggregateHolder)m_aggregateHolderList[i];
					Global.Tracer.Assert(aggregateHolder != null);
					DataAggregateInfoList[] aggregateLists = aggregateHolder.GetAggregateLists();
					Global.Tracer.Assert(aggregateLists != null);
					Global.Tracer.Assert(aggregateLists.Length != 0);
					CompactAggregates(aggregateLists, aggregateHashByType);
					if (CompactAggregates(aggregateHolder.GetPostSortAggregateLists(), aggregateHashByType))
					{
						m_report.HasPostSortAggregates = true;
					}
					if (aggregateHolder is Grouping && CompactAggregates(((Grouping)aggregateHolder).RecursiveAggregates, aggregateHashByType))
					{
						m_report.NeedPostGroupProcessing = true;
					}
					aggregateHolder.ClearIfEmpty();
				}
			}
			finally
			{
				m_aggregateHolderList = null;
			}
		}

		private bool CompactAggregates(DataAggregateInfoList[] aggregateLists, Hashtable aggregateHashByType)
		{
			bool result = false;
			if (aggregateLists != null)
			{
				foreach (DataAggregateInfoList dataAggregateInfoList in aggregateLists)
				{
					Global.Tracer.Assert(dataAggregateInfoList != null);
					if (CompactAggregates(dataAggregateInfoList, aggregateHashByType))
					{
						result = true;
					}
					aggregateHashByType.Clear();
				}
			}
			return result;
		}

		private bool CompactAggregates(DataAggregateInfoList aggregateList, Hashtable aggregateHashByType)
		{
			bool result = false;
			for (int num = aggregateList.Count - 1; num >= 0; num--)
			{
				result = true;
				DataAggregateInfo dataAggregateInfo = aggregateList[num];
				Global.Tracer.Assert(dataAggregateInfo != null);
				if (!dataAggregateInfo.IsCopied)
				{
					Hashtable hashtable = (Hashtable)aggregateHashByType[dataAggregateInfo.AggregateType];
					if (hashtable == null)
					{
						hashtable = new Hashtable();
						aggregateHashByType[dataAggregateInfo.AggregateType] = hashtable;
					}
					DataAggregateInfo dataAggregateInfo2 = (DataAggregateInfo)hashtable[dataAggregateInfo.ExpressionText];
					if (dataAggregateInfo2 == null)
					{
						hashtable[dataAggregateInfo.ExpressionText] = dataAggregateInfo;
					}
					else
					{
						if (dataAggregateInfo2.DuplicateNames == null)
						{
							dataAggregateInfo2.DuplicateNames = new StringList();
						}
						dataAggregateInfo2.DuplicateNames.Add(dataAggregateInfo.Name);
						aggregateList.RemoveAt(num);
					}
				}
			}
			return result;
		}

		private void CompactRunningValues()
		{
			try
			{
				Hashtable runningValueHashByType = new Hashtable();
				for (int i = 0; i < m_runningValueHolderList.Count; i++)
				{
					IRunningValueHolder runningValueHolder = (IRunningValueHolder)m_runningValueHolderList[i];
					Global.Tracer.Assert(runningValueHolder != null);
					CompactRunningValueList(runningValueHolder.GetRunningValueList(), runningValueHashByType);
					if (runningValueHolder is OWCChart)
					{
						CompactRunningValueList(((OWCChart)runningValueHolder).DetailRunningValues, runningValueHashByType);
					}
					else if (runningValueHolder is Chart)
					{
						CompactRunningValueList(((Chart)runningValueHolder).CellRunningValues, runningValueHashByType);
					}
					else if (runningValueHolder is CustomReportItem)
					{
						CompactRunningValueList(((CustomReportItem)runningValueHolder).CellRunningValues, runningValueHashByType);
					}
					runningValueHolder.ClearIfEmpty();
				}
			}
			finally
			{
				m_runningValueHolderList = null;
			}
		}

		private void CompactRunningValueList(RunningValueInfoList runningValueList, Hashtable runningValueHashByType)
		{
			Global.Tracer.Assert(runningValueList != null);
			Global.Tracer.Assert(runningValueHashByType != null);
			for (int num = runningValueList.Count - 1; num >= 0; num--)
			{
				m_report.HasPostSortAggregates = true;
				RunningValueInfo runningValueInfo = runningValueList[num];
				Global.Tracer.Assert(runningValueInfo != null);
				AllowNullKeyHashtable allowNullKeyHashtable = (AllowNullKeyHashtable)runningValueHashByType[runningValueInfo.AggregateType];
				if (allowNullKeyHashtable == null)
				{
					allowNullKeyHashtable = new AllowNullKeyHashtable();
					runningValueHashByType[runningValueInfo.AggregateType] = allowNullKeyHashtable;
				}
				Hashtable hashtable = (Hashtable)allowNullKeyHashtable[runningValueInfo.Scope];
				if (hashtable == null)
				{
					hashtable = new Hashtable();
					allowNullKeyHashtable[runningValueInfo.Scope] = hashtable;
				}
				RunningValueInfo runningValueInfo2 = (RunningValueInfo)hashtable[runningValueInfo.ExpressionText];
				if (runningValueInfo2 == null)
				{
					hashtable[runningValueInfo.ExpressionText] = runningValueInfo;
				}
				else
				{
					if (runningValueInfo2.DuplicateNames == null)
					{
						runningValueInfo2.DuplicateNames = new StringList();
					}
					runningValueInfo2.DuplicateNames.Add(runningValueInfo.Name);
					runningValueList.RemoveAt(num);
				}
			}
			runningValueHashByType.Clear();
		}

		internal static void CalculateChildrenDependencies(ReportItem reportItem)
		{
			ReportItemCollection reportItemCollection = null;
			if (!(reportItem is DataRegion) && !(reportItem is Rectangle) && !(reportItem is Report))
			{
				return;
			}
			if (reportItem is Rectangle)
			{
				reportItemCollection = ((Rectangle)reportItem).ReportItems;
			}
			else if (reportItem is List)
			{
				reportItemCollection = ((List)reportItem).ReportItems;
			}
			else if (reportItem is Report)
			{
				reportItemCollection = ((Report)reportItem).ReportItems;
			}
			if (reportItemCollection == null || reportItemCollection.Count < 1)
			{
				return;
			}
			double num = -1.0;
			double num2 = 0.0;
			for (int i = 0; i < reportItemCollection.Count; i++)
			{
				ReportItem reportItem2 = reportItemCollection[i];
				num2 = reportItem2.TopValue + reportItem2.HeightValue;
				num = -1.0;
				bool flag = HasPageBreakAtStart(reportItem2);
				for (int j = i + 1; j < reportItemCollection.Count; j++)
				{
					ReportItem reportItem3 = reportItemCollection[j];
					if (reportItem3.TopValue < reportItem2.TopValue)
					{
						continue;
					}
					bool flag2 = false;
					if (flag && reportItem3.TopValue >= reportItem2.TopValue && reportItem3.TopValue <= num2)
					{
						flag2 = true;
					}
					if (num >= 0.0 && num <= reportItem3.TopValue + 0.0009)
					{
						break;
					}
					if (!reportItemCollection.IsReportItemComputed(j))
					{
						flag2 = true;
					}
					bool flag3 = false;
					if (num2 <= reportItem3.TopValue + 0.0009 || flag2)
					{
						flag3 = true;
						if (!flag2 && (num < 0.0 || num > reportItem3.TopValue + reportItem3.HeightValue))
						{
							num = reportItem3.TopValue + reportItem3.HeightValue;
						}
					}
					else if (!flag2 && i + 1 == j && reportItem3.DistanceBeforeTop == 0)
					{
						flag3 = true;
					}
					if (flag3)
					{
						if (reportItem3.SiblingAboveMe == null)
						{
							reportItem3.SiblingAboveMe = new IntList();
						}
						reportItem3.SiblingAboveMe.Add(i);
					}
				}
				CalculateChildrenDependencies(reportItem2);
			}
		}

		private static bool HasPageBreakAtStart(ReportItem reportItem)
		{
			if (!(reportItem is DataRegion) && !(reportItem is Rectangle))
			{
				return false;
			}
			if (reportItem is List)
			{
				return ((List)reportItem).PropagatedPageBreakAtStart;
			}
			if (reportItem is Table)
			{
				return ((Table)reportItem).PropagatedPageBreakAtStart;
			}
			if (reportItem is Matrix)
			{
				return ((Matrix)reportItem).PropagatedPageBreakAtStart;
			}
			IPageBreakItem pageBreakItem = (IPageBreakItem)reportItem;
			if (pageBreakItem != null)
			{
				if (pageBreakItem.IgnorePageBreaks())
				{
					return false;
				}
				return pageBreakItem.HasPageBreaks(atStart: true);
			}
			return false;
		}

		internal static void CalculateChildrenPostions(ReportItem reportItem)
		{
			ReportItemCollection reportItemCollection = null;
			if (!(reportItem is DataRegion) && !(reportItem is Rectangle) && !(reportItem is Report))
			{
				return;
			}
			if (reportItem is Rectangle)
			{
				reportItemCollection = ((Rectangle)reportItem).ReportItems;
			}
			else if (reportItem is List)
			{
				reportItemCollection = ((List)reportItem).ReportItems;
			}
			else if (reportItem is Report)
			{
				reportItemCollection = ((Report)reportItem).ReportItems;
				if (-1 == reportItem.DistanceFromReportTop)
				{
					reportItem.DistanceFromReportTop = 0;
				}
			}
			if (reportItemCollection == null)
			{
				return;
			}
			_ = reportItem.HeightValue;
			for (int i = 0; i < reportItemCollection.Count; i++)
			{
				ReportItem reportItem2 = reportItemCollection[i];
				reportItem2.DistanceBeforeTop = (int)reportItem2.TopValue;
				_ = reportItem2.TopValue;
				_ = reportItem2.HeightValue;
				reportItem2.DistanceFromReportTop = reportItem.DistanceFromReportTop + (int)reportItem2.TopValue;
				if (reportItem2 is List)
				{
					((List)reportItem2).IsListMostInner = IsListMostInner(((List)reportItem2).ReportItems);
				}
				for (int j = 0; j < i; j++)
				{
					ReportItem reportItem3 = reportItemCollection[j];
					double num = reportItem3.TopValue + reportItem3.HeightValue;
					if (num < reportItem2.TopValue && !(reportItem2.LeftValue > reportItem3.LeftValue + reportItem3.WidthValue) && !(reportItem2.LeftValue + reportItem2.WidthValue < reportItem3.LeftValue))
					{
						reportItem2.DistanceBeforeTop = Math.Min(reportItem2.DistanceBeforeTop, (int)(reportItem2.TopValue - num));
					}
					else if (0.5 > Math.Abs(reportItemCollection[j].TopValue - reportItem2.TopValue))
					{
						reportItem2.DistanceBeforeTop = 0;
					}
				}
				CalculateChildrenPostions(reportItem2);
			}
		}

		private static bool IsListMostInner(ReportItemCollection reportItemCollection)
		{
			if (reportItemCollection == null || reportItemCollection.Count < 1)
			{
				return true;
			}
			for (int i = 0; i < reportItemCollection.Count; i++)
			{
				ReportItem reportItem = reportItemCollection[i];
				if (reportItem is DataRegion)
				{
					return false;
				}
				if (reportItem is Rectangle && ((Rectangle)reportItem).ReportItems.ComputedReportItems != null)
				{
					return false;
				}
			}
			return true;
		}
	}
}
