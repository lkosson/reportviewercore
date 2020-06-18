using System.Drawing;

namespace Microsoft.Reporting.Map.WebForms
{
	internal delegate Image[,] LoadTilesHandler(Layer layer, string[,] tileUrls);
}
