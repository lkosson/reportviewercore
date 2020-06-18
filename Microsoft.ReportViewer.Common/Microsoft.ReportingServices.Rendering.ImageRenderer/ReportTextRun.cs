using Microsoft.ReportingServices.Rendering.RichText;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Microsoft.ReportingServices.Rendering.ImageRenderer
{
	internal class ReportTextRun : ITextRunProps
	{
		private string m_uniqueName;

		private RPLActionInfo m_actionInfo;

		private RPLElementStyle m_sourceStyle;

		private float m_fontSize = 10f;

		private string m_fontKey;

		private Color m_color;

		public string UniqueName => m_uniqueName;

		public RPLActionInfo ActionInfo => m_actionInfo;

		public string FontFamily
		{
			get
			{
				object obj = m_sourceStyle[20];
				if (obj == null)
				{
					return "Arial";
				}
				return (string)obj;
			}
		}

		public float FontSize => m_fontSize;

		public Color Color => m_color;

		public bool Bold
		{
			get
			{
				object obj = m_sourceStyle[22];
				if (obj == null)
				{
					return false;
				}
				return SharedRenderer.IsWeightBold((RPLFormat.FontWeights)obj);
			}
		}

		public bool Italic
		{
			get
			{
				object obj = m_sourceStyle[19];
				if (obj == null)
				{
					return false;
				}
				return (RPLFormat.FontStyles)obj == RPLFormat.FontStyles.Italic;
			}
		}

		public RPLFormat.TextDecorations TextDecoration
		{
			get
			{
				object obj = m_sourceStyle[24];
				if (obj == null)
				{
					return RPLFormat.TextDecorations.None;
				}
				return (RPLFormat.TextDecorations)obj;
			}
		}

		public int IndexInParagraph => -1;

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

		internal ReportTextRun(RPLElementStyle sourceStyle, Dictionary<string, float> cachedReportSizes, Dictionary<string, Color> cachedReportColors)
		{
			m_sourceStyle = sourceStyle;
			SetFontSize(cachedReportSizes);
			SetFontColor(cachedReportColors);
		}

		internal ReportTextRun(RPLElementStyle sourceStyle, string uniqueName, RPLActionInfo sourceActionInfo, Dictionary<string, float> cachedReportSizes, Dictionary<string, Color> cachedReportColors)
			: this(sourceStyle, cachedReportSizes, cachedReportColors)
		{
			m_uniqueName = uniqueName;
			m_actionInfo = sourceActionInfo;
		}

		public void AddSplitIndex(int index)
		{
		}

		private void SetFontSize(Dictionary<string, float> cachedReportSizes)
		{
			string text = (string)m_sourceStyle[21];
			if (!string.IsNullOrEmpty(text))
			{
				float value;
				if (cachedReportSizes == null)
				{
					value = (float)new RPLReportSize(text).ToPoints();
				}
				else if (!cachedReportSizes.TryGetValue(text, out value))
				{
					value = (float)new RPLReportSize(text).ToPoints();
					cachedReportSizes.Add(text, value);
				}
				m_fontSize = value;
			}
		}

		private void SetFontColor(Dictionary<string, Color> cachedReportColors)
		{
			string text = (string)m_sourceStyle[27];
			if (string.IsNullOrEmpty(text) || string.Compare(text, "TRANSPARENT", StringComparison.OrdinalIgnoreCase) == 0)
			{
				m_color = Color.Black;
			}
			else if (cachedReportColors == null)
			{
				m_color = new RPLReportColor(text).ToColor();
			}
			else if (!cachedReportColors.TryGetValue(text, out m_color))
			{
				m_color = new RPLReportColor(text).ToColor();
				cachedReportColors.Add(text, m_color);
			}
		}
	}
}
