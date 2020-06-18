using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class Rectangle : ReportItem, IPageBreakOwner, IPersistable
	{
		private ReportItemCollection m_reportItems;

		private PageBreak m_pageBreak;

		private ExpressionInfo m_pageName;

		private int m_linkToChild = -1;

		private bool m_keepTogether;

		private bool m_omitBorderOnPageBreak;

		private bool m_isSimple;

		[NonSerialized]
		private ReportItemExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal override Microsoft.ReportingServices.ReportProcessing.ObjectType ObjectType => Microsoft.ReportingServices.ReportProcessing.ObjectType.Rectangle;

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

		internal int LinkToChild
		{
			get
			{
				return m_linkToChild;
			}
			set
			{
				m_linkToChild = value;
			}
		}

		internal bool KeepTogether
		{
			get
			{
				return m_keepTogether;
			}
			set
			{
				m_keepTogether = value;
			}
		}

		internal bool OmitBorderOnPageBreak
		{
			get
			{
				return m_omitBorderOnPageBreak;
			}
			set
			{
				m_omitBorderOnPageBreak = value;
			}
		}

		internal bool IsSimple
		{
			get
			{
				return m_isSimple;
			}
			set
			{
				m_isSimple = value;
			}
		}

		internal override DataElementOutputTypes DataElementOutputDefault => DataElementOutputTypes.ContentsOnly;

		internal ExpressionInfo PageName
		{
			get
			{
				return m_pageName;
			}
			set
			{
				m_pageName = value;
			}
		}

		internal PageBreak PageBreak
		{
			get
			{
				return m_pageBreak;
			}
			set
			{
				m_pageBreak = value;
			}
		}

		PageBreak IPageBreakOwner.PageBreak
		{
			get
			{
				return m_pageBreak;
			}
			set
			{
				m_pageBreak = value;
			}
		}

		Microsoft.ReportingServices.ReportProcessing.ObjectType IPageBreakOwner.ObjectType => Microsoft.ReportingServices.ReportProcessing.ObjectType.Rectangle;

		string IPageBreakOwner.ObjectName => m_name;

		IInstancePath IPageBreakOwner.InstancePath => this;

		internal Rectangle(ReportItem parent)
			: base(parent)
		{
		}

		internal Rectangle(int id, int idForReportItems, ReportItem parent)
			: base(id, parent)
		{
			m_reportItems = new ReportItemCollection(idForReportItems, normal: true);
		}

		internal override void CalculateSizes(double width, double height, InitializationContext context, bool overwrite)
		{
			base.CalculateSizes(width, height, context, overwrite);
			m_reportItems.CalculateSizes(context, overwrite: false);
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.ObjectType = ObjectType;
			context.ObjectName = m_name;
			context.ExprHostBuilder.RectangleStart(m_name);
			m_isSimple = (m_toolTip == null && m_documentMapLabel == null && m_bookmark == null && m_styleClass == null && m_visibility == null);
			base.Initialize(context);
			context.InitializeAbsolutePosition(this);
			if (m_pageBreak != null)
			{
				m_pageBreak.Initialize(context);
			}
			if (m_pageName != null)
			{
				m_pageName.Initialize("PageName", context);
				context.ExprHostBuilder.PageName(m_pageName);
			}
			if (m_visibility != null)
			{
				m_visibility.Initialize(context);
			}
			bool num = context.RegisterVisibility(m_visibility, this);
			context.IsTopLevelCellContents = false;
			m_reportItems.Initialize(context);
			if (num)
			{
				context.UnRegisterVisibility(m_visibility, this);
			}
			base.ExprHostID = context.ExprHostBuilder.RectangleEnd();
			return false;
		}

		internal override void TraverseScopes(IRIFScopeVisitor visitor)
		{
			if (m_reportItems == null)
			{
				return;
			}
			foreach (ReportItem reportItem in m_reportItems)
			{
				reportItem.TraverseScopes(visitor);
			}
		}

		internal override void InitializeRVDirectionDependentItems(InitializationContext context)
		{
			if (m_reportItems != null)
			{
				m_reportItems.InitializeRVDirectionDependentItems(context);
			}
		}

		internal override void DetermineGroupingExprValueCount(InitializationContext context, int groupingExprCount)
		{
			if (m_reportItems != null)
			{
				m_reportItems.DetermineGroupingExprValueCount(context, groupingExprCount);
			}
		}

		internal bool ContainsDataRegionOrSubReport()
		{
			for (int i = 0; i < m_reportItems.Count; i++)
			{
				if (m_reportItems[i].IsOrContainsDataRegionOrSubReport())
				{
					return true;
				}
			}
			return false;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ReportItems, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItemCollection));
			list.Add(new ReadOnlyMemberInfo(MemberName.PageBreakLocation, Token.Enum));
			list.Add(new MemberInfo(MemberName.LinkToChild, Token.Int32));
			list.Add(new MemberInfo(MemberName.OmitBorderOnPageBreak, Token.Boolean));
			list.Add(new MemberInfo(MemberName.KeepTogether, Token.Boolean));
			list.Add(new MemberInfo(MemberName.IsSimple, Token.Boolean));
			list.Add(new MemberInfo(MemberName.PageBreak, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PageBreak));
			list.Add(new MemberInfo(MemberName.PageName, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Rectangle, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItem, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ReportItems:
					writer.Write(m_reportItems);
					break;
				case MemberName.LinkToChild:
					writer.Write(m_linkToChild);
					break;
				case MemberName.OmitBorderOnPageBreak:
					writer.Write(m_omitBorderOnPageBreak);
					break;
				case MemberName.KeepTogether:
					writer.Write(m_keepTogether);
					break;
				case MemberName.IsSimple:
					writer.Write(m_isSimple);
					break;
				case MemberName.PageBreak:
					writer.Write(m_pageBreak);
					break;
				case MemberName.PageName:
					writer.Write(m_pageName);
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
				case MemberName.ReportItems:
					m_reportItems = (ReportItemCollection)reader.ReadRIFObject();
					break;
				case MemberName.PageBreakLocation:
					m_pageBreak = new PageBreak();
					m_pageBreak.BreakLocation = (PageBreakLocation)reader.ReadEnum();
					break;
				case MemberName.LinkToChild:
					m_linkToChild = reader.ReadInt32();
					break;
				case MemberName.OmitBorderOnPageBreak:
					m_omitBorderOnPageBreak = reader.ReadBoolean();
					break;
				case MemberName.KeepTogether:
					m_keepTogether = reader.ReadBoolean();
					break;
				case MemberName.IsSimple:
					m_isSimple = reader.ReadBoolean();
					break;
				case MemberName.PageBreak:
					m_pageBreak = (PageBreak)reader.ReadRIFObject();
					break;
				case MemberName.PageName:
					m_pageName = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Rectangle;
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			Rectangle rectangle = (Rectangle)base.PublishClone(context);
			if (m_reportItems != null)
			{
				rectangle.m_reportItems = (ReportItemCollection)m_reportItems.PublishClone(context);
			}
			if (m_pageBreak != null)
			{
				rectangle.m_pageBreak = (PageBreak)m_pageBreak.PublishClone(context);
			}
			if (m_pageName != null)
			{
				rectangle.m_pageName = (ExpressionInfo)m_pageName.PublishClone(context);
			}
			return rectangle;
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null, "(reportExprHost != null && reportObjectModel != null)");
				m_exprHost = reportExprHost.RectangleHostsRemotable[base.ExprHostID];
				ReportItemSetExprHost(m_exprHost, reportObjectModel);
				if (m_pageBreak != null && m_exprHost.PageBreakExprHost != null)
				{
					m_pageBreak.SetExprHost(m_exprHost.PageBreakExprHost, reportObjectModel);
				}
			}
		}

		internal string EvaluatePageName(IReportScopeInstance romInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, romInstance);
			return context.ReportRuntime.EvaluateRectanglePageNameExpression(this, m_pageName, m_name);
		}
	}
}
