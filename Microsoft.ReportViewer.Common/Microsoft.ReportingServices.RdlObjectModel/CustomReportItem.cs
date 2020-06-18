using System;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class CustomReportItem : ReportItem
	{
		internal new class Definition : DefinitionStore<CustomReportItem, Definition.Properties>
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
				Type,
				AltReportItem,
				CustomData,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		public string Type
		{
			get
			{
				return (string)base.PropertyStore.GetObject(18);
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				base.PropertyStore.SetObject(18, value);
			}
		}

		public AltReportItem AltReportItem
		{
			get
			{
				return (AltReportItem)base.PropertyStore.GetObject(19);
			}
			set
			{
				base.PropertyStore.SetObject(19, value);
			}
		}

		public CustomData CustomData
		{
			get
			{
				return (CustomData)base.PropertyStore.GetObject(20);
			}
			set
			{
				base.PropertyStore.SetObject(20, value);
			}
		}

		public CustomReportItem()
		{
		}

		internal CustomReportItem(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			Type = "";
		}
	}
}
