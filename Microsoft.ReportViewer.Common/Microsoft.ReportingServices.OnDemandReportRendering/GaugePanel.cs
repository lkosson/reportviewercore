using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class GaugePanel : DataRegion
	{
		private enum CompilationState
		{
			NotCompiled,
			Compiling,
			Compiled
		}

		private GaugeMember m_gaugeMember;

		private GaugeMember m_gaugeRowMember;

		private GaugeRowCollection m_gaugeRowCollection;

		private LinearGaugeCollection m_linearGauges;

		private RadialGaugeCollection m_radialGauges;

		private NumericIndicatorCollection m_numericIndicators;

		private StateIndicatorCollection m_stateIndicators;

		private GaugeImageCollection m_gaugeImages;

		private GaugeLabelCollection m_gaugeLabels;

		private ReportEnumProperty<GaugeAntiAliasings> m_antiAliasing;

		private ReportBoolProperty m_autoLayout;

		private BackFrame m_backFrame;

		private ReportDoubleProperty m_shadowIntensity;

		private ReportEnumProperty<TextAntiAliasingQualities> m_textAntiAliasingQuality;

		private TopImage m_topImage;

		private CompilationState m_compilationState;

		public GaugeMember GaugeMember
		{
			get
			{
				if (m_gaugeMember == null)
				{
					m_gaugeMember = new GaugeMember(ReportScope, this, this, null, GaugePanelDef.GaugeMember);
				}
				return m_gaugeMember;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel GaugePanelDef => m_reportItemDef as Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel;

		internal override bool HasDataCells => true;

		internal override IDataRegionRowCollection RowCollection
		{
			get
			{
				if (m_gaugeRowCollection == null && GaugePanelDef.Rows != null)
				{
					m_gaugeRowCollection = new GaugeRowCollection(this, (GaugeRowList)GaugePanelDef.Rows);
				}
				return m_gaugeRowCollection;
			}
		}

		public LinearGaugeCollection LinearGauges
		{
			get
			{
				if (m_linearGauges == null && GaugePanelDef.LinearGauges != null)
				{
					m_linearGauges = new LinearGaugeCollection(this);
				}
				return m_linearGauges;
			}
		}

		public RadialGaugeCollection RadialGauges
		{
			get
			{
				if (m_radialGauges == null && GaugePanelDef.RadialGauges != null)
				{
					m_radialGauges = new RadialGaugeCollection(this);
				}
				return m_radialGauges;
			}
		}

		public NumericIndicatorCollection NumericIndicators
		{
			get
			{
				if (m_numericIndicators == null && GaugePanelDef.NumericIndicators != null)
				{
					m_numericIndicators = new NumericIndicatorCollection(this);
				}
				return m_numericIndicators;
			}
		}

		public StateIndicatorCollection StateIndicators
		{
			get
			{
				if (m_stateIndicators == null && GaugePanelDef.StateIndicators != null)
				{
					m_stateIndicators = new StateIndicatorCollection(this);
				}
				return m_stateIndicators;
			}
		}

		public GaugeImageCollection GaugeImages
		{
			get
			{
				if (m_gaugeImages == null && GaugePanelDef.GaugeImages != null)
				{
					m_gaugeImages = new GaugeImageCollection(this);
				}
				return m_gaugeImages;
			}
		}

		public GaugeLabelCollection GaugeLabels
		{
			get
			{
				if (m_gaugeLabels == null && GaugePanelDef.GaugeLabels != null)
				{
					m_gaugeLabels = new GaugeLabelCollection(this);
				}
				return m_gaugeLabels;
			}
		}

		public ReportEnumProperty<GaugeAntiAliasings> AntiAliasing
		{
			get
			{
				if (m_antiAliasing == null && GaugePanelDef.AntiAliasing != null)
				{
					m_antiAliasing = new ReportEnumProperty<GaugeAntiAliasings>(GaugePanelDef.AntiAliasing.IsExpression, GaugePanelDef.AntiAliasing.OriginalText, EnumTranslator.TranslateGaugeAntiAliasings(GaugePanelDef.AntiAliasing.StringValue, null));
				}
				return m_antiAliasing;
			}
		}

		public ReportBoolProperty AutoLayout
		{
			get
			{
				if (m_autoLayout == null && GaugePanelDef.AutoLayout != null)
				{
					m_autoLayout = new ReportBoolProperty(GaugePanelDef.AutoLayout);
				}
				return m_autoLayout;
			}
		}

		public BackFrame BackFrame
		{
			get
			{
				if (m_backFrame == null && GaugePanelDef.BackFrame != null)
				{
					m_backFrame = new BackFrame(GaugePanelDef.BackFrame, this);
				}
				return m_backFrame;
			}
		}

		public ReportDoubleProperty ShadowIntensity
		{
			get
			{
				if (m_shadowIntensity == null && GaugePanelDef.ShadowIntensity != null)
				{
					m_shadowIntensity = new ReportDoubleProperty(GaugePanelDef.ShadowIntensity);
				}
				return m_shadowIntensity;
			}
		}

		public ReportEnumProperty<TextAntiAliasingQualities> TextAntiAliasingQuality
		{
			get
			{
				if (m_textAntiAliasingQuality == null && GaugePanelDef.TextAntiAliasingQuality != null)
				{
					m_textAntiAliasingQuality = new ReportEnumProperty<TextAntiAliasingQualities>(GaugePanelDef.TextAntiAliasingQuality.IsExpression, GaugePanelDef.TextAntiAliasingQuality.OriginalText, EnumTranslator.TranslateTextAntiAliasingQualities(GaugePanelDef.TextAntiAliasingQuality.StringValue, null));
				}
				return m_textAntiAliasingQuality;
			}
		}

		public TopImage TopImage
		{
			get
			{
				if (m_topImage == null && GaugePanelDef.TopImage != null)
				{
					m_topImage = new TopImage(GaugePanelDef.TopImage, this);
				}
				return m_topImage;
			}
		}

		public new GaugePanelInstance Instance => (GaugePanelInstance)GetOrCreateInstance();

		private bool RequiresCompilation
		{
			get
			{
				if (GaugeMember.IsStatic)
				{
					return StateIndicators != null;
				}
				return true;
			}
		}

		internal GaugePanel(IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel reportItemDef, RenderingContext renderingContext)
			: base(parentDefinitionPath, indexIntoParentCollectionDef, reportItemDef, renderingContext)
		{
		}

		internal override ReportItemInstance GetOrCreateInstance()
		{
			if (m_instance == null)
			{
				m_instance = new GaugePanelInstance(this);
			}
			return m_instance;
		}

		internal override void SetNewContextChildren()
		{
			if (m_gaugeMember != null)
			{
				m_gaugeMember.ResetContext();
			}
			if (m_gaugeRowMember != null)
			{
				m_gaugeRowMember.ResetContext();
			}
			if (m_gaugeRowCollection != null)
			{
				m_gaugeRowCollection.SetNewContext();
			}
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_linearGauges != null)
			{
				m_linearGauges.SetNewContext();
			}
			if (m_radialGauges != null)
			{
				m_radialGauges.SetNewContext();
			}
			if (m_numericIndicators != null)
			{
				m_numericIndicators.SetNewContext();
			}
			if (m_stateIndicators != null)
			{
				m_stateIndicators.SetNewContext();
			}
			if (m_gaugeImages != null)
			{
				m_gaugeImages.SetNewContext();
			}
			if (m_gaugeLabels != null)
			{
				m_gaugeLabels.SetNewContext();
			}
			if (m_backFrame != null)
			{
				m_backFrame.SetNewContext();
			}
			if (m_topImage != null)
			{
				m_topImage.SetNewContext();
			}
			m_compilationState = CompilationState.NotCompiled;
		}

		internal List<GaugeInputValue> GetGaugeInputValues()
		{
			List<GaugeInputValue> list = new List<GaugeInputValue>();
			if (RadialGauges != null)
			{
				foreach (RadialGauge radialGauge in RadialGauges)
				{
					if (radialGauge.GaugeScales == null)
					{
						continue;
					}
					foreach (RadialScale gaugeScale in radialGauge.GaugeScales)
					{
						if (gaugeScale.MaximumValue != null)
						{
							list.Add(gaugeScale.MaximumValue);
						}
						if (gaugeScale.MinimumValue != null)
						{
							list.Add(gaugeScale.MinimumValue);
						}
						if (gaugeScale.GaugePointers != null)
						{
							foreach (RadialPointer gaugePointer in gaugeScale.GaugePointers)
							{
								if (gaugePointer.GaugeInputValue != null)
								{
									list.Add(gaugePointer.GaugeInputValue);
								}
							}
						}
						if (gaugeScale.ScaleRanges == null)
						{
							continue;
						}
						foreach (ScaleRange scaleRange in gaugeScale.ScaleRanges)
						{
							if (scaleRange.StartValue != null)
							{
								list.Add(scaleRange.StartValue);
							}
							if (scaleRange.EndValue != null)
							{
								list.Add(scaleRange.EndValue);
							}
						}
					}
				}
			}
			if (LinearGauges != null)
			{
				foreach (LinearGauge linearGauge in LinearGauges)
				{
					if (linearGauge.GaugeScales == null)
					{
						continue;
					}
					foreach (LinearScale gaugeScale2 in linearGauge.GaugeScales)
					{
						if (gaugeScale2.MaximumValue != null)
						{
							list.Add(gaugeScale2.MaximumValue);
						}
						if (gaugeScale2.MinimumValue != null)
						{
							list.Add(gaugeScale2.MinimumValue);
						}
						if (gaugeScale2.GaugePointers != null)
						{
							foreach (LinearPointer gaugePointer2 in gaugeScale2.GaugePointers)
							{
								if (gaugePointer2.GaugeInputValue != null)
								{
									list.Add(gaugePointer2.GaugeInputValue);
								}
							}
						}
						if (gaugeScale2.ScaleRanges == null)
						{
							continue;
						}
						foreach (ScaleRange scaleRange2 in gaugeScale2.ScaleRanges)
						{
							if (scaleRange2.StartValue != null)
							{
								list.Add(scaleRange2.StartValue);
							}
							if (scaleRange2.EndValue != null)
							{
								list.Add(scaleRange2.EndValue);
							}
						}
					}
				}
			}
			_ = NumericIndicators;
			if (StateIndicators != null)
			{
				foreach (StateIndicator stateIndicator in StateIndicators)
				{
					if (stateIndicator.GaugeInputValue != null)
					{
						list.Add(stateIndicator.GaugeInputValue);
					}
					if (stateIndicator.MinimumValue != null)
					{
						list.Add(stateIndicator.MinimumValue);
					}
					if (stateIndicator.MaximumValue != null)
					{
						list.Add(stateIndicator.MaximumValue);
					}
					if (stateIndicator.IndicatorStates != null)
					{
						foreach (IndicatorState indicatorState in stateIndicator.IndicatorStates)
						{
							if (indicatorState.StartValue != null)
							{
								list.Add(indicatorState.StartValue);
							}
							if (indicatorState.EndValue != null)
							{
								list.Add(indicatorState.EndValue);
							}
						}
					}
				}
				return list;
			}
			return list;
		}

		internal void ProcessCompiledInstances()
		{
			if (RequiresCompilation && m_compilationState == CompilationState.NotCompiled)
			{
				try
				{
					m_compilationState = CompilationState.Compiling;
					GaugeMapperFactory.CreateGaugeMapperInstance(this, base.RenderingContext.OdpContext.ReportDefinition.DefaultFontFamily).RenderDataGaugePanel();
					m_compilationState = CompilationState.Compiled;
				}
				catch (Exception innerException)
				{
					m_compilationState = CompilationState.NotCompiled;
					throw new RenderingObjectModelException(innerException);
				}
			}
		}
	}
}
