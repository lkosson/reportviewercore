using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class HotRegionList : MapObject
	{
		private ArrayList list = new ArrayList();

		private Dictionary<object, int> selectedObjectIndex = new Dictionary<object, int>();

		private float scaleFactorX = 1f;

		private float scaleFactorY = 1f;

		internal ArrayList List => list;

		internal float ScaleFactorX
		{
			get
			{
				return scaleFactorX;
			}
			set
			{
				scaleFactorX = value;
			}
		}

		internal float ScaleFactorY
		{
			get
			{
				return scaleFactorY;
			}
			set
			{
				scaleFactorY = value;
			}
		}

		public HotRegionList(object parent)
			: base(parent)
		{
		}

		internal int FindHotRegionOfObject(object obj)
		{
			if (selectedObjectIndex.TryGetValue(obj, out int value))
			{
				return value;
			}
			return -1;
		}

		internal void RemoveHotRegionOfObject(object obj)
		{
			int num = 0;
			HotRegion hotRegion;
			while (true)
			{
				if (num < list.Count)
				{
					hotRegion = (HotRegion)list[num];
					if (hotRegion.SelectedObject == obj)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			list.RemoveAt(num);
			selectedObjectIndex.Remove(hotRegion.SelectedObject);
			hotRegion.Dispose();
		}

		internal void SetHotRegion(MapGraphics g, object selectedObject, params GraphicsPath[] pathArray)
		{
			SetHotRegion(g, selectedObject, PointF.Empty, pathArray);
		}

		internal void SetHotRegion(MapGraphics g, object selectedObject, PointF pinPoint, params GraphicsPath[] pathArray)
		{
			GraphicsPath[] array = new GraphicsPath[pathArray.Length];
			for (int i = 0; i < pathArray.Length; i++)
			{
				if (pathArray[i] != null)
				{
					array[i] = (GraphicsPath)pathArray[i].Clone();
				}
			}
			HotRegion hotRegion;
			if (!selectedObjectIndex.ContainsKey(selectedObject))
			{
				hotRegion = new HotRegion();
				int value = list.Add(hotRegion);
				selectedObjectIndex[selectedObject] = value;
			}
			else
			{
				int index = selectedObjectIndex[selectedObject];
				hotRegion = (HotRegion)list[index];
			}
			hotRegion.SelectedObject = selectedObject;
			Matrix transform = g.Transform;
			if (transform != null)
			{
				for (int j = 0; j < array.Length; j++)
				{
					if (array[j] != null)
					{
						try
						{
							array[j].Transform(transform);
						}
						catch
						{
							return;
						}
					}
				}
			}
			hotRegion.Paths = array;
			if (!pinPoint.IsEmpty)
			{
				pinPoint.X += transform.OffsetX;
				pinPoint.Y += transform.OffsetY;
			}
			hotRegion.PinPoint = pinPoint;
			hotRegion.BuildMatrices(g);
		}

		internal HotRegion[] CheckHotRegions(int x, int y, Type[] objectTypes, bool needTooltipOnly)
		{
			ArrayList arrayList = new ArrayList();
			for (int num = list.Count - 1; num >= 0; num--)
			{
				HotRegion hotRegion = (HotRegion)list[num];
				if (!IsOfType(objectTypes, hotRegion.SelectedObject) || (needTooltipOnly && (!(hotRegion.SelectedObject is IToolTipProvider) || ((IToolTipProvider)hotRegion.SelectedObject).GetToolTip() == string.Empty)))
				{
					continue;
				}
				Shape shape = hotRegion.SelectedObject as Shape;
				Path path = hotRegion.SelectedObject as Path;
				Symbol symbol = hotRegion.SelectedObject as Symbol;
				GridAttributes gridAttributes = hotRegion.SelectedObject as GridAttributes;
				if (shape != null || path != null || symbol != null || gridAttributes != null)
				{
					RectangleF rectangleF = new RectangleF(Common.MapCore.Viewport.GetAbsoluteLocation(), Common.MapCore.Viewport.GetAbsoluteSize());
					rectangleF.X *= ScaleFactorX;
					rectangleF.Y *= ScaleFactorY;
					rectangleF.Width *= ScaleFactorX;
					rectangleF.Height *= ScaleFactorY;
					if (!rectangleF.Contains(x, y))
					{
						continue;
					}
				}
				GraphicsPath[] paths = ((HotRegion)list[num]).Paths;
				foreach (GraphicsPath graphicsPath in paths)
				{
					if (graphicsPath == null)
					{
						continue;
					}
					GraphicsPath graphicsPath2 = graphicsPath;
					float x2 = x;
					float y2 = y;
					bool flag = false;
					if (shape != null || path != null || gridAttributes != null)
					{
						RectangleF bounds = graphicsPath.GetBounds();
						float num2 = Math.Max(bounds.Width, bounds.Height);
						if (num2 > 1000f)
						{
							float num3 = num2 / 1000f;
							PointF[] pathPoints = graphicsPath.PathPoints;
							for (int j = 0; j < pathPoints.Length; j++)
							{
								pathPoints[j].X /= num3;
								pathPoints[j].Y /= num3;
							}
							graphicsPath2 = new GraphicsPath(pathPoints, graphicsPath.PathTypes, graphicsPath.FillMode);
							flag = true;
							x2 = (float)x / num3;
							y2 = (float)y / num3;
						}
					}
					if (path != null)
					{
						using (Pen pen = path.GetBorderPen())
						{
							if (pen != null)
							{
								if (pen.Width < 7f)
								{
									pen.Width = 7f;
								}
								if (graphicsPath2.IsOutlineVisible(x2, y2, pen))
								{
									if (flag)
									{
										graphicsPath2.Dispose();
									}
									arrayList.Add(hotRegion);
									goto IL_03ae;
								}
							}
						}
					}
					else if (gridAttributes != null)
					{
						using (Pen pen2 = gridAttributes.GetPen())
						{
							if (pen2 != null)
							{
								if (pen2.Width < 5f)
								{
									pen2.Width = 5f;
								}
								if (graphicsPath2.IsOutlineVisible(x2, y2, pen2))
								{
									if (flag)
									{
										graphicsPath2.Dispose();
									}
									arrayList.Add(hotRegion);
									goto IL_03ae;
								}
							}
						}
					}
					else if (symbol != null)
					{
						RectangleF bounds2 = graphicsPath2.GetBounds();
						if (bounds2.Width < 3f)
						{
							bounds2.Inflate(3f - bounds2.Width, 0f);
						}
						if (bounds2.Height < 3f)
						{
							bounds2.Inflate(0f, 3f - bounds2.Height);
						}
						if (bounds2.Contains(x2, y2))
						{
							if (flag)
							{
								graphicsPath2.Dispose();
							}
							arrayList.Add(hotRegion);
							break;
						}
					}
					if (gridAttributes == null && graphicsPath2.IsVisible(x2, y2))
					{
						if (flag)
						{
							graphicsPath2.Dispose();
						}
						arrayList.Add(hotRegion);
						break;
					}
				}
				IL_03ae:;
			}
			if (arrayList.Count > 0)
			{
				return (HotRegion[])arrayList.ToArray(typeof(HotRegion));
			}
			return null;
		}

		internal bool IsOfType(Type[] objectTypes, object obj)
		{
			if (objectTypes.Length == 0)
			{
				return true;
			}
			for (int i = 0; i < objectTypes.Length; i++)
			{
				if (objectTypes[i].IsInstanceOfType(obj))
				{
					return true;
				}
			}
			return false;
		}

		internal void ClearContentElements()
		{
			for (int num = list.Count - 1; num >= 0; num--)
			{
				HotRegion hotRegion = list[num] as HotRegion;
				if (hotRegion != null && (hotRegion.SelectedObject is IContentElement || hotRegion.SelectedObject is GridAttributes))
				{
					list.RemoveAt(num);
					selectedObjectIndex.Remove(hotRegion.SelectedObject);
					hotRegion.Dispose();
				}
			}
		}

		internal void Clear()
		{
			foreach (HotRegion item in list)
			{
				item.Dispose();
			}
			list.Clear();
			selectedObjectIndex.Clear();
		}
	}
}
