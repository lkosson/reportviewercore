using System.Collections.Generic;
using System.Text;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class HtmlLexer
	{
		internal sealed class Constants
		{
			internal class AttributeNames
			{
				internal const string Align = "align";

				internal const string Padding = "padding";

				internal const string PaddingTop = "padding-top";

				internal const string PaddingBottom = "padding-bottom";

				internal const string PaddingLeft = "padding-left";

				internal const string PaddingRight = "padding-right";

				internal const string Href = "href";

				internal const string Size = "size";

				internal const string Face = "face";

				internal const string Color = "color";

				internal const string Style = "style";

				internal const string FontFamily = "font-family";

				internal const string FontSize = "font-size";

				internal const string FontWeight = "font-weight";

				internal const string TextAlign = "text-align";

				internal const string TextIndent = "text-indent";
			}

			internal class ElementNames
			{
				internal const string SCRIPT = "SCRIPT";

				internal const string STYLE = "STYLE";

				internal const string P = "P";

				internal const string DIV = "DIV";

				internal const string BR = "BR";

				internal const string UL = "UL";

				internal const string OL = "OL";

				internal const string LI = "LI";

				internal const string SPAN = "SPAN";

				internal const string FONT = "FONT";

				internal const string A = "A";

				internal const string STRONG = "STRONG";

				internal const string STRIKE = "STRIKE";

				internal const string B = "B";

				internal const string I = "I";

				internal const string U = "U";

				internal const string S = "S";

				internal const string EM = "EM";

				internal const string H1 = "H1";

				internal const string H2 = "H2";

				internal const string H3 = "H3";

				internal const string H4 = "H4";

				internal const string H5 = "H5";

				internal const string H6 = "H6";

				internal const string DD = "DD";

				internal const string DT = "DT";

				internal const string BLOCKQUOTE = "BLOCKQUOTE";

				internal const string TITLE = "TITLE";
			}
		}

		private enum AttributeEscapeState
		{
			None,
			SingleQuote,
			DoubleQuote,
			NoQuote,
			RawEquals
		}

		private sealed class HtmlStringReader
		{
			private int m_markedIndex;

			private int m_currentIndex;

			private string m_html;

			internal int Position => m_currentIndex;

			internal HtmlStringReader(string html)
			{
				m_html = html;
			}

			internal bool Read(out char c)
			{
				if (Peek(out c))
				{
					m_currentIndex++;
					return true;
				}
				return false;
			}

			internal bool Peek(out char c)
			{
				if (m_currentIndex < m_html.Length)
				{
					c = m_html[m_currentIndex];
					switch (c)
					{
					case '\r':
						m_currentIndex++;
						return Peek(out c);
					case '\t':
					case '\n':
					case '\v':
					case '\f':
						c = ' ';
						break;
					}
					return true;
				}
				c = '\0';
				return false;
			}

			internal bool Peek(int lookAhead, out char c)
			{
				int currentIndex = m_currentIndex;
				m_currentIndex += lookAhead;
				bool result = Peek(out c);
				m_currentIndex = currentIndex;
				return result;
			}

			internal void Advance()
			{
				m_currentIndex++;
			}

			internal void Advance(int amount)
			{
				m_currentIndex += amount;
			}

			internal void Mark()
			{
				m_markedIndex = m_currentIndex;
			}

			internal void Reset()
			{
				m_currentIndex = m_markedIndex;
			}
		}

		private StringBuilder m_sb = new StringBuilder(16);

		private HtmlElement m_currentElement;

		private bool m_readWhiteSpace;

		private HtmlStringReader m_htmlReader;

		private Stack<HtmlElement> m_elementStack;

		internal HtmlElement CurrentElement => m_currentElement;

		internal HtmlLexer(string html)
		{
			m_htmlReader = new HtmlStringReader(html);
			m_elementStack = new Stack<HtmlElement>();
		}

		private static HtmlElement.HtmlElementType GetElementType(string elementName)
		{
			switch (elementName.ToUpperInvariant())
			{
			case "A":
				return HtmlElement.HtmlElementType.A;
			case "B":
				return HtmlElement.HtmlElementType.B;
			case "BLOCKQUOTE":
				return HtmlElement.HtmlElementType.BLOCKQUOTE;
			case "BR":
				return HtmlElement.HtmlElementType.BR;
			case "DD":
				return HtmlElement.HtmlElementType.DD;
			case "DIV":
				return HtmlElement.HtmlElementType.DIV;
			case "DT":
				return HtmlElement.HtmlElementType.DT;
			case "EM":
				return HtmlElement.HtmlElementType.EM;
			case "FONT":
				return HtmlElement.HtmlElementType.FONT;
			case "H1":
				return HtmlElement.HtmlElementType.H1;
			case "H2":
				return HtmlElement.HtmlElementType.H2;
			case "H3":
				return HtmlElement.HtmlElementType.H3;
			case "H4":
				return HtmlElement.HtmlElementType.H4;
			case "H5":
				return HtmlElement.HtmlElementType.H5;
			case "H6":
				return HtmlElement.HtmlElementType.H6;
			case "I":
				return HtmlElement.HtmlElementType.I;
			case "LI":
				return HtmlElement.HtmlElementType.LI;
			case "OL":
				return HtmlElement.HtmlElementType.OL;
			case "P":
				return HtmlElement.HtmlElementType.P;
			case "S":
				return HtmlElement.HtmlElementType.S;
			case "SPAN":
				return HtmlElement.HtmlElementType.SPAN;
			case "STRIKE":
				return HtmlElement.HtmlElementType.STRIKE;
			case "STRONG":
				return HtmlElement.HtmlElementType.STRONG;
			case "U":
				return HtmlElement.HtmlElementType.U;
			case "UL":
				return HtmlElement.HtmlElementType.UL;
			case "STYLE":
				return HtmlElement.HtmlElementType.STYLE;
			case "SCRIPT":
				return HtmlElement.HtmlElementType.SCRIPT;
			case "TITLE":
				return HtmlElement.HtmlElementType.TITLE;
			default:
				return HtmlElement.HtmlElementType.Unsupported;
			}
		}

		internal bool Read()
		{
			if (m_htmlReader.Peek(out char c))
			{
				if (c == '<')
				{
					if (m_htmlReader.Peek(1, out c))
					{
						switch (c)
						{
						case '/':
							m_htmlReader.Advance();
							ReadEndElement();
							break;
						case '!':
							m_htmlReader.Advance();
							ReadBangElement();
							break;
						default:
							if (char.IsLetter(c))
							{
								m_htmlReader.Advance();
								ReadStartElement();
								break;
							}
							ReadTextElement();
							if (!string.IsNullOrEmpty(m_currentElement.Value))
							{
								break;
							}
							return Read();
						}
					}
					else
					{
						ReadTextElement();
					}
					return true;
				}
				HtmlElement.HtmlElementType htmlElementType = HtmlElement.HtmlElementType.None;
				if (m_elementStack.Count > 0)
				{
					htmlElementType = m_elementStack.Peek().ElementType;
					if (htmlElementType == HtmlElement.HtmlElementType.STYLE || htmlElementType == HtmlElement.HtmlElementType.SCRIPT)
					{
						ReadScriptOrStyleContents(htmlElementType);
						return true;
					}
				}
				ReadTextElement();
				if (string.IsNullOrEmpty(m_currentElement.Value))
				{
					return Read();
				}
				return true;
			}
			return false;
		}

		private void ReadStartElement()
		{
			int position = m_htmlReader.Position;
			HtmlElement.HtmlElementType type = ReadElementType(isEndElement: false);
			m_currentElement = new HtmlElement(HtmlElement.HtmlNodeType.Element, type, GetAttributesAsString(out bool isEmpty), isEmpty, position);
			m_elementStack.Push(m_currentElement);
			AdvanceToEndOfElement();
		}

		private void ReadScriptOrStyleContents(HtmlElement.HtmlElementType aElementType)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			int position = m_htmlReader.Position;
			char c;
			while (m_htmlReader.Peek(out c) && flag)
			{
				if (c == '<')
				{
					m_htmlReader.Mark();
					m_htmlReader.Advance();
					if (!m_htmlReader.Peek(out char c2))
					{
						continue;
					}
					if (c2 == '!' && m_htmlReader.Peek(1, out c2) && c2 == '-' && m_htmlReader.Peek(2, out c2) && c2 == '-')
					{
						flag = false;
						m_htmlReader.Reset();
					}
					else if (c2 == '/')
					{
						HtmlElement.HtmlElementType num = ReadElementType(isEndElement: true);
						m_htmlReader.Reset();
						if (num == aElementType)
						{
							flag = false;
							continue;
						}
						m_htmlReader.Advance();
						stringBuilder.Append(c);
					}
					else
					{
						stringBuilder.Append(c);
					}
				}
				else
				{
					m_htmlReader.Advance();
					stringBuilder.Append(c);
				}
			}
			m_currentElement = new HtmlElement((aElementType == HtmlElement.HtmlElementType.SCRIPT) ? HtmlElement.HtmlNodeType.ScriptText : HtmlElement.HtmlNodeType.StyleText, HtmlElement.HtmlElementType.None, stringBuilder.ToString(), position);
		}

		private void ReadBangElement()
		{
			bool flag = false;
			m_sb.Length = 0;
			m_htmlReader.Advance();
			if (m_htmlReader.Peek(out char c) && c == '-' && m_htmlReader.Peek(1, out char c2) && c2 == '-')
			{
				int position = m_htmlReader.Position;
				m_htmlReader.Advance(2);
				m_htmlReader.Mark();
				while (m_htmlReader.Read(out c))
				{
					if (c == '-' && m_htmlReader.Peek(out c2) && c2 == '-' && m_htmlReader.Peek(1, out c2) && c2 == '>')
					{
						m_htmlReader.Advance(2);
						flag = true;
						break;
					}
					m_sb.Append(c);
				}
				if (!flag)
				{
					m_htmlReader.Reset();
					m_sb = ReadTextContent(inComment: true, out bool _);
				}
				m_currentElement = new HtmlElement(HtmlElement.HtmlNodeType.Comment, HtmlElement.HtmlElementType.None, m_sb.ToString(), position);
			}
			else
			{
				ReadStartElement();
			}
		}

		private void AdvanceToEndOfElement()
		{
			char c;
			while (m_htmlReader.Peek(out c))
			{
				switch (c)
				{
				case '<':
					return;
				case '>':
					m_htmlReader.Advance();
					return;
				}
				m_htmlReader.Advance();
			}
		}

		private HtmlElement.HtmlElementType ReadElementType(bool isEndElement)
		{
			m_sb.Length = 0;
			bool flag = true;
			char c;
			while (flag && m_htmlReader.Peek(out c))
			{
				if (c != ' ')
				{
					if (c != '/')
					{
						if (c != '>')
						{
							m_htmlReader.Advance();
							m_sb.Append(c);
							continue;
						}
					}
					else if (isEndElement)
					{
						m_htmlReader.Advance();
						continue;
					}
				}
				else if (isEndElement && m_sb.Length == 0)
				{
					m_htmlReader.Advance();
					continue;
				}
				if (m_sb.Length == 0)
				{
					return HtmlElement.HtmlElementType.Unsupported;
				}
				return GetElementType(m_sb.ToString());
			}
			return HtmlElement.HtmlElementType.Unsupported;
		}

		private string GetAttributesAsString(out bool isEmpty)
		{
			m_sb.Length = 0;
			isEmpty = false;
			AttributeEscapeState attributeEscapeState = AttributeEscapeState.None;
			char c;
			while (m_htmlReader.Peek(out c))
			{
				switch (attributeEscapeState)
				{
				case AttributeEscapeState.None:
					switch (c)
					{
					case '=':
						ConsumeAndAppend(c);
						attributeEscapeState = AttributeEscapeState.RawEquals;
						break;
					case '<':
					case '>':
						return m_sb.ToString();
					case '/':
					{
						if (m_htmlReader.Peek(1, out char c2) && c2 == '>')
						{
							isEmpty = true;
							return m_sb.ToString();
						}
						ConsumeAndAppend(c);
						break;
					}
					default:
						ConsumeAndAppend(c);
						break;
					}
					break;
				case AttributeEscapeState.RawEquals:
					switch (c)
					{
					case ' ':
						ConsumeAndAppend(c);
						break;
					case '"':
						ConsumeAndAppend(c);
						attributeEscapeState = AttributeEscapeState.DoubleQuote;
						break;
					case '\'':
						attributeEscapeState = AttributeEscapeState.SingleQuote;
						ConsumeAndAppend(c);
						break;
					case '>':
						return m_sb.ToString();
					default:
						attributeEscapeState = AttributeEscapeState.NoQuote;
						ConsumeAndAppend(c);
						break;
					}
					break;
				case AttributeEscapeState.DoubleQuote:
					ConsumeAndAppend(c);
					if (c == '"')
					{
						attributeEscapeState = AttributeEscapeState.None;
					}
					break;
				case AttributeEscapeState.SingleQuote:
					ConsumeAndAppend(c);
					if (c == '\'')
					{
						attributeEscapeState = AttributeEscapeState.None;
					}
					break;
				case AttributeEscapeState.NoQuote:
					switch (c)
					{
					case '>':
						return m_sb.ToString();
					case ' ':
						ConsumeAndAppend(c);
						attributeEscapeState = AttributeEscapeState.None;
						break;
					default:
						ConsumeAndAppend(c);
						break;
					}
					break;
				}
			}
			return m_sb.ToString();
		}

		private void ConsumeAndAppend(char c)
		{
			m_htmlReader.Advance();
			m_sb.Append(c);
		}

		private StringBuilder ReadTextContent(bool inComment, out bool hasEntity)
		{
			hasEntity = false;
			m_sb.Length = 0;
			bool flag = true;
			char c;
			while (flag && m_htmlReader.Peek(out c))
			{
				if ((uint)c <= 38u)
				{
					if (c != ' ')
					{
						if (c == '&')
						{
							hasEntity = true;
						}
						goto IL_0085;
					}
					if (!m_readWhiteSpace)
					{
						m_sb.Append(c);
						m_readWhiteSpace = true;
					}
				}
				else if (c != '<')
				{
					if (c != '>' || !inComment)
					{
						goto IL_0085;
					}
					flag = false;
				}
				else
				{
					m_htmlReader.Peek(1, out char c2);
					if (c2 == '!' || c2 == '/')
					{
						flag = false;
					}
					else if (char.IsLetter(c2))
					{
						flag = false;
					}
					if (flag)
					{
						goto IL_0085;
					}
				}
				goto IL_00a1;
				IL_0085:
				m_sb.Append(c);
				if (m_readWhiteSpace)
				{
					m_readWhiteSpace = false;
				}
				goto IL_00a1;
				IL_00a1:
				if (flag)
				{
					m_htmlReader.Advance();
				}
			}
			return m_sb;
		}

		private void ReadTextElement()
		{
			int position = m_htmlReader.Position;
			m_sb = ReadTextContent(inComment: false, out bool hasEntity);
			if (hasEntity)
			{
				HtmlEntityResolver.ResolveEntities(m_sb);
			}
			m_currentElement = new HtmlElement(HtmlElement.HtmlNodeType.Text, HtmlElement.HtmlElementType.None, m_sb.ToString(), position);
		}

		private void ReadEndElement()
		{
			int position = m_htmlReader.Position;
			HtmlElement.HtmlElementType elemntType = ReadElementType(isEndElement: true);
			m_currentElement = new HtmlElement(HtmlElement.HtmlNodeType.EndElement, elemntType, position);
			AdvanceToEndOfElement();
			if (m_elementStack.Count > 0)
			{
				_ = m_elementStack.Pop().ElementType;
			}
		}
	}
}
