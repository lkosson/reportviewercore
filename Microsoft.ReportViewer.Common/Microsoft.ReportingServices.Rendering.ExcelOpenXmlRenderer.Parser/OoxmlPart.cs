using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser
{
	internal abstract class OoxmlPart
	{
		public static string XmlDeclaration => "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\" ?>";

		public abstract OoxmlComplexType Root
		{
			get;
		}

		public abstract string Tag
		{
			get;
		}

		public abstract Dictionary<string, string> Namespaces
		{
			get;
		}

		protected OoxmlPart()
		{
		}

		protected OoxmlPart(XmlDocument xml)
		{
		}

		public void Write(TextWriter s)
		{
			s.Write(XmlDeclaration);
			Root.WriteAsRoot(s, Tag, 0, Namespaces);
		}
	}
}
