using System.Drawing;

namespace Microsoft.Reporting.Map.WebForms
{
	internal interface ISelectable
	{
		void DrawSelection(MapGraphics g, RectangleF clipRect, bool designTimeSelection);

		bool IsSelected();

		bool IsVisible();

		RectangleF GetSelectionRectangle(MapGraphics g, RectangleF clipRect);
	}
}
