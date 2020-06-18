using Microsoft.Reporting.Map.WebForms.BingMaps;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.Reporting.Map.WebForms
{
	[TypeConverter(typeof(LayerConverter))]
	internal class Layer : NamedElement, ISelectable
	{
		private LayerVisibility visibility;

		private float visibleFromZoom = 50f;

		private float visibleToZoom = 200f;

		private float labelVisibleFromZoom = 50f;

		private float transparency;

		private TileSystem tileSystem;

		private bool useSecureConnectionForTiles;

		private ImageryProvider[] tileImageryProviders;

		private string tileImageUriFormat = string.Empty;

		private string[] tileImageUriSubdomains;

		private string tileError = string.Empty;

		public override string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				switch (value)
				{
				case "(none)":
				case "(all)":
					throw new ArgumentException("bad_layer_name");
				}
				if (base.Name != value)
				{
					string name = base.Name;
					base.Name = value;
					UpdateLayerElements(name, value);
				}
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[SRCategory("CategoryAttribute_GeneralVisibility")]
		[SRDescription("DescriptionAttributeLayer_Visibility")]
		[DefaultValue(LayerVisibility.Shown)]
		public LayerVisibility Visibility
		{
			get
			{
				return visibility;
			}
			set
			{
				if (visibility != value)
				{
					visibility = value;
					InvalidateViewport();
				}
			}
		}

		[SRCategory("CategoryAttribute_GeneralVisibility")]
		[SRDescription("DescriptionAttributeLayer_VisibleFromZoom")]
		[DefaultValue(50)]
		public float VisibleFromZoom
		{
			get
			{
				return visibleFromZoom;
			}
			set
			{
				if (visibleFromZoom != value)
				{
					visibleFromZoom = value;
					if (Visibility == LayerVisibility.ZoomBased)
					{
						InvalidateViewport();
					}
				}
			}
		}

		[SRCategory("CategoryAttribute_GeneralVisibility")]
		[SRDescription("DescriptionAttributeLayer_VisibleToZoom")]
		[DefaultValue(200)]
		public float VisibleToZoom
		{
			get
			{
				return visibleToZoom;
			}
			set
			{
				if (visibleToZoom != value)
				{
					visibleToZoom = value;
					if (Visibility == LayerVisibility.ZoomBased)
					{
						InvalidateViewport();
					}
				}
			}
		}

		[SRCategory("CategoryAttribute_LabelVisibility")]
		[SRDescription("DescriptionAttributeLayer_LabelVisibleFromZoom")]
		[DefaultValue(50)]
		public float LabelVisibleFromZoom
		{
			get
			{
				return labelVisibleFromZoom;
			}
			set
			{
				if (labelVisibleFromZoom != value)
				{
					labelVisibleFromZoom = value;
					if (Visibility == LayerVisibility.ZoomBased)
					{
						InvalidateViewport();
					}
				}
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool Visible
		{
			get
			{
				if (Transparency == 100f)
				{
					return false;
				}
				if (Visibility == LayerVisibility.ZoomBased)
				{
					if (Common == null || Common.MapCore == null)
					{
						return false;
					}
					float zoom = Common.MapCore.Viewport.Zoom;
					if (zoom >= VisibleFromZoom)
					{
						return zoom <= VisibleToZoom;
					}
					return false;
				}
				return Visibility == LayerVisibility.Shown;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool LabelVisible
		{
			get
			{
				if (Visibility == LayerVisibility.ZoomBased)
				{
					if (Common == null || Common.MapCore == null)
					{
						return false;
					}
					float zoom = Common.MapCore.Viewport.Zoom;
					if (zoom >= LabelVisibleFromZoom)
					{
						return zoom <= VisibleToZoom;
					}
					return false;
				}
				return Visibility == LayerVisibility.Shown;
			}
		}

		[SRCategory("CategoryAttribute_GeneralVisibility")]
		[SRDescription("DescriptionAttributeLayer_Transparency")]
		[DefaultValue(0f)]
		public float Transparency
		{
			get
			{
				return transparency;
			}
			set
			{
				if (value > 100f || value < 0f)
				{
					throw new ArgumentOutOfRangeException(SR.out_of_range(0.0, 100.0));
				}
				transparency = value;
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Misc")]
		[SRDescription("DescriptionAttributeLayer_TileSystem")]
		[DefaultValue(TileSystem.None)]
		public TileSystem TileSystem
		{
			get
			{
				return tileSystem;
			}
			set
			{
				if (tileSystem != value)
				{
					tileSystem = value;
					ResetStoredVirtualEarthParameters();
					InvalidateViewport();
				}
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[SRCategory("DescriptionAttributeLayer_TileSystem")]
		[SRDescription("DescriptionAttributeLayer_UseSecureConnectionForTiles")]
		[DefaultValue(false)]
		public bool UseSecureConnectionForTiles
		{
			get
			{
				return useSecureConnectionForTiles;
			}
			set
			{
				if (useSecureConnectionForTiles != value)
				{
					useSecureConnectionForTiles = value;
					ResetStoredVirtualEarthParameters();
					InvalidateViewport();
				}
			}
		}

		internal ImageryProvider[] TileImageryProviders
		{
			get
			{
				return tileImageryProviders;
			}
			set
			{
				tileImageryProviders = value;
			}
		}

		internal string TileImageUriFormat
		{
			get
			{
				return tileImageUriFormat;
			}
			set
			{
				tileImageUriFormat = value;
			}
		}

		internal string[] TileImageUriSubdomains
		{
			get
			{
				return tileImageUriSubdomains;
			}
			set
			{
				tileImageUriSubdomains = value;
			}
		}

		internal string TileError
		{
			get
			{
				return tileError;
			}
			set
			{
				tileError = value;
			}
		}

		internal override void OnRemove()
		{
			base.OnRemove();
			UpdateLayerElements(Name, string.Empty);
		}

		internal string GetAttributionStrings()
		{
			if (TileImageryProviders == null)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			RectangleF rectangleF = new RectangleF(Common.MapCore.Viewport.GetAbsoluteLocation(), Common.MapCore.Viewport.GetAbsoluteSize());
			MapPoint minimumPoint = Common.MapCore.PixelsToGeographic(new PointF(rectangleF.Left, rectangleF.Bottom));
			MapPoint maximumPoint = Common.MapCore.PixelsToGeographic(new PointF(rectangleF.Right, rectangleF.Top));
			MapBounds a = new MapBounds(minimumPoint, maximumPoint);
			int num = Math.Max((int)VirtualEarthTileSystem.LevelOfDetail(Common.MapCore.Viewport.GetGroundResolutionAtEquator()), 1);
			ImageryProvider[] array = TileImageryProviders;
			foreach (ImageryProvider imageryProvider in array)
			{
				CoverageArea[] coverageAreas = imageryProvider.CoverageAreas;
				foreach (CoverageArea coverageArea in coverageAreas)
				{
					if (num < coverageArea.ZoomMin || num > coverageArea.ZoomMax)
					{
						continue;
					}
					MapBounds b = new MapBounds(new MapPoint(coverageArea.BoundingBox[1], coverageArea.BoundingBox[1]), new MapPoint(coverageArea.BoundingBox[3], coverageArea.BoundingBox[3]));
					if (MapBounds.Intersect(a, b))
					{
						if (stringBuilder.Length > 0)
						{
							stringBuilder.Append("|");
						}
						stringBuilder.Append(imageryProvider.Attribution);
						break;
					}
				}
			}
			return stringBuilder.ToString();
		}

		internal List<ILayerElement> GetLayerElements()
		{
			List<ILayerElement> list = new List<ILayerElement>();
			if (Common != null && Common.MapCore != null)
			{
				MapCore mapCore = Common.MapCore;
				foreach (ILayerElement group in mapCore.Groups)
				{
					if (group.Layer == Name)
					{
						list.Add(group);
					}
				}
				foreach (ILayerElement shape in mapCore.Shapes)
				{
					if (shape.Layer == Name)
					{
						list.Add(shape);
					}
				}
				foreach (ILayerElement path in mapCore.Paths)
				{
					if (path.Layer == Name)
					{
						list.Add(path);
					}
				}
				{
					foreach (ILayerElement symbol in mapCore.Symbols)
					{
						if (symbol.Layer == Name)
						{
							list.Add(symbol);
						}
					}
					return list;
				}
			}
			return list;
		}

		private void UpdateLayerElements(string oldLayerName, string newLayerName)
		{
			foreach (ILayerElement layerElement in GetLayerElements())
			{
				if (layerElement.Layer == oldLayerName)
				{
					layerElement.Layer = newLayerName;
				}
			}
		}

		void ISelectable.DrawSelection(MapGraphics g, RectangleF clipRect, bool designTimeSelection)
		{
			RectangleF selectionRectangle = ((ISelectable)this).GetSelectionRectangle(g, clipRect);
			if (!selectionRectangle.IsEmpty)
			{
				g.DrawSelection(selectionRectangle, designTimeSelection, Common.MapCore.SelectionBorderColor, Common.MapCore.SelectionMarkerColor);
			}
		}

		bool ISelectable.IsSelected()
		{
			return false;
		}

		bool ISelectable.IsVisible()
		{
			return Visible;
		}

		RectangleF ISelectable.GetSelectionRectangle(MapGraphics g, RectangleF clipRect)
		{
			RectangleF rectangleF = RectangleF.Empty;
			foreach (ISelectable layerElement in GetLayerElements())
			{
				bool flag = false;
				if ((layerElement as IContentElement)?.IsVisible(g, this, allLayers: false, clipRect) ?? layerElement.IsVisible())
				{
					rectangleF = ((!rectangleF.IsEmpty) ? RectangleF.Union(rectangleF, layerElement.GetSelectionRectangle(g, clipRect)) : layerElement.GetSelectionRectangle(g, clipRect));
				}
			}
			return rectangleF;
		}

		internal bool IsVirtualEarthServiceQueried()
		{
			lock (TileImageUriFormat)
			{
				return !string.IsNullOrEmpty(TileImageUriFormat);
			}
		}

		internal void ResetStoredVirtualEarthParameters()
		{
			lock (TileImageUriFormat)
			{
				TileImageUriFormat = string.Empty;
				TileImageUriSubdomains = null;
				TileImageryProviders = null;
			}
			lock (TileError)
			{
				TileError = string.Empty;
			}
		}

		internal bool QueryVirtualEarthService(bool asyncQuery)
		{
			if (string.IsNullOrEmpty(Common.MapCore.TileServerAppId) || Common.MapCore.TileServerAppId.ToUpper(CultureInfo.InvariantCulture) == "(DEFAULT)")
			{
				lock (TileError)
				{
					TileError = SR.ProvideBingMapsAppID;
				}
				return false;
			}
			try
			{
				ImageryMetadataRequest imageryRequest = new ImageryMetadataRequest
				{
					BingMapsKey = common.MapCore.TileServerAppId,
					ImagerySet = VirtualEarthTileSystem.TileSystemToMapStyle(TileSystem),
					IncludeImageryProviders = true,
					UseHTTPS = true
				};
				if (asyncQuery)
				{
					BingMapsService.GetImageryMetadataAsync(imageryRequest, ProcessImageryMetadataResponse, delegate(Exception ex)
					{
						lock (TileError)
						{
							TileError = ex.Message;
						}
					});
				}
				else
				{
					Response imageryMetadata = BingMapsService.GetImageryMetadata(imageryRequest);
					ProcessImageryMetadataResponse(imageryMetadata);
				}
			}
			catch (Exception ex2)
			{
				lock (TileError)
				{
					TileError = ex2.Message;
				}
				return false;
			}
			return true;
		}

		private void ProcessImageryMetadataResponse(Response response)
		{
			try
			{
				ResetStoredVirtualEarthParameters();
				lock (TileImageUriFormat)
				{
					if (response != null && response.ResourceSets.Any())
					{
						ImageryMetadata imageryMetadata = response.ResourceSets.First().Resources.OfType<ImageryMetadata>().FirstOrDefault();
						if (imageryMetadata != null)
						{
							TileImageUriFormat = imageryMetadata.ImageUrl;
							TileImageUriSubdomains = imageryMetadata.ImageUrlSubdomains;
							TileImageryProviders = imageryMetadata.ImageryProviders;
						}
					}
				}
			}
			catch (Exception ex)
			{
				lock (TileError)
				{
					TileError = ex.InnerException.Message;
				}
			}
		}
	}
}
