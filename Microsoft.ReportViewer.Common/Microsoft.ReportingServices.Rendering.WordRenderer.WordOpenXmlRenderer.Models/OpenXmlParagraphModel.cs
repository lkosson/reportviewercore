using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal sealed class OpenXmlParagraphModel : OpenXmlTableCellModel.ICellContent
	{
		public sealed class PageBreakParagraph : OpenXmlTableCellModel.ICellContent
		{
			public void Write(TextWriter writer)
			{
				writer.Write("<w:p><w:pPr><w:spacing w:after=\"0\" w:line=\"240\" w:lineRule=\"auto\"/><w:rPr><w:sz w:val=\"0\"/></w:rPr></w:pPr><w:r><w:br w:type=\"page\"/></w:r></w:p>");
			}
		}

		public sealed class EmptyParagraph : OpenXmlTableCellModel.ICellContent
		{
			public void Write(TextWriter writer)
			{
				writer.Write("<w:p><w:pPr><w:spacing w:after=\"0\" w:line=\"240\" w:lineRule=\"auto\"/></w:pPr></w:p>");
			}
		}

		public interface IParagraphContent
		{
			void Write(TextWriter writer);
		}

		private OpenXmlParagraphPropertiesModel _properties;

		private List<IParagraphContent> _contents;

		public OpenXmlParagraphPropertiesModel Properties => _properties;

		public OpenXmlParagraphModel()
		{
			_properties = new OpenXmlParagraphPropertiesModel();
			_contents = new List<IParagraphContent>();
		}

		private void AddRun(StringBuilder text, bool breakFirst, OpenXmlRunPropertiesModel style)
		{
			_contents.Add(new OpenXmlTextRunModel(text.ToString(), breakFirst, style));
		}

		public void AddText(string text, OpenXmlRunPropertiesModel style)
		{
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			foreach (char c in text)
			{
				if (c != '\r')
				{
					if (c == '\n')
					{
						AddRun(stringBuilder, !flag, style);
						stringBuilder = new StringBuilder();
						flag = false;
					}
					else if (c < ' ')
					{
						stringBuilder.Append(' ');
					}
					else
					{
						stringBuilder.Append(WordOpenXmlUtils.EscapeChar(c));
					}
				}
			}
			AddRun(stringBuilder, !flag, style);
		}

		public void AddPageNumberField(OpenXmlRunPropertiesModel textStyle)
		{
			_contents.Add(OpenXmlFieldGenerators.PageNumberField(textStyle));
		}

		public void AddPageCountField(OpenXmlRunPropertiesModel textStyle)
		{
			_contents.Add(OpenXmlFieldGenerators.PageCountField(textStyle));
		}

		public void AddLabel(string label, int level, OpenXmlRunPropertiesModel textStyle)
		{
			_contents.Add(OpenXmlFieldGenerators.TableOfContentsEntry(label, level));
		}

		public void StartHyperlink(string target, bool bookmarkLink, OpenXmlRunPropertiesModel textStyle)
		{
			_contents.Add(OpenXmlFieldGenerators.StartHyperlink(target, bookmarkLink));
		}

		public void EndHyperlink(OpenXmlRunPropertiesModel textStyle)
		{
			_contents.Add(OpenXmlFieldGenerators.EndHyperlink());
		}

		public void AddBookmark(string name, int id)
		{
			_contents.Add(new OpenXmlBookmarkModel(name, id));
		}

		public void AddImage(OpenXmlPictureModel picture)
		{
			_contents.Add(picture);
		}

		public void Write(TextWriter writer)
		{
			writer.Write("<w:p>");
			_properties.Write(writer);
			for (int i = 0; i < _contents.Count; i++)
			{
				_contents[i].Write(writer);
			}
			writer.Write("</w:p>");
		}

		void OpenXmlTableCellModel.ICellContent.Write(TextWriter writer)
		{
			Write(writer);
		}

		public static void WritePageBreakParagraph(TextWriter writer)
		{
			writer.Write("<w:p><w:pPr><w:spacing w:after=\"0\" w:line=\"240\" w:lineRule=\"auto\"/><w:rPr><w:sz w:val=\"0\"/></w:rPr></w:pPr><w:r><w:br w:type=\"page\"/></w:r></w:p>");
		}

		public static void WriteInvisibleParagraph(TextWriter writer)
		{
			writer.Write("<w:p><w:pPr><w:spacing w:after=\"0\" w:line=\"0\" w:lineRule=\"auto\"/><w:rPr><w:sz w:val=\"0\"/></w:rPr></w:pPr></w:p>");
		}

		public static void WriteEmptyParagraph(TextWriter writer)
		{
			writer.Write("<w:p><w:pPr><w:spacing w:after=\"0\" w:line=\"240\" w:lineRule=\"auto\"/></w:pPr></w:p>");
		}

		public static void WriteEmptyLayoutCellParagraph(TextWriter writer)
		{
			writer.Write("<w:p><w:pPr><w:pStyle w:val=\"EmptyCellLayoutStyle\"/><w:spacing w:after=\"0\" w:line=\"240\" w:lineRule=\"auto\"/></w:pPr></w:p>");
		}
	}
}
