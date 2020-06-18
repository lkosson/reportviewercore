namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal abstract class MapSubItem : ReportObject
	{
		internal class Definition : DefinitionStore<MapSubItem, Definition.Properties>
		{
			internal enum Properties
			{
				Style,
				MapLocation,
				MapSize,
				LeftMargin,
				RightMargin,
				TopMargin,
				BottomMargin,
				ZIndex
			}

			private Definition()
			{
			}
		}

		public Style Style
		{
			get
			{
				return (Style)base.PropertyStore.GetObject(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		public MapLocation MapLocation
		{
			get
			{
				return (MapLocation)base.PropertyStore.GetObject(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		public MapSize MapSize
		{
			get
			{
				return (MapSize)base.PropertyStore.GetObject(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ReportSize), "0in")]
		public ReportExpression<ReportSize> LeftMargin
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

		[ReportExpressionDefaultValue(typeof(ReportSize), "0in")]
		public ReportExpression<ReportSize> RightMargin
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

		[ReportExpressionDefaultValue(typeof(ReportSize), "0in")]
		public ReportExpression<ReportSize> TopMargin
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportSize>>(5);
			}
			set
			{
				base.PropertyStore.SetObject(5, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ReportSize), "0in")]
		public ReportExpression<ReportSize> BottomMargin
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportSize>>(6);
			}
			set
			{
				base.PropertyStore.SetObject(6, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(int), "0")]
		public ReportExpression<int> ZIndex
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<int>>(7);
			}
			set
			{
				base.PropertyStore.SetObject(7, value);
			}
		}

		public MapSubItem()
		{
		}

		internal MapSubItem(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			ZIndex = 0;
		}
	}
}
