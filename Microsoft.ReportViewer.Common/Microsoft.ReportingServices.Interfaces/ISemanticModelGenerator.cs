using System.Data;
using System.Security.Permissions;
using System.Xml;

namespace Microsoft.ReportingServices.Interfaces
{
	public interface ISemanticModelGenerator : IExtension
	{
		void Generate(IDbConnection connection, XmlWriter newModelWriter);

		void ReGenerateModel(IDbConnection connection, XmlReader currentModelReader, XmlWriter newModelWriter);
	}
}
