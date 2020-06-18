using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Gauge.WebForms
{
	[SRDescription("DescriptionAttributeMapAreaCollection_MapAreaCollection")]
	internal class MapAreaCollection : IList, ICollection, IEnumerable
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
				throw new ArgumentException(Utils.SRGetStr("ExceptionImagemapInvalidObject"));
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
				throw new ArgumentException(Utils.SRGetStr("ExceptionImagemapInvalidObject"));
			}
			array.Insert(index, value);
		}

		public void CopyTo(Array array, int index)
		{
			array.CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			return array.GetEnumerator();
		}

		public int Add(string toolTip, string href, string attr, GraphicsPath path, object tag)
		{
			int num = Add(toolTip, href, attr, path);
			if (num >= 0)
			{
				this[num].Tag = tag;
			}
			return num;
		}

		public int Add(string toolTip, string href, string attr, GraphicsPath path)
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
				return Add(MapAreaShape.Polygon, toolTip, href, attr, array);
			}
			return -1;
		}

		public int Add(string toolTip, string href, string attr, RectangleF rect)
		{
			return Add(MapAreaShape.Rectangle, toolTip, href, attr, new float[4]
			{
				rect.X,
				rect.Y,
				rect.Right,
				rect.Bottom
			});
		}

		public int Add(MapAreaShape shape, string toolTip, string href, string attr, float[] coordinates)
		{
			if (shape == MapAreaShape.Circle && coordinates.Length != 3)
			{
				throw new InvalidOperationException(Utils.SRGetStr("ExceptionImagemapInvalidCircle"));
			}
			if (shape == MapAreaShape.Rectangle && coordinates.Length != 4)
			{
				throw new InvalidOperationException(Utils.SRGetStr("ExceptionImagemapInvalidRectangle"));
			}
			if (shape == MapAreaShape.Polygon && (float)coordinates.Length % 2f != 0f)
			{
				throw new InvalidOperationException(Utils.SRGetStr("ExceptionImagemapInvalidPolygon"));
			}
			MapArea mapArea = new MapArea();
			mapArea.Custom = false;
			mapArea.ToolTip = toolTip;
			mapArea.Href = href;
			mapArea.MapAreaAttributes = attr;
			mapArea.Shape = shape;
			mapArea.Coordinates = new float[coordinates.Length];
			coordinates.CopyTo(mapArea.Coordinates, 0);
			return Add(mapArea);
		}

		public void Insert(int index, string toolTip, string href, string attr, GraphicsPath path)
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
				Insert(index, MapAreaShape.Polygon, toolTip, href, attr, array);
			}
		}

		public void Insert(int index, string toolTip, string href, string attr, RectangleF rect)
		{
			Insert(index, MapAreaShape.Rectangle, toolTip, href, attr, new float[4]
			{
				rect.X,
				rect.Y,
				rect.Right,
				rect.Bottom
			});
		}

		public void Insert(int index, MapAreaShape shape, string toolTip, string href, string attr, float[] coordinates)
		{
			if (shape == MapAreaShape.Circle && coordinates.Length != 3)
			{
				throw new InvalidOperationException(Utils.SRGetStr("ExceptionImagemapInvalidCircle"));
			}
			if (shape == MapAreaShape.Rectangle && coordinates.Length != 4)
			{
				throw new InvalidOperationException(Utils.SRGetStr("ExceptionImagemapInvalidRectangle"));
			}
			if (shape == MapAreaShape.Polygon && (float)coordinates.Length % 2f != 0f)
			{
				throw new InvalidOperationException(Utils.SRGetStr("ExceptionImagemapInvalidPolygon"));
			}
			MapArea mapArea = new MapArea();
			mapArea.Custom = false;
			mapArea.ToolTip = toolTip;
			mapArea.Href = href;
			mapArea.MapAreaAttributes = attr;
			mapArea.Shape = shape;
			mapArea.Coordinates = new float[coordinates.Length];
			coordinates.CopyTo(mapArea.Coordinates, 0);
			Insert(index, mapArea);
		}

		public int Add(MapArea value)
		{
			return array.Add(value);
		}

		public void Remove(MapArea value)
		{
			array.Remove(value);
		}

		public bool Contains(MapArea value)
		{
			return array.Contains(value);
		}

		public int IndexOf(MapArea value)
		{
			return array.IndexOf(value);
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
