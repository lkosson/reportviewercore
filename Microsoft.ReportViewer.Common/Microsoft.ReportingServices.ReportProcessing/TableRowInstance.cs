using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class TableRowInstance : InstanceInfoOwner, IShowHideContainer, ISearchByUniqueName
	{
		private int m_uniqueName;

		private ReportItemColInstance m_tableRowReportItemColInstance;

		[NonSerialized]
		[Reference]
		private TableRow m_tableRowDef;

		internal int UniqueName
		{
			get
			{
				return m_uniqueName;
			}
			set
			{
				m_uniqueName = value;
			}
		}

		internal ReportItemColInstance TableRowReportItemColInstance
		{
			get
			{
				return m_tableRowReportItemColInstance;
			}
			set
			{
				m_tableRowReportItemColInstance = value;
			}
		}

		internal TableRow TableRowDef
		{
			get
			{
				return m_tableRowDef;
			}
			set
			{
				m_tableRowDef = value;
			}
		}

		internal TableRowInstance(ReportProcessing.ProcessingContext pc, TableRow rowDef, Table tableDef, IndexedExprHost visibilityHiddenExprHost)
		{
			m_uniqueName = pc.CreateUniqueName();
			m_instanceInfo = new TableRowInstanceInfo(pc, rowDef, this, tableDef, visibilityHiddenExprHost);
			m_tableRowDef = rowDef;
			m_tableRowReportItemColInstance = new ReportItemColInstance(pc, rowDef.ReportItems);
		}

		internal TableRowInstance()
		{
		}

		object ISearchByUniqueName.Find(int targetUniqueName, ref NonComputedUniqueNames nonCompNames, ChunkManager.RenderingChunkManager chunkManager)
		{
			return ((ISearchByUniqueName)m_tableRowReportItemColInstance).Find(targetUniqueName, ref nonCompNames, chunkManager);
		}

		void IShowHideContainer.BeginProcessContainer(ReportProcessing.ProcessingContext context)
		{
			context.BeginProcessContainer(m_uniqueName, m_tableRowDef.Visibility);
		}

		void IShowHideContainer.EndProcessContainer(ReportProcessing.ProcessingContext context)
		{
			context.EndProcessContainer(m_uniqueName, m_tableRowDef.Visibility);
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.UniqueName, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.TableRowReportItemColInstance, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemColInstance));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.InstanceInfoOwner, memberInfoList);
		}

		internal TableRowInstanceInfo GetInstanceInfo(ChunkManager.RenderingChunkManager chunkManager)
		{
			if (m_instanceInfo is OffsetInfo)
			{
				Global.Tracer.Assert(chunkManager != null);
				return chunkManager.GetReader(((OffsetInfo)m_instanceInfo).Offset).ReadTableRowInstanceInfo();
			}
			return (TableRowInstanceInfo)m_instanceInfo;
		}
	}
}
