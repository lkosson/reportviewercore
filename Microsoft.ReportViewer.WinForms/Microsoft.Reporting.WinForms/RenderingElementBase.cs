using System.Drawing;

namespace Microsoft.Reporting.WinForms
{
	internal abstract class RenderingElementBase
	{
		protected string m_accessibleName;

		protected RectangleF m_position = RectangleF.Empty;

		internal string AccessibleName => m_accessibleName;

		internal RectangleF Position => m_position;

		internal RenderingElementBase()
		{
		}

		internal void SetWidth(float width)
		{
			m_position.Width = width;
		}
	}
}
