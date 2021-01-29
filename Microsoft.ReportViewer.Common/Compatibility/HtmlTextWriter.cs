using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace System.Web.UI
{
	class HtmlTextWriter : StreamWriter
	{
		public HtmlTextWriter(Stream stream, Encoding encoding) : base(stream, encoding)
		{
		}
	}
}
