using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Chart.WebForms
{
	internal class Matrix3D
	{
		private enum RotationAxis
		{
			X,
			Y,
			Z
		}

		private float[][] mainMatrix;

		private float translateX;

		private float translateY;

		private float translateZ;

		private float scale;

		private float shiftX;

		private float shiftY;

		internal float perspective;

		private bool rightAngleAxis;

		private float perspectiveFactor = float.NaN;

		private float perspectiveZ;

		internal float angleX;

		internal float angleY;

		private Point3D[] lightVectors = new Point3D[7];

		private LightStyle lightStyle;

		public bool IsInitialized()
		{
			return mainMatrix != null;
		}

		internal void Initialize(RectangleF innerPlotRectangle, float depth, float angleX, float angleY, float perspective, bool rightAngleAxis)
		{
			Reset();
			translateX = innerPlotRectangle.X + innerPlotRectangle.Width / 2f;
			translateY = innerPlotRectangle.Y + innerPlotRectangle.Height / 2f;
			translateZ = depth / 2f;
			float width = innerPlotRectangle.Width;
			float height = innerPlotRectangle.Height;
			this.perspective = perspective;
			this.rightAngleAxis = rightAngleAxis;
			this.angleX = angleX;
			this.angleY = angleY;
			angleX = angleX / 180f * (float)Math.PI;
			angleY = angleY / 180f * (float)Math.PI;
			Point3D[] array = Set3DBarPoints(width, height, depth);
			Translate(translateX, translateY, 0f);
			if (!rightAngleAxis)
			{
				Rotate(angleX, RotationAxis.X);
				Rotate(angleY, RotationAxis.Y);
			}
			else if (this.angleY >= 45f)
			{
				Rotate(Math.PI / 2.0, RotationAxis.Y);
			}
			else if (this.angleY <= -45f)
			{
				Rotate(-Math.PI / 2.0, RotationAxis.Y);
			}
			GetValues(array);
			float num = float.MinValue;
			Point3D[] array2;
			if (perspective != 0f || rightAngleAxis)
			{
				array2 = array;
				foreach (Point3D point3D in array2)
				{
					if (point3D.Z > num)
					{
						num = point3D.Z;
					}
				}
				perspectiveZ = num;
			}
			if (perspective != 0f)
			{
				perspectiveFactor = perspective / 2000f;
				Perspective(array);
			}
			if (rightAngleAxis)
			{
				RightAngleProjection(array);
				float num2 = 0f;
				float num3 = 0f;
				float num4 = 0f;
				float num5 = 0f;
				array2 = array;
				foreach (Point3D point3D2 in array2)
				{
					if (point3D2.X - translateX < 0f && Math.Abs(point3D2.X - translateX) > num2)
					{
						num2 = Math.Abs(point3D2.X - translateX);
					}
					if (point3D2.X - translateX >= 0f && Math.Abs(point3D2.X - translateX) > num4)
					{
						num4 = Math.Abs(point3D2.X - translateX);
					}
					if (point3D2.Y - translateY < 0f && Math.Abs(point3D2.Y - translateY) > num3)
					{
						num3 = Math.Abs(point3D2.Y - translateY);
					}
					if (point3D2.Y - translateY >= 0f && Math.Abs(point3D2.Y - translateY) > num5)
					{
						num5 = Math.Abs(point3D2.Y - translateY);
					}
				}
				shiftX = (num4 - num2) / 2f;
				shiftY = (num5 - num3) / 2f;
				RightAngleShift(array);
			}
			float num6 = float.MinValue;
			float num7 = float.MinValue;
			array2 = array;
			foreach (Point3D point3D3 in array2)
			{
				if (num6 < Math.Abs(point3D3.X - translateX) / width * 2f)
				{
					num6 = Math.Abs(point3D3.X - translateX) / width * 2f;
				}
				if (num7 < Math.Abs(point3D3.Y - translateY) / height * 2f)
				{
					num7 = Math.Abs(point3D3.Y - translateY) / height * 2f;
				}
			}
			scale = ((num7 > num6) ? num7 : num6);
			Scale(array);
		}

		public void TransformPoints(Point3D[] points)
		{
			TransformPoints(points, withPerspective: true);
		}

		private void TransformPoints(Point3D[] points, bool withPerspective)
		{
			if (mainMatrix == null)
			{
				throw new InvalidOperationException(SR.ExceptionMatrix3DNotinitialized);
			}
			foreach (Point3D obj in points)
			{
				obj.X -= translateX;
				obj.Y -= translateY;
				obj.Z -= translateZ;
			}
			GetValues(points);
			if (perspective != 0f && withPerspective)
			{
				Perspective(points);
			}
			if (rightAngleAxis)
			{
				RightAngleProjection(points);
				RightAngleShift(points);
			}
			Scale(points);
		}

		private void RightAngleShift(Point3D[] points)
		{
			foreach (Point3D obj in points)
			{
				obj.X -= shiftX;
				obj.Y -= shiftY;
			}
		}

		private void RightAngleProjection(Point3D[] points)
		{
			float num = 45f;
			float num2 = angleX / 45f;
			float num3 = (angleY >= 45f) ? ((angleY - 90f) / num) : ((!(angleY <= -45f)) ? (angleY / num) : ((angleY + 90f) / num));
			foreach (Point3D point3D in points)
			{
				point3D.X += (perspectiveZ - point3D.Z) * num3;
				point3D.Y -= (perspectiveZ - point3D.Z) * num2;
			}
		}

		private void Perspective(Point3D[] points)
		{
			foreach (Point3D point3D in points)
			{
				point3D.X = translateX + (point3D.X - translateX) / (1f + (perspectiveZ - point3D.Z) * perspectiveFactor);
				point3D.Y = translateY + (point3D.Y - translateY) / (1f + (perspectiveZ - point3D.Z) * perspectiveFactor);
			}
		}

		private void Scale(Point3D[] points)
		{
			foreach (Point3D point3D in points)
			{
				point3D.X = translateX + (point3D.X - translateX) / scale;
				point3D.Y = translateY + (point3D.Y - translateY) / scale;
			}
		}

		private void Translate(float dx, float dy, float dz)
		{
			float[][] array = new float[4][]
			{
				new float[4],
				new float[4],
				new float[4],
				new float[4]
			};
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					if (i == j)
					{
						array[i][j] = 1f;
					}
					else
					{
						array[i][j] = 0f;
					}
				}
			}
			array[0][3] = dx;
			array[1][3] = dy;
			array[2][3] = dz;
			Multiply(array, MatrixOrder.Prepend, setMainMatrix: true);
		}

		private void Reset()
		{
			mainMatrix = new float[4][];
			mainMatrix[0] = new float[4];
			mainMatrix[1] = new float[4];
			mainMatrix[2] = new float[4];
			mainMatrix[3] = new float[4];
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					if (i == j)
					{
						mainMatrix[i][j] = 1f;
					}
					else
					{
						mainMatrix[i][j] = 0f;
					}
				}
			}
		}

		private float[][] Multiply(float[][] mulMatrix, MatrixOrder order, bool setMainMatrix)
		{
			float[][] array = new float[4][]
			{
				new float[4],
				new float[4],
				new float[4],
				new float[4]
			};
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					array[i][j] = 0f;
					for (int k = 0; k < 4; k++)
					{
						if (order == MatrixOrder.Prepend)
						{
							array[i][j] += mainMatrix[i][k] * mulMatrix[k][j];
						}
						else
						{
							array[i][j] += mulMatrix[i][k] * mainMatrix[k][j];
						}
					}
				}
			}
			if (setMainMatrix)
			{
				mainMatrix = array;
			}
			return array;
		}

		private void MultiplyVector(float[] mulVector, ref float[] resultVector)
		{
			for (int i = 0; i < 3; i++)
			{
				resultVector[i] = 0f;
				for (int j = 0; j < 4; j++)
				{
					resultVector[i] += mainMatrix[i][j] * mulVector[j];
				}
			}
		}

		private void Rotate(double angle, RotationAxis axis)
		{
			float[][] array = new float[4][]
			{
				new float[4],
				new float[4],
				new float[4],
				new float[4]
			};
			angle = -1.0 * angle;
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					if (i == j)
					{
						array[i][j] = 1f;
					}
					else
					{
						array[i][j] = 0f;
					}
				}
			}
			switch (axis)
			{
			case RotationAxis.X:
				array[1][1] = (float)Math.Cos(angle);
				array[1][2] = (float)(0.0 - Math.Sin(angle));
				array[2][1] = (float)Math.Sin(angle);
				array[2][2] = (float)Math.Cos(angle);
				break;
			case RotationAxis.Y:
				array[0][0] = (float)Math.Cos(angle);
				array[0][2] = (float)Math.Sin(angle);
				array[2][0] = (float)(0.0 - Math.Sin(angle));
				array[2][2] = (float)Math.Cos(angle);
				break;
			case RotationAxis.Z:
				array[0][0] = (float)Math.Cos(angle);
				array[0][1] = (float)(0.0 - Math.Sin(angle));
				array[1][0] = (float)Math.Sin(angle);
				array[1][1] = (float)Math.Cos(angle);
				break;
			}
			Multiply(array, MatrixOrder.Prepend, setMainMatrix: true);
		}

		private void GetValues(Point3D[] points)
		{
			float[] array = new float[4];
			float[] resultVector = new float[4];
			foreach (Point3D point3D in points)
			{
				array[0] = point3D.X;
				array[1] = point3D.Y;
				array[2] = point3D.Z;
				array[3] = 1f;
				MultiplyVector(array, ref resultVector);
				point3D.X = resultVector[0];
				point3D.Y = resultVector[1];
				point3D.Z = resultVector[2];
			}
		}

		private Point3D[] Set3DBarPoints(float dx, float dy, float dz)
		{
			return new Point3D[8]
			{
				new Point3D((0f - dx) / 2f, (0f - dy) / 2f, dz / 2f),
				new Point3D(dx / 2f, (0f - dy) / 2f, dz / 2f),
				new Point3D(dx / 2f, dy / 2f, dz / 2f),
				new Point3D((0f - dx) / 2f, dy / 2f, dz / 2f),
				new Point3D((0f - dx) / 2f, (0f - dy) / 2f, (0f - dz) / 2f),
				new Point3D(dx / 2f, (0f - dy) / 2f, (0f - dz) / 2f),
				new Point3D(dx / 2f, dy / 2f, (0f - dz) / 2f),
				new Point3D((0f - dx) / 2f, dy / 2f, (0f - dz) / 2f)
			};
		}

		internal void InitLight(LightStyle lightStyle)
		{
			this.lightStyle = lightStyle;
			lightVectors[0] = new Point3D(0f, 0f, 0f);
			lightVectors[1] = new Point3D(0f, 0f, 1f);
			lightVectors[2] = new Point3D(0f, 0f, -1f);
			lightVectors[3] = new Point3D(-1f, 0f, 0f);
			lightVectors[4] = new Point3D(1f, 0f, 0f);
			lightVectors[5] = new Point3D(0f, -1f, 0f);
			lightVectors[6] = new Point3D(0f, 1f, 0f);
			TransformPoints(lightVectors, withPerspective: false);
			lightVectors[1].X -= lightVectors[0].X;
			lightVectors[1].Y -= lightVectors[0].Y;
			lightVectors[1].Z -= lightVectors[0].Z;
			lightVectors[2].X -= lightVectors[0].X;
			lightVectors[2].Y -= lightVectors[0].Y;
			lightVectors[2].Z -= lightVectors[0].Z;
			lightVectors[3].X -= lightVectors[0].X;
			lightVectors[3].Y -= lightVectors[0].Y;
			lightVectors[3].Z -= lightVectors[0].Z;
			lightVectors[4].X -= lightVectors[0].X;
			lightVectors[4].Y -= lightVectors[0].Y;
			lightVectors[4].Z -= lightVectors[0].Z;
			lightVectors[5].X -= lightVectors[0].X;
			lightVectors[5].Y -= lightVectors[0].Y;
			lightVectors[5].Z -= lightVectors[0].Z;
			lightVectors[6].X -= lightVectors[0].X;
			lightVectors[6].Y -= lightVectors[0].Y;
			lightVectors[6].Z -= lightVectors[0].Z;
		}

		internal void GetLight(Color surfaceColor, out Color front, out Color back, out Color left, out Color right, out Color top, out Color bottom)
		{
			switch (lightStyle)
			{
			case LightStyle.None:
				front = surfaceColor;
				left = surfaceColor;
				top = surfaceColor;
				back = surfaceColor;
				right = surfaceColor;
				bottom = surfaceColor;
				return;
			case LightStyle.Simplistic:
				front = surfaceColor;
				left = ChartGraphics.GetGradientColor(surfaceColor, Color.Black, 0.25);
				top = ChartGraphics.GetGradientColor(surfaceColor, Color.Black, 0.15);
				back = surfaceColor;
				right = ChartGraphics.GetGradientColor(surfaceColor, Color.Black, 0.25);
				bottom = ChartGraphics.GetGradientColor(surfaceColor, Color.Black, 0.15);
				return;
			}
			if (rightAngleAxis)
			{
				Point3D point3D = new Point3D(0f, 0f, -1f);
				RightAngleProjection(new Point3D[1]
				{
					point3D
				});
				if (angleY >= 45f || angleY <= -45f)
				{
					front = ChartGraphics.GetGradientColor(surfaceColor, Color.Black, (double)GetAngle(point3D, lightVectors[1]) / Math.PI);
					back = ChartGraphics.GetGradientColor(surfaceColor, Color.Black, (double)GetAngle(point3D, lightVectors[2]) / Math.PI);
					left = ChartGraphics.GetGradientColor(surfaceColor, Color.Black, 0.0);
					right = ChartGraphics.GetGradientColor(surfaceColor, Color.Black, 0.0);
				}
				else
				{
					front = ChartGraphics.GetGradientColor(surfaceColor, Color.Black, 0.0);
					back = ChartGraphics.GetGradientColor(surfaceColor, Color.Black, 1.0);
					left = ChartGraphics.GetGradientColor(surfaceColor, Color.Black, (double)GetAngle(point3D, lightVectors[3]) / Math.PI);
					right = ChartGraphics.GetGradientColor(surfaceColor, Color.Black, (double)GetAngle(point3D, lightVectors[4]) / Math.PI);
				}
				top = ChartGraphics.GetGradientColor(surfaceColor, Color.Black, (double)GetAngle(point3D, lightVectors[5]) / Math.PI);
				bottom = ChartGraphics.GetGradientColor(surfaceColor, Color.Black, (double)GetAngle(point3D, lightVectors[6]) / Math.PI);
			}
			else
			{
				Point3D a = new Point3D(0f, 0f, 1f);
				front = GetBrightGradientColor(surfaceColor, (double)GetAngle(a, lightVectors[1]) / Math.PI);
				back = GetBrightGradientColor(surfaceColor, (double)GetAngle(a, lightVectors[2]) / Math.PI);
				left = GetBrightGradientColor(surfaceColor, (double)GetAngle(a, lightVectors[3]) / Math.PI);
				right = GetBrightGradientColor(surfaceColor, (double)GetAngle(a, lightVectors[4]) / Math.PI);
				top = GetBrightGradientColor(surfaceColor, (double)GetAngle(a, lightVectors[5]) / Math.PI);
				bottom = GetBrightGradientColor(surfaceColor, (double)GetAngle(a, lightVectors[6]) / Math.PI);
			}
		}

		internal Color GetPolygonLight(Point3D[] points, Color surfaceColor, bool visiblePolygon, float yAngle, SurfaceNames surfaceName, bool switchSeriesOrder)
		{
			Color result = surfaceColor;
			Point3D point3D = new Point3D(0f, 0f, 1f);
			switch (lightStyle)
			{
			case LightStyle.Simplistic:
			{
				Point3D point3D5 = new Point3D();
				point3D5.X = points[0].X - points[1].X;
				point3D5.Y = points[0].Y - points[1].Y;
				point3D5.Z = points[0].Z - points[1].Z;
				Point3D point3D6 = new Point3D();
				point3D6.X = points[2].X - points[1].X;
				point3D6.Y = points[2].Y - points[1].Y;
				point3D6.Z = points[2].Z - points[1].Z;
				Point3D point3D7 = new Point3D();
				point3D7.X = point3D5.Y * point3D6.Z - point3D5.Z * point3D6.Y;
				point3D7.Y = point3D5.Z * point3D6.X - point3D5.X * point3D6.Z;
				point3D7.Z = point3D5.X * point3D6.Y - point3D5.Y * point3D6.X;
				switch (surfaceName)
				{
				case SurfaceNames.Left:
					result = ChartGraphics.GetGradientColor(surfaceColor, Color.Black, 0.15);
					break;
				case SurfaceNames.Right:
					result = ChartGraphics.GetGradientColor(surfaceColor, Color.Black, 0.15);
					break;
				case SurfaceNames.Front:
					result = surfaceColor;
					break;
				case SurfaceNames.Back:
					result = surfaceColor;
					break;
				default:
				{
					float angle;
					float angle2;
					if (switchSeriesOrder)
					{
						if (yAngle > 0f && yAngle <= 90f)
						{
							angle = GetAngle(point3D7, lightVectors[3]);
							angle2 = GetAngle(point3D7, lightVectors[4]);
						}
						else
						{
							angle = GetAngle(point3D7, lightVectors[4]);
							angle2 = GetAngle(point3D7, lightVectors[3]);
						}
					}
					else if (yAngle > 0f && yAngle <= 90f)
					{
						angle = GetAngle(point3D7, lightVectors[4]);
						angle2 = GetAngle(point3D7, lightVectors[3]);
					}
					else
					{
						angle = GetAngle(point3D7, lightVectors[3]);
						angle2 = GetAngle(point3D7, lightVectors[4]);
					}
					result = ((!((double)Math.Abs(angle - angle2) < 0.01)) ? ((!(angle < angle2)) ? ChartGraphics.GetGradientColor(surfaceColor, Color.Black, 0.15) : ChartGraphics.GetGradientColor(surfaceColor, Color.Black, 0.25)) : ChartGraphics.GetGradientColor(surfaceColor, Color.Black, 0.25));
					break;
				}
				}
				break;
			}
			default:
			{
				Point3D point3D2 = new Point3D();
				point3D2.X = points[0].X - points[1].X;
				point3D2.Y = points[0].Y - points[1].Y;
				point3D2.Z = points[0].Z - points[1].Z;
				Point3D point3D3 = new Point3D();
				point3D3.X = points[2].X - points[1].X;
				point3D3.Y = points[2].Y - points[1].Y;
				point3D3.Z = points[2].Z - points[1].Z;
				Point3D point3D4 = new Point3D();
				point3D4.X = point3D2.Y * point3D3.Z - point3D2.Z * point3D3.Y;
				point3D4.Y = point3D2.Z * point3D3.X - point3D2.X * point3D3.Z;
				point3D4.Z = point3D2.X * point3D3.Y - point3D2.Y * point3D3.X;
				switch (surfaceName)
				{
				case SurfaceNames.Front:
					point3D.Z *= -1f;
					result = GetBrightGradientColor(surfaceColor, (double)GetAngle(point3D, lightVectors[2]) / Math.PI);
					break;
				case SurfaceNames.Back:
					point3D.Z *= -1f;
					result = GetBrightGradientColor(surfaceColor, (double)GetAngle(point3D, lightVectors[1]) / Math.PI);
					break;
				default:
					if (visiblePolygon)
					{
						point3D.Z *= -1f;
					}
					result = GetBrightGradientColor(surfaceColor, (double)GetAngle(point3D, point3D4) / Math.PI);
					break;
				}
				break;
			}
			case LightStyle.None:
				break;
			}
			return result;
		}

		private Color GetBrightGradientColor(Color beginColor, double position)
		{
			position *= 2.0;
			double num = 0.5;
			if (position < num)
			{
				return ChartGraphics.GetGradientColor(Color.FromArgb(beginColor.A, 255, 255, 255), beginColor, 1.0 - num + position);
			}
			if (0.0 - num + position < 1.0)
			{
				return ChartGraphics.GetGradientColor(beginColor, Color.Black, 0.0 - num + position);
			}
			return Color.FromArgb(beginColor.A, 0, 0, 0);
		}

		private float GetAngle(Point3D a, Point3D b)
		{
			return (float)Math.Acos((double)(a.X * b.X + a.Y * b.Y + a.Z * b.Z) / (Math.Sqrt(a.X * a.X + a.Y * a.Y + a.Z * a.Z) * Math.Sqrt(b.X * b.X + b.Y * b.Y + b.Z * b.Z)));
		}
	}
}
