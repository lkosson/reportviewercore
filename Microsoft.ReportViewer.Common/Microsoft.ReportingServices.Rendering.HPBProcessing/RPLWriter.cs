using Microsoft.ReportingServices.Rendering.RichText;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class RPLWriter
	{
		private RPLReport m_rplReport;

		private BinaryWriter m_binaryWriter;

		private RPLTablixRow m_tablixRow;

		private int m_delayedTBLevels;

		private List<Dictionary<string, List<object>>> m_currentDelayedTB;

		private byte[] m_copyBuffer;

		private bool m_cacheRichData;

		private SectionItemizedData m_sectionItemizedData;

		private Dictionary<string, List<TextRunItemizedData>> m_pageParagraphsData;

		private List<SectionItemizedData> m_glyphCache;

		internal RPLReport Report
		{
			get
			{
				return m_rplReport;
			}
			set
			{
				m_rplReport = value;
			}
		}

		internal RPLTablixRow TablixRow
		{
			get
			{
				return m_tablixRow;
			}
			set
			{
				m_tablixRow = value;
			}
		}

		internal BinaryWriter BinaryWriter
		{
			get
			{
				return m_binaryWriter;
			}
			set
			{
				m_binaryWriter = value;
			}
		}

		internal int DelayedTBLevels
		{
			get
			{
				return m_delayedTBLevels;
			}
			set
			{
				m_delayedTBLevels = value;
			}
		}

		internal byte[] CopyBuffer
		{
			get
			{
				if (m_copyBuffer == null)
				{
					m_copyBuffer = new byte[1024];
				}
				return m_copyBuffer;
			}
		}

		internal Dictionary<string, List<TextRunItemizedData>> PageParagraphsItemizedData
		{
			get
			{
				if (m_cacheRichData)
				{
					return m_pageParagraphsData;
				}
				return null;
			}
			set
			{
				m_pageParagraphsData = value;
			}
		}

		internal List<SectionItemizedData> GlyphCache => m_glyphCache;

		internal void AddTextBoxes(Dictionary<string, List<object>> delayedTextBoxes)
		{
			if (m_currentDelayedTB == null)
			{
				m_currentDelayedTB = new List<Dictionary<string, List<object>>>();
			}
			m_currentDelayedTB.Add(delayedTextBoxes);
		}

		internal void AddTextBox(string name, object value)
		{
			if (m_currentDelayedTB == null)
			{
				m_currentDelayedTB = new List<Dictionary<string, List<object>>>();
			}
			Dictionary<string, List<object>> dictionary = new Dictionary<string, List<object>>(1);
			List<object> list = new List<object>(1);
			list.Add(value);
			dictionary.Add(name, list);
			m_currentDelayedTB.Add(dictionary);
		}

		internal void EnterDelayedTBLevel(bool isLTR, ref RTLTextBoxes delayedTB)
		{
			if (!isLTR)
			{
				delayedTB = new RTLTextBoxes(m_currentDelayedTB);
				m_currentDelayedTB = null;
				m_delayedTBLevels++;
			}
		}

		internal void RegisterCellTextBoxes(bool isLTR, RTLTextBoxes delayedTB)
		{
			if (!isLTR)
			{
				delayedTB.Push(m_currentDelayedTB);
				m_currentDelayedTB = null;
			}
		}

		internal void LeaveDelayedTBLevel(bool isLTR, RTLTextBoxes delayedTB, PageContext pageContext)
		{
			if (!isLTR)
			{
				m_delayedTBLevels--;
				if (m_delayedTBLevels > 0)
				{
					m_currentDelayedTB = delayedTB.RegisterRTLLevel();
				}
				else
				{
					delayedTB.RegisterTextBoxes(pageContext);
				}
			}
		}

		internal void RegisterCacheRichData(bool cacheRichData)
		{
			m_cacheRichData = cacheRichData;
			if (m_cacheRichData && m_pageParagraphsData == null)
			{
				m_pageParagraphsData = new Dictionary<string, List<TextRunItemizedData>>();
			}
		}

		internal void RegisterSectionItemizedData()
		{
			if (m_sectionItemizedData == null)
			{
				m_sectionItemizedData = new SectionItemizedData();
			}
			if (m_pageParagraphsData != null && m_pageParagraphsData.Count == 0)
			{
				m_pageParagraphsData = null;
			}
			m_sectionItemizedData.Columns.Add(m_pageParagraphsData);
			m_pageParagraphsData = null;
		}

		internal void RegisterSectionHeaderFooter()
		{
			if (m_sectionItemizedData == null)
			{
				m_sectionItemizedData = new SectionItemizedData();
			}
			if (m_pageParagraphsData != null && m_pageParagraphsData.Count == 0)
			{
				m_pageParagraphsData = null;
			}
			m_sectionItemizedData.HeaderFooter = m_pageParagraphsData;
			m_pageParagraphsData = null;
		}

		internal void RegisterPageItemizedData()
		{
			if (m_glyphCache == null)
			{
				m_glyphCache = new List<SectionItemizedData>();
			}
			if (m_sectionItemizedData != null && m_sectionItemizedData.Columns.Count == 0 && m_sectionItemizedData.HeaderFooter == null)
			{
				m_sectionItemizedData = null;
			}
			m_glyphCache.Add(m_sectionItemizedData);
			m_sectionItemizedData = null;
		}
	}
}
