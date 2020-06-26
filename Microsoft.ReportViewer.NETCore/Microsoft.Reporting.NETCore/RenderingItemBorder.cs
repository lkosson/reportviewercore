using Microsoft.ReportingServices.Rendering.ImageRenderer;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System.Collections.Generic;
using System.Drawing;

namespace Microsoft.Reporting.NETCore
{
	internal struct RenderingItemBorder
	{
		internal RPLFormat.BorderStyles Style;

		internal float Width;

		internal Color Color;

		internal float Point;

		internal float Edge;

		internal List<Operation> Operations;

		internal bool IsVisible
		{
			get
			{
				if (Style != 0 && Width > 0f && Color != Color.Empty)
				{
					return true;
				}
				return false;
			}
		}
	}
}
