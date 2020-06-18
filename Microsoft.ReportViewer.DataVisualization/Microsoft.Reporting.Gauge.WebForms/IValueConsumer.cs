namespace Microsoft.Reporting.Gauge.WebForms
{
	internal interface IValueConsumer
	{
		void ProviderRemoved(IValueProvider provider);

		void ProviderNameChanged(IValueProvider provider);

		void InputValueChanged(object sender, ValueChangedEventArgs e);

		void Reset();

		void Refresh();

		IValueProvider GetProvider();
	}
}
