using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Microsoft.ReportingServices.ReportRendering;
using System;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class Report : ReportItem, IAggregateHolder
	{
		internal enum ShowHideTypes
		{
			None,
			Static,
			Interactive
		}

		private IntermediateFormatVersion m_intermediateFormatVersion;

		private Guid m_reportVersion = Guid.Empty;

		private string m_author;

		private int m_autoRefresh;

		private EmbeddedImageHashtable m_embeddedImages;

		private PageSection m_pageHeader;

		private PageSection m_pageFooter;

		private ReportItemCollection m_reportItems;

		private DataSourceList m_dataSources;

		private string m_pageHeight = "11in";

		private double m_pageHeightValue;

		private string m_pageWidth = "8.5in";

		private double m_pageWidthValue;

		private string m_leftMargin = "0in";

		private double m_leftMarginValue;

		private string m_rightMargin = "0in";

		private double m_rightMarginValue;

		private string m_topMargin = "0in";

		private double m_topMarginValue;

		private string m_bottomMargin = "0in";

		private double m_bottomMarginValue;

		private int m_columns = 1;

		private string m_columnSpacing = "0.5in";

		private double m_columnSpacingValue;

		private DataAggregateInfoList m_pageAggregates;

		private byte[] m_exprCompiledCode;

		private bool m_exprCompiledCodeGeneratedWithRefusedPermissions;

		private bool m_mergeOnePass;

		private bool m_pageMergeOnePass;

		private bool m_subReportMergeTransactions;

		private bool m_needPostGroupProcessing;

		private bool m_hasPostSortAggregates;

		private bool m_hasReportItemReferences;

		private ShowHideTypes m_showHideType;

		private ImageStreamNames m_imageStreamNames;

		private int m_lastID;

		private int m_bodyID;

		private SubReportList m_subReports;

		private bool m_hasImageStreams;

		private bool m_hasLabels;

		private bool m_hasBookmarks;

		private bool m_parametersNotUsedInQuery;

		private ParameterDefList m_parameters;

		private string m_oneDataSetName;

		private StringList m_codeModules;

		private CodeClassList m_codeClasses;

		private bool m_hasSpecialRecursiveAggregates;

		private ExpressionInfo m_language;

		private string m_dataTransform;

		private string m_dataSchema;

		private bool m_dataElementStyleAttribute = true;

		private string m_code;

		private bool m_hasUserSortFilter;

		private string m_interactiveHeight;

		private double m_interactiveHeightValue = -1.0;

		private string m_interactiveWidth;

		private double m_interactiveWidthValue = -1.0;

		private InScopeSortFilterHashtable m_nonDetailSortFiltersInScope;

		private InScopeSortFilterHashtable m_detailSortFiltersInScope;

		[NonSerialized]
		private int m_lastAggregateID = -1;

		[NonSerialized]
		private ReportExprHost m_exprHost;

		[NonSerialized]
		private ReportSize m_pageHeightForRendering;

		[NonSerialized]
		private ReportSize m_pageWidthForRendering;

		[NonSerialized]
		private ReportSize m_leftMarginForRendering;

		[NonSerialized]
		private ReportSize m_rightMarginForRendering;

		[NonSerialized]
		private ReportSize m_topMarginForRendering;

		[NonSerialized]
		private ReportSize m_bottomMarginForRendering;

		[NonSerialized]
		private ReportSize m_columnSpacingForRendering;

		[NonSerialized]
		private long m_mainChunkSize = -1L;

		internal override ObjectType ObjectType => ObjectType.Report;

		internal override string DataElementNameDefault => "Report";

		internal IntermediateFormatVersion IntermediateFormatVersion => m_intermediateFormatVersion;

		internal Guid ReportVersion => m_reportVersion;

		internal string Author
		{
			get
			{
				return m_author;
			}
			set
			{
				m_author = value;
			}
		}

		internal int AutoRefresh
		{
			get
			{
				return m_autoRefresh;
			}
			set
			{
				m_autoRefresh = value;
			}
		}

		internal EmbeddedImageHashtable EmbeddedImages
		{
			get
			{
				return m_embeddedImages;
			}
			set
			{
				m_embeddedImages = value;
			}
		}

		internal PageSection PageHeader
		{
			get
			{
				return m_pageHeader;
			}
			set
			{
				m_pageHeader = value;
			}
		}

		internal bool PageHeaderEvaluation
		{
			get
			{
				if (m_pageHeader == null)
				{
					return false;
				}
				return m_pageHeader.PostProcessEvaluate;
			}
		}

		internal PageSection PageFooter
		{
			get
			{
				return m_pageFooter;
			}
			set
			{
				m_pageFooter = value;
			}
		}

		internal bool PageFooterEvaluation
		{
			get
			{
				if (m_pageFooter == null)
				{
					return false;
				}
				return m_pageFooter.PostProcessEvaluate;
			}
		}

		internal double PageSectionWidth
		{
			get
			{
				double num = m_widthValue;
				if (m_columns > 1)
				{
					num += (double)(m_columns - 1) * (num + m_columnSpacingValue);
				}
				return num;
			}
		}

		internal ReportItemCollection ReportItems
		{
			get
			{
				return m_reportItems;
			}
			set
			{
				m_reportItems = value;
			}
		}

		internal DataSourceList DataSources
		{
			get
			{
				return m_dataSources;
			}
			set
			{
				m_dataSources = value;
			}
		}

		internal int DataSourceCount
		{
			get
			{
				if (m_dataSources != null)
				{
					return m_dataSources.Count;
				}
				return 0;
			}
		}

		internal string PageHeight
		{
			get
			{
				return m_pageHeight;
			}
			set
			{
				m_pageHeight = value;
			}
		}

		internal double PageHeightValue
		{
			get
			{
				return m_pageHeightValue;
			}
			set
			{
				m_pageHeightValue = value;
			}
		}

		internal string PageWidth
		{
			get
			{
				return m_pageWidth;
			}
			set
			{
				m_pageWidth = value;
			}
		}

		internal double PageWidthValue
		{
			get
			{
				return m_pageWidthValue;
			}
			set
			{
				m_pageWidthValue = value;
			}
		}

		internal string LeftMargin
		{
			get
			{
				return m_leftMargin;
			}
			set
			{
				m_leftMargin = value;
			}
		}

		internal double LeftMarginValue
		{
			get
			{
				return m_leftMarginValue;
			}
			set
			{
				m_leftMarginValue = value;
			}
		}

		internal string RightMargin
		{
			get
			{
				return m_rightMargin;
			}
			set
			{
				m_rightMargin = value;
			}
		}

		internal double RightMarginValue
		{
			get
			{
				return m_rightMarginValue;
			}
			set
			{
				m_rightMarginValue = value;
			}
		}

		internal string TopMargin
		{
			get
			{
				return m_topMargin;
			}
			set
			{
				m_topMargin = value;
			}
		}

		internal double TopMarginValue
		{
			get
			{
				return m_topMarginValue;
			}
			set
			{
				m_topMarginValue = value;
			}
		}

		internal string BottomMargin
		{
			get
			{
				return m_bottomMargin;
			}
			set
			{
				m_bottomMargin = value;
			}
		}

		internal double BottomMarginValue
		{
			get
			{
				return m_bottomMarginValue;
			}
			set
			{
				m_bottomMarginValue = value;
			}
		}

		internal int Columns
		{
			get
			{
				return m_columns;
			}
			set
			{
				m_columns = value;
			}
		}

		internal string ColumnSpacing
		{
			get
			{
				return m_columnSpacing;
			}
			set
			{
				m_columnSpacing = value;
			}
		}

		internal double ColumnSpacingValue
		{
			get
			{
				return m_columnSpacingValue;
			}
			set
			{
				m_columnSpacingValue = value;
			}
		}

		internal DataAggregateInfoList PageAggregates
		{
			get
			{
				return m_pageAggregates;
			}
			set
			{
				m_pageAggregates = value;
			}
		}

		internal byte[] CompiledCode
		{
			get
			{
				return m_exprCompiledCode;
			}
			set
			{
				m_exprCompiledCode = value;
			}
		}

		internal bool CompiledCodeGeneratedWithRefusedPermissions
		{
			get
			{
				return m_exprCompiledCodeGeneratedWithRefusedPermissions;
			}
			set
			{
				m_exprCompiledCodeGeneratedWithRefusedPermissions = value;
			}
		}

		internal bool MergeOnePass
		{
			get
			{
				return m_mergeOnePass;
			}
			set
			{
				m_mergeOnePass = value;
			}
		}

		internal bool PageMergeOnePass
		{
			get
			{
				return m_pageMergeOnePass;
			}
			set
			{
				m_pageMergeOnePass = value;
			}
		}

		internal bool SubReportMergeTransactions
		{
			get
			{
				return m_subReportMergeTransactions;
			}
			set
			{
				m_subReportMergeTransactions = value;
			}
		}

		internal bool NeedPostGroupProcessing
		{
			get
			{
				return m_needPostGroupProcessing;
			}
			set
			{
				m_needPostGroupProcessing = value;
			}
		}

		internal bool HasPostSortAggregates
		{
			get
			{
				return m_hasPostSortAggregates;
			}
			set
			{
				m_hasPostSortAggregates = value;
			}
		}

		internal bool HasReportItemReferences
		{
			get
			{
				return m_hasReportItemReferences;
			}
			set
			{
				m_hasReportItemReferences = value;
			}
		}

		internal ShowHideTypes ShowHideType
		{
			get
			{
				return m_showHideType;
			}
			set
			{
				m_showHideType = value;
			}
		}

		internal ImageStreamNames ImageStreamNames
		{
			get
			{
				return m_imageStreamNames;
			}
			set
			{
				m_imageStreamNames = value;
			}
		}

		internal bool ParametersNotUsedInQuery
		{
			get
			{
				return m_parametersNotUsedInQuery;
			}
			set
			{
				m_parametersNotUsedInQuery = value;
			}
		}

		internal int LastID
		{
			get
			{
				return m_lastID;
			}
			set
			{
				m_lastID = value;
			}
		}

		internal int BodyID
		{
			get
			{
				return m_bodyID;
			}
			set
			{
				m_bodyID = value;
			}
		}

		internal SubReportList SubReports
		{
			get
			{
				return m_subReports;
			}
			set
			{
				m_subReports = value;
			}
		}

		internal bool HasImageStreams
		{
			get
			{
				return m_hasImageStreams;
			}
			set
			{
				m_hasImageStreams = value;
			}
		}

		internal bool HasLabels
		{
			get
			{
				return m_hasLabels;
			}
			set
			{
				m_hasLabels = value;
			}
		}

		internal bool HasBookmarks
		{
			get
			{
				return m_hasBookmarks;
			}
			set
			{
				m_hasBookmarks = value;
			}
		}

		internal ReportSize PageHeightForRendering
		{
			get
			{
				return m_pageHeightForRendering;
			}
			set
			{
				m_pageHeightForRendering = value;
			}
		}

		internal ReportSize PageWidthForRendering
		{
			get
			{
				return m_pageWidthForRendering;
			}
			set
			{
				m_pageWidthForRendering = value;
			}
		}

		internal ReportSize LeftMarginForRendering
		{
			get
			{
				return m_leftMarginForRendering;
			}
			set
			{
				m_leftMarginForRendering = value;
			}
		}

		internal ReportSize RightMarginForRendering
		{
			get
			{
				return m_rightMarginForRendering;
			}
			set
			{
				m_rightMarginForRendering = value;
			}
		}

		internal ReportSize TopMarginForRendering
		{
			get
			{
				return m_topMarginForRendering;
			}
			set
			{
				m_topMarginForRendering = value;
			}
		}

		internal ReportSize BottomMarginForRendering
		{
			get
			{
				return m_bottomMarginForRendering;
			}
			set
			{
				m_bottomMarginForRendering = value;
			}
		}

		internal ReportSize ColumnSpacingForRendering
		{
			get
			{
				return m_columnSpacingForRendering;
			}
			set
			{
				m_columnSpacingForRendering = value;
			}
		}

		internal ParameterDefList Parameters
		{
			get
			{
				return m_parameters;
			}
			set
			{
				m_parameters = value;
			}
		}

		internal string OneDataSetName
		{
			get
			{
				return m_oneDataSetName;
			}
			set
			{
				m_oneDataSetName = value;
			}
		}

		internal StringList CodeModules
		{
			get
			{
				return m_codeModules;
			}
			set
			{
				m_codeModules = value;
			}
		}

		internal CodeClassList CodeClasses
		{
			get
			{
				return m_codeClasses;
			}
			set
			{
				m_codeClasses = value;
			}
		}

		internal bool HasSpecialRecursiveAggregates
		{
			get
			{
				return m_hasSpecialRecursiveAggregates;
			}
			set
			{
				m_hasSpecialRecursiveAggregates = value;
			}
		}

		internal ExpressionInfo Language
		{
			get
			{
				return m_language;
			}
			set
			{
				m_language = value;
			}
		}

		internal ReportExprHost ReportExprHost => m_exprHost;

		internal string DataTransform
		{
			get
			{
				return m_dataTransform;
			}
			set
			{
				m_dataTransform = value;
			}
		}

		internal string DataSchema
		{
			get
			{
				return m_dataSchema;
			}
			set
			{
				m_dataSchema = value;
			}
		}

		internal bool DataElementStyleAttribute
		{
			get
			{
				return m_dataElementStyleAttribute;
			}
			set
			{
				m_dataElementStyleAttribute = value;
			}
		}

		internal string Code
		{
			get
			{
				return m_code;
			}
			set
			{
				m_code = value;
			}
		}

		internal bool HasUserSortFilter
		{
			get
			{
				return m_hasUserSortFilter;
			}
			set
			{
				m_hasUserSortFilter = value;
			}
		}

		internal string InteractiveHeight
		{
			get
			{
				return m_interactiveHeight;
			}
			set
			{
				m_interactiveHeight = value;
			}
		}

		internal double InteractiveHeightValue
		{
			get
			{
				if (!(m_interactiveHeightValue < 0.0))
				{
					return m_interactiveHeightValue;
				}
				return m_pageHeightValue;
			}
			set
			{
				m_interactiveHeightValue = value;
			}
		}

		internal string InteractiveWidth
		{
			get
			{
				return m_interactiveWidth;
			}
			set
			{
				m_interactiveWidth = value;
			}
		}

		internal double InteractiveWidthValue
		{
			get
			{
				if (!(m_interactiveWidthValue < 0.0))
				{
					return m_interactiveWidthValue;
				}
				return m_pageWidthValue;
			}
			set
			{
				m_interactiveWidthValue = value;
			}
		}

		internal InScopeSortFilterHashtable NonDetailSortFiltersInScope
		{
			get
			{
				return m_nonDetailSortFiltersInScope;
			}
			set
			{
				m_nonDetailSortFiltersInScope = value;
			}
		}

		internal InScopeSortFilterHashtable DetailSortFiltersInScope
		{
			get
			{
				return m_detailSortFiltersInScope;
			}
			set
			{
				m_detailSortFiltersInScope = value;
			}
		}

		internal string ExprHostAssemblyName => "expression_host_" + m_reportVersion.ToString().Replace("-", "");

		internal int LastAggregateID
		{
			get
			{
				return m_lastAggregateID;
			}
			set
			{
				m_lastAggregateID = value;
			}
		}

		internal long MainChunkSize
		{
			get
			{
				return m_mainChunkSize;
			}
			set
			{
				m_mainChunkSize = value;
			}
		}

		internal Report(int id, int idForReportItems)
			: base(id, null)
		{
			m_intermediateFormatVersion = new IntermediateFormatVersion();
			m_reportVersion = Guid.NewGuid();
			m_height = "11in";
			m_width = "8.5in";
			m_dataSources = new DataSourceList();
			m_reportItems = new ReportItemCollection(idForReportItems, normal: true);
			m_pageAggregates = new DataAggregateInfoList();
			m_exprCompiledCode = new byte[0];
		}

		internal Report(ReportItem parent, IntermediateFormatVersion version, Guid reportVersion)
			: base(parent)
		{
			m_intermediateFormatVersion = version;
			m_reportVersion = reportVersion;
			m_startPage = 0;
			m_endPage = 0;
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.Location = LocationFlags.None;
			context.ObjectType = ObjectType;
			context.ObjectName = null;
			base.Initialize(context);
			if (m_language != null)
			{
				m_language.Initialize("Language", context);
				context.ExprHostBuilder.ReportLanguage(m_language);
			}
			context.ReportDataElementStyleAttribute = m_dataElementStyleAttribute;
			m_pageHeightValue = context.ValidateSize(ref m_pageHeight, "PageHeight");
			if (m_pageHeightValue <= 0.0)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidSize, Severity.Error, context.ObjectType, context.ObjectName, "PageHeight", m_pageHeightValue.ToString(CultureInfo.InvariantCulture));
			}
			m_pageWidthValue = context.ValidateSize(ref m_pageWidth, "PageWidth");
			if (m_pageWidthValue <= 0.0)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidSize, Severity.Error, context.ObjectType, context.ObjectName, "PageWidth", m_pageWidthValue.ToString(CultureInfo.InvariantCulture));
			}
			if (m_interactiveHeight != null)
			{
				m_interactiveHeightValue = context.ValidateSize(ref m_interactiveHeight, restrictMaxValue: false, "InteractiveHeight");
				if (0.0 == m_interactiveHeightValue)
				{
					m_interactiveHeightValue = double.MaxValue;
				}
				else if (m_interactiveHeightValue < 0.0)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidSize, Severity.Error, context.ObjectType, context.ObjectName, "InteractiveHeight", m_interactiveHeightValue.ToString(CultureInfo.InvariantCulture));
				}
			}
			if (m_interactiveWidth != null)
			{
				m_interactiveWidthValue = context.ValidateSize(ref m_interactiveWidth, restrictMaxValue: false, "InteractiveWidth");
				if (0.0 == m_interactiveWidthValue)
				{
					m_interactiveWidthValue = double.MaxValue;
				}
				else if (m_interactiveWidthValue < 0.0)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidSize, Severity.Error, context.ObjectType, context.ObjectName, "InteractiveWidth", m_interactiveWidthValue.ToString(CultureInfo.InvariantCulture));
				}
			}
			m_leftMarginValue = context.ValidateSize(ref m_leftMargin, "LeftMargin");
			m_rightMarginValue = context.ValidateSize(ref m_rightMargin, "RightMargin");
			m_topMarginValue = context.ValidateSize(ref m_topMargin, "TopMargin");
			m_bottomMarginValue = context.ValidateSize(ref m_bottomMargin, "BottomMargin");
			m_columnSpacingValue = context.ValidateSize(ref m_columnSpacing, "ColumnSpacing");
			if (m_dataSources != null)
			{
				for (int i = 0; i < m_dataSources.Count; i++)
				{
					Global.Tracer.Assert(m_dataSources[i] != null);
					m_dataSources[i].Initialize(context);
				}
			}
			BodyInitialize(context);
			PageHeaderFooterInitialize(context);
			if (context.ExprHostBuilder.CustomCode)
			{
				context.ExprHostBuilder.CustomCodeProxyStart();
				if (m_codeClasses != null && m_codeClasses.Count > 0)
				{
					for (int num = m_codeClasses.Count - 1; num >= 0; num--)
					{
						CodeClass codeClass = m_codeClasses[num];
						context.ExprHostBuilder.CustomCodeClassInstance(codeClass.ClassName, codeClass.InstanceName, num);
					}
				}
				if (m_code != null && m_code.Length > 0)
				{
					context.ExprHostBuilder.ReportCode(m_code);
				}
				context.ExprHostBuilder.CustomCodeProxyEnd();
			}
			return false;
		}

		internal void BodyInitialize(InitializationContext context)
		{
			context.RegisterReportItems(m_reportItems);
			m_reportItems.Initialize(context, registerRunningValues: true);
			context.ValidateUserSortInnerScope("0_ReportScope");
			context.TextboxesWithDetailSortExpressionInitialize();
			context.CalculateSortFilterGroupingLists();
			context.UnRegisterReportItems(m_reportItems);
		}

		internal void PageHeaderFooterInitialize(InitializationContext context)
		{
			context.RegisterPageSectionScope(m_pageAggregates);
			if (m_pageHeader != null)
			{
				m_pageHeader.Initialize(context);
			}
			if (m_pageFooter != null)
			{
				m_pageFooter.Initialize(context);
			}
			context.UnRegisterPageSectionScope();
		}

		DataAggregateInfoList[] IAggregateHolder.GetAggregateLists()
		{
			return new DataAggregateInfoList[1]
			{
				m_pageAggregates
			};
		}

		DataAggregateInfoList[] IAggregateHolder.GetPostSortAggregateLists()
		{
			return null;
		}

		void IAggregateHolder.ClearIfEmpty()
		{
			Global.Tracer.Assert(m_pageAggregates != null);
			if (m_pageAggregates.Count == 0)
			{
				m_pageAggregates = null;
			}
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null, "(reportExprHost != null && reportObjectModel != null)");
			m_exprHost = reportExprHost;
			ReportItemSetExprHost(m_exprHost, reportObjectModel);
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.IntermediateFormatVersion, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.IntermediateFormatVersion));
			memberInfoList.Add(new MemberInfo(MemberName.ReportVersion, Token.Guid));
			memberInfoList.Add(new MemberInfo(MemberName.Author, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.AutoRefresh, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.EmbeddedImages, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.EmbeddedImageHashtable));
			memberInfoList.Add(new MemberInfo(MemberName.PageHeader, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.PageSection));
			memberInfoList.Add(new MemberInfo(MemberName.PageFooter, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.PageSection));
			memberInfoList.Add(new MemberInfo(MemberName.ReportItems, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemCollection));
			memberInfoList.Add(new MemberInfo(MemberName.DataSources, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.DataSourceList));
			memberInfoList.Add(new MemberInfo(MemberName.PageHeight, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.PageHeightValue, Token.Double));
			memberInfoList.Add(new MemberInfo(MemberName.PageWidth, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.PageWidthValue, Token.Double));
			memberInfoList.Add(new MemberInfo(MemberName.LeftMargin, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.LeftMarginValue, Token.Double));
			memberInfoList.Add(new MemberInfo(MemberName.RightMargin, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.RightMarginValue, Token.Double));
			memberInfoList.Add(new MemberInfo(MemberName.TopMargin, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.TopMarginValue, Token.Double));
			memberInfoList.Add(new MemberInfo(MemberName.BottomMargin, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.BottomMarginValue, Token.Double));
			memberInfoList.Add(new MemberInfo(MemberName.Columns, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.ColumnSpacing, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.ColumnSpacingValue, Token.Double));
			memberInfoList.Add(new MemberInfo(MemberName.PageAggregates, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.DataAggregateInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.CompiledCode, Token.TypedArray));
			memberInfoList.Add(new MemberInfo(MemberName.MergeOnePass, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.PageMergeOnePass, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.SubReportMergeTransactions, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.NeedPostGroupProcessing, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.HasPostSortAggregates, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.HasReportItemReferences, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.ShowHideType, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.Images, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ImageStreamNames));
			memberInfoList.Add(new MemberInfo(MemberName.LastID, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.BodyID, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.SubReports, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.SubReportList));
			memberInfoList.Add(new MemberInfo(MemberName.HasImageStreams, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.HasLabels, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.HasBookmarks, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.SnapshotProcessingEnabled, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.Parameters, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ParameterDefList));
			memberInfoList.Add(new MemberInfo(MemberName.DataSetName, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.CodeModules, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.StringList));
			memberInfoList.Add(new MemberInfo(MemberName.CodeClasses, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.CodeClassList));
			memberInfoList.Add(new MemberInfo(MemberName.HasSpecialRecursiveAggregates, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.Language, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.DataTransform, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.DataSchema, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.DataElementStyleAttribute, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.Code, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.HasUserSortFilter, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.CompiledCodeGeneratedWithRefusedPermissions, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.InteractiveHeight, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.InteractiveHeightValue, Token.Double));
			memberInfoList.Add(new MemberInfo(MemberName.InteractiveWidth, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.InteractiveWidthValue, Token.Double));
			memberInfoList.Add(new MemberInfo(MemberName.NonDetailSortFiltersInScope, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.InScopeSortFilterHashtable));
			memberInfoList.Add(new MemberInfo(MemberName.DetailSortFiltersInScope, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.InScopeSortFilterHashtable));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItem, memberInfoList);
		}
	}
}
