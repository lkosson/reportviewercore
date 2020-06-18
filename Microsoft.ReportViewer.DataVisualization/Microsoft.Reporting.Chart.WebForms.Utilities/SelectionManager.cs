using Microsoft.Reporting.Chart.WebForms.ChartTypes;
using Microsoft.Reporting.Chart.WebForms.Data;
using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Drawing;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms.Utilities
{
	internal class SelectionManager : IServiceProvider
	{
		internal class ChartAreaRectangle
		{
			internal ChartArea ChartArea;

			internal RectangleF Rectangle = RectangleF.Empty;
		}

		private IServiceContainer service;

		private ArrayList selectableObjectList = new ArrayList();

		private bool enabled;

		internal Point selectionPoint = Point.Empty;

		internal bool invalidated = true;

		private ContextElementTypes selectableTypes = ContextElementTypes.Any;

		internal ObjectInfo selectedObjectInfo = new ObjectInfo();

		private HotRegion hotRegion = new HotRegion();

		private Chart chartControl;

		private ChartPicture chartPicture;

		private DataManager dataManager;

		internal Chart ChartControl
		{
			get
			{
				if (chartControl == null && Chart != null)
				{
					chartControl = Chart.common.Chart;
				}
				return chartControl;
			}
		}

		internal ChartPicture Chart
		{
			get
			{
				if (chartPicture == null)
				{
					chartPicture = (service.GetService(typeof(ChartImage)) as ChartPicture);
					if (chartPicture == null)
					{
						chartPicture = (service.GetService(typeof(ChartPicture)) as ChartPicture);
					}
				}
				return chartPicture;
			}
		}

		internal DataManager DataManager
		{
			get
			{
				if (dataManager == null)
				{
					dataManager = (service.GetService(typeof(DataManager)) as DataManager);
				}
				return dataManager;
			}
		}

		internal ChartGraphics Graph
		{
			get
			{
				if (Chart != null)
				{
					return Chart.common.graph;
				}
				return null;
			}
		}

		internal ContextElementTypes SelectableTypes
		{
			get
			{
				return selectableTypes;
			}
			set
			{
				selectableTypes = value;
				Invalidate();
			}
		}

		public virtual Point SelectionPoint
		{
			get
			{
				return selectionPoint;
			}
			set
			{
				selectionPoint = value;
				Invalidate();
			}
		}

		internal ObjectInfo Result
		{
			get
			{
				CheckInvalidated();
				return selectedObjectInfo;
			}
			set
			{
				invalidated = false;
				selectedObjectInfo = value;
			}
		}

		internal HitTestResult HitTestResult
		{
			get
			{
				return selectedObjectInfo.InspectedObject as HitTestResult;
			}
			set
			{
				Result = ObjectInfo.Get(value, chartControl);
			}
		}

		internal HotRegion HotRegion
		{
			get
			{
				return hotRegion;
			}
			set
			{
				hotRegion = value;
			}
		}

		internal bool Enabled
		{
			get
			{
				return enabled;
			}
			set
			{
				enabled = value;
			}
		}

		internal SelectionManager(IServiceContainer service, bool assignService)
		{
			this.service = service;
		}

		internal SelectionManager(IServiceContainer service)
		{
			if (service == null)
			{
				throw new ArgumentNullException("service");
			}
			if (service.GetService(typeof(SelectionManager)) != null)
			{
				throw new ArgumentException(SR.ExceptionObjectSelectorAlreadyRegistred);
			}
			this.service = service;
			this.service.AddService(typeof(SelectionManager), this);
		}

		internal bool SelectChartElement(ChartElementType elementType, object chartObject, object chartSubObject)
		{
			if (ChartControl != null)
			{
				string seriesName = string.Empty;
				int result = 0;
				if ((elementType == ChartElementType.DataPoint || elementType == ChartElementType.DataPointLabel) && !(chartObject is Series) && !(chartObject is DataPoint))
				{
					return false;
				}
				DataPoint dataPoint = null;
				if (chartObject is Series)
				{
					seriesName = (chartObject as Series).Name;
					if (chartSubObject is DataPoint && (chartSubObject as DataPoint).series != null)
					{
						dataPoint = (chartSubObject as DataPoint);
						seriesName = dataPoint.series.Name;
						result = dataPoint.series.Points.IndexOf(dataPoint);
					}
				}
				else if (chartObject is DataPoint && (chartObject as DataPoint).series != null)
				{
					dataPoint = (chartObject as DataPoint);
					seriesName = dataPoint.series.Name;
					result = dataPoint.series.Points.IndexOf(dataPoint);
				}
				if (result == -1 && dataPoint != null && dataPoint.IsAttributeSet("OriginalPointIndex"))
				{
					int.TryParse(dataPoint["OriginalPointIndex"], NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
				}
				if (result == -1)
				{
					return false;
				}
				HitTestResult = ChartControl.GetHitTestResult(seriesName, result, elementType, chartObject);
				return true;
			}
			return false;
		}

		internal bool IsChartElementSelected(ChartElementType elementType, object chartObject, object chartSubObject)
		{
			if (HitTestResult != null && HitTestResult.ChartElementType == elementType && HitTestResult.Object == chartObject && HitTestResult.SubObject == chartSubObject)
			{
				return true;
			}
			return false;
		}

		internal void Invalidate()
		{
			invalidated = true;
		}

		internal virtual void CheckInvalidated()
		{
			if (invalidated)
			{
				HitTest();
			}
		}

		private PointF GetRelativeHitPoint()
		{
			if (Chart != null && Chart.common != null && Chart.common.graph != null)
			{
				return Chart.common.graph.GetRelativePoint(selectionPoint);
			}
			return PointF.Empty;
		}

		private IList GetHitOrderList()
		{
			ArrayList arrayList = new ArrayList();
			ContextElementTypes contextElementTypes = selectableTypes;
			if ((contextElementTypes & ContextElementTypes.Annotation) == ContextElementTypes.Annotation)
			{
				arrayList.Add(ChartElementType.Annotation);
			}
			if ((contextElementTypes & ContextElementTypes.Title) == ContextElementTypes.Title)
			{
				arrayList.Add(ChartElementType.Title);
			}
			if ((contextElementTypes & ContextElementTypes.Legend) == ContextElementTypes.Legend)
			{
				arrayList.Add(ChartElementType.LegendArea);
				arrayList.Add(ChartElementType.LegendItem);
				arrayList.Add(ChartElementType.LegendTitle);
			}
			if ((contextElementTypes & ContextElementTypes.Series) == ContextElementTypes.Series)
			{
				arrayList.Add(ChartElementType.DataPoint);
				arrayList.Add(ChartElementType.DataPointLabel);
			}
			if ((contextElementTypes & ContextElementTypes.Axis) == ContextElementTypes.Axis || (contextElementTypes & ContextElementTypes.AxisLabel) == ContextElementTypes.AxisLabel)
			{
				arrayList.Add(ChartElementType.Axis);
				arrayList.Add(ChartElementType.AxisLabelImage);
				arrayList.Add(ChartElementType.AxisTitle);
				arrayList.Add(ChartElementType.AxisLabels);
				arrayList.Add(ChartElementType.TickMarks);
			}
			if ((contextElementTypes & ContextElementTypes.ChartArea) == ContextElementTypes.ChartArea)
			{
				arrayList.Add(ChartElementType.PlottingArea);
				arrayList.Add(ChartElementType.StripLines);
				arrayList.Add(ChartElementType.Gridlines);
			}
			return arrayList;
		}

		protected bool IsArea3D(ChartArea area)
		{
			if (area.Area3DStyle.Enable3D && !IsChartAreaCircular(area) && area.matrix3D != null)
			{
				return area.matrix3D.IsInitialized();
			}
			return false;
		}

		protected internal virtual IList GetAxisMarkers(ChartGraphics graph, Axis axis)
		{
			ArrayList arrayList = new ArrayList();
			if (axis == null)
			{
				return arrayList;
			}
			PointF empty = PointF.Empty;
			PointF empty2 = PointF.Empty;
			switch (axis.AxisPosition)
			{
			case AxisPosition.Left:
				empty.X = (float)axis.GetAxisPosition();
				empty.Y = axis.PlotAreaPosition.Bottom();
				empty2.X = (float)axis.GetAxisPosition();
				empty2.Y = axis.PlotAreaPosition.Y;
				break;
			case AxisPosition.Right:
				empty.X = (float)axis.GetAxisPosition();
				empty.Y = axis.PlotAreaPosition.Bottom();
				empty2.X = (float)axis.GetAxisPosition();
				empty2.Y = axis.PlotAreaPosition.Y;
				break;
			case AxisPosition.Bottom:
				empty.X = axis.PlotAreaPosition.X;
				empty.Y = (float)axis.GetAxisPosition();
				empty2.X = axis.PlotAreaPosition.Right();
				empty2.Y = (float)axis.GetAxisPosition();
				break;
			case AxisPosition.Top:
				empty.X = axis.PlotAreaPosition.X;
				empty.Y = (float)axis.GetAxisPosition();
				empty2.X = axis.PlotAreaPosition.Right();
				empty2.Y = (float)axis.GetAxisPosition();
				break;
			}
			IList markers = GetMarkers(RectangleF.FromLTRB(empty.X, empty.Y, empty2.X, empty2.Y));
			if (IsArea3D(axis.chartArea))
			{
				float areaSceneDepth = axis.chartArea.areaSceneDepth;
				Point3D[] array = new Point3D[markers.Count];
				for (int i = 0; i < markers.Count; i++)
				{
					array[i] = new Point3D(((PointF)markers[i]).X, ((PointF)markers[i]).Y, areaSceneDepth);
				}
				axis.chartArea.matrix3D.TransformPoints(array);
				for (int j = 0; j < markers.Count; j++)
				{
					markers[j] = array[j].PointF;
				}
			}
			foreach (PointF item in markers)
			{
				arrayList.Add(graph.GetAbsolutePoint(item));
			}
			return arrayList;
		}

		protected internal bool IsChartAreaCircular(ChartArea area)
		{
			foreach (object chartType2 in area.ChartTypes)
			{
				IChartType chartType = area.Common.ChartTypeRegistry.GetChartType(chartType2.ToString());
				if (chartType != null && (chartType.CircularChartArea || !chartType.RequireAxes))
				{
					return true;
				}
			}
			return false;
		}

		protected internal virtual IList GetAreaMarkers(ChartGraphics graph, ChartArea area)
		{
			ArrayList arrayList = new ArrayList();
			if (area == null)
			{
				return arrayList;
			}
			IList markers = GetMarkers(area.PlotAreaPosition.ToRectangleF());
			if (IsChartAreaCircular(area))
			{
				markers = GetMarkers(area.Position.ToRectangleF());
			}
			if (IsArea3D(area))
			{
				float z = 0f;
				Point3D[] array = new Point3D[markers.Count];
				for (int i = 0; i < markers.Count; i++)
				{
					array[i] = new Point3D(((PointF)markers[i]).X, ((PointF)markers[i]).Y, z);
				}
				area.matrix3D.TransformPoints(array);
				for (int j = 0; j < markers.Count; j++)
				{
					markers[j] = array[j].PointF;
				}
			}
			foreach (PointF item in markers)
			{
				arrayList.Add(graph.GetAbsolutePoint(item));
			}
			return arrayList;
		}

		protected internal virtual void SearchForHotRegion()
		{
			object contextObjectNoLabel = Result.GetContextObjectNoLabel();
			ChartElementType chartElementType = Result.GetChartElementType();
			this.hotRegion = new HotRegion();
			if (contextObjectNoLabel == null)
			{
				return;
			}
			HotRegionsList hotRegionsList = Chart.common.HotRegionsList;
			if (hotRegionsList.List.Count == 0)
			{
				ChartControl.HitTest(2, 2);
			}
			int num = hotRegionsList.List.Count - 1;
			HotRegion hotRegion;
			while (true)
			{
				if (num >= 0)
				{
					hotRegion = (HotRegion)hotRegionsList.List[num];
					if (hotRegion.SelectedObject == contextObjectNoLabel && hotRegion.Type == chartElementType && hotRegion.Type == chartElementType)
					{
						break;
					}
					num--;
					continue;
				}
				return;
			}
			this.hotRegion = hotRegion;
		}

		internal void Reset()
		{
			SelectionPoint = Point.Empty;
		}

		internal virtual void HitTest()
		{
			invalidated = false;
			selectedObjectInfo = new ObjectInfo();
			if (selectionPoint == Point.Empty || SelectableTypes == ContextElementTypes.None)
			{
				return;
			}
			Chart chart = ChartControl;
			if (chart == null)
			{
				return;
			}
			try
			{
				HitTestResult hitTestResult = null;
				if (SelectableTypes == ContextElementTypes.Any)
				{
					hitTestResult = chart.HitTest(SelectionPoint.X, SelectionPoint.Y);
				}
				else
				{
					foreach (ChartElementType hitOrder in GetHitOrderList())
					{
						hitTestResult = chart.HitTest(SelectionPoint.X, SelectionPoint.Y, hitOrder);
						if (hitTestResult.Object != null)
						{
							break;
						}
					}
				}
				selectedObjectInfo = ObjectInfo.Get(hitTestResult, chart);
			}
			catch
			{
				chart.IsDesignMode();
			}
		}

		internal virtual void DrawSelection()
		{
			if (enabled)
			{
				ChartGraphics graph = Graph;
				if (graph != null)
				{
					DrawSelection(graph.Graphics);
				}
			}
		}

		protected internal virtual IList GetMarkers(RectangleF rect)
		{
			return GetMarkers(rect, addAdditionalMarkers: true);
		}

		protected internal virtual IList GetMarkers(RectangleF rect, bool addAdditionalMarkers)
		{
			ArrayList arrayList = new ArrayList();
			if (!addAdditionalMarkers)
			{
				if (rect.Width > 0f && rect.Height > 0f)
				{
					arrayList.Add(new PointF(rect.Left, rect.Top));
					arrayList.Add(new PointF(rect.Right, rect.Top));
					arrayList.Add(new PointF(rect.Right, rect.Bottom));
					arrayList.Add(new PointF(rect.Left, rect.Bottom));
				}
				else if (rect.Width > 0f)
				{
					arrayList.Add(new PointF(rect.Left, rect.Top));
					arrayList.Add(new PointF(rect.Right, rect.Top));
				}
				else if (rect.Height > 0f)
				{
					arrayList.Add(new PointF(rect.Left, rect.Top));
					arrayList.Add(new PointF(rect.Left, rect.Bottom));
				}
			}
			else if (rect.Width > 0f)
			{
				arrayList.Add(new PointF(rect.Left, rect.Top));
				if (rect.Width > 30f)
				{
					arrayList.Add(new PointF(rect.Left + rect.Width / 2f, rect.Top));
				}
				arrayList.Add(new PointF(rect.Right, rect.Top));
				if (rect.Height > 30f)
				{
					arrayList.Add(new PointF(rect.Right, rect.Top + rect.Height / 2f));
				}
				arrayList.Add(new PointF(rect.Right, rect.Bottom));
				if (rect.Width > 30f)
				{
					arrayList.Add(new PointF(rect.Left + rect.Width / 2f, rect.Bottom));
				}
				arrayList.Add(new PointF(rect.Left, rect.Bottom));
				if (rect.Height > 30f)
				{
					arrayList.Add(new PointF(rect.Left, rect.Top + rect.Height / 2f));
				}
			}
			else if (rect.Width > 0f)
			{
				arrayList.Add(new PointF(rect.Left, rect.Top));
				if (rect.Width > 30f)
				{
					arrayList.Add(new PointF(rect.Left + rect.Width / 2f, rect.Top));
				}
				arrayList.Add(new PointF(rect.Right, rect.Top));
			}
			else if (rect.Height > 0f)
			{
				arrayList.Add(new PointF(rect.Left, rect.Bottom));
				if (rect.Height > 30f)
				{
					arrayList.Add(new PointF(rect.Left, rect.Top + rect.Height / 2f));
				}
				arrayList.Add(new PointF(rect.Left, rect.Top));
			}
			return arrayList;
		}

		protected internal PointF Transform3D(ChartArea3D chartArea, DataPoint point, ChartGraphics graph)
		{
			if (chartArea is ChartArea && IsArea3D((ChartArea)chartArea))
			{
				float positionZ = chartArea.areaSceneDepth;
				if (point != null && point.series != null)
				{
					float depth = 0f;
					chartArea.GetSeriesZPositionAndDepth(point.series, out depth, out positionZ);
					positionZ += depth / 2f;
				}
				PointF positionRel = point.positionRel;
				Point3D[] array = new Point3D[1]
				{
					new Point3D(positionRel.X, positionRel.Y, positionZ)
				};
				chartArea.matrix3D.TransformPoints(array);
				return array[0].PointF;
			}
			return point.positionRel;
		}

		internal bool IsElementClickable(object element, ChartElementType chartElementType)
		{
			if (element != null)
			{
				HotRegionsList hotRegionsList = Chart.common.HotRegionsList;
				if (hotRegionsList.List.Count == 0)
				{
					ChartControl.HitTest(2, 2);
				}
				foreach (HotRegion item in hotRegionsList.List)
				{
					ChartElementType type = item.Type;
					if (type == ChartElementType.DataPointLabel)
					{
						if (element is Series && string.Compare(item.SeriesName, ((Series)element).Name, StringComparison.Ordinal) == 0 && item.Type == chartElementType)
						{
							return true;
						}
					}
					else if (item.SelectedObject == element && item.Type == chartElementType)
					{
						return true;
					}
				}
			}
			return false;
		}

		protected internal virtual IList GetMarkers(ChartGraphics graph)
		{
			ArrayList arrayList = new ArrayList();
			if (Result.ElementType == ContextElementTypes.None)
			{
				return arrayList;
			}
			if (Result.ElementType == ContextElementTypes.ChartArea)
			{
				return GetAreaMarkers(graph, Result.ChartArea);
			}
			if (Result.ElementType == ContextElementTypes.Series)
			{
				Series series = Result.GetContextObject() as Series;
				if (series != null)
				{
					string text = series.ChartArea;
					if (string.CompareOrdinal(text, "Default") == 0 && Chart.ChartAreas.GetIndex(text) == -1 && Chart.ChartAreas.Count > 0)
					{
						text = Chart.ChartAreas[0].Name;
					}
					if (Chart.ChartAreas.GetIndex(text) != -1 && series.Enabled)
					{
						ChartArea chartArea = Chart.ChartAreas[text];
						if (ChartControl.Series.GetIndex(series.Name) != -1)
						{
							series = ChartControl.Series[series.Name];
						}
						DataPointCollection dataPointCollection = series.Points;
						if (dataPointCollection.Count == 0)
						{
							dataPointCollection = series.fakeDataPoints;
						}
						{
							foreach (DataPoint item in dataPointCollection)
							{
								PointF relative = Transform3D(chartArea, item, graph);
								if (!float.IsNaN(relative.X) && !float.IsNaN(relative.Y))
								{
									arrayList.Add(graph.GetAbsolutePoint(relative));
								}
							}
							return arrayList;
						}
					}
				}
			}
			else
			{
				if (Result.ElementType.ToString().IndexOf("Axis", StringComparison.Ordinal) != -1)
				{
					return GetAxisMarkers(graph, Result.GetContextObjectNoLabel() as Axis);
				}
				if (hotRegion.Type != 0)
				{
					RectangleF absoluteRectangle = graph.GetAbsoluteRectangle(hotRegion.BoundingRectangle);
					if (Result.ElementType == ContextElementTypes.Axis || Result.ElementType == ContextElementTypes.AxisLabel)
					{
						SizeF size = absoluteRectangle.Size;
						if (absoluteRectangle.Width > absoluteRectangle.Height)
						{
							absoluteRectangle.Height = 0f;
						}
						else
						{
							absoluteRectangle.Width = 0f;
						}
						Axis axis = Result.GetContextObjectNoLabel() as Axis;
						if (axis != null)
						{
							switch (axis.Type)
							{
							case AxisName.X2:
								absoluteRectangle.Offset(0f, size.Height);
								break;
							case AxisName.Y:
								absoluteRectangle.Offset(size.Width, 0f);
								break;
							}
						}
					}
					return GetMarkers(absoluteRectangle);
				}
			}
			return arrayList;
		}

		internal virtual void DrawSelection(Graphics g)
		{
			if (Chart.isSelectionMode)
			{
				return;
			}
			CheckInvalidated();
			if (Result.ElementType == ContextElementTypes.None)
			{
				return;
			}
			SearchForHotRegion();
			ChartGraphics graph = Graph;
			if (graph == null)
			{
				return;
			}
			graph.Graphics = g;
			foreach (PointF marker in GetMarkers(graph))
			{
				int markerSize = 5;
				graph.DrawMarkerAbs(marker, MarkerStyle.Square, markerSize, Color.White, Color.Gray, 1, string.Empty, Color.Empty, 0, Color.Empty, RectangleF.Empty, forceAntiAlias: true);
			}
		}

		object IServiceProvider.GetService(Type serviceType)
		{
			if (serviceType == GetType())
			{
				return this;
			}
			return service.GetService(serviceType);
		}
	}
}
