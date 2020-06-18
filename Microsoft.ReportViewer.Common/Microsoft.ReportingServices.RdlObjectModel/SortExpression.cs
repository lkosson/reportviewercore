using System.ComponentModel;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class SortExpression : ReportObject
	{
		internal class Definition : DefinitionStore<SortExpression, Definition.Properties>
		{
			internal enum Properties
			{
				Value,
				Direction
			}

			private Definition()
			{
			}
		}

		public ReportExpression Value
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		[DefaultValue(SortDirections.Ascending)]
		public SortDirections Direction
		{
			get
			{
				return (SortDirections)base.PropertyStore.GetInteger(1);
			}
			set
			{
				base.PropertyStore.SetInteger(1, (int)value);
			}
		}

		public SortExpression()
		{
		}

		internal SortExpression(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
