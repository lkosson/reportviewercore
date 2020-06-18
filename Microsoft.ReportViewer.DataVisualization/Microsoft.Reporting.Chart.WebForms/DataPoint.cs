using Microsoft.Reporting.Chart.WebForms.Design;
using Microsoft.Reporting.Chart.WebForms.Utilities;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeDataPoint_DataPoint")]
	[DefaultProperty("YValues")]
	internal class DataPoint : DataPointAttributes
	{
		private double xValue;

		private double[] yValue = new double[1];

		internal PointF positionRel = PointF.Empty;

		[SRCategory("CategoryAttributeData")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeDataPoint_XValue")]
		[TypeConverter(typeof(DataPointValueConverter))]
		[DefaultValue(typeof(double), "0.0")]
		[Browsable(false)]
		public double XValue
		{
			get
			{
				return xValue;
			}
			set
			{
				xValue = value;
				Invalidate(invalidateLegend: false);
			}
		}

		[SRCategory("CategoryAttributeData")]
		[SRDescription("DescriptionAttributeDataPoint_YValues")]
		[Bindable(true)]
		[Browsable(false)]
		[TypeConverter(typeof(DoubleArrayConverter))]
		[RefreshProperties(RefreshProperties.All)]
		[SerializationVisibility(SerializationVisibility.Attribute)]
		public double[] YValues
		{
			get
			{
				return yValue;
			}
			set
			{
				if (value == null)
				{
					for (int i = 0; i < yValue.Length; i++)
					{
						yValue[i] = 0.0;
					}
				}
				else
				{
					yValue = value;
				}
				Invalidate(invalidateLegend: false);
			}
		}

		[SRCategory("CategoryAttributeData")]
		[Browsable(false)]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeDataPoint_Empty")]
		[DefaultValue(false)]
		public bool Empty
		{
			get
			{
				return emptyPoint;
			}
			set
			{
				emptyPoint = value;
				Invalidate(invalidateLegend: true);
			}
		}

		[SRCategory("CategoryAttributeData")]
		[Bindable(true)]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeDataPoint_Name")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public string Name
		{
			get
			{
				return "DataPoint";
			}
			set
			{
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeMarker")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeMarkerStyle4")]
		[RefreshProperties(RefreshProperties.All)]
		public new MarkerStyle MarkerStyle
		{
			get
			{
				return base.MarkerStyle;
			}
			set
			{
				base.MarkerStyle = value;
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeMarker")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeMarkerSize")]
		[DefaultValue(5)]
		[RefreshProperties(RefreshProperties.All)]
		public new int MarkerSize
		{
			get
			{
				return base.MarkerSize;
			}
			set
			{
				base.MarkerSize = value;
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeMarker")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeMarkerColor3")]
		[DefaultValue(typeof(Color), "")]
		[RefreshProperties(RefreshProperties.All)]
		public new Color MarkerColor
		{
			get
			{
				return base.MarkerColor;
			}
			set
			{
				base.MarkerColor = value;
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeMarker")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeMarkerBorderColor")]
		[DefaultValue(typeof(Color), "")]
		[RefreshProperties(RefreshProperties.All)]
		public new Color MarkerBorderColor
		{
			get
			{
				return base.MarkerBorderColor;
			}
			set
			{
				base.MarkerBorderColor = value;
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeColor4")]
		[DefaultValue(typeof(Color), "")]
		public new Color Color
		{
			get
			{
				return base.Color;
			}
			set
			{
				base.Color = value;
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeBorderColor9")]
		[DefaultValue(typeof(Color), "")]
		public new Color BorderColor
		{
			get
			{
				return base.BorderColor;
			}
			set
			{
				base.BorderColor = value;
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeBorderStyle3")]
		[DefaultValue(ChartDashStyle.Solid)]
		public new ChartDashStyle BorderStyle
		{
			get
			{
				return base.BorderStyle;
			}
			set
			{
				base.BorderStyle = value;
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeBorderWidth8")]
		[DefaultValue(1)]
		public new int BorderWidth
		{
			get
			{
				return base.BorderWidth;
			}
			set
			{
				base.BorderWidth = value;
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeBackGradientType4")]
		[DefaultValue(GradientType.None)]
		public new GradientType BackGradientType
		{
			get
			{
				return base.BackGradientType;
			}
			set
			{
				base.BackGradientType = value;
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeBackGradientEndColor7")]
		[DefaultValue(typeof(Color), "")]
		public new Color BackGradientEndColor
		{
			get
			{
				return base.BackGradientEndColor;
			}
			set
			{
				base.BackGradientEndColor = value;
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeBackHatchStyle9")]
		[DefaultValue(ChartHatchStyle.None)]
		public new ChartHatchStyle BackHatchStyle
		{
			get
			{
				return base.BackHatchStyle;
			}
			set
			{
				base.BackHatchStyle = value;
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeLabelAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeFont")]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8pt")]
		public new Font Font
		{
			get
			{
				return base.Font;
			}
			set
			{
				base.Font = value;
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeLabelAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeFontColor")]
		[DefaultValue(typeof(Color), "Black")]
		public new Color FontColor
		{
			get
			{
				return base.FontColor;
			}
			set
			{
				base.FontColor = value;
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeLabel")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLabel")]
		[DefaultValue("")]
		public override string Label
		{
			get
			{
				return base.Label;
			}
			set
			{
				base.Label = value;
			}
		}

		public DataPoint()
			: base(null, pointAttributes: true)
		{
			yValue = new double[1];
		}

		public DataPoint(Series series)
			: base(series, pointAttributes: true)
		{
			yValue = new double[series.YValuesPerPoint];
			xValue = 0.0;
		}

		public DataPoint(double xValue, string yValues)
			: base(null, pointAttributes: true)
		{
			string[] array = yValues.Split(',');
			yValue = new double[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				yValue[i] = CommonElements.ParseDouble(array[i]);
			}
			this.xValue = xValue;
		}

		public DataPoint(double xValue, double yValue)
			: base(null, pointAttributes: true)
		{
			this.yValue = new double[1];
			this.yValue[0] = yValue;
			this.xValue = xValue;
		}

		internal void SetPointAttribute(object obj, string attributeName, string format)
		{
			string text = obj as string;
			if (text == null)
			{
				double num = double.NaN;
				ChartValueTypes valueType = ChartValueTypes.Auto;
				if (obj is DateTime)
				{
					num = ((DateTime)obj).ToOADate();
					valueType = ChartValueTypes.Date;
				}
				else
				{
					num = ConvertValue(obj);
				}
				if (!double.IsNaN(num))
				{
					try
					{
						text = ValueConverter.FormatValue(series.chart, this, num, format, valueType, ChartElementType.DataPoint);
					}
					catch
					{
						text = obj.ToString();
					}
				}
				else
				{
					text = obj.ToString();
				}
			}
			if (text.Length > 0)
			{
				if (string.Compare(attributeName, "AxisLabel", StringComparison.OrdinalIgnoreCase) == 0)
				{
					AxisLabel = text;
				}
				else if (string.Compare(attributeName, "Tooltip", StringComparison.OrdinalIgnoreCase) == 0)
				{
					base.ToolTip = text;
				}
				else if (string.Compare(attributeName, "Href", StringComparison.OrdinalIgnoreCase) == 0)
				{
					base.Href = text;
				}
				else if (string.Compare(attributeName, "Label", StringComparison.OrdinalIgnoreCase) == 0)
				{
					Label = text;
				}
				else if (string.Compare(attributeName, "LegendTooltip", StringComparison.OrdinalIgnoreCase) == 0)
				{
					base.LegendToolTip = text;
				}
				else if (string.Compare(attributeName, "LegendText", StringComparison.OrdinalIgnoreCase) == 0)
				{
					base.LegendText = text;
				}
				else if (string.Compare(attributeName, "LabelToolTip", StringComparison.OrdinalIgnoreCase) == 0)
				{
					base.LabelToolTip = text;
				}
				else
				{
					base[attributeName] = text;
				}
			}
		}

		private double ConvertValue(object value)
		{
			if (value == null)
			{
				return 0.0;
			}
			if (value is double)
			{
				return (double)value;
			}
			if (value is float)
			{
				return (float)value;
			}
			if (value is decimal)
			{
				return (double)(decimal)value;
			}
			if (value is int)
			{
				return (int)value;
			}
			if (value is uint)
			{
				return (uint)value;
			}
			if (value is long)
			{
				return (long)value;
			}
			if (value is ulong)
			{
				return (ulong)value;
			}
			if (value is byte)
			{
				return (int)(byte)value;
			}
			if (value is sbyte)
			{
				return (sbyte)value;
			}
			if (value is bool)
			{
				if (!(bool)value)
				{
					return 0.0;
				}
				return 1.0;
			}
			return CommonElements.ParseDouble(value.ToString());
		}

		public void SetValueXY(object xValue, params object[] yValue)
		{
			SetValueY(yValue);
			Type type = xValue.GetType();
			if (series != null)
			{
				series.CheckSupportedTypes(type);
			}
			if (type == typeof(string))
			{
				AxisLabel = (string)xValue;
			}
			else if (type == typeof(DateTime))
			{
				this.xValue = ((DateTime)xValue).ToOADate();
			}
			else
			{
				this.xValue = ConvertValue(xValue);
			}
			if (series != null && xValue is DateTime)
			{
				if (series.XValueType == ChartValueTypes.Date)
				{
					DateTime dateTime = new DateTime(((DateTime)xValue).Year, ((DateTime)xValue).Month, ((DateTime)xValue).Day, 0, 0, 0, 0);
					this.xValue = dateTime.ToOADate();
				}
				else if (series.XValueType == ChartValueTypes.Time)
				{
					DateTime dateTime2 = new DateTime(1899, 12, 30, ((DateTime)xValue).Hour, ((DateTime)xValue).Minute, ((DateTime)xValue).Second, ((DateTime)xValue).Millisecond);
					this.xValue = dateTime2.ToOADate();
				}
			}
			bool flag = false;
			double[] array = this.yValue;
			for (int i = 0; i < array.Length; i++)
			{
				if (double.IsNaN(array[i]))
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				Empty = true;
				for (int j = 0; j < this.yValue.Length; j++)
				{
					this.yValue[j] = 0.0;
				}
			}
		}

		public void SetValueY(params object[] yValue)
		{
			if (yValue.Length == 0 || (series != null && yValue.Length > series.YValuesPerPoint))
			{
				throw new ArgumentOutOfRangeException("yValue", SR.ExceptionDataPointYValuesSettingCountMismatch(series.YValuesPerPoint.ToString(CultureInfo.InvariantCulture)));
			}
			for (int i = 0; i < yValue.Length; i++)
			{
				if (yValue[i] == null || yValue[i] is DBNull)
				{
					yValue[i] = 0.0;
					if (i == 0)
					{
						Empty = true;
					}
				}
			}
			Type type = yValue[0].GetType();
			if (series != null)
			{
				series.CheckSupportedTypes(type);
			}
			if (this.yValue.Length < yValue.Length)
			{
				this.yValue = new double[yValue.Length];
			}
			if (type == typeof(string))
			{
				try
				{
					for (int j = 0; j < yValue.Length; j++)
					{
						this.yValue[j] = CommonElements.ParseDouble((string)yValue[j]);
					}
				}
				catch
				{
					if (series != null && series.chart == null && series.serviceContainer != null)
					{
						series.chart = (Chart)series.serviceContainer.GetService(typeof(Chart));
					}
					if (series == null || series.chart == null || !series.chart.chartPicture.SuppressExceptions)
					{
						throw new ArgumentException(SR.ExceptionDataPointYValueStringFormat);
					}
					Empty = true;
					for (int k = 0; k < yValue.Length; k++)
					{
						yValue[k] = 0.0;
					}
				}
			}
			else if (type == typeof(DateTime))
			{
				for (int l = 0; l < yValue.Length; l++)
				{
					if (yValue[l] == null || (yValue[l] is double && (double)yValue[l] == 0.0))
					{
						this.yValue[l] = DateTime.Now.ToOADate();
					}
					else
					{
						this.yValue[l] = ((DateTime)yValue[l]).ToOADate();
					}
				}
			}
			else
			{
				for (int m = 0; m < yValue.Length; m++)
				{
					this.yValue[m] = ConvertValue(yValue[m]);
				}
			}
			if (series == null)
			{
				return;
			}
			for (int n = 0; n < yValue.Length; n++)
			{
				if (yValue[n] == null || (yValue[n] is double && (double)yValue[n] == 0.0))
				{
					if (series.YValueType == ChartValueTypes.Date)
					{
						this.yValue[n] = Math.Floor(this.yValue[n]);
					}
					else if (series.YValueType == ChartValueTypes.Time)
					{
						this.yValue[n] = xValue - Math.Floor(this.yValue[n]);
					}
				}
				else if (series.YValueType == ChartValueTypes.Date)
				{
					DateTime dateTime = (yValue[n] is DateTime) ? ((DateTime)yValue[n]) : ((!(yValue[n] is double)) ? Convert.ToDateTime(yValue[n], CultureInfo.InvariantCulture) : DateTime.FromOADate((double)yValue[n]));
					DateTime dateTime2 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, 0);
					this.yValue[n] = dateTime2.ToOADate();
				}
				else if (series.YValueType == ChartValueTypes.Time)
				{
					DateTime dateTime3;
					if (yValue[n] is DateTime)
					{
						dateTime3 = (DateTime)yValue[n];
					}
					dateTime3 = ((!(yValue[n] is double)) ? Convert.ToDateTime(yValue[n], CultureInfo.InvariantCulture) : DateTime.FromOADate((double)yValue[n]));
					DateTime dateTime4 = new DateTime(1899, 12, 30, dateTime3.Hour, dateTime3.Minute, dateTime3.Second, dateTime3.Millisecond);
					this.yValue[n] = dateTime4.ToOADate();
				}
			}
		}

		public DataPoint Clone()
		{
			DataPoint dataPoint = new DataPoint();
			dataPoint.series = null;
			dataPoint.pointAttributes = pointAttributes;
			dataPoint.xValue = XValue;
			dataPoint.yValue = new double[yValue.Length];
			yValue.CopyTo(dataPoint.yValue, 0);
			dataPoint.tempColorIsSet = tempColorIsSet;
			dataPoint.emptyPoint = emptyPoint;
			foreach (object key in attributes.Keys)
			{
				dataPoint.attributes.Add(key, attributes[key]);
			}
			((IMapAreaAttributes)dataPoint).Tag = ((IMapAreaAttributes)this).Tag;
			return dataPoint;
		}

		internal void ResizeYValueArray(int newSize)
		{
			double[] array = new double[newSize];
			if (yValue != null)
			{
				for (int i = 0; i < ((yValue.Length < newSize) ? yValue.Length : newSize); i++)
				{
					array[i] = yValue[i];
				}
			}
			yValue = array;
		}

		public double GetValueY(int yValueIndex)
		{
			return YValues[yValueIndex];
		}

		public void SetValueY(int yValueIndex, double yValue)
		{
			YValues[yValueIndex] = yValue;
		}

		internal double GetValueByName(string valueName)
		{
			valueName = valueName.ToUpper(CultureInfo.InvariantCulture);
			if (string.Compare(valueName, "X", StringComparison.Ordinal) == 0)
			{
				return XValue;
			}
			if (valueName.StartsWith("Y", StringComparison.Ordinal))
			{
				if (valueName.Length == 1)
				{
					return YValues[0];
				}
				int num = 0;
				try
				{
					num = int.Parse(valueName.Substring(1), CultureInfo.InvariantCulture) - 1;
				}
				catch (Exception)
				{
					throw new ArgumentException(SR.ExceptionDataPointValueNameInvalid, "valueName");
				}
				if (num < 0)
				{
					throw new ArgumentException(SR.ExceptionDataPointValueNameYIndexIsNotPositive, "valueName");
				}
				if (num >= YValues.Length)
				{
					throw new ArgumentException(SR.ExceptionDataPointValueNameYIndexOutOfRange, "valueName");
				}
				return YValues[num];
			}
			throw new ArgumentException(SR.ExceptionDataPointValueNameInvalid, "valueName");
		}

		internal string ReplaceKeywords(string strOriginal)
		{
			if (strOriginal == null || strOriginal.Length == 0)
			{
				return strOriginal;
			}
			string text = strOriginal;
			text = text.Replace("\\n", "\n");
			text = text.Replace("#LABEL", Label);
			text = text.Replace("#LEGENDTEXT", base.LegendText);
			text = text.Replace("#AXISLABEL", AxisLabel);
			if (series != null)
			{
				text = text.Replace("#INDEX", series.Points.IndexOf(this).ToString(CultureInfo.InvariantCulture));
				text = series.ReplaceKeywords(text);
				text = series.ReplaceOneKeyword(series.chart, this, ChartElementType.DataPoint, text, "#PERCENT", YValues[0] / series.GetTotalYValue(), ChartValueTypes.Double, "P");
				text = ((series.XValueType != ChartValueTypes.String) ? series.ReplaceOneKeyword(series.chart, this, ChartElementType.DataPoint, text, "#VALX", XValue, series.XValueType, "") : text.Replace("#VALX", AxisLabel));
				for (int i = YValues.Length; i <= 7; i++)
				{
					text = RemoveOneKeyword(text, "#VALY" + (i + 1), SR.FormatErrorString);
				}
				for (int j = 1; j <= YValues.Length; j++)
				{
					text = series.ReplaceOneKeyword(series.chart, this, ChartElementType.DataPoint, text, "#VALY" + j, YValues[j - 1], series.YValueType, "");
				}
				text = series.ReplaceOneKeyword(series.chart, this, ChartElementType.DataPoint, text, "#VALY", YValues[0], series.YValueType, "");
				text = series.ReplaceOneKeyword(series.chart, this, ChartElementType.DataPoint, text, "#VAL", YValues[0], series.YValueType, "");
			}
			return text;
		}

		private string RemoveOneKeyword(string strOriginal, string keyword, string strToReplace)
		{
			string text = strOriginal;
			int num = -1;
			while ((num = text.IndexOf(keyword, StringComparison.Ordinal)) != -1)
			{
				int num2 = num + keyword.Length;
				if (text.Length > num2 && text[num2] == '{')
				{
					int num3 = text.IndexOf('}', num2);
					if (num3 == -1)
					{
						throw new InvalidOperationException(SR.ExceptionDataSeriesKeywordFormatInvalid(text));
					}
					num2 = num3 + 1;
				}
				text = text.Remove(num, num2 - num);
				if (!string.IsNullOrEmpty(strToReplace))
				{
					text = text.Insert(num, strToReplace);
				}
			}
			return text;
		}

		public void ResetYValues()
		{
			YValues = null;
		}
	}
}
