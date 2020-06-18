using Microsoft.Reporting.Map.WebForms;
using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Drawing;
using System.Globalization;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal class RuleMapper
	{
		protected RuleBase m_coreRule;

		protected MapAppearanceRule m_mapRule;

		protected MapControl m_coreMap;

		protected MapVectorLayer m_mapVectorLayer;

		protected MapMapper m_mapMapper;

		private string m_ruleFieldName;

		private bool? m_fieldNameBased;

		private CoreSpatialElementManager m_coreSpatialElementManager;

		private object m_startValue;

		private bool m_startValueEvaluated;

		private object m_endValue;

		private bool m_endValueEvaluated;

		private static string m_distinctBucketFieldName = "(Name)";

		private static int m_defaultBucketCount = 5;

		private bool IsRuleFieldDefined => m_mapRule.DataValue != null;

		private bool IsRuleFieldScalar
		{
			get
			{
				if (m_coreRule.Field != "")
				{
					return m_coreSpatialElementManager.FieldDefinitions[m_coreRule.Field].Type != typeof(string);
				}
				return true;
			}
		}

		internal RuleMapper(MapAppearanceRule mapRule, VectorLayerMapper vectorLayerMapper, CoreSpatialElementManager coreSpatialElementManager)
		{
			m_mapRule = mapRule;
			m_mapVectorLayer = vectorLayerMapper.m_mapVectorLayer;
			m_coreMap = vectorLayerMapper.m_coreMap;
			m_coreSpatialElementManager = coreSpatialElementManager;
			m_mapMapper = vectorLayerMapper.m_mapMapper;
		}

		internal bool HasDataValue(ISpatialElement element)
		{
			if (m_mapRule.DataValue != null)
			{
				if (element[m_coreRule.Field] != null)
				{
					return IsValueInRange(m_coreRule.Field, element);
				}
				return false;
			}
			return true;
		}

		private bool IsValueInRange(string fieldName, ISpatialElement element)
		{
			Type type = ((Microsoft.Reporting.Map.WebForms.Field)m_coreSpatialElementManager.FieldDefinitions.GetByName(fieldName)).Type;
			object startValue = GetStartValue(type);
			object endValue = GetEndValue(type);
			if (type == typeof(int))
			{
				if (startValue != null && (int)startValue > (int)element[fieldName])
				{
					return false;
				}
				if (endValue != null && (int)endValue < (int)element[fieldName])
				{
					return false;
				}
			}
			else if (type == typeof(double))
			{
				if (startValue != null && (double)startValue > (double)element[fieldName])
				{
					return false;
				}
				if (endValue != null && (double)endValue < (double)element[fieldName])
				{
					return false;
				}
			}
			else if (type == typeof(decimal))
			{
				if (startValue != null && (decimal)startValue > (decimal)element[fieldName])
				{
					return false;
				}
				if (endValue != null && (decimal)endValue < (decimal)element[fieldName])
				{
					return false;
				}
			}
			return true;
		}

		private object GetStartValue(Type fieldType)
		{
			if (!m_startValueEvaluated)
			{
				if (GetDistributionType() == MapRuleDistributionType.Custom)
				{
					MapBucketCollection mapBuckets = m_mapRule.MapBuckets;
					if (mapBuckets != null && mapBuckets.Count > 0)
					{
						ReportVariantProperty startValue = mapBuckets[0].StartValue;
						if (startValue != null)
						{
							if (!startValue.IsExpression)
							{
								m_startValue = startValue.Value;
							}
							m_startValue = mapBuckets[0].Instance.StartValue;
						}
					}
				}
				if (m_startValue == null)
				{
					ReportVariantProperty startValue2 = m_mapRule.StartValue;
					if (startValue2 != null)
					{
						if (!startValue2.IsExpression)
						{
							m_startValue = startValue2.Value;
						}
						m_startValue = m_mapRule.Instance.StartValue;
					}
				}
				if (m_startValue != null)
				{
					try
					{
						m_startValue = Convert.ChangeType(m_startValue, fieldType, CultureInfo.InvariantCulture);
					}
					catch (Exception e)
					{
						if (AsynchronousExceptionDetection.IsStoppingException(e))
						{
							throw;
						}
						m_startValue = null;
					}
				}
				m_startValueEvaluated = true;
			}
			return m_startValue;
		}

		private object GetEndValue(Type fieldType)
		{
			if (!m_endValueEvaluated)
			{
				if (GetDistributionType() == MapRuleDistributionType.Custom)
				{
					MapBucketCollection mapBuckets = m_mapRule.MapBuckets;
					if (mapBuckets != null && mapBuckets.Count > 0)
					{
						ReportVariantProperty endValue = mapBuckets[mapBuckets.Count - 1].EndValue;
						if (endValue != null)
						{
							if (!endValue.IsExpression)
							{
								m_endValue = endValue.Value;
							}
							m_endValue = mapBuckets[mapBuckets.Count - 1].Instance.EndValue;
						}
					}
				}
				if (m_endValue == null)
				{
					ReportVariantProperty endValue2 = m_mapRule.EndValue;
					if (endValue2 != null)
					{
						if (!endValue2.IsExpression)
						{
							m_endValue = endValue2.Value;
						}
						m_endValue = m_mapRule.Instance.EndValue;
					}
				}
				if (m_endValue != null)
				{
					try
					{
						m_endValue = Convert.ChangeType(m_endValue, fieldType, CultureInfo.InvariantCulture);
					}
					catch (Exception e)
					{
						if (AsynchronousExceptionDetection.IsStoppingException(e))
						{
							throw;
						}
						m_endValue = null;
					}
				}
				m_endValueEvaluated = true;
			}
			return m_endValue;
		}

		internal void SetRuleFieldValue(ISpatialElement spatialElement)
		{
			if (m_fieldNameBased.HasValue && m_fieldNameBased.Value)
			{
				return;
			}
			object obj = EvaluateRuleDataValue();
			if (obj == null)
			{
				return;
			}
			if (!m_fieldNameBased.HasValue && Type.GetTypeCode(obj.GetType()) == TypeCode.String && ((string)obj).StartsWith("#", StringComparison.Ordinal))
			{
				m_ruleFieldName = SpatialDataMapper.GetUniqueFieldName(m_mapVectorLayer.Name, ((string)obj).Remove(0, 1));
				m_coreRule.Field = m_ruleFieldName;
				m_fieldNameBased = true;
				return;
			}
			if (m_ruleFieldName == null)
			{
				m_ruleFieldName = m_coreSpatialElementManager.AddRuleField(obj);
				m_fieldNameBased = false;
				m_coreRule.Field = m_ruleFieldName;
			}
			m_coreSpatialElementManager.AddFieldValue(spatialElement, m_ruleFieldName, obj);
		}

		protected void SetRuleFieldName()
		{
			if (m_mapRule.DataValue == null)
			{
				m_ruleFieldName = m_distinctBucketFieldName;
				m_coreRule.Field = m_ruleFieldName;
				m_fieldNameBased = true;
			}
			else
			{
				if (m_mapVectorLayer.MapDataRegion != null)
				{
					return;
				}
				object obj = EvaluateRuleDataValue();
				if (obj is string)
				{
					string text = (string)obj;
					if (text.StartsWith("#", StringComparison.Ordinal))
					{
						m_ruleFieldName = SpatialDataMapper.GetUniqueFieldName(m_mapVectorLayer.Name, text.Remove(0, 1));
						m_coreRule.Field = m_ruleFieldName;
						m_fieldNameBased = true;
					}
				}
			}
		}

		protected void SetRuleLegendProperties(RuleBase coreRule)
		{
			SetLegendText(coreRule);
			if (m_mapRule.LegendName != null)
			{
				coreRule.ShowInLegend = m_mapRule.LegendName;
			}
		}

		protected void SetLegendText(RuleBase coreRule)
		{
			ReportStringProperty legendText = m_mapRule.LegendText;
			if (legendText != null)
			{
				if (!legendText.IsExpression)
				{
					coreRule.LegendText = legendText.Value;
				}
				else
				{
					coreRule.LegendText = m_mapRule.Instance.LegendText;
				}
			}
			else
			{
				coreRule.LegendText = "";
			}
		}

		protected void SetRuleDistribution(RuleBase coreRule)
		{
			MapRuleDistributionType distributionType = GetDistributionType();
			if (distributionType != MapRuleDistributionType.Custom)
			{
				coreRule.DataGrouping = GetDataGrouping(distributionType);
				coreRule.FromValue = GetFromValue();
				coreRule.ToValue = GetToValue();
			}
			else
			{
				coreRule.DataGrouping = DataGrouping.EqualInterval;
			}
		}

		protected MapRuleDistributionType GetDistributionType()
		{
			ReportEnumProperty<MapRuleDistributionType> distributionType = m_mapRule.DistributionType;
			if (distributionType != null)
			{
				if (!distributionType.IsExpression)
				{
					return distributionType.Value;
				}
				return m_mapRule.Instance.DistributionType;
			}
			return MapRuleDistributionType.Optimal;
		}

		protected DataGrouping GetDataGrouping(MapRuleDistributionType distributionType)
		{
			switch (distributionType)
			{
			case MapRuleDistributionType.EqualDistribution:
				return DataGrouping.EqualDistribution;
			case MapRuleDistributionType.EqualInterval:
				return DataGrouping.EqualInterval;
			default:
				return DataGrouping.Optimal;
			}
		}

		protected string GetFromValue()
		{
			ReportVariantProperty startValue = m_mapRule.StartValue;
			if (startValue != null)
			{
				if (!startValue.IsExpression)
				{
					return ConvertBucketValueToString(startValue.Value);
				}
				return ConvertBucketValueToString(m_mapRule.Instance.StartValue);
			}
			return "";
		}

		protected string GetToValue()
		{
			ReportVariantProperty endValue = m_mapRule.EndValue;
			if (endValue != null)
			{
				if (!endValue.IsExpression)
				{
					return ConvertBucketValueToString(endValue.Value);
				}
				return ConvertBucketValueToString(m_mapRule.Instance.EndValue);
			}
			return "";
		}

		protected string GetFromValue(MapBucket bucket)
		{
			ReportVariantProperty startValue = bucket.StartValue;
			if (startValue != null)
			{
				if (!startValue.IsExpression)
				{
					return ConvertBucketValueToString(startValue.Value);
				}
				return ConvertBucketValueToString(bucket.Instance.StartValue);
			}
			return "";
		}

		protected string GetToValue(MapBucket bucket)
		{
			ReportVariantProperty endValue = bucket.EndValue;
			if (endValue != null)
			{
				if (!endValue.IsExpression)
				{
					return ConvertBucketValueToString(endValue.Value);
				}
				return ConvertBucketValueToString(bucket.Instance.EndValue);
			}
			return "";
		}

		internal ShapeRule CreatePolygonRule()
		{
			ShapeRule shapeRule = (ShapeRule)(m_coreRule = new ShapeRule());
			shapeRule.BorderColor = Color.Empty;
			shapeRule.Text = "";
			shapeRule.Category = m_mapVectorLayer.Name;
			shapeRule.Field = "";
			m_coreMap.ShapeRules.Add(shapeRule);
			SetRuleFieldName();
			return shapeRule;
		}

		internal virtual SymbolRule CreateSymbolRule()
		{
			SymbolRule symbolRule = (SymbolRule)(m_coreRule = new SymbolRule());
			symbolRule.Category = m_mapVectorLayer.Name;
			symbolRule.Field = "";
			m_coreMap.SymbolRules.Add(symbolRule);
			SetRuleFieldName();
			return symbolRule;
		}

		protected void InitializePredefinedSymbols(PredefinedSymbol predefinedSymbol, PointTemplateMapper symbolTemplateMapper)
		{
			predefinedSymbol.BorderColor = symbolTemplateMapper.GetBorderColor(hasScope: false);
			predefinedSymbol.BorderStyle = symbolTemplateMapper.GetBorderStyle(hasScope: false);
			predefinedSymbol.BorderWidth = symbolTemplateMapper.GetBorderWidth(hasScope: false);
			predefinedSymbol.Font = symbolTemplateMapper.GetFont(hasScope: false);
			predefinedSymbol.GradientType = symbolTemplateMapper.GetGradientType(hasScope: false);
			predefinedSymbol.HatchStyle = symbolTemplateMapper.GetHatchStyle(hasScope: false);
			predefinedSymbol.SecondaryColor = symbolTemplateMapper.GetBackGradientEndColor(hasScope: false);
			predefinedSymbol.ShadowOffset = symbolTemplateMapper.GetShadowOffset(hasScope: false);
			predefinedSymbol.TextColor = symbolTemplateMapper.GetTextColor(hasScope: false);
			predefinedSymbol.LegendText = "";
			predefinedSymbol.Text = "";
		}

		protected int GetBucketCount()
		{
			if (!IsRuleFieldDefined)
			{
				return m_coreSpatialElementManager.GetSpatialElementCount();
			}
			MapRuleDistributionType distributionType = GetDistributionType();
			ReportIntProperty bucketCount = m_mapRule.BucketCount;
			int num = m_defaultBucketCount;
			if (bucketCount != null)
			{
				num = (bucketCount.IsExpression ? m_mapRule.Instance.BucketCount : bucketCount.Value);
			}
			if (!IsRuleFieldScalar)
			{
				return m_coreSpatialElementManager.GetDistinctValuesCount(m_coreRule.Field);
			}
			switch (distributionType)
			{
			case MapRuleDistributionType.Optimal:
			case MapRuleDistributionType.EqualDistribution:
				return Math.Min(num, m_coreSpatialElementManager.GetDistinctValuesCount(m_coreRule.Field));
			case MapRuleDistributionType.Custom:
				return (m_mapRule.MapBuckets ?? throw new RenderingObjectModelException(RPRes.rsMapLayerMissingProperty(RPRes.rsObjectTypeMap, m_mapRule.MapDef.Name, m_mapVectorLayer.Name, "MapBuckets"))).Count;
			default:
				return num;
			}
		}

		private string ConvertBucketValueToString(object value)
		{
			if (value == null)
			{
				return "";
			}
			if (value is IFormattable)
			{
				return ((IFormattable)value).ToString("", CultureInfo.CurrentCulture);
			}
			return value.ToString();
		}

		private object EvaluateRuleDataValue()
		{
			ReportVariantProperty dataValue = m_mapRule.DataValue;
			object result = null;
			if (dataValue != null)
			{
				result = (dataValue.IsExpression ? m_mapRule.Instance.DataValue : dataValue.Value);
			}
			return result;
		}
	}
}
