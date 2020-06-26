using Microsoft.ReportingServices.Rendering.ImageRenderer;
using Microsoft.ReportingServices.Rendering.RichText;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System.Drawing;

namespace Microsoft.Reporting.NETCore
{
	internal sealed class RenderingTextRun : RenderingElement, ITextRunProps
	{
		private TextRun m_richTextRun;

		private string m_fontKey;

		private int m_indexInParagraph;

		internal string Text
		{
			get
			{
				string text = null;
				RPLTextBoxProps rPLTextBoxProps = InstanceProperties as RPLTextBoxProps;
				if (rPLTextBoxProps != null)
				{
					text = rPLTextBoxProps.Value;
					if (string.IsNullOrEmpty(text))
					{
						text = ((RPLTextBoxPropsDef)DefinitionProperties).Value;
					}
				}
				else
				{
					text = ((RPLTextRunProps)InstanceProperties).Value;
					if (string.IsNullOrEmpty(text))
					{
						text = ((RPLTextRunPropsDef)DefinitionProperties).Value;
					}
				}
				return text;
			}
		}

		internal string ToolTip
		{
			get
			{
				string toolTip = (InstanceProperties as RPLTextRunProps).ToolTip;
				if (string.IsNullOrEmpty(toolTip))
				{
					toolTip = (DefinitionProperties as RPLTextRunPropsDef).ToolTip;
				}
				return toolTip;
			}
		}

		internal TextRun RichTextRun => m_richTextRun;

		public string FontFamily
		{
			get
			{
				string stylePropertyValueString = GdiContext.GetStylePropertyValueString(InstanceProperties, 20);
				if (string.IsNullOrEmpty(stylePropertyValueString))
				{
					return "Arial";
				}
				return stylePropertyValueString;
			}
		}

		public float FontSize
		{
			get
			{
				float stylePropertyValueSizePT = GdiContext.GetStylePropertyValueSizePT(InstanceProperties, 21);
				if (float.IsNaN(stylePropertyValueSizePT))
				{
					return 10f;
				}
				return stylePropertyValueSizePT;
			}
		}

		public Color Color
		{
			get
			{
				Color stylePropertyValueColor = GdiContext.GetStylePropertyValueColor(InstanceProperties, 27);
				if (stylePropertyValueColor == Color.Empty)
				{
					return Color.Black;
				}
				return stylePropertyValueColor;
			}
		}

		public bool Bold
		{
			get
			{
				object obj = (RPLFormat.FontWeights)SharedRenderer.GetStylePropertyValueObject(InstanceProperties, 22);
				if (obj != null)
				{
					return SharedRenderer.IsWeightBold((RPLFormat.FontWeights)obj);
				}
				return false;
			}
		}

		public bool Italic
		{
			get
			{
				object obj = (RPLFormat.FontStyles)SharedRenderer.GetStylePropertyValueObject(InstanceProperties, 19);
				if (obj != null)
				{
					return (RPLFormat.FontStyles)obj == RPLFormat.FontStyles.Italic;
				}
				return false;
			}
		}

		public RPLFormat.TextDecorations TextDecoration
		{
			get
			{
				object obj = (RPLFormat.TextDecorations)SharedRenderer.GetStylePropertyValueObject(InstanceProperties, 24);
				if (obj == null)
				{
					return RPLFormat.TextDecorations.None;
				}
				return (RPLFormat.TextDecorations)obj;
			}
		}

		public int IndexInParagraph => m_indexInParagraph;

		public string FontKey
		{
			get
			{
				return m_fontKey;
			}
			set
			{
				m_fontKey = value;
			}
		}

		internal void Initialize(RPLElement rplElement, RectangleF bounds, int indexInParagraph)
		{
			Initialize(rplElement);
			m_indexInParagraph = indexInParagraph;
			m_position = GdiContext.GetMeasurementRectangle(null, bounds);
		}

		public void AddSplitIndex(int index)
		{
		}

		internal override void DrawContent(GdiContext context)
		{
			if (context.SearchHit)
			{
				m_richTextRun = new HighlightTextRun(Text, this);
			}
			else
			{
				m_richTextRun = new TextRun(Text, this);
			}
		}
	}
}
