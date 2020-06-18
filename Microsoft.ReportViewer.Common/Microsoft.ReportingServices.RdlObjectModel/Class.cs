namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class Class : ReportObject
	{
		internal class Definition : DefinitionStore<Class, Definition.Properties>
		{
			internal enum Properties
			{
				ClassName,
				InstanceName
			}

			private Definition()
			{
			}
		}

		public string ClassName
		{
			get
			{
				return (string)base.PropertyStore.GetObject(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		public string InstanceName
		{
			get
			{
				return (string)base.PropertyStore.GetObject(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		public Class()
		{
		}

		internal Class(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
