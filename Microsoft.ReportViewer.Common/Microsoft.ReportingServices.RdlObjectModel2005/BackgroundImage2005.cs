using Microsoft.ReportingServices.RdlObjectModel;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	internal class BackgroundImage2005 : BackgroundImage
	{
		internal new class Definition : DefinitionStore<BackgroundImage, Definition.Properties>
		{
			public enum Properties
			{
				BackgroundRepeat = 6,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		public new ReportExpression<BackgroundRepeatTypes2005> BackgroundRepeat
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<BackgroundRepeatTypes2005>>(6);
			}
			set
			{
				base.PropertyStore.SetObject(6, value);
			}
		}

		public BackgroundImage2005()
		{
		}

		public BackgroundImage2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
