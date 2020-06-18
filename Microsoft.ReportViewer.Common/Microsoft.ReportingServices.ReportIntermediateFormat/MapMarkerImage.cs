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
	internal sealed class MapMarkerImage : IPersistable
	{
		[NonSerialized]
		private MapMarkerImageExprHost m_exprHost;

		[Reference]
		private Map m_map;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_source;

		private ExpressionInfo m_value;

		private ExpressionInfo m_mIMEType;

		private ExpressionInfo m_transparentColor;

		private ExpressionInfo m_resizeMode;

		internal ExpressionInfo Source
		{
			get
			{
				return m_source;
			}
			set
			{
				m_source = value;
			}
		}

		internal ExpressionInfo Value
		{
			get
			{
				return m_value;
			}
			set
			{
				m_value = value;
			}
		}

		internal ExpressionInfo MIMEType
		{
			get
			{
				return m_mIMEType;
			}
			set
			{
				m_mIMEType = value;
			}
		}

		internal ExpressionInfo TransparentColor
		{
			get
			{
				return m_transparentColor;
			}
			set
			{
				m_transparentColor = value;
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

		internal string OwnerName => m_map.Name;

		internal MapMarkerImageExprHost ExprHost => m_exprHost;

		internal MapMarkerImage()
		{
		}

		internal MapMarkerImage(Map map)
		{
			m_map = map;
		}

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapMarkerImageStart();
			if (m_source != null)
			{
				m_source.Initialize("Source", context);
				context.ExprHostBuilder.MapMarkerImageSource(m_source);
			}
			if (m_value != null)
			{
				m_value.Initialize("Value", context);
				context.ExprHostBuilder.MapMarkerImageValue(m_value);
			}
			if (m_mIMEType != null)
			{
				m_mIMEType.Initialize("MIMEType", context);
				context.ExprHostBuilder.MapMarkerImageMIMEType(m_mIMEType);
			}
			if (m_transparentColor != null)
			{
				m_transparentColor.Initialize("TransparentColor", context);
				context.ExprHostBuilder.MapMarkerImageTransparentColor(m_transparentColor);
			}
			if (m_resizeMode != null)
			{
				m_resizeMode.Initialize("ResizeMode", context);
				context.ExprHostBuilder.MapMarkerImageResizeMode(m_resizeMode);
			}
			context.ExprHostBuilder.MapMarkerImageEnd();
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			MapMarkerImage mapMarkerImage = (MapMarkerImage)MemberwiseClone();
			mapMarkerImage.m_map = context.CurrentMapClone;
			if (m_source != null)
			{
				mapMarkerImage.m_source = (ExpressionInfo)m_source.PublishClone(context);
			}
			if (m_value != null)
			{
				mapMarkerImage.m_value = (ExpressionInfo)m_value.PublishClone(context);
			}
			if (m_mIMEType != null)
			{
				mapMarkerImage.m_mIMEType = (ExpressionInfo)m_mIMEType.PublishClone(context);
			}
			if (m_transparentColor != null)
			{
				mapMarkerImage.m_transparentColor = (ExpressionInfo)m_transparentColor.PublishClone(context);
			}
			if (m_resizeMode != null)
			{
				mapMarkerImage.m_resizeMode = (ExpressionInfo)m_resizeMode.PublishClone(context);
			}
			return mapMarkerImage;
		}

		internal void SetExprHost(MapMarkerImageExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			m_exprHost = exprHost;
			m_exprHost.SetReportObjectModel(reportObjectModel);
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Source, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Value, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MIMEType, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.TransparentColor, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ResizeMode, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Map, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Map, Token.Reference));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapMarkerImage, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Map:
					writer.WriteReference(m_map);
					break;
				case MemberName.Source:
					writer.Write(m_source);
					break;
				case MemberName.Value:
					writer.Write(m_value);
					break;
				case MemberName.MIMEType:
					writer.Write(m_mIMEType);
					break;
				case MemberName.TransparentColor:
					writer.Write(m_transparentColor);
					break;
				case MemberName.ResizeMode:
					writer.Write(m_resizeMode);
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
				case MemberName.Map:
					m_map = reader.ReadReference<Map>(this);
					break;
				case MemberName.Source:
					m_source = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Value:
					m_value = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MIMEType:
					m_mIMEType = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.TransparentColor:
					m_transparentColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ResizeMode:
					m_resizeMode = (ExpressionInfo)reader.ReadRIFObject();
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
				if (memberName == MemberName.Map)
				{
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					m_map = (Map)referenceableItems[item.RefID];
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapMarkerImage;
		}

		internal Microsoft.ReportingServices.OnDemandReportRendering.Image.SourceType EvaluateSource(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return EnumTranslator.TranslateImageSourceType(context.ReportRuntime.EvaluateMapMarkerImageSourceExpression(this, m_map.Name), context.ReportRuntime);
		}

		internal string EvaluateStringValue(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context, out bool errorOccurred)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapMarkerImageStringValueExpression(this, m_map.Name, out errorOccurred);
		}

		internal byte[] EvaluateBinaryValue(IReportScopeInstance romInstance, OnDemandProcessingContext context, out bool errOccurred)
		{
			context.SetupContext(m_map, romInstance);
			return context.ReportRuntime.EvaluateMapMarkerImageBinaryValueExpression(this, m_map.Name, out errOccurred);
		}

		internal string EvaluateMIMEType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapMarkerImageMIMETypeExpression(this, m_map.Name);
		}

		internal string EvaluateTransparentColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapMarkerImageTransparentColorExpression(this, m_map.Name);
		}

		internal MapResizeMode EvaluateResizeMode(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return EnumTranslator.TranslateMapResizeMode(context.ReportRuntime.EvaluateMapMarkerImageResizeModeExpression(this, m_map.Name), context.ReportRuntime);
		}
	}
}
