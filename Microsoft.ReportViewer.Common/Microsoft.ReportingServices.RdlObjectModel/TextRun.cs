using System.ComponentModel;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class TextRun : ReportElement
	{
		internal new class Definition : DefinitionStore<TextRun, Definition.Properties>
		{
			internal enum Properties
			{
				Style,
				Label,
				Value,
				ValueLocID,
				ActionInfo,
				ToolTip,
				ToolTipLocID,
				MarkupType
			}

			private Definition()
			{
			}
		}

		[DefaultValue("")]
		public string Label
		{
			get
			{
				return base.PropertyStore.GetObject<string>(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		public ReportExpression Value
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		public ActionInfo ActionInfo
		{
			get
			{
				return base.PropertyStore.GetObject<ActionInfo>(4);
			}
			set
			{
				base.PropertyStore.SetObject(4, value);
			}
		}

		[ReportExpressionDefaultValue("")]
		public ReportExpression ToolTip
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

		[ReportExpressionDefaultValue(typeof(MarkupType), Microsoft.ReportingServices.RdlObjectModel.MarkupType.None)]
		public ReportExpression<MarkupType> MarkupType
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<MarkupType>>(7);
			}
			set
			{
				base.PropertyStore.SetObject(7, value);
			}
		}

		public TextRun()
		{
		}

		internal TextRun(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			Value = default(ReportExpression);
		}
	}
}
