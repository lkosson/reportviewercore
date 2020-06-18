using System.Drawing;

namespace Microsoft.Reporting.Map.WebForms
{
	internal interface IContentElement
	{
		bool IsVisible(MapGraphics g, Layer layer, bool allLayers, RectangleF clipRect);

		void RenderShadow(MapGraphics g);

		void RenderBack(MapGraphics g, HotRegionList hotRegions);

		void RenderFront(MapGraphics g, HotRegionList hotRegions);

		void RenderText(MapGraphics g, HotRegionList hotRegions);

		RectangleF GetBoundRect(MapGraphics g);
	}
}
