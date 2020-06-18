using System;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal interface IValueProvider
	{
		void AttachConsumer(IValueConsumer consumer);

		void DetachConsumer(IValueConsumer consumer);

		double GetValue();

		DateTime GetDate();

		string GetValueProviderName();

		HistoryCollection GetData(GaugeDuration period, DateTime currentDate);

		bool GetPlayBackMode();

		ValueState GetProvderState();
	}
}
