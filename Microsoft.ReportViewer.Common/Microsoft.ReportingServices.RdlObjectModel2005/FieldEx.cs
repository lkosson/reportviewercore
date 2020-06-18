using Microsoft.ReportingServices.RdlObjectModel;
using Microsoft.ReportingServices.RdlObjectModel.Serialization;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	[XmlElementClass("Field")]
	internal class FieldEx : Field
	{
		private string m_typeName;

		[XmlElement(Namespace = "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner")]
		public string TypeName
		{
			get
			{
				return m_typeName;
			}
			set
			{
				m_typeName = value;
			}
		}
	}
}
