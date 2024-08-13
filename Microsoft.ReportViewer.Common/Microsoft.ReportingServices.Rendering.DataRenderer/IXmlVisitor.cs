using System;

namespace Microsoft.ReportingServices.Rendering.DataRenderer;

internal interface IXmlVisitor
{
	void StartReport(string name, bool firstInstance);

	void StartRootReport(string name);

	void StartReportSection(string name, bool firstInstance);

	void StartDataRegion(string name, bool firstInstance, bool optional);

	void StartGroup(string name, bool firstInstance);

	void StartTablixMember(string name, bool isStatic, bool firstInstance);

	void StartCollection(string name, bool firstInstance);

	void StartCell(string name, bool firstInstance);

	void StartRectangle(string name, bool firstInstance);

	void StartChartMember(string name, bool isStatic, bool firstInstance);

	void StartChartDataPoint(string name, bool firstInstance);

	void StartMapLayer(string name, bool firstInstance);

	void EndElement(bool firstInstance);

	void ValueElement(string name, object val, TypeCode tc, bool firstInstance);

	void ValueAttribute(string name, object val, TypeCode tc, bool firstInstance);

	void ValueElement(string name, object val, string ID, bool firstInstance);

	void ValueAttribute(string name, object val, string ID, bool firstInstance);

	RowCount Count(bool noRows);

	void Flush();

	string EncodeString(string aName);

	string getNameTag();

	string getXmlRdlNameTag();

	string getDefaultReportTag();

	string getDefaultGroupDetailTag();

	string getDefaultTextboxTag();

	string getDefaultTablixTag();

	string getDefaultGroupCollectionTag(bool isColumn, int aCellIndex);

	string getDefaultCellTag(string aCellID);

	string getDefaultTablixStaticMemberTag(bool isColumn, int aCellIndex);

	string getDefaultRectangleTag();

	string getDefaultSubreportTag();

	string getDefaultChartTag();

	string getDefaultChartMemberTag(bool isCategory, int aCellIndex);

	string getDefaultChartDataPointTag();

	string getChartDataValueTag(int aIndex);

	string getDefaultChartCollectionTag(bool isCategory, int aIndex);

	string getChartMemberLabelTag();

	string getDefaultGaugePanelTag();

	string getGaugeTag(int aGaugeIndex);

	string getStateIndicatorTag(int index);

	string getDefaultStateNameTag();

	string getIndicatorStateCollectionTag();

	string getIndicatorStateTag(int index);

	string getGaugeScaleCollectionTag();

	string getGaugeScaleTag(int aGaugeScaleIndex);

	string getGaugePointerCollectionTag();

	string getGaugePointerTag(int aGaugePointerIndex);

	string getDefaultGaugeInputValueTag();

	string getGaugeScaleRangeCollectionTag();

	string getGaugeScaleRangeTag(int aGaugeScaleRangeIndex);

	string getDefaultGaugeStartValueTag();

	string getDefaultGaugeEndValueTag();

	string getDefaultGaugeMinimumValueTag();

	string getDefaultGaugeMaximumValueTag();

	string getDefaultSectionTag();

	string getDefaultMapRuleValueTag(int index);
}
