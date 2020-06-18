using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ChartHeadingInstance : InstanceInfoOwner
	{
		private int m_uniqueName;

		[Reference]
		private ChartHeading m_chartHeadingDef;

		private ChartHeadingInstanceList m_subHeadingInstances;

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

		internal ChartHeading ChartHeadingDef
		{
			get
			{
				return m_chartHeadingDef;
			}
			set
			{
				m_chartHeadingDef = value;
			}
		}

		internal ChartHeadingInstanceList SubHeadingInstances
		{
			get
			{
				return m_subHeadingInstances;
			}
			set
			{
				m_subHeadingInstances = value;
			}
		}

		internal ChartHeadingInstanceInfo InstanceInfo
		{
			get
			{
				if (m_instanceInfo is OffsetInfo)
				{
					Global.Tracer.Assert(condition: false, string.Empty);
					return null;
				}
				return (ChartHeadingInstanceInfo)m_instanceInfo;
			}
		}

		internal ChartHeadingInstance(ReportProcessing.ProcessingContext pc, int headingCellIndex, ChartHeading chartHeadingDef, int labelIndex, VariantList groupExpressionValues)
		{
			m_uniqueName = pc.CreateUniqueName();
			if (chartHeadingDef.SubHeading != null)
			{
				m_subHeadingInstances = new ChartHeadingInstanceList();
			}
			m_instanceInfo = new ChartHeadingInstanceInfo(pc, headingCellIndex, chartHeadingDef, labelIndex, groupExpressionValues);
			m_chartHeadingDef = chartHeadingDef;
		}

		internal ChartHeadingInstance()
		{
		}

		internal ChartHeadingInstanceInfo GetInstanceInfo(ChunkManager.RenderingChunkManager chunkManager)
		{
			if (m_instanceInfo is OffsetInfo)
			{
				return chunkManager.GetReader(((OffsetInfo)m_instanceInfo).Offset).ReadChartHeadingInstanceInfo();
			}
			return (ChartHeadingInstanceInfo)m_instanceInfo;
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.UniqueName, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.SubHeadingInstances, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ChartHeadingInstanceList));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.InstanceInfoOwner, memberInfoList);
		}
	}
}
