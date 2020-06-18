using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System.Drawing;

namespace Microsoft.ReportingServices.Rendering.RichText
{
	internal interface ITextRunProps
	{
		string FontFamily
		{
			get;
		}

		float FontSize
		{
			get;
		}

		Color Color
		{
			get;
		}

		bool Bold
		{
			get;
		}

		bool Italic
		{
			get;
		}

		RPLFormat.TextDecorations TextDecoration
		{
			get;
		}

		int IndexInParagraph
		{
			get;
		}

		string FontKey
		{
			get;
			set;
		}

		void AddSplitIndex(int index);
	}
}
