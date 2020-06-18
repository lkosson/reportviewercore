namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal abstract class ReportElement : ReportObject
	{
		internal class Definition : DefinitionStore<ReportElement, Definition.Properties>
		{
			internal enum Properties
			{
				Style
			}

			private Definition()
			{
			}
		}

		public Style Style
		{
			get
			{
				return (Style)base.PropertyStore.GetObject(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		public ReportElement()
		{
		}

		internal ReportElement(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
