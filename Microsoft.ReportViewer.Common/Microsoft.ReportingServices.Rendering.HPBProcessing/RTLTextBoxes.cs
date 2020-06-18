using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.HPBProcessing
{
	internal class RTLTextBoxes
	{
		private List<Dictionary<string, List<object>>> m_delayedTB;

		private Stack<Dictionary<string, List<object>>> m_rtlDelayedTB;

		internal List<Dictionary<string, List<object>>> DelayedTB
		{
			get
			{
				return m_delayedTB;
			}
			set
			{
				m_delayedTB = value;
			}
		}

		internal Stack<Dictionary<string, List<object>>> RTLDelayedTB
		{
			get
			{
				return m_rtlDelayedTB;
			}
			set
			{
				m_rtlDelayedTB = value;
			}
		}

		internal RTLTextBoxes(List<Dictionary<string, List<object>>> delayedTB)
		{
			m_delayedTB = delayedTB;
		}

		internal void Push(List<Dictionary<string, List<object>>> delayedTB)
		{
			if (delayedTB != null && delayedTB.Count != 0)
			{
				if (m_rtlDelayedTB == null)
				{
					m_rtlDelayedTB = new Stack<Dictionary<string, List<object>>>();
				}
				for (int i = 0; i < delayedTB.Count; i++)
				{
					m_rtlDelayedTB.Push(delayedTB[i]);
				}
			}
		}

		internal List<Dictionary<string, List<object>>> RegisterRTLLevel()
		{
			if (m_rtlDelayedTB != null && m_rtlDelayedTB.Count > 0)
			{
				if (m_delayedTB == null)
				{
					m_delayedTB = new List<Dictionary<string, List<object>>>();
				}
				while (m_rtlDelayedTB.Count > 0)
				{
					m_delayedTB.Add(m_rtlDelayedTB.Pop());
				}
			}
			return m_delayedTB;
		}

		internal void RegisterTextBoxes(PageContext pageContext)
		{
			if (m_delayedTB != null)
			{
				for (int i = 0; i < m_delayedTB.Count; i++)
				{
					RegisterTextBoxes(m_delayedTB[i], pageContext);
				}
				m_delayedTB = null;
			}
			if (m_rtlDelayedTB != null && m_rtlDelayedTB.Count > 0)
			{
				while (m_rtlDelayedTB.Count > 0)
				{
					RegisterTextBoxes(m_rtlDelayedTB.Pop(), pageContext);
				}
				m_rtlDelayedTB = null;
			}
		}

		private void RegisterTextBoxes(Dictionary<string, List<object>> textBoxValues, PageContext pageContext)
		{
			if (textBoxValues == null || pageContext.Common.InSubReport)
			{
				return;
			}
			foreach (string key in textBoxValues.Keys)
			{
				foreach (object item in textBoxValues[key])
				{
					pageContext.AddTextBox(key, item);
				}
			}
		}
	}
}
