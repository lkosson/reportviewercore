using Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.dc.elements.x1_1;
using Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.package.x2006.metadata.core_properties;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal sealed class OpenXmlDocumentPropertiesModel
	{
		private CorePropertiesPart _propertyPart;

		internal CorePropertiesPart PropertiesPart => _propertyPart;

		internal OpenXmlDocumentPropertiesModel(string author, string title, string description)
		{
			_propertyPart = new CorePropertiesPart();
			CT_CoreProperties obj = (CT_CoreProperties)_propertyPart.Root;
			obj.Creator = new SimpleLiteral
			{
				Content = WordOpenXmlUtils.Escape(author)
			};
			obj.Title = new SimpleLiteral
			{
				Content = WordOpenXmlUtils.Escape(title)
			};
			obj.Description = new SimpleLiteral
			{
				Content = WordOpenXmlUtils.Escape(description)
			};
		}
	}
}
