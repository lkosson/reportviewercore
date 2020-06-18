using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportInstance : ReportItemInstance, IPageItem
	{
		private ReportItemColInstance m_reportItemColInstance;

		private string m_language;

		private int m_numberOfPages;

		[NonSerialized]
		private ReportInstanceInfo m_cachedInstanceInfo;

		[NonSerialized]
		private bool m_noRows;

		[NonSerialized]
		private int m_startPage = -1;

		[NonSerialized]
		private int m_endPage = -1;

		internal ReportItemColInstance ReportItemColInstance
		{
			get
			{
				return m_reportItemColInstance;
			}
			set
			{
				m_reportItemColInstance = value;
			}
		}

		internal string Language
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

		internal int NumberOfPages
		{
			get
			{
				return m_numberOfPages;
			}
			set
			{
				m_numberOfPages = value;
			}
		}

		internal bool NoRows => m_noRows;

		int IPageItem.StartPage
		{
			get
			{
				return m_startPage;
			}
			set
			{
				m_startPage = value;
			}
		}

		int IPageItem.EndPage
		{
			get
			{
				return m_endPage;
			}
			set
			{
				m_endPage = value;
			}
		}

		internal ReportInstance(ReportProcessing.ProcessingContext pc, Report reportItemDef, ParameterInfoCollection parameters, string reportlanguage, bool noRows)
			: base(pc.CreateUniqueName(), reportItemDef)
		{
			m_instanceInfo = new ReportInstanceInfo(pc, reportItemDef, this, parameters, noRows);
			pc.Pagination.EnterIgnoreHeight(reportItemDef.StartHidden);
			m_reportItemColInstance = new ReportItemColInstance(pc, reportItemDef.ReportItems);
			m_language = reportlanguage;
			m_noRows = noRows;
		}

		internal ReportInstance()
		{
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.ReportItemColInstance, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemColInstance));
			memberInfoList.Add(new MemberInfo(MemberName.Language, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.NumberOfPages, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemInstance, memberInfoList);
		}

		protected override object SearchChildren(int targetUniqueName, ref NonComputedUniqueNames nonCompNames, ChunkManager.RenderingChunkManager chunkManager)
		{
			return ((ISearchByUniqueName)m_reportItemColInstance).Find(targetUniqueName, ref nonCompNames, chunkManager);
		}

		internal ReportInstanceInfo GetCachedReportInstanceInfo(ChunkManager.RenderingChunkManager chunkManager)
		{
			if (m_instanceInfo is OffsetInfo)
			{
				if (m_cachedInstanceInfo == null)
				{
					IntermediateFormatReader reader = chunkManager.GetReader(((OffsetInfo)m_instanceInfo).Offset);
					m_cachedInstanceInfo = reader.ReadReportInstanceInfo((Report)m_reportItemDef);
				}
				return m_cachedInstanceInfo;
			}
			return (ReportInstanceInfo)m_instanceInfo;
		}

		internal override ReportItemInstanceInfo ReadInstanceInfo(IntermediateFormatReader reader)
		{
			Global.Tracer.Assert(m_instanceInfo is OffsetInfo);
			return reader.ReadReportInstanceInfo((Report)m_reportItemDef);
		}
	}
}
