using Microsoft.ReportingServices.RdlObjectModel;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	internal class Marker2005 : ChartMarker
	{
		internal new class Definition : DefinitionStore<Marker2005, Definition.Properties>
		{
			public enum Properties
			{
				Type = 3,
				Size,
				Style
			}

			private Definition()
			{
			}
		}

		[DefaultValueConstant("DefaultZeroSize")]
		public new ReportSize Size
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

		public new EmptyColorStyle2005 Style
		{
			get
			{
				return (EmptyColorStyle2005)base.PropertyStore.GetObject(5);
			}
			set
			{
				base.PropertyStore.SetObject(5, value);
			}
		}

		public Marker2005()
		{
		}

		public Marker2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
