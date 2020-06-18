using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class HotRegion : IDisposable
	{
		private GraphicsPath[] paths;

		private RectangleF boundingRectangle = RectangleF.Empty;

		private object selectedObject;

		private PointF circularPinPoint = PointF.Empty;

		private Matrix relMatrix = new Matrix();

		private Matrix absMatrix = new Matrix();

		private PointF[] pointsRect = new PointF[4];

		private PointF[] pointsPoint = new PointF[1];

		protected bool disposed;

		internal GraphicsPath[] Paths
		{
			get
			{
				return paths;
			}
			set
			{
				paths = value;
			}
		}

		internal RectangleF BoundingRectangle
		{
			get
			{
				if (boundingRectangle == RectangleF.Empty && paths.Length != 0)
				{
					RectangleF bounds = paths[0].GetBounds();
					for (int i = 1; i < paths.Length; i++)
					{
						if (paths[i] != null)
						{
							bounds.Intersect(paths[i].GetBounds());
						}
					}
					boundingRectangle = bounds;
				}
				return boundingRectangle;
			}
		}

		internal object SelectedObject
		{
			get
			{
				return selectedObject;
			}
			set
			{
				selectedObject = value;
			}
		}

		internal PointF PinPoint
		{
			get
			{
				return circularPinPoint;
			}
			set
			{
				circularPinPoint = value;
			}
		}

		internal HotRegion()
		{
		}

		internal void BuildMatrices(GaugeGraphics g)
		{
			absMatrix.Reset();
			RectangleF rectangleF = new RectangleF(0f, 0f, 1f, 1f);
			RectangleF absoluteRectangle = g.GetAbsoluteRectangle(rectangleF);
			absMatrix.Translate(g.Transform.OffsetX, g.Transform.OffsetY);
			absMatrix.Scale(absoluteRectangle.Size.Width, absoluteRectangle.Size.Height);
			absoluteRectangle = g.GetRelativeRectangle(rectangleF);
			relMatrix.Reset();
			relMatrix.Scale(absoluteRectangle.Size.Width, absoluteRectangle.Size.Height);
			relMatrix.Translate(0f - g.Transform.OffsetX, 0f - g.Transform.OffsetY);
		}

		internal RectangleF GetAbsoluteRectangle(RectangleF relativeRect)
		{
			return new RectangleF(GetAbsolutePoint(relativeRect.Location), GetAbsoluteSize(relativeRect.Size));
		}

		internal RectangleF GetRelativeRectangle(RectangleF absoluteRect)
		{
			return new RectangleF(GetRelativePoint(absoluteRect.Location), GetRelativeSize(absoluteRect.Size));
		}

		internal PointF GetAbsolutePoint(PointF relativePoint)
		{
			pointsPoint[0] = relativePoint;
			absMatrix.TransformPoints(pointsPoint);
			return pointsPoint[0];
		}

		internal PointF GetRelativePoint(PointF absolutePoint)
		{
			pointsPoint[0] = absolutePoint;
			relMatrix.TransformPoints(pointsPoint);
			return pointsPoint[0];
		}

		public SizeF GetAbsoluteSize(SizeF relativeSize)
		{
			return new SizeF(GetAbsolutePoint(relativeSize.ToPointF()));
		}

		internal SizeF GetRelativeSize(SizeF absoluteSize)
		{
			return new SizeF(GetRelativePoint(absoluteSize.ToPointF()));
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed && disposing)
			{
				if (paths != null)
				{
					GraphicsPath[] array = paths;
					for (int i = 0; i < array.Length; i++)
					{
						array[i]?.Dispose();
					}
					paths = null;
				}
				if (relMatrix != null)
				{
					relMatrix.Dispose();
				}
				if (absMatrix != null)
				{
					absMatrix.Dispose();
				}
			}
			disposed = true;
		}
	}
}
