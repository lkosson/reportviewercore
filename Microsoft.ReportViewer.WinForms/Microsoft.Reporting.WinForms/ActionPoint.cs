using System.Drawing;

namespace Microsoft.Reporting.WinForms
{
	internal class ActionPoint
	{
		private PointF m_point;

		private Point m_pointInPixels;

		internal Point Point => m_pointInPixels;

		internal ActionPoint(PointF point)
		{
			m_point = point;
		}

		internal void SetDpi(float dpiX, float dpiY)
		{
			m_pointInPixels.X = Global.ToPixels(m_point.X, dpiX);
			m_pointInPixels.Y = Global.ToPixels(m_point.Y, dpiY);
			m_point = PointF.Empty;
		}
	}
}
