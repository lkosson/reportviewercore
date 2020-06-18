using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using System;
using System.Globalization;
using System.Threading;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal class Formatter
	{
		private Microsoft.ReportingServices.ReportIntermediateFormat.Style m_styleClass;

		private OnDemandProcessingContext m_context;

		private bool m_sharedFormatSettings;

		private bool m_calendarValidated;

		private uint m_languageInstanceId;

		private ObjectType m_objectType;

		private string m_objectName;

		private Calendar m_formattingCalendar;

		internal Formatter(Microsoft.ReportingServices.ReportIntermediateFormat.Style styleClass, OnDemandProcessingContext context, ObjectType objectType, string objectName)
		{
			m_context = context;
			m_styleClass = styleClass;
			m_objectType = objectType;
			m_objectName = objectName;
		}

		internal static string FormatWithClientCulture(object value)
		{
			bool errorOccurred;
			return FormatWithSpecificCulture(value, Localization.ClientPrimaryCulture, out errorOccurred);
		}

		internal static string FormatWithInvariantCulture(object value)
		{
			bool errorOccurred;
			return FormatWithInvariantCulture(value, out errorOccurred);
		}

		internal static string FormatWithInvariantCulture(object value, out bool errorOccurred)
		{
			return FormatWithSpecificCulture(value, CultureInfo.InvariantCulture, out errorOccurred);
		}

		internal static string FormatWithSpecificCulture(object value, CultureInfo culture, out bool errorOccurred)
		{
			errorOccurred = false;
			if (value == null)
			{
				return null;
			}
			string result = null;
			if (value is IFormattable)
			{
				try
				{
					result = ((IFormattable)value).ToString(null, culture);
					return result;
				}
				catch (Exception)
				{
					errorOccurred = true;
					return result;
				}
			}
			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			Thread.CurrentThread.CurrentCulture = culture;
			try
			{
				result = value.ToString();
				return result;
			}
			catch (Exception)
			{
				errorOccurred = true;
				return result;
			}
			finally
			{
				Thread.CurrentThread.CurrentCulture = currentCulture;
			}
		}

		internal static string Format(object value, ref Formatter formatter, Microsoft.ReportingServices.ReportIntermediateFormat.Style reportItemStyle, Microsoft.ReportingServices.ReportIntermediateFormat.Style reportElementStyle, OnDemandProcessingContext context, ObjectType objectType, string objectName)
		{
			if (formatter == null)
			{
				formatter = new Formatter(reportItemStyle, context, objectType, objectName);
			}
			TypeCode typeCode = Type.GetTypeCode(value.GetType());
			bool sharedFormatSettings = false;
			string styleStringValue = "";
			reportElementStyle?.GetStyleAttribute(objectType, objectName, "Format", context, ref sharedFormatSettings, out styleStringValue);
			return formatter.FormatValue(value, styleStringValue, typeCode);
		}

		internal string FormatValue(object value, TypeCode typeCode)
		{
			return FormatValue(value, null, typeCode);
		}

		internal string FormatValue(object value, string formatString, TypeCode typeCode)
		{
			return FormatValue(value, formatString, typeCode, addDateTimeOffsetSuffix: false);
		}

		internal string FormatValue(object value, string formatString, TypeCode typeCode, bool addDateTimeOffsetSuffix)
		{
			CultureInfo formattingCulture = null;
			string styleStringValue = null;
			bool flag = false;
			bool isThreadCulture = false;
			bool flag2 = false;
			int num = 0;
			bool flag3 = false;
			string text = null;
			Calendar calendar = null;
			bool flag4 = false;
			try
			{
				if (m_styleClass != null)
				{
					if (formatString == null)
					{
						m_styleClass.GetStyleAttribute(m_objectType, m_objectName, "Format", m_context, ref m_sharedFormatSettings, out formatString);
					}
					num = m_styleClass.GetStyleAttribute(m_objectType, m_objectName, "Language", m_context, ref m_sharedFormatSettings, out styleStringValue);
					if (!GetCulture(styleStringValue, ref formattingCulture, ref isThreadCulture, ref num))
					{
						text = RPRes.rsExpressionErrorValue;
						flag4 = true;
					}
					if (!flag4 && typeCode == TypeCode.DateTime && !m_calendarValidated)
					{
						CreateAndValidateCalendar(num, formattingCulture);
					}
				}
				if (!flag4 && formattingCulture != null && m_formattingCalendar != null)
				{
					if (isThreadCulture)
					{
						if (formattingCulture.DateTimeFormat.IsReadOnly)
						{
							formattingCulture = (CultureInfo)formattingCulture.Clone();
							flag2 = true;
						}
						else
						{
							calendar = formattingCulture.DateTimeFormat.Calendar;
						}
					}
					formattingCulture.DateTimeFormat.Calendar = m_formattingCalendar;
				}
				if (!flag4 && formatString != null && value is IFormattable)
				{
					try
					{
						if (formattingCulture == null)
						{
							formattingCulture = Thread.CurrentThread.CurrentCulture;
							isThreadCulture = true;
						}
						if (ReportProcessing.CompareWithInvariantCulture(formatString, "x", ignoreCase: true) == 0)
						{
							flag3 = true;
						}
						text = ((IFormattable)value).ToString(formatString, formattingCulture);
						if (addDateTimeOffsetSuffix)
						{
							text += " +0".ToString(formattingCulture);
						}
					}
					catch (Exception ex)
					{
						text = RPRes.rsExpressionErrorValue;
						m_context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFormatString, Severity.Warning, m_objectType, m_objectName, "Format", ex.Message);
					}
					flag4 = true;
				}
				if (!flag4)
				{
					CultureInfo cultureInfo = null;
					if ((!isThreadCulture && formattingCulture != null) || flag2)
					{
						cultureInfo = Thread.CurrentThread.CurrentCulture;
						Thread.CurrentThread.CurrentCulture = formattingCulture;
						try
						{
							text = value.ToString();
						}
						finally
						{
							if (cultureInfo != null)
							{
								Thread.CurrentThread.CurrentCulture = cultureInfo;
							}
						}
					}
					else
					{
						text = value.ToString();
					}
				}
			}
			finally
			{
				if (isThreadCulture && calendar != null)
				{
					Global.Tracer.Assert(!Thread.CurrentThread.CurrentCulture.DateTimeFormat.IsReadOnly, "(!System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.IsReadOnly)");
					Thread.CurrentThread.CurrentCulture.DateTimeFormat.Calendar = calendar;
				}
			}
			if (!flag3 && m_styleClass != null)
			{
				if ((uint)(typeCode - 5) <= 10u)
				{
					flag = true;
				}
				if (flag)
				{
					int styleIntValue = 1;
					m_styleClass.GetStyleAttribute(m_objectType, m_objectName, "NumeralVariant", m_context, ref m_sharedFormatSettings, out styleIntValue);
					if (styleIntValue > 2)
					{
						CultureInfo cultureInfo2 = formattingCulture;
						if (cultureInfo2 == null)
						{
							cultureInfo2 = Thread.CurrentThread.CurrentCulture;
						}
						string numberDecimalSeparator = cultureInfo2.NumberFormat.NumberDecimalSeparator;
						m_styleClass.GetStyleAttribute(m_objectType, m_objectName, "NumeralLanguage", m_context, ref m_sharedFormatSettings, out styleStringValue);
						if (styleStringValue != null)
						{
							formattingCulture = new CultureInfo(styleStringValue, useUserOverride: false);
						}
						else if (formattingCulture == null)
						{
							formattingCulture = cultureInfo2;
						}
						bool numberTranslated = true;
						text = FormatDigitReplacement.FormatNumeralVariant(text, styleIntValue, formattingCulture, numberDecimalSeparator, out numberTranslated);
						if (!numberTranslated)
						{
							m_context.ErrorContext.Register(ProcessingErrorCode.rsInvalidNumeralVariantForLanguage, Severity.Warning, m_objectType, m_objectName, "NumeralVariant", styleIntValue.ToString(CultureInfo.InvariantCulture), formattingCulture.Name);
						}
					}
				}
			}
			return text;
		}

		internal CultureInfo GetCulture(string language)
		{
			bool isThreadCulture = false;
			int languageState = 0;
			CultureInfo formattingCulture = null;
			if (GetCulture(language, ref formattingCulture, ref isThreadCulture, ref languageState))
			{
				return formattingCulture;
			}
			return Thread.CurrentThread.CurrentCulture;
		}

		private bool GetCulture(string language, ref CultureInfo formattingCulture, ref bool isThreadCulture, ref int languageState)
		{
			if (language != null)
			{
				try
				{
					formattingCulture = new CultureInfo(language, useUserOverride: false);
					if (formattingCulture.IsNeutralCulture)
					{
						formattingCulture = CultureInfo.CreateSpecificCulture(language);
						formattingCulture = new CultureInfo(formattingCulture.Name, useUserOverride: false);
					}
				}
				catch (Exception)
				{
					m_context.ErrorContext.Register(ProcessingErrorCode.rsInvalidLanguage, Severity.Warning, m_objectType, m_objectName, "Language", language);
					return false;
				}
			}
			else
			{
				languageState = 0;
				isThreadCulture = true;
				formattingCulture = Thread.CurrentThread.CurrentCulture;
				if (m_context.LanguageInstanceId != m_languageInstanceId)
				{
					m_calendarValidated = false;
					m_formattingCalendar = null;
					m_languageInstanceId = m_context.LanguageInstanceId;
				}
			}
			return true;
		}

		private void CreateAndValidateCalendar(int languageState, CultureInfo formattingCulture)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.AttributeInfo styleAttribute = null;
			Calendars calendars = Calendars.Default;
			bool flag = false;
			if (m_styleClass.GetAttributeInfo("Calendar", out styleAttribute))
			{
				if (styleAttribute.IsExpression)
				{
					flag = true;
					calendars = (Calendars)m_styleClass.EvaluateStyle(m_objectType, m_objectName, styleAttribute, Microsoft.ReportingServices.ReportIntermediateFormat.Style.StyleId.Calendar, m_context);
					m_sharedFormatSettings = false;
				}
				else
				{
					calendars = StyleTranslator.TranslateCalendar(styleAttribute.Value, m_context.ReportRuntime);
					switch (languageState)
					{
					case 1:
						flag = true;
						break;
					default:
						if (!m_calendarValidated)
						{
							m_calendarValidated = true;
							m_formattingCalendar = ProcessingValidator.CreateCalendar(calendars);
							return;
						}
						break;
					case 0:
						break;
					}
				}
			}
			if (flag || !m_calendarValidated)
			{
				if (calendars != 0 && ProcessingValidator.ValidateCalendar(formattingCulture, calendars, m_objectType, m_objectName, "Calendar", m_context.ErrorContext))
				{
					m_formattingCalendar = ProcessingValidator.CreateCalendar(calendars);
				}
				if (!flag)
				{
					m_calendarValidated = true;
				}
			}
		}
	}
}
