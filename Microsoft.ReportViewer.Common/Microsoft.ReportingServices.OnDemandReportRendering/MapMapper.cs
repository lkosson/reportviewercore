using Microsoft.Reporting.Map.WebForms;
using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal class MapMapper : MapperBase, IMapMapper, IDVMappingLayer, IDisposable
	{
		private class BoundsRectCalculator
		{
			private bool hasValue;

			internal Microsoft.Reporting.Map.WebForms.MapPoint Min;

			internal Microsoft.Reporting.Map.WebForms.MapPoint Max;

			internal Microsoft.Reporting.Map.WebForms.MapPoint Center => new Microsoft.Reporting.Map.WebForms.MapPoint((Min.X + Max.X) / 2.0, (Min.Y + Max.Y) / 2.0);

			internal void AddSpatialElement(ISpatialElement spatialElement)
			{
				if (!hasValue)
				{
					Min = new Microsoft.Reporting.Map.WebForms.MapPoint(spatialElement.MinimumExtent.X + spatialElement.Offset.X, spatialElement.MinimumExtent.Y + spatialElement.Offset.Y);
					Max = new Microsoft.Reporting.Map.WebForms.MapPoint(spatialElement.MaximumExtent.X + spatialElement.Offset.X, spatialElement.MaximumExtent.Y + spatialElement.Offset.Y);
					hasValue = true;
				}
				else
				{
					Min.X = Math.Min(Min.X, spatialElement.MinimumExtent.X + spatialElement.Offset.X);
					Min.Y = Math.Min(Min.Y, spatialElement.MinimumExtent.Y + spatialElement.Offset.Y);
					Max.X = Math.Max(Max.X, spatialElement.MaximumExtent.X + spatialElement.Offset.X);
					Max.Y = Math.Max(Max.Y, spatialElement.MaximumExtent.Y + spatialElement.Offset.Y);
				}
			}
		}

		private Map m_map;

		private int m_remainingSpatialElementCount = 20000;

		private int m_remainingTotalPointCount = 1000000;

		private MapControl m_coreMap;

		private ActionInfoWithDynamicImageMapCollection m_actions = new ActionInfoWithDynamicImageMapCollection();

		private static string m_defaultContentMarginString = "10pt";

		private static ReportSize m_defaultContentMargin = new ReportSize(m_defaultContentMarginString);

		private static string m_defaultTickMarkLengthString = "2.25pt";

		private static ReportSize m_defaultTickMarkLength = new ReportSize(m_defaultTickMarkLengthString);

		private BoundsRectCalculator m_boundRectCalculator;

		private MapSimplifier m_mapSimplifier;

		private double? m_simpificationResolution;

		private TileLayerMapper m_tileLayerMapper;

		private Formatter m_formatter;

		internal int RemainingSpatialElementCount => m_remainingSpatialElementCount;

		internal int RemainingTotalPointCount => m_remainingTotalPointCount;

		internal bool CanAddSpatialElement
		{
			get
			{
				if (m_remainingSpatialElementCount > 0 && m_remainingTotalPointCount > 0)
				{
					return true;
				}
				if (m_remainingSpatialElementCount < 1)
				{
					m_coreMap.Viewport.ErrorMessage = RPRes.rsMapMaximumSpatialElementCountReached(RPRes.rsObjectTypeMap, m_map.Name);
				}
				else if (m_remainingTotalPointCount < 1)
				{
					m_coreMap.Viewport.ErrorMessage = RPRes.rsMapMaximumTotalPointCountReached(RPRes.rsObjectTypeMap, m_map.Name);
				}
				return false;
			}
		}

		private MapSimplifier Simplifier
		{
			get
			{
				if (!m_simpificationResolution.HasValue)
				{
					m_simpificationResolution = EvaluateSimplificationResolution();
					if (m_simpificationResolution.Value != 0.0)
					{
						m_mapSimplifier = new MapSimplifier();
					}
				}
				return m_mapSimplifier;
			}
		}

		public MapMapper(Map map, string defaultFontFamily)
			: base(defaultFontFamily)
		{
			m_map = map;
		}

		public void RenderMap()
		{
			try
			{
				if (m_map != null)
				{
					InitializeMap();
					SetMapProperties();
					RenderLayers();
					RenderViewport();
					RenderLegends();
					RenderTitles();
					RenderDistanceScale();
					RenderColorScale();
					RenderBorderSkin();
					RenderMapStyle();
				}
			}
			catch (RSException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(ex2))
				{
					throw;
				}
				throw new RenderingObjectModelException(ex2);
			}
		}

		public Stream GetCoreXml()
		{
			try
			{
				m_coreMap.Serializer.Content = SerializationContent.All;
				m_coreMap.Serializer.NonSerializableContent = "";
				MemoryStream memoryStream = new MemoryStream();
				m_coreMap.Serializer.Save(memoryStream);
				memoryStream.Position = 0L;
				return memoryStream;
			}
			catch (Exception ex)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(ex))
				{
					throw;
				}
				Global.Tracer.Trace(TraceLevel.Verbose, ex.Message);
				return null;
			}
		}

		public Stream GetImage(DynamicImageInstance.ImageType imageType)
		{
			try
			{
				if (m_coreMap == null)
				{
					return null;
				}
				int width = 300;
				if (base.WidthOverrideInPixels.HasValue)
				{
					width = base.WidthOverrideInPixels.Value;
				}
				else if (m_map.Width != null)
				{
					width = MappingHelper.ToIntPixels(m_map.Width, base.DpiX);
				}
				m_coreMap.Width = width;
				int height = 300;
				if (base.HeightOverrideInPixels.HasValue)
				{
					height = base.HeightOverrideInPixels.Value;
				}
				else if (m_map.Height != null)
				{
					height = MappingHelper.ToIntPixels(m_map.Height, base.DpiY);
				}
				m_coreMap.Height = height;
				Stream imageStream = null;
				switch (imageType)
				{
				case DynamicImageInstance.ImageType.EMF:
					GetEmfImage(out imageStream, width, height);
					break;
				case DynamicImageInstance.ImageType.PNG:
					GetPngImage(out imageStream, width, height);
					break;
				}
				imageStream.Position = 0L;
				return imageStream;
			}
			catch (RSException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(ex2))
				{
					throw;
				}
				throw new RenderingObjectModelException(ex2);
			}
		}

		public ActionInfoWithDynamicImageMapCollection GetImageMaps()
		{
			ActionInfoWithDynamicImageMapCollection actionInfoWithDynamicImageMapCollection = MappingHelper.GetImageMaps(GetMapAreaInfoList(), m_actions, m_map);
			ActionInfoWithDynamicImageMap mapImageMap = GetMapImageMap();
			if (mapImageMap != null)
			{
				if (actionInfoWithDynamicImageMapCollection == null)
				{
					actionInfoWithDynamicImageMapCollection = new ActionInfoWithDynamicImageMapCollection();
				}
				actionInfoWithDynamicImageMapCollection.InternalList.Add(mapImageMap);
			}
			return actionInfoWithDynamicImageMapCollection;
		}

		private ActionInfoWithDynamicImageMap GetMapImageMap()
		{
			string href;
			ActionInfoWithDynamicImageMap actionInfoWithDynamicImageMap = MappingHelper.CreateActionInfoDynamic(m_map, m_map.ActionInfo, string.Empty, out href, applyExpression: true);
			actionInfoWithDynamicImageMap?.CreateImageMapAreaInstance(ImageMapArea.ImageMapAreaShape.Rectangle, new float[4]
			{
				0f,
				0f,
				100f,
				100f
			}, string.Empty);
			return actionInfoWithDynamicImageMap;
		}

		internal IEnumerable<MappingHelper.MapAreaInfo> GetMapAreaInfoList()
		{
			m_coreMap.mapCore.PopulateImageMaps();
			float width = m_coreMap.Width;
			float height = m_coreMap.Height;
			foreach (MapArea mapArea in m_coreMap.MapAreas)
			{
				yield return new MappingHelper.MapAreaInfo(mapArea.ToolTip, ((IMapAreaAttributes)mapArea).Tag, GetMapAreaShape(mapArea.Shape), MappingHelper.ConvertCoordinatesToRelative(mapArea.Coordinates, width, height));
			}
		}

		private void InitializeMap()
		{
			m_coreMap = new MapControl();
			m_coreMap.mapCore.UppercaseFieldKeywords = false;
			m_coreMap.mapCore.SetUserLocales(new string[3]
			{
				Localization.ClientBrowserCultureName,
				Localization.ClientCurrentCultureName,
				Localization.ClientPrimaryCulture.Name
			});
			m_coreMap.ShapeFields.Clear();
			m_coreMap.ShapeRules.Clear();
			m_coreMap.Shapes.Clear();
			m_coreMap.SymbolFields.Clear();
			m_coreMap.SymbolRules.Clear();
			m_coreMap.Symbols.Clear();
			m_coreMap.PathFields.Clear();
			m_coreMap.PathRules.Clear();
			m_coreMap.Paths.Clear();
			MapControl coreMap = m_coreMap;
			coreMap.FormatNumberHandler = (FormatNumberHandler)Delegate.Combine(coreMap.FormatNumberHandler, new FormatNumberHandler(FormatNumber));
			_ = RSTrace.ProcessingTracer.TraceVerbose;
		}

		private void RenderViewport()
		{
			RenderSubItem(m_map.MapViewport, m_coreMap.Viewport);
			SetViewportProperties();
			RenderMapLimits();
			RenderMapView();
			RenderGridLines(m_map.MapViewport.MapMeridians, m_coreMap.Meridians);
			RenderGridLines(m_map.MapViewport.MapParallels, m_coreMap.Parallels);
		}

		private void RenderGridLines(MapGridLines mapGridLines, GridAttributes coreGridLines)
		{
			if (mapGridLines != null)
			{
				SetGridLinesProperties(mapGridLines, coreGridLines);
				RenderGridLinesStyle(mapGridLines, coreGridLines);
			}
		}

		private void RenderSubItem(MapSubItem mapSubItem, Panel coreSubItem)
		{
			SetSubItemProperties(mapSubItem, coreSubItem);
			RenderSubItemStyle(mapSubItem, coreSubItem);
			if (mapSubItem != null)
			{
				RenderLocation(mapSubItem.MapLocation, coreSubItem);
			}
			if (mapSubItem != null)
			{
				RenderSize(mapSubItem.MapSize, coreSubItem);
			}
		}

		private void RenderDockableSubItem(MapDockableSubItem mapDockableSubItem, DockablePanel coreSubItem)
		{
			RenderSubItem(mapDockableSubItem, coreSubItem);
			SetDockableSubItemProperties(mapDockableSubItem, coreSubItem);
			RenderActionInfo(mapDockableSubItem.ActionInfo, coreSubItem.ToolTip, coreSubItem, null, hasScope: true);
		}

		private void RenderLegends()
		{
			if (m_map.MapLegends == null)
			{
				return;
			}
			foreach (MapLegend mapLegend in m_map.MapLegends)
			{
				RenderLegend(mapLegend);
			}
		}

		private void RenderLayers()
		{
			if (m_map.MapLayers == null)
			{
				return;
			}
			foreach (MapLayer mapLayer in m_map.MapLayers)
			{
				RenderLayer(mapLayer);
			}
		}

		private void RenderTitles()
		{
			if (m_map.MapTitles == null)
			{
				return;
			}
			foreach (MapTitle mapTitle in m_map.MapTitles)
			{
				RenderTitle(mapTitle);
			}
		}

		private void RenderLegend(MapLegend mapLegend)
		{
			Microsoft.Reporting.Map.WebForms.Legend legend = new Microsoft.Reporting.Map.WebForms.Legend();
			RenderDockableSubItem(mapLegend, legend);
			SetLegendProperties(mapLegend, legend);
			RenderLegendTitle(mapLegend.MapLegendTitle, legend);
			m_coreMap.Legends.Add(legend);
		}

		private void RenderLegendTitle(MapLegendTitle mapLegendTitle, Microsoft.Reporting.Map.WebForms.Legend coreLegend)
		{
			if (mapLegendTitle != null)
			{
				SetLegendTitleProperties(mapLegendTitle, coreLegend);
				RenderLegendTitleStyle(mapLegendTitle, coreLegend);
			}
		}

		private void RenderTitle(MapTitle mapTitle)
		{
			MapLabel mapLabel = new MapLabel();
			RenderDockableSubItem(mapTitle, mapLabel);
			SetTitleProperties(mapTitle, mapLabel);
			m_coreMap.Labels.Add(mapLabel);
		}

		private void RenderLayer(MapLayer mapLayer)
		{
			Layer layer = new Layer();
			SetLayerProperties(mapLayer, layer);
			m_coreMap.Layers.Add(layer);
			if (mapLayer is MapTileLayer)
			{
				RenderTileLayer((MapTileLayer)mapLayer);
			}
			else if (mapLayer is MapVectorLayer)
			{
				RenderVectorLayer((MapVectorLayer)mapLayer);
			}
		}

		private void SetLayerProperties(MapLayer mapLayer, Layer coreLayer)
		{
			coreLayer.Name = mapLayer.Name;
			ReportDoubleProperty transparency = mapLayer.Transparency;
			if (transparency != null)
			{
				if (!transparency.IsExpression)
				{
					coreLayer.Transparency = (float)transparency.Value;
				}
				else
				{
					coreLayer.Transparency = (float)mapLayer.Instance.Transparency;
				}
			}
			else
			{
				coreLayer.Transparency = 0f;
			}
			ReportDoubleProperty maximumZoom = mapLayer.MaximumZoom;
			if (maximumZoom != null)
			{
				if (!maximumZoom.IsExpression)
				{
					coreLayer.VisibleToZoom = (float)maximumZoom.Value;
				}
				else
				{
					coreLayer.VisibleToZoom = (float)mapLayer.Instance.MaximumZoom;
				}
			}
			else
			{
				coreLayer.VisibleToZoom = 200f;
			}
			ReportDoubleProperty minimumZoom = mapLayer.MinimumZoom;
			if (minimumZoom != null)
			{
				if (!minimumZoom.IsExpression)
				{
					coreLayer.VisibleFromZoom = (float)minimumZoom.Value;
				}
				else
				{
					coreLayer.VisibleFromZoom = (float)mapLayer.Instance.MinimumZoom;
				}
			}
			else
			{
				coreLayer.VisibleFromZoom = 50f;
			}
			ReportEnumProperty<MapVisibilityMode> visibilityMode = mapLayer.VisibilityMode;
			if (visibilityMode != null)
			{
				if (!visibilityMode.IsExpression)
				{
					coreLayer.Visibility = GetLayerVisibility(visibilityMode.Value);
				}
				else
				{
					coreLayer.Visibility = GetLayerVisibility(mapLayer.Instance.VisibilityMode);
				}
			}
			else
			{
				coreLayer.Visibility = LayerVisibility.Shown;
			}
		}

		private void RenderVectorLayer(MapVectorLayer mapVectorLayer)
		{
			if (mapVectorLayer is MapPolygonLayer)
			{
				new PolygonLayerMapper((MapPolygonLayer)mapVectorLayer, m_coreMap, this).Render();
			}
			else if (mapVectorLayer is MapPointLayer)
			{
				new PointLayerMapper((MapPointLayer)mapVectorLayer, m_coreMap, this).Render();
			}
			else if (mapVectorLayer is MapLineLayer)
			{
				new LineLayerMapper((MapLineLayer)mapVectorLayer, m_coreMap, this).Render();
			}
		}

		private void RenderTileLayer(MapTileLayer mapTileLayer)
		{
			if (m_tileLayerMapper == null)
			{
				m_tileLayerMapper = new TileLayerMapper(m_map, m_coreMap);
			}
			m_tileLayerMapper.AddLayer(mapTileLayer);
		}

		private void RenderDistanceScale()
		{
			if (m_map.MapDistanceScale != null)
			{
				RenderDockableSubItem(m_map.MapDistanceScale, m_coreMap.DistanceScalePanel);
				SetDistanceScaleProperties();
			}
		}

		private void RenderColorScale()
		{
			if (m_map.MapColorScale != null)
			{
				RenderDockableSubItem(m_map.MapColorScale, m_coreMap.ColorSwatchPanel);
				SetColorScaleProperties();
				RenderColorScaleTitle();
			}
		}

		private void RenderColorScaleTitle()
		{
			SetColorScaleTitleProperties();
			RenderColorScaleTitleStyle();
		}

		private void RenderBorderSkin()
		{
			if (m_map.MapBorderSkin != null)
			{
				SetBorderSkinProperties();
				RenderBorderSkinStyle();
			}
		}

		private void SetSubItemProperties(MapSubItem mapSubItem, Panel coreSubItem)
		{
			ReportSizeProperty leftMargin = mapSubItem.LeftMargin;
			if (leftMargin != null)
			{
				if (!leftMargin.IsExpression)
				{
					coreSubItem.Margins.Left = MappingHelper.ToIntPixels(leftMargin.Value, base.DpiX);
				}
				else
				{
					coreSubItem.Margins.Left = MappingHelper.ToIntPixels(mapSubItem.Instance.LeftMargin, base.DpiX);
				}
			}
			leftMargin = mapSubItem.TopMargin;
			if (leftMargin != null)
			{
				if (!leftMargin.IsExpression)
				{
					coreSubItem.Margins.Top = MappingHelper.ToIntPixels(leftMargin.Value, base.DpiY);
				}
				else
				{
					coreSubItem.Margins.Top = MappingHelper.ToIntPixels(mapSubItem.Instance.TopMargin, base.DpiY);
				}
			}
			leftMargin = mapSubItem.RightMargin;
			if (leftMargin != null)
			{
				if (!leftMargin.IsExpression)
				{
					coreSubItem.Margins.Right = MappingHelper.ToIntPixels(leftMargin.Value, base.DpiX);
				}
				else
				{
					coreSubItem.Margins.Right = MappingHelper.ToIntPixels(mapSubItem.Instance.RightMargin, base.DpiX);
				}
			}
			leftMargin = mapSubItem.BottomMargin;
			if (leftMargin != null)
			{
				if (!leftMargin.IsExpression)
				{
					coreSubItem.Margins.Bottom = MappingHelper.ToIntPixels(leftMargin.Value, base.DpiX);
				}
				else
				{
					coreSubItem.Margins.Bottom = MappingHelper.ToIntPixels(mapSubItem.Instance.BottomMargin, base.DpiX);
				}
			}
			ReportIntProperty zIndex = mapSubItem.ZIndex;
			if (zIndex != null)
			{
				if (!zIndex.IsExpression)
				{
					coreSubItem.ZOrder = zIndex.Value;
				}
				else
				{
					coreSubItem.ZOrder = mapSubItem.Instance.ZIndex;
				}
			}
		}

		private void RenderLocation(MapLocation mapLocation, Panel coreSubItem)
		{
			if (mapLocation == null)
			{
				return;
			}
			ReportEnumProperty<Unit> unit = mapLocation.Unit;
			Unit unit2 = Unit.Percentage;
			if (unit != null)
			{
				unit2 = (unit.IsExpression ? mapLocation.Instance.Unit : unit.Value);
			}
			coreSubItem.LocationUnit = ((unit2 == Unit.Percentage) ? CoordinateUnit.Percent : CoordinateUnit.Pixel);
			ReportDoubleProperty left = mapLocation.Left;
			if (left != null)
			{
				double num = left.IsExpression ? mapLocation.Instance.Left : left.Value;
				if (unit2 != 0)
				{
					num = MappingHelper.ToPixels(num, unit2, base.DpiX);
				}
				coreSubItem.Location.X = (float)num;
			}
			left = mapLocation.Top;
			if (left != null)
			{
				double num = left.IsExpression ? mapLocation.Instance.Top : left.Value;
				if (unit2 != 0)
				{
					num = MappingHelper.ToPixels(num, unit2, base.DpiY);
				}
				coreSubItem.Location.Y = (float)num;
			}
		}

		private void RenderSize(MapSize mapSize, Panel coreSubItem)
		{
			if (mapSize == null)
			{
				return;
			}
			ReportEnumProperty<Unit> unit = mapSize.Unit;
			Unit unit2 = Unit.Percentage;
			if (unit != null)
			{
				unit2 = (unit.IsExpression ? mapSize.Instance.Unit : unit.Value);
			}
			coreSubItem.SizeUnit = ((unit2 == Unit.Percentage) ? CoordinateUnit.Percent : CoordinateUnit.Pixel);
			ReportDoubleProperty width = mapSize.Width;
			if (width != null)
			{
				double num = width.IsExpression ? mapSize.Instance.Width : width.Value;
				if (unit2 != 0)
				{
					num = MappingHelper.ToPixels(num, unit2, base.DpiX);
				}
				coreSubItem.Size.Width = (float)num;
			}
			width = mapSize.Height;
			if (width != null)
			{
				double num = width.IsExpression ? mapSize.Instance.Height : width.Value;
				if (unit2 != 0)
				{
					num = MappingHelper.ToPixels(num, unit2, base.DpiY);
				}
				coreSubItem.Size.Height = (float)num;
			}
		}

		private void RenderMapLimits()
		{
			MapLimits mapLimits = m_map.MapViewport.MapLimits;
			if (mapLimits == null)
			{
				return;
			}
			ReportDoubleProperty minimumX = mapLimits.MinimumX;
			if (minimumX != null)
			{
				if (!minimumX.IsExpression)
				{
					m_coreMap.MapLimits.MinimumX = minimumX.Value;
				}
				else
				{
					m_coreMap.MapLimits.MinimumX = mapLimits.Instance.MinimumX;
				}
			}
			minimumX = mapLimits.MinimumY;
			if (minimumX != null)
			{
				if (!minimumX.IsExpression)
				{
					m_coreMap.MapLimits.MinimumY = minimumX.Value;
				}
				else
				{
					m_coreMap.MapLimits.MinimumY = mapLimits.Instance.MinimumY;
				}
			}
			minimumX = mapLimits.MaximumX;
			if (minimumX != null)
			{
				if (!minimumX.IsExpression)
				{
					m_coreMap.MapLimits.MaximumX = minimumX.Value;
				}
				else
				{
					m_coreMap.MapLimits.MaximumX = mapLimits.Instance.MaximumX;
				}
			}
			minimumX = mapLimits.MaximumY;
			if (minimumX != null)
			{
				if (!minimumX.IsExpression)
				{
					m_coreMap.MapLimits.MaximumY = minimumX.Value;
				}
				else
				{
					m_coreMap.MapLimits.MaximumY = mapLimits.Instance.MaximumY;
				}
			}
		}

		private void RenderMapView()
		{
			MapView mapView = m_map.MapViewport.MapView;
			if (mapView != null)
			{
				ReportDoubleProperty zoom = mapView.Zoom;
				double num = 0.0;
				if (zoom != null)
				{
					num = (zoom.IsExpression ? ((double)(float)mapView.Instance.Zoom) : ((double)(float)zoom.Value));
				}
				if (num != 0.0)
				{
					m_coreMap.Viewport.Zoom = (float)num;
				}
				if (mapView is MapCustomView)
				{
					RenderCustomView((MapCustomView)mapView);
				}
				else if (m_boundRectCalculator != null)
				{
					CenterView(num == 0.0);
				}
			}
		}

		private void CenterView(bool zoomToFit)
		{
			if (zoomToFit)
			{
				m_coreMap.MapLimits.MinimumX = m_boundRectCalculator.Min.X;
				m_coreMap.MapLimits.MinimumY = m_boundRectCalculator.Min.Y;
				m_coreMap.MapLimits.MaximumX = m_boundRectCalculator.Max.X;
				m_coreMap.MapLimits.MaximumY = m_boundRectCalculator.Max.Y;
				m_coreMap.Viewport.Zoom = 100f;
			}
			else
			{
				m_coreMap.CenterView(m_boundRectCalculator.Center);
			}
		}

		internal void AddSpatialElementToView(ISpatialElement spatialElement)
		{
			if (m_boundRectCalculator == null)
			{
				m_boundRectCalculator = new BoundsRectCalculator();
			}
			m_boundRectCalculator.AddSpatialElement(spatialElement);
		}

		private void RenderCustomView(MapCustomView mapView)
		{
			ReportDoubleProperty centerX = mapView.CenterX;
			if (centerX != null)
			{
				if (!centerX.IsExpression)
				{
					m_coreMap.Viewport.ViewCenter.X = (float)centerX.Value;
				}
				else
				{
					m_coreMap.Viewport.ViewCenter.X = (float)mapView.Instance.CenterX;
				}
			}
			centerX = mapView.CenterY;
			if (centerX != null)
			{
				if (!centerX.IsExpression)
				{
					m_coreMap.Viewport.ViewCenter.Y = (float)centerX.Value;
				}
				else
				{
					m_coreMap.Viewport.ViewCenter.Y = (float)mapView.Instance.CenterY;
				}
			}
		}

		private void SetDockableSubItemProperties(MapDockableSubItem mapDockableSubItem, DockablePanel coreDockableSubItem)
		{
			_ = mapDockableSubItem.Position;
			MapPosition position = MapPosition.TopCenter;
			if (mapDockableSubItem.Position != null)
			{
				position = (mapDockableSubItem.Position.IsExpression ? mapDockableSubItem.Instance.Position : mapDockableSubItem.Position.Value);
			}
			coreDockableSubItem.DockAlignment = GetDockablePanelAlignment(position);
			coreDockableSubItem.Dock = ((mapDockableSubItem.MapLocation == null) ? GetDockablePanelDocking(position) : PanelDockStyle.None);
			ReportBoolProperty dockOutsideViewport = mapDockableSubItem.DockOutsideViewport;
			if (dockOutsideViewport != null)
			{
				if (!dockOutsideViewport.IsExpression)
				{
					coreDockableSubItem.DockedInsideViewport = !dockOutsideViewport.Value;
				}
				else
				{
					coreDockableSubItem.DockedInsideViewport = !mapDockableSubItem.Instance.DockOutsideViewport;
				}
			}
			else
			{
				coreDockableSubItem.DockedInsideViewport = true;
			}
			ReportBoolProperty hidden = mapDockableSubItem.Hidden;
			if (hidden != null)
			{
				if (!hidden.IsExpression)
				{
					coreDockableSubItem.Visible = !hidden.Value;
				}
				else
				{
					coreDockableSubItem.Visible = !mapDockableSubItem.Instance.Hidden;
				}
			}
			else
			{
				coreDockableSubItem.Visible = true;
			}
			ReportStringProperty toolTip = mapDockableSubItem.ToolTip;
			if (toolTip != null)
			{
				if (!toolTip.IsExpression)
				{
					coreDockableSubItem.ToolTip = toolTip.Value;
				}
				else
				{
					coreDockableSubItem.ToolTip = mapDockableSubItem.Instance.ToolTip;
				}
			}
		}

		private DockAlignment GetDockablePanelAlignment(MapPosition position)
		{
			switch (position)
			{
			case MapPosition.TopCenter:
			case MapPosition.LeftCenter:
			case MapPosition.RightCenter:
			case MapPosition.BottomCenter:
				return DockAlignment.Center;
			case MapPosition.TopRight:
			case MapPosition.LeftBottom:
			case MapPosition.RightBottom:
			case MapPosition.BottomRight:
				return DockAlignment.Far;
			default:
				return DockAlignment.Near;
			}
		}

		private PanelDockStyle GetDockablePanelDocking(MapPosition position)
		{
			switch (position)
			{
			case MapPosition.BottomRight:
			case MapPosition.BottomCenter:
			case MapPosition.BottomLeft:
				return PanelDockStyle.Bottom;
			case MapPosition.TopCenter:
			case MapPosition.TopLeft:
			case MapPosition.TopRight:
				return PanelDockStyle.Top;
			case MapPosition.LeftTop:
			case MapPosition.LeftCenter:
			case MapPosition.LeftBottom:
				return PanelDockStyle.Left;
			default:
				return PanelDockStyle.Right;
			}
		}

		private void SetMapProperties()
		{
			if (m_map.AntiAliasing != null)
			{
				if (!m_map.AntiAliasing.IsExpression)
				{
					m_coreMap.AntiAliasing = GetAntiAliasing(m_map.AntiAliasing.Value);
				}
				else
				{
					m_coreMap.AntiAliasing = GetAntiAliasing(m_map.Instance.AntiAliasing);
				}
			}
			if (m_map.ShadowIntensity != null)
			{
				if (!m_map.ShadowIntensity.IsExpression)
				{
					m_coreMap.ShadowIntensity = (float)m_map.ShadowIntensity.Value;
				}
				else
				{
					m_coreMap.ShadowIntensity = (float)m_map.Instance.ShadowIntensity;
				}
			}
			if (m_map.TextAntiAliasingQuality != null)
			{
				if (!m_map.TextAntiAliasingQuality.IsExpression)
				{
					m_coreMap.TextAntiAliasingQuality = GetTextAntiAliasingQuality(m_map.TextAntiAliasingQuality.Value);
				}
				else
				{
					m_coreMap.TextAntiAliasingQuality = GetTextAntiAliasingQuality(m_map.Instance.TextAntiAliasingQuality);
				}
			}
			m_remainingSpatialElementCount = m_map.MaximumSpatialElementCount;
			m_remainingTotalPointCount = m_map.MaximumTotalPointCount;
			SetTileServerConfiguration();
		}

		private void SetTileServerConfiguration()
		{
			IConfiguration configuration = m_map.RenderingContext.OdpContext.Configuration;
			IMapTileServerConfiguration mapTileServerConfiguration = null;
			if (configuration != null)
			{
				mapTileServerConfiguration = configuration.MapTileServerConfiguration;
			}
			if (mapTileServerConfiguration != null)
			{
				m_coreMap.TileServerMaxConnections = mapTileServerConfiguration.MaxConnections;
				m_coreMap.TileServerTimeout = mapTileServerConfiguration.Timeout * 1000;
				m_coreMap.TileServerAppId = mapTileServerConfiguration.AppID;
				m_coreMap.TileCacheLevel = MapTileServerConsts.ConvertFromMapTileCacheLevel(mapTileServerConfiguration.CacheLevel);
				m_coreMap.TileCulture = GetTileLanguage();
			}
		}

		private CultureInfo GetTileLanguage()
		{
			return new Formatter(m_map.MapDef.StyleClass, m_map.RenderingContext.OdpContext, m_map.MapDef.ObjectType, m_map.Name).GetCulture(EvaluateLanguage());
		}

		private string EvaluateLanguage()
		{
			ReportStringProperty tileLanguage = m_map.TileLanguage;
			if (tileLanguage != null)
			{
				if (!tileLanguage.IsExpression)
				{
					return tileLanguage.Value;
				}
				return m_map.Instance.TileLanguage;
			}
			if (m_map.Style != null)
			{
				tileLanguage = m_map.Style.Language;
				if (tileLanguage != null)
				{
					if (!tileLanguage.IsExpression)
					{
						return tileLanguage.Value;
					}
					return m_map.Instance.Style.Language;
				}
			}
			return null;
		}

		private AntiAliasing GetAntiAliasing(MapAntiAliasing mapAntiAliasing)
		{
			switch (mapAntiAliasing)
			{
			case MapAntiAliasing.Graphics:
				return AntiAliasing.Graphics;
			case MapAntiAliasing.None:
				return AntiAliasing.None;
			case MapAntiAliasing.Text:
				return AntiAliasing.Text;
			default:
				return AntiAliasing.All;
			}
		}

		private TextAntiAliasingQuality GetTextAntiAliasingQuality(MapTextAntiAliasingQuality textAntiAliasingQuality)
		{
			switch (textAntiAliasingQuality)
			{
			case MapTextAntiAliasingQuality.Normal:
				return TextAntiAliasingQuality.Normal;
			case MapTextAntiAliasingQuality.SystemDefault:
				return TextAntiAliasingQuality.SystemDefault;
			default:
				return TextAntiAliasingQuality.High;
			}
		}

		private void SetViewportProperties()
		{
			MapViewport mapViewport = m_map.MapViewport;
			m_coreMap.Viewport.AutoSize = (mapViewport.MapSize == null && mapViewport.MapLocation == null);
			ReportEnumProperty<MapCoordinateSystem> mapCoordinateSystem = mapViewport.MapCoordinateSystem;
			MapCoordinateSystem mapCoordinateSystem2 = (mapCoordinateSystem != null) ? (mapCoordinateSystem.IsExpression ? mapViewport.Instance.MapCoordinateSystem : mapCoordinateSystem.Value) : MapCoordinateSystem.Planar;
			m_coreMap.GeographyMode = (mapCoordinateSystem2 == MapCoordinateSystem.Geographic);
			ReportEnumProperty<MapProjection> mapProjection = mapViewport.MapProjection;
			MapProjection projection = (mapProjection != null) ? (mapProjection.IsExpression ? mapViewport.Instance.MapProjection : mapProjection.Value) : MapProjection.Equirectangular;
			m_coreMap.Projection = GetProjection(projection);
			ReportDoubleProperty projectionCenterX = mapViewport.ProjectionCenterX;
			if (projectionCenterX != null)
			{
				if (!projectionCenterX.IsExpression)
				{
					m_coreMap.ProjectionCenter.X = projectionCenterX.Value;
				}
				else
				{
					m_coreMap.ProjectionCenter.X = mapViewport.Instance.ProjectionCenterX;
				}
			}
			projectionCenterX = mapViewport.ProjectionCenterY;
			if (projectionCenterX != null)
			{
				if (!projectionCenterX.IsExpression)
				{
					m_coreMap.ProjectionCenter.Y = projectionCenterX.Value;
				}
				else
				{
					m_coreMap.ProjectionCenter.Y = mapViewport.Instance.ProjectionCenterY;
				}
			}
			ReportDoubleProperty maximumZoom = mapViewport.MaximumZoom;
			if (maximumZoom != null)
			{
				if (!maximumZoom.IsExpression)
				{
					m_coreMap.Viewport.MaximumZoom = (int)Math.Round(maximumZoom.Value);
				}
				else
				{
					m_coreMap.Viewport.MaximumZoom = (int)Math.Round(mapViewport.Instance.MaximumZoom);
				}
			}
			maximumZoom = mapViewport.MinimumZoom;
			if (maximumZoom != null)
			{
				if (!maximumZoom.IsExpression)
				{
					m_coreMap.Viewport.MinimumZoom = (int)Math.Round(maximumZoom.Value);
				}
				else
				{
					m_coreMap.Viewport.MinimumZoom = (int)Math.Round(mapViewport.Instance.MinimumZoom);
				}
			}
			ReportSizeProperty contentMargin = mapViewport.ContentMargin;
			ReportSize size = (contentMargin == null) ? m_defaultContentMargin : (contentMargin.IsExpression ? mapViewport.Instance.ContentMargin : mapViewport.ContentMargin.Value);
			m_coreMap.Viewport.ContentAutoFitMargin = MappingHelper.ToIntPixels(size, base.DpiX);
			ReportBoolProperty gridUnderContent = mapViewport.GridUnderContent;
			if (gridUnderContent != null)
			{
				if (!gridUnderContent.IsExpression)
				{
					m_coreMap.GridUnderContent = gridUnderContent.Value;
				}
				else
				{
					m_coreMap.GridUnderContent = gridUnderContent.Value;
				}
			}
			else
			{
				m_coreMap.GridUnderContent = false;
			}
		}

		private Projection GetProjection(MapProjection projection)
		{
			switch (projection)
			{
			case MapProjection.Bonne:
				return Projection.Bonne;
			case MapProjection.Eckert1:
				return Projection.Eckert1;
			case MapProjection.Eckert3:
				return Projection.Eckert3;
			case MapProjection.Fahey:
				return Projection.Fahey;
			case MapProjection.HammerAitoff:
				return Projection.HammerAitoff;
			case MapProjection.Mercator:
				return Projection.Mercator;
			case MapProjection.Robinson:
				return Projection.Robinson;
			case MapProjection.Wagner3:
				return Projection.Wagner3;
			default:
				return Projection.Equirectangular;
			}
		}

		private void SetGridLinesProperties(MapGridLines mapGridLines, GridAttributes coreGridLines)
		{
			if (mapGridLines == null)
			{
				return;
			}
			ReportBoolProperty hidden = mapGridLines.Hidden;
			if (hidden != null)
			{
				if (!hidden.IsExpression)
				{
					coreGridLines.Visible = !hidden.Value;
				}
				else
				{
					coreGridLines.Visible = !mapGridLines.Instance.Hidden;
				}
			}
			ReportDoubleProperty interval = mapGridLines.Interval;
			if (interval != null)
			{
				if (!interval.IsExpression)
				{
					coreGridLines.Interval = interval.Value;
				}
				else
				{
					coreGridLines.Interval = mapGridLines.Instance.Interval;
				}
			}
			ReportBoolProperty showLabels = mapGridLines.ShowLabels;
			if (showLabels != null)
			{
				if (!showLabels.IsExpression)
				{
					coreGridLines.ShowLabels = showLabels.Value;
				}
				else
				{
					coreGridLines.ShowLabels = mapGridLines.Instance.ShowLabels;
				}
			}
			ReportEnumProperty<MapLabelPosition> labelPosition = mapGridLines.LabelPosition;
			MapLabelPosition labelPosition2 = MapLabelPosition.Near;
			if (labelPosition != null)
			{
				labelPosition2 = (labelPosition.IsExpression ? mapGridLines.Instance.LabelPosition : labelPosition.Value);
			}
			coreGridLines.LabelPosition = GetLabelPosition(labelPosition2);
		}

		private LabelPosition GetLabelPosition(MapLabelPosition labelPosition)
		{
			switch (labelPosition)
			{
			case MapLabelPosition.Center:
				return LabelPosition.Center;
			case MapLabelPosition.Far:
				return LabelPosition.Far;
			case MapLabelPosition.OneQuarter:
				return LabelPosition.OneQuarter;
			case MapLabelPosition.ThreeQuarters:
				return LabelPosition.ThreeQuarters;
			default:
				return LabelPosition.Near;
			}
		}

		private void SetLegendProperties(MapLegend mapLegend, Microsoft.Reporting.Map.WebForms.Legend legend)
		{
			legend.MaxAutoSize = 50f;
			Style style = mapLegend.Style;
			if (style == null)
			{
				legend.Font = GetDefaultFontFromCache(0);
			}
			else
			{
				StyleInstance style2 = mapLegend.Instance.Style;
				legend.Font = GetFontFromCache(0, style, style2);
				legend.TextColor = MappingHelper.GetStyleColor(style, style2);
			}
			legend.AutoSize = (mapLegend.MapSize == null);
			if (mapLegend.Hidden != null)
			{
				if (!mapLegend.Hidden.IsExpression)
				{
					legend.Visible = !mapLegend.Hidden.Value;
				}
				else
				{
					legend.Visible = !mapLegend.Instance.Hidden;
				}
			}
			if (mapLegend.Layout != null)
			{
				if (!mapLegend.Layout.IsExpression)
				{
					SetLegendLayout(mapLegend.Layout.Value, legend);
				}
				else
				{
					SetLegendLayout(mapLegend.Instance.Layout, legend);
				}
			}
			if (mapLegend.AutoFitTextDisabled != null)
			{
				if (!mapLegend.AutoFitTextDisabled.IsExpression)
				{
					legend.AutoFitText = !mapLegend.AutoFitTextDisabled.Value;
				}
				else
				{
					legend.AutoFitText = !mapLegend.Instance.AutoFitTextDisabled;
				}
			}
			else
			{
				legend.AutoFitText = true;
			}
			if (mapLegend.EquallySpacedItems != null)
			{
				if (!mapLegend.EquallySpacedItems.IsExpression)
				{
					legend.EquallySpacedItems = mapLegend.EquallySpacedItems.Value;
				}
				else
				{
					legend.EquallySpacedItems = mapLegend.Instance.EquallySpacedItems;
				}
			}
			if (mapLegend.InterlacedRows != null)
			{
				if (!mapLegend.InterlacedRows.IsExpression)
				{
					legend.InterlacedRows = mapLegend.InterlacedRows.Value;
				}
				else
				{
					legend.InterlacedRows = mapLegend.Instance.InterlacedRows;
				}
			}
			if (mapLegend.InterlacedRowsColor != null)
			{
				Color color = Color.Empty;
				if (MappingHelper.GetColorFromReportColorProperty(mapLegend.InterlacedRowsColor, ref color))
				{
					legend.InterlacedRowsColor = color;
				}
				else if (mapLegend.Instance.InterlacedRowsColor != null)
				{
					legend.InterlacedRowsColor = mapLegend.Instance.InterlacedRowsColor.ToColor();
				}
			}
			if (mapLegend.MinFontSize != null)
			{
				if (!mapLegend.MinFontSize.IsExpression)
				{
					legend.AutoFitMinFontSize = (int)Math.Round(mapLegend.MinFontSize.Value.ToPoints());
				}
				else
				{
					legend.AutoFitMinFontSize = (int)Math.Round(mapLegend.Instance.MinFontSize.ToPoints());
				}
			}
			if (mapLegend.TextWrapThreshold != null)
			{
				if (!mapLegend.TextWrapThreshold.IsExpression)
				{
					legend.TextWrapThreshold = mapLegend.TextWrapThreshold.Value;
				}
				else
				{
					legend.TextWrapThreshold = mapLegend.Instance.TextWrapThreshold;
				}
			}
		}

		private void SetLegendLayout(MapLegendLayout layout, Microsoft.Reporting.Map.WebForms.Legend legend)
		{
			switch (layout)
			{
			case MapLegendLayout.Row:
				legend.LegendStyle = LegendStyle.Row;
				break;
			case MapLegendLayout.Column:
				legend.LegendStyle = LegendStyle.Column;
				break;
			case MapLegendLayout.AutoTable:
				legend.LegendStyle = LegendStyle.Table;
				legend.TableStyle = LegendTableStyle.Auto;
				break;
			case MapLegendLayout.TallTable:
				legend.LegendStyle = LegendStyle.Table;
				legend.TableStyle = LegendTableStyle.Tall;
				break;
			case MapLegendLayout.WideTable:
				legend.LegendStyle = LegendStyle.Table;
				legend.TableStyle = LegendTableStyle.Wide;
				break;
			}
		}

		private void SetLegendTitleProperties(MapLegendTitle mapLegendTitle, Microsoft.Reporting.Map.WebForms.Legend legend)
		{
			if (mapLegendTitle.Caption != null)
			{
				if (!mapLegendTitle.Caption.IsExpression)
				{
					if (mapLegendTitle.Caption.Value != null)
					{
						legend.Title = mapLegendTitle.Caption.Value;
					}
				}
				else if (mapLegendTitle.Instance.Caption != null)
				{
					legend.Title = mapLegendTitle.Instance.Caption;
				}
			}
			if (mapLegendTitle.TitleSeparator != null)
			{
				if (!mapLegendTitle.TitleSeparator.IsExpression)
				{
					legend.TitleSeparator = GetLegendSeparatorStyle(mapLegendTitle.TitleSeparator.Value);
				}
				else
				{
					legend.TitleSeparator = GetLegendSeparatorStyle(mapLegendTitle.Instance.TitleSeparator);
				}
			}
		}

		private LegendSeparatorType GetLegendSeparatorStyle(MapLegendTitleSeparator legendTitleSeparator)
		{
			switch (legendTitleSeparator)
			{
			case MapLegendTitleSeparator.DashLine:
				return LegendSeparatorType.DashLine;
			case MapLegendTitleSeparator.DotLine:
				return LegendSeparatorType.DotLine;
			case MapLegendTitleSeparator.DoubleLine:
				return LegendSeparatorType.DoubleLine;
			case MapLegendTitleSeparator.GradientLine:
				return LegendSeparatorType.GradientLine;
			case MapLegendTitleSeparator.Line:
				return LegendSeparatorType.Line;
			case MapLegendTitleSeparator.ThickGradientLine:
				return LegendSeparatorType.ThickGradientLine;
			case MapLegendTitleSeparator.ThickLine:
				return LegendSeparatorType.ThickLine;
			default:
				return LegendSeparatorType.None;
			}
		}

		private void SetTitleProperties(MapTitle mapTitle, MapLabel coreTitle)
		{
			Style style = mapTitle.Style;
			if (style == null)
			{
				coreTitle.Font = GetDefaultFontFromCache(0);
			}
			else
			{
				StyleInstance style2 = mapTitle.Instance.Style;
				coreTitle.Font = GetFontFromCache(0, style, style2);
				coreTitle.TextColor = MappingHelper.GetStyleColor(style, style2);
				coreTitle.TextAlignment = MappingHelper.GetStyleContentAlignment(style, style2);
			}
			coreTitle.AutoSize = (mapTitle.MapSize == null);
			coreTitle.Name = mapTitle.Name;
			ReportDoubleProperty angle = mapTitle.Angle;
			if (angle != null)
			{
				if (!angle.IsExpression)
				{
					coreTitle.Angle = (float)angle.Value;
				}
				else
				{
					coreTitle.Angle = (float)mapTitle.Instance.Angle;
				}
			}
			ReportStringProperty text = mapTitle.Text;
			if (text != null)
			{
				if (!text.IsExpression)
				{
					if (text.Value != null)
					{
						coreTitle.Text = text.Value;
					}
				}
				else
				{
					string text2 = mapTitle.Instance.Text;
					if (text2 != null)
					{
						coreTitle.Text = text2;
					}
				}
			}
			ReportSizeProperty textShadowOffset = mapTitle.TextShadowOffset;
			if (textShadowOffset != null)
			{
				int shadowOffset = textShadowOffset.IsExpression ? MappingHelper.ToIntPixels(mapTitle.Instance.TextShadowOffset, base.DpiX) : MappingHelper.ToIntPixels(textShadowOffset.Value, base.DpiX);
				coreTitle.TextShadowOffset = GetValidShadowOffset(shadowOffset);
			}
		}

		private void SetDistanceScaleProperties()
		{
			MapDistanceScale mapDistanceScale = m_map.MapDistanceScale;
			Style style = mapDistanceScale.Style;
			if (style == null)
			{
				m_coreMap.DistanceScalePanel.Font = GetDefaultFontFromCache(0);
			}
			else
			{
				StyleInstance style2 = mapDistanceScale.Instance.Style;
				m_coreMap.DistanceScalePanel.Font = GetFontFromCache(0, style, style2);
				m_coreMap.DistanceScalePanel.LabelColor = MappingHelper.GetStyleColor(style, style2);
			}
			ReportColorProperty scaleColor = mapDistanceScale.ScaleColor;
			Color color = Color.Empty;
			if (scaleColor != null)
			{
				if (MappingHelper.GetColorFromReportColorProperty(scaleColor, ref color))
				{
					m_coreMap.DistanceScalePanel.ScaleForeColor = color;
				}
				else
				{
					ReportColor scaleColor2 = mapDistanceScale.Instance.ScaleColor;
					if (scaleColor2 != null)
					{
						m_coreMap.DistanceScalePanel.ScaleForeColor = scaleColor2.ToColor();
					}
				}
			}
			else
			{
				m_coreMap.DistanceScalePanel.ScaleForeColor = Color.White;
			}
			scaleColor = mapDistanceScale.ScaleBorderColor;
			if (scaleColor != null)
			{
				if (MappingHelper.GetColorFromReportColorProperty(scaleColor, ref color))
				{
					m_coreMap.DistanceScalePanel.ScaleBorderColor = color;
				}
				else
				{
					ReportColor scaleBorderColor = mapDistanceScale.Instance.ScaleBorderColor;
					if (scaleBorderColor != null)
					{
						m_coreMap.DistanceScalePanel.ScaleBorderColor = scaleBorderColor.ToColor();
					}
				}
			}
			else
			{
				m_coreMap.DistanceScalePanel.ScaleBorderColor = Color.DarkGray;
			}
			if (!m_coreMap.GeographyMode)
			{
				m_coreMap.DistanceScalePanel.Visible = false;
			}
		}

		private void SetColorScaleProperties()
		{
			MapColorScale mapColorScale = m_map.MapColorScale;
			Style style = mapColorScale.Style;
			if (style == null)
			{
				m_coreMap.ColorSwatchPanel.Font = GetDefaultFontFromCache(0);
			}
			else
			{
				StyleInstance style2 = mapColorScale.Instance.Style;
				m_coreMap.ColorSwatchPanel.Font = GetFontFromCache(0, style, style2);
				m_coreMap.ColorSwatchPanel.LabelColor = MappingHelper.GetStyleColor(style, style2);
			}
			m_coreMap.ColorSwatchPanel.AutoSize = (mapColorScale.MapSize == null);
			ReportSizeProperty tickMarkLength = mapColorScale.TickMarkLength;
			ReportSize size = (tickMarkLength == null) ? m_defaultTickMarkLength : (tickMarkLength.IsExpression ? mapColorScale.Instance.TickMarkLength : tickMarkLength.Value);
			m_coreMap.ColorSwatchPanel.TickMarkLength = MappingHelper.ToIntPixels(size, base.DpiX);
			ReportColorProperty colorBarBorderColor = mapColorScale.ColorBarBorderColor;
			Color color = Color.Empty;
			if (colorBarBorderColor != null)
			{
				if (MappingHelper.GetColorFromReportColorProperty(colorBarBorderColor, ref color))
				{
					m_coreMap.ColorSwatchPanel.OutlineColor = color;
				}
				else if (mapColorScale.Instance.ColorBarBorderColor != null)
				{
					m_coreMap.ColorSwatchPanel.OutlineColor = mapColorScale.Instance.ColorBarBorderColor.ToColor();
				}
			}
			else
			{
				m_coreMap.ColorSwatchPanel.OutlineColor = Color.Black;
			}
			colorBarBorderColor = mapColorScale.RangeGapColor;
			if (colorBarBorderColor != null)
			{
				if (MappingHelper.GetColorFromReportColorProperty(colorBarBorderColor, ref color))
				{
					m_coreMap.ColorSwatchPanel.RangeGapColor = color;
				}
				else if (mapColorScale.Instance.RangeGapColor != null)
				{
					m_coreMap.ColorSwatchPanel.RangeGapColor = mapColorScale.Instance.RangeGapColor.ToColor();
				}
			}
			else
			{
				m_coreMap.ColorSwatchPanel.RangeGapColor = Color.White;
			}
			ReportIntProperty labelInterval = mapColorScale.LabelInterval;
			if (labelInterval != null)
			{
				if (!labelInterval.IsExpression)
				{
					m_coreMap.ColorSwatchPanel.LabelInterval = labelInterval.Value;
				}
				else
				{
					m_coreMap.ColorSwatchPanel.LabelInterval = mapColorScale.Instance.LabelInterval;
				}
			}
			ReportStringProperty labelFormat = mapColorScale.LabelFormat;
			string text = null;
			if (labelFormat != null)
			{
				text = (labelFormat.IsExpression ? mapColorScale.Instance.LabelFormat : labelFormat.Value);
			}
			if (text != null)
			{
				m_coreMap.ColorSwatchPanel.NumericLabelFormat = text;
			}
			ReportEnumProperty<MapLabelPlacement> labelPlacement = mapColorScale.LabelPlacement;
			if (labelPlacement != null)
			{
				if (!labelPlacement.IsExpression)
				{
					m_coreMap.ColorSwatchPanel.LabelAlignment = GetLabelAlignment(labelPlacement.Value);
				}
				else
				{
					m_coreMap.ColorSwatchPanel.LabelAlignment = GetLabelAlignment(mapColorScale.Instance.LabelPlacement);
				}
			}
			ReportEnumProperty<MapLabelBehavior> labelBehavior = mapColorScale.LabelBehavior;
			if (labelBehavior != null)
			{
				if (!labelBehavior.IsExpression)
				{
					m_coreMap.ColorSwatchPanel.LabelType = GetSwatchLabelType(labelBehavior.Value);
				}
				else
				{
					m_coreMap.ColorSwatchPanel.LabelType = GetSwatchLabelType(mapColorScale.Instance.LabelBehavior);
				}
			}
			ReportBoolProperty hideEndLabels = mapColorScale.HideEndLabels;
			if (hideEndLabels != null)
			{
				if (!hideEndLabels.IsExpression)
				{
					m_coreMap.ColorSwatchPanel.ShowEndLabels = !hideEndLabels.Value;
				}
				else
				{
					m_coreMap.ColorSwatchPanel.ShowEndLabels = !mapColorScale.Instance.HideEndLabels;
				}
			}
			ReportStringProperty noDataText = mapColorScale.NoDataText;
			string text2 = null;
			if (noDataText != null)
			{
				text2 = (noDataText.IsExpression ? mapColorScale.Instance.NoDataText : noDataText.Value);
			}
			m_coreMap.ColorSwatchPanel.NoDataText = ((text2 != null) ? text2 : "");
		}

		private void SetColorScaleTitleProperties()
		{
			MapColorScaleTitle mapColorScaleTitle = m_map.MapColorScale.MapColorScaleTitle;
			ReportStringProperty caption = mapColorScaleTitle.Caption;
			string text = null;
			if (caption != null)
			{
				text = (caption.IsExpression ? mapColorScaleTitle.Instance.Caption : caption.Value);
			}
			m_coreMap.ColorSwatchPanel.Title = ((text != null) ? text : "");
		}

		private LabelAlignment GetLabelAlignment(MapLabelPlacement labelPlacement)
		{
			switch (labelPlacement)
			{
			case MapLabelPlacement.Bottom:
				return LabelAlignment.Bottom;
			case MapLabelPlacement.Top:
				return LabelAlignment.Top;
			default:
				return LabelAlignment.Alternate;
			}
		}

		private SwatchLabelType GetSwatchLabelType(MapLabelBehavior labelBehavior)
		{
			switch (labelBehavior)
			{
			case MapLabelBehavior.ShowBorderValue:
				return SwatchLabelType.ShowBorderValue;
			case MapLabelBehavior.ShowMiddleValue:
				return SwatchLabelType.ShowMiddleValue;
			default:
				return SwatchLabelType.Auto;
			}
		}

		private void SetBorderSkinProperties()
		{
			ReportEnumProperty<MapBorderSkinType> mapBorderSkinType = m_map.MapBorderSkin.MapBorderSkinType;
			if (mapBorderSkinType != null)
			{
				MapBorderSkinType mapBorderSkinType2 = MapBorderSkinType.None;
				mapBorderSkinType2 = (mapBorderSkinType.IsExpression ? m_map.MapBorderSkin.Instance.MapBorderSkinType : mapBorderSkinType.Value);
				m_coreMap.Frame.FrameStyle = GetFrameStyle(mapBorderSkinType2);
			}
		}

		private static FrameStyle GetFrameStyle(MapBorderSkinType type)
		{
			FrameStyle result = FrameStyle.None;
			switch (type)
			{
			case MapBorderSkinType.Emboss:
				result = FrameStyle.Emboss;
				break;
			case MapBorderSkinType.FrameThin1:
				result = FrameStyle.FrameThin1;
				break;
			case MapBorderSkinType.FrameThin2:
				result = FrameStyle.FrameThin2;
				break;
			case MapBorderSkinType.FrameThin3:
				result = FrameStyle.FrameThin3;
				break;
			case MapBorderSkinType.FrameThin4:
				result = FrameStyle.FrameThin4;
				break;
			case MapBorderSkinType.FrameThin5:
				result = FrameStyle.FrameThin5;
				break;
			case MapBorderSkinType.FrameThin6:
				result = FrameStyle.FrameThin6;
				break;
			case MapBorderSkinType.FrameTitle1:
				result = FrameStyle.FrameTitle1;
				break;
			case MapBorderSkinType.FrameTitle2:
				result = FrameStyle.FrameTitle2;
				break;
			case MapBorderSkinType.FrameTitle3:
				result = FrameStyle.FrameTitle3;
				break;
			case MapBorderSkinType.FrameTitle4:
				result = FrameStyle.FrameTitle4;
				break;
			case MapBorderSkinType.FrameTitle5:
				result = FrameStyle.FrameTitle5;
				break;
			case MapBorderSkinType.FrameTitle6:
				result = FrameStyle.FrameTitle6;
				break;
			case MapBorderSkinType.FrameTitle7:
				result = FrameStyle.FrameTitle7;
				break;
			case MapBorderSkinType.FrameTitle8:
				result = FrameStyle.FrameTitle8;
				break;
			case MapBorderSkinType.None:
				result = FrameStyle.None;
				break;
			case MapBorderSkinType.Raised:
				result = FrameStyle.Raised;
				break;
			case MapBorderSkinType.Sunken:
				result = FrameStyle.Sunken;
				break;
			}
			return result;
		}

		private void RenderMapStyle()
		{
			Border border = null;
			m_coreMap.BackColor = Color.Empty;
			Style style = m_map.Style;
			if (style != null)
			{
				StyleInstance style2 = m_map.Instance.Style;
				m_coreMap.BackColor = MappingHelper.GetStyleBackgroundColor(style, style2);
				m_coreMap.BackSecondaryColor = MappingHelper.GetStyleBackGradientEndColor(style, style2);
				m_coreMap.BackGradientType = GetGradientType(style, style2);
				m_coreMap.BackHatchStyle = GetHatchStyle(style, style2);
				border = m_map.Style.Border;
			}
			if (m_coreMap.BackColor.A != byte.MaxValue)
			{
				m_coreMap.AntiAliasing = AntiAliasing.None;
			}
			if (m_map.SpecialBorderHandling)
			{
				RenderMapBorder(border);
			}
		}

		private void RenderMapBorder(Border border)
		{
			if (border != null)
			{
				m_coreMap.BorderLineColor = MappingHelper.GetStyleBorderColor(border);
				m_coreMap.BorderLineStyle = GetDashStyle(MappingHelper.GetStyleBorderStyle(border), isLine: false);
				m_coreMap.BorderLineWidth = MappingHelper.GetStyleBorderWidth(border, base.DpiX);
			}
		}

		private void RenderSubItemStyle(MapSubItem mapSubItem, Panel coreSubItem)
		{
			Style style = mapSubItem.Style;
			if (style == null)
			{
				coreSubItem.BackColor = Color.Empty;
				return;
			}
			StyleInstance style2 = mapSubItem.Instance.Style;
			coreSubItem.BackColor = MappingHelper.GetStyleBackgroundColor(style, style2);
			coreSubItem.BackSecondaryColor = MappingHelper.GetStyleBackGradientEndColor(style, style2);
			coreSubItem.BackGradientType = GetGradientType(style, style2);
			coreSubItem.BackHatchStyle = GetHatchStyle(style, style2);
			coreSubItem.BackShadowOffset = GetValidShadowOffset(MappingHelper.GetStyleShadowOffset(style, style2, base.DpiX));
			Border border = style.Border;
			if (border != null)
			{
				coreSubItem.BorderColor = MappingHelper.GetStyleBorderColor(border);
				coreSubItem.BorderStyle = GetDashStyle(MappingHelper.GetStyleBorderStyle(border), isLine: false);
				coreSubItem.BorderWidth = MappingHelper.GetStyleBorderWidth(border, base.DpiX);
			}
		}

		private void RenderGridLinesStyle(MapGridLines mapGridLines, GridAttributes coreGridLines)
		{
			Style style = mapGridLines.Style;
			Border border = null;
			if (style != null)
			{
				border = style.Border;
			}
			if (style == null)
			{
				coreGridLines.Font = GetDefaultFontFromCache(0);
				coreGridLines.LabelColor = Color.Black;
				coreGridLines.LabelFormatString = "";
			}
			else
			{
				StyleInstance style2 = mapGridLines.Instance.Style;
				coreGridLines.Font = GetFontFromCache(0, style, style2);
				coreGridLines.LabelColor = MappingHelper.GetStyleColor(style, style2);
				coreGridLines.LabelFormatString = MappingHelper.GetStyleFormat(style, style2);
			}
			if (border == null)
			{
				coreGridLines.LineWidth = MappingHelper.GetDefaultBorderWidth(base.DpiX);
				coreGridLines.LineColor = Color.Black;
				coreGridLines.LineStyle = MapDashStyle.Solid;
			}
			else
			{
				coreGridLines.LineWidth = MappingHelper.GetStyleBorderWidth(border, base.DpiX);
				coreGridLines.LineColor = MappingHelper.GetStyleBorderColor(border);
				coreGridLines.LineStyle = GetDashStyle(MappingHelper.GetStyleBorderStyle(border), isLine: true);
			}
		}

		private void RenderLegendTitleStyle(MapLegendTitle mapLegendTitle, Microsoft.Reporting.Map.WebForms.Legend legend)
		{
			Style style = mapLegendTitle.Style;
			if (style != null)
			{
				StyleInstance style2 = mapLegendTitle.Instance.Style;
				if (MappingHelper.IsStylePropertyDefined(style.Color))
				{
					legend.TitleColor = MappingHelper.GetStyleColor(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(style.BackgroundColor))
				{
					legend.TitleBackColor = MappingHelper.GetStyleBackgroundColor(style, style2);
				}
				RenderLegendTitleBorder(style.Border, legend);
				legend.TitleAlignment = GetLegendTitleAlign(MappingHelper.GetStyleTextAlign(style, style2));
			}
			RenderLegendTitleFont(mapLegendTitle, legend);
		}

		private void RenderLegendTitleFont(MapLegendTitle mapLegendTitle, Microsoft.Reporting.Map.WebForms.Legend legend)
		{
			Style style = mapLegendTitle.Style;
			if (style == null)
			{
				legend.TitleFont = GetDefaultFont();
			}
			else
			{
				legend.TitleFont = GetFont(style, mapLegendTitle.Instance.Style);
			}
		}

		private StringAlignment GetLegendTitleAlign(TextAlignments textAlignment)
		{
			switch (textAlignment)
			{
			case TextAlignments.Left:
				return StringAlignment.Near;
			case TextAlignments.Right:
				return StringAlignment.Far;
			default:
				return StringAlignment.Center;
			}
		}

		private void RenderLegendTitleBorder(Border border, Microsoft.Reporting.Map.WebForms.Legend legend)
		{
			if (border != null && MappingHelper.IsStylePropertyDefined(border.Color))
			{
				legend.TitleSeparatorColor = MappingHelper.GetStyleBorderColor(border);
			}
		}

		private void RenderColorScaleTitleStyle()
		{
			Style style = m_map.MapColorScale.MapColorScaleTitle.Style;
			if (style == null)
			{
				m_coreMap.ColorSwatchPanel.TitleFont = GetDefaultFontFromCache(0);
				return;
			}
			StyleInstance style2 = m_map.MapColorScale.MapColorScaleTitle.Instance.Style;
			m_coreMap.ColorSwatchPanel.TitleColor = MappingHelper.GetStyleColor(style, style2);
			m_coreMap.ColorSwatchPanel.TitleFont = GetFontFromCache(0, style, style2);
		}

		private void RenderBorderSkinStyle()
		{
			Style style = m_map.MapBorderSkin.Style;
			if (style != null)
			{
				StyleInstance style2 = m_map.MapBorderSkin.Instance.Style;
				Frame frame = m_coreMap.Frame;
				frame.PageColor = MappingHelper.GetStyleColor(style, style2);
				frame.BackColor = MappingHelper.GetStyleBackgroundColor(style, style2);
				frame.BackSecondaryColor = MappingHelper.GetStyleBackGradientEndColor(style, style2);
				frame.BackGradientType = GetGradientType(style, style2);
				frame.BackHatchStyle = GetHatchStyle(style, style2);
				RenderBorderSkinBorder(style.Border, frame);
			}
		}

		private void RenderBorderSkinBorder(Border border, Frame borderSkin)
		{
			if (border != null)
			{
				if (MappingHelper.IsStylePropertyDefined(border.Color))
				{
					borderSkin.BorderColor = MappingHelper.GetStyleBorderColor(border);
				}
				if (MappingHelper.IsStylePropertyDefined(border.Style))
				{
					borderSkin.BorderStyle = GetDashStyle(MappingHelper.GetStyleBorderStyle(border), isLine: false);
				}
				borderSkin.BorderWidth = MappingHelper.GetStyleBorderWidth(border, base.DpiX);
			}
		}

		private void GetPngImage(out Stream imageStream, int width, int height)
		{
			using (Bitmap bitmap = new Bitmap(width, height))
			{
				bitmap.SetResolution(base.DpiX, base.DpiY);
				using (Graphics graphics = Graphics.FromImage(bitmap))
				{
					GetImage(graphics);
					imageStream = new MemoryStream();
					bitmap.Save(imageStream, ImageFormat.Png);
				}
			}
		}

		private void GetEmfImage(out Stream imageStream, int width, int height)
		{
			using (Bitmap image = new Bitmap(width, height))
			{
				using (Graphics graphics = Graphics.FromImage(image))
				{
					IntPtr hdc = graphics.GetHdc();
					imageStream = m_map.RenderingContext.OdpContext.CreateStreamCallback(m_map.Name, "emf", null, "image/emf", willSeek: true, StreamOper.CreateOnly);
					using (Metafile image2 = new Metafile(imageStream, hdc, new System.Drawing.Rectangle(0, 0, width, height), MetafileFrameUnit.Pixel, EmfType.EmfPlusOnly))
					{
						using (Graphics graphics2 = Graphics.FromImage(image2))
						{
							GetImage(graphics2);
						}
					}
					graphics.ReleaseHdc(hdc);
				}
			}
		}

		private void GetImage(Graphics graphics)
		{
			m_coreMap.mapCore.Paint(graphics);
		}

		private static LayerVisibility GetLayerVisibility(MapVisibilityMode visibility)
		{
			switch (visibility)
			{
			case MapVisibilityMode.Hidden:
				return LayerVisibility.Hidden;
			case MapVisibilityMode.ZoomBased:
				return LayerVisibility.ZoomBased;
			default:
				return LayerVisibility.Shown;
			}
		}

		internal static MarkerStyle GetMarkerStyle(MapMarkerStyle mapMarkerStyle)
		{
			switch (mapMarkerStyle)
			{
			case MapMarkerStyle.Circle:
				return MarkerStyle.Circle;
			case MapMarkerStyle.Diamond:
				return MarkerStyle.Diamond;
			case MapMarkerStyle.Pentagon:
				return MarkerStyle.Pentagon;
			case MapMarkerStyle.PushPin:
				return MarkerStyle.PushPin;
			case MapMarkerStyle.Rectangle:
				return MarkerStyle.Rectangle;
			case MapMarkerStyle.Star:
				return MarkerStyle.Star;
			case MapMarkerStyle.Trapezoid:
				return MarkerStyle.Trapezoid;
			case MapMarkerStyle.Triangle:
				return MarkerStyle.Triangle;
			case MapMarkerStyle.Wedge:
				return MarkerStyle.Wedge;
			default:
				return MarkerStyle.None;
			}
		}

		internal static MapMarkerStyle GetMarkerStyle(MapMarker mapMarker, bool hasScope)
		{
			if (mapMarker != null)
			{
				ReportEnumProperty<MapMarkerStyle> mapMarkerStyle = mapMarker.MapMarkerStyle;
				if (mapMarkerStyle != null)
				{
					if (!mapMarkerStyle.IsExpression)
					{
						return mapMarkerStyle.Value;
					}
					if (hasScope)
					{
						return mapMarker.Instance.MapMarkerStyle;
					}
				}
			}
			return MapMarkerStyle.None;
		}

		internal static GradientType GetGradientType(Style style, StyleInstance styleInstance)
		{
			switch (MappingHelper.GetStyleBackGradientType(style, styleInstance))
			{
			case BackgroundGradients.Center:
				return GradientType.Center;
			case BackgroundGradients.DiagonalLeft:
				return GradientType.DiagonalLeft;
			case BackgroundGradients.DiagonalRight:
				return GradientType.DiagonalRight;
			case BackgroundGradients.HorizontalCenter:
				return GradientType.HorizontalCenter;
			case BackgroundGradients.LeftRight:
				return GradientType.LeftRight;
			case BackgroundGradients.TopBottom:
				return GradientType.TopBottom;
			case BackgroundGradients.VerticalCenter:
				return GradientType.VerticalCenter;
			default:
				return GradientType.None;
			}
		}

		internal static MapHatchStyle GetHatchStyle(Style style, StyleInstance styleInstance)
		{
			switch (MappingHelper.GetStyleBackgroundHatchType(style, styleInstance))
			{
			case BackgroundHatchTypes.BackwardDiagonal:
				return MapHatchStyle.BackwardDiagonal;
			case BackgroundHatchTypes.Cross:
				return MapHatchStyle.Cross;
			case BackgroundHatchTypes.DarkDownwardDiagonal:
				return MapHatchStyle.DarkDownwardDiagonal;
			case BackgroundHatchTypes.DarkHorizontal:
				return MapHatchStyle.DarkHorizontal;
			case BackgroundHatchTypes.DarkUpwardDiagonal:
				return MapHatchStyle.DarkUpwardDiagonal;
			case BackgroundHatchTypes.DarkVertical:
				return MapHatchStyle.DarkVertical;
			case BackgroundHatchTypes.DashedDownwardDiagonal:
				return MapHatchStyle.DashedDownwardDiagonal;
			case BackgroundHatchTypes.DashedHorizontal:
				return MapHatchStyle.DashedHorizontal;
			case BackgroundHatchTypes.DashedUpwardDiagonal:
				return MapHatchStyle.DashedUpwardDiagonal;
			case BackgroundHatchTypes.DashedVertical:
				return MapHatchStyle.DashedVertical;
			case BackgroundHatchTypes.DiagonalBrick:
				return MapHatchStyle.DiagonalBrick;
			case BackgroundHatchTypes.DiagonalCross:
				return MapHatchStyle.DiagonalCross;
			case BackgroundHatchTypes.Divot:
				return MapHatchStyle.Divot;
			case BackgroundHatchTypes.DottedDiamond:
				return MapHatchStyle.DottedDiamond;
			case BackgroundHatchTypes.DottedGrid:
				return MapHatchStyle.DottedGrid;
			case BackgroundHatchTypes.ForwardDiagonal:
				return MapHatchStyle.ForwardDiagonal;
			case BackgroundHatchTypes.Horizontal:
				return MapHatchStyle.Horizontal;
			case BackgroundHatchTypes.HorizontalBrick:
				return MapHatchStyle.HorizontalBrick;
			case BackgroundHatchTypes.LargeCheckerBoard:
				return MapHatchStyle.LargeCheckerBoard;
			case BackgroundHatchTypes.LargeConfetti:
				return MapHatchStyle.LargeConfetti;
			case BackgroundHatchTypes.LargeGrid:
				return MapHatchStyle.LargeGrid;
			case BackgroundHatchTypes.LightDownwardDiagonal:
				return MapHatchStyle.LightDownwardDiagonal;
			case BackgroundHatchTypes.LightHorizontal:
				return MapHatchStyle.LightHorizontal;
			case BackgroundHatchTypes.LightUpwardDiagonal:
				return MapHatchStyle.LightUpwardDiagonal;
			case BackgroundHatchTypes.LightVertical:
				return MapHatchStyle.LightVertical;
			case BackgroundHatchTypes.NarrowHorizontal:
				return MapHatchStyle.NarrowHorizontal;
			case BackgroundHatchTypes.NarrowVertical:
				return MapHatchStyle.NarrowVertical;
			case BackgroundHatchTypes.OutlinedDiamond:
				return MapHatchStyle.OutlinedDiamond;
			case BackgroundHatchTypes.Percent05:
				return MapHatchStyle.Percent05;
			case BackgroundHatchTypes.Percent10:
				return MapHatchStyle.Percent10;
			case BackgroundHatchTypes.Percent20:
				return MapHatchStyle.Percent20;
			case BackgroundHatchTypes.Percent25:
				return MapHatchStyle.Percent25;
			case BackgroundHatchTypes.Percent30:
				return MapHatchStyle.Percent30;
			case BackgroundHatchTypes.Percent40:
				return MapHatchStyle.Percent40;
			case BackgroundHatchTypes.Percent50:
				return MapHatchStyle.Percent50;
			case BackgroundHatchTypes.Percent60:
				return MapHatchStyle.Percent60;
			case BackgroundHatchTypes.Percent70:
				return MapHatchStyle.Percent70;
			case BackgroundHatchTypes.Percent75:
				return MapHatchStyle.Percent75;
			case BackgroundHatchTypes.Percent80:
				return MapHatchStyle.Percent80;
			case BackgroundHatchTypes.Percent90:
				return MapHatchStyle.Percent90;
			case BackgroundHatchTypes.Plaid:
				return MapHatchStyle.Plaid;
			case BackgroundHatchTypes.Shingle:
				return MapHatchStyle.Shingle;
			case BackgroundHatchTypes.SmallCheckerBoard:
				return MapHatchStyle.SmallCheckerBoard;
			case BackgroundHatchTypes.SmallConfetti:
				return MapHatchStyle.SmallConfetti;
			case BackgroundHatchTypes.SmallGrid:
				return MapHatchStyle.SmallGrid;
			case BackgroundHatchTypes.SolidDiamond:
				return MapHatchStyle.SolidDiamond;
			case BackgroundHatchTypes.Sphere:
				return MapHatchStyle.Sphere;
			case BackgroundHatchTypes.Trellis:
				return MapHatchStyle.Trellis;
			case BackgroundHatchTypes.Vertical:
				return MapHatchStyle.Vertical;
			case BackgroundHatchTypes.Wave:
				return MapHatchStyle.Wave;
			case BackgroundHatchTypes.Weave:
				return MapHatchStyle.Weave;
			case BackgroundHatchTypes.WideDownwardDiagonal:
				return MapHatchStyle.WideDownwardDiagonal;
			case BackgroundHatchTypes.WideUpwardDiagonal:
				return MapHatchStyle.WideUpwardDiagonal;
			case BackgroundHatchTypes.ZigZag:
				return MapHatchStyle.ZigZag;
			default:
				return MapHatchStyle.None;
			}
		}

		internal static int GetValidShadowOffset(int shadowOffset)
		{
			return Math.Min(shadowOffset, 100);
		}

		internal static MapDashStyle GetDashStyle(Border border, bool hasScope, bool isLine)
		{
			BorderStyles borderStyle = (!MappingHelper.IsPropertyExpression(border.Style) || hasScope) ? MappingHelper.GetStyleBorderStyle(border) : BorderStyles.Default;
			return GetDashStyle(borderStyle, isLine);
		}

		private static MapDashStyle GetDashStyle(BorderStyles borderStyle, bool isLine)
		{
			switch (borderStyle)
			{
			case BorderStyles.DashDot:
				return MapDashStyle.DashDot;
			case BorderStyles.DashDotDot:
				return MapDashStyle.DashDotDot;
			case BorderStyles.Dashed:
				return MapDashStyle.Dash;
			case BorderStyles.Dotted:
				return MapDashStyle.Dot;
			case BorderStyles.Solid:
			case BorderStyles.Double:
				return MapDashStyle.Solid;
			case BorderStyles.None:
				return MapDashStyle.None;
			default:
				if (isLine)
				{
					return MapDashStyle.Solid;
				}
				return MapDashStyle.None;
			}
		}

		internal string AddImage(MapMarkerImage mapMarkerImage)
		{
			byte[] imageData = mapMarkerImage.Instance.ImageData;
			if (imageData == null)
			{
				return "";
			}
			System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(imageData, writable: false));
			if (image == null)
			{
				return "";
			}
			string text = m_coreMap.NamedImages.Count.ToString(CultureInfo.InvariantCulture);
			m_coreMap.NamedImages.Add(new NamedImage(text, image));
			return text;
		}

		internal ResizeMode GetImageResizeMode(MapMarkerImage mapMarkerImage)
		{
			ReportEnumProperty<MapResizeMode> resizeMode = mapMarkerImage.ResizeMode;
			if (resizeMode != null)
			{
				if (!resizeMode.IsExpression)
				{
					return GetResizeMode(resizeMode.Value);
				}
				return GetResizeMode(mapMarkerImage.Instance.ResizeMode);
			}
			return ResizeMode.AutoFit;
		}

		private ResizeMode GetResizeMode(MapResizeMode resizeMode)
		{
			if (resizeMode == MapResizeMode.None)
			{
				return ResizeMode.None;
			}
			return ResizeMode.AutoFit;
		}

		internal Color GetImageTransColor(MapMarkerImage image)
		{
			ReportColorProperty transparentColor = image.TransparentColor;
			Color color = Color.Empty;
			if (transparentColor != null && !MappingHelper.GetColorFromReportColorProperty(transparentColor, ref color))
			{
				ReportColor transparentColor2 = image.Instance.TransparentColor;
				if (transparentColor2 != null)
				{
					color = transparentColor2.ToColor();
				}
			}
			return color;
		}

		internal void RenderActionInfo(ActionInfo actionInfo, string toolTip, IImageMapProvider imageMapProvider, string layerName, bool hasScope)
		{
			if (actionInfo == null && string.IsNullOrEmpty(toolTip))
			{
				return;
			}
			string href;
			ActionInfoWithDynamicImageMap actionInfoWithDynamicImageMap = MappingHelper.CreateActionInfoDynamic(m_map, actionInfo, toolTip, out href, hasScope);
			if (actionInfoWithDynamicImageMap == null)
			{
				return;
			}
			if (href != null)
			{
				if (layerName != null)
				{
					href = VectorLayerMapper.AddPrefixToFieldNames(layerName, href);
				}
				imageMapProvider.Href = href;
			}
			int count = m_actions.Count;
			m_actions.InternalList.Add(actionInfoWithDynamicImageMap);
			imageMapProvider.Tag = count;
		}

		internal void OnSpatialElementAdded(SpatialElementInfo spatialElementInfo)
		{
			DecrementRemainingSpatialElementCount();
			if (spatialElementInfo.CoreSpatialElement.Points != null)
			{
				DecrementRemainingTotalCount(spatialElementInfo.CoreSpatialElement.Points.Length);
			}
		}

		private void DecrementRemainingSpatialElementCount()
		{
			m_remainingSpatialElementCount--;
		}

		private void DecrementRemainingTotalCount(int count)
		{
			m_remainingTotalPointCount -= count;
		}

		private ImageMapArea.ImageMapAreaShape GetMapAreaShape(MapAreaShape shape)
		{
			if (shape == MapAreaShape.Rectangle)
			{
				return ImageMapArea.ImageMapAreaShape.Rectangle;
			}
			if (MapAreaShape.Circle == shape)
			{
				return ImageMapArea.ImageMapAreaShape.Circle;
			}
			return ImageMapArea.ImageMapAreaShape.Polygon;
		}

		private string FormatNumber(object sender, object value, string format)
		{
			if (m_formatter == null)
			{
				m_formatter = new Formatter(m_map.MapDef.StyleClass, m_map.RenderingContext.OdpContext, m_map.MapDef.ObjectType, m_map.Name);
			}
			return m_formatter.FormatValue(value, format, Type.GetTypeCode(value.GetType()));
		}

		private double EvaluateSimplificationResolution()
		{
			ReportDoubleProperty simplificationResolution = m_map.MapViewport.SimplificationResolution;
			if (simplificationResolution != null)
			{
				if (!simplificationResolution.IsExpression)
				{
					return simplificationResolution.Value;
				}
				return m_map.MapViewport.Instance.SimplificationResolution;
			}
			return 0.0;
		}

		internal void Simplify(Shape shape)
		{
			if (Simplifier != null)
			{
				Simplifier.Simplify(shape, m_simpificationResolution.Value);
			}
		}

		internal void Simplify(Microsoft.Reporting.Map.WebForms.Path path)
		{
			if (Simplifier != null)
			{
				Simplifier.Simplify(path, m_simpificationResolution.Value);
			}
		}

		public override void Dispose()
		{
			if (m_coreMap != null)
			{
				m_coreMap.Dispose();
			}
			m_coreMap = null;
			base.Dispose();
		}
	}
}
