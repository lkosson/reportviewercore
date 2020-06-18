namespace Microsoft.Reporting.Gauge.WebForms
{
	internal interface IPointerProvider
	{
		double Position
		{
			get;
			set;
		}

		void DataValueChanged(bool initialize);
	}
}
