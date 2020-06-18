namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class FrameBackground : ReportObject
	{
		internal class Definition : DefinitionStore<FrameBackground, Definition.Properties>
		{
			internal enum Properties
			{
				Style,
				PropertyCount
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

		public FrameBackground()
		{
		}

		internal FrameBackground(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
