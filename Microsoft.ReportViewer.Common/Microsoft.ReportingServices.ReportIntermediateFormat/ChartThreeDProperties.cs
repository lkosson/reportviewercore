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
	internal sealed class ChartThreeDProperties : IPersistable
	{
		private ExpressionInfo m_enabled;

		private ExpressionInfo m_projectionMode;

		private ExpressionInfo m_rotation;

		private ExpressionInfo m_inclination;

		private ExpressionInfo m_perspective;

		private ExpressionInfo m_depthRatio;

		private ExpressionInfo m_shading;

		private ExpressionInfo m_gapDepth;

		private ExpressionInfo m_wallThickness;

		private ExpressionInfo m_clustered;

		[Reference]
		private Chart m_chart;

		[NonSerialized]
		private Chart3DPropertiesExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal ExpressionInfo Enabled
		{
			get
			{
				return m_enabled;
			}
			set
			{
				m_enabled = value;
			}
		}

		internal ExpressionInfo ProjectionMode
		{
			get
			{
				return m_projectionMode;
			}
			set
			{
				m_projectionMode = value;
			}
		}

		internal ExpressionInfo Rotation
		{
			get
			{
				return m_rotation;
			}
			set
			{
				m_rotation = value;
			}
		}

		internal ExpressionInfo Inclination
		{
			get
			{
				return m_inclination;
			}
			set
			{
				m_inclination = value;
			}
		}

		internal ExpressionInfo Perspective
		{
			get
			{
				return m_perspective;
			}
			set
			{
				m_perspective = value;
			}
		}

		internal ExpressionInfo DepthRatio
		{
			get
			{
				return m_depthRatio;
			}
			set
			{
				m_depthRatio = value;
			}
		}

		internal ExpressionInfo Shading
		{
			get
			{
				return m_shading;
			}
			set
			{
				m_shading = value;
			}
		}

		internal ExpressionInfo GapDepth
		{
			get
			{
				return m_gapDepth;
			}
			set
			{
				m_gapDepth = value;
			}
		}

		internal ExpressionInfo WallThickness
		{
			get
			{
				return m_wallThickness;
			}
			set
			{
				m_wallThickness = value;
			}
		}

		internal ExpressionInfo Clustered
		{
			get
			{
				return m_clustered;
			}
			set
			{
				m_clustered = value;
			}
		}

		internal Chart3DPropertiesExprHost ExprHost => m_exprHost;

		internal ChartThreeDProperties()
		{
		}

		internal ChartThreeDProperties(Chart chart)
		{
			m_chart = chart;
		}

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.Chart3DPropertiesStart();
			if (m_enabled != null)
			{
				m_enabled.Initialize("Rotation", context);
				context.ExprHostBuilder.Chart3DPropertiesEnabled(m_enabled);
			}
			if (m_projectionMode != null)
			{
				m_projectionMode.Initialize("ProjectionMode", context);
				context.ExprHostBuilder.Chart3DPropertiesProjectionMode(m_projectionMode);
			}
			if (m_rotation != null)
			{
				m_rotation.Initialize("Rotation", context);
				context.ExprHostBuilder.Chart3DPropertiesRotation(m_rotation);
			}
			if (m_inclination != null)
			{
				m_inclination.Initialize("Inclination", context);
				context.ExprHostBuilder.Chart3DPropertiesInclination(m_inclination);
			}
			if (m_perspective != null)
			{
				m_perspective.Initialize("Perspective", context);
				context.ExprHostBuilder.Chart3DPropertiesPerspective(m_perspective);
			}
			if (m_depthRatio != null)
			{
				m_depthRatio.Initialize("DepthRatio", context);
				context.ExprHostBuilder.Chart3DPropertiesDepthRatio(m_depthRatio);
			}
			if (m_shading != null)
			{
				m_shading.Initialize("Shading", context);
				context.ExprHostBuilder.Chart3DPropertiesShading(m_shading);
			}
			if (m_gapDepth != null)
			{
				m_gapDepth.Initialize("GapDepth", context);
				context.ExprHostBuilder.Chart3DPropertiesGapDepth(m_gapDepth);
			}
			if (m_wallThickness != null)
			{
				m_wallThickness.Initialize("WallThickness", context);
				context.ExprHostBuilder.Chart3DPropertiesWallThickness(m_wallThickness);
			}
			if (m_clustered != null)
			{
				m_clustered.Initialize("Clustered", context);
				context.ExprHostBuilder.Chart3DPropertiesClustered(m_clustered);
			}
			context.ExprHostBuilder.Chart3DPropertiesEnd();
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			ChartThreeDProperties chartThreeDProperties = (ChartThreeDProperties)MemberwiseClone();
			chartThreeDProperties.m_chart = (Chart)context.CurrentDataRegionClone;
			if (m_enabled != null)
			{
				chartThreeDProperties.m_enabled = (ExpressionInfo)m_enabled.PublishClone(context);
			}
			if (m_projectionMode != null)
			{
				chartThreeDProperties.m_projectionMode = (ExpressionInfo)m_projectionMode.PublishClone(context);
			}
			if (m_rotation != null)
			{
				chartThreeDProperties.m_rotation = (ExpressionInfo)m_rotation.PublishClone(context);
			}
			if (m_inclination != null)
			{
				chartThreeDProperties.m_inclination = (ExpressionInfo)m_inclination.PublishClone(context);
			}
			if (m_perspective != null)
			{
				chartThreeDProperties.m_perspective = (ExpressionInfo)m_perspective.PublishClone(context);
			}
			if (m_depthRatio != null)
			{
				chartThreeDProperties.m_depthRatio = (ExpressionInfo)m_depthRatio.PublishClone(context);
			}
			if (m_shading != null)
			{
				chartThreeDProperties.m_shading = (ExpressionInfo)m_shading.PublishClone(context);
			}
			if (m_gapDepth != null)
			{
				chartThreeDProperties.m_gapDepth = (ExpressionInfo)m_gapDepth.PublishClone(context);
			}
			if (m_wallThickness != null)
			{
				chartThreeDProperties.m_wallThickness = (ExpressionInfo)m_wallThickness.PublishClone(context);
			}
			if (m_clustered != null)
			{
				chartThreeDProperties.m_clustered = (ExpressionInfo)m_clustered.PublishClone(context);
			}
			return chartThreeDProperties;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Enabled, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ProjectionMode, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Rotation, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Inclination, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Perspective, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DepthRatio, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Shading, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.GapDepth, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.WallThickness, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Clustered, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Chart, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Chart, Token.Reference));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ThreeDProperties, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Enabled:
					writer.Write(m_enabled);
					break;
				case MemberName.ProjectionMode:
					writer.Write(m_projectionMode);
					break;
				case MemberName.Rotation:
					writer.Write(m_rotation);
					break;
				case MemberName.Inclination:
					writer.Write(m_inclination);
					break;
				case MemberName.Perspective:
					writer.Write(m_perspective);
					break;
				case MemberName.DepthRatio:
					writer.Write(m_depthRatio);
					break;
				case MemberName.Shading:
					writer.Write(m_shading);
					break;
				case MemberName.GapDepth:
					writer.Write(m_gapDepth);
					break;
				case MemberName.WallThickness:
					writer.Write(m_wallThickness);
					break;
				case MemberName.Clustered:
					writer.Write(m_clustered);
					break;
				case MemberName.Chart:
					writer.WriteReference(m_chart);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Enabled:
					m_enabled = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ProjectionMode:
					m_projectionMode = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Rotation:
					m_rotation = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Inclination:
					m_inclination = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Perspective:
					m_perspective = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DepthRatio:
					m_depthRatio = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Shading:
					m_shading = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.GapDepth:
					m_gapDepth = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.WallThickness:
					m_wallThickness = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Clustered:
					m_clustered = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Chart:
					m_chart = reader.ReadReference<Chart>(this);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			if (!memberReferencesCollection.TryGetValue(m_Declaration.ObjectType, out List<MemberReference> value))
			{
				return;
			}
			foreach (MemberReference item in value)
			{
				MemberName memberName = item.MemberName;
				if (memberName == MemberName.Chart)
				{
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					m_chart = (Chart)referenceableItems[item.RefID];
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ThreeDProperties;
		}

		internal void SetExprHost(Chart3DPropertiesExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			m_exprHost = exprHost;
			m_exprHost.SetReportObjectModel(reportObjectModel);
		}

		internal bool EvaluateEnabled(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, instance);
			return context.ReportRuntime.EvaluateChartThreeDPropertiesEnabledExpression(this, m_chart.Name, "Enabled");
		}

		internal ChartThreeDProjectionModes EvaluateProjectionMode(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, instance);
			return EnumTranslator.TranslateChartThreeDProjectionMode(context.ReportRuntime.EvaluateChartThreeDPropertiesProjectionModeExpression(this, m_chart.Name, "ProjectionMode"), context.ReportRuntime);
		}

		internal int EvaluateRotation(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, instance);
			return context.ReportRuntime.EvaluateChartThreeDPropertiesRotationExpression(this, m_chart.Name, "Rotation");
		}

		internal int EvaluateInclination(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, instance);
			return context.ReportRuntime.EvaluateChartThreeDPropertiesInclinationExpression(this, m_chart.Name, "Inclination");
		}

		internal int EvaluatePerspective(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, instance);
			return context.ReportRuntime.EvaluateChartThreeDPropertiesPerspectiveExpression(this, m_chart.Name, "Perspective");
		}

		internal int EvaluateDepthRatio(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, instance);
			return context.ReportRuntime.EvaluateChartThreeDPropertiesDepthRatioExpression(this, m_chart.Name, "DepthRatio");
		}

		internal ChartThreeDShadingTypes EvaluateShading(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, instance);
			return EnumTranslator.TranslateChartThreeDShading(context.ReportRuntime.EvaluateChartThreeDPropertiesShadingExpression(this, m_chart.Name, "Shading"), context.ReportRuntime);
		}

		internal int EvaluateGapDepth(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, instance);
			return context.ReportRuntime.EvaluateChartThreeDPropertiesGapDepthExpression(this, m_chart.Name, "GapDepth");
		}

		internal int EvaluateWallThickness(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, instance);
			return context.ReportRuntime.EvaluateChartThreeDPropertiesWallThicknessExpression(this, m_chart.Name, "WallThickness");
		}

		internal bool EvaluateClustered(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, instance);
			return context.ReportRuntime.EvaluateChartThreeDPropertiesClusteredExpression(this, m_chart.Name, "Clustered");
		}
	}
}
