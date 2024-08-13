using System.Collections.Specialized;
using System.Xml;

namespace Microsoft.ReportingServices.Rendering.DataRenderer;

public abstract class AtomVisitor
{
	protected const string NsXmlns = "xmlns";

	protected const string NsUriXmlns = "http://www.w3.org/2000/xmlns/";

	protected const string NsNameAtom = "atom";

	protected const string NsUriAtom = "http://www.w3.org/2005/Atom";

	protected const string NsNameApp = "app";

	protected const string NsUriApp = "http://www.w3.org/2007/app";

	protected const string ElementValueDefault = "Default";

	protected const string RsEntityContainerName = "ReportDataFeedContainer";

	protected XmlWriter m_xmlWriter;

	protected NameValueCollection m_reportServerParameters;

	public AtomVisitor(XmlWriter xmlWriter, NameValueCollection reportServerParameters)
	{
		m_xmlWriter = xmlWriter;
		m_reportServerParameters = reportServerParameters;
	}
}
