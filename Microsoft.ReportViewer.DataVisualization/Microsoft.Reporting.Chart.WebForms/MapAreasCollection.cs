using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeMapAreasCollection_MapAreasCollection")]
	internal class MapAreasCollection : IList, ICollection, IEnumerable
	{
		internal ArrayList array = new ArrayList();

		public MapArea this[int index]
		{
			get
			{
				return (MapArea)array[index];
			}
			set
			{
				array[index] = value;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return array[index];
			}
			set
			{
				array[index] = value;
			}
		}

		public bool IsFixedSize => array.IsFixedSize;

		public bool IsReadOnly => array.IsReadOnly;

		public int Count => array.Count;

		public bool IsSynchronized => array.IsSynchronized;

		public object SyncRoot => array.SyncRoot;

		public void Clear()
		{
			array.Clear();
		}

		public bool Contains(object value)
		{
			return array.Contains(value);
		}

		public int IndexOf(object value)
		{
			return array.IndexOf(value);
		}

		public void Remove(object value)
		{
			array.Remove(value);
		}

		public void RemoveAt(int index)
		{
			array.RemoveAt(index);
		}

		public int Add(object value)
		{
			if (!(value is MapArea))
			{
				throw new ArgumentException(SR.ExceptionImageMapAddedHasWrongType);
			}
			return array.Add(value);
		}

		public void Insert(int index, MapArea value)
		{
			Insert(index, (object)value);
		}

		public void Insert(int index, object value)
		{
			if (!(value is MapArea))
			{
				throw new ArgumentException(SR.ExceptionImageMapInsertedHasWrongType);
			}
			array.Insert(index, value);
		}

		public bool Contains(MapArea value)
		{
			return array.Contains(value);
		}

		public int IndexOf(MapArea value)
		{
			return array.IndexOf(value);
		}

		public void Remove(MapArea value)
		{
			array.Remove(value);
		}

		public void CopyTo(Array array, int index)
		{
			this.array.CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			return array.GetEnumerator();
		}

		internal int Add(string toolTip, string href, string attr, GraphicsPath path, object tag)
		{
			if (path.PointCount > 0)
			{
				path.Flatten();
				PointF[] pathPoints = path.PathPoints;
				float[] array = new float[pathPoints.Length * 2];
				int num = 0;
				PointF[] array2 = pathPoints;
				for (int i = 0; i < array2.Length; i++)
				{
					PointF pointF = array2[i];
					array[num++] = pointF.X;
					array[num++] = pointF.Y;
				}
				return Add(MapAreaShape.Polygon, toolTip, href, attr, array, tag);
			}
			return -1;
		}

		internal int Add(string toolTip, string href, string attr, RectangleF rect, object tag)
		{
			return Add(MapAreaShape.Rectangle, toolTip, href, attr, new float[4]
			{
				rect.X,
				rect.Y,
				rect.Right,
				rect.Bottom
			}, tag);
		}

		internal int Add(MapAreaShape shape, string toolTip, string href, string attr, float[] coordinates, object tag)
		{
			if (shape == MapAreaShape.Circle && coordinates.Length != 3)
			{
				throw new InvalidOperationException(SR.ExceptionImageMapCircleShapeInvalid);
			}
			if (shape == MapAreaShape.Rectangle && coordinates.Length != 4)
			{
				throw new InvalidOperationException(SR.ExceptionImageMapRectangleShapeInvalid);
			}
			if (shape == MapAreaShape.Polygon && (float)coordinates.Length % 2f != 0f)
			{
				throw new InvalidOperationException(SR.ExceptionImageMapPolygonShapeInvalid);
			}
			MapArea mapArea = new MapArea();
			mapArea.Custom = false;
			mapArea.ToolTip = toolTip;
			mapArea.Href = href;
			mapArea.MapAreaAttributes = attr;
			mapArea.Shape = shape;
			mapArea.Coordinates = new float[coordinates.Length];
			coordinates.CopyTo(mapArea.Coordinates, 0);
			((IMapAreaAttributes)mapArea).Tag = tag;
			return Add(mapArea);
		}

		internal void Insert(int index, string toolTip, string href, string attr, GraphicsPath path, object tag)
		{
			if (path.PointCount > 0)
			{
				path.Flatten();
				PointF[] pathPoints = path.PathPoints;
				float[] array = new float[pathPoints.Length * 2];
				int num = 0;
				PointF[] array2 = pathPoints;
				for (int i = 0; i < array2.Length; i++)
				{
					PointF pointF = array2[i];
					array[num++] = pointF.X;
					array[num++] = pointF.Y;
				}
				Insert(index, MapAreaShape.Polygon, toolTip, href, attr, array, tag);
			}
		}

		internal void Insert(int index, string toolTip, string href, string attr, GraphicsPath path, bool absCoordinates, ChartGraphics graph)
		{
			GraphicsPathIterator graphicsPathIterator = new GraphicsPathIterator(path);
			if (graphicsPathIterator.SubpathCount > 1)
			{
				GraphicsPath graphicsPath = new GraphicsPath();
				while (graphicsPathIterator.NextMarker(graphicsPath) > 0)
				{
					InsertSubpath(index, toolTip, href, attr, graphicsPath, absCoordinates, graph);
					graphicsPath.Reset();
				}
			}
			else
			{
				InsertSubpath(index, toolTip, href, attr, path, absCoordinates, graph);
			}
		}

		private void InsertSubpath(int index, string toolTip, string href, string attr, GraphicsPath path, bool absCoordinates, ChartGraphics graph)
		{
			if (path.PointCount <= 0)
			{
				return;
			}
			path.Flatten();
			PointF[] pathPoints = path.PathPoints;
			float[] array = new float[pathPoints.Length * 2];
			if (absCoordinates)
			{
				for (int i = 0; i < pathPoints.Length; i++)
				{
					pathPoints[i] = graph.GetRelativePoint(pathPoints[i]);
				}
			}
			int num = 0;
			PointF[] array2 = pathPoints;
			for (int j = 0; j < array2.Length; j++)
			{
				PointF pointF = array2[j];
				array[num++] = pointF.X;
				array[num++] = pointF.Y;
			}
			Insert(index, MapAreaShape.Polygon, toolTip, href, attr, array, null);
		}

		internal void Insert(int index, string toolTip, string href, string attr, RectangleF rect, object tag)
		{
			Insert(index, MapAreaShape.Rectangle, toolTip, href, attr, new float[4]
			{
				rect.X,
				rect.Y,
				rect.Right,
				rect.Bottom
			}, tag);
		}

		internal void Insert(int index, MapAreaShape shape, string toolTip, string href, string attr, float[] coordinates, object tag)
		{
			if (shape == MapAreaShape.Circle && coordinates.Length != 3)
			{
				throw new InvalidOperationException(SR.ExceptionImageMapCircleShapeInvalid);
			}
			if (shape == MapAreaShape.Rectangle && coordinates.Length != 4)
			{
				throw new InvalidOperationException(SR.ExceptionImageMapRectangleShapeInvalid);
			}
			if (shape == MapAreaShape.Polygon && (float)coordinates.Length % 2f != 0f)
			{
				throw new InvalidOperationException(SR.ExceptionImageMapPolygonShapeInvalid);
			}
			MapArea mapArea = new MapArea();
			mapArea.Custom = false;
			mapArea.ToolTip = toolTip;
			mapArea.Href = href;
			mapArea.MapAreaAttributes = attr;
			mapArea.Shape = shape;
			mapArea.Coordinates = new float[coordinates.Length];
			coordinates.CopyTo(mapArea.Coordinates, 0);
			((IMapAreaAttributes)mapArea).Tag = tag;
			Insert(index, mapArea);
		}

		public int Add(string href, GraphicsPath path)
		{
			return Add("", href, "", path, null);
		}

		public int Add(string href, RectangleF rect)
		{
			return Add("", href, "", rect, null);
		}

		public int Add(MapAreaShape shape, string href, float[] coordinates)
		{
			return Add(shape, "", href, "", coordinates, null);
		}

		public void Insert(int index, string href, GraphicsPath path)
		{
			Insert(index, "", href, "", path, null);
		}

		public void Insert(int index, string href, RectangleF rect)
		{
			Insert(index, "", href, "", rect, null);
		}

		public void Insert(int index, MapAreaShape shape, string href, float[] coordinates)
		{
			Insert(index, shape, "", href, "", coordinates, null);
		}

		internal void RemoveNonCustom()
		{
			for (int i = 0; i < Count; i++)
			{
				if (!this[i].Custom)
				{
					RemoveAt(i);
					i--;
				}
			}
		}
	}
}
