using System.ComponentModel;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal abstract class MapSpatialElementTemplate : ReportObject
	{
		internal class Definition : DefinitionStore<MapSpatialElementTemplate, Definition.Properties>
		{
			internal enum Properties
			{
				Style,
				ActionInfo,
				Hidden,
				OffsetX,
				OffsetY,
				Label,
				ToolTip,
				DataElementName,
				DataElementOutput,
				DataElementLabel
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

		public ActionInfo ActionInfo
		{
			get
			{
				return (ActionInfo)base.PropertyStore.GetObject(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> Hidden
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

		[ReportExpressionDefaultValue(typeof(double), 0)]
		public ReportExpression<double> OffsetX
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<double>>(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(double), 0)]
		public ReportExpression<double> OffsetY
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<double>>(4);
			}
			set
			{
				base.PropertyStore.SetObject(4, value);
			}
		}

		[ReportExpressionDefaultValue("")]
		public ReportExpression Label
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(5);
			}
			set
			{
				base.PropertyStore.SetObject(5, value);
			}
		}

		[ReportExpressionDefaultValue("")]
		public ReportExpression ToolTip
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(6);
			}
			set
			{
				base.PropertyStore.SetObject(6, value);
			}
		}

		[DefaultValue("")]
		public string DataElementName
		{
			get
			{
				return (string)base.PropertyStore.GetObject(7);
			}
			set
			{
				base.PropertyStore.SetObject(7, value);
			}
		}

		[DefaultValue(DataElementOutputTypes.Output)]
		[ValidEnumValues("MapDataElementOutputTypes")]
		public DataElementOutputTypes DataElementOutput
		{
			get
			{
				return (DataElementOutputTypes)base.PropertyStore.GetInteger(8);
			}
			set
			{
				((EnumProperty)DefinitionStore<MapSpatialElementTemplate, Definition.Properties>.GetProperty(8)).Validate(this, (int)value);
				base.PropertyStore.SetInteger(8, (int)value);
			}
		}

		[ReportExpressionDefaultValue("")]
		public ReportExpression DataElementLabel
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(9);
			}
			set
			{
				base.PropertyStore.SetObject(9, value);
			}
		}

		public MapSpatialElementTemplate()
		{
		}

		internal MapSpatialElementTemplate(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			DataElementOutput = DataElementOutputTypes.Output;
		}
	}
}
