using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class GeoUtils
	{
		private class PolygonCutter
		{
			private List<PolygonPart> m_polygonPartsList;

			private bool m_polygonIsClosed;

			private bool m_polygonIsCut;

			public void ProcessShapeSegment(ShapeSegment segment, IList<MapPoint> points, int firstPointIndex, PolygonClosingPole closingPole, out List<ShapeSegment> segments, out List<MapPoint> segmentPoints, out bool isClosedAtPole)
			{
				isClosedAtPole = false;
				segments = new List<ShapeSegment>();
				segmentPoints = new List<MapPoint>();
				Load(points, firstPointIndex, segment.Length, closingPole);
				foreach (PolygonPart shape in GetShapes())
				{
					ShapeSegment item = default(ShapeSegment);
					item.Type = segment.Type;
					item.Length = shape.Points.Count;
					segments.Add(item);
					segmentPoints.AddRange(shape.Points);
					isClosedAtPole |= shape.isClosedAtPole;
				}
			}

			public void Load(IList<MapPoint> points, int firstPointIndex, int segmentLength, PolygonClosingPole closingPole)
			{
				MapPoint mapPoint = points[firstPointIndex];
				MapPoint mapPoint2 = points[firstPointIndex + segmentLength - 1];
				m_polygonIsClosed = mapPoint.Equals(mapPoint2);
				m_polygonIsCut = false;
				m_polygonPartsList = new List<PolygonPart>();
				PolygonPart polygonPart = new PolygonPart(closingPole);
				MapPoint mapPoint3 = default(MapPoint);
				for (int i = 0; i < segmentLength; i++)
				{
					MapPoint mapPoint4 = points[firstPointIndex + i];
					if (i == 0)
					{
						polygonPart.Points.Add(mapPoint4);
					}
					else
					{
						MapPoint mapPoint5 = mapPoint3;
						foreach (MapPoint item in DensifyLine(mapPoint3, mapPoint4, DEFAULT_DENSIFICATION_ANGLE))
						{
							if (Math.Abs(item.X - mapPoint5.X) <= 180.0)
							{
								polygonPart.Points.Add(item);
								mapPoint5 = item;
								continue;
							}
							if (Math.Abs(item.X) == 180.0)
							{
								MapPoint mapPoint6 = new MapPoint(0.0 - item.X, item.Y);
								polygonPart.Points.Add(mapPoint6);
								mapPoint5 = mapPoint6;
								continue;
							}
							if (Math.Abs(mapPoint5.X) == 180.0 && polygonPart.Points.Count == 1)
							{
								mapPoint5.X = 0.0 - mapPoint5.X;
								polygonPart.Points[polygonPart.Points.Count - 1] = mapPoint5;
								polygonPart.Points.Add(item);
								mapPoint5 = item;
								continue;
							}
							FindIntersectionPoints(mapPoint5, item, out MapPoint intersectionPoint, out MapPoint intersectionPoint2);
							if (!mapPoint5.Equals(intersectionPoint))
							{
								polygonPart.Points.Add(intersectionPoint);
							}
							m_polygonPartsList.Add(polygonPart);
							polygonPart = new PolygonPart(closingPole);
							if (!item.Equals(intersectionPoint2))
							{
								polygonPart.Points.Add(intersectionPoint2);
							}
							polygonPart.Points.Add(item);
							m_polygonIsCut = true;
							mapPoint5 = item;
						}
						mapPoint4 = mapPoint5;
					}
					mapPoint3 = mapPoint4;
				}
				if (m_polygonIsCut && m_polygonIsClosed && polygonPart.LastPoint.Equals(m_polygonPartsList[0].FirstPoint))
				{
					polygonPart.Points.RemoveAt(polygonPart.Points.Count - 1);
					m_polygonPartsList[0].Points.InsertRange(0, polygonPart.Points);
				}
				else
				{
					m_polygonPartsList.Add(polygonPart);
				}
				if (!m_polygonIsCut && m_polygonIsClosed && Math.Abs(polygonPart.FirstPoint.X) == 180.0 && polygonPart.FirstPoint.X == 0.0 - polygonPart.LastPoint.X)
				{
					m_polygonIsCut = true;
				}
			}

			public List<PolygonPart> GetPaths()
			{
				return m_polygonPartsList;
			}

			public List<PolygonPart> GetShapes()
			{
				if (!m_polygonIsCut)
				{
					return m_polygonPartsList;
				}
				List<PolygonPart> list = new List<PolygonPart>();
				if (!m_polygonIsClosed)
				{
					foreach (PolygonPart polygonParts in m_polygonPartsList)
					{
						list.Add(polygonParts);
					}
					return list;
				}
				if (m_polygonPartsList.Count > 1)
				{
					m_polygonPartsList.Sort(new PolygonPart.Comparer());
					int num;
					for (num = 0; num < m_polygonPartsList.Count; num++)
					{
						PolygonPart polygonPart = m_polygonPartsList[num];
						polygonPart.CollectKids(m_polygonPartsList);
						num = m_polygonPartsList.IndexOf(polygonPart);
					}
				}
				foreach (PolygonPart polygonParts2 in m_polygonPartsList)
				{
					polygonParts2.SaveToResultsWithChildren(list);
				}
				return list;
			}

			private void FindIntersectionPoints(MapPoint point1, MapPoint point2, out MapPoint intersectionPoint1, out MapPoint intersectionPoint2)
			{
				double num = (point1.X > 0.0) ? 180 : (-180);
				MapPoint mapPoint = default(MapPoint);
				mapPoint.X = point2.X + num * 2.0;
				mapPoint.Y = point2.Y;
				double num2 = mapPoint.X - point1.X;
				double num3 = mapPoint.Y - point1.Y;
				intersectionPoint1 = default(MapPoint);
				intersectionPoint1.X = num;
				if (num2 != 0.0)
				{
					intersectionPoint1.Y = point1.Y + (num - point1.X) * num3 / num2;
				}
				else
				{
					intersectionPoint1.Y = point1.Y + num3 / 2.0;
				}
				intersectionPoint2 = default(MapPoint);
				intersectionPoint2.X = 0.0 - num;
				intersectionPoint2.Y = intersectionPoint1.Y;
			}
		}

		private enum PolygonClosingPole
		{
			North,
			South
		}

		private class PolygonPart
		{
			internal class Comparer : IComparer<PolygonPart>
			{
				public int Compare(PolygonPart p1, PolygonPart p2)
				{
					double topPointY = p1.TopPointY;
					double topPointY2 = p2.TopPointY;
					if (topPointY == topPointY2)
					{
						return 0;
					}
					if (topPointY < topPointY2)
					{
						return -1;
					}
					return 1;
				}
			}

			internal class ReverseComparer : IComparer<PolygonPart>
			{
				public int Compare(PolygonPart p1, PolygonPart p2)
				{
					double topPointY = p1.TopPointY;
					double topPointY2 = p2.TopPointY;
					if (topPointY == topPointY2)
					{
						return 0;
					}
					if (topPointY > topPointY2)
					{
						return -1;
					}
					return 1;
				}
			}

			public List<MapPoint> Points = new List<MapPoint>();

			public List<PolygonPart> Kids = new List<PolygonPart>();

			private PolygonClosingPole closingPole;

			internal bool isClosedAtPole;

			public virtual MapPoint FirstPoint => Points[0];

			public virtual MapPoint LastPoint => Points[Points.Count - 1];

			public bool InNorth => FirstPoint.Y > 0.0;

			public bool InWest
			{
				get
				{
					if (FirstPoint.X != -180.0)
					{
						return LastPoint.X == -180.0;
					}
					return true;
				}
			}

			public bool InEast
			{
				get
				{
					if (FirstPoint.X != 180.0)
					{
						return LastPoint.X == 180.0;
					}
					return true;
				}
			}

			public double TopPointY
			{
				get
				{
					if (InNorth && FirstPoint.X != LastPoint.X)
					{
						return 90.0;
					}
					return Math.Max(FirstPoint.Y, LastPoint.Y);
				}
			}

			public double BottomPointY
			{
				get
				{
					if (!InNorth && FirstPoint.X != LastPoint.X)
					{
						return -90.0;
					}
					return Math.Min(FirstPoint.Y, LastPoint.Y);
				}
			}

			public PolygonPart(PolygonClosingPole closingPole)
			{
				this.closingPole = closingPole;
			}

			public void CollectKids(List<PolygonPart> list)
			{
				if (IsComplete())
				{
					return;
				}
				int num = 0;
				while (num < list.Count)
				{
					PolygonPart polygonPart = list[num];
					if (this != polygonPart && IsOnTheSameSide(polygonPart) && polygonPart.TopPointY < TopPointY && polygonPart.BottomPointY > BottomPointY)
					{
						Kids.Add(polygonPart);
						polygonPart.CollectKids(list);
						list.Remove(polygonPart);
					}
					else
					{
						num++;
					}
				}
			}

			public bool IsOnTheSameSide(PolygonPart other)
			{
				if (!InWest || !other.InWest)
				{
					if (InEast)
					{
						return other.InEast;
					}
					return false;
				}
				return true;
			}

			public bool IsComplete()
			{
				return FirstPoint.Equals(LastPoint);
			}

			public void ConnectByMeridianOrAroundTheGlobe(MapPoint point1, MapPoint point2)
			{
				if ((point1.X == 180.0 && point2.X == -180.0) || (point1.X == -180.0 && point2.X == 180.0))
				{
					isClosedAtPole = true;
					double y = (closingPole == PolygonClosingPole.North) ? 90 : (-90);
					MapPoint mapPoint = new MapPoint(point1.X, y);
					MapPoint mapPoint2 = new MapPoint(point2.X, y);
					AddVerticalMidPoints(point1, mapPoint);
					Points.Add(mapPoint);
					AddHorizontalMidPoints(mapPoint, mapPoint2);
					Points.Add(mapPoint2);
					AddVerticalMidPoints(mapPoint2, point2);
				}
				else if ((point1.X == 180.0 && point2.X == 180.0) || (point1.X == -180.0 && point2.X == -180.0))
				{
					AddVerticalMidPoints(point1, point2);
				}
			}

			public void AddHorizontalMidPoints(MapPoint point1, MapPoint point2)
			{
				if (point1.X < point2.X)
				{
					for (double num = Math.Floor(point1.X + 1.0); num < point2.X; num += 1.0)
					{
						Points.Add(new MapPoint(num, point1.Y));
					}
					return;
				}
				for (double num2 = Math.Ceiling(point1.X - 1.0); num2 > point2.X; num2 -= 1.0)
				{
					Points.Add(new MapPoint(num2, point1.Y));
				}
			}

			public void AddVerticalMidPoints(MapPoint point1, MapPoint point2)
			{
				if (point1.Y < point2.Y)
				{
					for (double num = Math.Floor(point1.Y + 1.0); num < point2.Y; num += 1.0)
					{
						Points.Add(new MapPoint(point1.X, num));
					}
					return;
				}
				for (double num2 = Math.Ceiling(point1.Y - 1.0); num2 > point2.Y; num2 -= 1.0)
				{
					Points.Add(new MapPoint(point1.X, num2));
				}
			}

			public void SaveToResultsWithChildren(List<PolygonPart> results)
			{
				results.Add(this);
				if (!IsComplete())
				{
					foreach (PolygonPart kid in Kids)
					{
						ConnectByMeridianOrAroundTheGlobe(LastPoint, kid.FirstPoint);
						Points.AddRange(kid.Points);
					}
					ConnectByMeridianOrAroundTheGlobe(LastPoint, FirstPoint);
					Points.Add(Points[0]);
				}
				foreach (PolygonPart kid2 in Kids)
				{
					foreach (PolygonPart kid3 in kid2.Kids)
					{
						kid3.SaveToResultsWithChildren(results);
					}
				}
			}

			public override string ToString()
			{
				string text = string.Format(CultureInfo.CurrentCulture, "{0} - {1}. ", FirstPoint, LastPoint);
				foreach (MapPoint point in Points)
				{
					text += point.ToString();
				}
				return text;
			}
		}

		internal class Vector3
		{
			public double x;

			public double y;

			public double z;

			public double Longitude => Math.Atan2(y, x);

			public double LongitudeDeg => ToDegrees(Longitude);

			public double Latitude => Math.Asin(z);

			public double LatitudeDeg => ToDegrees(Latitude);

			public Vector3(double x, double y, double z)
			{
				this.x = x;
				this.y = y;
				this.z = z;
			}

			public static Vector3 operator +(Vector3 a, Vector3 b)
			{
				return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
			}

			public static Vector3 operator -(Vector3 a, Vector3 b)
			{
				return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
			}

			public static Vector3 operator *(Vector3 vector, double a)
			{
				return new Vector3(vector.x * a, vector.y * a, vector.z * a);
			}

			public static Vector3 operator /(Vector3 v, double a)
			{
				return v * (1.0 / a);
			}

			public Vector3 Unitize()
			{
				return this / VectorLength();
			}

			public static double operator *(Vector3 a, Vector3 b)
			{
				return b.x * a.x + b.y * a.y + b.z * a.z;
			}

			public double LengthSquared()
			{
				return this * this;
			}

			public double VectorLength()
			{
				return Math.Sqrt(LengthSquared());
			}

			public double DistanceSquared(Vector3 a)
			{
				return (this - a) * (this - a);
			}

			public double Distance(Vector3 a)
			{
				return Math.Sqrt(DistanceSquared(a));
			}

			public Vector3 CrossProduct(Vector3 a)
			{
				return new Vector3(y * a.z - z * a.y, z * a.x - x * a.z, x * a.y - y * a.x);
			}

			public double Angle(Vector3 a)
			{
				return 2.0 * Math.Asin(Distance(a) / (2.0 * a.VectorLength()));
			}

			public double AngleInDegrees(Vector3 a)
			{
				return ToDegrees(Angle(a));
			}

			public static Vector3 FromLatLong(double latitudeDeg, double longitudeDeg)
			{
				double num = ToRadians(latitudeDeg);
				double num2 = ToRadians(longitudeDeg);
				double num3 = Math.Cos(num);
				return new Vector3(num3 * Math.Cos(num2), num3 * Math.Sin(num2), Math.Sin(num));
			}
		}

		private static double DEFAULT_DENSIFICATION_ANGLE = 0.1;

		public static void CutShapes(ref MapPoint[] points, ref ShapeSegment[] segments)
		{
			List<ShapeSegment> list = new List<ShapeSegment>();
			List<MapPoint> list2 = new List<MapPoint>();
			PolygonCutter polygonCutter = new PolygonCutter();
			int num = 0;
			ShapeSegment[] array = segments;
			for (int i = 0; i < array.Length; i++)
			{
				ShapeSegment segment = array[i];
				if (segment.Length > 0)
				{
					bool isClosedAtPole = false;
					polygonCutter.ProcessShapeSegment(segment, points, num, PolygonClosingPole.North, out List<ShapeSegment> segments2, out List<MapPoint> segmentPoints, out isClosedAtPole);
					if (isClosedAtPole)
					{
						polygonCutter.ProcessShapeSegment(segment, points, num, PolygonClosingPole.South, out List<ShapeSegment> segments3, out List<MapPoint> segmentPoints2, out isClosedAtPole);
						ShapeSegment[] segments4 = segments2.ToArray();
						MapPoint[] points2 = segmentPoints.ToArray();
						CalculateSignedArea(ref points2, ref segments4);
						double num2 = 0.0;
						for (int j = 0; j < segments4.Length; j++)
						{
							num2 += Math.Abs(segments4[j].PolygonSignedArea);
						}
						ShapeSegment[] segments5 = segments3.ToArray();
						MapPoint[] points3 = segmentPoints2.ToArray();
						CalculateSignedArea(ref points3, ref segments5);
						double num3 = 0.0;
						for (int k = 0; k < segments5.Length; k++)
						{
							num3 += Math.Abs(segments5[k].PolygonSignedArea);
						}
						if (num2 < num3)
						{
							list.AddRange(segments2);
							list2.AddRange(segmentPoints);
						}
						else
						{
							list.AddRange(segments3);
							list2.AddRange(segmentPoints2);
						}
					}
					else
					{
						list.AddRange(segments2);
						list2.AddRange(segmentPoints);
					}
				}
				num += segment.Length;
			}
			segments = list.ToArray();
			points = list2.ToArray();
		}

		public static void CutPaths(ref MapPoint[] points, ref PathSegment[] segments)
		{
			List<PathSegment> list = new List<PathSegment>();
			List<MapPoint> list2 = new List<MapPoint>();
			PolygonCutter polygonCutter = new PolygonCutter();
			int num = 0;
			PathSegment[] array = segments;
			for (int i = 0; i < array.Length; i++)
			{
				PathSegment pathSegment = array[i];
				if (pathSegment.Length > 0)
				{
					polygonCutter.Load(points, num, pathSegment.Length, PolygonClosingPole.North);
					foreach (PolygonPart path in polygonCutter.GetPaths())
					{
						PathSegment item = default(PathSegment);
						item.Type = pathSegment.Type;
						item.Length = path.Points.Count;
						list.Add(item);
						list2.AddRange(path.Points);
					}
				}
				num += pathSegment.Length;
			}
			segments = list.ToArray();
			points = list2.ToArray();
		}

		public static IEnumerable<MapPoint> DensifyLine(MapPoint prevPoint, MapPoint point, double maxAngle)
		{
			double num = Math.Abs(point.X - prevPoint.X);
			double num2 = Math.Abs(point.Y - prevPoint.Y);
			if (num + num2 > maxAngle)
			{
				double num3 = ToRadians(maxAngle);
				Vector3 startPoint = Vector3.FromLatLong(prevPoint.Y, prevPoint.X);
				Vector3 vector = Vector3.FromLatLong(point.Y, point.X);
				double num4 = vector.Angle(startPoint);
				if (num4 > num3)
				{
					Vector3 a = (startPoint + vector).CrossProduct(startPoint - vector).Unitize();
					Vector3 yAxis = startPoint.CrossProduct(a);
					int count = Convert.ToInt32(Math.Ceiling(num4 / num3));
					double num5 = num4 / (double)count;
					double cosine = Math.Cos(num5);
					double sine = Math.Sin(num5);
					double x = cosine;
					double y = sine;
					for (int i = 0; i < count - 1; i++)
					{
						Vector3 vector2 = (startPoint * x + yAxis * y).Unitize();
						yield return new MapPoint(vector2.LongitudeDeg, vector2.LatitudeDeg);
						double num6 = x * cosine - y * sine;
						y = x * sine + y * cosine;
						x = num6;
					}
				}
			}
			yield return point;
		}

		public static double ToRadians(double a)
		{
			return a / 180.0 * Math.PI;
		}

		public static double ToDegrees(double a)
		{
			return a * 180.0 / Math.PI;
		}

		public static void NormalizePointsLongigute(ref MapPoint[] points)
		{
			for (int i = 0; i < points.Length; i++)
			{
				if (points[i].X < -180.0)
				{
					points[i].X = (points[i].X - 180.0) % 360.0 + 180.0;
				}
				else if (points[i].X >= 180.0)
				{
					points[i].X = (points[i].X + 180.0) % 360.0 - 180.0;
				}
			}
		}

		internal static void CalculateSignedArea(ref MapPoint[] points, ref ShapeSegment[] segments)
		{
			int num = 0;
			for (int i = 0; i < segments.Length; i++)
			{
				segments[i].PolygonSignedArea = 0.0;
				for (int j = 0; j < segments[i].Length; j++)
				{
					int num2 = num - j;
					int num3 = (j + 1) % segments[i].Length;
					segments[i].PolygonSignedArea += points[j + num2].X * points[num3 + num2].Y;
					segments[i].PolygonSignedArea -= points[j + num2].Y * points[num3 + num2].X;
					num++;
				}
				segments[i].PolygonSignedArea /= 2.0;
			}
		}

		internal static void FixOrientationForGeometry(ref MapPoint[] points, ref ShapeSegment[] segments)
		{
			CalculateSignedArea(ref points, ref segments);
			int num = 0;
			for (int i = 0; i < segments.Length; i++)
			{
				if ((i == 0 && segments[i].PolygonSignedArea >= 0.0) || (i != 0 && segments[i].PolygonSignedArea < 0.0))
				{
					for (int j = num; j < num + segments[i].Length / 2; j++)
					{
						MapPoint mapPoint = points[j];
						points[j] = points[segments[i].Length + num - (j - num) - 1];
						points[segments[i].Length + num - (j - num) - 1] = mapPoint;
					}
				}
				num += segments[i].Length;
			}
		}

		internal static void MoveLargestSegmentToFront(ref MapPoint[] points, ref ShapeSegment[] segments)
		{
			CalculateSignedArea(ref points, ref segments);
			double num = 0.0;
			int num2 = 0;
			int num3 = 0;
			for (int i = 0; i < segments.Length; i++)
			{
				if (segments[i].PolygonSignedArea < num)
				{
					num = segments[i].PolygonSignedArea;
					num2 = i;
				}
			}
			if (num2 != 0)
			{
				List<MapPoint> list = new List<MapPoint>(points);
				num3 = 0;
				for (int j = 0; j < num2; j++)
				{
					num3 += segments[j].Length;
				}
				List<MapPoint> range = list.GetRange(num3, segments[num2].Length);
				list.RemoveRange(num3, segments[num2].Length);
				list.InsertRange(0, range);
				points = list.ToArray();
				ShapeSegment shapeSegment = segments[0];
				segments[0] = segments[num2];
				segments[num2] = shapeSegment;
			}
		}
	}
}
