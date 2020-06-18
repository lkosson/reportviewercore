namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class MapLine : MapSpatialElement
	{
		internal new class Definition : DefinitionStore<MapLine, Definition.Properties>
		{
			internal enum Properties
			{
				VectorData,
				MapFields,
				UseCustomLineTemplate,
				MapLineTemplate,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> UseCustomLineTemplate
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

		public MapLineTemplate MapLineTemplate
		{
			get
			{
				return (MapLineTemplate)base.PropertyStore.GetObject(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		public MapLine()
		{
		}

		internal MapLine(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
		}
	}
}
