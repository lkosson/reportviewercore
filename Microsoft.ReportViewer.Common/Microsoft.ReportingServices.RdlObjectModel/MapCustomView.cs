namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class MapCustomView : MapView
	{
		internal new class Definition : DefinitionStore<MapCustomView, Definition.Properties>
		{
			internal enum Properties
			{
				Zoom,
				CenterX,
				CenterY,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		[ReportExpressionDefaultValue(typeof(double), "50")]
		public ReportExpression<double> CenterX
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<double>>(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(double), "50")]
		public ReportExpression<double> CenterY
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<double>>(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		public MapCustomView()
		{
		}

		internal MapCustomView(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			CenterX = 50.0;
			CenterY = 50.0;
		}
	}
}
