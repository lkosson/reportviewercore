namespace Microsoft.Reporting.Map.WebForms
{
	internal interface ILayerElement
	{
		string Layer
		{
			get;
			set;
		}

		bool BelongsToLayer
		{
			get;
		}

		bool BelongsToAllLayers
		{
			get;
		}

		Layer LayerObject
		{
			get;
			set;
		}
	}
}
