using System.IO;
using System.Text;

namespace Microsoft.Reporting.NETCore
{
	public delegate Stream CreateStreamCallback(string name, string extension, Encoding encoding, string mimeType, bool willSeek);
}
