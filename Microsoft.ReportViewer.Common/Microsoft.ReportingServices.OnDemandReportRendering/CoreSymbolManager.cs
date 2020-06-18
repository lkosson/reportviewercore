using Microsoft.Reporting.Map.WebForms;
using System.Drawing;
using System.Globalization;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal class CoreSymbolManager : CoreSpatialElementManager
	{
		protected override NamedCollection SpatialElements => m_coreMap.Symbols;

		internal override Microsoft.Reporting.Map.WebForms.FieldCollection FieldDefinitions => m_coreMap.SymbolFields;

		internal CoreSymbolManager(MapControl mapControl, MapVectorLayer mapVectorLayer)
			: base(mapControl, mapVectorLayer)
		{
		}

		internal override void AddSpatialElement(ISpatialElement spatialElement)
		{
			((NamedElement)spatialElement).Name = m_coreMap.Symbols.Count.ToString(CultureInfo.InvariantCulture);
			m_coreMap.Symbols.Add((Symbol)spatialElement);
		}

		internal override void RemoveSpatialElement(ISpatialElement spatialElement)
		{
			m_coreMap.Symbols.Remove((Symbol)spatialElement);
		}

		internal override ISpatialElement CreateSpatialElement()
		{
			return new Symbol
			{
				BorderColor = Color.Black,
				Text = ""
			};
		}
	}
}
