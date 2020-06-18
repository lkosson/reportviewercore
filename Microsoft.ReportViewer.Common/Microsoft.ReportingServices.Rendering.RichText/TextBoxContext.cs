using System.Globalization;

namespace Microsoft.ReportingServices.Rendering.RichText
{
	internal class TextBoxContext
	{
		internal int ParagraphIndex;

		internal int TextRunIndex;

		internal int TextRunCharacterIndex;

		internal TextBoxContext()
		{
		}

		internal void IncrementParagraph()
		{
			ParagraphIndex++;
			TextRunIndex = 0;
			TextRunCharacterIndex = 0;
		}

		internal TextBoxContext Clone()
		{
			return new TextBoxContext
			{
				ParagraphIndex = ParagraphIndex,
				TextRunIndex = TextRunIndex,
				TextRunCharacterIndex = TextRunCharacterIndex
			};
		}

		internal void Reset()
		{
			ParagraphIndex = 0;
			TextRunIndex = 0;
			TextRunCharacterIndex = 0;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "P:{0} TR:{1} NCI:{2}", ParagraphIndex, TextRunIndex, TextRunCharacterIndex);
		}
	}
}
