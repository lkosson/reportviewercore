using Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models.Relationships
{
	internal class XmlPart : RelPart
	{
		private OoxmlPart _hydratedPart;

		public virtual OoxmlPart HydratedPart
		{
			get
			{
				return _hydratedPart;
			}
			set
			{
				_hydratedPart = value;
			}
		}
	}
}
