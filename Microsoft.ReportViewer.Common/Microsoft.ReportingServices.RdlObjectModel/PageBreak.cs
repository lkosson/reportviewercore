using Microsoft.ReportingServices.RdlObjectModel.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class PageBreak : ReportObject, IShouldSerialize
	{
		internal class Definition : DefinitionStore<PageBreak, Definition.Properties>
		{
			internal enum Properties
			{
				BreakLocation,
				Disabled,
				ResetPageNumber
			}

			private Definition()
			{
			}
		}

		public BreakLocations BreakLocation
		{
			get
			{
				return (BreakLocations)base.PropertyStore.GetInteger(0);
			}
			set
			{
				base.PropertyStore.SetInteger(0, (int)value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> Disabled
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> ResetPageNumber
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		public PageBreak()
		{
		}

		internal PageBreak(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		bool IShouldSerialize.ShouldSerializeThis()
		{
			return BreakLocation != BreakLocations.None;
		}

		SerializationMethod IShouldSerialize.ShouldSerializeProperty(string name)
		{
			return SerializationMethod.Auto;
		}
	}
}
