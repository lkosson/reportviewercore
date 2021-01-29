using Microsoft.ReportingServices.HtmlRendering;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using Microsoft.ReportingServices.Rendering.SPBProcessing;
using System.Collections;
using System.Collections.Specialized;
using System.Text;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal sealed class MHTMLRenderer : ServerRenderer
	{
		private static readonly int MAXLINELENGTH = 950;

		internal Hashtable m_mhtmlStreamNames;

		protected override bool HasFindStringScript => false;

		protected override bool HasInteractiveScript => false;

		protected override bool FillPageHeight => false;

		public MHTMLRenderer(ROMReport report, Microsoft.ReportingServices.Rendering.SPBProcessing.SPBProcessing spbProcessing, NameValueCollection reportServerParams, DeviceInfo deviceInfo, NameValueCollection rawDeviceInfo, NameValueCollection browserCaps, CreateAndRegisterStream createAndRegisterStreamCallback, SecondaryStreams secondaryStreams)
			: base(report, spbProcessing, reportServerParams, deviceInfo, rawDeviceInfo, browserCaps, createAndRegisterStreamCallback, secondaryStreams)
		{
			m_isMHTML = true;
		}

		public override void WriteStream(string theString)
		{
			int length = theString.Length;
			if (length == 0)
			{
				return;
			}
			byte[] array = null;
			if (m_outputLineLength + length < MAXLINELENGTH)
			{
				array = m_encoding.GetBytes(theString);
				m_mainStream.Write(array, 0, array.Length);
				m_outputLineLength += length;
				return;
			}
			WriteStreamLineBreak();
			int num2;
			for (int i = 0; i < length; i += num2)
			{
				int num = length - i;
				if (num <= MAXLINELENGTH)
				{
					array = m_encoding.GetBytes(theString.Substring(i, num));
					m_mainStream.Write(array, 0, array.Length);
					m_outputLineLength = num;
					break;
				}
				num2 = MAXLINELENGTH;
				while (num2 > 0 && !char.IsWhiteSpace(theString[i + num2]))
				{
					num2--;
				}
				if (num2 == 0)
				{
					num2 = MAXLINELENGTH;
				}
				array = m_encoding.GetBytes(theString.Substring(i, num2));
				m_mainStream.Write(array, 0, array.Length);
				WriteStreamLineBreak();
			}
		}

		protected override void WriteAttribute(byte[] attributeName, byte[] value)
		{
			int num = attributeName.Length + value.Length + HTML4Renderer.m_quote.Length;
			if (num + m_outputLineLength >= MAXLINELENGTH)
			{
				WriteStreamLineBreak();
			}
			base.WriteAttribute(attributeName, value);
		}

		protected override void WriteClassStyle(byte[] styleBytes, bool close)
		{
			int num = m_stylePrefixIdBytes.Length + styleBytes.Length + HTML4Renderer.m_classStyle.Length;
			if (num + m_outputLineLength > MAXLINELENGTH)
			{
				WriteStreamLineBreak();
			}
			base.WriteClassStyle(styleBytes, close);
		}

		public override void WriteStream(byte[] theBytes)
		{
			int num = theBytes.Length;
			if ((theBytes == HTML4Renderer.m_classStyle && m_outputLineLength > 900) || m_outputLineLength + num >= MAXLINELENGTH)
			{
				WriteStreamLineBreak();
			}
			m_outputLineLength += num;
			m_mainStream.Write(theBytes, 0, theBytes.Length);
		}

		protected override bool NeedSharedToggleParent(RPLTextBoxProps textBoxProps)
		{
			return false;
		}

		protected override bool CanSort(RPLTextBoxPropsDef textBoxDef)
		{
			return false;
		}

		protected override void RenderDynamicImageSrc(RPLDynamicImageProps dynamicImageProps)
		{
			WriteStream("cid:");
			if (dynamicImageProps.StreamName != null)
			{
				WriteStream(dynamicImageProps.StreamName);
			}
		}

		protected override void RenderReportItemId(string repItemId)
		{
			if (m_outputLineLength > 800)
			{
				WriteStreamLineBreak();
			}
			base.RenderReportItemId(repItemId);
		}

		protected override void WriteStreamLineBreak()
		{
			if (m_outputLineLength != 0)
			{
				m_mainStream.Write(HTML4Renderer.m_newLine, 0, HTML4Renderer.m_newLine.Length);
				m_outputLineLength = 0;
			}
		}

		private void RenderMHTMLInternalImageSrc(string imageName)
		{
			if (imageName != null)
			{
				string text = null;
				if (m_mhtmlStreamNames == null)
				{
					m_mhtmlStreamNames = new Hashtable();
				}
				else
				{
					text = (string)m_mhtmlStreamNames[imageName];
				}
				if (text == null)
				{
					text = imageName;
					ROMReport.GetRenderingResource(m_createAndRegisterStreamCallback, imageName);
					m_mhtmlStreamNames[imageName] = text;
				}
				WriteStream("cid:");
				WriteStream(text);
			}
		}

		protected override void RenderBlankImage()
		{
			WriteStream(HTML4Renderer.m_img);
			if (m_browserIE)
			{
				WriteStream(HTML4Renderer.m_imgOnError);
			}
			WriteStream(HTML4Renderer.m_src);
			RenderMHTMLInternalImageSrc("Blank.gif");
			WriteStream(HTML4Renderer.m_quote);
			WriteStream(HTML4Renderer.m_alt);
			WriteAttrEncoded(RenderRes.BlankAltText);
			WriteStream(HTML4Renderer.m_closeTag);
		}

		protected override void RenderImageUrl(bool useSessionId, RPLImageData image)
		{
			RenderMHTMLImageUrl(image, !m_useInlineStyle);
		}

		protected override void RenderPageStartDimensionStyles(bool lastPage)
		{
		}

		private void RenderMHTMLImageUrl(RPLImageData image, bool forCss)
		{
			string text = CreateImageStream(image);
			WriteStream("cid:");
			if (forCss)
			{
				EncodeCSSStyle(text);
			}
			else
			{
				WriteAttrEncoded(text);
			}
		}

		internal override string GetStyleStreamUrl()
		{
			string styleStreamName = HTML4Renderer.GetStyleStreamName(m_rplReport.ReportName, m_pageNum);
			return "cid:" + styleStreamName;
		}

		private void EncodeCSSStyle(string input)
		{
			if (input == null)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder(input);
			for (int i = 0; i < stringBuilder.Length; i++)
			{
				if (stringBuilder[i] == '\\' || stringBuilder[i] == '\'' || stringBuilder[i] == '(' || stringBuilder[i] == ')' || stringBuilder[i] == ',')
				{
					stringBuilder.Insert(i, '\\');
					i++;
				}
			}
			WriteStream(stringBuilder.ToString());
		}
	}
}
