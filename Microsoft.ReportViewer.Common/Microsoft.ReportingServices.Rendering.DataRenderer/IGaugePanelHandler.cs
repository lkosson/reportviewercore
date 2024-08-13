using Microsoft.ReportingServices.OnDemandReportRendering;

namespace Microsoft.ReportingServices.Rendering.DataRenderer;

internal interface IGaugePanelHandler
{
	void OnGaugePanelBegin(GaugePanel gaugePanel, bool outputGaugePanel, ref bool walkGaugePanel);

	void OnGaugePanelEnd(GaugePanel gaugePanel);

	void OnGaugeInputValueBegin(GaugeInputValue gaugeInputValue, string defaultGaugeInputValueColName, bool output, object value, ref bool walkThisGaugeInputValue);

	void OnGaugeInputValueEnd(GaugeInputValue gaugeInputVAlue);

	void OnStateIndicatorStateNameBegin(StateIndicator stateIndicator, string defaultStateIndicatorStateNameColName, bool output, ref bool walkThisStateName);

	void OnStateIndicatorStateNameEnd(StateIndicator stateIndicator);
}
