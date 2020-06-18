using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class Rectangle : ReportItem
	{
		internal new class Definition : DefinitionStore<Rectangle, Definition.Properties>
		{
			internal enum Properties
			{
				Style,
				Name,
				ActionInfo,
				Top,
				Left,
				Height,
				Width,
				ZIndex,
				Visibility,
				ToolTip,
				ToolTipLocID,
				DocumentMapLabel,
				DocumentMapLabelLocID,
				Bookmark,
				RepeatWith,
				CustomProperties,
				DataElementName,
				DataElementOutput,
				ReportItems,
				PageBreak,
				KeepTogether,
				OmitBorderOnPageBreak,
				LinkToChild,
				PageName,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		[XmlElement(typeof(RdlCollection<ReportItem>))]
		public IList<ReportItem> ReportItems
		{
			get
			{
				return (IList<ReportItem>)base.PropertyStore.GetObject(18);
			}
			set
			{
				base.PropertyStore.SetObject(18, value);
			}
		}

		public PageBreak PageBreak
		{
			get
			{
				return (PageBreak)base.PropertyStore.GetObject(19);
			}
			set
			{
				base.PropertyStore.SetObject(19, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression PageName
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(23);
			}
			set
			{
				base.PropertyStore.SetObject(23, value);
			}
		}

		[DefaultValue(false)]
		public bool KeepTogether
		{
			get
			{
				return base.PropertyStore.GetBoolean(20);
			}
			set
			{
				base.PropertyStore.SetBoolean(20, value);
			}
		}

		[DefaultValue(false)]
		public bool OmitBorderOnPageBreak
		{
			get
			{
				return base.PropertyStore.GetBoolean(21);
			}
			set
			{
				base.PropertyStore.SetBoolean(21, value);
			}
		}

		[DefaultValue("")]
		public string LinkToChild
		{
			get
			{
				return (string)base.PropertyStore.GetObject(22);
			}
			set
			{
				base.PropertyStore.SetObject(22, value);
			}
		}

		[DefaultValue(DataElementOutputTypes.Auto)]
		[ValidEnumValues("RectangleDataElementOutputTypes")]
		public new DataElementOutputTypes DataElementOutput
		{
			get
			{
				return base.DataElementOutput;
			}
			set
			{
				base.DataElementOutput = value;
			}
		}

		public Rectangle()
		{
		}

		internal Rectangle(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			ReportItems = new RdlCollection<ReportItem>();
		}
	}
}
