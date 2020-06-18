using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class PageSection : ReportItem
	{
		private bool m_printOnFirstPage;

		private bool m_printOnLastPage;

		private ReportItemCollection m_reportItems;

		private bool m_postProcessEvaluate;

		[NonSerialized]
		private bool m_isHeader;

		[NonSerialized]
		private StyleExprHost m_exprHost;

		internal override ObjectType ObjectType
		{
			get
			{
				if (!m_isHeader)
				{
					return ObjectType.PageFooter;
				}
				return ObjectType.PageHeader;
			}
		}

		internal bool PrintOnFirstPage
		{
			get
			{
				return m_printOnFirstPage;
			}
			set
			{
				m_printOnFirstPage = value;
			}
		}

		internal bool PrintOnLastPage
		{
			get
			{
				return m_printOnLastPage;
			}
			set
			{
				m_printOnLastPage = value;
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

		internal bool PostProcessEvaluate
		{
			get
			{
				return m_postProcessEvaluate;
			}
			set
			{
				m_postProcessEvaluate = value;
			}
		}

		internal PageSection(bool isHeader, int id, int idForReportItems, Report report)
			: base(id, report)
		{
			m_reportItems = new ReportItemCollection(idForReportItems, normal: true);
			m_isHeader = isHeader;
		}

		internal PageSection(bool isHeader, ReportItem parent)
			: base(parent)
		{
			m_isHeader = isHeader;
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.Location |= LocationFlags.InPageSection;
			context.ObjectType = ObjectType;
			context.ObjectName = null;
			context.ExprHostBuilder.PageSectionStart();
			base.Initialize(context);
			m_reportItems.Initialize(context, registerRunningValues: true);
			base.ExprHostID = context.ExprHostBuilder.PageSectionEnd();
			return false;
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null);
				m_exprHost = reportExprHost.PageSectionHostsRemotable[base.ExprHostID];
				m_exprHost.SetReportObjectModel(reportObjectModel);
				if (m_styleClass != null)
				{
					m_styleClass.SetStyleExprHost(m_exprHost);
				}
			}
		}

		protected override void DataRendererInitialize(InitializationContext context)
		{
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.PrintOnFirstPage, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.PrintOnLastPage, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.ReportItems, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemCollection));
			memberInfoList.Add(new MemberInfo(MemberName.PostProcessEvaluate, Token.Boolean));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItem, memberInfoList);
		}
	}
}
