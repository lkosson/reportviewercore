using System.Collections.Generic;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class MapSimplifier
	{
		private class ReducedPointList : List<MapPoint>
		{
			private Dictionary<MapPoint, int> distinctPointIndex = new Dictionary<MapPoint, int>();

			public int DistinctCount => distinctPointIndex.Count;

			public MapPoint LastPoint => base[base.Count - 1];

			public new void Add(MapPoint point)
			{
				if (base.Count == 0 || !point.Equals(LastPoint))
				{
					base.Add(point);
					distinctPointIndex[point] = base.Count - 1;
				}
			}
		}

		private Dictionary<MapPoint, MapPoint> simplifiedPoints = new Dictionary<MapPoint, MapPoint>();

		public void Reset()
		{
			simplifiedPoints.Clear();
		}

		public void Simplify(MapCore map, double resolution)
		{
			if (resolution <= 0.0)
			{
				return;
			}
			foreach (Shape shape in map.Shapes)
			{
				Simplify(shape, resolution);
			}
			foreach (Path path in map.Paths)
			{
				Simplify(path, resolution);
			}
		}

		public void Simplify(IEnumerable<ISpatialElement> elements, double resolution)
		{
			if (resolution <= 0.0)
			{
				return;
			}
			foreach (ISpatialElement element in elements)
			{
				Shape shape = element as Shape;
				if (shape != null)
				{
					Simplify(shape, resolution);
					continue;
				}
				Path path = element as Path;
				if (path != null)
				{
					Simplify(path, resolution);
				}
			}
		}

		public void Simplify(Shape shape, double resolution)
		{
			if (resolution <= 0.0)
			{
				return;
			}
			List<ShapeSegment> list = new List<ShapeSegment>();
			List<MapPoint> list2 = new List<MapPoint>();
			int num = 0;
			for (int i = 0; i < shape.ShapeData.Segments.Length; i++)
			{
				ShapeSegment item = shape.ShapeData.Segments[i];
				if (item.Type != 0 && item.Type != SegmentType.PolyLine)
				{
					list.Add(item);
					for (int j = 0; j < item.Length; j++)
					{
						list2.Add(shape.ShapeData.Points[num++]);
					}
					continue;
				}
				MapPoint[] array = new MapPoint[item.Length];
				for (int k = 0; k < item.Length; k++)
				{
					array[k] = shape.ShapeData.Points[num++];
				}
				ReducedPointList reducedPointList = ReducePoints(array, resolution);
				if (reducedPointList.DistinctCount >= 3)
				{
					ShapeSegment item2 = default(ShapeSegment);
					item2.Type = item.Type;
					item2.Length = reducedPointList.Count;
					list.Add(item2);
					list2.AddRange(reducedPointList);
				}
			}
			shape.ShapeData.Segments = list.ToArray();
			shape.ShapeData.Points = list2.ToArray();
			shape.ShapeData.UpdateStoredParameters();
		}

		public void Simplify(Path path, double resolution)
		{
			if (resolution <= 0.0)
			{
				return;
			}
			List<PathSegment> list = new List<PathSegment>();
			List<MapPoint> list2 = new List<MapPoint>();
			int num = 0;
			for (int i = 0; i < path.PathData.Segments.Length; i++)
			{
				PathSegment item = path.PathData.Segments[i];
				if (item.Type != 0 && item.Type != SegmentType.PolyLine)
				{
					list.Add(item);
					for (int j = 0; j < item.Length; j++)
					{
						list2.Add(path.PathData.Points[num++]);
					}
					continue;
				}
				MapPoint[] array = new MapPoint[item.Length];
				for (int k = 0; k < item.Length; k++)
				{
					array[k] = path.PathData.Points[num++];
				}
				ReducedPointList reducedPointList = ReducePoints(array, resolution);
				if (reducedPointList.DistinctCount >= 2)
				{
					PathSegment item2 = default(PathSegment);
					item2.Type = item.Type;
					item2.Length = reducedPointList.Count;
					list.Add(item2);
					list2.AddRange(reducedPointList);
				}
			}
			path.PathData.Segments = list.ToArray();
			path.PathData.Points = list2.ToArray();
			path.PathData.UpdateStoredParameters();
		}

		private ReducedPointList ReducePoints(MapPoint[] points, double resolution)
		{
			ReducedPointList reducedPointList = new ReducedPointList();
			double num = resolution * resolution;
			for (int i = 0; i < points.Length; i++)
			{
				MapPoint mapPoint = points[i];
				if (i == 0)
				{
					if (simplifiedPoints.ContainsKey(mapPoint))
					{
						reducedPointList.Add(simplifiedPoints[mapPoint]);
						continue;
					}
					reducedPointList.Add(mapPoint);
					simplifiedPoints.Add(mapPoint, mapPoint);
				}
				else if (simplifiedPoints.ContainsKey(mapPoint))
				{
					reducedPointList.Add(simplifiedPoints[mapPoint]);
				}
				else if (Utils.GetDistanceSqr(reducedPointList.LastPoint, mapPoint) > num)
				{
					reducedPointList.Add(mapPoint);
					simplifiedPoints.Add(mapPoint, mapPoint);
				}
				else
				{
					simplifiedPoints.Add(mapPoint, reducedPointList.LastPoint);
				}
			}
			return reducedPointList;
		}
	}
}
