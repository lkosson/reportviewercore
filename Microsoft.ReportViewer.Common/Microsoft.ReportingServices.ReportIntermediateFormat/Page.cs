using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal class Page : IDOwner, IPersistable, IStyleContainer, IRIFReportScope, IInstancePath, IAggregateHolder
	{
		private PageSection m_pageHeader;

		private PageSection m_pageFooter;

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

		private string m_interactiveHeight;

		private double m_interactiveHeightValue = -1.0;

		private string m_interactiveWidth;

		private double m_interactiveWidthValue = -1.0;

		private int m_columns = 1;

		private string m_columnSpacing = "0.5in";

		private double m_columnSpacingValue;

		private int m_exprHostID = -1;

		private byte[] m_textboxesInScope;

		private byte[] m_variablesInScope;

		private List<TextBox> m_inScopeTextBoxes;

		private List<DataAggregateInfo> m_pageAggregates;

		[NonSerialized]
		internal const int UpgradedExprHostId = 0;

		[NonSerialized]
		private ReportSize m_columnSpacingForRendering;

		[NonSerialized]
		private ReportSize m_pageWidthForRendering;

		[NonSerialized]
		private ReportSize m_pageHeightForRendering;

		private Style m_styleClass;

		[NonSerialized]
		private ReportSize m_bottomMarginForRendering;

		[NonSerialized]
		private ReportSize m_topMarginForRendering;

		[NonSerialized]
		private ReportSize m_rightMarginForRendering;

		[NonSerialized]
		private ReportSize m_leftMarginForRendering;

		[NonSerialized]
		private ReportSize m_interactiveHeightForRendering;

		[NonSerialized]
		private ReportSize m_interactiveWidthForRendering;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

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

		internal bool UpgradedSnapshotPageHeaderEvaluation
		{
			get
			{
				if (m_pageHeader == null)
				{
					return false;
				}
				return m_pageHeader.UpgradedSnapshotPostProcessEvaluate;
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

		internal bool UpgradedSnapshotPageFooterEvaluation
		{
			get
			{
				if (m_pageFooter == null)
				{
					return false;
				}
				return m_pageFooter.UpgradedSnapshotPostProcessEvaluate;
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

		IInstancePath IStyleContainer.InstancePath => this;

		public Style StyleClass
		{
			get
			{
				return m_styleClass;
			}
			set
			{
				m_styleClass = value;
			}
		}

		public Microsoft.ReportingServices.ReportProcessing.ObjectType ObjectType => Microsoft.ReportingServices.ReportProcessing.ObjectType.Page;

		public string Name => "Page";

		internal int ExprHostID
		{
			get
			{
				return m_exprHostID;
			}
			set
			{
				m_exprHostID = value;
			}
		}

		internal ReportSize InteractiveHeightForRendering
		{
			get
			{
				return m_interactiveHeightForRendering;
			}
			set
			{
				m_interactiveHeightForRendering = value;
			}
		}

		internal ReportSize InteractiveWidthForRendering
		{
			get
			{
				return m_interactiveWidthForRendering;
			}
			set
			{
				m_interactiveWidthForRendering = value;
			}
		}

		internal List<DataAggregateInfo> PageAggregates
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

		public bool NeedToCacheDataRows
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		DataScopeInfo IAggregateHolder.DataScopeInfo => null;

		internal Page()
		{
		}

		internal Page(int id)
			: base(id)
		{
			m_pageAggregates = new List<DataAggregateInfo>();
		}

		internal double GetPageSectionWidth(double width)
		{
			if (m_columns > 1)
			{
				width += (double)(m_columns - 1) * (width + m_columnSpacingValue);
			}
			return width;
		}

		internal void Initialize(InitializationContext context)
		{
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
			if (m_styleClass != null)
			{
				context.ExprHostBuilder.PageStart();
				m_styleClass.Initialize(context);
				ExprHostID = context.ExprHostBuilder.PageEnd();
			}
		}

		internal void PageHeaderFooterInitialize(InitializationContext context)
		{
			context.RegisterPageSectionScope(this, m_pageAggregates);
			if (m_pageHeader != null)
			{
				context.RegisterReportItems(m_pageHeader.ReportItems);
			}
			if (m_pageFooter != null)
			{
				context.RegisterReportItems(m_pageFooter.ReportItems);
			}
			m_textboxesInScope = context.GetCurrentReferencableTextboxesInSection();
			if (m_pageHeader != null)
			{
				m_pageHeader.Initialize(context);
			}
			if (m_pageFooter != null)
			{
				m_pageFooter.Initialize(context);
			}
			if (m_pageHeader != null)
			{
				context.UnRegisterReportItems(m_pageHeader.ReportItems);
			}
			if (m_pageFooter != null)
			{
				context.UnRegisterReportItems(m_pageFooter.ReportItems);
			}
			context.ValidateToggleItems();
			context.UnRegisterPageSectionScope();
		}

		internal void SetExprHost(ReportExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			if (m_styleClass == null || ExprHostID < 0)
			{
				return;
			}
			StyleExprHost styleExprHost = null;
			if (exprHost.PageHostsRemotable != null)
			{
				styleExprHost = exprHost.PageHostsRemotable[ExprHostID];
			}
			else if (ExprHostID == 0)
			{
				styleExprHost = exprHost.PageHost;
				if (styleExprHost == null)
				{
					return;
				}
			}
			else
			{
				Global.Tracer.Assert(false, "Missing ReportExprHost.PageHostRemotable for Page ExprHostID: {0}", ExprHostID);
			}
			styleExprHost.SetReportObjectModel(reportObjectModel);
			m_styleClass.SetStyleExprHost(styleExprHost);
		}

		public bool TextboxInScope(int sequenceIndex)
		{
			return SequenceIndex.GetBit(m_textboxesInScope, sequenceIndex, returnValueIfSequenceNull: true);
		}

		public void AddInScopeTextBox(TextBox textbox)
		{
			if (m_inScopeTextBoxes == null)
			{
				m_inScopeTextBoxes = new List<TextBox>();
			}
			m_inScopeTextBoxes.Add(textbox);
		}

		public void ResetTextBoxImpls(OnDemandProcessingContext context)
		{
			if (m_inScopeTextBoxes != null)
			{
				for (int i = 0; i < m_inScopeTextBoxes.Count; i++)
				{
					m_inScopeTextBoxes[i].ResetTextBoxImpl(context);
				}
			}
		}

		public bool VariableInScope(int sequenceIndex)
		{
			return SequenceIndex.GetBit(m_variablesInScope, sequenceIndex, returnValueIfSequenceNull: true);
		}

		public void AddInScopeEventSource(IInScopeEventSource eventSource)
		{
			Global.Tracer.Assert(condition: false, "Top level event sources should be registered on the Report, not ReportSection");
		}

		List<DataAggregateInfo> IAggregateHolder.GetAggregateList()
		{
			return m_pageAggregates;
		}

		List<DataAggregateInfo> IAggregateHolder.GetPostSortAggregateList()
		{
			return null;
		}

		void IAggregateHolder.ClearIfEmpty()
		{
			Global.Tracer.Assert(m_pageAggregates != null, "(null != m_pageAggregates)");
			if (m_pageAggregates.Count == 0)
			{
				m_pageAggregates = null;
			}
		}

		internal void SetTextboxesInScope(byte[] items)
		{
			m_textboxesInScope = items;
		}

		internal void SetInScopeTextBoxes(List<TextBox> items)
		{
			m_inScopeTextBoxes = items;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.PageHeader, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PageSection));
			list.Add(new MemberInfo(MemberName.PageFooter, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PageSection));
			list.Add(new MemberInfo(MemberName.PageHeight, Token.String));
			list.Add(new MemberInfo(MemberName.PageHeightValue, Token.Double));
			list.Add(new MemberInfo(MemberName.PageWidth, Token.String));
			list.Add(new MemberInfo(MemberName.PageWidthValue, Token.Double));
			list.Add(new MemberInfo(MemberName.LeftMargin, Token.String));
			list.Add(new MemberInfo(MemberName.LeftMarginValue, Token.Double));
			list.Add(new MemberInfo(MemberName.RightMargin, Token.String));
			list.Add(new MemberInfo(MemberName.RightMarginValue, Token.Double));
			list.Add(new MemberInfo(MemberName.TopMargin, Token.String));
			list.Add(new MemberInfo(MemberName.TopMarginValue, Token.Double));
			list.Add(new MemberInfo(MemberName.BottomMargin, Token.String));
			list.Add(new MemberInfo(MemberName.BottomMarginValue, Token.Double));
			list.Add(new MemberInfo(MemberName.InteractiveHeight, Token.String));
			list.Add(new MemberInfo(MemberName.InteractiveHeightValue, Token.Double));
			list.Add(new MemberInfo(MemberName.InteractiveWidth, Token.String));
			list.Add(new MemberInfo(MemberName.InteractiveWidthValue, Token.Double));
			list.Add(new MemberInfo(MemberName.Columns, Token.Int32));
			list.Add(new MemberInfo(MemberName.ColumnSpacing, Token.String));
			list.Add(new MemberInfo(MemberName.ColumnSpacingValue, Token.Double));
			list.Add(new MemberInfo(MemberName.StyleClass, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Style));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.InScopeTextBoxes, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Token.Reference, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TextBox));
			list.Add(new MemberInfo(MemberName.TextboxesInScope, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Byte));
			list.Add(new MemberInfo(MemberName.VariablesInScope, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Byte));
			list.Add(new MemberInfo(MemberName.PageAggregates, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Page, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.PageHeader:
					writer.Write(m_pageHeader);
					break;
				case MemberName.PageFooter:
					writer.Write(m_pageFooter);
					break;
				case MemberName.PageHeight:
					writer.Write(m_pageHeight);
					break;
				case MemberName.PageHeightValue:
					writer.Write(m_pageHeightValue);
					break;
				case MemberName.PageWidth:
					writer.Write(m_pageWidth);
					break;
				case MemberName.PageWidthValue:
					writer.Write(m_pageWidthValue);
					break;
				case MemberName.LeftMargin:
					writer.Write(m_leftMargin);
					break;
				case MemberName.LeftMarginValue:
					writer.Write(m_leftMarginValue);
					break;
				case MemberName.RightMargin:
					writer.Write(m_rightMargin);
					break;
				case MemberName.RightMarginValue:
					writer.Write(m_rightMarginValue);
					break;
				case MemberName.TopMargin:
					writer.Write(m_topMargin);
					break;
				case MemberName.TopMarginValue:
					writer.Write(m_topMarginValue);
					break;
				case MemberName.BottomMargin:
					writer.Write(m_bottomMargin);
					break;
				case MemberName.BottomMarginValue:
					writer.Write(m_bottomMarginValue);
					break;
				case MemberName.InteractiveHeight:
					writer.Write(m_interactiveHeight);
					break;
				case MemberName.InteractiveHeightValue:
					writer.Write(m_interactiveHeightValue);
					break;
				case MemberName.InteractiveWidth:
					writer.Write(m_interactiveWidth);
					break;
				case MemberName.InteractiveWidthValue:
					writer.Write(m_interactiveWidthValue);
					break;
				case MemberName.Columns:
					writer.Write(m_columns);
					break;
				case MemberName.ColumnSpacing:
					writer.Write(m_columnSpacing);
					break;
				case MemberName.ColumnSpacingValue:
					writer.Write(m_columnSpacingValue);
					break;
				case MemberName.StyleClass:
					writer.Write(m_styleClass);
					break;
				case MemberName.ExprHostID:
					writer.Write(m_exprHostID);
					break;
				case MemberName.TextboxesInScope:
					writer.Write(m_textboxesInScope);
					break;
				case MemberName.VariablesInScope:
					writer.Write(m_variablesInScope);
					break;
				case MemberName.InScopeTextBoxes:
					writer.WriteListOfReferences(m_inScopeTextBoxes);
					break;
				case MemberName.PageAggregates:
					writer.Write(m_pageAggregates);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.PageHeader:
					m_pageHeader = (PageSection)reader.ReadRIFObject();
					break;
				case MemberName.PageFooter:
					m_pageFooter = (PageSection)reader.ReadRIFObject();
					break;
				case MemberName.PageHeight:
					m_pageHeight = reader.ReadString();
					break;
				case MemberName.PageHeightValue:
					m_pageHeightValue = reader.ReadDouble();
					break;
				case MemberName.PageWidth:
					m_pageWidth = reader.ReadString();
					break;
				case MemberName.PageWidthValue:
					m_pageWidthValue = reader.ReadDouble();
					break;
				case MemberName.LeftMargin:
					m_leftMargin = reader.ReadString();
					break;
				case MemberName.LeftMarginValue:
					m_leftMarginValue = reader.ReadDouble();
					break;
				case MemberName.RightMargin:
					m_rightMargin = reader.ReadString();
					break;
				case MemberName.RightMarginValue:
					m_rightMarginValue = reader.ReadDouble();
					break;
				case MemberName.TopMargin:
					m_topMargin = reader.ReadString();
					break;
				case MemberName.TopMarginValue:
					m_topMarginValue = reader.ReadDouble();
					break;
				case MemberName.BottomMargin:
					m_bottomMargin = reader.ReadString();
					break;
				case MemberName.BottomMarginValue:
					m_bottomMarginValue = reader.ReadDouble();
					break;
				case MemberName.InteractiveHeight:
					m_interactiveHeight = reader.ReadString();
					break;
				case MemberName.InteractiveHeightValue:
					m_interactiveHeightValue = reader.ReadDouble();
					break;
				case MemberName.InteractiveWidth:
					m_interactiveWidth = reader.ReadString();
					break;
				case MemberName.InteractiveWidthValue:
					m_interactiveWidthValue = reader.ReadDouble();
					break;
				case MemberName.Columns:
					m_columns = reader.ReadInt32();
					break;
				case MemberName.ColumnSpacing:
					m_columnSpacing = reader.ReadString();
					break;
				case MemberName.ColumnSpacingValue:
					m_columnSpacingValue = reader.ReadDouble();
					break;
				case MemberName.StyleClass:
					m_styleClass = (Style)reader.ReadRIFObject();
					break;
				case MemberName.ExprHostID:
					m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.TextboxesInScope:
					m_textboxesInScope = reader.ReadByteArray();
					break;
				case MemberName.VariablesInScope:
					m_variablesInScope = reader.ReadByteArray();
					break;
				case MemberName.InScopeTextBoxes:
					m_inScopeTextBoxes = reader.ReadGenericListOfReferences<TextBox>(this);
					break;
				case MemberName.PageAggregates:
					m_pageAggregates = reader.ReadGenericListOfRIFObjects<DataAggregateInfo>();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			if (!memberReferencesCollection.TryGetValue(m_Declaration.ObjectType, out List<MemberReference> value))
			{
				return;
			}
			foreach (MemberReference item2 in value)
			{
				MemberName memberName = item2.MemberName;
				if (memberName == MemberName.InScopeTextBoxes)
				{
					if (m_inScopeTextBoxes == null)
					{
						m_inScopeTextBoxes = new List<TextBox>();
					}
					referenceableItems.TryGetValue(item2.RefID, out IReferenceable value2);
					TextBox item = (TextBox)value2;
					m_inScopeTextBoxes.Add(item);
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Page;
		}
	}
}
