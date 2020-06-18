using Microsoft.ReportingServices.RdlObjectModel;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	internal class BorderColor2005 : ReportObject
	{
		internal class Definition : DefinitionStore<BorderColor2005, Definition.Properties>
		{
			public enum Properties
			{
				Default,
				Left,
				Right,
				Top,
				Bottom
			}

			private Definition()
			{
			}
		}

		public ReportExpression<ReportColor> Default
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportColor>>(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		public ReportExpression<ReportColor> Left
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportColor>>(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		public ReportExpression<ReportColor> Right
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportColor>>(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		public ReportExpression<ReportColor> Top
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportColor>>(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		public ReportExpression<ReportColor> Bottom
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportColor>>(4);
			}
			set
			{
				base.PropertyStore.SetObject(4, value);
			}
		}

		public BorderColor2005()
		{
		}

		public BorderColor2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			Default = Constants.DefaultBorderColor;
		}
	}
}
