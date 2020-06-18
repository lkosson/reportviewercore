using System.Drawing;

namespace Microsoft.Reporting.Map.WebForms
{
	internal delegate void SaveTilesHandler(Layer layer, string[,] tileUrls, Image[,] tileImages);
}
