using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Microsoft.ReportingServices.ReportRendering;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class List : DataRegion
	{
		private ReportHierarchyNode m_hierarchyDef;

		private ReportItemCollection m_reportItems;

		private bool m_fillPage;

		private string m_dataInstanceName;

		private DataElementOutputTypes m_dataInstanceElementOutput;

		private bool m_isListMostInner;

		[NonSerialized]
		private ListExprHost m_exprHost;

		[NonSerialized]
		private int m_ContentStartPage = -1;

		[NonSerialized]
		private int m_keepWithChildFirstPage = -1;

		internal override ObjectType ObjectType => ObjectType.List;

		internal Grouping Grouping
		{
			get
			{
				return m_hierarchyDef.Grouping;
			}
			set
			{
				m_hierarchyDef.Grouping = value;
			}
		}

		internal Sorting Sorting
		{
			get
			{
				return m_hierarchyDef.Sorting;
			}
			set
			{
				m_hierarchyDef.Sorting = value;
			}
		}

		internal ReportHierarchyNode HierarchyDef
		{
			get
			{
				return m_hierarchyDef;
			}
			set
			{
				m_hierarchyDef = value;
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

		internal bool FillPage
		{
			get
			{
				return m_fillPage;
			}
			set
			{
				m_fillPage = value;
			}
		}

		internal int ListContentID => m_hierarchyDef.ID;

		internal string DataInstanceName
		{
			get
			{
				return m_dataInstanceName;
			}
			set
			{
				m_dataInstanceName = value;
			}
		}

		internal DataElementOutputTypes DataInstanceElementOutput
		{
			get
			{
				return m_dataInstanceElementOutput;
			}
			set
			{
				m_dataInstanceElementOutput = value;
			}
		}

		internal bool IsListMostInner
		{
			get
			{
				return m_isListMostInner;
			}
			set
			{
				m_isListMostInner = value;
			}
		}

		internal bool PropagatedPageBreakAtStart
		{
			get
			{
				if (Grouping == null)
				{
					return false;
				}
				return Grouping.PageBreakAtStart;
			}
		}

		internal bool PropagatedPageBreakAtEnd
		{
			get
			{
				if (Grouping == null)
				{
					return false;
				}
				return Grouping.PageBreakAtEnd;
			}
		}

		internal ListExprHost ListExprHost => m_exprHost;

		internal int ContentStartPage
		{
			get
			{
				return m_ContentStartPage;
			}
			set
			{
				m_ContentStartPage = value;
			}
		}

		internal int KeepWithChildFirstPage
		{
			get
			{
				return m_keepWithChildFirstPage;
			}
			set
			{
				m_keepWithChildFirstPage = value;
			}
		}

		protected override DataRegionExprHost DataRegionExprHost => m_exprHost;

		internal List(ReportItem parent)
			: base(parent)
		{
		}

		internal List(int id, int idForListContent, int idForReportItems, ReportItem parent)
			: base(id, parent)
		{
			m_hierarchyDef = new ReportHierarchyNode(idForListContent, this);
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
			context.RegisterDataRegion(this);
			InternalInitialize(context);
			context.UnRegisterDataRegion(this);
			return false;
		}

		private void InternalInitialize(InitializationContext context)
		{
			context.Location = (context.Location | LocationFlags.InDataSet | LocationFlags.InDataRegion);
			context.ExprHostBuilder.ListStart(m_name);
			base.Initialize(context);
			context.Location &= ~LocationFlags.InMatrixCellTopLevelItem;
			if (Grouping != null)
			{
				context.Location |= LocationFlags.InGrouping;
			}
			else
			{
				context.Location |= LocationFlags.InDetail;
				context.DetailObjectType = ObjectType.List;
			}
			if (Grouping != null)
			{
				context.RegisterGroupingScope(Grouping.Name, Grouping.SimpleGroupExpressions, Grouping.Aggregates, Grouping.PostSortAggregates, Grouping.RecursiveAggregates, Grouping);
			}
			Global.Tracer.Assert(m_hierarchyDef != null);
			m_hierarchyDef.Initialize(context);
			context.RegisterRunningValues(m_reportItems.RunningValues);
			context.RegisterReportItems(m_reportItems);
			if (m_visibility != null)
			{
				m_visibility.Initialize(context, isContainer: true, tableRowCol: false);
			}
			m_reportItems.Initialize(context, registerRunningValues: false);
			if (m_visibility != null)
			{
				m_visibility.UnRegisterReceiver(context);
			}
			context.UnRegisterReportItems(m_reportItems);
			context.UnRegisterRunningValues(m_reportItems.RunningValues);
			if (Grouping != null)
			{
				context.UnRegisterGroupingScope(Grouping.Name);
			}
			base.ExprHostID = context.ExprHostBuilder.ListEnd();
		}

		protected override void DataRendererInitialize(InitializationContext context)
		{
			base.DataRendererInitialize(context);
			CLSNameValidator.ValidateDataElementName(ref m_dataInstanceName, "Item", context.ObjectType, context.ObjectName, "DataInstanceName", context.ErrorContext);
		}

		internal override void RegisterReceiver(InitializationContext context)
		{
			context.RegisterReportItems(m_reportItems);
			if (m_visibility != null)
			{
				m_visibility.RegisterReceiver(context, isContainer: true);
			}
			m_reportItems.RegisterReceiver(context);
			if (m_visibility != null)
			{
				m_visibility.UnRegisterReceiver(context);
			}
			context.UnRegisterReportItems(m_reportItems);
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null);
				m_exprHost = reportExprHost.ListHostsRemotable[base.ExprHostID];
				DataRegionSetExprHost(m_exprHost, reportObjectModel);
				if (m_exprHost.GroupingHost != null || m_exprHost.SortingHost != null)
				{
					Global.Tracer.Assert(m_hierarchyDef != null);
					m_hierarchyDef.ReportHierarchyNodeSetExprHost(m_exprHost.GroupingHost, m_exprHost.SortingHost, reportObjectModel);
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.HierarchyDef, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportHierarchyNode));
			memberInfoList.Add(new MemberInfo(MemberName.ReportItems, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemCollection));
			memberInfoList.Add(new MemberInfo(MemberName.FillPage, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.DataInstanceName, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.DataInstanceElementOutput, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.IsListMostInner, Token.Boolean));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.DataRegion, memberInfoList);
		}
	}
}
