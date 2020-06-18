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
	internal sealed class PageSection : ReportItem, IPersistable
	{
		private bool m_printOnFirstPage;

		private bool m_printOnLastPage;

		private bool m_printBetweenSections;

		private ReportItemCollection m_reportItems;

		private bool m_postProcessEvaluate;

		[NonSerialized]
		private bool m_isHeader;

		[NonSerialized]
		private StyleExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal override Microsoft.ReportingServices.ReportProcessing.ObjectType ObjectType
		{
			get
			{
				if (!m_isHeader)
				{
					return Microsoft.ReportingServices.ReportProcessing.ObjectType.PageFooter;
				}
				return Microsoft.ReportingServices.ReportProcessing.ObjectType.PageHeader;
			}
		}

		internal bool IsHeader
		{
			get
			{
				return m_isHeader;
			}
			set
			{
				m_isHeader = value;
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

		internal bool PrintBetweenSections
		{
			get
			{
				return m_printBetweenSections;
			}
			set
			{
				m_printBetweenSections = value;
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

		internal bool UpgradedSnapshotPostProcessEvaluate => m_postProcessEvaluate;

		internal PageSection(bool isHeader, int id, int idForReportItems, ReportSection reportSection)
			: base(id, reportSection)
		{
			m_reportItems = new ReportItemCollection(idForReportItems, normal: true);
			m_isHeader = isHeader;
		}

		internal PageSection(ReportItem parent)
			: base(parent)
		{
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.Location |= Microsoft.ReportingServices.ReportPublishing.LocationFlags.InPageSection;
			context.ObjectType = ObjectType;
			context.ObjectName = null;
			context.ExprHostBuilder.PageSectionStart();
			base.Initialize(context);
			m_reportItems.Initialize(context);
			base.ExprHostID = context.ExprHostBuilder.PageSectionEnd();
			return false;
		}

		protected override void DataRendererInitialize(InitializationContext context)
		{
		}

		[SkipMemberStaticValidation(MemberName.PostProcessEvaluate)]
		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.PrintOnFirstPage, Token.Boolean));
			list.Add(new MemberInfo(MemberName.PrintOnLastPage, Token.Boolean));
			list.Add(new MemberInfo(MemberName.ReportItems, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItemCollection));
			list.Add(new ReadOnlyMemberInfo(MemberName.PostProcessEvaluate, Token.Boolean));
			list.Add(new MemberInfo(MemberName.PrintBetweenSections, Token.Boolean));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PageSection, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItem, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.PrintOnFirstPage:
					writer.Write(m_printOnFirstPage);
					break;
				case MemberName.PrintOnLastPage:
					writer.Write(m_printOnLastPage);
					break;
				case MemberName.ReportItems:
					writer.Write(m_reportItems);
					break;
				case MemberName.PrintBetweenSections:
					writer.Write(m_printBetweenSections);
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
				case MemberName.PrintOnFirstPage:
					m_printOnFirstPage = reader.ReadBoolean();
					break;
				case MemberName.PrintOnLastPage:
					m_printOnLastPage = reader.ReadBoolean();
					break;
				case MemberName.ReportItems:
					m_reportItems = (ReportItemCollection)reader.ReadRIFObject();
					break;
				case MemberName.PostProcessEvaluate:
					m_postProcessEvaluate = reader.ReadBoolean();
					break;
				case MemberName.PrintBetweenSections:
					m_printBetweenSections = reader.ReadBoolean();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
			if (m_name == null)
			{
				if (IsHeader)
				{
					m_name = "PageHeader";
				}
				else
				{
					m_name = "PageFooter";
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PageSection;
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null, "(reportExprHost != null && reportObjectModel != null)");
				m_exprHost = reportExprHost.PageSectionHostsRemotable[base.ExprHostID];
				m_exprHost.SetReportObjectModel(reportObjectModel);
				if (m_styleClass != null)
				{
					m_styleClass.SetStyleExprHost(m_exprHost);
				}
			}
		}
	}
}
