namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal abstract class MapDockableSubItem : MapSubItem
	{
		internal new class Definition : DefinitionStore<MapDockableSubItem, Definition.Properties>
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
				ZIndex,
				ActionInfo,
				Position,
				DockOutsideViewport,
				Hidden,
				ToolTip
			}

			private Definition()
			{
			}
		}

		public ActionInfo ActionInfo
		{
			get
			{
				return (ActionInfo)base.PropertyStore.GetObject(8);
			}
			set
			{
				base.PropertyStore.SetObject(8, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(MapPositions), MapPositions.TopCenter)]
		public ReportExpression<MapPositions> Position
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<MapPositions>>(9);
			}
			set
			{
				base.PropertyStore.SetObject(9, value);
			}
		}

		public ReportExpression<bool> DockOutsideViewport
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(10);
			}
			set
			{
				base.PropertyStore.SetObject(10, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> Hidden
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(11);
			}
			set
			{
				base.PropertyStore.SetObject(11, value);
			}
		}

		[ReportExpressionDefaultValue("")]
		public ReportExpression ToolTip
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(12);
			}
			set
			{
				base.PropertyStore.SetObject(12, value);
			}
		}

		public MapDockableSubItem()
		{
		}

		internal MapDockableSubItem(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			Position = MapPositions.TopCenter;
		}
	}
}
