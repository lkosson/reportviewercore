using Microsoft.ReportingServices.RdlObjectModel;
using System.ComponentModel;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	internal class Axis2005 : ChartAxis
	{
		internal new class Definition : DefinitionStore<Axis2005, Definition.Properties>
		{
			public enum Properties
			{
				Visible = 46,
				Style,
				Title,
				Margin,
				MajorTickMarks,
				MinorTickMarks,
				MajorGridLines,
				MinorGridLines,
				MajorInterval,
				MinorInterval,
				Reverse,
				Location,
				Interlaced,
				Min,
				Max,
				LogScale,
				Angle
			}

			private Definition()
			{
			}
		}

		[DefaultValue(false)]
		public new bool Visible
		{
			get
			{
				return base.PropertyStore.GetBoolean(46);
			}
			set
			{
				base.PropertyStore.SetBoolean(46, value);
			}
		}

		public new Style2005 Style
		{
			get
			{
				return (Style2005)base.PropertyStore.GetObject(47);
			}
			set
			{
				base.PropertyStore.SetObject(47, value);
			}
		}

		public Title2005 Title
		{
			get
			{
				return (Title2005)base.PropertyStore.GetObject(48);
			}
			set
			{
				base.PropertyStore.SetObject(48, value);
			}
		}

		[DefaultValue(false)]
		public new bool Margin
		{
			get
			{
				return base.PropertyStore.GetBoolean(49);
			}
			set
			{
				base.PropertyStore.SetBoolean(49, value);
			}
		}

		[DefaultValue(TickMarks2005.None)]
		public TickMarks2005 MajorTickMarks
		{
			get
			{
				return (TickMarks2005)base.PropertyStore.GetInteger(50);
			}
			set
			{
				base.PropertyStore.SetInteger(50, (int)value);
			}
		}

		[DefaultValue(TickMarks2005.None)]
		public TickMarks2005 MinorTickMarks
		{
			get
			{
				return (TickMarks2005)base.PropertyStore.GetInteger(51);
			}
			set
			{
				base.PropertyStore.SetInteger(51, (int)value);
			}
		}

		public GridLines2005 MajorGridLines
		{
			get
			{
				return (GridLines2005)base.PropertyStore.GetObject(52);
			}
			set
			{
				base.PropertyStore.SetObject(52, value);
			}
		}

		public GridLines2005 MinorGridLines
		{
			get
			{
				return (GridLines2005)base.PropertyStore.GetObject(53);
			}
			set
			{
				base.PropertyStore.SetObject(53, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression MajorInterval
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(54);
			}
			set
			{
				base.PropertyStore.SetObject(54, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression MinorInterval
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(55);
			}
			set
			{
				base.PropertyStore.SetObject(55, value);
			}
		}

		[DefaultValue(false)]
		public new bool Reverse
		{
			get
			{
				return base.PropertyStore.GetBoolean(56);
			}
			set
			{
				base.PropertyStore.SetBoolean(56, value);
			}
		}

		[DefaultValue(false)]
		public new bool Interlaced
		{
			get
			{
				return base.PropertyStore.GetBoolean(58);
			}
			set
			{
				base.PropertyStore.SetBoolean(58, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression Min
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(59);
			}
			set
			{
				base.PropertyStore.SetObject(59, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression Max
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(60);
			}
			set
			{
				base.PropertyStore.SetObject(60, value);
			}
		}

		[DefaultValue(false)]
		public new bool LogScale
		{
			get
			{
				return base.PropertyStore.GetBoolean(61);
			}
			set
			{
				base.PropertyStore.SetBoolean(61, value);
			}
		}

		[DefaultValue("")]
		public new string Angle
		{
			get
			{
				return (string)base.PropertyStore.GetObject(62);
			}
			set
			{
				base.PropertyStore.SetObject(62, value);
			}
		}

		public Axis2005()
		{
		}

		public Axis2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.CustomProperties = new RdlCollection<CustomProperty>();
			MajorTickMarks = TickMarks2005.None;
			MinorTickMarks = TickMarks2005.None;
		}
	}
}
