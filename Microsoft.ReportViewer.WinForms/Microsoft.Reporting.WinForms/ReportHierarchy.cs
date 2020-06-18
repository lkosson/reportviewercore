using System;
using System.Collections.Generic;

namespace Microsoft.Reporting.WinForms
{
	internal sealed class ReportHierarchy : IDisposable
	{
		private Stack<ReportInfo> m_stack = new Stack<ReportInfo>();

		private bool m_keepSessionAlive = true;

		public int Count => m_stack.Count;

		public bool KeepSessionAlive
		{
			get
			{
				return m_keepSessionAlive;
			}
			set
			{
				if (m_keepSessionAlive == value)
				{
					return;
				}
				m_keepSessionAlive = value;
				if (!m_keepSessionAlive)
				{
					foreach (ReportInfo item in m_stack)
					{
						item.StopTimer();
					}
					return;
				}
				ReportInfo reportInfo = m_stack.Peek();
				if (m_stack.Count > 1 || reportInfo.ServerReport.HasExecutionId)
				{
					throw new InvalidOperationException(CommonStrings.KeepSessionAliveException);
				}
				reportInfo.StartTimer();
			}
		}

		public void Dispose()
		{
			Clear();
		}

		public void Push(ReportInfo reportInfo)
		{
			m_stack.Push(reportInfo);
			if (KeepSessionAlive)
			{
				reportInfo.StartTimer();
			}
		}

		public void Pop()
		{
			m_stack.Pop().Dispose();
		}

		public ReportInfo Peek()
		{
			return m_stack.Peek();
		}

		public void Clear()
		{
			while (m_stack.Count > 0)
			{
				Pop();
			}
		}

		public ReportInfo[] ToArray()
		{
			return m_stack.ToArray();
		}
	}
}
