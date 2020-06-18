using Microsoft.ReportingServices.RdlObjectModel;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	internal class DataValue2005 : ChartDataPointValues
	{
		internal new class Definition : DefinitionStore<DataValue2005, Definition.Properties>
		{
			public enum Properties
			{
				Name = 9,
				Value
			}

			private Definition()
			{
			}
		}

		public string Name
		{
			get
			{
				return (string)base.PropertyStore.GetObject(9);
			}
			set
			{
				base.PropertyStore.SetObject(9, value);
			}
		}

		public string Value
		{
			get
			{
				return (string)base.PropertyStore.GetObject(10);
			}
			set
			{
				base.PropertyStore.SetObject(10, value);
			}
		}

		public DataValue2005()
		{
		}

		public DataValue2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
