using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal static class OpenXmlFieldGenerators
	{
		private sealed class FieldWithCache : OpenXmlParagraphModel.IParagraphContent
		{
			private readonly OpenXmlRunPropertiesModel _properties;

			private readonly string _instructions;

			public FieldWithCache(OpenXmlRunPropertiesModel properties, string instructions)
			{
				_properties = properties;
				_instructions = instructions;
			}

			public void Write(TextWriter writer)
			{
				string value = BeginInstructions(writer, _properties);
				writer.Write(_instructions + "</w:r><w:r>");
				writer.Write(value);
				writer.Write("<w:fldChar w:fldCharType=\"separate\" w:fldLock=\"0\" w:dirty=\"0\"/></w:r><w:r>");
				writer.Write(value);
				writer.Write("<w:t xml:space=\"preserve\">1</w:t></w:r><w:r>");
				writer.Write(value);
				writer.Write("<w:fldChar w:fldCharType=\"end\" w:fldLock=\"0\" w:dirty=\"0\"/></w:r>");
			}
		}

		private sealed class TableOfContentsEntryContent : OpenXmlParagraphModel.IParagraphContent
		{
			private readonly string _label;

			private readonly int _level;

			public TableOfContentsEntryContent(string label, int level)
			{
				_label = label;
				_level = level;
			}

			public void Write(TextWriter writer)
			{
				writer.Write("<w:r><w:fldChar w:fldCharType=\"begin\" w:fldLock=\"0\" w:dirty=\"0\"/></w:r><w:r><w:rPr><w:noProof/></w:rPr><w:instrText xml:space=\"preserve\"> TC &quot;");
				writer.Write(Escape(_label));
				writer.Write("&quot; \\f C \\l &quot;");
				writer.Write(_level.ToString(CultureInfo.InvariantCulture));
				writer.Write("&quot; </w:instrText></w:r><w:r><w:fldChar w:fldCharType=\"end\" w:fldLock=\"0\" w:dirty=\"0\"/></w:r>");
			}
		}

		private sealed class StartHyperlinkContent : OpenXmlParagraphModel.IParagraphContent
		{
			private readonly string _target;

			private readonly bool _bookmarkLink;

			public StartHyperlinkContent(string target, bool bookmarkLink)
			{
				_target = target;
				_bookmarkLink = bookmarkLink;
			}

			public void Write(TextWriter writer)
			{
				writer.Write("<w:r><w:fldChar w:fldCharType=\"begin\" w:fldLock=\"0\" w:dirty=\"0\"/></w:r><w:r><w:rPr><w:noProof/></w:rPr>");
				if (_bookmarkLink)
				{
					writer.Write("<w:instrText xml:space=\"preserve\"> HYPERLINK \\l &quot;");
					writer.Write(OpenXmlBookmarkModel.CleanseBookmarkName(_target));
				}
				else
				{
					writer.Write("<w:instrText xml:space=\"preserve\"> HYPERLINK &quot;");
					writer.Write(Escape(_target));
				}
				writer.Write("&quot; </w:instrText></w:r><w:r><w:fldChar w:fldCharType=\"separate\" w:fldLock=\"0\" w:dirty=\"0\"/></w:r>");
			}
		}

		private sealed class EndHyperlinkContent : OpenXmlParagraphModel.IParagraphContent
		{
			public void Write(TextWriter writer)
			{
				writer.Write("<w:r><w:fldChar w:fldCharType=\"end\" w:fldLock=\"0\" w:dirty=\"0\"/></w:r>");
			}
		}

		private static string BeginInstructions(TextWriter writer, OpenXmlRunPropertiesModel properties)
		{
			StringBuilder stringBuilder = new StringBuilder();
			using (StringWriter q = new StringWriter(stringBuilder, CultureInfo.InvariantCulture))
			{
				properties.Write(q);
			}
			string text = stringBuilder.ToString();
			writer.Write("<w:r>");
			writer.Write(text);
			writer.Write("<w:fldChar w:fldCharType=\"begin\" w:fldLock=\"0\" w:dirty=\"0\"/></w:r><w:r>");
			properties.NoProof = true;
			properties.Write(writer);
			properties.NoProof = false;
			return text;
		}

		public static OpenXmlParagraphModel.IParagraphContent PageNumberField(OpenXmlRunPropertiesModel textStyle)
		{
			return new FieldWithCache(textStyle, "<w:instrText xml:space=\"preserve\"> PAGE </w:instrText>");
		}

		public static OpenXmlParagraphModel.IParagraphContent PageCountField(OpenXmlRunPropertiesModel textStyle)
		{
			return new FieldWithCache(textStyle, "<w:instrText xml:space=\"preserve\"> NUMPAGES </w:instrText>");
		}

		public static OpenXmlParagraphModel.IParagraphContent TableOfContentsEntry(string label, int level)
		{
			return new TableOfContentsEntryContent(label, level);
		}

		public static OpenXmlParagraphModel.IParagraphContent StartHyperlink(string target, bool bookmarkLink)
		{
			return new StartHyperlinkContent(target, bookmarkLink);
		}

		private static string Escape(string instruction)
		{
			StringBuilder stringBuilder = new StringBuilder(instruction.Length);
			foreach (char c in instruction)
			{
				if (c < ' ')
				{
					stringBuilder.Append(' ');
				}
				else
				{
					stringBuilder.Append(WordOpenXmlUtils.EscapeChar(c));
				}
			}
			return stringBuilder.ToString();
		}

		public static OpenXmlParagraphModel.IParagraphContent EndHyperlink()
		{
			return new EndHyperlinkContent();
		}
	}
}
