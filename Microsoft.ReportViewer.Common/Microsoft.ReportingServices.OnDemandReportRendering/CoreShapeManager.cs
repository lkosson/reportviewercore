using Microsoft.Reporting.Map.WebForms;
using System.Drawing;
using System.Globalization;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal class CoreShapeManager : CoreSpatialElementManager
	{
		internal override Microsoft.Reporting.Map.WebForms.FieldCollection FieldDefinitions => m_coreMap.ShapeFields;

		protected override NamedCollection SpatialElements => m_coreMap.Shapes;

		internal CoreShapeManager(MapControl mapControl, MapVectorLayer mapVectorLayer)
			: base(mapControl, mapVectorLayer)
		{
		}

		internal override void AddSpatialElement(ISpatialElement spatialElement)
		{
			((NamedElement)spatialElement).Name = m_coreMap.Shapes.Count.ToString(CultureInfo.InvariantCulture);
			m_coreMap.Shapes.Add((Shape)spatialElement);
		}

		internal override void RemoveSpatialElement(ISpatialElement spatialElement)
		{
			m_coreMap.Shapes.Remove((Shape)spatialElement);
		}

		internal override ISpatialElement CreateSpatialElement()
		{
			return new Shape
			{
				BorderColor = Color.Black,
				Text = ""
			};
		}
	}
}
