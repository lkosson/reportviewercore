using System;
using System.IO;
using System.Text;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal class OpenXmlBookmarkModel : OpenXmlParagraphModel.IParagraphContent
	{
		private readonly string _name;

		private readonly int _id;

		public OpenXmlBookmarkModel(string name, int id)
		{
			_name = name;
			_id = id;
		}

		private static StringBuilder EscapeBookmarkText(string text)
		{
			StringBuilder stringBuilder = new StringBuilder(text.Length);
			for (int i = 0; i < text.Length; i++)
			{
				switch (text[i])
				{
				case '\r':
					if (i < text.Length - 1 && text[i + 1] == '\n')
					{
						stringBuilder.Append('_');
						i++;
					}
					else
					{
						stringBuilder.Append('_');
					}
					break;
				case '\u0001':
				case '\u0002':
				case '\u0003':
				case '\u0004':
				case '\u0005':
				case '\u0006':
				case '\a':
				case '\b':
				case '\t':
				case '\n':
				case '\v':
				case '\f':
				case '\u000e':
				case '\u000f':
				case '\u0010':
				case '\u0011':
				case '\u0012':
				case '\u0013':
				case '\u0014':
				case '\u0015':
				case '\u0016':
				case '\u0017':
				case '\u0018':
				case '\u0019':
				case '\u001a':
				case '\u001b':
				case '\u001c':
				case '\u001d':
				case '\u001e':
				case '\u001f':
				case ' ':
					stringBuilder.Append('_');
					break;
				default:
					stringBuilder.Append(WordOpenXmlUtils.EscapeChar(text[i]));
					break;
				}
			}
			return stringBuilder;
		}

		public static string CleanseBookmarkName(string name)
		{
			StringBuilder stringBuilder = EscapeBookmarkText(name);
			return stringBuilder.ToString(0, Math.Min(40, stringBuilder.Length));
		}

		public void Write(TextWriter writer)
		{
			writer.Write("<w:bookmarkStart w:id=\"");
			writer.Write(_id);
			writer.Write("\" w:name=\"");
			writer.Write(CleanseBookmarkName(_name));
			writer.Write("\"/><w:bookmarkEnd w:id=\"");
			writer.Write(_id);
			writer.Write("\"/>");
		}
	}
}
