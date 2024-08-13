using System;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Xml;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandReportRendering;

namespace Microsoft.ReportingServices.Rendering.DataRenderer;

internal abstract class XmlVisitorBase
{
	protected sealed class Scope
	{
		internal Hashtable Elements = new Hashtable();

		internal Hashtable Attributes = new Hashtable();
	}

	protected const string XmlXmlns = "xmlns";

	protected const int UNICODE_HORIZ_TAB = 9;

	protected const int UNICODE_NEWLINE = 10;

	protected const int UNICODE_CAR_RETURN = 13;

	protected const int UNICODE_SPACE = 32;

	private const string XmlMapRuleValueTag = "Value{0}";

	private const string XmlStateIndicatorTag = "StateIndicator{0}";

	private const string XmlStateIndicatorStateNameTag = "StateName";

	private const string XmlIndicatorStateCollectionTag = "IndicatorState_Collection";

	private const string XmlIndicatorStateTag = "IndicatorState{0}";

	protected Scope m_currentScope = new Scope();

	private Stack m_scopeStack = new Stack();

	protected XmlWriter m_xw;

	protected void ValidateElement(ref string name)
	{
		if (m_currentScope.Elements.Contains(name))
		{
			int num = (int)m_currentScope.Elements[name];
			string text;
			do
			{
				num++;
				m_currentScope.Elements[name] = num;
				text = name + "_" + num.ToString(CultureInfo.InvariantCulture);
			}
			while (m_currentScope.Elements.Contains(text));
			name = text;
			m_currentScope.Elements.Add(name, 0);
		}
		else
		{
			m_currentScope.Elements.Add(name, 0);
		}
	}

	protected void ValidateAttribute(string name)
	{
		if (m_currentScope.Attributes.Contains(name))
		{
			throw new ReportRenderingException(StringResources.rrAttrNameCollision(name));
		}
		m_currentScope.Attributes.Add(name, null);
	}

	protected void ValidateAttribute(string name, object value)
	{
		if (m_currentScope.Attributes.Contains(name))
		{
			throw new ReportRenderingException(StringResources.rrAttrNameCollision(name));
		}
		m_currentScope.Attributes.Add(name, value);
	}

	protected void PushScope()
	{
		m_scopeStack.Push(m_currentScope);
		m_currentScope = new Scope();
	}

	protected void PopScope()
	{
		m_currentScope = (Scope)m_scopeStack.Pop();
	}

	internal static string FilterInvalidXMLCharacters(string value)
	{
		bool flag = false;
		for (int i = 0; i < 32; i++)
		{
			if (i != 9 && i != 13 && i != 10 && value.IndexOf((char)i) != -1)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (char c in value)
			{
				if (c == '\t' || c == '\n' || c == '\r' || c >= ' ')
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}
		return value;
	}

	protected void WriteAttributeString(string localName, string value)
	{
		RSTrace.RenderingTracer.Assert(m_xw != null, "The xml writer should not be null");
		try
		{
			m_xw.WriteAttributeString(localName, FilterInvalidXMLCharacters(value));
		}
		catch (ArgumentException innerException)
		{
			throw new ReportRenderingException(innerException);
		}
	}

	protected void WriteAttributeString(string prefix, string localName, string ns, string value)
	{
		RSTrace.RenderingTracer.Assert(m_xw != null, "The xml writer should not be null");
		try
		{
			m_xw.WriteAttributeString(prefix, localName, ns, FilterInvalidXMLCharacters(value));
		}
		catch (ArgumentException innerException)
		{
			throw new ReportRenderingException(innerException);
		}
	}

	protected void WriteString(string text)
	{
		RSTrace.RenderingTracer.Assert(m_xw != null, "The xml writer should not be null");
		try
		{
			m_xw.WriteString(FilterInvalidXMLCharacters(text));
		}
		catch (ArgumentException innerException)
		{
			throw new ReportRenderingException(innerException);
		}
	}

	public string getDefaultMapRuleValueTag(int index)
	{
		return string.Format(CultureInfo.InvariantCulture, "Value{0}", index);
	}

	public string getStateIndicatorTag(int index)
	{
		return string.Format(CultureInfo.InvariantCulture, "StateIndicator{0}", index);
	}

	public string getDefaultStateNameTag()
	{
		return string.Format(CultureInfo.InvariantCulture, "StateName");
	}

	public string getIndicatorStateCollectionTag()
	{
		return "IndicatorState_Collection";
	}

	public string getIndicatorStateTag(int index)
	{
		return string.Format(CultureInfo.InvariantCulture, "IndicatorState{0}", index);
	}
}
