using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser
{
	internal interface IOoxmlComplexType
	{
		void WriteAsRoot(TextWriter s, string tagName, Dictionary<string, string> namespaces);

		void Write(TextWriter s, string tagName);

		void WriteOpenTag(TextWriter s, string tagName, Dictionary<string, string> namespaces);

		void WriteCloseTag(TextWriter s, string tagName);

		void WriteAttributes(TextWriter s);

		void WriteElements(TextWriter s);
	}
}
