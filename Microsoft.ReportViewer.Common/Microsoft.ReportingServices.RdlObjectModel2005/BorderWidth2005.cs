using Microsoft.ReportingServices.RdlObjectModel;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	internal class BorderWidth2005 : ReportObject
	{
		internal class Definition : DefinitionStore<BorderWidth2005, Definition.Properties>
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

		public ReportExpression<ReportSize> Default
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportSize>>(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		public ReportExpression<ReportSize> Left
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportSize>>(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		public ReportExpression<ReportSize> Right
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportSize>>(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		public ReportExpression<ReportSize> Top
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportSize>>(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		public ReportExpression<ReportSize> Bottom
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportSize>>(4);
			}
			set
			{
				base.PropertyStore.SetObject(4, value);
			}
		}

		public BorderWidth2005()
		{
		}

		public BorderWidth2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			Default = Constants.DefaultBorderWidth;
		}
	}
}
