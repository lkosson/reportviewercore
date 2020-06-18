using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal class HtmlElement
	{
		internal enum HtmlNodeType
		{
			Element,
			EndElement,
			Text,
			Comment,
			ScriptText,
			StyleText
		}

		internal enum HtmlElementType
		{
			None,
			Unsupported,
			SCRIPT,
			STYLE,
			P,
			DIV,
			BR,
			UL,
			OL,
			LI,
			SPAN,
			FONT,
			A,
			STRONG,
			STRIKE,
			B,
			I,
			U,
			S,
			EM,
			H1,
			H2,
			H3,
			H4,
			H5,
			H6,
			DD,
			DT,
			BLOCKQUOTE,
			TITLE
		}

		private string m_value;

		private bool m_isEmptyElement;

		private HtmlNodeType m_nodeType;

		private HtmlElementType m_elementType;

		private string m_attributesAsString;

		private Dictionary<string, string> m_parsedAttributes;

		private Dictionary<string, string> m_parsedCssStyleValues;

		private int m_characterPosition;

		private static Regex m_AttributeRegEx = new Regex("((?<name>\\w+)(\\s*=\\s*((\"(?<quotedvalue>[^\"]*)\")|('(?<singlequotedvalue>[^']*)')|(?<value>[^ =]+))?)?)*", RegexOptions.Compiled | RegexOptions.CultureInvariant);

		internal bool IsEmptyElement => m_isEmptyElement;

		internal HtmlNodeType NodeType => m_nodeType;

		internal HtmlElementType ElementType => m_elementType;

		internal Dictionary<string, string> Attributes
		{
			get
			{
				ParseAttributes();
				return m_parsedAttributes;
			}
		}

		internal Dictionary<string, string> CssStyle
		{
			get
			{
				if (HasAttributes)
				{
					ParseAttributes();
					if (m_parsedAttributes.TryGetValue("style", out string value) && !string.IsNullOrEmpty(value))
					{
						ParseCssStyle(value);
					}
				}
				return m_parsedCssStyleValues;
			}
		}

		internal bool HasAttributes
		{
			get
			{
				if (m_attributesAsString == null)
				{
					return m_parsedAttributes != null;
				}
				return true;
			}
		}

		internal string Value => m_value;

		internal int CharacterPosition => m_characterPosition;

		internal HtmlElement(HtmlNodeType nodeType, HtmlElementType elemntType, int characterPosition)
			: this(nodeType, elemntType, null, isEmpty: true, characterPosition)
		{
		}

		internal HtmlElement(HtmlNodeType nodeType, HtmlElementType elemntType, string value, int characterPosition)
			: this(nodeType, elemntType, null, isEmpty: false, characterPosition)
		{
			m_value = value;
		}

		internal HtmlElement(HtmlNodeType nodeType, HtmlElementType type, string attributesAsString, bool isEmpty, int characterPosition)
		{
			m_nodeType = nodeType;
			m_elementType = type;
			m_isEmptyElement = isEmpty;
			m_characterPosition = characterPosition;
			if (!string.IsNullOrEmpty(attributesAsString))
			{
				m_attributesAsString = attributesAsString;
			}
		}

		private void ParseCssStyle(string cssStyles)
		{
			string[] array = cssStyles.Split(new char[1]
			{
				';'
			}, StringSplitOptions.RemoveEmptyEntries);
			m_parsedCssStyleValues = new Dictionary<string, string>(array.Length, StringEqualityComparer.Instance);
			foreach (string text in array)
			{
				string text2 = string.Empty;
				string html = string.Empty;
				int num = text.IndexOf(':');
				if (num == -1)
				{
					text2 = text.Trim();
				}
				else if (num > 0)
				{
					text2 = text.Substring(0, num).Trim();
					if (num + 1 < text.Length)
					{
						html = text.Substring(num + 1).Trim();
					}
				}
				if (!string.IsNullOrEmpty(text2))
				{
					m_parsedCssStyleValues[text2.ToLowerInvariant()] = HtmlEntityResolver.ResolveEntities(html).ToLowerInvariant();
				}
			}
		}

		private void ParseAttributes()
		{
			if (m_attributesAsString != null)
			{
				MatchCollection matchCollection = m_AttributeRegEx.Matches(m_attributesAsString.Trim());
				if (matchCollection.Count > 0)
				{
					m_parsedAttributes = new Dictionary<string, string>(matchCollection.Count, StringEqualityComparer.Instance);
					for (int i = 0; i < matchCollection.Count; i++)
					{
						Match match = matchCollection[i];
						string text = null;
						string html = null;
						System.Text.RegularExpressions.Group group = match.Groups["name"];
						if (group.Length <= 0)
						{
							continue;
						}
						text = group.Value;
						group = match.Groups["quotedvalue"];
						if (group.Length > 0)
						{
							html = group.Value;
						}
						else
						{
							group = match.Groups["singlequotedvalue"];
							if (group.Length > 0)
							{
								html = group.Value;
							}
							else
							{
								group = match.Groups["value"];
								if (group.Length > 0)
								{
									html = group.Value;
								}
							}
						}
						m_parsedAttributes[text.ToLowerInvariant()] = HtmlEntityResolver.ResolveEntities(html);
					}
				}
			}
			m_attributesAsString = null;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(m_nodeType.ToString());
			stringBuilder.Append(" Type = ");
			stringBuilder.Append(m_elementType.ToString());
			if (!m_isEmptyElement)
			{
				if (HasAttributes)
				{
					GetDictionaryAsString("Attributes", Attributes, stringBuilder);
					GetDictionaryAsString("CssStyle", CssStyle, stringBuilder);
				}
				if (m_value != null)
				{
					stringBuilder.Append("; Value = \"");
					stringBuilder.Append(m_value);
					stringBuilder.Append("\"");
				}
			}
			return stringBuilder.ToString();
		}

		private void GetDictionaryAsString(string name, Dictionary<string, string> dict, StringBuilder sb)
		{
			if (dict == null)
			{
				return;
			}
			sb.Append("; ");
			sb.Append(name);
			sb.Append(" = { ");
			int num = 0;
			foreach (KeyValuePair<string, string> item in dict)
			{
				if (num > 0)
				{
					sb.Append(", ");
				}
				num++;
				sb.Append(item.Key);
				sb.Append("=\"");
				sb.Append(item.Value);
				sb.Append("\"");
			}
			sb.Append(" }");
		}
	}
}
