using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Rels.Relationships
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
