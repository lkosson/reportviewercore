using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.RdlExpressions;
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
	internal sealed class GaugePanel : DataRegion, IPersistable
	{
		private List<LinearGauge> m_linearGauges;

		private List<RadialGauge> m_radialGauges;

		private List<NumericIndicator> m_numericIndicators;

		private List<StateIndicator> m_stateIndicators;

		private List<GaugeImage> m_gaugeImages;

		private List<GaugeLabel> m_gaugeLabels;

		private ExpressionInfo m_antiAliasing;

		private ExpressionInfo m_autoLayout;

		private BackFrame m_backFrame;

		private ExpressionInfo m_shadowIntensity;

		private ExpressionInfo m_textAntiAliasingQuality;

		private TopImage m_topImage;

		private GaugeMemberList m_columnMembers;

		private GaugeMemberList m_rowMembers;

		private GaugeRowList m_rows;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private GaugePanelExprHost m_exprHost;

		[NonSerialized]
		private int m_actionOwnerCounter;

		internal override Microsoft.ReportingServices.ReportProcessing.ObjectType ObjectType => Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel;

		internal override HierarchyNodeList ColumnMembers => m_columnMembers;

		internal override HierarchyNodeList RowMembers => m_rowMembers;

		internal override RowList Rows => m_rows;

		internal GaugeMember GaugeMember
		{
			get
			{
				if (m_columnMembers != null && m_columnMembers.Count > 0)
				{
					return m_columnMembers[0];
				}
				return null;
			}
			set
			{
				if (m_columnMembers == null)
				{
					m_columnMembers = new GaugeMemberList();
				}
				else
				{
					m_columnMembers.Clear();
				}
				m_columnMembers.Add(value);
			}
		}

		internal GaugeMember GaugeRowMember
		{
			get
			{
				if (m_rowMembers != null && m_rowMembers.Count == 1)
				{
					return m_rowMembers[0];
				}
				return null;
			}
			set
			{
				if (m_rowMembers == null)
				{
					m_rowMembers = new GaugeMemberList();
				}
				else
				{
					m_rowMembers.Clear();
				}
				m_rowMembers.Add(value);
			}
		}

		internal GaugeRow GaugeRow
		{
			get
			{
				if (m_rows != null && m_rows.Count > 0)
				{
					return m_rows[0];
				}
				return null;
			}
			set
			{
				if (m_rows == null)
				{
					m_rows = new GaugeRowList();
				}
				else
				{
					m_rows.Clear();
				}
				m_rows.Add(value);
			}
		}

		internal List<LinearGauge> LinearGauges
		{
			get
			{
				return m_linearGauges;
			}
			set
			{
				m_linearGauges = value;
			}
		}

		internal List<RadialGauge> RadialGauges
		{
			get
			{
				return m_radialGauges;
			}
			set
			{
				m_radialGauges = value;
			}
		}

		internal List<NumericIndicator> NumericIndicators
		{
			get
			{
				return m_numericIndicators;
			}
			set
			{
				m_numericIndicators = value;
			}
		}

		internal List<StateIndicator> StateIndicators
		{
			get
			{
				return m_stateIndicators;
			}
			set
			{
				m_stateIndicators = value;
			}
		}

		internal List<GaugeImage> GaugeImages
		{
			get
			{
				return m_gaugeImages;
			}
			set
			{
				m_gaugeImages = value;
			}
		}

		internal List<GaugeLabel> GaugeLabels
		{
			get
			{
				return m_gaugeLabels;
			}
			set
			{
				m_gaugeLabels = value;
			}
		}

		internal ExpressionInfo AntiAliasing
		{
			get
			{
				return m_antiAliasing;
			}
			set
			{
				m_antiAliasing = value;
			}
		}

		internal ExpressionInfo AutoLayout
		{
			get
			{
				return m_autoLayout;
			}
			set
			{
				m_autoLayout = value;
			}
		}

		internal BackFrame BackFrame
		{
			get
			{
				return m_backFrame;
			}
			set
			{
				m_backFrame = value;
			}
		}

		internal ExpressionInfo ShadowIntensity
		{
			get
			{
				return m_shadowIntensity;
			}
			set
			{
				m_shadowIntensity = value;
			}
		}

		internal ExpressionInfo TextAntiAliasingQuality
		{
			get
			{
				return m_textAntiAliasingQuality;
			}
			set
			{
				m_textAntiAliasingQuality = value;
			}
		}

		internal TopImage TopImage
		{
			get
			{
				return m_topImage;
			}
			set
			{
				m_topImage = value;
			}
		}

		internal GaugePanelExprHost GaugePanelExprHost => m_exprHost;

		protected override IndexedExprHost UserSortExpressionsHost
		{
			get
			{
				if (m_exprHost == null)
				{
					return null;
				}
				return m_exprHost.UserSortExpressionsHost;
			}
		}

		internal GaugePanel(ReportItem parent)
			: base(parent)
		{
		}

		internal GaugePanel(int id, ReportItem parent)
			: base(id, parent)
		{
			base.RowCount = 1;
			base.ColumnCount = 1;
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.ObjectType = ObjectType;
			context.ObjectName = m_name;
			if ((context.Location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDetail) != 0 && (context.Location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InGrouping) == 0)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsDataRegionInDetailList, Severity.Error, context.ObjectType, context.ObjectName, null);
			}
			else
			{
				if (!context.RegisterDataRegion(this))
				{
					return false;
				}
				context.Location |= (Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDataSet | Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDataRegion);
				context.ExprHostBuilder.DataRegionStart(Microsoft.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.GaugePanel, m_name);
				base.Initialize(context);
				base.ExprHostID = context.ExprHostBuilder.DataRegionEnd(Microsoft.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.GaugePanel);
				context.UnRegisterDataRegion(this);
			}
			return false;
		}

		protected override void InitializeCorner(InitializationContext context)
		{
			if (GaugeRow != null)
			{
				GaugeRow.Initialize(context);
			}
			if (m_linearGauges != null)
			{
				for (int i = 0; i < m_linearGauges.Count; i++)
				{
					m_linearGauges[i].Initialize(context);
				}
			}
			if (m_radialGauges != null)
			{
				for (int j = 0; j < m_radialGauges.Count; j++)
				{
					m_radialGauges[j].Initialize(context);
				}
			}
			if (m_numericIndicators != null)
			{
				for (int k = 0; k < m_numericIndicators.Count; k++)
				{
					m_numericIndicators[k].Initialize(context);
				}
			}
			if (m_stateIndicators != null)
			{
				for (int l = 0; l < m_stateIndicators.Count; l++)
				{
					m_stateIndicators[l].Initialize(context);
				}
			}
			if (m_gaugeImages != null)
			{
				for (int m = 0; m < m_gaugeImages.Count; m++)
				{
					m_gaugeImages[m].Initialize(context);
				}
			}
			if (m_gaugeLabels != null)
			{
				for (int n = 0; n < m_gaugeLabels.Count; n++)
				{
					m_gaugeLabels[n].Initialize(context);
				}
			}
			if (m_antiAliasing != null)
			{
				m_antiAliasing.Initialize("AntiAliasing", context);
				context.ExprHostBuilder.GaugePanelAntiAliasing(m_antiAliasing);
			}
			if (m_autoLayout != null)
			{
				m_autoLayout.Initialize("AutoLayout", context);
				context.ExprHostBuilder.GaugePanelAutoLayout(m_autoLayout);
			}
			if (m_backFrame != null)
			{
				m_backFrame.Initialize(context);
			}
			if (m_shadowIntensity != null)
			{
				m_shadowIntensity.Initialize("ShadowIntensity", context);
				context.ExprHostBuilder.GaugePanelShadowIntensity(m_shadowIntensity);
			}
			if (m_textAntiAliasingQuality != null)
			{
				m_textAntiAliasingQuality.Initialize("TextAntiAliasingQuality", context);
				context.ExprHostBuilder.GaugePanelTextAntiAliasingQuality(m_textAntiAliasingQuality);
			}
			if (m_topImage != null)
			{
				m_topImage.Initialize(context);
			}
		}

		protected override bool ValidateInnerStructure(InitializationContext context)
		{
			return true;
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			GaugePanel gaugePanel = (GaugePanel)(context.CurrentDataRegionClone = (GaugePanel)base.PublishClone(context));
			gaugePanel.m_rows = new GaugeRowList();
			gaugePanel.m_rowMembers = new GaugeMemberList();
			gaugePanel.m_columnMembers = new GaugeMemberList();
			if (GaugeMember != null)
			{
				gaugePanel.GaugeMember = (GaugeMember)GaugeMember.PublishClone(context, gaugePanel);
			}
			if (GaugeRowMember != null)
			{
				gaugePanel.GaugeRowMember = (GaugeMember)GaugeRowMember.PublishClone(context);
			}
			if (GaugeRow != null)
			{
				gaugePanel.GaugeRow = (GaugeRow)GaugeRow.PublishClone(context);
			}
			if (m_linearGauges != null)
			{
				gaugePanel.m_linearGauges = new List<LinearGauge>(m_linearGauges.Count);
				foreach (LinearGauge linearGauge in m_linearGauges)
				{
					gaugePanel.m_linearGauges.Add((LinearGauge)linearGauge.PublishClone(context));
				}
			}
			if (m_radialGauges != null)
			{
				gaugePanel.m_radialGauges = new List<RadialGauge>(m_radialGauges.Count);
				foreach (RadialGauge radialGauge in m_radialGauges)
				{
					gaugePanel.m_radialGauges.Add((RadialGauge)radialGauge.PublishClone(context));
				}
			}
			if (m_numericIndicators != null)
			{
				gaugePanel.m_numericIndicators = new List<NumericIndicator>(m_numericIndicators.Count);
				foreach (NumericIndicator numericIndicator in m_numericIndicators)
				{
					gaugePanel.m_numericIndicators.Add((NumericIndicator)numericIndicator.PublishClone(context));
				}
			}
			if (m_stateIndicators != null)
			{
				gaugePanel.m_stateIndicators = new List<StateIndicator>(m_stateIndicators.Count);
				foreach (StateIndicator stateIndicator in m_stateIndicators)
				{
					gaugePanel.m_stateIndicators.Add((StateIndicator)stateIndicator.PublishClone(context));
				}
			}
			if (m_gaugeImages != null)
			{
				gaugePanel.m_gaugeImages = new List<GaugeImage>(m_gaugeImages.Count);
				foreach (GaugeImage gaugeImage in m_gaugeImages)
				{
					gaugePanel.m_gaugeImages.Add((GaugeImage)gaugeImage.PublishClone(context));
				}
			}
			if (m_gaugeLabels != null)
			{
				gaugePanel.m_gaugeLabels = new List<GaugeLabel>(m_gaugeLabels.Count);
				foreach (GaugeLabel gaugeLabel in m_gaugeLabels)
				{
					gaugePanel.m_gaugeLabels.Add((GaugeLabel)gaugeLabel.PublishClone(context));
				}
			}
			if (m_antiAliasing != null)
			{
				gaugePanel.m_antiAliasing = (ExpressionInfo)m_antiAliasing.PublishClone(context);
			}
			if (m_autoLayout != null)
			{
				gaugePanel.m_autoLayout = (ExpressionInfo)m_autoLayout.PublishClone(context);
			}
			if (m_backFrame != null)
			{
				gaugePanel.m_backFrame = (BackFrame)m_backFrame.PublishClone(context);
			}
			if (m_shadowIntensity != null)
			{
				gaugePanel.m_shadowIntensity = (ExpressionInfo)m_shadowIntensity.PublishClone(context);
			}
			if (m_textAntiAliasingQuality != null)
			{
				gaugePanel.m_textAntiAliasingQuality = (ExpressionInfo)m_textAntiAliasingQuality.PublishClone(context);
			}
			if (m_topImage != null)
			{
				gaugePanel.m_topImage = (TopImage)m_topImage.PublishClone(context);
			}
			return gaugePanel;
		}

		internal GaugeAntiAliasings EvaluateAntiAliasing(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			return EnumTranslator.TranslateGaugeAntiAliasings(context.ReportRuntime.EvaluateGaugePanelAntiAliasingExpression(this, base.Name), context.ReportRuntime);
		}

		internal bool EvaluateAutoLayout(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugePanelAutoLayoutExpression(this, base.Name);
		}

		internal double EvaluateShadowIntensity(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugePanelShadowIntensityExpression(this, base.Name);
		}

		internal TextAntiAliasingQualities EvaluateTextAntiAliasingQuality(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			return EnumTranslator.TranslateTextAntiAliasingQualities(context.ReportRuntime.EvaluateGaugePanelTextAntiAliasingQualityExpression(this, base.Name), context.ReportRuntime);
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new ReadOnlyMemberInfo(MemberName.GaugeMember, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeMember));
			list.Add(new ReadOnlyMemberInfo(MemberName.GaugeRowMember, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeMember));
			list.Add(new ReadOnlyMemberInfo(MemberName.GaugeRow, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeRow));
			list.Add(new MemberInfo(MemberName.LinearGauges, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LinearGauge));
			list.Add(new MemberInfo(MemberName.RadialGauges, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RadialGauge));
			list.Add(new MemberInfo(MemberName.NumericIndicators, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.NumericIndicator));
			list.Add(new MemberInfo(MemberName.StateIndicators, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StateIndicator));
			list.Add(new MemberInfo(MemberName.GaugeImages, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeImage));
			list.Add(new MemberInfo(MemberName.GaugeLabels, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeLabel));
			list.Add(new MemberInfo(MemberName.AntiAliasing, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.AutoLayout, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.BackFrame, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BackFrame));
			list.Add(new MemberInfo(MemberName.ShadowIntensity, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.TextAntiAliasingQuality, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.TopImage, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TopImage));
			list.Add(new MemberInfo(MemberName.ColumnMembers, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeMember));
			list.Add(new MemberInfo(MemberName.RowMembers, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeMember));
			list.Add(new MemberInfo(MemberName.Rows, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeRow));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanel, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegion, list);
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
			if (NumericIndicators != null)
			{
				foreach (NumericIndicator numericIndicator in NumericIndicators)
				{
					if (numericIndicator.GaugeInputValue != null)
					{
						list.Add(numericIndicator.GaugeInputValue);
					}
					if (numericIndicator.MaximumValue != null)
					{
						list.Add(numericIndicator.MaximumValue);
					}
					if (numericIndicator.MinimumValue != null)
					{
						list.Add(numericIndicator.MinimumValue);
					}
					if (numericIndicator.NumericIndicatorRanges == null)
					{
						continue;
					}
					foreach (NumericIndicatorRange numericIndicatorRange in numericIndicator.NumericIndicatorRanges)
					{
						if (numericIndicatorRange.StartValue != null)
						{
							list.Add(numericIndicatorRange.StartValue);
						}
						if (numericIndicatorRange.EndValue != null)
						{
							list.Add(numericIndicatorRange.EndValue);
						}
					}
				}
			}
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

		internal int GenerateActionOwnerID()
		{
			return ++m_actionOwnerCounter;
		}

		public override void CreateDomainScopeMember(ReportHierarchyNode parentNode, Grouping grouping, AutomaticSubtotalContext context)
		{
			GaugeMember gaugeMember = new GaugeMember(context.GenerateID(), this);
			gaugeMember.Grouping = grouping.CloneForDomainScope(context, gaugeMember);
			HierarchyNodeList hierarchyNodeList = (parentNode != null) ? parentNode.InnerHierarchy : ColumnMembers;
			if (hierarchyNodeList != null)
			{
				hierarchyNodeList.Add(gaugeMember);
				gaugeMember.IsColumn = true;
				GaugeRow.Cells.Insert(ColumnMembers.GetMemberIndex(gaugeMember), new GaugeCell(context.GenerateID(), this));
				base.ColumnCount++;
			}
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.LinearGauges:
					writer.Write(m_linearGauges);
					break;
				case MemberName.RadialGauges:
					writer.Write(m_radialGauges);
					break;
				case MemberName.NumericIndicators:
					writer.Write(m_numericIndicators);
					break;
				case MemberName.StateIndicators:
					writer.Write(m_stateIndicators);
					break;
				case MemberName.GaugeImages:
					writer.Write(m_gaugeImages);
					break;
				case MemberName.GaugeLabels:
					writer.Write(m_gaugeLabels);
					break;
				case MemberName.AntiAliasing:
					writer.Write(m_antiAliasing);
					break;
				case MemberName.AutoLayout:
					writer.Write(m_autoLayout);
					break;
				case MemberName.BackFrame:
					writer.Write(m_backFrame);
					break;
				case MemberName.ShadowIntensity:
					writer.Write(m_shadowIntensity);
					break;
				case MemberName.TextAntiAliasingQuality:
					writer.Write(m_textAntiAliasingQuality);
					break;
				case MemberName.TopImage:
					writer.Write(m_topImage);
					break;
				case MemberName.ColumnMembers:
					writer.Write(m_columnMembers);
					break;
				case MemberName.RowMembers:
					writer.Write(m_rowMembers);
					break;
				case MemberName.Rows:
					writer.Write(m_rows);
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
				case MemberName.GaugeMember:
					GaugeMember = (GaugeMember)reader.ReadRIFObject();
					break;
				case MemberName.GaugeRowMember:
					GaugeRowMember = (GaugeMember)reader.ReadRIFObject();
					break;
				case MemberName.GaugeRow:
					GaugeRow = (GaugeRow)reader.ReadRIFObject();
					break;
				case MemberName.LinearGauges:
					m_linearGauges = reader.ReadGenericListOfRIFObjects<LinearGauge>();
					break;
				case MemberName.RadialGauges:
					m_radialGauges = reader.ReadGenericListOfRIFObjects<RadialGauge>();
					break;
				case MemberName.NumericIndicators:
					m_numericIndicators = reader.ReadGenericListOfRIFObjects<NumericIndicator>();
					break;
				case MemberName.StateIndicators:
					m_stateIndicators = reader.ReadGenericListOfRIFObjects<StateIndicator>();
					break;
				case MemberName.GaugeImages:
					m_gaugeImages = reader.ReadGenericListOfRIFObjects<GaugeImage>();
					break;
				case MemberName.GaugeLabels:
					m_gaugeLabels = reader.ReadGenericListOfRIFObjects<GaugeLabel>();
					break;
				case MemberName.AntiAliasing:
					m_antiAliasing = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.AutoLayout:
					m_autoLayout = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.BackFrame:
					m_backFrame = (BackFrame)reader.ReadRIFObject();
					break;
				case MemberName.ShadowIntensity:
					m_shadowIntensity = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.TextAntiAliasingQuality:
					m_textAntiAliasingQuality = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.TopImage:
					m_topImage = (TopImage)reader.ReadRIFObject();
					break;
				case MemberName.ColumnMembers:
					m_columnMembers = reader.ReadListOfRIFObjects<GaugeMemberList>();
					break;
				case MemberName.RowMembers:
					m_rowMembers = reader.ReadListOfRIFObjects<GaugeMemberList>();
					break;
				case MemberName.Rows:
					m_rows = reader.ReadListOfRIFObjects<GaugeRowList>();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanel;
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null);
				m_exprHost = reportExprHost.GaugePanelHostsRemotable[base.ExprHostID];
				DataRegionSetExprHost(m_exprHost, m_exprHost.SortHost, m_exprHost.FilterHostsRemotable, m_exprHost.UserSortExpressionsHost, m_exprHost.PageBreakExprHost, m_exprHost.JoinConditionExprHostsRemotable, reportObjectModel);
			}
		}

		internal override void DataRegionContentsSetExprHost(ObjectModelImpl reportObjectModel, bool traverseDataRegions)
		{
			if (m_exprHost == null)
			{
				return;
			}
			IList<LinearGaugeExprHost> linearGaugesHostsRemotable = m_exprHost.LinearGaugesHostsRemotable;
			if (m_linearGauges != null && linearGaugesHostsRemotable != null)
			{
				for (int i = 0; i < m_linearGauges.Count; i++)
				{
					LinearGauge linearGauge = m_linearGauges[i];
					if (linearGauge != null && linearGauge.ExpressionHostID > -1)
					{
						linearGauge.SetExprHost(linearGaugesHostsRemotable[linearGauge.ExpressionHostID], reportObjectModel);
					}
				}
			}
			IList<RadialGaugeExprHost> radialGaugesHostsRemotable = m_exprHost.RadialGaugesHostsRemotable;
			if (m_radialGauges != null && radialGaugesHostsRemotable != null)
			{
				for (int j = 0; j < m_radialGauges.Count; j++)
				{
					RadialGauge radialGauge = m_radialGauges[j];
					if (radialGauge != null && radialGauge.ExpressionHostID > -1)
					{
						radialGauge.SetExprHost(radialGaugesHostsRemotable[radialGauge.ExpressionHostID], reportObjectModel);
					}
				}
			}
			IList<NumericIndicatorExprHost> numericIndicatorsHostsRemotable = m_exprHost.NumericIndicatorsHostsRemotable;
			if (m_numericIndicators != null && numericIndicatorsHostsRemotable != null)
			{
				for (int k = 0; k < m_numericIndicators.Count; k++)
				{
					NumericIndicator numericIndicator = m_numericIndicators[k];
					if (numericIndicator != null && numericIndicator.ExpressionHostID > -1)
					{
						numericIndicator.SetExprHost(numericIndicatorsHostsRemotable[numericIndicator.ExpressionHostID], reportObjectModel);
					}
				}
			}
			IList<StateIndicatorExprHost> stateIndicatorsHostsRemotable = m_exprHost.StateIndicatorsHostsRemotable;
			if (m_stateIndicators != null && stateIndicatorsHostsRemotable != null)
			{
				for (int l = 0; l < m_stateIndicators.Count; l++)
				{
					StateIndicator stateIndicator = m_stateIndicators[l];
					if (stateIndicator != null && stateIndicator.ExpressionHostID > -1)
					{
						stateIndicator.SetExprHost(stateIndicatorsHostsRemotable[stateIndicator.ExpressionHostID], reportObjectModel);
					}
				}
			}
			IList<GaugeImageExprHost> gaugeImagesHostsRemotable = m_exprHost.GaugeImagesHostsRemotable;
			if (m_gaugeImages != null && gaugeImagesHostsRemotable != null)
			{
				for (int m = 0; m < m_gaugeImages.Count; m++)
				{
					GaugeImage gaugeImage = m_gaugeImages[m];
					if (gaugeImage != null && gaugeImage.ExpressionHostID > -1)
					{
						gaugeImage.SetExprHost(gaugeImagesHostsRemotable[gaugeImage.ExpressionHostID], reportObjectModel);
					}
				}
			}
			IList<GaugeLabelExprHost> gaugeLabelsHostsRemotable = m_exprHost.GaugeLabelsHostsRemotable;
			if (m_gaugeLabels != null && gaugeLabelsHostsRemotable != null)
			{
				for (int n = 0; n < m_gaugeLabels.Count; n++)
				{
					GaugeLabel gaugeLabel = m_gaugeLabels[n];
					if (gaugeLabel != null && gaugeLabel.ExpressionHostID > -1)
					{
						gaugeLabel.SetExprHost(gaugeLabelsHostsRemotable[gaugeLabel.ExpressionHostID], reportObjectModel);
					}
				}
			}
			if (m_backFrame != null && m_exprHost.BackFrameHost != null)
			{
				m_backFrame.SetExprHost(m_exprHost.BackFrameHost, reportObjectModel);
			}
			if (m_topImage != null && m_exprHost.TopImageHost != null)
			{
				m_topImage.SetExprHost(m_exprHost.TopImageHost, reportObjectModel);
			}
			IList<GaugeCellExprHost> cellHostsRemotable = m_exprHost.CellHostsRemotable;
			if (cellHostsRemotable != null && GaugeRow != null && cellHostsRemotable.Count > 0 && GaugeRow.GaugeCell != null)
			{
				GaugeRow.GaugeCell.SetExprHost(cellHostsRemotable[0], reportObjectModel);
			}
		}

		internal override object EvaluateNoRowsMessageExpression()
		{
			return m_exprHost.NoRowsExpr;
		}
	}
}
