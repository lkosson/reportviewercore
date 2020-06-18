using Microsoft.Reporting.Chart.WebForms.Design;
using Microsoft.Reporting.Chart.WebForms.Utilities;
using System;
using System.ComponentModel;

namespace Microsoft.Reporting.Chart.WebForms
{
	[TypeConverter(typeof(CustomAttributeTypeConverter))]
	internal class CustomAttributes
	{
		internal DataPointAttributes m_DataPointAttributes;

		internal virtual DataPointAttributes DataPointAttributes
		{
			get
			{
				return m_DataPointAttributes;
			}
			set
			{
				m_DataPointAttributes = value;
			}
		}

		public CustomAttributes(DataPointAttributes attributes)
		{
			DataPointAttributes = attributes;
		}

		internal virtual string GetUserDefinedAttributes()
		{
			return GetUserDefinedAttributes(userDefined: true);
		}

		internal virtual string GetUserDefinedAttributes(bool userDefined)
		{
			string customAttributes = DataPointAttributes.CustomAttributes;
			string text = string.Empty;
			CustomAttributeRegistry customAttributeRegistry = (CustomAttributeRegistry)((DataPointAttributes is Series) ? ((Series)DataPointAttributes) : DataPointAttributes.series).chart.chartPicture.common.container.GetService(typeof(CustomAttributeRegistry));
			customAttributes = customAttributes.Replace("\\,", "\\x45");
			customAttributes = customAttributes.Replace("\\=", "\\x46");
			if (customAttributes.Length > 0)
			{
				string[] array = customAttributes.Split(',');
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split('=');
					if (array2.Length != 2)
					{
						throw new FormatException(SR.ExceptionAttributeInvalidFormat);
					}
					array2[0] = array2[0].Trim();
					array2[1] = array2[1].Trim();
					if (array2[0].Length == 0)
					{
						throw new FormatException(SR.ExceptionAttributeInvalidFormat);
					}
					bool flag = true;
					foreach (CustomAttributeInfo registeredCustomAttribute in customAttributeRegistry.registeredCustomAttributes)
					{
						if (string.Compare(registeredCustomAttribute.Name, array2[0], StringComparison.OrdinalIgnoreCase) == 0)
						{
							flag = false;
						}
					}
					if (flag == userDefined)
					{
						if (text.Length > 0)
						{
							text += ", ";
						}
						string text2 = array2[1].Replace("\\x45", ",");
						text2 = text2.Replace("\\x46", "=");
						text = text + array2[0] + "=" + text2;
					}
				}
			}
			return text;
		}

		internal virtual void SetUserDefinedAttributes(string val)
		{
			string text = GetUserDefinedAttributes(userDefined: false);
			if (val.Length > 0)
			{
				if (text.Length > 0)
				{
					text += ", ";
				}
				text += val;
			}
			DataPointAttributes.CustomAttributes = text;
		}

		public override bool Equals(object obj)
		{
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
