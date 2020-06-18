using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.ReportingServices.ReportPublishing
{
	internal sealed class StyleInformation
	{
		internal sealed class StyleInformationAttribute
		{
			public string Name;

			public Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo Value;

			public ValueType ValueType;
		}

		private List<StyleInformationAttribute> m_attributes = new List<StyleInformationAttribute>();

		private static Hashtable StyleNameIndexes;

		private static bool[,] AllowStyleAttributeByType;

		internal List<StyleInformationAttribute> Attributes => m_attributes;

		static StyleInformation()
		{
			AllowStyleAttributeByType = new bool[52, 18]
			{
				{
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					false,
					false,
					true
				},
				{
					true,
					false,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					false,
					false,
					true
				},
				{
					true,
					false,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					false,
					false,
					true
				},
				{
					true,
					false,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					false,
					false,
					true
				},
				{
					true,
					false,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					false,
					false,
					true
				},
				{
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					false,
					false,
					true
				},
				{
					true,
					false,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					false,
					false,
					true
				},
				{
					true,
					false,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					false,
					false,
					true
				},
				{
					true,
					false,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					false,
					false,
					true
				},
				{
					true,
					false,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					false,
					false,
					true
				},
				{
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					false,
					false,
					true
				},
				{
					true,
					false,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					false,
					false,
					true
				},
				{
					true,
					false,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					false,
					false,
					true
				},
				{
					true,
					false,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					false,
					false,
					true
				},
				{
					true,
					false,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					false,
					false,
					true
				},
				{
					true,
					false,
					true,
					false,
					false,
					false,
					true,
					true,
					true,
					false,
					true,
					true,
					true,
					true,
					true,
					false,
					false,
					true
				},
				{
					true,
					false,
					true,
					false,
					false,
					false,
					true,
					true,
					true,
					false,
					true,
					true,
					true,
					false,
					true,
					false,
					false,
					false
				},
				{
					true,
					false,
					true,
					false,
					false,
					false,
					true,
					true,
					true,
					false,
					true,
					true,
					true,
					false,
					true,
					false,
					false,
					false
				},
				{
					true,
					false,
					true,
					false,
					false,
					false,
					true,
					true,
					true,
					false,
					true,
					true,
					true,
					false,
					true,
					false,
					false,
					false
				},
				{
					true,
					false,
					true,
					false,
					false,
					false,
					true,
					true,
					true,
					false,
					true,
					true,
					true,
					false,
					true,
					false,
					false,
					false
				},
				{
					true,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					true,
					false,
					false,
					true,
					true
				},
				{
					true,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					true,
					false,
					false,
					true,
					true
				},
				{
					true,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					true,
					false,
					false,
					true,
					true
				},
				{
					true,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					true,
					false,
					false,
					true,
					true
				},
				{
					true,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					true,
					false,
					false,
					true,
					true
				},
				{
					true,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					true,
					false,
					false,
					true,
					true
				},
				{
					true,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					false
				},
				{
					true,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					false,
					false
				},
				{
					true,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					true,
					false,
					false,
					true,
					true
				},
				{
					true,
					false,
					false,
					false,
					true,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					false,
					false
				},
				{
					true,
					false,
					false,
					false,
					true,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					false,
					false
				},
				{
					true,
					false,
					false,
					false,
					true,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					false,
					false
				},
				{
					true,
					false,
					false,
					false,
					true,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					false,
					false
				},
				{
					true,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					false
				},
				{
					true,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					false,
					true,
					false,
					false,
					false
				},
				{
					true,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					true,
					false,
					false,
					true,
					true
				},
				{
					true,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false
				},
				{
					true,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					true,
					false,
					false,
					true,
					true
				},
				{
					true,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					true,
					false,
					false,
					true,
					true
				},
				{
					true,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					true,
					false,
					false,
					true,
					true
				},
				{
					true,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					false,
					true,
					false,
					false,
					false
				},
				{
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					true,
					false,
					false,
					false,
					true
				},
				{
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					true,
					false,
					false,
					false,
					true
				},
				{
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					false,
					false,
					false,
					false,
					false
				},
				{
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					true,
					false,
					false,
					false,
					true
				},
				{
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					false,
					false,
					false,
					false,
					false
				},
				{
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					true,
					false,
					false,
					false,
					true
				},
				{
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					false,
					false,
					false,
					false,
					false
				},
				{
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					false,
					false,
					false,
					false,
					false
				},
				{
					true,
					false,
					true,
					false,
					false,
					false,
					true,
					true,
					true,
					false,
					true,
					true,
					true,
					false,
					true,
					false,
					false,
					false
				},
				{
					true,
					false,
					true,
					false,
					false,
					false,
					true,
					true,
					true,
					false,
					true,
					true,
					true,
					false,
					true,
					false,
					false,
					false
				},
				{
					true,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					false,
					true,
					false,
					false,
					true,
					true
				}
			};
			StyleNameIndexes = new Hashtable();
			StyleNameIndexes.Add("BorderColor", 0);
			StyleNameIndexes.Add("BorderColorLeft", 1);
			StyleNameIndexes.Add("BorderColorRight", 2);
			StyleNameIndexes.Add("BorderColorTop", 3);
			StyleNameIndexes.Add("BorderColorBottom", 4);
			StyleNameIndexes.Add("BorderStyle", 5);
			StyleNameIndexes.Add("BorderStyleLeft", 6);
			StyleNameIndexes.Add("BorderStyleRight", 7);
			StyleNameIndexes.Add("BorderStyleTop", 8);
			StyleNameIndexes.Add("BorderStyleBottom", 9);
			StyleNameIndexes.Add("BorderWidth", 10);
			StyleNameIndexes.Add("BorderWidthLeft", 11);
			StyleNameIndexes.Add("BorderWidthRight", 12);
			StyleNameIndexes.Add("BorderWidthTop", 13);
			StyleNameIndexes.Add("BorderWidthBottom", 14);
			StyleNameIndexes.Add("BackgroundColor", 15);
			StyleNameIndexes.Add("BackgroundImageSource", 16);
			StyleNameIndexes.Add("BackgroundImageValue", 17);
			StyleNameIndexes.Add("BackgroundImageMIMEType", 18);
			StyleNameIndexes.Add("BackgroundRepeat", 19);
			StyleNameIndexes.Add("FontStyle", 20);
			StyleNameIndexes.Add("FontFamily", 21);
			StyleNameIndexes.Add("FontSize", 22);
			StyleNameIndexes.Add("FontWeight", 23);
			StyleNameIndexes.Add("Format", 24);
			StyleNameIndexes.Add("TextDecoration", 25);
			StyleNameIndexes.Add("TextAlign", 26);
			StyleNameIndexes.Add("VerticalAlign", 27);
			StyleNameIndexes.Add("Color", 28);
			StyleNameIndexes.Add("PaddingLeft", 29);
			StyleNameIndexes.Add("PaddingRight", 30);
			StyleNameIndexes.Add("PaddingTop", 31);
			StyleNameIndexes.Add("PaddingBottom", 32);
			StyleNameIndexes.Add("LineHeight", 33);
			StyleNameIndexes.Add("Direction", 34);
			StyleNameIndexes.Add("Language", 35);
			StyleNameIndexes.Add("UnicodeBiDi", 36);
			StyleNameIndexes.Add("Calendar", 37);
			StyleNameIndexes.Add("NumeralLanguage", 38);
			StyleNameIndexes.Add("NumeralVariant", 39);
			StyleNameIndexes.Add("WritingMode", 40);
			StyleNameIndexes.Add("BackgroundGradientType", 41);
			StyleNameIndexes.Add("BackgroundGradientEndColor", 42);
			StyleNameIndexes.Add("TextEffect", 43);
			StyleNameIndexes.Add("BackgroundHatchType", 44);
			StyleNameIndexes.Add("ShadowColor", 45);
			StyleNameIndexes.Add("ShadowOffset", 46);
			StyleNameIndexes.Add("TransparentColor", 47);
			StyleNameIndexes.Add("Position", 48);
			StyleNameIndexes.Add("EmbeddingMode", 49);
			StyleNameIndexes.Add("Transparency", 50);
			StyleNameIndexes.Add("CurrencyLanguage", 51);
		}

		internal void AddAttribute(string name, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			AddAttribute(name, expression, ValueType.Constant);
		}

		internal void AddAttribute(string name, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ValueType valueType)
		{
			Global.Tracer.Assert(name != null);
			Global.Tracer.Assert(expression != null);
			m_attributes.Add(new StyleInformationAttribute
			{
				Name = name,
				Value = expression,
				ValueType = valueType
			});
		}

		internal void RemoveAttribute(string name)
		{
			Global.Tracer.Assert(name != null);
			m_attributes.RemoveAll((StyleInformationAttribute a) => a.Name == name);
		}

		internal StyleInformationAttribute GetAttributeByName(string name)
		{
			Global.Tracer.Assert(name != null);
			return m_attributes.SingleOrDefault((StyleInformationAttribute a) => a.Name == name);
		}

		internal void Filter(StyleOwnerType ownerType, bool hasNoRows)
		{
			int ownerType2 = MapStyleOwnerTypeToIndex(ownerType, hasNoRows);
			for (int num = m_attributes.Count - 1; num >= 0; num--)
			{
				if (!Allow(MapStyleNameToIndex(m_attributes[num].Name), ownerType2))
				{
					m_attributes.RemoveAt(num);
				}
			}
		}

		internal void FilterChartLegendTitleStyle()
		{
			int ownerType = MapStyleOwnerTypeToIndex(StyleOwnerType.Chart, hasNoRows: false);
			for (int num = m_attributes.Count - 1; num >= 0; num--)
			{
				string name = m_attributes[num].Name;
				if (!Allow(MapStyleNameToIndex(name), ownerType) && name != "TextAlign")
				{
					m_attributes.RemoveAt(num);
				}
			}
		}

		internal void FilterChartStripLineStyle()
		{
			int ownerType = MapStyleOwnerTypeToIndex(StyleOwnerType.Chart, hasNoRows: false);
			for (int num = m_attributes.Count - 1; num >= 0; num--)
			{
				string name = m_attributes[num].Name;
				if (!Allow(MapStyleNameToIndex(name), ownerType) && name != "VerticalAlign" && name != "TextAlign")
				{
					m_attributes.RemoveAt(num);
				}
			}
		}

		internal void FilterChartSeriesStyle()
		{
			MapStyleOwnerTypeToIndex(StyleOwnerType.Chart, hasNoRows: false);
			for (int num = m_attributes.Count - 1; num >= 0; num--)
			{
				string name = m_attributes[num].Name;
				if (name != "ShadowColor" && name != "ShadowOffset")
				{
					m_attributes.RemoveAt(num);
				}
			}
		}

		internal void FilterGaugeLabelStyle()
		{
			int ownerType = MapStyleOwnerTypeToIndex(StyleOwnerType.GaugePanel, hasNoRows: false);
			for (int num = m_attributes.Count - 1; num >= 0; num--)
			{
				string name = m_attributes[num].Name;
				if (!Allow(MapStyleNameToIndex(name), ownerType) && name != "VerticalAlign" && name != "TextAlign")
				{
					m_attributes.RemoveAt(num);
				}
			}
		}

		internal void FilterMapTitleStyle()
		{
			int ownerType = MapStyleOwnerTypeToIndex(StyleOwnerType.Map, hasNoRows: false);
			for (int num = m_attributes.Count - 1; num >= 0; num--)
			{
				string name = m_attributes[num].Name;
				if (!Allow(MapStyleNameToIndex(name), ownerType) && name != "VerticalAlign" && name != "TextAlign")
				{
					m_attributes.RemoveAt(num);
				}
			}
		}

		internal void FilterMapLegendTitleStyle()
		{
			int ownerType = MapStyleOwnerTypeToIndex(StyleOwnerType.Map, hasNoRows: false);
			for (int num = m_attributes.Count - 1; num >= 0; num--)
			{
				string name = m_attributes[num].Name;
				if (!Allow(MapStyleNameToIndex(name), ownerType) && name != "TextAlign")
				{
					m_attributes.RemoveAt(num);
				}
			}
		}

		private int MapStyleOwnerTypeToIndex(StyleOwnerType ownerType, bool hasNoRows)
		{
			if (hasNoRows)
			{
				return 0;
			}
			switch (ownerType)
			{
			case StyleOwnerType.PageSection:
				return 2;
			case StyleOwnerType.SubReport:
			case StyleOwnerType.Subtotal:
				return 0;
			default:
				return (int)ownerType;
			}
		}

		private int MapStyleNameToIndex(string name)
		{
			return (int)StyleNameIndexes[name];
		}

		private bool Allow(int styleName, int ownerType)
		{
			return AllowStyleAttributeByType[styleName, ownerType];
		}
	}
}
