using System.Drawing;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class HitTestResult
	{
		private PointF htPoint;

		private ObjectType objectType;

		private object obj;

		private HotRegion region;

		public ObjectType ObjectType => objectType;

		public object Object => obj;

		public bool Success => obj != null;

		public string Name
		{
			get
			{
				if (obj is NamedElement)
				{
					return ((NamedElement)obj).Name;
				}
				return obj.ToString();
			}
		}

		internal HotRegion Region => region;

		internal HitTestResult(HotRegion region, PointF hitTestPoint)
		{
			this.region = region;
			if (region != null)
			{
				obj = region.SelectedObject;
			}
			htPoint = hitTestPoint;
			if (Object is Group)
			{
				objectType = ObjectType.Group;
			}
			else if (Object is Shape)
			{
				objectType = ObjectType.Shape;
			}
			else if (Object is Path)
			{
				objectType = ObjectType.Path;
			}
			else if (Object is Symbol)
			{
				objectType = ObjectType.Symbol;
			}
			else if (Object is Viewport)
			{
				objectType = ObjectType.Viewport;
			}
			else if (Object is Legend)
			{
				objectType = ObjectType.Legend;
			}
			else if (Object is LegendCell)
			{
				objectType = ObjectType.LegendCell;
			}
			else if (Object is NavigationPanel)
			{
				objectType = ObjectType.NavigationPanel;
			}
			else if (Object is ZoomPanel)
			{
				objectType = ObjectType.ZoomPanel;
			}
			else if (Object is ColorSwatchPanel)
			{
				objectType = ObjectType.ColorSwatchPanel;
			}
			else if (Object is DistanceScalePanel)
			{
				objectType = ObjectType.DistanceScalePanel;
			}
			else if (Object is MapImage)
			{
				objectType = ObjectType.MapImage;
			}
			else if (Object is MapLabel)
			{
				objectType = ObjectType.MapLabel;
			}
			else
			{
				objectType = ObjectType.Unknown;
			}
		}
	}
}
