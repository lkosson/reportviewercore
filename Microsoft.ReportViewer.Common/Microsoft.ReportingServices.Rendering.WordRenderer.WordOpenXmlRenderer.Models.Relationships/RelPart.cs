namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models.Relationships
{
	internal abstract class RelPart
	{
		private string _location;

		private string _contentType;

		public string Location
		{
			get
			{
				return _location;
			}
			set
			{
				_location = value;
			}
		}

		public string ContentType
		{
			get
			{
				return _contentType;
			}
			set
			{
				_contentType = value;
			}
		}
	}
}
