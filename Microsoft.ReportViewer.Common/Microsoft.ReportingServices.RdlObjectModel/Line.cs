namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class Line : ReportItem
	{
		internal new class Definition : DefinitionStore<Line, Definition.Properties>
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
				PropertyCount
			}

			private Definition()
			{
			}
		}

		public Line()
		{
		}

		internal Line(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
