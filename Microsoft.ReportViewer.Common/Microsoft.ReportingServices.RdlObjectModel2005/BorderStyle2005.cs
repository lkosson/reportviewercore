using Microsoft.ReportingServices.RdlObjectModel;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	internal class BorderStyle2005 : ReportObject
	{
		internal class Definition : DefinitionStore<BorderStyle2005, Definition.Properties>
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

		public ReportExpression<BorderStyles2005> Default
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<BorderStyles2005>>(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		public ReportExpression<BorderStyles2005> Left
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<BorderStyles2005>>(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		public ReportExpression<BorderStyles2005> Right
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<BorderStyles2005>>(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		public ReportExpression<BorderStyles2005> Top
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<BorderStyles2005>>(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		public ReportExpression<BorderStyles2005> Bottom
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<BorderStyles2005>>(4);
			}
			set
			{
				base.PropertyStore.SetObject(4, value);
			}
		}

		public BorderStyle2005()
		{
		}

		public BorderStyle2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			Default = BorderStyles2005.None;
		}
	}
}
