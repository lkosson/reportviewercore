using System.Collections.Generic;

namespace Microsoft.Reporting.Common
{
	internal sealed class ResourceList
	{
		private const string DebugTag = "_Debug";

		public const string MimeTypeImage = "image/gif";

		public const string MimeTypeImagePng = "image/png";

		public const string MimeTypeImageSvg = "image/svg+xml";

		public const string MimeTypeScript = "application/javascript";

		public const string MimeTypeStyle = "text/css";

		public const string MimeTypeHtml = "text/html";

		public const string XFontOpenType = "application/x-font-opentype";

		public const string MimeTypeCab = "application/vnd.ms-cab-compressed";

		private readonly Dictionary<string, ResourceItem> m_items = new Dictionary<string, ResourceItem>();

		internal Dictionary<string, ResourceItem> Resources => m_items;

		internal void Add(string name, string mimeType)
		{
			Add(name, mimeType, hasDebugMode: false);
		}

		internal void Add(string name, string mimeType, bool hasDebugMode)
		{
			ResourceItem value = hasDebugMode ? new ResourceItem(name, name + "_Debug", mimeType) : new ResourceItem(name, mimeType);
			m_items.Add(name, value);
		}

		internal bool TryGetResourceItem(string name, out ResourceItem item)
		{
			return m_items.TryGetValue(name, out item);
		}
	}
}
