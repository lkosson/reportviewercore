using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLPageContent
	{
		private RPLSizes[] m_reportSectionSizes;

		private RPLPageLayout m_pageLayout;

		private long[] m_sectionOffsets;

		private int m_sectionCount;

		private Queue<RPLReportSection> m_sections;

		private long m_endOffset = -1L;

		private RPLContext m_context;

		private float m_maxSectionWidth = -1f;

		public RPLSizes[] ReportSectionSizes
		{
			get
			{
				return m_reportSectionSizes;
			}
			set
			{
				m_reportSectionSizes = value;
			}
		}

		public RPLPageLayout PageLayout
		{
			get
			{
				return m_pageLayout;
			}
			set
			{
				m_pageLayout = value;
			}
		}

		public float MaxSectionWidth
		{
			get
			{
				if (m_maxSectionWidth < 0f)
				{
					for (int i = 0; i < m_reportSectionSizes.Length; i++)
					{
						m_maxSectionWidth = Math.Max(m_maxSectionWidth, m_reportSectionSizes[i].Width);
					}
				}
				return m_maxSectionWidth;
			}
		}

		internal long[] SectionOffsets
		{
			set
			{
				m_sectionOffsets = value;
			}
		}

		internal int SectionCount
		{
			set
			{
				m_sectionCount = value;
			}
		}

		internal RPLPageContent(long endOffset, RPLContext context, Version rplVersion)
		{
			m_endOffset = endOffset;
			m_context = context;
			switch (m_context.VersionPicker)
			{
			case RPLVersionEnum.RPL2008:
			case RPLVersionEnum.RPL2008WithImageConsolidation:
				RPLReader.ReadPageContent2008(this, endOffset, context);
				break;
			case RPLVersionEnum.RPLAccess:
			case RPLVersionEnum.RPLMap:
			case RPLVersionEnum.RPL2009:
				RPLReader.ReadPageContent(this, endOffset, context);
				break;
			default:
				throw new ArgumentException(RPLRes.UnsupportedRPLVersion(rplVersion.ToString(3), "10.6"));
			}
		}

		internal RPLPageContent(int sectionCount, RPLPageLayout pageLayout)
		{
			m_reportSectionSizes = new RPLSizes[sectionCount];
			m_pageLayout = pageLayout;
		}

		internal RPLPageContent(int sectionCount)
		{
			m_reportSectionSizes = new RPLSizes[sectionCount];
		}

		public bool HasNextReportSection()
		{
			if (m_sections == null && m_sectionCount == 0)
			{
				return false;
			}
			return true;
		}

		public RPLReportSection GetNextReportSection()
		{
			if (m_context != null && m_context.VersionPicker == RPLVersionEnum.RPL2008)
			{
				if (m_sections != null)
				{
					m_sectionCount--;
					RPLReportSection result = m_sections.Dequeue();
					m_sections = null;
					return result;
				}
				return null;
			}
			if (m_sections != null)
			{
				m_sectionCount--;
				RPLReportSection result2 = m_sections.Dequeue();
				if (m_sections.Count == 0)
				{
					m_sections = null;
				}
				return result2;
			}
			if (m_sectionCount == 0)
			{
				return null;
			}
			m_sectionCount--;
			return RPLReader.ReadReportSection(m_sectionOffsets[m_sectionOffsets.Length - m_sectionCount - 1], m_context);
		}

		internal void AddReportSection(RPLReportSection section)
		{
			if (m_sections == null)
			{
				m_sections = new Queue<RPLReportSection>();
			}
			m_sections.Enqueue(section);
			m_sectionCount++;
		}
	}
}
