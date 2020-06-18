using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Map.WebForms
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

		private PointF lastOffset = PointF.Empty;

		private bool doNotDispose;

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
				BoundingRectangle = RectangleF.Empty;
			}
		}

		internal RectangleF BoundingRectangle
		{
			get
			{
				if (boundingRectangle == RectangleF.Empty && paths.Length != 0)
				{
					RectangleF a = paths[0].GetBounds();
					for (int i = 1; i < paths.Length; i++)
					{
						if (paths[i] != null)
						{
							a = RectangleF.Union(a, paths[i].GetBounds());
						}
					}
					boundingRectangle = a;
				}
				return boundingRectangle;
			}
			set
			{
				boundingRectangle = value;
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

		internal Matrix AbsMatrix => absMatrix;

		internal Matrix RelMatrix => relMatrix;

		internal bool DoNotDispose
		{
			get
			{
				return doNotDispose;
			}
			set
			{
				doNotDispose = value;
			}
		}

		internal HotRegion()
		{
		}

		internal void BuildMatrices(MapGraphics g)
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

		internal RectangleF GetAbsRectangle(RectangleF relRect)
		{
			return new RectangleF(GetAbsPoint(relRect.Location), GetAbsSize(relRect.Size));
		}

		internal RectangleF GetRelRectangle(RectangleF absRect)
		{
			return new RectangleF(GetRelPoint(absRect.Location), GetRelSize(absRect.Size));
		}

		internal PointF GetAbsPoint(PointF relPoint)
		{
			pointsPoint[0] = relPoint;
			absMatrix.TransformPoints(pointsPoint);
			return pointsPoint[0];
		}

		internal PointF GetRelPoint(PointF absPoint)
		{
			pointsPoint[0] = absPoint;
			relMatrix.TransformPoints(pointsPoint);
			return pointsPoint[0];
		}

		public SizeF GetAbsSize(SizeF relSize)
		{
			return new SizeF(GetAbsPoint(relSize.ToPointF()));
		}

		internal SizeF GetRelSize(SizeF absSize)
		{
			return new SizeF(GetRelPoint(absSize.ToPointF()));
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed && !DoNotDispose && disposing && paths != null)
			{
				GraphicsPath[] array = paths;
				for (int i = 0; i < array.Length; i++)
				{
					array[i]?.Dispose();
				}
				paths = null;
			}
			disposed = true;
		}

		internal void OffsetBy(PointF sectionOffset)
		{
			PointF pointF = new PointF(sectionOffset.X - lastOffset.X, sectionOffset.Y - lastOffset.Y);
			using (Matrix matrix = new Matrix())
			{
				matrix.Translate(pointF.X, pointF.Y);
				GraphicsPath[] array = Paths;
				for (int i = 0; i < array.Length; i++)
				{
					array[i]?.Transform(matrix);
				}
			}
			lastOffset = sectionOffset;
		}
	}
}
