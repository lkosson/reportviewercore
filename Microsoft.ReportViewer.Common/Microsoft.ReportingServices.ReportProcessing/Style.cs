using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class Style
	{
		private StyleAttributeHashtable m_styleAttributes;

		private ExpressionInfoList m_expressionList;

		[NonSerialized]
		private StyleExprHost m_exprHost;

		[NonSerialized]
		private int m_customSharedStyleCount = -1;

		internal StyleAttributeHashtable StyleAttributes
		{
			get
			{
				return m_styleAttributes;
			}
			set
			{
				m_styleAttributes = value;
			}
		}

		internal ExpressionInfoList ExpressionList
		{
			get
			{
				return m_expressionList;
			}
			set
			{
				m_expressionList = value;
			}
		}

		internal StyleExprHost ExprHost => m_exprHost;

		internal int CustomSharedStyleCount
		{
			get
			{
				return m_customSharedStyleCount;
			}
			set
			{
				m_customSharedStyleCount = value;
			}
		}

		internal Style(ConstructionPhase phase)
		{
			if (phase == ConstructionPhase.Publishing)
			{
				m_styleAttributes = new StyleAttributeHashtable();
			}
		}

		internal void AddAttribute(string name, ExpressionInfo expressionInfo)
		{
			AttributeInfo attributeInfo = new AttributeInfo();
			attributeInfo.IsExpression = (ExpressionInfo.Types.Constant != expressionInfo.Type);
			if (attributeInfo.IsExpression)
			{
				if (m_expressionList == null)
				{
					m_expressionList = new ExpressionInfoList();
				}
				attributeInfo.IntValue = m_expressionList.Add(expressionInfo);
			}
			else
			{
				attributeInfo.Value = expressionInfo.Value;
				attributeInfo.BoolValue = expressionInfo.BoolValue;
				attributeInfo.IntValue = expressionInfo.IntValue;
			}
			Global.Tracer.Assert(m_styleAttributes != null);
			m_styleAttributes.Add(name, attributeInfo);
		}

		internal void Initialize(InitializationContext context)
		{
			Global.Tracer.Assert(m_styleAttributes != null);
			IDictionaryEnumerator enumerator = m_styleAttributes.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string text = (string)enumerator.Key;
				AttributeInfo attributeInfo = (AttributeInfo)enumerator.Value;
				Global.Tracer.Assert(text != null);
				Global.Tracer.Assert(attributeInfo != null);
				if (attributeInfo.IsExpression)
				{
					string name = text;
					switch (text)
					{
					case "BorderColorLeft":
					case "BorderColorRight":
					case "BorderColorTop":
					case "BorderColorBottom":
						text = "BorderColor";
						break;
					case "BorderStyleLeft":
					case "BorderStyleRight":
					case "BorderStyleTop":
					case "BorderStyleBottom":
						text = "BorderStyle";
						break;
					case "BorderWidthLeft":
					case "BorderWidthRight":
					case "BorderWidthTop":
					case "BorderWidthBottom":
						text = "BorderWidth";
						break;
					}
					Global.Tracer.Assert(m_expressionList != null);
					ExpressionInfo expressionInfo = m_expressionList[attributeInfo.IntValue];
					expressionInfo.Initialize(text, context);
					context.ExprHostBuilder.StyleAttribute(name, expressionInfo);
				}
			}
			AttributeInfo attributeInfo2 = m_styleAttributes["BackgroundImageSource"];
			if (attributeInfo2 != null)
			{
				Global.Tracer.Assert(!attributeInfo2.IsExpression);
				Image.SourceType intValue = (Image.SourceType)attributeInfo2.IntValue;
				if (Image.SourceType.Embedded == intValue)
				{
					AttributeInfo attributeInfo3 = m_styleAttributes["BackgroundImageValue"];
					Global.Tracer.Assert(attributeInfo3 != null);
					PublishingValidator.ValidateEmbeddedImageName(attributeInfo3, context.EmbeddedImages, context.ObjectType, context.ObjectName, "BackgroundImageValue", context.ErrorContext);
				}
				else if (intValue == Image.SourceType.External)
				{
					AttributeInfo attributeInfo4 = m_styleAttributes["BackgroundImageValue"];
					Global.Tracer.Assert(attributeInfo4 != null);
					if (!attributeInfo4.IsExpression)
					{
						context.ImageStreamNames[attributeInfo4.Value] = new ImageInfo(context.ObjectName, null);
					}
				}
			}
			context.CheckInternationalSettings(m_styleAttributes);
		}

		internal void SetStyleExprHost(StyleExprHost exprHost)
		{
			Global.Tracer.Assert(exprHost != null);
			m_exprHost = exprHost;
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.StyleAttributes, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.StyleAttributeHashtable));
			memberInfoList.Add(new MemberInfo(MemberName.ExpressionList, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfoList));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
