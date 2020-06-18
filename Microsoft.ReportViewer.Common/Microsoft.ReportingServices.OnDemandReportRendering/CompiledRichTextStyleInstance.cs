using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class CompiledRichTextStyleInstance : StyleInstance, ICompiledStyleInstance
	{
		private List<StyleAttributeNames> m_nonSharedStyles;

		public override List<StyleAttributeNames> StyleAttributes
		{
			get
			{
				if (m_nonSharedStyles == null)
				{
					CompleteStyle();
				}
				return m_nonSharedStyles;
			}
		}

		internal CompiledRichTextStyleInstance(IROMStyleDefinitionContainer styleDefinitionContainer, IReportScope reportScope, RenderingContext context)
			: base(styleDefinitionContainer, reportScope, context)
		{
		}

		private void CompleteStyle()
		{
			List<StyleAttributeNames> styleAttributes = base.StyleAttributes;
			Dictionary<StyleAttributeNames, bool> dictionary = null;
			if (styleAttributes != null)
			{
				dictionary = new Dictionary<StyleAttributeNames, bool>(styleAttributes.Count);
				foreach (StyleAttributeNames item in styleAttributes)
				{
					dictionary[item] = true;
				}
			}
			else
			{
				m_nonSharedStyles = new List<StyleAttributeNames>();
			}
			if (m_assignedValues != null)
			{
				foreach (KeyValuePair<StyleAttributeNames, bool> assignedValue in m_assignedValues)
				{
					if (assignedValue.Value)
					{
						StyleAttributeNames key = assignedValue.Key;
						if (dictionary == null)
						{
							m_nonSharedStyles.Add(key);
						}
						else
						{
							dictionary[key] = true;
						}
					}
				}
			}
			if (dictionary == null)
			{
				return;
			}
			m_nonSharedStyles = new List<StyleAttributeNames>(dictionary.Count);
			foreach (StyleAttributeNames key2 in dictionary.Keys)
			{
				m_nonSharedStyles.Add(key2);
			}
		}
	}
}
