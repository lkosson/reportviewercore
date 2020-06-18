using System.Drawing;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal interface IRenderable
	{
		void RenderStaticElements(GaugeGraphics g);

		void RenderDynamicElements(GaugeGraphics g);

		int GetZOrder();

		RectangleF GetBoundRect(GaugeGraphics g);

		object GetParentRenderable();

		string GetParentRenderableName();
	}
}
