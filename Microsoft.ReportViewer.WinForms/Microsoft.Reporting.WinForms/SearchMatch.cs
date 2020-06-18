using Microsoft.ReportingServices.Rendering.RichText;
using System.Drawing;

namespace Microsoft.Reporting.WinForms
{
	internal sealed class SearchMatch
	{
		private RTSelectionHighlight m_match;

		private PointF m_point = PointF.Empty;

		internal RTSelectionHighlight Match => m_match;

		internal PointF Point
		{
			get
			{
				return m_point;
			}
			set
			{
				m_point = value;
			}
		}

		internal SearchMatch(RTSelectionHighlight match)
		{
			m_match = match;
		}
	}
}
