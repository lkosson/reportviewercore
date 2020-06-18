using System.Globalization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class MapDistanceScale : MapDockableSubItem
	{
		internal new class Definition : DefinitionStore<MapDistanceScale, Definition.Properties>
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
				MapPosition,
				DockOutsideViewport,
				Hidden,
				ToolTip,
				ScaleColor,
				ScaleBorderColor,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		[ReportExpressionDefaultValue(typeof(ReportColor), "White")]
		public ReportExpression<ReportColor> ScaleColor
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportColor>>(13);
			}
			set
			{
				base.PropertyStore.SetObject(13, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ReportColor), "DarkGray")]
		public ReportExpression<ReportColor> ScaleBorderColor
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportColor>>(14);
			}
			set
			{
				base.PropertyStore.SetObject(14, value);
			}
		}

		public MapDistanceScale()
		{
		}

		internal MapDistanceScale(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			ScaleColor = new ReportExpression<ReportColor>("White", CultureInfo.InvariantCulture);
			ScaleBorderColor = new ReportExpression<ReportColor>("DarkGray", CultureInfo.InvariantCulture);
		}
	}
}
