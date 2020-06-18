using Microsoft.Reporting.Chart.WebForms.ChartTypes;
using Microsoft.Reporting.Chart.WebForms.Design;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms
{
	internal class ChartArea3D : ChartAreaAxes
	{
		internal class PointsDrawingOrderComparer : IComparer
		{
			private ChartArea area;

			private Point3D areaProjectionCenter = new Point3D(float.NaN, float.NaN, float.NaN);

			private bool selection;

			public PointsDrawingOrderComparer(ChartArea area, bool selection, COPCoordinates coord)
			{
				this.area = area;
				this.selection = selection;
				if (area.DrawPointsToCenter(ref coord))
				{
					areaProjectionCenter = area.GetCenterOfProjection(coord);
				}
			}

			public int Compare(object o1, object o2)
			{
				DataPoint3D dataPoint3D = (DataPoint3D)o1;
				DataPoint3D dataPoint3D2 = (DataPoint3D)o2;
				int num = 0;
				if (dataPoint3D.xPosition < dataPoint3D2.xPosition)
				{
					num = -1;
				}
				else if (dataPoint3D.xPosition > dataPoint3D2.xPosition)
				{
					num = 1;
				}
				else
				{
					if (dataPoint3D.yPosition < dataPoint3D2.yPosition)
					{
						num = 1;
					}
					else if (dataPoint3D.yPosition > dataPoint3D2.yPosition)
					{
						num = -1;
					}
					if (!float.IsNaN(areaProjectionCenter.Y))
					{
						double num2 = Math.Min(dataPoint3D.yPosition, dataPoint3D.height);
						double num3 = Math.Max(dataPoint3D.yPosition, dataPoint3D.height);
						double num4 = Math.Min(dataPoint3D2.yPosition, dataPoint3D2.height);
						double num5 = Math.Max(dataPoint3D2.yPosition, dataPoint3D2.height);
						if (!area.IsBottomSceneWallVisible())
						{
							num = ((num3 >= (double)areaProjectionCenter.Y && num5 >= (double)areaProjectionCenter.Y) ? num : ((num3 >= (double)areaProjectionCenter.Y) ? 1 : (num * -1)));
						}
						else if (num2 <= (double)areaProjectionCenter.Y && num4 <= (double)areaProjectionCenter.Y)
						{
							num *= -1;
						}
						else if (num2 <= (double)areaProjectionCenter.Y)
						{
							num = 1;
						}
					}
					else if (!area.IsBottomSceneWallVisible())
					{
						num *= -1;
					}
				}
				if (dataPoint3D.xPosition != dataPoint3D2.xPosition)
				{
					if (!float.IsNaN(areaProjectionCenter.X))
					{
						if (dataPoint3D.xPosition + dataPoint3D.width / 2.0 >= (double)areaProjectionCenter.X && dataPoint3D2.xPosition + dataPoint3D2.width / 2.0 >= (double)areaProjectionCenter.X)
						{
							num *= -1;
						}
					}
					else if (area.DrawPointsInReverseOrder())
					{
						num *= -1;
					}
				}
				if (!selection)
				{
					return num;
				}
				return -num;
			}
		}

		private ChartArea3DStyle area3DStyle = new ChartArea3DStyle();

		internal Matrix3D matrix3D = new Matrix3D();

		internal SizeF areaSceneWallWidth = SizeF.Empty;

		internal float areaSceneDepth;

		private SurfaceNames visibleSurfaces;

		private double pointsDepth;

		private double pointsGapDepth;

		internal bool reverseSeriesOrder;

		internal bool oldReverseX;

		internal bool oldReverseY;

		internal int oldYAngle = 30;

		internal ArrayList seriesDrawingOrder;

		internal ArrayList stackGroupNames;

		internal ArrayList seriesClusters;

		[SRCategory("CategoryAttribute3D")]
		[Bindable(true)]
		[DefaultValue(null)]
		[SRDescription("DescriptionAttributeArea3DStyle")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		public ChartArea3DStyle Area3DStyle
		{
			get
			{
				return area3DStyle;
			}
			set
			{
				area3DStyle = value;
				area3DStyle.Initialize((ChartArea)this);
			}
		}

		public ChartArea3D()
		{
			area3DStyle.Initialize((ChartArea)this);
		}

		public void TransformPoints(Point3D[] points)
		{
			foreach (Point3D obj in points)
			{
				obj.Z = obj.Z / 100f * areaSceneDepth;
			}
			matrix3D.TransformPoints(points);
		}

		protected void DrawArea3DScene(ChartGraphics graph, RectangleF position)
		{
			ChartArea chartArea = (ChartArea)this;
			areaSceneWallWidth = graph.GetRelativeSize(new SizeF(Area3DStyle.WallWidth, Area3DStyle.WallWidth));
			areaSceneDepth = GetArea3DSceneDepth();
			matrix3D.Initialize(position, areaSceneDepth, Area3DStyle.XAngle, Area3DStyle.YAngle, Area3DStyle.Perspective, Area3DStyle.RightAngleAxes);
			matrix3D.InitLight(Area3DStyle.Light);
			visibleSurfaces = graph.GetVisibleSurfaces(position, 0f, areaSceneDepth, matrix3D);
			Color color = chartArea.BackColor;
			if (color == Color.Transparent)
			{
				areaSceneWallWidth = SizeF.Empty;
				return;
			}
			if (color == Color.Empty)
			{
				color = Color.LightGray;
			}
			if (IsBottomSceneWallVisible())
			{
				position.Height += areaSceneWallWidth.Height;
			}
			position.Width += areaSceneWallWidth.Width;
			if (Area3DStyle.YAngle > 0)
			{
				position.X -= areaSceneWallWidth.Width;
			}
			RectangleF position2 = new RectangleF(position.Location, position.Size);
			float width = areaSceneWallWidth.Width;
			float positionZ = 0f - width;
			if (IsMainSceneWallOnFront())
			{
				positionZ = areaSceneDepth;
			}
			graph.Fill3DRectangle(position2, positionZ, width, matrix3D, chartArea.Area3DStyle.Light, color, chartArea.BackHatchStyle, chartArea.BackImage, chartArea.BackImageMode, chartArea.BackImageTransparentColor, chartArea.BackImageAlign, chartArea.BackGradientType, chartArea.BackGradientEndColor, chartArea.BorderColor, chartArea.BorderWidth, chartArea.BorderStyle, PenAlignment.Outset, DrawingOperationTypes.DrawElement);
			position2 = new RectangleF(position.Location, position.Size);
			position2.Width = areaSceneWallWidth.Width;
			if (!IsSideSceneWallOnLeft())
			{
				position2.X = position.Right - areaSceneWallWidth.Width;
			}
			graph.Fill3DRectangle(position2, 0f, areaSceneDepth, matrix3D, chartArea.Area3DStyle.Light, color, chartArea.BackHatchStyle, chartArea.BackImage, chartArea.BackImageMode, chartArea.BackImageTransparentColor, chartArea.BackImageAlign, chartArea.BackGradientType, chartArea.BackGradientEndColor, chartArea.BorderColor, chartArea.BorderWidth, chartArea.BorderStyle, PenAlignment.Outset, DrawingOperationTypes.DrawElement);
			if (IsBottomSceneWallVisible())
			{
				position2 = new RectangleF(position.Location, position.Size);
				position2.Height = areaSceneWallWidth.Height;
				position2.Y = position.Bottom - areaSceneWallWidth.Height;
				position2.Width -= areaSceneWallWidth.Width;
				if (IsSideSceneWallOnLeft())
				{
					position2.X += areaSceneWallWidth.Width;
				}
				positionZ = 0f;
				graph.Fill3DRectangle(position2, 0f, areaSceneDepth, matrix3D, chartArea.Area3DStyle.Light, color, chartArea.BackHatchStyle, chartArea.BackImage, chartArea.BackImageMode, chartArea.BackImageTransparentColor, chartArea.BackImageAlign, chartArea.BackGradientType, chartArea.BackGradientEndColor, chartArea.BorderColor, chartArea.BorderWidth, chartArea.BorderStyle, PenAlignment.Outset, DrawingOperationTypes.DrawElement);
			}
		}

		internal bool IsBottomSceneWallVisible()
		{
			return Area3DStyle.XAngle >= 0;
		}

		internal bool IsMainSceneWallOnFront()
		{
			return false;
		}

		internal bool IsSideSceneWallOnLeft()
		{
			return Area3DStyle.YAngle > 0;
		}

		public float GetSeriesZPosition(Series series)
		{
			GetSeriesZPositionAndDepth(series, out float depth, out float positionZ);
			return (positionZ + depth / 2f) / areaSceneDepth * 100f;
		}

		public float GetSeriesDepth(Series series)
		{
			GetSeriesZPositionAndDepth(series, out float depth, out float _);
			return depth / areaSceneDepth * 100f;
		}

		private float GetArea3DSceneDepth()
		{
			bool num = IndexedSeries((string[])base.series.ToArray(typeof(string)));
			Series series = null;
			if (base.series.Count > 0)
			{
				series = base.Common.DataManager.Series[(string)base.series[0]];
			}
			Axis axis = ((ChartArea)this).AxisX;
			if (base.series.Count > 0)
			{
				Series series2 = base.Common.DataManager.Series[base.series[0]];
				if (series2 != null && series2.XAxisType == AxisType.Secondary)
				{
					axis = ((ChartArea)this).AxisX2;
				}
			}
			double num2 = 1.0;
			if (!num)
			{
				num2 = GetPointsInterval(base.series, axis.Logarithmic, axis.logarithmBase, checkSameInterval: false, out bool _, out series);
			}
			bool flag = false;
			if (series != null)
			{
				flag = base.Common.ChartTypeRegistry.GetChartType(series.ChartTypeName).SideBySideSeries;
				foreach (string item in base.series)
				{
					if (base.Common.DataManager.Series[item].IsAttributeSet("DrawSideBySide"))
					{
						string strA = base.Common.DataManager.Series[item]["DrawSideBySide"];
						if (string.Compare(strA, "False", StringComparison.OrdinalIgnoreCase) == 0)
						{
							flag = false;
						}
						else if (string.Compare(strA, "True", StringComparison.OrdinalIgnoreCase) == 0)
						{
							flag = true;
						}
						else if (string.Compare(strA, "Auto", StringComparison.OrdinalIgnoreCase) != 0)
						{
							throw new InvalidOperationException(SR.ExceptionAttributeDrawSideBySideInvalid);
						}
					}
				}
			}
			Axis axis2 = ((ChartArea)this).AxisX;
			if (series != null && series.XAxisType == AxisType.Secondary)
			{
				axis2 = ((ChartArea)this).AxisX2;
			}
			double num3 = 0.8;
			int num4 = 1;
			if (series != null && Area3DStyle.Clustered && flag)
			{
				num4 = 0;
				foreach (string item2 in base.series)
				{
					if (string.Compare(base.Common.DataManager.Series[item2].ChartTypeName, series.ChartTypeName, StringComparison.OrdinalIgnoreCase) == 0)
					{
						num4++;
					}
				}
			}
			if (series != null && Area3DStyle.Clustered && base.Common.ChartTypeRegistry.GetChartType(series.ChartTypeName).SupportStackedGroups)
			{
				_ = string.Empty;
				if (series.IsAttributeSet("StackedGroupName"))
				{
					_ = series["StackedGroupName"];
				}
				num4 = 0;
				ArrayList arrayList = new ArrayList();
				foreach (string item3 in base.series)
				{
					Series series3 = base.Common.DataManager.Series[item3];
					if (string.Compare(series3.ChartTypeName, series.ChartTypeName, StringComparison.OrdinalIgnoreCase) == 0)
					{
						string text = string.Empty;
						if (series3.IsAttributeSet("StackedGroupName"))
						{
							text = series3["StackedGroupName"];
						}
						if (!arrayList.Contains(text))
						{
							arrayList.Add(text);
						}
					}
				}
				num4 = arrayList.Count;
			}
			pointsDepth = num2 * num3 * (double)Area3DStyle.PointDepth / 100.0;
			pointsDepth = axis2.GetPixelInterval(pointsDepth);
			if (series != null)
			{
				pointsDepth = series.GetPointWidth(base.Common.graph, axis2, num2, 0.8) / (double)num4;
				pointsDepth *= (double)Area3DStyle.PointDepth / 100.0;
			}
			pointsGapDepth = pointsDepth * 0.8 * (double)Area3DStyle.PointGapDepth / 100.0;
			series?.GetPointDepthAndGap(base.Common.graph, axis2, ref pointsDepth, ref pointsGapDepth);
			return (float)((pointsGapDepth + pointsDepth) * (double)GetNumberOfClusters());
		}

		internal void GetSeriesZPositionAndDepth(Series series, out float depth, out float positionZ)
		{
			int seriesClusterIndex = GetSeriesClusterIndex(series);
			depth = (float)pointsDepth;
			positionZ = (float)(pointsGapDepth / 2.0 + (pointsDepth + pointsGapDepth) * (double)seriesClusterIndex);
		}

		internal int GetNumberOfClusters()
		{
			if (seriesClusters == null)
			{
				ArrayList arrayList = new ArrayList();
				ArrayList arrayList2 = new ArrayList();
				seriesClusters = new ArrayList();
				int num = -1;
				foreach (string item in base.series)
				{
					Series series = base.Common.DataManager.Series[item];
					if (!Area3DStyle.Clustered && base.Common.ChartTypeRegistry.GetChartType(series.ChartTypeName).SupportStackedGroups)
					{
						string seriesStackGroupName = StackedColumnChart.GetSeriesStackGroupName(series);
						if (arrayList2.Contains(seriesStackGroupName))
						{
							bool flag = false;
							int num2 = 0;
							while (!flag && num2 < seriesClusters.Count)
							{
								foreach (string item2 in (ArrayList)seriesClusters[num2])
								{
									Series series2 = base.Common.DataManager.Series[item2];
									if (seriesStackGroupName == StackedColumnChart.GetSeriesStackGroupName(series2))
									{
										num = num2;
										flag = true;
									}
								}
								num2++;
							}
						}
						else
						{
							num = seriesClusters.Count;
							arrayList2.Add(seriesStackGroupName);
						}
					}
					else if (base.Common.ChartTypeRegistry.GetChartType(series.ChartTypeName).Stacked || (Area3DStyle.Clustered && base.Common.ChartTypeRegistry.GetChartType(series.ChartTypeName).SideBySideSeries))
					{
						if (arrayList.Contains(series.ChartTypeName.ToUpper(CultureInfo.InvariantCulture)))
						{
							bool flag2 = false;
							int num3 = 0;
							while (!flag2 && num3 < seriesClusters.Count)
							{
								foreach (string item3 in (ArrayList)seriesClusters[num3])
								{
									if (base.Common.DataManager.Series[item3].ChartTypeName.ToUpper(CultureInfo.InvariantCulture) == series.ChartTypeName.ToUpper(CultureInfo.InvariantCulture))
									{
										num = num3;
										flag2 = true;
									}
								}
								num3++;
							}
						}
						else
						{
							num = seriesClusters.Count;
							arrayList.Add(series.ChartTypeName.ToUpper(CultureInfo.InvariantCulture));
						}
					}
					else
					{
						num = seriesClusters.Count;
					}
					if (seriesClusters.Count <= num)
					{
						seriesClusters.Add(new ArrayList());
					}
					((ArrayList)seriesClusters[num]).Add(item);
				}
			}
			return seriesClusters.Count;
		}

		internal int GetSeriesClusterIndex(Series series)
		{
			if (seriesClusters == null)
			{
				GetNumberOfClusters();
			}
			for (int i = 0; i < seriesClusters.Count; i++)
			{
				foreach (string item in (ArrayList)seriesClusters[i])
				{
					if (item == series.Name)
					{
						if (reverseSeriesOrder)
						{
							i = seriesClusters.Count - 1 - i;
						}
						return i;
					}
				}
			}
			return 0;
		}

		private float GetEstimatedSceneDepth()
		{
			ChartArea chartArea = (ChartArea)this;
			seriesClusters = null;
			ElementPosition innerPlotPosition = chartArea.InnerPlotPosition;
			chartArea.AxisX.PlotAreaPosition = chartArea.Position;
			chartArea.AxisY.PlotAreaPosition = chartArea.Position;
			chartArea.AxisX2.PlotAreaPosition = chartArea.Position;
			chartArea.AxisY2.PlotAreaPosition = chartArea.Position;
			float area3DSceneDepth = GetArea3DSceneDepth();
			chartArea.AxisX.PlotAreaPosition = innerPlotPosition;
			chartArea.AxisY.PlotAreaPosition = innerPlotPosition;
			chartArea.AxisX2.PlotAreaPosition = innerPlotPosition;
			chartArea.AxisY2.PlotAreaPosition = innerPlotPosition;
			return area3DSceneDepth;
		}

		internal void Estimate3DInterval(ChartGraphics graph)
		{
			_ = (ChartArea)this;
			areaSceneWallWidth = graph.GetRelativeSize(new SizeF(Area3DStyle.WallWidth, Area3DStyle.WallWidth));
			ChartArea chartArea = (ChartArea)this;
			areaSceneDepth = GetEstimatedSceneDepth();
			RectangleF innerPlotRectangle = chartArea.Position.ToRectangleF();
			if (base.PlotAreaPosition.Width == 0f && base.PlotAreaPosition.Height == 0f && !chartArea.InnerPlotPosition.Auto && !chartArea.Position.Auto && !chartArea.InnerPlotPosition.Auto)
			{
				innerPlotRectangle.X += chartArea.Position.Width / 100f * chartArea.InnerPlotPosition.X;
				innerPlotRectangle.Y += chartArea.Position.Height / 100f * chartArea.InnerPlotPosition.Y;
				innerPlotRectangle.Width = chartArea.Position.Width / 100f * chartArea.InnerPlotPosition.Width;
				innerPlotRectangle.Height = chartArea.Position.Height / 100f * chartArea.InnerPlotPosition.Height;
			}
			int realYAngle = GetRealYAngle();
			Matrix3D matrix3D = new Matrix3D();
			matrix3D.Initialize(innerPlotRectangle, areaSceneDepth, Area3DStyle.XAngle, realYAngle, Area3DStyle.Perspective, Area3DStyle.RightAngleAxes);
			Point3D[] array = new Point3D[8];
			bool axisOnEdge;
			if (chartArea.switchValueAxes)
			{
				float marksZPosition = axisX.GetMarksZPosition(out axisOnEdge);
				array[0] = new Point3D(innerPlotRectangle.X, innerPlotRectangle.Y, marksZPosition);
				array[1] = new Point3D(innerPlotRectangle.X, innerPlotRectangle.Bottom, marksZPosition);
				marksZPosition = axisY.GetMarksZPosition(out axisOnEdge);
				array[2] = new Point3D(innerPlotRectangle.X, innerPlotRectangle.Bottom, marksZPosition);
				array[3] = new Point3D(innerPlotRectangle.Right, innerPlotRectangle.Bottom, marksZPosition);
				marksZPosition = axisX2.GetMarksZPosition(out axisOnEdge);
				array[4] = new Point3D(innerPlotRectangle.X, innerPlotRectangle.Y, marksZPosition);
				array[5] = new Point3D(innerPlotRectangle.X, innerPlotRectangle.Bottom, marksZPosition);
				marksZPosition = axisY2.GetMarksZPosition(out axisOnEdge);
				array[6] = new Point3D(innerPlotRectangle.X, innerPlotRectangle.Y, marksZPosition);
				array[7] = new Point3D(innerPlotRectangle.Right, innerPlotRectangle.Y, marksZPosition);
			}
			else
			{
				float marksZPosition = axisX.GetMarksZPosition(out axisOnEdge);
				array[0] = new Point3D(innerPlotRectangle.X, innerPlotRectangle.Bottom, marksZPosition);
				array[1] = new Point3D(innerPlotRectangle.Right, innerPlotRectangle.Bottom, marksZPosition);
				marksZPosition = axisY.GetMarksZPosition(out axisOnEdge);
				array[2] = new Point3D(innerPlotRectangle.X, innerPlotRectangle.Y, marksZPosition);
				array[3] = new Point3D(innerPlotRectangle.X, innerPlotRectangle.Bottom, marksZPosition);
				marksZPosition = axisX2.GetMarksZPosition(out axisOnEdge);
				array[4] = new Point3D(innerPlotRectangle.X, innerPlotRectangle.Y, marksZPosition);
				array[5] = new Point3D(innerPlotRectangle.Right, innerPlotRectangle.Y, marksZPosition);
				marksZPosition = axisY2.GetMarksZPosition(out axisOnEdge);
				array[6] = new Point3D(innerPlotRectangle.X, innerPlotRectangle.Y, marksZPosition);
				array[7] = new Point3D(innerPlotRectangle.X, innerPlotRectangle.Bottom, marksZPosition);
			}
			Axis[] axes = chartArea.Axes;
			foreach (Axis obj in axes)
			{
				obj.crossing = obj.tempCrossing;
			}
			matrix3D.TransformPoints(array);
			int num = 0;
			axes = chartArea.Axes;
			foreach (Axis axis in axes)
			{
				double num2 = Math.Sqrt((array[num].X - array[num + 1].X) * (array[num].X - array[num + 1].X) + (array[num].Y - array[num + 1].Y) * (array[num].Y - array[num + 1].Y));
				float num3 = 1f;
				if (!chartArea.switchValueAxes)
				{
					num3 = 0.5f;
				}
				if (axis.Type == AxisName.X || axis.Type == AxisName.X2)
				{
					if (chartArea.switchValueAxes)
					{
						axis.interval3DCorrection = num2 / (double)innerPlotRectangle.Height;
					}
					else
					{
						axis.interval3DCorrection = num2 / (double)innerPlotRectangle.Width;
					}
				}
				else if (chartArea.switchValueAxes)
				{
					axis.interval3DCorrection = num2 / (double)innerPlotRectangle.Width;
				}
				else
				{
					axis.interval3DCorrection = num2 / (double)innerPlotRectangle.Height * (double)num3;
				}
				if (axis.interval3DCorrection < 0.15)
				{
					axis.interval3DCorrection = 0.15;
				}
				if (axis.interval3DCorrection > 0.8)
				{
					axis.interval3DCorrection = 1.0;
				}
				num += 2;
			}
		}

		internal int GetRealYAngle()
		{
			int result = Area3DStyle.YAngle;
			if (reverseSeriesOrder && Area3DStyle.YAngle >= 0)
			{
				result = Area3DStyle.YAngle - 180;
			}
			if (reverseSeriesOrder && Area3DStyle.YAngle <= 0)
			{
				result = Area3DStyle.YAngle + 180;
			}
			return result;
		}

		internal bool ShouldDrawOnSurface(SurfaceNames surfaceName, bool backLayer, bool onEdge)
		{
			bool flag = (visibleSurfaces & surfaceName) == surfaceName;
			if (onEdge)
			{
				return backLayer;
			}
			return backLayer == !flag;
		}

		internal bool DrawPointsInReverseOrder()
		{
			return Area3DStyle.YAngle <= 0;
		}

		internal bool DrawPointsToCenter(ref COPCoordinates coord)
		{
			bool result = false;
			COPCoordinates cOPCoordinates = (COPCoordinates)0;
			if (Area3DStyle.Perspective != 0)
			{
				if ((coord & COPCoordinates.X) == COPCoordinates.X)
				{
					if ((visibleSurfaces & SurfaceNames.Left) == 0 && (visibleSurfaces & SurfaceNames.Right) == 0)
					{
						result = true;
					}
					cOPCoordinates |= COPCoordinates.X;
				}
				if ((coord & COPCoordinates.Y) == COPCoordinates.Y)
				{
					if ((visibleSurfaces & SurfaceNames.Top) == 0 && (visibleSurfaces & SurfaceNames.Bottom) == 0)
					{
						result = true;
					}
					cOPCoordinates |= COPCoordinates.Y;
				}
				if ((coord & COPCoordinates.Z) == COPCoordinates.Z)
				{
					if ((visibleSurfaces & SurfaceNames.Front) == 0 && (visibleSurfaces & SurfaceNames.Back) == 0)
					{
						result = true;
					}
					cOPCoordinates |= COPCoordinates.Z;
				}
			}
			return result;
		}

		internal bool DrawSeriesToCenter()
		{
			if (Area3DStyle.Perspective != 0 && (visibleSurfaces & SurfaceNames.Front) == 0 && (visibleSurfaces & SurfaceNames.Back) == 0)
			{
				return true;
			}
			return false;
		}

		protected void PaintChartSeries3D(ChartGraphics graph)
		{
			ChartArea area = (ChartArea)this;
			foreach (Series item in GetSeriesDrawingOrder(reverseSeriesOrder))
			{
				base.Common.ChartTypeRegistry.GetChartType(item.ChartTypeName).Paint(graph, base.Common, area, item);
			}
		}

		internal ArrayList GetClusterSeriesNames(string seriesName)
		{
			foreach (ArrayList seriesCluster in seriesClusters)
			{
				if (seriesCluster.Contains(seriesName))
				{
					return seriesCluster;
				}
			}
			return new ArrayList();
		}

		private ArrayList GetSeriesDrawingOrder(bool reverseSeriesOrder)
		{
			ArrayList arrayList = new ArrayList();
			foreach (ArrayList seriesCluster in seriesClusters)
			{
				if (seriesCluster.Count > 0)
				{
					Series value = base.Common.DataManager.Series[(string)seriesCluster[0]];
					arrayList.Add(value);
				}
			}
			if (reverseSeriesOrder)
			{
				arrayList.Reverse();
			}
			if (DrawSeriesToCenter() && matrix3D.IsInitialized())
			{
				Point3D point3D = new Point3D(float.NaN, float.NaN, float.NaN);
				point3D = GetCenterOfProjection(COPCoordinates.Z);
				if (!float.IsNaN(point3D.Z))
				{
					for (int i = 0; i < arrayList.Count; i++)
					{
						if (((Series)arrayList[i]).Points.Count == 0)
						{
							continue;
						}
						GetSeriesZPositionAndDepth((Series)arrayList[i], out float _, out float positionZ);
						if (positionZ >= point3D.Z)
						{
							i--;
							if (i < 0)
							{
								i = 0;
							}
							arrayList.Reverse(i, arrayList.Count - i);
							break;
						}
					}
				}
			}
			return arrayList;
		}

		private int GetNumberOfStackGroups(ArrayList seriesNamesList)
		{
			stackGroupNames = new ArrayList();
			foreach (object seriesNames in seriesNamesList)
			{
				_ = stackGroupNames.Count;
				Series series = base.Common.DataManager.Series[(string)seriesNames];
				string text = string.Empty;
				if (series.IsAttributeSet("StackedGroupName"))
				{
					text = series["StackedGroupName"];
				}
				if (!stackGroupNames.Contains(text))
				{
					stackGroupNames.Add(text);
				}
			}
			return stackGroupNames.Count;
		}

		internal int GetSeriesStackGroupIndex(Series series, ref string stackGroupName)
		{
			stackGroupName = string.Empty;
			if (stackGroupNames != null)
			{
				if (series.IsAttributeSet("StackedGroupName"))
				{
					stackGroupName = series["StackedGroupName"];
				}
				return stackGroupNames.IndexOf(stackGroupName);
			}
			return 0;
		}

		internal ArrayList GetDataPointDrawingOrder(ArrayList seriesNamesList, IChartType chartType, bool selection, COPCoordinates coord, IComparer comparer, int mainYValueIndex, bool sideBySide)
		{
			ChartArea chartArea = (ChartArea)this;
			ArrayList arrayList = new ArrayList();
			double num = 1.0;
			if (chartArea.Area3DStyle.Clustered && !chartType.Stacked && sideBySide)
			{
				num = seriesNamesList.Count;
			}
			if (chartType.SupportStackedGroups)
			{
				int numberOfStackGroups = GetNumberOfStackGroups(seriesNamesList);
				if (Area3DStyle.Clustered && seriesNamesList.Count > 0)
				{
					num = numberOfStackGroups;
				}
			}
			bool flag = chartArea.IndexedSeries((string[])seriesNamesList.ToArray(typeof(string)));
			int num2 = 0;
			foreach (object seriesNames in seriesNamesList)
			{
				Series series = base.Common.DataManager.Series[(string)seriesNames];
				if (chartType.SupportStackedGroups && stackGroupNames != null)
				{
					string stackGroupName = string.Empty;
					num2 = GetSeriesStackGroupIndex(series, ref stackGroupName);
					if (chartType is StackedColumnChart)
					{
						((StackedColumnChart)chartType).currentStackGroup = stackGroupName;
					}
					else if (chartType is StackedBarChart)
					{
						((StackedBarChart)chartType).currentStackGroup = stackGroupName;
					}
				}
				Axis axis = (series.YAxisType == AxisType.Primary) ? chartArea.AxisY : chartArea.AxisY2;
				Axis axis2 = (series.XAxisType == AxisType.Primary) ? chartArea.AxisX : chartArea.AxisX2;
				axis2.GetViewMinimum();
				axis2.GetViewMaximum();
				axis.GetViewMinimum();
				axis.GetViewMaximum();
				bool sameInterval = true;
				double interval = 1.0;
				if (!flag)
				{
					interval = chartArea.GetPointsInterval(seriesNamesList, axis2.Logarithmic, axis2.logarithmBase, checkSameInterval: true, out sameInterval);
				}
				double num3 = series.GetPointWidth(chartArea.Common.graph, axis2, interval, 0.8) / num;
				GetSeriesZPositionAndDepth(series, out float depth, out float positionZ);
				int num4 = 0;
				foreach (DataPoint point in series.Points)
				{
					num4++;
					double xPosition;
					double position;
					if (flag)
					{
						xPosition = axis2.GetPosition(num4) - num3 * num / 2.0 + num3 / 2.0 + (double)num2 * num3;
						position = axis2.GetPosition(num4);
					}
					else if (sameInterval)
					{
						xPosition = axis2.GetPosition(point.XValue) - num3 * num / 2.0 + num3 / 2.0 + (double)num2 * num3;
						position = axis2.GetPosition(point.XValue);
					}
					else
					{
						xPosition = axis2.GetPosition(point.XValue);
						position = axis2.GetPosition(point.XValue);
					}
					DataPoint3D dataPoint3D = new DataPoint3D();
					dataPoint3D.indexedSeries = flag;
					dataPoint3D.dataPoint = point;
					dataPoint3D.index = num4;
					dataPoint3D.xPosition = xPosition;
					dataPoint3D.xCenterVal = position;
					dataPoint3D.width = series.GetPointWidth(chartArea.Common.graph, axis2, interval, 0.8) / num;
					dataPoint3D.depth = depth;
					dataPoint3D.zPosition = positionZ;
					double yValue = chartType.GetYValue(base.Common, chartArea, series, point, num4 - 1, mainYValueIndex);
					dataPoint3D.yPosition = axis.GetPosition(yValue);
					dataPoint3D.height = axis.GetPosition(yValue - chartType.GetYValue(base.Common, chartArea, series, point, num4 - 1, -1));
					arrayList.Add(dataPoint3D);
				}
				if (num > 1.0 && sideBySide)
				{
					num2++;
				}
			}
			if (comparer == null)
			{
				comparer = new PointsDrawingOrderComparer((ChartArea)this, selection, coord);
			}
			arrayList.Sort(comparer);
			return arrayList;
		}

		internal Point3D GetCenterOfProjection(COPCoordinates coord)
		{
			Point3D[] array = new Point3D[2]
			{
				new Point3D(base.PlotAreaPosition.X, base.PlotAreaPosition.Bottom(), 0f),
				new Point3D(base.PlotAreaPosition.Right(), base.PlotAreaPosition.Y, areaSceneDepth)
			};
			CheckSurfaceOrientation(coord, array[0], array[1], out bool xSameOrientation, out bool ySameOrientation, out bool zSameOrientation);
			Point3D point3D = new Point3D(xSameOrientation ? float.NaN : 0f, ySameOrientation ? float.NaN : 0f, zSameOrientation ? float.NaN : 0f);
			if (((coord & COPCoordinates.X) != COPCoordinates.X || xSameOrientation) && ((coord & COPCoordinates.Y) != COPCoordinates.Y || ySameOrientation) && ((coord & COPCoordinates.Z) != COPCoordinates.Z || zSameOrientation))
			{
				return point3D;
			}
			SizeF sizeF = new SizeF(0.5f, 0.5f);
			sizeF.Width = sizeF.Width * 100f / (float)(base.Common.Chart.Width - 1);
			sizeF.Height = sizeF.Height * 100f / (float)(base.Common.Chart.Height - 1);
			bool flag = false;
			while (!flag)
			{
				Point3D point3D2 = new Point3D((array[0].X + array[1].X) / 2f, (array[0].Y + array[1].Y) / 2f, (array[0].Z + array[1].Z) / 2f);
				CheckSurfaceOrientation(coord, array[0], point3D2, out xSameOrientation, out ySameOrientation, out zSameOrientation);
				array[(!xSameOrientation) ? 1 : 0].X = point3D2.X;
				array[(!ySameOrientation) ? 1 : 0].Y = point3D2.Y;
				array[(!zSameOrientation) ? 1 : 0].Z = point3D2.Z;
				flag = true;
				if ((coord & COPCoordinates.X) == COPCoordinates.X && Math.Abs(array[1].X - array[0].X) >= sizeF.Width)
				{
					flag = false;
				}
				if ((coord & COPCoordinates.Y) == COPCoordinates.Y && Math.Abs(array[1].Y - array[0].Y) >= sizeF.Height)
				{
					flag = false;
				}
				if ((coord & COPCoordinates.Z) == COPCoordinates.Z && Math.Abs(array[1].Z - array[0].Z) >= sizeF.Width)
				{
					flag = false;
				}
			}
			if (!float.IsNaN(point3D.X))
			{
				point3D.X = (array[0].X + array[1].X) / 2f;
			}
			if (!float.IsNaN(point3D.Y))
			{
				point3D.Y = (array[0].Y + array[1].Y) / 2f;
			}
			if (!float.IsNaN(point3D.Z))
			{
				point3D.Z = (array[0].Z + array[1].Z) / 2f;
			}
			return point3D;
		}

		private void CheckSurfaceOrientation(COPCoordinates coord, Point3D point1, Point3D point2, out bool xSameOrientation, out bool ySameOrientation, out bool zSameOrientation)
		{
			Point3D[] array = new Point3D[3];
			xSameOrientation = true;
			ySameOrientation = true;
			zSameOrientation = true;
			if ((coord & COPCoordinates.X) == COPCoordinates.X)
			{
				array[0] = new Point3D(point1.X, base.PlotAreaPosition.Y, 0f);
				array[1] = new Point3D(point1.X, base.PlotAreaPosition.Bottom(), 0f);
				array[2] = new Point3D(point1.X, base.PlotAreaPosition.Bottom(), areaSceneDepth);
				matrix3D.TransformPoints(array);
				bool flag = ChartGraphics3D.IsSurfaceVisible(array[0], array[1], array[2]);
				array[0] = new Point3D(point2.X, base.PlotAreaPosition.Y, 0f);
				array[1] = new Point3D(point2.X, base.PlotAreaPosition.Bottom(), 0f);
				array[2] = new Point3D(point2.X, base.PlotAreaPosition.Bottom(), areaSceneDepth);
				matrix3D.TransformPoints(array);
				bool flag2 = ChartGraphics3D.IsSurfaceVisible(array[0], array[1], array[2]);
				xSameOrientation = (flag == flag2);
			}
			if ((coord & COPCoordinates.Y) == COPCoordinates.Y)
			{
				array[0] = new Point3D(base.PlotAreaPosition.X, point1.Y, areaSceneDepth);
				array[1] = new Point3D(base.PlotAreaPosition.X, point1.Y, 0f);
				array[2] = new Point3D(base.PlotAreaPosition.Right(), point1.Y, 0f);
				matrix3D.TransformPoints(array);
				bool flag = ChartGraphics3D.IsSurfaceVisible(array[0], array[1], array[2]);
				array[0] = new Point3D(base.PlotAreaPosition.X, point2.Y, areaSceneDepth);
				array[1] = new Point3D(base.PlotAreaPosition.X, point2.Y, 0f);
				array[2] = new Point3D(base.PlotAreaPosition.Right(), point2.Y, 0f);
				matrix3D.TransformPoints(array);
				bool flag2 = ChartGraphics3D.IsSurfaceVisible(array[0], array[1], array[2]);
				ySameOrientation = (flag == flag2);
			}
			if ((coord & COPCoordinates.Z) == COPCoordinates.Z)
			{
				array[0] = new Point3D(base.PlotAreaPosition.X, base.PlotAreaPosition.Y, point1.Z);
				array[1] = new Point3D(base.PlotAreaPosition.X, base.PlotAreaPosition.Bottom(), point1.Z);
				array[2] = new Point3D(base.PlotAreaPosition.Right(), base.PlotAreaPosition.Bottom(), point1.Z);
				matrix3D.TransformPoints(array);
				bool flag = ChartGraphics3D.IsSurfaceVisible(array[0], array[1], array[2]);
				array[0] = new Point3D(base.PlotAreaPosition.X, base.PlotAreaPosition.Y, point2.Z);
				array[1] = new Point3D(base.PlotAreaPosition.X, base.PlotAreaPosition.Bottom(), point2.Z);
				array[2] = new Point3D(base.PlotAreaPosition.Right(), base.PlotAreaPosition.Bottom(), point2.Z);
				matrix3D.TransformPoints(array);
				bool flag2 = ChartGraphics3D.IsSurfaceVisible(array[0], array[1], array[2]);
				zSameOrientation = (flag == flag2);
			}
		}
	}
}
