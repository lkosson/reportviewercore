using System.ComponentModel;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class ConnectionProperties : ReportObject
	{
		internal class Definition : DefinitionStore<ConnectionProperties, Definition.Properties>
		{
			internal enum Properties
			{
				DataProvider,
				ConnectString,
				IntegratedSecurity,
				Prompt,
				PromptLocID
			}
		}

		public string DataProvider
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

		public ReportExpression ConnectString
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		[DefaultValue(false)]
		public bool IntegratedSecurity
		{
			get
			{
				return base.PropertyStore.GetBoolean(2);
			}
			set
			{
				base.PropertyStore.SetBoolean(2, value);
			}
		}

		[DefaultValue("")]
		public string Prompt
		{
			get
			{
				return (string)base.PropertyStore.GetObject(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		public ConnectionProperties()
		{
		}

		internal ConnectionProperties(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
