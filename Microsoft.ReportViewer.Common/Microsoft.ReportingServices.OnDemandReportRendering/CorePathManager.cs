using Microsoft.Reporting.Map.WebForms;
using System.Drawing;
using System.Globalization;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal class CorePathManager : CoreSpatialElementManager
	{
		protected override NamedCollection SpatialElements => m_coreMap.Paths;

		internal override Microsoft.Reporting.Map.WebForms.FieldCollection FieldDefinitions => m_coreMap.PathFields;

		internal CorePathManager(MapControl mapControl, MapVectorLayer mapVectorLayer)
			: base(mapControl, mapVectorLayer)
		{
		}

		internal override void AddSpatialElement(ISpatialElement spatialElement)
		{
			((NamedElement)spatialElement).Name = m_coreMap.Paths.Count.ToString(CultureInfo.InvariantCulture);
			m_coreMap.Paths.Add((Path)spatialElement);
		}

		internal override void RemoveSpatialElement(ISpatialElement spatialElement)
		{
			m_coreMap.Paths.Remove((Path)spatialElement);
		}

		internal override ISpatialElement CreateSpatialElement()
		{
			new Path
			{
				BorderColor = Color.Black,
				Text = ""
			};
			return new Path();
		}
	}
}
