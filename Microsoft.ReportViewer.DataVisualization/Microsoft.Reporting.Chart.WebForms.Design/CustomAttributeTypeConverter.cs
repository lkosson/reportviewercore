using Microsoft.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms.Design
{
	internal class CustomAttributeTypeConverter : TypeConverter
	{
		protected class CustomAttributesPropertyDescriptor : SimplePropertyDescriptor
		{
			private string name = string.Empty;

			private CustomAttributeInfo customAttributeInfo;

			internal CustomAttributesPropertyDescriptor(Type componentType, string name, Type propertyType, Attribute[] attributes, CustomAttributeInfo customAttributeInfo)
				: base(componentType, name, propertyType, attributes)
			{
				this.name = name;
				this.customAttributeInfo = customAttributeInfo;
			}

			internal CustomAttributeInfo GetCustomAttributeInfo()
			{
				return customAttributeInfo;
			}

			public override object GetValue(object component)
			{
				CustomAttributes customAttributes = component as CustomAttributes;
				if (name == "UserDefined")
				{
					return customAttributes.GetUserDefinedAttributes();
				}
				object obj = null;
				string text = customAttributes.DataPointAttributes[name];
				if (customAttributeInfo != null)
				{
					if (text == null || text.Length == 0)
					{
						return GetValueFromString(customAttributeInfo.DefaultValue);
					}
					return GetValueFromString(text);
				}
				return text;
			}

			public override void SetValue(object component, object value)
			{
				ValidateValue(name, value);
				string stringFromValue = GetStringFromValue(value);
				CustomAttributes customAttributes = component as CustomAttributes;
				if (name == "UserDefined")
				{
					customAttributes.SetUserDefinedAttributes(stringFromValue);
				}
				else
				{
					bool flag = true;
					if (IsDefaultValue(stringFromValue) && (!(customAttributes.DataPointAttributes is DataPoint) || !((DataPoint)customAttributes.DataPointAttributes).series.IsAttributeSet(name)) && customAttributes.DataPointAttributes.IsAttributeSet(name))
					{
						customAttributes.DataPointAttributes.DeleteAttribute(name);
						flag = false;
					}
					if (flag)
					{
						customAttributes.DataPointAttributes[name] = stringFromValue;
					}
				}
				customAttributes.DataPointAttributes.CustomAttributes = customAttributes.DataPointAttributes.CustomAttributes;
				if (component is IChangeTracking)
				{
					((IChangeTracking)component).AcceptChanges();
				}
			}

			public bool IsDefaultValue(string val)
			{
				string stringFromValue = GetStringFromValue(customAttributeInfo.DefaultValue);
				return string.Compare(val, stringFromValue, StringComparison.Ordinal) == 0;
			}

			public virtual object GetValueFromString(object obj)
			{
				object result = null;
				if (obj != null)
				{
					if (customAttributeInfo.ValueType == obj.GetType())
					{
						return obj;
					}
					if (obj is string)
					{
						string text = (string)obj;
						if (customAttributeInfo.ValueType == typeof(string))
						{
							result = text;
						}
						else if (customAttributeInfo.ValueType == typeof(float))
						{
							result = float.Parse(text, CultureInfo.InvariantCulture);
						}
						else if (customAttributeInfo.ValueType == typeof(double))
						{
							result = double.Parse(text, CultureInfo.InvariantCulture);
						}
						else if (customAttributeInfo.ValueType == typeof(int))
						{
							result = int.Parse(text, CultureInfo.InvariantCulture);
						}
						else if (customAttributeInfo.ValueType == typeof(bool))
						{
							result = bool.Parse(text);
						}
						else if (customAttributeInfo.ValueType == typeof(Color))
						{
							result = (Color)new ColorConverter().ConvertFromString(null, CultureInfo.InvariantCulture, text);
						}
						else
						{
							if (!customAttributeInfo.ValueType.IsEnum)
							{
								throw new InvalidOperationException(SR.ExceptionCustomAttributeTypeUnsupported(customAttributeInfo.ValueType.ToString()));
							}
							result = Enum.Parse(customAttributeInfo.ValueType, text, ignoreCase: true);
						}
					}
				}
				return result;
			}

			public string GetStringFromValue(object value)
			{
				if (value is Color)
				{
					return new ColorConverter().ConvertToString(null, CultureInfo.InvariantCulture, value);
				}
				if (value is float)
				{
					return ((float)value).ToString(CultureInfo.InvariantCulture);
				}
				if (value is double)
				{
					return ((double)value).ToString(CultureInfo.InvariantCulture);
				}
				if (value is int)
				{
					return ((int)value).ToString(CultureInfo.InvariantCulture);
				}
				if (value is bool)
				{
					return ((bool)value).ToString();
				}
				return value.ToString();
			}

			public virtual void ValidateValue(string attrName, object value)
			{
				if (customAttributeInfo == null)
				{
					return;
				}
				bool flag = false;
				if (customAttributeInfo.MaxValue != null)
				{
					if (value.GetType() != customAttributeInfo.MaxValue.GetType())
					{
						throw new InvalidOperationException(SR.ExceptionCustomAttributeTypeOrMaximumPossibleValueInvalid(attrName));
					}
					if (value is float)
					{
						if ((float)value > (float)customAttributeInfo.MaxValue)
						{
							flag = true;
						}
					}
					else if (value is double)
					{
						if ((double)value > (double)customAttributeInfo.MaxValue)
						{
							flag = true;
						}
					}
					else
					{
						if (!(value is int))
						{
							throw new InvalidOperationException(SR.ExceptionCustomAttributeTypeOrMinimumPossibleValueUnsupported(attrName));
						}
						if ((int)value > (int)customAttributeInfo.MaxValue)
						{
							flag = true;
						}
					}
				}
				if (customAttributeInfo.MinValue != null)
				{
					if (value.GetType() != customAttributeInfo.MinValue.GetType())
					{
						throw new InvalidOperationException(SR.ExceptionCustomAttributeTypeOrMinimumPossibleValueInvalid(attrName));
					}
					if (value is float)
					{
						if ((float)value < (float)customAttributeInfo.MinValue)
						{
							flag = true;
						}
					}
					else if (value is double)
					{
						if ((double)value < (double)customAttributeInfo.MinValue)
						{
							flag = true;
						}
					}
					else
					{
						if (!(value is int))
						{
							throw new InvalidOperationException(SR.ExceptionCustomAttributeTypeOrMinimumPossibleValueUnsupported(attrName));
						}
						if ((int)value < (int)customAttributeInfo.MinValue)
						{
							flag = true;
						}
					}
				}
				if (flag)
				{
					if (customAttributeInfo.MaxValue != null && customAttributeInfo.MinValue != null)
					{
						throw new InvalidOperationException(SR.ExceptionCustomAttributeMustBeInRange(attrName, customAttributeInfo.MinValue.ToString(), customAttributeInfo.MaxValue.ToString()));
					}
					if (customAttributeInfo.MinValue != null)
					{
						throw new InvalidOperationException(SR.ExceptionCustomAttributeMustBeBiggerThenValue(attrName, customAttributeInfo.MinValue.ToString()));
					}
					if (customAttributeInfo.MaxValue != null)
					{
						throw new InvalidOperationException(SR.ExceptionCustomAttributeMustBeMoreThenValue(attrName, customAttributeInfo.MaxValue.ToString()));
					}
				}
			}
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
			{
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(CustomAttributes))
			{
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				return ((CustomAttributes)value).DataPointAttributes.CustomAttributes;
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string && context != null && context.Instance != null)
			{
				if (context.Instance is DataPointAttributes)
				{
					((DataPointAttributes)context.Instance).CustomAttributes = (string)value;
					return new CustomAttributes((DataPointAttributes)context.Instance);
				}
				if (context.Instance is CustomAttributes)
				{
					return new CustomAttributes(((CustomAttributes)context.Instance).DataPointAttributes);
				}
				if (context.Instance is IDataPointAttributesProvider)
				{
					return new CustomAttributes(((IDataPointAttributesProvider)context.Instance).DataPointAttributes);
				}
				if (context.Instance is Array)
				{
					DataPointAttributes dataPointAttributes = null;
					foreach (object item in (Array)context.Instance)
					{
						if (item is DataPointAttributes)
						{
							dataPointAttributes = (DataPointAttributes)item;
							dataPointAttributes.CustomAttributes = (string)value;
						}
					}
					if (dataPointAttributes != null)
					{
						return new CustomAttributes(dataPointAttributes);
					}
				}
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object obj, Attribute[] attributes)
		{
			PropertyDescriptorCollection propertyDescriptorCollection = new PropertyDescriptorCollection(null);
			CustomAttributes customAttributes = obj as CustomAttributes;
			if (customAttributes != null && context != null)
			{
				Series series = (customAttributes.DataPointAttributes is Series) ? ((Series)customAttributes.DataPointAttributes) : customAttributes.DataPointAttributes.series;
				if (series != null && series.chart != null && series.chart.chartPicture != null && series.chart.chartPicture.common != null)
				{
					foreach (CustomAttributeInfo registeredCustomAttribute in ((CustomAttributeRegistry)series.chart.chartPicture.common.container.GetService(typeof(CustomAttributeRegistry))).registeredCustomAttributes)
					{
						if (IsApplicableCustomAttribute(registeredCustomAttribute, context.Instance))
						{
							Attribute[] propertyAttributes = GetPropertyAttributes(registeredCustomAttribute);
							CustomAttributesPropertyDescriptor value = new CustomAttributesPropertyDescriptor(typeof(CustomAttributes), registeredCustomAttribute.Name, registeredCustomAttribute.ValueType, propertyAttributes, registeredCustomAttribute);
							propertyDescriptorCollection.Add(value);
						}
					}
					Attribute[] attributes2 = new Attribute[3]
					{
						new NotifyParentPropertyAttribute(notifyParent: true),
						new RefreshPropertiesAttribute(RefreshProperties.All),
						new DescriptionAttribute(SR.DescriptionAttributeUserDefined)
					};
					CustomAttributesPropertyDescriptor value2 = new CustomAttributesPropertyDescriptor(typeof(CustomAttributes), "UserDefined", typeof(string), attributes2, null);
					propertyDescriptorCollection.Add(value2);
				}
			}
			return propertyDescriptorCollection;
		}

		private bool IsApplicableCustomAttribute(CustomAttributeInfo attrInfo, object obj)
		{
			if (obj is CustomAttributes)
			{
				obj = ((CustomAttributes)obj).DataPointAttributes;
			}
			if (((IsDataPoint(obj) && attrInfo.AppliesToDataPoint) || (!IsDataPoint(obj) && attrInfo.AppliesToSeries)) && ((Is3DChartType(obj) && attrInfo.AppliesTo3D) || (!Is3DChartType(obj) && attrInfo.AppliesTo2D)))
			{
				SeriesChartType[] selectedChartTypes = GetSelectedChartTypes(obj);
				foreach (SeriesChartType seriesChartType in selectedChartTypes)
				{
					SeriesChartType[] appliesToChartType = attrInfo.AppliesToChartType;
					for (int j = 0; j < appliesToChartType.Length; j++)
					{
						if (appliesToChartType[j] == seriesChartType)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		private bool IsDataPoint(object obj)
		{
			if (obj is Series)
			{
				return false;
			}
			if (obj is Array && ((Array)obj).Length > 0 && ((Array)obj).GetValue(0) is Series)
			{
				return false;
			}
			return true;
		}

		private bool Is3DChartType(object obj)
		{
			Series[] selectedSeries = GetSelectedSeries(obj);
			foreach (Series series in selectedSeries)
			{
				if (series.chart.ChartAreas[series.ChartArea].Area3DStyle.Enable3D)
				{
					return true;
				}
			}
			return false;
		}

		private Series[] GetSelectedSeries(object obj)
		{
			Series[] array = new Series[0];
			if (obj is Array && ((Array)obj).Length > 0)
			{
				if (((Array)obj).GetValue(0) is Series)
				{
					array = new Series[((Array)obj).Length];
					((Array)obj).CopyTo(array, 0);
				}
				else if (((Array)obj).GetValue(0) is DataPointAttributes)
				{
					array = new Series[1]
					{
						((DataPointAttributes)((Array)obj).GetValue(0)).series
					};
				}
			}
			else if (obj is Series)
			{
				array = new Series[1]
				{
					(Series)obj
				};
			}
			else if (obj is DataPointAttributes)
			{
				array = new Series[1]
				{
					((DataPointAttributes)obj).series
				};
			}
			return array;
		}

		private SeriesChartType[] GetSelectedChartTypes(object obj)
		{
			Series[] selectedSeries = GetSelectedSeries(obj);
			int num = 0;
			SeriesChartType[] array = new SeriesChartType[selectedSeries.Length];
			Series[] array2 = selectedSeries;
			foreach (Series series in array2)
			{
				array[num++] = series.ChartType;
			}
			return array;
		}

		private Attribute[] GetPropertyAttributes(CustomAttributeInfo attrInfo)
		{
			DefaultValueAttribute defaultValueAttribute = null;
			if (attrInfo.DefaultValue.GetType() == attrInfo.ValueType)
			{
				defaultValueAttribute = new DefaultValueAttribute(attrInfo.DefaultValue);
			}
			else
			{
				if (!(attrInfo.DefaultValue is string))
				{
					throw new InvalidOperationException(SR.ExceptionCustomAttributeDefaultValueTypeInvalid);
				}
				defaultValueAttribute = new DefaultValueAttribute(attrInfo.ValueType, (string)attrInfo.DefaultValue);
			}
			ArrayList obj = new ArrayList
			{
				new NotifyParentPropertyAttribute(notifyParent: true),
				new RefreshPropertiesAttribute(RefreshProperties.All),
				new DescriptionAttribute(attrInfo.Description),
				defaultValueAttribute
			};
			int num = 0;
			Attribute[] array = new Attribute[obj.Count];
			foreach (Attribute item in obj)
			{
				array[num++] = item;
			}
			return array;
		}
	}
}
