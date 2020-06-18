using Microsoft.ReportingServices.RdlObjectModel.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class ReportParametersLayout : ReportObject, IShouldSerialize
	{
		internal class Definition : DefinitionStore<GridLayoutDefinition, Definition.Properties>
		{
			internal enum Properties
			{
				GridLayoutDefinition
			}

			private Definition()
			{
			}
		}

		public GridLayoutDefinition GridLayoutDefinition
		{
			get
			{
				return base.PropertyStore.GetObject<GridLayoutDefinition>(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		public ReportParametersLayout()
		{
			GridLayoutDefinition = new GridLayoutDefinition();
		}

		internal ReportParametersLayout(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		bool IShouldSerialize.ShouldSerializeThis()
		{
			return true;
		}

		SerializationMethod IShouldSerialize.ShouldSerializeProperty(string name)
		{
			return SerializationMethod.Auto;
		}
	}
}
