using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	[SkipStaticValidation]
	internal sealed class NumericIndicator : GaugePanelItem, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private GaugeInputValue m_gaugeInputValue;

		private List<NumericIndicatorRange> m_numericIndicatorRanges;

		private ExpressionInfo m_decimalDigitColor;

		private ExpressionInfo m_digitColor;

		private ExpressionInfo m_useFontPercent;

		private ExpressionInfo m_decimalDigits;

		private ExpressionInfo m_digits;

		private GaugeInputValue m_minimumValue;

		private GaugeInputValue m_maximumValue;

		private ExpressionInfo m_multiplier;

		private ExpressionInfo m_nonNumericString;

		private ExpressionInfo m_outOfRangeString;

		private ExpressionInfo m_resizeMode;

		private ExpressionInfo m_showDecimalPoint;

		private ExpressionInfo m_showLeadingZeros;

		private ExpressionInfo m_indicatorStyle;

		private ExpressionInfo m_showSign;

		private ExpressionInfo m_snappingEnabled;

		private ExpressionInfo m_snappingInterval;

		private ExpressionInfo m_ledDimColor;

		private ExpressionInfo m_separatorWidth;

		private ExpressionInfo m_separatorColor;

		internal GaugeInputValue GaugeInputValue
		{
			get
			{
				return m_gaugeInputValue;
			}
			set
			{
				m_gaugeInputValue = value;
			}
		}

		internal List<NumericIndicatorRange> NumericIndicatorRanges
		{
			get
			{
				return m_numericIndicatorRanges;
			}
			set
			{
				m_numericIndicatorRanges = value;
			}
		}

		internal ExpressionInfo DecimalDigitColor
		{
			get
			{
				return m_decimalDigitColor;
			}
			set
			{
				m_decimalDigitColor = value;
			}
		}

		internal ExpressionInfo DigitColor
		{
			get
			{
				return m_digitColor;
			}
			set
			{
				m_digitColor = value;
			}
		}

		internal ExpressionInfo UseFontPercent
		{
			get
			{
				return m_useFontPercent;
			}
			set
			{
				m_useFontPercent = value;
			}
		}

		internal ExpressionInfo DecimalDigits
		{
			get
			{
				return m_decimalDigits;
			}
			set
			{
				m_decimalDigits = value;
			}
		}

		internal ExpressionInfo Digits
		{
			get
			{
				return m_digits;
			}
			set
			{
				m_digits = value;
			}
		}

		internal GaugeInputValue MinimumValue
		{
			get
			{
				return m_minimumValue;
			}
			set
			{
				m_minimumValue = value;
			}
		}

		internal GaugeInputValue MaximumValue
		{
			get
			{
				return m_maximumValue;
			}
			set
			{
				m_maximumValue = value;
			}
		}

		internal ExpressionInfo Multiplier
		{
			get
			{
				return m_multiplier;
			}
			set
			{
				m_multiplier = value;
			}
		}

		internal ExpressionInfo NonNumericString
		{
			get
			{
				return m_nonNumericString;
			}
			set
			{
				m_nonNumericString = value;
			}
		}

		internal ExpressionInfo OutOfRangeString
		{
			get
			{
				return m_outOfRangeString;
			}
			set
			{
				m_outOfRangeString = value;
			}
		}

		internal ExpressionInfo ResizeMode
		{
			get
			{
				return m_resizeMode;
			}
			set
			{
				m_resizeMode = value;
			}
		}

		internal ExpressionInfo ShowDecimalPoint
		{
			get
			{
				return m_showDecimalPoint;
			}
			set
			{
				m_showDecimalPoint = value;
			}
		}

		internal ExpressionInfo ShowLeadingZeros
		{
			get
			{
				return m_showLeadingZeros;
			}
			set
			{
				m_showLeadingZeros = value;
			}
		}

		internal ExpressionInfo IndicatorStyle
		{
			get
			{
				return m_indicatorStyle;
			}
			set
			{
				m_indicatorStyle = value;
			}
		}

		internal ExpressionInfo ShowSign
		{
			get
			{
				return m_showSign;
			}
			set
			{
				m_showSign = value;
			}
		}

		internal ExpressionInfo SnappingEnabled
		{
			get
			{
				return m_snappingEnabled;
			}
			set
			{
				m_snappingEnabled = value;
			}
		}

		internal ExpressionInfo SnappingInterval
		{
			get
			{
				return m_snappingInterval;
			}
			set
			{
				m_snappingInterval = value;
			}
		}

		internal ExpressionInfo LedDimColor
		{
			get
			{
				return m_ledDimColor;
			}
			set
			{
				m_ledDimColor = value;
			}
		}

		internal ExpressionInfo SeparatorWidth
		{
			get
			{
				return m_separatorWidth;
			}
			set
			{
				m_separatorWidth = value;
			}
		}

		internal ExpressionInfo SeparatorColor
		{
			get
			{
				return m_separatorColor;
			}
			set
			{
				m_separatorColor = value;
			}
		}

		internal new NumericIndicatorExprHost ExprHost => (NumericIndicatorExprHost)m_exprHost;

		internal NumericIndicator()
		{
		}

		internal NumericIndicator(GaugePanel gaugePanel, int id)
			: base(gaugePanel, id)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.NumericIndicatorStart(m_name);
			base.Initialize(context);
			if (m_numericIndicatorRanges != null)
			{
				for (int i = 0; i < m_numericIndicatorRanges.Count; i++)
				{
					m_numericIndicatorRanges[i].Initialize(context);
				}
			}
			if (m_decimalDigitColor != null)
			{
				m_decimalDigitColor.Initialize("DecimalDigitColor", context);
				context.ExprHostBuilder.NumericIndicatorDecimalDigitColor(m_decimalDigitColor);
			}
			if (m_digitColor != null)
			{
				m_digitColor.Initialize("DigitColor", context);
				context.ExprHostBuilder.NumericIndicatorDigitColor(m_digitColor);
			}
			if (m_useFontPercent != null)
			{
				m_useFontPercent.Initialize("UseFontPercent", context);
				context.ExprHostBuilder.NumericIndicatorUseFontPercent(m_useFontPercent);
			}
			if (m_decimalDigits != null)
			{
				m_decimalDigits.Initialize("DecimalDigits", context);
				context.ExprHostBuilder.NumericIndicatorDecimalDigits(m_decimalDigits);
			}
			if (m_digits != null)
			{
				m_digits.Initialize("Digits", context);
				context.ExprHostBuilder.NumericIndicatorDigits(m_digits);
			}
			if (m_multiplier != null)
			{
				m_multiplier.Initialize("Multiplier", context);
				context.ExprHostBuilder.NumericIndicatorMultiplier(m_multiplier);
			}
			if (m_nonNumericString != null)
			{
				m_nonNumericString.Initialize("NonNumericString", context);
				context.ExprHostBuilder.NumericIndicatorNonNumericString(m_nonNumericString);
			}
			if (m_outOfRangeString != null)
			{
				m_outOfRangeString.Initialize("OutOfRangeString", context);
				context.ExprHostBuilder.NumericIndicatorOutOfRangeString(m_outOfRangeString);
			}
			if (m_resizeMode != null)
			{
				m_resizeMode.Initialize("ResizeMode", context);
				context.ExprHostBuilder.NumericIndicatorResizeMode(m_resizeMode);
			}
			if (m_showDecimalPoint != null)
			{
				m_showDecimalPoint.Initialize("ShowDecimalPoint", context);
				context.ExprHostBuilder.NumericIndicatorShowDecimalPoint(m_showDecimalPoint);
			}
			if (m_showLeadingZeros != null)
			{
				m_showLeadingZeros.Initialize("ShowLeadingZeros", context);
				context.ExprHostBuilder.NumericIndicatorShowLeadingZeros(m_showLeadingZeros);
			}
			if (m_indicatorStyle != null)
			{
				m_indicatorStyle.Initialize("IndicatorStyle", context);
				context.ExprHostBuilder.NumericIndicatorIndicatorStyle(m_indicatorStyle);
			}
			if (m_showSign != null)
			{
				m_showSign.Initialize("ShowSign", context);
				context.ExprHostBuilder.NumericIndicatorShowSign(m_showSign);
			}
			if (m_snappingEnabled != null)
			{
				m_snappingEnabled.Initialize("SnappingEnabled", context);
				context.ExprHostBuilder.NumericIndicatorSnappingEnabled(m_snappingEnabled);
			}
			if (m_snappingInterval != null)
			{
				m_snappingInterval.Initialize("SnappingInterval", context);
				context.ExprHostBuilder.NumericIndicatorSnappingInterval(m_snappingInterval);
			}
			if (m_ledDimColor != null)
			{
				m_ledDimColor.Initialize("LedDimColor", context);
				context.ExprHostBuilder.NumericIndicatorLedDimColor(m_ledDimColor);
			}
			if (m_separatorWidth != null)
			{
				m_separatorWidth.Initialize("SeparatorWidth", context);
				context.ExprHostBuilder.NumericIndicatorSeparatorWidth(m_separatorWidth);
			}
			if (m_separatorColor != null)
			{
				m_separatorColor.Initialize("SeparatorColor", context);
				context.ExprHostBuilder.NumericIndicatorSeparatorColor(m_separatorColor);
			}
			m_exprHostID = context.ExprHostBuilder.NumericIndicatorEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			NumericIndicator numericIndicator = (NumericIndicator)base.PublishClone(context);
			if (m_gaugeInputValue != null)
			{
				numericIndicator.m_gaugeInputValue = (GaugeInputValue)m_gaugeInputValue.PublishClone(context);
			}
			if (m_numericIndicatorRanges != null)
			{
				numericIndicator.m_numericIndicatorRanges = new List<NumericIndicatorRange>(m_numericIndicatorRanges.Count);
				foreach (NumericIndicatorRange numericIndicatorRange in m_numericIndicatorRanges)
				{
					numericIndicator.m_numericIndicatorRanges.Add((NumericIndicatorRange)numericIndicatorRange.PublishClone(context));
				}
			}
			if (m_decimalDigitColor != null)
			{
				numericIndicator.m_decimalDigitColor = (ExpressionInfo)m_decimalDigitColor.PublishClone(context);
			}
			if (m_digitColor != null)
			{
				numericIndicator.m_digitColor = (ExpressionInfo)m_digitColor.PublishClone(context);
			}
			if (m_useFontPercent != null)
			{
				numericIndicator.m_useFontPercent = (ExpressionInfo)m_useFontPercent.PublishClone(context);
			}
			if (m_decimalDigits != null)
			{
				numericIndicator.m_decimalDigits = (ExpressionInfo)m_decimalDigits.PublishClone(context);
			}
			if (m_digits != null)
			{
				numericIndicator.m_digits = (ExpressionInfo)m_digits.PublishClone(context);
			}
			if (m_minimumValue != null)
			{
				numericIndicator.m_minimumValue = (GaugeInputValue)m_minimumValue.PublishClone(context);
			}
			if (m_maximumValue != null)
			{
				numericIndicator.m_maximumValue = (GaugeInputValue)m_maximumValue.PublishClone(context);
			}
			if (m_multiplier != null)
			{
				numericIndicator.m_multiplier = (ExpressionInfo)m_multiplier.PublishClone(context);
			}
			if (m_nonNumericString != null)
			{
				numericIndicator.m_nonNumericString = (ExpressionInfo)m_nonNumericString.PublishClone(context);
			}
			if (m_outOfRangeString != null)
			{
				numericIndicator.m_outOfRangeString = (ExpressionInfo)m_outOfRangeString.PublishClone(context);
			}
			if (m_resizeMode != null)
			{
				numericIndicator.m_resizeMode = (ExpressionInfo)m_resizeMode.PublishClone(context);
			}
			if (m_showDecimalPoint != null)
			{
				numericIndicator.m_showDecimalPoint = (ExpressionInfo)m_showDecimalPoint.PublishClone(context);
			}
			if (m_showLeadingZeros != null)
			{
				numericIndicator.m_showLeadingZeros = (ExpressionInfo)m_showLeadingZeros.PublishClone(context);
			}
			if (m_indicatorStyle != null)
			{
				numericIndicator.m_indicatorStyle = (ExpressionInfo)m_indicatorStyle.PublishClone(context);
			}
			if (m_showSign != null)
			{
				numericIndicator.m_showSign = (ExpressionInfo)m_showSign.PublishClone(context);
			}
			if (m_snappingEnabled != null)
			{
				numericIndicator.m_snappingEnabled = (ExpressionInfo)m_snappingEnabled.PublishClone(context);
			}
			if (m_snappingInterval != null)
			{
				numericIndicator.m_snappingInterval = (ExpressionInfo)m_snappingInterval.PublishClone(context);
			}
			if (m_ledDimColor != null)
			{
				numericIndicator.m_ledDimColor = (ExpressionInfo)m_ledDimColor.PublishClone(context);
			}
			if (m_separatorWidth != null)
			{
				numericIndicator.m_separatorWidth = (ExpressionInfo)m_separatorWidth.PublishClone(context);
			}
			if (m_separatorColor != null)
			{
				numericIndicator.m_separatorColor = (ExpressionInfo)m_separatorColor.PublishClone(context);
			}
			return numericIndicator;
		}

		internal void SetExprHost(NumericIndicatorExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			SetExprHost((GaugePanelItemExprHost)exprHost, reportObjectModel);
			if (m_gaugeInputValue != null && ExprHost.GaugeInputValueHost != null)
			{
				m_gaugeInputValue.SetExprHost(ExprHost.GaugeInputValueHost, reportObjectModel);
			}
			IList<NumericIndicatorRangeExprHost> numericIndicatorRangesHostsRemotable = ExprHost.NumericIndicatorRangesHostsRemotable;
			if (m_numericIndicatorRanges != null && numericIndicatorRangesHostsRemotable != null)
			{
				for (int i = 0; i < m_numericIndicatorRanges.Count; i++)
				{
					NumericIndicatorRange numericIndicatorRange = m_numericIndicatorRanges[i];
					if (numericIndicatorRange != null && numericIndicatorRange.ExpressionHostID > -1)
					{
						numericIndicatorRange.SetExprHost(numericIndicatorRangesHostsRemotable[numericIndicatorRange.ExpressionHostID], reportObjectModel);
					}
				}
			}
			if (m_minimumValue != null && ExprHost.MinimumValueHost != null)
			{
				m_minimumValue.SetExprHost(ExprHost.MinimumValueHost, reportObjectModel);
			}
			if (m_maximumValue != null && ExprHost.MaximumValueHost != null)
			{
				m_maximumValue.SetExprHost(ExprHost.MaximumValueHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> memberInfoList = new List<MemberInfo>();
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.NumericIndicator, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanelItem, memberInfoList);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.GaugeInputValue:
					writer.Write(m_gaugeInputValue);
					break;
				case MemberName.NumericIndicatorRanges:
					writer.Write(m_numericIndicatorRanges);
					break;
				case MemberName.DecimalDigitColor:
					writer.Write(m_decimalDigitColor);
					break;
				case MemberName.DigitColor:
					writer.Write(m_digitColor);
					break;
				case MemberName.UseFontPercent:
					writer.Write(m_useFontPercent);
					break;
				case MemberName.DecimalDigits:
					writer.Write(m_decimalDigits);
					break;
				case MemberName.Digits:
					writer.Write(m_digits);
					break;
				case MemberName.MinimumValue:
					writer.Write(m_minimumValue);
					break;
				case MemberName.MaximumValue:
					writer.Write(m_maximumValue);
					break;
				case MemberName.Multiplier:
					writer.Write(m_multiplier);
					break;
				case MemberName.NonNumericString:
					writer.Write(m_nonNumericString);
					break;
				case MemberName.OutOfRangeString:
					writer.Write(m_outOfRangeString);
					break;
				case MemberName.ResizeMode:
					writer.Write(m_resizeMode);
					break;
				case MemberName.ShowDecimalPoint:
					writer.Write(m_showDecimalPoint);
					break;
				case MemberName.ShowLeadingZeros:
					writer.Write(m_showLeadingZeros);
					break;
				case MemberName.IndicatorStyle:
					writer.Write(m_indicatorStyle);
					break;
				case MemberName.ShowSign:
					writer.Write(m_showSign);
					break;
				case MemberName.SnappingEnabled:
					writer.Write(m_snappingEnabled);
					break;
				case MemberName.SnappingInterval:
					writer.Write(m_snappingInterval);
					break;
				case MemberName.LedDimColor:
					writer.Write(m_ledDimColor);
					break;
				case MemberName.SeparatorWidth:
					writer.Write(m_separatorWidth);
					break;
				case MemberName.SeparatorColor:
					writer.Write(m_separatorColor);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.GaugeInputValue:
					m_gaugeInputValue = (GaugeInputValue)reader.ReadRIFObject();
					break;
				case MemberName.NumericIndicatorRanges:
					m_numericIndicatorRanges = reader.ReadGenericListOfRIFObjects<NumericIndicatorRange>();
					break;
				case MemberName.DecimalDigitColor:
					m_decimalDigitColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DigitColor:
					m_digitColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.UseFontPercent:
					m_useFontPercent = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DecimalDigits:
					m_decimalDigits = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Digits:
					m_digits = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MinimumValue:
					m_minimumValue = (GaugeInputValue)reader.ReadRIFObject();
					break;
				case MemberName.MaximumValue:
					m_maximumValue = (GaugeInputValue)reader.ReadRIFObject();
					break;
				case MemberName.Multiplier:
					m_multiplier = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.NonNumericString:
					m_nonNumericString = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.OutOfRangeString:
					m_outOfRangeString = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ResizeMode:
					m_resizeMode = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ShowDecimalPoint:
					m_showDecimalPoint = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ShowLeadingZeros:
					m_showLeadingZeros = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.IndicatorStyle:
					m_indicatorStyle = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ShowSign:
					m_showSign = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.SnappingEnabled:
					m_snappingEnabled = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.SnappingInterval:
					m_snappingInterval = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.LedDimColor:
					m_ledDimColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.SeparatorWidth:
					m_separatorWidth = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.SeparatorColor:
					m_separatorColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.NumericIndicator;
		}

		internal string EvaluateDecimalDigitColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateNumericIndicatorDecimalDigitColorExpression(this, m_gaugePanel.Name);
		}

		internal string EvaluateDigitColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateNumericIndicatorDigitColorExpression(this, m_gaugePanel.Name);
		}

		internal bool EvaluateUseFontPercent(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateNumericIndicatorUseFontPercentExpression(this, m_gaugePanel.Name);
		}

		internal int EvaluateDecimalDigits(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateNumericIndicatorDecimalDigitsExpression(this, m_gaugePanel.Name);
		}

		internal int EvaluateDigits(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateNumericIndicatorDigitsExpression(this, m_gaugePanel.Name);
		}

		internal double EvaluateMultiplier(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateNumericIndicatorMultiplierExpression(this, m_gaugePanel.Name);
		}

		internal string EvaluateNonNumericString(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateNumericIndicatorNonNumericStringExpression(this, m_gaugePanel.Name);
		}

		internal string EvaluateOutOfRangeString(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateNumericIndicatorOutOfRangeStringExpression(this, m_gaugePanel.Name);
		}

		internal GaugeResizeModes EvaluateResizeMode(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateGaugeResizeModes(context.ReportRuntime.EvaluateNumericIndicatorResizeModeExpression(this, m_gaugePanel.Name), context.ReportRuntime);
		}

		internal bool EvaluateShowDecimalPoint(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateNumericIndicatorShowDecimalPointExpression(this, m_gaugePanel.Name);
		}

		internal bool EvaluateShowLeadingZeros(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateNumericIndicatorShowLeadingZerosExpression(this, m_gaugePanel.Name);
		}

		internal GaugeIndicatorStyles EvaluateIndicatorStyle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateGaugeIndicatorStyles(context.ReportRuntime.EvaluateNumericIndicatorIndicatorStyleExpression(this, m_gaugePanel.Name), context.ReportRuntime);
		}

		internal GaugeShowSigns EvaluateShowSign(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateGaugeShowSigns(context.ReportRuntime.EvaluateNumericIndicatorShowSignExpression(this, m_gaugePanel.Name), context.ReportRuntime);
		}

		internal bool EvaluateSnappingEnabled(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateNumericIndicatorSnappingEnabledExpression(this, m_gaugePanel.Name);
		}

		internal double EvaluateSnappingInterval(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateNumericIndicatorSnappingIntervalExpression(this, m_gaugePanel.Name);
		}

		internal string EvaluateLedDimColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateNumericIndicatorLedDimColorExpression(this, m_gaugePanel.Name);
		}

		internal double EvaluateSeparatorWidth(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateNumericIndicatorSeparatorWidthExpression(this, m_gaugePanel.Name);
		}

		internal string EvaluateSeparatorColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateNumericIndicatorSeparatorColorExpression(this, m_gaugePanel.Name);
		}
	}
}
