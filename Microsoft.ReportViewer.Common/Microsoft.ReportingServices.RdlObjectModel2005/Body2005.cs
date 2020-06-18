using Microsoft.ReportingServices.RdlObjectModel;
using System.ComponentModel;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	internal class Body2005 : Body
	{
		public new class Definition : DefinitionStore<Body2005, Definition.Properties>
		{
			public enum Properties
			{
				Columns = 3,
				ColumnSpacing,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		[DefaultValue(1)]
		[ValidValues(1, 100)]
		public int Columns
		{
			get
			{
				return base.PropertyStore.GetInteger(3);
			}
			set
			{
				((IntProperty)DefinitionStore<Body2005, Definition.Properties>.GetProperty(3)).Validate(this, value);
				base.PropertyStore.SetInteger(3, value);
			}
		}

		public ReportSize ColumnSpacing
		{
			get
			{
				return base.PropertyStore.GetSize(4);
			}
			set
			{
				base.PropertyStore.SetSize(4, value);
			}
		}

		public Body2005()
		{
		}

		public Body2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			Columns = 1;
		}
	}
}
