using Microsoft.Reporting.Map.WebForms;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal class SymbolMarkerTemplateMapper : PointTemplateMapper
	{
		private string sharedImageName;

		internal SymbolMarkerTemplateMapper(MapMapper mapMapper, VectorLayerMapper vectorLayerMapper, MapVectorLayer mapVectorLayer)
			: base(mapMapper, vectorLayerMapper, mapVectorLayer)
		{
		}

		protected override void RenderPointTemplate(MapPointTemplate mapPointTemplate, Symbol coreSymbol, bool customTemplate, bool ignoreBackgoundColor, bool ignoreSize, bool ignoreMarker, bool hasScope)
		{
			base.RenderPointTemplate(mapPointTemplate, coreSymbol, customTemplate, ignoreBackgoundColor, ignoreSize, ignoreMarker, hasScope);
			if (ignoreMarker)
			{
				return;
			}
			MapMarker mapMarker = ((MapMarkerTemplate)mapPointTemplate).MapMarker;
			MapMarkerStyle markerStyle = MapMapper.GetMarkerStyle(mapMarker, hasScope);
			if (markerStyle != MapMarkerStyle.Image)
			{
				coreSymbol.MarkerStyle = MapMapper.GetMarkerStyle(markerStyle);
				return;
			}
			MapMarkerImage mapMarkerImage = mapMarker.MapMarkerImage;
			if (mapMarkerImage == null)
			{
				throw new RenderingObjectModelException(RPRes.rsMapLayerMissingProperty(RPRes.rsObjectTypeMap, m_mapVectorLayer.MapDef.Name, m_mapVectorLayer.Name, "MapMarkerImage"));
			}
			string image;
			if (CanShareMarkerImage(mapMarkerImage, customTemplate))
			{
				if (sharedImageName == null)
				{
					sharedImageName = m_mapMapper.AddImage(mapMarkerImage);
				}
				image = sharedImageName;
			}
			else
			{
				image = m_mapMapper.AddImage(mapMarkerImage);
			}
			coreSymbol.Image = image;
			coreSymbol.ImageResizeMode = m_mapMapper.GetImageResizeMode(mapMarkerImage);
			coreSymbol.ImageTransColor = m_mapMapper.GetImageTransColor(mapMarkerImage);
		}

		private bool CanShareMarkerImage(MapMarkerImage mapMarkerImage, bool customTemplate)
		{
			if (!mapMarkerImage.MIMEType.IsExpression && !mapMarkerImage.Value.IsExpression)
			{
				return !customTemplate;
			}
			return false;
		}
	}
}
