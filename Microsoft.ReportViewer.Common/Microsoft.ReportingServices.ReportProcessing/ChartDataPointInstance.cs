using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ChartDataPointInstance : InstanceInfoOwner
	{
		private int m_uniqueName;

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

		internal ChartDataPointInstanceInfo InstanceInfo
		{
			get
			{
				if (m_instanceInfo is OffsetInfo)
				{
					Global.Tracer.Assert(condition: false, string.Empty);
					return null;
				}
				return (ChartDataPointInstanceInfo)m_instanceInfo;
			}
		}

		internal ChartDataPointInstance(ReportProcessing.ProcessingContext pc, Chart chart, ChartDataPoint dataPointDef, int dataPointIndex)
		{
			m_uniqueName = pc.CreateUniqueName();
			m_instanceInfo = new ChartDataPointInstanceInfo(pc, chart, dataPointDef, dataPointIndex, this);
		}

		internal ChartDataPointInstance()
		{
		}

		internal ChartDataPointInstanceInfo GetInstanceInfo(ChunkManager.RenderingChunkManager chunkManager, ChartDataPointList chartDataPoints)
		{
			if (m_instanceInfo is OffsetInfo)
			{
				Global.Tracer.Assert(chunkManager != null);
				return chunkManager.GetReader(((OffsetInfo)m_instanceInfo).Offset).ReadChartDataPointInstanceInfo(chartDataPoints);
			}
			return (ChartDataPointInstanceInfo)m_instanceInfo;
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.UniqueName, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.InstanceInfoOwner, memberInfoList);
		}
	}
}
