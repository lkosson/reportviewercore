using Microsoft.Cloud.Platform.Utils;
using Microsoft.ReportingServices.DataExtensions;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Schema;

namespace Microsoft.ReportingServices.ReportPublishing
{
	internal sealed class ReportPublishing
	{
		private bool m_static;

		private bool m_interactive;

		private int m_idCounter;

		private int m_dataSetIndexCounter;

		private RmlValidatingReader m_reader;

		private CLSUniqueNameValidator m_reportItemNames;

		private CLSUniqueNameValidator m_reportSectionNames;

		private VariableNameValidator m_variableNames;

		private ScopeNameValidator m_scopeNames;

		private string m_description;

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.SubReport> m_subReports;

		private UserLocationFlags m_reportLocationFlags = UserLocationFlags.ReportBody;

		private UserLocationFlags m_userReferenceLocation = UserLocationFlags.None;

		private bool m_hasExternalImages;

		private bool m_hasHyperlinks;

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion> m_nestedDataRegions;

		private SortedList<double, Pair<double, int>> m_headerLevelSizeList;

		private double m_firstCumulativeHeaderSize;

		private Microsoft.ReportingServices.ReportIntermediateFormat.ReportSection m_currentReportSection;

		private DataSourceInfoCollection m_dataSources;

		private DataSetInfoCollection m_sharedDataSetReferences;

		private bool m_hasGrouping;

		private bool m_hasSorting;

		private bool m_requiresSortingPostGrouping;

		private bool m_hasUserSort;

		private bool m_hasGroupFilters;

		private bool m_hasSpecialRecursiveAggregates;

		private bool m_subReportMergeTransactions;

		private ExprHostCompiler m_reportCT;

		private bool m_hasImageStreams;

		private bool m_hasLabels;

		private bool m_hasBookmarks;

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.TextBox> m_textBoxesWithUserSortTarget = new List<Microsoft.ReportingServices.ReportIntermediateFormat.TextBox>();

		private List<ICreateSubtotals> m_createSubtotalsDefs = new List<ICreateSubtotals>();

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.Grouping> m_domainScopeGroups = new List<Microsoft.ReportingServices.ReportIntermediateFormat.Grouping>();

		private Holder<int> m_variableSequenceIdCounter = new Holder<int>();

		private Holder<int> m_textboxSequenceIdCounter = new Holder<int>();

		private bool m_hasFilters;

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.DataSet> m_dataSets = new List<Microsoft.ReportingServices.ReportIntermediateFormat.DataSet>();

		private bool m_parametersNotUsedInQuery = true;

		private Hashtable m_usedInQueryInfos = new Hashtable();

		private Hashtable m_reportParamUserProfile = new Hashtable();

		private Hashtable m_dataSetQueryInfo = new Hashtable();

		private ArrayList m_dynamicParameters = new ArrayList();

		private CultureInfo m_reportLanguage;

		private bool m_hasUserSortPeerScopes;

		private Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.ISortFilterScope> m_reportScopes = new Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.ISortFilterScope>();

		private StringDictionary m_dataSourceNames = new StringDictionary();

		private int m_dataRegionCount;

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.ReportItemCollection> m_reportItemCollectionList = new List<Microsoft.ReportingServices.ReportIntermediateFormat.ReportItemCollection>();

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.IAggregateHolder> m_aggregateHolderList = new List<Microsoft.ReportingServices.ReportIntermediateFormat.IAggregateHolder>();

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.IRunningValueHolder> m_runningValueHolderList = new List<Microsoft.ReportingServices.ReportIntermediateFormat.IRunningValueHolder>();

		private Microsoft.ReportingServices.ReportIntermediateFormat.Report m_report;

		private ParametersGridLayout m_parametersLayout;

		private PublishingErrorContext m_errorContext;

		private PublishingContextBase m_publishingContext;

		private readonly ReportUpgradeStrategy m_reportUpgradeStrategy;

		private ScopeTree m_scopeTree;

		private Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes, Dictionary<string, Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo>>> m_aggregateHashByType = new Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes, Dictionary<string, Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo>>>();

		private Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes, Dictionary<string, AllowNullKeyDictionary<string, Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.RunningValueInfo>>>> m_runningValueHashByType = new Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes, Dictionary<string, AllowNullKeyDictionary<string, Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.RunningValueInfo>>>>();

		private bool m_wroteAggregateHeaderInformation;

		private DataSetCore m_dataSetCore;

		internal ReportPublishing(PublishingContextBase publishingContext, PublishingErrorContext errorContext)
		{
			m_publishingContext = publishingContext;
			m_errorContext = errorContext;
		}

		internal ReportPublishing(PublishingContextBase publishingContext, PublishingErrorContext errorContext, ReportUpgradeStrategy reportUpgradeStrategy)
		{
			m_publishingContext = publishingContext;
			m_errorContext = errorContext;
			m_reportUpgradeStrategy = reportUpgradeStrategy;
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.Report CreateProgressiveIntermediateFormat(Stream definitionStream, out string reportDescription, out ParameterInfoCollection parameters, out DataSourceInfoCollection dataSources)
		{
			DataSetInfoCollection sharedDataSetReferences = null;
			ArrayList dataSetsName = null;
			byte[] dataSetsHash = null;
			CheckForMissingDefinition(definitionStream);
			string language;
			UserLocationFlags userReferenceLocation;
			bool hasExternalImages;
			bool hasHyperlinks;
			return InternalCreateIntermediateFormat(definitionStream, out reportDescription, out language, out parameters, out dataSources, out sharedDataSetReferences, out userReferenceLocation, out dataSetsName, out hasExternalImages, out hasHyperlinks, out dataSetsHash);
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.Report CreateIntermediateFormat(byte[] definition, out string description, out string language, out ParameterInfoCollection parameters, out DataSourceInfoCollection dataSources, out DataSetInfoCollection sharedDataSetReferences, out UserLocationFlags userReferenceLocation, out ArrayList dataSetsName, out bool hasExternalImages, out bool hasHyperlinks, out byte[] dataSetsHash)
		{
			CheckForMissingDefinition(definition);
			Stream definitionStream = new MemoryStream(definition, writable: false);
			return InternalCreateIntermediateFormat(definitionStream, out description, out language, out parameters, out dataSources, out sharedDataSetReferences, out userReferenceLocation, out dataSetsName, out hasExternalImages, out hasHyperlinks, out dataSetsHash);
		}

		private void CheckForMissingDefinition(object definition)
		{
			if (definition == null)
			{
				m_errorContext.Register(ProcessingErrorCode.rsNotAReportDefinition, Severity.Error, Microsoft.ReportingServices.ReportProcessing.ObjectType.Report, null, null);
				throw new ReportPublishingException(m_errorContext.Messages, ReportProcessingFlags.YukonEngine);
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.Report InternalCreateIntermediateFormat(Stream definitionStream, out string description, out string language, out ParameterInfoCollection parameters, out DataSourceInfoCollection dataSources, out DataSetInfoCollection sharedDataSetReferences, out UserLocationFlags userReferenceLocation, out ArrayList dataSetsName, out bool hasExternalImages, out bool hasHyperlinks, out byte[] dataSetsHash)
		{
			try
			{
				m_report = null;
				ReportProcessingCompatibilityVersion.TraceCompatibilityVersion(m_publishingContext.Configuration);
				Phase1(definitionStream, out description, out language, out dataSources, out sharedDataSetReferences, out hasExternalImages, out hasHyperlinks);
				dataSetsHash = CreateHashForCachedDataSets();
				Phase2();
				Phase3(out parameters, out Dictionary<string, int> groupingExprCountAtScope);
				Phase4(groupingExprCountAtScope, out dataSetsName);
				userReferenceLocation = m_userReferenceLocation;
				if (m_errorContext.HasError)
				{
					throw new ReportPublishingException(m_errorContext.Messages, ReportProcessingFlags.YukonEngine);
				}
				return m_report;
			}
			finally
			{
				m_report = null;
				m_errorContext = null;
			}
		}

		private byte[] CreateHashForCachedDataSets()
		{
			MemoryStream memoryStream = new MemoryStream();
			try
			{
				IntermediateFormatWriter intermediateFormatWriter = new IntermediateFormatWriter(memoryStream, 0);
				List<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo>();
				list.Add(new Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(MemberName.Value, Token.Object));
				Declaration declaration = new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObject, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Null, list);
				intermediateFormatWriter.RegisterDeclaration(declaration);
				intermediateFormatWriter.NextMember();
				intermediateFormatWriter.WriteVariantOrPersistable(m_report.Language);
				intermediateFormatWriter.WriteVariantOrPersistable(m_report.Code);
				if (((IExpressionHostAssemblyHolder)m_report).CodeModules != null)
				{
					foreach (string codeModule in ((IExpressionHostAssemblyHolder)m_report).CodeModules)
					{
						intermediateFormatWriter.WriteVariantOrPersistable(codeModule);
					}
				}
				else
				{
					intermediateFormatWriter.WriteNull();
				}
				if (((IExpressionHostAssemblyHolder)m_report).CodeClasses != null)
				{
					foreach (Microsoft.ReportingServices.ReportIntermediateFormat.CodeClass codeClass in ((IExpressionHostAssemblyHolder)m_report).CodeClasses)
					{
						intermediateFormatWriter.WriteVariantOrPersistable(codeClass);
					}
				}
				else
				{
					intermediateFormatWriter.WriteNull();
				}
				if (m_report.DataSources != null)
				{
					foreach (Microsoft.ReportingServices.ReportIntermediateFormat.DataSource dataSource in m_report.DataSources)
					{
						intermediateFormatWriter.WriteVariantOrPersistable(dataSource.Name);
						intermediateFormatWriter.WriteVariantOrPersistable(dataSource.Transaction);
						intermediateFormatWriter.WriteVariantOrPersistable(dataSource.Type);
						intermediateFormatWriter.WriteVariantOrPersistable(dataSource.ConnectStringExpression);
						intermediateFormatWriter.WriteVariantOrPersistable(dataSource.IntegratedSecurity);
						intermediateFormatWriter.WriteVariantOrPersistable(dataSource.Prompt);
						intermediateFormatWriter.WriteVariantOrPersistable(dataSource.DataSourceReference);
					}
				}
				else
				{
					intermediateFormatWriter.WriteNull();
				}
				if (m_dataSets != null)
				{
					foreach (Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet in m_dataSets)
					{
						intermediateFormatWriter.WriteVariantOrPersistable(dataSet.Name);
						intermediateFormatWriter.WriteVariantOrPersistable(dataSet.Query);
						if (dataSet.Query != null)
						{
							intermediateFormatWriter.WriteVariantOrPersistable(dataSet.Query.DataSourceName);
						}
						if (dataSet.Fields != null)
						{
							foreach (Microsoft.ReportingServices.ReportIntermediateFormat.Field field in dataSet.Fields)
							{
								intermediateFormatWriter.WriteVariantOrPersistable(field);
							}
						}
						else
						{
							intermediateFormatWriter.WriteNull();
						}
					}
				}
				else
				{
					intermediateFormatWriter.WriteNull();
				}
				memoryStream.Flush();
				memoryStream.Seek(0L, SeekOrigin.Begin);
				using (HashAlgorithm hashAlgorithm = SHA1.Create())
				{
					hashAlgorithm.ComputeHash(memoryStream);
					return hashAlgorithm.Hash;
				}
			}
			finally
			{
				if (memoryStream != null)
				{
					memoryStream.Close();
					memoryStream = null;
				}
			}
		}

		private void RegisterDataRegion(Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataRegion)
		{
			m_dataRegionCount++;
			m_aggregateHolderList.Add(dataRegion);
			m_runningValueHolderList.Add(dataRegion);
		}

		private int GenerateID()
		{
			return ++m_idCounter;
		}

		private int GenerateVariableSequenceID()
		{
			return m_variableSequenceIdCounter.Value++;
		}

		private int GenerateTextboxSequenceID()
		{
			return m_textboxSequenceIdCounter.Value++;
		}

		private void Phase1(Stream definitionStream, out string description, out string language, out DataSourceInfoCollection dataSources, out DataSetInfoCollection sharedDataSetReferences, out bool hasExternalImages, out bool hasHyperlinks)
		{
			try
			{
				Global.Tracer.Assert(m_reportUpgradeStrategy != null, "There is no Upgrade Strategy for this stream.");
				Stream stream = m_reportUpgradeStrategy.Upgrade(definitionStream);
				Pair<string, Stream> pair = default(Pair<string, Stream>);
				List<Pair<string, Stream>> list = new List<Pair<string, Stream>>();
				pair = GetRDLNamespaceSchemaStreamPair("http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition", "Microsoft.ReportingServices.ReportProcessing.ReportPublishing.ReportDefinition.xsd");
				list.Add(pair);
				pair = GetRDLNamespaceSchemaStreamPair("http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition", "Microsoft.ReportingServices.ReportProcessing.ReportPublishing.ReportDefinition2010.xsd");
				list.Add(pair);
				pair = GetRDLNamespaceSchemaStreamPair("http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition", "Microsoft.ReportingServices.ReportProcessing.ReportPublishing.ReportDefinition2011.xsd");
				list.Add(pair);
				pair = GetRDLNamespaceSchemaStreamPair("http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition", "Microsoft.ReportingServices.ReportProcessing.ReportPublishing.ReportDefinition2012.xsd");
				list.Add(pair);
				pair = GetRDLNamespaceSchemaStreamPair("http://schemas.microsoft.com/sqlserver/reporting/2013/01/reportdefinition", "Microsoft.ReportingServices.ReportProcessing.ReportPublishing.ReportDefinition2013.xsd");
				list.Add(pair);
				pair = GetRDLNamespaceSchemaStreamPair("http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition/defaultfontfamily", "Microsoft.ReportingServices.ReportProcessing.ReportPublishing.DefaultFontFamily.xsd");
				list.Add(pair);
				m_reader = new RmlValidatingReader(stream, list, m_errorContext, m_publishingContext.IsRdlx ? RmlValidatingReader.ItemType.Rdlx : RmlValidatingReader.ItemType.Rdl);
				m_reportItemNames = new CLSUniqueNameValidator(ProcessingErrorCode.rsInvalidNameNotCLSCompliant, ProcessingErrorCode.rsDuplicateReportItemName, ProcessingErrorCode.rsInvalidNameLength);
				m_reportSectionNames = new CLSUniqueNameValidator(ProcessingErrorCode.rsInvalidNameNotCLSCompliant, ProcessingErrorCode.rsDuplicateReportSectionName, ProcessingErrorCode.rsInvalidNameLength);
				m_variableNames = new VariableNameValidator();
				m_scopeNames = new ScopeNameValidator();
				m_dataSources = new DataSourceInfoCollection();
				sharedDataSetReferences = null;
				m_sharedDataSetReferences = new DataSetInfoCollection();
				m_subReports = new List<Microsoft.ReportingServices.ReportIntermediateFormat.SubReport>();
				while (m_reader.Read())
				{
					if (XmlNodeType.Element == m_reader.NodeType && "Report" == m_reader.LocalName)
					{
						m_reportCT = new ExprHostCompiler(new Microsoft.ReportingServices.RdlExpressions.VBExpressionParser(m_errorContext), m_errorContext);
						ReadReport(m_publishingContext.DataProtection);
					}
				}
				if (m_report == null)
				{
					m_errorContext.Register(ProcessingErrorCode.rsNotACurrentReportDefinition, Severity.Error, Microsoft.ReportingServices.ReportProcessing.ObjectType.Report, null, "Namespace", "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition");
					throw new ReportProcessingException(m_errorContext.Messages);
				}
			}
			catch (XmlSchemaException e)
			{
				CreateInvalidReportDefinitionException(e);
			}
			catch (XmlException e2)
			{
				CreateInvalidReportDefinitionException(e2);
			}
			catch (ArgumentException e3)
			{
				CreateInvalidReportDefinitionException(e3);
			}
			catch (IndexOutOfRangeException e4)
			{
				CreateInvalidReportDefinitionException(e4);
			}
			catch (FormatException e5)
			{
				CreateInvalidReportDefinitionException(e5);
			}
			finally
			{
				if (m_reader != null)
				{
					m_reader.Close();
					m_reader = null;
				}
				description = m_description;
				language = null;
				if (m_reportLanguage != null)
				{
					language = m_reportLanguage.Name;
				}
				dataSources = m_dataSources;
				sharedDataSetReferences = m_sharedDataSetReferences;
				hasExternalImages = m_hasExternalImages;
				hasHyperlinks = m_hasHyperlinks;
				m_description = null;
				m_dataSources = null;
			}
		}

		private Pair<string, Stream> GetRDLNamespaceSchemaStreamPair(string validationNamespace, string xsdResource)
		{
			Stream stream = null;
			stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(xsdResource);
			Global.Tracer.Assert(stream != null, "(schemaStream != null)");
			return new Pair<string, Stream>(validationNamespace, stream);
		}

		private void CreateInvalidReportDefinitionException(Exception e)
		{
			m_errorContext.Register(ProcessingErrorCode.rsInvalidReportDefinition, Severity.Error, Microsoft.ReportingServices.ReportProcessing.ObjectType.Report, null, null, e.Message);
			throw new ReportProcessingException(m_errorContext.Messages);
		}

		private void ReadReport(IDataProtection dataProtection)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.Report report = new Microsoft.ReportingServices.ReportIntermediateFormat.Report(GenerateID(), GenerateID());
			report.Name = "Report";
			int maxExpressionLength = -1;
			if (m_publishingContext.IsRdlSandboxingEnabled)
			{
				maxExpressionLength = m_publishingContext.Configuration.RdlSandboxing.MaxExpressionLength;
			}
			m_report = report;
			PublishingContextStruct context = new PublishingContextStruct(LocationFlags.None, report.ObjectType, maxExpressionLength, m_errorContext);
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = null;
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
						report.AutoRefreshExpression = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
						break;
					case "DataSources":
					{
						List<Microsoft.ReportingServices.ReportIntermediateFormat.DataSource> list = ReadDataSources(context, dataProtection);
						if (report.DataSources == null)
						{
							report.DataSources = list;
						}
						else
						{
							report.DataSources.AddRange(list);
						}
						break;
					}
					case "DataSets":
						ReadDataSets(context);
						break;
					case "ReportParameters":
						report.Parameters = ReadReportParameters(context);
						break;
					case "ReportParametersLayout":
						m_parametersLayout = ReadReportParametersLayout(context, report.Parameters);
						break;
					case "CustomProperties":
						report.CustomProperties = ReadCustomProperties(context);
						break;
					case "Code":
					{
						string code = m_reader.ReadString();
						if (m_publishingContext.IsRdlSandboxingEnabled)
						{
							context.ErrorContext.Register(ProcessingErrorCode.rsSandboxingCustomCodeNotAllowed, Severity.Error, context.ObjectType, context.ObjectName, "Code");
							break;
						}
						if (m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.Report_Code))
						{
							context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, context.ObjectType, context.ObjectName, m_reader.LocalName);
							break;
						}
						report.Code = code;
						m_reportCT.Builder.SetCustomCode();
						break;
					}
					case "EmbeddedImages":
						report.EmbeddedImages = ReadEmbeddedImages(context);
						break;
					case "Language":
						expressionInfo = (report.Language = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.ReportLanguage, DataType.String, context));
						break;
					case "CodeModules":
					{
						List<string> codeModules = ReadCodeModules(context);
						if (m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.Report_CodeModules))
						{
							context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, context.ObjectType, context.ObjectName, m_reader.LocalName);
						}
						else
						{
							((IExpressionHostAssemblyHolder)report).CodeModules = codeModules;
						}
						break;
					}
					case "Classes":
					{
						List<Microsoft.ReportingServices.ReportIntermediateFormat.CodeClass> codeClasses = ReadClasses(context);
						if (m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.Report_Classes))
						{
							context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, context.ObjectType, context.ObjectName, m_reader.LocalName);
						}
						else
						{
							((IExpressionHostAssemblyHolder)report).CodeClasses = codeClasses;
						}
						break;
					}
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
					case "Variables":
						report.Variables = ReadVariables(context, isGrouping: false, null);
						break;
					case "DeferVariableEvaluation":
						report.DeferVariableEvaluation = m_reader.ReadBoolean(context.ObjectType, context.ObjectName, m_reader.LocalName);
						break;
					case "ConsumeContainerWhitespace":
						report.ConsumeContainerWhitespace = m_reader.ReadBoolean(context.ObjectType, context.ObjectName, m_reader.LocalName);
						break;
					case "ReportSections":
						report.ReportSections = ReadReportSections(context, report);
						break;
					case "InitialPageName":
					{
						report.InitialPageName = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out bool _);
						break;
					}
					case "DefaultFontFamily":
					{
						string text = m_reader.ReadString();
						if (m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.DefaultFontFamily))
						{
							context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, context.ObjectType, context.ObjectName, m_reader.LocalName);
						}
						else if (!text.IsNullOrWhiteSpace())
						{
							report.DefaultFontFamily = text;
						}
						break;
					}
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
			if (report.Parameters != null && m_parametersLayout != null)
			{
				ReportParametersGridLayoutValidator.Validate(report.Parameters, m_parametersLayout, m_errorContext);
			}
			if (expressionInfo == null)
			{
				m_reportLanguage = Localization.DefaultReportServerSpecificCulture;
			}
			else if (Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == expressionInfo.Type)
			{
				PublishingValidator.ValidateSpecificLanguage(expressionInfo, Microsoft.ReportingServices.ReportProcessing.ObjectType.Report, null, "Language", context.ErrorContext, out m_reportLanguage);
			}
			if (m_interactive)
			{
				report.ShowHideType = Microsoft.ReportingServices.ReportIntermediateFormat.Report.ShowHideTypes.Interactive;
			}
			else if (m_static)
			{
				report.ShowHideType = Microsoft.ReportingServices.ReportIntermediateFormat.Report.ShowHideTypes.Static;
			}
			else
			{
				report.ShowHideType = Microsoft.ReportingServices.ReportIntermediateFormat.Report.ShowHideTypes.None;
			}
			report.SubReports = m_subReports;
			report.LastID = m_idCounter;
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.ReportSection> ReadReportSections(PublishingContextStruct context, Microsoft.ReportingServices.ReportIntermediateFormat.Report report)
		{
			List<Microsoft.ReportingServices.ReportIntermediateFormat.ReportSection> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.ReportSection>();
			bool flag = false;
			if (!m_reader.IsEmptyElement)
			{
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName = m_reader.LocalName;
						if (localName == "ReportSection")
						{
							Microsoft.ReportingServices.ReportIntermediateFormat.ReportSection item = ReadReportSection(context, report, list.Count);
							list.Add(item);
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "ReportSections")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ReportSection ReadReportSection(PublishingContextStruct context, Microsoft.ReportingServices.ReportIntermediateFormat.Report report, int sectionIndex)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ReportSection reportSection = new Microsoft.ReportingServices.ReportIntermediateFormat.ReportSection(sectionIndex, report, GenerateID(), GenerateID());
			context.ObjectType = Microsoft.ReportingServices.ReportProcessing.ObjectType.ReportSection;
			string attributeLocalName = m_reader.GetAttributeLocalName("Name");
			if (attributeLocalName != null)
			{
				if (m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.ReportSectionName))
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlAttribute, Severity.Error, context.ObjectType, attributeLocalName, "Name");
				}
				reportSection.Name = attributeLocalName;
			}
			else
			{
				reportSection.Name = "ReportSection" + sectionIndex.ToString(CultureInfo.InvariantCulture);
			}
			context.ObjectName = reportSection.Name;
			m_reportSectionNames.Validate(context.ObjectType, context.ObjectName, context.ErrorContext);
			m_reportItemCollectionList.Add(reportSection.ReportItems);
			m_currentReportSection = reportSection;
			bool flag = false;
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
						case "Body":
							ReadBody(reportSection, context);
							break;
						case "Width":
							reportSection.Width = ReadSize();
							break;
						case "Page":
							reportSection.Page = ReadPage(context, reportSection, sectionIndex);
							break;
						case "DataElementName":
							reportSection.DataElementName = m_reader.ReadString();
							break;
						case "DataElementOutput":
							reportSection.DataElementOutput = ReadDataElementOutput();
							break;
						case "LayoutDirection":
							reportSection.LayoutDirection = ReadLayoutDirection();
							if (reportSection.LayoutDirection && m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.ReportSection_LayoutDirection))
							{
								context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, context.ObjectType, attributeLocalName, "LayoutDirection");
							}
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "ReportSection")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return reportSection;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.Page ReadPage(PublishingContextStruct context, Microsoft.ReportingServices.ReportIntermediateFormat.ReportSection section, int sectionNum)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.Page page = new Microsoft.ReportingServices.ReportIntermediateFormat.Page(GenerateID());
			m_aggregateHolderList.Add(page);
			bool flag = false;
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
						case "PageHeader":
							page.PageHeader = ReadPageSection(isHeader: true, section, context);
							if (m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.PageHeaderFooter))
							{
								page.PageHeader = null;
								context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, context.ObjectType, context.ObjectName, "PageHeader");
							}
							break;
						case "PageFooter":
							page.PageFooter = ReadPageSection(isHeader: false, section, context);
							if (m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.PageHeaderFooter))
							{
								page.PageFooter = null;
								context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, context.ObjectType, context.ObjectName, "PageFooter");
							}
							break;
						case "PageHeight":
						{
							string pageHeight = ReadSize();
							if (sectionNum == 0)
							{
								page.PageHeight = pageHeight;
							}
							else
							{
								context.ErrorContext.Register(ProcessingErrorCode.rsPagePropertyInSubsequentReportSection, Severity.Warning, context.ObjectType, context.ObjectName, "PageHeight");
							}
							break;
						}
						case "PageWidth":
						{
							string pageWidth = ReadSize();
							if (sectionNum == 0)
							{
								page.PageWidth = pageWidth;
							}
							else
							{
								context.ErrorContext.Register(ProcessingErrorCode.rsPagePropertyInSubsequentReportSection, Severity.Warning, context.ObjectType, context.ObjectName, "PageWidth");
							}
							break;
						}
						case "InteractiveHeight":
						{
							string interactiveHeight = ReadSize();
							if (sectionNum == 0)
							{
								page.InteractiveHeight = interactiveHeight;
							}
							else
							{
								context.ErrorContext.Register(ProcessingErrorCode.rsPagePropertyInSubsequentReportSection, Severity.Warning, context.ObjectType, context.ObjectName, "InteractiveHeight");
							}
							break;
						}
						case "InteractiveWidth":
						{
							string interactiveWidth = ReadSize();
							if (sectionNum == 0)
							{
								page.InteractiveWidth = interactiveWidth;
							}
							else
							{
								context.ErrorContext.Register(ProcessingErrorCode.rsPagePropertyInSubsequentReportSection, Severity.Warning, context.ObjectType, context.ObjectName, "InteractiveWidth");
							}
							break;
						}
						case "LeftMargin":
						{
							string leftMargin = ReadSize();
							if (sectionNum == 0)
							{
								page.LeftMargin = leftMargin;
							}
							else
							{
								context.ErrorContext.Register(ProcessingErrorCode.rsPagePropertyInSubsequentReportSection, Severity.Warning, context.ObjectType, context.ObjectName, "LeftMargin");
							}
							break;
						}
						case "RightMargin":
						{
							string rightMargin = ReadSize();
							if (sectionNum == 0)
							{
								page.RightMargin = rightMargin;
							}
							else
							{
								context.ErrorContext.Register(ProcessingErrorCode.rsPagePropertyInSubsequentReportSection, Severity.Warning, context.ObjectType, context.ObjectName, "RightMargin");
							}
							break;
						}
						case "TopMargin":
						{
							string topMargin = ReadSize();
							if (sectionNum == 0)
							{
								page.TopMargin = topMargin;
							}
							else
							{
								context.ErrorContext.Register(ProcessingErrorCode.rsPagePropertyInSubsequentReportSection, Severity.Warning, context.ObjectType, context.ObjectName, "TopMargin");
							}
							break;
						}
						case "BottomMargin":
						{
							string bottomMargin = ReadSize();
							if (sectionNum == 0)
							{
								page.BottomMargin = bottomMargin;
							}
							else
							{
								context.ErrorContext.Register(ProcessingErrorCode.rsPagePropertyInSubsequentReportSection, Severity.Warning, context.ObjectType, context.ObjectName, "BottomMargin");
							}
							break;
						}
						case "Columns":
						{
							int columns = m_reader.ReadInteger(context.ObjectType, context.ObjectName, m_reader.LocalName);
							if (PublishingValidator.ValidateColumns(columns, context.ObjectType, context.ObjectName, "Columns", context.ErrorContext, sectionNum))
							{
								page.Columns = columns;
							}
							break;
						}
						case "ColumnSpacing":
							page.ColumnSpacing = ReadSize();
							break;
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Body, hasNoRows: false);
							if (sectionNum == 0)
							{
								page.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, context.ErrorContext);
							}
							else if (styleInformation.Attributes.Count > 0)
							{
								context.ErrorContext.Register(ProcessingErrorCode.rsPagePropertyInSubsequentReportSection, Severity.Warning, context.ObjectType, context.ObjectName, "Style");
							}
							break;
						}
						}
						break;
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "Page")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return page;
		}

		private List<string> ReadCodeModules(PublishingContextStruct context)
		{
			List<string> list = new List<string>();
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
						list.Add(m_reader.ReadString());
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
			return list;
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.CodeClass> ReadClasses(PublishingContextStruct context)
		{
			List<Microsoft.ReportingServices.ReportIntermediateFormat.CodeClass> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.CodeClass>();
			CLSUniqueNameValidator instanceNameValidator = new CLSUniqueNameValidator(ProcessingErrorCode.rsInvalidNameNotCLSCompliant, ProcessingErrorCode.rsDuplicateClassInstanceName, ProcessingErrorCode.rsInvalidNameLength);
			context.ObjectType = Microsoft.ReportingServices.ReportProcessing.ObjectType.CodeClass;
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
						ReadClass(list, instanceNameValidator, context);
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
			return list;
		}

		private void ReadClass(List<Microsoft.ReportingServices.ReportIntermediateFormat.CodeClass> codeClasses, CLSUniqueNameValidator instanceNameValidator, PublishingContextStruct context)
		{
			bool flag = false;
			Microsoft.ReportingServices.ReportIntermediateFormat.CodeClass item = default(Microsoft.ReportingServices.ReportIntermediateFormat.CodeClass);
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
							item.InstanceName = m_reader.ReadString();
							if (!instanceNameValidator.Validate(context.ObjectType, item.InstanceName, context.ErrorContext))
							{
								item.InstanceName = null;
							}
						}
					}
					else
					{
						item.ClassName = m_reader.ReadString();
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
			codeClasses.Add(item);
		}

		private void ReadBody(Microsoft.ReportingServices.ReportIntermediateFormat.ReportSection section, PublishingContextStruct context)
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
						ReadReportItems(null, section, section.ReportItems, context, null);
						break;
					case "Height":
						section.Height = ReadSize();
						break;
					case "Style":
					{
						StyleInformation styleInformation = ReadStyle(context);
						styleInformation.Filter(StyleOwnerType.Body, hasNoRows: false);
						section.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, context.ErrorContext);
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

		private Microsoft.ReportingServices.ReportIntermediateFormat.PageSection ReadPageSection(bool isHeader, Microsoft.ReportingServices.ReportIntermediateFormat.ReportSection section, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.PageSection pageSection = new Microsoft.ReportingServices.ReportIntermediateFormat.PageSection(isHeader, GenerateID(), GenerateID(), section);
			pageSection.Name = section.Name + "." + (isHeader ? "PageHeader" : "PageFooter");
			context.Location |= LocationFlags.InPageSection;
			context.ObjectType = pageSection.ObjectType;
			context.ObjectName = pageSection.Name;
			m_report.HasHeadersOrFooters = true;
			m_reportItemCollectionList.Add(pageSection.ReportItems);
			m_reportLocationFlags = UserLocationFlags.ReportPageSection;
			m_reportCT.ResetPageSectionRefersFlags();
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
						pageSection.PrintOnFirstPage = m_reader.ReadBoolean(context.ObjectType, context.ObjectName, m_reader.LocalName);
						break;
					case "PrintOnLastPage":
						pageSection.PrintOnLastPage = m_reader.ReadBoolean(context.ObjectType, context.ObjectName, m_reader.LocalName);
						break;
					case "PrintBetweenSections":
						pageSection.PrintBetweenSections = m_reader.ReadBoolean(context.ObjectType, context.ObjectName, m_reader.LocalName);
						break;
					case "ReportItems":
						ReadReportItems(null, pageSection, pageSection.ReportItems, context, null, out computed);
						break;
					case "Style":
					{
						StyleInformation styleInformation = ReadStyle(context, out computed2);
						styleInformation.Filter(StyleOwnerType.PageSection, hasNoRows: false);
						pageSection.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, context.ErrorContext);
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
			section.NeedsReportItemsOnPage |= m_reportCT.PageSectionRefersToReportItems;
			section.NeedsOverallTotalPages |= m_reportCT.PageSectionRefersToOverallTotalPages;
			section.NeedsPageBreakTotalPages |= m_reportCT.PageSectionRefersToTotalPages;
			m_reportLocationFlags = UserLocationFlags.ReportBody;
			return pageSection;
		}

		private void ReadReportItems(string propertyName, Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem parent, Microsoft.ReportingServices.ReportIntermediateFormat.ReportItemCollection parentCollection, PublishingContextStruct context, List<Microsoft.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget, out bool computed)
		{
			computed = false;
			int num = 0;
			bool isParentTablix = parent is Microsoft.ReportingServices.ReportIntermediateFormat.Tablix;
			bool flag = false;
			do
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem reportItem = null;
				Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem altReportItem = null;
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (m_reader.LocalName)
					{
					case "Tablix":
						num++;
						reportItem = ReadTablix(parent, context);
						break;
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
						reportItem = ReadCustomReportItem(parent, context, textBoxesWithDefaultSortTarget, out altReportItem);
						Global.Tracer.Assert(altReportItem != null);
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
					case "Chart":
						num++;
						reportItem = ReadChart(parent, context);
						break;
					case "GaugePanel":
						num++;
						reportItem = ReadGaugePanel(parent, context);
						break;
					case "Map":
						num++;
						reportItem = ReadMap(parent, context);
						break;
					}
					if (reportItem != null)
					{
						computed |= AddReportItemToParentCollection(reportItem, parentCollection, isParentTablix);
						if (altReportItem != null)
						{
							computed |= AddReportItemToParentCollection(altReportItem, parentCollection, isParentTablix);
						}
					}
					break;
				case XmlNodeType.EndElement:
					if ("ReportItems" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
		}

		private bool AddReportItemToParentCollection(Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem reportItem, Microsoft.ReportingServices.ReportIntermediateFormat.ReportItemCollection parentCollection, bool isParentTablix)
		{
			parentCollection.AddReportItem(reportItem);
			return reportItem.Computed;
		}

		private void ReadReportItems(string propertyName, Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem parent, Microsoft.ReportingServices.ReportIntermediateFormat.ReportItemCollection parentCollection, PublishingContextStruct context, List<Microsoft.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget)
		{
			ReadReportItems(propertyName, parent, parentCollection, context, textBoxesWithDefaultSortTarget, out bool _);
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo ReadPageNameExpression(PublishingContextStruct context)
		{
			bool computed;
			return ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out computed);
		}

		private void ReadPageBreak(IPageBreakOwner pageBreakOwner, PublishingContextStruct context)
		{
			bool flag = false;
			if (m_reader.IsEmptyElement)
			{
				return;
			}
			Microsoft.ReportingServices.ReportIntermediateFormat.PageBreak pageBreak2 = pageBreakOwner.PageBreak = new Microsoft.ReportingServices.ReportIntermediateFormat.PageBreak();
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					bool computed;
					switch (m_reader.LocalName)
					{
					case "BreakLocation":
						pageBreak2.BreakLocation = ReadPageBreakLocation();
						break;
					case "ResetPageNumber":
						pageBreak2.ResetPageNumber = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context, out computed);
						break;
					case "Disabled":
						pageBreak2.Disabled = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context, out computed);
						break;
					}
					break;
				}
				case XmlNodeType.EndElement:
					flag = (m_reader.LocalName == "PageBreak");
					break;
				}
			}
			while (!flag);
		}

		private void SetSortTargetForTextBoxes(List<Microsoft.ReportingServices.ReportIntermediateFormat.TextBox> textBoxes, Microsoft.ReportingServices.ReportIntermediateFormat.ISortFilterScope target)
		{
			if (textBoxes != null)
			{
				for (int i = 0; i < textBoxes.Count; i++)
				{
					textBoxes[i].UserSort.SetDefaultSortTarget(target);
				}
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.SubReport ReadSubreport(Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem parent, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.SubReport subReport = new Microsoft.ReportingServices.ReportIntermediateFormat.SubReport(GenerateID(), parent);
			subReport.Name = m_reader.GetAttribute("Name");
			subReport.SetContainingSection(m_currentReportSection);
			context.ObjectType = subReport.ObjectType;
			context.ObjectName = subReport.Name;
			m_reportItemNames.Validate(context.ObjectType, context.ObjectName, context.ErrorContext);
			bool flag = true;
			if ((context.Location & LocationFlags.InPageSection) != 0)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsDataRegionInPageSection, Severity.Error, context.ObjectType, context.ObjectName, null);
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
						subReport.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, context.ErrorContext);
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
						subReport.ZIndex = m_reader.ReadInteger(context.ObjectType, context.ObjectName, m_reader.LocalName);
						break;
					case "Visibility":
						subReport.Visibility = ReadVisibility(context);
						break;
					case "ToolTip":
						subReport.ToolTip = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
						break;
					case "DocumentMapLabel":
						subReport.DocumentMapLabel = ReadDocumentMapLabelExpression(m_reader.LocalName, context);
						break;
					case "Bookmark":
						subReport.Bookmark = ReadBookmarkExpression(m_reader.LocalName, context);
						break;
					case "CustomProperties":
						subReport.CustomProperties = ReadCustomProperties(context);
						break;
					case "ReportName":
						subReport.ReportName = PublishingValidator.ValidateReportName(m_publishingContext.CatalogContext, m_reader.ReadString(), context.ObjectType, context.ObjectName, "ReportName", context.ErrorContext);
						break;
					case "Parameters":
						subReport.Parameters = ReadParameters(context, doClsValidation: true);
						break;
					case "NoRowsMessage":
						subReport.NoRowsMessage = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
						break;
					case "MergeTransactions":
						subReport.MergeTransactions = m_reader.ReadBoolean(context.ObjectType, context.ObjectName, m_reader.LocalName);
						if (subReport.MergeTransactions)
						{
							m_subReportMergeTransactions = true;
						}
						break;
					case "DataElementName":
						subReport.DataElementName = m_reader.ReadString();
						break;
					case "DataElementOutput":
						subReport.DataElementOutput = ReadDataElementOutput();
						break;
					case "KeepTogether":
						subReport.KeepTogether = m_reader.ReadBoolean(context.ObjectType, context.ObjectName, m_reader.LocalName);
						break;
					case "OmitBorderOnPageBreak":
						subReport.OmitBorderOnPageBreak = m_reader.ReadBoolean(context.ObjectType, context.ObjectName, m_reader.LocalName);
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
			if (m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.SubReports))
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, context.ObjectType, context.ObjectName, "Subreport");
			}
			subReport.Computed = true;
			if (flag)
			{
				m_subReports.Add(subReport);
				m_parametersNotUsedInQuery = false;
				return subReport;
			}
			return null;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.DataValueList ReadCustomProperties(PublishingContextStruct context)
		{
			bool computed;
			return ReadCustomProperties(context, out computed);
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.DataValueList ReadCustomProperties(PublishingContextStruct context, out bool computed)
		{
			bool flag = false;
			computed = false;
			int num = 0;
			Microsoft.ReportingServices.ReportIntermediateFormat.DataValueList dataValueList = new Microsoft.ReportingServices.ReportIntermediateFormat.DataValueList();
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
						dataValueList.Add(ReadDataValue(isCustomProperty: true, nameRequired: true, ++num, ref computed, context));
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

		private DataType ReadDataTypeAttribute()
		{
			bool hadExplicitDataType;
			return ReadDataTypeAttribute(out hadExplicitDataType);
		}

		private DataType ReadDataTypeAttribute(out bool hadExplicitDataType)
		{
			if (m_reader.HasAttributes)
			{
				string attribute = m_reader.GetAttribute("DataType");
				if (attribute != null)
				{
					hadExplicitDataType = true;
					return (DataType)Enum.Parse(typeof(DataType), attribute, ignoreCase: false);
				}
			}
			hadExplicitDataType = false;
			return DataType.String;
		}

		private PageBreakLocation ReadPageBreakLocation()
		{
			string value = m_reader.ReadString();
			return (PageBreakLocation)Enum.Parse(typeof(PageBreakLocation), value, ignoreCase: false);
		}

		private bool ReadDataElementStyle()
		{
			return Validator.CompareWithInvariantCulture(m_reader.ReadString(), "Attribute");
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem.DataElementStyles ReadDataElementStyleRDL()
		{
			string value = m_reader.ReadString();
			return (Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem.DataElementStyles)Enum.Parse(typeof(Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem.DataElementStyles), value, ignoreCase: false);
		}

		private DataElementOutputTypes ReadDataElementOutput()
		{
			string value = m_reader.ReadString();
			return (DataElementOutputTypes)Enum.Parse(typeof(DataElementOutputTypes), value, ignoreCase: false);
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.Action ReadActionInfo(PublishingContextStruct context, StyleOwnerType styleOwnerType, out bool computed)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.Action action = new Microsoft.ReportingServices.ReportIntermediateFormat.Action();
			bool computed2 = false;
			bool computed3 = false;
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
							if (localName == "Actions")
							{
								ReadActionItemList(action, context, out computed3);
								if (action.ActionItems.Count > 1)
								{
									context.ErrorContext.Register(ProcessingErrorCode.rsInvalidActionsCount, Severity.Error, context.ObjectType, context.ObjectName, "ActionInfo", "Action");
								}
							}
						}
						else
						{
							StyleInformation styleInformation = ReadStyle(context, out computed2);
							styleInformation.Filter(styleOwnerType, hasNoRows: false);
							action.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, context.ErrorContext);
						}
						break;
					}
					case XmlNodeType.EndElement:
						if ("ActionInfo" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			computed = (computed2 || computed3);
			return action;
		}

		private void ReadActionItemList(Microsoft.ReportingServices.ReportIntermediateFormat.Action actionInfo, PublishingContextStruct context, out bool computed)
		{
			computed = false;
			int computedIndex = -1;
			bool flag = false;
			bool computed2 = false;
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
						actionInfo.ActionItems.Add(ReadActionItem(context, out computed2, ref computedIndex, ref missingLabel, out bool hasDrillthroughParameter));
						actionInfo.TrackFieldsUsedInValueExpression |= hasDrillthroughParameter;
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
			computedIndex++;
			computed = (computedIndex > 0);
			if (missingLabel && actionInfo.ActionItems.Count > 1)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidActionLabel, Severity.Error, context.ObjectType, context.ObjectName, "Actions");
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ActionItem ReadActionItem(PublishingContextStruct context, out bool computed, ref int computedIndex, ref bool missingLabel, out bool hasDrillthroughParameter)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ActionItem actionItem = new Microsoft.ReportingServices.ReportIntermediateFormat.ActionItem();
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool computed2 = false;
			bool computed3 = false;
			bool computed4 = false;
			bool computed5 = false;
			hasDrillthroughParameter = false;
			context.PrefixPropertyName = "ActionInfo.Action.";
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
							actionItem.HyperLinkURL = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out computed2);
							break;
						case "Drillthrough":
							flag2 = true;
							ReadDrillthrough(context, actionItem, out computed3);
							break;
						case "BookmarkLink":
							flag3 = true;
							actionItem.BookmarkLink = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out computed4);
							break;
						case "Label":
							flag4 = true;
							actionItem.Label = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out computed5);
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
				hasDrillthroughParameter = (actionItem.DrillthroughParameters != null && actionItem.DrillthroughParameters.Count > 0);
			}
			if (flag3)
			{
				num++;
			}
			if (1 != num)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidAction, Severity.Error, context.ObjectType, context.ObjectName, "Action");
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

		private void ReadDrillthrough(PublishingContextStruct context, Microsoft.ReportingServices.ReportIntermediateFormat.ActionItem actionItem, out bool computed)
		{
			computed = false;
			bool computed2 = false;
			bool computed3 = false;
			bool computed4 = false;
			bool flag = false;
			context.PrefixPropertyName = "ActionInfo.Action.Drillthrough.";
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (m_reader.LocalName)
					{
					case "ReportName":
						actionItem.DrillthroughReportName = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out computed4);
						if (Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == actionItem.DrillthroughReportName.Type)
						{
							actionItem.DrillthroughReportName.StringValue = PublishingValidator.ValidateReportName(m_publishingContext.CatalogContext, actionItem.DrillthroughReportName.StringValue, context.ObjectType, context.ObjectName, "DrillthroughReportName", context.ErrorContext);
						}
						break;
					case "Parameters":
						actionItem.DrillthroughParameters = ReadParameters(context, omitAllowed: true, doClsValidation: false, isSubreportParameter: false, out computed2);
						break;
					case "BookmarkLink":
						actionItem.DrillthroughBookmarkLink = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out computed3);
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

		private Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo ReadBookmarkExpression(PublishingContextStruct context, out bool computedBookmark)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo result = ReadBookmarkExpression(m_reader.LocalName, context, out computedBookmark);
			if ((context.Location & LocationFlags.InPageSection) != 0)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsBookmarkInPageSection, Severity.Warning, context.ObjectType, context.ObjectName, null);
			}
			return result;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.BandLayoutOptions ReadBandLayoutOptions(Microsoft.ReportingServices.ReportIntermediateFormat.Tablix tablix, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.BandLayoutOptions bandLayoutOptions = new Microsoft.ReportingServices.ReportIntermediateFormat.BandLayoutOptions();
			int num = 0;
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
						case "RowCount":
							bandLayoutOptions.RowCount = m_reader.ReadInteger(context.ObjectType, context.ObjectName, m_reader.LocalName);
							break;
						case "ColumnCount":
							bandLayoutOptions.ColumnCount = m_reader.ReadInteger(context.ObjectType, context.ObjectName, m_reader.LocalName);
							break;
						case "Coverflow":
							num++;
							if (bandLayoutOptions.Navigation == null)
							{
								bandLayoutOptions.Navigation = ReadCoverflow(tablix, context);
							}
							break;
						case "Tabstrip":
							num++;
							if (bandLayoutOptions.Navigation == null)
							{
								bandLayoutOptions.Navigation = ReadTabstrip(tablix, context);
							}
							break;
						case "PlayAxis":
							num++;
							if (bandLayoutOptions.Navigation == null)
							{
								bandLayoutOptions.Navigation = ReadPlayAxis(context);
							}
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "BandLayoutOptions")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			if (num > 1)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidBandNavigations, Severity.Error, context.ObjectType, tablix.Name, null);
			}
			return bandLayoutOptions;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.NavigationItem ReadNavigationItem(Microsoft.ReportingServices.ReportIntermediateFormat.Tablix tablix, PublishingContextStruct context, string navigationType)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.NavigationItem navigationItem = new Microsoft.ReportingServices.ReportIntermediateFormat.NavigationItem();
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
						if (!(localName == "ReportItemReference"))
						{
							if (localName == "ReportItem")
							{
								List<Microsoft.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget = new List<Microsoft.ReportingServices.ReportIntermediateFormat.TextBox>();
								navigationItem.BandNavigationCell = new BandNavigationCell(GenerateID(), tablix);
								navigationItem.BandNavigationCell.CellContents = ReadCellContents(tablix, context, textBoxesWithDefaultSortTarget, readRowColSpans: false, out Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem altCellContents, out int? _, out int? _);
								navigationItem.BandNavigationCell.AltCellContents = altCellContents;
							}
						}
						else
						{
							navigationItem.ReportItemReference = m_reader.ReadString();
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "NavigationItem")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
				if (navigationItem.ReportItemReference != null && navigationItem.BandNavigationCell != null)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidBandNavigationItem, Severity.Error, context.ObjectType, tablix.Name, navigationType);
				}
			}
			return navigationItem;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.Coverflow ReadCoverflow(Microsoft.ReportingServices.ReportIntermediateFormat.Tablix tablix, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.Coverflow coverflow = new Microsoft.ReportingServices.ReportIntermediateFormat.Coverflow();
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
						if (!(localName == "NavigationItem"))
						{
							if (localName == "Slider")
							{
								coverflow.Slider = ReadSlider(context);
							}
						}
						else
						{
							coverflow.NavigationItem = ReadNavigationItem(tablix, context, "Coverflow");
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "Coverflow")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return coverflow;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.Tabstrip ReadTabstrip(Microsoft.ReportingServices.ReportIntermediateFormat.Tablix tablix, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.Tabstrip tabstrip = new Microsoft.ReportingServices.ReportIntermediateFormat.Tabstrip();
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
						if (!(localName == "NavigationItem"))
						{
							if (localName == "Slider")
							{
								tabstrip.Slider = ReadSlider(context);
							}
						}
						else
						{
							tabstrip.NavigationItem = ReadNavigationItem(tablix, context, "Tabstrip");
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "Tabstrip")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return tabstrip;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.PlayAxis ReadPlayAxis(PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.PlayAxis playAxis = new Microsoft.ReportingServices.ReportIntermediateFormat.PlayAxis();
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
						if (!(localName == "Slider"))
						{
							if (localName == "DockingOption")
							{
								playAxis.DockingOption = ReadDockingOption();
							}
						}
						else
						{
							playAxis.Slider = ReadSlider(context);
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "PlayAxis")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return playAxis;
		}

		private DockingOption ReadDockingOption()
		{
			string value = m_reader.ReadString();
			return (DockingOption)Enum.Parse(typeof(DockingOption), value, ignoreCase: false);
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.Slider ReadSlider(PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.Slider slider = new Microsoft.ReportingServices.ReportIntermediateFormat.Slider();
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
						if (!(localName == "Hidden"))
						{
							if (localName == "LabelData")
							{
								slider.LabelData = ReadLabelData(context);
							}
						}
						else
						{
							slider.Hidden = m_reader.ReadBoolean(context.ObjectType, context.ObjectName, m_reader.LocalName);
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "Slider")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return slider;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.LabelData ReadLabelData(PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.LabelData labelData = new Microsoft.ReportingServices.ReportIntermediateFormat.LabelData();
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
						case "DataSetName":
							labelData.DataSetName = m_reader.ReadString();
							break;
						case "Key":
						{
							string item = m_reader.ReadString();
							if (labelData.KeyFields == null)
							{
								labelData.KeyFields = new List<string>(1);
								labelData.KeyFields.Add(item);
							}
							break;
						}
						case "KeyFields":
							labelData.KeyFields = ReadKeyFields();
							if (m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.LabelData_KeyFields))
							{
								context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, context.ObjectType, context.ObjectName, "KeyFields");
							}
							break;
						case "Label":
							labelData.Label = m_reader.ReadString();
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "LabelData")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			if (labelData.KeyFields == null)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidReportDefinition, Severity.Error, context.ObjectType, context.ObjectName, "LabelData", "KeyFields");
			}
			return labelData;
		}

		private List<string> ReadKeyFields()
		{
			List<string> list = new List<string>();
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
						if (localName == "Key")
						{
							list.Add(m_reader.ReadString());
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "KeyFields")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.Chart ReadChart(Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem parent, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart = new Microsoft.ReportingServices.ReportIntermediateFormat.Chart(GenerateID(), parent);
			chart.Name = m_reader.GetAttribute("Name");
			if ((context.Location & LocationFlags.InDataRegion) != 0)
			{
				Global.Tracer.Assert(m_nestedDataRegions != null, "(m_nestedDataRegions != null)");
				m_nestedDataRegions.Add(chart);
			}
			context.Location = (context.Location | LocationFlags.InDataSet | LocationFlags.InDataRegion);
			context.ObjectType = chart.ObjectType;
			context.ObjectName = chart.Name;
			RegisterDataRegion(chart);
			bool flag = true;
			if (!m_reportItemNames.Validate(context.ObjectType, context.ObjectName, context.ErrorContext))
			{
				flag = false;
			}
			if (m_scopeNames.Validate(isGrouping: false, context.ObjectName, context.ObjectType, context.ObjectName, context.ErrorContext))
			{
				m_reportScopes.Add(chart.Name, chart);
			}
			else
			{
				flag = false;
			}
			if ((context.Location & LocationFlags.InPageSection) != 0)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsDataRegionInPageSection, Severity.Error, context.ObjectType, context.ObjectName, null);
				flag = false;
			}
			StyleInformation styleInformation = null;
			IdcRelationship relationship = null;
			if (!m_reader.IsEmptyElement)
			{
				bool flag2 = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (m_reader.LocalName)
						{
						case "SortExpressions":
							chart.Sorting = ReadSortExpressions(isDataRowSortExpression: true, context);
							break;
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
							chart.ZIndex = m_reader.ReadInteger(context.ObjectType, context.ObjectName, m_reader.LocalName);
							break;
						case "Visibility":
							chart.Visibility = ReadVisibility(context);
							break;
						case "ToolTip":
							chart.ToolTip = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "DocumentMapLabel":
							chart.DocumentMapLabel = ReadDocumentMapLabelExpression(m_reader.LocalName, context);
							break;
						case "Bookmark":
							chart.Bookmark = ReadBookmarkExpression(m_reader.LocalName, context);
							break;
						case "CustomProperties":
							chart.CustomProperties = ReadCustomProperties(context);
							break;
						case "DataElementName":
							chart.DataElementName = m_reader.ReadString();
							break;
						case "DataElementOutput":
							chart.DataElementOutput = ReadDataElementOutput();
							break;
						case "NoRowsMessage":
							chart.NoRowsMessage = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "DataSetName":
							chart.DataSetName = m_reader.ReadString();
							break;
						case "Relationship":
							relationship = ReadRelationship(context);
							break;
						case "PageBreak":
							ReadPageBreak(chart, context);
							break;
						case "PageName":
							chart.PageName = ReadPageNameExpression(context);
							break;
						case "Filters":
							chart.Filters = ReadFilters(Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.DataRegionFilters, context);
							break;
						case "ChartSeriesHierarchy":
							chart.SeriesMembers = ReadChartHierarchy(chart, context, isCategoryHierarchy: false);
							break;
						case "ChartCategoryHierarchy":
							chart.CategoryMembers = ReadChartHierarchy(chart, context, isCategoryHierarchy: true);
							break;
						case "ChartData":
						{
							ReadChartData(chart, context, out bool hasAggregates);
							chart.HasDataValueAggregates = hasAggregates;
							break;
						}
						case "ChartAreas":
							chart.ChartAreas = ReadChartAreas(chart, context);
							break;
						case "ChartLegends":
							chart.Legends = ReadChartLegends(chart, context);
							break;
						case "ChartTitles":
							chart.Titles = ReadChartTitles(chart, context);
							break;
						case "Palette":
							chart.Palette = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chart.Palette.IsExpression)
							{
								Validator.ValidatePalette(chart.Palette.StringValue, context.ErrorContext, context, m_reader.LocalName);
							}
							break;
						case "PaletteHatchBehavior":
							chart.PaletteHatchBehavior = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chart.PaletteHatchBehavior.IsExpression)
							{
								Validator.ValidatePaletteHatchBehavior(chart.PaletteHatchBehavior.StringValue, context.ErrorContext, context, m_reader.LocalName);
							}
							break;
						case "ChartCodeParameters":
							chart.CodeParameters = ReadChartCodeParameters(context);
							break;
						case "ChartCustomPaletteColors":
							chart.CustomPaletteColors = ReadChartCustomPaletteColors(chart, context);
							break;
						case "ChartBorderSkin":
							chart.BorderSkin = ReadChartBorderSkin(chart, context);
							break;
						case "ChartNoDataMessage":
						{
							ChartNoDataMessage chartNoDataMessage = new ChartNoDataMessage(chart);
							ReadChartTitle(chart, chartNoDataMessage, isNoDataMessage: true, context, new DynamicImageObjectUniqueNameValidator());
							chart.NoDataMessage = chartNoDataMessage;
							break;
						}
						case "DynamicHeight":
							chart.DynamicHeight = ReadExpression("DynamicHeight", Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "DynamicWidth":
							chart.DynamicWidth = ReadExpression("DynamicWidth", Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("Chart" == m_reader.LocalName)
						{
							flag2 = true;
						}
						break;
					}
				}
				while (!flag2);
			}
			chart.SetColumnGroupingDirection(m_publishingContext.IsRdlx);
			chart.DataScopeInfo.SetRelationship(chart.DataSetName, relationship);
			if (styleInformation != null)
			{
				styleInformation.Filter(StyleOwnerType.Chart, chart.NoRowsMessage != null);
				chart.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: false, context.ErrorContext);
			}
			if (chart.CategoryMembers == null || chart.CategoryMembers.Count == 0)
			{
				ChartFakeStaticCategory(chart);
			}
			if (chart.SeriesMembers == null || chart.SeriesMembers.Count == 0)
			{
				ChartFakeStaticSeries(chart);
			}
			if (chart.StyleClass != null)
			{
				PublishingValidator.ValidateBorderColorNotTransparent(chart.ObjectType, chart.Name, chart.StyleClass, "BorderColor", context.ErrorContext);
				PublishingValidator.ValidateBorderColorNotTransparent(chart.ObjectType, chart.Name, chart.StyleClass, "BorderColorBottom", context.ErrorContext);
				PublishingValidator.ValidateBorderColorNotTransparent(chart.ObjectType, chart.Name, chart.StyleClass, "BorderColorTop", context.ErrorContext);
				PublishingValidator.ValidateBorderColorNotTransparent(chart.ObjectType, chart.Name, chart.StyleClass, "BorderColorLeft", context.ErrorContext);
				PublishingValidator.ValidateBorderColorNotTransparent(chart.ObjectType, chart.Name, chart.StyleClass, "BorderColorRight", context.ErrorContext);
			}
			chart.Computed = true;
			if (flag)
			{
				m_hasImageStreams = true;
				return chart;
			}
			return null;
		}

		private ChartMemberList ReadChartHierarchy(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context, bool isCategoryHierarchy)
		{
			ChartMemberList chartMemberList = null;
			int leafNodes = 0;
			int maxLevel = 0;
			bool flag = false;
			if (!m_reader.IsEmptyElement)
			{
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName = m_reader.LocalName;
						if (!(localName == "ChartMembers"))
						{
							if (localName == "EnableDrilldown" && isCategoryHierarchy)
							{
								bool flag2 = m_reader.ReadBoolean(context.ObjectType, context.ObjectName, m_reader.LocalName);
								if (flag2 && m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.ChartHierarchy_EnableDrilldown))
								{
									context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, context.ObjectType, chart.Name, "EnableDrilldown");
								}
								chart.EnableCategoryDrilldown = flag2;
							}
						}
						else
						{
							chartMemberList = ReadChartMembers(chart, context, isCategoryHierarchy, 0, ref leafNodes, ref maxLevel);
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == (isCategoryHierarchy ? "ChartCategoryHierarchy" : "ChartSeriesHierarchy"))
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			if (isCategoryHierarchy)
			{
				chart.CategoryCount = leafNodes;
			}
			else
			{
				chart.SeriesCount = leafNodes;
			}
			SetCategoryOrSeriesSpans(chartMemberList, isCategoryHierarchy, maxLevel + 1);
			return chartMemberList;
		}

		private void SetCategoryOrSeriesSpans(ChartMemberList members, bool isCategoryHierarchy, int totalSpansFromLevel)
		{
			foreach (Microsoft.ReportingServices.ReportIntermediateFormat.ChartMember member in members)
			{
				int num;
				if (member.ChartMembers != null && member.ChartMembers.Count > 0)
				{
					num = 1;
					SetCategoryOrSeriesSpans(member.ChartMembers, isCategoryHierarchy, totalSpansFromLevel - 1);
				}
				else
				{
					num = totalSpansFromLevel;
				}
				if (isCategoryHierarchy)
				{
					member.RowSpan = num;
				}
				else
				{
					member.ColSpan = num;
				}
			}
		}

		private ChartMemberList ReadChartMembers(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context, bool isCategoryHierarchy, int level, ref int leafNodes, ref int maxLevel)
		{
			ChartMemberList chartMemberList = new ChartMemberList();
			bool flag = false;
			if (!m_reader.IsEmptyElement)
			{
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName = m_reader.LocalName;
						if (localName == "ChartMember")
						{
							Microsoft.ReportingServices.ReportIntermediateFormat.ChartMember value = ReadChartMember(chart, context, isCategoryHierarchy, level, ref leafNodes, ref maxLevel);
							chartMemberList.Add(value);
						}
						break;
					}
					case XmlNodeType.EndElement:
						if ("ChartMembers" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			if (chartMemberList.Count <= 0)
			{
				return null;
			}
			return chartMemberList;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartMember ReadChartMember(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context, bool isCategoryHierarchy, int level, ref int aLeafNodes, ref int maxLevel)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ChartMember chartMember = new Microsoft.ReportingServices.ReportIntermediateFormat.ChartMember(GenerateID(), chart);
			m_runningValueHolderList.Add(chartMember);
			chartMember.IsColumn = isCategoryHierarchy;
			chartMember.Level = level;
			maxLevel = Math.Max(maxLevel, level);
			bool flag = false;
			int leafNodes = 0;
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
						case "Group":
							chartMember.Grouping = ReadGrouping(chartMember, context);
							if (chartMember.Grouping.PageBreak != null && chartMember.Grouping.PageBreak.BreakLocation != 0)
							{
								context.ErrorContext.Register(ProcessingErrorCode.rsPageBreakOnChartGroup, Severity.Warning, context.ObjectType, context.ObjectName, isCategoryHierarchy ? "CategoryGroupings" : "SeriesGroupings", chartMember.Grouping.Name.MarkAsModelInfo());
							}
							break;
						case "SortExpressions":
							chartMember.Sorting = ReadSortExpressions(isDataRowSortExpression: false, context);
							break;
						case "ChartMembers":
							chartMember.ChartMembers = ReadChartMembers(chart, context, isCategoryHierarchy, level + 1, ref leafNodes, ref maxLevel);
							break;
						case "Label":
							chartMember.Label = ReadExpression("Label", Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "CustomProperties":
							chartMember.CustomProperties = ReadCustomProperties(context);
							break;
						case "DataElementName":
							chartMember.DataElementName = m_reader.ReadString();
							break;
						case "DataElementOutput":
							chartMember.DataElementOutput = ReadDataElementOutput();
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartMember" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			if (chartMember.ChartMembers == null || chartMember.ChartMembers.Count == 0)
			{
				aLeafNodes++;
				if (isCategoryHierarchy)
				{
					chartMember.ColSpan = 1;
				}
				else
				{
					chartMember.RowSpan = 1;
				}
			}
			else
			{
				aLeafNodes += leafNodes;
				if (isCategoryHierarchy)
				{
					chartMember.ColSpan = leafNodes;
				}
				else
				{
					chartMember.RowSpan = leafNodes;
				}
			}
			ValidateAndProcessMemberGroupAndSort(chartMember, context);
			if (chartMember.Grouping != null)
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo label = chartMember.Label;
				if ((label == null || (label.Type == Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant && label.StringValue.Length == 0)) && chartMember.Grouping.GroupExpressions != null && chartMember.Grouping.GroupExpressions.Count > 0)
				{
					chartMember.Label = chartMember.Grouping.GroupExpressions[0];
				}
			}
			else
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo label2 = chartMember.Label;
				if (label2.Type == Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant && label2.StringValue.Length == 0)
				{
					label2.StringValue = null;
				}
			}
			return chartMember;
		}

		private void ChartFakeStaticSeries(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart)
		{
			Global.Tracer.Assert(chart != null);
			Global.Tracer.Assert(chart.SeriesMembers == null || chart.SeriesMembers.Count == 0);
			chart.SeriesCount = 1;
			chart.SeriesMembers = new ChartMemberList(1);
			Microsoft.ReportingServices.ReportIntermediateFormat.ChartMember chartMember = new Microsoft.ReportingServices.ReportIntermediateFormat.ChartMember(GenerateID(), chart);
			chartMember.ColSpan = 1;
			chartMember.RowSpan = 1;
			chart.SeriesMembers.Add(chartMember);
		}

		private void ChartFakeStaticCategory(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart)
		{
			Global.Tracer.Assert(chart != null);
			Global.Tracer.Assert(chart.CategoryMembers == null || chart.CategoryMembers.Count == 0);
			chart.CategoryCount = 1;
			chart.CategoryMembers = new ChartMemberList(1);
			Microsoft.ReportingServices.ReportIntermediateFormat.ChartMember chartMember = new Microsoft.ReportingServices.ReportIntermediateFormat.ChartMember(GenerateID(), chart);
			chartMember.ColSpan = 1;
			chartMember.RowSpan = 1;
			chart.CategoryMembers.Add(chartMember);
		}

		private void ReadChartTitle(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, Microsoft.ReportingServices.ReportIntermediateFormat.ChartTitle title, bool isNoDataMessage, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator namesValidator)
		{
			if (!isNoDataMessage)
			{
				title.TitleName = m_reader.GetAttribute("Name");
				namesValidator.Validate(Severity.Error, "ChartTitle", Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, chart.Name, title.TitleName, context.ErrorContext);
			}
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
					case "Caption":
						title.Caption = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
						break;
					case "Style":
					{
						StyleInformation styleInformation = ReadStyle(context);
						styleInformation.Filter(StyleOwnerType.Chart, hasNoRows: false);
						title.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: true, context.ErrorContext);
						break;
					}
					case "Position":
						title.Position = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
						if (!title.Position.IsExpression)
						{
							Validator.ValidateChartTitlePositions(title.Position.StringValue, context.ErrorContext, context, m_reader.LocalName);
						}
						break;
					case "Hidden":
						title.Hidden = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
						break;
					case "Docking":
						title.Docking = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
						if (!title.Docking.IsExpression)
						{
							Validator.ValidateChartTitleDockings(title.Docking.StringValue, context.ErrorContext, context, m_reader.LocalName);
						}
						break;
					case "DockToChartArea":
						title.DockToChartArea = m_reader.ReadString();
						break;
					case "DockOutsideChartArea":
						title.DockOutsideChartArea = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
						break;
					case "DockOffset":
						title.DockOffset = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
						break;
					case "ToolTip":
						title.ToolTip = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
						break;
					case "ActionInfo":
					{
						title.Action = ReadActionInfo(context, StyleOwnerType.Chart, out bool _);
						break;
					}
					case "TextOrientation":
						title.TextOrientation = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
						if (!title.TextOrientation.IsExpression)
						{
							Validator.ValidateTextOrientations(title.TextOrientation.StringValue, context.ErrorContext, context, m_reader.LocalName);
						}
						break;
					case "ChartElementPosition":
						title.ChartElementPosition = ReadChartElementPosition(chart, context, "ChartElementPosition");
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if (isNoDataMessage)
					{
						if ("ChartNoDataMessage" == m_reader.LocalName)
						{
							flag = true;
						}
					}
					else if ("ChartTitle" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxisTitle ReadChartAxisTitle(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxisTitle chartAxisTitle = new Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxisTitle(chart);
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
							chartAxisTitle.Caption = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Chart, hasNoRows: false);
							chartAxisTitle.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: true, context.ErrorContext);
							break;
						}
						case "Position":
							chartAxisTitle.Position = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartAxisTitle.Position.IsExpression)
							{
								Validator.ValidateChartAxisTitlePositions(chartAxisTitle.Position.StringValue, context.ErrorContext, context, m_reader.LocalName);
							}
							break;
						case "TextOrientation":
							chartAxisTitle.TextOrientation = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartAxisTitle.TextOrientation.IsExpression)
							{
								Validator.ValidateTextOrientations(chartAxisTitle.TextOrientation.StringValue, context.ErrorContext, context, m_reader.LocalName);
							}
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartAxisTitle" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartAxisTitle;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendTitle ReadChartLegendTitle(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendTitle chartLegendTitle = new Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendTitle(chart);
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
							chartLegendTitle.Caption = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.FilterChartLegendTitleStyle();
							chartLegendTitle.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: true, context.ErrorContext);
							break;
						}
						case "TitleSeparator":
							chartLegendTitle.TitleSeparator = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartLegendTitle" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartLegendTitle;
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis> ReadValueAxes(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis>();
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
						if (localName == "ChartAxis")
						{
							list.Add(ReadAxis(chart, context, isCategory: false, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "ChartValueAxes")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis> ReadCategoryAxes(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis>();
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
						if (localName == "ChartAxis")
						{
							list.Add(ReadAxis(chart, context, isCategory: true, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "ChartCategoryAxes")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.ChartArea> ReadChartAreas(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<Microsoft.ReportingServices.ReportIntermediateFormat.ChartArea> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.ChartArea>();
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
						if (localName == "ChartArea")
						{
							list.Add(ReadChartArea(chart, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "ChartAreas")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.ChartTitle> ReadChartTitles(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator namesValidator = new DynamicImageObjectUniqueNameValidator();
			List<Microsoft.ReportingServices.ReportIntermediateFormat.ChartTitle> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.ChartTitle>();
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
						if (localName == "ChartTitle")
						{
							Microsoft.ReportingServices.ReportIntermediateFormat.ChartTitle chartTitle = new Microsoft.ReportingServices.ReportIntermediateFormat.ChartTitle(chart);
							ReadChartTitle(chart, chartTitle, isNoDataMessage: false, context, namesValidator);
							list.Add(chartTitle);
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "ChartTitles")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegend> ReadChartLegends(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegend> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegend>();
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
						if (localName == "ChartLegend")
						{
							list.Add(ReadChartLegend(chart, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "ChartLegends")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartTickMarks ReadChartTickMarks(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context, bool isMajor)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ChartTickMarks chartTickMarks = new Microsoft.ReportingServices.ReportIntermediateFormat.ChartTickMarks(chart);
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
							chartTickMarks.Enabled = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Type":
							chartTickMarks.Type = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartTickMarks.Type.IsExpression)
							{
								Validator.ValidateChartTickMarksType(chartTickMarks.Type.StringValue, context.ErrorContext, context, m_reader.LocalName);
							}
							break;
						case "Length":
							chartTickMarks.Length = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Interval":
							chartTickMarks.Interval = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "IntervalType":
							chartTickMarks.IntervalType = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartTickMarks.IntervalType.IsExpression)
							{
								Validator.ValidateChartIntervalType(chartTickMarks.IntervalType.StringValue, context.ErrorContext, context, m_reader.LocalName);
							}
							break;
						case "IntervalOffset":
							chartTickMarks.IntervalOffset = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "IntervalOffsetType":
							chartTickMarks.IntervalOffsetType = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartTickMarks.IntervalOffsetType.IsExpression)
							{
								Validator.ValidateChartIntervalType(chartTickMarks.IntervalOffsetType.StringValue, context.ErrorContext, context, m_reader.LocalName);
							}
							break;
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Chart, hasNoRows: false);
							chartTickMarks.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: true, context.ErrorContext);
							break;
						}
						}
						break;
					case XmlNodeType.EndElement:
						if ((isMajor && "ChartMajorTickMarks" == m_reader.LocalName) || (!isMajor && "ChartMinorTickMarks" == m_reader.LocalName))
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartTickMarks;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis ReadAxis(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context, bool isCategory, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis = new Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis(chart);
			chartAxis.AxisName = m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, isCategory ? "ChartCategoryAxis" : "ChartValueAxis", Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, chart.Name, chartAxis.AxisName, context.ErrorContext);
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
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Chart, hasNoRows: false);
							chartAxis.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: true, context.ErrorContext);
							break;
						}
						case "Visible":
							chartAxis.Visible = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartAxis.Visible.IsExpression)
							{
								Validator.ValidateChartAutoBool(chartAxis.Visible.StringValue, context.ErrorContext, context, m_reader.LocalName);
							}
							break;
						case "Margin":
							chartAxis.Margin = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartAxis.Margin.IsExpression)
							{
								Validator.ValidateChartAutoBool(chartAxis.Margin.StringValue, context.ErrorContext, context, m_reader.LocalName);
							}
							break;
						case "Interval":
							chartAxis.Interval = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "IntervalType":
							chartAxis.IntervalType = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartAxis.IntervalType.IsExpression)
							{
								Validator.ValidateChartIntervalType(chartAxis.IntervalType.StringValue, context.ErrorContext, context, m_reader.LocalName);
							}
							break;
						case "IntervalOffset":
							chartAxis.IntervalOffset = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "IntervalOffsetType":
							chartAxis.IntervalOffsetType = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartAxis.IntervalOffsetType.IsExpression)
							{
								Validator.ValidateChartIntervalType(chartAxis.IntervalOffsetType.StringValue, context.ErrorContext, context, m_reader.LocalName);
							}
							break;
						case "ChartMajorTickMarks":
							chartAxis.MajorTickMarks = ReadChartTickMarks(chart, context, isMajor: true);
							break;
						case "ChartMinorTickMarks":
							chartAxis.MinorTickMarks = ReadChartTickMarks(chart, context, isMajor: false);
							break;
						case "MarksAlwaysAtPlotEdge":
							chartAxis.MarksAlwaysAtPlotEdge = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "Reverse":
							chartAxis.Reverse = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "Location":
							chartAxis.Location = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartAxis.Location.IsExpression)
							{
								Validator.ValidateChartAxisLocation(chartAxis.Location.StringValue, context.ErrorContext, context, m_reader.LocalName);
							}
							break;
						case "Interlaced":
							chartAxis.Interlaced = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "InterlacedColor":
							chartAxis.InterlacedColor = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartAxis.InterlacedColor.IsExpression)
							{
								PublishingValidator.ValidateColor(chartAxis.InterlacedColor, context.ObjectType, context.ObjectName, m_reader.LocalName, context.ErrorContext);
							}
							break;
						case "LogScale":
							chartAxis.LogScale = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "LogBase":
							chartAxis.LogBase = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "HideLabels":
							chartAxis.HideLabels = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "Angle":
							chartAxis.Angle = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Arrows":
							chartAxis.Arrows = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartAxis.Arrows.IsExpression)
							{
								Validator.ValidateChartAxisArrow(chartAxis.Arrows.StringValue, context.ErrorContext, context, m_reader.LocalName);
							}
							break;
						case "PreventFontShrink":
							chartAxis.PreventFontShrink = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "PreventFontGrow":
							chartAxis.PreventFontGrow = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "PreventLabelOffset":
							chartAxis.PreventLabelOffset = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "PreventWordWrap":
							chartAxis.PreventWordWrap = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "AllowLabelRotation":
							chartAxis.AllowLabelRotation = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartAxis.AllowLabelRotation.IsExpression)
							{
								Validator.ValidateChartAxisLabelRotation(chartAxis.AllowLabelRotation.StringValue, context.ErrorContext, context, m_reader.LocalName);
							}
							break;
						case "IncludeZero":
							chartAxis.IncludeZero = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "LabelsAutoFitDisabled":
							chartAxis.LabelsAutoFitDisabled = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "MinFontSize":
							chartAxis.MinFontSize = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartAxis.MinFontSize.IsExpression)
							{
								PublishingValidator.ValidateSize(chartAxis.MinFontSize, Validator.FontSizeMin, Validator.FontSizeMax, context.ObjectType, context.ObjectName, m_reader.LocalName, context.ErrorContext);
							}
							break;
						case "MaxFontSize":
							chartAxis.MaxFontSize = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartAxis.MaxFontSize.IsExpression)
							{
								PublishingValidator.ValidateSize(chartAxis.MaxFontSize, Validator.FontSizeMin, Validator.FontSizeMax, context.ObjectType, context.ObjectName, m_reader.LocalName, context.ErrorContext);
							}
							break;
						case "OffsetLabels":
							chartAxis.OffsetLabels = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "HideEndLabels":
							chartAxis.HideEndLabels = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "ChartAxisScaleBreak":
							chartAxis.AxisScaleBreak = ReadChartAxisScaleBreak(chart, context);
							break;
						case "ChartAxisTitle":
							chartAxis.Title = ReadChartAxisTitle(chart, context);
							break;
						case "ChartMajorGridLines":
							chartAxis.MajorGridLines = ReadGridLines(chart, context, isMajor: true);
							break;
						case "ChartMinorGridLines":
							chartAxis.MinorGridLines = ReadGridLines(chart, context, isMajor: false);
							break;
						case "CrossAt":
							chartAxis.CrossAt = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Scalar":
							chartAxis.Scalar = m_reader.ReadBoolean(context.ObjectType, context.ObjectName, m_reader.LocalName);
							break;
						case "Minimum":
							chartAxis.Minimum = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Maximum":
							chartAxis.Maximum = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "ChartStripLines":
							chartAxis.StripLines = ReadChartStripLines(chart, context);
							break;
						case "CustomProperties":
							chartAxis.CustomProperties = ReadCustomProperties(context);
							break;
						case "VariableAutoInterval":
							chartAxis.VariableAutoInterval = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "LabelInterval":
							chartAxis.LabelInterval = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "LabelIntervalType":
							chartAxis.LabelIntervalType = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartAxis.LabelIntervalType.IsExpression)
							{
								Validator.ValidateChartIntervalType(chartAxis.LabelIntervalType.StringValue, context.ErrorContext, context, m_reader.LocalName);
							}
							break;
						case "LabelIntervalOffset":
							chartAxis.LabelIntervalOffset = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "LabelIntervalOffsetType":
							chartAxis.LabelIntervalOffsetType = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartAxis.LabelIntervalOffsetType.IsExpression)
							{
								Validator.ValidateChartIntervalType(chartAxis.LabelIntervalOffsetType.StringValue, context.ErrorContext, context, m_reader.LocalName);
							}
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartAxis" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartAxis;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxisScaleBreak ReadChartAxisScaleBreak(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxisScaleBreak chartAxisScaleBreak = new Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxisScaleBreak(chart);
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
							chartAxisScaleBreak.Enabled = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "BreakLineType":
							chartAxisScaleBreak.BreakLineType = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartAxisScaleBreak.BreakLineType.IsExpression)
							{
								Validator.ValidateChartBreakLineType(chartAxisScaleBreak.BreakLineType.StringValue, context.ErrorContext, context, m_reader.LocalName);
							}
							break;
						case "CollapsibleSpaceThreshold":
							chartAxisScaleBreak.CollapsibleSpaceThreshold = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "MaxNumberOfBreaks":
							chartAxisScaleBreak.MaxNumberOfBreaks = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "Spacing":
							chartAxisScaleBreak.Spacing = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "IncludeZero":
							chartAxisScaleBreak.IncludeZero = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartAxisScaleBreak.IncludeZero.IsExpression)
							{
								Validator.ValidateChartAutoBool(chartAxisScaleBreak.IncludeZero.StringValue, context.ErrorContext, context, m_reader.LocalName);
							}
							break;
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Chart, hasNoRows: false);
							chartAxisScaleBreak.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: true, context.ErrorContext);
							break;
						}
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartAxisScaleBreak" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartAxisScaleBreak;
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.ChartFormulaParameter> ReadChartFormulaParameters(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, Microsoft.ReportingServices.ReportIntermediateFormat.ChartDerivedSeries chartDerivedSeries, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<Microsoft.ReportingServices.ReportIntermediateFormat.ChartFormulaParameter> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.ChartFormulaParameter>();
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
						if (localName == "ChartFormulaParameter")
						{
							list.Add(ReadChartFormulaParameter(chart, chartDerivedSeries, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "ChartFormulaParameters")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartFormulaParameter ReadChartFormulaParameter(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, Microsoft.ReportingServices.ReportIntermediateFormat.ChartDerivedSeries chartDerivedSeries, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ChartFormulaParameter chartFormulaParameter = new Microsoft.ReportingServices.ReportIntermediateFormat.ChartFormulaParameter(chart, chartDerivedSeries);
			chartFormulaParameter.FormulaParameterName = m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "ChartFormulaParameter", Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, chart.Name, chartFormulaParameter.FormulaParameterName, context.ErrorContext);
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
						if (!(localName == "Value"))
						{
							if (localName == "Source")
							{
								chartFormulaParameter.Source = m_reader.ReadString();
							}
						}
						else
						{
							chartFormulaParameter.Value = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
						}
						break;
					}
					case XmlNodeType.EndElement:
						if ("ChartFormulaParameter" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartFormulaParameter;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartNoMoveDirections ReadChartNoMoveDirections(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, Microsoft.ReportingServices.ReportIntermediateFormat.ChartSeries chartSeries, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ChartNoMoveDirections chartNoMoveDirections = new Microsoft.ReportingServices.ReportIntermediateFormat.ChartNoMoveDirections(chart, chartSeries);
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
						case "Up":
							chartNoMoveDirections.Up = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "Down":
							chartNoMoveDirections.Down = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "Left":
							chartNoMoveDirections.Left = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "Right":
							chartNoMoveDirections.Right = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "UpLeft":
							chartNoMoveDirections.UpLeft = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "UpRight":
							chartNoMoveDirections.UpRight = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "DownLeft":
							chartNoMoveDirections.DownLeft = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "DownRight":
							chartNoMoveDirections.DownRight = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartNoMoveDirections" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartNoMoveDirections;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendColumn ReadChartLegendColumn(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendColumn chartLegendColumn = new Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendColumn(chart, chart.GenerateActionOwnerID());
			chartLegendColumn.LegendColumnName = m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "ChartLegendColumn", Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, chart.Name, chartLegendColumn.LegendColumnName, context.ErrorContext);
			if (!m_reader.IsEmptyElement)
			{
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
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Chart, hasNoRows: false);
							chartLegendColumn.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: true, context.ErrorContext);
							break;
						}
						case "ActionInfo":
							chartLegendColumn.Action = ReadActionInfo(context, StyleOwnerType.Chart, out computed);
							break;
						case "ColumnType":
							chartLegendColumn.ColumnType = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartLegendColumn.ColumnType.IsExpression)
							{
								Validator.ValidateChartColumnType(chartLegendColumn.ColumnType.StringValue, context.ErrorContext, context, m_reader.LocalName);
							}
							break;
						case "Value":
							chartLegendColumn.Value = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "ToolTip":
							chartLegendColumn.ToolTip = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "MinimumWidth":
							chartLegendColumn.MinimumWidth = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartLegendColumn.MinimumWidth.IsExpression)
							{
								PublishingValidator.ValidateSize(chartLegendColumn.MinimumWidth, Validator.NormalMin, Validator.NormalMax, context.ObjectType, context.ObjectName, m_reader.LocalName, context.ErrorContext);
							}
							break;
						case "MaximumWidth":
							chartLegendColumn.MaximumWidth = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartLegendColumn.MaximumWidth.IsExpression)
							{
								PublishingValidator.ValidateSize(chartLegendColumn.MaximumWidth, Validator.NormalMin, Validator.NormalMax, context.ObjectType, context.ObjectName, m_reader.LocalName, context.ErrorContext);
							}
							break;
						case "SeriesSymbolWidth":
							chartLegendColumn.SeriesSymbolWidth = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "SeriesSymbolHeight":
							chartLegendColumn.SeriesSymbolHeight = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "Header":
							chartLegendColumn.Header = ReadChartLegendColumnHeader(chart, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartLegendColumn" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartLegendColumn;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartElementPosition ReadChartElementPosition(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context, string chartElementPositionName)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ChartElementPosition chartElementPosition = new Microsoft.ReportingServices.ReportIntermediateFormat.ChartElementPosition(chart);
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
						case "Top":
							chartElementPosition.Top = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Left":
							chartElementPosition.Left = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Height":
							chartElementPosition.Height = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Width":
							chartElementPosition.Width = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if (chartElementPositionName == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartElementPosition;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartSmartLabel ReadChartSmartLabel(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, Microsoft.ReportingServices.ReportIntermediateFormat.ChartSeries chartSeries, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ChartSmartLabel chartSmartLabel = new Microsoft.ReportingServices.ReportIntermediateFormat.ChartSmartLabel(chart, chartSeries);
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
						case "AllowOutSidePlotArea":
							chartSmartLabel.AllowOutSidePlotArea = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartSmartLabel.AllowOutSidePlotArea.IsExpression)
							{
								Validator.ValidateChartAllowOutsideChartArea(chartSmartLabel.AllowOutSidePlotArea.StringValue, context.ErrorContext, context, m_reader.LocalName);
							}
							break;
						case "CalloutBackColor":
							chartSmartLabel.CalloutBackColor = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartSmartLabel.CalloutBackColor.IsExpression)
							{
								PublishingValidator.ValidateColor(chartSmartLabel.CalloutBackColor, context.ObjectType, context.ObjectName, m_reader.LocalName, context.ErrorContext);
							}
							break;
						case "CalloutLineAnchor":
							chartSmartLabel.CalloutLineAnchor = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartSmartLabel.CalloutLineAnchor.IsExpression)
							{
								Validator.ValidateChartCalloutLineAnchor(chartSmartLabel.CalloutLineAnchor.StringValue, context.ErrorContext, context, m_reader.LocalName);
							}
							break;
						case "CalloutLineColor":
							chartSmartLabel.CalloutLineColor = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartSmartLabel.CalloutLineColor.IsExpression)
							{
								PublishingValidator.ValidateColor(chartSmartLabel.CalloutLineColor, context.ObjectType, context.ObjectName, m_reader.LocalName, context.ErrorContext);
							}
							break;
						case "CalloutLineStyle":
							chartSmartLabel.CalloutLineStyle = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartSmartLabel.CalloutLineStyle.IsExpression)
							{
								Validator.ValidateChartCalloutLineStyle(chartSmartLabel.CalloutLineStyle.StringValue, context.ErrorContext, context, m_reader.LocalName);
							}
							break;
						case "CalloutLineWidth":
							chartSmartLabel.CalloutLineWidth = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartSmartLabel.CalloutLineWidth.IsExpression)
							{
								PublishingValidator.ValidateSize(chartSmartLabel.CalloutLineWidth, Validator.NormalMin, Validator.NormalMax, context.ObjectType, context.ObjectName, m_reader.LocalName, context.ErrorContext);
							}
							break;
						case "CalloutStyle":
							chartSmartLabel.CalloutStyle = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartSmartLabel.CalloutStyle.IsExpression)
							{
								Validator.ValidateChartCalloutStyle(chartSmartLabel.CalloutStyle.StringValue, context.ErrorContext, context, m_reader.LocalName);
							}
							break;
						case "ShowOverlapped":
							chartSmartLabel.ShowOverlapped = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "MarkerOverlapping":
							chartSmartLabel.MarkerOverlapping = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "MaxMovingDistance":
							chartSmartLabel.MaxMovingDistance = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartSmartLabel.MaxMovingDistance.IsExpression)
							{
								PublishingValidator.ValidateSize(chartSmartLabel.MaxMovingDistance, Validator.NormalMin, Validator.NormalMax, context.ObjectType, context.ObjectName, m_reader.LocalName, context.ErrorContext);
							}
							break;
						case "MinMovingDistance":
							chartSmartLabel.MinMovingDistance = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartSmartLabel.MinMovingDistance.IsExpression)
							{
								PublishingValidator.ValidateSize(chartSmartLabel.MinMovingDistance, Validator.NormalMin, Validator.NormalMax, context.ObjectType, context.ObjectName, m_reader.LocalName, context.ErrorContext);
							}
							break;
						case "Disabled":
							chartSmartLabel.Disabled = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "ChartNoMoveDirections":
							chartSmartLabel.NoMoveDirections = ReadChartNoMoveDirections(chart, chartSeries, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartSmartLabel" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartSmartLabel;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell ReadChartLegendCustomItemCell(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell chartLegendCustomItemCell = new Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell(chart, chart.GenerateActionOwnerID());
			chartLegendCustomItemCell.LegendCustomItemCellName = m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "ChartLegendCustomItemCell", Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, chart.Name, chartLegendCustomItemCell.LegendCustomItemCellName, context.ErrorContext);
			if (!m_reader.IsEmptyElement)
			{
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
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Chart, hasNoRows: false);
							chartLegendCustomItemCell.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: true, context.ErrorContext);
							break;
						}
						case "ActionInfo":
							chartLegendCustomItemCell.Action = ReadActionInfo(context, StyleOwnerType.Chart, out computed);
							break;
						case "CellType":
							chartLegendCustomItemCell.CellType = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartLegendCustomItemCell.CellType.IsExpression)
							{
								Validator.ValidateChartCellType(chartLegendCustomItemCell.CellType.StringValue, context.ErrorContext, context, m_reader.LocalName);
							}
							break;
						case "Text":
							chartLegendCustomItemCell.Text = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "CellSpan":
							chartLegendCustomItemCell.CellSpan = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "ToolTip":
							chartLegendCustomItemCell.ToolTip = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "ImageWidth":
							chartLegendCustomItemCell.ImageWidth = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "ImageHeight":
							chartLegendCustomItemCell.ImageHeight = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "SymbolHeight":
							chartLegendCustomItemCell.SymbolHeight = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "SymbolWidth":
							chartLegendCustomItemCell.SymbolWidth = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "Alignment":
							chartLegendCustomItemCell.Alignment = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "TopMargin":
							chartLegendCustomItemCell.TopMargin = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "BottomMargin":
							chartLegendCustomItemCell.BottomMargin = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "LeftMargin":
							chartLegendCustomItemCell.LeftMargin = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "RightMargin":
							chartLegendCustomItemCell.RightMargin = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartLegendCustomItemCell" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartLegendCustomItemCell;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItem ReadChartLegendCustomItem(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItem chartLegendCustomItem = new Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItem(chart, chart.GenerateActionOwnerID());
			chartLegendCustomItem.LegendCustomItemName = m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "ChartLegendCustomItem", Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, chart.Name, chartLegendCustomItem.LegendCustomItemName, context.ErrorContext);
			if (!m_reader.IsEmptyElement)
			{
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
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Chart, hasNoRows: false);
							chartLegendCustomItem.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: true, context.ErrorContext);
							break;
						}
						case "ActionInfo":
							chartLegendCustomItem.Action = ReadActionInfo(context, StyleOwnerType.Chart, out computed);
							break;
						case "ChartMarker":
							chartLegendCustomItem.Marker = ReadChartMarker(chart, null, null, context);
							break;
						case "Separator":
							chartLegendCustomItem.Separator = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartLegendCustomItem.Separator.IsExpression)
							{
								Validator.ValidateChartCustomItemSeparator(chartLegendCustomItem.Separator.StringValue, context.ErrorContext, context, m_reader.LocalName);
							}
							break;
						case "SeparatorColor":
							chartLegendCustomItem.SeparatorColor = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartLegendCustomItem.SeparatorColor.IsExpression)
							{
								PublishingValidator.ValidateColor(chartLegendCustomItem.SeparatorColor, context.ObjectType, context.ObjectName, m_reader.LocalName, context.ErrorContext);
							}
							break;
						case "ToolTip":
							chartLegendCustomItem.ToolTip = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "ChartLegendCustomItemCells":
							chartLegendCustomItem.LegendCustomItemCells = ReadChartLegendCustomItemCells(chart, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartLegendCustomItem" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartLegendCustomItem;
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell> ReadChartLegendCustomItemCells(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell>();
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
						if (localName == "ChartLegendCustomItemCell")
						{
							list.Add(ReadChartLegendCustomItemCell(chart, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "ChartLegendCustomItemCells")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegend ReadChartLegend(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend = new Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegend(chart);
			chartLegend.LegendName = m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "ChartLegend", Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, chart.Name, chartLegend.LegendName, context.ErrorContext);
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
						case "Hidden":
							chartLegend.Hidden = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Chart, hasNoRows: false);
							chartLegend.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: true, context.ErrorContext);
							break;
						}
						case "Position":
							chartLegend.Position = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Layout":
							chartLegend.Layout = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "DockToChartArea":
							chartLegend.DockToChartArea = m_reader.ReadString();
							break;
						case "DockOutsideChartArea":
							chartLegend.DockOutsideChartArea = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "ChartLegendTitle":
							chartLegend.LegendTitle = ReadChartLegendTitle(chart, context);
							break;
						case "AutoFitTextDisabled":
							chartLegend.AutoFitTextDisabled = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "MinFontSize":
							chartLegend.MinFontSize = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartLegend.MinFontSize.IsExpression)
							{
								PublishingValidator.ValidateSize(chartLegend.MinFontSize, Validator.FontSizeMin, Validator.FontSizeMax, context.ObjectType, context.ObjectName, m_reader.LocalName, context.ErrorContext);
							}
							break;
						case "HeaderSeparator":
							chartLegend.HeaderSeparator = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartLegend.HeaderSeparator.IsExpression)
							{
								Validator.ValidateChartCustomItemSeparator(chartLegend.HeaderSeparator.StringValue, context.ErrorContext, context, m_reader.LocalName);
							}
							break;
						case "HeaderSeparatorColor":
							chartLegend.HeaderSeparatorColor = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartLegend.HeaderSeparatorColor.IsExpression)
							{
								PublishingValidator.ValidateColor(chartLegend.HeaderSeparatorColor, context.ObjectType, context.ObjectName, m_reader.LocalName, context.ErrorContext);
							}
							break;
						case "ColumnSeparator":
							chartLegend.ColumnSeparator = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartLegend.ColumnSeparator.IsExpression)
							{
								Validator.ValidateChartCustomItemSeparator(chartLegend.ColumnSeparator.StringValue, context.ErrorContext, context, m_reader.LocalName);
							}
							break;
						case "ColumnSeparatorColor":
							chartLegend.ColumnSeparatorColor = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartLegend.ColumnSeparatorColor.IsExpression)
							{
								PublishingValidator.ValidateColor(chartLegend.ColumnSeparatorColor, context.ObjectType, context.ObjectName, m_reader.LocalName, context.ErrorContext);
							}
							break;
						case "ColumnSpacing":
							chartLegend.ColumnSpacing = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "InterlacedRows":
							chartLegend.InterlacedRows = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "InterlacedRowsColor":
							chartLegend.InterlacedRowsColor = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartLegend.InterlacedRowsColor.IsExpression)
							{
								PublishingValidator.ValidateColor(chartLegend.InterlacedRowsColor, context.ObjectType, context.ObjectName, m_reader.LocalName, context.ErrorContext);
							}
							break;
						case "EquallySpacedItems":
							chartLegend.EquallySpacedItems = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "Reversed":
							chartLegend.Reversed = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartLegend.Reversed.IsExpression)
							{
								Validator.ValidateChartAutoBool(chartLegend.Reversed.StringValue, context.ErrorContext, context, m_reader.LocalName);
							}
							break;
						case "MaxAutoSize":
							chartLegend.MaxAutoSize = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "TextWrapThreshold":
							chartLegend.TextWrapThreshold = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "ChartLegendColumns":
							chartLegend.LegendColumns = ReadChartLegendColumns(chart, context);
							break;
						case "ChartLegendCustomItems":
							chartLegend.LegendCustomItems = ReadChartLegendCustomItems(chart, context);
							break;
						case "ChartElementPosition":
							chartLegend.ChartElementPosition = ReadChartElementPosition(chart, context, "ChartElementPosition");
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartLegend" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartLegend;
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendColumn> ReadChartLegendColumns(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendColumn> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendColumn>();
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
						if (localName == "ChartLegendColumn")
						{
							list.Add(ReadChartLegendColumn(chart, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "ChartLegendColumns")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItem> ReadChartLegendCustomItems(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItem> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItem>();
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
						if (localName == "ChartLegendCustomItem")
						{
							list.Add(ReadChartLegendCustomItem(chart, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "ChartLegendCustomItems")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.ChartStripLine> ReadChartStripLines(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context)
		{
			List<Microsoft.ReportingServices.ReportIntermediateFormat.ChartStripLine> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.ChartStripLine>();
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
						if (localName == "ChartStripLine")
						{
							list.Add(ReadChartStripLine(chart, context));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "ChartStripLines")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.ChartDerivedSeries> ReadChartDerivedSeriesCollection(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<Microsoft.ReportingServices.ReportIntermediateFormat.ChartDerivedSeries> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.ChartDerivedSeries>();
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
						if (localName == "ChartDerivedSeries")
						{
							list.Add(ReadChartDerivedSeries(chart, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "ChartDerivedSeriesCollection")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartDerivedSeries ReadChartDerivedSeries(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ChartDerivedSeries chartDerivedSeries = new Microsoft.ReportingServices.ReportIntermediateFormat.ChartDerivedSeries(chart);
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				bool hasAggregates = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (m_reader.LocalName)
						{
						case "ChartSeries":
							chartDerivedSeries.Series = ReadChartSeries(chart, chartDerivedSeries, context, ref hasAggregates, nameValidator);
							break;
						case "SourceChartSeriesName":
							chartDerivedSeries.SourceChartSeriesName = m_reader.ReadString();
							break;
						case "DerivedSeriesFormula":
							chartDerivedSeries.DerivedSeriesFormula = (ChartSeriesFormula)Enum.Parse(typeof(ChartSeriesFormula), m_reader.ReadString(), ignoreCase: false);
							break;
						case "ChartFormulaParameters":
							chartDerivedSeries.FormulaParameters = ReadChartFormulaParameters(chart, chartDerivedSeries, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartDerivedSeries" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartDerivedSeries;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartStripLine ReadChartStripLine(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ChartStripLine chartStripLine = new Microsoft.ReportingServices.ReportIntermediateFormat.ChartStripLine(chart, chart.GenerateActionOwnerID());
			if (!m_reader.IsEmptyElement)
			{
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
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.FilterChartStripLineStyle();
							chartStripLine.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: true, context.ErrorContext);
							break;
						}
						case "ActionInfo":
							chartStripLine.Action = ReadActionInfo(context, StyleOwnerType.Chart, out computed);
							break;
						case "Title":
							chartStripLine.Title = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "TitleAngle":
							chartStripLine.TitleAngle = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "ToolTip":
							chartStripLine.ToolTip = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Interval":
							chartStripLine.Interval = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "IntervalType":
							chartStripLine.IntervalType = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartStripLine.IntervalType.IsExpression)
							{
								Validator.ValidateChartIntervalType(chartStripLine.IntervalType.StringValue, context.ErrorContext, context, m_reader.LocalName);
							}
							break;
						case "IntervalOffset":
							chartStripLine.IntervalOffset = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "IntervalOffsetType":
							chartStripLine.IntervalOffsetType = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartStripLine.IntervalOffsetType.IsExpression)
							{
								Validator.ValidateChartIntervalType(chartStripLine.IntervalOffsetType.StringValue, context.ErrorContext, context, m_reader.LocalName);
							}
							break;
						case "StripWidth":
							chartStripLine.StripWidth = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "StripWidthType":
							chartStripLine.StripWidthType = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartStripLine.StripWidthType.IsExpression)
							{
								Validator.ValidateChartIntervalType(chartStripLine.StripWidthType.StringValue, context.ErrorContext, context, m_reader.LocalName);
							}
							break;
						case "TextOrientation":
							chartStripLine.TextOrientation = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartStripLine.TextOrientation.IsExpression)
							{
								Validator.ValidateTextOrientations(chartStripLine.TextOrientation.StringValue, context.ErrorContext, context, m_reader.LocalName);
							}
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartStripLine" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartStripLine;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartGridLines ReadGridLines(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context, bool isMajor)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ChartGridLines chartGridLines = new Microsoft.ReportingServices.ReportIntermediateFormat.ChartGridLines(chart);
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
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Chart, hasNoRows: false);
							chartGridLines.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: true, context.ErrorContext);
							break;
						}
						case "Enabled":
							chartGridLines.Enabled = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Interval":
							chartGridLines.Interval = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "IntervalType":
							chartGridLines.IntervalType = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartGridLines.IntervalType.IsExpression)
							{
								Validator.ValidateChartIntervalType(chartGridLines.IntervalType.StringValue, context.ErrorContext, context, m_reader.LocalName);
							}
							break;
						case "IntervalOffset":
							chartGridLines.IntervalOffset = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "IntervalOffsetType":
							chartGridLines.IntervalOffsetType = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartGridLines.IntervalOffsetType.IsExpression)
							{
								Validator.ValidateChartIntervalType(chartGridLines.IntervalOffsetType.StringValue, context.ErrorContext, context, m_reader.LocalName);
							}
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ((isMajor && "ChartMajorGridLines" == m_reader.LocalName) || (!isMajor && "ChartMinorGridLines" == m_reader.LocalName))
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartGridLines;
		}

		private void ReadChartData(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context, out bool hasAggregates)
		{
			hasAggregates = false;
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
						if (!(localName == "ChartSeriesCollection"))
						{
							if (localName == "ChartDerivedSeriesCollection")
							{
								chart.DerivedSeriesCollection = ReadChartDerivedSeriesCollection(chart, context);
							}
						}
						else
						{
							chart.ChartSeriesCollection = ReadChartSeriesCollection(chart, context, ref hasAggregates);
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
			}
			if (chart.DerivedSeriesCollection == null)
			{
				return;
			}
			foreach (Microsoft.ReportingServices.ReportIntermediateFormat.ChartDerivedSeries item in chart.DerivedSeriesCollection)
			{
				if (item.SourceSeries == null)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidSourceSeriesName, Severity.Error, context.ObjectType, context.ObjectName, "SourceChartSeriesName", item.SourceChartSeriesName.MarkAsPrivate());
				}
			}
		}

		private ChartSeriesList ReadChartSeriesCollection(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context, ref bool hasAggregates)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			ChartSeriesList chartSeriesList = new ChartSeriesList();
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
						if (localName == "ChartSeries")
						{
							chartSeriesList.Add(ReadChartSeries(chart, null, context, ref hasAggregates, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if ("ChartSeriesCollection" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartSeriesList;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartArea ReadChartArea(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ChartArea chartArea = new Microsoft.ReportingServices.ReportIntermediateFormat.ChartArea(chart);
			chartArea.ChartAreaName = m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "ChartArea", Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, chart.Name, chartArea.ChartAreaName, context.ErrorContext);
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
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Chart, hasNoRows: false);
							chartArea.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: true, context.ErrorContext);
							break;
						}
						case "ChartCategoryAxes":
							chartArea.CategoryAxes = ReadCategoryAxes(chart, context);
							break;
						case "ChartValueAxes":
							chartArea.ValueAxes = ReadValueAxes(chart, context);
							break;
						case "ChartThreeDProperties":
							chartArea.ThreeDProperties = ReadThreeDProperties(chart, context);
							break;
						case "Hidden":
							chartArea.Hidden = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "AlignOrientation":
							chartArea.AlignOrientation = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "ChartAlignType":
							chartArea.ChartAlignType = ReadChartAlignType(chart, context);
							break;
						case "AlignWithChartArea":
							chartArea.AlignWithChartArea = m_reader.ReadString();
							break;
						case "EquallySizedAxesFont":
							chartArea.EquallySizedAxesFont = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "ChartElementPosition":
							chartArea.ChartElementPosition = ReadChartElementPosition(chart, context, "ChartElementPosition");
							break;
						case "ChartInnerPlotPosition":
							chartArea.ChartInnerPlotPosition = ReadChartElementPosition(chart, context, "ChartInnerPlotPosition");
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartArea" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartArea;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartAlignType ReadChartAlignType(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ChartAlignType chartAlignType = null;
			if (!m_reader.IsEmptyElement)
			{
				chartAlignType = new Microsoft.ReportingServices.ReportIntermediateFormat.ChartAlignType(chart);
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (m_reader.LocalName)
						{
						case "Position":
							chartAlignType.Position = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "InnerPlotPosition":
							chartAlignType.InnerPlotPosition = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "AxesView":
							chartAlignType.AxesView = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "Cursor":
							chartAlignType.Cursor = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartAlignType" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartAlignType;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartSeries ReadChartSeries(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, Microsoft.ReportingServices.ReportIntermediateFormat.ChartDerivedSeries chartDerivedSeries, PublishingContextStruct context, ref bool hasAggregates, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ChartSeries chartSeries = new Microsoft.ReportingServices.ReportIntermediateFormat.ChartSeries(chart, chartDerivedSeries, GenerateID());
			chartSeries.Name = m_reader.GetAttribute("Name");
			if (!string.IsNullOrEmpty(chartSeries.Name))
			{
				nameValidator.Validate(Severity.Error, "ChartSeries", Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, chart.Name, chartSeries.Name, context.ErrorContext);
			}
			if (!m_reader.IsEmptyElement)
			{
				string text = null;
				bool flag = false;
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
							if (chartDerivedSeries != null)
							{
								styleInformation.Filter(StyleOwnerType.Chart, hasNoRows: false);
							}
							else
							{
								styleInformation.FilterChartSeriesStyle();
							}
							chartSeries.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: true, context.ErrorContext);
							break;
						}
						case "CustomProperties":
							chartSeries.CustomProperties = ReadCustomProperties(context);
							break;
						case "ChartDataPoints":
							chartSeries.DataPoints = ReadChartDataPoints(chart, context, ref hasAggregates);
							break;
						case "Type":
							chartSeries.Type = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartSeries.Type.IsExpression)
							{
								Validator.ValidateChartSeriesType(chartSeries.Type.StringValue, context.ErrorContext, context, m_reader.LocalName);
							}
							break;
						case "Subtype":
						{
							Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!expressionInfo.IsExpression)
							{
								Validator.ValidateChartSeriesSubtype(expressionInfo.StringValue, context.ErrorContext, context, m_reader.LocalName, m_reader.NamespaceURI);
							}
							if (text == null || RdlNamespaceComparer.Instance.Compare(m_reader.NamespaceURI, text) > 0)
							{
								chartSeries.Subtype = expressionInfo;
								text = m_reader.NamespaceURI;
							}
							break;
						}
						case "ChartEmptyPoints":
							chartSeries.EmptyPoints = ReadChartEmptyPoints(chart, chartSeries, context);
							break;
						case "LegendName":
							chartSeries.LegendName = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "ChartAreaName":
							chartSeries.ChartAreaName = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "ValueAxisName":
							chartSeries.ValueAxisName = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "CategoryAxisName":
							chartSeries.CategoryAxisName = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Hidden":
							chartSeries.Hidden = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "ChartSmartLabel":
							chartSeries.ChartSmartLabel = ReadChartSmartLabel(chart, chartSeries, context);
							break;
						case "ChartDataLabel":
							if (chartDerivedSeries != null)
							{
								chartSeries.DataLabel = ReadChartDataLabel(chart, chartSeries, null, context);
							}
							break;
						case "ChartMarker":
							if (chartDerivedSeries != null)
							{
								chartSeries.Marker = ReadChartMarker(chart, chartSeries, null, context);
							}
							break;
						case "ChartItemInLegend":
							chartSeries.ChartItemInLegend = ReadChartItemInLegend(chart, chartSeries, null, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartSeries" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartSeries;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPointList ReadChartDataPoints(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context, ref bool hasAggregates)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPointList chartDataPointList = new Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPointList();
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
						if (localName == "ChartDataPoint")
						{
							chartDataPointList.Add(ReadChartDataPoint(chart, context, ref hasAggregates));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if ("ChartDataPoints" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartDataPointList;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint ReadChartDataPoint(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context, ref bool hasAggregates)
		{
			context.Location |= LocationFlags.InDynamicTablixCell;
			Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint chartDataPoint = new Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint(GenerateID(), chart);
			m_aggregateHolderList.Add(chartDataPoint);
			m_runningValueHolderList.Add(chartDataPoint);
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				bool computed = false;
				string dataSetName = null;
				List<IdcRelationship> relationships = null;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (m_reader.LocalName)
						{
						case "DataSetName":
							dataSetName = m_reader.ReadString();
							break;
						case "Relationships":
							relationships = ReadRelationships(context);
							break;
						case "ChartDataPointValues":
							chartDataPoint.DataPointValues = ReadChartDataPointValues(chart, chartDataPoint, context);
							break;
						case "ChartDataLabel":
							chartDataPoint.DataLabel = ReadChartDataLabel(chart, null, chartDataPoint, context);
							break;
						case "ActionInfo":
							chartDataPoint.Action = ReadActionInfo(context, StyleOwnerType.Chart, out computed);
							break;
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Chart, hasNoRows: false);
							chartDataPoint.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: true, context.ErrorContext);
							break;
						}
						case "ChartMarker":
							chartDataPoint.Marker = ReadChartMarker(chart, null, chartDataPoint, context);
							break;
						case "DataElementName":
							chartDataPoint.DataElementName = m_reader.ReadString();
							break;
						case "DataElementOutput":
							chartDataPoint.DataElementOutput = ReadDataElementOutput();
							break;
						case "CustomProperties":
							chartDataPoint.CustomProperties = ReadCustomProperties(context);
							break;
						case "AxisLabel":
							chartDataPoint.AxisLabel = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "ChartItemInLegend":
							chartDataPoint.ItemInLegend = ReadChartItemInLegend(chart, null, chartDataPoint, context);
							break;
						case "ToolTip":
							chartDataPoint.ToolTip = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartDataPoint" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
				chartDataPoint.DataScopeInfo.SetRelationship(dataSetName, relationships);
			}
			return chartDataPoint;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartMarker ReadChartMarker(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, Microsoft.ReportingServices.ReportIntermediateFormat.ChartSeries series, Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ChartMarker chartMarker = (dataPoint == null) ? new Microsoft.ReportingServices.ReportIntermediateFormat.ChartMarker(chart, series) : new Microsoft.ReportingServices.ReportIntermediateFormat.ChartMarker(chart, dataPoint);
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
						case "Type":
							chartMarker.Type = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartMarker.Type.IsExpression)
							{
								Validator.ValidateChartMarkerType(chartMarker.Type.StringValue, context.ErrorContext, context, m_reader.LocalName);
							}
							break;
						case "Size":
							chartMarker.Size = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartMarker.Size.IsExpression)
							{
								PublishingValidator.ValidateSize(chartMarker.Size, Validator.NormalMin, Validator.NormalMax, context.ObjectType, context.ObjectName, m_reader.LocalName, context.ErrorContext);
							}
							break;
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Chart, hasNoRows: false);
							chartMarker.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: true, context.ErrorContext);
							break;
						}
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartMarker" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartMarker;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataLabel ReadChartDataLabel(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, Microsoft.ReportingServices.ReportIntermediateFormat.ChartSeries series, Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataLabel chartDataLabel = (dataPoint == null) ? new Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataLabel(chart, series) : new Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataLabel(chart, dataPoint);
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
							chartDataLabel.Visible = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Chart, hasNoRows: false);
							chartDataLabel.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: true, context.ErrorContext);
							break;
						}
						case "Label":
							chartDataLabel.Label = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Position":
							chartDataLabel.Position = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartDataLabel.Position.IsExpression)
							{
								Validator.ValidateChartDataLabelPosition(chartDataLabel.Position.StringValue, context.ErrorContext, context, m_reader.LocalName);
							}
							break;
						case "Rotation":
							chartDataLabel.Rotation = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "UseValueAsLabel":
							chartDataLabel.UseValueAsLabel = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "ActionInfo":
						{
							chartDataLabel.Action = ReadActionInfo(context, StyleOwnerType.Chart, out bool _);
							break;
						}
						case "ToolTip":
							chartDataLabel.ToolTip = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartDataLabel" == m_reader.LocalName)
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

		private Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo ReadChartDataPointFormatExpressionValues(string propertyName, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo result = null;
			if (m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.CellLevelFormatting))
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, context.ObjectType, context.ObjectName, propertyName);
			}
			else
			{
				result = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
			}
			return result;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPointValues ReadChartDataPointValues(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPointValues chartDataPointValues = new Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPointValues(chart, dataPoint);
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
						case "X":
							chartDataPointValues.X = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Y":
							chartDataPointValues.Y = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Size":
							chartDataPointValues.Size = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "High":
							chartDataPointValues.High = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Low":
							chartDataPointValues.Low = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Start":
							chartDataPointValues.Start = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "End":
							chartDataPointValues.End = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Mean":
							chartDataPointValues.Mean = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Median":
							chartDataPointValues.Median = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "HighlightX":
							chartDataPointValues.HighlightX = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "HighlightY":
							chartDataPointValues.HighlightY = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "HighlightSize":
							chartDataPointValues.HighlightSize = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "FormatX":
						{
							Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo6 = ReadChartDataPointFormatExpressionValues("FormatX", context);
							if (expressionInfo6 != null)
							{
								chartDataPointValues.FormatX = expressionInfo6;
							}
							break;
						}
						case "FormatY":
						{
							Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo5 = ReadChartDataPointFormatExpressionValues("FormatY", context);
							if (expressionInfo5 != null)
							{
								chartDataPointValues.FormatY = expressionInfo5;
							}
							break;
						}
						case "FormatSize":
						{
							Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo4 = ReadChartDataPointFormatExpressionValues("FormatSize", context);
							if (expressionInfo4 != null)
							{
								chartDataPointValues.FormatSize = expressionInfo4;
							}
							break;
						}
						case "CurrencyLanguageX":
						{
							Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo3 = ReadChartDataPointFormatExpressionValues("CurrencyLanguageX", context);
							if (expressionInfo3 != null)
							{
								chartDataPointValues.CurrencyLanguageX = expressionInfo3;
							}
							break;
						}
						case "CurrencyLanguageY":
						{
							Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo2 = ReadChartDataPointFormatExpressionValues("CurrencyLanguageY", context);
							if (expressionInfo2 != null)
							{
								chartDataPointValues.CurrencyLanguageY = expressionInfo2;
							}
							break;
						}
						case "CurrencyLanguageSize":
						{
							Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = ReadChartDataPointFormatExpressionValues("CurrencyLanguageSize", context);
							if (expressionInfo != null)
							{
								chartDataPointValues.CurrencyLanguageSize = expressionInfo;
							}
							break;
						}
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartDataPointValues" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartDataPointValues;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartThreeDProperties ReadThreeDProperties(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ChartThreeDProperties chartThreeDProperties = new Microsoft.ReportingServices.ReportIntermediateFormat.ChartThreeDProperties(chart);
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
							chartThreeDProperties.Enabled = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "ProjectionMode":
							chartThreeDProperties.ProjectionMode = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartThreeDProperties.ProjectionMode.IsExpression)
							{
								Validator.ValidateChartThreeDProjectionMode(chartThreeDProperties.ProjectionMode.StringValue, context.ErrorContext, context, m_reader.LocalName);
							}
							break;
						case "Rotation":
							chartThreeDProperties.Rotation = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "Inclination":
							chartThreeDProperties.Inclination = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "Perspective":
							chartThreeDProperties.Perspective = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "DepthRatio":
							chartThreeDProperties.DepthRatio = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "Shading":
							chartThreeDProperties.Shading = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartThreeDProperties.Shading.IsExpression)
							{
								Validator.ValidateChartThreeDShading(chartThreeDProperties.Shading.StringValue, context.ErrorContext, context, m_reader.LocalName);
							}
							break;
						case "GapDepth":
							chartThreeDProperties.GapDepth = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "WallThickness":
							chartThreeDProperties.WallThickness = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "Clustered":
							chartThreeDProperties.Clustered = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartThreeDProperties" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartThreeDProperties;
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.ChartCustomPaletteColor> ReadChartCustomPaletteColors(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context)
		{
			List<Microsoft.ReportingServices.ReportIntermediateFormat.ChartCustomPaletteColor> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.ChartCustomPaletteColor>();
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
						if (localName == "ChartCustomPaletteColor")
						{
							Microsoft.ReportingServices.ReportIntermediateFormat.ChartCustomPaletteColor chartCustomPaletteColor = new Microsoft.ReportingServices.ReportIntermediateFormat.ChartCustomPaletteColor(chart);
							chartCustomPaletteColor.Color = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							list.Add(chartCustomPaletteColor);
						}
						break;
					}
					case XmlNodeType.EndElement:
						if ("ChartCustomPaletteColors" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.DataValueList ReadChartCodeParameters(PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.DataValueList dataValueList = new Microsoft.ReportingServices.ReportIntermediateFormat.DataValueList();
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				bool isComputed = false;
				int num = 0;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName = m_reader.LocalName;
						if (localName == "ChartCodeParameter")
						{
							dataValueList.Add(ReadDataValue(isCustomProperty: false, nameRequired: true, ++num, ref isComputed, context));
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
			}
			return dataValueList;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartItemInLegend ReadChartItemInLegend(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, Microsoft.ReportingServices.ReportIntermediateFormat.ChartSeries series, Microsoft.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ChartItemInLegend chartItemInLegend = (dataPoint == null) ? new Microsoft.ReportingServices.ReportIntermediateFormat.ChartItemInLegend(chart, series) : new Microsoft.ReportingServices.ReportIntermediateFormat.ChartItemInLegend(chart, dataPoint);
			if (!m_reader.IsEmptyElement)
			{
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
						case "ActionInfo":
							chartItemInLegend.Action = ReadActionInfo(context, StyleOwnerType.Chart, out computed);
							break;
						case "LegendText":
							chartItemInLegend.LegendText = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "ToolTip":
							chartItemInLegend.ToolTip = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Hidden":
							chartItemInLegend.Hidden = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartItemInLegend" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartItemInLegend;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartEmptyPoints ReadChartEmptyPoints(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, Microsoft.ReportingServices.ReportIntermediateFormat.ChartSeries series, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ChartEmptyPoints chartEmptyPoints = new Microsoft.ReportingServices.ReportIntermediateFormat.ChartEmptyPoints(chart, series);
			if (!m_reader.IsEmptyElement)
			{
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
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Chart, hasNoRows: false);
							chartEmptyPoints.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: true, context.ErrorContext);
							break;
						}
						case "ActionInfo":
							chartEmptyPoints.Action = ReadActionInfo(context, StyleOwnerType.Chart, out computed);
							break;
						case "ChartMarker":
							chartEmptyPoints.Marker = ReadChartMarker(chart, series, null, context);
							break;
						case "ChartDataLabel":
							chartEmptyPoints.DataLabel = ReadChartDataLabel(chart, series, null, context);
							break;
						case "AxisLabel":
							chartEmptyPoints.AxisLabel = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "CustomProperties":
							chartEmptyPoints.CustomProperties = ReadCustomProperties(context);
							break;
						case "ToolTip":
							chartEmptyPoints.ToolTip = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartEmptyPoints" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartEmptyPoints;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendColumnHeader ReadChartLegendColumnHeader(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendColumnHeader chartLegendColumnHeader = new Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendColumnHeader(chart);
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
							if (localName == "Value")
							{
								chartLegendColumnHeader.Value = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							}
						}
						else
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Chart, hasNoRows: false);
							chartLegendColumnHeader.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: true, context.ErrorContext);
						}
						break;
					}
					case XmlNodeType.EndElement:
						if ("ChartLegendColumnHeader" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartLegendColumnHeader;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartBorderSkin ReadChartBorderSkin(Microsoft.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ChartBorderSkin chartBorderSkin = new Microsoft.ReportingServices.ReportIntermediateFormat.ChartBorderSkin(chart);
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
							if (localName == "ChartBorderSkinType")
							{
								chartBorderSkin.BorderSkinType = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
								if (!chartBorderSkin.BorderSkinType.IsExpression)
								{
									Validator.ValidateChartBorderSkinType(chartBorderSkin.BorderSkinType.StringValue, context.ErrorContext, context, m_reader.LocalName);
								}
							}
						}
						else
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Chart, hasNoRows: false);
							chartBorderSkin.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: true, context.ErrorContext);
						}
						break;
					}
					case XmlNodeType.EndElement:
						if ("ChartBorderSkin" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartBorderSkin;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel ReadGaugePanel(Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem parent, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel = new Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel(GenerateID(), parent);
			gaugePanel.Name = m_reader.GetAttribute("Name");
			if ((context.Location & LocationFlags.InDataRegion) != 0)
			{
				Global.Tracer.Assert(m_nestedDataRegions != null, "(m_nestedDataRegions != null)");
				m_nestedDataRegions.Add(gaugePanel);
			}
			context.Location = (context.Location | LocationFlags.InDataSet | LocationFlags.InDataRegion);
			context.ObjectType = gaugePanel.ObjectType;
			context.ObjectName = gaugePanel.Name;
			RegisterDataRegion(gaugePanel);
			bool flag = true;
			if (!m_reportItemNames.Validate(context.ObjectType, context.ObjectName, m_errorContext))
			{
				flag = false;
			}
			if (m_scopeNames.Validate(isGrouping: false, context.ObjectName, context.ObjectType, context.ObjectName, m_errorContext))
			{
				m_reportScopes.Add(gaugePanel.Name, gaugePanel);
			}
			else
			{
				flag = false;
			}
			if ((context.Location & LocationFlags.InPageSection) != 0)
			{
				m_errorContext.Register(ProcessingErrorCode.rsDataRegionInPageSection, Severity.Error, context.ObjectType, context.ObjectName, null);
				flag = false;
			}
			StyleInformation styleInformation = null;
			IdcRelationship relationship = null;
			if (!m_reader.IsEmptyElement)
			{
				bool flag2 = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (m_reader.LocalName)
						{
						case "SortExpressions":
							gaugePanel.Sorting = ReadSortExpressions(isDataRowSortExpression: true, context);
							break;
						case "Style":
							styleInformation = ReadStyle(context);
							break;
						case "Top":
							gaugePanel.Top = ReadSize();
							break;
						case "Left":
							gaugePanel.Left = ReadSize();
							break;
						case "Height":
							gaugePanel.Height = ReadSize();
							break;
						case "Width":
							gaugePanel.Width = ReadSize();
							break;
						case "ZIndex":
							gaugePanel.ZIndex = m_reader.ReadInteger(context.ObjectType, context.ObjectName, m_reader.LocalName);
							break;
						case "Visibility":
							gaugePanel.Visibility = ReadVisibility(context);
							break;
						case "ToolTip":
							gaugePanel.ToolTip = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "DocumentMapLabel":
							gaugePanel.DocumentMapLabel = ReadDocumentMapLabelExpression(m_reader.LocalName, context);
							break;
						case "Bookmark":
							gaugePanel.Bookmark = ReadBookmarkExpression(m_reader.LocalName, context);
							break;
						case "CustomProperties":
							gaugePanel.CustomProperties = ReadCustomProperties(context);
							break;
						case "DataElementName":
							gaugePanel.DataElementName = m_reader.ReadString();
							break;
						case "DataElementOutput":
							gaugePanel.DataElementOutput = ReadDataElementOutput();
							break;
						case "NoRowsMessage":
							gaugePanel.NoRowsMessage = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "DataSetName":
							gaugePanel.DataSetName = m_reader.ReadString();
							break;
						case "Relationship":
							relationship = ReadRelationship(context);
							break;
						case "PageBreak":
							ReadPageBreak(gaugePanel, context);
							break;
						case "PageName":
							gaugePanel.PageName = ReadPageNameExpression(context);
							break;
						case "Filters":
							gaugePanel.Filters = ReadFilters(Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.DataRegionFilters, context);
							break;
						case "GaugeMember":
						{
							int aLeafNodes = 0;
							gaugePanel.GaugeMember = ReadGaugeMember(gaugePanel, context, 0, ref aLeafNodes);
							break;
						}
						case "LinearGauges":
							gaugePanel.LinearGauges = ReadLinearGauges(gaugePanel, context);
							break;
						case "RadialGauges":
							gaugePanel.RadialGauges = ReadRadialGauges(gaugePanel, context);
							break;
						case "NumericIndicators":
							gaugePanel.NumericIndicators = ReadNumericIndicators(gaugePanel, context);
							break;
						case "StateIndicators":
							gaugePanel.StateIndicators = ReadStateIndicators(gaugePanel, context);
							break;
						case "GaugeImages":
							gaugePanel.GaugeImages = ReadGaugeImages(gaugePanel, context);
							break;
						case "GaugeLabels":
							gaugePanel.GaugeLabels = ReadGaugeLabels(gaugePanel, context);
							break;
						case "AntiAliasing":
							gaugePanel.AntiAliasing = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!gaugePanel.AntiAliasing.IsExpression)
							{
								Validator.ValidateGaugeAntiAliasings(gaugePanel.AntiAliasing.StringValue, m_errorContext, context, m_reader.LocalName);
							}
							break;
						case "AutoLayout":
							gaugePanel.AutoLayout = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "BackFrame":
							gaugePanel.BackFrame = ReadBackFrame(gaugePanel, context);
							break;
						case "ShadowIntensity":
							gaugePanel.ShadowIntensity = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "TextAntiAliasingQuality":
							gaugePanel.TextAntiAliasingQuality = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!gaugePanel.TextAntiAliasingQuality.IsExpression)
							{
								Validator.ValidateTextAntiAliasingQualities(gaugePanel.TextAntiAliasingQuality.StringValue, m_errorContext, context, m_reader.LocalName);
							}
							break;
						case "TopImage":
							gaugePanel.TopImage = ReadTopImage(gaugePanel, context, "TopImage");
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("GaugePanel" == m_reader.LocalName)
						{
							flag2 = true;
						}
						break;
					}
				}
				while (!flag2);
			}
			gaugePanel.DataScopeInfo.SetRelationship(gaugePanel.DataSetName, relationship);
			if (styleInformation != null)
			{
				styleInformation.Filter(StyleOwnerType.GaugePanel, gaugePanel.NoRowsMessage != null);
				gaugePanel.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: false, m_errorContext);
			}
			if (gaugePanel.GaugeMember == null)
			{
				AddStaticGaugeMember(GenerateID(), gaugePanel);
			}
			AddStaticGaugeRowMember(GenerateID(), gaugePanel);
			AddGaugeRow(GenerateID(), GenerateID(), gaugePanel);
			if (gaugePanel.StyleClass != null)
			{
				PublishingValidator.ValidateBorderColorNotTransparent(gaugePanel.ObjectType, gaugePanel.Name, gaugePanel.StyleClass, "BorderColor", m_errorContext);
				PublishingValidator.ValidateBorderColorNotTransparent(gaugePanel.ObjectType, gaugePanel.Name, gaugePanel.StyleClass, "BorderColorBottom", m_errorContext);
				PublishingValidator.ValidateBorderColorNotTransparent(gaugePanel.ObjectType, gaugePanel.Name, gaugePanel.StyleClass, "BorderColorTop", m_errorContext);
				PublishingValidator.ValidateBorderColorNotTransparent(gaugePanel.ObjectType, gaugePanel.Name, gaugePanel.StyleClass, "BorderColorLeft", m_errorContext);
				PublishingValidator.ValidateBorderColorNotTransparent(gaugePanel.ObjectType, gaugePanel.Name, gaugePanel.StyleClass, "BorderColorRight", m_errorContext);
			}
			gaugePanel.Computed = true;
			if (flag)
			{
				m_hasImageStreams = true;
				return gaugePanel;
			}
			return null;
		}

		private void AddStaticGaugeMember(int ID, Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel)
		{
			gaugePanel.GaugeMember = new Microsoft.ReportingServices.ReportIntermediateFormat.GaugeMember(ID, gaugePanel);
			gaugePanel.GaugeMember.Level = 0;
			gaugePanel.GaugeMember.ColSpan = 1;
			gaugePanel.GaugeMember.IsColumn = true;
		}

		private void AddStaticGaugeRowMember(int ID, Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel)
		{
			gaugePanel.GaugeRowMember = new Microsoft.ReportingServices.ReportIntermediateFormat.GaugeMember(ID, gaugePanel);
			gaugePanel.GaugeRowMember.Level = 0;
			gaugePanel.GaugeRowMember.RowSpan = 1;
		}

		private void AddGaugeRow(int rowID, int cellID, Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel)
		{
			gaugePanel.GaugeRow = new Microsoft.ReportingServices.ReportIntermediateFormat.GaugeRow(rowID, gaugePanel);
			gaugePanel.GaugeRow.GaugeCell = new Microsoft.ReportingServices.ReportIntermediateFormat.GaugeCell(cellID, gaugePanel);
			m_aggregateHolderList.Add(gaugePanel.GaugeRow.GaugeCell);
			m_runningValueHolderList.Add(gaugePanel.GaugeRow.GaugeCell);
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.GaugeMember ReadGaugeMember(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context, int level, ref int aLeafNodes)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.GaugeMember gaugeMember = new Microsoft.ReportingServices.ReportIntermediateFormat.GaugeMember(GenerateID(), gaugePanel);
			m_runningValueHolderList.Add(gaugeMember);
			gaugeMember.IsColumn = true;
			gaugeMember.Level = level;
			bool flag = false;
			int aLeafNodes2 = 0;
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
						case "Group":
							gaugeMember.Grouping = ReadGrouping(gaugeMember, context);
							if (gaugeMember.Grouping.PageBreak != null && gaugeMember.Grouping.PageBreak.BreakLocation != 0)
							{
								m_errorContext.Register(ProcessingErrorCode.rsPageBreakOnGaugeGroup, Severity.Warning, context.ObjectType, context.ObjectName, "Group", gaugeMember.Grouping.Name);
							}
							break;
						case "SortExpressions":
							gaugeMember.Sorting = ReadSortExpressions(isDataRowSortExpression: false, context);
							break;
						case "GaugeMember":
							gaugeMember.ChildGaugeMember = ReadGaugeMember(gaugePanel, context, level + 1, ref aLeafNodes2);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("GaugeMember" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			if (gaugeMember.ChildGaugeMember == null)
			{
				aLeafNodes++;
				gaugeMember.ColSpan = 1;
			}
			else
			{
				aLeafNodes += aLeafNodes2;
				gaugeMember.ColSpan = aLeafNodes2;
			}
			ValidateAndProcessMemberGroupAndSort(gaugeMember, context);
			return gaugeMember;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.TopImage ReadTopImage(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context, string elementName)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.TopImage topImage = new Microsoft.ReportingServices.ReportIntermediateFormat.TopImage(gaugePanel);
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
						case "Source":
							topImage.Source = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!topImage.Source.IsExpression)
							{
								Validator.ValidateImageSourceType(topImage.Source.StringValue, m_errorContext, context, m_reader.LocalName);
							}
							break;
						case "Value":
							topImage.Value = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "MIMEType":
							topImage.MIMEType = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "HueColor":
							topImage.HueColor = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "TransparentColor":
							topImage.TransparentColor = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if (elementName == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return topImage;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.PointerImage ReadPointerImage(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.PointerImage pointerImage = new Microsoft.ReportingServices.ReportIntermediateFormat.PointerImage(gaugePanel);
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
						case "Source":
							pointerImage.Source = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!pointerImage.Source.IsExpression)
							{
								Validator.ValidateImageSourceType(pointerImage.Source.StringValue, m_errorContext, context, m_reader.LocalName);
							}
							break;
						case "Value":
							pointerImage.Value = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "MIMEType":
							pointerImage.MIMEType = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "TransparentColor":
							pointerImage.TransparentColor = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "HueColor":
							pointerImage.HueColor = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Transparency":
							pointerImage.Transparency = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "OffsetX":
							pointerImage.OffsetX = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "OffsetY":
							pointerImage.OffsetY = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("PointerImage" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return pointerImage;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.FrameImage ReadFrameImage(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.FrameImage frameImage = new Microsoft.ReportingServices.ReportIntermediateFormat.FrameImage(gaugePanel);
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
						case "Source":
							frameImage.Source = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!frameImage.Source.IsExpression)
							{
								Validator.ValidateImageSourceType(frameImage.Source.StringValue, m_errorContext, context, m_reader.LocalName);
							}
							break;
						case "Value":
							frameImage.Value = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "MIMEType":
							frameImage.MIMEType = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "TransparentColor":
							frameImage.TransparentColor = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "HueColor":
							frameImage.HueColor = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Transparency":
							frameImage.Transparency = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "ClipImage":
							frameImage.ClipImage = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("FrameImage" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return frameImage;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.CapImage ReadCapImage(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.CapImage capImage = new Microsoft.ReportingServices.ReportIntermediateFormat.CapImage(gaugePanel);
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
						case "Source":
							capImage.Source = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Value":
							capImage.Value = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "MIMEType":
							capImage.MIMEType = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "TransparentColor":
							capImage.TransparentColor = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "HueColor":
							capImage.HueColor = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "OffsetX":
							capImage.OffsetX = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "OffsetY":
							capImage.OffsetY = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("CapImage" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return capImage;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.BackFrame ReadBackFrame(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.BackFrame backFrame = new Microsoft.ReportingServices.ReportIntermediateFormat.BackFrame(gaugePanel);
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
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.GaugePanel, hasNoRows: false);
							backFrame.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: true, m_errorContext);
							break;
						}
						case "FrameStyle":
							backFrame.FrameStyle = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!backFrame.FrameStyle.IsExpression)
							{
								Validator.ValidateGaugeFrameStyles(backFrame.FrameStyle.StringValue, m_errorContext, context, m_reader.LocalName);
							}
							break;
						case "FrameShape":
							backFrame.FrameShape = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!backFrame.FrameShape.IsExpression)
							{
								Validator.ValidateGaugeFrameShapes(backFrame.FrameShape.StringValue, m_errorContext, context, m_reader.LocalName);
							}
							break;
						case "FrameWidth":
							backFrame.FrameWidth = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "GlassEffect":
							backFrame.GlassEffect = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!backFrame.GlassEffect.IsExpression)
							{
								Validator.ValidateGaugeGlassEffects(backFrame.GlassEffect.StringValue, m_errorContext, context, m_reader.LocalName);
							}
							break;
						case "FrameBackground":
							backFrame.FrameBackground = ReadFrameBackground(gaugePanel, context);
							break;
						case "FrameImage":
							backFrame.FrameImage = ReadFrameImage(gaugePanel, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("BackFrame" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return backFrame;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.FrameBackground ReadFrameBackground(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.FrameBackground frameBackground = new Microsoft.ReportingServices.ReportIntermediateFormat.FrameBackground(gaugePanel);
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
							styleInformation.Filter(StyleOwnerType.GaugePanel, hasNoRows: false);
							frameBackground.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: true, m_errorContext);
						}
						break;
					}
					case XmlNodeType.EndElement:
						if ("FrameBackground" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return frameBackground;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.CustomLabel ReadCustomLabel(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.CustomLabel customLabel = new Microsoft.ReportingServices.ReportIntermediateFormat.CustomLabel(gaugePanel);
			customLabel.Name = m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "CustomLabel", Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, gaugePanel.Name, customLabel.Name, m_errorContext);
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
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.GaugePanel, hasNoRows: false);
							customLabel.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: true, m_errorContext);
							break;
						}
						case "Text":
							customLabel.Text = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "AllowUpsideDown":
							customLabel.AllowUpsideDown = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "DistanceFromScale":
							customLabel.DistanceFromScale = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "FontAngle":
							customLabel.FontAngle = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Placement":
							customLabel.Placement = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!customLabel.Placement.IsExpression)
							{
								Validator.ValidateGaugeLabelPlacements(customLabel.Placement.StringValue, m_errorContext, context, m_reader.LocalName);
							}
							break;
						case "RotateLabel":
							customLabel.RotateLabel = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "TickMarkStyle":
							customLabel.TickMarkStyle = ReadTickMarkStyle(gaugePanel, context);
							break;
						case "Value":
							customLabel.Value = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Hidden":
							customLabel.Hidden = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "UseFontPercent":
							customLabel.UseFontPercent = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("CustomLabel" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return customLabel;
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.CustomLabel> ReadCustomLabels(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<Microsoft.ReportingServices.ReportIntermediateFormat.CustomLabel> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.CustomLabel>();
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
						if (localName == "CustomLabel")
						{
							list.Add(ReadCustomLabel(gaugePanel, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "CustomLabels")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.TickMarkStyle ReadTickMarkStyle(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.TickMarkStyle tickMarkStyle = new Microsoft.ReportingServices.ReportIntermediateFormat.TickMarkStyle(gaugePanel);
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
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.GaugePanel, hasNoRows: false);
							tickMarkStyle.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: true, m_errorContext);
							break;
						}
						case "DistanceFromScale":
							tickMarkStyle.DistanceFromScale = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Placement":
							tickMarkStyle.Placement = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!tickMarkStyle.Placement.IsExpression)
							{
								Validator.ValidateGaugeLabelPlacements(tickMarkStyle.Placement.StringValue, m_errorContext, context, m_reader.LocalName);
							}
							break;
						case "EnableGradient":
							tickMarkStyle.EnableGradient = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "GradientDensity":
							tickMarkStyle.GradientDensity = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "TickMarkImage":
							tickMarkStyle.TickMarkImage = ReadTopImage(gaugePanel, context, "TickMarkImage");
							break;
						case "Length":
							tickMarkStyle.Length = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Width":
							tickMarkStyle.Width = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Shape":
							tickMarkStyle.Shape = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!tickMarkStyle.Shape.IsExpression)
							{
								Validator.ValidateGaugeTickMarkShapes(tickMarkStyle.Shape.StringValue, m_errorContext, context, m_reader.LocalName);
							}
							break;
						case "Hidden":
							tickMarkStyle.Hidden = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("TickMarkStyle" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return tickMarkStyle;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.GaugeTickMarks ReadGaugeTickMarks(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context, string elementName)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.GaugeTickMarks gaugeTickMarks = new Microsoft.ReportingServices.ReportIntermediateFormat.GaugeTickMarks(gaugePanel);
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
						case "Interval":
							gaugeTickMarks.Interval = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "IntervalOffset":
							gaugeTickMarks.IntervalOffset = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.GaugePanel, hasNoRows: false);
							gaugeTickMarks.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: true, m_errorContext);
							break;
						}
						case "DistanceFromScale":
							gaugeTickMarks.DistanceFromScale = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Placement":
							gaugeTickMarks.Placement = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!gaugeTickMarks.Placement.IsExpression)
							{
								Validator.ValidateGaugeLabelPlacements(gaugeTickMarks.Placement.StringValue, m_errorContext, context, m_reader.LocalName);
							}
							break;
						case "EnableGradient":
							gaugeTickMarks.EnableGradient = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "GradientDensity":
							gaugeTickMarks.GradientDensity = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "TickMarkImage":
							gaugeTickMarks.TickMarkImage = ReadTopImage(gaugePanel, context, "TickMarkImage");
							break;
						case "Length":
							gaugeTickMarks.Length = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Width":
							gaugeTickMarks.Width = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Shape":
							gaugeTickMarks.Shape = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!gaugeTickMarks.Shape.IsExpression)
							{
								Validator.ValidateGaugeTickMarkShapes(gaugeTickMarks.Shape.StringValue, m_errorContext, context, m_reader.LocalName);
							}
							break;
						case "Hidden":
							gaugeTickMarks.Hidden = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if (elementName == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return gaugeTickMarks;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.GaugeImage ReadGaugeImage(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.GaugeImage gaugeImage = new Microsoft.ReportingServices.ReportIntermediateFormat.GaugeImage(gaugePanel, gaugePanel.GenerateActionOwnerID());
			gaugeImage.Name = m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "GaugeImage", Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, gaugePanel.Name, gaugeImage.Name, m_errorContext);
			if (!m_reader.IsEmptyElement)
			{
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
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.GaugePanel, hasNoRows: false);
							gaugeImage.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: true, m_errorContext);
							break;
						}
						case "ActionInfo":
							gaugeImage.Action = ReadActionInfo(context, StyleOwnerType.Chart, out computed);
							break;
						case "Top":
							gaugeImage.Top = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Left":
							gaugeImage.Left = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Height":
							gaugeImage.Height = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Width":
							gaugeImage.Width = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "ZIndex":
							gaugeImage.ZIndex = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "Hidden":
							gaugeImage.Hidden = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "ToolTip":
							gaugeImage.ToolTip = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "ParentItem":
							gaugeImage.ParentItem = m_reader.ReadString();
							break;
						case "Source":
							gaugeImage.Source = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Value":
							gaugeImage.Value = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "TransparentColor":
							gaugeImage.TransparentColor = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("GaugeImage" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return gaugeImage;
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.GaugeImage> ReadGaugeImages(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<Microsoft.ReportingServices.ReportIntermediateFormat.GaugeImage> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.GaugeImage>();
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
						if (localName == "GaugeImage")
						{
							list.Add(ReadGaugeImage(gaugePanel, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "GaugeImages")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.GaugeInputValue ReadGaugeInputValue(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context, string inputValueName)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.GaugeInputValue gaugeInputValue = new Microsoft.ReportingServices.ReportIntermediateFormat.GaugeInputValue(gaugePanel);
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
						case "Value":
							gaugeInputValue.Value = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Formula":
							gaugeInputValue.Formula = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!gaugeInputValue.Formula.IsExpression)
							{
								Validator.ValidateGaugeInputValueFormulas(gaugeInputValue.Formula.StringValue, m_errorContext, context, m_reader.LocalName);
							}
							break;
						case "MinPercent":
							gaugeInputValue.MinPercent = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "MaxPercent":
							gaugeInputValue.MaxPercent = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Multiplier":
							gaugeInputValue.Multiplier = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "AddConstant":
							gaugeInputValue.AddConstant = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "DataElementName":
							gaugeInputValue.DataElementName = m_reader.ReadString();
							break;
						case "DataElementOutput":
							gaugeInputValue.DataElementOutput = ReadDataElementOutput();
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if (inputValueName == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return gaugeInputValue;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.GaugeLabel ReadGaugeLabel(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.GaugeLabel gaugeLabel = new Microsoft.ReportingServices.ReportIntermediateFormat.GaugeLabel(gaugePanel, gaugePanel.GenerateActionOwnerID());
			gaugeLabel.Name = m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "GaugeLabel", Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, gaugePanel.Name, gaugeLabel.Name, m_errorContext);
			if (!m_reader.IsEmptyElement)
			{
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
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.FilterGaugeLabelStyle();
							gaugeLabel.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: true, m_errorContext);
							break;
						}
						case "ActionInfo":
							gaugeLabel.Action = ReadActionInfo(context, StyleOwnerType.Chart, out computed);
							break;
						case "Top":
							gaugeLabel.Top = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Left":
							gaugeLabel.Left = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Height":
							gaugeLabel.Height = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Width":
							gaugeLabel.Width = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "ZIndex":
							gaugeLabel.ZIndex = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "Hidden":
							gaugeLabel.Hidden = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "ToolTip":
							gaugeLabel.ToolTip = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "ParentItem":
							gaugeLabel.ParentItem = m_reader.ReadString();
							break;
						case "Text":
							gaugeLabel.Text = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Angle":
							gaugeLabel.Angle = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "ResizeMode":
							gaugeLabel.ResizeMode = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!gaugeLabel.ResizeMode.IsExpression)
							{
								Validator.ValidateGaugeResizeModes(gaugeLabel.ResizeMode.StringValue, m_errorContext, context, m_reader.LocalName);
							}
							break;
						case "TextShadowOffset":
							gaugeLabel.TextShadowOffset = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "UseFontPercent":
							gaugeLabel.UseFontPercent = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("GaugeLabel" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return gaugeLabel;
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.GaugeLabel> ReadGaugeLabels(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<Microsoft.ReportingServices.ReportIntermediateFormat.GaugeLabel> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.GaugeLabel>();
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
						if (localName == "GaugeLabel")
						{
							list.Add(ReadGaugeLabel(gaugePanel, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "GaugeLabels")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.LinearGauge ReadLinearGauge(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.LinearGauge linearGauge = new Microsoft.ReportingServices.ReportIntermediateFormat.LinearGauge(gaugePanel, gaugePanel.GenerateActionOwnerID());
			linearGauge.Name = m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "LinearGauge", Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, gaugePanel.Name, linearGauge.Name, m_errorContext);
			if (!m_reader.IsEmptyElement)
			{
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
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.GaugePanel, hasNoRows: false);
							linearGauge.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: true, m_errorContext);
							break;
						}
						case "ActionInfo":
							linearGauge.Action = ReadActionInfo(context, StyleOwnerType.Chart, out computed);
							break;
						case "Top":
							linearGauge.Top = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Left":
							linearGauge.Left = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Height":
							linearGauge.Height = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Width":
							linearGauge.Width = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "ZIndex":
							linearGauge.ZIndex = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "Hidden":
							linearGauge.Hidden = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "ToolTip":
							linearGauge.ToolTip = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "ParentItem":
							linearGauge.ParentItem = m_reader.ReadString();
							break;
						case "BackFrame":
							linearGauge.BackFrame = ReadBackFrame(gaugePanel, context);
							break;
						case "ClipContent":
							linearGauge.ClipContent = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "TopImage":
							linearGauge.TopImage = ReadTopImage(gaugePanel, context, "TopImage");
							break;
						case "GaugeScales":
							linearGauge.GaugeScales = ReadLinearScales(gaugePanel, context);
							break;
						case "Orientation":
							linearGauge.Orientation = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!linearGauge.Orientation.IsExpression)
							{
								Validator.ValidateGaugeOrientations(linearGauge.Orientation.StringValue, m_errorContext, context, m_reader.LocalName);
							}
							break;
						case "AspectRatio":
							linearGauge.AspectRatio = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("LinearGauge" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return linearGauge;
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.LinearGauge> ReadLinearGauges(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<Microsoft.ReportingServices.ReportIntermediateFormat.LinearGauge> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.LinearGauge>();
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
						if (localName == "LinearGauge")
						{
							list.Add(ReadLinearGauge(gaugePanel, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "LinearGauges")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.LinearPointer ReadLinearPointer(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.LinearPointer linearPointer = new Microsoft.ReportingServices.ReportIntermediateFormat.LinearPointer(gaugePanel, gaugePanel.GenerateActionOwnerID());
			linearPointer.Name = m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "LinearPointer", Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, gaugePanel.Name, linearPointer.Name, m_errorContext);
			if (!m_reader.IsEmptyElement)
			{
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
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.GaugePanel, hasNoRows: false);
							linearPointer.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: true, m_errorContext);
							break;
						}
						case "ActionInfo":
							linearPointer.Action = ReadActionInfo(context, StyleOwnerType.Chart, out computed);
							break;
						case "GaugeInputValue":
							linearPointer.GaugeInputValue = ReadGaugeInputValue(gaugePanel, context, "GaugeInputValue");
							break;
						case "BarStart":
							linearPointer.BarStart = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!linearPointer.BarStart.IsExpression)
							{
								Validator.ValidateGaugeBarStarts(linearPointer.BarStart.StringValue, m_errorContext, context, m_reader.LocalName);
							}
							break;
						case "DistanceFromScale":
							linearPointer.DistanceFromScale = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "PointerImage":
							linearPointer.PointerImage = ReadPointerImage(gaugePanel, context);
							break;
						case "MarkerLength":
							linearPointer.MarkerLength = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "MarkerStyle":
							linearPointer.MarkerStyle = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!linearPointer.MarkerStyle.IsExpression)
							{
								Validator.ValidateGaugeMarkerStyles(linearPointer.MarkerStyle.StringValue, m_errorContext, context, m_reader.LocalName);
							}
							break;
						case "Placement":
							linearPointer.Placement = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!linearPointer.Placement.IsExpression)
							{
								Validator.ValidateGaugePointerPlacements(linearPointer.Placement.StringValue, m_errorContext, context, m_reader.LocalName);
							}
							break;
						case "SnappingEnabled":
							linearPointer.SnappingEnabled = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "SnappingInterval":
							linearPointer.SnappingInterval = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "ToolTip":
							linearPointer.ToolTip = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Hidden":
							linearPointer.Hidden = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "Width":
							linearPointer.Width = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Type":
							linearPointer.Type = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!linearPointer.Type.IsExpression)
							{
								Validator.ValidateLinearPointerTypes(linearPointer.Type.StringValue, m_errorContext, context, m_reader.LocalName);
							}
							break;
						case "Thermometer":
							linearPointer.Thermometer = ReadThermometer(gaugePanel, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("LinearPointer" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return linearPointer;
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.LinearPointer> ReadLinearPointers(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<Microsoft.ReportingServices.ReportIntermediateFormat.LinearPointer> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.LinearPointer>();
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
						if (localName == "LinearPointer")
						{
							list.Add(ReadLinearPointer(gaugePanel, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "GaugePointers")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.LinearScale ReadLinearScale(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.LinearScale linearScale = new Microsoft.ReportingServices.ReportIntermediateFormat.LinearScale(gaugePanel, gaugePanel.GenerateActionOwnerID());
			linearScale.Name = m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "LinearScale", Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, gaugePanel.Name, linearScale.Name, m_errorContext);
			if (!m_reader.IsEmptyElement)
			{
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
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.GaugePanel, hasNoRows: false);
							linearScale.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: true, m_errorContext);
							break;
						}
						case "ActionInfo":
							linearScale.Action = ReadActionInfo(context, StyleOwnerType.Chart, out computed);
							break;
						case "ScaleRanges":
							linearScale.ScaleRanges = ReadScaleRanges(gaugePanel, context);
							break;
						case "CustomLabels":
							linearScale.CustomLabels = ReadCustomLabels(gaugePanel, context);
							break;
						case "Interval":
							linearScale.Interval = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "IntervalOffset":
							linearScale.IntervalOffset = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Logarithmic":
							linearScale.Logarithmic = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "LogarithmicBase":
							linearScale.LogarithmicBase = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "MaximumValue":
							linearScale.MaximumValue = ReadGaugeInputValue(gaugePanel, context, "MaximumValue");
							break;
						case "MinimumValue":
							linearScale.MinimumValue = ReadGaugeInputValue(gaugePanel, context, "MinimumValue");
							break;
						case "Multiplier":
							linearScale.Multiplier = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Reversed":
							linearScale.Reversed = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "GaugeMajorTickMarks":
							linearScale.GaugeMajorTickMarks = ReadGaugeTickMarks(gaugePanel, context, "GaugeMajorTickMarks");
							break;
						case "GaugeMinorTickMarks":
							linearScale.GaugeMinorTickMarks = ReadGaugeTickMarks(gaugePanel, context, "GaugeMinorTickMarks");
							break;
						case "MaximumPin":
							linearScale.MaximumPin = ReadScalePin(gaugePanel, context, "MaximumPin");
							break;
						case "MinimumPin":
							linearScale.MinimumPin = ReadScalePin(gaugePanel, context, "MinimumPin");
							break;
						case "ScaleLabels":
							linearScale.ScaleLabels = ReadScaleLabels(gaugePanel, context);
							break;
						case "TickMarksOnTop":
							linearScale.TickMarksOnTop = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "ToolTip":
							linearScale.ToolTip = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Hidden":
							linearScale.Hidden = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "Width":
							linearScale.Width = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "GaugePointers":
							linearScale.GaugePointers = ReadLinearPointers(gaugePanel, context);
							break;
						case "StartMargin":
							linearScale.StartMargin = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "EndMargin":
							linearScale.EndMargin = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Position":
							linearScale.Position = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("LinearScale" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return linearScale;
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.LinearScale> ReadLinearScales(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<Microsoft.ReportingServices.ReportIntermediateFormat.LinearScale> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.LinearScale>();
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
						if (localName == "LinearScale")
						{
							list.Add(ReadLinearScale(gaugePanel, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "GaugeScales")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.NumericIndicator ReadNumericIndicator(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator = new Microsoft.ReportingServices.ReportIntermediateFormat.NumericIndicator(gaugePanel, gaugePanel.GenerateActionOwnerID());
			numericIndicator.Name = m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "NumericIndicator", Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, gaugePanel.Name, numericIndicator.Name, m_errorContext);
			if (!m_reader.IsEmptyElement)
			{
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
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.GaugePanel, hasNoRows: false);
							numericIndicator.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: true, m_errorContext);
							break;
						}
						case "ActionInfo":
							numericIndicator.Action = ReadActionInfo(context, StyleOwnerType.Chart, out computed);
							break;
						case "Top":
							numericIndicator.Top = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Left":
							numericIndicator.Left = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Height":
							numericIndicator.Height = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Width":
							numericIndicator.Width = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "ZIndex":
							numericIndicator.ZIndex = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "Hidden":
							numericIndicator.Hidden = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "ToolTip":
							numericIndicator.ToolTip = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "ParentItem":
							numericIndicator.ParentItem = m_reader.ReadString();
							break;
						case "GaugeInputValue":
							numericIndicator.GaugeInputValue = ReadGaugeInputValue(gaugePanel, context, "GaugeInputValue");
							break;
						case "NumericIndicatorRanges":
							numericIndicator.NumericIndicatorRanges = ReadNumericIndicatorRanges(gaugePanel, context);
							break;
						case "DecimalDigitColor":
							numericIndicator.DecimalDigitColor = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "DigitColor":
							numericIndicator.DigitColor = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "UseFontPercent":
							numericIndicator.UseFontPercent = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "DecimalDigits":
							numericIndicator.DecimalDigits = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "Digits":
							numericIndicator.Digits = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "MinimumValue":
							numericIndicator.MinimumValue = ReadGaugeInputValue(gaugePanel, context, "MinimumValue");
							break;
						case "MaximumValue":
							numericIndicator.MaximumValue = ReadGaugeInputValue(gaugePanel, context, "MaximumValue");
							break;
						case "Multiplier":
							numericIndicator.Multiplier = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "NonNumericString":
							numericIndicator.NonNumericString = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "OutOfRangeString":
							numericIndicator.OutOfRangeString = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "ResizeMode":
							numericIndicator.ResizeMode = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!numericIndicator.ResizeMode.IsExpression)
							{
								Validator.ValidateGaugeResizeModes(numericIndicator.ResizeMode.StringValue, m_errorContext, context, m_reader.LocalName);
							}
							break;
						case "ShowDecimalPoint":
							numericIndicator.ShowDecimalPoint = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "ShowLeadingZeros":
							numericIndicator.ShowLeadingZeros = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "IndicatorStyle":
							numericIndicator.IndicatorStyle = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!numericIndicator.IndicatorStyle.IsExpression)
							{
								Validator.ValidateGaugeIndicatorStyles(numericIndicator.IndicatorStyle.StringValue, m_errorContext, context, m_reader.LocalName);
							}
							break;
						case "ShowSign":
							numericIndicator.ShowSign = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!numericIndicator.ShowSign.IsExpression)
							{
								Validator.ValidateGaugeShowSigns(numericIndicator.ShowSign.StringValue, m_errorContext, context, m_reader.LocalName);
							}
							break;
						case "SnappingEnabled":
							numericIndicator.SnappingEnabled = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "SnappingInterval":
							numericIndicator.SnappingInterval = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "LedDimColor":
							numericIndicator.LedDimColor = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "SeparatorWidth":
							numericIndicator.SeparatorWidth = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "SeparatorColor":
							numericIndicator.SeparatorColor = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("NumericIndicator" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return numericIndicator;
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.NumericIndicator> ReadNumericIndicators(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<Microsoft.ReportingServices.ReportIntermediateFormat.NumericIndicator> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.NumericIndicator>();
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
						if (localName == "NumericIndicator")
						{
							list.Add(ReadNumericIndicator(gaugePanel, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "NumericIndicators")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private NumericIndicatorRange ReadNumericIndicatorRange(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			NumericIndicatorRange numericIndicatorRange = new NumericIndicatorRange(gaugePanel);
			numericIndicatorRange.Name = m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "NumericIndicatorRange", Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, gaugePanel.Name, numericIndicatorRange.Name, m_errorContext);
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadNumericIndicatorRangeElement(gaugePanel, numericIndicatorRange, context);
						break;
					case XmlNodeType.EndElement:
						if ("NumericIndicatorRange" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return numericIndicatorRange;
		}

		private void ReadNumericIndicatorRangeElement(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, NumericIndicatorRange numericIndicatorRange, PublishingContextStruct context)
		{
			switch (m_reader.LocalName)
			{
			case "StartValue":
				numericIndicatorRange.StartValue = ReadGaugeInputValue(gaugePanel, context, "StartValue");
				break;
			case "EndValue":
				numericIndicatorRange.EndValue = ReadGaugeInputValue(gaugePanel, context, "EndValue");
				break;
			case "DecimalDigitColor":
				numericIndicatorRange.DecimalDigitColor = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "DigitColor":
				numericIndicatorRange.DigitColor = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			}
		}

		private List<NumericIndicatorRange> ReadNumericIndicatorRanges(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<NumericIndicatorRange> list = new List<NumericIndicatorRange>();
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
						if (localName == "NumericIndicatorRange")
						{
							list.Add(ReadNumericIndicatorRange(gaugePanel, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "NumericIndicatorRanges")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.PinLabel ReadPinLabel(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.PinLabel pinLabel = new Microsoft.ReportingServices.ReportIntermediateFormat.PinLabel(gaugePanel);
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
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.GaugePanel, hasNoRows: false);
							pinLabel.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: true, m_errorContext);
							break;
						}
						case "Text":
							pinLabel.Text = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "AllowUpsideDown":
							pinLabel.AllowUpsideDown = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "DistanceFromScale":
							pinLabel.DistanceFromScale = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "FontAngle":
							pinLabel.FontAngle = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Placement":
							pinLabel.Placement = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!pinLabel.Placement.IsExpression)
							{
								Validator.ValidateGaugeLabelPlacements(pinLabel.Placement.StringValue, m_errorContext, context, m_reader.LocalName);
							}
							break;
						case "RotateLabel":
							pinLabel.RotateLabel = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "UseFontPercent":
							pinLabel.UseFontPercent = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("PinLabel" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return pinLabel;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.PointerCap ReadPointerCap(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.PointerCap pointerCap = new Microsoft.ReportingServices.ReportIntermediateFormat.PointerCap(gaugePanel);
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
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.GaugePanel, hasNoRows: false);
							pointerCap.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: true, m_errorContext);
							break;
						}
						case "CapImage":
							pointerCap.CapImage = ReadCapImage(gaugePanel, context);
							break;
						case "OnTop":
							pointerCap.OnTop = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "Reflection":
							pointerCap.Reflection = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "CapStyle":
							pointerCap.CapStyle = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!pointerCap.CapStyle.IsExpression)
							{
								Validator.ValidateGaugeCapStyles(pointerCap.CapStyle.StringValue, m_errorContext, context, m_reader.LocalName);
							}
							break;
						case "Hidden":
							pointerCap.Hidden = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "Width":
							pointerCap.Width = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("PointerCap" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return pointerCap;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.RadialGauge ReadRadialGauge(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.RadialGauge radialGauge = new Microsoft.ReportingServices.ReportIntermediateFormat.RadialGauge(gaugePanel, gaugePanel.GenerateActionOwnerID());
			radialGauge.Name = m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "RadialGauge", Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, gaugePanel.Name, radialGauge.Name, m_errorContext);
			if (!m_reader.IsEmptyElement)
			{
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
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.GaugePanel, hasNoRows: false);
							radialGauge.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: true, m_errorContext);
							break;
						}
						case "ActionInfo":
							radialGauge.Action = ReadActionInfo(context, StyleOwnerType.Chart, out computed);
							break;
						case "Top":
							radialGauge.Top = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Left":
							radialGauge.Left = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Height":
							radialGauge.Height = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Width":
							radialGauge.Width = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "ZIndex":
							radialGauge.ZIndex = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "Hidden":
							radialGauge.Hidden = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "ToolTip":
							radialGauge.ToolTip = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "ParentItem":
							radialGauge.ParentItem = m_reader.ReadString();
							break;
						case "BackFrame":
							radialGauge.BackFrame = ReadBackFrame(gaugePanel, context);
							break;
						case "ClipContent":
							radialGauge.ClipContent = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "TopImage":
							radialGauge.TopImage = ReadTopImage(gaugePanel, context, "TopImage");
							break;
						case "GaugeScales":
							radialGauge.GaugeScales = ReadRadialScales(gaugePanel, context);
							break;
						case "PivotX":
							radialGauge.PivotX = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "PivotY":
							radialGauge.PivotY = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "AspectRatio":
							radialGauge.AspectRatio = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("RadialGauge" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return radialGauge;
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.RadialGauge> ReadRadialGauges(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<Microsoft.ReportingServices.ReportIntermediateFormat.RadialGauge> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.RadialGauge>();
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
						if (localName == "RadialGauge")
						{
							list.Add(ReadRadialGauge(gaugePanel, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "RadialGauges")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.RadialPointer ReadRadialPointer(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.RadialPointer radialPointer = new Microsoft.ReportingServices.ReportIntermediateFormat.RadialPointer(gaugePanel, gaugePanel.GenerateActionOwnerID());
			radialPointer.Name = m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "RadialPointer", Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, gaugePanel.Name, radialPointer.Name, m_errorContext);
			if (!m_reader.IsEmptyElement)
			{
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
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.GaugePanel, hasNoRows: false);
							radialPointer.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: true, m_errorContext);
							break;
						}
						case "ActionInfo":
							radialPointer.Action = ReadActionInfo(context, StyleOwnerType.Chart, out computed);
							break;
						case "GaugeInputValue":
							radialPointer.GaugeInputValue = ReadGaugeInputValue(gaugePanel, context, "GaugeInputValue");
							break;
						case "BarStart":
							radialPointer.BarStart = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!radialPointer.BarStart.IsExpression)
							{
								Validator.ValidateGaugeBarStarts(radialPointer.BarStart.StringValue, m_errorContext, context, m_reader.LocalName);
							}
							break;
						case "DistanceFromScale":
							radialPointer.DistanceFromScale = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "PointerImage":
							radialPointer.PointerImage = ReadPointerImage(gaugePanel, context);
							break;
						case "MarkerLength":
							radialPointer.MarkerLength = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "MarkerStyle":
							radialPointer.MarkerStyle = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!radialPointer.MarkerStyle.IsExpression)
							{
								Validator.ValidateGaugeMarkerStyles(radialPointer.MarkerStyle.StringValue, m_errorContext, context, m_reader.LocalName);
							}
							break;
						case "Placement":
							radialPointer.Placement = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!radialPointer.Placement.IsExpression)
							{
								Validator.ValidateGaugePointerPlacements(radialPointer.Placement.StringValue, m_errorContext, context, m_reader.LocalName);
							}
							break;
						case "SnappingEnabled":
							radialPointer.SnappingEnabled = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "SnappingInterval":
							radialPointer.SnappingInterval = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "ToolTip":
							radialPointer.ToolTip = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Hidden":
							radialPointer.Hidden = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "Width":
							radialPointer.Width = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Type":
							radialPointer.Type = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!radialPointer.Type.IsExpression)
							{
								Validator.ValidateRadialPointerTypes(radialPointer.Type.StringValue, m_errorContext, context, m_reader.LocalName);
							}
							break;
						case "PointerCap":
							radialPointer.PointerCap = ReadPointerCap(gaugePanel, context);
							break;
						case "NeedleStyle":
							radialPointer.NeedleStyle = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!radialPointer.NeedleStyle.IsExpression)
							{
								Validator.ValidateRadialPointerNeedleStyles(radialPointer.NeedleStyle.StringValue, m_errorContext, context, m_reader.LocalName);
							}
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("RadialPointer" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return radialPointer;
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.RadialPointer> ReadRadialPointers(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<Microsoft.ReportingServices.ReportIntermediateFormat.RadialPointer> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.RadialPointer>();
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
						if (localName == "RadialPointer")
						{
							list.Add(ReadRadialPointer(gaugePanel, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "GaugePointers")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.RadialScale ReadRadialScale(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.RadialScale radialScale = new Microsoft.ReportingServices.ReportIntermediateFormat.RadialScale(gaugePanel, gaugePanel.GenerateActionOwnerID());
			radialScale.Name = m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "RadialScale", Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, gaugePanel.Name, radialScale.Name, m_errorContext);
			if (!m_reader.IsEmptyElement)
			{
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
						case "GaugePointers":
							radialScale.GaugePointers = ReadRadialPointers(gaugePanel, context);
							break;
						case "Radius":
							radialScale.Radius = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "StartAngle":
							radialScale.StartAngle = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "SweepAngle":
							radialScale.SweepAngle = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.GaugePanel, hasNoRows: false);
							radialScale.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: true, m_errorContext);
							break;
						}
						case "ActionInfo":
							radialScale.Action = ReadActionInfo(context, StyleOwnerType.Chart, out computed);
							break;
						case "ScaleRanges":
							radialScale.ScaleRanges = ReadScaleRanges(gaugePanel, context);
							break;
						case "CustomLabels":
							radialScale.CustomLabels = ReadCustomLabels(gaugePanel, context);
							break;
						case "Interval":
							radialScale.Interval = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "IntervalOffset":
							radialScale.IntervalOffset = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Logarithmic":
							radialScale.Logarithmic = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "LogarithmicBase":
							radialScale.LogarithmicBase = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "MaximumValue":
							radialScale.MaximumValue = ReadGaugeInputValue(gaugePanel, context, "MaximumValue");
							break;
						case "MinimumValue":
							radialScale.MinimumValue = ReadGaugeInputValue(gaugePanel, context, "MinimumValue");
							break;
						case "Multiplier":
							radialScale.Multiplier = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Reversed":
							radialScale.Reversed = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "GaugeMajorTickMarks":
							radialScale.GaugeMajorTickMarks = ReadGaugeTickMarks(gaugePanel, context, "GaugeMajorTickMarks");
							break;
						case "GaugeMinorTickMarks":
							radialScale.GaugeMinorTickMarks = ReadGaugeTickMarks(gaugePanel, context, "GaugeMinorTickMarks");
							break;
						case "MaximumPin":
							radialScale.MaximumPin = ReadScalePin(gaugePanel, context, "MaximumPin");
							break;
						case "MinimumPin":
							radialScale.MinimumPin = ReadScalePin(gaugePanel, context, "MinimumPin");
							break;
						case "ScaleLabels":
							radialScale.ScaleLabels = ReadScaleLabels(gaugePanel, context);
							break;
						case "TickMarksOnTop":
							radialScale.TickMarksOnTop = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "ToolTip":
							radialScale.ToolTip = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Hidden":
							radialScale.Hidden = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "Width":
							radialScale.Width = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("RadialScale" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return radialScale;
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.RadialScale> ReadRadialScales(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<Microsoft.ReportingServices.ReportIntermediateFormat.RadialScale> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.RadialScale>();
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
						if (localName == "RadialScale")
						{
							list.Add(ReadRadialScale(gaugePanel, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "GaugeScales")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ScaleLabels ReadScaleLabels(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ScaleLabels scaleLabels = new Microsoft.ReportingServices.ReportIntermediateFormat.ScaleLabels(gaugePanel);
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
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.GaugePanel, hasNoRows: false);
							scaleLabels.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: true, m_errorContext);
							break;
						}
						case "Interval":
							scaleLabels.Interval = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "IntervalOffset":
							scaleLabels.IntervalOffset = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "AllowUpsideDown":
							scaleLabels.AllowUpsideDown = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "DistanceFromScale":
							scaleLabels.DistanceFromScale = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "FontAngle":
							scaleLabels.FontAngle = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Placement":
							scaleLabels.Placement = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!scaleLabels.Placement.IsExpression)
							{
								Validator.ValidateGaugeLabelPlacements(scaleLabels.Placement.StringValue, m_errorContext, context, m_reader.LocalName);
							}
							break;
						case "RotateLabels":
							scaleLabels.RotateLabels = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "ShowEndLabels":
							scaleLabels.ShowEndLabels = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "Hidden":
							scaleLabels.Hidden = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "UseFontPercent":
							scaleLabels.UseFontPercent = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ScaleLabels" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return scaleLabels;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ScalePin ReadScalePin(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context, string elementName)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ScalePin scalePin = new Microsoft.ReportingServices.ReportIntermediateFormat.ScalePin(gaugePanel);
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
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.GaugePanel, hasNoRows: false);
							scalePin.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: true, m_errorContext);
							break;
						}
						case "DistanceFromScale":
							scalePin.DistanceFromScale = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Placement":
							scalePin.Placement = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!scalePin.Placement.IsExpression)
							{
								Validator.ValidateGaugeLabelPlacements(scalePin.Placement.StringValue, m_errorContext, context, m_reader.LocalName);
							}
							break;
						case "EnableGradient":
							scalePin.EnableGradient = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "GradientDensity":
							scalePin.GradientDensity = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "TickMarkImage":
							scalePin.TickMarkImage = ReadTopImage(gaugePanel, context, "TickMarkImage");
							break;
						case "Length":
							scalePin.Length = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Width":
							scalePin.Width = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Shape":
							scalePin.Shape = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!scalePin.Shape.IsExpression)
							{
								Validator.ValidateGaugeTickMarkShapes(scalePin.Shape.StringValue, m_errorContext, context, m_reader.LocalName);
							}
							break;
						case "Hidden":
							scalePin.Hidden = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "Location":
							scalePin.Location = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Enable":
							scalePin.Enable = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "PinLabel":
							scalePin.PinLabel = ReadPinLabel(gaugePanel, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if (elementName == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return scalePin;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ScaleRange ReadScaleRange(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ScaleRange scaleRange = new Microsoft.ReportingServices.ReportIntermediateFormat.ScaleRange(gaugePanel, gaugePanel.GenerateActionOwnerID());
			scaleRange.Name = m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "ScaleRange", Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, gaugePanel.Name, scaleRange.Name, m_errorContext);
			if (!m_reader.IsEmptyElement)
			{
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
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.GaugePanel, hasNoRows: false);
							scaleRange.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: true, m_errorContext);
							break;
						}
						case "ActionInfo":
							scaleRange.Action = ReadActionInfo(context, StyleOwnerType.Chart, out computed);
							break;
						case "DistanceFromScale":
							scaleRange.DistanceFromScale = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "StartValue":
							scaleRange.StartValue = ReadGaugeInputValue(gaugePanel, context, "StartValue");
							break;
						case "EndValue":
							scaleRange.EndValue = ReadGaugeInputValue(gaugePanel, context, "EndValue");
							break;
						case "StartWidth":
							scaleRange.StartWidth = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "EndWidth":
							scaleRange.EndWidth = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "InRangeBarPointerColor":
							scaleRange.InRangeBarPointerColor = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "InRangeLabelColor":
							scaleRange.InRangeLabelColor = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "InRangeTickMarksColor":
							scaleRange.InRangeTickMarksColor = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "BackgroundGradientType":
							scaleRange.BackgroundGradientType = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!scaleRange.BackgroundGradientType.IsExpression)
							{
								Validator.ValidateBackgroundGradientTypes(scaleRange.BackgroundGradientType.StringValue, m_errorContext, context, m_reader.LocalName);
							}
							break;
						case "Placement":
							scaleRange.Placement = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!scaleRange.Placement.IsExpression)
							{
								Validator.ValidateScaleRangePlacements(scaleRange.Placement.StringValue, m_errorContext, context, m_reader.LocalName);
							}
							break;
						case "ToolTip":
							scaleRange.ToolTip = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Hidden":
							scaleRange.Hidden = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ScaleRange" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return scaleRange;
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.ScaleRange> ReadScaleRanges(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<Microsoft.ReportingServices.ReportIntermediateFormat.ScaleRange> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.ScaleRange>();
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
						if (localName == "ScaleRange")
						{
							list.Add(ReadScaleRange(gaugePanel, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "ScaleRanges")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.StateIndicator ReadStateIndicator(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.StateIndicator stateIndicator = new Microsoft.ReportingServices.ReportIntermediateFormat.StateIndicator(gaugePanel, gaugePanel.GenerateActionOwnerID());
			stateIndicator.Name = m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "StateIndicator", Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, gaugePanel.Name, stateIndicator.Name, m_errorContext);
			if (!m_reader.IsEmptyElement)
			{
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
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.GaugePanel, hasNoRows: false);
							stateIndicator.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: true, m_errorContext);
							break;
						}
						case "ActionInfo":
							stateIndicator.Action = ReadActionInfo(context, StyleOwnerType.Chart, out computed);
							break;
						case "Top":
							stateIndicator.Top = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Left":
							stateIndicator.Left = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Height":
							stateIndicator.Height = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Width":
							stateIndicator.Width = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "ZIndex":
							stateIndicator.ZIndex = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "Hidden":
							stateIndicator.Hidden = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "ToolTip":
							stateIndicator.ToolTip = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "ParentItem":
							stateIndicator.ParentItem = m_reader.ReadString();
							break;
						case "GaugeInputValue":
							stateIndicator.GaugeInputValue = ReadGaugeInputValue(gaugePanel, context, "GaugeInputValue");
							break;
						case "TransformationType":
							stateIndicator.TransformationType = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!stateIndicator.TransformationType.IsExpression)
							{
								Validator.ValidateGaugeTransformationType(stateIndicator.TransformationType.StringValue, m_errorContext, context, m_reader.LocalName);
							}
							break;
						case "TransformationScope":
							stateIndicator.TransformationScope = m_reader.ReadString();
							break;
						case "MaximumValue":
							stateIndicator.MaximumValue = ReadGaugeInputValue(gaugePanel, context, "MaximumValue");
							break;
						case "MinimumValue":
							stateIndicator.MinimumValue = ReadGaugeInputValue(gaugePanel, context, "MinimumValue");
							break;
						case "IndicatorStyle":
							stateIndicator.IndicatorStyle = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!stateIndicator.IndicatorStyle.IsExpression)
							{
								Validator.ValidateGaugeStateIndicatorStyles(stateIndicator.IndicatorStyle.StringValue, m_errorContext, context, m_reader.LocalName);
							}
							break;
						case "IndicatorImage":
							stateIndicator.IndicatorImage = ReadIndicatorImage(gaugePanel, context);
							break;
						case "ScaleFactor":
							stateIndicator.ScaleFactor = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "IndicatorStates":
							stateIndicator.IndicatorStates = ReadIndicatorStates(gaugePanel, context);
							break;
						case "ResizeMode":
							stateIndicator.ResizeMode = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!stateIndicator.ResizeMode.IsExpression)
							{
								Validator.ValidateGaugeResizeModes(stateIndicator.ResizeMode.StringValue, m_errorContext, context, m_reader.LocalName);
							}
							break;
						case "Angle":
							stateIndicator.Angle = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "StateDataElementName":
							stateIndicator.StateDataElementName = m_reader.ReadString();
							break;
						case "StateDataElementOutput":
							stateIndicator.StateDataElementOutput = ReadDataElementOutput();
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("StateIndicator" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			InitializeStateIndicatorMinMax(gaugePanel, stateIndicator, context);
			return stateIndicator;
		}

		private void InitializeStateIndicatorMinMax(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, Microsoft.ReportingServices.ReportIntermediateFormat.StateIndicator stateIndicator, PublishingContextStruct context)
		{
			if (stateIndicator.TransformationType == null || Validator.IsStateIndicatorTransformationTypePercent(stateIndicator.TransformationType.StringValue) || stateIndicator.TransformationType.IsExpression)
			{
				string text = GenerateStateIndicatorAutoMinMaxExpression(gaugePanel, stateIndicator, max: false);
				if (text != null)
				{
					stateIndicator.MinimumValue = new AutoGeneratedGaugeInputValue(gaugePanel, stateIndicator.Name);
					stateIndicator.MinimumValue.Value = ReadExpression(text, "Value", null, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.EvaluationMode.Auto, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				}
				text = GenerateStateIndicatorAutoMinMaxExpression(gaugePanel, stateIndicator, max: true);
				if (text != null)
				{
					stateIndicator.MaximumValue = new AutoGeneratedGaugeInputValue(gaugePanel, stateIndicator.Name);
					stateIndicator.MaximumValue.Value = ReadExpression(text, "Value", null, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.EvaluationMode.Auto, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				}
			}
		}

		private string GenerateStateIndicatorAutoMinMaxExpression(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, Microsoft.ReportingServices.ReportIntermediateFormat.StateIndicator stateIndicator, bool max)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.GaugeInputValue gaugeInputValue = max ? stateIndicator.MaximumValue : stateIndicator.MinimumValue;
			if (gaugeInputValue != null && !Validator.CompareWithInvariantCulture(gaugeInputValue.Value.StringValue, "NaN"))
			{
				return null;
			}
			if (string.IsNullOrEmpty(stateIndicator.TransformationScope))
			{
				m_errorContext.Register(ProcessingErrorCode.rsStateIndicatorInvalidTransformationScope, Severity.Error, gaugePanel.ObjectType, gaugePanel.Name, "TransformationScope", stateIndicator.Name);
			}
			if (stateIndicator.GaugeInputValue == null || string.IsNullOrEmpty(stateIndicator.GaugeInputValue.Value.OriginalText))
			{
				return null;
			}
			string text = stateIndicator.GaugeInputValue.Value.OriginalText.Trim();
			if (text.StartsWith("=", StringComparison.Ordinal))
			{
				text = text.Remove(0, 1);
			}
			if (max)
			{
				return "=Max(" + text + ", \"" + stateIndicator.TransformationScope + "\")";
			}
			return "=IIF(Count(" + text + ", \"" + stateIndicator.TransformationScope + "\")=1, 0, Min(" + text + ", \"" + stateIndicator.TransformationScope + "\"))";
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.StateIndicator> ReadStateIndicators(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<Microsoft.ReportingServices.ReportIntermediateFormat.StateIndicator> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.StateIndicator>();
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
						if (localName == "StateIndicator")
						{
							list.Add(ReadStateIndicator(gaugePanel, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "StateIndicators")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.Thermometer ReadThermometer(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.Thermometer thermometer = new Microsoft.ReportingServices.ReportIntermediateFormat.Thermometer(gaugePanel);
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
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.GaugePanel, hasNoRows: false);
							thermometer.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: true, m_errorContext);
							break;
						}
						case "BulbOffset":
							thermometer.BulbOffset = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "BulbSize":
							thermometer.BulbSize = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "ThermometerStyle":
							thermometer.ThermometerStyle = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!thermometer.ThermometerStyle.IsExpression)
							{
								Validator.ValidateGaugeThermometerStyles(thermometer.ThermometerStyle.StringValue, m_errorContext, context, m_reader.LocalName);
							}
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("Thermometer" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return thermometer;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.IndicatorImage ReadIndicatorImage(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.IndicatorImage indicatorImage = new Microsoft.ReportingServices.ReportIntermediateFormat.IndicatorImage(gaugePanel);
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadIndicatorImageElement(gaugePanel, indicatorImage, context);
						break;
					case XmlNodeType.EndElement:
						if ("IndicatorImage" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return indicatorImage;
		}

		private void ReadIndicatorImageElement(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, Microsoft.ReportingServices.ReportIntermediateFormat.IndicatorImage indicatorImage, PublishingContextStruct context)
		{
			switch (m_reader.LocalName)
			{
			case "Source":
				indicatorImage.Source = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				if (!indicatorImage.Source.IsExpression)
				{
					Validator.ValidateImageSourceType(indicatorImage.Source.StringValue, m_errorContext, context, m_reader.LocalName);
				}
				break;
			case "Value":
				indicatorImage.Value = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "MIMEType":
				indicatorImage.MIMEType = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "HueColor":
				indicatorImage.HueColor = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "TransparentColor":
				indicatorImage.TransparentColor = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "Transparency":
				indicatorImage.Transparency = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
				break;
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.IndicatorState ReadIndicatorState(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.IndicatorState indicatorState = new Microsoft.ReportingServices.ReportIntermediateFormat.IndicatorState(gaugePanel);
			indicatorState.Name = m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "IndicatorState", Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, gaugePanel.Name, indicatorState.Name, m_errorContext);
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadIndicatorStateElement(gaugePanel, indicatorState, context);
						break;
					case XmlNodeType.EndElement:
						if ("IndicatorState" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return indicatorState;
		}

		private void ReadIndicatorStateElement(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, Microsoft.ReportingServices.ReportIntermediateFormat.IndicatorState indicatorState, PublishingContextStruct context)
		{
			switch (m_reader.LocalName)
			{
			case "StartValue":
				indicatorState.StartValue = ReadGaugeInputValue(gaugePanel, context, "StartValue");
				break;
			case "EndValue":
				indicatorState.EndValue = ReadGaugeInputValue(gaugePanel, context, "EndValue");
				break;
			case "Color":
				indicatorState.Color = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "ScaleFactor":
				indicatorState.ScaleFactor = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
				break;
			case "IndicatorStyle":
				indicatorState.IndicatorStyle = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				if (!indicatorState.IndicatorStyle.IsExpression)
				{
					Validator.ValidateGaugeStateIndicatorStyles(indicatorState.IndicatorStyle.StringValue, m_errorContext, context, m_reader.LocalName);
				}
				break;
			case "IndicatorImage":
				indicatorState.IndicatorImage = ReadIndicatorImage(gaugePanel, context);
				break;
			}
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.IndicatorState> ReadIndicatorStates(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<Microsoft.ReportingServices.ReportIntermediateFormat.IndicatorState> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.IndicatorState>();
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
						if (localName == "IndicatorState")
						{
							list.Add(ReadIndicatorState(gaugePanel, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "IndicatorStates")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.Map ReadMap(Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem parent, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.Map map = new Microsoft.ReportingServices.ReportIntermediateFormat.Map(GenerateID(), parent);
			map.Name = m_reader.GetAttribute("Name");
			context.ObjectType = map.ObjectType;
			context.ObjectName = map.Name;
			bool flag = true;
			if (!m_reportItemNames.Validate(context.ObjectType, context.ObjectName, m_errorContext))
			{
				flag = false;
			}
			StyleInformation styleInformation = null;
			double maxValue = 914.4;
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
						double validSizeInMM;
						string newSize;
						switch (m_reader.LocalName)
						{
						case "Style":
							styleInformation = ReadStyle(context);
							break;
						case "Top":
							map.Top = ReadSize();
							break;
						case "Left":
							map.Left = ReadSize();
							break;
						case "Height":
							map.Height = ReadSize();
							PublishingValidator.ValidateSize(map.Height, allowNegative: false, 0.0, maxValue, map.ObjectType, map.Name, "Height", m_errorContext, out validSizeInMM, out newSize);
							break;
						case "Width":
							map.Width = ReadSize();
							PublishingValidator.ValidateSize(map.Width, allowNegative: false, 0.0, maxValue, map.ObjectType, map.Name, "Width", m_errorContext, out validSizeInMM, out newSize);
							break;
						case "ZIndex":
							map.ZIndex = m_reader.ReadInteger(Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, map.Name, "ZIndex");
							break;
						case "Visibility":
							map.Visibility = ReadVisibility(context);
							break;
						case "ToolTip":
							map.ToolTip = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "DocumentMapLabel":
							map.DocumentMapLabel = ReadDocumentMapLabelExpression(m_reader.LocalName, context);
							break;
						case "Bookmark":
							map.Bookmark = ReadBookmarkExpression(m_reader.LocalName, context);
							break;
						case "CustomProperties":
							map.CustomProperties = ReadCustomProperties(context);
							break;
						case "DataElementName":
							map.DataElementName = m_reader.ReadString();
							break;
						case "DataElementOutput":
							map.DataElementOutput = ReadDataElementOutput();
							break;
						case "PageBreak":
							ReadPageBreak(map, context);
							break;
						case "PageName":
							map.PageName = ReadPageNameExpression(context);
							break;
						case "MapDataRegions":
							map.MapDataRegions = ReadMapDataRegions(map, context);
							break;
						case "MapViewport":
							map.MapViewport = ReadMapViewport(map, context);
							break;
						case "MapLayers":
							map.MapLayers = ReadMapLayers(map, context);
							break;
						case "MapLegends":
							map.MapLegends = ReadMapLegends(map, context);
							break;
						case "MapTitles":
							map.MapTitles = ReadMapTitles(map, context);
							break;
						case "MapDistanceScale":
							map.MapDistanceScale = ReadMapDistanceScale(map, context);
							break;
						case "MapColorScale":
							map.MapColorScale = ReadMapColorScale(map, context);
							break;
						case "MapBorderSkin":
							map.MapBorderSkin = ReadMapBorderSkin(map, context);
							break;
						case "AntiAliasing":
							map.AntiAliasing = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!map.AntiAliasing.IsExpression)
							{
								Validator.ValidateMapAntiAliasing(map.AntiAliasing.StringValue, m_errorContext, context, m_reader.LocalName);
							}
							break;
						case "TextAntiAliasingQuality":
							map.TextAntiAliasingQuality = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!map.TextAntiAliasingQuality.IsExpression)
							{
								Validator.ValidateMapTextAntiAliasingQuality(map.TextAntiAliasingQuality.StringValue, m_errorContext, context, m_reader.LocalName);
							}
							break;
						case "ShadowIntensity":
							map.ShadowIntensity = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "MaximumSpatialElementCount":
							map.MaximumSpatialElementCount = m_reader.ReadInteger(Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, map.Name, "MaximumSpatialElementCount");
							break;
						case "MaximumTotalPointCount":
							map.MaximumTotalPointCount = m_reader.ReadInteger(Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, map.Name, "MaximumTotalPointCount");
							break;
						case "ActionInfo":
						{
							map.Action = ReadActionInfo(context, StyleOwnerType.Map, out bool _);
							break;
						}
						case "TileLanguage":
							map.TileLanguage = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						}
						break;
					}
					case XmlNodeType.EndElement:
						if ("Map" == m_reader.LocalName)
						{
							flag2 = true;
						}
						break;
					}
				}
				while (!flag2);
			}
			if (styleInformation != null)
			{
				styleInformation.Filter(StyleOwnerType.Map, hasNoRows: false);
				map.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: false, m_errorContext);
			}
			if (map.StyleClass != null)
			{
				PublishingValidator.ValidateBorderColorNotTransparent(map.ObjectType, map.Name, map.StyleClass, "BorderColor", m_errorContext);
				PublishingValidator.ValidateBorderColorNotTransparent(map.ObjectType, map.Name, map.StyleClass, "BorderColorBottom", m_errorContext);
				PublishingValidator.ValidateBorderColorNotTransparent(map.ObjectType, map.Name, map.StyleClass, "BorderColorTop", m_errorContext);
				PublishingValidator.ValidateBorderColorNotTransparent(map.ObjectType, map.Name, map.StyleClass, "BorderColorLeft", m_errorContext);
				PublishingValidator.ValidateBorderColorNotTransparent(map.ObjectType, map.Name, map.StyleClass, "BorderColorRight", m_errorContext);
			}
			ValidateDataRegionReferences(map);
			map.Computed = true;
			if (flag)
			{
				m_hasImageStreams = true;
				return map;
			}
			return null;
		}

		private void ValidateDataRegionReferences(Microsoft.ReportingServices.ReportIntermediateFormat.Map map)
		{
			List<Microsoft.ReportingServices.ReportIntermediateFormat.MapLayer> mapLayers = map.MapLayers;
			List<Microsoft.ReportingServices.ReportIntermediateFormat.MapDataRegion> mapDataRegions = map.MapDataRegions;
			if (mapLayers == null)
			{
				return;
			}
			foreach (Microsoft.ReportingServices.ReportIntermediateFormat.MapLayer item in mapLayers)
			{
				if (item is Microsoft.ReportingServices.ReportIntermediateFormat.MapVectorLayer)
				{
					string mapDataRegionName = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapVectorLayer)item).MapDataRegionName;
					if (mapDataRegionName != null && GetDataRegion(mapDataRegions, mapDataRegionName) == null)
					{
						m_errorContext.Register(ProcessingErrorCode.rsInvalidMapDataRegionName, Severity.Error, map.ObjectType, map.Name, "MapDataRegionName", mapDataRegionName);
						break;
					}
				}
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapDataRegion GetDataRegion(List<Microsoft.ReportingServices.ReportIntermediateFormat.MapDataRegion> dataRegions, string dataRegionName)
		{
			if (dataRegions == null)
			{
				return null;
			}
			foreach (Microsoft.ReportingServices.ReportIntermediateFormat.MapDataRegion dataRegion in dataRegions)
			{
				if (dataRegion.Name == dataRegionName)
				{
					return dataRegion;
				}
			}
			return null;
		}

		private void AddStaticMapMember(int ID, Microsoft.ReportingServices.ReportIntermediateFormat.MapDataRegion mapDataRegion)
		{
			mapDataRegion.MapMember = new Microsoft.ReportingServices.ReportIntermediateFormat.MapMember(ID, mapDataRegion);
			mapDataRegion.MapMember.Level = 0;
			mapDataRegion.MapMember.ColSpan = 1;
			mapDataRegion.MapMember.IsColumn = true;
		}

		private void AddStaticMapRowMember(int ID, Microsoft.ReportingServices.ReportIntermediateFormat.MapDataRegion mapDataRegion)
		{
			mapDataRegion.MapRowMember = new Microsoft.ReportingServices.ReportIntermediateFormat.MapMember(ID, mapDataRegion);
			mapDataRegion.MapRowMember.Level = 0;
			mapDataRegion.MapRowMember.RowSpan = 1;
		}

		private void AddMapRow(int rowID, int cellID, Microsoft.ReportingServices.ReportIntermediateFormat.MapDataRegion mapDataRegion)
		{
			mapDataRegion.MapRow = new MapRow(rowID, mapDataRegion);
			mapDataRegion.MapRow.Cell = new MapCell(cellID, mapDataRegion);
			m_aggregateHolderList.Add(mapDataRegion.MapRow.Cell);
			m_runningValueHolderList.Add(mapDataRegion.MapRow.Cell);
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.MapDataRegion> ReadMapDataRegions(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			List<Microsoft.ReportingServices.ReportIntermediateFormat.MapDataRegion> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.MapDataRegion>();
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
						if (localName == "MapDataRegion")
						{
							Microsoft.ReportingServices.ReportIntermediateFormat.MapDataRegion mapDataRegion = ReadMapDataRegion(map, context);
							if (mapDataRegion != null)
							{
								list.Add(mapDataRegion);
							}
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "MapDataRegions")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapDataRegion ReadMapDataRegion(Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem parent, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapDataRegion mapDataRegion = new Microsoft.ReportingServices.ReportIntermediateFormat.MapDataRegion(GenerateID(), parent);
			mapDataRegion.Name = m_reader.GetAttribute("Name");
			if ((context.Location & LocationFlags.InDataRegion) != 0)
			{
				Global.Tracer.Assert(m_nestedDataRegions != null, "(m_nestedDataRegions != null)");
				m_nestedDataRegions.Add(mapDataRegion);
			}
			context.Location = (context.Location | LocationFlags.InDataSet | LocationFlags.InDataRegion);
			context.ObjectType = mapDataRegion.ObjectType;
			context.ObjectName = mapDataRegion.Name;
			RegisterDataRegion(mapDataRegion);
			bool flag = true;
			if (m_scopeNames.Validate(isGrouping: false, context.ObjectName, context.ObjectType, context.ObjectName, m_errorContext))
			{
				m_reportScopes.Add(mapDataRegion.Name, mapDataRegion);
			}
			else
			{
				flag = false;
			}
			if ((context.Location & LocationFlags.InPageSection) != 0)
			{
				m_errorContext.Register(ProcessingErrorCode.rsDataRegionInPageSection, Severity.Error, context.ObjectType, context.ObjectName, null);
				flag = false;
			}
			IdcRelationship relationship = null;
			if (!m_reader.IsEmptyElement)
			{
				bool flag2 = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (m_reader.LocalName)
						{
						case "DataSetName":
							mapDataRegion.DataSetName = m_reader.ReadString();
							break;
						case "Relationship":
							relationship = ReadRelationship(context);
							break;
						case "Filters":
							mapDataRegion.Filters = ReadFilters(Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.DataRegionFilters, context);
							break;
						case "MapMember":
						{
							int aLeafNodes = 0;
							mapDataRegion.MapMember = ReadMapMember(mapDataRegion, context, 0, ref aLeafNodes);
							break;
						}
						}
						break;
					case XmlNodeType.EndElement:
						if ("MapDataRegion" == m_reader.LocalName)
						{
							flag2 = true;
						}
						break;
					}
				}
				while (!flag2);
			}
			mapDataRegion.DataScopeInfo.SetRelationship(mapDataRegion.DataSetName, relationship);
			if (mapDataRegion.MapMember == null)
			{
				AddStaticMapMember(GenerateID(), mapDataRegion);
			}
			AddStaticMapRowMember(GenerateID(), mapDataRegion);
			AddMapRow(GenerateID(), GenerateID(), mapDataRegion);
			mapDataRegion.Computed = true;
			if (flag)
			{
				m_hasImageStreams = true;
				return mapDataRegion;
			}
			return null;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapMember ReadMapMember(Microsoft.ReportingServices.ReportIntermediateFormat.MapDataRegion mapDataRegion, PublishingContextStruct context, int level, ref int aLeafNodes)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapMember mapMember = new Microsoft.ReportingServices.ReportIntermediateFormat.MapMember(GenerateID(), mapDataRegion);
			m_runningValueHolderList.Add(mapMember);
			mapMember.IsColumn = true;
			mapMember.Level = level;
			bool flag = false;
			int aLeafNodes2 = 0;
			if (!m_reader.IsEmptyElement)
			{
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName = m_reader.LocalName;
						if (!(localName == "Group"))
						{
							if (localName == "MapMember")
							{
								mapMember.ChildMapMember = ReadMapMember(mapDataRegion, context, level + 1, ref aLeafNodes2);
							}
							break;
						}
						mapMember.Grouping = ReadGrouping(mapMember, context);
						if (mapMember.Grouping.PageBreak != null && mapMember.Grouping.PageBreak.BreakLocation != 0)
						{
							m_errorContext.Register(ProcessingErrorCode.rsPageBreakOnMapGroup, Severity.Warning, context.ObjectType, context.ObjectName, "Group", mapMember.Grouping.Name.MarkAsModelInfo());
						}
						if (mapMember.Grouping.DomainScope != null)
						{
							mapMember.Grouping.DomainScope = null;
							m_domainScopeGroups.Remove(mapMember.Grouping);
							m_errorContext.Register(ProcessingErrorCode.rsInvalidGroupingDomainScopeMap, Severity.Error, context.ObjectType, context.ObjectName, "Group", mapMember.Grouping.Name.MarkAsModelInfo(), mapMember.Grouping.DomainScope.MarkAsPrivate());
						}
						break;
					}
					case XmlNodeType.EndElement:
						if ("MapMember" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			if (mapMember.ChildMapMember == null)
			{
				aLeafNodes++;
				mapMember.ColSpan = 1;
			}
			else
			{
				aLeafNodes += aLeafNodes2;
				mapMember.ColSpan = aLeafNodes2;
			}
			return mapMember;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapLocation ReadMapLocation(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapLocation mapLocation = new Microsoft.ReportingServices.ReportIntermediateFormat.MapLocation(map);
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadMapLocationElement(map, mapLocation, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapLocation" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapLocation;
		}

		private void ReadMapLocationElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapLocation mapLocation, PublishingContextStruct context)
		{
			switch (m_reader.LocalName)
			{
			case "Left":
				mapLocation.Left = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
				break;
			case "Top":
				mapLocation.Top = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
				break;
			case "Unit":
				mapLocation.Unit = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				if (!mapLocation.Unit.IsExpression)
				{
					Validator.ValidateUnit(mapLocation.Unit.StringValue, m_errorContext, context, m_reader.LocalName);
				}
				break;
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapSize ReadMapSize(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapSize mapSize = new Microsoft.ReportingServices.ReportIntermediateFormat.MapSize(map);
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadMapSizeElement(map, mapSize, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapSize" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapSize;
		}

		private void ReadMapSizeElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapSize mapSize, PublishingContextStruct context)
		{
			switch (m_reader.LocalName)
			{
			case "Width":
				mapSize.Width = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
				break;
			case "Height":
				mapSize.Height = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
				break;
			case "Unit":
				mapSize.Unit = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				if (!mapSize.Unit.IsExpression)
				{
					Validator.ValidateUnit(mapSize.Unit.StringValue, m_errorContext, context, m_reader.LocalName);
				}
				break;
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapGridLines ReadMapGridLines(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context, string tagName)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapGridLines mapGridLines = new Microsoft.ReportingServices.ReportIntermediateFormat.MapGridLines(map);
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadMapGridLinesElement(map, mapGridLines, context);
						break;
					case XmlNodeType.EndElement:
						if (tagName == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapGridLines;
		}

		private void ReadMapGridLinesElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapGridLines mapGridLines, PublishingContextStruct context)
		{
			switch (m_reader.LocalName)
			{
			case "Style":
				ReadMapStyle(mapGridLines, context);
				break;
			case "Hidden":
				mapGridLines.Hidden = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
				break;
			case "Interval":
				mapGridLines.Interval = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
				break;
			case "ShowLabels":
				mapGridLines.ShowLabels = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
				break;
			case "LabelPosition":
				mapGridLines.LabelPosition = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				if (!mapGridLines.LabelPosition.IsExpression)
				{
					Validator.ValidateLabelPosition(mapGridLines.LabelPosition.StringValue, m_errorContext, context, m_reader.LocalName);
				}
				break;
			}
		}

		private void ReadMapStyle(MapStyleContainer mapStyleContainer, PublishingContextStruct context)
		{
			StyleInformation styleInformation = ReadStyle(context);
			styleInformation.Filter(StyleOwnerType.Map, hasNoRows: false);
			mapStyleContainer.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: true, m_errorContext);
		}

		private void ReadMapTitleStyle(Microsoft.ReportingServices.ReportIntermediateFormat.MapTitle mapTitle, PublishingContextStruct context)
		{
			StyleInformation styleInformation = ReadStyle(context);
			styleInformation.FilterMapTitleStyle();
			mapTitle.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: true, m_errorContext);
		}

		private void ReadMapLegendTitleStyle(Microsoft.ReportingServices.ReportIntermediateFormat.MapLegendTitle legendTitle, PublishingContextStruct context)
		{
			StyleInformation styleInformation = ReadStyle(context);
			styleInformation.FilterMapLegendTitleStyle();
			legendTitle.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, isDynamicImageSubElement: true, m_errorContext);
		}

		private void ReadMapSubItemElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapSubItem mapSubItem, PublishingContextStruct context)
		{
			switch (m_reader.LocalName)
			{
			case "Style":
				ReadMapStyle(mapSubItem, context);
				break;
			case "MapLocation":
				mapSubItem.MapLocation = ReadMapLocation(map, context);
				break;
			case "MapSize":
				mapSubItem.MapSize = ReadMapSize(map, context);
				break;
			case "LeftMargin":
				mapSubItem.LeftMargin = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "RightMargin":
				mapSubItem.RightMargin = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "TopMargin":
				mapSubItem.TopMargin = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "BottomMargin":
				mapSubItem.BottomMargin = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "ZIndex":
				mapSubItem.ZIndex = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
				break;
			}
		}

		private void ReadMapDockableSubItemElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapDockableSubItem mapDockableSubItem, PublishingContextStruct context)
		{
			switch (m_reader.LocalName)
			{
			case "ActionInfo":
			{
				bool computed = false;
				mapDockableSubItem.Action = ReadActionInfo(context, StyleOwnerType.Map, out computed);
				break;
			}
			case "Position":
				mapDockableSubItem.Position = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				if (!mapDockableSubItem.Position.IsExpression)
				{
					Validator.ValidateMapPosition(mapDockableSubItem.Position.StringValue, m_errorContext, context, m_reader.LocalName);
				}
				break;
			case "DockOutsideViewport":
				mapDockableSubItem.DockOutsideViewport = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
				break;
			case "Hidden":
				mapDockableSubItem.Hidden = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
				break;
			case "ToolTip":
				mapDockableSubItem.ToolTip = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			default:
				ReadMapSubItemElement(map, mapDockableSubItem, context);
				break;
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapBindingFieldPair ReadMapBindingFieldPair(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapVectorLayer mapVectorLayer, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapBindingFieldPair mapBindingFieldPair = new Microsoft.ReportingServices.ReportIntermediateFormat.MapBindingFieldPair(map, mapVectorLayer);
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadMapBindingFieldPairElement(map, mapBindingFieldPair, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapBindingFieldPair" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapBindingFieldPair;
		}

		private void ReadMapBindingFieldPairElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapBindingFieldPair mapBindingFieldPair, PublishingContextStruct context)
		{
			string localName = m_reader.LocalName;
			if (!(localName == "FieldName"))
			{
				if (localName == "BindingExpression")
				{
					mapBindingFieldPair.BindingExpression = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				}
			}
			else
			{
				mapBindingFieldPair.FieldName = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
			}
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.MapBindingFieldPair> ReadMapBindingFieldPairs(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapVectorLayer mapVectorLayer, PublishingContextStruct context)
		{
			List<Microsoft.ReportingServices.ReportIntermediateFormat.MapBindingFieldPair> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.MapBindingFieldPair>();
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
						if (localName == "MapBindingFieldPair")
						{
							list.Add(ReadMapBindingFieldPair(map, mapVectorLayer, context));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "MapBindingFieldPairs")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapViewport ReadMapViewport(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapViewport mapViewport = new Microsoft.ReportingServices.ReportIntermediateFormat.MapViewport(map);
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadMapViewportElement(map, mapViewport, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapViewport" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapViewport;
		}

		private void ReadMapViewportElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapViewport mapViewport, PublishingContextStruct context)
		{
			switch (m_reader.LocalName)
			{
			case "MapCoordinateSystem":
				mapViewport.MapCoordinateSystem = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				if (!mapViewport.MapCoordinateSystem.IsExpression)
				{
					Validator.ValidateMapCoordinateSystem(mapViewport.MapCoordinateSystem.StringValue, m_errorContext, context, m_reader.LocalName);
				}
				break;
			case "MapProjection":
				mapViewport.MapProjection = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				if (!mapViewport.MapProjection.IsExpression)
				{
					Validator.ValidateMapProjection(mapViewport.MapProjection.StringValue, m_errorContext, context, m_reader.LocalName);
				}
				break;
			case "ProjectionCenterX":
				mapViewport.ProjectionCenterX = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
				break;
			case "ProjectionCenterY":
				mapViewport.ProjectionCenterY = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
				break;
			case "MapLimits":
				mapViewport.MapLimits = ReadMapLimits(map, context);
				break;
			case "MapCustomView":
			case "MapDataBoundView":
			case "MapElementView":
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.MapView mapView = mapViewport.MapView;
				ReadMapView(map, context, ref mapView, m_reader.LocalName);
				mapViewport.MapView = mapView;
				break;
			}
			case "MaximumZoom":
				mapViewport.MaximumZoom = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
				break;
			case "MinimumZoom":
				mapViewport.MinimumZoom = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
				break;
			case "ContentMargin":
				mapViewport.ContentMargin = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "MapMeridians":
				mapViewport.MapMeridians = ReadMapGridLines(map, context, "MapMeridians");
				break;
			case "MapParallels":
				mapViewport.MapParallels = ReadMapGridLines(map, context, "MapParallels");
				break;
			case "GridUnderContent":
				mapViewport.GridUnderContent = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
				break;
			case "SimplificationResolution":
				mapViewport.SimplificationResolution = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
				break;
			default:
				ReadMapSubItemElement(map, mapViewport, context);
				break;
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapLimits ReadMapLimits(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapLimits mapLimits = new Microsoft.ReportingServices.ReportIntermediateFormat.MapLimits(map);
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadMapLimitsElement(map, mapLimits, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapLimits" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapLimits;
		}

		private void ReadMapLimitsElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapLimits mapLimits, PublishingContextStruct context)
		{
			switch (m_reader.LocalName)
			{
			case "MinimumX":
				mapLimits.MinimumX = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
				break;
			case "MinimumY":
				mapLimits.MinimumY = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
				break;
			case "MaximumX":
				mapLimits.MaximumX = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
				break;
			case "MaximumY":
				mapLimits.MaximumY = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
				break;
			case "LimitToData":
				mapLimits.LimitToData = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
				break;
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapColorScale ReadMapColorScale(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapColorScale mapColorScale = new Microsoft.ReportingServices.ReportIntermediateFormat.MapColorScale(map, map.GenerateActionOwnerID());
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadMapColorScaleElement(map, mapColorScale, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapColorScale" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapColorScale;
		}

		private void ReadMapColorScaleElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapColorScale mapColorScale, PublishingContextStruct context)
		{
			switch (m_reader.LocalName)
			{
			case "MapColorScaleTitle":
				mapColorScale.MapColorScaleTitle = ReadMapColorScaleTitle(map, context);
				break;
			case "TickMarkLength":
				mapColorScale.TickMarkLength = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "ColorBarBorderColor":
				mapColorScale.ColorBarBorderColor = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "LabelInterval":
				mapColorScale.LabelInterval = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
				break;
			case "LabelFormat":
				mapColorScale.LabelFormat = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "LabelPlacement":
				mapColorScale.LabelPlacement = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				if (!mapColorScale.LabelPlacement.IsExpression)
				{
					Validator.ValidateLabelPlacement(mapColorScale.LabelPlacement.StringValue, m_errorContext, context, m_reader.LocalName);
				}
				break;
			case "LabelBehavior":
				mapColorScale.LabelBehavior = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				if (!mapColorScale.LabelBehavior.IsExpression)
				{
					Validator.ValidateLabelBehavior(mapColorScale.LabelBehavior.StringValue, m_errorContext, context, m_reader.LocalName);
				}
				break;
			case "HideEndLabels":
				mapColorScale.HideEndLabels = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
				break;
			case "RangeGapColor":
				mapColorScale.RangeGapColor = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "NoDataText":
				mapColorScale.NoDataText = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			default:
				ReadMapDockableSubItemElement(map, mapColorScale, context);
				break;
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapColorScaleTitle ReadMapColorScaleTitle(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapColorScaleTitle mapColorScaleTitle = new Microsoft.ReportingServices.ReportIntermediateFormat.MapColorScaleTitle(map);
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadMapColorScaleTitleElement(map, mapColorScaleTitle, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapColorScaleTitle" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapColorScaleTitle;
		}

		private void ReadMapColorScaleTitleElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapColorScaleTitle mapColorScaleTitle, PublishingContextStruct context)
		{
			string localName = m_reader.LocalName;
			if (!(localName == "Style"))
			{
				if (localName == "Caption")
				{
					mapColorScaleTitle.Caption = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				}
			}
			else
			{
				ReadMapStyle(mapColorScaleTitle, context);
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapDistanceScale ReadMapDistanceScale(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapDistanceScale mapDistanceScale = new Microsoft.ReportingServices.ReportIntermediateFormat.MapDistanceScale(map, map.GenerateActionOwnerID());
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadMapDistanceScaleElement(map, mapDistanceScale, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapDistanceScale" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapDistanceScale;
		}

		private void ReadMapDistanceScaleElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapDistanceScale mapDistanceScale, PublishingContextStruct context)
		{
			string localName = m_reader.LocalName;
			if (!(localName == "ScaleColor"))
			{
				if (localName == "ScaleBorderColor")
				{
					mapDistanceScale.ScaleBorderColor = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				}
				else
				{
					ReadMapDockableSubItemElement(map, mapDistanceScale, context);
				}
			}
			else
			{
				mapDistanceScale.ScaleColor = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapTitle ReadMapTitle(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapTitle mapTitle = new Microsoft.ReportingServices.ReportIntermediateFormat.MapTitle(map, map.GenerateActionOwnerID());
			mapTitle.Name = m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "MapTitle", Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, map.Name, mapTitle.Name, m_errorContext);
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadMapTitleElement(map, mapTitle, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapTitle" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapTitle;
		}

		private void ReadMapTitleElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapTitle mapTitle, PublishingContextStruct context)
		{
			switch (m_reader.LocalName)
			{
			case "Style":
				ReadMapTitleStyle(mapTitle, context);
				break;
			case "Text":
				mapTitle.Text = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "Angle":
				mapTitle.Angle = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
				break;
			case "TextShadowOffset":
				mapTitle.TextShadowOffset = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			default:
				ReadMapDockableSubItemElement(map, mapTitle, context);
				break;
			}
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.MapTitle> ReadMapTitles(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<Microsoft.ReportingServices.ReportIntermediateFormat.MapTitle> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.MapTitle>();
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
						if (localName == "MapTitle")
						{
							list.Add(ReadMapTitle(map, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "MapTitles")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapLegend ReadMapLegend(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapLegend mapLegend = new Microsoft.ReportingServices.ReportIntermediateFormat.MapLegend(map, map.GenerateActionOwnerID());
			mapLegend.Name = m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "MapLegend", Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, map.Name, mapLegend.Name, m_errorContext);
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadMapLegendElement(map, mapLegend, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapLegend" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapLegend;
		}

		private void ReadMapLegendElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapLegend mapLegend, PublishingContextStruct context)
		{
			switch (m_reader.LocalName)
			{
			case "Layout":
				mapLegend.Layout = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				if (!mapLegend.Layout.IsExpression)
				{
					Validator.ValidateMapLegendLayout(mapLegend.Layout.StringValue, m_errorContext, context, m_reader.LocalName);
				}
				break;
			case "MapLegendTitle":
				mapLegend.MapLegendTitle = ReadMapLegendTitle(map, context);
				break;
			case "AutoFitTextDisabled":
				mapLegend.AutoFitTextDisabled = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
				break;
			case "MinFontSize":
				mapLegend.MinFontSize = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "InterlacedRows":
				mapLegend.InterlacedRows = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
				break;
			case "InterlacedRowsColor":
				mapLegend.InterlacedRowsColor = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "EquallySpacedItems":
				mapLegend.EquallySpacedItems = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
				break;
			case "TextWrapThreshold":
				mapLegend.TextWrapThreshold = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
				break;
			default:
				ReadMapDockableSubItemElement(map, mapLegend, context);
				break;
			}
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.MapLegend> ReadMapLegends(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<Microsoft.ReportingServices.ReportIntermediateFormat.MapLegend> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.MapLegend>();
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
						if (localName == "MapLegend")
						{
							list.Add(ReadMapLegend(map, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "MapLegends")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapLegendTitle ReadMapLegendTitle(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapLegendTitle mapLegendTitle = new Microsoft.ReportingServices.ReportIntermediateFormat.MapLegendTitle(map);
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadMapLegendTitleElement(map, mapLegendTitle, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapLegendTitle" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapLegendTitle;
		}

		private void ReadMapLegendTitleElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapLegendTitle mapLegendTitle, PublishingContextStruct context)
		{
			switch (m_reader.LocalName)
			{
			case "Style":
				ReadMapLegendTitleStyle(mapLegendTitle, context);
				break;
			case "Caption":
				mapLegendTitle.Caption = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "TitleSeparator":
				mapLegendTitle.TitleSeparator = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				if (!mapLegendTitle.TitleSeparator.IsExpression)
				{
					Validator.ValidateMapLegendTitleSeparator(mapLegendTitle.TitleSeparator.StringValue, m_errorContext, context, m_reader.LocalName);
				}
				break;
			case "TitleSeparatorColor":
				mapLegendTitle.TitleSeparatorColor = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			}
		}

		private void ReadMapAppearanceRuleElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapAppearanceRule mapAppearanceRule, PublishingContextStruct context)
		{
			switch (m_reader.LocalName)
			{
			case "DataValue":
				mapAppearanceRule.DataValue = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "DistributionType":
				mapAppearanceRule.DistributionType = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				if (!mapAppearanceRule.DistributionType.IsExpression)
				{
					Validator.ValidateMapRuleDistributionType(mapAppearanceRule.DistributionType.StringValue, m_errorContext, context, m_reader.LocalName);
				}
				break;
			case "BucketCount":
				mapAppearanceRule.BucketCount = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
				break;
			case "StartValue":
				mapAppearanceRule.StartValue = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "EndValue":
				mapAppearanceRule.EndValue = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "MapBuckets":
				mapAppearanceRule.MapBuckets = ReadMapBuckets(map, context);
				break;
			case "LegendName":
				mapAppearanceRule.LegendName = m_reader.ReadString();
				break;
			case "LegendText":
				mapAppearanceRule.LegendText = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "DataElementName":
				mapAppearanceRule.DataElementName = m_reader.ReadString();
				break;
			case "DataElementOutput":
				mapAppearanceRule.DataElementOutput = ReadDataElementOutput();
				break;
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapBucket ReadMapBucket(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapBucket mapBucket = new Microsoft.ReportingServices.ReportIntermediateFormat.MapBucket(map);
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadMapBucketElement(map, mapBucket, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapBucket" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapBucket;
		}

		private void ReadMapBucketElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapBucket mapBucket, PublishingContextStruct context)
		{
			string localName = m_reader.LocalName;
			if (!(localName == "StartValue"))
			{
				if (localName == "EndValue")
				{
					mapBucket.EndValue = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				}
			}
			else
			{
				mapBucket.StartValue = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
			}
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.MapBucket> ReadMapBuckets(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			List<Microsoft.ReportingServices.ReportIntermediateFormat.MapBucket> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.MapBucket>();
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
						if (localName == "MapBucket")
						{
							list.Add(ReadMapBucket(map, context));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "MapBuckets")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapColorPaletteRule ReadMapColorPaletteRule(Microsoft.ReportingServices.ReportIntermediateFormat.MapVectorLayer mapVectorLayer, Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapColorPaletteRule mapColorPaletteRule = new Microsoft.ReportingServices.ReportIntermediateFormat.MapColorPaletteRule(mapVectorLayer, map);
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadMapColorPaletteRuleElement(map, mapColorPaletteRule, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapColorPaletteRule" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapColorPaletteRule;
		}

		private void ReadMapColorPaletteRuleElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapColorPaletteRule mapColorPaletteRule, PublishingContextStruct context)
		{
			string localName = m_reader.LocalName;
			if (localName == "Palette")
			{
				mapColorPaletteRule.Palette = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				if (!mapColorPaletteRule.Palette.IsExpression)
				{
					Validator.ValidateMapPalette(mapColorPaletteRule.Palette.StringValue, m_errorContext, context, m_reader.LocalName);
				}
			}
			else
			{
				ReadMapColorRuleElement(map, mapColorPaletteRule, context);
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapColorRangeRule ReadMapColorRangeRule(Microsoft.ReportingServices.ReportIntermediateFormat.MapVectorLayer mapVectorLayer, Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapColorRangeRule mapColorRangeRule = new Microsoft.ReportingServices.ReportIntermediateFormat.MapColorRangeRule(mapVectorLayer, map);
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadMapColorRangeRuleElement(map, mapColorRangeRule, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapColorRangeRule" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapColorRangeRule;
		}

		private void ReadMapColorRangeRuleElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapColorRangeRule mapColorRangeRule, PublishingContextStruct context)
		{
			switch (m_reader.LocalName)
			{
			case "StartColor":
				mapColorRangeRule.StartColor = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "MiddleColor":
				mapColorRangeRule.MiddleColor = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "EndColor":
				mapColorRangeRule.EndColor = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			default:
				ReadMapColorRuleElement(map, mapColorRangeRule, context);
				break;
			}
		}

		private void ReadMapColorRuleElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapColorRule mapColorRule, PublishingContextStruct context)
		{
			string localName = m_reader.LocalName;
			if (localName == "ShowInColorScale")
			{
				mapColorRule.ShowInColorScale = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
			}
			else
			{
				ReadMapAppearanceRuleElement(map, mapColorRule, context);
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapLineRules ReadMapLineRules(Microsoft.ReportingServices.ReportIntermediateFormat.MapLineLayer mapLineLayer, Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapLineRules mapLineRules = new Microsoft.ReportingServices.ReportIntermediateFormat.MapLineRules(map);
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadMapLineRulesElement(mapLineLayer, map, mapLineRules, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapLineRules" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapLineRules;
		}

		private void ReadMapLineRulesElement(Microsoft.ReportingServices.ReportIntermediateFormat.MapLineLayer mapLineLayer, Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapLineRules mapLineRules, PublishingContextStruct context)
		{
			switch (m_reader.LocalName)
			{
			case "MapSizeRule":
				mapLineRules.MapSizeRule = ReadMapSizeRule(mapLineLayer, map, context);
				break;
			case "MapColorPaletteRule":
			case "MapColorRangeRule":
			case "MapCustomColorRule":
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.MapColorRule colorRule = mapLineRules.MapColorRule;
				ReadMapColorRule(mapLineLayer, map, context, ref colorRule, m_reader.LocalName);
				mapLineRules.MapColorRule = colorRule;
				break;
			}
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygonRules ReadMapPolygonRules(Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygonLayer mapPolygonLayer, Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygonRules mapPolygonRules = new Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygonRules(map);
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadMapPolygonRulesElement(mapPolygonLayer, map, mapPolygonRules, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapPolygonRules" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapPolygonRules;
		}

		private void ReadMapPolygonRulesElement(Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygonLayer mapPolygonLayer, Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygonRules mapPolygonRules, PublishingContextStruct context)
		{
			switch (m_reader.LocalName)
			{
			case "MapColorPaletteRule":
			case "MapColorRangeRule":
			case "MapCustomColorRule":
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.MapColorRule colorRule = mapPolygonRules.MapColorRule;
				ReadMapColorRule(mapPolygonLayer, map, context, ref colorRule, m_reader.LocalName);
				mapPolygonRules.MapColorRule = colorRule;
				break;
			}
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapSizeRule ReadMapSizeRule(Microsoft.ReportingServices.ReportIntermediateFormat.MapVectorLayer mapVectorLayer, Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapSizeRule mapSizeRule = new Microsoft.ReportingServices.ReportIntermediateFormat.MapSizeRule(mapVectorLayer, map);
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadMapSizeRuleElement(map, mapSizeRule, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapSizeRule" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapSizeRule;
		}

		private void ReadMapSizeRuleElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapSizeRule mapSizeRule, PublishingContextStruct context)
		{
			string localName = m_reader.LocalName;
			if (!(localName == "StartSize"))
			{
				if (localName == "EndSize")
				{
					mapSizeRule.EndSize = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				}
				else
				{
					ReadMapAppearanceRuleElement(map, mapSizeRule, context);
				}
			}
			else
			{
				mapSizeRule.StartSize = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapMarkerImage ReadMapMarkerImage(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapMarkerImage mapMarkerImage = new Microsoft.ReportingServices.ReportIntermediateFormat.MapMarkerImage(map);
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadMapMarkerImageElement(map, mapMarkerImage, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapMarkerImage" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapMarkerImage;
		}

		private void ReadMapMarkerImageElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapMarkerImage mapMarkerImage, PublishingContextStruct context)
		{
			switch (m_reader.LocalName)
			{
			case "Source":
				mapMarkerImage.Source = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				if (!mapMarkerImage.Source.IsExpression)
				{
					Validator.ValidateImageSourceType(mapMarkerImage.Source.StringValue, m_errorContext, context, m_reader.LocalName);
				}
				break;
			case "Value":
				mapMarkerImage.Value = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "MIMEType":
				mapMarkerImage.MIMEType = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "TransparentColor":
				mapMarkerImage.TransparentColor = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "ResizeMode":
				mapMarkerImage.ResizeMode = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				if (!mapMarkerImage.ResizeMode.IsExpression)
				{
					Validator.ValidateMapResizeMode(mapMarkerImage.ResizeMode.StringValue, m_errorContext, context, m_reader.LocalName);
				}
				break;
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapMarker ReadMapMarker(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapMarker mapMarker = new Microsoft.ReportingServices.ReportIntermediateFormat.MapMarker(map);
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadMapMarkerElement(map, mapMarker, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapMarker" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapMarker;
		}

		private void ReadMapMarkerElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapMarker mapMarker, PublishingContextStruct context)
		{
			string localName = m_reader.LocalName;
			if (!(localName == "MapMarkerStyle"))
			{
				if (localName == "MapMarkerImage")
				{
					mapMarker.MapMarkerImage = ReadMapMarkerImage(map, context);
				}
				return;
			}
			mapMarker.MapMarkerStyle = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
			if (!mapMarker.MapMarkerStyle.IsExpression)
			{
				Validator.ValidateMapMarkerStyle(mapMarker.MapMarkerStyle.StringValue, m_errorContext, context, m_reader.LocalName);
			}
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.MapMarker> ReadMapMarkers(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			List<Microsoft.ReportingServices.ReportIntermediateFormat.MapMarker> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.MapMarker>();
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
						if (localName == "MapMarker")
						{
							list.Add(ReadMapMarker(map, context));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "MapMarkers")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapMarkerRule ReadMapMarkerRule(Microsoft.ReportingServices.ReportIntermediateFormat.MapVectorLayer mapVectorLayer, Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapMarkerRule mapMarkerRule = new Microsoft.ReportingServices.ReportIntermediateFormat.MapMarkerRule(mapVectorLayer, map);
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadMapMarkerRuleElement(map, mapMarkerRule, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapMarkerRule" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapMarkerRule;
		}

		private void ReadMapMarkerRuleElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapMarkerRule mapMarkerRule, PublishingContextStruct context)
		{
			string localName = m_reader.LocalName;
			if (localName == "MapMarkers")
			{
				mapMarkerRule.MapMarkers = ReadMapMarkers(map, context);
			}
			else
			{
				ReadMapAppearanceRuleElement(map, mapMarkerRule, context);
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapPointRules ReadMapPointRules(Microsoft.ReportingServices.ReportIntermediateFormat.MapVectorLayer mapVectorLayer, Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context, string tagName)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapPointRules mapPointRules = new Microsoft.ReportingServices.ReportIntermediateFormat.MapPointRules(map);
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadMapPointRulesElement(mapVectorLayer, map, mapPointRules, context);
						break;
					case XmlNodeType.EndElement:
						if (tagName == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapPointRules;
		}

		private void ReadMapPointRulesElement(Microsoft.ReportingServices.ReportIntermediateFormat.MapVectorLayer mapVectorLayer, Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapPointRules mapPointRules, PublishingContextStruct context)
		{
			switch (m_reader.LocalName)
			{
			case "MapSizeRule":
				mapPointRules.MapSizeRule = ReadMapSizeRule(mapVectorLayer, map, context);
				break;
			case "MapColorPaletteRule":
			case "MapColorRangeRule":
			case "MapCustomColorRule":
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.MapColorRule colorRule = mapPointRules.MapColorRule;
				ReadMapColorRule(mapVectorLayer, map, context, ref colorRule, m_reader.LocalName);
				mapPointRules.MapColorRule = colorRule;
				break;
			}
			case "MapMarkerRule":
				mapPointRules.MapMarkerRule = ReadMapMarkerRule(mapVectorLayer, map, context);
				break;
			}
		}

		private void ReadMapColorRule(Microsoft.ReportingServices.ReportIntermediateFormat.MapVectorLayer mapVectorLayer, Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context, ref Microsoft.ReportingServices.ReportIntermediateFormat.MapColorRule colorRule, string propertyName)
		{
			if (colorRule != null)
			{
				m_errorContext.Register(ProcessingErrorCode.rsMapPropertyAlreadyDefined, Severity.Error, context.ObjectType, context.ObjectName, propertyName);
				return;
			}
			switch (propertyName)
			{
			case "MapColorPaletteRule":
				colorRule = ReadMapColorPaletteRule(mapVectorLayer, map, context);
				break;
			case "MapColorRangeRule":
				colorRule = ReadMapColorRangeRule(mapVectorLayer, map, context);
				break;
			case "MapCustomColorRule":
				colorRule = ReadMapCustomColorRule(mapVectorLayer, map, context);
				break;
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapCustomColorRule ReadMapCustomColorRule(Microsoft.ReportingServices.ReportIntermediateFormat.MapVectorLayer mapVectorLayer, Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapCustomColorRule mapCustomColorRule = new Microsoft.ReportingServices.ReportIntermediateFormat.MapCustomColorRule(mapVectorLayer, map);
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadMapCustomColorRuleElement(map, mapCustomColorRule, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapCustomColorRule" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapCustomColorRule;
		}

		private void ReadMapCustomColorRuleElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapCustomColorRule mapCustomColorRule, PublishingContextStruct context)
		{
			string localName = m_reader.LocalName;
			if (localName == "MapCustomColors")
			{
				mapCustomColorRule.MapCustomColors = ReadMapCustomColors(map, context);
			}
			else
			{
				ReadMapColorRuleElement(map, mapCustomColorRule, context);
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapCustomColor ReadMapCustomColor(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapCustomColor mapCustomColor = new Microsoft.ReportingServices.ReportIntermediateFormat.MapCustomColor(map);
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadMapCustomColorElement(map, mapCustomColor, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapCustomColor" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapCustomColor;
		}

		private void ReadMapCustomColorElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapCustomColor mapCustomColor, PublishingContextStruct context)
		{
			string localName = m_reader.LocalName;
			if (localName == "Color")
			{
				mapCustomColor.Color = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
			}
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.MapCustomColor> ReadMapCustomColors(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			List<Microsoft.ReportingServices.ReportIntermediateFormat.MapCustomColor> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.MapCustomColor>();
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
						if (localName == "MapCustomColor")
						{
							Microsoft.ReportingServices.ReportIntermediateFormat.MapCustomColor mapCustomColor = new Microsoft.ReportingServices.ReportIntermediateFormat.MapCustomColor(map);
							mapCustomColor.Color = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							list.Add(mapCustomColor);
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "MapCustomColors")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapLineTemplate ReadMapLineTemplate(Microsoft.ReportingServices.ReportIntermediateFormat.MapLineLayer mapLineLayer, Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapLineTemplate mapLineTemplate = new Microsoft.ReportingServices.ReportIntermediateFormat.MapLineTemplate(mapLineLayer, map, map.GenerateActionOwnerID());
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadMapLineTemplateElement(map, mapLineTemplate, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapLineTemplate" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapLineTemplate;
		}

		private void ReadMapLineTemplateElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapLineTemplate mapLineTemplate, PublishingContextStruct context)
		{
			string localName = m_reader.LocalName;
			if (!(localName == "Width"))
			{
				if (localName == "LabelPlacement")
				{
					mapLineTemplate.LabelPlacement = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
					if (!mapLineTemplate.LabelPlacement.IsExpression)
					{
						Validator.ValidateMapLineLabelPlacement(mapLineTemplate.LabelPlacement.StringValue, m_errorContext, context, m_reader.LocalName);
					}
				}
				else
				{
					ReadMapSpatialElementTemplateElement(map, mapLineTemplate, context);
				}
			}
			else
			{
				mapLineTemplate.Width = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygonTemplate ReadMapPolygonTemplate(Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygonLayer mapPolygonLayer, Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygonTemplate mapPolygonTemplate = new Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygonTemplate(mapPolygonLayer, map, map.GenerateActionOwnerID());
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadMapPolygonTemplateElement(map, mapPolygonTemplate, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapPolygonTemplate" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapPolygonTemplate;
		}

		private void ReadMapPolygonTemplateElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygonTemplate mapPolygonTemplate, PublishingContextStruct context)
		{
			switch (m_reader.LocalName)
			{
			case "ScaleFactor":
				mapPolygonTemplate.ScaleFactor = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
				break;
			case "CenterPointOffsetX":
				mapPolygonTemplate.CenterPointOffsetX = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
				break;
			case "CenterPointOffsetY":
				mapPolygonTemplate.CenterPointOffsetY = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
				break;
			case "ShowLabel":
				mapPolygonTemplate.ShowLabel = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				if (!mapPolygonTemplate.ShowLabel.IsExpression)
				{
					Validator.ValidateMapAutoBool(mapPolygonTemplate.ShowLabel.StringValue, m_errorContext, context, m_reader.LocalName);
				}
				break;
			case "LabelPlacement":
				mapPolygonTemplate.LabelPlacement = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				if (!mapPolygonTemplate.LabelPlacement.IsExpression)
				{
					Validator.ValidateMapPolygonLabelPlacement(mapPolygonTemplate.LabelPlacement.StringValue, m_errorContext, context, m_reader.LocalName);
				}
				break;
			default:
				ReadMapSpatialElementTemplateElement(map, mapPolygonTemplate, context);
				break;
			}
		}

		private void ReadMapSpatialElementTemplateElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapSpatialElementTemplate mapSpatialElementTemplate, PublishingContextStruct context)
		{
			switch (m_reader.LocalName)
			{
			case "Style":
				ReadMapStyle(mapSpatialElementTemplate, context);
				break;
			case "ActionInfo":
			{
				bool computed = false;
				mapSpatialElementTemplate.Action = ReadActionInfo(context, StyleOwnerType.Map, out computed);
				break;
			}
			case "Hidden":
				mapSpatialElementTemplate.Hidden = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
				break;
			case "OffsetX":
				mapSpatialElementTemplate.OffsetX = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
				break;
			case "OffsetY":
				mapSpatialElementTemplate.OffsetY = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
				break;
			case "Label":
				mapSpatialElementTemplate.Label = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "ToolTip":
				mapSpatialElementTemplate.ToolTip = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "DataElementName":
				mapSpatialElementTemplate.DataElementName = m_reader.ReadString();
				break;
			case "DataElementOutput":
				mapSpatialElementTemplate.DataElementOutput = ReadDataElementOutput();
				break;
			case "DataElementLabel":
				mapSpatialElementTemplate.DataElementLabel = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapMarkerTemplate ReadMapMarkerTemplate(Microsoft.ReportingServices.ReportIntermediateFormat.MapVectorLayer mapVectorLayer, Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapMarkerTemplate mapMarkerTemplate = new Microsoft.ReportingServices.ReportIntermediateFormat.MapMarkerTemplate(mapVectorLayer, map, map.GenerateActionOwnerID());
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadMapMarkerTemplateElement(map, mapMarkerTemplate, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapMarkerTemplate" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapMarkerTemplate;
		}

		private void ReadMapMarkerTemplateElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapMarkerTemplate mapMarkerTemplate, PublishingContextStruct context)
		{
			string localName = m_reader.LocalName;
			if (localName == "MapMarker")
			{
				mapMarkerTemplate.MapMarker = ReadMapMarker(map, context);
			}
			else
			{
				ReadMapPointTemplateElement(map, mapMarkerTemplate, context);
			}
		}

		private void ReadMapPointTemplateElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapPointTemplate mapPointTemplate, PublishingContextStruct context)
		{
			string localName = m_reader.LocalName;
			if (!(localName == "Size"))
			{
				if (localName == "LabelPlacement")
				{
					mapPointTemplate.LabelPlacement = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
					if (!mapPointTemplate.LabelPlacement.IsExpression)
					{
						Validator.ValidateMapPointLabelPlacement(mapPointTemplate.LabelPlacement.StringValue, m_errorContext, context, m_reader.LocalName);
					}
				}
				else
				{
					ReadMapSpatialElementTemplateElement(map, mapPointTemplate, context);
				}
			}
			else
			{
				mapPointTemplate.Size = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				PublishingValidator.ValidateSize(mapPointTemplate.Size, Validator.NormalMin, Validator.NormalMax, map.ObjectType, map.Name, "Size", m_errorContext);
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapField ReadMapField(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapField mapField = new Microsoft.ReportingServices.ReportIntermediateFormat.MapField(map);
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadMapFieldElement(map, mapField, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapField" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapField;
		}

		private void ReadMapFieldElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapField mapField, PublishingContextStruct context)
		{
			string localName = m_reader.LocalName;
			if (!(localName == "Name"))
			{
				if (localName == "Value")
				{
					mapField.Value = m_reader.ReadString();
				}
			}
			else
			{
				mapField.Name = m_reader.ReadString();
			}
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.MapField> ReadMapFields(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			List<Microsoft.ReportingServices.ReportIntermediateFormat.MapField> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.MapField>();
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
						if (localName == "MapField")
						{
							list.Add(ReadMapField(map, context));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "MapFields")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapLine ReadMapLine(Microsoft.ReportingServices.ReportIntermediateFormat.MapLineLayer mapLineLayer, Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapLine mapLine = new Microsoft.ReportingServices.ReportIntermediateFormat.MapLine(mapLineLayer, map);
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadMapLineElement(mapLineLayer, map, mapLine, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapLine" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapLine;
		}

		private void ReadMapLineElement(Microsoft.ReportingServices.ReportIntermediateFormat.MapLineLayer mapLineLayer, Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapLine mapLine, PublishingContextStruct context)
		{
			string localName = m_reader.LocalName;
			if (!(localName == "UseCustomLineTemplate"))
			{
				if (localName == "MapLineTemplate")
				{
					mapLine.MapLineTemplate = ReadMapLineTemplate(mapLineLayer, map, context);
				}
				else
				{
					ReadMapSpatialElementElement(map, mapLine, context);
				}
			}
			else
			{
				mapLine.UseCustomLineTemplate = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
			}
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.MapLine> ReadMapLines(Microsoft.ReportingServices.ReportIntermediateFormat.MapLineLayer mapLineLayer, Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			List<Microsoft.ReportingServices.ReportIntermediateFormat.MapLine> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.MapLine>();
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
						if (localName == "MapLine")
						{
							list.Add(ReadMapLine(mapLineLayer, map, context));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "MapLines")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygon ReadMapPolygon(Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygonLayer mapPolygonLayer, Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygon mapPolygon = new Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygon(mapPolygonLayer, map);
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadMapPolygonElement(mapPolygonLayer, map, mapPolygon, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapPolygon" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapPolygon;
		}

		private void ReadMapPolygonElement(Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygonLayer mapPolygonLayer, Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygon mapPolygon, PublishingContextStruct context)
		{
			switch (m_reader.LocalName)
			{
			case "UseCustomPolygonTemplate":
				mapPolygon.UseCustomPolygonTemplate = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
				break;
			case "MapPolygonTemplate":
				mapPolygon.MapPolygonTemplate = ReadMapPolygonTemplate(mapPolygonLayer, map, context);
				break;
			case "UseCustomCenterPointTemplate":
				mapPolygon.UseCustomCenterPointTemplate = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
				break;
			case "MapMarkerTemplate":
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.MapPointTemplate symbolTemplate = mapPolygon.MapCenterPointTemplate;
				ReadMapPointTemplate(mapPolygonLayer, map, context, ref symbolTemplate, m_reader.LocalName);
				mapPolygon.MapCenterPointTemplate = symbolTemplate;
				break;
			}
			default:
				ReadMapSpatialElementElement(map, mapPolygon, context);
				break;
			}
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygon> ReadMapPolygons(Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygonLayer mapPolygonLayer, Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			List<Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygon> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygon>();
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
						if (localName == "MapPolygon")
						{
							list.Add(ReadMapPolygon(mapPolygonLayer, map, context));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "MapPolygons")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private void ReadMapSpatialElementElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapSpatialElement mapSpatialElement, PublishingContextStruct context)
		{
			string localName = m_reader.LocalName;
			if (!(localName == "VectorData"))
			{
				if (localName == "MapFields")
				{
					mapSpatialElement.MapFields = ReadMapFields(map, context);
				}
			}
			else
			{
				mapSpatialElement.VectorData = m_reader.ReadString();
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapPoint ReadMapPoint(Microsoft.ReportingServices.ReportIntermediateFormat.MapPointLayer mapPointLayer, Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapPoint mapPoint = new Microsoft.ReportingServices.ReportIntermediateFormat.MapPoint(mapPointLayer, map);
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadMapPointElement(mapPointLayer, map, mapPoint, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapPoint" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapPoint;
		}

		private void ReadMapPointElement(Microsoft.ReportingServices.ReportIntermediateFormat.MapPointLayer mapPointLayer, Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapPoint mapPoint, PublishingContextStruct context)
		{
			string localName = m_reader.LocalName;
			if (!(localName == "UseCustomPointTemplate"))
			{
				if (localName == "MapMarkerTemplate")
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.MapPointTemplate symbolTemplate = mapPoint.MapPointTemplate;
					ReadMapPointTemplate(mapPointLayer, map, context, ref symbolTemplate, m_reader.LocalName);
					mapPoint.MapPointTemplate = symbolTemplate;
				}
				else
				{
					ReadMapSpatialElementElement(map, mapPoint, context);
				}
			}
			else
			{
				mapPoint.UseCustomPointTemplate = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
			}
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.MapPoint> ReadMapPoints(Microsoft.ReportingServices.ReportIntermediateFormat.MapPointLayer mapPointLayer, Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			List<Microsoft.ReportingServices.ReportIntermediateFormat.MapPoint> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.MapPoint>();
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
						if (localName == "MapPoint")
						{
							list.Add(ReadMapPoint(mapPointLayer, map, context));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "MapPoints")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private void ReadMapPointTemplate(Microsoft.ReportingServices.ReportIntermediateFormat.MapVectorLayer mapVectorLayer, Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context, ref Microsoft.ReportingServices.ReportIntermediateFormat.MapPointTemplate symbolTemplate, string propertyName)
		{
			if (symbolTemplate != null)
			{
				m_errorContext.Register(ProcessingErrorCode.rsMapPropertyAlreadyDefined, Severity.Error, context.ObjectType, context.ObjectName, propertyName);
			}
			else if (propertyName == "MapMarkerTemplate")
			{
				symbolTemplate = ReadMapMarkerTemplate(mapVectorLayer, map, context);
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapFieldDefinition ReadMapFieldDefinition(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapFieldDefinition mapFieldDefinition = new Microsoft.ReportingServices.ReportIntermediateFormat.MapFieldDefinition(map);
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadMapFieldDefinitionElement(map, mapFieldDefinition, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapFieldDefinition" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapFieldDefinition;
		}

		private void ReadMapFieldDefinitionElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapFieldDefinition mapFieldDefinition, PublishingContextStruct context)
		{
			string localName = m_reader.LocalName;
			if (!(localName == "Name"))
			{
				if (localName == "DataType")
				{
					mapFieldDefinition.DataType = ReadDataType();
				}
			}
			else
			{
				mapFieldDefinition.Name = m_reader.ReadString();
			}
		}

		private MapDataType ReadDataType()
		{
			return (MapDataType)Enum.Parse(typeof(MapDataType), m_reader.ReadString(), ignoreCase: false);
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.MapFieldDefinition> ReadMapFieldDefinitions(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			List<Microsoft.ReportingServices.ReportIntermediateFormat.MapFieldDefinition> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.MapFieldDefinition>();
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
						if (localName == "MapFieldDefinition")
						{
							list.Add(ReadMapFieldDefinition(map, context));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "MapFieldDefinitions")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private void ReadMapLayerElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapLayer mapLayer, PublishingContextStruct context)
		{
			switch (m_reader.LocalName)
			{
			case "VisibilityMode":
				mapLayer.VisibilityMode = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				if (!mapLayer.VisibilityMode.IsExpression)
				{
					Validator.ValidateMapVisibilityMode(mapLayer.VisibilityMode.StringValue, m_errorContext, context, m_reader.LocalName);
				}
				break;
			case "MinimumZoom":
				mapLayer.MinimumZoom = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
				break;
			case "MaximumZoom":
				mapLayer.MaximumZoom = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
				break;
			case "Transparency":
				mapLayer.Transparency = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
				break;
			}
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.MapLayer> ReadMapLayers(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<Microsoft.ReportingServices.ReportIntermediateFormat.MapLayer> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.MapLayer>();
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
						case "MapTileLayer":
							list.Add(ReadMapTileLayer(map, context, nameValidator));
							break;
						case "MapPolygonLayer":
							list.Add(ReadMapPolygonLayer(map, context, nameValidator));
							break;
						case "MapPointLayer":
							list.Add(ReadMapPointLayer(map, context, nameValidator));
							break;
						case "MapLineLayer":
							list.Add(ReadMapLineLayer(map, context, nameValidator));
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "MapLayers")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapLineLayer ReadMapLineLayer(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapLineLayer mapLineLayer = new Microsoft.ReportingServices.ReportIntermediateFormat.MapLineLayer(GenerateID(), map);
			mapLineLayer.Name = m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "MapLayer", Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, map.Name, mapLineLayer.Name, m_errorContext);
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadMapLineLayerElement(map, mapLineLayer, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapLineLayer" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			mapLineLayer.Validate(m_errorContext);
			return mapLineLayer;
		}

		private void ReadMapLineLayerElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapLineLayer mapLineLayer, PublishingContextStruct context)
		{
			switch (m_reader.LocalName)
			{
			case "MapLineTemplate":
				mapLineLayer.MapLineTemplate = ReadMapLineTemplate(mapLineLayer, map, context);
				break;
			case "MapLineRules":
				mapLineLayer.MapLineRules = ReadMapLineRules(mapLineLayer, map, context);
				break;
			case "MapLines":
				mapLineLayer.MapLines = ReadMapLines(mapLineLayer, map, context);
				break;
			default:
				ReadMapVectorLayerElement(map, mapLineLayer, context);
				break;
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapShapefile ReadMapShapefile(Microsoft.ReportingServices.ReportIntermediateFormat.MapVectorLayer mapVectorLayer, Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapShapefile mapShapefile = new Microsoft.ReportingServices.ReportIntermediateFormat.MapShapefile(mapVectorLayer, map);
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadMapShapefileElement(map, mapShapefile, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapShapefile" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapShapefile;
		}

		private void ReadMapShapefileElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapShapefile mapShapefile, PublishingContextStruct context)
		{
			string localName = m_reader.LocalName;
			if (!(localName == "Source"))
			{
				if (localName == "MapFieldNames")
				{
					mapShapefile.MapFieldNames = ReadMapFieldNames(map, context);
				}
			}
			else
			{
				mapShapefile.Source = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygonLayer ReadMapPolygonLayer(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygonLayer mapPolygonLayer = new Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygonLayer(GenerateID(), map);
			mapPolygonLayer.Name = m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "MapLayer", Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, map.Name, mapPolygonLayer.Name, m_errorContext);
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadMapPolygonLayerElement(map, mapPolygonLayer, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapPolygonLayer" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			mapPolygonLayer.Validate(m_errorContext);
			return mapPolygonLayer;
		}

		private void ReadMapPolygonLayerElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygonLayer mapPolygonLayer, PublishingContextStruct context)
		{
			switch (m_reader.LocalName)
			{
			case "MapPolygonTemplate":
				mapPolygonLayer.MapPolygonTemplate = ReadMapPolygonTemplate(mapPolygonLayer, map, context);
				break;
			case "MapPolygonRules":
				mapPolygonLayer.MapPolygonRules = ReadMapPolygonRules(mapPolygonLayer, map, context);
				break;
			case "MapMarkerTemplate":
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.MapPointTemplate symbolTemplate = mapPolygonLayer.MapCenterPointTemplate;
				ReadMapPointTemplate(mapPolygonLayer, map, context, ref symbolTemplate, m_reader.LocalName);
				mapPolygonLayer.MapCenterPointTemplate = symbolTemplate;
				break;
			}
			case "MapCenterPointRules":
				mapPolygonLayer.MapCenterPointRules = ReadMapPointRules(mapPolygonLayer, map, context, "MapCenterPointRules");
				break;
			case "MapPolygons":
				mapPolygonLayer.MapPolygons = ReadMapPolygons(mapPolygonLayer, map, context);
				break;
			default:
				ReadMapVectorLayerElement(map, mapPolygonLayer, context);
				break;
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapSpatialDataRegion ReadMapSpatialDataRegion(Microsoft.ReportingServices.ReportIntermediateFormat.MapVectorLayer mapVectorLayer, Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapSpatialDataRegion mapSpatialDataRegion = new Microsoft.ReportingServices.ReportIntermediateFormat.MapSpatialDataRegion(mapVectorLayer, map);
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadMapSpatialDataRegionElement(map, mapSpatialDataRegion, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapSpatialDataRegion" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapSpatialDataRegion;
		}

		private void ReadMapSpatialDataRegionElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapSpatialDataRegion mapSpatialDataRegion, PublishingContextStruct context)
		{
			string localName = m_reader.LocalName;
			if (localName == "VectorData")
			{
				mapSpatialDataRegion.VectorData = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapSpatialDataSet ReadMapSpatialDataSet(Microsoft.ReportingServices.ReportIntermediateFormat.MapVectorLayer mapVectorLayer, Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapSpatialDataSet mapSpatialDataSet = new Microsoft.ReportingServices.ReportIntermediateFormat.MapSpatialDataSet(mapVectorLayer, map);
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadMapSpatialDataSetElement(map, mapSpatialDataSet, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapSpatialDataSet" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapSpatialDataSet;
		}

		private void ReadMapSpatialDataSetElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapSpatialDataSet mapSpatialDataSet, PublishingContextStruct context)
		{
			switch (m_reader.LocalName)
			{
			case "DataSetName":
				mapSpatialDataSet.DataSetName = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "SpatialField":
				mapSpatialDataSet.SpatialField = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "MapFieldNames":
				mapSpatialDataSet.MapFieldNames = ReadMapFieldNames(map, context);
				break;
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapPointLayer ReadMapPointLayer(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapPointLayer mapPointLayer = new Microsoft.ReportingServices.ReportIntermediateFormat.MapPointLayer(GenerateID(), map);
			mapPointLayer.Name = m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "MapLayer", Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, map.Name, mapPointLayer.Name, m_errorContext);
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadMapPointLayerElement(map, mapPointLayer, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapPointLayer" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			mapPointLayer.Validate(m_errorContext);
			return mapPointLayer;
		}

		private void ReadMapPointLayerElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapPointLayer mapPointLayer, PublishingContextStruct context)
		{
			switch (m_reader.LocalName)
			{
			case "MapMarkerTemplate":
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.MapPointTemplate symbolTemplate = mapPointLayer.MapPointTemplate;
				ReadMapPointTemplate(mapPointLayer, map, context, ref symbolTemplate, m_reader.LocalName);
				mapPointLayer.MapPointTemplate = symbolTemplate;
				break;
			}
			case "MapPointRules":
				mapPointLayer.MapPointRules = ReadMapPointRules(mapPointLayer, map, context, "MapPointRules");
				break;
			case "MapPoints":
				mapPointLayer.MapPoints = ReadMapPoints(mapPointLayer, map, context);
				break;
			default:
				ReadMapVectorLayerElement(map, mapPointLayer, context);
				break;
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapTile ReadMapTile(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapTile mapTile = new Microsoft.ReportingServices.ReportIntermediateFormat.MapTile(map);
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadMapTileElement(map, mapTile, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapTile" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapTile;
		}

		private void ReadMapTileElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapTile mapTile, PublishingContextStruct context)
		{
			switch (m_reader.LocalName)
			{
			case "Name":
				mapTile.Name = m_reader.ReadString();
				break;
			case "TileData":
				mapTile.TileData = m_reader.ReadString();
				break;
			case "MIMEType":
				mapTile.MIMEType = m_reader.ReadString();
				break;
			}
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.MapTile> ReadMapTiles(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			List<Microsoft.ReportingServices.ReportIntermediateFormat.MapTile> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.MapTile>();
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
						if (localName == "MapTile")
						{
							list.Add(ReadMapTile(map, context));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "MapTiles")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapTileLayer ReadMapTileLayer(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapTileLayer mapTileLayer = new Microsoft.ReportingServices.ReportIntermediateFormat.MapTileLayer(map);
			mapTileLayer.Name = m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "MapLayer", Microsoft.ReportingServices.ReportProcessing.ObjectType.Map, map.Name, mapTileLayer.Name, m_errorContext);
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadMapTileLayerElement(map, mapTileLayer, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapTileLayer" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapTileLayer;
		}

		private void ReadMapTileLayerElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapTileLayer mapTileLayer, PublishingContextStruct context)
		{
			switch (m_reader.LocalName)
			{
			case "ServiceUrl":
				mapTileLayer.ServiceUrl = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "TileStyle":
				mapTileLayer.TileStyle = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				if (!mapTileLayer.TileStyle.IsExpression)
				{
					Validator.ValidateMapTileStyle(mapTileLayer.TileStyle.StringValue, m_errorContext, context, m_reader.LocalName);
				}
				break;
			case "UseSecureConnection":
				mapTileLayer.UseSecureConnection = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
				break;
			case "MapTiles":
				mapTileLayer.MapTiles = ReadMapTiles(map, context);
				break;
			default:
				ReadMapLayerElement(map, mapTileLayer, context);
				break;
			}
		}

		private void ReadMapVectorLayerElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapVectorLayer mapVectorLayer, PublishingContextStruct context)
		{
			switch (m_reader.LocalName)
			{
			case "MapDataRegionName":
				mapVectorLayer.MapDataRegionName = m_reader.ReadString();
				break;
			case "MapBindingFieldPairs":
				mapVectorLayer.MapBindingFieldPairs = ReadMapBindingFieldPairs(map, mapVectorLayer, context);
				break;
			case "MapFieldDefinitions":
				mapVectorLayer.MapFieldDefinitions = ReadMapFieldDefinitions(map, context);
				break;
			case "MapShapefile":
			case "MapSpatialDataRegion":
			case "MapSpatialDataSet":
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.MapSpatialData mapSpatialData = mapVectorLayer.MapSpatialData;
				ReadMapSpatialData(mapVectorLayer, map, context, ref mapSpatialData, m_reader.LocalName);
				mapVectorLayer.MapSpatialData = mapSpatialData;
				break;
			}
			case "DataElementName":
				mapVectorLayer.DataElementName = m_reader.ReadString();
				break;
			case "DataElementOutput":
				mapVectorLayer.DataElementOutput = ReadDataElementOutput();
				break;
			default:
				ReadMapLayerElement(map, mapVectorLayer, context);
				break;
			}
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.MapFieldName> ReadMapFieldNames(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			List<Microsoft.ReportingServices.ReportIntermediateFormat.MapFieldName> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.MapFieldName>();
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
						if (localName == "MapFieldName")
						{
							Microsoft.ReportingServices.ReportIntermediateFormat.MapFieldName mapFieldName = new Microsoft.ReportingServices.ReportIntermediateFormat.MapFieldName(map);
							mapFieldName.Name = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							list.Add(mapFieldName);
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "MapFieldNames")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private void ReadMapSpatialData(Microsoft.ReportingServices.ReportIntermediateFormat.MapVectorLayer mapVectorLayer, Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context, ref Microsoft.ReportingServices.ReportIntermediateFormat.MapSpatialData mapSpatialData, string propertyName)
		{
			if (mapSpatialData != null)
			{
				m_errorContext.Register(ProcessingErrorCode.rsMapPropertyAlreadyDefined, Severity.Error, context.ObjectType, context.ObjectName, propertyName);
				return;
			}
			switch (propertyName)
			{
			case "MapShapefile":
				mapSpatialData = ReadMapShapefile(mapVectorLayer, map, context);
				break;
			case "MapSpatialDataRegion":
				mapSpatialData = ReadMapSpatialDataRegion(mapVectorLayer, map, context);
				break;
			case "MapSpatialDataSet":
				mapSpatialData = ReadMapSpatialDataSet(mapVectorLayer, map, context);
				break;
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapBorderSkin ReadMapBorderSkin(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapBorderSkin mapBorderSkin = new Microsoft.ReportingServices.ReportIntermediateFormat.MapBorderSkin(map);
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadMapBorderSkinElement(map, mapBorderSkin, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapBorderSkin" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapBorderSkin;
		}

		private void ReadMapBorderSkinElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapBorderSkin mapBorderSkin, PublishingContextStruct context)
		{
			string localName = m_reader.LocalName;
			if (!(localName == "Style"))
			{
				if (localName == "MapBorderSkinType")
				{
					mapBorderSkin.MapBorderSkinType = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
					if (!mapBorderSkin.MapBorderSkinType.IsExpression)
					{
						Validator.ValidateMapBorderSkinType(mapBorderSkin.MapBorderSkinType.StringValue, m_errorContext, context, m_reader.LocalName);
					}
				}
			}
			else
			{
				ReadMapStyle(mapBorderSkin, context);
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapCustomView ReadMapCustomView(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapCustomView mapCustomView = new Microsoft.ReportingServices.ReportIntermediateFormat.MapCustomView(map);
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadMapCustomViewElement(map, mapCustomView, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapCustomView" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapCustomView;
		}

		private void ReadMapCustomViewElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapCustomView mapCustomView, PublishingContextStruct context)
		{
			string localName = m_reader.LocalName;
			if (!(localName == "CenterX"))
			{
				if (localName == "CenterY")
				{
					mapCustomView.CenterY = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
				}
				else
				{
					ReadMapViewElement(map, mapCustomView, context);
				}
			}
			else
			{
				mapCustomView.CenterX = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapDataBoundView ReadMapDataBoundView(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapDataBoundView mapDataBoundView = new Microsoft.ReportingServices.ReportIntermediateFormat.MapDataBoundView(map);
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadMapDataBoundViewElement(map, mapDataBoundView, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapDataBoundView" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapDataBoundView;
		}

		private void ReadMapDataBoundViewElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapDataBoundView mapDataBoundView, PublishingContextStruct context)
		{
			_ = m_reader.LocalName;
			ReadMapViewElement(map, mapDataBoundView, context);
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapElementView ReadMapElementView(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapElementView mapElementView = new Microsoft.ReportingServices.ReportIntermediateFormat.MapElementView(map);
			if (!m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						ReadMapElementViewElement(map, mapElementView, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapElementView" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapElementView;
		}

		private void ReadMapElementViewElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapElementView mapElementView, PublishingContextStruct context)
		{
			string localName = m_reader.LocalName;
			if (!(localName == "LayerName"))
			{
				if (localName == "MapBindingFieldPairs")
				{
					mapElementView.MapBindingFieldPairs = ReadMapBindingFieldPairs(map, null, context);
				}
				else
				{
					ReadMapViewElement(map, mapElementView, context);
				}
			}
			else
			{
				mapElementView.LayerName = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
			}
		}

		private void ReadMapViewElement(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, Microsoft.ReportingServices.ReportIntermediateFormat.MapView mapView, PublishingContextStruct context)
		{
			string localName = m_reader.LocalName;
			if (localName == "Zoom")
			{
				mapView.Zoom = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
			}
		}

		private void ReadMapView(Microsoft.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context, ref Microsoft.ReportingServices.ReportIntermediateFormat.MapView mapView, string propertyName)
		{
			if (mapView != null)
			{
				m_errorContext.Register(ProcessingErrorCode.rsMapPropertyAlreadyDefined, Severity.Error, context.ObjectType, context.ObjectName, propertyName);
				return;
			}
			switch (propertyName)
			{
			case "MapCustomView":
				mapView = ReadMapCustomView(map, context);
				break;
			case "MapElementView":
				mapView = ReadMapElementView(map, context);
				break;
			case "MapDataBoundView":
				mapView = ReadMapDataBoundView(map, context);
				break;
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.CustomReportItem ReadCustomReportItem(Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem parent, PublishingContextStruct context, List<Microsoft.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget, out Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem altReportItem)
		{
			altReportItem = null;
			Microsoft.ReportingServices.ReportIntermediateFormat.CustomReportItem customReportItem = new Microsoft.ReportingServices.ReportIntermediateFormat.CustomReportItem(GenerateID(), parent);
			customReportItem.Name = m_reader.GetAttribute("Name");
			context.ObjectType = customReportItem.ObjectType;
			context.ObjectName = customReportItem.Name;
			RegisterDataRegion(customReportItem);
			bool validName = true;
			if (!m_reportItemNames.Validate(context.ObjectType, context.ObjectName, context.ErrorContext))
			{
				validName = false;
			}
			if ((context.Location & LocationFlags.InPageSection) != 0)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsCRIInPageSection, Severity.Error, context.ObjectType, context.ObjectName, null);
				validName = false;
			}
			bool computed = false;
			bool computed2 = false;
			bool flag = false;
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
							customReportItem.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, context.ErrorContext);
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
							customReportItem.ZIndex = m_reader.ReadInteger(context.ObjectType, context.ObjectName, m_reader.LocalName);
							break;
						case "Visibility":
							customReportItem.Visibility = ReadVisibility(context, out computed2);
							break;
						case "RepeatWith":
							customReportItem.RepeatedSibling = true;
							customReportItem.RepeatWith = m_reader.ReadString();
							break;
						case "Type":
							customReportItem.Type = m_reader.ReadString();
							break;
						case "AltReportItem":
							customReportItem.AltReportItem = ReadAltReportItem(parent, context, textBoxesWithDefaultSortTarget);
							break;
						case "CustomData":
							ReadCustomData(customReportItem, context, ref validName);
							break;
						case "CustomProperties":
							customReportItem.CustomProperties = ReadCustomProperties(context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("CustomReportItem" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			customReportItem.Computed = true;
			if (customReportItem.AltReportItem == null)
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.Rectangle rectangle = new Microsoft.ReportingServices.ReportIntermediateFormat.Rectangle(GenerateID(), GenerateID(), parent);
				rectangle.Name = customReportItem.Name + "_" + customReportItem.ID + "_" + rectangle.ID;
				m_reportItemNames.Validate(rectangle.ObjectType, rectangle.Name, context.ErrorContext);
				rectangle.Computed = false;
				Microsoft.ReportingServices.ReportIntermediateFormat.Visibility visibility = new Microsoft.ReportingServices.ReportIntermediateFormat.Visibility();
				visibility.Hidden = Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(value: true);
				rectangle.Visibility = visibility;
				m_reportItemCollectionList.Add(rectangle.ReportItems);
				customReportItem.AltReportItem = rectangle;
			}
			else
			{
				customReportItem.ExplicitlyDefinedAltReportItem = true;
			}
			customReportItem.AltReportItem.Top = customReportItem.Top;
			customReportItem.AltReportItem.Left = customReportItem.Left;
			customReportItem.AltReportItem.Height = customReportItem.Height;
			customReportItem.AltReportItem.Width = customReportItem.Width;
			customReportItem.AltReportItem.ZIndex = customReportItem.ZIndex;
			if (validName)
			{
				m_createSubtotalsDefs.Add(customReportItem);
			}
			altReportItem = customReportItem.AltReportItem;
			if (!validName)
			{
				return null;
			}
			return customReportItem;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem ReadAltReportItem(Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem parent, PublishingContextStruct context, List<Microsoft.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem result = null;
			if (!m_reader.IsEmptyElement)
			{
				int num = 0;
				bool flag = false;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (m_reader.LocalName)
						{
						case "Line":
							result = ReadLine(parent, context);
							num++;
							break;
						case "Rectangle":
							result = ReadRectangle(parent, context, textBoxesWithDefaultSortTarget);
							num++;
							break;
						case "CustomReportItem":
							context.ErrorContext.Register(ProcessingErrorCode.rsInvalidAltReportItem, Severity.Error, context.ObjectType, context.ObjectName, "AltReportItem");
							num++;
							break;
						case "Textbox":
							result = ReadTextbox(parent, context, textBoxesWithDefaultSortTarget);
							num++;
							break;
						case "Image":
							result = ReadImage(parent, context);
							num++;
							break;
						case "Subreport":
							result = ReadSubreport(parent, context);
							num++;
							break;
						case "Tablix":
							result = ReadTablix(parent, context);
							num++;
							break;
						case "Chart":
							result = ReadChart(parent, context);
							num++;
							break;
						}
						if (num > 1)
						{
							result = null;
							context.ErrorContext.Register(ProcessingErrorCode.rsMultiReportItemsInCustomReportItem, Severity.Error, context.ObjectType, context.ObjectName, "AltReportItem");
						}
						break;
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "AltReportItem")
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

		private void ReadCustomData(Microsoft.ReportingServices.ReportIntermediateFormat.CustomReportItem crItem, PublishingContextStruct context, ref bool validName)
		{
			crItem.SetAsDataRegion();
			if ((context.Location & LocationFlags.InDataRegion) != 0)
			{
				Global.Tracer.Assert(m_nestedDataRegions != null);
				m_nestedDataRegions.Add(crItem);
			}
			context.Location = (context.Location | LocationFlags.InDataSet | LocationFlags.InDataRegion);
			if (m_scopeNames.Validate(isGrouping: false, context.ObjectName, context.ObjectType, context.ObjectName, context.ErrorContext))
			{
				m_reportScopes.Add(crItem.Name, crItem);
			}
			else
			{
				validName = true;
			}
			if ((context.Location & LocationFlags.InPageSection) != 0)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsDataRegionInPageSection, Severity.Error, context.ObjectType, context.ObjectName, null);
			}
			bool flag = false;
			IdcRelationship relationship = null;
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
					case "Relationship":
						relationship = ReadRelationship(context);
						break;
					case "DataColumnHierarchy":
						crItem.DataColumnMembers = ReadCustomDataHierarchy(crItem, context, isColumnHierarchy: true, ref validName);
						break;
					case "DataRowHierarchy":
						crItem.DataRowMembers = ReadCustomDataHierarchy(crItem, context, isColumnHierarchy: false, ref validName);
						break;
					case "DataRows":
						crItem.DataRows = ReadCustomDataRows(crItem, context);
						break;
					case "Filters":
						crItem.Filters = ReadFilters(Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.DataRegionFilters, context);
						break;
					case "SortExpressions":
						crItem.Sorting = ReadSortExpressions(isDataRowSortExpression: true, context);
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
			crItem.DataScopeInfo.SetRelationship(crItem.DataSetName, relationship);
		}

		private DataMemberList ReadCustomDataHierarchy(Microsoft.ReportingServices.ReportIntermediateFormat.CustomReportItem crItem, PublishingContextStruct context, bool isColumnHierarchy, ref bool validName)
		{
			DataMemberList result = null;
			int leafNodes = 0;
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "DataMembers")
					{
						result = ReadCustomDataMembers(crItem, context, isColumnHierarchy, 0, ref leafNodes, ref validName);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if (m_reader.LocalName == (isColumnHierarchy ? "DataColumnHierarchy" : "DataRowHierarchy"))
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (isColumnHierarchy)
			{
				crItem.ColumnCount = leafNodes;
			}
			else
			{
				crItem.RowCount = leafNodes;
			}
			return result;
		}

		private DataMemberList ReadCustomDataMembers(Microsoft.ReportingServices.ReportIntermediateFormat.CustomReportItem crItem, PublishingContextStruct context, bool isColumnHierarchy, int level, ref int leafNodes, ref bool validName)
		{
			DataMemberList dataMemberList = new DataMemberList();
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "DataMember")
					{
						dataMemberList.Add(ReadCustomDataMember(crItem, context, isColumnHierarchy, level, ref leafNodes, ref validName));
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("DataMembers" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (dataMemberList.Count <= 0)
			{
				return null;
			}
			return dataMemberList;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.DataMember ReadCustomDataMember(Microsoft.ReportingServices.ReportIntermediateFormat.CustomReportItem crItem, PublishingContextStruct context, bool isColumnHierarchy, int level, ref int aLeafNodes, ref bool validName)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.DataMember dataMember = new Microsoft.ReportingServices.ReportIntermediateFormat.DataMember(GenerateID(), crItem);
			m_runningValueHolderList.Add(dataMember);
			dataMember.IsColumn = isColumnHierarchy;
			dataMember.Level = level;
			int leafNodes = 0;
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
						case "Group":
							dataMember.Grouping = ReadGrouping(dataMember, context, ref validName);
							break;
						case "SortExpressions":
							dataMember.Sorting = ReadSortExpressions(isDataRowSortExpression: false, context);
							break;
						case "CustomProperties":
							dataMember.CustomProperties = ReadCustomProperties(context);
							break;
						case "DataMembers":
							dataMember.SubMembers = ReadCustomDataMembers(crItem, context, isColumnHierarchy, level + 1, ref leafNodes, ref validName);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("DataMember" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			if (dataMember.SubMembers == null || dataMember.SubMembers.Count == 0)
			{
				aLeafNodes++;
				if (isColumnHierarchy)
				{
					dataMember.ColSpan = 1;
				}
				else
				{
					dataMember.RowSpan = 1;
				}
			}
			else
			{
				aLeafNodes += leafNodes;
				if (isColumnHierarchy)
				{
					dataMember.ColSpan = leafNodes;
				}
				else
				{
					dataMember.RowSpan = leafNodes;
				}
			}
			ValidateAndProcessMemberGroupAndSort(dataMember, context);
			return dataMember;
		}

		private CustomDataRowList ReadCustomDataRows(Microsoft.ReportingServices.ReportIntermediateFormat.CustomReportItem crItem, PublishingContextStruct context)
		{
			CustomDataRowList customDataRowList = new CustomDataRowList();
			bool flag = false;
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
						customDataRowList.Add(ReadCustomDataRow(crItem, context));
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
			return customDataRowList;
		}

		private CustomDataRow ReadCustomDataRow(Microsoft.ReportingServices.ReportIntermediateFormat.CustomReportItem crItem, PublishingContextStruct context)
		{
			CustomDataRow customDataRow = new CustomDataRow(GenerateID());
			bool flag = false;
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
						if (customDataRow.DataCells == null)
						{
							customDataRow.DataCells = new Microsoft.ReportingServices.ReportIntermediateFormat.DataCellList();
						}
						customDataRow.DataCells.Add(ReadCustomDataCell(crItem, context));
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
			return customDataRow;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.DataCell ReadCustomDataCell(Microsoft.ReportingServices.ReportIntermediateFormat.CustomReportItem crItem, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.DataCell dataCell = new Microsoft.ReportingServices.ReportIntermediateFormat.DataCell(GenerateID(), crItem);
			m_aggregateHolderList.Add(dataCell);
			m_runningValueHolderList.Add(dataCell);
			string dataSetName = null;
			List<IdcRelationship> relationships = null;
			int num = 0;
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (m_reader.LocalName)
					{
					case "DataValue":
						if (dataCell.DataValues == null)
						{
							dataCell.DataValues = new Microsoft.ReportingServices.ReportIntermediateFormat.DataValueList();
						}
						dataCell.DataValues.Add(ReadDataValue(isCustomProperty: false, nameRequired: false, ++num, context));
						break;
					case "DataSetName":
						dataSetName = m_reader.ReadString();
						break;
					case "Relationships":
						relationships = ReadRelationships(context);
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if (m_reader.LocalName == "DataCell")
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			dataCell.DataScopeInfo.SetRelationship(dataSetName, relationships);
			return dataCell;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.DataValue ReadDataValue(bool isCustomProperty, bool nameRequired, int index, PublishingContextStruct context)
		{
			bool isComputed = false;
			return ReadDataValue(isCustomProperty, nameRequired, index, ref isComputed, context);
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.DataValue ReadDataValue(bool isCustomProperty, bool nameRequired, int index, ref bool isComputed, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.DataValue dataValue = new Microsoft.ReportingServices.ReportIntermediateFormat.DataValue();
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
							dataValue.Value = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out computed2);
						}
					}
					else
					{
						dataValue.Name = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out computed);
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
			if (dataValue.Name == null && nameRequired)
			{
				if (isCustomProperty)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsMissingCustomPropertyName, Severity.Error, context.ObjectType, context.ObjectName, "Name", index.ToString(CultureInfo.CurrentCulture));
				}
				else
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsMissingChartDataValueName, Severity.Error, context.ObjectType, context.ObjectName, "DataValue", index.ToString(CultureInfo.CurrentCulture), "Name");
				}
			}
			isComputed = (isComputed || computed2 || computed);
			return dataValue;
		}

		private void ReadConnectionProperties(Microsoft.ReportingServices.ReportIntermediateFormat.DataSource dataSource, PublishingContextStruct context, ref bool hasComplexParams, Dictionary<string, bool> parametersInQuery)
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
						Global.Tracer.Assert(Microsoft.ReportingServices.ReportProcessing.ObjectType.DataSource == context.ObjectType);
						dataSource.ConnectStringExpression = ReadQueryOrParameterExpression(context, DataType.String, ref hasComplexParams, parametersInQuery);
						if (!dataSource.ConnectStringExpression.IsExpression && DataSourceInfo.HasUseridReference(dataSource.ConnectStringExpression.OriginalText))
						{
							SetConnectionStringUserProfileDependency();
						}
						break;
					case "IntegratedSecurity":
						dataSource.IntegratedSecurity = m_reader.ReadBoolean(context.ObjectType, context.ObjectName, m_reader.LocalName);
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
					if (!(localName == "DataSet"))
					{
						break;
					}
					Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet = ReadDataSet(context);
					m_dataSets.Add(dataSet);
					if (dataSet.IsReferenceToSharedDataSet && m_report.SharedDSContainer == null)
					{
						m_report.SharedDSContainerCollectionIndex = m_report.DataSourceCount;
						m_report.SharedDSContainer = new Microsoft.ReportingServices.ReportIntermediateFormat.DataSource(GenerateID(), Guid.Empty);
						if (m_report.DataSources == null)
						{
							m_report.DataSources = new List<Microsoft.ReportingServices.ReportIntermediateFormat.DataSource>();
						}
						m_report.DataSources.Add(m_report.SharedDSContainer);
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

		private Microsoft.ReportingServices.ReportIntermediateFormat.DataSet ReadDataSet(PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet = new Microsoft.ReportingServices.ReportIntermediateFormat.DataSet(GenerateID(), m_dataSetIndexCounter++);
			dataSet.Name = m_reader.GetAttribute("Name");
			context.Location |= LocationFlags.InDataSet;
			context.ObjectType = dataSet.ObjectType;
			context.ObjectName = dataSet.Name;
			if (m_scopeNames.Validate(isGrouping: false, context.ObjectName, context.ObjectType, context.ObjectName, context.ErrorContext))
			{
				m_reportScopes.Add(dataSet.Name, dataSet);
			}
			m_aggregateHolderList.Add(dataSet);
			bool isComplex = false;
			Dictionary<string, bool> referencedReportParameters = new Dictionary<string, bool>();
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
						dataSet.Query = ReadQuery(context, ref isComplex, referencedReportParameters);
						break;
					case "SharedDataSet":
						dataSet.SharedDataSetQuery = ReadSharedDataSetQuery(context, ref isComplex, referencedReportParameters);
						break;
					case "CaseSensitivity":
						dataSet.CaseSensitivity = ReadTriState();
						break;
					case "Collation":
					{
						dataSet.Collation = m_reader.ReadString();
						if (DataSetValidator.ValidateCollation(dataSet.Collation, out uint lcid))
						{
							dataSet.LCID = lcid;
							break;
						}
						context.ErrorContext.Register(ProcessingErrorCode.rsInvalidCollationName, Severity.Warning, context.ObjectType, context.ObjectName, null, dataSet.Collation.MarkAsPrivate());
						break;
					}
					case "CollationCulture":
						dataSet.CollationCulture = m_reader.ReadString();
						ValidateCollationCultureAndSetLcid(dataSet, context);
						break;
					case "AccentSensitivity":
						dataSet.AccentSensitivity = ReadTriState();
						break;
					case "KanatypeSensitivity":
						dataSet.KanatypeSensitivity = ReadTriState();
						break;
					case "WidthSensitivity":
						dataSet.WidthSensitivity = ReadTriState();
						break;
					case "NullsAsBlanks":
						dataSet.NullsAsBlanks = m_reader.ReadBoolean(context.ObjectType, context.ObjectName, m_reader.LocalName);
						break;
					case "Filters":
						dataSet.Filters = ReadFilters(Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.DataSetFilters, context, ref isComplex, referencedReportParameters);
						break;
					case "InterpretSubtotalsAsDetails":
						dataSet.InterpretSubtotalsAsDetails = ReadTriState();
						break;
					case "DefaultRelationships":
						dataSet.DefaultRelationships = ReadDefaultRelationships(context);
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
			if (!string.IsNullOrEmpty(dataSet.Collation) && !string.IsNullOrEmpty(dataSet.CollationCulture))
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsCollationAndCollationCultureSpecified, Severity.Error, context.ObjectType, context.ObjectName, "CollationCulture", "Collation");
			}
			ValidateDataSet(dataSet, context, isComplex, referencedReportParameters);
			return dataSet;
		}

		private SharedDataSetQuery ReadSharedDataSetQuery(PublishingContextStruct context, ref bool isComplex, Dictionary<string, bool> referencedReportParameters)
		{
			SharedDataSetQuery sharedDataSetQuery = new SharedDataSetQuery();
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (!(localName == "SharedDataSetReference"))
					{
						if (localName == "QueryParameters")
						{
							sharedDataSetQuery.Parameters = ReadQueryParameters(context, ref isComplex, referencedReportParameters);
						}
					}
					else
					{
						sharedDataSetQuery.SharedDataSetReference = m_reader.ReadString();
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("SharedDataSet" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return sharedDataSetQuery;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ReportQuery ReadQuery(PublishingContextStruct context, ref bool isComplex, Dictionary<string, bool> referencedReportParameters)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ReportQuery reportQuery = new Microsoft.ReportingServices.ReportIntermediateFormat.ReportQuery();
			bool flag = false;
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
						Global.Tracer.Assert(Microsoft.ReportingServices.ReportProcessing.ObjectType.DataSet == context.ObjectType);
						context.ObjectType = Microsoft.ReportingServices.ReportProcessing.ObjectType.Query;
						reportQuery.CommandText = ReadQueryOrParameterExpression(context, DataType.String, ref isComplex, referencedReportParameters);
						context.ObjectType = Microsoft.ReportingServices.ReportProcessing.ObjectType.DataSet;
						break;
					case "QueryParameters":
						reportQuery.Parameters = ReadQueryParameters(context, ref isComplex, referencedReportParameters);
						break;
					case "Timeout":
						reportQuery.TimeOut = m_reader.ReadInteger(context.ObjectType, context.ObjectName, m_reader.LocalName);
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
			return reportQuery;
		}

		private CommandType ReadCommandType()
		{
			string value = m_reader.ReadString();
			return (CommandType)Enum.Parse(typeof(CommandType), value, ignoreCase: false);
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.ParameterValue> ReadQueryParameters(PublishingContextStruct context, ref bool hasComplexParams, Dictionary<string, bool> parametersInQuery)
		{
			List<Microsoft.ReportingServices.ReportIntermediateFormat.ParameterValue> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.ParameterValue>();
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
					if (!(localName == "QueryParameter"))
					{
						if (localName == "DataSetParameter")
						{
							list.Add(ReadRSDDataSetParameter(context, ref hasComplexParams, parametersInQuery));
						}
					}
					else
					{
						list.Add(ReadQueryParameter(context, ref hasComplexParams, parametersInQuery));
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("QueryParameters" == m_reader.LocalName || "DataSetParameters" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			context.ObjectName = objectName;
			return list;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ParameterValue ReadQueryParameter(PublishingContextStruct context, ref bool isComplex, Dictionary<string, bool> parametersInQuery)
		{
			Global.Tracer.Assert(Microsoft.ReportingServices.ReportProcessing.ObjectType.DataSet == context.ObjectType);
			Microsoft.ReportingServices.ReportIntermediateFormat.ParameterValue parameterValue = new Microsoft.ReportingServices.ReportIntermediateFormat.ParameterValue();
			parameterValue.Name = m_reader.GetAttribute("Name");
			context.ObjectType = Microsoft.ReportingServices.ReportProcessing.ObjectType.QueryParameter;
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
						parameterValue.ConstantDataType = ReadDataTypeAttribute();
						parameterValue.Value = ReadQueryOrParameterExpression(context, parameterValue.ConstantDataType, ref isComplex, parametersInQuery);
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

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.Field> ReadFields(PublishingContextStruct context, out int calculatedFieldStartIndex)
		{
			List<Microsoft.ReportingServices.ReportIntermediateFormat.Field> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.Field>();
			List<string> aggregateIndicatorFieldNames = new List<string>();
			CLSUniqueNameValidator names = new CLSUniqueNameValidator(ProcessingErrorCode.rsInvalidFieldNameNotCLSCompliant, ProcessingErrorCode.rsDuplicateFieldName, ProcessingErrorCode.rsInvalidFieldNameLength);
			Microsoft.ReportingServices.ReportIntermediateFormat.Field field = null;
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
					if (localName == "Field")
					{
						field = ReadField(names, context, out string aggregateIndicatorFieldName);
						InsertField(field, aggregateIndicatorFieldName, list, aggregateIndicatorFieldNames, ref calculatedFieldStartIndex);
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
				calculatedFieldStartIndex = list.Count;
			}
			AssignAndValidateAggregateIndicatorFieldIndex(context, list, aggregateIndicatorFieldNames);
			return list;
		}

		private static void AssignAndValidateAggregateIndicatorFieldIndex(PublishingContextStruct context, List<Microsoft.ReportingServices.ReportIntermediateFormat.Field> fields, List<string> aggregateIndicatorFieldNames)
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>(fields.Count, StringComparer.Ordinal);
			for (int i = 0; i < fields.Count; i++)
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.Field field = fields[i];
				dictionary[field.Name] = i;
			}
			for (int j = 0; j < aggregateIndicatorFieldNames.Count; j++)
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.Field field2 = fields[j];
				string text = aggregateIndicatorFieldNames[j];
				if (string.IsNullOrEmpty(text))
				{
					continue;
				}
				if (field2.IsCalculatedField)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsAggregateIndicatorFieldOnCalculatedField, Severity.Error, Microsoft.ReportingServices.ReportProcessing.ObjectType.Field, field2.Name, "AggregateIndicatorField", "Value", context.ObjectName.MarkAsPrivate());
				}
				if (dictionary.TryGetValue(text, out int value))
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.Field field3 = fields[value];
					if (field3.IsCalculatedField && field3.Value.Type != Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Literal && (field3.Value.Type != Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant || field3.Value.ConstantType != DataType.Boolean))
					{
						context.ErrorContext.Register(ProcessingErrorCode.rsInvalidAggregateIndicatorField, Severity.Error, Microsoft.ReportingServices.ReportProcessing.ObjectType.Field, field2.Name, "AggregateIndicatorField", context.ObjectName.MarkAsPrivate());
					}
					field2.AggregateIndicatorFieldIndex = value;
				}
				else
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidAggregateIndicatorField, Severity.Error, Microsoft.ReportingServices.ReportProcessing.ObjectType.Field, field2.Name, "AggregateIndicatorField", context.ObjectName.MarkAsModelInfo());
				}
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.Field ReadField(CLSUniqueNameValidator names, PublishingContextStruct context, out string aggregateIndicatorFieldName)
		{
			Global.Tracer.Assert(Microsoft.ReportingServices.ReportProcessing.ObjectType.DataSet == context.ObjectType || Microsoft.ReportingServices.ReportProcessing.ObjectType.SharedDataSet == context.ObjectType);
			string objectName = context.ObjectName;
			Microsoft.ReportingServices.ReportIntermediateFormat.Field field = new Microsoft.ReportingServices.ReportIntermediateFormat.Field();
			context.ObjectType = Microsoft.ReportingServices.ReportProcessing.ObjectType.Field;
			string text = null;
			aggregateIndicatorFieldName = null;
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
						switch (m_reader.LocalName)
						{
						case "DataField":
							field.DataField = m_reader.ReadString();
							names.Validate(field.Name, field.DataField, objectName, context.ErrorContext);
							break;
						case "AggregateIndicatorField":
							aggregateIndicatorFieldName = m_reader.ReadString();
							break;
						case "Value":
						{
							field.DataType = ReadDataTypeAttribute();
							Microsoft.ReportingServices.RdlExpressions.ExpressionParser.EvaluationMode mode = ReadEvaluationModeAttribute();
							text = m_reader.ReadString();
							if (text != null)
							{
								field.Value = ReadExpression(text, m_reader.LocalName, objectName, mode, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.FieldValue, field.DataType, context);
							}
							break;
						}
						}
						break;
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
			ValidateField(field, text, context, objectName);
			return field;
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.Filter> ReadFilters(Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType expressionType, PublishingContextStruct context)
		{
			bool isComplex = false;
			Dictionary<string, bool> referencedReportParameters = new Dictionary<string, bool>();
			return ReadFilters(expressionType, context, ref isComplex, referencedReportParameters);
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.Filter> ReadFilters(Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType expressionType, PublishingContextStruct context, ref bool isComplex, Dictionary<string, bool> referencedReportParameters)
		{
			List<Microsoft.ReportingServices.ReportIntermediateFormat.Filter> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.Filter>();
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
						list.Add(ReadFilter(expressionType, context, ref isComplex, referencedReportParameters));
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
			if (list.Count > 0 && m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.Filters))
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, context.ObjectType, context.ObjectName, "Filters");
			}
			return list;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.Filter ReadFilter(Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType expressionType, PublishingContextStruct context, ref bool isComplex, Dictionary<string, bool> referencedReportParameters)
		{
			m_hasFilters = true;
			Microsoft.ReportingServices.ReportIntermediateFormat.Filter filter = new Microsoft.ReportingServices.ReportIntermediateFormat.Filter();
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
						filter.Expression = ReadFilterExpression(m_reader.LocalName, expressionType, DataType.String, context, ref isComplex, referencedReportParameters);
						break;
					case "Operator":
						filter.Operator = ReadOperator();
						break;
					case "FilterValues":
						filter.Values = ReadFilterValues(expressionType, context, ref isComplex, referencedReportParameters);
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
			case Microsoft.ReportingServices.ReportIntermediateFormat.Filter.Operators.Equal:
			case Microsoft.ReportingServices.ReportIntermediateFormat.Filter.Operators.Like:
			case Microsoft.ReportingServices.ReportIntermediateFormat.Filter.Operators.GreaterThan:
			case Microsoft.ReportingServices.ReportIntermediateFormat.Filter.Operators.GreaterThanOrEqual:
			case Microsoft.ReportingServices.ReportIntermediateFormat.Filter.Operators.LessThan:
			case Microsoft.ReportingServices.ReportIntermediateFormat.Filter.Operators.LessThanOrEqual:
			case Microsoft.ReportingServices.ReportIntermediateFormat.Filter.Operators.NotEqual:
				VerifyFilterValueCount(context, filter, num, 1);
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Filter.Operators.TopN:
			case Microsoft.ReportingServices.ReportIntermediateFormat.Filter.Operators.BottomN:
				VerifyTopBottomFilterValue(context, filter, num, isPercentFilter: false);
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Filter.Operators.TopPercent:
			case Microsoft.ReportingServices.ReportIntermediateFormat.Filter.Operators.BottomPercent:
				VerifyTopBottomFilterValue(context, filter, num, isPercentFilter: true);
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Filter.Operators.Between:
				VerifyFilterValueCount(context, filter, num, 2);
				break;
			}
			if (Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.GroupingFilters == expressionType && filter.Expression.HasRecursiveAggregates())
			{
				m_hasSpecialRecursiveAggregates = true;
			}
			return filter;
		}

		private void VerifyTopBottomFilterValue(PublishingContextStruct context, Microsoft.ReportingServices.ReportIntermediateFormat.Filter filter, int count, bool isPercentFilter)
		{
			if (!VerifyFilterValueCount(context, filter, count, 1))
			{
				return;
			}
			ExpressionInfoTypeValuePair expressionInfoTypeValuePair = filter.Values[0];
			if (expressionInfoTypeValuePair.Value.IsExpression)
			{
				return;
			}
			if (expressionInfoTypeValuePair.HadExplicitDataType)
			{
				if (isPercentFilter)
				{
					if (expressionInfoTypeValuePair.DataType != DataType.Integer && expressionInfoTypeValuePair.DataType != DataType.Float)
					{
						context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFilterValueDataType, Severity.Error, context.ObjectType, context.ObjectName, "FilterValues", filter.Operator.ToString(), RPRes.rsDataTypeIntegerOrFloat);
					}
				}
				else if (expressionInfoTypeValuePair.DataType != DataType.Integer)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFilterValueDataType, Severity.Error, context.ObjectType, context.ObjectName, "FilterValues", filter.Operator.ToString(), RPRes.rsDataTypeInteger);
				}
			}
			else
			{
				DataType constantType = (!isPercentFilter) ? DataType.Integer : DataType.Float;
				Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo value = expressionInfoTypeValuePair.Value;
				Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ParseRDLConstant(value.StringValue, value, constantType, context.ErrorContext, context.ObjectType, context.ObjectName, "FilterValues");
			}
		}

		private bool VerifyFilterValueCount(PublishingContextStruct context, Microsoft.ReportingServices.ReportIntermediateFormat.Filter filter, int expectedCount, int actualCount)
		{
			if (expectedCount != actualCount)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidNumberOfFilterValues, Severity.Error, context.ObjectType, context.ObjectName, "FilterValues", filter.Operator.ToString(), Convert.ToString(expectedCount, CultureInfo.InvariantCulture));
				return false;
			}
			return true;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo ReadFilterExpression(string propertyName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType expressionType, DataType dataType, PublishingContextStruct context, ref bool isComplex, Dictionary<string, bool> referencedReportParameters)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = ReadExpression(m_reader.LocalName, expressionType, dataType, context);
			if (expressionInfo != null)
			{
				isComplex |= expressionInfo.HasDynamicParameterReference;
				if (expressionInfo.ReferencedParameters != null)
				{
					foreach (string referencedParameter in expressionInfo.ReferencedParameters)
					{
						if (!string.IsNullOrEmpty(referencedParameter))
						{
							referencedReportParameters[referencedParameter] = true;
						}
					}
					return expressionInfo;
				}
			}
			return expressionInfo;
		}

		private List<ExpressionInfoTypeValuePair> ReadFilterValues(Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType expressionType, PublishingContextStruct context, ref bool isComplex, Dictionary<string, bool> referencedReportParameters)
		{
			List<ExpressionInfoTypeValuePair> list = new List<ExpressionInfoTypeValuePair>();
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
						bool hadExplicitDataType;
						DataType dataType = ReadDataTypeAttribute(out hadExplicitDataType);
						Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = ReadFilterExpression(m_reader.LocalName, expressionType, dataType, context, ref isComplex, referencedReportParameters);
						list.Add(new ExpressionInfoTypeValuePair(dataType, hadExplicitDataType, expressionInfo));
						if (Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.GroupingFilters == expressionType && expressionInfo.HasRecursiveAggregates())
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
			return list;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.Filter.Operators ReadOperator()
		{
			string value = m_reader.ReadString();
			return (Microsoft.ReportingServices.ReportIntermediateFormat.Filter.Operators)Enum.Parse(typeof(Microsoft.ReportingServices.ReportIntermediateFormat.Filter.Operators), value, ignoreCase: false);
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

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.DataSource> ReadDataSources(PublishingContextStruct context, IDataProtection dataProtection)
		{
			List<Microsoft.ReportingServices.ReportIntermediateFormat.DataSource> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.DataSource>();
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
						list.Add(ReadDataSource(dataSourceNames, context, dataProtection));
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
			return list;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.DataSource ReadDataSource(DataSourceNameValidator dataSourceNames, PublishingContextStruct context, IDataProtection dataProtection)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.DataSource dataSource = new Microsoft.ReportingServices.ReportIntermediateFormat.DataSource(GenerateID());
			dataSource.Name = m_reader.GetAttribute("Name");
			context.ObjectType = Microsoft.ReportingServices.ReportProcessing.ObjectType.DataSource;
			context.ObjectName = dataSource.Name;
			bool flag = false;
			if (dataSourceNames.Validate(context.ObjectType, context.ObjectName, context.ErrorContext))
			{
				flag = true;
			}
			bool flag2 = false;
			bool flag3 = false;
			bool hasComplexParams = false;
			Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
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
							dataSource.Transaction = m_reader.ReadBoolean(context.ObjectType, context.ObjectName, m_reader.LocalName);
							break;
						case "ConnectionProperties":
							flag2 = true;
							ReadConnectionProperties(dataSource, context, ref hasComplexParams, dictionary);
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
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidDataSource, Severity.Error, context.ObjectType, context.ObjectName, null);
			}
			if (flag && !m_dataSourceNames.ContainsKey(dataSource.Name))
			{
				m_dataSourceNames.Add(dataSource.Name, null);
			}
			DataSourceInfo dataSourceInfo = null;
			if (flag2)
			{
				dataSource.IsComplex = hasComplexParams;
				dataSource.ParameterNames = dictionary;
				bool flag5 = false;
				if (dataSource.ConnectStringExpression.Type != Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant)
				{
					flag5 = true;
				}
				dataSourceInfo = new DataSourceInfo(dataSource.Name, dataSource.Type, flag5 ? null : dataSource.ConnectStringExpression.OriginalText, flag5, dataSource.IntegratedSecurity, dataSource.Prompt, dataProtection);
			}
			else if (flag3)
			{
				dataSourceInfo = CreateSharedDataSourceLink(context, dataSource);
			}
			if (dataSourceInfo != null)
			{
				if (m_publishingContext.ResolveTemporaryDataSourceCallback != null)
				{
					m_publishingContext.ResolveTemporaryDataSourceCallback(dataSourceInfo, m_publishingContext.OriginalDataSources);
				}
				dataSource.ID = dataSourceInfo.ID;
				m_dataSources.Add(dataSourceInfo);
			}
			return dataSource;
		}

		private DataSourceInfo CreateSharedDataSourceLink(PublishingContextStruct context, Microsoft.ReportingServices.ReportIntermediateFormat.DataSource dataSource)
		{
			DataSourceInfo dataSourceInfo = null;
			string text = (m_publishingContext.CatalogContext == null) ? dataSource.DataSourceReference : m_publishingContext.CatalogContext.MapUserProvidedPath(dataSource.DataSourceReference);
			if (m_publishingContext.CheckDataSourceCallback == null)
			{
				dataSourceInfo = new DataSourceInfo(dataSource.Name, text, Guid.Empty);
			}
			else
			{
				Guid catalogItemId = Guid.Empty;
				DataSourceInfo dataSourceInfo2 = m_publishingContext.CheckDataSourceCallback(text, out catalogItemId);
				if (dataSourceInfo2 == null)
				{
					dataSourceInfo = new DataSourceInfo(dataSource.Name);
					string plainString = (m_publishingContext.PublishingContextKind == PublishingContextKind.SharedDataSet) ? dataSource.DataSourceReference : dataSource.Name;
					context.ErrorContext.Register(ProcessingErrorCode.rsDataSourceReferenceNotPublished, Severity.Warning, context.ObjectType, context.ObjectName, (m_publishingContext.PublishingContextKind == PublishingContextKind.SharedDataSet) ? RPRes.rsObjectTypeSharedDataSet : RPRes.rsObjectTypeReport, plainString.MarkAsPrivate());
				}
				else
				{
					dataSourceInfo = new DataSourceInfo(dataSource.Name, text, catalogItemId, dataSourceInfo2);
				}
			}
			return dataSourceInfo;
		}

		private void ValidateCollationCultureAndSetLcid(Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet, PublishingContextStruct context)
		{
			if (Validator.ValidateSpecificLanguage(dataSet.CollationCulture, out CultureInfo culture))
			{
				if (culture != null)
				{
					dataSet.LCID = (uint)culture.LCID;
				}
			}
			else
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidLanguage, Severity.Error, context.ObjectType, context.ObjectName, "CollationCulture", dataSet.CollationCulture);
			}
		}

		private void ValidateDataSet(Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet, PublishingContextStruct context, bool isComplex, Dictionary<string, bool> referencedReportParameters)
		{
			PublishingDataSetInfo publishingDataSetInfo = new PublishingDataSetInfo(dataSet.Name, m_dataSets.Count, isComplex, referencedReportParameters);
			if (dataSet.Query == null == (dataSet.SharedDataSetQuery == null))
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidDataSetQuery, Severity.Error, context.ObjectType, dataSet.Name, null);
				return;
			}
			if (!m_dataSetQueryInfo.ContainsKey(context.ObjectName))
			{
				m_dataSetQueryInfo.Add(context.ObjectName, publishingDataSetInfo);
				int num = (dataSet.Fields != null) ? dataSet.Fields.Count : 0;
				while (num > 0 && dataSet.Fields[num - 1].IsCalculatedField)
				{
					num--;
				}
				publishingDataSetInfo.CalculatedFieldIndex = num;
			}
			if (!dataSet.IsReferenceToSharedDataSet)
			{
				return;
			}
			if (m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.SharedDataSetReferences))
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, context.ObjectType, dataSet.Name, "SharedDataSet");
				return;
			}
			DataSetInfo dataSetInfo = null;
			string text = (m_publishingContext.CatalogContext == null) ? dataSet.SharedDataSetQuery.SharedDataSetReference : m_publishingContext.CatalogContext.MapUserProvidedPath(dataSet.SharedDataSetQuery.SharedDataSetReference);
			if (m_publishingContext.CheckDataSetCallback == null)
			{
				dataSetInfo = new DataSetInfo(dataSet.DataSetCore.Name, text);
			}
			else
			{
				Guid catalogItemId = Guid.Empty;
				if (m_publishingContext.CheckDataSetCallback(text, out catalogItemId))
				{
					dataSetInfo = new DataSetInfo(dataSet.DataSetCore.Name, text, catalogItemId);
				}
				else
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsDataSetReferenceNotPublished, Severity.Warning, context.ObjectType, context.ObjectName, null, text.MarkAsPrivate());
					dataSetInfo = new DataSetInfo(dataSet.DataSetCore.Name, text);
				}
			}
			if (m_publishingContext.ResolveTemporaryDataSetCallback != null)
			{
				m_publishingContext.ResolveTemporaryDataSetCallback(dataSetInfo, m_publishingContext.OriginalDataSets);
			}
			dataSet.DataSetCore.SetCatalogID(dataSetInfo.ID);
			m_sharedDataSetReferences.Add(dataSetInfo);
		}

		private void InsertField(Microsoft.ReportingServices.ReportIntermediateFormat.Field field, string aggregateIndicatorFieldName, List<Microsoft.ReportingServices.ReportIntermediateFormat.Field> fields, List<string> aggregateIndicatorFieldNames, ref int calculatedFieldStartIndex)
		{
			if (field.IsCalculatedField)
			{
				if (calculatedFieldStartIndex < 0)
				{
					calculatedFieldStartIndex = fields.Count;
				}
				fields.Add(field);
				aggregateIndicatorFieldNames.Add(aggregateIndicatorFieldName);
			}
			else if (calculatedFieldStartIndex < 0)
			{
				fields.Add(field);
				aggregateIndicatorFieldNames.Add(aggregateIndicatorFieldName);
			}
			else
			{
				fields.Insert(calculatedFieldStartIndex, field);
				aggregateIndicatorFieldNames.Insert(calculatedFieldStartIndex, aggregateIndicatorFieldName);
				calculatedFieldStartIndex++;
			}
		}

		private void ValidateField(Microsoft.ReportingServices.ReportIntermediateFormat.Field field, object fieldValue, PublishingContextStruct context, string dataSetName)
		{
			if (field.DataField != null == (fieldValue != null))
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidField, Severity.Error, context.ObjectType, field.Name, null, dataSetName.MarkAsPrivate());
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.Sorting ReadSortExpressions(bool isDataRowSortExpression, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.Sorting sorting = new Microsoft.ReportingServices.ReportIntermediateFormat.Sorting(Microsoft.ReportingServices.ReportIntermediateFormat.ConstructionPhase.Publishing);
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "SortExpression")
					{
						ReadSortExpression(sorting, isDataRowSortExpression, context);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("SortExpressions" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (sorting.SortExpressions == null || sorting.SortExpressions.Count == 0)
			{
				sorting = null;
			}
			else
			{
				m_hasSorting = true;
				sorting.ValidateNaturalSortFlags(context);
				sorting.ValidateDeferredSortFlags(context);
				if (m_publishingContext.IsRestrictedDataRegionSort(isDataRowSortExpression) || m_publishingContext.IsRestrictedGroupSort(isDataRowSortExpression, sorting))
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, context.ObjectType, context.ObjectName, "SortExpressions");
				}
				else if (sorting.NaturalSort && isDataRowSortExpression)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidNaturalSortContainer, Severity.Error, context.ObjectType, context.ObjectName, "SortExpressions");
				}
				else if (sorting.DeferredSort && isDataRowSortExpression)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidDeferredSortContainer, Severity.Error, context.ObjectType, context.ObjectName, "SortExpressions");
				}
			}
			return sorting;
		}

		private void ReadSortExpression(Microsoft.ReportingServices.ReportIntermediateFormat.Sorting sorting, bool isDataRowSortExpression, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = null;
			bool item = true;
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
					case "Value":
						expressionInfo = ReadExpression("SortExpression." + m_reader.LocalName, isDataRowSortExpression ? Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.DataRegionSortExpression : Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.SortExpression, DataType.String, context);
						break;
					case "Direction":
						item = ReadDirection();
						break;
					case "NaturalSort":
						flag = m_reader.ReadBoolean(context.ObjectType, context.ObjectName, m_reader.LocalName);
						break;
					case "DeferredSort":
						flag2 = m_reader.ReadBoolean(context.ObjectType, context.ObjectName, m_reader.LocalName);
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("SortExpression" == m_reader.LocalName)
					{
						flag3 = true;
					}
					break;
				}
			}
			while (!flag3);
			if (expressionInfo.IsExpression)
			{
				sorting.SortExpressions.Add(expressionInfo);
				sorting.SortDirections.Add(item);
				sorting.NaturalSortFlags.Add(flag);
				sorting.DeferredSortFlags.Add(flag2);
				if (expressionInfo.HasRecursiveAggregates())
				{
					m_hasSpecialRecursiveAggregates = true;
				}
			}
			if (flag && m_publishingContext.IsRestrictedNaturalGroupSort(expressionInfo))
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidNaturalSortGroupExpressionNotSimpleFieldReference, Severity.Error, context.ObjectType, context.ObjectName, "SortExpressions", "NaturalSort");
			}
			if (flag2 && m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.DeferredSort))
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, context.ObjectType, context.ObjectName, "DeferredSort");
			}
			if (flag && flag2)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsConflictingSortFlags, Severity.Error, context.ObjectType, context.ObjectName, "SortExpression");
			}
		}

		private bool ReadDirection()
		{
			string x = m_reader.ReadString();
			return Microsoft.ReportingServices.ReportProcessing.ReportProcessing.CompareWithInvariantCulture(x, "Ascending", ignoreCase: false) == 0;
		}

		private void ValidateAndProcessMemberGroupAndSort(Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode member, PublishingContextStruct context)
		{
			if (member.IsStatic)
			{
				if (member.Sorting != null)
				{
					m_errorContext.Register(ProcessingErrorCode.rsInvalidSortNotAllowed, Severity.Error, context.ObjectType, context.ObjectName, "SortExpressions", "Group", member.RdlElementName);
				}
			}
			else
			{
				MergeGroupingAndSortingIfCompatible(member);
				if (member.Sorting != null && member.Sorting.NaturalSort && !member.Grouping.NaturalGroup)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidNaturalSortContainer, Severity.Error, context.ObjectType, context.ObjectName, "SortExpressions");
				}
			}
		}

		private bool ShouldMergeGroupingAndSorting(Microsoft.ReportingServices.ReportIntermediateFormat.Grouping grouping, Microsoft.ReportingServices.ReportIntermediateFormat.Sorting sorting)
		{
			if (grouping != null && grouping.Parent == null && sorting != null && grouping.GroupExpressions != null && sorting.SortExpressions != null && sorting.ShouldApplySorting && grouping.GroupExpressions.Count == sorting.SortExpressions.Count)
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

		private void MergeGroupingAndSortingIfCompatible(Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode member)
		{
			if (ShouldMergeGroupingAndSorting(member.Grouping, member.Sorting))
			{
				member.Grouping.GroupAndSort = true;
				member.Grouping.SortDirections = member.Sorting.SortDirections;
				member.Sorting = null;
			}
			if (member.Sorting != null && member.Sorting.ShouldApplySorting)
			{
				m_requiresSortingPostGrouping = true;
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.Grouping ReadGrouping(Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode scope, PublishingContextStruct context)
		{
			bool validName = false;
			return ReadGrouping(scope, context, ref validName);
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.Grouping ReadGrouping(Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode scope, PublishingContextStruct context, ref bool validName)
		{
			m_hasGrouping = true;
			Microsoft.ReportingServices.ReportIntermediateFormat.Grouping grouping = new Microsoft.ReportingServices.ReportIntermediateFormat.Grouping(GenerateID(), Microsoft.ReportingServices.ReportIntermediateFormat.ConstructionPhase.Publishing);
			grouping.Name = m_reader.GetAttribute("Name");
			if (m_scopeNames.Validate(isGrouping: true, grouping.Name, context.ObjectType, context.ObjectName, context.ErrorContext))
			{
				m_reportScopes.Add(grouping.Name, grouping);
			}
			else
			{
				validName = false;
			}
			m_aggregateHolderList.Add(grouping);
			string dataSetName = null;
			IdcRelationship relationship = null;
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
						case "DocumentMapLabel":
							grouping.GroupLabel = ReadDocumentMapLabelExpression(m_reader.LocalName, context);
							break;
						case "GroupExpressions":
							ReadGroupExpressions(grouping.GroupExpressions, context);
							break;
						case "DataSetName":
							dataSetName = m_reader.ReadString();
							break;
						case "Relationship":
							relationship = ReadRelationship(context);
							break;
						case "PageBreak":
							ReadPageBreak(grouping, context);
							break;
						case "PageName":
							grouping.PageName = ReadPageNameExpression(context);
							break;
						case "Filters":
							grouping.Filters = ReadFilters(Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.GroupingFilters, context);
							m_hasGroupFilters = true;
							break;
						case "Parent":
							grouping.Parent = new List<Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo>();
							grouping.Parent.Add(ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.GroupExpression, DataType.String, context));
							if (m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.GroupParent))
							{
								context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, Microsoft.ReportingServices.ReportProcessing.ObjectType.Grouping, grouping.Name, "Parent");
							}
							break;
						case "DataElementName":
							grouping.DataElementName = m_reader.ReadString();
							break;
						case "DataElementOutput":
							grouping.DataElementOutput = ReadDataElementOutput();
							break;
						case "Variables":
							grouping.Variables = ReadVariables(context, isGrouping: true, grouping.Name);
							break;
						case "DomainScope":
							grouping.DomainScope = m_reader.ReadString();
							if (m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.DomainScope))
							{
								context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, Microsoft.ReportingServices.ReportProcessing.ObjectType.Grouping, grouping.Name, "DomainScope");
							}
							break;
						case "NaturalGroup":
							grouping.NaturalGroup = m_reader.ReadBoolean(context.ObjectType, context.ObjectName, m_reader.LocalName);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("Group" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			scope.DataScopeInfo.SetRelationship(dataSetName, relationship);
			if (grouping.Parent != null && 1 != grouping.GroupExpressions.Count)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidGroupingParent, Severity.Error, context.ObjectType, context.ObjectName, "Parent");
			}
			if (grouping.NaturalGroup)
			{
				if (grouping.Parent != null)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidGroupingNaturalGroupFeature, Severity.Warning, context.ObjectType, context.ObjectName, "NaturalGroup", "Parent");
					grouping.NaturalGroup = false;
				}
				if (grouping.DomainScope != null)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidGroupingNaturalGroupFeature, Severity.Warning, context.ObjectType, context.ObjectName, "NaturalGroup", "DomainScope");
					grouping.NaturalGroup = false;
				}
				foreach (Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo groupExpression in grouping.GroupExpressions)
				{
					if (m_publishingContext.IsRestrictedNaturalGroupSort(groupExpression))
					{
						context.ErrorContext.Register(ProcessingErrorCode.rsInvalidNaturalSortGroupExpressionNotSimpleFieldReference, Severity.Error, context.ObjectType, context.ObjectName, "GroupExpression", "NaturalGroup");
					}
				}
			}
			if (grouping.DomainScope != null)
			{
				if (grouping.Parent != null)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidGroupingDomainScopeWithParent, Severity.Error, context.ObjectType, context.ObjectName, "DomainScope", grouping.Name.MarkAsModelInfo(), grouping.DomainScope.MarkAsPrivate());
				}
				else if (grouping.GroupExpressions.Count == 0)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidGroupingDomainScopeWithDetailGroup, Severity.Error, context.ObjectType, context.ObjectName, "DomainScope", grouping.Name.MarkAsModelInfo(), grouping.DomainScope.MarkAsPrivate());
				}
				else
				{
					m_domainScopeGroups.Add(grouping);
				}
			}
			return grouping;
		}

		private void ReadGroupExpressions(List<Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo> groupExpressions, PublishingContextStruct context)
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
						groupExpressions.Add(ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.GroupExpression, DataType.String, context));
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

		private List<Variable> ReadVariables(PublishingContextStruct context, bool isGrouping, string groupName)
		{
			List<Variable> list = new List<Variable>();
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "Variable")
					{
						list.Add(ReadVariable(context, isGrouping, groupName));
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("Variables" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (list.Count == 0)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidVariableCount, Severity.Error, context.ObjectType, context.ObjectName, "Variables");
			}
			else if (m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.Variables))
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, context.ObjectType, context.ObjectName, "Variables");
			}
			return list;
		}

		private Variable ReadVariable(PublishingContextStruct context, bool isGrouping, string groupingName)
		{
			Variable variable = new Variable();
			variable.SequenceID = GenerateVariableSequenceID();
			variable.Name = m_reader.GetAttribute("Name");
			m_variableNames.Validate(variable.Name, context.ObjectType, context.ObjectName, context.ErrorContext, isGrouping, groupingName);
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
						if (!(localName == "Writable"))
						{
							break;
						}
						if ((context.Location & (LocationFlags.InDataRegion | LocationFlags.InGrouping)) != 0)
						{
							context.ErrorContext.Register(ProcessingErrorCode.rsInvalidWritableVariable, Severity.Error, context.ObjectType, context.ObjectName, variable.Name, "Variable");
							break;
						}
						variable.Writable = m_reader.ReadBoolean(context.ObjectType, variable.Name, "Writable");
						if (variable.Writable)
						{
							m_userReferenceLocation |= UserLocationFlags.ReportBody;
						}
					}
					else
					{
						variable.DataType = ReadDataTypeAttribute();
						variable.Value = ReadExpression(variable.GetPropertyName(), isGrouping ? Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.GroupVariableValue : Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.VariableValue, variable.DataType, context);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("Variable" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (variable.Value == null)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsMissingExpression, Severity.Error, context.ObjectType, context.ObjectName, "Variable");
			}
			return variable;
		}

		private List<IdcRelationship> ReadRelationships(PublishingContextStruct context)
		{
			List<IdcRelationship> list = new List<IdcRelationship>();
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "Relationship")
					{
						list.Add(ReadRelationship(context));
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("Relationships" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (list.Count == 0)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsElementMustContainChildren, Severity.Error, context.ObjectType, context.ObjectName, "Relationships", "Relationship");
			}
			return list;
		}

		private IdcRelationship ReadRelationship(PublishingContextStruct context)
		{
			IdcRelationship idcRelationship = new IdcRelationship();
			bool flag = false;
			bool sortDirectionSpecified = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (m_reader.LocalName)
					{
					case "ParentScope":
						idcRelationship.ParentScope = m_reader.ReadString();
						break;
					case "NaturalJoin":
						idcRelationship.NaturalJoin = m_reader.ReadBoolean(context.ObjectType, context.ObjectName, m_reader.LocalName);
						break;
					case "JoinConditions":
						ReadJoinConditions(context, idcRelationship, out sortDirectionSpecified);
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("Relationship" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (!idcRelationship.NaturalJoin && sortDirectionSpecified)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidSortDirectionMustNotBeSpecified, Severity.Error, context.ObjectType, context.ObjectName, "JoinCondition", "SortDirection", "ParentScope", idcRelationship.ParentScope);
			}
			return idcRelationship;
		}

		private List<DefaultRelationship> ReadDefaultRelationships(PublishingContextStruct context)
		{
			List<DefaultRelationship> list = new List<DefaultRelationship>();
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "DefaultRelationship")
					{
						list.Add(ReadDefaultRelationship(context));
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("DefaultRelationships" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (list.Count == 0)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsElementMustContainChildren, Severity.Error, context.ObjectType, context.ObjectName, "DefaultRelationships", "DefaultRelationship");
			}
			return list;
		}

		private DefaultRelationship ReadDefaultRelationship(PublishingContextStruct context)
		{
			DefaultRelationship defaultRelationship = new DefaultRelationship();
			bool sortDirectionSpecified = false;
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (m_reader.LocalName)
					{
					case "RelatedDataSet":
						defaultRelationship.RelatedDataSetName = m_reader.ReadString();
						break;
					case "NaturalJoin":
						defaultRelationship.NaturalJoin = m_reader.ReadBoolean(context.ObjectType, context.ObjectName, m_reader.LocalName);
						break;
					case "JoinConditions":
						ReadJoinConditions(context, defaultRelationship, out sortDirectionSpecified);
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("DefaultRelationship" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (!defaultRelationship.NaturalJoin && sortDirectionSpecified)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidSortDirectionMustNotBeSpecified, Severity.Error, context.ObjectType, context.ObjectName, "JoinCondition", "SortDirection", "RelatedDataSet", defaultRelationship.RelatedDataSetName.MarkAsPrivate());
			}
			return defaultRelationship;
		}

		private void ReadJoinConditions(PublishingContextStruct context, Relationship relationship, out bool sortDirectionSpecified)
		{
			bool flag = false;
			sortDirectionSpecified = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "JoinCondition")
					{
						ReadJoinCondition(context, relationship, ref sortDirectionSpecified);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("JoinConditions" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (relationship.JoinConditionCount == 0)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsElementMustContainChildren, Severity.Error, context.ObjectType, context.ObjectName, "JoinConditions", "JoinCondition");
			}
		}

		private void ReadJoinCondition(PublishingContextStruct context, Relationship relationship, ref bool sortDirectionSpecified)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = null;
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo2 = null;
			SortDirection direction = SortDirection.Ascending;
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (m_reader.LocalName)
					{
					case "ForeignKey":
						expressionInfo = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.JoinExpression, DataType.String, context);
						break;
					case "PrimaryKey":
						expressionInfo2 = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.JoinExpression, DataType.String, context);
						break;
					case "SortDirection":
						sortDirectionSpecified = true;
						direction = ReadSortDirection();
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("JoinCondition" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (expressionInfo == null)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsElementMustContainChild, Severity.Error, context.ObjectType, context.ObjectName, "JoinCondition", "ForeignKey");
			}
			if (expressionInfo2 == null)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsElementMustContainChild, Severity.Error, context.ObjectType, context.ObjectName, "JoinCondition", "PrimaryKey");
			}
			if (expressionInfo != null && expressionInfo2 != null)
			{
				relationship.AddJoinCondition(expressionInfo, expressionInfo2, direction);
			}
		}

		private SortDirection ReadSortDirection()
		{
			return (SortDirection)Enum.Parse(typeof(SortDirection), m_reader.ReadString());
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo ReadExpression(string expression, string propertyName, string dataSetName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.EvaluationMode mode, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType expressionType, DataType constantType, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionContext context2 = context.CreateExpressionContext(expressionType, constantType, propertyName, dataSetName, m_publishingContext);
			if (!CheckUserProfileDependency())
			{
				return m_reportCT.ParseExpression(expression, mode, context2);
			}
			bool userCollectionReferenced;
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo result = m_reportCT.ParseExpression(expression, context2, mode, out userCollectionReferenced);
			if (userCollectionReferenced)
			{
				SetUserProfileDependency();
			}
			return result;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo ReadExpression(string propertyName, string dataSetName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType expressionType, DataType constantType, PublishingContextStruct context, out bool userCollectionReferenced)
		{
			Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionContext context2 = context.CreateExpressionContext(expressionType, constantType, propertyName, dataSetName, m_publishingContext);
			Microsoft.ReportingServices.RdlExpressions.ExpressionParser.EvaluationMode evaluationMode = ReadEvaluationModeAttribute();
			return m_reportCT.ParseExpression(m_reader.ReadString(), context2, evaluationMode, out userCollectionReferenced);
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo ReadExpression(string propertyName, string dataSetName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType expressionType, DataType constantType, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.RdlExpressions.ExpressionParser.EvaluationMode mode = ReadEvaluationModeAttribute();
			return ReadExpression(m_reader.ReadString(), propertyName, dataSetName, mode, expressionType, constantType, context);
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo ReadExpression(string propertyName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType expressionType, DataType constantType, PublishingContextStruct context)
		{
			return ReadExpression(propertyName, null, expressionType, constantType, context);
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo ReadExpression(string propertyName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType expressionType, DataType constantType, PublishingContextStruct context, out bool computed)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = ReadExpression(propertyName, expressionType, constantType, context);
			if (Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == expressionInfo.Type)
			{
				computed = false;
			}
			else
			{
				computed = true;
			}
			return expressionInfo;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo ReadDocumentMapLabelExpression(string propertyName, PublishingContextStruct context)
		{
			bool computed = false;
			return ReadDocumentMapLabelExpression(propertyName, context, out computed);
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo ReadDocumentMapLabelExpression(string propertyName, PublishingContextStruct context, out bool computed)
		{
			Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType expressionType = Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General;
			DataType constantType = DataType.String;
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = null;
			computed = false;
			if ((context.Location & LocationFlags.InPageSection) != 0)
			{
				if (context.ObjectType != Microsoft.ReportingServices.ReportProcessing.ObjectType.Tablix && context.ObjectType != Microsoft.ReportingServices.ReportProcessing.ObjectType.Subreport && context.ObjectType != Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart && context.ObjectType != Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel && context.ObjectType != Microsoft.ReportingServices.ReportProcessing.ObjectType.Map)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidReportItemInPageSection, Severity.Warning, context.ObjectType, context.ObjectName, "DocumentMapLabel");
				}
			}
			else
			{
				expressionInfo = ReadExpression(propertyName, expressionType, constantType, context, out computed);
				if (expressionInfo != null && (expressionInfo.Type != Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant || !string.IsNullOrEmpty(expressionInfo.StringValue)))
				{
					m_hasLabels = true;
				}
			}
			return expressionInfo;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo ReadBookmarkExpression(string propertyName, PublishingContextStruct context)
		{
			bool computed = false;
			return ReadBookmarkExpression(propertyName, context, out computed);
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo ReadBookmarkExpression(string propertyName, PublishingContextStruct context, out bool computed)
		{
			Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType expressionType = Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General;
			DataType constantType = DataType.String;
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = ReadExpression(propertyName, expressionType, constantType, context, out computed);
			if (expressionInfo != null && (expressionInfo.Type != Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant || expressionInfo.StringValue != null))
			{
				m_hasBookmarks = true;
			}
			return expressionInfo;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo ReadExpression(string expression, string propertyName, string dataSetName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType expressionType, DataType constantType, PublishingContextStruct context, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.EvaluationMode evaluationMode, out bool reportParameterReferenced, out string reportParameterName)
		{
			bool userCollectionReferenced;
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo result = ReadExpression(expression, propertyName, dataSetName, expressionType, constantType, context, evaluationMode, out reportParameterReferenced, out reportParameterName, out userCollectionReferenced);
			if (userCollectionReferenced)
			{
				SetUserProfileDependency();
			}
			return result;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo ReadExpression(string expression, string propertyName, string dataSetName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType expressionType, DataType constantType, PublishingContextStruct context, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.EvaluationMode evaluationMode, out bool reportParameterReferenced, out string reportParameterName, out bool userCollectionReferenced)
		{
			Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionContext context2 = context.CreateExpressionContext(expressionType, constantType, propertyName, dataSetName, m_publishingContext);
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = m_reportCT.ParseExpression(expression, context2, evaluationMode, out userCollectionReferenced);
			if (expressionInfo != null && expressionInfo.IsExpression)
			{
				reportParameterReferenced = true;
				reportParameterName = expressionInfo.SimpleParameterName;
			}
			else
			{
				reportParameterName = null;
				reportParameterReferenced = false;
			}
			return expressionInfo;
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

		private void SetConnectionStringUserProfileDependency()
		{
			m_userReferenceLocation |= UserLocationFlags.ReportQueries;
		}

		private Microsoft.ReportingServices.RdlExpressions.ExpressionParser.EvaluationMode ReadEvaluationModeAttribute()
		{
			if (m_reader.HasAttributes)
			{
				string attribute = m_reader.GetAttribute("EvaluationMode");
				if (attribute != null)
				{
					return (Microsoft.ReportingServices.RdlExpressions.ExpressionParser.EvaluationMode)Enum.Parse(typeof(Microsoft.ReportingServices.RdlExpressions.ExpressionParser.EvaluationMode), attribute, ignoreCase: false);
				}
			}
			return Microsoft.ReportingServices.RdlExpressions.ExpressionParser.EvaluationMode.Auto;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo ReadToggleImage(PublishingContextStruct context, out bool computed)
		{
			computed = false;
			m_static = true;
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo result = null;
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
						result = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context, out computed);
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

		private Microsoft.ReportingServices.ReportIntermediateFormat.Image ReadImage(Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem parent, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.Image image = new Microsoft.ReportingServices.ReportIntermediateFormat.Image(GenerateID(), parent);
			image.Name = m_reader.GetAttribute("Name");
			context.ObjectType = image.ObjectType;
			context.ObjectName = image.Name;
			bool flag = true;
			if (!m_reportItemNames.Validate(context.ObjectType, context.ObjectName, context.ErrorContext))
			{
				flag = false;
			}
			bool computed = false;
			bool computed2 = false;
			bool computed3 = false;
			bool computed4 = false;
			bool computedBookmark = false;
			bool computed5 = false;
			bool computed6 = false;
			bool computed7 = false;
			bool computed8 = false;
			bool computedTag = false;
			bool flag2 = false;
			Microsoft.ReportingServices.OnDemandReportRendering.Image.EmbeddingModes? embeddingMode = null;
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
						image.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, context.ErrorContext);
						break;
					}
					case "ActionInfo":
						image.Action = ReadActionInfo(context, StyleOwnerType.Image, out computed2);
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
						image.ZIndex = m_reader.ReadInteger(context.ObjectType, context.ObjectName, m_reader.LocalName);
						break;
					case "Visibility":
						image.Visibility = ReadVisibility(context, out computed3);
						break;
					case "ToolTip":
						image.ToolTip = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out computed5);
						break;
					case "DocumentMapLabel":
						image.DocumentMapLabel = ReadDocumentMapLabelExpression(m_reader.LocalName, context, out computed4);
						break;
					case "Bookmark":
						image.Bookmark = ReadBookmarkExpression(context, out computedBookmark);
						break;
					case "RepeatWith":
						image.RepeatedSibling = true;
						image.RepeatWith = m_reader.ReadString();
						break;
					case "CustomProperties":
						image.CustomProperties = ReadCustomProperties(context, out computed8);
						break;
					case "Source":
						image.Source = ReadSource();
						break;
					case "Value":
						image.Value = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out computed6);
						break;
					case "MIMEType":
						image.MIMEType = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out computed7);
						break;
					case "Sizing":
						image.Sizing = ReadSizing();
						break;
					case "Tag":
					{
						if (m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.ImageTag))
						{
							context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, context.ObjectType, context.ObjectName, "Tag");
							break;
						}
						Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo item = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out computedTag);
						if (image.Tags == null)
						{
							image.Tags = new List<Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo>(1)
							{
								item
							};
						}
						break;
					}
					case "Tags":
					{
						List<Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo> tags = ReadImageTagsCollection(context, ref computedTag);
						if (m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.ImageTagsCollection))
						{
							context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, context.ObjectType, context.ObjectName, "Tags");
						}
						else
						{
							image.Tags = tags;
						}
						break;
					}
					case "EmbeddingMode":
						embeddingMode = ReadEmbeddingMode(context);
						image.EmbeddingMode = embeddingMode.Value;
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("Image" == m_reader.LocalName)
					{
						flag2 = true;
					}
					break;
				}
			}
			while (!flag2);
			ValidateImageEmbeddingMode(context, image.Source, embeddingMode);
			if (Microsoft.ReportingServices.OnDemandReportRendering.Image.SourceType.Database == image.Source)
			{
				if (image.Tags == null || !m_publishingContext.IsRdlx)
				{
					Global.Tracer.Assert(image.Value != null);
					if (Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == image.Value.Type)
					{
						context.ErrorContext.Register(ProcessingErrorCode.rsBinaryConstant, Severity.Error, context.ObjectType, context.ObjectName, "Value");
					}
				}
				if (!PublishingValidator.ValidateMimeType(image.MIMEType, context.ObjectType, context.ObjectName, "MIMEType", context.ErrorContext))
				{
					image.MIMEType = null;
				}
			}
			else
			{
				if (image.Source == Microsoft.ReportingServices.OnDemandReportRendering.Image.SourceType.External && Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == image.Value.Type && image.Value.StringValue != null && image.Value.StringValue.Trim().Length == 0)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidEmptyImageReference, Severity.Error, context.ObjectType, context.ObjectName, "Value");
				}
				image.MIMEType = null;
			}
			image.Computed = (computed || computed2 || computed3 || computed8 || computed4 || computedBookmark || computed5 || computed6 || computed7 || computedTag);
			m_hasImageStreams = true;
			if (image.Source == Microsoft.ReportingServices.OnDemandReportRendering.Image.SourceType.External)
			{
				m_hasExternalImages = true;
			}
			if (!flag)
			{
				return null;
			}
			return image;
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo> ReadImageTagsCollection(PublishingContextStruct context, ref bool computedTag)
		{
			List<Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo>();
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
						if (localName == "Tag")
						{
							Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo item = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out computedTag);
							list.Add(item);
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (m_reader.LocalName == "Tags")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private Microsoft.ReportingServices.OnDemandReportRendering.Image.SourceType ReadSource()
		{
			string value = m_reader.ReadString();
			return (Microsoft.ReportingServices.OnDemandReportRendering.Image.SourceType)Enum.Parse(typeof(Microsoft.ReportingServices.OnDemandReportRendering.Image.SourceType), value, ignoreCase: false);
		}

		private void ReadBackgroundImage(StyleInformation styleInfo, PublishingContextStruct context, out bool computed)
		{
			bool computed2 = false;
			bool computed3 = false;
			bool computedAttribute = false;
			bool flag = false;
			bool computed4 = false;
			bool computed5 = false;
			bool computed6 = false;
			string attributeNamespace = null;
			Microsoft.ReportingServices.OnDemandReportRendering.Image.SourceType? sourceType = null;
			Microsoft.ReportingServices.OnDemandReportRendering.Image.EmbeddingModes? embeddingMode = null;
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
						sourceType = ReadSource();
						Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression2 = Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression((int)sourceType.Value);
						styleInfo.AddAttribute("BackgroundImageSource", expression2);
						if (sourceType == Microsoft.ReportingServices.OnDemandReportRendering.Image.SourceType.External)
						{
							m_hasExternalImages = true;
						}
						break;
					}
					case "Value":
						styleInfo.AddAttribute("BackgroundImageValue", ReadExpression("BackgroundImageValue", Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out computed2));
						break;
					case "MIMEType":
						styleInfo.AddAttribute("BackgroundImageMIMEType", ReadExpression("BackgroundImageMIMEType", Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out computed3));
						break;
					case "BackgroundRepeat":
					{
						if (!ReadMultiNamespaceStyleAttribute(styleInfo, context, RdlFeatures.BackgroundImageFitting, readValueType: false, ref attributeNamespace, out computedAttribute))
						{
							break;
						}
						StyleInformation.StyleInformationAttribute attributeByName = styleInfo.GetAttributeByName("BackgroundRepeat");
						if (!attributeByName.Value.IsExpression)
						{
							string stringValue = attributeByName.Value.StringValue;
							if (!Validator.ValidateBackgroundRepeatForNamespace(stringValue, attributeNamespace))
							{
								context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlPropertyValue, Severity.Error, context.ObjectType, context.ObjectName, "BackgroundRepeat", stringValue);
							}
						}
						break;
					}
					case "Transparency":
						if (m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.BackgroundImageTransparency))
						{
							context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, context.ObjectType, context.ObjectName, "Transparency");
						}
						else
						{
							styleInfo.AddAttribute("Transparency", ReadExpression("Transparency", Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context, out computed6));
						}
						break;
					case "TransparentColor":
						styleInfo.AddAttribute("TransparentColor", ReadExpression("TransparentColor", Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out computed4));
						break;
					case "Position":
						styleInfo.AddAttribute("Position", ReadExpression("Position", Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out computed5));
						break;
					case "EmbeddingMode":
					{
						embeddingMode = ReadEmbeddingMode(context);
						Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression = Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression((int)embeddingMode.Value);
						styleInfo.AddAttribute("EmbeddingMode", expression);
						break;
					}
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
			ValidateImageEmbeddingMode(context, sourceType, embeddingMode);
			computed = (computed2 || computed3 || computedAttribute || computed6 || computed4 || computed5);
			m_hasImageStreams = true;
		}

		private void ValidateImageEmbeddingMode(PublishingContextStruct context, Microsoft.ReportingServices.OnDemandReportRendering.Image.SourceType? source, Microsoft.ReportingServices.OnDemandReportRendering.Image.EmbeddingModes? embeddingMode)
		{
			if (source == Microsoft.ReportingServices.OnDemandReportRendering.Image.SourceType.Embedded)
			{
				Microsoft.ReportingServices.OnDemandReportRendering.Image.EmbeddingModes embeddingModes = embeddingMode ?? Microsoft.ReportingServices.OnDemandReportRendering.Image.EmbeddingModes.Inline;
				if (m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.Image_Embedded))
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlPropertyValue, Severity.Error, context.ObjectType, context.ObjectName, "Source", source.ToString());
				}
				if (embeddingModes == Microsoft.ReportingServices.OnDemandReportRendering.Image.EmbeddingModes.Inline && m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.EmbeddingMode_Inline))
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlPropertyValue, Severity.Error, context.ObjectType, context.ObjectName, "EmbeddingMode", embeddingModes.ToString());
				}
			}
			if (embeddingMode.HasValue && Microsoft.ReportingServices.OnDemandReportRendering.Image.SourceType.Embedded != source)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidEmbeddingModeImageProperty, Severity.Error, context.ObjectType, context.ObjectName, "EmbeddingMode");
			}
		}

		private Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.ImageInfo> ReadEmbeddedImages(PublishingContextStruct context)
		{
			Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.ImageInfo> dictionary = new Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.ImageInfo>();
			CLSUniqueNameValidator embeddedImageNames = new CLSUniqueNameValidator(ProcessingErrorCode.rsInvalidEmbeddedImageNameNotCLSCompliant, ProcessingErrorCode.rsDuplicateEmbeddedImageName, ProcessingErrorCode.rsInvalidEmbeddedImageNameLength);
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
						ReadEmbeddedImage(dictionary, embeddedImageNames, context);
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
			return dictionary;
		}

		private void ReadEmbeddedImage(Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.ImageInfo> embeddedImages, CLSUniqueNameValidator embeddedImageNames, PublishingContextStruct context)
		{
			string attribute = m_reader.GetAttribute("Name");
			context.ObjectType = Microsoft.ReportingServices.ReportProcessing.ObjectType.EmbeddedImage;
			context.ObjectName = attribute;
			embeddedImageNames.Validate(context.ObjectType, context.ObjectName, context.ErrorContext);
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
								context.ErrorContext.Register(ProcessingErrorCode.rsInvalidEmbeddedImage, Severity.Error, context.ObjectType, context.ObjectName, "ImageData");
							}
						}
					}
					else
					{
						text = m_reader.ReadString();
						if (!PublishingValidator.ValidateMimeType(text, context.ObjectType, context.ObjectName, m_reader.LocalName, context.ErrorContext))
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
			embeddedImages[attribute] = new Microsoft.ReportingServices.ReportIntermediateFormat.ImageInfo(text2, text);
			if (array != null && text != null && m_publishingContext.CreateChunkFactory != null)
			{
				using (Stream stream = m_publishingContext.CreateChunkFactory.CreateChunk(text2, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.StaticImage, text))
				{
					stream.Write(array, 0, array.Length);
				}
			}
		}

		private Microsoft.ReportingServices.OnDemandReportRendering.Image.Sizings ReadSizing()
		{
			string value = m_reader.ReadString();
			return (Microsoft.ReportingServices.OnDemandReportRendering.Image.Sizings)Enum.Parse(typeof(Microsoft.ReportingServices.OnDemandReportRendering.Image.Sizings), value, ignoreCase: false);
		}

		private Microsoft.ReportingServices.OnDemandReportRendering.Image.EmbeddingModes ReadEmbeddingMode(PublishingContextStruct context)
		{
			if (m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.EmbeddingMode))
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, context.ObjectType, context.ObjectName, "EmbeddingMode");
			}
			string value = m_reader.ReadString();
			return (Microsoft.ReportingServices.OnDemandReportRendering.Image.EmbeddingModes)Enum.Parse(typeof(Microsoft.ReportingServices.OnDemandReportRendering.Image.EmbeddingModes), value, ignoreCase: false);
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo ReadQueryOrParameterExpression(PublishingContextStruct context, DataType dataType, ref bool isComplex, Dictionary<string, bool> parametersInQuery)
		{
			return ReadQueryOrParameterExpression(m_reader.ReadString(), m_reader.LocalName, ReadEvaluationModeAttribute(), context, dataType, ref isComplex, parametersInQuery);
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo ReadQueryOrParameterExpression(string expression, string propertyName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.EvaluationMode evaluationMode, PublishingContextStruct context, DataType dataType, ref bool isComplex, Dictionary<string, bool> parametersInQuery)
		{
			Global.Tracer.Assert(Microsoft.ReportingServices.ReportProcessing.ObjectType.QueryParameter == context.ObjectType || Microsoft.ReportingServices.ReportProcessing.ObjectType.Query == context.ObjectType || Microsoft.ReportingServices.ReportProcessing.ObjectType.DataSource == context.ObjectType);
			m_reportLocationFlags = UserLocationFlags.ReportQueries;
			bool reportParameterReferenced;
			string reportParameterName;
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo result = ReadExpression(expression, propertyName, context.ObjectName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.QueryParameter, dataType, context, evaluationMode, out reportParameterReferenced, out reportParameterName);
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
						parametersInQuery[reportParameterName] = true;
					}
				}
			}
			m_reportLocationFlags = UserLocationFlags.ReportBody;
			return result;
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.ParameterDef> ReadReportParameters(PublishingContextStruct context)
		{
			List<Microsoft.ReportingServices.ReportIntermediateFormat.ParameterDef> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.ParameterDef>();
			CLSUniqueNameValidator reportParameterNames = new CLSUniqueNameValidator(ProcessingErrorCode.rsInvalidNameNotCLSCompliant, ProcessingErrorCode.rsDuplicateReportParameterName, ProcessingErrorCode.rsInvalidParameterNameLength, ProcessingErrorCode.rsDuplicateCaseInsensitiveReportParameterName);
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
						list.Add(ReadReportParameter(reportParameterNames, parameterNames, context, num));
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
			return list;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ParameterDef ReadReportParameter(CLSUniqueNameValidator reportParameterNames, Hashtable parameterNames, PublishingContextStruct context, int count)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ParameterDef parameterDef = new Microsoft.ReportingServices.ReportIntermediateFormat.ParameterDef(GenerateID());
			parameterDef.Name = m_reader.GetAttribute("Name");
			context.ObjectType = Microsoft.ReportingServices.ReportProcessing.ObjectType.ReportParameter;
			context.ObjectName = parameterDef.Name;
			reportParameterNames.Validate(context.ObjectType, context.ObjectName, context.ErrorContext);
			string type = null;
			string nullable = null;
			bool flag = false;
			string allowBlank = null;
			bool flag2 = false;
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo prompt = null;
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
						prompt = Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(m_reader.ReadString());
						break;
					case "ValidValues":
						flag3 = ReadValidValues(context, parameterDef, parameterNames, ref isComplex, out validValueDataSet);
						break;
					case "Hidden":
						flag4 = m_reader.ReadBoolean(context.ObjectType, context.ObjectName, m_reader.LocalName);
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
			if (isComplex && parameterNames.Count > 0)
			{
				parameterDef.Dependencies = (Hashtable)parameterNames.Clone();
			}
			parameterDef.Parse(parameterDef.Name, list, type, nullable, prompt, null, allowBlank, multiValue, usedInQuery, flag4, context.ErrorContext, CultureInfo.InvariantCulture);
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
				context.ErrorContext.Register(ProcessingErrorCode.rsMissingParameterDefault, Severity.Error, context.ObjectType, context.ObjectName, null);
			}
			if (parameterDef.Nullable && parameterDef.MultiValue)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidMultiValueParameter, Severity.Error, context.ObjectType, context.ObjectName, null);
			}
			if (!parameterDef.MultiValue && list != null && list.Count > 1)
			{
				list.RemoveRange(1, list.Count - 1);
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidDefaultValueValues, Severity.Warning, context.ObjectType, context.ObjectName, null);
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

		private List<string> ReadDefaultValue(PublishingContextStruct context, Microsoft.ReportingServices.ReportIntermediateFormat.ParameterDef parameter, Hashtable parameterNames, ref bool isComplex, out DataSetReference defaultDataSet)
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
								result = ReadValues(context, parameter, parameterNames, ref isComplex);
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
			if (flag == flag2)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidDefaultValue, Severity.Error, context.ObjectType, context.ObjectName, "DefaultValue");
			}
			return result;
		}

		private List<string> ReadValues(PublishingContextStruct context, Microsoft.ReportingServices.ReportIntermediateFormat.ParameterDef parameter, Hashtable parameterNames, ref bool isComplex)
		{
			List<string> list = null;
			List<Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo> list2 = new List<Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo>();
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = null;
			bool dynamic = false;
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
						string attribute = m_reader.GetAttribute("nil", "http://www.w3.org/2001/XMLSchema-instance");
						expressionInfo = ((attribute == null || !XmlConvert.ToBoolean(attribute)) ? ReadParameterExpression(m_reader.LocalName, context, parameter, parameterNames, ref dynamic, ref isComplex) : Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(null));
						list2.Add(expressionInfo);
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
			if (dynamic)
			{
				parameter.DefaultExpressions = list2;
			}
			else
			{
				list = new List<string>(list2.Count);
				for (int i = 0; i < list2.Count; i++)
				{
					list.Add(list2[i].StringValue);
				}
			}
			return list;
		}

		private bool ReadValidValues(PublishingContextStruct context, Microsoft.ReportingServices.ReportIntermediateFormat.ParameterDef parameter, Hashtable parameterNames, ref bool isComplex, out DataSetReference validValueDataSet)
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
			if (flag == flag2)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidValidValues, Severity.Error, context.ObjectType, context.ObjectName, "ValidValues");
			}
			return containsExplicitNull;
		}

		private void ReadParameterValues(PublishingContextStruct context, Microsoft.ReportingServices.ReportIntermediateFormat.ParameterDef parameter, Hashtable parameterNames, ref bool isComplex, ref bool containsExplicitNull)
		{
			List<Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo>();
			List<Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo> list2 = new List<Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo>();
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
					Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = null;
					Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo item = null;
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
										item = ReadParameterExpression(m_reader.LocalName, context, parameter, parameterNames, ref dynamic, ref isComplex);
									}
								}
								else
								{
									expressionInfo = ReadParameterExpression(m_reader.LocalName, context, parameter, parameterNames, ref dynamic, ref isComplex);
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
					list.Add(expressionInfo);
					list2.Add(item);
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
			parameter.ValidValuesValueExpressions = list;
			parameter.ValidValuesLabelExpressions = list2;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo ReadParameterExpression(string propertyName, PublishingContextStruct context, Microsoft.ReportingServices.ReportIntermediateFormat.ParameterDef parameter, Hashtable parameterNames, ref bool dynamic, ref bool isComplex)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = null;
			string reportParameterName = null;
			bool flag = false;
			bool userCollectionReferenced;
			if (isComplex)
			{
				dynamic = true;
				expressionInfo = ReadExpression(propertyName, null, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.ReportParameter, DataType.String, context, out userCollectionReferenced);
			}
			else
			{
				expressionInfo = ReadExpression(m_reader.ReadString(), propertyName, null, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.ReportParameter, DataType.String, context, ReadEvaluationModeAttribute(), out bool reportParameterReferenced, out reportParameterName, out userCollectionReferenced);
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
						if (parameter.Dependencies == null)
						{
							parameter.Dependencies = new Hashtable();
						}
						if (!parameter.Dependencies.ContainsKey(reportParameterName))
						{
							parameter.Dependencies.Add(reportParameterName, parameterNames[reportParameterName]);
						}
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
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidReportParameterDependency, Severity.Error, Microsoft.ReportingServices.ReportProcessing.ObjectType.ReportParameter, parameter.Name, "ValidValues", reportParameterName.MarkAsPrivate());
			}
			return expressionInfo;
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.ParameterValue> ReadParameters(PublishingContextStruct context, bool doClsValidation)
		{
			bool computed;
			return ReadParameters(context, omitAllowed: false, doClsValidation, isSubreportParameter: true, out computed);
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.ParameterValue> ReadParameters(PublishingContextStruct context, bool omitAllowed, bool doClsValidation, bool isSubreportParameter, out bool computed)
		{
			computed = false;
			List<Microsoft.ReportingServices.ReportIntermediateFormat.ParameterValue> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.ParameterValue>();
			ParameterNameValidator parameterNames = new ParameterNameValidator();
			string propertyNamePrefix = isSubreportParameter ? "SubreportParameters" : "DrillthroughParameters";
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
						list.Add(ReadParameter(parameterNames, context, propertyNamePrefix, omitAllowed, doClsValidation, isSubreportParameter, out bool computed2));
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
			return list;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ParameterValue ReadParameter(ParameterNameValidator parameterNames, PublishingContextStruct context, string propertyNamePrefix, bool omitAllowed, bool doClsValidation, bool isSubreportParameter, out bool computed)
		{
			Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType expressionType = isSubreportParameter ? Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.SubReportParameter : Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General;
			computed = false;
			bool computed2 = false;
			bool computed3 = false;
			Microsoft.ReportingServices.ReportIntermediateFormat.ParameterValue parameterValue = new Microsoft.ReportingServices.ReportIntermediateFormat.ParameterValue();
			parameterValue.Name = m_reader.GetAttribute("Name");
			if (doClsValidation)
			{
				parameterNames.Validate(parameterValue.Name, context.ObjectType, context.ObjectName, context.ErrorContext);
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
							parameterValue.Omit = ReadExpression(propertyNamePrefix + "." + m_reader.LocalName, expressionType, DataType.Boolean, context, out computed3);
						}
					}
					else
					{
						parameterValue.Value = ReadExpression(propertyNamePrefix + "." + m_reader.LocalName, expressionType, DataType.String, context, out computed2);
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

		private ParametersGridLayout ReadReportParametersLayout(PublishingContextStruct context, List<Microsoft.ReportingServices.ReportIntermediateFormat.ParameterDef> parameters)
		{
			ParametersGridLayout result = null;
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "GridLayoutDefinition")
					{
						result = ReadGridLayoutDefinition(context);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("ReportParametersLayout" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return result;
		}

		private ParametersGridLayout ReadGridLayoutDefinition(PublishingContextStruct context)
		{
			ParametersGridLayout parametersGridLayout = new ParametersGridLayout();
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (m_reader.LocalName)
					{
					case "NumberOfColumns":
						parametersGridLayout.NumberOfColumns = m_reader.ReadInteger(Microsoft.ReportingServices.ReportProcessing.ObjectType.ParameterLayout, "GridLayoutDefinition", "NumberOfColumns");
						break;
					case "NumberOfRows":
						parametersGridLayout.NumberOfRows = m_reader.ReadInteger(Microsoft.ReportingServices.ReportProcessing.ObjectType.ParameterLayout, "GridLayoutDefinition", "NumberOfRows");
						break;
					case "CellDefinitions":
						parametersGridLayout.CellDefinitions = ReadCellDefinitions(context);
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("GridLayoutDefinition" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return parametersGridLayout;
		}

		private ParametersGridCellDefinitionList ReadCellDefinitions(PublishingContextStruct context)
		{
			ParametersGridCellDefinitionList parametersGridCellDefinitionList = new ParametersGridCellDefinitionList();
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "CellDefinition")
					{
						parametersGridCellDefinitionList.Add(ReadCellDefinition(context));
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("CellDefinitions" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return parametersGridCellDefinitionList;
		}

		private ParameterGridLayoutCellDefinition ReadCellDefinition(PublishingContextStruct context)
		{
			ParameterGridLayoutCellDefinition parameterGridLayoutCellDefinition = new ParameterGridLayoutCellDefinition();
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (m_reader.LocalName)
					{
					case "RowIndex":
						parameterGridLayoutCellDefinition.RowIndex = m_reader.ReadInteger(Microsoft.ReportingServices.ReportProcessing.ObjectType.ParameterLayout, "CellDefinition", "RowIndex");
						break;
					case "ColumnIndex":
						parameterGridLayoutCellDefinition.ColumnIndex = m_reader.ReadInteger(Microsoft.ReportingServices.ReportProcessing.ObjectType.ParameterLayout, "CellDefinition", "ColumnIndex");
						break;
					case "ParameterName":
						parameterGridLayoutCellDefinition.ParameterName = m_reader.ReadString();
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("CellDefinition" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return parameterGridLayoutCellDefinition;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.TextBox ReadTextbox(Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem parent, PublishingContextStruct context, List<Microsoft.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.TextBox textBox = new Microsoft.ReportingServices.ReportIntermediateFormat.TextBox(GenerateID(), parent);
			textBox.Name = m_reader.GetAttribute("Name");
			textBox.SequenceID = GenerateTextboxSequenceID();
			context.ObjectType = textBox.ObjectType;
			context.ObjectName = textBox.Name;
			bool flag = true;
			if (!m_reportItemNames.Validate(context.ObjectType, context.ObjectName, context.ErrorContext))
			{
				flag = false;
			}
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
						if (m_reportCT.ValueReferenced)
						{
							textBox.ValueReferenced = true;
							m_reportCT.ResetValueReferencedFlag();
						}
						StyleInformation styleInformation = ReadStyle(context, out computed);
						styleInformation.Filter(StyleOwnerType.TextBox, hasNoRows: false);
						textBox.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, textBox.ObjectType, textBox.Name, context.ErrorContext, m_reportCT.ValueReferenced && !textBox.ValueReferenced, out bool meDotValueReferenced);
						if (meDotValueReferenced)
						{
							textBox.ValueReferenced = true;
						}
						m_reportCT.ResetValueReferencedFlag();
						break;
					}
					case "ActionInfo":
						textBox.Action = ReadActionInfo(context, StyleOwnerType.TextBox, out computed2);
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
						textBox.ZIndex = m_reader.ReadInteger(context.ObjectType, context.ObjectName, m_reader.LocalName);
						break;
					case "Visibility":
						textBox.Visibility = ReadVisibility(context, out computed3);
						break;
					case "ToolTip":
						textBox.ToolTip = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out computed5);
						break;
					case "DocumentMapLabel":
						textBox.DocumentMapLabel = ReadDocumentMapLabelExpression(m_reader.LocalName, context, out computed4);
						break;
					case "Bookmark":
						textBox.Bookmark = ReadBookmarkExpression(context, out computedBookmark);
						break;
					case "RepeatWith":
						textBox.RepeatedSibling = true;
						textBox.RepeatWith = m_reader.ReadString();
						break;
					case "CustomProperties":
						textBox.CustomProperties = ReadCustomProperties(context, out computed8);
						break;
					case "Paragraphs":
						if (m_reportCT.ValueReferenced)
						{
							textBox.ValueReferenced = true;
							m_reportCT.ResetValueReferencedFlag();
						}
						textBox.Paragraphs = ReadParagraphs(context, textBox, out computed6);
						break;
					case "CanScrollVertically":
						textBox.CanScrollVertically = m_reader.ReadBoolean(context.ObjectType, context.ObjectName, m_reader.LocalName);
						break;
					case "CanGrow":
						textBox.CanGrow = m_reader.ReadBoolean(context.ObjectType, context.ObjectName, m_reader.LocalName);
						break;
					case "CanShrink":
						textBox.CanShrink = m_reader.ReadBoolean(context.ObjectType, context.ObjectName, m_reader.LocalName);
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
						textBox.DataElementOutput = ReadDataElementOutput();
						break;
					case "DataElementStyle":
					{
						Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem.DataElementStyles dataElementStyles = ReadDataElementStyleRDL();
						if (Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem.DataElementStyles.Auto != dataElementStyles)
						{
							textBox.OverrideReportDataElementStyle = true;
							textBox.DataElementStyleAttribute = (dataElementStyles == Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem.DataElementStyles.Attribute);
						}
						break;
					}
					case "KeepTogether":
						textBox.KeepTogether = m_reader.ReadBoolean(context.ObjectType, context.ObjectName, m_reader.LocalName);
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("Textbox" == m_reader.LocalName)
					{
						flag2 = true;
					}
					break;
				}
			}
			while (!flag2);
			textBox.Computed = (computed || computed2 || computed3 || computed8 || computed4 || computedBookmark || computed5 || computed6 || computed7 || textBox.UserSort != null || textBox.HideDuplicates != null);
			textBox.ValueReferenced |= m_reportCT.ValueReferenced;
			m_reportCT.ResetValueReferencedFlag();
			if (!flag)
			{
				return null;
			}
			return textBox;
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.Paragraph> ReadParagraphs(PublishingContextStruct context, Microsoft.ReportingServices.ReportIntermediateFormat.TextBox textbox, out bool computed)
		{
			List<Microsoft.ReportingServices.ReportIntermediateFormat.Paragraph> paragraphs = textbox.Paragraphs;
			computed = false;
			int num = 0;
			bool flag = false;
			if (!m_reader.IsEmptyElement)
			{
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName = m_reader.LocalName;
						if (localName == "Paragraph")
						{
							Microsoft.ReportingServices.ReportIntermediateFormat.Paragraph paragraph = ReadParagraph(context, textbox, num, ref computed);
							paragraphs.Add(paragraph);
							if (paragraph.TextRunValueReferenced)
							{
								textbox.TextRunValueReferenced = true;
							}
							num++;
						}
						break;
					}
					case XmlNodeType.EndElement:
						if ("Paragraphs" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			if (paragraphs.Count > 0)
			{
				return paragraphs;
			}
			return null;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.Paragraph ReadParagraph(PublishingContextStruct context, Microsoft.ReportingServices.ReportIntermediateFormat.TextBox textbox, int index, ref bool computed)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.Paragraph paragraph = new Microsoft.ReportingServices.ReportIntermediateFormat.Paragraph(textbox, index, GenerateID());
			context.ObjectType = paragraph.ObjectType;
			context.ObjectName = paragraph.Name;
			bool flag = false;
			bool computed2 = false;
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
						case "TextRuns":
							paragraph.TextRuns = ReadTextRuns(context, paragraph, index, ref computed);
							break;
						case "Style":
						{
							StyleInformation styleInformation = ReadStyle(context, out computed2);
							computed |= computed2;
							styleInformation.Filter(StyleOwnerType.Paragraph, hasNoRows: false);
							paragraph.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, paragraph.ObjectType, paragraph.Name, context.ErrorContext);
							break;
						}
						case "LeftIndent":
							paragraph.LeftIndent = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out computed2);
							computed |= computed2;
							if (!computed2)
							{
								PublishingValidator.ValidateSize(paragraph.LeftIndent.StringValue, paragraph.ObjectType, paragraph.Name, "LeftIndent", context.ErrorContext);
							}
							break;
						case "RightIndent":
							paragraph.RightIndent = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out computed2);
							computed |= computed2;
							if (!computed2)
							{
								PublishingValidator.ValidateSize(paragraph.RightIndent.StringValue, paragraph.ObjectType, paragraph.Name, "RightIndent", context.ErrorContext);
							}
							break;
						case "HangingIndent":
							paragraph.HangingIndent = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out computed2);
							computed |= computed2;
							if (!computed2)
							{
								PublishingValidator.ValidateSize(paragraph.HangingIndent.StringValue, paragraph.ObjectType, paragraph.Name, "HangingIndent", restrictMaxValue: false, allowNegative: true, context.ErrorContext, out double _, out string _);
							}
							break;
						case "SpaceBefore":
							paragraph.SpaceBefore = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out computed2);
							computed |= computed2;
							if (!computed2)
							{
								PublishingValidator.ValidateSize(paragraph.SpaceBefore.StringValue, paragraph.ObjectType, paragraph.Name, "SpaceBefore", context.ErrorContext);
							}
							break;
						case "SpaceAfter":
							paragraph.SpaceAfter = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out computed2);
							computed |= computed2;
							if (!computed2)
							{
								PublishingValidator.ValidateSize(paragraph.SpaceAfter.StringValue, paragraph.ObjectType, paragraph.Name, "SpaceAfter", context.ErrorContext);
							}
							break;
						case "ListLevel":
						{
							Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo2 = paragraph.ListLevel = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context, out computed2);
							computed |= computed2;
							if (expressionInfo2 != null && !expressionInfo2.IsExpression && !Validator.ValidateParagraphListLevel(expressionInfo2.IntValue, out int? _))
							{
								context.ErrorContext.Register(ProcessingErrorCode.rsOutOfRangeSize, Severity.Error, paragraph.ObjectType, paragraph.Name, "ListLevel", expressionInfo2.OriginalText.MarkAsPrivate(), Convert.ToString(0, CultureInfo.InvariantCulture), Convert.ToString(9, CultureInfo.InvariantCulture));
							}
							break;
						}
						case "ListStyle":
							paragraph.ListStyle = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out computed2);
							computed |= computed2;
							if (!computed2)
							{
								PublishingValidator.ValidateParagraphListStyle(paragraph.ListStyle.StringValue, paragraph.ObjectType, paragraph.Name, "ListStyle", context.ErrorContext);
							}
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("Paragraph" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return paragraph;
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.TextRun> ReadTextRuns(PublishingContextStruct context, Microsoft.ReportingServices.ReportIntermediateFormat.Paragraph paragraph, int paragraphIndex, ref bool computed)
		{
			List<Microsoft.ReportingServices.ReportIntermediateFormat.TextRun> textRuns = paragraph.TextRuns;
			int num = 0;
			bool flag = false;
			if (!m_reader.IsEmptyElement)
			{
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName = m_reader.LocalName;
						if (localName == "TextRun")
						{
							Microsoft.ReportingServices.ReportIntermediateFormat.TextRun textRun = ReadTextRun(context, paragraph, paragraphIndex, num, ref computed);
							textRuns.Add(textRun);
							if (textRun.ValueReferenced)
							{
								paragraph.TextRunValueReferenced = true;
							}
							num++;
						}
						break;
					}
					case XmlNodeType.EndElement:
						if ("TextRuns" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			if (textRuns.Count > 0)
			{
				return textRuns;
			}
			return null;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.TextRun ReadTextRun(PublishingContextStruct context, Microsoft.ReportingServices.ReportIntermediateFormat.Paragraph paragraph, int paragraphIndex, int index, ref bool computed)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.TextRun textRun = new Microsoft.ReportingServices.ReportIntermediateFormat.TextRun(paragraph, index, GenerateID());
			context.ObjectType = textRun.ObjectType;
			context.ObjectName = textRun.Name;
			bool flag = false;
			bool computed2 = false;
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
						case "ActionInfo":
							textRun.Action = ReadActionInfo(context, StyleOwnerType.TextRun, out computed2);
							computed |= computed2;
							break;
						case "Style":
						{
							if (m_reportCT.ValueReferenced)
							{
								textRun.ValueReferenced = true;
								m_reportCT.ResetValueReferencedFlag();
							}
							StyleInformation styleInformation = ReadStyle(context, out computed2);
							computed |= computed2;
							styleInformation.Filter(StyleOwnerType.TextRun, hasNoRows: false);
							textRun.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, textRun.ObjectType, textRun.Name, context.ErrorContext, m_reportCT.ValueReferenced && !textRun.ValueReferenced, out bool meDotValueReferenced);
							if (meDotValueReferenced)
							{
								textRun.ValueReferenced = true;
							}
							m_reportCT.ResetValueReferencedFlag();
							break;
						}
						case "Value":
							textRun.DataType = ReadDataTypeAttribute();
							textRun.Value = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, textRun.DataType, context, out computed2);
							computed |= computed2;
							break;
						case "Label":
							textRun.Label = m_reader.ReadString();
							break;
						case "ToolTip":
							textRun.ToolTip = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out computed2);
							computed |= computed2;
							break;
						case "MarkupType":
							textRun.MarkupType = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out computed2);
							computed |= computed2;
							if (!computed2)
							{
								PublishingValidator.ValidateTextRunMarkupType(textRun.MarkupType.StringValue, textRun.ObjectType, textRun.Name, "MarkupType", context.ErrorContext);
							}
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("TextRun" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			if (m_reportCT.ValueReferenced)
			{
				textRun.ValueReferenced = true;
				m_reportCT.ResetValueReferencedFlag();
			}
			return textRun;
		}

		private void ReadUserSort(PublishingContextStruct context, Microsoft.ReportingServices.ReportIntermediateFormat.TextBox textbox, List<Microsoft.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget)
		{
			bool flag = (context.Location & LocationFlags.InPageSection) != 0;
			bool flag2 = false;
			Microsoft.ReportingServices.ReportIntermediateFormat.EndUserSort endUserSort = new Microsoft.ReportingServices.ReportIntermediateFormat.EndUserSort();
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
						endUserSort.SortExpression = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.UserSortExpression, DataType.String, context, out bool _);
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
			if (m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.UserSort))
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, Microsoft.ReportingServices.ReportProcessing.ObjectType.Textbox, textbox.Name, "UserSort");
				return;
			}
			if (flag)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidTextboxInPageSection, Severity.Error, textbox.ObjectType, textbox.Name, "UserSort");
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

		private Microsoft.ReportingServices.ReportIntermediateFormat.Rectangle ReadRectangle(Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem parent, PublishingContextStruct context, List<Microsoft.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.Rectangle rectangle = new Microsoft.ReportingServices.ReportIntermediateFormat.Rectangle(GenerateID(), GenerateID(), parent);
			rectangle.Name = m_reader.GetAttribute("Name");
			context.ObjectType = rectangle.ObjectType;
			context.ObjectName = rectangle.Name;
			bool flag = true;
			if (!m_reportItemNames.Validate(context.ObjectType, context.ObjectName, context.ErrorContext))
			{
				flag = false;
			}
			m_reportItemCollectionList.Add(rectangle.ReportItems);
			bool computed = false;
			bool computed2 = false;
			bool computed3 = false;
			bool computedBookmark = false;
			bool computed4 = false;
			bool computed5 = false;
			bool computed6 = false;
			bool flag2 = false;
			string text = null;
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = null;
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
							rectangle.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, context.ErrorContext);
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
							rectangle.ZIndex = m_reader.ReadInteger(context.ObjectType, context.ObjectName, m_reader.LocalName);
							break;
						case "Visibility":
							rectangle.Visibility = ReadVisibility(context, out computed2);
							break;
						case "ToolTip":
							rectangle.ToolTip = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out computed4);
							break;
						case "DocumentMapLabel":
							expressionInfo = (rectangle.DocumentMapLabel = ReadDocumentMapLabelExpression(m_reader.LocalName, context, out computed3));
							break;
						case "LinkToChild":
							text = m_reader.ReadString();
							break;
						case "Bookmark":
							rectangle.Bookmark = ReadBookmarkExpression(context, out computedBookmark);
							break;
						case "RepeatWith":
							rectangle.RepeatedSibling = true;
							rectangle.RepeatWith = m_reader.ReadString();
							break;
						case "CustomProperties":
							rectangle.CustomProperties = ReadCustomProperties(context, out computed6);
							break;
						case "ReportItems":
							ReadReportItems(null, rectangle, rectangle.ReportItems, context, textBoxesWithDefaultSortTarget, out computed5);
							break;
						case "PageBreak":
							ReadPageBreak(rectangle, context);
							break;
						case "PageName":
						{
							rectangle.PageName = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out bool _);
							break;
						}
						case "KeepTogether":
							rectangle.KeepTogether = m_reader.ReadBoolean(context.ObjectType, context.ObjectName, m_reader.LocalName);
							break;
						case "DataElementName":
							rectangle.DataElementName = m_reader.ReadString();
							break;
						case "DataElementOutput":
							rectangle.DataElementOutput = ReadDataElementOutput();
							break;
						case "OmitBorderOnPageBreak":
							rectangle.OmitBorderOnPageBreak = m_reader.ReadBoolean(context.ObjectType, context.ObjectName, m_reader.LocalName);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("Rectangle" == m_reader.LocalName)
						{
							flag2 = true;
						}
						break;
					}
				}
				while (!flag2);
			}
			rectangle.Computed = (computed || computed2 || computed3 || computedBookmark || computed5 || computed4 || computed6 || (rectangle.PageBreak != null && rectangle.PageBreak.BreakLocation != PageBreakLocation.None));
			if (expressionInfo != null && text != null)
			{
				rectangle.ReportItems.LinkToChild = text;
			}
			if (!flag)
			{
				return null;
			}
			return rectangle;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.Line ReadLine(Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem parent, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.Line line = new Microsoft.ReportingServices.ReportIntermediateFormat.Line(GenerateID(), parent);
			line.Name = m_reader.GetAttribute("Name");
			context.ObjectType = line.ObjectType;
			context.ObjectName = line.Name;
			bool flag = true;
			if (!m_reportItemNames.Validate(context.ObjectType, context.ObjectName, context.ErrorContext))
			{
				flag = false;
			}
			bool computed = false;
			bool computed2 = false;
			bool computed3 = false;
			bool computedBookmark = false;
			bool computed4 = false;
			bool flag2 = false;
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
							line.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, context.ErrorContext);
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
							line.ZIndex = m_reader.ReadInteger(context.ObjectType, context.ObjectName, m_reader.LocalName);
							break;
						case "Visibility":
							line.Visibility = ReadVisibility(context, out computed2);
							break;
						case "DocumentMapLabel":
							line.DocumentMapLabel = ReadDocumentMapLabelExpression(m_reader.LocalName, context, out computed3);
							break;
						case "Bookmark":
							line.Bookmark = ReadBookmarkExpression(context, out computedBookmark);
							break;
						case "RepeatWith":
							line.RepeatedSibling = true;
							line.RepeatWith = m_reader.ReadString();
							break;
						case "CustomProperties":
							line.CustomProperties = ReadCustomProperties(context, out computed4);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("Line" == m_reader.LocalName)
						{
							flag2 = true;
						}
						break;
					}
				}
				while (!flag2);
			}
			line.Computed = (computed || computed2 || computed3 || computedBookmark || computed4);
			if (!flag)
			{
				return null;
			}
			return line;
		}

		private StyleInformation ReadStyle(PublishingContextStruct context, out bool computed)
		{
			computed = false;
			StyleInformation styleInformation = new StyleInformation();
			if (!m_reader.IsEmptyElement)
			{
				string attributeNamespace = null;
				string attributeNamespace2 = null;
				string attributeNamespace3 = null;
				string attributeNamespace4 = null;
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
						case "Border":
							ReadBorderAttributes("", styleInformation, context, out computed2);
							break;
						case "TopBorder":
							ReadBorderAttributes("Top", styleInformation, context, out computed2);
							break;
						case "BottomBorder":
							ReadBorderAttributes("Bottom", styleInformation, context, out computed2);
							break;
						case "LeftBorder":
							ReadBorderAttributes("Left", styleInformation, context, out computed2);
							break;
						case "RightBorder":
							ReadBorderAttributes("Right", styleInformation, context, out computed2);
							break;
						case "BackgroundImage":
							ReadBackgroundImage(styleInformation, context, out computed2);
							break;
						case "FontFamily":
							ReadMultiNamespaceStyleAttribute(styleInformation, context, RdlFeatures.ThemeFonts, readValueType: true, ref attributeNamespace, out computed2);
							break;
						case "BackgroundColor":
							ReadMultiNamespaceStyleAttribute(styleInformation, context, RdlFeatures.ThemeColors, readValueType: true, ref attributeNamespace3, out computed2);
							break;
						case "Color":
							ReadMultiNamespaceStyleAttribute(styleInformation, context, RdlFeatures.ThemeColors, readValueType: true, ref attributeNamespace2, out computed2);
							break;
						case "CurrencyLanguage":
							ReadMultiNamespaceStyleAttribute(styleInformation, context, RdlFeatures.CellLevelFormatting, readValueType: true, ref attributeNamespace4, out computed2);
							break;
						case "Format":
						case "Language":
						case "Calendar":
						case "NumeralLanguage":
						case "BackgroundGradientType":
						case "BackgroundGradientEndColor":
						case "FontStyle":
						case "FontSize":
						case "FontWeight":
						case "TextDecoration":
						case "TextAlign":
						case "VerticalAlign":
						case "PaddingLeft":
						case "PaddingRight":
						case "PaddingTop":
						case "PaddingBottom":
						case "LineHeight":
						case "Direction":
						case "WritingMode":
						case "UnicodeBiDi":
						case "TextEffect":
						case "ShadowColor":
						case "ShadowOffset":
						case "BackgroundHatchType":
							styleInformation.AddAttribute(m_reader.LocalName, ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out computed2));
							break;
						case "NumeralVariant":
							styleInformation.AddAttribute(m_reader.LocalName, ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context, out computed2));
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

		private bool ReadMultiNamespaceStyleAttribute(StyleInformation styleInfo, PublishingContextStruct context, RdlFeatures feature, bool readValueType, ref string attributeNamespace, out bool computedAttribute)
		{
			computedAttribute = false;
			string localName = m_reader.LocalName;
			if (attributeNamespace == null || RdlNamespaceComparer.Instance.Compare(m_reader.NamespaceURI, attributeNamespace) > 0)
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.ValueType valueType = Microsoft.ReportingServices.ReportIntermediateFormat.ValueType.Constant;
				if (m_reader.NamespaceURI == "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition" || m_reader.NamespaceURI == "http://schemas.microsoft.com/sqlserver/reporting/2013/01/reportdefinition")
				{
					if (m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(feature))
					{
						context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, context.ObjectType, context.ObjectName, localName);
					}
					if (readValueType)
					{
						string attribute = m_reader.GetAttribute("ValueType", m_reader.NamespaceURI);
						if (!string.IsNullOrEmpty(attribute))
						{
							valueType = (Microsoft.ReportingServices.ReportIntermediateFormat.ValueType)Enum.Parse(typeof(Microsoft.ReportingServices.ReportIntermediateFormat.ValueType), attribute);
						}
					}
				}
				if (attributeNamespace != null)
				{
					styleInfo.RemoveAttribute(localName);
				}
				attributeNamespace = m_reader.NamespaceURI;
				Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = ReadExpression(localName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out computedAttribute);
				if (feature == RdlFeatures.ThemeFonts && string.IsNullOrEmpty(expressionInfo.StringValue))
				{
					expressionInfo.StringValue = "Arial";
				}
				styleInfo.AddAttribute(localName, expressionInfo, valueType);
				return true;
			}
			return false;
		}

		private StyleInformation ReadStyle(PublishingContextStruct context)
		{
			bool computed;
			return ReadStyle(context, out computed);
		}

		private void ReadBorderAttributes(string borderLocation, StyleInformation styleInfo, PublishingContextStruct context, out bool computed)
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
					bool computed2 = false;
					switch (m_reader.LocalName)
					{
					case "Color":
						styleInfo.AddAttribute("BorderColor" + borderLocation, ReadExpression("BorderColor", Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out computed2));
						break;
					case "Style":
						styleInfo.AddAttribute("BorderStyle" + borderLocation, ReadExpression("BorderStyle", Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out computed2));
						break;
					case "Width":
						styleInfo.AddAttribute("BorderWidth" + borderLocation, ReadExpression("BorderWidth", Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out computed2));
						break;
					}
					computed |= computed2;
					break;
				}
				case XmlNodeType.EndElement:
					if (borderLocation + "Border" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
		}

		private string ReadSize()
		{
			return m_reader.ReadString();
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo ReadSizeExpression(PublishingContextStruct context)
		{
			return ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.DataSet.TriState ReadTriState()
		{
			string value = m_reader.ReadString();
			return (Microsoft.ReportingServices.ReportIntermediateFormat.DataSet.TriState)Enum.Parse(typeof(Microsoft.ReportingServices.ReportIntermediateFormat.DataSet.TriState), value, ignoreCase: false);
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.Visibility ReadVisibility(PublishingContextStruct context, out bool computed)
		{
			m_static = true;
			Microsoft.ReportingServices.ReportIntermediateFormat.Visibility visibility = new Microsoft.ReportingServices.ReportIntermediateFormat.Visibility();
			bool computed2 = false;
			bool flag = false;
			context.PrefixPropertyName = "Visibility.";
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
									context.ErrorContext.Register(ProcessingErrorCode.rsToggleInPageSection, Severity.Error, context.ObjectType, context.ObjectName, "ToggleItem");
								}
								m_interactive = true;
								visibility.Toggle = m_reader.ReadString();
							}
						}
						else
						{
							visibility.Hidden = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context, out computed2);
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

		private Microsoft.ReportingServices.ReportIntermediateFormat.Visibility ReadVisibility(PublishingContextStruct context)
		{
			bool computed;
			return ReadVisibility(context, out computed);
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.Tablix ReadTablix(Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem parent, PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.Tablix tablix = new Microsoft.ReportingServices.ReportIntermediateFormat.Tablix(GenerateID(), parent);
			tablix.Name = m_reader.GetAttribute("Name");
			bool isTopLevelDataRegion = false;
			if ((context.Location & LocationFlags.InDataRegion) == 0)
			{
				isTopLevelDataRegion = true;
				m_nestedDataRegions = new List<Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion>();
			}
			else
			{
				Global.Tracer.Assert(m_nestedDataRegions != null);
				m_nestedDataRegions.Add(tablix);
			}
			context.Location |= (LocationFlags.InDataSet | LocationFlags.InDataRegion);
			context.ObjectType = tablix.ObjectType;
			context.ObjectName = tablix.Name;
			RegisterDataRegion(tablix);
			bool validName = true;
			if (!m_reportItemNames.Validate(context.ObjectType, context.ObjectName, context.ErrorContext))
			{
				validName = false;
			}
			if (m_scopeNames.Validate(isGrouping: false, context.ObjectName, context.ObjectType, context.ObjectName, context.ErrorContext))
			{
				m_reportScopes.Add(tablix.Name, tablix);
			}
			else
			{
				validName = false;
			}
			if ((context.Location & LocationFlags.InPageSection) != 0)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsDataRegionInPageSection, Severity.Error, context.ObjectType, context.ObjectName, null);
				validName = false;
			}
			StyleInformation styleInformation = null;
			bool flag = false;
			List<Microsoft.ReportingServices.ReportIntermediateFormat.TextBox> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.TextBox>();
			IdcRelationship relationship = null;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (m_reader.LocalName)
					{
					case "SortExpressions":
						tablix.Sorting = ReadSortExpressions(isDataRowSortExpression: true, context);
						break;
					case "Style":
						styleInformation = ReadStyle(context);
						break;
					case "Top":
						tablix.Top = ReadSize();
						break;
					case "Left":
						tablix.Left = ReadSize();
						break;
					case "Height":
						tablix.Height = ReadSize();
						break;
					case "Width":
						tablix.Width = ReadSize();
						break;
					case "CanScroll":
						tablix.CanScroll = m_reader.ReadBoolean(context.ObjectType, context.ObjectName, m_reader.LocalName);
						break;
					case "ZIndex":
						tablix.ZIndex = m_reader.ReadInteger(context.ObjectType, context.ObjectName, m_reader.LocalName);
						break;
					case "Visibility":
						tablix.Visibility = ReadVisibility(context);
						break;
					case "ToolTip":
						tablix.ToolTip = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
						break;
					case "DocumentMapLabel":
						tablix.DocumentMapLabel = ReadDocumentMapLabelExpression(m_reader.LocalName, context);
						break;
					case "Bookmark":
						tablix.Bookmark = ReadBookmarkExpression(m_reader.LocalName, context);
						break;
					case "CustomProperties":
						tablix.CustomProperties = ReadCustomProperties(context);
						break;
					case "DataElementName":
						tablix.DataElementName = m_reader.ReadString();
						break;
					case "DataElementOutput":
						tablix.DataElementOutput = ReadDataElementOutput();
						break;
					case "KeepTogether":
						tablix.KeepTogether = m_reader.ReadBoolean(context.ObjectType, context.ObjectName, m_reader.LocalName);
						break;
					case "NoRowsMessage":
						tablix.NoRowsMessage = ReadExpression(m_reader.LocalName, Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
						break;
					case "DataSetName":
						tablix.DataSetName = m_reader.ReadString();
						break;
					case "Relationship":
						relationship = ReadRelationship(context);
						break;
					case "PageBreak":
						ReadPageBreak(tablix, context);
						break;
					case "PageName":
						tablix.PageName = ReadPageNameExpression(context);
						break;
					case "Filters":
						tablix.Filters = ReadFilters(Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.DataRegionFilters, context);
						break;
					case "TablixCorner":
						ReadTablixCorner(tablix, context, list);
						break;
					case "TablixBody":
						ReadTablixBody(tablix, context, list);
						break;
					case "TablixColumnHierarchy":
						tablix.TablixColumnMembers = ReadTablixHierarchy(tablix, context, list, isColumnHierarchy: true, ref validName);
						break;
					case "TablixRowHierarchy":
						tablix.TablixRowMembers = ReadTablixHierarchy(tablix, context, list, isColumnHierarchy: false, ref validName);
						break;
					case "LayoutDirection":
						tablix.LayoutDirection = ReadLayoutDirection();
						break;
					case "GroupsBeforeRowHeaders":
						tablix.GroupsBeforeRowHeaders = m_reader.ReadInteger(context.ObjectType, context.ObjectName, m_reader.LocalName);
						break;
					case "RepeatColumnHeaders":
						tablix.RepeatColumnHeaders = m_reader.ReadBoolean(context.ObjectType, context.ObjectName, m_reader.LocalName);
						break;
					case "RepeatRowHeaders":
						tablix.RepeatRowHeaders = m_reader.ReadBoolean(context.ObjectType, context.ObjectName, m_reader.LocalName);
						break;
					case "FixedColumnHeaders":
						tablix.FixedColumnHeaders = m_reader.ReadBoolean(context.ObjectType, context.ObjectName, m_reader.LocalName);
						break;
					case "FixedRowHeaders":
						tablix.FixedRowHeaders = m_reader.ReadBoolean(context.ObjectType, context.ObjectName, m_reader.LocalName);
						break;
					case "OmitBorderOnPageBreak":
						tablix.OmitBorderOnPageBreak = m_reader.ReadBoolean(context.ObjectType, context.ObjectName, m_reader.LocalName);
						break;
					case "BandLayoutOptions":
						tablix.BandLayout = ReadBandLayoutOptions(tablix, context);
						break;
					case "TopMargin":
						tablix.TopMargin = ReadSizeExpression(context);
						break;
					case "BottomMargin":
						tablix.BottomMargin = ReadSizeExpression(context);
						break;
					case "LeftMargin":
						tablix.LeftMargin = ReadSizeExpression(context);
						break;
					case "RightMargin":
						tablix.RightMargin = ReadSizeExpression(context);
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("Tablix" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			tablix.InitializationData.IsTopLevelDataRegion = isTopLevelDataRegion;
			if (tablix.BandLayout != null)
			{
				tablix.ValidateBandStructure(context);
			}
			tablix.DataScopeInfo.SetRelationship(tablix.DataSetName, relationship);
			if (tablix.Height == null)
			{
				tablix.ComputeHeight = true;
			}
			if (tablix.Width == null)
			{
				tablix.ComputeWidth = true;
			}
			if (styleInformation != null)
			{
				styleInformation.Filter(StyleOwnerType.Tablix, tablix.NoRowsMessage != null);
				tablix.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, context.ErrorContext);
			}
			SetSortTargetForTextBoxes(list, tablix);
			tablix.Computed = true;
			if (validName)
			{
				m_createSubtotalsDefs.Add(tablix);
				if (tablix.Corner == null && tablix.ColumnHeaderRowCount > 0 && tablix.RowHeaderColumnCount > 0)
				{
					List<List<Microsoft.ReportingServices.ReportIntermediateFormat.TablixCornerCell>> list2 = new List<List<Microsoft.ReportingServices.ReportIntermediateFormat.TablixCornerCell>>(1);
					List<Microsoft.ReportingServices.ReportIntermediateFormat.TablixCornerCell> list3 = new List<Microsoft.ReportingServices.ReportIntermediateFormat.TablixCornerCell>(1);
					Microsoft.ReportingServices.ReportIntermediateFormat.TablixCornerCell tablixCornerCell = new Microsoft.ReportingServices.ReportIntermediateFormat.TablixCornerCell(GenerateID(), tablix);
					tablixCornerCell.RowSpan = tablix.ColumnHeaderRowCount;
					tablixCornerCell.ColSpan = tablix.RowHeaderColumnCount;
					list3.Add(tablixCornerCell);
					list2.Add(list3);
					tablix.Corner = list2;
				}
			}
			if (!validName)
			{
				return null;
			}
			return tablix;
		}

		private TablixMemberList ReadTablixHierarchy(Microsoft.ReportingServices.ReportIntermediateFormat.Tablix tablix, PublishingContextStruct context, List<Microsoft.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget, bool isColumnHierarchy, ref bool validName)
		{
			TablixMemberList tablixMemberList = null;
			bool flag = false;
			int leafNodes = 0;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (!(localName == "TablixMembers"))
					{
						if (localName == "EnableDrilldown")
						{
							bool flag2 = m_reader.ReadBoolean(context.ObjectType, context.ObjectName, m_reader.LocalName);
							if (flag2 && m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.TablixHierarchy_EnableDrilldown))
							{
								context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, context.ObjectType, tablix.Name, "EnableDrilldown");
							}
							if (isColumnHierarchy)
							{
								tablix.EnableColumnDrilldown = flag2;
							}
							else
							{
								tablix.EnableRowDrilldown = flag2;
							}
						}
					}
					else
					{
						tablixMemberList = ReadTablixMembers(tablix, null, context, textBoxesWithDefaultSortTarget, isColumnHierarchy, 0, ref leafNodes, ref validName, out bool _);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if (m_reader.LocalName == (isColumnHierarchy ? "TablixColumnHierarchy" : "TablixRowHierarchy"))
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			ValidateHeaderSizesAndSetSpans(tablix, context, tablixMemberList, isColumnHierarchy);
			if (isColumnHierarchy)
			{
				tablix.ColumnCount = leafNodes;
			}
			else
			{
				tablix.RowCount = leafNodes;
			}
			return tablixMemberList;
		}

		private void ValidateHeaderSizesAndSetSpans(Microsoft.ReportingServices.ReportIntermediateFormat.Tablix tablix, PublishingContextStruct context, TablixMemberList members, bool isColumnHierarchy)
		{
			bool hasError = context.ErrorContext.HasError;
			context.ErrorContext.HasError = false;
			m_headerLevelSizeList = new SortedList<double, Pair<double, int>>(Validator.DoubleComparer.Instance);
			m_headerLevelSizeList.Add(0.0, new Pair<double, int>(0.0, 0));
			m_firstCumulativeHeaderSize = -1.0;
			int rowOrColumnNumber = 0;
			ValidateHeaderSizes(context, members, isColumnHierarchy, 0.0, 0, ref rowOrColumnNumber, out int _);
			IList<Pair<double, int>> values = m_headerLevelSizeList.Values;
			int totalSpanCount = GetTotalSpanCount();
			if (isColumnHierarchy)
			{
				tablix.InitializationData.ColumnHeaderLevelSizeList = values;
				tablix.ColumnHeaderRowCount = totalSpanCount;
			}
			else
			{
				tablix.InitializationData.RowHeaderLevelSizeList = values;
				tablix.RowHeaderColumnCount = totalSpanCount;
			}
			if (!context.ErrorContext.HasError)
			{
				bool[] parentHeaderLevelHasStaticArray = new bool[totalSpanCount];
				SetHeaderSpans(context, members, isColumnHierarchy, 0.0, outerConsumedInnerZeroHeightLevel: false, 0, parentHeaderLevelHasStaticArray);
			}
			context.ErrorContext.HasError |= hasError;
		}

		private int GetTotalSpanCount()
		{
			int num = -1;
			for (int i = 0; i < m_headerLevelSizeList.Count; i++)
			{
				num += m_headerLevelSizeList.Values[i].Second + 1;
			}
			return num;
		}

		private bool ValidateHeaderSizes(PublishingContextStruct context, TablixMemberList members, bool isColumnHierarchy, double ancestorHeaderSize, int consecutiveZeroHeightAncestorCount, ref int rowOrColumnNumber, out int maxConsecutiveZeroHeightDescendentCount)
		{
			bool? flag = null;
			bool flag2 = false;
			maxConsecutiveZeroHeightDescendentCount = 0;
			bool flag3 = false;
			for (int i = 0; i < members.Count; i++)
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.TablixMember tablixMember = members[i];
				tablixMember.ConsecutiveZeroHeightAncestorCount = consecutiveZeroHeightAncestorCount;
				flag2 = false;
				double num = 0.0;
				double num2 = 0.0;
				bool flag4 = false;
				if (tablixMember.TablixHeader != null)
				{
					flag2 = true;
					num = tablixMember.TablixHeader.SizeValue;
					num2 = ancestorHeaderSize + num;
					if (num == 0.0)
					{
						flag4 = true;
						flag3 = true;
						int second = m_headerLevelSizeList[num2].Second;
						m_headerLevelSizeList[num2] = new Pair<double, int>(num2, Math.Max(consecutiveZeroHeightAncestorCount + 1, second));
					}
					else if (!m_headerLevelSizeList.ContainsKey(num2))
					{
						m_headerLevelSizeList.Add(num2, new Pair<double, int>(num2, 0));
					}
				}
				if (tablixMember.SubMembers != null)
				{
					flag2 |= ValidateHeaderSizes(context, tablixMember.SubMembers, isColumnHierarchy, ancestorHeaderSize + num, flag4 ? (consecutiveZeroHeightAncestorCount + 1) : 0, ref rowOrColumnNumber, out int maxConsecutiveZeroHeightDescendentCount2);
					maxConsecutiveZeroHeightDescendentCount = Math.Max(maxConsecutiveZeroHeightDescendentCount2, maxConsecutiveZeroHeightDescendentCount);
					tablixMember.ConsecutiveZeroHeightDescendentCount = maxConsecutiveZeroHeightDescendentCount;
				}
				else
				{
					rowOrColumnNumber++;
				}
				if (tablixMember.IsInnerMostMemberWithHeader)
				{
					int num3 = 0;
					if (tablixMember.SubMembers != null)
					{
						num3 = ((!isColumnHierarchy) ? (tablixMember.RowSpan - 1) : (tablixMember.ColSpan - 1));
					}
					if (m_firstCumulativeHeaderSize == -1.0)
					{
						m_firstCumulativeHeaderSize = num2;
					}
					else
					{
						double first = Math.Round(m_firstCumulativeHeaderSize, 4);
						double second2 = Math.Round(num2, 4);
						if (Validator.CompareDoubles(first, second2) != 0)
						{
							context.ErrorContext.Register(ProcessingErrorCode.rsInvalidTablixHeaderSize, Severity.Error, context.ObjectType, context.ObjectName, isColumnHierarchy ? "TablixColumnHierarchy" : "TablixRowHierarchy", "TablixHeader.Size", (rowOrColumnNumber - num3).ToString(CultureInfo.InvariantCulture.NumberFormat), first.ToString(CultureInfo.InvariantCulture.NumberFormat) + "mm", second2.ToString(CultureInfo.InvariantCulture.NumberFormat) + "mm", isColumnHierarchy ? "TablixColumn" : "TablixRow");
						}
					}
				}
				if (!flag.HasValue)
				{
					flag = flag2;
				}
				if (flag2 != flag.Value)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidTablixHeaders, Severity.Error, context.ObjectType, context.ObjectName, isColumnHierarchy ? "TablixColumnHierarchy" : "TablixRowHierarchy", "TablixMember", "TablixHeader", members[i].Level.ToString(CultureInfo.InvariantCulture.NumberFormat));
				}
			}
			if (flag3)
			{
				maxConsecutiveZeroHeightDescendentCount++;
			}
			else
			{
				maxConsecutiveZeroHeightDescendentCount = 0;
			}
			return flag2;
		}

		private void SetHeaderSpans(PublishingContextStruct context, TablixMemberList members, bool isColumnHierarchy, double ancestorHeaderSize, bool outerConsumedInnerZeroHeightLevel, int parentHeaderLevelPlusSpans, bool[] parentHeaderLevelHasStaticArray)
		{
			for (int i = 0; i < members.Count; i++)
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.TablixMember tablixMember = members[i];
				double num = 0.0;
				int num2 = 0;
				bool consumedInnerZeroHeightLevel = false;
				if (tablixMember.TablixHeader != null)
				{
					num = tablixMember.TablixHeader.SizeValue;
					if (num == 0.0)
					{
						consumedInnerZeroHeightLevel = outerConsumedInnerZeroHeightLevel;
						num2 = 1;
					}
					else
					{
						num2 = GetSpans(tablixMember, ancestorHeaderSize, num, outerConsumedInnerZeroHeightLevel, out consumedInnerZeroHeightLevel);
					}
					if (isColumnHierarchy)
					{
						tablixMember.RowSpan = num2;
					}
					else
					{
						tablixMember.ColSpan = num2;
					}
					tablixMember.HeaderLevel = parentHeaderLevelPlusSpans;
					if (parentHeaderLevelHasStaticArray != null)
					{
						parentHeaderLevelHasStaticArray[parentHeaderLevelPlusSpans] |= tablixMember.IsStatic;
					}
				}
				bool[] array = parentHeaderLevelHasStaticArray;
				if (tablixMember.HasConditionalOrToggleableVisibility)
				{
					array = (tablixMember.HeaderLevelHasStaticArray = new bool[parentHeaderLevelHasStaticArray.Length]);
				}
				if (tablixMember.SubMembers != null)
				{
					double ancestorHeaderSize2 = ancestorHeaderSize + num;
					SetHeaderSpans(context, tablixMember.SubMembers, isColumnHierarchy, ancestorHeaderSize2, consumedInnerZeroHeightLevel, parentHeaderLevelPlusSpans + num2, array);
				}
				if (parentHeaderLevelHasStaticArray != null && tablixMember.HasConditionalOrToggleableVisibility)
				{
					for (int j = tablixMember.HeaderLevel + 1; j < parentHeaderLevelHasStaticArray.Length; j++)
					{
						parentHeaderLevelHasStaticArray[j] |= array[j];
					}
				}
			}
		}

		private int GetSpans(Microsoft.ReportingServices.ReportIntermediateFormat.TablixMember member, double ancestorHeaderSize, double headerSize, bool outerConsumedInnerZeroHeightLevel, out bool consumedInnerZeroHeightLevel)
		{
			double second = ancestorHeaderSize + headerSize;
			int num = 0;
			consumedInnerZeroHeightLevel = false;
			int num2 = m_headerLevelSizeList.IndexOfKey(ancestorHeaderSize);
			if (!outerConsumedInnerZeroHeightLevel)
			{
				int second2 = m_headerLevelSizeList.Values[num2].Second;
				if (second2 > 0)
				{
					num += second2 - member.ConsecutiveZeroHeightAncestorCount;
				}
			}
			for (int i = num2 + 1; i < m_headerLevelSizeList.Count; i++)
			{
				double first = m_headerLevelSizeList.Values[i].First;
				int second2 = m_headerLevelSizeList.Values[i].Second;
				num++;
				if (Validator.CompareDoubles(first, second) == 0)
				{
					if (second2 > 0)
					{
						int num3 = second2 - member.ConsecutiveZeroHeightDescendentCount;
						if (num3 > 0)
						{
							consumedInnerZeroHeightLevel = true;
							num += num3;
						}
					}
					break;
				}
				num += second2;
			}
			return num;
		}

		private TablixMemberList ReadTablixMembers(Microsoft.ReportingServices.ReportIntermediateFormat.Tablix tablix, Microsoft.ReportingServices.ReportIntermediateFormat.TablixMember parentMember, PublishingContextStruct context, List<Microsoft.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget, bool isColumnHierarchy, int level, ref int leafNodes, ref bool validName, out bool innerMostMemberWithHeaderFound)
		{
			TablixMemberList tablixMemberList = new TablixMemberList();
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			int num = -1;
			bool isStaticWithHeader = false;
			innerMostMemberWithHeaderFound = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (!(localName == "TablixMember"))
					{
						break;
					}
					num = leafNodes;
					List<Microsoft.ReportingServices.ReportIntermediateFormat.TextBox> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.TextBox>();
					Microsoft.ReportingServices.ReportIntermediateFormat.TablixMember tablixMember = ReadTablixMember(tablix, context, list, isColumnHierarchy, level, ref leafNodes, ref isStaticWithHeader, ref validName, out innerMostMemberWithHeaderFound);
					tablixMember.ParentMember = parentMember;
					if (level == 0)
					{
						if (flag3)
						{
							if (tablixMember.FixedData)
							{
								context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFixedDataNotContiguous, Severity.Error, context.ObjectType, context.ObjectName, "FixedData", isColumnHierarchy ? "TablixColumnHierarchy" : "TablixRowHierarchy");
							}
						}
						else if (flag2)
						{
							if (tablixMember.FixedData)
							{
								tablix.InitializationData.FixedColLength += tablixMember.ColSpan;
							}
							else
							{
								flag3 = true;
							}
						}
						else if (tablixMember.FixedData)
						{
							if (isColumnHierarchy)
							{
								tablix.InitializationData.HasFixedColData = true;
								tablix.InitializationData.FixedColStartIndex = num;
								tablix.InitializationData.FixedColLength = tablixMember.ColSpan;
							}
							else
							{
								tablix.InitializationData.HasFixedRowData = true;
							}
							flag2 = true;
						}
					}
					else
					{
						if (tablixMember.FixedData && !parentMember.FixedData)
						{
							context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFixedDataInHierarchy, Severity.Warning, context.ObjectType, context.ObjectName, "FixedData", isColumnHierarchy ? "TablixColumnHierarchy" : "TablixRowHierarchy");
						}
						tablixMember.FixedData = parentMember.FixedData;
					}
					tablixMemberList.Add(tablixMember);
					if (tablixMember.Grouping != null)
					{
						SetSortTargetForTextBoxes(list, tablixMember.Grouping);
					}
					else
					{
						textBoxesWithDefaultSortTarget.AddRange(list);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("TablixMembers" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (isStaticWithHeader)
			{
				foreach (Microsoft.ReportingServices.ReportIntermediateFormat.TablixMember item in tablixMemberList)
				{
					item.HasStaticPeerWithHeader = true;
				}
			}
			if (tablixMemberList.Count <= 0)
			{
				return null;
			}
			return tablixMemberList;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.TablixMember ReadTablixMember(Microsoft.ReportingServices.ReportIntermediateFormat.Tablix tablix, PublishingContextStruct context, List<Microsoft.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget, bool isColumnHierarchy, int level, ref int aLeafNodes, ref bool isStaticWithHeader, ref bool validName, out bool innerMostMemberWithHeaderFound)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.TablixMember tablixMember = new Microsoft.ReportingServices.ReportIntermediateFormat.TablixMember(GenerateID(), tablix);
			m_runningValueHolderList.Add(tablixMember);
			tablixMember.IsColumn = isColumnHierarchy;
			tablixMember.Level = level;
			innerMostMemberWithHeaderFound = false;
			bool flag = false;
			bool computed = false;
			int leafNodes = 0;
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
						case "Group":
							tablixMember.Grouping = ReadGrouping(tablixMember, context, ref validName);
							break;
						case "SortExpressions":
							tablixMember.Sorting = ReadSortExpressions(isDataRowSortExpression: false, context);
							break;
						case "TablixHeader":
							tablixMember.TablixHeader = ReadTablixHeader(tablix, context, textBoxesWithDefaultSortTarget);
							tablixMember.TablixHeader.SizeValue = context.ValidateSize(tablixMember.TablixHeader.Size, isColumnHierarchy ? "Height" : "Width", context.ErrorContext);
							break;
						case "TablixMembers":
							tablixMember.SubMembers = ReadTablixMembers(tablix, tablixMember, context, textBoxesWithDefaultSortTarget, isColumnHierarchy, level + 1, ref leafNodes, ref validName, out innerMostMemberWithHeaderFound);
							break;
						case "CustomProperties":
							tablixMember.CustomProperties = ReadCustomProperties(context);
							break;
						case "FixedData":
							tablixMember.FixedData = m_reader.ReadBoolean(context.ObjectType, context.ObjectName, m_reader.LocalName);
							break;
						case "Visibility":
							tablixMember.Visibility = ReadVisibility(context, out computed);
							break;
						case "HideIfNoRows":
							tablixMember.HideIfNoRows = m_reader.ReadBoolean(context.ObjectType, context.ObjectName, m_reader.LocalName);
							break;
						case "KeepWithGroup":
							tablixMember.KeepWithGroup = (KeepWithGroup)Enum.Parse(typeof(KeepWithGroup), m_reader.ReadString());
							break;
						case "RepeatOnNewPage":
							tablixMember.RepeatOnNewPage = m_reader.ReadBoolean(context.ObjectType, context.ObjectName, m_reader.LocalName);
							break;
						case "DataElementName":
							tablixMember.DataElementName = m_reader.ReadString();
							break;
						case "DataElementOutput":
							tablixMember.DataElementOutput = ReadDataElementOutput();
							break;
						case "KeepTogether":
							tablixMember.KeepTogether = m_reader.ReadBoolean(context.ObjectType, context.ObjectName, m_reader.LocalName);
							tablixMember.KeepTogetherSpecified = true;
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("TablixMember" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			if (!innerMostMemberWithHeaderFound && tablixMember.TablixHeader != null)
			{
				tablixMember.IsInnerMostMemberWithHeader = true;
				innerMostMemberWithHeaderFound = true;
			}
			if (tablixMember.SubMembers == null || tablixMember.SubMembers.Count == 0)
			{
				aLeafNodes++;
				if (isColumnHierarchy)
				{
					tablixMember.ColSpan = 1;
				}
				else
				{
					tablixMember.RowSpan = 1;
				}
			}
			else
			{
				aLeafNodes += leafNodes;
				if (isColumnHierarchy)
				{
					tablixMember.ColSpan = leafNodes;
				}
				else
				{
					tablixMember.RowSpan = leafNodes;
				}
			}
			ValidateAndProcessMemberGroupAndSort(tablixMember, context);
			if (tablixMember.IsStatic && tablixMember.TablixHeader != null)
			{
				isStaticWithHeader = true;
			}
			if (isColumnHierarchy)
			{
				if (tablixMember.KeepWithGroup != 0)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidKeepWithGroupOnColumnTablixMember, Severity.Error, context.ObjectType, context.ObjectName, "TablixMember", "KeepWithGroup", "None");
				}
				if (tablixMember.RepeatOnNewPage)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidRepeatOnNewPageOnColumnTablixMember, Severity.Error, context.ObjectType, context.ObjectName, "TablixMember", "RepeatOnNewPage");
				}
			}
			return tablixMember;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.TablixHeader ReadTablixHeader(Microsoft.ReportingServices.ReportIntermediateFormat.Tablix tablix, PublishingContextStruct context, List<Microsoft.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.TablixHeader tablixHeader = new Microsoft.ReportingServices.ReportIntermediateFormat.TablixHeader(GenerateID());
			int? colSpan = null;
			int? rowSpan = null;
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (!(localName == "Size"))
					{
						if (localName == "CellContents")
						{
							tablixHeader.CellContents = ReadCellContents(tablix, context, textBoxesWithDefaultSortTarget, readRowColSpans: true, out Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem altCellContents, out colSpan, out rowSpan);
							tablixHeader.AltCellContents = altCellContents;
						}
					}
					else
					{
						tablixHeader.Size = ReadSize();
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("TablixHeader" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return tablixHeader;
		}

		private void ReadTablixCorner(Microsoft.ReportingServices.ReportIntermediateFormat.Tablix tablix, PublishingContextStruct context, List<Microsoft.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget)
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
					if (localName == "TablixCornerRows")
					{
						tablix.Corner = ReadTablixCornerRows(tablix, context, textBoxesWithDefaultSortTarget);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("TablixCorner" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
		}

		private List<List<Microsoft.ReportingServices.ReportIntermediateFormat.TablixCornerCell>> ReadTablixCornerRows(Microsoft.ReportingServices.ReportIntermediateFormat.Tablix tablix, PublishingContextStruct context, List<Microsoft.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget)
		{
			List<List<Microsoft.ReportingServices.ReportIntermediateFormat.TablixCornerCell>> list = new List<List<Microsoft.ReportingServices.ReportIntermediateFormat.TablixCornerCell>>();
			int[] array = null;
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (!(localName == "TablixCornerRow"))
					{
						break;
					}
					List<Microsoft.ReportingServices.ReportIntermediateFormat.TablixCornerCell> list2 = ReadTablixCornerRow(tablix, context, array, textBoxesWithDefaultSortTarget);
					if (array == null)
					{
						array = new int[list2.Count];
						for (int i = 0; i < array.Length; i++)
						{
							array[i] = 1;
						}
					}
					else if (array.Length != list2.Count)
					{
						context.ErrorContext.Register(ProcessingErrorCode.rsInconsistentNumberofCellsInRow, Severity.Error, context.ObjectType, context.ObjectName, "TablixCorner");
						int num = array.Length;
						if (num < list2.Count)
						{
							int j = 0;
							int[] array2 = new int[list2.Count];
							for (; j < num; j++)
							{
								array2[j] = array[j];
							}
							for (; j < array2.Length; j++)
							{
								array2[j] = 1;
							}
							array = array2;
						}
					}
					for (int k = 0; k < list2.Count; k++)
					{
						Microsoft.ReportingServices.ReportIntermediateFormat.TablixCornerCell tablixCornerCell = list2[k];
						if (array[k] <= 1)
						{
							array[k] = tablixCornerCell.RowSpan;
							if (tablixCornerCell.RowSpan <= 1 || tablixCornerCell.ColSpan <= 1)
							{
								continue;
							}
							for (int l = 1; l < tablixCornerCell.ColSpan; l++)
							{
								k++;
								if (k == array.Length)
								{
									context.ErrorContext.Register(ProcessingErrorCode.rsInvalidTablixCornerCellSpan, Severity.Error, context.ObjectType, context.ObjectName, "ColSpan");
									break;
								}
								array[k] = tablixCornerCell.RowSpan;
							}
						}
						else
						{
							array[k]--;
						}
					}
					list.Add(list2);
					break;
				}
				case XmlNodeType.EndElement:
					if ("TablixCornerRows" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return list;
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.TablixCornerCell> ReadTablixCornerRow(Microsoft.ReportingServices.ReportIntermediateFormat.Tablix tablix, PublishingContextStruct context, int[] rowSpans, List<Microsoft.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget)
		{
			List<Microsoft.ReportingServices.ReportIntermediateFormat.TablixCornerCell> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.TablixCornerCell>();
			bool flag = false;
			int num = 1;
			int num2 = 0;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "TablixCornerCell")
					{
						Microsoft.ReportingServices.ReportIntermediateFormat.TablixCornerCell tablixCornerCell = ReadTablixCornerCell(tablix, context, num > 1 || (rowSpans != null && rowSpans.Length > num2 && rowSpans[num2] > 1), textBoxesWithDefaultSortTarget);
						num = ((num > 1) ? (num - 1) : tablixCornerCell.ColSpan);
						list.Add(tablixCornerCell);
						num2++;
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("TablixCornerRow" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return list;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.TablixCornerCell ReadTablixCornerCell(Microsoft.ReportingServices.ReportIntermediateFormat.Tablix tablix, PublishingContextStruct context, bool shouldBeEmpty, List<Microsoft.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.TablixCornerCell tablixCornerCell = new Microsoft.ReportingServices.ReportIntermediateFormat.TablixCornerCell(GenerateID(), tablix);
			bool flag = false;
			int? rowSpan = null;
			int? colSpan = null;
			if (!m_reader.IsEmptyElement)
			{
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName = m_reader.LocalName;
						if (localName == "CellContents")
						{
							tablixCornerCell.CellContents = ReadCellContents(tablix, context, textBoxesWithDefaultSortTarget, readRowColSpans: true, out Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem altCellContents, out colSpan, out rowSpan);
							tablixCornerCell.AltCellContents = altCellContents;
						}
						break;
					}
					case XmlNodeType.EndElement:
						if ("TablixCornerCell" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			if (shouldBeEmpty)
			{
				if (tablixCornerCell.CellContents != null)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsCellContentsNotOmitted, Severity.Error, context.ObjectType, context.ObjectName, "TablixCornerCell");
				}
				if (colSpan.HasValue)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidTablixCornerCellSpan, Severity.Error, context.ObjectType, context.ObjectName, "ColSpan");
				}
				if (rowSpan.HasValue)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidTablixCornerCellSpan, Severity.Error, context.ObjectType, context.ObjectName, "RowSpan");
				}
			}
			else if (tablixCornerCell.CellContents == null)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsCellContentsRequired, Severity.Error, context.ObjectType, context.ObjectName, "TablixCornerCell");
			}
			else
			{
				if (!rowSpan.HasValue)
				{
					tablixCornerCell.RowSpan = 1;
				}
				else if (rowSpan == 0)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidTablixCornerCellSpan, Severity.Error, context.ObjectType, context.ObjectName, "RowSpan");
				}
				else
				{
					tablixCornerCell.RowSpan = rowSpan.Value;
				}
				if (!colSpan.HasValue)
				{
					tablixCornerCell.ColSpan = 1;
				}
				else if (colSpan == 0)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidTablixCornerCellSpan, Severity.Error, context.ObjectType, context.ObjectName, "ColSpan");
				}
				else
				{
					tablixCornerCell.ColSpan = colSpan.Value;
				}
			}
			return tablixCornerCell;
		}

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.TablixColumn> ReadTablixColumns(Microsoft.ReportingServices.ReportIntermediateFormat.Tablix tablix, PublishingContextStruct context, List<Microsoft.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget)
		{
			List<Microsoft.ReportingServices.ReportIntermediateFormat.TablixColumn> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.TablixColumn>();
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "TablixColumn")
					{
						list.Add(ReadTablixColumn(tablix, context, textBoxesWithDefaultSortTarget));
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("TablixColumns" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return list;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.TablixRow ReadTablixRow(Microsoft.ReportingServices.ReportIntermediateFormat.Tablix tablix, PublishingContextStruct context, List<Microsoft.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.TablixRow tablixRow = new Microsoft.ReportingServices.ReportIntermediateFormat.TablixRow(GenerateID());
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
					if (!(localName == "Height"))
					{
						if (localName == "TablixCells")
						{
							tablixRow.TablixCells = ReadTablixCells(tablix, context, textBoxesWithDefaultSortTarget);
							if (num > 0 && tablixRow.Cells.Count != num)
							{
								context.ErrorContext.Register(ProcessingErrorCode.rsInconsistentNumberofCellsInRow, Severity.Error, context.ObjectType, context.ObjectName, "Tablix");
							}
							num = tablixRow.Cells.Count;
						}
					}
					else
					{
						tablixRow.Height = ReadSize();
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("TablixRow" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (tablixRow.Cells == null)
			{
				tablixRow.TablixCells = new TablixCellList();
			}
			return tablixRow;
		}

		private TablixCellList ReadTablixCells(Microsoft.ReportingServices.ReportIntermediateFormat.Tablix tablix, PublishingContextStruct context, List<Microsoft.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget)
		{
			TablixCellList tablixCellList = new TablixCellList();
			bool flag = false;
			int num = 1;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "TablixCell")
					{
						Microsoft.ReportingServices.ReportIntermediateFormat.TablixCell tablixCell = ReadTablixCell(tablix, context, num > 1, textBoxesWithDefaultSortTarget);
						num = ((num != 1) ? (num - 1) : tablixCell.ColSpan);
						tablixCellList.Add(tablixCell);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("TablixCells" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return tablixCellList;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.TablixCell ReadTablixCell(Microsoft.ReportingServices.ReportIntermediateFormat.Tablix tablix, PublishingContextStruct context, bool shouldBeEmpty, List<Microsoft.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.TablixCell tablixCell = new Microsoft.ReportingServices.ReportIntermediateFormat.TablixCell(GenerateID(), tablix);
			m_aggregateHolderList.Add(tablixCell);
			m_runningValueHolderList.Add(tablixCell);
			bool flag = false;
			int? colSpan = null;
			int? rowSpan = null;
			bool flag2 = false;
			if (!m_reader.IsEmptyElement)
			{
				string dataSetName = null;
				List<IdcRelationship> relationships = null;
				do
				{
					m_reader.Read();
					switch (m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (m_reader.LocalName)
						{
						case "DataSetName":
							dataSetName = m_reader.ReadString();
							break;
						case "Relationships":
							relationships = ReadRelationships(context);
							break;
						case "CellContents":
						{
							flag2 = true;
							tablixCell.CellContents = ReadCellContents(tablix, context, textBoxesWithDefaultSortTarget, readRowColSpans: true, out Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem altCellContents, out colSpan, out rowSpan);
							tablixCell.AltCellContents = altCellContents;
							break;
						}
						case "DataElementName":
							tablixCell.DataElementName = m_reader.ReadString();
							break;
						case "DataElementOutput":
							tablixCell.DataElementOutput = ReadDataElementOutput();
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("TablixCell" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
				tablixCell.DataScopeInfo.SetRelationship(dataSetName, relationships);
			}
			if (shouldBeEmpty)
			{
				if (flag2)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsCellContentsNotOmitted, Severity.Error, context.ObjectType, context.ObjectName, "TablixCell");
				}
				if (colSpan.HasValue && colSpan.Value != 0)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidTablixCellCellSpan, Severity.Error, context.ObjectType, context.ObjectName, "ColSpan");
				}
			}
			else
			{
				if (!flag2)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsCellContentsRequired, Severity.Error, context.ObjectType, context.ObjectName, "TablixCell");
				}
				if (!colSpan.HasValue)
				{
					tablixCell.ColSpan = 1;
				}
				else if (colSpan.Value == 0)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidTablixCellCellSpan, Severity.Error, context.ObjectType, context.ObjectName, "ColSpan");
				}
				else
				{
					tablixCell.ColSpan = colSpan.Value;
				}
			}
			if (rowSpan.HasValue && rowSpan.Value != 1)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidTablixCellRowSpan, Severity.Error, context.ObjectType, context.ObjectName, "RowSpan");
			}
			else
			{
				tablixCell.RowSpan = 1;
			}
			return tablixCell;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.TablixColumn ReadTablixColumn(Microsoft.ReportingServices.ReportIntermediateFormat.Tablix tablix, PublishingContextStruct context, List<Microsoft.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.TablixColumn tablixColumn = new Microsoft.ReportingServices.ReportIntermediateFormat.TablixColumn(GenerateID());
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
						tablixColumn.Width = ReadSize();
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("TablixColumn" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return tablixColumn;
		}

		private TablixRowList ReadTablixRows(Microsoft.ReportingServices.ReportIntermediateFormat.Tablix tablix, PublishingContextStruct context, List<Microsoft.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget)
		{
			TablixRowList tablixRowList = new TablixRowList();
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (localName == "TablixRow")
					{
						tablixRowList.Add(ReadTablixRow(tablix, context, textBoxesWithDefaultSortTarget));
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("TablixRows" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return tablixRowList;
		}

		private void ReadTablixBody(Microsoft.ReportingServices.ReportIntermediateFormat.Tablix tablix, PublishingContextStruct context, List<Microsoft.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget)
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
					if (!(localName == "TablixColumns"))
					{
						if (localName == "TablixRows")
						{
							tablix.TablixRows = ReadTablixRows(tablix, context, textBoxesWithDefaultSortTarget);
						}
					}
					else
					{
						tablix.TablixColumns = ReadTablixColumns(tablix, context, textBoxesWithDefaultSortTarget);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("TablixBody" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem ReadCellContents(Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem parent, PublishingContextStruct context, List<Microsoft.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget, bool readRowColSpans, out Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem altCellContents, out int? colSpan, out int? rowSpan)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem result = null;
			altCellContents = null;
			colSpan = null;
			rowSpan = null;
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
						case "Line":
							result = ReadLine(parent, context);
							break;
						case "Rectangle":
							result = ReadRectangle(parent, context, textBoxesWithDefaultSortTarget);
							break;
						case "CustomReportItem":
							result = ReadCustomReportItem(parent, context, textBoxesWithDefaultSortTarget, out altCellContents);
							Global.Tracer.Assert(altCellContents != null);
							break;
						case "Textbox":
							result = ReadTextbox(parent, context, textBoxesWithDefaultSortTarget);
							break;
						case "Image":
							result = ReadImage(parent, context);
							break;
						case "Subreport":
							result = ReadSubreport(parent, context);
							break;
						case "Tablix":
							result = ReadTablix(parent, context);
							break;
						case "Chart":
							result = ReadChart(parent, context);
							break;
						case "GaugePanel":
							result = ReadGaugePanel(parent, context);
							break;
						case "Map":
							result = ReadMap(parent, context);
							break;
						default:
						{
							if (!readRowColSpans)
							{
								break;
							}
							string localName = m_reader.LocalName;
							if (!(localName == "ColSpan"))
							{
								if (localName == "RowSpan")
								{
									rowSpan = m_reader.ReadInteger(context.ObjectType, context.ObjectName, m_reader.LocalName);
								}
							}
							else
							{
								colSpan = m_reader.ReadInteger(context.ObjectType, context.ObjectName, m_reader.LocalName);
							}
							break;
						}
						}
						break;
					case XmlNodeType.EndElement:
						if ((readRowColSpans && m_reader.LocalName == "CellContents") || (!readRowColSpans && m_reader.LocalName == "ReportItem"))
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

		private bool ReadLayoutDirection()
		{
			string x = m_reader.ReadString();
			return Microsoft.ReportingServices.ReportProcessing.ReportProcessing.CompareWithInvariantCulture(x, "RTL", ignoreCase: false) == 0;
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
					Microsoft.ReportingServices.ReportIntermediateFormat.EndUserSort userSort = m_textBoxesWithUserSortTarget[i].UserSort;
					Microsoft.ReportingServices.ReportIntermediateFormat.ISortFilterScope value = null;
					m_reportScopes.TryGetValue(userSort.SortTargetString, out value);
					if (value != null)
					{
						userSort.SetSortTarget(value);
					}
				}
			}
			m_report.LastAggregateID = m_reportCT.LastAggregateID;
			m_report.LastLookupID = m_reportCT.LastLookupID;
			MapAndValidateDataSets();
			CreateAutomaticSubtotalsAndDomainScopeMembers();
			m_report.MergeOnePass = (!m_hasGrouping && !m_hasSorting && !m_reportCT.BodyRefersToReportItems && !m_reportCT.ValueReferencedGlobal && !m_subReportMergeTransactions && !m_hasUserSort && !m_reportCT.AggregateOfAggregateUsed);
			m_report.SubReportMergeTransactions = m_subReportMergeTransactions;
			m_report.NeedPostGroupProcessing = (m_requiresSortingPostGrouping || m_hasGroupFilters || m_reportCT.AggregateOfAggregateUsed || m_domainScopeGroups.Count > 0);
			m_report.HasSpecialRecursiveAggregates = m_hasSpecialRecursiveAggregates;
			m_report.HasReportItemReferences = m_reportCT.BodyRefersToReportItems;
			m_report.HasImageStreams = m_hasImageStreams;
			m_report.HasBookmarks = m_hasBookmarks;
			m_report.HasLabels = m_hasLabels;
			m_report.HasUserSortFilter = m_hasUserSort;
			if (m_report.ReportSections == null)
			{
				return;
			}
			foreach (Microsoft.ReportingServices.ReportIntermediateFormat.ReportSection reportSection in m_report.ReportSections)
			{
				reportSection.NeedsReportItemsOnPage |= (reportSection.Page.PageAggregates.Count != 0);
			}
		}

		private static void RegisterDataSetWithDataSource(Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet, Microsoft.ReportingServices.ReportIntermediateFormat.DataSource dataSource, int dataSourceIndex, Hashtable dataSetQueryInfo, bool hasDynamicParameters)
		{
			dataSet.DataSource = dataSource;
			if (dataSource.DataSets == null)
			{
				dataSource.DataSets = new List<Microsoft.ReportingServices.ReportIntermediateFormat.DataSet>();
			}
			if (hasDynamicParameters)
			{
				PublishingDataSetInfo publishingDataSetInfo = (PublishingDataSetInfo)dataSetQueryInfo[dataSet.Name];
				Global.Tracer.Assert(publishingDataSetInfo != null, "(null != dataSetInfo)");
				publishingDataSetInfo.DataSourceIndex = dataSourceIndex;
				publishingDataSetInfo.DataSetIndex = dataSource.DataSets.Count;
				publishingDataSetInfo.MergeFlagsFromDataSource(dataSource.IsComplex, dataSource.ParameterNames);
			}
			dataSource.DataSets.Add(dataSet);
		}

		private void MapAndValidateDataSets()
		{
			if (m_dataSets == null || m_dataSets.Count == 0)
			{
				return;
			}
			bool hasDynamicParameters = false;
			if (m_dynamicParameters != null && m_dynamicParameters.Count > 0)
			{
				hasDynamicParameters = true;
			}
			foreach (Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet in m_dataSets)
			{
				bool flag = false;
				if (dataSet.IsReferenceToSharedDataSet)
				{
					if (m_report.SharedDSContainer != null)
					{
						flag = true;
						RegisterDataSetWithDataSource(dataSet, m_report.SharedDSContainer, m_report.SharedDSContainerCollectionIndex, m_dataSetQueryInfo, hasDynamicParameters);
					}
				}
				else if (dataSet.Query != null && m_report.DataSources != null)
				{
					for (int i = 0; i < m_report.DataSources.Count; i++)
					{
						Microsoft.ReportingServices.ReportIntermediateFormat.DataSource dataSource = m_report.DataSources[i];
						if (dataSet.Query.DataSourceName == dataSource.Name)
						{
							flag = true;
							RegisterDataSetWithDataSource(dataSet, dataSource, i, m_dataSetQueryInfo, hasDynamicParameters);
							break;
						}
					}
				}
				if (!flag && dataSet.Query != null)
				{
					m_errorContext.Register(ProcessingErrorCode.rsInvalidDataSourceReference, Severity.Error, dataSet.ObjectType, dataSet.Name, "DataSourceName", dataSet.Query.DataSourceName.MarkAsPrivate());
				}
			}
		}

		private void CreateAutomaticSubtotalsAndDomainScopeMembers()
		{
			if (!m_errorContext.HasError && (ShouldCreateAutomaticSubtotals() || ShouldCreateDomainScopeMembers()))
			{
				AutomaticSubtotalContext context = new AutomaticSubtotalContext(m_report, m_createSubtotalsDefs, m_domainScopeGroups, m_reportItemNames, m_scopeNames, m_variableNames, m_reportScopes, m_reportItemCollectionList, m_aggregateHolderList, m_runningValueHolderList, m_variableSequenceIdCounter, m_textboxSequenceIdCounter, m_report.BuildScopeTree());
				if (ShouldCreateAutomaticSubtotals())
				{
					for (int i = 0; i < m_createSubtotalsDefs.Count; i++)
					{
						m_createSubtotalsDefs[i].CreateAutomaticSubtotals(context);
					}
				}
				if (ShouldCreateDomainScopeMembers())
				{
					foreach (Microsoft.ReportingServices.ReportIntermediateFormat.Grouping domainScopeGroup in m_domainScopeGroups)
					{
						CreateDomainScopeMember(domainScopeGroup, context);
					}
				}
			}
			m_reportItemNames = null;
			m_scopeNames = null;
			m_variableNames = null;
		}

		private bool ShouldCreateDomainScopeMembers()
		{
			if (m_domainScopeGroups.Count > 0)
			{
				return !m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.DomainScope);
			}
			return false;
		}

		private bool ShouldCreateAutomaticSubtotals()
		{
			if (m_createSubtotalsDefs.Count > 0)
			{
				return !m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.AutomaticSubtotals);
			}
			return false;
		}

		private void CreateDomainScopeMember(Microsoft.ReportingServices.ReportIntermediateFormat.Grouping group, AutomaticSubtotalContext context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode owner = group.Owner;
			Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef = owner.DataRegionDef;
			if (owner.InnerDynamicMembers != null && owner.InnerDynamicMembers.Count > 0)
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidGroupingDomainScopeNotLeaf, Severity.Error, dataRegionDef.ObjectType, dataRegionDef.Name, "DomainScope", group.Name.MarkAsModelInfo(), group.DomainScope.MarkAsPrivate());
				return;
			}
			if (!m_reportScopes.ContainsKey(group.DomainScope))
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidGroupingDomainScope, Severity.Error, dataRegionDef.ObjectType, dataRegionDef.Name, "DomainScope", group.Name.MarkAsModelInfo(), group.DomainScope.MarkAsPrivate());
				return;
			}
			Microsoft.ReportingServices.ReportIntermediateFormat.ISortFilterScope sortFilterScope = m_reportScopes[group.DomainScope];
			if (sortFilterScope is Microsoft.ReportingServices.ReportIntermediateFormat.DataSet)
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidGroupingDomainScopeDataSet, Severity.Error, dataRegionDef.ObjectType, dataRegionDef.Name, "DomainScope", group.Name.MarkAsModelInfo(), group.DomainScope.MarkAsPrivate());
				return;
			}
			Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode reportHierarchyNode = null;
			Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataRegion;
			if (sortFilterScope is Microsoft.ReportingServices.ReportIntermediateFormat.Grouping)
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.Grouping grouping = (Microsoft.ReportingServices.ReportIntermediateFormat.Grouping)sortFilterScope;
				if (grouping.Parent != null)
				{
					m_errorContext.Register(ProcessingErrorCode.rsInvalidGroupingDomainScopeTargetWithParent, Severity.Error, dataRegionDef.ObjectType, dataRegionDef.Name, "DomainScope", group.Name.MarkAsModelInfo(), group.DomainScope.MarkAsPrivate());
					return;
				}
				reportHierarchyNode = grouping.Owner;
				dataRegion = reportHierarchyNode.DataRegionDef;
			}
			else
			{
				dataRegion = (Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion)sortFilterScope;
			}
			dataRegion.CreateDomainScopeMember(reportHierarchyNode, group, context);
		}

		private void Phase3(out ParameterInfoCollection parameters, out Dictionary<string, int> groupingExprCountAtScope)
		{
			try
			{
				m_report.HasPreviousAggregates = m_reportCT.PreviousAggregateUsed;
				m_report.HasAggregatesOfAggregates = m_reportCT.AggregateOfAggregateUsed;
				m_report.HasAggregatesOfAggregatesInUserSort = m_reportCT.AggregateOfAggregateUsedInUserSort;
				m_scopeTree = ScopeTreeBuilderForDataScopeDataSet.BuildScopeTree(m_report, m_errorContext);
				m_reportCT.Builder.ReportStart();
				Microsoft.ReportingServices.ReportIntermediateFormat.InitializationContext context = new Microsoft.ReportingServices.ReportIntermediateFormat.InitializationContext(m_publishingContext.CatalogContext, m_hasFilters, m_dataSourceNames, m_dataSets, m_dynamicParameters, m_dataSetQueryInfo, m_errorContext, m_reportCT.Builder, m_report, m_reportLanguage, m_reportScopes, m_hasUserSortPeerScopes, m_hasUserSort, m_dataRegionCount, m_textboxSequenceIdCounter.Value, m_variableSequenceIdCounter.Value, m_publishingContext, m_scopeTree, isSharedDataSetContext: false);
				m_report.Initialize(context);
				InitializeParameters(context, out parameters);
				groupingExprCountAtScope = context.GroupingExprCountAtScope;
				m_reportCT.Builder.ReportEnd();
				if (!m_errorContext.HasError)
				{
					((IExpressionHostAssemblyHolder)m_report).CompiledCode = m_reportCT.Compile(m_report, m_publishingContext.CompilationTempAppDomain, m_publishingContext.GenerateExpressionHostWithRefusedPermissions, m_publishingContext.PublishingVersioning);
				}
				int num = 0;
				for (int i = 0; i < m_dataSets.Count; i++)
				{
					if (!m_dataSets[i].UsedOnlyInParameters)
					{
						if (num == 0)
						{
							m_report.FirstDataSet = m_dataSets[i];
						}
						num++;
						if (1 < num)
						{
							m_report.MergeOnePass = false;
						}
					}
				}
				m_report.DataSetsNotOnlyUsedInParameters = num;
				m_report.HasLookups = context.HasLookups;
			}
			finally
			{
				m_reportCT = null;
			}
		}

		private void InitializeParameters(Microsoft.ReportingServices.ReportIntermediateFormat.InitializationContext context, out ParameterInfoCollection parameters)
		{
			bool parametersNotUsedInQuery = false;
			Microsoft.ReportingServices.ReportProcessing.ParameterInfo parameterInfo = null;
			parameters = new ParameterInfoCollection();
			parameters.ParametersLayout = m_parametersLayout;
			List<Microsoft.ReportingServices.ReportIntermediateFormat.ParameterDef> parameters2 = m_report.Parameters;
			if (parameters2 != null && parameters2.Count > 0)
			{
				context.InitializeParameters(m_report.Parameters, m_dataSets);
				for (int i = 0; i < parameters2.Count; i++)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.ParameterDef parameterDef = parameters2[i];
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
					parameterInfo = new Microsoft.ReportingServices.ReportProcessing.ParameterInfo(parameterDef);
					if (parameterDef.PromptExpression != null)
					{
						if (parameterDef.PromptExpression.Type == Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant)
						{
							parameterInfo.DynamicPrompt = false;
							parameterInfo.Prompt = parameterDef.PromptExpression.StringValue;
						}
						else
						{
							parameterInfo.DynamicPrompt = true;
							parameterInfo.Prompt = parameterDef.Name;
						}
					}
					if (parameterDef.Dependencies != null && parameterDef.Dependencies.Count > 0)
					{
						int num = 0;
						IDictionaryEnumerator enumerator = parameterDef.Dependencies.GetEnumerator();
						List<Microsoft.ReportingServices.ReportIntermediateFormat.ParameterDef> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.ParameterDef>();
						ParameterInfoCollection parameterInfoCollection = new ParameterInfoCollection();
						while (enumerator.MoveNext())
						{
							num = (int)enumerator.Value;
							list.Add(parameters2[num]);
							parameterInfoCollection.Add(parameters[num]);
							if (parameterDef.UsedInQuery)
							{
								parameters[num].UsedInQuery = true;
							}
						}
						parameterDef.DependencyList = list;
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
							Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = parameterDef.ValidValuesValueExpressions[j];
							Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo2 = parameterDef.ValidValuesLabelExpressions[j];
							if ((expressionInfo != null && Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant != expressionInfo.Type) || (expressionInfo2 != null && Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant != expressionInfo2.Type))
							{
								parameterInfo.DynamicValidValues = true;
							}
						}
						if (!parameterInfo.DynamicValidValues)
						{
							parameterInfo.ValidValues = new ValidValueList(count);
							for (int k = 0; k < count; k++)
							{
								Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo3 = parameterDef.ValidValuesValueExpressions[k];
								Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo4 = parameterDef.ValidValuesLabelExpressions[k];
								object paramValue = null;
								string paramLabel = null;
								if (expressionInfo4 != null)
								{
									paramLabel = expressionInfo4.StringValue;
								}
								if (expressionInfo3 != null)
								{
									switch (expressionInfo3.ConstantType)
									{
									case DataType.Boolean:
										paramValue = expressionInfo3.BoolValue;
										break;
									case DataType.DateTime:
										paramValue = expressionInfo3.GetDateTimeValue();
										break;
									case DataType.Float:
										paramValue = expressionInfo3.FloatValue;
										break;
									case DataType.Integer:
										paramValue = expressionInfo3.IntValue;
										break;
									case DataType.String:
										paramValue = expressionInfo3.StringValue;
										break;
									}
								}
								parameterInfo.AddValidValueExplicit(paramValue, paramLabel);
							}
						}
					}
					parameterInfo.DynamicDefaultValue = (parameterDef.DefaultDataSource != null || parameterDef.DefaultExpressions != null);
					parameterInfo.Values = parameterDef.DefaultValues;
					parameters.Add(parameterInfo);
				}
			}
			else
			{
				parametersNotUsedInQuery = true;
			}
			m_parametersNotUsedInQuery = parametersNotUsedInQuery;
			m_report.ParametersNotUsedInQuery = m_parametersNotUsedInQuery;
		}

		private void Phase4(Dictionary<string, int> groupingExprCountAtScope, out ArrayList dataSetsName)
		{
			PopulateReportItemCollections();
			CompactAggregates(out List<Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion> dataRegions);
			CompactRunningValues(groupingExprCountAtScope);
			for (int i = 0; i < dataRegions.Count; i++)
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataRegion = dataRegions[i];
				bool flag = false;
				if (dataRegion.CellAggregates != null)
				{
					m_aggregateHashByType.Clear();
					CompactAggregates(dataRegion.CellAggregates, nonDataRegionScopedCellAggs: true, m_aggregateHashByType);
					flag = true;
				}
				if (dataRegion.CellPostSortAggregates != null)
				{
					m_aggregateHashByType.Clear();
					if (CompactAggregates(dataRegion.CellPostSortAggregates, nonDataRegionScopedCellAggs: true, m_aggregateHashByType))
					{
						m_report.HasPostSortAggregates = true;
					}
					flag = true;
				}
				if (dataRegion.CellRunningValues != null)
				{
					m_runningValueHashByType.Clear();
					CompactRunningValueList(groupingExprCountAtScope, dataRegion.CellRunningValues, nonDataRegionScopedCellRVs: true, m_runningValueHashByType);
					flag = true;
				}
				if (flag)
				{
					dataRegion.ConvertCellAggregatesToIndexes();
				}
			}
			dataSetsName = null;
			if (m_errorContext.HasError)
			{
				return;
			}
			for (int j = 0; j < m_dataSets.Count; j++)
			{
				if (m_publishingContext.IsRdlx)
				{
					m_dataSets[j].RestrictDataSetAggregates(m_errorContext);
				}
				if (!m_dataSets[j].UsedOnlyInParameters)
				{
					if (dataSetsName == null)
					{
						dataSetsName = new ArrayList();
					}
					dataSetsName.Add(m_dataSets[j].Name);
				}
				else
				{
					m_report.ClearDatasetParameterOnlyDependencies(m_dataSets[j].IndexInCollection);
				}
			}
			m_report.Phase4_DetermineFirstDatasetToProcess();
		}

		private void PopulateReportItemCollections()
		{
			try
			{
				Global.Tracer.Assert(m_reportItemCollectionList != null);
				for (int i = 0; i < m_reportItemCollectionList.Count; i++)
				{
					m_reportItemCollectionList[i].Populate(m_errorContext);
				}
			}
			finally
			{
				m_reportItemCollectionList = null;
			}
		}

		private void CompactAggregates(out List<Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion> dataRegions)
		{
			dataRegions = new List<Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion>();
			try
			{
				List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo> list = null;
				for (int i = 0; i < m_aggregateHolderList.Count; i++)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.IAggregateHolder aggregateHolder = m_aggregateHolderList[i];
					Global.Tracer.Assert(aggregateHolder != null);
					list = aggregateHolder.GetAggregateList();
					Global.Tracer.Assert(list != null);
					m_aggregateHashByType.Clear();
					CompactAggregates(list, nonDataRegionScopedCellAggs: false, m_aggregateHashByType);
					list = aggregateHolder.GetPostSortAggregateList();
					m_aggregateHashByType.Clear();
					if (list != null && CompactAggregates(list, nonDataRegionScopedCellAggs: false, m_aggregateHashByType))
					{
						m_report.HasPostSortAggregates = true;
					}
					DataScopeInfo dataScopeInfo = aggregateHolder.DataScopeInfo;
					if (dataScopeInfo != null)
					{
						BucketedDataAggregateInfos aggregatesOfAggregates = dataScopeInfo.AggregatesOfAggregates;
						if (list != null)
						{
							m_aggregateHashByType.Clear();
							CompactAggregates(aggregatesOfAggregates, nonDataRegionScopedCellAggs: false, m_aggregateHashByType);
						}
						aggregatesOfAggregates = dataScopeInfo.PostSortAggregatesOfAggregates;
						m_aggregateHashByType.Clear();
						if (list != null && CompactAggregates(aggregatesOfAggregates, nonDataRegionScopedCellAggs: false, m_aggregateHashByType))
						{
							m_report.HasPostSortAggregates = true;
						}
						dataScopeInfo.ClearAggregatesIfEmpty();
					}
					if (aggregateHolder is Microsoft.ReportingServices.ReportIntermediateFormat.Grouping)
					{
						if (CompactAggregates(((Microsoft.ReportingServices.ReportIntermediateFormat.Grouping)aggregateHolder).RecursiveAggregates, nonDataRegionScopedCellAggs: false, m_aggregateHashByType))
						{
							m_report.NeedPostGroupProcessing = true;
						}
					}
					else if (aggregateHolder is Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion && ((Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion)aggregateHolder).IsDataRegion)
					{
						dataRegions.Add(aggregateHolder as Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion);
					}
					aggregateHolder.ClearIfEmpty();
				}
			}
			finally
			{
				m_aggregateHolderList = null;
			}
		}

		private bool CompactAggregates(BucketedDataAggregateInfos aggregates, bool nonDataRegionScopedCellAggs, Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes, Dictionary<string, Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo>>> aggregateHashByType)
		{
			bool flag = false;
			foreach (AggregateBucket<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo> bucket in aggregates.Buckets)
			{
				if (bucket.Aggregates != null)
				{
					flag |= CompactAggregates(bucket.Aggregates, nonDataRegionScopedCellAggs, aggregateHashByType);
				}
			}
			return flag;
		}

		private bool CompactAggregates(List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo> aggregates, bool nonDataRegionScopedCellAggs, Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes, Dictionary<string, Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo>>> aggregateHashByType)
		{
			Global.Tracer.Assert(aggregates != null);
			Global.Tracer.Assert(aggregateHashByType != null);
			bool result = false;
			for (int num = aggregates.Count - 1; num >= 0; num--)
			{
				result = true;
				Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo dataAggregateInfo = aggregates[num];
				Global.Tracer.Assert(dataAggregateInfo != null);
				string text = null;
				text = ((!nonDataRegionScopedCellAggs) ? "" : dataAggregateInfo.EvaluationScopeName);
				bool flag = false;
				if (aggregateHashByType.TryGetValue(dataAggregateInfo.AggregateType, out Dictionary<string, Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo>> value))
				{
					if (value.TryGetValue(text, out Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo> value2))
					{
						if (value2.TryGetValue(dataAggregateInfo.ExpressionTextForCompaction, out Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo value3))
						{
							if (!dataAggregateInfo.IsAggregateOfAggregate || AreNestedAggregateScopesIdentical(dataAggregateInfo, value3))
							{
								if (value3.DuplicateNames == null)
								{
									value3.DuplicateNames = new List<string>();
								}
								value3.DuplicateNames.Add(dataAggregateInfo.Name);
								if (dataAggregateInfo.DuplicateNames != null)
								{
									value3.DuplicateNames.AddRange(dataAggregateInfo.DuplicateNames);
								}
								aggregates.RemoveAt(num);
								flag = true;
							}
						}
						else
						{
							value2.Add(dataAggregateInfo.ExpressionTextForCompaction, dataAggregateInfo);
						}
					}
					else
					{
						value2 = new Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo>();
						value2.Add(dataAggregateInfo.ExpressionTextForCompaction, dataAggregateInfo);
						value.Add(text, value2);
					}
				}
				else
				{
					Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo> dictionary = new Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo>();
					dictionary.Add(dataAggregateInfo.ExpressionTextForCompaction, dataAggregateInfo);
					value = new Dictionary<string, Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo>>();
					value.Add(text, dictionary);
					aggregateHashByType.Add(dataAggregateInfo.AggregateType, value);
				}
				if (!flag)
				{
					ProcessPostCompactedAggOrRv(dataAggregateInfo);
				}
			}
			return result;
		}

		private bool AreNestedAggregateScopesIdentical(Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo agg1, Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo agg2)
		{
			List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo> nestedAggregates = agg1.PublishingInfo.NestedAggregates;
			List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo> nestedAggregates2 = agg2.PublishingInfo.NestedAggregates;
			Global.Tracer.Assert(nestedAggregates.Count == nestedAggregates2.Count, "Duplicate candidate has identical text but different number of nested aggs.");
			for (int i = 0; i < nestedAggregates.Count; i++)
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo dataAggregateInfo = nestedAggregates[i];
				Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo dataAggregateInfo2 = nestedAggregates2[i];
				if (dataAggregateInfo.EvaluationScope != dataAggregateInfo2.EvaluationScope)
				{
					return false;
				}
				if (dataAggregateInfo.IsAggregateOfAggregate && !AreNestedAggregateScopesIdentical(dataAggregateInfo, dataAggregateInfo2))
				{
					return false;
				}
			}
			return true;
		}

		private void ProcessPostCompactedAggOrRv(Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo aggregate)
		{
			if (!aggregate.IsAggregateOfAggregate)
			{
				TraceAggregateInformation(aggregate, null);
				return;
			}
			IRIFDataScope evaluationScope = aggregate.EvaluationScope;
			if (evaluationScope == null)
			{
				return;
			}
			foreach (Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo nestedAggregate in aggregate.PublishingInfo.NestedAggregates)
			{
				IRIFDataScope evaluationScope2 = nestedAggregate.EvaluationScope;
				if (evaluationScope2 != null)
				{
					if (!m_scopeTree.IsSameOrProperParentScope(aggregate.EvaluationScope, evaluationScope2))
					{
						RegisterIncompatibleAggregateScopes(aggregate.EvaluationScope, evaluationScope2, ProcessingErrorCode.rsInvalidNestedAggregateScope, aggregate);
					}
					if (m_scopeTree.IsSameOrProperParentScope(evaluationScope, evaluationScope2))
					{
						evaluationScope = nestedAggregate.EvaluationScope;
					}
					else if (!m_scopeTree.IsSameOrProperParentScope(evaluationScope2, evaluationScope))
					{
						RegisterIncompatibleAggregateScopes(evaluationScope, evaluationScope2, ProcessingErrorCode.rsIncompatibleNestedAggregateScopes, aggregate);
					}
				}
			}
			if (evaluationScope != null && evaluationScope.DataScopeInfo != null)
			{
				aggregate.UpdatesAtRowScope = aggregate.PublishingInfo.HasAnyFieldReferences;
				evaluationScope.DataScopeInfo.HasAggregatesToUpdateAtRowScope |= aggregate.PublishingInfo.HasAnyFieldReferences;
				aggregate.UpdateScopeID = evaluationScope.DataScopeInfo.ScopeID;
				if (!aggregate.IsRunningValue() && !aggregate.IsPostSortAggregate())
				{
					m_scopeTree.Traverse(CheckSpannedGroupFilters, aggregate.EvaluationScope, evaluationScope, visitOuterScope: false);
					m_report.NeedPostGroupProcessing = true;
				}
			}
			TraceAggregateInformation(aggregate, evaluationScope);
		}

		private void RegisterIncompatibleAggregateScopes(IRIFDataScope firstScope, IRIFDataScope secondScope, ProcessingErrorCode fallbackMessage, Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo aggregate)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode reportHierarchyNode = firstScope as Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode;
			Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode reportHierarchyNode2 = secondScope as Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode;
			if (reportHierarchyNode != null && reportHierarchyNode2 != null && reportHierarchyNode.DataRegionDef == reportHierarchyNode2.DataRegionDef && reportHierarchyNode.IsColumn != reportHierarchyNode2.IsColumn)
			{
				RegisterAggregateScopeValidationError(ProcessingErrorCode.rsNestedAggregateScopesFromDifferentAxes, aggregate);
			}
			else if (secondScope is Cell)
			{
				RegisterAggregateScopeValidationError(ProcessingErrorCode.rsNestedAggregateScopeRequired, aggregate);
			}
			else
			{
				RegisterAggregateScopeValidationError(fallbackMessage, aggregate);
			}
		}

		private void RegisterAggregateScopeValidationError(ProcessingErrorCode errorCode, Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo aggregate)
		{
			m_errorContext.Register(errorCode, Severity.Error, aggregate.PublishingInfo.ObjectType, aggregate.PublishingInfo.ObjectName, aggregate.PublishingInfo.PropertyName);
		}

		private void TraceAggregateInformation(Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo aggregate, IRIFDataScope commonAggregateScope)
		{
			if (!Global.Tracer.TraceVerbose || aggregate.AggregateType == Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes.Previous)
			{
				return;
			}
			if (!m_wroteAggregateHeaderInformation)
			{
				m_wroteAggregateHeaderInformation = true;
				Global.Tracer.Trace(TraceLevel.Verbose, "Aggregate Debugging Information (duplicate items removed):");
			}
			string scopeName = GetScopeName(aggregate.EvaluationScope);
			Microsoft.ReportingServices.ReportIntermediateFormat.RunningValueInfo runningValueInfo = aggregate as Microsoft.ReportingServices.ReportIntermediateFormat.RunningValueInfo;
			string text = (runningValueInfo == null) ? scopeName : runningValueInfo.Scope;
			string text2;
			if (commonAggregateScope == null)
			{
				text2 = scopeName + "-ROW";
			}
			else
			{
				text2 = GetScopeName(commonAggregateScope);
				if (aggregate.UpdatesAtRowScope)
				{
					text2 += "-ROW";
				}
			}
			Global.Tracer.Trace(TraceLevel.Verbose, "    Aggregate: ContainingObject:{4} ContainingProperty:{5} Text:{0} UpdateScope:{1} OutputScope:{2} ResetScope:{3}", aggregate.GetAsString().MarkAsPrivate(), text2, scopeName, text, aggregate.PublishingInfo.ObjectName.MarkAsPrivate(), aggregate.PublishingInfo.PropertyName);
		}

		private string GetScopeName(IRIFDataScope dataScope)
		{
			if (dataScope == null)
			{
				return "UNKNOWN";
			}
			Cell cell = dataScope as Cell;
			if (cell != null)
			{
				return m_scopeTree.GetScopeName(cell);
			}
			return dataScope.Name;
		}

		private void CheckSpannedGroupFilters(IRIFDataScope dataScope)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode reportHierarchyNode = dataScope as Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode;
			if (reportHierarchyNode != null && reportHierarchyNode.Grouping != null && reportHierarchyNode.Grouping.Filters != null && reportHierarchyNode.Grouping.Filters.Count > 0)
			{
				reportHierarchyNode.DataScopeInfo.AggregatesSpanGroupFilter = true;
			}
		}

		private void CompactRunningValues(Dictionary<string, int> groupingExprCountAtScope)
		{
			try
			{
				for (int i = 0; i < m_runningValueHolderList.Count; i++)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.IRunningValueHolder runningValueHolder = m_runningValueHolderList[i];
					Global.Tracer.Assert(runningValueHolder != null);
					m_runningValueHashByType.Clear();
					CompactRunningValueList(groupingExprCountAtScope, runningValueHolder.GetRunningValueList(), nonDataRegionScopedCellRVs: false, m_runningValueHashByType);
					DataScopeInfo dataScopeInfo = runningValueHolder.DataScopeInfo;
					if (dataScopeInfo != null)
					{
						m_runningValueHashByType.Clear();
						CompactRunningValueList(groupingExprCountAtScope, dataScopeInfo.RunningValuesOfAggregates, nonDataRegionScopedCellRVs: false, m_runningValueHashByType);
						dataScopeInfo.ClearRunningValuesIfEmpty();
					}
					runningValueHolder.ClearIfEmpty();
				}
			}
			finally
			{
				m_runningValueHolderList = null;
			}
		}

		private void CompactRunningValueList(Dictionary<string, int> groupingExprCountAtScope, List<Microsoft.ReportingServices.ReportIntermediateFormat.RunningValueInfo> runningValueList, bool nonDataRegionScopedCellRVs, Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes, Dictionary<string, AllowNullKeyDictionary<string, Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.RunningValueInfo>>>> runningValueHashByType)
		{
			Global.Tracer.Assert(runningValueList != null);
			Global.Tracer.Assert(runningValueHashByType != null);
			for (int num = runningValueList.Count - 1; num >= 0; num--)
			{
				m_report.HasPostSortAggregates = true;
				Microsoft.ReportingServices.ReportIntermediateFormat.RunningValueInfo runningValueInfo = runningValueList[num];
				Global.Tracer.Assert(runningValueInfo != null);
				string text = null;
				text = ((!nonDataRegionScopedCellRVs) ? "" : runningValueInfo.EvaluationScopeName);
				if (runningValueInfo.AggregateType == Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes.Previous && runningValueInfo.Scope != null)
				{
					if (groupingExprCountAtScope.TryGetValue(runningValueInfo.Scope, out int value))
					{
						runningValueInfo.TotalGroupingExpressionCount = value;
					}
					else
					{
						Global.Tracer.Assert(condition: false);
					}
				}
				bool flag = false;
				if (runningValueHashByType.TryGetValue(runningValueInfo.AggregateType, out Dictionary<string, AllowNullKeyDictionary<string, Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.RunningValueInfo>>> value2))
				{
					if (value2.TryGetValue(text, out AllowNullKeyDictionary<string, Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.RunningValueInfo>> value3))
					{
						if (value3.TryGetValue(runningValueInfo.Scope, out Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.RunningValueInfo> value4))
						{
							if (value4.TryGetValue(runningValueInfo.ExpressionTextForCompaction, out Microsoft.ReportingServices.ReportIntermediateFormat.RunningValueInfo value5))
							{
								if (value5.DuplicateNames == null)
								{
									value5.DuplicateNames = new List<string>();
								}
								value5.DuplicateNames.Add(runningValueInfo.Name);
								if (runningValueInfo.DuplicateNames != null)
								{
									value5.DuplicateNames.AddRange(runningValueInfo.DuplicateNames);
								}
								runningValueList.RemoveAt(num);
								flag = true;
							}
							else
							{
								value4.Add(runningValueInfo.ExpressionTextForCompaction, runningValueInfo);
							}
						}
						else
						{
							value4 = new Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.RunningValueInfo>();
							value4.Add(runningValueInfo.ExpressionTextForCompaction, runningValueInfo);
							value3.Add(runningValueInfo.Scope, value4);
						}
					}
					else
					{
						Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.RunningValueInfo> dictionary = new Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.RunningValueInfo>();
						dictionary.Add(runningValueInfo.ExpressionTextForCompaction, runningValueInfo);
						value3 = new AllowNullKeyDictionary<string, Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.RunningValueInfo>>();
						value3.Add(runningValueInfo.Scope, dictionary);
						value2.Add(text, value3);
					}
				}
				else
				{
					Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.RunningValueInfo> dictionary2 = new Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.RunningValueInfo>();
					dictionary2.Add(runningValueInfo.ExpressionTextForCompaction, runningValueInfo);
					AllowNullKeyDictionary<string, Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.RunningValueInfo>> allowNullKeyDictionary = new AllowNullKeyDictionary<string, Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.RunningValueInfo>>();
					allowNullKeyDictionary.Add(runningValueInfo.Scope, dictionary2);
					value2 = new Dictionary<string, AllowNullKeyDictionary<string, Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.RunningValueInfo>>>();
					value2.Add(text, allowNullKeyDictionary);
					runningValueHashByType.Add(runningValueInfo.AggregateType, value2);
				}
				if (!flag)
				{
					ProcessPostCompactedAggOrRv(runningValueInfo);
				}
			}
			runningValueHashByType.Clear();
		}

		internal DataSetPublishingResult CreateSharedDataSet(byte[] definition)
		{
			if (definition == null)
			{
				m_errorContext.Register(ProcessingErrorCode.rsNotASharedDataSetDefinition, Severity.Error, Microsoft.ReportingServices.ReportProcessing.ObjectType.SharedDataSet, null, null);
				throw new DataSetPublishingException(m_errorContext.Messages);
			}
			RSDPhase1(definition);
			RSDPhase3(out ParameterInfoCollection parameters);
			if (!m_errorContext.HasError)
			{
				DataSourceInfo theOnlyDataSource = m_dataSources.GetTheOnlyDataSource();
				DataSetDefinition dataSetDefinition = new DataSetDefinition(m_dataSetCore, m_description, theOnlyDataSource, parameters);
				SerializeSharedDataSetDefinition();
				if (m_userReferenceLocation != UserLocationFlags.None)
				{
					m_userReferenceLocation = UserLocationFlags.ReportQueries;
				}
				return new DataSetPublishingResult(dataSetDefinition, theOnlyDataSource, m_userReferenceLocation, m_errorContext.Messages);
			}
			throw new DataSetPublishingException(m_errorContext.Messages);
		}

		private void SerializeSharedDataSetDefinition()
		{
			if (m_dataSetCore != null && m_publishingContext.CreateChunkFactory != null)
			{
				Stream stream = null;
				try
				{
					stream = m_publishingContext.CreateChunkFactory.CreateChunk("CompiledDefinition", Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.CompiledDefinition, null);
					int compatibilityVersion = ReportProcessingCompatibilityVersion.GetCompatibilityVersion(m_publishingContext.Configuration);
					new IntermediateFormatWriter(stream, compatibilityVersion).Write(m_dataSetCore);
				}
				finally
				{
					stream?.Close();
				}
			}
		}

		private void RSDPhase1(byte[] definition)
		{
			try
			{
				m_dataSources = new DataSourceInfoCollection();
				Stream stream = new MemoryStream(definition, writable: false);
				Pair<string, Stream> pair = default(Pair<string, Stream>);
				List<Pair<string, Stream>> list = new List<Pair<string, Stream>>();
				pair = GetRSDNamespaceSchemaStreamPair("http://schemas.microsoft.com/sqlserver/reporting/2010/01/shareddatasetdefinition", "Microsoft.ReportingServices.ReportProcessing.ReportPublishing.SharedDataSetDefinition.xsd");
				list.Add(pair);
				m_reader = new RmlValidatingReader(stream, list, m_errorContext, RmlValidatingReader.ItemType.Rsd);
				while (m_reader.Read())
				{
					if (XmlNodeType.Element == m_reader.NodeType && "SharedDataSet" == m_reader.LocalName)
					{
						m_reportCT = new ExprHostCompiler(new Microsoft.ReportingServices.RdlExpressions.VBExpressionParser(m_errorContext), m_errorContext);
						ReadRSDSharedDataSet();
					}
				}
				if (m_dataSetCore == null)
				{
					m_errorContext.Register(ProcessingErrorCode.rsNotASharedDataSetDefinition, Severity.Error, Microsoft.ReportingServices.ReportProcessing.ObjectType.SharedDataSet, null, "Namespace");
					throw new ReportProcessingException(m_errorContext.Messages);
				}
			}
			catch (XmlException ex)
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidSharedDataSetDefinition, Severity.Error, Microsoft.ReportingServices.ReportProcessing.ObjectType.SharedDataSet, null, null, ex.Message);
				throw new ReportProcessingException(m_errorContext.Messages);
			}
			finally
			{
				if (m_reader != null)
				{
					m_reader.Close();
					m_reader = null;
				}
			}
		}

		private Pair<string, Stream> GetRSDNamespaceSchemaStreamPair(string validationNamespace, string xsdResource)
		{
			Stream stream = null;
			stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(xsdResource);
			Global.Tracer.Assert(stream != null, "(schemaStream != null)");
			return new Pair<string, Stream>(validationNamespace, stream);
		}

		private void ReadRSDSharedDataSet()
		{
			int maxExpressionLength = -1;
			if (m_publishingContext.IsRdlSandboxingEnabled)
			{
				maxExpressionLength = m_publishingContext.Configuration.RdlSandboxing.MaxExpressionLength;
			}
			PublishingContextStruct context = new PublishingContextStruct(LocationFlags.None, Microsoft.ReportingServices.ReportProcessing.ObjectType.SharedDataSet, maxExpressionLength, m_errorContext);
			bool flag = false;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName = m_reader.LocalName;
					if (!(localName == "Description"))
					{
						if (localName == "DataSet")
						{
							m_dataSets.Add(ReadRSDDataSet(context));
						}
					}
					else
					{
						m_description = m_reader.ReadString();
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("SharedDataSet" == m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.DataSet ReadRSDDataSet(PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet = new Microsoft.ReportingServices.ReportIntermediateFormat.DataSet(GenerateID(), m_dataSetIndexCounter++);
			m_dataSetCore = dataSet.DataSetCore;
			m_dataSetCore.Name = m_reader.GetAttribute("Name");
			context.Location |= LocationFlags.InDataSet;
			context.ObjectType = Microsoft.ReportingServices.ReportProcessing.ObjectType.SharedDataSet;
			context.ObjectName = m_dataSetCore.Name;
			m_reportScopes.Add(m_dataSetCore.Name, dataSet);
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
						m_dataSetCore.Fields = ReadFields(context, out int calculatedFieldStartIndex);
						m_dataSetCore.NonCalculatedFieldCount = calculatedFieldStartIndex;
						break;
					}
					case "Query":
						m_dataSetCore.Query = ReadRSDQuery(context);
						break;
					case "CaseSensitivity":
						m_dataSetCore.CaseSensitivity = ReadTriState();
						break;
					case "Collation":
					{
						m_dataSetCore.Collation = m_reader.ReadString();
						if (DataSetValidator.ValidateCollation(m_dataSetCore.Collation, out uint lcid))
						{
							m_dataSetCore.LCID = lcid;
							break;
						}
						m_errorContext.Register(ProcessingErrorCode.rsInvalidCollationName, Severity.Warning, context.ObjectType, context.ObjectName, null, m_dataSetCore.Collation);
						break;
					}
					case "AccentSensitivity":
						m_dataSetCore.AccentSensitivity = ReadTriState();
						break;
					case "KanatypeSensitivity":
						m_dataSetCore.KanatypeSensitivity = ReadTriState();
						break;
					case "WidthSensitivity":
						m_dataSetCore.WidthSensitivity = ReadTriState();
						break;
					case "Filters":
						m_dataSetCore.Filters = ReadFilters(Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.DataSetFilters, context);
						break;
					case "InterpretSubtotalsAsDetails":
						m_dataSetCore.InterpretSubtotalsAsDetails = ReadTriState();
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
			if (m_dataSetCore.Fields == null || m_dataSetCore.Fields.Count == 0)
			{
				m_errorContext.Register(ProcessingErrorCode.rsDataSetWithoutFields, Severity.Error, context.ObjectType, context.ObjectName, null);
			}
			return dataSet;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ReportQuery ReadRSDQuery(PublishingContextStruct context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ReportQuery reportQuery = new Microsoft.ReportingServices.ReportIntermediateFormat.ReportQuery();
			Microsoft.ReportingServices.ReportIntermediateFormat.DataSource dataSource = new Microsoft.ReportingServices.ReportIntermediateFormat.DataSource(GenerateID());
			bool flag = false;
			bool hasComplexParams = false;
			Dictionary<string, bool> parametersInQuery = null;
			do
			{
				m_reader.Read();
				switch (m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (m_reader.LocalName)
					{
					case "DataSourceReference":
					{
						dataSource.DataSourceReference = m_reader.ReadString();
						string text3 = reportQuery.DataSourceName = (dataSource.Name = "DataSetDataSource");
						break;
					}
					case "CommandType":
						reportQuery.CommandType = ReadCommandType();
						break;
					case "CommandText":
					{
						Microsoft.ReportingServices.ReportProcessing.ObjectType objectType = context.ObjectType;
						context.ObjectType = Microsoft.ReportingServices.ReportProcessing.ObjectType.Query;
						reportQuery.CommandText = ReadQueryOrParameterExpression(context, DataType.String, ref hasComplexParams, parametersInQuery);
						context.ObjectType = objectType;
						break;
					}
					case "DataSetParameters":
						reportQuery.Parameters = ReadQueryParameters(context, ref hasComplexParams, parametersInQuery);
						SharedDataSetParameterNameMapper.MakeUnique(reportQuery.Parameters);
						break;
					case "Timeout":
						reportQuery.TimeOut = m_reader.ReadInteger(context.ObjectType, context.ObjectName, m_reader.LocalName);
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
			if (reportQuery.DataSourceName != null)
			{
				DataSourceInfo dataSourceInfo = CreateSharedDataSourceLink(context, dataSource);
				if (dataSourceInfo != null)
				{
					if (m_publishingContext.ResolveTemporaryDataSourceCallback != null)
					{
						m_publishingContext.ResolveTemporaryDataSourceCallback(dataSourceInfo, m_publishingContext.OriginalDataSources);
					}
					dataSource.ID = dataSourceInfo.ID;
					m_dataSources.Add(dataSourceInfo);
				}
			}
			return reportQuery;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ParameterValue ReadRSDDataSetParameter(PublishingContextStruct context, ref bool isComplex, Dictionary<string, bool> parametersInQuery)
		{
			Global.Tracer.Assert(Microsoft.ReportingServices.ReportProcessing.ObjectType.SharedDataSet == context.ObjectType);
			DataSetParameterValue dataSetParameterValue = new DataSetParameterValue();
			dataSetParameterValue.UniqueName = m_reader.GetAttribute("UniqueName");
			dataSetParameterValue.Name = m_reader.GetAttribute("Name");
			context.ObjectType = Microsoft.ReportingServices.ReportProcessing.ObjectType.QueryParameter;
			context.ObjectName = dataSetParameterValue.Name;
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
						case "DefaultValue":
							dataSetParameterValue.ConstantDataType = ReadDataTypeAttribute();
							dataSetParameterValue.Value = ReadQueryOrParameterExpression(context, dataSetParameterValue.ConstantDataType, ref isComplex, parametersInQuery);
							break;
						case "ReadOnly":
							dataSetParameterValue.ReadOnly = m_reader.ReadBoolean(context.ObjectType, context.ObjectName, "ReadOnly");
							break;
						case "Nullable":
							dataSetParameterValue.Nullable = m_reader.ReadBoolean(context.ObjectType, context.ObjectName, "Nullable");
							break;
						case "OmitFromQuery":
							dataSetParameterValue.OmitFromQuery = m_reader.ReadBoolean(context.ObjectType, context.ObjectName, "OmitFromQuery");
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("DataSetParameter" == m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			if (dataSetParameterValue.ReadOnly && !dataSetParameterValue.Nullable && (dataSetParameterValue.Value == null || dataSetParameterValue.Value.OriginalText == null))
			{
				m_errorContext.Register(ProcessingErrorCode.rsMissingDataSetParameterDefault, Severity.Error, context.ObjectType, context.ObjectName, null);
			}
			return dataSetParameterValue;
		}

		private void RSDPhase3(out ParameterInfoCollection parameters)
		{
			try
			{
				m_report = new Microsoft.ReportingServices.ReportIntermediateFormat.Report();
				m_reportCT.Builder.SharedDataSetStart();
				Microsoft.ReportingServices.ReportIntermediateFormat.InitializationContext context = new Microsoft.ReportingServices.ReportIntermediateFormat.InitializationContext(m_publishingContext.CatalogContext, m_dataSets, m_errorContext, m_reportCT.Builder, m_report, m_reportScopes, m_publishingContext);
				context.RSDRegisterDataSetParameters(m_dataSetCore);
				context.ExprHostBuilder.DataSetStart(m_dataSetCore.Name);
				context.Location |= LocationFlags.InDataSet;
				context.ObjectType = Microsoft.ReportingServices.ReportProcessing.ObjectType.SharedDataSet;
				context.ObjectName = m_dataSetCore.Name;
				m_dataSetCore.Initialize(context);
				InitializeDataSetParameters(context, out parameters);
				m_dataSetCore.ExprHostID = context.ExprHostBuilder.DataSetEnd();
				m_reportCT.Builder.SharedDataSetEnd();
				if (!m_errorContext.HasError)
				{
					((IExpressionHostAssemblyHolder)m_dataSetCore).CompiledCode = m_reportCT.Compile(m_dataSetCore, m_publishingContext.CompilationTempAppDomain, m_publishingContext.GenerateExpressionHostWithRefusedPermissions, m_publishingContext.PublishingVersioning);
				}
			}
			finally
			{
				m_reportCT = null;
			}
		}

		private void InitializeDataSetParameters(Microsoft.ReportingServices.ReportIntermediateFormat.InitializationContext context, out ParameterInfoCollection parameters)
		{
			parameters = new ParameterInfoCollection();
			List<Microsoft.ReportingServices.ReportIntermediateFormat.ParameterValue> parameters2 = m_dataSetCore.Query.Parameters;
			if (parameters2 == null || parameters2.Count <= 0)
			{
				return;
			}
			foreach (DataSetParameterValue item in parameters2)
			{
				bool usedInQuery = true;
				Microsoft.ReportingServices.ReportProcessing.ParameterInfo parameterInfo = new Microsoft.ReportingServices.ReportProcessing.ParameterInfo(item, usedInQuery);
				parameters.Add(parameterInfo);
				if (item.Value != null && item.Value.IsExpression)
				{
					parameterInfo.DynamicDefaultValue = true;
				}
			}
		}
	}
}
