using Microsoft.ReportingServices.OnDemandReportRendering;

namespace Microsoft.ReportingServices.Rendering.DataRenderer;

internal interface IMapHandler
{
	void OnMapBegin(Map map, bool outputMap, ref bool walkMap);

	void OnMapEnd(Map map);

	void OnMapVectorLayerBegin(MapVectorLayer layer, bool outputLayer, ref bool walkLayer);

	void OnMapVectorLayerEnd(MapVectorLayer layer);

	void OnMapMemberBegin(MapMember mapMember, MapSpatialElementTemplate template, bool outputMapMemberLabel, bool outputDynamicMembers, MapVectorLayer layer, ref bool walkMapMember);

	void OnMapMemberEnd(MapMember mapMember);

	void OnMapAppearanceRuleBegin(MapAppearanceRule mapRule, bool outputDynamicMembers, ref int index, ref bool walkRule);

	void OnMapAppearanceRuleEnd(MapAppearanceRule mapRule);
}
