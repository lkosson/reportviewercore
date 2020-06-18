using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class GraphicsPathOutliner
	{
		private RectangleF[] boundsArray;

		private Hashtable pointsTable;

		private GraphicsPath[] paths;

		private Graphics graphics;

		private Hashtable visitedPoints;

		public GraphicsPathOutliner(Graphics graphics)
		{
			this.graphics = graphics;
		}

		public GraphicsPath GetOutlinePath(GraphicsPath[] paths)
		{
			this.paths = paths;
			Pen pen = new Pen(Brushes.Blue, 0f);
			RectangleF a = RectangleF.Empty;
			boundsArray = new RectangleF[paths.Length];
			pointsTable = new Hashtable();
			for (int i = 0; i < paths.Length; i++)
			{
				boundsArray[i] = paths[i].GetBounds();
				a = ((!a.IsEmpty) ? RectangleF.Union(a, boundsArray[i]) : boundsArray[i]);
			}
			visitedPoints = new Hashtable();
			PointInfo currentPoint = default(PointInfo);
			for (int j = 0; j < paths.Length; j++)
			{
				if (boundsArray[j].X == a.X)
				{
					currentPoint.Path = paths[j];
					currentPoint.Points = paths[j].PathPoints;
					break;
				}
			}
			pointsTable.Add(currentPoint.Path, currentPoint.Points);
			for (int k = 0; k < currentPoint.Points.Length; k++)
			{
				if (currentPoint.Points[k].X == a.X)
				{
					currentPoint.Point = currentPoint.Points[k];
					currentPoint.Index = k;
					break;
				}
			}
			ArrayList arrayList = new ArrayList();
			arrayList.Add(currentPoint.Point);
			PointInfo currentPoint2 = GetNextPoint(previousPoint: new PointF(a.X - a.Width, a.Y + 2f * a.Height), currentPoint: currentPoint, pen: pen);
			PointF point = currentPoint.Point;
			while (arrayList.Count < 3000 && currentPoint2.Point != currentPoint.Point)
			{
				arrayList.Add(currentPoint2.Point);
				if (!visitedPoints.Contains(currentPoint2.Point))
				{
					visitedPoints.Add(currentPoint2.Point, null);
				}
				PointInfo nextPoint = GetNextPoint(currentPoint2, point, pen);
				point = currentPoint2.Point;
				currentPoint2 = nextPoint;
			}
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.StartFigure();
			graphicsPath.AddPolygon((PointF[])arrayList.ToArray(typeof(PointF)));
			graphicsPath.CloseFigure();
			return graphicsPath;
		}

		private PointInfo GetNextPoint(PointInfo currentPoint, PointF previousPoint, Pen pen)
		{
			ArrayList arrayList = new ArrayList();
			for (int i = 0; i < boundsArray.Length; i++)
			{
				if (paths[i] != currentPoint.Path)
				{
					RectangleF rectangleF = boundsArray[i];
					rectangleF.Inflate(2f, 2f);
					if (rectangleF.Contains(currentPoint.Point) && (paths[i].IsOutlineVisible(currentPoint.Point, pen) || paths[i].IsVisible(currentPoint.Point)))
					{
						DrawMarker(currentPoint.Point, 2f);
						PointF[] pathPoints = GetPathPoints(paths[i]);
						PointInfo closestPoint = GetClosestPoint(currentPoint.Point, pathPoints, paths[i]);
						arrayList.Add(closestPoint);
					}
				}
			}
			if (arrayList.Count == 0)
			{
				return currentPoint.GetNextPoint(currentPoint.Direction);
			}
			PointInfo result = currentPoint.GetNextPoint(currentPoint.Direction);
			double num = CalculateAngle(previousPoint, currentPoint.Point, result.Point);
			foreach (PointInfo item in arrayList)
			{
				PointInfo nextPoint = item.GetNextPoint(Direction.Forward);
				double num2 = CalculateAngle(previousPoint, currentPoint.Point, nextPoint.Point);
				PointInfo nextPoint2 = item.GetNextPoint(Direction.Backward);
				double num3 = CalculateAngle(previousPoint, currentPoint.Point, nextPoint2.Point);
				if (num2 > num && !visitedPoints.Contains(nextPoint.Point))
				{
					num = num2;
					result = nextPoint;
				}
				if (num3 > num && !visitedPoints.Contains(nextPoint2.Point))
				{
					num = num3;
					result = nextPoint2;
				}
			}
			return result;
		}

		private void DrawMarker(PointF point, float size)
		{
			RectangleF rect = new RectangleF(point.X, point.Y, 0f, 0f);
			rect.Inflate(size / 5f + 2f, size / 5f + 2f);
			graphics.FillEllipse(Brushes.Blue, rect);
		}

		private double CalculateAngle(PointF previousPoint, PointF point, PointF forwardPoint)
		{
			double num = Math.Atan2(previousPoint.Y - point.Y, previousPoint.X - point.X);
			if (num < 0.0)
			{
				num += Math.PI * 2.0;
			}
			double num2 = Math.Atan2(forwardPoint.Y - point.Y, forwardPoint.X - point.X);
			if (num2 < 0.0)
			{
				num2 += Math.PI * 2.0;
			}
			double num3 = num - num2;
			if (num3 < 0.0)
			{
				num3 += Math.PI * 2.0;
			}
			return num3;
		}

		private PointInfo GetClosestPoint(PointF point, PointF[] points, GraphicsPath graphicsPath)
		{
			PointInfo result = default(PointInfo);
			result.Points = points;
			result.Path = graphicsPath;
			double num = double.PositiveInfinity;
			for (int i = 0; i < points.Length; i++)
			{
				double num2 = points[i].X - point.X;
				double num3 = points[i].Y - point.Y;
				double num4 = num2 * num2 + num3 * num3;
				if (num4 < num)
				{
					result.Point = points[i];
					result.Index = i;
					num = num4;
				}
			}
			return result;
		}

		private PointF[] GetPathPoints(GraphicsPath graphicsPath)
		{
			if (pointsTable.Contains(graphicsPath))
			{
				return (PointF[])pointsTable[graphicsPath];
			}
			PointF[] pathPoints = graphicsPath.PathPoints;
			pointsTable.Add(graphicsPath, pathPoints);
			return pathPoints;
		}
	}
}
