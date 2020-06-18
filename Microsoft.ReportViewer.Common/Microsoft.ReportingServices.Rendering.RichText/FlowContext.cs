namespace Microsoft.ReportingServices.Rendering.RichText
{
	internal class FlowContext
	{
		internal float Width;

		internal float Height;

		internal float ContentOffset;

		internal bool WordTrim = true;

		internal bool LineLimit = true;

		internal float OmittedLineHeight;

		internal bool AtEndOfTextBox;

		internal TextBoxContext Context = new TextBoxContext();

		internal TextBoxContext ClipContext;

		internal bool Updatable;

		internal bool VerticalCanGrow;

		internal bool ForcedCharTrim;

		internal bool CharTrimLastLine = true;

		internal int CharTrimmedRunWidth;

		private FlowContext()
		{
		}

		internal FlowContext(float width, float height)
		{
			Width = width;
			Height = height;
		}

		internal FlowContext(float width, float height, int paragraphIndex, int runIndex, int runCharIndex)
		{
			Width = width;
			Height = height;
			Context.ParagraphIndex = paragraphIndex;
			Context.TextRunIndex = runIndex;
			Context.TextRunCharacterIndex = runCharIndex;
		}

		internal FlowContext(float width, float height, bool wordTrim, bool lineLimit)
			: this(width, height)
		{
			WordTrim = wordTrim;
			LineLimit = lineLimit;
		}

		internal FlowContext(float width, float height, TextBoxContext context)
			: this(width, height)
		{
			Context = context;
		}

		internal FlowContext Clone()
		{
			FlowContext flowContext = (FlowContext)MemberwiseClone();
			flowContext.Context = Context.Clone();
			if (ClipContext != null)
			{
				flowContext.ClipContext = ClipContext.Clone();
			}
			return flowContext;
		}

		internal void Reset()
		{
			Context.Reset();
			ClipContext = null;
			ContentOffset = 0f;
			AtEndOfTextBox = false;
			OmittedLineHeight = 0f;
			CharTrimmedRunWidth = 0;
			ForcedCharTrim = false;
		}
	}
}
